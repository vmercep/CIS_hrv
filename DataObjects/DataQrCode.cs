using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObjects
{
    public class DataQrCode
    {
        public string TotalAmount_Bill { get; set; }
        public int IdTicket { get; set; }
        public string Notes { get; set; }
        public DateTime DateTimeIssue_Bill { get; set; }

    }
}
