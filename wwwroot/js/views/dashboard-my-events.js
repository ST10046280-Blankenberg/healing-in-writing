/**
 * Dashboard My Events Page
 * Handles tab switching functionality for event status filtering
 */

(function() {
    'use strict';

    // Wait for DOM to be fully loaded
    document.addEventListener('DOMContentLoaded', function() {
        initializeTabSwitching();
    });

    /**
     * Initialize tab switching functionality
     */
    function initializeTabSwitching() {
        const tabs = document.querySelectorAll('.my-events__nav-button');
        const cards = document.querySelectorAll('.event-card');

        if (tabs.length === 0 || cards.length === 0) {
            return; // Exit if elements don't exist
        }

        tabs.forEach(function(tab) {
            tab.addEventListener('click', function() {
                handleTabClick(tab, tabs, cards);
            });
        });
    }

    /**
     * Handle tab click event
     * @param {HTMLElement} clickedTab - The tab that was clicked
     * @param {NodeList} allTabs - All tab elements
     * @param {NodeList} allCards - All event card elements
     */
    function handleTabClick(clickedTab, allTabs, allCards) {
        // Remove active class from all tabs
        allTabs.forEach(function(tab) {
            tab.classList.remove('my-events__nav-button--active');
        });

        // Add active class to clicked tab
        clickedTab.classList.add('my-events__nav-button--active');

        // Get the tab name from data attribute
        const tabName = clickedTab.getAttribute('data-tab');

        // Show/hide cards based on status
        filterCardsByStatus(allCards, tabName);
    }

    /**
     * Filter cards by status
     * @param {NodeList} cards - All event card elements
     * @param {string} status - The status to filter by
     */
    function filterCardsByStatus(cards, status) {
        cards.forEach(function(card) {
            const cardStatus = card.getAttribute('data-status');

            if (cardStatus === status) {
                card.style.display = 'flex';
            } else {
                card.style.display = 'none';
            }
        });
    }
})();
