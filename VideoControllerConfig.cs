using System.Collections.Generic;
using System.Management;

namespace powerLabel
{
    public class VideoControllerConfig
    {
        public int id { get; set; }
        public VideoController videoController { get; set; }
        public ComputerSystem computerSystem { get; set; }

        public static List<VideoControllerConfig> GetVideoControllers(ComputerSystem system)
        {
            List<VideoControllerConfig> list = new List<VideoControllerConfig>();

            ManagementObjectCollection returned = PSInterface.RunObjectQuery("SELECT * FROM Win32_VideoController");
            foreach (ManagementObject item in returned)
            {
                VideoControllerConfig videoController = new VideoControllerConfig();
                videoController.videoController = new VideoController();
                videoController.videoController.manufacturer = (string)item["AdapterCompatibility"];
                videoController.videoController.name = (string)item["Caption"];
                videoController.videoController.vram = (uint)item["AdapterRam"];
                videoController.computerSystem = system;
                list.Add(videoController);
            }
            return list;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            VideoControllerConfig videoControllerConfig = (VideoControllerConfig)obj;
            if (this.videoController.Equals(videoControllerConfig.videoController)
                )
            {
                return true;
            }
            return false;
        }
    }
}