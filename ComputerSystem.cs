using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace powerLabel
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

        public static ComputerSystem GetSystem()
        {
            system = new ComputerSystem();
            system.motherboard = Motherboard.GetMotherboard();
            system.bios = Bios.GetBios();
            system.processor = Processor.GetProcessor(system);
            system.memoryModules = MemoryConfig.GetMemory(system);
            system.diskConfigs = DiskConfig.GetDisks(system);
            system.videoControllerConfigs = VideoControllerConfig.GetVideoControllers(system);
            system.operatingSystem = OS.GetOS();

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
            ComputerSystem.system = system;
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
                @"HP Z\w+( \d{2}\w? G\d)?",     // HP Systems (HP Z840, HP ZBook 15 G3, HP ZBook 14U G5)
                @"Precision \w* \w*"            // DELL Systems (Precision WorkStation T3500, Precision Tower 3620)
            });

            // CPU Label string processing
            string cpuString = processor.name;
            cpuString = getShortString(cpuString, new string[] {
                @"(((Platinum)|(Gold)|(Silver)|(Bronze)) \w*-*\d+\w*)|(\w+-*\d{3,}\w*( v\d)*)" // regex voor vrijwel alle core i en xeon processoren vanaf 2006
            });
            if (processorAmount > 1)
            {
                cpuString = cpuString.Insert(0, "2x ");
            }

            // RAM Label string processing
            string ramString = memoryModules.Sum(item => Convert.ToInt64(item.module.capacity)) / 1073741824 + "GB " + MemoryModule.memoryTypeLookup[memoryModules.First().module.memoryType];

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
                    @"Quadro (RTX )*\w+",           // Quadro's (Quadro RTX 4000, Quadro K2200, Quadro M2000M)
                    @"GeForce \wTX \d{3,}( \w+)*"   // Nvidia GeForce GTX / RTX 3060 Ti
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
                    return item.Match(input).Groups[0].Value;
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
                if (((this.motherboard == null && sys.motherboard == null) || this.motherboard.Equals(sys.motherboard)) &&
                ((this.bios == null && sys.bios == null) || this.bios.Equals(sys.bios)) &&
                (this.processorAmount == sys.processorAmount) &&
                ((this.processor == null && sys.processor == null) || this.processor.Equals(sys.processor)) &&
                ((this.memoryModules == null && sys.memoryModules == null) || this.memoryModules.SequenceEqual(sys.memoryModules)) &&
                ((this.diskConfigs == null && sys.diskConfigs == null) || this.diskConfigs.SequenceEqual(sys.diskConfigs)) &&
                ((this.videoControllerConfigs == null && sys.videoControllerConfigs == null) || this.videoControllerConfigs.SequenceEqual(sys.videoControllerConfigs)) &&
                ((this.operatingSystem == null && sys.operatingSystem == null) || this.operatingSystem.Equals(sys.operatingSystem))
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
