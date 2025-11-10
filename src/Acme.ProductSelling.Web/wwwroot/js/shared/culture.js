class CultureHelper {
    static getCurrentCulture() {
        const path = window.location.pathname;
        const parts = path.split('/');

        if (parts.length >= 2) {
            const potentialCulture = parts[1].toLowerCase();
            if (['en', 'vi'].includes(potentialCulture)) {
                return potentialCulture;
            }
        }

        const cookieCulture = CultureHelper.getCookie('culture');
        if (cookieCulture && ['en', 'vi'].includes(cookieCulture.toLowerCase())) {
            return cookieCulture.toLowerCase();
        }

        return 'vi';
    }

    static setCultureCookie(culture) {
        const expires = new Date();
        expires.setDate(expires.getDate() + 30); 

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
        const currentCulture = CultureHelper.getCurrentCulture();
        const links = document.querySelectorAll('a[href^="/"]');

        links.forEach(link => {
            const href = link.getAttribute('href');
            if (!href) return;
            if (link.hasAttribute('data-no-culture')) {
                return; // Skip
            }
            // Extract path without culture prefix for checking
            const pathParts = href.split('/').filter(s => s);
            const pathWithoutCulture = pathParts.length > 0 && ['en', 'vi'].includes(pathParts[0].toLowerCase())
                ? '/' + pathParts.slice(1).join('/')
                : href;

            // Check if it's a path that should be excluded
            if (href.startsWith(`/${currentCulture}/`) ||
                pathWithoutCulture.startsWith('/api/') ||
                pathWithoutCulture.startsWith('/_framework/') ||
                pathWithoutCulture.startsWith('/css/') ||
                pathWithoutCulture.startsWith('/js/') ||
                pathWithoutCulture.startsWith('/lib/') ||
                pathWithoutCulture.toLowerCase().startsWith('/admin/') ||
                pathWithoutCulture.startsWith('/Abp/') ||
                pathWithoutCulture.startsWith('/identity/') ||
                pathWithoutCulture.startsWith('/account/') ||
                pathWithoutCulture.startsWith('/SettingManagement/') ||
                pathWithoutCulture.startsWith('/TenantManagement/') ||
                pathWithoutCulture.startsWith('/PermissionManagement/') ||
                pathWithoutCulture.startsWith('/hangfire/') || 
                pathWithoutCulture.startsWith('/swagger/') ||
                pathWithoutCulture.startsWith('/Account/Logout') ||

                pathWithoutCulture.startsWith('/Identity/') || 
                href.includes('.')) {
                return; // Skip this link
            }

            // Add or update culture prefix
            const segments = href.split('/').filter(s => s);
            if (segments.length >= 1 && ['en', 'vi'].includes(segments[0].toLowerCase())) {
                segments[0] = currentCulture;
                link.setAttribute('href', '/' + segments.join('/'));
            } else {
                link.setAttribute('href', `/${currentCulture}${href}`);
            }
        });
    }
    static syncCultureWithUrl() {
        const urlCulture = CultureHelper.getCultureFromUrl();
        const cookieCulture = CultureHelper.getCookie('culture');

        if (urlCulture && urlCulture !== cookieCulture) {
            CultureHelper.setCultureCookie(urlCulture);

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
        CultureHelper.syncCultureWithUrl();

        const currentCulture = CultureHelper.getCurrentCulture();
        CultureHelper.setCultureCookie(currentCulture);

        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => {
                CultureHelper.updateAllLinks();
                CultureHelper.syncCultureWithUrl();
            });
        } else {
            CultureHelper.updateAllLinks();
            CultureHelper.syncCultureWithUrl();
        }

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

        window.addEventListener('popstate', () => {
            setTimeout(() => {
                CultureHelper.syncCultureWithUrl();
                CultureHelper.updateAllLinks();
            }, 100);
        });
    }
}

CultureHelper.initializeCultureHandling();



const pathSegments = window.location.pathname.split('/').filter(s => s);
//console.log('Path segments:', pathSegments);

const urlCulture = CultureHelper.getCultureFromUrl();
const cookieCulture = CultureHelper.getCookie('culture');
const abpCookieCulture = CultureHelper.getCookie('Abp.Localization.CultureName');

if (urlCulture && cookieCulture && urlCulture !== cookieCulture) {

}

if (pathSegments.length >= 2 &&
    ['en', 'vi'].includes(pathSegments[0].toLowerCase()) &&
    ['en', 'vi'].includes(pathSegments[1].toLowerCase())) {
}


class AspNetCoreCultureHelper {

    static setAspNetCoreCulture(culture) {
        const expires = new Date();
        expires.setDate(expires.getDate() + 30); // 30 days

        const aspNetCoreCultureValue = `c=${culture}|uic=${culture}`;
        document.cookie = `.AspNetCore.Culture=${aspNetCoreCultureValue}; expires=${expires.toUTCString()}; path=/; SameSite=Lax`;

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

            AspNetCoreCultureHelper.setAspNetCoreCulture(newCulture);
        }
    }

    static monitorCookieChanges() {
        let lastAspNetCoreCulture = AspNetCoreCultureHelper.getAspNetCoreCulture();

        setInterval(() => {
            const currentAspNetCoreCulture = AspNetCoreCultureHelper.getAspNetCoreCulture();
            const urlCulture = CultureHelper.getCultureFromUrl();

            if (currentAspNetCoreCulture &&
                currentAspNetCoreCulture !== lastAspNetCoreCulture &&
                currentAspNetCoreCulture !== urlCulture) {

                window.location.reload();
            }

            lastAspNetCoreCulture = currentAspNetCoreCulture;
        }, 1000); 
    }
}

document.addEventListener('DOMContentLoaded', function () {

    AspNetCoreCultureHelper.monitorCookieChanges();


});

window.switchCulture = AspNetCoreCultureHelper.switchCulture;

