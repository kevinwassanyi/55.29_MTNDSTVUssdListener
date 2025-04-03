using CookComputing.XmlRpc;
using DSTVListener.ControlObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using UssdProcessorLib.EntityObjects;

namespace UssdProcessorLib.ControlObjects
{
    public class UssdService
    {
        public XmlRpcStruct handleUSSDRequest(XmlRpcStruct vals)
        {
            try
            {
                bool end = false;
                String _transationId = "TransactionId";
                String _transactionTime = "TransactionTime";
                String _msisdn = "MSISDN";
                String _ussdServiceCode = "USSDServiceCode";
                String _ussdRequestString = "USSDRequestString";
                String reponseMessage = "";

                if (vals.ContainsKey(_transationId) && vals.ContainsKey(_transactionTime) && vals.ContainsKey(_msisdn) && vals.ContainsKey(_ussdServiceCode) && vals.ContainsKey(_ussdRequestString))
                {
                    ResponseObj res = new ResponseObj();
                    String transactionId = (String)vals["TransactionId"];
                    DateTime ussdTransactionTime = (DateTime)vals["TransactionTime"];
                    String msisdn = (String)vals["MSISDN"];
                    String calledNumber = (String)vals["CalledNumber"];
                    String ussdServiceCode = (String)vals["USSDServiceCode"];
                    String ussdRequestString = (String)vals["USSDRequestString"];
                    String ussdResponse = (String)vals["response"];
                    String network = (String)vals["Network"];
                    res = ProcessRequest(msisdn, transactionId, ussdRequestString, calledNumber, network);
                    reponseMessage = res.Response;
                    end = res.End;
                    //------------------------------- end -----------------------------

                    //------------  DO NOT TAMPER WITH THE CODE BELOW ------------------------------------------

                    //We need to create an XML-RPC Struct to return

                    //ussdTransactionTime = DateTime.Parse(ussdTransactionTime.ToString("s"));
                    XmlRpcStruct rpcstruct = new XmlRpcStruct();
                    rpcstruct.Add(_transationId, transactionId);
                    rpcstruct.Add(_transactionTime, (DateTime)vals["TransactionTime"]);
                    rpcstruct.Add("USSDResponseString", reponseMessage);
                    rpcstruct.Add("MSISDN", msisdn);


                    if (end)
                    {
                        rpcstruct.Add("action", "end");
                    }
                    else
                    {
                        rpcstruct.Add("action", "request");

                    }
                    //HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("gzip");
                    return rpcstruct;
                }
                else
                {
                    throw new XmlRpcFaultException(100, "Invalid argument");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private ResponseObj ProcessRequest(string msisdn, string transactionId, string ussdRequestString, string calledNumber, string network)
        {
            ResponseObj resp = new ResponseObj();
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = RemoteCertificateValidation;

                BussinessLogic bll = new BussinessLogic();
                DatabaseHandler dh = new DatabaseHandler();
                DataTable dtable = new DataTable();
                string RequestAction = "";
                string ToAction = "";
                string phone = bll.EncryptPhone(msisdn);

                if (!bll.WhiteListed(msisdn, calledNumber, network))
                {
                    resp.Response = "Sorry, you are not allowed to access this service";
                    resp.End = true;
                    resp.Log = true;
                    resp.ToNode = "MENU CHOICE";
                    resp.FromNode = "HIGH MENU";
                    return resp;
                }

                if (ussdRequestString.ToUpper().Equals("CONTINUE"))
                {
                    ToAction = "NONE";
                    RequestAction = "MAIN MENU";

                    dh.LogUssdTrans(phone, transactionId, RequestAction, ToAction, calledNumber);

                    ussdRequestString = calledNumber.Equals("*267#") ? "CONTINUE" : "0";
                }

                //get Last Ussd Step
                dtable = dh._UssdGetPreTran(phone, transactionId);
                if (dtable.Rows.Count > 0)
                {
                    string from = dtable.Rows[0]["RequestAction"].ToString();
                    string to = dtable.Rows[0]["ToAction"].ToString();


                    if (calledNumber.Contains("*272*0#"))
                    {
                        DstvUssd dstvUssd = new DstvUssd();
                        resp = dstvUssd.ProcessDSTVUssdMenu(phone, transactionId, calledNumber, ussdRequestString, from, to);
                    }
                    else
                    {
                        resp.Response = "End session";
                        resp.End = true;
                        resp.Log = false;
                    }
                }
                else
                {
                    resp.Response = "Invalid Operation Selection";
                    resp.FromNode = "MAIN";
                    resp.ToNode = "MAIN";
                    resp.End = true;
                    resp.Log = true;
                }

                if (resp.Log)
                {
                    dh.LogUssdTrans(phone, transactionId, resp.FromNode, resp.ToNode, calledNumber);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return resp;
        }

        private static bool RemoteCertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }


    }
}
