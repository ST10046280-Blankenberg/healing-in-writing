// Hours Approval Modal Functions

document.addEventListener('DOMContentLoaded', function () {
    console.log('Hours approval modal script loaded');

    // Event delegation for opening modals
    document.body.addEventListener('click', function (event) {
        const openBtn = event.target.closest('.js-open-modal');
        if (openBtn) {
            const id = openBtn.getAttribute('data-modal-id');
            openModal(id);
        }

        const closeBtn = event.target.closest('.js-close-modal');
        if (closeBtn) {
            const id = closeBtn.getAttribute('data-modal-id');
            closeModal(id);
        }

        // Close when clicking outside
        if (event.target.classList.contains('modal')) {
            event.target.style.display = 'none';
            document.body.style.overflow = '';
        }
    });
});

function openModal(id) {
    console.log('Opening modal for id:', id);
    const modal = document.getElementById('modal-' + id);
    if (modal) {
        modal.style.display = 'flex';
        document.body.style.overflow = 'hidden';
    } else {
        console.error('Modal not found for id:', id);
    }
}

function closeModal(id) {
    console.log('Closing modal for id:', id);
    const modal = document.getElementById('modal-' + id);
    if (modal) {
        modal.style.display = 'none';
        document.body.style.overflow = '';
    }
}