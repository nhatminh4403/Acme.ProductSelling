class CultureHelper {
    static getCurrentCulture() {
        // Lấy culture từ URL path (ưu tiên cao nhất)
        const path = window.location.pathname;
        const parts = path.split('/');

        if (parts.length >= 2) {
            const potentialCulture = parts[1].toLowerCase();
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

        // Set cả 2 cookies để đảm bảo tương thích
        document.cookie = `culture=${culture.toLowerCase()}; expires=${expires.toUTCString()}; path=/; SameSite=Lax`;
        document.cookie = `Abp.Localization.CultureName=${culture.toLowerCase()}; expires=${expires.toUTCString()}; path=/; SameSite=Lax`;

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

        let updatedCount = 0;

        links.forEach(link => {
            const href = link.getAttribute('href');
            if (href && !href.startsWith(`/${currentCulture}/`) &&
                !href.startsWith('/api/') &&
                !href.startsWith('/_framework/') &&
                !href.startsWith('/css/') &&
                !href.startsWith('/js/') &&
                !href.startsWith('/lib/') &&
                !href.startsWith('/admin/') &&
                !href.startsWith('/Abp/') &&
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
                    updatedCount++;
                } else {
                    // Thêm culture prefix nếu chưa có
                    const newHref = `/${currentCulture}${href}`;
                    link.setAttribute('href', newHref);
                    updatedCount++;
                }
            }
        });
/*
        if (updatedCount > 0) {
            console.log(`✅ Updated ${updatedCount} links with culture prefix: ${currentCulture}`);
        }*/
    }

    static syncCultureWithUrl() {
        // Đồng bộ culture cookie với URL
        const urlCulture = CultureHelper.getCultureFromUrl();
        const cookieCulture = CultureHelper.getCookie('culture');

        if (urlCulture && urlCulture !== cookieCulture) {
            CultureHelper.setCultureCookie(urlCulture);

            // Dispatch custom event để notify components khác
            window.dispatchEvent(new CustomEvent('cultureChanged', {
                detail: { culture: urlCulture }
            }));
        }
    }

    static getCultureFromUrl() {
        const path = window.location.pathname;
        const parts = path.split('/');

        if (parts.length >= 2) {
            const potentialCulture = parts[1].toLowerCase();
            if (['en', 'vi'].includes(potentialCulture)) {
                return potentialCulture;
            }
        }
        return null;
    }

    static initializeCultureHandling() {
        // Đồng bộ culture ngay khi trang load
        CultureHelper.syncCultureWithUrl();

        // Set culture cookie khi trang load
        const currentCulture = CultureHelper.getCurrentCulture();
        CultureHelper.setCultureCookie(currentCulture);

        // Cập nhật links khi DOM ready
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => {
                CultureHelper.updateAllLinks();
                CultureHelper.syncCultureWithUrl();
            });
        } else {
            CultureHelper.updateAllLinks();
            CultureHelper.syncCultureWithUrl();
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
                setTimeout(() => {
                    CultureHelper.updateAllLinks();
                    CultureHelper.syncCultureWithUrl();
                }, 100);
            }
        });

        observer.observe(document.body, {
            childList: true,
            subtree: true
        });

        // Listen for popstate events (back/forward navigation)
        window.addEventListener('popstate', () => {
            setTimeout(() => {
                CultureHelper.syncCultureWithUrl();
                CultureHelper.updateAllLinks();
            }, 100);
        });
    }
}

// Khởi tạo khi trang load
CultureHelper.initializeCultureHandling();



// Parse current culture from URL
const pathSegments = window.location.pathname.split('/').filter(s => s);
console.log('Path segments:', pathSegments);

const urlCulture = CultureHelper.getCultureFromUrl();
const cookieCulture = CultureHelper.getCookie('culture');
const abpCookieCulture = CultureHelper.getCookie('Abp.Localization.CultureName');

//console.log('URL Culture:', urlCulture || 'Not found');
//console.log('Cookie Culture:', cookieCulture || 'Not found');
//console.log('ABP Cookie Culture:', abpCookieCulture || 'Not found');

