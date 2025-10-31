/**
 * Site Settings - Gallery Image Upload
 * Handles image upload, drag-and-drop, preview, and form submission
 * CSP-compliant: No inline JavaScript
 */

document.addEventListener("DOMContentLoaded", () => {
    const uploadArea = document.querySelector(".gallery-page__upload-area");
    const uploadText = document.querySelector(".gallery-page__upload-text");
    const titleInput = document.querySelector(".gallery-page__input[placeholder='Enter photo title']");
    const descriptionTextarea = document.querySelector(".gallery-page__textarea");
    const collectionSelect = document.querySelector(".gallery-page__select");
    const newCollectionInput = document.querySelector(".gallery-page__input[placeholder='Enter new collection name']");
    const addPhotoBtn = document.querySelector(".gallery-page__btn-add");
    const cancelBtn = document.querySelector(".gallery-page__btn-cancel");
    const galleryForm = document.querySelector(".gallery-page__form");
    
    let selectedFile = null;
    let fileInput = null;

    // Create hidden file input
    function createFileInput() {
        if (fileInput) {
            fileInput.remove();
        }
        
        fileInput = document.createElement("input");
        fileInput.type = "file";
        fileInput.accept = "image/*";
        fileInput.style.display = "none";
        
        fileInput.addEventListener("change", handleFileSelect);
        document.body.appendChild(fileInput);
    }

    createFileInput();

    // Handle file selection
    function handleFileSelect(e) {
        const file = e.target.files[0];
        if (file && file.type.startsWith("image/")) {
            selectedFile = file;
            displayFilePreview(file);
        } else {
            alert("Please select a valid image file.");
        }
    }

    // Display file preview
    function displayFilePreview(file) {
        const reader = new FileReader();
        
        reader.onload = (e) => {
            // Update upload area with preview
            uploadArea.style.backgroundImage = `url(${e.target.result})`;
            uploadArea.style.backgroundSize = "cover";
            uploadArea.style.backgroundPosition = "center";
            uploadText.textContent = `Selected: ${file.name}`;
            uploadText.style.backgroundColor = "rgba(255, 255, 255, 0.9)";
            uploadText.style.padding = "4px 8px";
            uploadText.style.borderRadius = "4px";
        };
        
        reader.readAsDataURL(file);
    }

    // Click to upload
    if (uploadArea) {
        uploadArea.addEventListener("click", () => {
            fileInput.click();
        });
    }

    // Drag and drop functionality
    if (uploadArea) {
        uploadArea.addEventListener("dragover", (e) => {
            e.preventDefault();
            uploadArea.style.borderColor = "#4f46e5";
            uploadArea.style.backgroundColor = "#f3f4f6";
        });

        uploadArea.addEventListener("dragleave", () => {
            uploadArea.style.borderColor = "#d1d5db";
            uploadArea.style.backgroundColor = "transparent";
        });

        uploadArea.addEventListener("drop", (e) => {
            e.preventDefault();
            uploadArea.style.borderColor = "#d1d5db";
            uploadArea.style.backgroundColor = "transparent";
            
            const file = e.dataTransfer.files[0];
            if (file && file.type.startsWith("image/")) {
                selectedFile = file;
                displayFilePreview(file);
            } else {
                alert("Please drop a valid image file.");
            }
        });
    }

    // Add Photo button handler
    if (addPhotoBtn) {
        addPhotoBtn.addEventListener("click", async () => {
            // Validate form
            if (!selectedFile) {
                alert("Please select an image to upload.");
                return;
            }

            const title = titleInput.value.trim();
            if (!title) {
                alert("Please enter a title for the photo.");
                titleInput.focus();
                return;
            }

            const description = descriptionTextarea.value.trim();
            const collection = collectionSelect.value;
            const newCollection = newCollectionInput.value.trim();

            // Create form data
            const formData = new FormData();
            formData.append("image", selectedFile);
            formData.append("altText", title);
            formData.append("description", description);
            formData.append("collection", newCollection || collection);
            formData.append("isAlbum", false);

            // Disable button during upload
            addPhotoBtn.disabled = true;
            addPhotoBtn.textContent = "Uploading...";

            try {
                // Send to server
                const response = await fetch("/Admin/SiteSettings/AddGalleryItem", {
                    method: "POST",
                    body: formData,
                    headers: {
                        "RequestVerificationToken": document.querySelector('input[name="__RequestVerificationToken"]').value
                    }
                });

                if (response.ok) {
                    alert("Photo uploaded successfully!");
                    resetForm();
                } else {
                    const error = await response.text();
                    alert(`Upload failed: ${error}`);
                }
            } catch (error) {
                console.error("Upload error:", error);
                alert("An error occurred while uploading. Please try again.");
            } finally {
                addPhotoBtn.disabled = false;
                addPhotoBtn.textContent = "Add Photo";
            }
        });
    }

    // Cancel button handler
    if (cancelBtn) {
        cancelBtn.addEventListener("click", () => {
            if (confirm("Are you sure you want to cancel? All unsaved changes will be lost.")) {
                resetForm();
            }
        });
    }

    // Reset form
    function resetForm() {
        selectedFile = null;
        titleInput.value = "";
        descriptionTextarea.value = "";
        collectionSelect.selectedIndex = 0;
        newCollectionInput.value = "";
        uploadArea.style.backgroundImage = "";
        uploadText.textContent = "Click to upload or drag and drop";
        uploadText.style.backgroundColor = "transparent";
        uploadText.style.padding = "0";
        createFileInput();
    }
});

