document.addEventListener('DOMContentLoaded', function () {
    const booksListContainer = document.getElementById('books-list-container');
    const filterForm = document.getElementById('book-filter-form');
    const pagination = document.getElementById('books-pagination');
    let currentPage = pagination ? parseInt(pagination.dataset.currentPage, 10) : 1;
    const pageSize = pagination ? parseInt(pagination.dataset.pageSize, 10) : 10;
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

        fetch('/Books/ListPaged?' + params.toString())
            .then(response => response.json())
            .then(data => {
                booksListContainer.innerHTML = data.html;
                totalCount = data.totalCount; // update totalCount for pagination
                currentPage = page;
                updatePaginationUI();
            });
    }

    if (pagination) {
        updatePaginationUI();

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
});