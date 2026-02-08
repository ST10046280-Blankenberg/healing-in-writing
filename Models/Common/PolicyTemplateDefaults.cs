using System.Collections.Generic;

namespace HealingInWriting.Models.Common
{
    public static class PolicyTemplateDefaults
    {
        public static PolicyTemplateData CreatePrivacyDefaults()
        {
            return new PolicyTemplateData
            {
                IntroLastUpdated = "7 November 2025",
                IntroText =
                    "Healing-In-Writing is committed to protecting your privacy and personal information in accordance " +
                    "with the Protection of Personal Information Act (POPIA) 4 of 2013. This Privacy Policy explains what " +
                    "personal information we collect, why we collect it, how we use it, and your rights regarding your information.",
                Sections = new List<PolicyTemplateSection>
                {
                    // Section 1: Information We Collect
                    new PolicyTemplateSection
                    {
                        Title = "1. Information We Collect",
                        Body =
                            "<strong>1.1 Account Information</strong><br>" +
                            "When you create an account with us, we collect:" +
                            "<ul>" +
                            "<li><strong>First Name and Last Name</strong> \u2013 Used for personalisation and communication</li>" +
                            "<li><strong>Email Address</strong> \u2013 Used for authentication, communication, and account recovery</li>" +
                            "<li><strong>Password</strong> \u2013 Stored securely as a cryptographic hash using PBKDF2 with HMACSHA256</li>" +
                            "<li><strong>Account Creation Date</strong> \u2013 Used for record-keeping and security</li>" +
                            "<li><strong>Last Login Date</strong> \u2013 Used for security monitoring and account management</li>" +
                            "</ul>" +
                            "<strong>1.2 User-Generated Content</strong><br>" +
                            "When you use our services, we collect:" +
                            "<ul>" +
                            "<li><strong>Stories</strong> \u2013 Your written submissions, which may contain personal experiences</li>" +
                            "<li><strong>Anonymity Preferences</strong> \u2013 Whether you choose to publish content anonymously</li>" +
                            "<li><strong>Draft Content</strong> \u2013 Temporarily stored in your browser\u2019s local storage</li>" +
                            "<li><strong>Event RSVPs</strong> \u2013 Your event participation preferences</li>" +
                            "</ul>" +
                            "<strong>1.3 Technical Information</strong><br>" +
                            "We automatically collect certain technical information:" +
                            "<ul>" +
                            "<li><strong>IP Addresses</strong> \u2013 Temporarily used for rate limiting and security (not permanently logged)</li>" +
                            "<li><strong>Authentication Cookies</strong> \u2013 Session management (expire after 24 hours)</li>" +
                            "<li><strong>Browser Information</strong> \u2013 Collected via standard HTTP headers for security purposes</li>" +
                            "<li><strong>Security Logs</strong> \u2013 Login attempts, account lockouts, and security events</li>" +
                            "</ul>",
                        Bullets = new List<string>(),
                        HighlightTitle = "What We Don\u2019t Collect",
                        HighlightBody =
                            "We only collect information necessary for our services. We do not collect physical addresses, " +
                            "phone numbers, ID numbers, financial information, or any other personal information beyond " +
                            "what is listed above."
                    },

                    // Section 2: Legal Basis for Processing
                    new PolicyTemplateSection
                    {
                        Title = "2. Legal Basis for Processing Your Information",
                        Body =
                            "Under POPIA, we process your personal information based on the following lawful grounds:" +
                            "<br><br>" +
                            "<strong>2.1 Consent</strong><br>" +
                            "You explicitly consent to data processing during registration by accepting our Terms of Service " +
                            "and Privacy Policy. You may withdraw consent at any time by deleting your account." +
                            "<br><br>" +
                            "<strong>2.2 Contractual Necessity</strong><br>" +
                            "Processing is necessary to provide the services you request, such as creating an account, " +
                            "displaying your name, and managing your content." +
                            "<br><br>" +
                            "<strong>2.3 Legitimate Interest</strong><br>" +
                            "We have legitimate interests in security monitoring, fraud prevention, and service improvement. " +
                            "We balance these interests against your privacy rights and only use data when necessary.",
                        Bullets = new List<string>()
                    },

                    // Section 3: How We Use Your Information
                    new PolicyTemplateSection
                    {
                        Title = "3. How We Use Your Information",
                        Body = "We use your personal information for the following purposes:",
                        Bullets = new List<string>
                        {
                            "Account Management \u2013 Creating and maintaining your account, authentication, password resets",
                            "Service Delivery \u2013 Providing access to our platform, displaying your content, managing events",
                            "Personalisation \u2013 Displaying your name, customising your experience",
                            "Communication \u2013 Sending important service updates, responding to enquiries",
                            "Security \u2013 Monitoring for unauthorised access, preventing fraud and abuse",
                            "Legal Compliance \u2013 Complying with applicable laws and regulations",
                            "Service Improvement \u2013 Understanding how our services are used to make improvements"
                        },
                        HighlightTitle = "What We Don\u2019t Do",
                        HighlightBody =
                            "We do not sell your personal information to third parties. We do not use your data for marketing " +
                            "purposes without your explicit consent. We do not engage in profiling or automated decision-making " +
                            "that significantly affects you."
                    },

                    // Section 4: How We Share Your Information
                    new PolicyTemplateSection
                    {
                        Title = "4. How We Share Your Information",
                        Body =
                            "<strong>4.1 Public Content</strong><br>" +
                            "Content you choose to publish (such as stories marked for publication) will be visible to other " +
                            "users and the public. You control whether your name appears or whether content is published anonymously." +
                            "<br><br>" +
                            "<strong>4.2 Service Providers</strong><br>" +
                            "We use the following third-party services to operate our platform:" +
                            "<ul>" +
                            "<li><strong>Microsoft Azure</strong> \u2013 Database hosting (South Africa-based servers)</li>" +
                            "<li><strong>jsDelivr CDN</strong> \u2013 Content delivery for JavaScript libraries (does not access your personal data)</li>" +
                            "</ul>" +
                            "These service providers are bound by contractual obligations to protect your information and only " +
                            "process data on our instructions." +
                            "<br><br>" +
                            "<strong>4.3 Legal Requirements</strong><br>" +
                            "We may disclose your information if required by law, court order, or to protect our rights, property, " +
                            "or safety, or that of our users or the public." +
                            "<br><br>" +
                            "<strong>4.4 Business Transfers</strong><br>" +
                            "If Healing-In-Writing is involved in a merger, acquisition, or sale of assets, your information may " +
                            "be transferred. We will notify you before your information becomes subject to a different privacy policy.",
                        Bullets = new List<string>()
                    },

                    // Section 5: Data Storage and Security
                    new PolicyTemplateSection
                    {
                        Title = "5. Data Storage and Security",
                        Body =
                            "<strong>5.1 Where We Store Your Data</strong><br>" +
                            "Your personal information is stored on Microsoft Azure servers located in South Africa. This " +
                            "ensures compliance with POPIA\u2019s requirements for data residency and protection." +
                            "<br><br>" +
                            "<strong>5.2 Security Measures</strong><br>" +
                            "We implement comprehensive security measures to protect your information:" +
                            "<ul>" +
                            "<li><strong>Encryption</strong> \u2013 Passwords are hashed using PBKDF2 with HMACSHA256</li>" +
                            "<li><strong>HTTPS</strong> \u2013 All data transmitted between your browser and our servers is encrypted</li>" +
                            "<li><strong>Session Security</strong> \u2013 HttpOnly and Secure cookies prevent unauthorised access</li>" +
                            "<li><strong>Access Controls</strong> \u2013 Role-based access ensures only authorised users can access data</li>" +
                            "<li><strong>Rate Limiting</strong> \u2013 Protection against brute force attacks and automated abuse</li>" +
                            "<li><strong>SQL Injection Prevention</strong> \u2013 Parameterised queries protect against database attacks</li>" +
                            "<li><strong>XSS Protection</strong> \u2013 Content Security Policy and input sanitisation prevent script injection</li>" +
                            "<li><strong>CSRF Protection</strong> \u2013 Anti-forgery tokens on all state-changing operations</li>" +
                            "</ul>" +
                            "<strong>5.3 Account Security</strong><br>" +
                            "Your account is protected by:",
                        Bullets = new List<string>
                        {
                            "Password complexity requirements (minimum 8 characters)",
                            "Account lockout after 5 failed login attempts (5-minute lockout)",
                            "Email verification required before account activation",
                            "Automatic session expiration after 24 hours"
                        }
                    },

                    // Section 6: Data Retention
                    new PolicyTemplateSection
                    {
                        Title = "6. How Long We Keep Your Information",
                        Body =
                            "We retain your personal information only as long as necessary for the purposes outlined in this policy:",
                        Bullets = new List<string>
                        {
                            "Active Accounts \u2013 Retained until you delete your account",
                            "Inactive Accounts \u2013 Flagged for review after 24 months of no login activity",
                            "Published Stories \u2013 Retained indefinitely (anonymised if you delete your account)",
                            "Draft Content \u2013 Deleted when you delete your account",
                            "Security Logs \u2013 Retained for 12 months, then archived",
                            "Deleted Account Records \u2013 Audit trail kept for 90 days, then anonymised",
                            "Authentication Cookies \u2013 Expire after 24 hours",
                            "Rate Limiting Data \u2013 Stored in memory only, cleared on server restart"
                        }
                    },

                    // Section 7: Your Rights Under POPIA
                    new PolicyTemplateSection
                    {
                        Title = "7. Your Rights Under POPIA",
                        Body =
                            "Under the Protection of Personal Information Act, you have the following rights regarding your personal information:" +
                            "<br><br>" +
                            "<strong>7.1 Right to Access</strong><br>" +
                            "You have the right to request access to the personal information we hold about you. You can request " +
                            "a copy of your data by contacting us at <a href=\"mailto:info@healinginwriting.org\">info@healinginwriting.org</a>." +
                            "<br><br>" +
                            "<strong>7.2 Right to Rectification</strong><br>" +
                            "You have the right to correct inaccurate or incomplete information. You can update your first name, " +
                            "last name, and email address through your account settings." +
                            "<br><br>" +
                            "<strong>7.3 Right to Erasure</strong><br>" +
                            "You have the right to request deletion of your account and associated personal information. Upon deletion:" +
                            "<ul>" +
                            "<li>Your account information will be permanently deleted</li>" +
                            "<li>Published stories will be anonymised (author link removed) or deleted per your preference</li>" +
                            "<li>Draft content will be permanently deleted</li>" +
                            "<li>Security logs will be retained for 90 days for legal compliance, then anonymised</li>" +
                            "<li>Backups will be purged within 30 days</li>" +
                            "</ul>" +
                            "To request account deletion, contact us at <a href=\"mailto:info@healinginwriting.org\">info@healinginwriting.org</a>." +
                            "<br><br>" +
                            "<strong>7.4 Right to Object</strong><br>" +
                            "You have the right to object to certain types of processing. You can choose to submit stories anonymously " +
                            "to limit the display of your personal information." +
                            "<br><br>" +
                            "<strong>7.5 Right to Data Portability</strong><br>" +
                            "You have the right to receive your personal information in a structured, machine-readable format (JSON). " +
                            "Contact us to request a data export." +
                            "<br><br>" +
                            "<strong>7.6 Right to Complain</strong><br>" +
                            "If you believe we have not complied with POPIA, you have the right to lodge a complaint with the " +
                            "Information Regulator of South Africa:<br>" +
                            "<strong>Information Regulator South Africa</strong><br>" +
                            "Email: <a href=\"mailto:inforeg@justice.gov.za\">inforeg@justice.gov.za</a><br>" +
                            "Website: <a href=\"https://inforegulator.org.za\" target=\"_blank\" rel=\"noopener noreferrer\">https://inforegulator.org.za</a>",
                        Bullets = new List<string>()
                    },

                    // Section 8: Cookies and Tracking
                    new PolicyTemplateSection
                    {
                        Title = "8. Cookies and Tracking Technologies",
                        Body =
                            "<strong>8.1 Essential Cookies</strong><br>" +
                            "We use essential cookies that are necessary for the operation of our services:" +
                            "<ul>" +
                            "<li><strong>Authentication Cookies</strong> \u2013 Keep you logged in (HttpOnly, Secure, SameSite)</li>" +
                            "<li><strong>Anti-Forgery Tokens</strong> \u2013 Protect against CSRF attacks</li>" +
                            "</ul>" +
                            "These cookies are essential for security and functionality. They cannot be disabled without affecting " +
                            "your ability to use our services." +
                            "<br><br>" +
                            "<strong>8.2 Local Storage</strong><br>" +
                            "We use your browser\u2019s local storage to save draft stories whilst you write. This data remains on your " +
                            "device and is not transmitted to our servers until you submit your story. You can clear local storage " +
                            "at any time through your browser settings." +
                            "<br><br>" +
                            "<strong>8.3 Analytics</strong><br>" +
                            "We currently do not use analytics or tracking cookies. If we implement analytics in the future, we will " +
                            "update this policy and request your consent.",
                        Bullets = new List<string>()
                    },

                    // Section 9: Children's Privacy
                    new PolicyTemplateSection
                    {
                        Title = "9. Children\u2019s Privacy",
                        Body =
                            "Our services are intended for individuals aged 18 and older. We do not knowingly collect personal " +
                            "information from children under 18 without parental consent. If you are under 18, please obtain " +
                            "parental or guardian consent before using our services." +
                            "<br><br>" +
                            "If we become aware that we have collected personal information from a child under 18 without proper " +
                            "consent, we will take steps to delete that information promptly.",
                        Bullets = new List<string>()
                    },

                    // Section 10: Data Breach Notification
                    new PolicyTemplateSection
                    {
                        Title = "10. Data Breach Notification",
                        Body = "In the unlikely event of a data breach that is likely to cause harm, we will:",
                        Bullets = new List<string>
                        {
                            "Notify the Information Regulator of South Africa as soon as reasonably possible",
                            "Notify affected users directly via email",
                            "Provide details about the breach, affected information, and recommended actions",
                            "Take immediate steps to contain and remediate the breach",
                            "Document the incident and implement measures to prevent future occurrences"
                        }
                    },

                    // Section 11: Changes to This Policy
                    new PolicyTemplateSection
                    {
                        Title = "11. Changes to This Privacy Policy",
                        Body =
                            "We may update this Privacy Policy from time to time to reflect changes in our practices, legal " +
                            "requirements, or service features. When we make material changes, we will:",
                        Bullets = new List<string>
                        {
                            "Update the \u201cLast Updated\u201d date at the top of this policy",
                            "Notify you via email if the changes significantly affect your rights",
                            "Display a notice on our website"
                        }
                    },

                    // Section 12: International Data Transfers
                    new PolicyTemplateSection
                    {
                        Title = "12. International Data Transfers",
                        Body =
                            "Currently, all your data is stored on servers located in South Africa. We do not transfer your " +
                            "personal information outside of South Africa." +
                            "<br><br>" +
                            "If we need to transfer data internationally in the future (for example, to use additional service " +
                            "providers), we will:",
                        Bullets = new List<string>
                        {
                            "Ensure adequate safeguards are in place as required by POPIA",
                            "Only transfer to jurisdictions with adequate data protection laws",
                            "Inform you of such transfers and obtain consent where required",
                            "Update this Privacy Policy accordingly"
                        }
                    }
                },
                FooterText =
                    "This Privacy Policy was last updated on 7 November 2025. We encourage you to review this policy " +
                    "periodically for any changes. Questions or concerns about your privacy should be directed to our " +
                    "Information Officer using the contact information below.",
                ContactLines = new List<string>
                {
                    "Organisation: Healing-In-Writing (NPO)",
                    "Information Officer: Privacy Officer",
                    "Email: info@healinginwriting.org",
                    "Contact Page: /Home/Contact",
                    "To exercise your POPIA rights: Email us with your request (access, rectification, deletion, data export). We will respond within a reasonable timeframe as required by law."
                }
            };
        }

