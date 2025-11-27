# UI Improvements - Button Loading States & Toast Notifications

This document explains the new UI components added to improve user feedback for alerts, error messages, success messages, and loading states.

## Components Added

### 1. Button Loading States
**Location**: `/wwwroot/css/shared/button-loading.css`

Provides visual feedback during form submissions and async operations.

#### Features
- Spinning loader animation inside buttons
- Prevents double-submission
- Accessible screen reader announcements
- Multiple size variants (small, default, large)
- Alternative dot-style loader
- Reduced motion support

#### Basic Usage

```html
<!-- Add loading state via JavaScript -->
<button type="submit" class="auth__button" id="submitBtn">Submit</button>

<script>
    const button = document.getElementById('submitBtn');

    // On form submit
    button.classList.add('btn-loading');
    button.disabled = true;

    // Remove when complete
    button.classList.remove('btn-loading');
    button.disabled = false;
</script>
```

#### Size Variants

```html
<!-- Small button -->
<button class="btn btn-sm btn-loading">Loading...</button>

<!-- Default button -->
<button class="btn btn-loading">Loading...</button>

<!-- Large button -->
<button class="btn btn-lg btn-loading">Loading...</button>
```

#### Alternative Dot Style

```html
<!-- Three-dot animation instead of spinner -->
<button class="btn btn-loading-dots">Loading...</button>
```

#### Inline Spinner

```html
<!-- For buttons with icons where spinner replaces icon -->
<button class="btn">
    <span class="btn-spinner"></span>
    Loading...
</button>
```

---

### 2. Toast Notification System
**Location**:
- CSS: `/wwwroot/css/shared/toast.css`
- JavaScript: `/wwwroot/js/toast.js`
- Examples: `/wwwroot/js/toast-examples.js`

Auto-dismissing notifications with progress indicators.

#### Features
- Four notification types (success, error, warning, info)
- Auto-dismiss with customisable duration
- Visual progress bar showing time remaining
- Multiple positioning options
- Stack multiple notifications
- Accessible with ARIA labels
- Responsive design
- Reduced motion support

#### Basic Usage

```html
<!-- Include CSS and JS in your view -->
<link rel="stylesheet" href="~/css/shared/toast.css" />
<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css" />
<script src="~/js/toast.js"></script>

<script>
    // Success notification
    ToastManager.success('Success!', 'Your changes have been saved.');

    // Error notification
    ToastManager.error('Error', 'Unable to save changes.');

    // Warning notification
    ToastManager.warning('Warning', 'Your session will expire soon.');

    // Info notification
    ToastManager.info('Tip', 'Press Ctrl+S to save quickly.');
</script>
```

#### Advanced Options

```javascript
// Custom duration (in milliseconds)
ToastManager.success('Saved', 'Document saved successfully.', {
    duration: 3000  // Show for 3 seconds
});

// Disable auto-dismiss (requires manual close)
ToastManager.error('Critical', 'Please contact support.', {
    duration: 0,  // Never auto-dismiss
    closeButton: true
});

// Hide progress bar
ToastManager.info('Tip', 'Use keyboard shortcuts.', {
    showProgress: false
});

// Hide close button
ToastManager.warning('Processing', 'Please wait...', {
    closeButton: false
});
```

#### Configuration

```javascript
// Customise default settings on page load
ToastManager.init({
    position: 'top-right',      // Position on screen
    duration: 5000,             // Default duration (5 seconds)
    showProgress: true,         // Show progress bar
    closeButton: true,          // Show close button
    maxToasts: 5                // Maximum toasts to show
});

// Change position later
ToastManager.updateConfig({
    position: 'bottom-right'
});
```

#### Position Options
- `top-right` (default)
- `top-left`
- `top-center`
- `bottom-right`
- `bottom-left`
- `bottom-center`

#### Clear All Toasts

```javascript
// Remove all visible toasts
ToastManager.clearAll();
```

---

## Integration Examples

### Example 1: Form Submission with Loading State

```html
<form id="myForm">
    <input type="text" name="username" required />
    <button type="submit" class="auth__button">Submit</button>
</form>

<script>
    document.getElementById('myForm').addEventListener('submit', function(e) {
        if (this.checkValidity()) {
            const button = this.querySelector('button[type="submit"]');

            // Show loading state
            button.classList.add('btn-loading');
            button.disabled = true;

            // Add accessibility label
            const label = document.createElement('span');
            label.className = 'btn-loading-label';
            label.textContent = 'Processing, please wait...';
            button.appendChild(label);
        }
    });
</script>
```

### Example 2: Show Toast on Validation Error

```html
<script>
    document.addEventListener('DOMContentLoaded', function() {
        const validationSummary = document.querySelector('.validation-summary-errors');

        if (validationSummary) {
            ToastManager.error(
                'Validation Failed',
                'Please correct the errors below.',
                { duration: 6000 }
            );
        }
    });
</script>
```

### Example 3: ASP.NET MVC TempData Integration

```csharp
// In your controller
TempData["SuccessMessage"] = "Account created successfully!";
return RedirectToAction("Dashboard");
```

```html
<!-- In your view or layout -->
<script>
    document.addEventListener('DOMContentLoaded', function() {
        var successMsg = '@TempData["SuccessMessage"]';
        if (successMsg && successMsg !== '') {
            ToastManager.success('Success', successMsg);
        }

        var errorMsg = '@TempData["ErrorMessage"]';
        if (errorMsg && errorMsg !== '') {
            ToastManager.error('Error', errorMsg);
        }
    });
</script>
```

