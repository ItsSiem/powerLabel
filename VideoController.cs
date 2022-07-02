namespace powerLabel
{
    public class VideoController
    {
        public int id { get; set; }
        public string manufacturer { get; set; }
        public string name { get; set; }
        public uint vram { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            VideoController videoController = (VideoController)obj;
            if (this.manufacturer.Trim() == videoController.manufacturer.Trim() &&
                this.name.Trim() == videoController.name.Trim() &&
                this.vram == videoController.vram
                )
            {
                return true;
            }
            return false;
        }
    }
}
