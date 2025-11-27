/* =========================================================
   Toast Notification System - Usage Examples
   Demonstrates various toast notification patterns
   ========================================================= */

/**
 * This file contains usage examples for the ToastManager system.
 * You can reference these examples when implementing toasts in your views.
 *
 * IMPORTANT: This is a reference file, not meant to be included in production.
 * Copy the relevant examples to your actual view files.
 */

// ===================================================================
// BASIC USAGE EXAMPLES
// ===================================================================

/**
 * Example 1: Simple Success Toast
 * Use after successful form submission, save operations, etc.
 */
function showSuccessExample() {
    ToastManager.success(
        'Success!',
        'Your changes have been saved successfully.'
    );
}

/**
 * Example 2: Error Toast
 * Use for failed operations, validation errors, etc.
 */
function showErrorExample() {
    ToastManager.error(
        'Error Occurred',
        'Unable to complete the operation. Please try again.'
    );
}

/**
 * Example 3: Warning Toast
 * Use for non-critical warnings or cautionary messages
 */
function showWarningExample() {
    ToastManager.warning(
        'Warning',
        'Your session will expire in 5 minutes. Please save your work.'
    );
}

/**
 * Example 4: Info Toast
 * Use for general informational messages
 */
function showInfoExample() {
    ToastManager.info(
        'Did You Know?',
        'You can use keyboard shortcuts to navigate faster.'
    );
}

// ===================================================================
// ADVANCED USAGE EXAMPLES
// ===================================================================

/**
 * Example 5: Toast Without Auto-Dismiss
 * Use for important messages that require user acknowledgement
 */
function showPersistentToast() {
    ToastManager.error(
        'Critical Error',
        'System backup failed. Please contact support immediately.',
        {
            duration: 0,  // 0 or false disables auto-dismiss
            closeButton: true
        }
    );
}

/**
 * Example 6: Custom Duration
 * Use when you need more or less time than the default 5 seconds
 */
function showCustomDurationToast() {
    ToastManager.success(
        'File Uploaded',
        'Your document has been uploaded successfully.',
        { duration: 3000 }  // Show for 3 seconds only
    );
}

/**
 * Example 7: Toast Without Progress Bar
 * Cleaner look for simple notifications
 */
function showNoProgressToast() {
    ToastManager.info(
        'Tip',
        'Press Ctrl+S to quick-save your work.',
        { showProgress: false }
    );
}

/**
 * Example 8: Toast Without Close Button
 * Force users to wait for auto-dismiss
 */
function showNoCloseButtonToast() {
    ToastManager.warning(
        'Processing',
        'Please wait while we process your request...',
        {
            closeButton: false,
            duration: 8000
        }
    );
}

// ===================================================================
// FORM VALIDATION EXAMPLES
// ===================================================================

/**
 * Example 9: Show Toast After Successful Form Submission
 * Typically used after redirecting to a success page
 */
function handleFormSuccess() {
    // Check for success message in URL parameters
    const urlParams = new URLSearchParams(window.location.search);
    const successMessage = urlParams.get('success');

    if (successMessage) {
        ToastManager.success(
            'Success!',
            decodeURIComponent(successMessage),
            { duration: 6000 }
        );

        // Clean up URL
        window.history.replaceState({}, document.title, window.location.pathname);
    }
}

/**
 * Example 10: Show Toast for Form Validation Errors
 * Display friendly error message when validation fails
 */
function handleFormValidationErrors() {
    const validationSummary = document.querySelector('.validation-summary-errors');

    if (validationSummary) {
        const errorCount = validationSummary.querySelectorAll('li').length;
        const errorText = errorCount === 1 ? 'error' : 'errors';

        ToastManager.error(
            'Validation Failed',
            `Please correct ${errorCount} ${errorText} in the form below.`,
            { duration: 7000 }
        );
    }
}

/**
 * Example 11: Field-Specific Validation Toast
 * Show toast for a specific field error
 */
function showFieldError(fieldName, errorMessage) {
    ToastManager.error(
        `Invalid ${fieldName}`,
        errorMessage,
        { duration: 5000 }
    );
}

// ===================================================================
// ASYNCHRONOUS OPERATION EXAMPLES
// ===================================================================

/**
 * Example 12: Toast Sequence for Async Operations
 * Show progress through multiple stages
 */
async function performAsyncOperation() {
    // Initial info toast
    ToastManager.info(
        'Processing',
        'Uploading your files...',
        { duration: 0, closeButton: false }
    );

    try {
        // Simulate async operation
        await uploadFiles();

        // Clear previous toasts
        ToastManager.clearAll();

        // Success toast
        ToastManager.success(
            'Upload Complete',
            'All files have been uploaded successfully.',
            { duration: 5000 }
        );
    } catch (error) {
        // Clear previous toasts
        ToastManager.clearAll();

        // Error toast
        ToastManager.error(
            'Upload Failed',
            'Some files could not be uploaded. Please try again.',
            { duration: 7000 }
        );
    }
}

