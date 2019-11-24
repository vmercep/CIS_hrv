namespace DataObjects
{
    public class DataPay
    {
        public int IdCtick;

        public int TypePay;

        public short IdCmoyPay;

        public decimal Prix;

        public NacinPlacanjaType MoyPayFinal;

        public bool Tip;

        public DataPay()
        {
            IdCtick = 0;
            TypePay = 0;
            IdCmoyPay = 0;
            Prix = default(decimal);
            Tip = false;
        }


    }
}