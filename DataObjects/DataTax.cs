using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObjects
{
    public class DataTax
    {
        public string TaxableAmount;

        public string TaxRate;

        public string TaxAmount;

        public string TaxFull;

        public int OperatorId;

        public string OperatorVAT;

        public int TaxId;

        public int NumArticle;

        public int TypePay;

        public NacinPlacanjaType MoyPayFinal;


        public DataTax()
        {
            TaxId = 0;
            OperatorId = 0;
            TypePay = 0;
            NumArticle = 0;
            TaxableAmount = string.Empty;
            TaxRate = string.Empty;
            TaxAmount = string.Empty;
            OperatorVAT = string.Empty;
            TaxFull = string.Empty;
        }
    }

}
