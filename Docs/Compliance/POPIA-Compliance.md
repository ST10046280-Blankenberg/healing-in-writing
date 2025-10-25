# POPIA Compliance Documentation

## Overview

This document outlines how the Healing In Writing application complies with the Protection of Personal Information Act (POPIA) 4 of 2013, South Africa's data protection legislation.

## Personal Information Collected

### User Accounts
- **First Name** - Used for personalisation and communication
- **Last Name** - Used for personalisation and communication
- **Email Address** - Used for authentication, communication, and account recovery
- **Password** - Stored as a cryptographic hash (PBKDF2 with HMACSHA256)
- **Last Login Date** - Used for security monitoring and account management
- **Account Creation Date** - Used for record-keeping and analytics
- **Account Status** - Used to manage active/inactive accounts

### User-Generated Content
- **Stories** - User submissions containing potentially sensitive personal experiences
- **Story Metadata** - Creation date, modification date, status (draft/published/pending)
- **Anonymous Flag** - Indicates whether user wants to publish anonymously

### Technical Data
- **IP Addresses** - Temporarily stored for rate limiting and security (not permanently logged)
- **Authentication Cookies** - Session management (expire after 24 hours)
- **Browser Information** - Collected via standard HTTP headers for security

### Future Data Points
- Event RSVPs
- Volunteer information
- Donation records

## Lawful Processing (POPIA Condition 1)

### Legal Basis for Processing

1. **Consent** - Users explicitly consent to data processing during registration
   - Consent checkbox required before account creation
   - Links to Terms of Service and Privacy Policy provided
   - Users must actively accept terms

2. **Contractual Necessity** - Processing required to provide the service
   - Authentication requires email and password
   - Displaying user name requires First Name and Last Name

3. **Legitimate Interest** - Security and fraud prevention
   - Rate limiting requires IP address tracking
   - Security logging for incident response

## Purpose Specification (POPIA Condition 2)

### Data Usage Purposes

