using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UssdProcessorLib.EntityObjects
{
    public class UssdRequest2
    {
        private string networkId, network, shortCode, mSISDN, sessionId, requestString, reponse, state, recordDate, responseDate, networkDate, mSC, msgId;

        public string MsgId
        {
            get { return msgId; }
            set { msgId = value; }
        }

        public string MSC
        {
            get { return mSC; }
            set { mSC = value; }
        }

        public string NetworkDate
        {
            get { return networkDate; }
            set { networkDate = value; }
        }

        public string ResponseDate
        {
            get { return responseDate; }
            set { responseDate = value; }
        }

        public string RecordDate
        {
            get { return recordDate; }
            set { recordDate = value; }
        }

        public string State
        {
            get { return state; }
            set { state = value; }
        }

        public string Reponse
        {
            get { return reponse; }
            set { reponse = value; }
        }

        public string RequestString
        {
            get { return requestString; }
            set { requestString = value; }
        }

        public string SessionId
        {
            get { return sessionId; }
            set { sessionId = value; }
        }

        public string MSISDN
        {
            get { return mSISDN; }
            set { mSISDN = value; }
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

        public string NetworkId
        {
            get { return networkId; }
            set { networkId = value; }
        }
    }
}
