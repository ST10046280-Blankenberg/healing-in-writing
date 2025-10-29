document.addEventListener('DOMContentLoaded', function () {
    // ISBN logic
    const isbnPrimary = document.getElementById('IsbnPrimary');
    const isbnLabel = document.getElementById('IsbnLabel');
    const isbnSecondary = document.getElementById('IsbnSecondary');
    const isbnSecondaryDisplay = document.getElementById('IsbnSecondaryDisplay');
    const isbnSecondaryLabel = document.getElementById('IsbnSecondaryLabel');
    const isbnSecondaryContainer = document.getElementById('IsbnSecondaryContainer');
    const importBtn = document.querySelector('.add-book__import-btn');
    const publishDateInput = document.getElementById('PublishDate');

    function normalizeIsbn(isbn) {
        // Remove all non-digit and non-X/x characters
        return isbn.replace(/[^0-9Xx]/g, '').toUpperCase();
    }

    function updateIsbnLabel() {
        const val = isbnPrimary.value.trim();
        const normalized = normalizeIsbn(val);
        if (normalized.length === 0) {
            isbnLabel.textContent = 'ISBN:';
        } else if (normalized.length === 13) {
            isbnLabel.textContent = 'ISBN: 13';
        } else if (normalized.length === 10) {
            isbnLabel.textContent = 'ISBN: 10';
        } else {
            isbnLabel.textContent = 'ISBN: Invalid';
        }
    }
    isbnPrimary.addEventListener('input', updateIsbnLabel);

    // Validate publish date on blur
    if (publishDateInput) {
        publishDateInput.addEventListener('blur', function () {
            const val = publishDateInput.value.trim();
            // Accept yyyy, yyyy-mm, or yyyy-mm-dd
            const regex = /^\d{4}(-\d{2}){0,2}$/;
            if (val && !regex.test(val)) {
                publishDateInput.setCustomValidity('Date must be in yyyy, yyyy-mm, or yyyy-mm-dd format.');
            } else {
                publishDateInput.setCustomValidity('');
            }
        });
    }

    importBtn.addEventListener('click', async function () {
        const isbn = isbnPrimary.value.trim();
        const messageDiv = document.getElementById('import-message');
        messageDiv.style.display = 'none';
        messageDiv.textContent = '';

        if (!isbn) {
            messageDiv.textContent = 'Please enter an ISBN.';
            messageDiv.style.display = 'block';
            return;
        }

        importBtn.disabled = true;
        importBtn.textContent = 'Importing...';
        try {
            const response = await fetch(`/Admin/Books/ImportBookByIsbn?isbn=${encodeURIComponent(normalized)}`);
            const result = await response.json();
            if (result.success && result.data) {
                // ISBNs
                const isbns = result.data.industryIdentifiers;
                if (isbns.length > 0) {
                    isbnPrimary.value = isbns[0];
                    isbnLabel.textContent = normalizeIsbn(isbns[0]).length === 13 ? 'ISBN: 13' : 'ISBN: 10';
                }
                if (isbns.length > 1) {
                    isbnSecondary.value = isbns[1];
                    isbnSecondaryDisplay.value = isbns[1];
                    isbnSecondaryLabel.textContent = normalizeIsbn(isbns[1]).length === 13 ? 'ISBN: 13' : 'ISBN: 10';
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
                categoryTagManager.setTags(result.data.categories || []);
                document.getElementById('Description').value = result.data.description;
                setCoverImage(result.data.thumbnailUrl, result.data.smallThumbnailUrl || result.data.thumbnailUrl);
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

    // Category tags logic - using TagManager
    const categoryTagManager = new TagManager({
        inputId: 'CategoriesInput',
        tagsDisplayId: 'CategoryTags',
        hiddenInputId: 'Categories',
        tagClass: 'book-card__tag',
        tagTextClass: 'book-card__tag-text',
        removeButtonClass: 'book-card__tag-remove'
    });

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
    function setCoverImage(url, smallUrl) {
        document.getElementById('ThumbnailUrl').value = url || '';
        document.getElementById('SmallThumbnailUrl').value = smallUrl || '';
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