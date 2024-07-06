using System.Management;

namespace powerLabel
{
    public class Processor
    {
        public int id { get; set; }
        public string name { get; set; }
        public uint cores { get; set; }
        public uint threads { get; set; }

        public static Processor GetProcessor(ComputerSystem system)
        {
            Processor processor = new Processor();

            ManagementObjectCollection returned = PSInterface.RunObjectQuery("SELECT * FROM Win32_Processor");

            ManagementObject item = null;

            foreach (ManagementObject obj in returned)
            {
                item = obj;
                break;
            }

            system.processorAmount = (returned.Count > 1) ? 2 : 1;
            processor.name = (string)item["Name"];
            processor.cores = (uint)item["NumberOfCores"];
            processor.threads = (uint)item["NumberOfLogicalProcessors"];

            return processor;
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            Processor processor = (Processor)obj;
            if (this.name.Trim() == processor.name.Trim()
                )
            {
                return true;
            }
            return false;
        }

    }
}
