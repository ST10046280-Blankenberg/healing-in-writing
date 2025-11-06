/* =========================================================
   FAQ Page - Accordion Functionality
   ========================================================= */

/**
 * Toggles the active state of an FAQ accordion item
 * @param {HTMLElement} button - The clicked FAQ question button
 */
function toggleFaq(button) {
    const item = button.closest('.faq__item');
    const isActive = item.classList.contains('faq__item--active');
    
    // Close all other items
    document.querySelectorAll('.faq__item--active').forEach(activeItem => {
        if (activeItem !== item) {
            activeItem.classList.remove('faq__item--active');
        }
    });
    
    // Toggle current item
    if (isActive) {
        item.classList.remove('faq__item--active');
    } else {
        item.classList.add('faq__item--active');
    }
}

/**
 * Initialize FAQ page functionality
 */
document.addEventListener('DOMContentLoaded', function() {
    // Add click event listeners to all FAQ question buttons
    const faqButtons = document.querySelectorAll('.faq__question');
    
    faqButtons.forEach(button => {
        button.addEventListener('click', function() {
            toggleFaq(this);
        });
        
        // Add keyboard support for Enter and Space keys
        button.addEventListener('keydown', function(event) {
            if (event.key === 'Enter' || event.key === ' ') {
                event.preventDefault();
                toggleFaq(this);
            }
        });
    });
    
    // Optional: Close FAQ items when clicking outside
    document.addEventListener('click', function(event) {
        const clickedInsideFaq = event.target.closest('.faq__item');
        if (!clickedInsideFaq) {
            // Uncomment below to close all items when clicking outside
            // document.querySelectorAll('.faq__item--active').forEach(item => {
            //     item.classList.remove('faq__item--active');
            // });
        }
    });
});

