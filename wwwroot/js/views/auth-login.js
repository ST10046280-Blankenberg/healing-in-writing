document.addEventListener('DOMContentLoaded', function () {
    if (typeof lucide !== 'undefined') {
        lucide.createIcons();
    }

    const loginForm = document.querySelector('.auth__form--login');
    if (!loginForm) return;

    const submitButton = loginForm.querySelector('button[type="submit"]');

    loginForm.addEventListener('submit', function () {
        if (loginForm.checkValidity()) {
            submitButton.classList.add('btn-loading');
            submitButton.disabled = true;

            const loadingLabel = document.createElement('span');
            loadingLabel.className = 'btn-loading-label';
            loadingLabel.textContent = 'Signing in, please wait...';
            submitButton.appendChild(loadingLabel);
        }
    });

    const validationSummary = document.querySelector('.auth__validation-summary');
    if (validationSummary && typeof ToastManager !== 'undefined') {
        ToastManager.error(
            'Sign In Failed',
            'Please correct the errors below and try again.',
            { duration: 6000 }
        );
    }
});
