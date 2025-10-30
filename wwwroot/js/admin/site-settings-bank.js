/**
 * Site Settings - Bank Details Editor
 * Handles edit/save functionality for bank details form
 * CSP-compliant: No inline JavaScript
 */

document.addEventListener("DOMContentLoaded", () => {
    const editBtn = document.getElementById("editBtn");
    const saveBtn = document.getElementById("saveBtn");
    const inputs = document.querySelectorAll(".donation-settings__input");
    
    if (!editBtn || !saveBtn) {
        console.warn("Bank details edit buttons not found");
        return;
    }
    
    // Enable editing mode
    editBtn.addEventListener("click", () => {
        inputs.forEach(input => {
            input.removeAttribute("readonly");
            input.style.borderColor = "#2F1648";
            input.style.color = "#111827";
        });
        
        editBtn.classList.add("hidden");
        saveBtn.classList.remove("hidden");
        
        // Focus first input
        if (inputs.length > 0) {
            inputs[0].focus();
        }
    });
    
});

