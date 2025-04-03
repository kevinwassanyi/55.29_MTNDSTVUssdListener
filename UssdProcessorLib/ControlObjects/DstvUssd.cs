using DSTVListener.ControlObjects;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UssdProcessorLib.EntityObjects;

namespace UssdProcessorLib.ControlObjects
{
    public class DstvUssd
    {
        private BussinessLogic _bll;
        private DatabaseHandler _dbHandler;
        public DstvUssd()
        {
            _bll = new BussinessLogic();
            _dbHandler = new DatabaseHandler();
        }

        public ResponseObj ProcessDSTVUssdMenu(string msisdn, string transactionId, string calledNumber, string ussdRequestString, string fromAction, string toAction)
        {
            ResponseObj resp = new ResponseObj();
            string phone = _bll.EncryptPhone(msisdn);
            //intro
            if (fromAction == "MAIN MENU" && toAction == "NONE")
            {
                return GetMainMenu();
            }
            else if (toAction == "SELECT PRODUCT")
            {
                return SelectMutichoiceProduct(ussdRequestString, transactionId, msisdn);
            }
            else if (toAction == "DSTV PACKAGES")
            {
                return ProcessDstvPackagesList(ussdRequestString);
            }
            else if (toAction == "SUBSCRIPTION MENU DSTV")
            {
                return SelectDSTVMenuType(ussdRequestString);
            }
            else if (toAction == "DSTV MONTHLY MENU")
            {
                return ConfirmDstvPayment(ussdRequestString, transactionId);
            }
            else if (toAction == "DSTV MONTHLY MENU")
            {
                return ConfirmDstvPayment(ussdRequestString, transactionId);
            }
            else if (toAction == "DSTV WEEKLY MENU")
            {
                return ConfirmDstvPaymentWeekly(ussdRequestString, transactionId);
            }
            else if (toAction == "ENTER SMARTCARD")
            {
                return ProcessSmartCardEntry(ussdRequestString, transactionId);
            }
            else if (toAction == "DSTV SHOWMAX MENU")
            {
                return ConfirmDstvShowMaxCombo(ussdRequestString, transactionId);
            }
            else if (toAction == "DSTV MOVIEADDONS MENU")
            {
                return ConfirmDstvMovieAddOnCombo(ussdRequestString, transactionId);
            }
            else if (toAction == "CONFIRM SMARTCARD GOTV")
            {
                return ConfirmSmartCardGotv(ussdRequestString, transactionId);
            }
            else if (toAction == "SELECT PACKAGE TYPE GOTV")
            {
                return SelectGotvPackage(ussdRequestString);
            }
            else if (toAction == "SUBSCRIPTION MENU GOTV")
            {
                return ProcessGotvSubscription(ussdRequestString);
            }
            else if (toAction == "GOTV MONTHLY")
            {
                return ConfirmGotvPayment(ussdRequestString, UssdMenus.GotvPackagesMenuOld1 + UssdMenus.GotvPackagesMenuOld2, transactionId);
            }
            else if (toAction == "GOTV WEEKLY")
            {
                return ConfirmGotvPayment(ussdRequestString, UssdMenus.GotvMenuWeekly, transactionId);
            }
            else if (toAction == "GOTV QUATERLY")
            {
                return ConfirmGotvPaymentQuaterly(ussdRequestString, UssdMenus.GotvQuaterly, transactionId);
            }
            else if (toAction == "GOTV ANUALLY")
            {
                return ConfirmGotvPaymentAnually(ussdRequestString, UssdMenus.GotvMenuAnually, transactionId);
            }
            else if (toAction == "CONFIRM PAYMENT")
            {
                return ProcessUCINumber(ussdRequestString, transactionId);
            }
            else if (toAction == "FINALNODE")
            {
                return ProcessFinalNode(ussdRequestString, transactionId);
            }

            resp.Response = "Invalid Option";
            resp.End = false;
            resp.ToNode = "NONE";
            resp.FromNode = "MAIN MENU";
            return resp;
        }

