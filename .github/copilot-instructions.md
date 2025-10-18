# 🧭 GitHub Copilot Instructions — ASP.NET MVC (.NET 8) Front-End Standards

## Purpose
These guidelines ensure consistent, clean, and maintainable front-end structure for this ASP.NET MVC (.NET 8) application with 40+ views or UI states.

Copilot must review and follow this document **before generating any Razor views, CSS, or front-end code**.

---

## 1. General Structure
- Each Razor View (`.cshtml`) represents one logical UI page or state.
- Use **partial views** for reusable UI components (headers, footers, modals, cards, etc.).
- Keep **one CSS file per major view or component** under `/wwwroot/css/views/`.
- Follow **BEM (Block–Element–Modifier)** naming convention for CSS classes:
    - Example: `.card`, `.card__header`, `.card--highlighted`
- Avoid inline styles and `<style>` tags in views.

---

## 2. CSS Guidelines
- Use **BEM** and **component-based organization**.
- Keep each CSS file under 300 lines by splitting large modules logically.
- Use a global `_variables.css` for shared color, font, and spacing tokens.
- Follow this CSS property order:
    1. Positioning (position, top, left, z-index)
    2. Box model (display, width, height, padding, margin)
    3. Typography (font, color, line-height)
    4. Visual (background, border, shadows)
    5. Animation or transitions
- Use responsive units (`rem`, `%`, `minmax`) instead of fixed pixels.
- Maintain consistent spacing and typography hierarchy.

---

## 3. File Naming Convention
| Type | Naming Convention | Example |
|------|-------------------|----------|
| Razor View | PascalCase | `FarmerDashboard.cshtml` |
| CSS File | kebab-case | `farmer-dashboard.css` |
| ViewModel | PascalCase | `FarmerDashboardViewModel.cs` |

**Recommended structure:**

/Views
/Farmers
FarmerDashboard.cshtml
/Employees
EmployeeDashboard.cshtml
/wwwroot
/css
/views
farmer-dashboard.css
employee-dashboard.css
/js
/views
farmer-dashboard.js

---

## 4. Razor View Conventions
- Keep logic minimal in `.cshtml` files. Use ViewModels for data shaping.
- Use **semantic HTML** elements (`<section>`, `<header>`, `<footer>`, `<nav>`).
- Wrap each view in a unique BEM block container:
  ```html
  <div class="farmer-dashboard">
      ...
  </div>

	•	Use partials for reusable UI patterns (e.g., cards, modals, lists).

⸻

5. Accessibility & Responsiveness
   •	Test all views on mobile, tablet, and desktop.
   •	Ensure keyboard navigation (tab order, focus states).
   •	Include aria-* attributes where needed.
   •	Keep layouts responsive using CSS Grid or Flexbox.

⸻

6. Example Copilot Prompt for Implementation

When prompting Copilot to create a new view or component, prepend this instruction:

“Generate the Razor view and CSS for the [page/component name] using BEM naming, modular CSS, and semantic HTML. Keep layout responsive, consistent, and minimal. Use partial views for reusable UI sections and follow all project conventions defined in COPILOT_INSTRUCTIONS.md.”

⸻

7. Reminder for All Contributors
   •	Always review generated HTML/CSS for adherence to these standards.
   •	Keep file organization modular and consistent.
   •	Prefer clarity and maintainability over brevity.

⸻

This file should remain in the repository root so GitHub Copilot references it automatically when suggesting front-end code.

