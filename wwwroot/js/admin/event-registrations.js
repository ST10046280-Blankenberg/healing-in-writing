/**
 * Event Registrations Page JavaScript
 * Handles confirmation dialogs for removing registrations
 * CSP-compliant - no inline event handlers
 */

(function () {
    'use strict';

    /**
     * Initialise confirmation dialogs for registration removal forms
     */
    function initialiseConfirmationDialogs() {
        const removeForms = document.querySelectorAll('.registration-item__remove-form');

        removeForms.forEach(function (form) {
            form.addEventListener('submit', function (event) {
                const confirmMessage = form.getAttribute('data-confirm-message') ||
                    'Are you sure you want to remove this registration?';

                if (!confirm(confirmMessage)) {
                    event.preventDefault();
                }
            });
        });
    }

    /**
     * Initialise the page when DOM is ready
     */
    function initialise() {
        initialiseConfirmationDialogs();
    }

    // Execute when DOM is fully loaded
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initialise);
    } else {
        initialise();
    }
})();
