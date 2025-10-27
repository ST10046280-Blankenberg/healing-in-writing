document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.manage-books__action-btn--delete').forEach(function (btn) {
        btn.addEventListener('click', function () {
            const bookId = btn.getAttribute('data-book-id');
            if (confirm('Are you sure you want to delete this book?')) {
                fetch(`/Admin/Books/Delete/${bookId}`, {
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
});