// Modal functionality for guest registration and other modals

document.addEventListener('DOMContentLoaded', function() {
    // Get modal elements
    const modals = document.querySelectorAll('.modal');

    modals.forEach(modal => {
        const modalId = modal.id;
        const closeButtons = modal.querySelectorAll('[data-modal-close]');

        // Close modal when clicking close button
        closeButtons.forEach(button => {
            button.addEventListener('click', function() {
                closeModal(modalId);
            });
        });

        // Close modal when clicking backdrop
        modal.addEventListener('click', function(e) {
            if (e.target === modal) {
                closeModal(modalId);
            }
        });

        // Close modal on Escape key
        document.addEventListener('keydown', function(e) {
            if (e.key === 'Escape' && modal.classList.contains('modal--active')) {
                closeModal(modalId);
            }
        });
    });

    // Open modal triggers
    document.querySelectorAll('[data-modal-open]').forEach(trigger => {
        trigger.addEventListener('click', function(e) {
            e.preventDefault();
            const modalId = this.getAttribute('data-modal-open');
            openModal(modalId);
        });
    });
});

function openModal(modalId) {
    const modal = document.getElementById(modalId);
    if (modal) {
        modal.classList.add('modal--active');
        document.body.style.overflow = 'hidden';
    }
}

function closeModal(modalId) {
    const modal = document.getElementById(modalId);
    if (modal) {
        modal.classList.remove('modal--active');
        document.body.style.overflow = '';
    }
}
