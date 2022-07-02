using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace powerLabel
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            refreshSettings();

        }

        private void refreshSettings()
        {
            SettingsHandler.Settings settings = SettingsHandler.ReadSettings();
            employeeList.Items.Clear();
            foreach (string item in settings.employees)
            {
                ListViewItem listViewItem = new ListViewItem();
                listViewItem.Content = item;
                employeeList.Items.Add(listViewItem);
            }

            printerHost.Text = settings.printerHost;
            printerShareName.Text = settings.printerShareName;

            ip.Text = settings.databaseIP;
            database.Text = settings.databaseName;
            username.Text = settings.databaseUser;
            password.Text = settings.databasePass;
        }

        private void addEmployeeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (employeeTextbox.Text.Trim() != "")
            {
                ListViewItem employee = new ListViewItem();
                employee.Content = employeeTextbox.Text.Trim();
                employeeList.Items.Add(employee);
                employeeTextbox.Text = "";
            }
        }

        private void updateEmployeeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (employeeTextbox.Text.Trim() != "")
            {
                ListViewItem selected = (ListViewItem)employeeList.SelectedItem;
                selected.Content = employeeTextbox.Text;
                employeeTextbox.Text = "";
                employeeList.UnselectAll();
            }
        }

        private void Employee_Selection_Changed(object sender, RoutedEventArgs e)
        {
            ListViewItem selected = (ListViewItem)employeeList.SelectedItem;
            if (selected != null)
            {
                addEmployeeBtn.Content = "Update";
                employeeTextbox.Text = selected.Content.ToString();
                addEmployeeBtn.Click += updateEmployeeBtn_Click;
                addEmployeeBtn.Click -= addEmployeeBtn_Click;

                deleteBtn.Visibility = Visibility.Visible;
            }
            else
            {
                addEmployeeBtn.Content = "Add employee";
                employeeTextbox.Text = "";
                addEmployeeBtn.Click -= updateEmployeeBtn_Click;
                addEmployeeBtn.Click += addEmployeeBtn_Click;

                deleteBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void saveSettings(object sender, RoutedEventArgs e)
        {
            SettingsHandler.Settings sett = new SettingsHandler.Settings();     // Create a new settings object

            List<string> empList = new List<string>();      // Make a list for the employees
            foreach (ListViewItem item in employeeList.Items)
            {
                empList.Add(item.Content.ToString());       // Add every employee in the listview to the list
            }

            sett.employees = empList;       // Add the list to the settings

            sett.printerHost = printerHost.Text; // Add the printer host to the settings
            sett.printerShareName = printerShareName.Text; // Add the printer share name to the settings

            sett.databaseIP = ip.Text;
            sett.databaseName = database.Text;
            sett.databaseUser = username.Text;
            sett.databasePass = password.Text;

            SettingsHandler.WriteSettings(sett); // Write settings to json

            DialogResult = true; // Close window
        }

        private void employeeTextbox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                addEmployeeBtn_Click(sender, e);
            }
        }

        private void deleteBtn_Click(object sender, RoutedEventArgs e)
        {
            ListViewItem selected = (ListViewItem)employeeList.SelectedItem;
            employeeList.Items.Remove(selected);
            employeeTextbox.Text = "";
            employeeList.UnselectAll();
        }

        private void printerSettings_TextChanged(object sender, TextChangedEventArgs e)
        {
            printerAddress.Text = @"\\" + printerHost.Text + @"\" + printerShareName.Text;
        }
    }
}