        private ResponseObj GetMainMenu()
        {
            return new ResponseObj
            {
                Response = UssdMenus.DstvGotvHomeMenu,
                End = false,
                ToNode = "SELECT PRODUCT",
                FromNode = "MAIN MENU",
                Log = true
            };
        }

        private ResponseObj SelectMutichoiceProduct(string ussdRequestString, string tranid = "", string clientId = "")
        {
            ResponseObj resp = new ResponseObj { Log = true, End = false };

            switch (ussdRequestString)
            {
                case "1":
                    resp.Response = UssdMenus.SubscriptionTypeMenuDSTV;
                    resp.ToNode = "SUBSCRIPTION MENU DSTV";
                    resp.FromNode = "SELECT PRODUCT";
                    break;
                case "2":
                    resp.Response = UssdMenus.SubscriptionTypeMenuGOTV;
                    resp.ToNode = "SUBSCRIPTION MENU GOTV";
                    resp.FromNode = "SELECT PRODUCT";
                    break;
                case "3":
                    resp.Response = UssdMenus.DstvPackagesList;
                    resp.ToNode = "DSTV PACKAGES";
                    resp.FromNode = "SELECT PRODUCT";
                    resp.End = true;
                    break;
                case "4":
                    resp.Response = "Please Enter Smartcard/IUC Number";
                    resp.ToNode = "CONFIRM PAYMENT";
                    resp.FromNode = "SELECT PRODUCT";
                    UssdSession ussdSession = new UssdSession()
                    {
                        SelectedPackage = "Box Office",
                        TransactionId = tranid,
                        ClientId = clientId,
                        SmartCardNumber = "",
                        boqouteCode = "",
                        Price = "100000"
                    };

                    UssdSessionStore.SaveSession(ussdSession);

                    break;
                case "0":
                    resp.Response = UssdMenus.DstvGotvHomeMenu;
                    resp.ToNode = "NONE";
                    resp.FromNode = "MAIN MENU";
                    break;
                default:
                    resp.Response = "Invalid Option";
                    resp.ToNode = "NONE";
                    resp.FromNode = "MAIN MENU";
                    break;
            }

            return resp;
        }

        private ResponseObj SelectDSTVMenuType(string ussdRequestString)
        {
            ResponseObj resp = new ResponseObj { Log = true, End = false };

            switch (ussdRequestString)
            {
                case "1":
                    resp.Response = UssdMenus.DstvMonthlyMenu;
                    resp.ToNode = "DSTV MONTHLY MENU";
                    resp.FromNode = "SUBSCRIPTION MENU DSTV";
                    break;
                case "2":
                    resp.Response = UssdMenus.DstvWeeklyMenu;
                    resp.ToNode = "DSTV WEEKLY MENU";
                    resp.FromNode = "SUBSCRIPTION MENU DSTV";
                    break;
                case "3":
                    resp.Response = UssdMenus.ShowMaxCombos;
                    resp.FromNode = "SUBSCRIPTION MENU DSTV";
                    resp.ToNode = "DSTV SHOWMAX MENU";
                    break;
                case "4":
                    resp.Response = UssdMenus.MovieAddOnsMenu;
                    resp.FromNode = "SUBSCRIPTION MENU DSTV";
                    resp.ToNode = "DSTV MOVIEADDONS MENU";
                    break;
                case "0":
                    resp.Response = UssdMenus.DstvGotvHomeMenu;
                    resp.ToNode = "SELECT PRODUCT";
                    resp.FromNode = "SUBSCRIPTION MENU DSTV";
                    break;
                case "m":
                    resp.Response = UssdMenus.DstvGotvHomeMenu;
                    resp.ToNode = "NONE";
                    resp.FromNode = "MAIN MENU";
                    break;
                default:
                    resp.Response = "Invalid Option";
                    resp.ToNode = "NONE";
                    resp.FromNode = "MAIN MENU";
                    break;
            }

            return resp;
        }

