using MongoDB.Driver;
using Payment.Tracker.DataLayer.Sys;

namespace Payment.Tracker.DataLayer
{
    public class PaymentContext
    {
        public IMongoDatabase Database { get; }

        public PaymentContext(string connectionString)
        {
            IMongoClient client = new MongoClient(connectionString);
            Database = client.GetDatabase(Consts.DatabaseName);
        }
    }
}