document.addEventListener('DOMContentLoaded', function () {
    const booksListContainer = document.getElementById('books-list-container');
    const filterForm = document.getElementById('book-filter-form');
    const pagination = document.getElementById('books-pagination');
    let currentPage = 1;
    const pageSize = pagination ? parseInt(pagination.dataset.pageSize, 10) : 20;
    let totalCount = pagination ? parseInt(pagination.dataset.totalCount, 10) : 0;

    function updatePaginationUI() {
        const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));
        document.getElementById('pagination-current').textContent = currentPage;
        document.getElementById('pagination-total').textContent = totalPages;
        pagination.querySelector('[data-page="prev"]').disabled = currentPage === 1;
        pagination.querySelector('[data-page="next"]').disabled = currentPage >= totalPages;
    }

    function loadPage(page) {
        const params = new URLSearchParams(new FormData(filterForm));
        params.set('skip', (page - 1) * pageSize);
        params.set('take', pageSize);

        fetch('/Admin/Books/ListPaged?' + params.toString())
            .then(response => response.json())
            .then(data => {
                booksListContainer.innerHTML = data.html;
                currentPage = page;
                totalCount = data.totalCount;
                updatePaginationUI();
            });
    }

    if (pagination) {
        updatePaginationUI(); // Initialize on page load

        pagination.addEventListener('click', function (e) {
            if (e.target.classList.contains('pagination__btn')) {
                if (e.target.dataset.page === 'prev' && currentPage > 1) {
                    loadPage(currentPage - 1);
                }
                if (e.target.dataset.page === 'next') {
                    loadPage(currentPage + 1);
                }
            }
        });
    }

    if (filterForm) {
        filterForm.addEventListener('submit', function (e) {
            e.preventDefault();
            loadPage(1);
        });
    }

    // Event delegation for actions in the books list
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
                                // Instead of removing the row, reload the current page
                                loadPage(currentPage);
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