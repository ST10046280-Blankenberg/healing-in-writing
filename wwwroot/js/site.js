// Mobile navigation toggle functionality
// Handles opening and closing the mobile menu with backdrop and focus management
document.addEventListener('DOMContentLoaded', function() {
    const toggle = document.querySelector('.header__mobile-toggle');
    const closeBtn = document.querySelector('.header__mobile-close');
    const nav = document.querySelector('.header__nav');
    const backdrop = document.querySelector('.header__backdrop');
    const body = document.body;

    // Get all focusable elements in the nav for focus trapping
    function getFocusableElements() {
        return nav.querySelectorAll(
            'a[href], button:not([disabled]), input:not([disabled]), select:not([disabled]), textarea:not([disabled]), [tabindex]:not([tabindex="-1"])'
        );
    }

    function openNav() {
        nav.classList.add('header__nav--open');
        backdrop.classList.add('header__backdrop--visible');
        toggle.setAttribute('aria-expanded', 'true');
        toggle.classList.add('header__mobile-toggle--hidden');
        closeBtn.classList.add('header__mobile-close--visible');
        body.style.overflow = 'hidden'; // Prevent scrolling when nav is open

        // Focus the close button when menu opens
        setTimeout(() => {
            closeBtn?.focus();
        }, 300); // Wait for animation to complete
    }

    function closeNav() {
        nav.classList.remove('header__nav--open');
        backdrop.classList.remove('header__backdrop--visible');
        toggle.setAttribute('aria-expanded', 'false');
        toggle.classList.remove('header__mobile-toggle--hidden');
        closeBtn.classList.remove('header__mobile-close--visible');
        body.style.overflow = ''; // Restore scrolling

        // Return focus to toggle button
        toggle?.focus();
    }

    // Toggle button click handler
    if (toggle && nav) {
        toggle.addEventListener('click', function() {
            const isOpen = nav.classList.contains('header__nav--open');
            if (isOpen) {
                closeNav();
            } else {
                openNav();
            }
        });
    }

    // Close button click handler
    if (closeBtn) {
        closeBtn.addEventListener('click', function() {
            closeNav();
        });
    }

    // Backdrop click handler
    if (backdrop) {
        backdrop.addEventListener('click', function() {
            closeNav();
        });
    }

    // Close menu on escape key
    document.addEventListener('keydown', function(e) {
        if (e.key === 'Escape' && nav.classList.contains('header__nav--open')) {
            closeNav();
        }
    });

    // Basic focus trap - keep focus within nav when open
    document.addEventListener('keydown', function(e) {
        if (!nav.classList.contains('header__nav--open')) return;
        if (e.key !== 'Tab') return;

        const focusableElements = getFocusableElements();
        const firstElement = focusableElements[0];
        const lastElement = focusableElements[focusableElements.length - 1];

        if (e.shiftKey) {
            // Shift + Tab
            if (document.activeElement === firstElement) {
                e.preventDefault();
                lastElement.focus();
            }
        } else {
            // Tab
            if (document.activeElement === lastElement) {
                e.preventDefault();
                firstElement.focus();
            }
        }
    });

    // Close nav when a link is clicked (for better mobile UX)
    const navLinks = nav.querySelectorAll('.header__nav-link');
    navLinks.forEach(link => {
        link.addEventListener('click', function() {
            closeNav();
        });
    });
});