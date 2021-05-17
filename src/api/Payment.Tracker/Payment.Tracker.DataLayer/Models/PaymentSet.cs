using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Payment.Tracker.DataLayer.Models
{
    public class PaymentSet : Identifiable
    {
        [Column(TypeName = "date")]
        public  DateTime ForMonth { get; set; }

        public bool InvoicesAttached { get; set; }
        
        public virtual ICollection<PaymentPosition> PaymentPositions { get; set; }
    }
}