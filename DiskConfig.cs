using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace powerLabel
{
    public class DiskConfig
    {
        public DiskConfig()
        {

        }
        public DiskConfig(ComputerSystem computerSystem)
        {
            disk = new Disk();
            this.computerSystem = computerSystem;
        }
        public DiskConfig(Disk disk, ComputerSystem computerSystem, int busType)
        {
            this.disk = disk;
            this.computerSystem = computerSystem;
            this.busType = busTypeEncoding[busType];
        }
        public int id { get; set; }
        public Disk disk { get; set; }
        public ComputerSystem computerSystem { get; set; }
        public bool systemDisk { get; set; }
        public string busType { get; set; }

        private static string[] busTypeEncoding = {
            "Unknown",
            "SCSI",
            "ATAPI",
            "ATA",
            "IEEE 1394",
            "SSA",
            "Fibre Channel",
            "USB",
            "RAID",
            "iSCSI",
            "SAS",
            "SATA",
            "SD",
            "MMC",
            "MAX",
            "File Backed Virtual",
            "Storage Spaces",
            "NVMe"
        };

        public static List<DiskConfig> GetDisks(ComputerSystem system)
        {
            List<DiskConfig> list = new List<DiskConfig>();

            uint osDiskId = 255;
            ManagementObjectCollection partitions = PSInterface.RunObjectQuery("SELECT * FROM Win32_DiskPartition");
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
                DiskConfig diskConfig = new DiskConfig(system);

                diskConfig.disk.model = (string)item["Model"];
                diskConfig.disk.size = (ulong)item["Size"];
                diskConfig.busType = busTypeEncoding[(UInt16)item["busType"]];

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

                if (UInt32.Parse((string)item["DeviceId"]) == osDiskId)
                {
                    diskConfig.systemDisk = true;
                }

                if (diskConfig.busType == "USB")
                {
                    continue;
                }

                list.Add(diskConfig);
            }

            return list.OrderByDescending(disk => disk.systemDisk).ThenByDescending(disk => disk.disk.mediaType).ThenBy(disk => disk.disk.size).ToList();
        }

        public override string ToString()
        {
            ulong shortSize = disk.size / 1000000000;
            string unit;
            string os = "";

            if (shortSize.ToString().Length == 3)
            {
                unit = "GB";
            }
            else
            {
                unit = "TB";
                shortSize = shortSize / 1000;
            }

            if (systemDisk)
            {
                if (computerSystem.operatingSystem.caption.Contains("10"))
                {
                    os = "+ W10P";
                }
                else if (computerSystem.operatingSystem.caption.Contains("11"))
                {
                    os = "+ W11P";
                }

                if (computerSystem.operatingSystem.language != "NL")
                {
                    os += " " + computerSystem.operatingSystem.language;
                }
            }

            if (disk.mediaType == "SSD")
            {
                if (busType == "NVMe")
                {
                    return $"{shortSize}{unit} {busType} {os} ";
                }
                return $"{shortSize}{unit} {busType} {disk.mediaType} {os} ";
            }
            if (disk.mediaType == "HDD")
            {
                return $"{shortSize}{unit} {disk.mediaType} {os} ";
            }
            return $"{shortSize}{unit} {disk.mediaType} {os} ";
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            DiskConfig diskConfig = (DiskConfig)obj;
            if (this.disk.Equals(diskConfig.disk) &&
                this.busType.Trim() == diskConfig.busType.Trim() &&
                this.systemDisk == diskConfig.systemDisk
                )
            {
                return true;
            }
            return false;
        }
    }
}