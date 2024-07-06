using System.Management;

namespace powerLabel
{
    public class PSInterface
    {
        public static ManagementObjectCollection RunObjectQuery(string command)
        {
            ManagementScope scope = new ManagementScope();
            scope.Connect();

            //Query system for Operating System information
            ObjectQuery query = new ObjectQuery(command);
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

            ManagementObjectCollection queryCollection = searcher.Get();
            return queryCollection;
        }

        public static void RunPowershell(string command)
        {
            var processInfo = new System.Diagnostics.ProcessStartInfo
            {
                Verb = "runas",
                LoadUserProfile = true,
                FileName = "powershell.exe",
                Arguments = command,
                RedirectStandardOutput = false,
                UseShellExecute = true,
                CreateNoWindow = true
            };

            var p = System.Diagnostics.Process.Start(processInfo);
            p.WaitForExit();
        }

    }
}
