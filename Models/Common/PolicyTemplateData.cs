using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HealingInWriting.Models.Common
{
    public class PolicyTemplateData
    {
        public string IntroLastUpdated { get; set; } = string.Empty;
        public string IntroText { get; set; } = string.Empty;
        public List<PolicyTemplateSection> Sections { get; set; } = new List<PolicyTemplateSection>();
        public string FooterText { get; set; } = string.Empty;
        public List<string> ContactLines { get; set; } = new List<string>();

        [JsonIgnore]
        public string ContactLinesText { get; set; } = string.Empty;
    }

    public class PolicyTemplateSection
    {
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public List<string> Bullets { get; set; } = new List<string>();
        public string? HighlightTitle { get; set; }
        public string? HighlightBody { get; set; }

        [JsonIgnore]
        public string BulletsText { get; set; } = string.Empty;
    }
}
