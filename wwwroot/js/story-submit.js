// Story submission page - Rich text editor and form handling

document.addEventListener('DOMContentLoaded', function() {
    // Get user-specific draft key from data attribute or use default
    const mainElement = document.querySelector('.story-submit');
    const userId = mainElement?.dataset?.userId || 'anonymous';
    const DRAFT_KEY = `story-draft-${userId}`;
    const AUTOSAVE_INTERVAL = 30000; // 30 seconds

    // Get form elements
    const titleInput = document.querySelector('input#title');
    const anonymousCheckbox = document.querySelector('input#anonymous');
    const form = document.querySelector('form');
    const coverImageInput = document.querySelector('input#coverImage');
    const imagePreview = document.querySelector('#imagePreview');
    const imageUploadArea = document.querySelector('#imageUploadArea');

    // Initialise tag manager
    const tagManager = new TagManager({
        inputId: 'tagsInput',
        tagsDisplayId: 'storyTags',
        hiddenInputId: 'tags',
        tagClass: 'story-submit__tag',
        tagTextClass: 'story-submit__tag-text',
        removeButtonClass: 'story-submit__tag-remove'
    });

    // Initialize Quill editor
    const quill = new Quill('#quill-editor', {
        theme: 'snow',
        placeholder: 'Share your story here...',
        modules: {
            toolbar: [
                [{ 'header': [1, 2, 3, false] }],
                ['bold', 'italic', 'underline'],
                [{ 'list': 'ordered' }, { 'list': 'bullet' }],
                ['blockquote'],
                ['clean']
            ]
        }
    });

    // Handle cover image preview
    if (coverImageInput && imagePreview && imageUploadArea) {
        coverImageInput.addEventListener('change', function(event) {
            const file = event.target.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = function(e) {
                    imagePreview.src = e.target.result;
                    imagePreview.style.display = 'block';
                    imageUploadArea.style.display = 'none';
                };
                reader.readAsDataURL(file);
            }
        });
    }

    // Load saved draft on page load
    function loadDraft() {
        try {
            const draftData = localStorage.getItem(DRAFT_KEY);
            if (draftData) {
                const draft = JSON.parse(draftData);

                // Restore form values
                if (draft.title) titleInput.value = draft.title;
                if (draft.content) quill.root.innerHTML = draft.content;
                if (draft.tags && Array.isArray(draft.tags)) {
                    tagManager.setTags(draft.tags);
                }
                if (draft.anonymous !== undefined) anonymousCheckbox.checked = draft.anonymous;

                // Show restoration message
                if (draft.title || draft.content) {
                    showDraftStatus('Draft restored from ' + new Date(draft.savedAt).toLocaleTimeString(), true);
                }
            }
        } catch (error) {
            console.error('Error loading draft:', error);
        }
    }

    // Save draft to localStorage
    function saveDraft() {
        try {
            const draft = {
                title: titleInput.value.trim(),
                content: quill.root.innerHTML,
                tags: tagManager.getTags(),
                anonymous: anonymousCheckbox.checked,
                savedAt: new Date().toISOString()
            };

            // Only save if there's actual content
            if (draft.title || quill.getText().trim().length > 0) {
                localStorage.setItem(DRAFT_KEY, JSON.stringify(draft));
                showDraftStatus('Draft saved at ' + new Date().toLocaleTimeString());
                return true;
            }
        } catch (error) {
            console.error('Error saving draft:', error);
            showDraftStatus('Failed to save draft', false);
        }
        return false;
    }

    // Clear draft from localStorage
    function clearDraft() {
        try {
            localStorage.removeItem(DRAFT_KEY);
        } catch (error) {
            console.error('Error clearing draft:', error);
        }
    }

    // Show draft status indicator
    function showDraftStatus(message, isInfo = false) {
        let indicator = document.querySelector('.draft-status-indicator');

        if (!indicator) {
            indicator = document.createElement('div');
            indicator.className = 'draft-status-indicator';

            // Insert after the content field
            const contentField = document.querySelector('.story-submit__field:has(#quill-editor)');
            if (contentField) {
                contentField.appendChild(indicator);
            }
        }

        indicator.textContent = message;
        indicator.classList.remove('draft-status-indicator--visible', 'draft-status-indicator--info');

        if (isInfo) {
            indicator.classList.add('draft-status-indicator--info');
        }

        // Trigger reflow to restart animation
        void indicator.offsetWidth;
        indicator.classList.add('draft-status-indicator--visible');

        // Auto-hide after 3 seconds
        setTimeout(() => {
            indicator.classList.remove('draft-status-indicator--visible');
        }, 3000);
    }

    // Debounce function to limit save frequency
    let saveTimeout;
    function debouncedSave() {
        clearTimeout(saveTimeout);
        saveTimeout = setTimeout(saveDraft, 2000); // Save 2 seconds after last change
    }

    // Auto-save at regular intervals
    setInterval(saveDraft, AUTOSAVE_INTERVAL);

    // Save on content changes
    quill.on('text-change', debouncedSave);
    titleInput.addEventListener('input', debouncedSave);
    anonymousCheckbox.addEventListener('change', debouncedSave);

    // Save when tags change (listen to the tags display container for any changes)
    document.getElementById('storyTags').addEventListener('DOMSubtreeModified', debouncedSave);

    // Form submission handler moved to validation section below

    // Manual save draft button
    const saveDraftButton = document.getElementById('saveDraftButton');
    if (saveDraftButton) {
        saveDraftButton.addEventListener('click', function(e) {
            e.preventDefault();
            if (saveDraft()) {
                showDraftStatus('Draft saved successfully!', true);
            } else {
                showDraftStatus('Please add a title or content to save a draft', false);
            }
        });
    }

    // Word and character counter
    function updateWordCounter() {
        const text = quill.getText().trim();

        // Count words (split by whitespace and filter empty strings)
        const words = text.length > 0 ? text.split(/\s+/).filter(word => word.length > 0) : [];
        const wordCount = words.length;

        // Count characters (excluding whitespace at start/end)
        const charCount = text.length;

        // Update display
        const wordCountElement = document.querySelector('.story-submit__word-count');
        const charCountElement = document.querySelector('.story-submit__char-count');

        if (wordCountElement) {
            wordCountElement.textContent = `${wordCount.toLocaleString()} ${wordCount === 1 ? 'word' : 'words'}`;
        }

        if (charCountElement) {
            charCountElement.textContent = `${charCount.toLocaleString()} ${charCount === 1 ? 'character' : 'characters'}`;
        }
    }

    // Update word counter on text changes
    quill.on('text-change', updateWordCounter);

    // Load draft when page loads
    loadDraft();

    // Update word counter after loading draft
    updateWordCounter();

    // Validation configuration
    const VALIDATION_RULES = {
        title: {
            minLength: 3,
            maxLength: 200,
            required: true
        },
        content: {
            minWords: 10,
            maxWords: 10000,
            required: true
        }
    };

    // Validation functions
    function validateTitle() {
        const title = titleInput.value.trim();
        const errors = [];

        if (VALIDATION_RULES.title.required && title.length === 0) {
            errors.push('Title is required');
        } else if (title.length < VALIDATION_RULES.title.minLength) {
            errors.push(`Title must be at least ${VALIDATION_RULES.title.minLength} characters`);
        } else if (title.length > VALIDATION_RULES.title.maxLength) {
            errors.push(`Title must not exceed ${VALIDATION_RULES.title.maxLength} characters`);
        }

        return errors;
    }

    function validateContent() {
        const text = quill.getText().trim();
        const words = text.length > 0 ? text.split(/\s+/).filter(word => word.length > 0) : [];
        const wordCount = words.length;
        const errors = [];

        if (VALIDATION_RULES.content.required && wordCount === 0) {
            errors.push('Story content is required');
        } else if (wordCount < VALIDATION_RULES.content.minWords) {
            errors.push(`Story must be at least ${VALIDATION_RULES.content.minWords} words (currently ${wordCount})`);
        } else if (wordCount > VALIDATION_RULES.content.maxWords) {
            errors.push(`Story must not exceed ${VALIDATION_RULES.content.maxWords} words (currently ${wordCount})`);
        }

        return errors;
    }

    function showFieldError(fieldName, errors) {
        // Remove existing error
        clearFieldError(fieldName);

        if (errors.length === 0) return;

        // Find the field
        const field = document.querySelector(`input#${fieldName}, #quill-editor`);
        if (!field) return;

        const fieldContainer = field.closest('.story-submit__field');
        if (!fieldContainer) return;

        // Add error class to field
        field.classList.add('story-submit__field--error');

        // Create error message element
        const errorElement = document.createElement('div');
        errorElement.className = 'story-submit__error-message';
        errorElement.textContent = errors[0]; // Show first error
        errorElement.dataset.field = fieldName;

        // Insert error message after the field
        if (fieldName === 'content') {
            // For Quill editor, insert after the word counter
            const wordCounter = fieldContainer.querySelector('.story-submit__word-counter');
            if (wordCounter) {
                wordCounter.after(errorElement);
            } else {
                field.after(errorElement);
            }
        } else {
            field.after(errorElement);
        }
    }

    function clearFieldError(fieldName) {
        const field = document.querySelector(`input#${fieldName}, #quill-editor`);
        if (field) {
            field.classList.remove('story-submit__field--error');
        }

        // Remove error message
        const errorElement = document.querySelector(`.story-submit__error-message[data-field="${fieldName}"]`);
        if (errorElement) {
            errorElement.remove();
        }
    }

    function validateForm() {
        const titleErrors = validateTitle();
        const contentErrors = validateContent();

        showFieldError('title', titleErrors);
        showFieldError('content', contentErrors);

        return titleErrors.length === 0 && contentErrors.length === 0;
    }

    // Real-time validation
    titleInput.addEventListener('blur', function() {
        const errors = validateTitle();
        showFieldError('title', errors);
    });

    titleInput.addEventListener('input', function() {
        // Clear error when user starts typing
        if (titleInput.value.trim().length >= VALIDATION_RULES.title.minLength) {
            clearFieldError('title');
        }
    });

    quill.on('text-change', function() {
        // Clear error when content meets minimum
        const text = quill.getText().trim();
        const words = text.length > 0 ? text.split(/\s+/).filter(word => word.length > 0) : [];
        if (words.length >= VALIDATION_RULES.content.minWords) {
            clearFieldError('content');
        }
    });

    // Always keep the hidden content field synchronized with Quill
    quill.on('text-change', function() {
        const content = document.querySelector('input#content');
        if (content) {
            content.value = quill.root.innerHTML;
        }
    });

    // Validate and submit form
    form.addEventListener('submit', function(e) {
        e.preventDefault(); // Always prevent default first

        // Ensure content is populated
        const content = document.querySelector('input#content');
        if (content && quill) {
            content.value = quill.root.innerHTML;
            console.log('Content populated:', content.value.substring(0, 100)); // Debug log
        }

        // Validate the form
        if (!validateForm()) {
            // Scroll to first error
            const firstError = document.querySelector('.story-submit__error-message');
            if (firstError) {
                firstError.scrollIntoView({ behavior: 'smooth', block: 'center' });
            }
            return false;
        }

        // Clear draft BEFORE submitting (so it happens reliably before page redirect)
        clearDraft();

        // If validation passes, submit the form
        console.log('Submitting form...'); // Debug log
        form.submit();
    });
});
