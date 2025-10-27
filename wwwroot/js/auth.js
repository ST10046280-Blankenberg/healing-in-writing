// Toggle between login and register forms with simple fade transition
document.addEventListener('DOMContentLoaded', function() {
    const authContainer = document.querySelector('.auth');
    const toggleLinks = document.querySelectorAll('[data-toggle]');
    const title = document.querySelector('.auth__title');
    const subtitle = document.querySelector('.auth__subtitle');
    const contentTitle = document.querySelector('.auth__content-title');
    const contentText = document.querySelector('.auth__content-text');

    // Set initial state based on query parameter
    const urlParams = new URLSearchParams(window.location.search);
    const mode = urlParams.get('mode');
    if (mode === 'register') {
        updateContent('register');
    } else {
        updateContent('login');
    }

    toggleLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            const targetMode = this.getAttribute('data-toggle');

            // Prevent multiple clicks during transition
            if (authContainer.classList.contains('transitioning')) {
                return;
            }

            // Add transitioning class for fade-out
            authContainer.classList.add('transitioning');

            // Wait for fade-out, then switch state
            setTimeout(() => {
                if (targetMode === 'register') {
                    authContainer.classList.remove('auth--login');
                    authContainer.classList.add('auth--register');
                    updateContent('register');
                    document.title = 'Join Our Community - Healing In Writing';
                    window.history.pushState({}, '', '/Auth/Auth?mode=register');
                } else {
                    authContainer.classList.remove('auth--register');
                    authContainer.classList.add('auth--login');
                    updateContent('login');
                    document.title = 'Sign In - Healing In Writing';
                    window.history.pushState({}, '', '/Auth/Auth?mode=login');
                }

                // Remove transitioning class for fade-in
                setTimeout(() => {
                    authContainer.classList.remove('transitioning');
                }, 50);
            }, 200);
        });
    });

    function updateContent(mode) {
        if (mode === 'register') {
            title.textContent = 'Join Our Community';
            subtitle.textContent = 'Create your account and start your healing journey';
            contentTitle.textContent = 'Your Story Matters';
            contentText.textContent = 'Join a supportive community where your voice is heard and your healing journey is honored through the transformative power of writing.';
        } else {
            title.textContent = 'Welcome Back';
            subtitle.textContent = 'Sign in to continue your healing journey';
            contentTitle.textContent = 'Join Our Community';
            contentText.textContent = 'Connect with others, share your story, and find healing through the power of written expression.';
        }
    }
});
