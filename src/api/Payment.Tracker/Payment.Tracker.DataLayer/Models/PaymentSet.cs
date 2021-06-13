using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Payment.Tracker.DataLayer.Models
{
    public class PaymentSet : Document
    {
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]

        public  DateTime ForMonth { get; set; }

        public bool InvoicesAttached { get; set; }
        
        public ICollection<PaymentPosition> PaymentPositions { get; set; }
    }
}