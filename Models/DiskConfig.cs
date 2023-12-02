using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace powerLabel.Models
{
    public class DiskConfig
    {
        public DiskConfig()
        {
            disk = new Disk();
        }
        public DiskConfig(Disk disk, int busType)
        {
            this.disk = disk;
            this.busType = busTypeEncoding[busType];
        }
        public int id { get; set; }
        public Disk disk { get; set; }
        public ComputerSystem computerSystem { get; set; }
        public bool systemDisk { get; set; }
        public string busType { get; set; }

        public static string[] busTypeEncoding = {
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
            if (disk.Equals(diskConfig.disk) &&
                busType.Trim() == diskConfig.busType.Trim() &&
                systemDisk == diskConfig.systemDisk
                )
            {
                return true;
            }
            return false;
        }
    }
}