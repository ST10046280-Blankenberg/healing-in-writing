// Mobile navigation toggle functionality
// Handles opening and closing the mobile menu
document.addEventListener('DOMContentLoaded', function() {
    const toggle = document.querySelector('.header__mobile-toggle');
    const nav = document.querySelector('.header__nav');

    if (toggle && nav) {
        toggle.addEventListener('click', function() {
            const isOpen = nav.classList.toggle('header__nav--open');
            toggle.setAttribute('aria-expanded', isOpen);
        });
    }
});
