using System.Management;

namespace powerLabel.Models
{
    public class Motherboard
    {
        public int id { get; set; }
        public string model { get; set; }
        public string manufacturer { get; set; }
        public string serialNumber { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            Motherboard mobo = (Motherboard)obj;
            if (model.Trim() == mobo.model.Trim() &&
                manufacturer.Trim() == mobo.manufacturer.Trim() &&
                serialNumber.Trim() == mobo.serialNumber.Trim()
                )
            {
                return true;
            }
            return false;
        }
    }


}
