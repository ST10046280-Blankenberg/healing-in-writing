document.addEventListener('DOMContentLoaded', function() {
    const editBtn = document.getElementById('editBtn');
    const saveBtn = document.getElementById('saveBtn');
    const inputs = document.querySelectorAll('.donation-settings__input');

    if (editBtn) {
        editBtn.addEventListener('click', function() {
            // Remove readonly from all inputs
            inputs.forEach(input => {
                input.removeAttribute('readonly');
            });

            // Toggle button visibility
            editBtn.classList.add('hidden');
            saveBtn.classList.remove('hidden');
        });
    }
});
