using System;


namespace DataObjects
{
    public class DataSalon
    {
        public string VATNumber_Salon;

        public int PremiseMark_Salon;

        public short LogFileIsActive;

        public DateTime DateIsActive;

        public string BillingDeviceMark;

        public string OIBSoftware;

        public DataSalon()
        {
            VATNumber_Salon = string.Empty;
            PremiseMark_Salon = 0;
            LogFileIsActive = 0;
            DateIsActive = DateTime.Now;
            BillingDeviceMark = "";
            OIBSoftware = "";
        }
    }

}
