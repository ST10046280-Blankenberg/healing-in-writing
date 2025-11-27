/**
 * Frosted Glass Header Effect
 * Adds a frosted glass effect to the header when the user scrolls down.
 */

document.addEventListener('DOMContentLoaded', () => {
    const header = document.querySelector('.header');

    if (!header) {
        return;
    }

    // Throttle function to limit scroll event frequency
    let isScrolling = false;
    let lastScrollY = window.scrollY;
    const scrollThreshold = 50; // Pixels to scroll before activating effect

    const handleScroll = () => {
        if (!isScrolling) {
            window.requestAnimationFrame(() => {
                const currentScrollY = window.scrollY;

                if (currentScrollY > scrollThreshold) {
                    header.classList.add('header--scrolled');
                } else {
                    header.classList.remove('header--scrolled');
                }

                lastScrollY = currentScrollY;
                isScrolling = false;
            });

            isScrolling = true;
        }
    };

    // Add scroll event listener
    window.addEventListener('scroll', handleScroll, { passive: true });

    // Check initial scroll position on load
    handleScroll();
});