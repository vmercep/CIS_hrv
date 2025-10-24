using System;
using System.Collections.Generic;

namespace DataObjects
{
    public class DataBill
    {
        public string VATNumber_Salon_Bill { get; set; } = string.Empty;
        public bool TaxPayer_Bill { get; set; } = true;
        public DateTime BillDate_Bill { get; set; } = DateTime.Now;
        public OznakaSlijednostiType SequenceMark_Bill { get; set; } = OznakaSlijednostiType.P;
        public string BillNumberMark_Bill { get; set; } = string.Empty;
        public string PremiseMark_Bill { get; set; } = string.Empty;
        public string BillingDeviceMark_Bill { get; set; } = string.Empty;
        public string VATTaxRate_Bill { get; set; } = string.Empty;
        public string VATBase_Bill { get; set; } = string.Empty;
        public string VATAmount_Bill { get; set; } = string.Empty;
        public string TotalAmount_Bill { get; set; } = string.Empty;
        public NacinPlacanjaType PaymentMethod_Bill { get; set; } = NacinPlacanjaType.G;
        public string CashierVATNumber_Bill { get; set; } = string.Empty;
        public bool MarkSubseqBillDelivery_Bill { get; set; } = true;
        public int CountLigPay_Bill { get; set; } = 0;
        public int IdTicket { get; set; } = 0;
        public string HashStatus { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public short TypeTik { get; set; } = 1;
        public List<DataTax> TaxList { get; set; } = new List<DataTax>();
        public string Payment_After { get; set; } = string.Empty;

        public bool IsPro { get; set; } = false;
        public string BuyerTaxNumber { get; set; } = string.Empty;

        /// <summary>
        /// Formats a DateTime into "dd.MM.yyyyTHH:mm:ss" string (e.g. 27.08.2025T14:35:10).
        /// </summary>
        public string DateTimeIssue_Bill(DateTime billDate)
        {
            return $"{billDate:dd.MM.yyyy}T{billDate:HH:mm:ss}";
        }


        /*
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

        public string Payment_After;

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
            Payment_After = string.Empty;
        }
        */
    }
}