# Component System Guide

A comprehensive guide to using the standardised component system in Healing In Writing.

## Table of Contents

1. [Overview](#overview)
2. [Form Components](#form-components)
3. [Card Components](#card-components)
4. [Button Components](#button-components)
5. [Migration Guide](#migration-guide)
6. [Best Practices](#best-practices)

---

## Overview

The component system provides standardised, reusable UI components that use CSS variables from the design system. All components are:

- **Consistent**: Use the same colours, spacing, and typography
- **Responsive**: Work across all device sizes
- **Accessible**: Include focus states, ARIA support, and keyboard navigation
- **Non-breaking**: Work alongside existing styles

### CSS Loading Order

Components are loaded in this order in `_Layout.cshtml`:

```html
<!-- Base styles and variables (load first) -->
<link rel="stylesheet" href="~/css/base/_variables.css" />
<link rel="stylesheet" href="~/css/base/buttons.css" />
<link rel="stylesheet" href="~/css/base/forms.css" />
<link rel="stylesheet" href="~/css/base/cards.css" />
```

---

## Form Components

Location: `/wwwroot/css/base/forms.css`

### Basic Form Structure

```html
<form class="form">
    <div class="form-group">
        <label for="email">Email Address</label>
        <input type="email" id="email" name="email" placeholder="you@example.com">
    </div>

    <div class="form-group">
        <label for="message">Message</label>
        <textarea id="message" name="message" placeholder="Your message here..."></textarea>
    </div>

    <button type="submit" class="btn btn--primary">Submit</button>
</form>
```

### Form Layouts

#### Two-Column Form Row

```html
<div class="form-row">
    <div class="form-group">
        <label for="firstName">First Name</label>
        <input type="text" id="firstName" name="firstName">
    </div>

    <div class="form-group">
        <label for="lastName">Last Name</label>
        <input type="text" id="lastName" name="lastName">
    </div>
</div>
```

**Note**: On mobile (< 768px), `.form-row` automatically becomes single-column.

### Input Types

All standard input types are supported:

```html
<!-- Text inputs -->
<input type="text">
<input type="email">
<input type="password">
<input type="number">
<input type="tel">
<input type="url">

<!-- Date/Time inputs -->
<input type="date">
<input type="time">
<input type="datetime-local">

<!-- Select dropdown -->
<select>
    <option>Option 1</option>
    <option>Option 2</option>
</select>

<!-- Textarea -->
<textarea></textarea>
```

### Validation States

```html
<!-- Valid state -->
<div class="form-group">
    <label for="email">Email</label>
    <input type="email" id="email" class="is-valid">
    <span class="valid-feedback">Looks good!</span>
</div>

<!-- Invalid state -->
<div class="form-group">
    <label for="password">Password</label>
    <input type="password" id="password" class="is-invalid">
    <span class="invalid-feedback">Password must be at least 8 characters.</span>
</div>
```

### Checkboxes and Radio Buttons

```html
<div class="form-check">
    <input type="checkbox" id="terms">
    <label for="terms">I agree to the terms and conditions</label>
</div>

<div class="form-check">
    <input type="radio" id="option1" name="options">
    <label for="option1">Option 1</label>
</div>
```

### File Upload

```html
<div class="form-group">
    <label for="file">Upload Document</label>
    <label for="file" class="form-file-upload">
        <svg class="form-file-upload__icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12"/>
        </svg>
        <span class="form-file-upload__text">Click to upload or drag and drop</span>
        <span class="form-file-upload__hint">PDF, DOC, DOCX up to 10MB</span>
    </label>
    <input type="file" id="file" name="file">
</div>
```

### Help Text

```html
<div class="form-group">
    <label for="username">Username</label>
    <input type="text" id="username">
    <span class="form-help">Must be 3-20 characters, letters and numbers only.</span>
</div>
```

### Disabled State

```html
<input type="text" disabled>
<select disabled></select>
<textarea disabled></textarea>
```

---

## Card Components

Location: `/wwwroot/css/base/cards.css`

### Basic Card

```html
<div class="card">
    <div class="card__header">
        <h3 class="card__title">Card Title</h3>
        <p class="card__subtitle">Optional subtitle or description</p>
    </div>

    <div class="card__body">
        <p class="card__text">Card content goes here.</p>
    </div>

    <div class="card__footer">
        <a href="#" class="btn btn--primary">Action</a>
    </div>
</div>
```

### Card Sizes

```html
<!-- Small card -->
<div class="card card--sm">...</div>

<!-- Medium card (default) -->
<div class="card">...</div>

<!-- Large card -->
<div class="card card--lg">...</div>
```

### Card Variants

```html
<!-- Flat card (no shadow, just border) -->
<div class="card card--flat">...</div>

<!-- Elevated card (stronger shadow) -->
<div class="card card--elevated">...</div>

<!-- Interactive card (clickable with pointer cursor) -->
<div class="card card--interactive">...</div>

<!-- Static card (no hover effects) -->
<div class="card card--static">...</div>
```

### Card with Image

```html
<div class="card">
    <div class="card__image card__image--md">
        <img src="/images/example.jpg" alt="Description">
    </div>

    <div class="card__content">
        <h3 class="card__title">Card Title</h3>
        <p class="card__description">Card description text.</p>
    </div>
</div>
```

**Image size variants**: `.card__image--sm` (150px), `.card__image--md` (200px), `.card__image--lg` (300px)

### Card with Badge

```html
<div class="card">
    <div class="card__header">
        <span class="card__badge card__badge--success">Published</span>
        <h3 class="card__title">Story Title</h3>
    </div>
    <div class="card__body">
        <p class="card__text">Story content...</p>
    </div>
</div>
```

**Badge variants**:
- `.card__badge--primary` (brand purple)
- `.card__badge--secondary` (accent pink)
- `.card__badge--success` (green)
- `.card__badge--warning` (yellow)
- `.card__badge--error` (red)
- `.card__badge--info` (purple)

### Card with Tags

```html
<div class="card">
    <div class="card__tags">
        <span class="card__tag">Mental Health</span>
        <span class="card__tag">Recovery</span>
        <span class="card__tag card__tag--sm">Featured</span>
    </div>
    <h3 class="card__title">Title</h3>
    <p class="card__description">Description...</p>
</div>
```

### Card with Meta Information

```html
<div class="card">
    <h3 class="card__title">Event Title</h3>

    <div class="card__meta">
        <div class="card__meta-item">
            <svg class="card__meta-icon"><!-- calendar icon --></svg>
            <span>15 Dec 2025</span>
        </div>
        <div class="card__meta-item">
            <svg class="card__meta-icon"><!-- location icon --></svg>
            <span>Cape Town</span>
        </div>
    </div>

    <p class="card__description">Event description...</p>
</div>
```

### Stat Card (Dashboards)

```html
<div class="stat-card">
    <div class="stat-card__icon">
        <svg><!-- icon --></svg>
    </div>
    <div class="stat-card__content">
        <p class="stat-card__label">Total Stories</p>
        <h3 class="stat-card__value">247</h3>
    </div>
</div>
```

### Action Card (Dashboards)

```html
<a href="/stories/create" class="action-card">
    <div class="action-card__icon">
        <svg><!-- plus icon --></svg>
    </div>
    <h4 class="action-card__title">Create New Story</h4>
</a>
```

### Card Grids

```html
<!-- Auto-fill grid (responsive) -->
<div class="cards-grid">
    <div class="card">...</div>
    <div class="card">...</div>
    <div class="card">...</div>
</div>

<!-- Fixed 2-column grid -->
<div class="cards-grid cards-grid--2">...</div>

<!-- Fixed 3-column grid -->
<div class="cards-grid cards-grid--3">...</div>

<!-- Fixed 4-column grid -->
<div class="cards-grid cards-grid--4">...</div>
```

**Note**: Grids automatically become single-column on mobile.

---

## Button Components

Location: `/wwwroot/css/base/buttons.css`

### Basic Buttons

Buttons already exist in the project. Use them with cards and forms:

```html
<button class="btn btn--primary">Primary Action</button>
<button class="btn btn--secondary">Secondary Action</button>
<button class="btn btn--outline">Outline Button</button>
```

---

## Migration Guide

### Migrating Forms

**Before:**
```html
<div class="contact-form__field">
    <label class="contact-form__label">Name</label>
    <input type="text" class="contact-form__input">
</div>
```

**After:**
```html
<div class="form-group">
    <label>Name</label>
    <input type="text">
</div>
```

**Benefits:**
- Removes view-specific CSS classes
- Consistent styling across all forms
- Automatic responsive behaviour

### Migrating Cards

**Before:**
```html
<div class="story-card">
    <img class="story-card__image" src="...">
    <div class="story-card__content">
        <h3 class="story-card__title">Title</h3>
        <p class="story-card__description">Description</p>
    </div>
</div>
```

**After:**
```html
<div class="card">
    <div class="card__image card__image--md">
        <img src="...">
    </div>
    <div class="card__content">
        <h3 class="card__title">Title</h3>
        <p class="card__description">Description</p>
    </div>
</div>
```

**Benefits:**
- Consistent card styling site-wide
- Built-in hover effects and transitions
- Responsive by default

### Gradual Migration Strategy

1. **Start with new features**: Use the component system for all new pages/features
2. **Migrate high-traffic pages**: Contact form, Sign-up form, Home page cards
3. **Migrate similar components together**: All story cards, then all event cards, etc.
4. **Test thoroughly**: Visual testing on desktop, tablet, and mobile
5. **Remove old CSS**: Once migrated, delete the old component-specific CSS

### Testing Checklist

After migrating a component:

- [ ] Desktop view (1440px+)
- [ ] Tablet view (768px-1024px)
- [ ] Mobile view (< 768px)
- [ ] Hover states work
- [ ] Focus states visible (keyboard navigation)
- [ ] Form validation displays correctly
- [ ] All interactive elements clickable/tappable
- [ ] No layout shifts or broken spacing

---

## Best Practices

### 1. Always Use CSS Variables

✅ **Good:**
```css
.custom-component {
    padding: var(--spacing-lg);
    color: var(--color-text-dark);
    border-radius: var(--radius-md);
}
```

❌ **Bad:**
```css
.custom-component {
    padding: 1.5rem;
    color: #0C0C20;
    border-radius: 0.625rem;
}
```

### 2. Prefer Base Classes Over Custom Styles

✅ **Good:**
```html
<div class="card card--elevated">
    <h3 class="card__title">Title</h3>
</div>
```

❌ **Bad:**
```html
<div class="my-custom-card">
    <h3 class="my-custom-title">Title</h3>
</div>
```

### 3. Use Modifiers for Variations

✅ **Good:**
```html
<div class="card card--sm card--flat">...</div>
<span class="card__badge card__badge--success">...</span>
```

❌ **Bad:**
```html
<div class="small-flat-card">...</div>
<span class="green-badge">...</span>
```

### 4. Keep View-Specific Styles Minimal

If you need custom styling, extend the base components:

```css
/* stories.css */
.story-card {
    /* Extends base .card */
}

.story-card .card__title {
    /* Custom title styling for stories only */
    font-size: 2rem;
}
```

### 5. Maintain Semantic HTML

```html
<!-- Good: Semantic and accessible -->
<article class="card">
    <header class="card__header">
        <h2 class="card__title">Title</h2>
    </header>
    <div class="card__body">
        <p>Content</p>
    </div>
    <footer class="card__footer">
        <a href="#" class="btn">Read More</a>
    </footer>
</article>
```

### 6. Add ARIA Labels for Accessibility

```html
<button class="btn btn--primary" aria-label="Submit contact form">
    <svg aria-hidden="true"><!-- icon --></svg>
    Submit
</button>

<div class="card card--interactive" role="article" aria-labelledby="story-title-123">
    <h3 id="story-title-123" class="card__title">Story Title</h3>
</div>
```

### 7. Use Form Validation Properly

```html
<div class="form-group">
    <label for="email">Email Address</label>
    <input
        type="email"
        id="email"
        name="email"
        required
        aria-describedby="email-error"
        class="is-invalid">
    <span id="email-error" class="invalid-feedback" role="alert">
        Please enter a valid email address.
    </span>
</div>
```

---

## CSS Variable Reference

### Colours

```css
/* Brand Colours */
--brand-700: #2F1648;
--brand-600: #3A1F53;
--brand-300: #E9D5EB;

/* Neutral Colours */
--neutral-900: #0C0C20;
--neutral-400: #4B5563;
--neutral-100: #F6FAF9;

/* Accent Colours */
--accent-teal-500: #52B69A;
--accent-100: #FDF5ED;
--accent-200: #CA9F9D;

/* Semantic Colours */
--color-success: #52B69A;
--color-error: #D32F2F;
--color-warning: #FFA92C;
--color-info: #2F1648;
```

### Spacing

```css
--spacing-xs: 0.25rem;   /* 4px */
--spacing-sm: 0.5rem;    /* 8px */
--spacing-md: 1rem;      /* 16px */
--spacing-lg: 1.5rem;    /* 24px */
--spacing-xl: 2rem;      /* 32px */
--spacing-2xl: 3rem;     /* 48px */
--spacing-3xl: 4rem;     /* 64px */
```

### Border Radius

```css
--radius-sm: 0.375rem;   /* 6px */
--radius-md: 0.625rem;   /* 10px */
--radius-lg: 0.875rem;   /* 14px */
--radius-xl: 1rem;       /* 16px */
--radius-full: 50%;
```

### Shadows

```css
--shadow-soft-sm: 0 2px 4px rgba(12, 12, 32, 0.10);
--shadow-soft-md: 0 4px 8px rgba(12, 12, 32, 0.10);
--shadow-soft-lg: 0 4px 8px rgba(12, 12, 32, 0.15);
--shadow-card: 0 4px 12px rgba(47, 22, 72, 0.08);
```

### Typography

```css
/* Font Families */
--font-family-base: 'Inter', sans-serif;
--font-family-heading: 'Poppins', sans-serif;

/* Font Weights */
--font-weight-regular: 400;
--font-weight-medium: 500;
--font-weight-semibold: 600;
--font-weight-bold: 700;

/* Font Sizes */
--font-size-display: 4.5rem;     /* 72px */
--font-size-h1: 3.5rem;          /* 56px */
--font-size-h2: 3rem;            /* 48px */
--font-size-h3: 2rem;            /* 32px */
--font-size-h4: 1.5rem;          /* 24px */
--font-size-h5: 1.25rem;         /* 20px */
--font-size-body-lg: 1.125rem;   /* 18px */
--font-size-body: 1rem;          /* 16px */
--font-size-body-sm: 0.875rem;   /* 14px */
```

### Transitions

```css
--transition-fast: 0.15s ease;
--transition-base: 0.2s ease;
--transition-slow: 0.3s ease;
```

---

## Support

For questions or issues with the component system:

1. Check this guide first
2. Review the CSS files in `/wwwroot/css/base/`
3. Look at existing implementations in the views
4. Test in multiple browsers and devices

---

**Last Updated**: 2025-12-01
**Version**: 1.0.0