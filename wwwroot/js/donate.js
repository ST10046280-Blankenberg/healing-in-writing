// Donate page - Copy to clipboard functionality

document.addEventListener('DOMContentLoaded', function() {
    // Get all copy buttons
    const copyButtons = document.querySelectorAll('.donate__copy-btn');

    copyButtons.forEach(button => {
        button.addEventListener('click', function() {
            // Get the value from the previous sibling element
            const valueElement = this.previousElementSibling;
            const textToCopy = valueElement.textContent;

            // Copy to clipboard
            navigator.clipboard.writeText(textToCopy).then(() => {
                // Show success feedback
                alert('Copied to clipboard!');
            }).catch(err => {
                console.error('Failed to copy: ', err);
            });
        });
    });
});
