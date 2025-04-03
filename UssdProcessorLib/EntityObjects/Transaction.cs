using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UssdProcessorLib.EntityObjects
{
    public class Transaction
    {
        string tranId, statusDescription, digitalSignature, statusCode, paymentDate, password, tranAmount, teller, vendorCode, tranNarration, vendorTranId, tranIdToReverse, paymentType, tranType, customerRef, customerName, customerType, customerTel, reversal, offline, queueTime;

        public string QueueTime
        {
            get { return queueTime; }
            set { queueTime = value; }
        }

        public string Area;

        public string TranId
        {
            get
            {
                return tranId;
            }
            set
            {
                tranId = value;
            }
        }
        public string StatusDescription
        {
            get
            {
                return statusDescription;
            }
            set
            {
                statusDescription = value;
            }
        }
        public string DigitalSignature
        {
            get
            {
                return digitalSignature;
            }
            set
            {
                digitalSignature = value;
            }
        }
        public string StatusCode
        {
            get
            {
                return statusCode;
            }
            set
            {
                statusCode = value;
            }
        }
        public string PaymentDate
        {
            get
            {
                return paymentDate;
            }
            set
            {
                paymentDate = value;
            }
        }
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
            }
        }
        public string TranAmount
        {
            get
            {
                return tranAmount;
            }
            set
            {
                tranAmount = value;
            }
        }
        public string Teller
        {
            get
            {
                return teller;
            }
            set
            {
                teller = value;
            }
        }
        public string VendorCode
        {
            get
            {
                return vendorCode;
            }
            set
            {
                vendorCode = value;
            }
        }
        public string TranNarration
        {
            get
            {
                return tranNarration;
            }
            set
            {
                tranNarration = value;
            }
        }
        public string VendorTranId
        {
            get
            {
                return vendorTranId;
            }
            set
            {
                vendorTranId = value;
            }
        }
        public string TranIdToReverse
        {
            get
            {
                return tranIdToReverse;
            }
            set
            {
                tranIdToReverse = value;
            }

        }
        public string PaymentType
        {
            get
            {
                return paymentType;
            }
            set
            {
                paymentType = value;
            }
        }
        public string TranType
        {
            get
            {
                return tranType;
            }
            set
            {
                tranType = value;
            }
        }
        public string CustomerRef
        {
            get
            {
                return customerRef;
            }
            set
            {
                customerRef = value;
            }
        }
        public string CustomerName
        {
            get
            {
                return customerName;
            }
            set
            {
                customerName = value;
            }
        }
        public string CustomerType
        {
            get
            {
                return customerType;
            }
            set
            {
                customerType = value;
            }
        }
        public string CustomerTel
        {
            get
            {
                return customerTel;
            }
            set
            {
                customerTel = value;
            }
        }
        public string Reversal
        {
            get
            {
                return reversal;
            }
            set
            {
                reversal = value;
            }
        }
        public string Offline
        {
            get
            {
                return offline;
            }
            set
            {
                offline = value;
            }
        }
    }
}
