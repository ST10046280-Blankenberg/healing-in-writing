/* =========================================================
   Auth Toast Handler
   Handles toast notifications for authentication pages
   ========================================================= */

(function() {
    'use strict';

    // Wait for DOM and ToastManager to be ready
    document.addEventListener('DOMContentLoaded', function() {
        // Get TempData values from data attributes on body
        const bodyElement = document.body;
        const tempDataError = bodyElement.getAttribute('data-error-message');
        const tempDataSuccess = bodyElement.getAttribute('data-success-message');

        // Check for TempData error messages (from controller)
        if (tempDataError && tempDataError !== '') {
            ToastManager.error('Error', tempDataError, { duration: 7000, showProgress: true });
        }

        // Check for TempData success messages
        if (tempDataSuccess && tempDataSuccess !== '') {
            ToastManager.success('Success', tempDataSuccess, { duration: 5000, showProgress: true });
        }

        // Check for validation summary errors
        var validationSummaries = document.querySelectorAll('.auth__validation-summary');
        var errorShown = false; // Prevent duplicate toasts

        validationSummaries.forEach(function(summary) {
            if (!errorShown && summary && summary.textContent.trim() !== '') {
                var errorList = summary.querySelector('ul');

                // Only show toast if there are actual error messages
                if (errorList && errorList.querySelector('li')) {
                    var errorMessage = 'Please check the form and try again.';
                    var firstError = errorList.querySelector('li');

                    if (firstError && firstError.textContent.trim()) {
                        errorMessage = firstError.textContent.trim();
                    }

                    // Determine if it's login or register based on visible form
                    var authContainer = document.querySelector('.auth');
                    var isLoginMode = authContainer && authContainer.classList.contains('auth--login');
                    var title = isLoginMode ? 'Sign In Failed' : 'Registration Failed';

                    ToastManager.error(title, errorMessage, { duration: 7000, showProgress: true });
                    errorShown = true; // Mark that we've shown the error
                }
            }
        });

        // Add button loading states
        var loginForm = document.querySelector('.auth__form--login');
        var registerForm = document.querySelector('.auth__form--register');

        if (loginForm) {
            loginForm.addEventListener('submit', function(e) {
                var submitButton = this.querySelector('button[type="submit"]');
                if (submitButton && this.checkValidity()) {
                    submitButton.classList.add('btn-loading');
                    submitButton.disabled = true;
                }
            });
        }

        if (registerForm) {
            registerForm.addEventListener('submit', function(e) {
                var submitButton = this.querySelector('button[type="submit"]');
                if (submitButton && this.checkValidity()) {
                    submitButton.classList.add('btn-loading');
                    submitButton.disabled = true;
                }
            });
        }
    });
})();