### Example 4: AJAX Request with Loading and Toast

```javascript
async function saveData() {
    const button = document.getElementById('saveBtn');

    // Show loading state
    button.classList.add('btn-loading');
    button.disabled = true;

    try {
        const response = await fetch('/api/save', {
            method: 'POST',
            body: JSON.stringify(data)
        });

        if (response.ok) {
            ToastManager.success('Saved', 'Your data has been saved successfully.');
        } else {
            ToastManager.error('Error', 'Failed to save data. Please try again.');
        }
    } catch (error) {
        ToastManager.error('Error', 'Network error. Please check your connection.');
    } finally {
        // Remove loading state
        button.classList.remove('btn-loading');
        button.disabled = false;
    }
}
```

---

## Already Implemented

Both Login and Register pages (`Views/Auth/Login.cshtml` and `Views/Auth/Register.cshtml`) have been updated with:

1. **Button loading states** on form submission
2. **Toast notifications** for validation errors
3. **Accessibility improvements** with screen reader labels
4. **Font Awesome icons** for visual feedback

### What Happens Now:

**On Form Submit:**
- Submit button shows spinning loader
- Button becomes disabled (prevents double-submission)
- Screen reader announces "Signing in, please wait..." or "Creating account, please wait..."

**On Validation Error:**
- Error toast appears in top-right corner
- Toast shows "Sign In Failed" or "Registration Failed"
- Inline validation errors remain visible below form fields
- Toast auto-dismisses after 6 seconds

---

## Customisation

### Change Toast Colours

Edit `/wwwroot/css/shared/toast.css` and modify the colour variables:

```css
.toast-success {
    border-left-color: var(--color-success);
    background: rgba(209, 231, 221, 0.95);
}
```

### Change Button Loader Style

Edit `/wwwroot/css/shared/button-loading.css`:

```css
.btn-loading::after {
    border-width: 3px;  /* Thicker spinner */
    width: 24px;        /* Larger spinner */
    height: 24px;
}
```

### Add Custom Toast Icons

Edit `/wwwroot/js/toast.js` and modify the `icons` object:

```javascript
const DEFAULT_CONFIG = {
    icons: {
        success: 'fa-circle-check',           // Change icon class
        error: 'fa-circle-xmark',
        warning: 'fa-triangle-exclamation',
        info: 'fa-circle-info'
    }
};
```

---

## Browser Support

- Modern browsers (Chrome, Firefox, Safari, Edge)
- IE11+ with polyfills for Promise and fetch
- Mobile browsers (iOS Safari, Chrome Mobile)
- Graceful fallback for older browsers

---

## Accessibility Features

1. **Keyboard Navigation**: All interactive elements are keyboard accessible
2. **Screen Readers**: ARIA labels and live regions announce state changes
3. **Focus Management**: Proper focus indicators and keyboard traps
4. **Reduced Motion**: Respects `prefers-reduced-motion` user preference
5. **Colour Contrast**: Meets WCAG AA standards for text contrast

---

## Performance Considerations

1. **CSS Only Animations**: Hardware-accelerated transforms for smooth performance
2. **Minimal JavaScript**: Lightweight toast manager (~200 lines)
3. **No Dependencies**: Pure vanilla JavaScript, no jQuery required
4. **Lazy Initialisation**: Toast container created only when first toast is shown
5. **Memory Management**: Toasts properly cleaned up after dismissal

---

## Troubleshooting

### Toasts Not Appearing
- Check that `toast.css` and `toast.js` are included
- Check that Font Awesome is loaded for icons
- Check browser console for JavaScript errors
- Ensure `ToastManager.init()` has been called

### Button Loading Not Working
- Verify `button-loading.css` is included
- Check that `btn-loading` class is being added correctly
- Inspect button element in DevTools to verify class application

### Icons Not Showing
- Verify Font Awesome CDN link is correct and accessible
- Check network tab to ensure Font Awesome CSS loaded
- Try using a different CDN or self-host Font Awesome

---

## Best Practices

1. **Use Appropriate Types**: Success for positive actions, error for failures, warning for caution, info for neutral messages
2. **Keep Messages Concise**: Short titles and messages are more effective
3. **Set Appropriate Durations**: Longer messages need longer display times
4. **Don't Overuse**: Too many toasts can be annoying; use sparingly
5. **Combine with Inline Errors**: Toasts should complement, not replace, inline validation
6. **Test Accessibility**: Always test with keyboard and screen readers
7. **Consider Mobile**: Ensure toasts work well on small screens

---

## Future Enhancements

Potential improvements for future iterations:

1. **Inline Field Validation**: Real-time validation as users type
2. **Skeleton Loaders**: Content placeholders for page loads
3. **Animated Checkmarks**: Success animations for completed actions
4. **Toast Actions**: Add buttons to toasts (e.g., "Undo" actions)
5. **Sound Notifications**: Optional audio feedback (with user control)
6. **Dark Mode Support**: Automatic theme switching
7. **Notification Centre**: Persistent notification history
8. **Custom Animations**: Additional entrance/exit animations

---

## Questions or Issues?

If you encounter any problems or have suggestions for improvements, please document them and consider:

1. Checking browser console for errors
2. Reviewing the examples in `toast-examples.js`
3. Testing in different browsers
4. Verifying all CSS/JS files are properly included
5. Checking for conflicting styles or scripts

---

**Version**: 1.0
**Last Updated**: 27 November 2025