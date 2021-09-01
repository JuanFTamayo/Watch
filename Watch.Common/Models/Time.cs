using System;
using System.Collections.Generic;
using System.Text;

namespace Watch.Common.Models
{
    public class Time
    {
        public int EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public byte Type { get; set; }
        public bool IsConsolidated { get; set; }
    }
}
