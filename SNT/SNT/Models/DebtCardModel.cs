using System;
using System.Collections.Generic;
using System.Text;

namespace SNT.Models
{
    public class DebtCardModel
    {
        public float water { get; set; }
        public float electricity { get; set; }
        public float penality { get; set; }
        public float membership { get; set; }
        public float target { get; set; }
        public int month { get; set; }
        public int year { get; set; }

        public DebtCardModel()
        {
            water = 0;
            electricity = 0;
            penality = 0;
            membership = 0;
            target = 0;
            month = 0;
            year = 0;
        }
    }
}
