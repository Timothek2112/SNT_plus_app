using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SNT.Models
{
    public class ElectricityCardModel
    {
        public ElectricityCardModel() { }

        public string period { get; set; }
        public float debt { get; set; }
        public float payment { get; set; }
        public float amount { get; set; }
        public Color amountColor { get; set; }

        //Временный метод, пока не реализован функционал
        public void SetAmount()
        {
            this.amount = this.payment - this.debt;
            if(amount >= 0) this.amountColor = Color.Green;
            else this.amountColor = Color.Red;
            this.amount = (float)Math.Round((double)Math.Abs(this.amount), 2, MidpointRounding.AwayFromZero);

        }
    }
}
