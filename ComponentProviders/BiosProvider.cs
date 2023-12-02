using powerLabel.Models;
using System;
using System.Management;
using System.Threading.Tasks;

namespace powerLabel.ComponentProviders
{
    internal class BiosProvider : IComponentProvider<Bios>
    {
        public Task<Bios> GetComponentAsync()
        {
            Bios bios = new Bios();

            var obj = PSInterface.GetObject("Win32_Bios");

            bios.version = (string)obj["SMBIOSBIOSVersion"];
            bios.date = DateTime.ParseExact(((string)obj["ReleaseDate"]).Substring(0, ((string)obj["ReleaseDate"]).Length - 4), "yyyyMMddHHmmss.FFFFFF", null);

            return Task.FromResult(bios);
        }
    }
}
