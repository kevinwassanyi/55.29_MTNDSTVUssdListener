using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UssdProcessorLib.EntityObjects
{
    public class UssdTransactions
    {
        public string PaymentReference { get; set; }
        public string TransAmount { get; set; }
        public string TransactionId { get; set; }
        public string Balance { get; set; }
        public string Phone { get; set; }
        public string VendorCode { get; set; }
        public string CustomerName { get; set; }
        public string Utility { get; set; }
        public string UmbrellaCode { get; set; }
        public object PaymentDate { get; set; }
        public string Network { get; set; }
        public string Naration { get; set; }
        public string TelecomId { get; set; }

        public string QueueID { get; set; }

        public string SchoolCode { get; set; }
        public string Area { get; set; }
        public string CustType { get; set; }
        public string ParentPhone { get; set; }
        //public string schoolcode { get; set; }
        public string CardNumber { get; set; }
        public string ChildName { get; set; }
        public string Relationship { get; set; }
        public string CardPin { get; set; }
        public string ChildId { get; set; }
    }
}
