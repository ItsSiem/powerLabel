using System.Linq;
using System.Management;
using System.Windows.Forms;

namespace powerLabel
{
    public class PSInterface
    {
        public static ManagementObjectCollection GetObjects(string command)
        {
            ManagementScope scope = new ManagementScope();
            scope.Connect();

            //Query system for Operating System information
            ObjectQuery query = new ObjectQuery(command);
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

            ManagementObjectCollection queryCollection = searcher.Get();
            return queryCollection;
        }

        public static ManagementObject GetObject(string objectPath)
        {
            return GetObjects($"SELECT * FROM {objectPath}").OfType<ManagementObject>().FirstOrDefault();
        }
    }
}
