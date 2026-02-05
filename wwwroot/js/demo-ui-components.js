document.addEventListener('DOMContentLoaded', function () {
    if (typeof lucide !== 'undefined') {
        lucide.createIcons();
    }

    if (typeof ToastManager !== 'undefined') {
        ToastManager.init({ position: 'top-right' });
    }

    const toastPositionSelect = document.querySelector('[data-toast-position]');
    if (toastPositionSelect) {
        toastPositionSelect.addEventListener('change', function () {
            if (typeof ToastManager === 'undefined') return;
            const position = toastPositionSelect.value;
            ToastManager.updateConfig({ position });
            ToastManager.info('Position Changed', `Toasts will now appear at: ${position}`);
        });
    }

    document.querySelectorAll('[data-toast]').forEach(function (button) {
        button.addEventListener('click', function () {
            if (typeof ToastManager === 'undefined') return;
            const action = button.getAttribute('data-toast');

            switch (action) {
                case 'success':
                    ToastManager.success('Success!', 'Operation completed successfully.');
                    break;
                case 'error':
                    ToastManager.error('Error Occurred', 'Unable to complete the operation. Please try again.');
                    break;
                case 'warning':
                    ToastManager.warning('Warning', 'Your session will expire in 5 minutes. Please save your work.');
                    break;
                case 'info':
                    ToastManager.info('Did You Know?', 'You can use keyboard shortcuts to navigate faster.');
                    break;
                case 'persistent':
                    ToastManager.error('Critical Error', 'This toast will not auto-dismiss. Click X to close.', {
                        duration: 0,
                        closeButton: true
                    });
                    break;
                case 'short':
                    ToastManager.success('Quick Save', 'Changes saved!', { duration: 2000 });
                    break;
                case 'no-progress':
                    ToastManager.info('Tip', 'Press Ctrl+S to quick-save.', { showProgress: false });
                    break;
                case 'multiple':
                    ToastManager.success('First', 'This is the first toast');
                    setTimeout(() => ToastManager.info('Second', 'This is the second toast'), 500);
                    setTimeout(() => ToastManager.warning('Third', 'This is the third toast'), 1000);
                    break;
                case 'clear':
                    ToastManager.clearAll();
                    break;
                default:
                    break;
            }
        });
    });

    document.querySelectorAll('[data-loading]').forEach(function (button) {
        button.addEventListener('click', function () {
            const mode = button.getAttribute('data-loading');
            const duration = parseInt(button.getAttribute('data-duration') || '3000', 10);

            if (mode === 'dots') {
                demoDotsLoading(button, duration);
                return;
            }

            demoButtonLoading(button, duration);
        });
    });

    const demoForm = document.getElementById('demoForm');
    if (demoForm) {
        demoForm.addEventListener('submit', function (event) {
            event.preventDefault();
            const button = demoForm.querySelector('button[type="submit"]');

            button.classList.add('btn-loading');
            button.disabled = true;

            setTimeout(() => {
                button.classList.remove('btn-loading');
                button.disabled = false;
                if (typeof ToastManager !== 'undefined') {
                    ToastManager.success('Form Submitted', 'Your form has been processed successfully.');
                }
                demoForm.reset();
            }, 2500);
        });
    }

    const flowButton = document.querySelector('[data-demo="simulate-flow"]');
    if (flowButton) {
        flowButton.addEventListener('click', function () {
            simulateFormFlow(flowButton);
        });
    }
});

function demoButtonLoading(button, duration) {
    button.classList.add('btn-loading');
    button.disabled = true;

    setTimeout(() => {
        button.classList.remove('btn-loading');
        button.disabled = false;
    }, duration);
}

function demoDotsLoading(button, duration) {
    const originalText = button.textContent;
    button.classList.add('btn-loading-dots');
    button.disabled = true;

    setTimeout(() => {
        button.classList.remove('btn-loading-dots');
        button.disabled = false;
        button.textContent = originalText;
    }, duration);
}

function simulateFormFlow(button) {
    button.classList.add('btn-loading');
    button.disabled = true;

    setTimeout(() => {
        button.classList.remove('btn-loading');
        button.disabled = false;

        if (typeof ToastManager !== 'undefined') {
            ToastManager.success(
                'Process Complete',
                'Your request has been processed successfully.',
                { duration: 5000 }
            );
        }
    }, 2500);
}
