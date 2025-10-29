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

    //// Pagination logic
    //const pagination = document.getElementById('books-pagination');
    //let currentPage = 1;
    //const pageSize = 20; // Should match the server-side default

    //function loadPage(page) {
    //    const form = document.getElementById('book-filter-form');
    //    const params = new URLSearchParams(new FormData(form));
    //    params.set('skip', (page - 1) * pageSize);
    //    params.set('take', pageSize);

    //    fetch('/Admin/Books/ListPaged?' + params.toString())
    //        .then(response => response.text())
    //        .then(html => {
    //            booksListContainer.innerHTML = html;
    //            currentPage = page;
    //            document.getElementById('pagination-current').textContent = currentPage;
    //            // Optionally, disable/enable prev/next based on data
    //            pagination.querySelector('[data-page="prev"]').disabled = currentPage === 1;
    //            // For demo, always enable next; in production, check if there are more results
    //        });
    //}

    //if (pagination) {
    //    pagination.addEventListener('click', function (e) {
    //        if (e.target.classList.contains('pagination__btn')) {
    //            if (e.target.dataset.page === 'prev' && currentPage > 1) {
    //                loadPage(currentPage - 1);
    //            }
    //            if (e.target.dataset.page === 'next') {
    //                loadPage(currentPage + 1);
    //            }
    //        }
    //    });
    //}

    //// When filter form is submitted, reset to page 1
    //const filterForm = document.getElementById('book-filter-form');
    //if (filterForm) {
    //    filterForm.addEventListener('submit', function (e) {
    //        e.preventDefault();
    //        loadPage(1);
    //    });
    //}
});