using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using DSTVListener.ControlObjects;
using CookComputing.XmlRpc;
using UssdProcessorLib.EntityObjects;
using UssdProcessorLib.ControlObjects;
using System.Collections;
using System.Net;

public class TCPServer
{
    private BussinessLogic bll = new BussinessLogic();
    DatabaseHandler dh = new DatabaseHandler();

    public TCPServer() { }
    public void ListenAndProcess()
    {
        try
        {


            //string filename = GetLogFileName();
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://192.168.120.44:9898/pegasusaggregation/dstvpaymentsV1/");//TEST URL

            listener.Start();

            while (true)
            {
                try
                {
                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    Console.WriteLine("************************************************");
                    Console.WriteLine("Listening For an HTTP Reques33t...");
                    Console.WriteLine("************************************************");
                    HttpListenerContext context = listener.GetContext();

                    //Thread workerThread = new Thread(new ParameterizedThreadStart(HandleRequest));
                    //workerThread.Start(context);

                    HandleRequest(context);
                }
                catch (Exception ex)
                {
                    //log Errors into a file;
                    Console.WriteLine(ex.Message);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    private void HandleRequest(object httpContext)
    {
        DatabaseHandler dh = new DatabaseHandler();
        try
        {
            //pick up the request
            System.Net.HttpListenerContext context = (System.Net.HttpListenerContext)httpContext;
            string request = (new System.IO.StreamReader(context.Request.InputStream).ReadToEnd());

            Console.WriteLine();
            Console.WriteLine("Time In:" + DateTime.Now);
            Console.WriteLine(".........................Request Made.........................");
            Console.WriteLine(request);

            //process the request
            string XmlResponse = ProcessRequest(request);
            Console.WriteLine(XmlResponse);
            //return the response
            byte[] buf = Encoding.ASCII.GetBytes(XmlResponse);
            context.Response.ContentLength64 = buf.Length;
            context.Response.ContentType = "text/xml";
            context.Response.OutputStream.Write(buf, 0, buf.Length);

            Console.WriteLine();
            Console.WriteLine(".........................Response to Request......................");
            Console.WriteLine(XmlResponse);


        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            dh.LogError(ex.Message);
        }
    }


    public string ProcessRequest(string request)
    {
        //log request recieved
        string XmlResponse = "";
        string timeIn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string timeOut = "";
        string requestType = null;
        try
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11;
            //get the type of request sent thru
            requestType = GetRequestType(request);

            if (requestType.Equals("PROCESS_USSD_REQUEST"))
            {
                //query Account Details
                UssdRequest ussdRequest = new UssdRequest();
                ussdRequest = GetUssdRequestDetails(request);
                UssdResponse ussdResponse = new UssdResponse();


                if (!ussdRequest.StatusCode.Equals("100"))
                {

                    ussdResponse = ProcessUssdRequest(ussdRequest);
                    XmlResponse = GetUssdResponse(ussdResponse);
                }
                else
                {
                    ussdRequest.StatusCode = "102";
                    ussdRequest.StatusDesc = "FAILED";
                    XmlResponse = ussdRequest.ToString();
                }
                timeOut = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                dh.LogRequest("MTNDSTV", ussdRequest.TransactionId, "[Time In: " + timeIn + "] " + request, "[Time Out: " + timeOut + "] " + XmlResponse);
            }
            //request is weird and has problems
            else
            {
                XmlDocument XmlRequest = new XmlDocument();
                XmlRequest.LoadXml(request);
                string tranid = XmlRequest.GetElementsByTagName("transactionid").Item(0).InnerText;
                XmlResponse = OperationNotSupportedYetResponse("");
                timeOut = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                dh.LogRequest("DSTV", "UNKWON REQUEST", "[Time In: " + timeIn + "] " + request, "[Time Out: " + timeOut + "] " + XmlResponse);
            }


        }
        catch (Exception ex)
        {
            dh.LogError("ProcessRequest: " + ex.Message);
            XmlDocument XmlRequest = new XmlDocument();
            XmlRequest.LoadXml(request);
            string tranid = XmlRequest.GetElementsByTagName("transactionid").Item(0).InnerText;
            XmlResponse = OperationNotSupportedYetResponse("");
        }
        string whatToLog = Environment.NewLine + "Request Recieved: " + request
            + Environment.NewLine
            + Environment.NewLine
            + "Response Sent at " + timeOut + ": " + XmlResponse
            + Environment.NewLine
            + "------------------------------------------------"
            + Environment.NewLine;
        return XmlResponse;

    }


    private UssdRequest GetUssdRequestDetails(string requestXml)
    {
        UssdRequest request = new UssdRequest();
        request.TrandactionDate = DateTime.Now.ToString("yyyy-MM-dd");
        try
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(requestXml);
            // Get the root node
            XmlNode methodNode = xmlDoc.SelectSingleNode("/request") ?? new XmlDocument().CreateElement("request");

            // Retrieve child nodes with null checks
            XmlNode structNode = methodNode.SelectSingleNode("msisdn")
                               ?? new XmlDocument().CreateElement("msisdn");

            XmlNode newRequest = methodNode.SelectSingleNode("newRequest")
                                ?? new XmlDocument().CreateElement("newRequest");

            XmlNode sessionId = methodNode.SelectSingleNode("sessionId")
                               ?? new XmlDocument().CreateElement("sessionId");

            XmlNode requestString = methodNode.SelectSingleNode("subscriberInput")
                               ?? new XmlDocument().CreateElement("subscriberInput");

            XmlNode statusCode = methodNode.SelectSingleNode("statusCode")
                               ?? new XmlDocument().CreateElement("statusCode");

            // Extract inner text safely to prevent null reference exceptions
            string phone = structNode.InnerText ?? "";
            string isNewRequest = newRequest.InnerText ?? "";
            string transactionId = sessionId.InnerText ?? "";
            string userRequest = requestString.InnerText ?? "";
            string passedStatusCode = statusCode.InnerText ?? "";

            request.StatusCode = "0";
            request.Network = "MTN";
            request.ClientId = phone; ;
            request.Action = "DSTV";
            request.RequestString = isNewRequest.Equals("1") ? "CONTINUE" : userRequest;
            request.StatusCode = "0";
            request.StatusDesc = passedStatusCode;
            request.CalledNmber = "*272*0#";
            request.TransactionId = transactionId;
            request.TrandactionDate = DateTime.Now.ToString();

            return request;
        }
        catch (Exception ee)
        {
           bll.LogExceptionToFile(ee);
            request.StatusCode = "100";
            return request;
        }
    }


    private string OperationNotSupportedYetResponse(string ProcessingNumber)
    {
        string xmlResponse = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                                "<ns2:getfinancialresourceinformationresponse " +
                                "xmlns:ns2=\"http://www.ericsson.com/em/emm/serviceprovider/v1_0/backend/client\">" +
                                "<message>OPERATION_NOT_SUPPORTED_YET</message>" +
                                "<extension/>" +
                                "</ns2:getfinancialresourceinformationresponse>";


        return xmlResponse;
    }


    public UssdResponse ProcessUssdRequest(UssdRequest request)
    {

        BussinessLogic bll = new BussinessLogic();
        UssdResponse resp = new UssdResponse();
        DatabaseHandler dh = new DatabaseHandler();
        try
        {

            dh.Log("LogThirdPartyUSSDRequestResponse", new string[] { DateTime.Now.ToString(), "TranId", "At the Start" });



            request = bll.ProcessRequest(request);

            if (request.StatusCode.Equals("0"))
            {
                XmlRpcStruct rpcstruct = new XmlRpcStruct();
                rpcstruct.Add("TransactionId", request.TransactionId);
                rpcstruct.Add("TransactionTime", DateTime.Now);
                rpcstruct.Add("USSDResponseString", "");
                rpcstruct.Add("MSISDN", request.ClientId);
                rpcstruct.Add("CalledNumber", request.CalledNmber);
                rpcstruct.Add("USSDRequestString", request.RequestString);
                rpcstruct.Add("response", "");
                rpcstruct.Add("USSDServiceCode", "");
                rpcstruct.Add("Network", request.Network);
                UssdService svc = new UssdService();

                XmlRpcStruct response = svc.handleUSSDRequest(rpcstruct);
                resp.TransactionId = response["TransactionId"].ToString();
                resp.ResponseString = response["USSDResponseString"].ToString();
                resp.ClientId = response["MSISDN"].ToString();
                resp.Action = response["action"].ToString();
            }
            else
            {
                resp.ResponseString = "Unable to process request";
                resp.ClientId = request.ClientId;
                resp.TransactionId = request.TransactionId;
                resp.Action = "END";
                dh.LogError(request.ClientId, request.Network, request.ShortCode, request.TransactionId, request.StatusDesc + "_ProcessUssdRequest");
            }
        }
        catch (Exception ex)
        {
            resp.ResponseString = "Unable to process request";
            resp.ClientId = request.ClientId;
            resp.TransactionId = request.TransactionId;
            resp.Action = "END";
            dh.LogError(request.ClientId, request.Network, request.ShortCode, request.TransactionId, ex.Message + "_ProcessUssdRequest");
        }
        dh.Log("LogThirdPartyUSSDRequestResponse", new string[] { DateTime.Now.ToString(), "TranId", "At the End" });
        return resp;
    }

    private string GetRequestType(string requestXml)
    {
        string requestType = "";
        XmlDocument xmlRequest = new XmlDocument();
        xmlRequest.LoadXml(requestXml);
        XmlNodeList processUssdRequestList = xmlRequest.GetElementsByTagName("ProcessUssdRequest");  // to capture MTN DSTV USSD
        XmlNodeList menuRedirectRequest = xmlRequest.GetElementsByTagName("request");
        if (processUssdRequestList.Count > 0) // Check for the new request type
        {
            requestType = "PROCESS_USSD_REQUEST"; // Assign appropriate type
        }
        else if (menuRedirectRequest.Count > 0) // Check for the new request type
        {
            requestType = "PROCESS_USSD_REQUEST"; // Assign appropriate type
        }
        else
        {
            requestType = "UNKNOWN";
        }

        return requestType;
    }


   

    private string CreateSoapResponse(string action, string responseString, string transactionId, string bouquetCode, string smartCardNumber = "", string amount = "")
    {
        StringBuilder soapResponse = new StringBuilder();
        soapResponse.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>")
            .Append("<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" ")
            .Append("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" ")
            .Append("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">")
            .Append("<soap:Body>")
            .Append("<ProcessUssdRequestResponse xmlns=\"http://PegasusUssd/\">")
            .Append("<ProcessUssdRequestResult>")
            .Append("<Action>").Append(action).Append("</Action>")
            .Append("<ResponseString>").Append(responseString).Append("</ResponseString>")
            .Append("<ClientId>").Append(action).Append("</ClientId>")
            .Append("<TransactionId>").Append(transactionId).Append("</TransactionId>");

        if (!string.IsNullOrEmpty(bouquetCode))
        {
            soapResponse.Append("<BouquetCode>").Append(bouquetCode).Append("</BouquetCode>")
                .Append("<SmartCardNumber>").Append(smartCardNumber).Append("</SmartCardNumber>")
                .Append("<Amount>").Append(amount).Append("</Amount>");
        }

        soapResponse.Append("</ProcessUssdRequestResult>")
            .Append("</ProcessUssdRequestResponse>")
            .Append("</soap:Body>")
            .Append("</soap:Envelope>");

        return soapResponse.ToString();
    }

    private string CreateSoapResponseNew(
    string msisdn, string applicationResponse, string appDrivenMenuCode,
    string freeflowState, string freeflowCharging, string freeflowChargingAmount,
    Dictionary<string, string> parameters)
    {
        StringBuilder soapResponse = new StringBuilder();
        soapResponse.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>")
            .Append("<response>")
            .Append("<msisdn>").Append(msisdn).Append("</msisdn>")
            .Append("<applicationResponse>").Append(applicationResponse).Append("</applicationResponse>")
            .Append("<appDrivenMenuCode>").Append("").Append("</appDrivenMenuCode>")
            .Append("<freeflow>")
            .Append("<freeflowState>").Append("FC").Append("</freeflowState>")
            .Append("<freeflowCharging>").Append("N").Append("</freeflowCharging>")
            .Append("<freeflowChargingAmount>").Append("0.0").Append("</freeflowChargingAmount>")
            .Append("</freeflow>")
            .Append("</response>");


        return soapResponse.ToString();
    }



    private string GetUssdResponse(UssdResponse ussdResponse)
    {
        if (ussdResponse == null) return CreateSoapResponse("", "", "", "", "");

        string action = ussdResponse.Action ?? "";
        string responseString = ussdResponse.ResponseString ?? "";
        string transactionId = ussdResponse.TransactionId ?? "";
        string continueMenu = "FE";
        if (responseString.Contains("COMPLETE"))
        {
            string[] responseParts = responseString.Split(':');
            if (responseParts.Length < 4) return CreateSoapResponse(action, responseString, transactionId, "", "");

            string selectedPackage = responseParts[1];
            string bouquetCode = "";
            string queryType = "";
            string smartCardNumber = responseParts[3];
            GetFinancialInformationRequest validateReq = new GetFinancialInformationRequest();
            if (!selectedPackage.Equals("TopUp"))
            {
                
                if (responseString.Contains("Showmax"))
                {
                    smartCardNumber = responseParts[4];
                }
                 bouquetCode = bll.GetBoquetCodeByName(selectedPackage);
                 queryType = bouquetCode.Contains("BO") ? "BoxOffice" : "QueryCustomerDetails";
                 validateReq = GetQueryAccountDetailsRequestForUssd(smartCardNumber, bouquetCode, ussdResponse.ClientId, queryType);

            }
            else
            {
                //handle top up
            }
            if (!validateReq.IsValidRequest())
            {
                return CreateSoapResponseNew(ussdResponse.ClientId, responseString, "20", continueMenu, "N", "0.0", new Dictionary<string, string>() { { "end", "true" } });

            }
            var queryResponse = bll.QueryDetailsUssd(validateReq);
            string failedValidateion = queryResponse.PackageCost.Equals("") ? "failed" : "success";
            string finalResponse = $"Please confirm you would like to pay {queryResponse.PackageCost} UGX and balance {queryResponse.RemainingAmount} for {selectedPackage} for {queryResponse.CustomerName} SmartCard {smartCardNumber}\r\n1)Continue\r\n0)Cancel";

            if (failedValidateion.Equals("failed"))
            {
                finalResponse = "INVALID SMART CARD NUMBER " + smartCardNumber;
            }
            else
            {
                double packageCost = 0;
                double remainingAmount = 0;
                double totalAmountToPay = 0;
                if (!string.IsNullOrEmpty(queryResponse.RemainingAmount))
                {
                    double.TryParse(queryResponse.PackageCost, out packageCost);
                    double.TryParse(queryResponse.RemainingAmount, out remainingAmount);
                   
                    totalAmountToPay = packageCost; 

                }
                UssdSession session = UssdSessionStore.GetSession(transactionId);
                if (session != null)
                {
                    
                    
                    double.TryParse(queryResponse.RemainingAmount, out remainingAmount);
                    session.Price = "" + totalAmountToPay;
                    session.ClientId = ussdResponse.ClientId;
                    session.boqouteCode = queryResponse.BouquetCode;
                    session.CustomerName = queryResponse.CustomerName ?? "MTN DSTV CUSTOMER";
                    UssdSessionStore.SaveSession(session);
                }
            }
            return CreateSoapResponseNew(ussdResponse.ClientId, finalResponse, "20", continueMenu, "N", "0.0", new Dictionary<string, string>() {
                {"smartCard",smartCardNumber },
                {"bouqetCode",queryResponse.BouquetCode },
                {"price",queryResponse.PackageCost },
                {"validation",failedValidateion },
                { "end", "true" }

            });

            //return CreateSoapResponse(action, finalResponse, transactionId, bouquetCode, smartCardNumber, price);
        }
        else if (responseString.Contains("SESSIONEND"))
        {
            string[] responseParts = responseString.Split(':');
            if (responseParts.Length < 4) return CreateSoapResponse(action, responseString, transactionId, "", "");

            string selectedPackage = responseParts[1];
            string price = responseParts[2];
            string smartCardNumber = responseParts[3];
            if (responseString.Contains("Showmax"))
            {
                smartCardNumber = responseParts[4];
            }
            string bouquetCode = bll.GetBoquetCodeByName(selectedPackage);
            string queryType = bouquetCode.Contains("BO") ? "BoxOffice" : "QueryCustomerDetails";
            var validateReq = GetQueryAccountDetailsRequestForUssd(smartCardNumber, bouquetCode, ussdResponse.ClientId, queryType);


            string finalResponse = $"Done";

            continueMenu = "FB";
            // dh.ExecuteNonQuery("InsertMtnDstvTransaction", new object[] { smartCardNumber, bouquetCode, "DSTV", price });
            return CreateSoapResponseNew(ussdResponse.ClientId, finalResponse, "20", continueMenu, "N", "0.0", new Dictionary<string, string>() {
                {"smartCard",smartCardNumber },
                {"bouqetCode",bouquetCode },
                {"price",price },
                {"validation","failed" },
                { "end", "true" }

            });

        }
        else if (responseString.Contains("FINAL"))
        {
            string[] responseParts = responseString.Split(':');
            if (responseParts.Length < 4) return CreateSoapResponse(action, responseString, transactionId, "", "");

            string selectedPackage = responseParts[1];
            string price = responseParts[2];
            string smartCardNumber = responseParts[3];
            if (responseString.Contains("Showmax"))
            {
                smartCardNumber = responseParts[4];
            }
            string bouquetCode = bll.GetBoquetCodeByName(selectedPackage);

            string customerType = selectedPackage.Contains("GO") ? "GOTV" : "DSTV";


            string finalResponse = $"Please wait for pin prompt";
            UssdSession session = UssdSessionStore.GetSession(transactionId);

            //log in table where they will be sent to dstv for notification
            //dh.ExecuteNonQueryUssd("UpdateUtility_Points", new object[] { session.SmartCardNumber, transactionId,session.boqouteCode, customerType,session.Price });
            UssdTransactions ussdTransactions = new UssdTransactions();
            PaymentManger paymentManger = new PaymentManger();
            ussdTransactions = paymentManger.GetUssdMomoPaymentObject(session.Price, session.ClientId, "MTN Multichoice Pulls", customerType, session.CustomerName ?? "MTN DSTV CUSTOMER", "MTN", transactionId, session.SmartCardNumber, "PAYMENT FOR MULTICHOICE", session.boqouteCode);
            //send to momo for payment
            paymentManger.LogTransactionInPinPromptQueue__(ussdTransactions);

            return CreateSoapResponseNew(ussdResponse.ClientId, finalResponse, "20", continueMenu, "N", "0.0", new Dictionary<string, string>() {
                {"smartCard",smartCardNumber },
                {"bouqetCode",bouquetCode },
                {"price",price },
                {"validation","ture" },
                { "end", "true" }

            });

        }
        return CreateSoapResponseNew(ussdResponse.ClientId, responseString, "20", continueMenu, "N", "0.0", new Dictionary<string, string>() { { "end", "false" }, { "validation", "pending" } });

    }

    private GetFinancialInformationRequest GetQueryAccountDetailsRequestForUssd(string smartCardNumber, string bouquetCode, string customerTel, string friRequestType)
    {
        GetFinancialInformationRequest request = new GetFinancialInformationRequest();
        try
        {
            request.SmartCardNumber = smartCardNumber;
            request.CustomerTel = customerTel;
            request.BouquetCode = bouquetCode;
            request.BouquetCode = bll.GetNewBoquetCode(bouquetCode);
            request.FRIRequestType = friRequestType;
            string BouquetCode = bouquetCode;
            request.UtilityCode = GetUtilityCode(bouquetCode);
            request.StatusCode = "0";
            request.StatusDescription = "SUCCESS";

            return request;
        }
        catch (Exception e)
        {
            BussinessLogic bll = new BussinessLogic();
            bll.LogExceptionToFile(e);
            request.StatusCode = "100";
            request.StatusDescription = "FAILED";
            return request;
        }
    }
    private string GetUtilityCode(string BouquetCode)
    {
        if (!string.IsNullOrEmpty(BouquetCode))
        {
            if (BouquetCode.StartsWith("GO"))
            {
                return "GOTV";
            }
            else
            {
                return "DSTV";
            }
        }
        else
        {
            return "DSTV";
        }
    }
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
                BussinessLogic bussinessLogic = new BussinessLogic();
                res = bussinessLogic.ProcessRequest(msisdn, transactionId, ussdRequestString, calledNumber, network);
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
}
