using AutoUpdaterDotNET;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace powerLabel
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            AutoUpdater.Mandatory = true;
            AutoUpdater.UpdateMode = Mode.Forced;
            AutoUpdater.Start("https://github.com/ItsSiem/powerLabel/releases/latest/download/versions.xml");
            InitializeComponent();
            refreshSettings();
        }

        private void refreshSettings()
        {
            SettingsHandler.Settings settings = SettingsHandler.ReadSettings();
            employeeDropdown.Items.Clear();
            foreach (string item in settings.employees)
            {
                ListViewItem listViewItem = new ListViewItem();
                listViewItem.Content = item;
                employeeDropdown.Items.Add(listViewItem);
            }

        }

        private void scanSystem(object sender, RoutedEventArgs e)
        {
            try
            {
                ComputerSystem.GetSystem();
                ComputerSystem system = ComputerSystem.system;

                // Model Label string processing
                string modelString = system.motherboard.model;
                modelString = ComputerSystem.getShortString(modelString, new string[] {
                @"(?<ZLine>HP Z\w+)(?:(?!G)[a-zA-Z ])*(?<screensize>\d{2}\w?)?(?:(?![G])[A-Za-z .\d])*(?<generation>G\d)?",     // HP Systems (HP Z840, HP ZBook 15 G3, HP ZBook 14U G5)
                @"Precision \w* \w*"            // DELL Systems (Precision WorkStation T3500, Precision Tower 3620)
                });

                // CPU Label string processing
                string cpuString = system.processor.name;
                cpuString = ComputerSystem.getShortString(cpuString, new string[] {
                @"(Platinum|Gold|Silver|Bronze)(?: )(\w*-*\d+\w*)", // Xeon gold, silver etc.
                @"(\w+-*\d{3,}\w*)(?: )*(v\d)*", // Core i and Xeon non metal
                });
                if (system.processorAmount > 1)
                {
                    cpuString = cpuString.Insert(0, "2x ");
                }

                // RAM Label string processing
                string ramString = system.memoryModules.Sum(item => Convert.ToInt64(item.module.capacity)) / 1073741824 + "GB " + MemoryModule.memoryTypeLookup[system.memoryModules.First().module.memoryType];

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
                            diskString += $"{multiplier}x {disk}\r\n";
                            doneDisks.Add(disk);
                        }
                        else
                        {
                            diskString += disk + "\r\n";
                            doneDisks.Add(disk);
                        }
                    }

                }


                // GPU Label string processing
                string gpuString = "";
                foreach (VideoControllerConfig gpu in system.videoControllerConfigs)
                {
                    gpuString += ComputerSystem.getShortString(gpu.videoController.name, new string[] {
                    @"\w{2,3} Graphics \w+",        // Intel intergrated graphics (HD Graphics 405, Pro Graphics 600)
                    @"(Quadro|RTX) *(\w+) ?(\d+)?",           // Quadro's (Quadro RTX 4000, Quadro K2200, Quadro M2000M)
                    @"(GeForce) (\wTX?) (\d{3,})(?: (\w+))*"   // Nvidia GeForce GTX / RTX 3060 Ti
                }) + "\r\n";
                }



                modelField.Content = modelString;
                cpuField.Content = cpuString;
                ramField.Content = ramString;
                disksField.Content = diskString;
                gpuField.Content = gpuString;

                int fontsize = 18;
                cpuField.FontSize = fontsize;
                ramField.FontSize = fontsize;
                disksField.FontSize = fontsize;
                gpuField.FontSize = fontsize;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured while parsing the system specifications. \r\n {ex}");
            }

            Database.AddSystem(ComputerSystem.system);
            ComputerSystem.GetSystem();
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
            updateEvents();
        }

        private void settingsBtn_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settWindow = new SettingsWindow();
            settWindow.ShowDialog();
            refreshSettings();
        }

        private void printBtn_Click(object sender, RoutedEventArgs e)
        {
            LabelPrinting.printLabel(labelGrid);
            updateEvents();
        }


        private void drawLabelPreview()
        {
            labelText.Text = LabelPrinting.formatString(ComputerSystem.system.getString());
            boldCheckbox_Changed(new object(), new RoutedEventArgs());
        }


        private void partSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            componentGrid.Children.Clear();
            componentGrid.RowDefinitions.Clear();
            componentListBox.Items.Clear();
            object component = null;
            if (ComputerSystem.system == null)
            {
                return;
            }
            switch (partSelector.SelectedIndex)
            {
                case 0:
                    //Motherboard
                    component = ComputerSystem.system.motherboard;
                    break;

                case 1:
                    //Bios
                    component = ComputerSystem.system.bios;
                    break;

                case 2:
                    //Processor
                    component = ComputerSystem.system.processor;
                    break;

                case 3:
                    //Memory
                    component = ComputerSystem.system.memoryModules;
                    break;

                case 4:
                    //Disks
                    component = ComputerSystem.system.diskConfigs;
                    break;

                case 5:
                    //VideoControllers
                    component = ComputerSystem.system.videoControllerConfigs;
                    break;

                case 6:
                    //OperatingSystem
                    component = ComputerSystem.system.operatingSystem;
                    break;

                default:
                    return;

            }

            var enumerable = component as System.Collections.IEnumerable;
            int propertyIndex = 0;
            int itemIndex = 0;
            if (enumerable != null)
            {
                foreach (var item in enumerable)
                {
                    ListBoxItem listItem = new ListBoxItem();
                    listItem.Content = itemIndex;
                    componentListBox.Items.Add(listItem);
                    itemIndex++;

                }
            }
            else
            {
                foreach (PropertyInfo propertyInfo in component.GetType().GetProperties())
                {
                    componentGrid.RowDefinitions.Add(new RowDefinition());
                    Label label = new Label();
                    label.Content = propertyInfo.Name;
                    componentGrid.Children.Add(label);
                    Grid.SetRow(label, propertyIndex);
                    Grid.SetColumn(label, 0);

                    Label value = new Label();
                    value.Content = propertyInfo.GetValue(component);
                    componentGrid.Children.Add(value);
                    Grid.SetRow(value, propertyIndex);
                    Grid.SetColumn(value, 1);

                    propertyIndex++;
                }
            }
        }

        private void componentListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            componentGrid.Children.Clear();
            componentGrid.RowDefinitions.Clear();

            if (componentListBox.SelectedItem == null)
            {
                return;
            }

            if (ComputerSystem.system == null)
            {
                return;
            }

            int selectedIndex = (int)((ListBoxItem)componentListBox.SelectedItem).Content;
            object component = null;

            switch (partSelector.SelectedIndex)
            {
                case 0:
                    //Motherboard
                    component = ComputerSystem.system.motherboard;
                    break;

                case 1:
                    //Bios
                    component = ComputerSystem.system.bios;
                    break;

                case 2:
                    //Processor
                    component = ComputerSystem.system.processor;
                    break;

                case 3:
                    //Memory
                    component = ComputerSystem.system.memoryModules;
                    break;

                case 4:
                    //Disks
                    component = ComputerSystem.system.diskConfigs;
                    break;

                case 5:
                    //VideoControllers
                    component = ComputerSystem.system.videoControllerConfigs;
                    break;

                case 6:
                    //OperatingSystem
                    component = ComputerSystem.system.operatingSystem;
                    break;
                default:
                    break;
            }

            var enumerable = component as System.Collections.IList;
            int propertyIndex = 0;

            foreach (PropertyInfo propertyInfo in enumerable[selectedIndex].GetType().GetProperties())
            {
                componentGrid.RowDefinitions.Add(new RowDefinition());
                Label label = new Label();
                label.Content = propertyInfo.Name;
                componentGrid.Children.Add(label);
                Grid.SetRow(label, propertyIndex);
                Grid.SetColumn(label, 0);

                Label value = new Label();
                value.Content = propertyInfo.GetValue(enumerable[selectedIndex]);
                componentGrid.Children.Add(value);
                Grid.SetRow(value, propertyIndex);
                Grid.SetColumn(value, 1);

                propertyIndex++;
            }
        }

        private void scanButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                scanSystem(sender, e);
                leftPanelGrid.Visibility = Visibility.Visible;
                drawLabelPreview();
            }
            catch (Exception ex)
            {
                MessageBox.Show("error: " + ex.Message);
            }

        }

        private void employeeDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (employeeDropdown.Text == "")
            {
                scanButton.IsEnabled = false;
                printBtn.IsEnabled = false;
                OrderSave.IsEnabled = false;
            }
            else
            {
                scanButton.IsEnabled = true;
                printBtn.IsEnabled = true;
                OrderSave.IsEnabled = true;
            }
        }

        private void employeeDropdown_DropDownClosed(object sender, EventArgs e)
        {
            if (employeeDropdown.Text == "")
            {
                scanButton.IsEnabled = false;
                printBtn.IsEnabled = false;
                OrderSave.IsEnabled = false;
            }
            else
            {
                scanButton.IsEnabled = true;
                printBtn.IsEnabled = true;
                OrderSave.IsEnabled = true;
            }
        }

        private void OrderSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new ComputerSystemContext())
                {
                    string employee = "";
                    employee = employeeDropdown.Text;
                    if (employee == "")
                    {
                        MessageBox.Show("No employee selected");
                        return;
                    }
                    db.Attach(ComputerSystem.system);
                    db.Events.Add(Event.NewMarkedAsOrderEvent(employee, DateTime.Now, db, ComputerSystem.system, orderNote.Text));
                    db.SaveChanges();
                }

                MessageBox.Show("Saved Order");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An exeption was encountered while saving the order.\r\n{ex}");
            }
            updateEvents();

        }

        private void updateEvents()
        {
            using (var db = new ComputerSystemContext())
            {
                db.ComputerSystems.Attach(ComputerSystem.system);

                eventsStackpanel.Children.Clear();
                foreach (Event evnt in db.Events.Where(a => a.computerSystem.id == ComputerSystem.system.id))
                {
                    StackPanel stackPanel = new StackPanel();
                    stackPanel.Children.Add(new Label { Content = evnt.name });
                    stackPanel.Children.Add(new Label { Content = evnt.employee });
                    stackPanel.Children.Add(new Label { Content = evnt.date.ToShortDateString() });
                    Border border = new Border
                    {
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffe6cc")),
                        CornerRadius = new CornerRadius(8),
                        BorderThickness = new Thickness(2),
                        Margin = new Thickness(5, 0, 10, 5)
                    };
                    border.Child = stackPanel;
                    eventsStackpanel.Children.Add(border);
                }

                configHistoryStackPanel.Children.Clear();
                foreach (ComputerSystem config in db.ComputerSystems.Where(a => a.motherboard.id == ComputerSystem.system.motherboard.id))
                {
                    Ellipse ellipse = new Ellipse
                    {
                        Width = 10,
                        Height = 10,
                        StrokeThickness = 2,
                        Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                        Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = new Thickness(5, 30, 5, 50)
                    };

                    configHistoryStackPanel.Children.Add(ellipse);

                }
            }
        }

        private void reload_Click(object sender, RoutedEventArgs e)
        {
            recentSystemsPanel.Children.Clear();
            using (var db = new ComputerSystemContext())
            {
                List<Event> events = db.Events.Where(a => Regex.IsMatch(a.name, ".*scan.*")).OrderByDescending(b => b.date).Include(c => c.computerSystem).Take(50).ToList();
                Event prevEvent = new Event { date = DateTime.Parse("1990-01-01") };

                foreach (Event evnt in events)
                {
                    ComputerSystem system = db.ComputerSystems
                        .Where(a => a.id == evnt.computerSystem.id)
                        .Include(a => a.motherboard)
                        .Include(a => a.bios)
                        .Include(a => a.processor)
                        .Include(a => a.memoryModules).ThenInclude(a => a.module)
                        .Include(a => a.diskConfigs).ThenInclude(a => a.disk)
                        .Include(a => a.videoControllerConfigs).ThenInclude(a => a.videoController)
                        .Include(a => a.operatingSystem)
                        .ToList()
                        .First();

                    string time = evnt.date.ToShortTimeString();

                    if (evnt.date.ToShortDateString() != prevEvent.date.ToShortDateString())
                    {
                        StackPanel dayMarker = new StackPanel { Orientation = Orientation.Horizontal };
                        dayMarker.Children.Add(new Separator { Width = 300 });
                        dayMarker.Children.Add(new TextBlock { Text = evnt.date.ToShortDateString(), HorizontalAlignment = HorizontalAlignment.Right });

                        recentSystemsPanel.Children.Add(dayMarker);
                        /*
                                <StackPanel Orientation="Horizontal">
                                    <Separator Width="300"/>
                                    <TextBlock Text="2022-06-23" HorizontalAlignment="Right"/>
                                </StackPanel>
                        */
                    }

                    StackPanel stackPanel = new StackPanel { Margin = new Thickness(5) };
                    stackPanel.Children.Add(new TextBlock
                    {
                        Text = $"{time} - {evnt.employee}",
                        FontSize = 9,
                        Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4C4C4C"))
                    });
                    stackPanel.Children.Add(new TextBlock { Text = system.motherboard.model });
                    stackPanel.Children.Add(new TextBlock { Text = $"{system.processorAmount}x {system.processor.name}" });
                    stackPanel.Children.Add(new TextBlock { Text = $"{system.memoryModules.Sum(a => Convert.ToInt64(a.module.capacity)) / 1073741824}GB {MemoryModule.memoryTypeLookup[system.memoryModules.First().module.memoryType]} {system.memoryModules.First().currentClockspeed}MT/s ({system.memoryModules.Count()})" });
                    foreach (DiskConfig disk in system.diskConfigs)
                    {
                        stackPanel.Children.Add(new TextBlock { Text = $"{disk.ToString()}" });
                    }
                    foreach (VideoControllerConfig video in system.videoControllerConfigs)
                    {
                        stackPanel.Children.Add(new TextBlock { Text = $"{video.videoController.name}" });
                    }
                    stackPanel.Children.Add(new TextBlock { Text = system.motherboard.serialNumber });

                    if (db.Events.Where(a => a.computerSystem.id == system.id).Any(b => Regex.IsMatch(b.name, ".*order.*")))
                    {
                        stackPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#90EE90"));
                    }

                    recentSystemsPanel.Children.Add(stackPanel);

                    prevEvent = evnt;
                }


            }

            /*
                                <StackPanel Margin="5">
                                    <TextBlock Text="10:45 - Siem" FontSize="9" Foreground="#FF4C4C4C"/>
                                    <TextBlock Text="HP Z840"/>
                                    <TextBlock Text="2x 2673v4"/>
                                    <TextBlock Text="128GB DDR4 2400MT/s (4)"/>
                                    <TextBlock Text="512GB NVMe + W10P"/>
                                    <TextBlock Text="   512GB SATA SSD"/>
                                    <TextBlock Text="   2x 1TB HDD"/>
                                    <TextBlock Text="Nvidia P5000"/>
                                    <TextBlock Text="CZC7077LBD"/>
                                </StackPanel>
            */
        }

        private void boldCheckbox_Changed(object sender, RoutedEventArgs e)
        {
            Bold bold = new Bold(new Run(labelText.Text));
            string normal = labelText.Text;
            labelText.Text = "";
            if ((bool)boldCheckbox.IsChecked)
            {
                labelText.Inlines.Add(bold);
            }
            else
            {
                labelText.Text = normal;
            }
        }
    }
}
