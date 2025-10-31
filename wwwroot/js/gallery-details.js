/**
 * Gallery Details - Image Carousel
 * Handles thumbnail navigation and main image switching
 * CSP-compliant: No inline JavaScript
 */

document.addEventListener("DOMContentLoaded", () => {
    const mainImage = document.getElementById('mainImage');
    const thumbList = document.getElementById('thumbList');
    const scrollLeftBtn = document.getElementById('scrollLeft');
    const scrollRightBtn = document.getElementById('scrollRight');
    const thumbnails = document.querySelectorAll('.gallery-details__thumb');

    if (!mainImage || !thumbList || thumbnails.length === 0) {
        return; // Exit if elements don't exist
    }

    // Handle thumbnail click
    thumbnails.forEach((thumb) => {
        thumb.addEventListener('click', function() {
            const imageUrl = this.getAttribute('data-url');
            const imageAlt = this.getAttribute('data-alt');

            // Update main image
            mainImage.src = imageUrl;
            mainImage.alt = imageAlt;

            // Update active state
            thumbnails.forEach(t => t.classList.remove('gallery-details__thumb--active'));
            this.classList.add('gallery-details__thumb--active');
        });
    });

    // Handle scroll buttons
    if (scrollLeftBtn) {
        scrollLeftBtn.addEventListener('click', () => {
            thumbList.scrollBy({
                left: -120, // Scroll by thumbnail width + gap
                behavior: 'smooth'
            });
        });
    }

    if (scrollRightBtn) {
        scrollRightBtn.addEventListener('click', () => {
            thumbList.scrollBy({
                left: 120, // Scroll by thumbnail width + gap
                behavior: 'smooth'
            });
        });
    }

    // Keyboard navigation for thumbnails
    thumbnails.forEach((thumb, index) => {
        thumb.setAttribute('tabindex', '0');
        thumb.addEventListener('keydown', (e) => {
            if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                thumb.click();
            } else if (e.key === 'ArrowLeft' && index > 0) {
                e.preventDefault();
                thumbnails[index - 1].focus();
            } else if (e.key === 'ArrowRight' && index < thumbnails.length - 1) {
                e.preventDefault();
                thumbnails[index + 1].focus();
            }
        });
    });
});

