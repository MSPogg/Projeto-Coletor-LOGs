using LCollector.Entities;
using System.Text.RegularExpressions;

namespace LCollector.Utils
{
    public static class PromptParser
    {
        public static (string hostname, Vendor vendor) ParsePrompt(string collectedPrompt)
        {
            string promptLimpo = collectedPrompt.Trim();

            string hostname = "Unknown";
            Vendor vendor = Vendor.Unknown;

            if (string.IsNullOrWhiteSpace(promptLimpo))
                return (hostname, vendor);

            // REGRA HUAWEI
            if (promptLimpo.Contains("<") && promptLimpo.Contains(">"))
            {
                vendor = Vendor.Huawei;
                hostname = promptLimpo.Replace("<", "").Replace(">", "").Trim();
            }
            // REGRA ALCATEL / NOKIA
            else if ((promptLimpo.Contains("A:") || promptLimpo.Contains("B:")) && promptLimpo.Contains("#"))
            {
                vendor = Vendor.Alcatel;
                hostname = promptLimpo.Replace("A:", "").Replace("B:", "").Replace("#", "").Trim();
            }
            // REGRA CISCO XR
            else if (Regex.IsMatch(promptLimpo, @"CPU\d+:") && promptLimpo.Contains("#"))
            {
                vendor = Vendor.CiscoXR;

                hostname = Regex.Replace(promptLimpo,@"^.*CPU\d+:","").Replace("#", "").Trim();
            }
            // REGRA CISCO XE
            else if (promptLimpo.Contains("#"))
            {
                vendor = Vendor.CiscoXE;
                hostname = promptLimpo.Replace("#", "").Trim();
            }

            return (hostname, vendor);
        }
    }
}