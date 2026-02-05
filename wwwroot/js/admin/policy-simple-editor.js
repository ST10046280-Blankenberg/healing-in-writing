document.addEventListener('DOMContentLoaded', function () {
    let activeEditor = null;

    // Policy editor tab switching
    document.querySelectorAll('.policy-tab-btn').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var targetId = this.getAttribute('data-tab');

            // Deactivate all tabs and panels
            document.querySelectorAll('.policy-tab-btn').forEach(function (b) {
                b.classList.remove('policy-tab-btn--active');
            });
            document.querySelectorAll('.policy-tab-panel').forEach(function (panel) {
                panel.style.display = 'none';
            });

            // Activate clicked tab and target panel
            this.classList.add('policy-tab-btn--active');
            var target = document.getElementById(targetId);
            if (target) {
                target.style.display = '';
            }
        });
    });

    function bindRichTextEditors(container) {
        container.querySelectorAll('.simple-editor__rich-text').forEach(function (editor) {
            editor.addEventListener('focus', function () {
                activeEditor = this;
            });
            editor.addEventListener('click', function () {
                activeEditor = this;
            });
        });
    }

    function bindToolbars(container) {
        container.querySelectorAll('.simple-editor__toolbar').forEach(function (toolbar) {
            toolbar.querySelectorAll('.simple-editor__btn').forEach(function (button) {
                button.addEventListener('click', function (e) {
                    e.preventDefault();
                    var command = this.getAttribute('data-command');
                    if (!command) return;

                    if (activeEditor) {
                        activeEditor.focus();
                    }

                    if (command === 'createLink') {
                        var url = prompt('Enter the link URL (include https://)');
                        if (url) {
                            document.execCommand('createLink', false, url);
                        }
                        return;
                    }

                    document.execCommand(command, false, null);
                });
            });
        });
    }

    // Initial binding for all existing editors and toolbars
    bindRichTextEditors(document);
    bindToolbars(document);

    // Form submission: copy contenteditable innerHTML to hidden inputs
    document.querySelectorAll('.simple-policy-form').forEach(function (form) {
        form.addEventListener('submit', function () {
            form.querySelectorAll('.simple-editor__rich-text').forEach(function (editor) {
                var inputName = editor.getAttribute('data-input');
                if (!inputName) return;
                var hidden = form.querySelector('input[name="' + inputName + '"]');
                if (hidden) {
                    hidden.value = editor.innerHTML.trim();
                }
            });
        });
    });

    // Build section HTML for a given prefix and index
    function buildSectionHtml(prefix, index) {
        var num = index + 1;
        return '<div class="simple-editor__section" data-section-index="' + index + '">' +
            '<div class="simple-editor__section-header">' +
                '<h4 class="simple-editor__section-title">Section ' + num + '</h4>' +
                '<button type="button" class="simple-editor__remove-section admin-settings-btn admin-settings-btn--danger">Remove</button>' +
            '</div>' +
            '<label class="privacy-editor__label">Section Title</label>' +
            '<input name="' + prefix + '.TemplateData.Sections[' + index + '].Title" value="" class="privacy-editor__input" />' +

            '<label class="privacy-editor__label">Section Text</label>' +
            '<div class="simple-editor__toolbar">' +
                '<button type="button" class="simple-editor__btn" data-command="bold">Bold</button>' +
                '<button type="button" class="simple-editor__btn" data-command="italic">Italic</button>' +
                '<button type="button" class="simple-editor__btn" data-command="insertUnorderedList">Bullets</button>' +
                '<button type="button" class="simple-editor__btn" data-command="createLink">Link</button>' +
            '</div>' +
            '<div class="simple-editor__rich-text" contenteditable="true" data-input="' + prefix + '.TemplateData.Sections[' + index + '].Body"></div>' +
            '<input type="hidden" name="' + prefix + '.TemplateData.Sections[' + index + '].Body" value="" />' +

            '<label class="privacy-editor__label">Bullet List (one per line)</label>' +
            '<textarea name="' + prefix + '.TemplateData.Sections[' + index + '].BulletsText" class="privacy-editor__textarea" rows="4"></textarea>' +

            '<label class="privacy-editor__label">Highlight Title (optional)</label>' +
            '<input name="' + prefix + '.TemplateData.Sections[' + index + '].HighlightTitle" value="" class="privacy-editor__input" />' +

            '<label class="privacy-editor__label">Highlight Text (optional)</label>' +
            '<div class="simple-editor__toolbar">' +
                '<button type="button" class="simple-editor__btn" data-command="bold">Bold</button>' +
                '<button type="button" class="simple-editor__btn" data-command="italic">Italic</button>' +
                '<button type="button" class="simple-editor__btn" data-command="insertUnorderedList">Bullets</button>' +
                '<button type="button" class="simple-editor__btn" data-command="createLink">Link</button>' +
            '</div>' +
            '<div class="simple-editor__rich-text" contenteditable="true" data-input="' + prefix + '.TemplateData.Sections[' + index + '].HighlightBody"></div>' +
            '<input type="hidden" name="' + prefix + '.TemplateData.Sections[' + index + '].HighlightBody" value="" />' +
        '</div>';
    }

    // Re-index all sections within a container to ensure sequential indices
    function reindexSections(container) {
        var prefix = container.closest('form').querySelector('.simple-editor__add-section').getAttribute('data-prefix');
        var sections = container.querySelectorAll('.simple-editor__section');
        sections.forEach(function (section, idx) {
            section.setAttribute('data-section-index', idx);
            section.querySelector('.simple-editor__section-title').textContent = 'Section ' + (idx + 1);

            // Update all name attributes containing Sections[
            section.querySelectorAll('[name*="Sections["]').forEach(function (el) {
                el.name = el.name.replace(/Sections\[\d+\]/, 'Sections[' + idx + ']');
            });

            // Update data-input attributes containing Sections[
            section.querySelectorAll('[data-input*="Sections["]').forEach(function (el) {
                el.setAttribute('data-input', el.getAttribute('data-input').replace(/Sections\[\d+\]/, 'Sections[' + idx + ']'));
            });
        });
    }

    // Add Section button handler
    document.querySelectorAll('.simple-editor__add-section').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var form = this.closest('form');
            var container = form.querySelector('.simple-editor__sections-container');
            var prefix = this.getAttribute('data-prefix');
            var sections = container.querySelectorAll('.simple-editor__section');
            var newIndex = sections.length;
            var html = buildSectionHtml(prefix, newIndex);

            container.insertAdjacentHTML('beforeend', html);

            // Re-bind rich text editors and toolbars for the new section
            var newSection = container.querySelector('.simple-editor__section:last-child');
            bindRichTextEditors(newSection);
            bindToolbars(newSection);
        });
    });

    // Remove Section button handler (delegated)
    document.addEventListener('click', function (e) {
        var removeBtn = e.target.closest('.simple-editor__remove-section');
        if (!removeBtn) return;

        if (!confirm('Are you sure you want to remove this section?')) return;

        var section = removeBtn.closest('.simple-editor__section');
        var container = section.closest('.simple-editor__sections-container');
        section.remove();
        reindexSections(container);
    });
});
