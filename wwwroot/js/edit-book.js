document.addEventListener('DOMContentLoaded', function () {
    const publishDateInput = document.getElementById('PublishDate');

    // Validate publish date on blur
    if (publishDateInput) {
        publishDateInput.addEventListener('blur', function () {
            const val = publishDateInput.value.trim();
            const regex = /^\d{4}(-\d{2}){0,2}$/;
            if (val && !regex.test(val)) {
                publishDateInput.setCustomValidity('Date must be in yyyy, yyyy-mm, or yyyy-mm-dd format.');
            } else {
                publishDateInput.setCustomValidity('');
            }
        });
    }

    // Category tags logic - using TagManager
    const categoryTagManager = new TagManager({
        inputId: 'CategoriesInput',
        tagsDisplayId: 'CategoryTags',
        hiddenInputId: 'Categories',
        tagClass: 'book-card__tag',
        tagTextClass: 'book-card__tag-text',
        removeButtonClass: 'book-card__tag-remove'
    });

    // Load existing categories from hidden input
    categoryTagManager.loadFromHiddenInput();

    // Cover image logic
    const coverInput = document.getElementById('coverImage');
    const coverPreview = document.getElementById('coverPreview');
    const coverPlaceholder = document.getElementById('coverPlaceholder');

    if (coverInput) {
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
    }
});