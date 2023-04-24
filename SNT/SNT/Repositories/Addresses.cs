using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace SNT.Repositories
{
    internal static class Addresses
    {
        public static string login = "http://109.174.29.40:7000/auth/login";
        public static string checkLogin = "http://109.174.29.40:7000/auth/checkLogin";
        public static string pokazaniaForPeriod = "http://109.174.29.40:7000/pokazania/pokazaniaForPeriod";
        public static string paymentsForPeriod = "http://109.174.29.40:7000/pokazania/paymentsForPeriod";
        public static string ratesForPeriod = "http://109.174.29.40:7000/pokazania/ratesForPeriod";
        public static string getUsersUchastki = "http://109.174.29.40:7000/auth/usersUchastki/";
        public static string createPokazanie = "http://109.174.29.40:7000/pokazania/createPokazanie";
        public static string getDebts = "http://109.174.29.40:7000/pokazania/calculateDebt";
        public static string getNews = "http://109.174.29.40:7000/news/first/";
        public static string getAppeals = "http://109.174.29.40:7000/appeal/forUser/";
        public static string postAppeal = "http://109.174.29.40:7000/appeal";
        public static string getUchastokId = "http://109.174.29.40:7000/get-pass/uchastokId/";
        public static string sendPushToken = "http://109.174.29.40:7000/auth/SetPushToken";
    }
}
