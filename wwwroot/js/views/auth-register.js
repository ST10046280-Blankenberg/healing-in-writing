document.addEventListener('DOMContentLoaded', function () {
    if (typeof lucide !== 'undefined') {
        lucide.createIcons();
    }

    const registerForm = document.querySelector('.auth__form--register');
    if (!registerForm) return;

    const submitButton = registerForm.querySelector('button[type="submit"]');

    registerForm.addEventListener('submit', function () {
        if (registerForm.checkValidity()) {
            submitButton.classList.add('btn-loading');
            submitButton.disabled = true;

            const loadingLabel = document.createElement('span');
            loadingLabel.className = 'btn-loading-label';
            loadingLabel.textContent = 'Creating account, please wait...';
            submitButton.appendChild(loadingLabel);
        }
    });

    const validationSummary = document.querySelector('.auth__validation-summary');
    if (validationSummary && typeof ToastManager !== 'undefined') {
        ToastManager.error(
            'Registration Failed',
            'Please correct the errors below and try again.',
            { duration: 6000 }
        );
    }
});
