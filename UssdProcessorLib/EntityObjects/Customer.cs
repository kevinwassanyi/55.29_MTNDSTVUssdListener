using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UssdProcessorLib.EntityObjects
{
    public class Customer
    {
        private string customerRef, customerName, customerType, statusCode, statusDescription, balance,
        agentCode, area, tin, sessionKey, bouquetPrice;

        public string BouquetPrice
        {
            get
            {
                return bouquetPrice;
            }
            set
            {
                bouquetPrice = value;
            }
        }

        public string SessionKey
        {
            get
            {
                return sessionKey;
            }
            set
            {
                sessionKey = value;
            }
        }
        public string AgentCode
        {
            get
            {
                return agentCode;
            }
            set
            {
                agentCode = value;
            }
        }
        public string TIN
        {
            get
            {
                return tin;
            }
            set
            {
                tin = value;
            }
        }
        public string Area
        {
            get
            {
                return area;
            }
            set
            {
                area = value;
            }
        }
        public string Balance
        {
            get
            {
                return balance;
            }
            set
            {
                balance = value;
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

    }
}