        private ResponseObj ProcessDstvPackagesList(string ussdRequestString)
        {
            ResponseObj resp = new ResponseObj { Log = true, End = false };

            switch (ussdRequestString)
            {

                case "0":
                    resp.Response = UssdMenus.DstvGotvHomeMenu;
                    resp.ToNode = "NONE";
                    resp.FromNode = "MAIN MENU";
                    break;

                default:
                    resp.Response = UssdMenus.DstvGotvHomeMenu;
                    resp.ToNode = "NONE";
                    resp.FromNode = "MAIN MENU";
                    break;
            }

            return resp;
        }
        private ResponseObj ProcessMonthlyMenuDstv(string ussdRequestString)
        {
            ResponseObj resp = new ResponseObj { Log = true, End = false };

            switch (ussdRequestString)
            {
                case "1":
                    resp.Response = UssdMenus.DstvMonthlyMenu;
                    resp.ToNode = "DSTV MONTHLY MENU";
                    resp.FromNode = "SUBSCRIPTION MENU DSTV";
                    break;
                case "2":
                    resp.Response = UssdMenus.DstvWeeklyMenu;
                    resp.ToNode = "DSTV MONTHLY MENU";
                    resp.FromNode = "SUBSCRIPTION MENU DSTV";
                    break;
                case "0":
                    resp.Response = UssdMenus.DstvGotvHomeMenu;
                    resp.ToNode = "NONE";
                    resp.FromNode = "MAIN MENU";
                    break;
                case "m":
                    resp.Response = UssdMenus.DstvGotvHomeMenu;
                    resp.ToNode = "NONE";
                    resp.FromNode = "MAIN MENU";
                    break;
                default:
                    resp.Response = "Invalid Option";
                    resp.ToNode = "NONE";
                    resp.FromNode = "MAIN MENU";
                    break;
            }

            return resp;
        }

        private ResponseObj ProcessWeeklyMenuDstv(string ussdRequestString)
        {
            ResponseObj resp = new ResponseObj { Log = true, End = false };

            switch (ussdRequestString)
            {
                case "1":
                    resp.Response = UssdMenus.DstvMonthlyMenu;
                    resp.ToNode = "DSTV MONTHLY MENU";
                    resp.FromNode = "SUBSCRIPTION MENU DSTV";
                    break;
                case "2":
                    resp.Response = UssdMenus.DstvWeeklyMenu;
                    resp.ToNode = "DSTV MONTHLY MENU";
                    resp.FromNode = "SUBSCRIPTION MENU DSTV";
                    break;
                case "0":
                    resp.Response = UssdMenus.DstvGotvHomeMenu;
                    resp.ToNode = "NONE";
                    resp.FromNode = "MAIN MENU";
                    break;
                case "m":
                    resp.Response = UssdMenus.DstvGotvHomeMenu;
                    resp.ToNode = "NONE";
                    resp.FromNode = "MAIN MENU";
                    break;
                default:
                    resp.Response = "Invalid Option";
                    resp.ToNode = "NONE";
                    resp.FromNode = "MAIN MENU";
                    break;
            }

            return resp;
        }
        private ResponseObj ProcessSmartCardEntry(string ussdRequestString, string transactionId)
        {
            ResponseObj resp = new ResponseObj { Log = true, End = false };

            switch (ussdRequestString)
            {
                case "1":
                    resp.Response = "Please enter Smartcard/IUC Number";
                    resp.ToNode = "CONFIRM SMARTCARD";
                    resp.FromNode = "MAIN MENU";
                    break;
                case "2":
                    resp.Response = "Please enter Smartcard/IUC Number";
                    resp.ToNode = "CONFIRM SMARTCARD GOTV";
                    resp.FromNode = "MAIN MENU";
                    break;
                case "0":
                case "m":
                    resp.Response = UssdMenus.DstvGotvHomeMenu;
                    resp.ToNode = "NONE";
                    resp.FromNode = "MAIN MENU";
                    break;
                default:
                    resp.Response = "Invalid Option";
                    resp.ToNode = "NONE";
                    resp.FromNode = "MAIN MENU";
                    break;
            }
            return resp;
        }

