namespace HealingInWriting.Models.Shared
{
    /// <summary>
    /// Configuration model for the reusable admin search bar component.
    /// Allows each admin page to customize search functionality without duplicating markup.
    /// </summary>
    public class AdminSearchBarConfig
    {
        /// <summary>
        /// Area name for form submission (default: "Admin")
        /// </summary>
        public string Area { get; set; } = "Admin";

        /// <summary>
        /// Controller name for form submission
        /// </summary>
        public string Controller { get; set; } = string.Empty;

        /// <summary>
        /// Action name for form submission (default: "Manage")
        /// </summary>
        public string Action { get; set; } = "Manage";

        /// <summary>
        /// Placeholder text for the search input
        /// </summary>
        public string SearchPlaceholder { get; set; } = "Search...";

        /// <summary>
        /// Name attribute for the search input field
        /// </summary>
        public string SearchInputName { get; set; } = "SearchText";

        /// <summary>
        /// List of filter dropdowns to render in the search bar
        /// </summary>
        public List<AdminFilterDropdown> Dropdowns { get; set; } = new();

        /// <summary>
        /// Hidden form fields to preserve state (e.g., page number)
        /// </summary>
        public Dictionary<string, string>? HiddenFields { get; set; }
    }

    /// <summary>
    /// Represents a filterable dropdown in the admin search bar
    /// </summary>
    public class AdminFilterDropdown
    {
        /// <summary>
        /// Name attribute for the select element (form field name)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Label text for the default option (e.g., "Any Status", "Any Date")
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Currently selected value
        /// </summary>
        public string? SelectedValue { get; set; }

        /// <summary>
        /// List of options for this dropdown
        /// </summary>
        public List<AdminDropdownOption> Options { get; set; } = new();
    }

    /// <summary>
    /// Represents an individual option within a dropdown filter
    /// </summary>
    public class AdminDropdownOption
    {
        /// <summary>
        /// Value attribute for the option element
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Display text for the option
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Whether this option is currently selected
        /// </summary>
        public bool Selected { get; set; }
    }
}

