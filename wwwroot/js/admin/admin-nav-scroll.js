/* =========================================================
   Admin Navigation Scroll Functionality
   Handles smooth scrolling and chevron button visibility
   ========================================================= */

/**
 * Initialize admin navigation scroll indicators
 */
document.addEventListener('DOMContentLoaded', function() {
    console.log('Admin nav scroll: DOMContentLoaded fired');
    
    // Small delay to ensure partial view is fully rendered
    setTimeout(function() {
        const navList = document.getElementById('adminNavList');
        const scrollLeftBtn = document.querySelector('.subnav__scroll-btn--left');
        const scrollRightBtn = document.querySelector('.subnav__scroll-btn--right');
        
        console.log('Admin nav scroll: Elements found:', {
            navList: !!navList,
            scrollLeftBtn: !!scrollLeftBtn,
            scrollRightBtn: !!scrollRightBtn
        });
        
        if (!navList || !scrollLeftBtn || !scrollRightBtn) {
            console.warn('Admin nav scroll: Required elements not found');
            return;
        }
        
        console.log('Admin nav scroll: Initializing...');
        
        // Scroll amount in pixels
        const scrollAmount = 200;
        
        /**
         * Update button visibility based on scroll position
         */
        function updateScrollButtons() {
            // Force reflow to get accurate measurements
            navList.offsetWidth;
            
            const scrollLeft = navList.scrollLeft;
            const scrollWidth = navList.scrollWidth;
            const clientWidth = navList.clientWidth;
            const maxScroll = scrollWidth - clientWidth;
            
            console.log('Scroll position:', {
                scrollLeft: scrollLeft,
                maxScroll: maxScroll,
                scrollWidth: scrollWidth,
                clientWidth: clientWidth,
                hasOverflow: scrollWidth > clientWidth
            });
            
            // Check if content overflows (needs scrolling)
            const hasOverflow = scrollWidth > clientWidth + 5; // 5px tolerance
            
            if (hasOverflow) {
                console.log('Content overflows - showing buttons');
                scrollLeftBtn.style.display = 'flex';
                scrollRightBtn.style.display = 'flex';
                
                // Hide left button if at start
                if (scrollLeft <= 5) {
                    scrollLeftBtn.style.opacity = '0';
                    scrollLeftBtn.style.pointerEvents = 'none';
                } else {
                    scrollLeftBtn.style.opacity = '1';
                    scrollLeftBtn.style.pointerEvents = 'auto';
                }
                
                // Hide right button if at end
                if (scrollLeft >= maxScroll - 5) {
                    scrollRightBtn.style.opacity = '0';
                    scrollRightBtn.style.pointerEvents = 'none';
                } else {
                    scrollRightBtn.style.opacity = '1';
                    scrollRightBtn.style.pointerEvents = 'auto';
                }
            } else {
                console.log('Content fits - hiding buttons');
                scrollLeftBtn.style.display = 'none';
                scrollRightBtn.style.display = 'none';
            }
        }
        
        /**
         * Scroll left button click handler
         */
        scrollLeftBtn.addEventListener('click', function() {
            console.log('Scroll left clicked');
            navList.scrollBy({
                left: -scrollAmount,
                behavior: 'smooth'
            });
        });
        
        /**
         * Scroll right button click handler
         */
        scrollRightBtn.addEventListener('click', function() {
            console.log('Scroll right clicked');
            navList.scrollBy({
                left: scrollAmount,
                behavior: 'smooth'
            });
        });
        
        // Update buttons on scroll
        navList.addEventListener('scroll', updateScrollButtons);
        
        // Update buttons on window resize with debounce
        let resizeTimeout;
        window.addEventListener('resize', function() {
            clearTimeout(resizeTimeout);
            resizeTimeout = setTimeout(updateScrollButtons, 150);
        });
        
        // Initial update
        console.log('Admin nav scroll: Running initial update...');
        updateScrollButtons();
        
        // Run again after a short delay to catch any late layout changes
        setTimeout(updateScrollButtons, 100);
        setTimeout(updateScrollButtons, 500);
        
        console.log('Admin nav scroll: Initialization complete');
    }, 100); // Wait 100ms for partial to render
});