// Check for sync issues
if (urlCulture && cookieCulture && urlCulture !== cookieCulture) {
    //console.log('⚠️ CULTURE SYNC ISSUE DETECTED!');
    //console.log(`URL has: ${urlCulture}, Cookie has: ${cookieCulture}`);
}

// Check for duplicates
if (pathSegments.length >= 2 &&
    ['en', 'vi'].includes(pathSegments[0].toLowerCase()) &&
    ['en', 'vi'].includes(pathSegments[1].toLowerCase())) {
    //console.log('❌ DUPLICATE CULTURE DETECTED!', pathSegments[0], pathSegments[1]);
}


class AspNetCoreCultureHelper {

    static setAspNetCoreCulture(culture) {
        // Set AspNetCore culture cookie với format chuẩn
        const expires = new Date();
        expires.setDate(expires.getDate() + 30); // 30 days

        const aspNetCoreCultureValue = `c=${culture}|uic=${culture}`;
        document.cookie = `.AspNetCore.Culture=${aspNetCoreCultureValue}; expires=${expires.toUTCString()}; path=/; SameSite=Lax`;

        //console.log('✅ Set AspNetCore culture cookie:', culture);

        // Trigger page reload để middleware xử lý redirect
        setTimeout(() => {
            window.location.reload();
        }, 100);
    }

    static getAspNetCoreCulture() {
        const cookieValue = AspNetCoreCultureHelper.getCookie('.AspNetCore.Culture');
        if (cookieValue) {
            try {
                const decodedValue = decodeURIComponent(cookieValue);
                const parts = decodedValue.split('|');

                for (const part of parts) {
                    if (part.startsWith('c=')) {
                        const culture = part.substring(2).toLowerCase();
                        if (['en', 'vi'].includes(culture)) {
                            return culture;
                        }
                    }
                }
            } catch (error) {
                console.error('Error parsing AspNetCore culture cookie:', error);
            }
        }
        return null;
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

    static switchCulture(newCulture) {
        if (['en', 'vi'].includes(newCulture)) {
            //console.log(`🔄 Switching to culture: ${newCulture}`);

            // Set AspNetCore cookie - này sẽ trigger middleware redirect
            AspNetCoreCultureHelper.setAspNetCoreCulture(newCulture);
        }
    }

    static monitorCookieChanges() {
        // Monitor cookie changes và sync URL
        let lastAspNetCoreCulture = AspNetCoreCultureHelper.getAspNetCoreCulture();

        setInterval(() => {
            const currentAspNetCoreCulture = AspNetCoreCultureHelper.getAspNetCoreCulture();
            const urlCulture = CultureHelper.getCultureFromUrl();

            if (currentAspNetCoreCulture &&
                currentAspNetCoreCulture !== lastAspNetCoreCulture &&
                currentAspNetCoreCulture !== urlCulture) {

                //console.log(`🔄 AspNetCore cookie changed: ${lastAspNetCoreCulture} -> ${currentAspNetCoreCulture}`);
                //console.log('🔄 URL needs update, reloading page...');

                // Reload page để middleware xử lý redirect
                window.location.reload();
            }

            lastAspNetCoreCulture = currentAspNetCoreCulture;
        }, 1000); // Check mỗi giây
    }
}

// Initialize khi DOM ready
document.addEventListener('DOMContentLoaded', function () {
    // Tạo culture switcher để test

    // Monitor cookie changes
    AspNetCoreCultureHelper.monitorCookieChanges();

    //// Debug info
    //console.log('=== ASPNETCORE CULTURE DEBUG ===');
    //console.log('AspNetCore Cookie Culture:', AspNetCoreCultureHelper.getAspNetCoreCulture());
    //console.log('URL Culture:', CultureHelper.getCultureFromUrl());
    //console.log('Custom Cookie Culture:', CultureHelper.getCookie('culture'));
    //console.log('================================');
});

// Global function để có thể call từ browser console
window.switchCulture = AspNetCoreCultureHelper.switchCulture;

//console.log('=== END DEBUG ===');