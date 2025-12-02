/* =========================================================
   Toast Notification System
   Modular JavaScript for creating and managing toasts
   ========================================================= */

/**
 * Toast Notification Manager
 * Handles creation, display, and removal of toast notifications
 */
const ToastManager = (function () {
    'use strict';

    // Configuration defaults
    const DEFAULT_CONFIG = {
        position: 'top-right',     // Position of toast container
        duration: 5000,            // Auto-dismiss duration in milliseconds
        showProgress: true,        // Show progress bar
        closeButton: true,         // Show close button
        maxToasts: 5,              // Maximum number of toasts to display
        icons: {
            success: 'fa-circle-check',
            error: 'fa-circle-xmark',
            warning: 'fa-triangle-exclamation',
            info: 'fa-circle-info'
        }
    };

    let config = { ...DEFAULT_CONFIG };
    let container = null;
    let toastQueue = [];

    /**
     * Initialises the toast system with custom configuration
     * @param {Object} userConfig - Custom configuration options
     */
    function init(userConfig = {}) {
        config = { ...DEFAULT_CONFIG, ...userConfig };
        createContainer();
    }

    /**
     * Creates the toast container element if it doesn't exist
     */
    function createContainer() {
        if (container) return;

        container = document.createElement('div');
        container.className = `toast-container toast-container-${config.position}`;
        container.setAttribute('aria-live', 'polite');
        container.setAttribute('aria-atomic', 'false');
        document.body.appendChild(container);
    }

    /**
     * Shows a toast notification
     * @param {Object} options - Toast configuration
     * @param {string} options.type - Toast type (success, error, warning, info)
     * @param {string} options.title - Toast title
     * @param {string} options.message - Toast message (optional)
     * @param {number} options.duration - Custom duration in ms (optional)
     * @param {boolean} options.showProgress - Show progress bar (optional)
     * @param {boolean} options.closeButton - Show close button (optional)
     * @returns {HTMLElement} The created toast element
     */
    function show(options) {
        if (!container) {
            createContainer();
        }

        // Enforce maximum toast limit
        if (toastQueue.length >= config.maxToasts) {
            removeToast(toastQueue[0]);
        }

        const toast = createToast(options);
        toastQueue.push(toast);
        container.appendChild(toast);

        // Trigger entrance animation
        requestAnimationFrame(() => {
            toast.classList.add(getEntranceAnimation());
        });

        // Auto-dismiss if duration is set
        if (options.duration !== 0 && options.duration !== false) {
            const duration = options.duration || config.duration;

            if (options.showProgress !== false && config.showProgress) {
                startProgressBar(toast, duration);
            }

            setTimeout(() => {
                removeToast(toast);
            }, duration);
        }

        return toast;
    }

    /**
     * Creates a toast element
     * @param {Object} options - Toast configuration
     * @returns {HTMLElement} The toast element
     */
    function createToast(options) {
        const toast = document.createElement('div');
        toast.className = `toast toast-${options.type}`;
        toast.setAttribute('role', options.type === 'error' ? 'alert' : 'status');

        // Icon
        const icon = document.createElement('div');
        icon.className = 'toast__icon';
        icon.innerHTML = `<i class="fas ${config.icons[options.type]}"></i>`;

        // Content
        const content = document.createElement('div');
        content.className = 'toast__content';

        const title = document.createElement('div');
        title.className = 'toast__title';
        title.textContent = options.title;
        content.appendChild(title);

        if (options.message) {
            const message = document.createElement('div');
            message.className = 'toast__message';
            message.textContent = options.message;
            content.appendChild(message);
        }

        // Close button
        const closeButton = document.createElement('button');
        closeButton.className = 'toast__close';
        closeButton.setAttribute('aria-label', 'Close notification');
        closeButton.innerHTML = '<i class="fas fa-xmark"></i>';
        closeButton.addEventListener('click', () => removeToast(toast));

        // Assemble toast
        toast.appendChild(icon);
        toast.appendChild(content);

        if (options.closeButton !== false && config.closeButton) {
            toast.appendChild(closeButton);
        }

        // Progress bar
        if (options.showProgress !== false && config.showProgress && options.duration !== 0) {
            const progress = document.createElement('div');
            progress.className = 'toast__progress';
            const progressBar = document.createElement('div');
            progressBar.className = 'toast__progress-bar';
            progress.appendChild(progressBar);
            toast.appendChild(progress);
        }

        return toast;
    }

    /**
     * Starts the progress bar animation
     * @param {HTMLElement} toast - The toast element
     * @param {number} duration - Duration in milliseconds
     */
    function startProgressBar(toast, duration) {
        const progressBar = toast.querySelector('.toast__progress-bar');
        if (progressBar) {
            progressBar.style.animationDuration = `${duration}ms`;
        }
    }

    /**
     * Removes a toast with exit animation
     * @param {HTMLElement} toast - The toast element to remove
     */
    function removeToast(toast) {
        if (!toast || !toast.parentElement) return;

        toast.classList.add('toast-exit');

        toast.addEventListener('animationend', () => {
            if (toast.parentElement) {
                toast.parentElement.removeChild(toast);
            }

            const index = toastQueue.indexOf(toast);
            if (index > -1) {
                toastQueue.splice(index, 1);
            }
        }, { once: true });
    }

    /**
     * Gets the entrance animation class based on position
     * @returns {string} Animation class name
     */
    function getEntranceAnimation() {
        if (config.position.includes('right')) {
            return 'toast-enter-right';
        } else if (config.position.includes('left')) {
            return 'toast-enter-left';
        } else if (config.position.includes('top')) {
            return 'toast-enter-top';
        } else {
            return 'toast-enter-bottom';
        }
    }

    /**
     * Shorthand methods for different toast types
     */
    function success(title, message, options = {}) {
        return show({ type: 'success', title, message, ...options });
    }

    function error(title, message, options = {}) {
        return show({ type: 'error', title, message, ...options });
    }

    function warning(title, message, options = {}) {
        return show({ type: 'warning', title, message, ...options });
    }

    function info(title, message, options = {}) {
        return show({ type: 'info', title, message, ...options });
    }

    /**
     * Removes all toasts
     */
    function clearAll() {
        toastQueue.forEach(toast => removeToast(toast));
        toastQueue = [];
    }

    /**
     * Updates configuration
     * @param {Object} newConfig - New configuration options
     */
    function updateConfig(newConfig) {
        config = { ...config, ...newConfig };

        // Recreate container if position changed
        if (newConfig.position && container) {
            container.className = `toast-container toast-container-${config.position}`;
        }
    }

    // Public API
    return {
        init,
        show,
        success,
        error,
        warning,
        info,
        clearAll,
        updateConfig
    };
})();

// Auto-initialise on DOM ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => {
        ToastManager.init();
    });
} else {
    ToastManager.init();
}

// Export for use in modules or global scope
if (typeof module !== 'undefined' && module.exports) {
    module.exports = ToastManager;
}

if (typeof window !== 'undefined') {
    window.ToastManager = ToastManager;
}