using System;
using System.Collections.Generic;
using System.Text;

namespace SNT.Models
{
    public class NewsModel
    {
        public string title { get; set; }
        public string full_text { get; set; }
        public DateTime date { get; set; }

        public NewsModel(string title, string full_text, DateTime date) 
        {
            this.title = title;
            this.full_text = full_text;
            this.date = date;
        }
    }
}
