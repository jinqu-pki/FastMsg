namespace FastMsg.Transport.Core.Models.EventArgs;

public class ErrorOccurredEventArgs : System.EventArgs
{
    public Exception Exception { get; set; }
    public string ErrorMessage { get; set; }
}
