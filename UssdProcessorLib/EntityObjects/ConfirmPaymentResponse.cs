using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UssdProcessorLib.BillPaymentsApi;

namespace UssdProcessorLib.EntityObjects
{
    public class ConfirmPaymentResponse : EntityObjects.Response
    {
        public string ThirdPartyAcctRef;
        public string Token;
    }
}
