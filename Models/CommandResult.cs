namespace Zus.Models
{
    public class CommandResult
    {
        public string? Result { get; set; }
        public string? Error { get; set; }
        public bool Success
        {
            get
            {
                return Error == null;
            }
        }
    }
}