        private ResponseObj ConfirmSmartCardGotv(string ussdRequestString, string transactionId)
        {
            return new ResponseObj
            {
                Response = "Selected Smartcard: " + ussdRequestString + "\n\n1) Confirm\n2) Cancel\n\n# Home\n* Back",
                End = false,
                FromNode = "CONFIRM SMARTCARD GOTV",
                ToNode = "SELECT PACKAGE TYPE GOTV",
                Log = true
            };
        }



        private ResponseObj SelectGotvPackage(string ussdRequestString)
        {
            ResponseObj resp = new ResponseObj { Log = true, End = false };

            switch (ussdRequestString)
            {
                case "1":
                    resp.Response = "";// UssdMenus.SubscriptionTypeMenu;
                    resp.ToNode = "SUBSCRIPTION MENU GOTV";
                    resp.FromNode = "SELECT PACKAGE TYPE GOTV";
                    break;
                case "2":
                    resp.Response = "Done !";
                    resp.ToNode = "NONE";
                    resp.FromNode = "SELECT PACKAGE TYPE GOTV";
                    resp.End = true;
                    break;
                default:
                    resp.Response = "Invalid Option";
                    resp.ToNode = "NONE";
                    resp.FromNode = "MAIN MENU";
                    break;
            }

            return resp;
        }

        private ResponseObj ProcessGotvSubscription(string ussdRequestString)
        {
            ResponseObj resp = new ResponseObj { End = false, Log = true };

            switch (ussdRequestString)
            {
                case "1":
                    resp.Response = UssdMenus.GotvPackagesMenuOld1;
                    resp.ToNode = "GOTV MONTHLY";
                    break;
                case "2":
                    resp.Response = UssdMenus.GotvMenuWeekly;
                    resp.ToNode = "GOTV WEEKLY";
                    break;
                case "3":
                    resp.Response = UssdMenus.GotvQuaterly;
                    resp.ToNode = "GOTV QUATERLY";
                    break;
                case "4":
                    resp.Response = UssdMenus.GotvMenuAnually;
                    resp.ToNode = "GOTV ANUALLY";
                    break;
                case "0":
                    resp.Response = UssdMenus.DstvGotvHomeMenu;
                    resp.ToNode = "SELECT PRODUCT";
                    break;
                default:
                    resp.Response = "Invalid Option";
                    resp.ToNode = "NONE";
                    resp.FromNode = "MAIN MENU";
                    break;
            }

            resp.FromNode = "SUBSCRIPTION MENU GOTV";
            return resp;
        }

