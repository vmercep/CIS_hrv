using DataObjects;
using System.Collections.Generic;

namespace CisDal
{
    public interface IMerlinData
    {
        List<DataBill> GetBill(string OIB, bool vatIsActive);

        bool checkIfNewTaxes();
        DataBill GetBillFollow(DataBill listBill);

        int SaveTicketJir(int ticketid, string jir);

        int SaveNotes(int ticketid, string notes);

        List<DataPerso> GetPerso();

        void UpdatePerso(string cmdSQL);

        void FlushBillsWithNoJir();
        List<DataNewTax> GetNewTaxes(int idTicket);
        List<DataBill> GetTicket(int dticket);
        int GetPersoByOib(string vATNumber_Salon);
        int UpdateTicketWithOib(int idTicket, int idCashier);

        List<DataBill> GetOffer(string OIB, bool vatIsActive);
    }
}