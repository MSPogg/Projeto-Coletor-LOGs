using LCollector.Entities;

namespace LCollector.Utils
{
    public static class PromptParser
    {
        public static (string hostname, Vendor vendor) ParsePrompt(string collectedPrompt)
        {
            // Forçamos o Trim para eliminar qualquer espaço invisível ou quebra de linha lateral
            string promptLimpo = collectedPrompt.Trim();

            string hostname = "Unknown";
            Vendor vendor = Vendor.Unknown;

            if (string.IsNullOrWhiteSpace(promptLimpo))
                return (hostname, vendor);

            // REGRA HUAWEI (Ex: <ESITM10-RMP01>)
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
            else if (promptLimpo.Contains("RP/0/RP0/CPU0:") && promptLimpo.Contains("#"))
            {
                vendor = Vendor.CiscoXR;
                hostname = promptLimpo.Replace("RP/0/RP0/CPU0:", "").Replace("#", "").Trim();
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