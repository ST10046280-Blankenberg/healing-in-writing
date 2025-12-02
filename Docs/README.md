# Documentation

Welcome to the Healing In Writing documentation. This folder contains all project documentation, guides, and reference materials.

## 📚 Documentation Index

### Getting Started
- **[Main README](../README.md)** - Project overview and setup instructions
- **[ABOUT](ABOUT.md)** - About the Healing In Writing project

### Development Guides
- **[ARCHITECTURE](ARCHITECTURE.md)** - System architecture and technical overview
- **[CONTRIBUTING](CONTRIBUTING.md)** - Contribution guidelines and workflow

### UI & Design System
- **[COMPONENT-SYSTEM-GUIDE](COMPONENT-SYSTEM-GUIDE.md)** - Complete guide to using the standardised component system (forms, cards, buttons)
- **[SPACING-AUDIT-GUIDE](SPACING-AUDIT-GUIDE.md)** - Guide for replacing hardcoded spacing with CSS variables
- **[UI-IMPROVEMENTS-README](UI-IMPROVEMENTS-README.md)** - UI improvement checklist and progress tracking

### Security
- **[SECURITY](SECURITY.md)** - Security policies and vulnerability reporting

---

## 🎨 UI Component System

The project uses a standardised component system with reusable UI components:

### Available Components
1. **Forms** (`wwwroot/css/base/forms.css`)
   - Form groups, labels, inputs, validation states
   - Checkboxes, radio buttons, file uploads
   - [Full documentation](COMPONENT-SYSTEM-GUIDE.md#form-components)

2. **Cards** (`wwwroot/css/base/cards.css`)
   - Basic cards, stat cards, action cards
   - Card grids and layouts
   - [Full documentation](COMPONENT-SYSTEM-GUIDE.md#card-components)

3. **Buttons** (`wwwroot/css/base/buttons.css`)
   - Primary, secondary, size variants
   - [Full documentation](COMPONENT-SYSTEM-GUIDE.md#button-components)

### Design System Variables
All components use CSS variables defined in `wwwroot/css/base/_variables.css`:
- Colours (brand, neutral, accent, semantic)
- Spacing (xs, sm, md, lg, xl, 2xl, 3xl)
- Typography (font families, sizes, weights)
- Shadows, borders, transitions

---

## 🔧 Development Workflow

### Before Making Changes
1. Read [CONTRIBUTING.md](CONTRIBUTING.md) for workflow guidelines
2. Check [ARCHITECTURE.md](ARCHITECTURE.md) to understand system structure
3. Review [COMPONENT-SYSTEM-GUIDE.md](COMPONENT-SYSTEM-GUIDE.md) for UI standards

### When Building UI
1. Use components from `wwwroot/css/base/` instead of custom styles
2. Always use CSS variables for spacing, colours, and typography
3. Follow [SPACING-AUDIT-GUIDE.md](SPACING-AUDIT-GUIDE.md) for spacing standards
4. Test on desktop, tablet, and mobile

### When Reporting Issues
1. Check [SECURITY.md](SECURITY.md) for security vulnerabilities
2. Use GitHub Issues for bugs and feature requests

---

## 📊 Project Status

### Component System
- ✅ Forms - Complete and documented
- ✅ Cards - Complete and documented
- ✅ Buttons - Complete and documented
- ✅ Empty States - Complete
- ⏸️ Other components - In progress

### Spacing Standardisation
- ✅ Base components (buttons, cards, forms)
- ⏳ View files (3 of 23 complete)
- ⏸️ Shared components (0 of 8 complete)
- See [SPACING-AUDIT-GUIDE.md](SPACING-AUDIT-GUIDE.md) for details

### UI Improvements
See [UI-IMPROVEMENTS-README.md](UI-IMPROVEMENTS-README.md) for full checklist

---

## 🆘 Need Help?

### For Contributors
- Read [CONTRIBUTING.md](CONTRIBUTING.md)
- Check existing documentation
- Ask in project discussions

### For Users
- Check [README.md](../README.md) for setup
- Review [ABOUT.md](ABOUT.md) for project info

### For Security Issues
- Follow [SECURITY.md](SECURITY.md) reporting guidelines
- Do not open public issues for vulnerabilities

---

## 📝 Documentation Standards

When adding new documentation:

1. **File Naming**: Use UPPERCASE for major docs, lowercase for specific guides
2. **Location**: Place in `/docs` folder
3. **Format**: Use Markdown (.md)
4. **Structure**: Include table of contents for long documents
5. **Links**: Use relative links to other docs
6. **Updates**: Add "Last Updated" date at bottom
7. **Index**: Update this README when adding new docs

---

**Last Updated**: 2025-12-02