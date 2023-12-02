using Microsoft.EntityFrameworkCore;
using powerLabel.Models;
using System;
using System.Linq;
using System.Windows;

namespace powerLabel
{
    public static class Database
    {
        public static void AddSystem(ComputerSystem system)
        {
            try
            {
                using (var db = new ComputerSystemContext())
                {
                    if (system.id != 0)
                    {
                        return;
                    }

                    ComputerSystem sysToInsert = new ComputerSystem();

                    db.ComputerSystems.Add(sysToInsert);
                    db.SaveChanges();
                    db.ComputerSystems.Attach(sysToInsert);


                    if (!db.Motherboards.ToList().Any(a => a.Equals(system.motherboard)))
                    {
                        db.Motherboards.Add(system.motherboard);
                        db.SaveChanges();
                    }

                    Motherboard mobo = db.Motherboards.ToList().Where(a => a.Equals(system.motherboard)).First();

                    sysToInsert.motherboard = mobo;

                    if (!db.Bioses.ToList().Any(a => a.Equals(system.bios)))
                    {
                        db.Bioses.Add(system.bios);
                        db.SaveChanges();
                    }

                    Bios bios = db.Bioses.ToList().Where(a => a.Equals(system.bios)).First();

                    sysToInsert.bios = bios;

                    if (!db.OSes.ToList().Any(a => a.Equals(system.operatingSystem)))
                    {
                        db.OSes.Add(system.operatingSystem);
                        db.SaveChanges();
                    }

                    OS os = db.OSes.ToList().Where(a => a.Equals(system.operatingSystem)).First();

                    sysToInsert.operatingSystem = os;

                    // Add processor if it doesnt exist in the database
                    if (!db.Processors.ToList().Any(a => a.Equals(system.processor)))
                    {
                        db.Processors.Add(system.processor);
                        db.SaveChanges();
                    }

                    Processor processor = db.Processors.ToList().Where(a => a.Equals(system.processor)).First();

                    sysToInsert.processorAmount = system.processorAmount;
                    sysToInsert.processor = processor;

                    foreach (MemoryConfig memoryConfig in system.memoryModules)
                    {
                        if (!db.MemoryModules.ToList().Any(a => a.Equals(memoryConfig.module)))
                        {
                            db.MemoryModules.Add(memoryConfig.module);
                            db.SaveChanges();
                        }
                        MemoryModule module = db.MemoryModules.ToList().Where(a => a.Equals(memoryConfig.module)).First();

                        db.MemoryConfigs.Add(new MemoryConfig() { module = module, currentClockspeed = memoryConfig.currentClockspeed, system = sysToInsert });
                    }


                    foreach (DiskConfig diskConfig in system.diskConfigs)
                    {
                        if (!db.Disks.ToList().Any(a => a.Equals(diskConfig.disk)))
                        {
                            db.Disks.Add(diskConfig.disk);
                            db.SaveChanges();
                        }
                        Disk disk = db.Disks.ToList().Where(a => a.Equals(diskConfig.disk)).First();

                        db.DiskConfigs.Add(new DiskConfig() { disk = disk, busType = diskConfig.busType, systemDisk = diskConfig.systemDisk, computerSystem = sysToInsert });
                    }

                    foreach (VideoControllerConfig videoControllerConfig in system.videoControllerConfigs)
                    {
                        if (!db.VideoControllerConfigs.Include(a => a.videoController).ToList().Any(a => a.videoController.Equals(videoControllerConfig.videoController)))
                        {
                            db.VideoControllers.Add(videoControllerConfig.videoController);
                            db.SaveChanges();
                        }
                        VideoController videoController = db.VideoControllers.ToList().Where(a => a.Equals(videoControllerConfig.videoController)).First();

                        db.VideoControllerConfigs.Add(new VideoControllerConfig() { videoController = videoController, computerSystem = sysToInsert });
                    }

                    db.ComputerSystems.Update(sysToInsert);

                    db.SaveChanges();

                    string employee = "";
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window.GetType() == typeof(MainWindow))
                        {
                            employee = (window as MainWindow).employeeDropdown.Text;
                        }
                    }
                    if (employee == "")
                    {
                        MessageBox.Show("No employee selected");
                        return;
                    }
                    db.Events.Add(Event.NewScanEvent(employee, DateTime.Now, db, sysToInsert));
                    db.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message);
            }
            
        }

    }
}
