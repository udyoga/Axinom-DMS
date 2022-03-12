namespace DMS_Sender.Models
{
    public class ProcessStatus
    {
        public string Action { get; set; }
        public bool Status { get; set; }
        public dynamic Exception { get; set; }
    }
}
