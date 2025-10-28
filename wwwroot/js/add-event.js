// add-event.js - Event Details Form Handler
// Handles rich text editor, image upload preview, and form validation

document.addEventListener('DOMContentLoaded', function() {
    initializeRichTextEditor();
    initializeImageUpload();
    initializeFormValidation();
});

/**
 * Initialize Rich Text Editor toolbar functionality
 */
function initializeRichTextEditor() {
    const toolbar = document.querySelector('.event-details__editor-toolbar');
    const textarea = document.querySelector('.event-details__editor-textarea');

    if (!toolbar || !textarea) return;

    // Get all toolbar buttons
    const boldBtn = toolbar.querySelector('[title="Bold"]');
    const italicBtn = toolbar.querySelector('[title="Italic"]');
    const listBtn = toolbar.querySelector('[title="List"]');

    // Bold formatting
    if (boldBtn) {
        boldBtn.addEventListener('click', function() {
            wrapSelectedText(textarea, '**', '**');
        });
    }

    // Italic formatting
    if (italicBtn) {
        italicBtn.addEventListener('click', function() {
            wrapSelectedText(textarea, '_', '_');
        });
    }

    // List formatting
    if (listBtn) {
        listBtn.addEventListener('click', function() {
            insertListItem(textarea);
        });
    }
}

/**
 * Wrap selected text with prefix and suffix (for bold, italic, etc.)
 */
function wrapSelectedText(textarea, prefix, suffix) {
    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    const selectedText = textarea.value.substring(start, end);
    const beforeText = textarea.value.substring(0, start);
    const afterText = textarea.value.substring(end);

    if (selectedText) {
        textarea.value = beforeText + prefix + selectedText + suffix + afterText;
        textarea.focus();
        textarea.setSelectionRange(start + prefix.length, end + prefix.length);
    }
}

/**
 * Insert a list item at cursor position
 */
function insertListItem(textarea) {
    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    const beforeText = textarea.value.substring(0, start);
    const afterText = textarea.value.substring(end);

    // Check if we're at the start of a line
    const lastNewLine = beforeText.lastIndexOf('\n');
    const currentLineStart = lastNewLine >= 0 ? lastNewLine + 1 : 0;
    const currentLine = beforeText.substring(currentLineStart);

    if (currentLine.trim() === '') {
        // Insert bullet point at current position
        textarea.value = beforeText + '• ' + afterText;
        textarea.focus();
        textarea.setSelectionRange(start + 2, start + 2);
    } else {
        // Insert bullet point on new line
        textarea.value = beforeText + '\n• ' + afterText;
        textarea.focus();
        textarea.setSelectionRange(start + 3, start + 3);
    }
}

/**
 * Initialize image upload preview functionality
 */
function initializeImageUpload() {
    const imageInput = document.getElementById('coverImage');
    const imagePreview = document.querySelector('.event-details__image-placeholder');

    if (!imageInput || !imagePreview) return;

    imageInput.addEventListener('change', function(e) {
        const file = e.target.files[0];
        
        if (file && file.type.startsWith('image/')) {
            const reader = new FileReader();
            
            reader.onload = function(e) {
                imagePreview.style.backgroundImage = `url(${e.target.result})`;
                imagePreview.style.backgroundSize = 'cover';
                imagePreview.style.backgroundPosition = 'center';
                imagePreview.innerHTML = '';
            };
            
            reader.readAsDataURL(file);
        }
    });
}

/**
 * Initialize form validation
 */
function initializeFormValidation() {
    const form = document.querySelector('.event-details');
    
    if (!form) return;

    form.addEventListener('submit', function(e) {
        // Clear previous validation messages
        clearValidationMessages();

        // Validate required fields
        let isValid = true;
        const requiredFields = form.querySelectorAll('[required]');

        requiredFields.forEach(field => {
            if (!field.value.trim()) {
                showValidationError(field, 'This field is required');
                isValid = false;
            }
        });

        // Validate time range
        const startTime = form.querySelector('[name="StartTime"]');
        const endTime = form.querySelector('[name="EndTime"]');

        if (startTime && endTime && startTime.value && endTime.value) {
            if (startTime.value >= endTime.value) {
                showValidationError(endTime, 'End time must be after start time');
                isValid = false;
            }
        }

        // Validate postal code format (4 digits)
        const postalCode = form.querySelector('[name="PostalCode"]');
        if (postalCode && postalCode.value) {
            const postalCodePattern = /^\d{4}$/;
            if (!postalCodePattern.test(postalCode.value)) {
                showValidationError(postalCode, 'Postal code must be 4 digits');
                isValid = false;
            }
        }

        if (!isValid) {
            e.preventDefault();
        }
    });
}

/**
 * Show validation error for a field
 */
function showValidationError(field, message) {
    const validationSpan = field.parentElement.querySelector('.text-danger');
    if (validationSpan) {
        validationSpan.textContent = message;
    }
}

/**
 * Clear all validation messages
 */
function clearValidationMessages() {
    const validationMessages = document.querySelectorAll('.text-danger');
    validationMessages.forEach(msg => {
        msg.textContent = '';
    });
}

