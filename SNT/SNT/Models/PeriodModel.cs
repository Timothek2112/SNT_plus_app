using System;
using System.Collections.Generic;
using System.Text;

namespace SNT.Models
{
    public class PeriodModel
    {
        public int yearStart;
        public int yearEnd;
        public int monthStart;
        public int monthEnd;
        public int yearFixed;
        public int monthFixed;

        public PeriodModel() { }
        public PeriodModel(int yearStart, int yearEnd, int monthStart, int monthEnd) {
            this.yearStart= yearStart;
            this.yearEnd= yearEnd;
            this.monthStart= monthStart;
            this.monthEnd= monthEnd;
        }
    
    }
}
