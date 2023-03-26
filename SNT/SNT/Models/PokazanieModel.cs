using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SNT.Models
{
    internal class PaymentPokazanieModel
    {
        public float membership = 0;
        public float water = 0;
        public float electricity = 0;
        public float target = 0;
        public float penality = 0;
        public int month = 0;
        public int year = 0;

        public PaymentPokazanieModel() { }
        public PaymentPokazanieModel(float membership, float water, float electricity, float target, float penality, int month, int year) {
            this.membership = membership;
            this.water = water;
            this.electricity = electricity;
            this.target = target;
            this.penality = penality;
            this.month = month;
            this.year = year;
        }
    }

    internal class DebtModel
    {
        public float water = 0;
        public float electricityActualDebt = 0;
        public float waterPokazanieActualDebt = 0;
        public float electricity = 0;
        public float target = 0;
        public float penality = 0;
        public float membership = 0;
        public int month = 1;
        public int year = 1;
        public PaymentPokazanieModel payment = new PaymentPokazanieModel();
        public PaymentPokazanieModel pokazanie = new PaymentPokazanieModel();
        public RateModel rate = new RateModel();
        public DebtModel() { }

        public DebtModel(int year, int month)
        {
            this.year = year;   
            this.month = month;
        }

        private static float CalculateMainState(float state, float prevState, float rate)
        {
            if(prevState > state)
            {
                return state * rate;
            }
            else
            {
                return (state - prevState) * rate;
            }
        }

        public void calculateDebt(PaymentPokazanieModel payment, PaymentPokazanieModel pokazanie, RateModel rate, PaymentPokazanieModel prevPokazanie)
        {
            if(prevPokazanie == null) prevPokazanie = new PaymentPokazanieModel();
            this.waterPokazanieActualDebt = CalculateMainState(pokazanie.water, prevPokazanie.water, rate.water);
            this.electricityActualDebt = CalculateMainState(pokazanie.electricity, prevPokazanie.electricity, rate.electricity);
            this.water = waterPokazanieActualDebt - payment.water;
            this.electricity = electricityActualDebt - payment.electricity;
            this.target = pokazanie.target - payment.target;
            this.penality = pokazanie.penality - payment.penality;
            this.membership = pokazanie.membership - payment.membership;
            if (pokazanie != null)
                this.pokazanie = pokazanie;
            if(payment != null)
                this.payment = payment;
            if(rate != null)
                this.rate = rate;
        }
    }

    internal class RateModel
    {
        public float water = 0;
        public float electricity = 0;
        public int month = 1;
        public int year = 1;

        public RateModel() { }

        public RateModel(float water, float electricity, int month, int year)
        {
            this.water = water;
            this.electricity = electricity;
            this.month = month;
            this.year = year;
        }
    }

    internal class PokazanieModel : PaymentPokazanieModel
    {
        public float rawWater = 0;
        public float rawElectricity = 0;
    }
}