        private ResponseObj ConfirmDstvPayment(string ussdRequestString, string transactionId)
        {
            ResponseObj resp = new ResponseObj { End = false, Log = true };
            string selectedPackage = "";
            switch (ussdRequestString)
            {
                case "2":
                    selectedPackage = "Compact: UGX 175,000";
                    break;
                case "1":
                    selectedPackage = "Premium: UGX 300,000";
                    break;
                case "3":
                    selectedPackage = "Compact: UGX 113,000";
                    break;
                case "4":
                    selectedPackage = "Family: UGX 113,000";
                    break;
                case "5":
                    selectedPackage = "Access: UGX 71,000";
                    break;
                case "6":
                    selectedPackage = "Lumba Bouquet: UGX 46,000";
                    break;
                case "0":
                    resp.Response = UssdMenus.SubscriptionTypeMenuDSTV;
                    resp.ToNode = "SUBSCRIPTION MENU DSTV";
                    resp.FromNode = "SUBSCRIPTION MENU MONTHLY";
                    return resp;
                default:
                    resp.Response = "Invalid selection. Please try again.";
                    resp.End = false;

                    return resp;
            }

            if (!string.IsNullOrEmpty(selectedPackage))
            {
                string pricePart = selectedPackage.Split(':')[1].Trim(); // "UGX 175,000"
                string packageName = selectedPackage.Split(':')[0].Trim();
                // Optionally, extract just the numeric part
                string priceNumeric = pricePart.Replace("UGX", "").Replace(",", "").Trim(); // "175000"
                decimal price = decimal.Parse(priceNumeric); // 175000

                UssdSession ussdSession = new UssdSession()
                {
                    SelectedPackage = packageName,
                    TransactionId = transactionId,
                    ClientId = transactionId,
                    SmartCardNumber = "",
                    boqouteCode = "",
                    Price = priceNumeric
                };

                UssdSessionStore.SaveSession(ussdSession);

            }
            resp.FromNode = "SUBSCRIPTION MENU MONTHLY";
            resp.Log = true;
            resp.Response = "Please enter smartcard/UCI Number";
            resp.End = false;
            resp.ToNode = "CONFIRM PAYMENT";
            return resp;
        }
        private ResponseObj ConfirmDstvShowMaxCombo(string ussdRequestString, string transactionId)
        {
            ResponseObj resp = new ResponseObj { End = false, Log = true };
            string selectedPackage = "";
            switch (ussdRequestString)
            {
                case "1":
                    selectedPackage = "Compact Plus + Showmax: UGX 185150";
                    break;
                case "2":
                    selectedPackage = "Compact + Showmax: UGX 125150";
                    break;
                case "3":
                    selectedPackage = "Family + Showmax: UGX 84150";
                    break;
                case "4":
                    selectedPackage = "Access + Showmax: UGX 60150";
                    break;
                case "5":
                    selectedPackage = "Premium E/Afr + Asian + Showmax: UGX 339000";
                    break;
                case "6":
                    selectedPackage = "Premium E/Afr + French + Showmax: UGX 447000";
                    break;
                case "0":
                    resp.Response = UssdMenus.SubscriptionTypeMenuDSTV;
                    resp.ToNode = "SUBSCRIPTION MENU DSTV";
                    resp.FromNode = "SUBSCRIPTION MENU SHOWMAX";
                    return resp;
                default:
                    resp.Response = "Invalid selection. Please try again.";
                    resp.End = false;
                    break;
            }

            if (!string.IsNullOrEmpty(selectedPackage))
            {
                string pricePart = selectedPackage.Split(':')[1].Trim(); // "UGX 175,000"

                // Optionally, extract just the numeric part
                string priceNumeric = pricePart.Replace("UGX", "").Replace(",", "").Trim(); // "175000"
                decimal price = decimal.Parse(priceNumeric); // 175000

                UssdSession ussdSession = new UssdSession()
                {
                    SelectedPackage = selectedPackage,
                    TransactionId = transactionId,
                    ClientId = transactionId,
                    SmartCardNumber = "",
                    boqouteCode = "",
                    Price = priceNumeric
                };

                UssdSessionStore.SaveSession(ussdSession);

            }
            resp.Response = "Please Enter smartcard/IUC Number";
            resp.End = false;
            resp.Log = true;
            resp.FromNode = "SUBSCRIPTION MENU WEEKLY";
            resp.ToNode = "CONFIRM PAYMENT";
            return resp;

        }

