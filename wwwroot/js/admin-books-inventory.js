document.addEventListener('DOMContentLoaded', function () {

    document.querySelectorAll('.manage-books__action-btn--delete').forEach(function (btn) {
        btn.addEventListener('click', function () {
            const bookId = btn.getAttribute('data-book-id');
            if (confirm('Are you sure you want to delete this book?')) {
                fetch(`/Admin/Books/DeleteBook/${bookId}`, {
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                    }
                })
                    .then(res => {
                        if (res.ok) {
                            btn.closest('tr').remove();
                        } else {
                            alert('Failed to delete book.');
                        }
                    });
            }
        });
    });

    document.querySelectorAll('.manage-books__action-btn--edit').forEach(function (btn) {
        btn.addEventListener('click', function () {
            const bookId = btn.getAttribute('data-book-id');
            window.location.href = `/Admin/Books/EditBook/${bookId}`;
        });
    });

    document.querySelectorAll('.manage-books__toggle-checkbox').forEach(function (checkbox) {
        checkbox.addEventListener('change', function () {
            const bookId = checkbox.getAttribute('data-book-id');
            const isVisible = checkbox.checked;

            fetch(`/Admin/Books/SetVisibility`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                },
                body: JSON.stringify({
                    bookId: parseInt(bookId, 10),
                    isVisible: isVisible
                })
            })
                .then(res => {
                    if (!res.ok) {
                        alert('Failed to update visibility.');
                        checkbox.checked = !isVisible;
                    }
                });
        });
    });
});