using System;
using System.Management;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace powerLabel
{
    public class LabelPrinting
    {
        public static void printLabel(Grid grid)
        {
            try
            {
                // Add printer event
                using (var db = new ComputerSystemContext())
                {
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
                    db.Attach(ComputerSystem.system);
                    db.Events.Add(Event.NewPrintEvent(employee, DateTime.Now, db, ComputerSystem.system));
                    db.SaveChanges();
                }

                // Add printer
                using (ManagementClass win32Printer = new ManagementClass("Win32_Printer"))
                {
                    using (ManagementBaseObject inputParam =
                       win32Printer.GetMethodParameters("AddPrinterConnection"))
                    {
                        inputParam.SetPropertyValue("Name", $"\\\\{SettingsHandler.ReadSettings().printerHost}\\{SettingsHandler.ReadSettings().printerShareName}");

                        using (ManagementBaseObject result =
                            (ManagementBaseObject)win32Printer.InvokeMethod("AddPrinterConnection", inputParam, null))
                        {
                            uint errorCode = (uint)result.Properties["returnValue"].Value;

                            switch (errorCode)
                            {
                                case 0:
                                    Console.Out.WriteLine("Successfully connected printer.");
                                    break;
                                case 5:
                                    Console.Out.WriteLine("Access Denied.");
                                    break;
                                case 123:
                                    Console.Out.WriteLine("The filename, directory name, or volume label syntax is incorrect.");
                                    break;
                                case 1801:
                                    Console.Out.WriteLine("Invalid Printer Name.");
                                    break;
                                case 1930:
                                    Console.Out.WriteLine("Incompatible Printer Driver.");
                                    break;
                                case 3019:
                                    Console.Out.WriteLine("The specified printer driver was not found on the system and needs to be downloaded.");
                                    break;
                            }
                        }
                    }
                }

                FrameworkElement e = grid as System.Windows.FrameworkElement;
                if (e == null)
                    return;

                PrintDialog pd = new PrintDialog();
                PrintServer myPrintServer = new PrintServer($"\\\\{SettingsHandler.ReadSettings().printerHost}");

                // List the print server's queues
                PrintQueueCollection myPrintQueues = myPrintServer.GetPrintQueues();
                foreach (PrintQueue pq in myPrintQueues)
                {
                    pd.PrintQueue = pq;
                }
                pd.PrintTicket.PageMediaSize = new PageMediaSize(216, 120);


                //store original scale
                Transform originalScale = e.LayoutTransform;
                //get selected printer capabilities
                PrintCapabilities capabilities = pd.PrintQueue.GetPrintCapabilities(pd.PrintTicket);

                //get scale of the print wrt to screen of WPF visual
                double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / e.ActualWidth, capabilities.PageImageableArea.ExtentHeight /
                               e.ActualHeight);

                //Transform the Visual to scale
                e.LayoutTransform = new ScaleTransform(scale, scale);

                //get the size of the printer page
                System.Windows.Size sz = new System.Windows.Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);

                //update the layout of the visual to the printer page size.
                e.Measure(sz);
                e.Arrange(new System.Windows.Rect(new System.Windows.Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), sz));

                //now print the visual to printer to fit on the one page.
                pd.PrintVisual(grid, "My Print");

                //apply the original transform.
                e.LayoutTransform = originalScale;

                // Remove printer
                ConnectionOptions options = new ConnectionOptions();
                options.EnablePrivileges = true;
                ManagementScope scope = new ManagementScope(ManagementPath.DefaultPath, options);
                scope.Connect();
                ManagementClass printerClass = new ManagementClass("Win32_Printer");
                ManagementObjectCollection printers = printerClass.GetInstances();
                foreach (ManagementObject printer in printers)
                {
                    if ((string)printer["ShareName"] == SettingsHandler.ReadSettings().printerShareName)
                    {
                        printer.Delete();
                    }
                }
        }
            catch (Exception ex)
            {
                MessageBox.Show("Printing process returned an error: " + ex.Message);
            }
}

        public static string formatString(string str)
        {
            // Replaces spaces with non braking spaces and periods with zerowidth spaces
            str = str.Replace(" ", "\u00a0").Replace(".", "\u200B");
            return str;
        }

    }    
}
