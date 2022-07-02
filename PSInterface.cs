using System.Management;

namespace powerLabel
{
    public class PSInterface
    {
        public static ManagementObjectCollection RunPowershell(string command)
        {
            ManagementScope scope = new ManagementScope();
            scope.Connect();

            //Query system for Operating System information
            ObjectQuery query = new ObjectQuery(command);
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

            ManagementObjectCollection queryCollection = searcher.Get();
            return queryCollection;
        }
    }
}
