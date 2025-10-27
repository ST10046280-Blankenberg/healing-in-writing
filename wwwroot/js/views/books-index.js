document.getElementById('book-filter-form').addEventListener('submit', function (e) {
    e.preventDefault();
    const form = e.target;
    const params = new URLSearchParams(new FormData(form)).toString();
    fetch('/Books/Filter?' + params)
        .then(response => response.text())
        .then(html => {
            document.getElementById('books-list-container').innerHTML = html;
        });
});