using System.Collections.Generic;

namespace powerLabel.Models
{
    public class MemoryModule
    {
        public int id { get; set; }
        public ulong capacity { get; set; }

        public uint maxClockspeed { get; set; }

        public uint formFactor { get; set; }          // 8 -> DIMM  12 -> SODIMM https://docs.microsoft.com/en-us/windows/win32/cimwin32prov/cim-chip

        public uint memoryType { get; set; }          // https://www.dmtf.org/sites/default/files/standards/documents/DSP0134_3.2.0.pdf Table 76

        public static Dictionary<uint, string> memoryTypeLookup = new Dictionary<uint, string>()
        {
            [18] = "DDR",
            [19] = "DDR2",
            [20] = "DDR2 FB-DIMM",
            [24] = "DDR3",
            [26] = "DDR4",
            [27] = "LPDDR",
            [28] = "LPDDR2",
            [29] = "LPDDR3",
            [30] = "LPDDR4"
        };

        public string partNubmer { get; set; }
        public string serialNubmer { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            MemoryModule memory = (MemoryModule)obj;
            if (capacity == memory.capacity &&
                maxClockspeed == memory.maxClockspeed &&
                formFactor == memory.formFactor &&
                memoryType == memory.memoryType &&
                partNubmer.Trim() == memory.partNubmer.Trim() &&
                serialNubmer.Trim() == memory.serialNubmer.Trim()
                )
            {
                return true;
            }
            return false;
        }
    }
}
