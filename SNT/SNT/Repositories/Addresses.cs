﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SNT.Repositories
{
    internal static class Addresses
    {
        public static string login = "http://10.0.2.2:7000/auth/login";
        public static string checkLogin = "http://10.0.2.2:7000/auth/checkLogin";
        public static string pokazaniaForPeriod = "http://10.0.2.2:7000/pokazania/pokazaniaForPeriod";
        public static string paymentsForPeriod = "http://10.0.2.2:7000/pokazania/paymentsForPeriod";
        public static string ratesForPeriod = "http://10.0.2.2:7000/pokazania/ratesForPeriod";
        public static string getUsersUchastki = "http://10.0.2.2:7000/auth/usersUchastki/";
        public static string createPokazanie = "http://10.0.2.2:7000/pokazania/createPokazanie";
        public static string getDebts = "http://10.0.2.2:7000/pokazania/calculateDebt";
    }
}
