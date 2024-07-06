using System.Management;

namespace powerLabel
{
    public class ProcessorConfig
    {
        public int id { get; set; }
        public Processor processor { get; set; }
        public int amount { get; set; }
        public ComputerSystem computerSystem { get; set; }

        public static ProcessorConfig GetProcessors(ComputerSystem system)
        {
            ProcessorConfig processorConfig = new ProcessorConfig();

            ManagementObjectCollection returned = PSInterface.RunObjectQuery("SELECT * FROM Win32_Processor");

            ManagementObject item = null;

            foreach (ManagementObject obj in returned)
            {
                item = obj;
                break;
            }

            processorConfig.amount = (returned.Count > 1) ? 2 : 1;
            processorConfig.processor = new Processor();
            processorConfig.processor.name = (string)item["Name"];
            processorConfig.processor.cores = (uint)item["NumberOfCores"];
            processorConfig.processor.threads = (uint)item["NumberOfLogicalProcessors"];
            processorConfig.computerSystem = system;
            
            return processorConfig;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            ProcessorConfig processorConfig = (ProcessorConfig)obj;
            if (this.processor.Equals(processorConfig.processor) &&
                this.amount == processorConfig.amount &&
                this.computerSystem.motherboard.Equals(processorConfig.computerSystem.motherboard)
                )
            {
                return true;
            }
            return false;
        }
    }
}