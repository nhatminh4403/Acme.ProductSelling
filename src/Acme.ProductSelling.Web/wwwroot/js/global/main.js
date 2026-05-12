const loginModalEl = document.getElementById('loginModal');
const registerModalEl = document.getElementById('registerModal');

const loginModal = bootstrap.Modal.getOrCreateInstance(loginModalEl);
const registerModal = bootstrap.Modal.getOrCreateInstance(registerModalEl);

document.getElementById('goToRegister').addEventListener('click', () => {
    loginModalEl.addEventListener('hidden.bs.modal', () => {
        registerModal.show();
    }, { once: true });          // ← auto-removes itself, no stacking
    loginModal.hide();
});

document.getElementById('goToLogin').addEventListener('click', () => {
    registerModalEl.addEventListener('hidden.bs.modal', () => {
        loginModal.show();
    }, { once: true });
    registerModal.hide();
});