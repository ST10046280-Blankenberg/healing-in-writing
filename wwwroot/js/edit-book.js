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