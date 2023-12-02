using powerLabel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace powerLabel.ComponentProviders
{
    internal class MotherboardProvider : IComponentProvider<Motherboard>
    {
        public Task<Motherboard> GetComponentAsync()
        {
            Motherboard motherboard = new Motherboard();

            var mobo = PSInterface.GetObject("Win32_ComputerSystem");

            motherboard.manufacturer = (string)mobo["Manufacturer"];

            if (motherboard.manufacturer.Trim().Equals("Lenovo", System.StringComparison.InvariantCultureIgnoreCase))
                motherboard.model = (string)mobo["SystemFamily"];
            else
                motherboard.model = (string)mobo["Model"];


            var bios = PSInterface.GetObject("Win32_Bios");

            motherboard.serialNumber = (string)bios["SerialNumber"];

            return Task.FromResult(motherboard);
        }
    }
}
