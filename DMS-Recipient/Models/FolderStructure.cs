using System.Collections.Generic;

namespace DMS_recipient.Models
{
    public class FolderStructure
    {
        public string type { get; set; }
        public string name { get; set; }
        public List<FolderStructure> subFolder { get; set; }
    }
}
