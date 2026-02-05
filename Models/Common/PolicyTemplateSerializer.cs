using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace HealingInWriting.Models.Common
{
    public static class PolicyTemplateSerializer
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static string Serialize(PolicyTemplateData data)
        {
            return JsonSerializer.Serialize(data, Options);
        }

        public static PolicyTemplateData Deserialize(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new PolicyTemplateData();
            }

            var data = JsonSerializer.Deserialize<PolicyTemplateData>(json, Options) ?? new PolicyTemplateData();
            PopulateTextFields(data);
            return data;
        }

        public static void PopulateTextFields(PolicyTemplateData data)
        {
            data.ContactLinesText = string.Join("\n", data.ContactLines ?? new List<string>());

            if (data.Sections == null)
            {
                data.Sections = new List<PolicyTemplateSection>();
            }

            foreach (var section in data.Sections)
            {
                section.BulletsText = string.Join("\n", section.Bullets ?? new List<string>());
            }
        }

        public static void PopulateListsFromText(PolicyTemplateData data)
        {
            data.ContactLines = SplitLines(data.ContactLinesText);

            if (data.Sections == null)
            {
                data.Sections = new List<PolicyTemplateSection>();
            }

            foreach (var section in data.Sections)
            {
                section.Bullets = SplitLines(section.BulletsText);
            }
        }

        private static List<string> SplitLines(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return new List<string>();
            }

            return value
                .Split('\n')
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToList();
        }
    }
}
