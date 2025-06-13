namespace Backbone.Core.Services.Logging.Models
{
    public class LogRequestModel
    {
        public string Message { get; set; }
        public string Level { get; set; } // "Error", "Warning", "Info"
        public string Source { get; set; }
        public string StackTrace { get; set; }
    }
}
