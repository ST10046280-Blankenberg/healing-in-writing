document.addEventListener('DOMContentLoaded', function () {
    let activeEditor = null;

    document.querySelectorAll('.simple-editor__rich-text').forEach(editor => {
        editor.addEventListener('focus', function () {
            activeEditor = this;
        });
        editor.addEventListener('click', function () {
            activeEditor = this;
        });
    });
    const tabButtons = document.querySelectorAll('.policy-tab-btn');
    tabButtons.forEach(btn => {
        btn.addEventListener('click', function () {
            const targetId = this.getAttribute('data-target');
            const container = this.closest('.privacy-editor');
            if (!container || !targetId) return;

            container.querySelectorAll('.policy-tab-btn').forEach(b => b.classList.remove('is-active'));
            container.querySelectorAll('.policy-tab-panel').forEach(panel => panel.classList.remove('is-active'));

            this.classList.add('is-active');
            const panel = container.querySelector(`#${targetId}`);
            if (panel) {
                panel.classList.add('is-active');
            }
        });
    });

    const simpleForms = document.querySelectorAll('.simple-policy-form');
    simpleForms.forEach(form => {
        form.addEventListener('submit', function () {
            const editors = form.querySelectorAll('.simple-editor__rich-text');
            editors.forEach(editor => {
                const inputName = editor.getAttribute('data-input');
                if (!inputName) return;
                const hidden = form.querySelector(`input[name="${inputName}"]`);
                if (hidden) {
                    hidden.value = editor.innerHTML.trim();
                }
            });
        });

        const toolbars = form.querySelectorAll('.simple-editor__toolbar');
        toolbars.forEach(toolbar => {
            const buttons = toolbar.querySelectorAll('.simple-editor__btn');
            buttons.forEach(button => {
                button.addEventListener('click', function (e) {
                    e.preventDefault();
                    const command = this.getAttribute('data-command');
                    if (!command) return;

                    if (activeEditor) {
                        activeEditor.focus();
                    }

                    if (command === 'createLink') {
                        const url = prompt('Enter the link URL (include https://)');
                        if (url) {
                            document.execCommand('createLink', false, url);
                        }
                        return;
                    }

                    document.execCommand(command, false, null);
                });
            });
        });
    });
});