**Explicitly Stated Purposes:**
- User authentication and account management
- Personalised experience (displaying user's name)
- Content management (stories, events, volunteer work)
- Security monitoring and fraud prevention
- Communication regarding account status
- Service improvement and analytics

**Not Used For:**
- Third-party marketing
- Sale to data brokers
- Profiling or automated decision-making

## Further Processing Limitation (POPIA Condition 3)

Data collected will only be used for the purposes specified above. If we need to use data for a new purpose incompatible with the original purpose, we will:

1. Notify users via email
2. Request explicit consent for the new purpose
3. Provide option to opt-out
4. Document the new purpose and legal basis

## Information Quality (POPIA Condition 4)

### Data Accuracy

- Users can update their First Name, Last Name via account settings (TO BE IMPLEMENTED)
- Email updates require verification via confirmation email (TO BE IMPLEMENTED)
- Stale accounts (inactive for 2+ years) will be flagged for review

### Data Minimisation

We only collect data necessary for service operation:
- ✅ First/Last names - needed for personalisation
- ✅ Email - needed for authentication
- ✅ Password - needed for security
- ✅ Timestamps - needed for security and management
- ❌ Address - not collected (not necessary)
- ❌ Phone number - not collected (not necessary)
- ❌ ID numbers - not collected (not necessary)

## Openness (POPIA Condition 5)

### Transparency Measures

1. **Privacy Policy** - Accessible at `/Home/Privacy`
   - Details what data is collected
   - Explains why it's collected
   - Describes how it's used
   - Lists user rights
   - Provides contact information

2. **Terms of Service** - Accessible at `/Home/Terms`
   - Defines user responsibilities
   - Outlines service limitations
   - Describes account termination

3. **Consent during Registration** - Users must actively accept terms before account creation

## Security Safeguards (POPIA Condition 6)

### Technical Measures Implemented

1. **Authentication Security**
   - Passwords hashed with PBKDF2 (HMACSHA256)
   - Minimum password requirements (8 chars, complexity)
   - Account lockout after 5 failed attempts (5-minute lockout)
   - Email verification required before login

2. **Session Security**
   - HttpOnly cookies (prevents JavaScript access)
   - SameSite cookie policy (CSRF protection)
   - Secure cookie flag (HTTPS only, when configured)
   - 24-hour session expiration with sliding renewal

3. **Network Security**
   - CSRF tokens on all state-changing operations
   - XSS protection via Content Security Policy
   - Clickjacking protection (X-Frame-Options)
   - Rate limiting (prevents brute force attacks)

4. **Access Control**
   - Role-based authorisation (User, Volunteer, Admin)
   - Resource ownership verification
   - Admin-only endpoints protected

5. **Data Protection**
   - Database files excluded from version control
   - SQL injection prevention (parameterised queries)
   - Input sanitisation for user content

### Organisational Measures

1. **Access Control**
   - Admin accounts require explicit role assignment
   - No default admin accounts in production
   - Regular review of user roles and permissions

2. **Audit Logging**
   - Security events logged (login attempts, lockouts)
   - Admin actions logged
   - Logs stored securely with restricted access

3. **Incident Response** - See separate Incident Response Plan

## Data Subject Participation (POPIA Condition 7)

### User Rights Under POPIA

Users have the following rights regarding their personal information:

#### 1. Right to Access

**Implementation Status:** TO BE IMPLEMENTED

Users can request access to their personal information.

**Required Implementation:**
```csharp
// Controller action to download personal data
[Authorize]
[HttpGet]
public async Task<IActionResult> DownloadMyData()
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var userData = await _userService.GetUserDataExportAsync(userId);

    var json = JsonSerializer.Serialize(userData, new JsonSerializerOptions
    {
        WriteIndented = true
    });

    return File(
        Encoding.UTF8.GetBytes(json),
        "application/json",
        $"my-data-{DateTime.UtcNow:yyyy-MM-dd}.json"
    );
}
```

#### 2. Right to Rectification

**Implementation Status:** TO BE IMPLEMENTED

Users can correct inaccurate or incomplete information.

**Required Implementation:**
- Account settings page to update First Name, Last Name
- Email change with verification
- Profile update functionality

#### 3. Right to Erasure ("Right to be Forgotten")

**Implementation Status:** TO BE IMPLEMENTED

Users can request deletion of their account and associated data.

**Required Implementation:**
```csharp
[Authorize]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteMyAccount(string confirmPassword)
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var user = await _userManager.FindByIdAsync(userId);

    // Verify password before deletion
    var passwordValid = await _userManager.CheckPasswordAsync(user, confirmPassword);
    if (!passwordValid)
        return BadRequest("Invalid password");

    // Delete user and associated data
    await _userService.DeleteUserAndDataAsync(userId);

    // Sign out
    await _signInManager.SignOutAsync();

    TempData["Message"] = "Your account has been permanently deleted.";
    return RedirectToAction("Index", "Home");
}
```

**Data Retention After Deletion:**
- User account: Permanently deleted
- Stories: Anonymised (author link removed) or deleted per user preference
- Logs: Retained for 90 days for security purposes, then anonymised
- Backups: Purged within 30 days

#### 4. Right to Object

Users can object to processing of their personal information for specific purposes.

**Current Implementation:**
- Users can opt for anonymous story submission
- Users control what information they share

**TO BE IMPLEMENTED:**
- Opt-out from marketing emails (when/if implemented)
- Object to analytics tracking (when/if implemented)

#### 5. Right to Data Portability

**Implementation Status:** TO BE IMPLEMENTED

Users can request their data in a structured, machine-readable format.

**Format:** JSON export containing:
- Account information
- All stories created
- Event RSVPs (when implemented)
- Volunteer records (when implemented)

## Accountability (POPIA Condition 8)

### Responsible Party

**Organisation:** Healing In Writing
**Information Officer:** [TO BE ASSIGNED]
**Contact:** [privacy@healinginwriting.org.za]

### Operator

If third-party services are engaged to process personal information, written agreements will be established ensuring:
- Operator processes only on instruction
- Operator implements appropriate security measures
- Operator maintains confidentiality

### Documentation

1. **This POPIA Compliance Document** - Records compliance measures
2. **Privacy Policy** - Public-facing transparency document
3. **Data Retention Policy** - Defines how long data is kept
4. **Incident Response Plan** - Procedures for data breaches
5. **Security Audit Log** - Regular security reviews

## Data Breach Notification

### Notification Requirements

Under POPIA, if a data breach occurs that is likely to cause harm, we must notify:

1. **Information Regulator** - Within a reasonable time after becoming aware
2. **Affected Users** - As soon as reasonably possible

### Breach Response Process

1. **Detection** - Security monitoring alerts on suspicious activity
2. **Assessment** - Determine scope, impact, and affected users
3. **Containment** - Stop the breach, secure systems
4. **Notification** - Inform regulator and affected users within required timeframe
5. **Remediation** - Fix vulnerabilities, improve security
6. **Documentation** - Record breach details, actions taken, lessons learnt

### What to Include in Notification

- Description of the breach
- Type of personal information affected
- Potential consequences
- Measures taken to address the breach
- Recommended actions for affected users
- Contact information for queries

## Data Retention Policy

### Retention Periods

| Data Type | Retention Period | Justification |
|-----------|-----------------|---------------|
| Active user accounts | Until account deletion | Contractual necessity |
| Inactive accounts (no login) | 24 months, then flagged for review | Legitimate interest |
| Deleted account audit trail | 90 days, then anonymised | Compliance and security |
| Published stories | Indefinite (anonymised if user deleted) | Service purpose |
| Draft/pending stories | Deleted with user account | User preference |
| Security logs | 12 months, then archived | Security and compliance |
| Authentication cookies | 24 hours | Session management |
| Rate limiting data | In-memory only, cleared on restart | Security (temporary) |

### Disposal

When retention periods expire:
1. **Soft Delete** - Mark as deleted, remove from active use
2. **Hard Delete** - Permanent removal from database (after 30 days)
3. **Anonymisation** - Replace identifying information with anonymous ID
4. **Backup Purge** - Remove from backups within 30 days

## Cross-Border Data Transfer

**Current Status:** All data stored in South Africa (SQLite database on local server)

**If Cloud Services Used:**
- Ensure cloud provider is POPIA-compliant
- Data residency in South Africa where possible
- If data transferred abroad, ensure adequate safeguards
- Inform users of international transfers

## Automated Decision-Making

**Current Status:** No automated decision-making or profiling implemented

**If Implemented in Future:**
- Obtain explicit consent
- Provide information about logic involved
- Allow users to object or request human review

## Children's Data

**Current Policy:** Service is intended for adults (18+)

**Measures:**
- Terms of Service state age requirement
- No specific collection of children's data
- If child account discovered, will be deleted

## Implementation Checklist

### ✅ Completed
- [x] Password hashing
- [x] HTTPS redirection (when SSL configured)
- [x] CSRF protection
- [x] XSS protection
- [x] Session security
- [x] Rate limiting
- [x] Admin role protection
- [x] Privacy consent on registration
- [x] Security logging
- [x] Database file protection

### ⚠️ To Be Implemented
- [ ] Privacy Policy page content
- [ ] Terms of Service page content
- [ ] User account settings (update name, email)
- [ ] Data export functionality (download my data)
- [ ] Account deletion functionality
- [ ] Story anonymisation on account deletion
- [ ] Email notifications (breach, account changes)
- [ ] Assign Information Officer
- [ ] Data retention automation (archive old accounts)
- [ ] Incident response procedures documentation
- [ ] Staff training on POPIA compliance

## Review Schedule

This POPIA compliance document should be reviewed:
- **Annually** - Regular compliance audit
- **When new features added** - Assess privacy impact
- **When legislation changes** - Update to reflect new requirements
- **After incidents** - Review and improve based on lessons learnt

## Contact

For privacy queries or to exercise POPIA rights:
- **Email:** [privacy@healinginwriting.org.za]
- **Information Officer:** [Name]
- **Address:** [Physical address]

## References

- [POPIA Full Text](https://popia.co.za/)
- [Information Regulator South Africa](https://inforegulator.org.za/)
- [POPIA Code of Conduct](https://inforegulator.org.za/codes-of-conduct/)
