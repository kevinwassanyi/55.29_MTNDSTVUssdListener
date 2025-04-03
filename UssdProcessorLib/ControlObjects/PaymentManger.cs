using DSTVListener.ControlObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using UssdProcessorLib.EntityObjects;

namespace UssdProcessorLib.ControlObjects
{
    internal class PaymentManger
    {
        public UssdTransactions GetUssdMomoPaymentObject(
    string transAmt,
    string phoneNo,
    string vendorCode,
    string utility,
    string custName,
    string network,
    string transId,
    string paymentRef,
    string narration,
    string area)
        {
            var ussdTransactions = new UssdTransactions
            {
                TransAmount = transAmt,
                Phone = phoneNo,
                VendorCode = vendorCode,
                Utility = utility,
                PaymentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                CustomerName = custName,
                Network = network,
                TransactionId = transId,
                PaymentReference = paymentRef,
                Naration = area + "|" + utility + "",
                TelecomId = "", // Provide a valid value if required
                Area = area
            };

            return ussdTransactions;
        }

        public bool LogTransactionInPinPromptQueue__(UssdTransactions txn)
        {

            DatabaseHandler dh = new DatabaseHandler();
            bool logged = false;
            string queueName = @".\private$\MtnDstvPinPromptPendingQueue";

            try
            {
                MessageQueue queue;
                if (MessageQueue.Exists(queueName))
                {
                    queue = new MessageQueue(queueName);
                }
                else
                {
                    queue = MessageQueue.Create(queueName);
                }


                Random rnd = new Random();
                int myRandomNo = rnd.Next(10000, 99999);
                txn.QueueID = HiResDateTime.UtcNowTicks.ToString();
                Message msg = new Message();
                msg.Body = txn;
                msg.Label = txn.QueueID;
                msg.Recoverable = true;
                queue.Send(msg);

                logged = true;
            }
            catch (Exception ee)
            {
                dh.LogError(txn.Phone, txn.Network, "*165#", txn.PaymentDate + "_" + txn.TransactionId + "_" + txn.VendorCode, ee.Message + "_FailureInsertingInPinPromptPendingQueue");

            }
            return logged;
        }



    }
}
