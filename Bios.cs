using System;
using System.Management;

namespace powerLabel
{
    public class Bios
    {
        public int id { get; set; }
        public string version { get; set; }
        public DateTime date { get; set; }

        public static Bios GetBios()
        {
            Bios bios = new Bios();

            foreach (ManagementObject item in PSInterface.RunObjectQuery("SELECT * FROM Win32_Bios"))
            {
                bios.version = (string)item["SMBIOSBIOSVersion"];
                bios.date = DateTime.ParseExact(((string)item["ReleaseDate"]).Substring(0, ((string)item["ReleaseDate"]).Length - 4), "yyyyMMddHHmmss.FFFFFF", null);
            }

            return bios;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            Bios bios = (Bios)obj;
            if (this.version.Trim() == bios.version.Trim() &&
                this.date == bios.date
                )
            {
                return true;
            }
            return false;
        }
    }
}
