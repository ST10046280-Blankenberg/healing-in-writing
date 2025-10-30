document.addEventListener('DOMContentLoaded', function() {

    // Get all the tab buttons and resource cards
    const tabContainer = document.querySelector('.resources-tabs');
    const tabs = document.querySelectorAll('.resources-tabs__btn');
    const cards = document.querySelectorAll('.resources-card');

    // Function to filter cards based on category
    function filterCards(category) {
        cards.forEach(card => {
            // Check if the card's category matches the selected one
            if (card.dataset.category === category) {
                // Show the card
                // We use 'flex' because the .resources-card class uses 'display: flex'
                card.style.display = 'flex';
            } else {
                // Hide the card
                card.style.display = 'none';
            }
        });
    }

    // Add click event listener to the tab container (event delegation)
    tabContainer.addEventListener('click', function(e) {
        // Check if the clicked element is a tab button
        const clickedTab = e.target.closest('.resources-tabs__btn');

        if (clickedTab) {
            // Get the category from the clicked button's data-attribute
            const category = clickedTab.dataset.category;

            // Update active state on buttons
            tabs.forEach(tab => {
                tab.classList.remove('resources-tabs__btn--active');
            });
            clickedTab.classList.add('resources-tabs__btn--active');

            // Filter the cards
            filterCards(category);
        }
    });

    // Run the filter on initial page load to show the default "crisis" tab
    filterCards('crisis');

});