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
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


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
                log.Debug("Found " + dataToRegen.Count +" to regenerate bills");
                int i = 0;
                foreach (var data in dataToRegen)
                {
                    progress.Report(i);
                    string zki = data.Notes.Replace("ZKI:", "").Trim();
                    log.Debug("Regenerate bill for ZKI " + zki);
                    if (!string.IsNullOrEmpty(zki)) BarCode.GenerateQrCode(zki, data.DateTimeIssue_Bill, data.TotalAmount_Bill, data.IdTicket);
                    i++;
                }
                

            }
            catch(Exception e)
            {
                log.Debug("Error ocured in qr code regeneration "+e.InnerException);
                log.Debug("Error ocured in qr code regeneration " + e.Message);
                throw new Exception("Error in qrGen "+e.Message);
            }
        }

        public void QrCodeRegenerationTest(IProgress<int> progress)
        {

            try
            {
                var dataToRegen = 1000;
                for(int i=0;i<dataToRegen;i++)
                {
                    progress.Report(i);
                    Thread.Sleep(10);
 
                }


            }
            catch (Exception e)
            {
                log.Error("Error ocured in qr code regeneration ", e);
                throw new Exception("Error in qrGen " + e.Message);
            }
        }
    }
}
