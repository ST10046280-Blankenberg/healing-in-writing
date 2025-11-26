/**
 * Lightbox Gallery
 * Handles opening images in a modal, navigation, and closing.
 */

document.addEventListener('DOMContentLoaded', function () {
    // Create lightbox HTML if it doesn't exist
    if (!document.querySelector('.lightbox')) {
        const lightboxHtml = `
            <div id="lightbox" class="lightbox">
                <span class="lightbox__close">&times;</span>
                <img class="lightbox__content" id="lightboxImage">
                <div id="lightboxCaption" class="lightbox__caption"></div>
                <a class="lightbox__prev">&#10094;</a>
                <a class="lightbox__next">&#10095;</a>
            </div>
        `;
        document.body.insertAdjacentHTML('beforeend', lightboxHtml);
    }

    const lightbox = document.getElementById('lightbox');
    const lightboxImg = document.getElementById('lightboxImage');
    const captionText = document.getElementById('lightboxCaption');
    const closeBtn = document.querySelector('.lightbox__close');
    const prevBtn = document.querySelector('.lightbox__prev');
    const nextBtn = document.querySelector('.lightbox__next');

    let currentGroup = [];
    let currentIndex = 0;

    // Open lightbox
    function openLightbox(index, group) {
        currentGroup = group;
        currentIndex = index;
        updateLightboxContent();
        lightbox.style.display = 'flex';
        // Trigger reflow
        lightbox.offsetHeight;
        lightbox.classList.add('open');
        document.body.style.overflow = 'hidden'; // Disable scroll
    }

    // Close lightbox
    function closeLightbox() {
        lightbox.classList.remove('open');
        setTimeout(() => {
            lightbox.style.display = 'none';
            document.body.style.overflow = ''; // Enable scroll
        }, 300);
    }

    // Update image and caption
    function updateLightboxContent() {
        if (currentGroup.length === 0) return;

        const item = currentGroup[currentIndex];
        lightboxImg.src = item.src;
        captionText.innerHTML = item.alt;

        // Show/hide nav buttons
        if (currentGroup.length > 1) {
            prevBtn.style.display = 'block';
            nextBtn.style.display = 'block';
        } else {
            prevBtn.style.display = 'none';
            nextBtn.style.display = 'none';
        }
    }

    // Show next image
    function showNext() {
        currentIndex = (currentIndex + 1) % currentGroup.length;
        updateLightboxContent();
    }

    // Show prev image
    function showPrev() {
        currentIndex = (currentIndex - 1 + currentGroup.length) % currentGroup.length;
        updateLightboxContent();
    }

    // Event Listeners for triggers
    const triggers = document.querySelectorAll('[data-lightbox]');

    // Group triggers by their data-lightbox value
    const groups = {};

    triggers.forEach((trigger, index) => {
        const groupName = trigger.getAttribute('data-lightbox');
        if (!groups[groupName]) {
            groups[groupName] = [];
        }

        // Store item data
        const itemData = {
            src: trigger.getAttribute('href') || trigger.getAttribute('src') || trigger.getAttribute('data-src'),
            alt: trigger.getAttribute('data-alt') || trigger.getAttribute('alt') || ''
        };

        // Add to group
        groups[groupName].push(itemData);

        // Store index within its group
        trigger.setAttribute('data-index', groups[groupName].length - 1);

        trigger.addEventListener('click', function (e) {
            e.preventDefault();
            const gName = this.getAttribute('data-lightbox');
            const idx = parseInt(this.getAttribute('data-index'));
            openLightbox(idx, groups[gName]);
        });
    });

    // Control Event Listeners
    closeBtn.addEventListener('click', closeLightbox);

    prevBtn.addEventListener('click', (e) => {
        e.stopPropagation();
        showPrev();
    });

    nextBtn.addEventListener('click', (e) => {
        e.stopPropagation();
        showNext();
    });

    // Close on outside click
    lightbox.addEventListener('click', (e) => {
        if (e.target === lightbox) {
            closeLightbox();
        }
    });

    // Keyboard navigation
    document.addEventListener('keydown', function (e) {
        if (!lightbox.classList.contains('open')) return;

        if (e.key === 'Escape') {
            closeLightbox();
        } else if (e.key === 'ArrowLeft') {
            showPrev();
        } else if (e.key === 'ArrowRight') {
            showNext();
        }
    });
});
