// Hours Approval Modal Functions

document.addEventListener('DOMContentLoaded', function () {

    // Event delegation for opening modals (Volunteer Details)
    document.body.addEventListener('click', function (e) {
        const openBtn = e.target.closest('.js-open-modal');
        if (openBtn) {
            const modalId = openBtn.getAttribute('data-modal-id');
            openModal(modalId);
        }
    });

    // Event delegation for closing modals (Volunteer Details)
    document.body.addEventListener('click', function (e) {
        const closeBtn = e.target.closest('.js-close-modal');
        if (closeBtn) {
            const modalId = closeBtn.getAttribute('data-modal-id');
            closeModal(modalId);
        }
    });

    // --- Delete Modal Logic ---

    // Open Delete Modal
    document.body.addEventListener('click', function (e) {
        const deleteBtn = e.target.closest('.js-open-delete-modal');
        if (deleteBtn) {
            const id = deleteBtn.getAttribute('data-delete-id');
            const deleteInput = document.getElementById('delete-id-input');
            const deleteModal = document.getElementById('delete-modal');

            if (deleteInput && deleteModal) {
                deleteInput.value = id;
                deleteModal.style.display = 'flex';
            }
        }
    });

    // Close Delete Modal
    document.body.addEventListener('click', function (e) {
        if (e.target.closest('.js-close-delete-modal') || e.target.id === 'delete-modal') {
            const deleteModal = document.getElementById('delete-modal');
            if (deleteModal) {
                deleteModal.style.display = 'none';
            }
        }
    });

    // Close when clicking outside of any modal (general modal class)
    document.body.addEventListener('click', function (event) {
        if (event.target.classList.contains('modal')) {
            event.target.style.display = 'none';
            document.body.style.overflow = '';
        }
    });
});

// Global functions for direct calls if needed
window.openModal = function (id) {
    const modal = document.getElementById('modal-' + id);
    if (modal) {
        modal.style.display = 'flex';
        document.body.style.overflow = 'hidden';
    } else {
        console.error('Modal not found for ID:', id);
    }
};

window.closeModal = function (id) {
    const modal = document.getElementById('modal-' + id);
    if (modal) {
        modal.style.display = 'none';
        document.body.style.overflow = '';
    } else {
        console.error('Modal not found for ID:', id);
    }
};