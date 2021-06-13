namespace Payment.Tracker.DataLayer.Models
{
    public class User : Document
    {
        public string UserName { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }
    }
}