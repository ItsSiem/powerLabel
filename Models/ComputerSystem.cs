using Microsoft.EntityFrameworkCore;
using powerLabel.ComponentProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace powerLabel.Models
{
    public class ComputerSystem
    {
        public int id { get; set; }
        public Motherboard motherboard { get; set; }
        public Bios bios { get; set; }
        public int processorAmount { get; set; }
        public Processor processor { get; set; }
        public List<MemoryConfig> memoryModules { get; set; }
        public List<DiskConfig> diskConfigs { get; set; }
        public List<VideoControllerConfig> videoControllerConfigs { get; set; }
        public OS operatingSystem { get; set; }

        public static ComputerSystem system { get; set; }

        public static async Task<ComputerSystem> GetSystemAsync()
        {
            system = new ComputerSystem();
            system.motherboard = await new MotherboardProvider().GetComponentAsync();
            system.bios = await new BiosProvider().GetComponentAsync();
            system.processor = await new ProcessorProvider().GetComponentAsync();
            system.memoryModules = await new MemoryProvider().GetComponentAsync();
            system.diskConfigs = await new DiskProvider().GetComponentAsync();
            system.videoControllerConfigs = VideoControllerConfig.GetVideoControllers(system);
            system.operatingSystem = await new OSProvider().GetComponentAsync();

            using (var db = new ComputerSystemContext())
            {
                if (db.ComputerSystems
                        .Include(a => a.motherboard)
                        .Include(a => a.bios)
                        .Include(a => a.processor)
                        .Include(a => a.memoryModules).ThenInclude(a => a.module)
                        .Include(a => a.diskConfigs).ThenInclude(a => a.disk)
                        .Include(a => a.videoControllerConfigs).ThenInclude(a => a.videoController)
                        .Include(a => a.operatingSystem)
                        .ToList()
                        .Any(a => a.Equals(system)))
                {
                    //This system config has been scanned before

                    //List of all the events with this motherboard ordered by date
                    List<Event> events = db.Events.ToList().Where(a => a.computerSystem.motherboard.id == db.Motherboards.ToList().Where(b => b.Equals(system.motherboard)).First().id).ToList().OrderBy(a => a.date).ToList();

                    //The last config this motherboard had
                    ComputerSystem lastConfig = db.ComputerSystems.ToList().Where(a => a.id == events.Last().computerSystem.id).Single();

                    if (system.Equals(lastConfig))
                    {
                        //The config hasn't changed since the last system scan, so set the system to the last config (in order to have the correct id's)
                        system = lastConfig;
                    }
                }
            }
            system = system;
            return system;
        }

        public string getString()
        {
            if (system == null)
            {
                return "";
            }

            // Model Label string processing
            string modelString = motherboard.model;
            modelString = getShortString(modelString, new string[] {
                @"(?<ZLine>HP Z\w+)(?:(?!G)[a-zA-Z ])*(?<screensize>\d{2}\w?)?(?:(?![G])[A-Za-z .\d])*(?<generation>G\d)?",     // HP Systems (HP Z840, HP ZBook 15 G3, HP ZBook 14U G5)
                @"Precision \w* \w*"            // DELL Systems (Precision WorkStation T3500, Precision Tower 3620)
            });

            // CPU Label string processing
            string cpuString = processor.name;
            cpuString = getShortString(cpuString, new string[] {
                @"(Platinum|Gold|Silver|Bronze)(?: )(\w*-*\d+\w*)", // Xeon gold, silver etc.
                @"(\w+-*\d{3,}\w*)(?: )*(v\d)*", // Core i and Xeon non metal
            });
            if (processorAmount > 1)
            {
                cpuString = cpuString.Insert(0, "2x ");
            }

            // RAM Label string processing
            string ramString = memoryModules.Sum(item => Convert.ToInt64(item.module.capacity)) / 1073741824 + "GB (" + memoryModules.Count + ") " + MemoryModule.memoryTypeLookup[memoryModules.First().module.memoryType];

            // Disk Label string processing
            string diskString = "";
            List<string> disks = new List<string>();
            List<string> doneDisks = new List<string>();
            foreach (DiskConfig disk in system.diskConfigs)
            {
                disks.Add(disk.ToString());
            }
            foreach (string disk in disks)
            {
                if (!doneDisks.Any(a => a == disk))
                {
                    if (disks.Where(a => a == disk).Count() > 1)
                    {
                        int multiplier = disks.Where(a => a == disk).Count();
                        diskString += $"{multiplier}x {disk}.";
                        doneDisks.Add(disk);
                    }
                    else
                    {
                        diskString += disk + ".";
                        doneDisks.Add(disk);
                    }
                }

            }


            // GPU Label string processing
            string gpuString = "";
            foreach (VideoControllerConfig gpu in videoControllerConfigs)
            {
                gpuString += getShortString(gpu.videoController.name, new string[] {
                    @"\w{2,3} Graphics \w+",        // Intel intergrated graphics (HD Graphics 405, Pro Graphics 600)
                    @"(Quadro|RTX) *(\w+) ?(\d+)?",           // Quadro's (Quadro RTX 4000, Quadro K2200, Quadro M2000M)
                    @"(GeForce) (\wTX?) (\d{3,})(?: (\w+))*"   // Nvidia GeForce GTX / RTX 3060 Ti
                }) + "\r\n";
            }

            // Final string formatting
            // '.' will be replaced by zero width space
            // spaces will be replaced with a non brakeable space
            return modelString + "\r\n." + cpuString + " | " + ramString + "\r\n." + diskString + " ." + gpuString;
        }

        public static string getShortString(string input, string[] regexes)
        {
            Regex[] regex = new Regex[regexes.Length];
            int i = 0;
            foreach (string str in regexes)
            {
                regex[i] = new Regex(@str);
                i++;
            }
            foreach (Regex item in regex)
            {
                if (item.IsMatch(input))
                {
                    var match = item.Match(input);
                    if (match.Groups.Count == 1)
                        return match.Groups[0].Value;
                    var s = "";
                    for (int j = 1; j < match.Groups.Count; j++)
                    {
                        s += match.Groups[j].Value;
                        if (j < match.Groups.Count)
                        {
                            s += " ";
                        }
                    }
                    return s;
                }
            }
            return input;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            ComputerSystem sys = (ComputerSystem)obj;
            try
            {
                if ((motherboard == null && sys.motherboard == null || motherboard.Equals(sys.motherboard)) &&
                (bios == null && sys.bios == null || bios.Equals(sys.bios)) &&
                processorAmount == sys.processorAmount &&
                (processor == null && sys.processor == null || processor.Equals(sys.processor)) &&
                (memoryModules == null && sys.memoryModules == null || memoryModules.SequenceEqual(sys.memoryModules)) &&
                (diskConfigs == null && sys.diskConfigs == null || diskConfigs.SequenceEqual(sys.diskConfigs)) &&
                (videoControllerConfigs == null && sys.videoControllerConfigs == null || videoControllerConfigs.SequenceEqual(sys.videoControllerConfigs)) &&
                (operatingSystem == null && sys.operatingSystem == null || operatingSystem.Equals(sys.operatingSystem))
                )
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
