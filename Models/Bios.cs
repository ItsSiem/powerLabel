using System;

namespace powerLabel.Models
{
    public class Bios
    {
        public int id { get; set; }
        public string version { get; set; }
        public DateTime date { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            Bios bios = (Bios)obj;
            if (version.Trim() == bios.version.Trim() &&
                date == bios.date
                )
            {
                return true;
            }
            return false;
        }
    }
}