        private ResponseObj ConfirmDstvMovieAddOnCombo(string ussdRequestString, string transactionId)
        {
            ResponseObj resp = new ResponseObj { End = false, Log = true };
            string selectedPackage = "";
            switch (ussdRequestString)
            {
                case "1":
                    selectedPackage = "MOVIESCOMPLE36: UGX 37000";
                    break;
                case "2":
                    selectedPackage = "MOVIESE36: UGX 22000";
                    break;
                case "3":
                    selectedPackage = "MOVIESE36: UGX 14000";
                    break;
                case "4":
                    selectedPackage = "MOVIESE36: UGX 5000";
                    break;
                case "0":
                    resp.Response = UssdMenus.SubscriptionTypeMenuDSTV;
                    resp.ToNode = "SUBSCRIPTION MENU DSTV";
                    resp.FromNode = "SUBSCRIPTION MENU SHOWMAX";
                    return resp;
                default:
                    resp.Response = "Invalid selection. Please try again.";
                    resp.End = false;
                    break;
            }

            if (!string.IsNullOrEmpty(selectedPackage))
            {
                string pricePart = selectedPackage.Split(':')[1].Trim(); // "UGX 175,000"

                // Optionally, extract just the numeric part
                string priceNumeric = pricePart.Replace("UGX", "").Replace(",", "").Trim(); // "175000"
                decimal price = decimal.Parse(priceNumeric); // 175000

                UssdSession ussdSession = new UssdSession()
                {
                    SelectedPackage = selectedPackage,
                    TransactionId = transactionId,
                    ClientId = transactionId,
                    SmartCardNumber = "",
                    boqouteCode = "",
                    Price = priceNumeric
                };

                UssdSessionStore.SaveSession(ussdSession);

            }
            resp.Response = "Please enter smartcard/IUC number";
            resp.End = false;
            resp.Log = true;
            resp.FromNode = "SUBSCRIPTION MENU WEEKLY";
            resp.ToNode = "CONFIRM PAYMENT";
            return resp;

        }
        private ResponseObj ConfirmDstvPaymentWeekly(string ussdRequestString, string transactionId)
        {
            ResponseObj resp = new ResponseObj { End = false, Log = true };
            string selectedPackage = "";
            switch (ussdRequestString)
            {
                case "1":
                    selectedPackage = "DSTV Compact 7D: UGX 37000";
                    break;
                case "2":
                    selectedPackage = "DSTV Family 7D: UGX 22000";
                    break;
                case "3":
                    selectedPackage = "DSTV Access 7D: UGX 14000";
                    break;
                case "4":
                    selectedPackage = "DSTV Lumba 7D: UGX 5000";
                    break;
                case "5":
                    selectedPackage = "TopUp: UGX 5000";
                    break;
                case "0":
                    resp.Response = UssdMenus.SubscriptionTypeMenuDSTV;
                    resp.ToNode = "SUBSCRIPTION MENU DSTV";
                    resp.FromNode = "SUBSCRIPTION MENU WEEKLY";
                    return resp;
                default:
                    resp.Response = "Invalid selection. Please try again.";
                    resp.End = false;
                    break;
            }
            if (!string.IsNullOrEmpty(selectedPackage))
            {
                string pricePart = selectedPackage.Split(':')[1].Trim(); // "UGX 175,000"

                // Optionally, extract just the numeric part
                string priceNumeric = pricePart.Replace("UGX", "").Replace(",", "").Trim(); // "175000"
                decimal price = decimal.Parse(priceNumeric); // 175000
                string packageName = selectedPackage.Split(':')[0].Trim();


                UssdSession ussdSession = new UssdSession()
                {
                    SelectedPackage = packageName,
                    TransactionId = transactionId,
                    ClientId = transactionId,
                    SmartCardNumber = "",
                    boqouteCode = "",
                    Price = priceNumeric
                };

                UssdSessionStore.SaveSession(ussdSession);

            }
            if (!string.IsNullOrEmpty(selectedPackage))
            {
                string pricePart = selectedPackage.Split(':')[1].Trim(); // "UGX 175,000"

                // Optionally, extract just the numeric part
                string priceNumeric = pricePart.Replace("UGX", "").Replace(",", "").Trim(); // "175000"
                decimal price = decimal.Parse(priceNumeric); // 175000
                string packageName = selectedPackage.Split(':')[0].Trim();

                UssdSession ussdSession = new UssdSession()
                {
                    SelectedPackage = packageName,
                    TransactionId = "",
                    ClientId = "",
                    SmartCardNumber = "",
                    boqouteCode = "",
                    Price = priceNumeric
                };

                UssdSessionStore.SaveSession(ussdSession);

            }
            resp.Response = "Please enter smartcard/IUC number";
            resp.End = false;
            resp.Log = true;
            resp.FromNode = "SUBSCRIPTION MENU WEEKLY";
            resp.ToNode = "CONFIRM PAYMENT";
            return resp;

        }