/**
 * Example 13: Multiple Related Notifications
 * Show multiple toasts for batch operations
 */
function showBatchOperationResults(successCount, failCount) {
    if (successCount > 0) {
        ToastManager.success(
            'Batch Complete',
            `Successfully processed ${successCount} items.`,
            { duration: 5000 }
        );
    }

    if (failCount > 0) {
        ToastManager.error(
            'Some Items Failed',
            `${failCount} items could not be processed.`,
            { duration: 7000 }
        );
    }
}

// ===================================================================
// USER ACTION CONFIRMATION EXAMPLES
// ===================================================================

/**
 * Example 14: Delete Confirmation Toast
 * Show confirmation after delete operation
 */
function confirmDeletion(itemName) {
    ToastManager.success(
        'Deleted',
        `"${itemName}" has been permanently deleted.`,
        { duration: 4000 }
    );
}

/**
 * Example 15: Save Draft Confirmation
 * Auto-save notification
 */
function confirmAutoSave() {
    ToastManager.info(
        'Draft Saved',
        'Your work has been automatically saved.',
        {
            duration: 3000,
            showProgress: false
        }
    );
}

/**
 * Example 16: Copy to Clipboard Confirmation
 * Quick feedback for clipboard operations
 */
function confirmCopy() {
    ToastManager.success(
        'Copied!',
        'Content has been copied to your clipboard.',
        {
            duration: 2000,
            showProgress: false
        }
    );
}

// ===================================================================
// CONFIGURATION EXAMPLES
// ===================================================================

/**
 * Example 17: Change Toast Position
 * Modify global configuration
 */
function changeToastPosition() {
    ToastManager.updateConfig({
        position: 'bottom-right'  // Options: top-right, top-left, bottom-right, bottom-left, top-center, bottom-center
    });
}

/**
 * Example 18: Customise Default Settings
 * Set custom defaults for your application
 */
function initialiseCustomToasts() {
    ToastManager.init({
        position: 'top-right',
        duration: 6000,         // 6 seconds default
        showProgress: true,
        closeButton: true,
        maxToasts: 3            // Only show 3 toasts at a time
    });
}

// ===================================================================
// INTEGRATION WITH ASP.NET MVC EXAMPLES
// ===================================================================

/**
 * Example 19: Show Toast from TempData
 * Display server-side messages using TempData
 *
 * In your controller:
 * TempData["SuccessMessage"] = "Account created successfully!";
 * return RedirectToAction("Dashboard");
 *
 * In your view (add to layout or specific view):
 */
function showTempDataToasts() {
    // Success message
    const successMessage = '@TempData["SuccessMessage"]';
    if (successMessage && successMessage !== '') {
        ToastManager.success('Success', successMessage);
    }

    // Error message
    const errorMessage = '@TempData["ErrorMessage"]';
    if (errorMessage && errorMessage !== '') {
        ToastManager.error('Error', errorMessage);
    }

    // Warning message
    const warningMessage = '@TempData["WarningMessage"]';
    if (warningMessage && warningMessage !== '') {
        ToastManager.warning('Warning', warningMessage);
    }

    // Info message
    const infoMessage = '@TempData["InfoMessage"]';
    if (infoMessage && infoMessage !== '') {
        ToastManager.info('Information', infoMessage);
    }
}

/**
 * Example 20: Integration with jQuery Validation
 * Show toast when jQuery validation fails
 */
function integrateWithJQueryValidation() {
    $('form').on('invalid-form.validate', function() {
        ToastManager.error(
            'Validation Error',
            'Please check the form for errors and try again.',
            { duration: 6000 }
        );
    });
}

// ===================================================================
// HELPER FUNCTION TO TEST ALL TOAST TYPES
// ===================================================================

/**
 * Example 21: Test All Toast Types
 * Useful for testing during development
 */
function testAllToasts() {
    setTimeout(() => ToastManager.success('Success', 'This is a success message'), 0);
    setTimeout(() => ToastManager.error('Error', 'This is an error message'), 500);
    setTimeout(() => ToastManager.warning('Warning', 'This is a warning message'), 1000);
    setTimeout(() => ToastManager.info('Info', 'This is an info message'), 1500);
}

// ===================================================================
// ACCESSIBILITY EXAMPLE
// ===================================================================

/**
 * Example 22: Accessible Toast with Focus Management
 * Ensure keyboard users can access toast close buttons
 */
function showAccessibleToast() {
    const toast = ToastManager.error(
        'Action Required',
        'Please review and accept the terms of service.',
        {
            duration: 0,
            closeButton: true
        }
    );

    // Optionally focus the close button for keyboard users
    setTimeout(() => {
        const closeButton = toast.querySelector('.toast__close');
        if (closeButton) {
            closeButton.focus();
        }
    }, 100);
}