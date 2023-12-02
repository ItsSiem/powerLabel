using System.Collections.Generic;
using System.Management;

namespace powerLabel.Models
{
    public class OS
    {
        public int id { get; set; }
        public string caption { get; set; }
        public string language { get; set; }

        public static Dictionary<uint, string> languageTable = new Dictionary<uint, string>()
        {
            {9, "EN" },
            {1031, "DE" },
            {1033, "US" },
            {1043, "NL" }
        };

        public static OS GetOS()
        {
            OS os = new OS();
            foreach (ManagementObject item in PSInterface.GetObjects("SELECT * FROM Win32_OperatingSystem"))
            {
                os.caption = (string)item["Caption"];
                os.language = languageTable[(uint)item["OSLanguage"]];
            }

            return os;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            OS os = (OS)obj;
            if (caption.Trim() == os.caption.Trim() &&
                language.Trim() == os.language.Trim()
                )
            {
                return true;
            }
            return false;
        }
    }
}