        private ResponseObj ConfirmGotvPayment(string ussdRequestString, string menu, string transactionId)
        {
            ResponseObj resp = new ResponseObj { End = false, Log = true };
            switch (ussdRequestString)
            {
                case "0":
                    resp.FromNode = "CONFIRM PAYMENT";
                    resp.ToNode = "SUBSCRIPTION MENU GOTV";
                    resp.Response = UssdMenus.GotvPackagesMenuOld1;
                    return resp;
                case "00":
                    resp.FromNode = "CONFIRM PAYMENT";
                    resp.ToNode = "GOTV MONTHLY";
                    resp.Response = UssdMenus.GotvPackagesMenuOld2;
                    return resp;
                default:
                    string selectedPackage = menu.Split('\n')[int.Parse(ussdRequestString) - 1];
                    selectedPackage = selectedPackage.Split(')')[1];

                    if (!string.IsNullOrEmpty(selectedPackage))
                    {
                        string pricePart = selectedPackage.Split('-')[1].Trim(); // "UGX 175,000"

                        // Optionally, extract just the numeric part
                        string priceNumeric = pricePart.Replace("UGX", "").Replace(",", "").Trim(); // "175000"
                        decimal price = decimal.Parse(priceNumeric); // 175000
                        string packageName = selectedPackage.Split('-')[0].Trim();

                        UssdSession ussdSession = new UssdSession()
                        {
                            SelectedPackage = packageName,
                            TransactionId = transactionId,
                            ClientId = transactionId,
                            SmartCardNumber = "",
                            boqouteCode = "",
                            Price = priceNumeric
                        };

                        UssdSessionStore.SaveSession(ussdSession);

                    }


                    return new ResponseObj
                    {
                        Response = "Please enter martcard/IUC Number",
                        End = false,
                        Log = true,
                        ToNode = "CONFIRM PAYMENT",
                        FromNode = "GOTV PACKAGE SELECTION"
                    };



            }

        }

        private ResponseObj ConfirmGotvPaymentQuaterly(string ussdRequestString, string menu, string transactionId)
        {
            string selectedPackage = menu.Split('\n')[int.Parse(ussdRequestString) - 1];
            selectedPackage = selectedPackage.Split(')')[1];

            if (!string.IsNullOrEmpty(selectedPackage))
            {
                string pricePart = selectedPackage.Split(':')[1].Trim(); // "UGX 175,000"

                // Optionally, extract just the numeric part
                string priceNumeric = pricePart.Replace("UGX", "").Replace(",", "").Trim(); // "175000"
                decimal price = decimal.Parse(priceNumeric); // 175000
                string packageName = selectedPackage.Split(':')[0].Trim();

                UssdSession ussdSession = new UssdSession()
                {
                    SelectedPackage = packageName,
                    TransactionId = transactionId,
                    ClientId = transactionId,
                    SmartCardNumber = "",
                    boqouteCode = "",
                    Price = priceNumeric
                };

                UssdSessionStore.SaveSession(ussdSession);

            }


            return new ResponseObj
            {
                Response = "Please enter smartcard/IUC Number",
                End = false,
                Log = true,
                ToNode = "CONFIRM PAYMENT",
                FromNode = "GOTV PACKAGE SELECTION"
            };

           
        }

        private ResponseObj ConfirmGotvPaymentAnually(string ussdRequestString, string menu, string transactionId)
        {
            string selectedPackage = menu.Split('\n')[int.Parse(ussdRequestString) - 1];
            selectedPackage = selectedPackage.Split(')')[1];

            if (!string.IsNullOrEmpty(selectedPackage))
            {
                string pricePart = selectedPackage.Split(':')[1].Trim(); // "UGX 175,000"

                // Optionally, extract just the numeric part
                string priceNumeric = pricePart.Replace("UGX", "").Replace(",", "").Trim(); // "175000"
                decimal price = decimal.Parse(priceNumeric); // 175000
                string packageName = selectedPackage.Split(':')[0].Trim();

                UssdSession ussdSession = new UssdSession()
                {
                    SelectedPackage = packageName,
                    TransactionId = transactionId,
                    ClientId = transactionId,
                    SmartCardNumber = "",
                    boqouteCode = "",
                    Price = priceNumeric
                };

                UssdSessionStore.SaveSession(ussdSession);

            }
            return new ResponseObj
            {
                Response = "Please enter smartcard/IUC  Number",
                End = false,
                Log = true,
                ToNode = "CONFIRM PAYMENT",
                FromNode = "GOTV PACKAGE SELECTION"
            };
        }

