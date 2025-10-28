/**
 * Tag Manager - Reusable tag input component
 * CSP-compliant implementation using event delegation
 *
 * Features:
 * - Add tags by pressing Enter
 * - Remove tags by clicking remove button
 * - Syncs with hidden input field
 * - Prevents duplicate tags
 * - Supports initial tag loading
 */

class TagManager {
    /**
     * Create a new TagManager instance
     * @param {Object} config - Configuration object
     * @param {string} config.inputId - ID of the text input element
     * @param {string} config.tagsDisplayId - ID of the container for displaying tags
     * @param {string} config.hiddenInputId - ID of the hidden input for form submission
     * @param {string} config.tagClass - CSS class for tag elements (default: 'tag')
     * @param {string} config.tagTextClass - CSS class for tag text (default: 'tag__text')
     * @param {string} config.removeButtonClass - CSS class for remove button (default: 'tag__remove')
     */
    constructor(config) {
        this.inputElement = document.getElementById(config.inputId);
        this.tagsDisplay = document.getElementById(config.tagsDisplayId);
        this.hiddenInput = document.getElementById(config.hiddenInputId);
        this.tagClass = config.tagClass || 'tag';
        this.tagTextClass = config.tagTextClass || 'tag__text';
        this.removeButtonClass = config.removeButtonClass || 'tag__remove';

        this.tags = [];

        if (!this.inputElement || !this.tagsDisplay || !this.hiddenInput) {
            console.error('TagManager: Required elements not found');
            return;
        }

        this.init();
    }

    /**
     * Initialise event listeners
     */
    init() {
        // Add tag on Enter key
        this.inputElement.addEventListener('keydown', (e) => {
            if (e.key === 'Enter' && this.inputElement.value.trim()) {
                e.preventDefault();
                this.addTag(this.inputElement.value.trim());
                this.inputElement.value = '';
            }
        });

        // Remove tag on button click (using event delegation for CSP compliance)
        this.tagsDisplay.addEventListener('click', (e) => {
            const removeButton = e.target.closest(`.${this.removeButtonClass}`);
            if (removeButton) {
                const tagElement = removeButton.closest(`.${this.tagClass}`);
                if (tagElement) {
                    const index = parseInt(tagElement.dataset.index, 10);
                    this.removeTag(index);
                }
            }
        });
    }

    /**
     * Add a new tag
     * @param {string} tagText - The tag text to add
     * @returns {boolean} True if tag was added, false if duplicate
     */
    addTag(tagText) {
        // Prevent duplicates (case-insensitive)
        const normalised = tagText.toLowerCase();
        if (this.tags.some(tag => tag.toLowerCase() === normalised)) {
            return false;
        }

        this.tags.push(tagText);
        this.render();
        return true;
    }

    /**
     * Remove a tag by index
     * @param {number} index - The index of the tag to remove
     */
    removeTag(index) {
        if (index >= 0 && index < this.tags.length) {
            this.tags.splice(index, 1);
            this.render();
        }
    }

    /**
     * Set tags from an array
     * @param {Array<string>} tags - Array of tag strings
     */
    setTags(tags) {
        this.tags = Array.isArray(tags) ? [...tags] : [];
        this.render();
    }

    /**
     * Get current tags
     * @returns {Array<string>} Current tags array
     */
    getTags() {
        return [...this.tags];
    }

    /**
     * Render tags to the display container
     */
    render() {
        // Clear existing tags
        this.tagsDisplay.innerHTML = '';

        // Render each tag
        this.tags.forEach((tag, index) => {
            const tagElement = this.createTagElement(tag, index);
            this.tagsDisplay.appendChild(tagElement);
        });

        // Update hidden input with comma-separated values
        this.hiddenInput.value = this.tags.join(',');
    }

    /**
     * Create a tag DOM element
     * @param {string} tagText - The tag text
     * @param {number} index - The tag index
     * @returns {HTMLElement} The tag element
     */
    createTagElement(tagText, index) {
        const span = document.createElement('span');
        span.className = this.tagClass;
        span.dataset.index = index;

        // Tag text
        const textSpan = document.createElement('span');
        textSpan.className = this.tagTextClass;
        textSpan.textContent = tagText;

        // Remove button
        const removeButton = document.createElement('button');
        removeButton.type = 'button';
        removeButton.className = this.removeButtonClass;
        removeButton.setAttribute('aria-label', `Remove ${tagText} tag`);
        removeButton.innerHTML = '&times;';

        span.appendChild(textSpan);
        span.appendChild(removeButton);

        return span;
    }

    /**
     * Load tags from the hidden input value
     * Useful for restoring tags from form data
     */
    loadFromHiddenInput() {
        const value = this.hiddenInput.value.trim();
        if (value) {
            const tags = value.split(',').map(t => t.trim()).filter(t => t.length > 0);
            this.setTags(tags);
        }
    }
}

// Export for use in other modules
if (typeof module !== 'undefined' && module.exports) {
    module.exports = TagManager;
}
