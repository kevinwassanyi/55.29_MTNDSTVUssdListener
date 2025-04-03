using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UssdProcessorLib.EntityObjects
{
    public class UssdResponse
    {
        private string transactionId, clientId, responseString, action, accountExpected, readMethod, print, printText, printCount;
        public string AccountExpected
        {
            get
            {
                return accountExpected;
            }
            set
            {
                accountExpected = value;
            }
        }
        public string ReadMethod
        {
            get
            {
                return readMethod;
            }
            set
            {
                readMethod = value;
            }
        }
        public string Print
        {
            get
            {
                return print;
            }
            set
            {
                print = value;
            }
        }
        public string PrintText
        {
            get
            {
                return printText;
            }
            set
            {
                printText = value;
            }
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
        public string ResponseString
        {
            get
            {
                return responseString;
            }
            set
            {
                responseString = value;
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
        public string PrintCount
        {
            get
            {
                return printCount;
            }
            set
            {
                printCount = value;
            }
        }
    }
}