        private ResponseObj ProcessUCINumber(string ussdRequestString, string transactionId)
        {
            ResponseObj resp = new ResponseObj { End = false, Log = true };
            UssdSession session = UssdSessionStore.GetSession(transactionId);

            if (session == null)
            {
                resp.Response = UssdMenus.DstvGotvHomeMenu;
                resp.End = false;
                resp.Log = true;
                resp.ToNode = "NONE";
                resp.FromNode = "MAIN MENU";
                return resp;
            }


            UssdSession ussdSession = new UssdSession()
            {
                SelectedPackage = session.SelectedPackage,
                TransactionId = session.TransactionId,
                ClientId = session.ClientId,
                SmartCardNumber = ussdRequestString,
                boqouteCode = session.boqouteCode,
                Price = session.Price
            };

            UssdSessionStore.SaveSession(ussdSession);


            resp.Response = "COMPLETED" + ":" + ussdSession.SelectedPackage + ":" + ussdSession.Price + ":" + ussdSession.SmartCardNumber + "";

            resp.End = false;
            resp.Log = true;
            resp.FromNode = "CONFIRM PAYMENT";
            resp.ToNode = "FINALNODE";
            return resp;

        }
        private ResponseObj ProcessFinalNode(string ussdRequestString, string transactionId)
        {
            ResponseObj resp = new ResponseObj { End = false, Log = true };
            UssdSession session = UssdSessionStore.GetSession(transactionId);

            if (session == null)
            {
                resp.Response = UssdMenus.DstvGotvHomeMenu;
                resp.End = false;
                resp.Log = true;
                resp.ToNode = "NONE";
                resp.FromNode = "MAIN MENU";
                return resp;
            }


            UssdSession ussdSession = new UssdSession()
            {
                SelectedPackage = session.SelectedPackage,
                TransactionId = session.TransactionId,
                ClientId = session.ClientId,
                SmartCardNumber = session.SmartCardNumber,
                boqouteCode = session.boqouteCode,
                Price = session.Price,
                CustomerName = session.CustomerName

            };

            UssdSessionStore.SaveSession(ussdSession);

            switch (ussdRequestString)
            {
                case ("1"):
                    resp.Response = "FINAL" + ":" + ussdSession.SelectedPackage + ":" + ussdSession.Price + ":" + ussdSession.SmartCardNumber + "";

                    break;
                default:
                    resp.Response = "SESSIONEND" + ":" + ussdSession.SelectedPackage + ":" + ussdSession.Price + ":" + ussdSession.SmartCardNumber + "";
                    break;
            }

            resp.End = true;
            resp.Log = true;
            resp.FromNode = "CONFIRM PAYMENT";
            resp.ToNode = "NONE";
            return resp;

        }
    }
    public class UssdSession
    {
        public string ClientId { get; set; }
        public string TransactionId { get; set; }
        public string SelectedPackage { get; set; }

        public string boqouteCode { get; set; }

        public string CustomerName { get; set; }
        public string Price { get; set; }
        public string SmartCardNumber { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public static class UssdSessionStore
    {
        private static readonly ConcurrentDictionary<string, UssdSession> _sessions
            = new ConcurrentDictionary<string, UssdSession>();

        // Add or update session data
        public static void SaveSession(UssdSession session)
        {
            session.LastUpdated = DateTime.Now;
            _sessions[session.TransactionId] = session;
        }

        // Retrieve session data
        public static UssdSession GetSession(string transactionId)
        {
            UssdSession session = null;
            return _sessions.TryGetValue(transactionId, out session) ? session : null;
        }

        // Remove session data
        public static void RemoveSession(string transactionId)
        {
            UssdSession session = null;
            _sessions.TryRemove(transactionId, out session);
        }

        // Cleanup old sessions (e.g., older than 5 minutes)
        public static void CleanupOldSessions()
        {
            UssdSession session = null;
            var expired = _sessions
                .Where(kvp => (DateTime.Now - kvp.Value.LastUpdated).TotalMinutes > 5)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expired)
            {
                _sessions.TryRemove(key, out session);
            }
        }
    }

}
