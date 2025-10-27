// Story submission page - Rich text editor and form handling

document.addEventListener('DOMContentLoaded', function() {
    const DRAFT_KEY = 'story-draft';
    const AUTOSAVE_INTERVAL = 30000; // 30 seconds

    // Get form elements
    const titleInput = document.querySelector('input#title');
    const tagsInput = document.querySelector('input#tags');
    const anonymousCheckbox = document.querySelector('input#anonymous');
    const form = document.querySelector('form');

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

    // Load saved draft on page load
    function loadDraft() {
        try {
            const draftData = localStorage.getItem(DRAFT_KEY);
            if (draftData) {
                const draft = JSON.parse(draftData);

                // Restore form values
                if (draft.title) titleInput.value = draft.title;
                if (draft.content) quill.root.innerHTML = draft.content;
                if (draft.tags) tagsInput.value = draft.tags;
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
                tags: tagsInput.value.trim(),
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
    tagsInput.addEventListener('input', debouncedSave);
    anonymousCheckbox.addEventListener('change', debouncedSave);

    // Store HTML in hidden input before submitting
    form.addEventListener('submit', function(e) {
        const content = document.querySelector('input#content');
        content.value = quill.root.innerHTML;

        // Clear draft on successful submission
        // Wait a bit to ensure form submission completes
        setTimeout(clearDraft, 1000);
    });

    // Manual save button (if we add one later)
    const saveDraftButton = document.querySelector('.story-submit__button--secondary');
    if (saveDraftButton) {
        saveDraftButton.addEventListener('click', function(e) {
            e.preventDefault();
            if (saveDraft()) {
                showDraftStatus('Draft saved successfully!');
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

    // TODO: Implement tag management
    // TODO: Implement form validation
});
