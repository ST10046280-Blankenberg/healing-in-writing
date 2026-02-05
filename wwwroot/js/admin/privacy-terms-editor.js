document.addEventListener('DOMContentLoaded', function () {
    const forms = document.querySelectorAll('.policy-editor-form');

    forms.forEach(form => {
        const textarea = form.querySelector('.privacy-editor__textarea');
        if (!textarea) return;

        const toolbarButtons = form.querySelectorAll('.privacy-editor__toolbar-btn');
        toolbarButtons.forEach(button => {
            button.addEventListener('click', function(e) {
                e.preventDefault();
                const title = this.getAttribute('title');
                handleToolbarAction(title, textarea);
            });
        });

        form.addEventListener('submit', function (e) {
            const content = textarea.value.trim();
            if (!content) {
                e.preventDefault();
                alert('Please enter policy content before saving.');
                return false;
            }
        });

        const discardBtn = form.querySelector('.privacy-editor__btn-discard');
        if (discardBtn) {
            discardBtn.addEventListener('click', function (e) {
                if (!confirm('Are you sure you want to discard your changes?')) {
                    e.preventDefault();
                }
            });
        }
    });
});

function handleToolbarAction(action, textarea) {
    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    const selectedText = textarea.value.substring(start, end);
    const beforeText = textarea.value.substring(0, start);
    const afterText = textarea.value.substring(end);
    
    let newText = '';
    let cursorOffset = 0;

    switch(action) {
        case 'Bold':
            if (selectedText) {
                newText = `<strong>${selectedText}</strong>`;
                cursorOffset = newText.length;
            } else {
                newText = '<strong></strong>';
                cursorOffset = 8; // Position cursor between tags
            }
            break;
            
        case 'Italic':
            if (selectedText) {
                newText = `<em>${selectedText}</em>`;
                cursorOffset = newText.length;
            } else {
                newText = '<em></em>';
                cursorOffset = 4; // Position cursor between tags
            }
            break;
            
        case 'Bullet List':
            if (selectedText) {
                const items = selectedText.split('\n').filter(line => line.trim());
                const listItems = items.map(item => `  <li>${item.trim()}</li>`).join('\n');
                newText = `<ul>\n${listItems}\n</ul>`;
                cursorOffset = newText.length;
            } else {
                newText = '<ul>\n  <li></li>\n</ul>';
                cursorOffset = 9; // Position cursor in first list item
            }
            break;
            
        default:
            return;
    }

    // Update textarea value
    textarea.value = beforeText + newText + afterText;
    
    // Set cursor position
    textarea.focus();
    textarea.selectionStart = textarea.selectionEnd = start + cursorOffset;
    
    // Trigger change event
    textarea.dispatchEvent(new Event('input', { bubbles: true }));
}
