using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SNT.Models
{
    public class AppealModel
    {
        private const int THEME_LENGTH_LIMIT = 50;
        private const int MAIN_TEXT_LENGTH_LIMIT = 100;
        public string theme { get; set; }
        public string themeRaw { get; set; }
        public string mainTextRaw { get; set; }
        public string mainText { get; set; }
        public string answer { get; set; }
        public DateTime answerDate { get; set; }
        public bool status { get; set; }
        public string statusString { get; set; }
        public Color statusColor { get; set; }
        public string date { get; set; }

        public AppealModel(string theme, string mainText, bool isAnswered, string answer, string date)
        {
            this.theme = theme;
            this.mainTextRaw = mainText;
            this.themeRaw = theme;
            this.mainText = mainText;
            this.status = isAnswered;
            this.answer = answer;
            this.date = date;
        }    

        public void BringToShowForm()
        {
            this.themeRaw = this.theme;
            this.mainTextRaw = this.mainText;
            if (this.theme.Length > THEME_LENGTH_LIMIT) this.theme = new string(this.theme.Take(THEME_LENGTH_LIMIT).ToArray()) + "...";
            if (this.mainText.Length > MAIN_TEXT_LENGTH_LIMIT) this.mainText = new string(this.mainText.Take(MAIN_TEXT_LENGTH_LIMIT).ToArray()) + "...";
            if (status) { this.statusString = "Отвечено"; statusColor = Color.Green; }
            else { this.statusString = "Еще нет ответа"; statusColor = Color.Red; }
        }
    }
}
