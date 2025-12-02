# Spacing Audit Guide

A guide for replacing hardcoded spacing values with CSS variables across the project.

## Overview

This guide helps maintain consistency by ensuring all spacing uses CSS variables from the design system instead of hardcoded rem values.

## Why Replace Hardcoded Spacing?

1. **Consistency**: All spacing follows the design system
2. **Maintainability**: Update spacing globally by changing variables
3. **Flexibility**: Easy to adjust responsive spacing
4. **Standards**: Follows best practices

## Available Spacing Variables

From `wwwroot/css/base/_variables.css`:

```css
--spacing-xs: 0.25rem;   /* 4px */
--spacing-sm: 0.5rem;    /* 8px */
--spacing-md: 1rem;      /* 16px */
--spacing-lg: 1.5rem;    /* 24px */
--spacing-xl: 2rem;      /* 32px */
--spacing-2xl: 3rem;     /* 48px */
--spacing-3xl: 4rem;     /* 64px */
```

## Conversion Reference

Quick reference for common hardcoded values:

| Hardcoded Value | CSS Variable | Use Case |
|----------------|--------------|----------|
| `0.25rem` | `var(--spacing-xs)` | Tiny gaps, small spacing |
| `0.5rem` | `var(--spacing-sm)` | Small gaps between items |
| `1rem` | `var(--spacing-md)` | Standard spacing, default gaps |
| `1.5rem` | `var(--spacing-lg)` | Larger spacing, section gaps |
| `2rem` | `var(--spacing-xl)` | Large padding, card spacing |
| `3rem` | `var(--spacing-2xl)` | Extra large spacing |
| `4rem` | `var(--spacing-3xl)` | Maximum spacing, hero sections |

### Non-Standard Values

Some values don't map exactly to spacing variables:

| Hardcoded Value | Recommendation |
|----------------|----------------|
| `0.75rem` (12px) | Round to `var(--spacing-md)` (1rem) or keep if critical |
| `1.25rem` (20px) | Round to `var(--spacing-lg)` (1.5rem) or keep if critical |
| `2.5rem` (40px) | Use `var(--spacing-2xl)` (3rem) or keep if critical |

**Rule**: Only keep non-standard values if they're critical to the design (e.g., specific height requirements).

## Files Requiring Updates

26 files contain hardcoded spacing values:

### Base Components (Already Updated)
- ✅ `wwwroot/css/base/buttons.css` - Complete
- ✅ `wwwroot/css/base/cards.css` - Complete
- ✅ `wwwroot/css/base/forms.css` - Complete

### View Files (Pending)
- ⏳ `wwwroot/css/views/contact.css` - Partially complete
- ⏸️ `wwwroot/css/views/home.css`
- ⏸️ `wwwroot/css/views/about.css`
- ⏸️ `wwwroot/css/views/donate.css`
- ⏸️ `wwwroot/css/views/resources.css`
- ⏸️ `wwwroot/css/views/auth.css`
- ⏸️ `wwwroot/css/views/gallery.css`
- ⏸️ `wwwroot/css/views/gallery-details.css`
- ⏸️ `wwwroot/css/views/privacy-terms.css`
- ⏸️ `wwwroot/css/views/admin-site-settings.css`
- ⏸️ `wwwroot/css/views/manage-events.css`
- ⏸️ `wwwroot/css/views/volunteer-dashboard.css`
- ⏸️ `wwwroot/css/views/event-registrations.css`

### Shared Components (Pending)
- ⏸️ `wwwroot/css/shared/header.css`
- ⏸️ `wwwroot/css/shared/footer.css`
- ⏸️ `wwwroot/css/shared/subnav.css`
- ⏸️ `wwwroot/css/shared/dashboard-subnav.css`
- ⏸️ `wwwroot/css/shared/alerts.css`
- ⏸️ `wwwroot/css/shared/toast.css`
- ⏸️ `wwwroot/css/shared/lightbox.css`
- ⏸️ `wwwroot/css/shared/empty-state.css`

### Other
- ⏸️ `wwwroot/css/signup.css`

## Step-by-Step Process

### 1. Choose a File

Start with high-traffic pages or components used across the site:
- Contact page (partially done)
- Home page
- Header/Footer (shared components)

### 2. Identify Hardcoded Values

Search for patterns:
```bash
grep -n "padding:\s*\d" filename.css
grep -n "margin:\s*\d" filename.css
grep -n "gap:\s*\d" filename.css
```

### 3. Replace Values

**Before:**
```css
.contact {
    padding: 2rem 4rem;
    gap: 2rem;
}

.contact-form {
    gap: 1.5rem;
}

.contact-card {
    padding: 2rem;
    margin-bottom: 1rem;
}
```

**After:**
```css
.contact {
    padding: var(--spacing-xl) var(--spacing-3xl);
    gap: var(--spacing-xl);
}

.contact-form {
    gap: var(--spacing-lg);
}

.contact-card {
    padding: var(--spacing-xl);
    margin-bottom: var(--spacing-md);
}
```

### 4. Test Visually

