using CisDal;
using Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CisBl
{
    public class QrCodeRegen
    {
        IMerlinData _dalMerlin = new MerlinData();


        public QrCodeRegen()
        {

        }

        public int GetDataCount(DateTime fromDate)
        {
            var dataToRegen = _dalMerlin.GetBillForQrCodeRegen(fromDate);      
            return dataToRegen.Count+5;

        }
 

        public void QrCodeRegeneration(DateTime fromDate, IProgress<int> progress)
        {
            try
            {
                var dataToRegen = _dalMerlin.GetBillForQrCodeRegen(fromDate);
                LogFile.LogToFile("Found " + dataToRegen.Count +" to regenerate bills", LogLevel.Debug);
                int i = 0;
                foreach (var data in dataToRegen)
                {
                    progress.Report(i);
                    string zki = data.Notes.Replace("ZKI:", "").Trim();
                    LogFile.LogToFile("Regenerate bill for ZKI " + zki, LogLevel.Debug);
                    if (!string.IsNullOrEmpty(zki)) BarCode.GenerateQrCode(zki, data.DateTimeIssue_Bill, data.TotalAmount_Bill, data.IdTicket);
                    i++;
                }
                

            }
            catch(Exception e)
            {
                LogFile.LogToFile("Error ocured in qr code regeneration "+e.InnerException, LogLevel.Debug);
                LogFile.LogToFile("Error ocured in qr code regeneration " + e.Message, LogLevel.Debug);
                throw new Exception("Error in qrGen "+e.Message);
            }
        }

        public void QrCodeRegenerationTest(IProgress<int> progress)
        {

            try
            {
                var dataToRegen = 1000;
                int count = 1000;
                //LogFile.LogToFile("Found " + dataToRegen.Count + " to regenerate bills", LogLevel.Debug);
                for(int i=0;i<dataToRegen;i++)
                {
                    progress.Report(i);
                    Thread.Sleep(10);
                    //count++;
                    //string zki = data.Notes.Replace("ZKI:", "").Trim();
                    //LogFile.LogToFile("Regenerate bill for ZKI " + zki, LogLevel.Debug);
                    //if (!string.IsNullOrEmpty(zki)) BarCode.GenerateQrCode(zki, data.DateTimeIssue_Bill, data.TotalAmount_Bill, data.IdTicket);
                }


            }
            catch (Exception e)
            {
                LogFile.LogToFile("Error ocured in qr code regeneration " + e.InnerException, LogLevel.Debug);
                LogFile.LogToFile("Error ocured in qr code regeneration " + e.Message, LogLevel.Debug);
                throw new Exception("Error in qrGen " + e.Message);
            }
        }
    }
}
