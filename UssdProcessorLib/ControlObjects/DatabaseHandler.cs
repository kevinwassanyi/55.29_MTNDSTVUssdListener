
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Security.Policy;
using UssdProcessorLib.EntityObjects;
using System.Collections;
using UssdProcessorLib.ControlObjects;

namespace DSTVListener.ControlObjects
{
    public class DatabaseHandler
    {
        private Database PegPayDB;
        private DataTable dt = new DataTable();
        private DbCommand command;
        public static string QueueName = "";
        //private string conString = "LivePegPay";
        private string conString = "PegasusUssddbConnection";
        public DatabaseHandler()
        {
            try
            {
                PegPayDB = DatabaseFactory.CreateDatabase(conString);
                //PegPayDB = DbLayer.CreateDatabase("LivePegPay", DbLayer.DB2);
                if (conString.Equals("LivePegPay"))
                {
                    QueueName = @".\private$\MtnDstvQueue";
                }
                else
                {
                    QueueName = @".\private$\testMtnDstvQueue";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal DataTable _UssdGetPreTran(string phone, string transactionId)
        {
            DataTable returnTable = null;
            try
            {
                //command = PegasusUssddb.GetStoredProcCommand("_UssdGetPreTran", phone, transactionId);
                returnTable = PegPayDB.ExecuteDataSet("_UssdGetPreTran", phone, transactionId).Tables[0];
                return returnTable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       

        internal string GetStatusDescr(string statusCode)
        {
            string descr = "";
            try
            {
                command = PegPayDB.GetStoredProcCommand("GetStatusDescr", statusCode);
                dt = PegPayDB.ExecuteDataSet(command).Tables[0];
                if (dt.Rows.Count != 0)
                {
                    descr = dt.Rows[0]["StatusDescription"].ToString();
                }
                else
                {
                    descr = "GENERAL ERROR AT PEGASUS failed to get status description";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return descr;
        }
      
        internal string GetSystemSetting(int GroupCode, int valueCode)
        {
            string value = "";
            try
            {
                command = PegPayDB.GetStoredProcCommand("GetSystemSetting", GroupCode, valueCode);
                dt = PegPayDB.ExecuteDataSet(command).Tables[0];
                value = dt.Rows[0]["ValueVarriable"].ToString().Trim();
                return value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable LogUssdTracker(string Network, string calledNumber, string msisdn, string transactionId)
        {
            try
            {
                DataTable datatable = new DataTable();
                //command = PegasusUssddb.GetStoredProcCommand("_UssdInsertUssdTrack", Network, calledNumber, msisdn, transactionId);
                datatable = ExecuteDataSet("_UssdInsertUssdTrack", Network, calledNumber, msisdn, transactionId).Tables[0];
                return datatable;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal void Log(string procedure, string[] data)
        {
            try
            {
                //command = PegasusUssddb.GetStoredProcCommand(procedure, data);
                PegPayDB.ExecuteNonQuery(procedure, data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        internal void LogError(string MSISDN, string Network, string ShortCode, string SessionId, string Message)
        {
            try
            {
                //command = PegasusUssddb.GetStoredProcCommand("LogUssdError", MSISDN, SessionId, Message,Network,ShortCode);
                ExecuteNonQuery("LogUssdError", MSISDN, SessionId, Message, Network, ShortCode);
            }
            catch (Exception ex)
            {
                ArrayList a = new ArrayList();
                a.Add(MSISDN + "," + SessionId + "," + ShortCode + "," + Message + "," + Network);
                BussinessLogic bll = new BussinessLogic();
                bll.LogExceptionToFile(ex);
            }
        }
        internal DataTable GetSessionDetails(string msisdn, string transactionId)
        {
            try
            {
                DataTable datatable = new DataTable();
                //command = PegasusUssddb.GetStoredProcCommand("_UssdGetSessionDetails", msisdn, transactionId);
                datatable = ExecuteDataSet("_UssdGetSessionDetails", msisdn, transactionId).Tables[0];
                return datatable;
            }
            catch (Exception ex)
            {
                BussinessLogic bll = new BussinessLogic();
                bll.LogExceptionToFile(ex);
                throw ex;
               
            }
        }


        internal void LogError(string exception)
        {
            try
            {
                command = PegPayDB.GetStoredProcCommand("LogError", exception, "MTN", DateTime.Now, "DSTV");
                PegPayDB.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                BussinessLogic bll = new BussinessLogic();
                bll.LogExceptionToFile(ex);
            }
        }

        public void LogRequest(string utilitycode, string tranId, string request,
            string response)
        {
            try
            {
                command = PegPayDB.GetStoredProcCommand(
                    "LogXmlRequestResponseNew",
                    utilitycode,
                    tranId,
                    request,
                    response
                );
                PegPayDB.ExecuteNonQuery(command);
            }
            catch (Exception ee) {
                BussinessLogic bll = new BussinessLogic();
                bll.LogExceptionToFile(ee);
            }
        }

        internal DataTable GetDuplicateVendorRef(string VendorTranId)
        {
            try
            {
                command = PegPayDB.GetStoredProcCommand("GetDuplicateVendorRef2", VendorTranId);
                DataTable returndetails = PegPayDB.ExecuteDataSet(command).Tables[0];
                return returndetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public int SaveReactivationRequest(string smartCardNumber)
        {
            try
            {
                command = PegPayDB.GetStoredProcCommand("SaveReactivateRequest", smartCardNumber);
                int rows = PegPayDB.ExecuteNonQuery(command);
                return rows;
            }
            catch (Exception ex)
            {
                BussinessLogic bll = new BussinessLogic();
                bll.LogExceptionToFile(ex);
                return -1;
            }
        }

        internal void LogUssdTrans(string Msisdn, string TransactionID, string RequestAction, string ToAction, string ShortCode)
        {
            try
            {

                //command = PegasusUssddb.GetStoredProcCommand("_UssdLogTran", Msisdn, TransactionID, RequestAction, ToAction, ShortCode);
                PegPayDB.ExecuteNonQuery("_UssdLogTran", Msisdn, TransactionID, RequestAction, ToAction, ShortCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

  
        internal DataSet ExecuteDataSet(string procedure, params object[] parameters)
        {
            try
            {
                return PegPayDB.ExecuteDataSet(procedure, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        internal int ExecuteNonQuery(string procedure, params object[] parameters)
        {
            try
            {
                return PegPayDB.ExecuteNonQuery(procedure, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}