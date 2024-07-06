using System;
using System.Collections.Generic;
using System.Management;

namespace powerLabel
{
    public class MemoryConfig
    {
        public MemoryConfig()
        {

        }
        public MemoryConfig(ComputerSystem system)
        {
            module = new MemoryModule();
        }
        public int id { get; set; }
        public MemoryModule module { get; set; }
        public ComputerSystem system { get; set; }
        public uint currentClockspeed { get; set; }

        public static List<MemoryConfig> GetMemory(ComputerSystem system)
        {
            List<MemoryConfig> list = new List<MemoryConfig>();

            ManagementObjectCollection returned = PSInterface.RunObjectQuery("SELECT * FROM Win32_PhysicalMemory");
            foreach (ManagementObject item in returned)
            {
                MemoryConfig module = new MemoryConfig(system);

                if (!MemoryModule.memoryTypeLookup.ContainsKey((uint)item["SMBIOSMemoryType"]))
                {
                    continue;
                }

                module.module.capacity = (ulong)item["Capacity"];
                module.currentClockspeed = (item["ConfiguredClockSpeed"] == null) ? 0 : (uint)item["ConfiguredClockSpeed"];
                module.module.maxClockspeed = (uint)item["Speed"];
                module.module.formFactor = (UInt16)item["FormFactor"];
                module.module.memoryType = (uint)item["SMBIOSMemoryType"];

                module.module.partNubmer = (string)item["PartNumber"];
                module.module.serialNubmer = (string)item["SerialNumber"];

                list.Add(module);
            }
            return list;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            MemoryConfig memoryConfig = (MemoryConfig)obj;
            if (this.module.Equals(memoryConfig.module) &&
                this.currentClockspeed == memoryConfig.currentClockspeed
                )
            {
                return true;
            }
            return false;
        }
    }
}