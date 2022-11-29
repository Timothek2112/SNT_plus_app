using System;
using System.Collections.Generic;
using System.Text;

namespace SNT.Models
{
    public class ElectricityCardModel
    {
        public ElectricityCardModel() { }

        public string period { get; set; }
        public double debt { get; set; }
        public double payment { get; set; }
        public double amount { get; set; }

        //Временный метод, пока не реализован функционал
        public void SetAmount()
        {
           this.amount = this.payment - this.debt;
        }
    }
}
