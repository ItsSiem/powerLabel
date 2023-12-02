using System.Management;

namespace powerLabel.Models
{
    public class Processor
    {
        public int id { get; set; }
        public string name { get; set; }
        public uint cores { get; set; }
        public uint threads { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            Processor processor = (Processor)obj;
            if (name.Trim() == processor.name.Trim()
                )
            {
                return true;
            }
            return false;
        }

    }
}
