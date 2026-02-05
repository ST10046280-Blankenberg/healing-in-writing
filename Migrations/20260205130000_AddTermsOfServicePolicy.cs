using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealingInWriting.Migrations
{
    /// <inheritdoc />
    public partial class AddTermsOfServicePolicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TermsOfServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedBy = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermsOfServices", x => x.Id);
                });

            var privacyContent = @"            <!-- Introduction -->
            <div class=""privacy__intro"">
                <p class=""privacy__effective-date"">Last Updated: 7 November 2025</p>
                <p class=""privacy__intro-text"">
                    Healing-In-Writing is committed to protecting your privacy and personal information in accordance
                    with the
                    Protection of Personal Information Act (POPIA) 4 of 2013. This Privacy Policy explains what personal
                    information
                    we collect, why we collect it, how we use it, and your rights regarding your information.
                </p>
            </div>

            <!-- Privacy Content -->
            <div class=""privacy__content"">
                <!-- Section 1: Information We Collect -->
                <section class=""privacy__section"">
                    <h2 class=""privacy__section-title"">1. Information We Collect</h2>
                    <div class=""privacy__section-content"">
                        <div class=""privacy__subsection"">
                            <h3 class=""privacy__subsection-title"">1.1 Account Information</h3>
                            <p class=""privacy__paragraph"">
                                When you create an account with us, we collect:
                            </p>
                            <ul class=""privacy__list privacy__list--unordered"">
                                <li class=""privacy__list-item""><strong>First Name and Last Name</strong> - Used for
                                    personalisation and communication</li>
                                <li class=""privacy__list-item""><strong>Email Address</strong> - Used for authentication,
                                    communication, and account recovery</li>
                                <li class=""privacy__list-item""><strong>Password</strong> - Stored securely as a
                                    cryptographic hash using PBKDF2 with HMACSHA256</li>
                                <li class=""privacy__list-item""><strong>Account Creation Date</strong> - Used for
                                    record-keeping and security</li>
                                <li class=""privacy__list-item""><strong>Last Login Date</strong> - Used for security
                                    monitoring and account management</li>
                            </ul>
                        </div>

                        <div class=""privacy__subsection"">
                            <h3 class=""privacy__subsection-title"">1.2 User-Generated Content</h3>
                            <p class=""privacy__paragraph"">
                                When you use our services, we collect:
                            </p>
                            <ul class=""privacy__list privacy__list--unordered"">
                                <li class=""privacy__list-item""><strong>Stories</strong> - Your written submissions,
                                    which may contain personal experiences</li>
                                <li class=""privacy__list-item""><strong>Anonymity Preferences</strong> - Whether you
                                    choose to publish content anonymously</li>
                                <li class=""privacy__list-item""><strong>Draft Content</strong> - Temporarily stored in
                                    your browser's local storage</li>
                                <li class=""privacy__list-item""><strong>Event RSVPs</strong> - Your event participation
                                    preferences</li>
                            </ul>
                        </div>

                        <div class=""privacy__subsection"">
                            <h3 class=""privacy__subsection-title"">1.3 Technical Information</h3>
                            <p class=""privacy__paragraph"">
                                We automatically collect certain technical information:
                            </p>
                            <ul class=""privacy__list privacy__list--unordered"">
                                <li class=""privacy__list-item""><strong>IP Addresses</strong> - Temporarily used for rate
                                    limiting and security (not permanently logged)</li>
                                <li class=""privacy__list-item""><strong>Authentication Cookies</strong> - Session
                                    management (expire after 24 hours)</li>
                                <li class=""privacy__list-item""><strong>Browser Information</strong> - Collected via
                                    standard HTTP headers for security purposes</li>
                                <li class=""privacy__list-item""><strong>Security Logs</strong> - Login attempts, account
                                    lockouts, and security events</li>
                            </ul>
                        </div>

                        <div class=""privacy__highlight"">
                            <h4 class=""privacy__highlight-title"">What We Don't Collect</h4>
                            <p class=""privacy__highlight-text"">
                                We only collect information necessary for our services. We do not collect physical
                                addresses, phone numbers,
                                ID numbers, financial information, or any other personal information beyond what is
                                listed above.
                            </p>
                        </div>
                    </div>
                </section>

                <!-- Section 2: Legal Basis for Processing -->
                <section class=""privacy__section"">
                    <h2 class=""privacy__section-title"">2. Legal Basis for Processing Your Information</h2>
                    <div class=""privacy__section-content"">
                        <p class=""privacy__paragraph"">
                            Under POPIA, we process your personal information based on the following lawful grounds:
                        </p>
                        <div class=""privacy__subsection"">
                            <h3 class=""privacy__subsection-title"">2.1 Consent</h3>
                            <p class=""privacy__paragraph"">
                                You explicitly consent to data processing during registration by accepting our Terms of
                                Service and
                                Privacy Policy. You may withdraw consent at any time by deleting your account.
                            </p>
                        </div>
                        <div class=""privacy__subsection"">
                            <h3 class=""privacy__subsection-title"">2.2 Contractual Necessity</h3>
                            <p class=""privacy__paragraph"">
                                Processing is necessary to provide the services you request, such as creating an
                                account, displaying
                                your name, and managing your content.
                            </p>
                        </div>
                        <div class=""privacy__subsection"">
                            <h3 class=""privacy__subsection-title"">2.3 Legitimate Interest</h3>
                            <p class=""privacy__paragraph"">
                                We have legitimate interests in security monitoring, fraud prevention, and service
                                improvement. We balance
                                these interests against your privacy rights and only use data when necessary.
                            </p>
                        </div>
                    </div>
                </section>

                <!-- Section 3: How We Use Your Information -->
                <section class=""privacy__section"">
                    <h2 class=""privacy__section-title"">3. How We Use Your Information</h2>
                    <div class=""privacy__section-content"">
                        <p class=""privacy__paragraph"">
                            We use your personal information for the following purposes:
                        </p>
                        <ul class=""privacy__list privacy__list--unordered"">
                            <li class=""privacy__list-item""><strong>Account Management</strong> - Creating and
                                maintaining your account, authentication, password resets</li>
                            <li class=""privacy__list-item""><strong>Service Delivery</strong> - Providing access to our
                                platform, displaying your content, managing events</li>
                            <li class=""privacy__list-item""><strong>Personalisation</strong> - Displaying your name,
                                customising your experience</li>
                            <li class=""privacy__list-item""><strong>Communication</strong> - Sending important service
                                updates, responding to enquiries</li>
                            <li class=""privacy__list-item""><strong>Security</strong> - Monitoring for unauthorised
                                access, preventing fraud and abuse</li>
                            <li class=""privacy__list-item""><strong>Legal Compliance</strong> - Complying with applicable
                                laws and regulations</li>
                            <li class=""privacy__list-item""><strong>Service Improvement</strong> - Understanding how our
                                services are used to make improvements</li>
                        </ul>
                        <div class=""privacy__highlight"">
                            <h4 class=""privacy__highlight-title"">What We Don't Do</h4>
                            <p class=""privacy__highlight-text"">
                                We do not sell your personal information to third parties. We do not use your data for
                                marketing purposes
                                without your explicit consent. We do not engage in profiling or automated
                                decision-making that significantly
                                affects you.
                            </p>
                        </div>
                    </div>
                </section>

                <!-- Section 4: Information Sharing -->
                <section class=""privacy__section"">
                    <h2 class=""privacy__section-title"">4. How We Share Your Information</h2>
                    <div class=""privacy__section-content"">
                        <div class=""privacy__subsection"">
                            <h3 class=""privacy__subsection-title"">4.1 Public Content</h3>
                            <p class=""privacy__paragraph"">
                                Content you choose to publish (such as stories marked for publication) will be visible
                                to other users
                                and the public. You control whether your name appears or whether content is published
                                anonymously.
                            </p>
                        </div>
                        <div class=""privacy__subsection"">
                            <h3 class=""privacy__subsection-title"">4.2 Service Providers</h3>
                            <p class=""privacy__paragraph"">
                                We use the following third-party services to operate our platform:
                            </p>
                            <ul class=""privacy__list privacy__list--unordered"">
                                <li class=""privacy__list-item""><strong>Microsoft Azure</strong> - Database hosting
                                    (South Africa-based servers)</li>
                                <li class=""privacy__list-item""><strong>jsDelivr CDN</strong> - Content delivery for
                                    JavaScript libraries (does not access your personal data)</li>
                            </ul>
                            <p class=""privacy__paragraph"">
                                These service providers are bound by contractual obligations to protect your information
                                and only process
                                data on our instructions.
                            </p>
                        </div>
                        <div class=""privacy__subsection"">
                            <h3 class=""privacy__subsection-title"">4.3 Legal Requirements</h3>
                            <p class=""privacy__paragraph"">
                                We may disclose your information if required by law, court order, or to protect our
                                rights, property,
                                or safety, or that of our users or the public.
                            </p>
                        </div>
                        <div class=""privacy__subsection"">
                            <h3 class=""privacy__subsection-title"">4.4 Business Transfers</h3>
                            <p class=""privacy__paragraph"">
                                If Healing-In-Writing is involved in a merger, acquisition, or sale of assets, your
                                information may be
                                transferred. We will notify you before your information becomes subject to a different
                                privacy policy.
                            </p>
                        </div>
                    </div>
                </section>

                <!-- Section 5: Data Storage and Security -->
                <section class=""privacy__section"">
                    <h2 class=""privacy__section-title"">5. Data Storage and Security</h2>
                    <div class=""privacy__section-content"">
                        <div class=""privacy__subsection"">
                            <h3 class=""privacy__subsection-title"">5.1 Where We Store Your Data</h3>
                            <p class=""privacy__paragraph"">
                                Your personal information is stored on Microsoft Azure servers located in South Africa.
                                This ensures
                                compliance with POPIA's requirements for data residency and protection.
                            </p>
                        </div>
                        <div class=""privacy__subsection"">
                            <h3 class=""privacy__subsection-title"">5.2 Security Measures</h3>
                            <p class=""privacy__paragraph"">
                                We implement comprehensive security measures to protect your information:
                            </p>
                            <ul class=""privacy__list privacy__list--unordered"">
                                <li class=""privacy__list-item""><strong>Encryption</strong> - Passwords are hashed using
                                    PBKDF2 with HMACSHA256</li>
                                <li class=""privacy__list-item""><strong>HTTPS</strong> - All data transmitted between
                                    your browser and our servers is encrypted</li>
                                <li class=""privacy__list-item""><strong>Session Security</strong> - HttpOnly and Secure
                                    cookies prevent unauthorised access</li>
                                <li class=""privacy__list-item""><strong>Access Controls</strong> - Role-based access
                                    ensures only authorised users can access data</li>
                                <li class=""privacy__list-item""><strong>Rate Limiting</strong> - Protection against brute
                                    force attacks and automated abuse</li>
                                <li class=""privacy__list-item""><strong>SQL Injection Prevention</strong> - Parameterised
                                    queries protect against database attacks</li>
                                <li class=""privacy__list-item""><strong>XSS Protection</strong> - Content Security Policy
                                    and input sanitisation prevent script injection</li>
                                <li class=""privacy__list-item""><strong>CSRF Protection</strong> - Anti-forgery tokens on
                                    all state-changing operations</li>
                            </ul>
                        </div>
                        <div class=""privacy__subsection"">
                            <h3 class=""privacy__subsection-title"">5.3 Account Security</h3>
                            <p class=""privacy__paragraph"">
                                Your account is protected by:
                            </p>
                            <ul class=""privacy__list privacy__list--unordered"">
                                <li class=""privacy__list-item"">Password complexity requirements (minimum 8 characters)
                                </li>
                                <li class=""privacy__list-item"">Account lockout after 5 failed login attempts (5-minute
                                    lockout)</li>
                                <li class=""privacy__list-item"">Email verification required before account activation
                                </li>
                                <li class=""privacy__list-item"">Automatic session expiration after 24 hours</li>
                            </ul>
                        </div>
                    </div>
                </section>

                <!-- Section 6: Data Retention -->
                <section class=""privacy__section"">
                    <h2 class=""privacy__section-title"">6. How Long We Keep Your Information</h2>
                    <div class=""privacy__section-content"">
                        <p class=""privacy__paragraph"">
                            We retain your personal information only as long as necessary for the purposes outlined in
                            this policy:
                        </p>
                        <ul class=""privacy__list privacy__list--unordered"">
                            <li class=""privacy__list-item""><strong>Active Accounts</strong> - Retained until you delete
                                your account</li>
                            <li class=""privacy__list-item""><strong>Inactive Accounts</strong> - Flagged for review after
                                24 months of no login activity</li>
                            <li class=""privacy__list-item""><strong>Published Stories</strong> - Retained indefinitely
                                (anonymised if you delete your account)</li>
                            <li class=""privacy__list-item""><strong>Draft Content</strong> - Deleted when you delete your
                                account</li>
                            <li class=""privacy__list-item""><strong>Security Logs</strong> - Retained for 12 months, then
                                archived</li>
                            <li class=""privacy__list-item""><strong>Deleted Account Records</strong> - Audit trail kept
                                for 90 days, then anonymised</li>
                            <li class=""privacy__list-item""><strong>Authentication Cookies</strong> - Expire after 24
                                hours</li>
                            <li class=""privacy__list-item""><strong>Rate Limiting Data</strong> - Stored in memory only,
                                cleared on server restart</li>
                        </ul>
                    </div>
                </section>

                <!-- Section 7: Your Rights Under POPIA -->
                <section class=""privacy__section"">
                    <h2 class=""privacy__section-title"">7. Your Rights Under POPIA</h2>
                    <div class=""privacy__section-content"">
                        <p class=""privacy__paragraph"">
                            Under the Protection of Personal Information Act, you have the following rights regarding
                            your personal information:
                        </p>
                        <div class=""privacy__subsection"">
                            <h3 class=""privacy__subsection-title"">7.1 Right to Access</h3>
                            <p class=""privacy__paragraph"">
                                You have the right to request access to the personal information we hold about you. You
                                can request a
                                copy of your data by contacting us at <a
                                    href=""mailto:st10046280@vcconnect.edu.za"">st10046280@vcconnect.edu.za</a>.
                            </p>
                        </div>
                        <div class=""privacy__subsection"">
                            <h3 class=""privacy__subsection-title"">7.2 Right to Rectification</h3>
                            <p class=""privacy__paragraph"">
                                You have the right to correct inaccurate or incomplete information. You can update your
                                first name,
                                last name, and email address through your account settings (feature to be implemented).
                            </p>
                        </div>
                        <div class=""privacy__subsection"">
                            <h3 class=""privacy__subsection-title"">7.3 Right to Erasure</h3>
                            <p class=""privacy__paragraph"">
                                You have the right to request deletion of your account and associated personal
                                information. Upon deletion:
                            </p>
                            <ul class=""privacy__list privacy__list--unordered"">
                                <li class=""privacy__list-item"">Your account information will be permanently deleted</li>
                                <li class=""privacy__list-item"">Published stories will be anonymised (author link
                                    removed) or deleted per your preference</li>
                                <li class=""privacy__list-item"">Draft content will be permanently deleted</li>
                                <li class=""privacy__list-item"">Security logs will be retained for 90 days for legal
                                    compliance, then anonymised</li>
                                <li class=""privacy__list-item"">Backups will be purged within 30 days</li>
                            </ul>
                            <p class=""privacy__paragraph"">
                                To request account deletion, contact us at <a
                                    href=""mailto:st10046280@vcconnect.edu.za"">st10046280@vcconnect.edu.za</a>.
                            </p>
                        </div>
                        <div class=""privacy__subsection"">
                            <h3 class=""privacy__subsection-title"">7.4 Right to Object</h3>
                            <p class=""privacy__paragraph"">
                                You have the right to object to certain types of processing. You can choose to submit
                                stories anonymously
                                to limit the display of your personal information.
                            </p>
                        </div>
                        <div class=""privacy__subsection"">
                            <h3 class=""privacy__subsection-title"">7.5 Right to Data Portability</h3>
                            <p class=""privacy__paragraph"">
                                You have the right to receive your personal information in a structured,
                                machine-readable format (JSON).
                                Contact us to request a data export.
                            </p>
                        </div>
                        <div class=""privacy__subsection"">
                            <h3 class=""privacy__subsection-title"">7.6 Right to Complain</h3>
                            <p class=""privacy__paragraph"">
                                If you believe we have not complied with POPIA, you have the right to lodge a complaint
                                with the
                                Information Regulator of South Africa:
                            </p>
                            <p class=""privacy__paragraph"">
                                <strong>Information Regulator South Africa</strong><br />
                                Email: <a href=""mailto:inforeg@justice.gov.za"">inforeg@justice.gov.za</a><br />
                                Website: <a href=""https://inforegulator.org.za"" target=""_blank""
                                    rel=""noopener noreferrer"">https://inforegulator.org.za</a>
                            </p>
                        </div>
                    </div>
                </section>

                <!-- Section 8: Cookies and Tracking -->
                <section class=""privacy__section"">
                    <h2 class=""privacy__section-title"">8. Cookies and Tracking Technologies</h2>
                    <div class=""privacy__section-content"">
                        <div class=""privacy__subsection"">
                            <h3 class=""privacy__subsection-title"">8.1 Essential Cookies</h3>
                            <p class=""privacy__paragraph"">
                                We use essential cookies that are necessary for the operation of our services:
                            </p>
                            <ul class=""privacy__list privacy__list--unordered"">
                                <li class=""privacy__list-item""><strong>Authentication Cookies</strong> - Keep you logged
                                    in (HttpOnly, Secure, SameSite)</li>
                                <li class=""privacy__list-item""><strong>Anti-Forgery Tokens</strong> - Protect against
                                    CSRF attacks</li>
                            </ul>
                            <p class=""privacy__paragraph"">
                                These cookies are essential for security and functionality. They cannot be disabled
                                without affecting
                                your ability to use our services.
                            </p>
                        </div>
                        <div class=""privacy__subsection"">
                            <h3 class=""privacy__subsection-title"">8.2 Local Storage</h3>
                            <p class=""privacy__paragraph"">
                                We use your browser's local storage to save draft stories whilst you write. This data
                                remains on your
                                device and is not transmitted to our servers until you submit your story. You can clear
                                local storage
                                at any time through your browser settings.
                            </p>
                        </div>
                        <div class=""privacy__subsection"">
                            <h3 class=""privacy__subsection-title"">8.3 Analytics</h3>
                            <p class=""privacy__paragraph"">
                                We currently do not use analytics or tracking cookies. If we implement analytics in the
                                future, we will
                                update this policy and request your consent.
                            </p>
                        </div>
                    </div>
                </section>

                <!-- Section 9: Children's Privacy -->
                <section class=""privacy__section"">
                    <h2 class=""privacy__section-title"">9. Children's Privacy</h2>
                    <div class=""privacy__section-content"">
                        <p class=""privacy__paragraph"">
                            Our services are intended for individuals aged 18 and older. We do not knowingly collect
                            personal information
                            from children under 18 without parental consent. If you are under 18, please obtain parental
                            or guardian
                            consent before using our services.
                        </p>
                        <p class=""privacy__paragraph"">
                            If we become aware that we have collected personal information from a child under 18 without
                            proper consent,
                            we will take steps to delete that information promptly.
                        </p>
                    </div>
                </section>

                <!-- Section 10: Data Breach Notification -->
                <section class=""privacy__section"">
                    <h2 class=""privacy__section-title"">10. Data Breach Notification</h2>
                    <div class=""privacy__section-content"">
                        <p class=""privacy__paragraph"">
                            In the unlikely event of a data breach that is likely to cause harm, we will:
                        </p>
                        <ul class=""privacy__list privacy__list--unordered"">
                            <li class=""privacy__list-item"">Notify the Information Regulator of South Africa as soon as
                                reasonably possible</li>
                            <li class=""privacy__list-item"">Notify affected users directly via email</li>
                            <li class=""privacy__list-item"">Provide details about the breach, affected information, and
                                recommended actions</li>
                            <li class=""privacy__list-item"">Take immediate steps to contain and remediate the breach</li>
                            <li class=""privacy__list-item"">Document the incident and implement measures to prevent
                                future occurrences</li>
                        </ul>
                    </div>
                </section>

                <!-- Section 11: Changes to This Policy -->
                <section class=""privacy__section"">
                    <h2 class=""privacy__section-title"">11. Changes to This Privacy Policy</h2>
                    <div class=""privacy__section-content"">
                        <p class=""privacy__paragraph"">
                            We may update this Privacy Policy from time to time to reflect changes in our practices,
                            legal requirements,
                            or service features. When we make material changes, we will:
                        </p>
                        <ul class=""privacy__list privacy__list--unordered"">
                            <li class=""privacy__list-item"">Update the ""Last Updated"" date at the top of this policy</li>
                            <li class=""privacy__list-item"">Notify you via email if the changes significantly affect your
                                rights</li>
                            <li class=""privacy__list-item"">Display a notice on our website</li>
                        </ul>
                        <p class=""privacy__paragraph"">
                            We encourage you to review this Privacy Policy periodically. Your continued use of our
                            services after changes
                            are made constitutes acceptance of the updated policy.
                        </p>
                    </div>
                </section>

                <!-- Section 12: International Data Transfers -->
                <section class=""privacy__section"">
                    <h2 class=""privacy__section-title"">12. International Data Transfers</h2>
                    <div class=""privacy__section-content"">
                        <p class=""privacy__paragraph"">
                            Currently, all your data is stored on servers located in South Africa. We do not transfer
                            your personal
                            information outside of South Africa.
                        </p>
                        <p class=""privacy__paragraph"">
                            If we need to transfer data internationally in the future (for example, to use additional
                            service providers),
                            we will:
                        </p>
                        <ul class=""privacy__list privacy__list--unordered"">
                            <li class=""privacy__list-item"">Ensure adequate safeguards are in place as required by POPIA
                            </li>
                            <li class=""privacy__list-item"">Only transfer to jurisdictions with adequate data protection
                                laws</li>
                            <li class=""privacy__list-item"">Inform you of such transfers and obtain consent where
                                required</li>
                            <li class=""privacy__list-item"">Update this Privacy Policy accordingly</li>
                        </ul>
                    </div>
                </section>
            </div>

            <!-- Footer -->
            <div class=""privacy__footer"">
                <p class=""privacy__footer-text"">
                    This Privacy Policy was last updated on 7 November 2025. We encourage you to review this policy
                    periodically
                    for any changes. Questions or concerns about your privacy should be directed to our Information
                    Officer
                    using the contact information below.
                </p>
                <div class=""privacy__contact-info"">
                    <h3 class=""privacy__contact-title"">Contact Information</h3>
                    <p class=""privacy__contact-detail""><strong>Organisation:</strong> Healing-In-Writing (NPO)</p>
                    <p class=""privacy__contact-detail""><strong>Information Officer:</strong> Privacy Officer</p>
                    <p class=""privacy__contact-detail""><strong>Email:</strong> <a
                            href=""mailto:st10046280@vcconnect.edu.za"">st10046280@vcconnect.edu.za</a></p>
                    <p class=""privacy__contact-detail"">
                        Or visit our <a href=""/Home/Contact"">Contact Page</a> to send us a message.
                    </p>
                    <p class=""privacy__contact-detail privacy__contact-detail--top-margin"">
                        <strong>To exercise your POPIA rights:</strong> Email us with your request (access,
                        rectification, deletion, data export).
                        We will respond within a reasonable timeframe as required by law.
                    </p>
                </div>
            </div>";
            var termsContent = @"            <!-- Introduction -->
            <div class=""terms__intro"">
                <p class=""terms__effective-date"">Last Updated: November 6, 2025</p>
                <p class=""terms__intro-text"">
                    Welcome to Healing in Writing. These Terms of Service (""Terms"") govern your access to and use of our website,
                    services, and programs. By accessing or using our services, you agree to be bound by these Terms. If you do not
                    agree with any part of these Terms, please do not use our services.
                </p>
            </div>

            <!-- Terms Content -->
            <div class=""terms__content"">
                <!-- Section 1: Acceptance of Terms -->
                <section class=""terms__section"">
                    <h2 class=""terms__section-title"">1. Acceptance of Terms</h2>
                    <div class=""terms__section-content"">
                        <p class=""terms__paragraph"">
                            By creating an account, participating in our programs, or using our website, you acknowledge that you have
                            read, understood, and agree to be bound by these Terms of Service and our Privacy Policy. These Terms apply
                            to all users of the site, including but not limited to visitors, registered users, volunteers, and contributors.
                        </p>
                        <p class=""terms__paragraph"">
                            We reserve the right to modify these Terms at any time. We will notify users of any material changes via
                            email or through a notice on our website. Your continued use of our services after such modifications
                            constitutes your acceptance of the updated Terms.
                        </p>
                    </div>
                </section>

                <!-- Section 2: Eligibility -->
                <section class=""terms__section"">
                    <h2 class=""terms__section-title"">2. Eligibility</h2>
                    <div class=""terms__section-content"">
                        <p class=""terms__paragraph"">
                            Our services are available to individuals who are:
                        </p>
                        <ul class=""terms__list terms__list--unordered"">
                            <li class=""terms__list-item"">At least 18 years of age, or have parental/guardian consent if under 18</li>
                            <li class=""terms__list-item"">Capable of forming a binding contract under applicable law</li>
                            <li class=""terms__list-item"">Not prohibited from using our services under the laws of South Africa or any other applicable jurisdiction</li>
                        </ul>
                        <p class=""terms__paragraph"">
                            By using our services, you represent and warrant that you meet these eligibility requirements.
                        </p>
                    </div>
                </section>

                <!-- Section 3: User Accounts -->
                <section class=""terms__section"">
                    <h2 class=""terms__section-title"">3. User Accounts</h2>
                    <div class=""terms__section-content"">
                        <div class=""terms__subsection"">
                            <h3 class=""terms__subsection-title"">3.1 Account Registration</h3>
                            <p class=""terms__paragraph"">
                                To access certain features of our services, you must create an account. You agree to provide accurate,
                                current, and complete information during registration and to update such information as necessary to
                                maintain its accuracy.
                            </p>
                        </div>
                        <div class=""terms__subsection"">
                            <h3 class=""terms__subsection-title"">3.2 Account Security</h3>
                            <p class=""terms__paragraph"">
                                You are responsible for maintaining the confidentiality of your account credentials and for all activities
                                that occur under your account. You must immediately notify us of any unauthorized use of your account or
                                any other security breach.
                            </p>
                        </div>
                        <div class=""terms__subsection"">
                            <h3 class=""terms__subsection-title"">3.3 Account Roles</h3>
                            <p class=""terms__paragraph"">
                                Our platform offers different user roles (Guest, Volunteer, Admin). Each role has specific permissions
                                and responsibilities. Volunteers must complete required training and adhere to volunteer guidelines.
                            </p>
                        </div>
                    </div>
                </section>

                <!-- Section 4: User Content -->
                <section class=""terms__section"">
                    <h2 class=""terms__section-title"">4. User Content and Submissions</h2>
                    <div class=""terms__section-content"">
                        <div class=""terms__subsection"">
                            <h3 class=""terms__subsection-title"">4.1 Content Ownership</h3>
                            <p class=""terms__paragraph"">
                                You retain all ownership rights to the content you submit to our platform, including stories, comments,
                                and other materials (""User Content""). However, by submitting User Content, you grant Healing in Writing
                                a non-exclusive, worldwide, royalty-free license to use, reproduce, modify, and display such content
                                solely for the purpose of operating and promoting our services.
                            </p>
                        </div>
                        <div class=""terms__subsection"">
                            <h3 class=""terms__subsection-title"">4.2 Content Guidelines</h3>
                            <p class=""terms__paragraph"">
                                When submitting content, you agree not to post content that:
                            </p>
                            <ul class=""terms__list terms__list--unordered"">
                                <li class=""terms__list-item"">Is false, misleading, or fraudulent</li>
                                <li class=""terms__list-item"">Violates any third-party rights, including copyright, trademark, or privacy rights</li>
                                <li class=""terms__list-item"">Contains hate speech, discrimination, or promotes violence</li>
                                <li class=""terms__list-item"">Contains malicious code, viruses, or other harmful software</li>
                                <li class=""terms__list-item"">Violates any applicable laws or regulations</li>
                            </ul>
                        </div>
                        <div class=""terms__subsection"">
                            <h3 class=""terms__subsection-title"">4.3 Story Submissions</h3>
                            <p class=""terms__paragraph"">
                                Stories submitted for publication are reviewed by our team. We reserve the right to decline publication
                                of any submission. Published stories may be edited for clarity, length, or appropriateness, always with
                                the author's consent. Authors maintain the right to request removal of their published stories at any time.
                            </p>
                        </div>
                        <div class=""terms__highlight"">
                            <h4 class=""terms__highlight-title"">Important: Anonymity and Privacy</h4>
                            <p class=""terms__highlight-text"">
                                You have complete control over your identity when sharing content. You may use your real name, a pseudonym,
                                or remain anonymous. We will never publish your personal information without your explicit consent.
                            </p>
                        </div>
                    </div>
                </section>

                <!-- Section 5: Prohibited Conduct -->
                <section class=""terms__section"">
                    <h2 class=""terms__section-title"">5. Prohibited Conduct</h2>
                    <div class=""terms__section-content"">
                        <p class=""terms__paragraph"">
                            You agree not to engage in any of the following prohibited activities:
                        </p>
                        <ul class=""terms__list terms__list--unordered"">
                            <li class=""terms__list-item"">Using our services for any illegal purpose or in violation of any laws</li>
                            <li class=""terms__list-item"">Impersonating any person or entity, or falsely stating or misrepresenting your affiliation</li>
                            <li class=""terms__list-item"">Interfering with or disrupting our services or servers</li>
                            <li class=""terms__list-item"">Attempting to gain unauthorized access to any portion of our services</li>
                            <li class=""terms__list-item"">Collecting or harvesting any personally identifiable information from other users</li>
                            <li class=""terms__list-item"">Using automated systems or software to extract data from our services (""scraping"")</li>
                            <li class=""terms__list-item"">Harassing, threatening, or intimidating other users</li>
                        </ul>
                    </div>
                </section>

                <!-- Section 6: Intellectual Property -->
                <section class=""terms__section"">
                    <h2 class=""terms__section-title"">6. Intellectual Property Rights</h2>
                    <div class=""terms__section-content"">
                        <p class=""terms__paragraph"">
                            The Healing in Writing website, including its design, functionality, text, graphics, logos, and other content
                            (excluding User Content) is owned by Healing in Writing and is protected by copyright, trademark, and other
                            intellectual property laws.
                        </p>
                        <p class=""terms__paragraph"">
                            You may not copy, modify, distribute, sell, or lease any part of our services without our express written
                            permission. You may not reverse engineer or attempt to extract source code from our software.
                        </p>
                    </div>
                </section>

                <!-- Section 7: Disclaimers -->
                <section class=""terms__section"">
                    <h2 class=""terms__section-title"">7. Disclaimers and Limitations of Liability</h2>
                    <div class=""terms__section-content"">
                        <div class=""terms__subsection"">
                            <h3 class=""terms__subsection-title"">7.1 Medical and Professional Advice</h3>
                            <p class=""terms__paragraph"">
                                <strong>Our services are not a substitute for professional medical, psychological, or legal advice.</strong>
                                The content and programs provided through Healing in Writing are for informational and therapeutic writing
                                purposes only. If you are in crisis or need immediate help, please contact emergency services or a crisis
                                helpline.
                            </p>
                        </div>
                        <div class=""terms__subsection"">
                            <h3 class=""terms__subsection-title"">7.2 Service Availability</h3>
                            <p class=""terms__paragraph"">
                                We strive to provide reliable services but cannot guarantee that our services will be uninterrupted,
                                secure, or error-free. We reserve the right to modify, suspend, or discontinue any aspect of our services
                                at any time without notice.
                            </p>
                        </div>
                        <div class=""terms__subsection"">
                            <h3 class=""terms__subsection-title"">7.3 Limitation of Liability</h3>
                            <p class=""terms__paragraph"">
                                To the fullest extent permitted by law, Healing in Writing shall not be liable for any indirect,
                                incidental, special, consequential, or punitive damages arising from or related to your use of our
                                services.
                            </p>
                        </div>
                    </div>
                </section>

                <!-- Section 8: Privacy -->
                <section class=""terms__section"">
                    <h2 class=""terms__section-title"">8. Privacy and Data Protection</h2>
                    <div class=""terms__section-content"">
                        <p class=""terms__paragraph"">
                            Your privacy is extremely important to us. Our collection, use, and disclosure of personal information is
                            governed by our <a href=""/Home/Privacy"">Privacy Policy</a>, which is incorporated into these Terms by
                            reference. We comply with the Protection of Personal Information Act (POPIA) and other applicable data
                            protection laws.
                        </p>
                        <p class=""terms__paragraph"">
                            By using our services, you consent to the collection and use of your information as described in our
                            Privacy Policy.
                        </p>
                    </div>
                </section>

                <!-- Section 9: Termination -->
                <section class=""terms__section"">
                    <h2 class=""terms__section-title"">9. Termination</h2>
                    <div class=""terms__section-content"">
                        <p class=""terms__paragraph"">
                            We reserve the right to suspend or terminate your account and access to our services at any time, with or
                            without notice, for any reason, including but not limited to violation of these Terms.
                        </p>
                        <p class=""terms__paragraph"">
                            You may terminate your account at any time by contacting us. Upon termination, your right to use our services
                            will immediately cease. Provisions of these Terms that by their nature should survive termination shall
                            survive, including ownership provisions, warranty disclaimers, and limitations of liability.
                        </p>
                    </div>
                </section>

                <!-- Section 10: Governing Law -->
                <section class=""terms__section"">
                    <h2 class=""terms__section-title"">10. Governing Law and Dispute Resolution</h2>
                    <div class=""terms__section-content"">
                        <p class=""terms__paragraph"">
                            These Terms shall be governed by and construed in accordance with the laws of the Republic of South Africa,
                            without regard to its conflict of law provisions.
                        </p>
                        <p class=""terms__paragraph"">
                            Any disputes arising from or relating to these Terms or your use of our services shall be resolved through
                            good faith negotiation. If negotiation fails, disputes shall be submitted to the jurisdiction of the courts
                            of South Africa.
                        </p>
                    </div>
                </section>

                <!-- Section 11: General Provisions -->
                <section class=""terms__section"">
                    <h2 class=""terms__section-title"">11. General Provisions</h2>
                    <div class=""terms__section-content"">
                        <div class=""terms__subsection"">
                            <h3 class=""terms__subsection-title"">11.1 Entire Agreement</h3>
                            <p class=""terms__paragraph"">
                                These Terms, together with our Privacy Policy, constitute the entire agreement between you and Healing
                                in Writing regarding your use of our services.
                            </p>
                        </div>
                        <div class=""terms__subsection"">
                            <h3 class=""terms__subsection-title"">11.2 Severability</h3>
                            <p class=""terms__paragraph"">
                                If any provision of these Terms is found to be invalid or unenforceable, the remaining provisions shall
                                remain in full force and effect.
                            </p>
                        </div>
                        <div class=""terms__subsection"">
                            <h3 class=""terms__subsection-title"">11.3 Waiver</h3>
                            <p class=""terms__paragraph"">
                                Our failure to enforce any right or provision of these Terms shall not be deemed a waiver of such right
                                or provision.
                            </p>
                        </div>
                        <div class=""terms__subsection"">
                            <h3 class=""terms__subsection-title"">11.4 Assignment</h3>
                            <p class=""terms__paragraph"">
                                You may not assign or transfer these Terms or your rights hereunder without our prior written consent.
                                We may assign these Terms without restriction.
                            </p>
                        </div>
                    </div>
                </section>
            </div>

            <!-- Footer -->
            <div class=""terms__footer"">
                <p class=""terms__footer-text"">
                    These Terms of Service were last updated on November 6, 2025. We encourage you to review these Terms periodically
                    for any changes. Questions or concerns about these Terms should be directed to us using the contact information below.
                </p>
                <div class=""terms__contact-info"">
                    <h3 class=""terms__contact-title"">Contact Information</h3>
                    <p class=""terms__contact-detail"">Healing in Writing</p>
                    <p class=""terms__contact-detail"">Email: <a href=""mailto:info@healinginwriting.org"">info@healinginwriting.org</a></p>
                    <p class=""terms__contact-detail"">Phone: <a href=""tel:+27123456789"">+27 12 345 6789</a></p>
                    <p class=""terms__contact-detail"">
                        Or visit our <a href=""/Home/Contact"">Contact Page</a> to send us a message.
                    </p>
                </div>
            </div>";

            var privacyContentSql = privacyContent.Replace("'", "''");
            var termsContentSql = termsContent.Replace("'", "''");

            migrationBuilder.Sql($@"
INSERT INTO PrivacyPolicies (Content, LastUpdated, UpdatedBy)
SELECT '{privacyContentSql}', '2025-11-07T00:00:00Z', 'System'
WHERE NOT EXISTS (SELECT 1 FROM PrivacyPolicies);
");

            migrationBuilder.Sql($@"
INSERT INTO TermsOfServices (Content, LastUpdated, UpdatedBy)
SELECT '{termsContentSql}', '2025-11-06T00:00:00Z', 'System'
WHERE NOT EXISTS (SELECT 1 FROM TermsOfServices);
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TermsOfServices");
        }
    }
}