        public static PolicyTemplateData CreateTermsDefaults()
        {
            return new PolicyTemplateData
            {
                IntroLastUpdated = "7 November 2025",
                IntroText =
                    "Welcome to Healing-In-Writing. These Terms of Service govern your use of our website and services. " +
                    "By creating an account or using our platform, you agree to be bound by these Terms. Please read them " +
                    "carefully before using our services.",
                Sections = new List<PolicyTemplateSection>
                {
                    // Section 1: Acceptance of Terms
                    new PolicyTemplateSection
                    {
                        Title = "1. Acceptance of Terms",
                        Body =
                            "By accessing or using the Healing-In-Writing platform, you acknowledge that you have read, " +
                            "understood, and agree to be bound by these Terms of Service, as well as our " +
                            "<a href=\"/Home/Privacy\">Privacy Policy</a>. If you do not agree to these Terms, you may " +
                            "not use our services." +
                            "<br><br>" +
                            "We may update these Terms from time to time. Continued use of the platform after changes are " +
                            "posted constitutes acceptance of the updated Terms.",
                        Bullets = new List<string>()
                    },

                    // Section 2: Eligibility
                    new PolicyTemplateSection
                    {
                        Title = "2. Eligibility",
                        Body =
                            "Our services are intended for individuals aged 18 and older. If you are under 18, you may only " +
                            "use our services with the consent and supervision of a parent or legal guardian." +
                            "<br><br>" +
                            "By creating an account, you represent and warrant that you meet the eligibility requirements and " +
                            "that the information you provide is accurate and complete.",
                        Bullets = new List<string>()
                    },

                    // Section 3: Account Responsibilities
                    new PolicyTemplateSection
                    {
                        Title = "3. Account Responsibilities",
                        Body = "When you create an account with Healing-In-Writing, you agree to:",
                        Bullets = new List<string>
                        {
                            "Provide accurate and truthful registration information",
                            "Keep your login credentials confidential and secure",
                            "Notify us immediately if you suspect unauthorised access to your account",
                            "Accept responsibility for all activity that occurs under your account",
                            "Not create multiple accounts or share your account with others"
                        },
                        HighlightTitle = "Account Security",
                        HighlightBody =
                            "We implement security measures including password hashing, account lockout after failed attempts, " +
                            "and session expiration to help protect your account. However, you are ultimately responsible for " +
                            "safeguarding your login details."
                    },

                    // Section 4: User Content & Stories
                    new PolicyTemplateSection
                    {
                        Title = "4. User Content and Stories",
                        Body =
                            "<strong>4.1 Content Submission</strong><br>" +
                            "You may submit stories, written experiences, and other content to our platform. By submitting content, " +
                            "you grant Healing-In-Writing a non-exclusive, royalty-free licence to display, distribute, and promote " +
                            "your content on our platform in connection with our mission." +
                            "<br><br>" +
                            "<strong>4.2 Anonymity</strong><br>" +
                            "You may choose to submit content anonymously. When you select anonymous submission, your name will not " +
                            "be publicly associated with the content. We respect your choice and will not reveal your identity " +
                            "without your consent, except where required by law." +
                            "<br><br>" +
                            "<strong>4.3 Content Moderation</strong><br>" +
                            "We reserve the right to review, edit, or remove content that violates these Terms, is harmful, or is " +
                            "otherwise inappropriate. We will endeavour to notify you if your content is removed." +
                            "<br><br>" +
                            "<strong>4.4 Ownership</strong><br>" +
                            "You retain ownership of the content you submit. You may request removal of your content at any time " +
                            "by contacting us.",
                        Bullets = new List<string>()
                    },

                    // Section 5: Acceptable Use Policy
                    new PolicyTemplateSection
                    {
                        Title = "5. Acceptable Use Policy",
                        Body = "When using our platform, you agree not to:",
                        Bullets = new List<string>
                        {
                            "Post content that is abusive, threatening, defamatory, or deliberately harmful to others",
                            "Harass, intimidate, or bully other users or community members",
                            "Share another person\u2019s personal information without their consent",
                            "Attempt to gain unauthorised access to accounts, systems, or data",
                            "Upload malicious software, viruses, or other harmful code",
                            "Use the platform for commercial purposes, advertising, or spam",
                            "Impersonate another person or misrepresent your affiliation",
                            "Violate any applicable laws or regulations, including POPIA"
                        },
                        HighlightTitle = "Safe Space Commitment",
                        HighlightBody =
                            "Healing-In-Writing is a safe space for trauma survivors. We take violations of our acceptable use " +
                            "policy seriously and may suspend or terminate accounts that breach these guidelines."
                    },

                    // Section 6: Intellectual Property
                    new PolicyTemplateSection
                    {
                        Title = "6. Intellectual Property",
                        Body =
                            "The Healing-In-Writing platform, including its design, logos, text, graphics, and software, is the " +
                            "property of Healing-In-Writing (NPO) and is protected by South African intellectual property laws." +
                            "<br><br>" +
                            "You may not reproduce, distribute, modify, or create derivative works from any part of our platform " +
                            "without our prior written consent. User-submitted content remains the intellectual property of the " +
                            "respective authors.",
                        Bullets = new List<string>()
                    },

                    // Section 7: Events & Workshops
                    new PolicyTemplateSection
                    {
                        Title = "7. Events and Workshops",
                        Body =
                            "Healing-In-Writing hosts events and workshops as part of our community programmes. By registering " +
                            "for or attending events:",
                        Bullets = new List<string>
                        {
                            "You agree to conduct yourself respectfully and in accordance with our safe space guidelines",
                            "RSVP confirmations are not transferable without prior approval",
                            "We reserve the right to cancel or reschedule events and will notify registered attendees",
                            "Photography or recording at events requires prior consent from organisers and participants",
                            "Healing-In-Writing is not liable for any personal injury or loss incurred during events"
                        }
                    },

                    // Section 8: Privacy
                    new PolicyTemplateSection
                    {
                        Title = "8. Privacy",
                        Body =
                            "Your privacy is important to us. Our collection, use, and protection of your personal information " +
                            "is governed by our <a href=\"/Home/Privacy\">Privacy Policy</a>, which forms an integral part of " +
                            "these Terms of Service." +
                            "<br><br>" +
                            "By using our services, you consent to the collection and processing of your personal information " +
                            "as described in our Privacy Policy, in compliance with the Protection of Personal Information " +
                            "Act (POPIA) 4 of 2013.",
                        Bullets = new List<string>()
                    },

                    // Section 9: Disclaimers & Limitation of Liability
                    new PolicyTemplateSection
                    {
                        Title = "9. Disclaimers and Limitation of Liability",
                        Body =
                            "<strong>9.1 Platform Provided \"As Is\"</strong><br>" +
                            "Our platform and services are provided on an \"as is\" and \"as available\" basis. While we strive to " +
                            "maintain a reliable and secure service, we do not warrant that the platform will be uninterrupted, " +
                            "error-free, or free from harmful components." +
                            "<br><br>" +
                            "<strong>9.2 No Professional Advice</strong><br>" +
                            "Content shared on our platform, including stories and community discussions, does not constitute " +
                            "professional medical, psychological, or legal advice. If you are in crisis or need professional " +
                            "support, please contact a qualified professional or emergency services." +
                            "<br><br>" +
                            "<strong>9.3 Limitation of Liability</strong><br>" +
                            "To the maximum extent permitted by South African law, Healing-In-Writing, its directors, employees, " +
                            "and volunteers shall not be liable for any indirect, incidental, special, or consequential damages " +
                            "arising from your use of our services." +
                            "<br><br>" +
                            "<strong>9.4 User Content Disclaimer</strong><br>" +
                            "We do not endorse or take responsibility for content submitted by users. Views expressed in user " +
                            "stories are those of the individual authors and do not represent the views of Healing-In-Writing.",
                        Bullets = new List<string>()
                    },

                    // Section 10: Termination
                    new PolicyTemplateSection
                    {
                        Title = "10. Termination",
                        Body =
                            "You may terminate your account at any time by contacting us at " +
                            "<a href=\"mailto:info@healinginwriting.org\">info@healinginwriting.org</a>." +
                            "<br><br>" +
                            "We may suspend or terminate your account if you violate these Terms, engage in prohibited conduct, " +
                            "or if we determine that your use of the platform poses a risk to other users or our community." +
                            "<br><br>" +
                            "Upon termination, your account data will be handled in accordance with our " +
                            "<a href=\"/Home/Privacy\">Privacy Policy</a>, including the deletion or anonymisation of your " +
                            "personal information.",
                        Bullets = new List<string>()
                    },

                    // Section 11: Changes to Terms
                    new PolicyTemplateSection
                    {
                        Title = "11. Changes to These Terms",
                        Body =
                            "We may update these Terms of Service from time to time. When we make material changes, we will:",
                        Bullets = new List<string>
                        {
                            "Update the \u201cLast Updated\u201d date at the top of these Terms",
                            "Notify registered users via email of significant changes",
                            "Display a notice on our website",
                            "Allow a reasonable period before changes take effect"
                        }
                    },

                    // Section 12: Governing Law
                    new PolicyTemplateSection
                    {
                        Title = "12. Governing Law",
                        Body =
                            "These Terms of Service are governed by and construed in accordance with the laws of the Republic " +
                            "of South Africa, including but not limited to:" +
                            "<ul>" +
                            "<li>The Protection of Personal Information Act (POPIA) 4 of 2013</li>" +
                            "<li>The Consumer Protection Act (CPA) 68 of 2008</li>" +
                            "<li>The Electronic Communications and Transactions Act (ECTA) 25 of 2002</li>" +
                            "</ul>" +
                            "Any disputes arising from these Terms shall be subject to the exclusive jurisdiction of the courts " +
                            "of the Republic of South Africa.",
                        Bullets = new List<string>()
                    }
                },
                FooterText =
                    "These Terms of Service were last updated on 7 November 2025. We encourage you to review these Terms " +
                    "periodically for any changes. Questions or concerns should be directed to us using the contact " +
                    "information below.",
                ContactLines = new List<string>
                {
                    "Organisation: Healing-In-Writing (NPO)",
                    "Email: info@healinginwriting.org",
                    "Address: 27 Lovebird Walk, Sunbird Park, Cape Town, 7580",
                    "Contact Page: /Home/Contact"
                }
            };
        }
    }
}
