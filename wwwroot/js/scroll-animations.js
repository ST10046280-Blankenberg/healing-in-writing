/**
 * Scroll Animations
 * Uses IntersectionObserver to trigger animations when elements enter the viewport.
 */

document.addEventListener('DOMContentLoaded', () => {
    // Check if user prefers reduced motion
    const prefersReducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

    if (prefersReducedMotion) {
        return;
    }

    const observerOptions = {
        root: null, // Use the viewport as the root
        rootMargin: '0px 0px -50px 0px', // Trigger slightly before the element is fully in view
        threshold: 0.1 // Trigger when 10% of the element is visible
    };

    const observer = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('visible');
            } else {
                // Remove the class when element leaves the viewport to allow re-animation
                entry.target.classList.remove('visible');
            }
        });
    }, observerOptions);

    // Select all elements with the .scroll-animate class
    const animatedElements = document.querySelectorAll('.scroll-animate');
    animatedElements.forEach(el => observer.observe(el));
});
