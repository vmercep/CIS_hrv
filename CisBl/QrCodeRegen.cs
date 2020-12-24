using CisDal;
using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CisBl
{
    public class QrCodeRegen
    {
        public bool QrCodeRegeneration(DateTime fromDate)
        {
            IMerlinData dalMerlin = new MerlinData();
            try
            {
                var dataToRegen = dalMerlin.GetBillForQrCodeRegen(fromDate);
                LogFile.LogToFile("Found " + dataToRegen.Count +" to regenerate bills", LogLevel.Debug);
                foreach (var data in dataToRegen)
                {
                    string zki = data.Notes.Replace("ZKI:", "").Trim();
                    LogFile.LogToFile("Regenerate bill for ZKI " + zki, LogLevel.Debug);
                    if (!string.IsNullOrEmpty(zki)) BarCode.GenerateQrCode(zki, data.DateTimeIssue_Bill, data.TotalAmount_Bill, data.IdTicket);
                }
                

            }
            catch(Exception e)
            {
                LogFile.LogToFile("Error ocured in qr code regeneration "+e.InnerException, LogLevel.Debug);
                LogFile.LogToFile("Error ocured in qr code regeneration " + e.Message, LogLevel.Debug);
                return false;
            }
            return true;
        }
    }
}
