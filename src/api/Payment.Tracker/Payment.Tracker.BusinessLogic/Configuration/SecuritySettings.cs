namespace Payment.Tracker.BusinessLogic.Configuration
{
    public class SecuritySettings : ISecuritySettings
    {
        public string AllowedHost { get; set; }
        
        public string AdminPassword { get; set; }

        public string TokenSecret { get; set; }
    }
}