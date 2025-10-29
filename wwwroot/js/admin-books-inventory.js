document.addEventListener('DOMContentLoaded', function () {
    // Event delegation for actions in the books list
    const booksListContainer = document.getElementById('books-list-container');

    if (booksListContainer) {
        booksListContainer.addEventListener('click', function (e) {
            // Delete action
            if (e.target.closest('.manage-books__action-btn--delete')) {
                const btn = e.target.closest('.manage-books__action-btn--delete');
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
            }

            // Edit action
            if (e.target.closest('.manage-books__action-btn--edit')) {
                const btn = e.target.closest('.manage-books__action-btn--edit');
                const bookId = btn.getAttribute('data-book-id');
                window.location.href = `/Admin/Books/EditBook/${bookId}`;
            }
        });

        // Event delegation for visibility toggle (change event)
        booksListContainer.addEventListener('change', function (e) {
            if (e.target.classList.contains('manage-books__toggle-checkbox')) {
                const checkbox = e.target;
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
            }
        });
    }

    // AJAX filter for book inventory
    const filterForm = document.getElementById('book-filter-form');
    if (filterForm) {
        filterForm.addEventListener('submit', function (e) {
            e.preventDefault();
            const form = e.target;
            const params = new URLSearchParams(new FormData(form)).toString();
            fetch('/Admin/Books/Filter?' + params)
                .then(response => response.text())
                .then(html => {
                    booksListContainer.innerHTML = html;
                });
        });
    }
});