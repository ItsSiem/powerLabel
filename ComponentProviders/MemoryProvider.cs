using powerLabel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace powerLabel.ComponentProviders
{
    internal class MemoryProvider : IComponentProvider<List<MemoryConfig>>
    {
        public Task<List<MemoryConfig>> GetComponentAsync()
        {
            List<MemoryConfig> list = new List<MemoryConfig>();

            ManagementObjectCollection returned = PSInterface.GetObjects("SELECT * FROM Win32_PhysicalMemory");
            foreach (ManagementObject item in returned)
            {
                MemoryConfig module = new MemoryConfig();

                if (!MemoryModule.memoryTypeLookup.ContainsKey((uint)item["SMBIOSMemoryType"]))
                    continue;

                module.module.capacity = (ulong)item["Capacity"];
                module.currentClockspeed = item["ConfiguredClockSpeed"] == null ? 0 : (uint)item["ConfiguredClockSpeed"];
                module.module.maxClockspeed = (uint)item["Speed"];
                module.module.formFactor = (ushort)item["FormFactor"];
                module.module.memoryType = (uint)item["SMBIOSMemoryType"];

                module.module.partNubmer = (string)item["PartNumber"];
                module.module.serialNubmer = (string)item["SerialNumber"];

                list.Add(module);
            }
            return Task.FromResult(list);
        }
    }
}
