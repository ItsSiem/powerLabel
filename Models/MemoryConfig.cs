using System;
using System.Collections.Generic;
using System.Management;

namespace powerLabel.Models
{
    public class MemoryConfig
    {
        public int id { get; set; }
        public MemoryModule module { get; set; }
        public ComputerSystem system { get; set; }
        public uint currentClockspeed { get; set; }

        public MemoryConfig()
        {
            module = new MemoryModule();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            MemoryConfig memoryConfig = (MemoryConfig)obj;
            if (module.Equals(memoryConfig.module) &&
                currentClockspeed == memoryConfig.currentClockspeed
                )
            {
                return true;
            }
            return false;
        }
    }
}