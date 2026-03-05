(function (global) {
    'use strict';

    const DEFAULT_RESOURCE = 'ProductSelling';

    const FALLBACK_TRANSLATIONS = {
        'Login:Processing': 'Đang xử lý đăng nhập...',
        'Login:Initiated': 'Bắt đầu đăng nhập',
        'Login:Success': 'Đăng nhập thành công!',
        'Login:Error': 'Đăng nhập thất bại. Vui lòng kiểm tra thông tin và thử lại.',
        'Login:ErrorTitle': 'Lỗi đăng nhập',
        'Login:Default': 'Đăng nhập',
        'Register:Processing': 'Đang xử lý đăng ký...',
        'Register:Initiated': 'Bắt đầu đăng ký',
        'Register:SuccessAndLoggingIn': 'Đăng ký thành công! Đang đăng nhập...',
        'Register:ErrorDefault': 'Đăng ký thất bại. Vui lòng kiểm tra thông tin và thử lại.',
        'Register:Button': 'Đăng ký',
        'Register:PasswordsDoNotMatch': 'Mật khẩu không khớp.',
        'Validation:ErrorTitle': 'Lỗi xác thực',
        'Logout:Processing': 'Đang đăng xuất...',
        'Logout:PleaseWait': 'Vui lòng đợi',
        'Logout:Success': 'Bạn đã đăng xuất thành công!',
        'Logout:Complete': 'Đăng xuất hoàn tất',
        'AutoLogin:Error': 'Tự động đăng nhập thất bại. Vui lòng đăng nhập thủ công.',
        'AutoLogin:WarnTitle': 'Cảnh báo tự động đăng nhập',
    };

    /**
     * Localize a key using the ABP localization service, falling back to the
     * built-in Vietnamese map, and finally returning the raw key.
     *
     * @param {string} key
     * @param {string} [resourceName]
     * @returns {string}
     */
    function L(key, resourceName) {
        resourceName = resourceName || DEFAULT_RESOURCE;

        if (typeof abp !== 'undefined' && abp.localization && abp.localization.localize) {
            try {
                const localized = abp.localization.localize(key, resourceName);
                if (localized && localized !== key) return localized;
            } catch (e) {
                console.warn('[L] Localization error for key:', key, e);
            }
        }

        return FALLBACK_TRANSLATIONS[key] || key;
    }

    global.L = L;

}(window));