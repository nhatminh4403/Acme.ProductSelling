class CultureHelper {
    static getCurrentCulture() {
        // Lấy culture từ URL path
        const path = window.location.pathname;
        const parts = path.split('/');

        if (parts.length >= 2) {
            const potentialCulture = parts[1].toLowerCase(); // Đảm bảo lowercase
            if (['en', 'vi'].includes(potentialCulture)) {
                return potentialCulture;
            }
        }

        // Fallback: lấy từ cookie
        const cookieCulture = CultureHelper.getCookie('culture');
        if (cookieCulture && ['en', 'vi'].includes(cookieCulture.toLowerCase())) {
            return cookieCulture.toLowerCase();
        }

        // Default fallback
        return 'vi';
    }

    static setCultureCookie(culture) {
        const expires = new Date();
        expires.setDate(expires.getDate() + 30); // 30 days
        document.cookie = `culture=${culture.toLowerCase()}; expires=${expires.toUTCString()}; path=/; SameSite=Lax`;
    }

    static getCookie(name) {
        const nameEQ = name + "=";
        const ca = document.cookie.split(';');
        for (let i = 0; i < ca.length; i++) {
            let c = ca[i];
            while (c.charAt(0) === ' ') c = c.substring(1, c.length);
            if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
        }
        return null;
    }

    static updateAllLinks() {
        // Cập nhật tất cả các links để đảm bảo có culture prefix
        const currentCulture = CultureHelper.getCurrentCulture();
        const links = document.querySelectorAll('a[href^="/"]');

        links.forEach(link => {
            const href = link.getAttribute('href');
            if (href && !href.startsWith(`/${currentCulture}/`) &&
                !href.startsWith('/api/') &&
                !href.startsWith('/_framework/') &&
                !href.startsWith('/css/') &&
                !href.startsWith('/js/') &&
                !href.startsWith('/lib/') &&
                !href.startsWith('/admin/') && !href.startsWith('/Abp/') &&
                !href.startsWith('/identity/') &&
                !href.startsWith('/account/') &&
                !href.includes('.')) {

                // Kiểm tra nếu href đã có culture khác
                const pathParts = href.split('/');
                if (pathParts.length >= 2 && ['en', 'vi'].includes(pathParts[1].toLowerCase())) {
                    // Thay thế culture cũ bằng culture mới
                    pathParts[1] = currentCulture;
                    const newHref = pathParts.join('/');
                    link.setAttribute('href', newHref);
                } else {
                    // Thêm culture prefix nếu chưa có
                    const newHref = `/${currentCulture}${href}`;
                    link.setAttribute('href', newHref);
                }
            }
        });
    }

    static initializeCultureHandling() {
        // Set culture cookie khi trang load
        const currentCulture = CultureHelper.getCurrentCulture();
        CultureHelper.setCultureCookie(currentCulture);

        // Cập nhật links khi DOM ready
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', CultureHelper.updateAllLinks);
        } else {
            CultureHelper.updateAllLinks();
        }

        // Theo dõi các thay đổi DOM và cập nhật links mới
        const observer = new MutationObserver((mutations) => {
            let hasNewLinks = false;
            mutations.forEach((mutation) => {
                mutation.addedNodes.forEach((node) => {
                    if (node.nodeType === Node.ELEMENT_NODE) {
                        if (node.tagName === 'A' || node.querySelectorAll('a').length > 0) {
                            hasNewLinks = true;
                        }
                    }
                });
            });

            if (hasNewLinks) {
                setTimeout(CultureHelper.updateAllLinks, 100);
            }
        });

        observer.observe(document.body, {
            childList: true,
            subtree: true
        });
    }
}

// Khởi tạo khi trang load
CultureHelper.initializeCultureHandling();

// Debug information
console.log('=== CULTURE DEBUG ===');
console.log('Current URL:', window.location.href);
console.log('Current pathname:', window.location.pathname);

// Parse current culture from URL
const pathSegments = window.location.pathname.split('/').filter(s => s);
console.log('Path segments:', pathSegments);

if (pathSegments.length > 0) {
    const firstSegment = pathSegments[0].toLowerCase();
    const secondSegment = pathSegments[1] ? pathSegments[1].toLowerCase() : null;

    console.log('First segment:', firstSegment);
    console.log('Second segment:', secondSegment);

    if (['en', 'vi'].includes(firstSegment)) {
        console.log('✅ Valid culture found:', firstSegment);

        if (secondSegment && ['en', 'vi'].includes(secondSegment)) {
            console.log('❌ DUPLICATE CULTURE DETECTED!', firstSegment, secondSegment);
            // Redirect để fix duplicate
            const correctPath = '/' + firstSegment + '/' + pathSegments.slice(2).join('/');
            console.log('Should redirect to:', correctPath);
            // window.location.replace(correctPath); // Uncomment để auto-fix
        }
    } else {
        console.log('❌ No culture prefix found');
    }
}

// Check cookies
const cultureCookie = document.cookie
    .split('; ')
    .find(row => row.startsWith('culture='))
    ?.split('=')[1];

console.log('Culture cookie:', cultureCookie);

// Check all links
const internalLinks = document.querySelectorAll('a[href^="/"]');
console.log('Internal links found:', internalLinks.length);

internalLinks.forEach((link, index) => {
    const href = link.getAttribute('href');
    const linkSegments = href.split('/').filter(s => s);

    if (linkSegments.length > 0 && !['api', '_framework', 'css', 'js', 'lib', 'admin', 'identity', 'account'].includes(linkSegments[0])) {
        const hasValidCulture = ['en', 'vi'].includes(linkSegments[0].toLowerCase());
        console.log(`Link ${index + 1}: ${href} - Culture: ${hasValidCulture ? '✅' : '❌'}`);
    }
});

console.log('=== END DEBUG ===');