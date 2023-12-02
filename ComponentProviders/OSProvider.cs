using powerLabel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace powerLabel.ComponentProviders
{
    internal class OSProvider : IComponentProvider<OS>
    {
        public Task<OS> GetComponentAsync()
        {
            OS os = new OS();
            var item = PSInterface.GetObject("Win32_OperatingSystem");

            os.caption = (string)item["Caption"];
            os.language = OS.languageTable[(uint)item["OSLanguage"]];

            return Task.FromResult(os);
        }
    }
}
