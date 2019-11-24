using System;
using System.Collections.Generic;

namespace DataObjects
{
    public class DataBill
    {
        public string VATNumber_Salon_Bill;

        public bool TaxPayer_Bill;

        public DateTime BillDate_Bill;

        public OznakaSlijednostiType SequenceMark_Bill;

        public string BillNumberMark_Bill;

        public string PremiseMark_Bill;

        public string BillingDeviceMark_Bill;

        public string VATTaxRate_Bill;

        public string VATBase_Bill;

        public string VATAmount_Bill;

        public string TotalAmount_Bill;

        public NacinPlacanjaType PaymentMethod_Bill;

        public string CashierVATNumber_Bill;

        public bool MarkSubseqBillDelivery_Bill;

        public int CountLigPay_Bill;

        public int IdTicket;

        public string HashStatus;

        public string Notes;

        public short TypeTik;

        public List<DataTax> TaxList;

        public string DateTimeIssue_Bill(DateTime MyBillDate)
        {
            return string.Format("{0:dd.MM.yyyy}T{1}", MyBillDate, MyBillDate.ToString("HH:mm:ss"));
        }



        public DataBill()
        {
            VATNumber_Salon_Bill = string.Empty;
            TaxPayer_Bill = true;
            BillDate_Bill = DateTime.Now;
            SequenceMark_Bill = OznakaSlijednostiType.P;
            BillNumberMark_Bill = string.Empty;
            PremiseMark_Bill = string.Empty;
            BillingDeviceMark_Bill = string.Empty;
            VATTaxRate_Bill = string.Empty;
            VATBase_Bill = string.Empty;
            VATAmount_Bill = string.Empty;
            TotalAmount_Bill = string.Empty;
            PaymentMethod_Bill = NacinPlacanjaType.G;
            CashierVATNumber_Bill = string.Empty;
            MarkSubseqBillDelivery_Bill = true;
            CountLigPay_Bill = 0;
            IdTicket = 0;
            HashStatus = string.Empty;
            Notes = string.Empty;
            TypeTik = 1;
            TaxList = new List<DataTax>();
        }
    }
}