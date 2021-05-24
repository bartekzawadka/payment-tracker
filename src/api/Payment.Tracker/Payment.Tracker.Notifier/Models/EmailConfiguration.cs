namespace Payment.Tracker.Notifier.Models
{
    public class EmailConfiguration
    {
        public string AppEndpoint { get; set; }
        
        public string Server { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
        
        public bool UseSsl { get; set; }

        public string FromAddress { get; set; }

        public string FromName { get; set; }

        public string To { get; set; }
    }
}