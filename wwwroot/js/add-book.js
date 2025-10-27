document.addEventListener('DOMContentLoaded', function () {
    // ISBN logic
    const isbnPrimary = document.getElementById('IsbnPrimary');
    const isbnLabel = document.getElementById('IsbnLabel');
    const isbnSecondary = document.getElementById('IsbnSecondary');
    const isbnSecondaryDisplay = document.getElementById('IsbnSecondaryDisplay');
    const isbnSecondaryLabel = document.getElementById('IsbnSecondaryLabel');
    const isbnSecondaryContainer = document.getElementById('IsbnSecondaryContainer');
    const importBtn = document.querySelector('.add-book__import-btn');

    function updateIsbnLabel() {
        const val = isbnPrimary.value.trim();
        if (val.length === 0) {
            isbnLabel.textContent = 'ISBN:';
        } else if (val.length === 13) {
            isbnLabel.textContent = 'ISBN: 13';
        } else if (val.length === 10) {
            isbnLabel.textContent = 'ISBN: 10';
        } else {
            isbnLabel.textContent = 'ISBN: Invalid';
        }
    }
    isbnPrimary.addEventListener('input', updateIsbnLabel);

    importBtn.addEventListener('click', async function () {
        const isbn = isbnPrimary.value.trim();
        if (!isbn) {
            alert('Please enter an ISBN.');
            return;
        }
        importBtn.disabled = true;
        importBtn.textContent = 'Importing...';
        try {
            const response = await fetch(`/Admin/Books/ImportBookByIsbn?isbn=${encodeURIComponent(isbn)}`);
            const result = await response.json();
            if (result.success && result.data) {
                // ISBNs
                const isbns = result.data.industryIdentifiers;
                if (isbns.length > 0) {
                    isbnPrimary.value = isbns[0];
                    isbnLabel.textContent = isbns[0].length === 13 ? 'ISBN: 13' : 'ISBN: 10';
                }
                if (isbns.length > 1) {
                    isbnSecondary.value = isbns[1];
                    isbnSecondaryDisplay.value = isbns[1];
                    isbnSecondaryLabel.textContent = isbns[1].length === 13 ? 'ISBN: 13' : 'ISBN: 10';
                    isbnSecondaryContainer.style.display = 'flex';
                } else {
                    isbnSecondary.value = '';
                    isbnSecondaryDisplay.value = '';
                    isbnSecondaryContainer.style.display = 'none';
                }
                document.getElementById('Title').value = result.data.title;
                document.getElementById('Author').value = result.data.authors;
                document.getElementById('Language').value = result.data.language;
                document.getElementById('PublishDate').value = result.data.publishedDate;
                document.getElementById('PageCount').value = result.data.pageCount;
                categoryTagInput.setTags(result.data.categories || []);
                document.getElementById('Description').value = result.data.description;
                setCoverImage(result.data.thumbnailUrl);
            } else {
                alert(result.message || 'Book not found.');
            }
        } catch (err) {
            alert('Error importing book details: ' + err);
            console.error('ImportBookByIsbn error:', err);
        } finally {
            importBtn.disabled = false;
            importBtn.textContent = 'Import Details';
        }
    });

    // Category tags logic
    let categoryTags = [];
    const categoryTagInput = {
        input: document.getElementById('CategoriesInput'),
        tagsDiv: document.getElementById('CategoryTags'),
        hidden: document.getElementById('Categories'),
        setTags: function (tags) {
            categoryTags = tags;
            this.render();
        },
        render: function () {
            this.tagsDiv.innerHTML = '';
            categoryTags.forEach((tag, idx) => {
                const span = document.createElement('span');
                span.className = 'book-card__tag';
                span.innerHTML = `<span class="book-card__tag-text">${tag}</span> <button type="button" style="border:none;background:none;color:#c00;font-size:14px;cursor:pointer;" onclick="removeCategoryTag(${idx})">&times;</button>`;
                this.tagsDiv.appendChild(span);
            });
            this.hidden.value = categoryTags.join(',');
        }
    };
    categoryTagInput.input.addEventListener('keydown', function (e) {
        if (e.key === 'Enter' && this.value.trim()) {
            e.preventDefault();
            categoryTags.push(this.value.trim());
            categoryTagInput.input.value = '';
            categoryTagInput.render();
        }
    });
    window.removeCategoryTag = function (idx) {
        categoryTags.splice(idx, 1);
        categoryTagInput.render();
    };

    // Cover image logic
    const coverInput = document.getElementById('coverImage');
    const coverPreview = document.getElementById('coverPreview');
    const coverPlaceholder = document.getElementById('coverPlaceholder');

    coverInput.addEventListener('change', function (e) {
        const file = e.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function (ev) {
                coverPreview.src = ev.target.result;
                coverPreview.style.display = 'block';
                coverPlaceholder.style.display = 'none';
            };
            reader.readAsDataURL(file);
        } else {
            coverPreview.src = '';
            coverPreview.style.display = 'none';
            coverPlaceholder.style.display = 'block';
        }
    });

    // When importing from API, also hide the placeholder
    function setCoverImage(url) {
        if (url) {
            coverPreview.src = url;
            coverPreview.style.display = 'block';
            coverPlaceholder.style.display = 'none';
        } else {
            coverPreview.src = '';
            coverPreview.style.display = 'none';
            coverPlaceholder.style.display = 'block';
        }
    }
});