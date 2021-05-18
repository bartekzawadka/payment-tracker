namespace Payment.Tracker.BusinessLogic.Configuration
{
    public interface ISecuritySettings
    {
        string AllowedHost { get; set; }

        string AdminPassword { get; set; }

        string TokenSecret { get; set; }
    }
}