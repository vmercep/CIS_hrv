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
        IMerlinData _dalMerlin = new MerlinData();
        private readonly DateTime _dateFrom;

        public QrCodeRegen(DateTime dateFrom)
        {
            _dateFrom = dateFrom;
        }
        

        public int QrCodeNumbers()
        {
            int countDataToRegen = _dalMerlin.CountQrCodeRegen(_dateFrom);
            return countDataToRegen;

        }

        public delegate void ReportProgressDelegate(int percentage);

        public void QrCodeRegeneration(ReportProgressDelegate reportProgress)
        {
            try
            {
                var dataToRegen = _dalMerlin.GetBillForQrCodeRegen(_dateFrom);
                int count = dataToRegen.Count;
                LogFile.LogToFile("Found " + dataToRegen.Count +" to regenerate bills", LogLevel.Debug);
                foreach (var data in dataToRegen)
                {
                    reportProgress(count);
                    count++;
                    string zki = data.Notes.Replace("ZKI:", "").Trim();
                    LogFile.LogToFile("Regenerate bill for ZKI " + zki, LogLevel.Debug);
                    if (!string.IsNullOrEmpty(zki)) BarCode.GenerateQrCode(zki, data.DateTimeIssue_Bill, data.TotalAmount_Bill, data.IdTicket);
                }
                

            }
            catch(Exception e)
            {
                LogFile.LogToFile("Error ocured in qr code regeneration "+e.InnerException, LogLevel.Debug);
                LogFile.LogToFile("Error ocured in qr code regeneration " + e.Message, LogLevel.Debug);
                throw new Exception("Error in qrGen "+e.Message);
            }
        }
    }
}