After updating a file:
1. Build the project: `dotnet build`
2. Run the application
3. Visually inspect the page at multiple breakpoints:
   - Desktop (1440px+)
   - Tablet (768px-1024px)
   - Mobile (< 768px)
4. Check for layout breaks or spacing issues

### 5. Responsive Breakpoints

Pay special attention to responsive overrides:

**Before:**
```css
@media (max-width: 768px) {
    .contact {
        padding: 1.5rem 1rem;
        gap: 1.5rem;
    }
}
```

**After:**
```css
@media (max-width: 768px) {
    .contact {
        padding: var(--spacing-lg) var(--spacing-md);
        gap: var(--spacing-lg);
    }
}
```

## Common Patterns

### Pattern 1: Container Padding

```css
/* Before */
.container {
    padding: 2rem 4rem;
}

/* After */
.container {
    padding: var(--spacing-xl) var(--spacing-3xl);
}
```

### Pattern 2: Flex/Grid Gaps

```css
/* Before */
.grid {
    gap: 2rem;
}

/* After */
.grid {
    gap: var(--spacing-xl);
}
```

### Pattern 3: Card/Component Padding

```css
/* Before */
.card {
    padding: 2rem;
}

/* After */
.card {
    padding: var(--spacing-xl);
}
```

### Pattern 4: Section Spacing

```css
/* Before */
.section {
    margin-bottom: 4rem;
}

/* After */
.section {
    margin-bottom: var(--spacing-3xl);
}
```

## Special Cases

### Heights & Widths

For specific height/width requirements, hardcoded values are often acceptable:

```css
/* OK to keep hardcoded */
.button {
    min-height: 3rem;
    height: 3.125rem;
}

.icon {
    width: 1.25rem;
    height: 1.25rem;
}
```

### Line Heights

Line heights are typography values, not spacing:

```css
/* Keep as-is or use line-height variables */
.text {
    line-height: 1.5rem;  /* OK */
    line-height: var(--line-height-normal);  /* Better */
}
```

### Border Radius

Use radius variables instead:

```css
/* Before */
.element {
    border-radius: 0.75rem;
}

/* After */
.element {
    border-radius: var(--radius-lg);
}
```

## Testing Checklist

After updating each file:

- [ ] Build succeeds (`dotnet build`)
- [ ] Visual check on desktop
- [ ] Visual check on tablet
- [ ] Visual check on mobile
- [ ] No layout breaks
- [ ] Spacing looks consistent with design
- [ ] Hover states still work
- [ ] Interactive elements still function

## Priority Order

Recommended order for updates:

1. **High-Traffic Pages** (Impact: High)
   - Home (`home.css`)
   - Contact (`contact.css`) - Partially done
   - About (`about.css`)

2. **Shared Components** (Impact: High)
   - Header (`header.css`)
   - Footer (`footer.css`)
   - Navigation (`subnav.css`, `dashboard-subnav.css`)

3. **Feature Pages** (Impact: Medium)
   - Resources (`resources.css`)
   - Gallery (`gallery.css`)
   - Donate (`donate.css`)

4. **Admin/Dashboard** (Impact: Medium)
   - Admin Site Settings (`admin-site-settings.css`)
   - Volunteer Dashboard (`volunteer-dashboard.css`)
   - Event Management (`manage-events.css`)

5. **Utility Components** (Impact: Low)
   - Alerts (`alerts.css`)
   - Toast (`toast.css`)
   - Lightbox (`lightbox.css`)

## Example: Complete File Update

Here's a complete example from `contact.css`:

### Before
```css
.contact {
    padding: 2rem 4rem;
    gap: 2rem;
}

.contact-grid {
    gap: 2rem;
}

.contact-column {
    gap: 2rem;
}

.contact-card {
    padding: 2rem;
}
```

### After
```css
.contact {
    padding: var(--spacing-xl) var(--spacing-3xl);
    gap: var(--spacing-xl);
}

.contact-grid {
    gap: var(--spacing-xl);
}

.contact-column {
    gap: var(--spacing-xl);
}

.contact-card {
    padding: var(--spacing-xl);
}
```

## Automation Opportunities

For bulk updates, consider using find-and-replace with regex:

**Find**: `padding:\s*(2rem)`
**Replace**: `padding: var(--spacing-xl)`

**Find**: `gap:\s*(1\.5rem)`
**Replace**: `gap: var(--spacing-lg)`

**Note**: Always review automated changes manually before committing.

## Git Workflow

When updating files:

1. Work on one file at a time
2. Test thoroughly after each file
3. Commit with clear messages:
   ```
   refactor: replace hardcoded spacing in contact.css with CSS variables
   ```
4. If a file breaks, revert and investigate

## Support

If unsure about a spacing value:
1. Check the design system in `_variables.css`
2. Look at similar components for consistency
3. When in doubt, test both options visually
4. Prefer standard spacing variables over hardcoded values

---

**Last Updated**: 2025-12-02
**Status**: In Progress (3 of 26 files complete)
**Next Target**: `home.css`