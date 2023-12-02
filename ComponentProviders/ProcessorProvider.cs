using powerLabel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace powerLabel.ComponentProviders
{
    internal class ProcessorProvider : IComponentProvider<Processor>
    {
        public Task<Processor> GetComponentAsync()
        {
            Processor processor = new Processor();

            var item = PSInterface.GetObject("Win32_Processor");

            // Todo: Set amount of cpu's somewhere else
            //system.processorAmount = returned.Count > 1 ? 2 : 1;
            processor.name = (string)item["Name"];
            processor.cores = (uint)item["NumberOfCores"];
            processor.threads = (uint)item["NumberOfLogicalProcessors"];

            return Task.FromResult(processor);
        }
    }
}
