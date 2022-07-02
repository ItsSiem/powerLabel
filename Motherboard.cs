using System.Management;

namespace powerLabel
{
    public class Motherboard
    {
        public int id { get; set; }
        public string model { get; set; }
        public string manufacturer { get; set; }
        public string serialNumber { get; set; }

        public static Motherboard GetMotherboard()
        {
            Motherboard motherboard = new Motherboard();

            foreach (ManagementObject system in PSInterface.RunPowershell("SELECT * FROM Win32_ComputerSystem"))
            {
                motherboard.model = (string)system["Model"];
                motherboard.manufacturer = (string)system["Manufacturer"];
            }

            foreach (ManagementObject bios in PSInterface.RunPowershell("SELECT * FROM Win32_Bios"))
            {
                motherboard.serialNumber = (string)bios["SerialNumber"];
            }

            return motherboard;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            Motherboard mobo = (Motherboard)obj;
            if (this.model.Trim() == mobo.model.Trim() &&
                this.manufacturer.Trim() == mobo.manufacturer.Trim() &&
                this.serialNumber.Trim() == mobo.serialNumber.Trim()
                )
            {
                return true;
            }
            return false;
        }
    }


}
