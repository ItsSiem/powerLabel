using powerLabel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace powerLabel.ComponentProviders
{
    internal class DiskProvider : IComponentProvider<List<DiskConfig>>
    {
        public Task<List<DiskConfig>> GetComponentAsync()
        {
            List<DiskConfig> list = new List<DiskConfig>();

            uint osDiskId = 255;
            ManagementObjectCollection partitions = PSInterface.GetObjects("SELECT * FROM Win32_DiskPartition");
            foreach (ManagementObject item in partitions)
            {
                if ((bool)item["BootPartition"])
                {
                    osDiskId = (uint)item["DiskIndex"];
                }
            }

            ManagementScope scope = new ManagementScope("root\\Microsoft\\Windows\\Storage");
            scope.Connect();

            ObjectQuery query = new ObjectQuery("SELECT * FROM MSFT_PhysicalDisk");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

            ManagementObjectCollection returned = searcher.Get();
            foreach (ManagementObject item in returned)
            {
                DiskConfig diskConfig = new DiskConfig();

                diskConfig.disk.model = (string)item["Model"];
                diskConfig.disk.size = (ulong)item["Size"];
                diskConfig.busType = DiskConfig.busTypeEncoding[(ushort)item["busType"]];

                switch ((ushort)item["MediaType"])
                {
                    case 0:
                        diskConfig.disk.mediaType = "Unspecified";
                        break;
                    case 3:
                        diskConfig.disk.mediaType = "HDD";
                        break;
                    case 4:
                        diskConfig.disk.mediaType = "SSD";
                        break;
                    case 5:
                        diskConfig.disk.mediaType = "SCM";
                        break;
                    default:
                        break;
                }

                diskConfig.disk.serialNumber = (string)item["SerialNumber"];

                if (uint.Parse((string)item["DeviceId"]) == osDiskId)
                {
                    diskConfig.systemDisk = true;
                }

                if (diskConfig.busType == "USB")
                {
                    continue;
                }

                list.Add(diskConfig);
            }

            return Task.FromResult(list.OrderByDescending(disk => disk.systemDisk).ThenByDescending(disk => disk.disk.mediaType).ThenBy(disk => disk.disk.size).ToList());
        }
    }
}
