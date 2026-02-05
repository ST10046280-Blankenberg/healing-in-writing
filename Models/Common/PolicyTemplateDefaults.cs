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
                IntroText = "Update this introduction to explain how you collect and protect personal information.",
                Sections = CreateSectionPlaceholders(),
                FooterText = "Add your contact details and a short reminder to review this policy regularly.",
                ContactLines = new List<string>
                {
                    "Organisation: Healing-In-Writing (NPO)",
                    "Information Officer: Privacy Officer",
                    "Email: info@healinginwriting.org",
                    "Contact Page: /Home/Contact"
                }
            };
        }

        public static PolicyTemplateData CreateTermsDefaults()
        {
            return new PolicyTemplateData
            {
                IntroLastUpdated = "6 November 2025",
                IntroText = "Update this introduction to explain the purpose of these Terms.",
                Sections = CreateSectionPlaceholders(),
                FooterText = "Add your contact details and a short reminder to review these Terms regularly.",
                ContactLines = new List<string>
                {
                    "Healing in Writing",
                    "Email: info@healinginwriting.org",
                    "Phone: +27 12 345 6789",
                    "Contact Page: /Home/Contact"
                }
            };
        }

        private static List<PolicyTemplateSection> CreateSectionPlaceholders()
        {
            return new List<PolicyTemplateSection>
            {
                new PolicyTemplateSection
                {
                    Title = "Section 1 Title",
                    Body = "Describe this section in plain language.",
                    Bullets = new List<string>()
                }
            };
        }
    }
}
