/**
 * Site Settings - Gallery Image Upload
 * Handles image upload, collection selection, and form submission
 * CSP-compliant: No inline JavaScript
 */

document.addEventListener("DOMContentLoaded", function () {
    // Get all form elements
    const isAlbumCheckbox = document.getElementById('isAlbumCheckbox');
    const albumFields = document.getElementById('albumFields');
    const collectionSelect = document.getElementById('collectionSelect');
    const customCollectionField = document.getElementById('customCollectionField');
    const customCollectionInput = document.getElementById('customCollectionInput');
    const galleryForm = document.getElementById('galleryForm');
    const imageUpload = document.getElementById('imageUpload');
    const uploadText = document.getElementById('uploadText');
    const uploadArea = document.getElementById('uploadArea');

    // Handle click on upload area to trigger file input
    if (uploadArea && imageUpload) {
        uploadArea.addEventListener('click', function () {
            imageUpload.click();
        });
    }

    // Toggle album fields when "Is Album" checkbox changes
    if (isAlbumCheckbox && albumFields) {
        isAlbumCheckbox.addEventListener('change', function () {
            albumFields.style.display = this.checked ? 'block' : 'none';
        });
    }

    // Handle collection dropdown change
    if (collectionSelect && customCollectionField) {
        collectionSelect.addEventListener('change', function () {
            if (this.value === '__custom__') {
                customCollectionField.style.display = 'block';
                if (customCollectionInput) {
                    customCollectionInput.required = true;
                }
            } else {
                customCollectionField.style.display = 'none';
                if (customCollectionInput) {
                    customCollectionInput.required = false;
                    customCollectionInput.value = '';
                }
            }
        });
    }

    // Update upload text on file selection
    if (imageUpload && uploadText) {
        imageUpload.addEventListener('change', function () {
            const files = this.files;
            if (files && files.length > 0) {
                if (files.length === 1) {
                    uploadText.textContent = 'Selected: ' + files[0].name;
                } else {
                    uploadText.textContent = 'Selected: ' + files.length + ' files';
                }
                uploadText.style.color = '#10b981';
            }
        });
    }

    // Handle form submission
    if (galleryForm) {
        galleryForm.addEventListener('submit', function (e) {
            // If custom collection is selected, use custom input value
            if (collectionSelect && collectionSelect.value === '__custom__' && customCollectionInput) {
                const customValue = customCollectionInput.value.trim();
                if (!customValue) {
                    e.preventDefault();
                    alert('Please enter a collection name.');
                    return false;
                }
                // Create hidden input with custom collection name
                const hiddenInput = document.createElement('input');
                hiddenInput.type = 'hidden';
                hiddenInput.name = 'collectionId';
                hiddenInput.value = customValue;
                galleryForm.appendChild(hiddenInput);

                // Disable the select so it doesn't send __custom__
                collectionSelect.disabled = true;
            }

            // Validate album requirements
            if (isAlbumCheckbox && isAlbumCheckbox.checked) {
                const collectionValue = collectionSelect ? collectionSelect.value : '';
                const customValue = customCollectionInput ? customCollectionInput.value.trim() : '';

                if (!collectionValue && !customValue) {
                    e.preventDefault();
                    alert('Please select or create a collection for album photos.');
                    return false;
                }
            }
        });
    }
});

