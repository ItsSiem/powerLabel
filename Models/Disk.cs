namespace powerLabel.Models
{
    public class Disk
    {
        public int id { get; set; }
        public string model { get; set; }
        public ulong size { get; set; }
        public string mediaType { get; set; }
        public string serialNumber { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            Disk disk = (Disk)obj;
            if (model.Trim() == disk.model.Trim() &&
                size == disk.size &&
                mediaType.Trim() == disk.mediaType.Trim() &&
                serialNumber.Trim() == disk.serialNumber.Trim()
                )
            {
                return true;
            }
            return false;
        }
    }
}