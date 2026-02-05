// Global loader helpers
(function () {
    function showLoader(text) {
        const loader = document.getElementById('global-loader');
        if (!loader) return;

        if (text) {
            const loadingText = loader.querySelector('.loading-text');
            if (loadingText) {
                loadingText.textContent = text;
            }
        }

        loader.classList.remove('d-none');
    }

    function hideLoader() {
        const loader = document.getElementById('global-loader');
        if (!loader) return;
        loader.classList.add('d-none');
    }

    window.showLoader = showLoader;
    window.hideLoader = hideLoader;
})();
