using SNT.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms;

namespace SNT.Repositories
{
    internal class CalculateRepository
    {

        public CalculateRepository() { }

        private static (int, int) GetPrevMonth(int year, int month)
        {
            month--;
            if(month < 1)
            {
                month = 12;
                year--;
            }
           return (year, month);
        }

        internal static PaymentPokazanieModel GetPokazanie(int year, int month, List<PaymentPokazanieModel> pokazania)
        {
            PaymentPokazanieModel prevPokazanie = pokazania.LastOrDefault(i => i.year <= year && i.month <= month);

            if (prevPokazanie != null) return prevPokazanie;
            else return new PaymentPokazanieModel();
        }

        public static async Task<List<DebtModel>> CalculateDebtForYear(List<PaymentPokazanieModel> pokazania, List<PaymentPokazanieModel> payments, List<RateModel> rates, int year)
        {
            List<DebtModel> debts = new List<DebtModel>();
            for (int y = 1; y <= DateTime.Now.Year % 100; y++)
            {
                for (int month = 1; month <= 12; month++)
                {
                    PaymentPokazanieModel prevPokazanie = new PaymentPokazanieModel();
                    PaymentPokazanieModel pokazanie = pokazania.FirstOrDefault(i => i.month == month && i.year == y);
                    PaymentPokazanieModel payment = payments.FirstOrDefault(i => i.month == month && i.year == y);
                    RateModel rate = new RateModel();
                    DebtModel debt = new DebtModel(year, 1);

                    if (payment != null && pokazanie != null)
                    {
                        rate = rates.LastOrDefault(i => i.year * 100 + i.month <= pokazanie.year * 100 + pokazanie.month);
                        var (lastY, lastM) = GetPrevMonth(y, month);
                        prevPokazanie = GetPokazanie(lastY, lastM, pokazania);
                        debt.calculateDebt(payment, pokazanie, rate, prevPokazanie);

                    }
                    else if (pokazanie != null)
                    {
                        rate = rates.LastOrDefault(i => i.year * 100 + i.month <= pokazanie.year * 100 + pokazanie.month);
                        var (lastY, lastM) = GetPrevMonth(y, month);
                        prevPokazanie = GetPokazanie(lastY, lastM, pokazania);
                        debt.calculateDebt(new PaymentPokazanieModel(), pokazanie, rate, prevPokazanie);

                    }
                    else if (payment != null)
                    {
                        rate = rates.LastOrDefault(i => i.year * 100 + i.month <= payment.year * 100 + payment.month);
                        debt.calculateDebt(payment, new PaymentPokazanieModel(), rate, new PaymentPokazanieModel());
                    }

                    if(y == year) debts.Add(debt);
                }
            }
            return debts;
        }

        public static async Task<List<PokazanieModel>> CalculatePokazania(List<PaymentPokazanieModel> pokazania, int year)
        {
            List<PokazanieModel> returnPokazania = new List<PokazanieModel>();
            for(int y = 1; y <= DateTime.Now.Year % 100; y++)
            {
                for (int m = 1; m <= 12; m++)
                {
                    PaymentPokazanieModel pokazanie = pokazania.LastOrDefault(i => i.month == m && i.year == y);
                    PokazanieModel pokazanieModel = new PokazanieModel();
                    var (lastY, lastM) = GetPrevMonth(y, m);
                    PaymentPokazanieModel prevPokazanie = GetPokazanie(lastY, lastM, pokazania);
                    if (pokazanie != null && prevPokazanie != null)
                    {
                        pokazanieModel.rawElectricity = pokazanie.electricity;
                        pokazanieModel.rawWater = pokazanie.water;

                        if (prevPokazanie.water <= pokazanie.water) pokazanieModel.water = pokazanie.water - prevPokazanie.water;
                        else pokazanieModel.water = pokazanieModel.rawWater;

                        if (prevPokazanie.electricity <= pokazanie.electricity) pokazanieModel.electricity = pokazanie.electricity - prevPokazanie.electricity;
                        else pokazanieModel.electricity = pokazanieModel.rawElectricity;
                    }
                    else
                    {
                        pokazanieModel.rawElectricity = prevPokazanie.electricity;
                        pokazanieModel.rawWater = prevPokazanie.water;
                        
                    }
                    

                    if (year == y) returnPokazania.Add(pokazanieModel);
                }
               
            }

            return returnPokazania;
        }

        public static async Task<List<DebtModel>> GetDebtForYear(int year, int uchastok, int sntId)
        {
            List<DebtModel> debts = null;
            try
            {
                DataRepository dataRepository = new DataRepository();
                List<PaymentPokazanieModel> pokazania = await dataRepository.GetStateForYear(yearFixed: year, uchastok, sntId, State.Pokazania) ;
                List<PaymentPokazanieModel> payments = await dataRepository.GetStateForYear(yearFixed: year, uchastok, sntId, State.Payments);
                List<RateModel> rates = await dataRepository.GetRates(sntId);
                debts = await CalculateDebtForYear(pokazania, payments, rates, year);
            }
            catch { }
            return debts;
        }
    }
}
