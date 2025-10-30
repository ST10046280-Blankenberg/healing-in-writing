/**
 * Donate page - Copy to clipboard functionality
 * CSP-compliant: No inline JavaScript
 */

document.addEventListener('DOMContentLoaded', function() {
    // Get all copy buttons
    const copyButtons = document.querySelectorAll('.donate__copy-btn');

    copyButtons.forEach(button => {
        button.addEventListener('click', function() {
            // Get the value from the data-copy attribute
            const textToCopy = this.getAttribute('data-copy');

            if (!textToCopy) {
                console.error('No data-copy attribute found');
                return;
            }

            // Copy to clipboard using modern API
            if (navigator.clipboard && navigator.clipboard.writeText) {
                navigator.clipboard.writeText(textToCopy).then(() => {
                    // Show success feedback
                    showCopyFeedback(this, 'Copied!');
                }).catch(err => {
                    console.error('Failed to copy: ', err);
                    showCopyFeedback(this, 'Failed');
                });
            } else {
                // Fallback for older browsers
                fallbackCopy(textToCopy, this);
            }
        });
    });
});

/**
 * Show temporary feedback on the button
 */
function showCopyFeedback(button, message) {
    const originalHTML = button.innerHTML;
    button.innerHTML = `<span style="font-size: 12px;">${message}</span>`;
    button.style.pointerEvents = 'none';
    
    setTimeout(() => {
        button.innerHTML = originalHTML;
        button.style.pointerEvents = 'auto';
    }, 2000);
}

/**
 * Fallback copy method for older browsers
 */
function fallbackCopy(text, button) {
    const textArea = document.createElement('textarea');
    textArea.value = text;
    textArea.style.position = 'fixed';
    textArea.style.left = '-9999px';
    document.body.appendChild(textArea);
    textArea.select();
    
    try {
        document.execCommand('copy');
        showCopyFeedback(button, 'Copied!');
    } catch (err) {
        console.error('Fallback copy failed: ', err);
        showCopyFeedback(button, 'Failed');
    }
    
    document.body.removeChild(textArea);
}

