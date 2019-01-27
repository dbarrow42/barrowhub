using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BarrowHubApp.Models
{
    public class Bill
    {
        public string from;
        public string subject;
        public string due;
        public string date;
        public string half;
        public void setHalf()
        {
            this.half = "$" + (Convert.ToDouble(this.due.Substring(1)) / 2).ToString("0.##");
        }
    }
}
