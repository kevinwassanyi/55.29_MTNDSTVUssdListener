using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UssdProcessorLib.EntityObjects
{
    public class UssdRequest
    {
        private string transactionId, transactionDate, clientId, requestString, action, calledNmber, network, shortCode, statusCode, statusDesc;

        public string StatusDesc
        {
            get { return statusDesc; }
            set { statusDesc = value; }
        }

        public string StatusCode
        {
            get { return statusCode; }
            set { statusCode = value; }
        }

        public string ShortCode
        {
            get { return shortCode; }
            set { shortCode = value; }
        }

        public string Network
        {
            get { return network; }
            set { network = value; }
        }

        public string CalledNmber
        {
            get { return calledNmber; }
            set { calledNmber = value; }
        }
        public string Action
        {
            get
            {
                return action;
            }
            set
            {
                action = value;
            }
        }
        public string RequestString
        {
            get
            {
                return requestString;
            }
            set
            {
                requestString = value;
            }
        }
        public string ClientId
        {
            get
            {
                return clientId;
            }
            set
            {
                clientId = value;
            }
        }
        public string TrandactionDate
        {
            get
            {
                return transactionDate;
            }
            set
            {
                transactionDate = value;
            }
        }
        public string TransactionId
        {
            get
            {
                return transactionId;
            }
            set
            {
                transactionId = value;
            }
        }
    }
}
