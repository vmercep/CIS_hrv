using DataObjects;
using System;
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

        List<DataQrCode> GetBillForQrCodeRegen(DateTime datetime);
        int CountQrCodeRegen(DateTime fromDate);

        List<SysLog> FetchSyslogWithDate(long id);

        void AlterMerlinTable();

        void UpdateSyslogStatus(int id);
        DataBill GetBillWithId(int objId, string vATNumber_Salon, bool vatActif);

        /// <summary>
        /// pobriši račune polije uvođenja eura...
        /// </summary>
        void FlushBillsWithNoJirPartial();
        DataTip GetTip(int idTicket);
        void UpdateTip(int idTicket, string m_Text);
        List<DataTip> GetFailedTips();

        DataBill GetOneBill(string OIB, bool vatIsActive, int billId);
    }
}