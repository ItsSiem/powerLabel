using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;

namespace powerLabel.Models
{
    public class Event
    {
        public int id { get; set; }
        public string employee { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string note { get; set; }
        public ComputerSystem computerSystem { get; set; }
        public DateTime date { get; set; }

        public Event()
        {

        }
        public Event(string employee, string eventName, string description, DateTime date, ComputerSystemContext db, ComputerSystem cs, string note = "")
        {

            try
            {
                computerSystem = cs;
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot make event when there is no system scanned.");
                throw;
            }

            this.employee = employee;
            name = eventName;
            this.description = description;
            this.note = note;
            this.date = date;

        }

        public static Event NewPrintEvent(string employee, DateTime date, ComputerSystemContext db, ComputerSystem cs, string note = "")
        {
            string name = "Printed Label";
            string description = "A label with the specs of this system has been printed.";
            return new Event(employee, name, description, date, db, cs, note);
        }
        public static Event NewScanEvent(string employee, DateTime date, ComputerSystemContext db, ComputerSystem cs, string note = "")
        {
            string name = "System Scanned";
            string description = "The specifications of the system have been scanned and recorded in the database.";
            return new Event(employee, name, description, date, db, cs, note);
        }
        public static Event NewMarkedAsOrderEvent(string employee, DateTime date, ComputerSystemContext db, ComputerSystem cs, string note = "")
        {
            string name = "Marked As Order";
            string description = "";
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    description = (window as MainWindow).orderNr.Text;
                }
            }
            return new Event(employee, name, description, date, db, cs, note);
        }


    }
}
