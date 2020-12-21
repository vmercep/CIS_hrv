using DataObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using static DataObjects.DataBill;

namespace CisDal
{
    public class MerlinData : IMerlinData, IDisposable
    {

        private SqlConnection _sqlConnection;

        private SqlConnection GetSqlConnection()
        {
            _sqlConnection = new SqlConnection(AppLink.ConnectionString);
            _sqlConnection.Open();
            return _sqlConnection;
        }


        public bool checkIfNewTaxes()
        {
            using (SqlConnection sqlConnection = GetSqlConnection())
            {
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = "\r\n\t\t\t\t\tselect case when exists((select * from information_schema.tables where table_name = 'CaisseTicketTaxe')) then 1 else 0 end\r\n\t\t\t\t";
                return (int)sqlCommand.ExecuteScalar() == 1;
            }
        }

        /// <summary>
        /// metoda za dohvat ponuda
        /// </summary>
        /// <param name="OIB"></param>
        /// <param name="vatIsActive"></param>
        /// <returns></returns>
        public List<DataBill> GetOffer(string OIB, bool vatIsActive)
        {
            List<DataBill> allBills = new List<DataBill>();
            DataSalon dataSalon = new DataSalon();
            //dataSalon.DateIsActive = Convert.ToDateTime(AppLink.DateIsActive);
            dataSalon.DateIsActive = DateTime.Now.AddDays(-60);
            try
            {
                using (SqlConnection sqlConnection = GetSqlConnection())
                {
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandText = "SELECT DateHeure, MontantHT, PrixFacture, code, id, Hash, idsysmachine, typetik, notes from caisseticket where datalength(Hash) > 64 and typetik in(3) and id>=@Date";

                    sqlCommand.Parameters.Add(new SqlParameter("@Date", AppLink.LongFromDate(dataSalon.DateIsActive)));
                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {
                            DataBill dataBill = new DataBill();
                            dataBill.VATNumber_Salon_Bill = OIB;
                            dataBill.TaxPayer_Bill = vatIsActive;
                            dataBill.BillDate_Bill = AppLink.DateFromLong((int)sqlDataReader["DateHeure"]);
                            dataBill.SequenceMark_Bill = OznakaSlijednostiType.P;
                            dataBill.PremiseMark_Bill = AppLink.PremiseMark;
                            dataBill.BillingDeviceMark_Bill = AppLink.BillingDeviceMark;
                            dataBill.BillNumberMark_Bill = Convert.ToString(sqlDataReader["Code"]);
                            
                            dataBill.TypeTik = (short)sqlDataReader["typetik"];
                            dataBill.IdTicket = (int)sqlDataReader["Id"];
                            dataBill.HashStatus = ((sqlDataReader["Hash"] == DBNull.Value) ? "0" : ((string)sqlDataReader["Hash"]));
                            dataBill.Notes = ((sqlDataReader["notes"] == DBNull.Value) ? "" : ((string)sqlDataReader["notes"]));
                            
                            allBills.Add(dataBill);
                            
                        }
                        sqlDataReader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                SimpleLog.Log(ex);
                throw new Exception("error ocured in dal get bill method " + ex.Message);
            }
            return allBills;
        }


        /// <summary>
        /// metoda za dohvat računa
        /// </summary>
        /// <param name="OIB"></param>
        /// <param name="vatIsActive"></param>
        /// <returns></returns>
        public List<DataBill> GetBill(string OIB, bool vatIsActive)
        {
            List<DataBill> allBills = new List<DataBill>();
            DataSalon dataSalon = new DataSalon();
            //dataSalon.DateIsActive = Convert.ToDateTime(AppLink.DateIsActive);
            dataSalon.DateIsActive = DateTime.Now.AddDays(-60);
            try
            {
                using (SqlConnection sqlConnection = GetSqlConnection())
                {
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandText = "SELECT DateHeure, MontantHT, PrixFacture, code, id, Hash, idsysmachine, typetik, notes, \r\n                    (select top 1 taux from systauxtva where flagarchive=0 order by taux desc) TauxTva, \r\n                    (select count(idcaisseticket) from caisseligpaiement where idcaisseticket=caisseticket.id) CountLigPay, \r\n                    (select numerosecu from perso where id=caisseticket.idpersoencaiss) OIBPerso from caisseticket\r\n                    where (Hash like @HashE or Hash like @Hash0 or Hash like @Hash1 or Hash is null or datalength(Hash) > 36) and typetik in(1,2) and id>=@Date";
                    sqlCommand.Parameters.Add(new SqlParameter("@HashE", "ERROR_%"));
                    sqlCommand.Parameters.Add(new SqlParameter("@Hash0", "0"));
                    sqlCommand.Parameters.Add(new SqlParameter("@Hash1", ""));
                    sqlCommand.Parameters.Add(new SqlParameter("@Date", AppLink.LongFromDate(dataSalon.DateIsActive)));
                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {
                            DataBill dataBill = new DataBill();
                            dataBill.VATNumber_Salon_Bill = OIB;
                            dataBill.TaxPayer_Bill = vatIsActive;
                            dataBill.BillDate_Bill = AppLink.DateFromLong((int)sqlDataReader["DateHeure"]);
                            dataBill.SequenceMark_Bill = OznakaSlijednostiType.P;
                            dataBill.PremiseMark_Bill = AppLink.PremiseMark;
                            dataBill.BillingDeviceMark_Bill = AppLink.BillingDeviceMark;
                            dataBill.BillNumberMark_Bill = Convert.ToString(sqlDataReader["Code"]);
                            if (vatIsActive)
                            {
                                dataBill.VATTaxRate_Bill = ((int)sqlDataReader["TauxTVA"] / 100).ToString("0.00");
                                dataBill.VATBase_Bill = ((decimal)sqlDataReader["MontantHT"] / 100m).ToString("0.00");
                            }
                            else
                            {
                                dataBill.VATTaxRate_Bill = "0.00";
                                dataBill.VATBase_Bill = "0.00";
                            }
                            dataBill.TypeTik = (short)sqlDataReader["typetik"];
                            dataBill.CashierVATNumber_Bill = ((sqlDataReader["OIBPerso"] == DBNull.Value) ? "OIBPERSOERROR" : Convert.ToString(sqlDataReader["OIBPerso"]));
                            dataBill.CountLigPay_Bill = (int)sqlDataReader["CountLigPay"];
                            dataBill.IdTicket = (int)sqlDataReader["Id"];
                            dataBill.HashStatus = ((sqlDataReader["Hash"] == DBNull.Value) ? "0" : ((string)sqlDataReader["Hash"]));
                            dataBill.Notes = ((sqlDataReader["notes"] == DBNull.Value) ? "" : ((string)sqlDataReader["notes"]));
                            if (dataBill.HashStatus.Length < 36 || dataBill.HashStatus.Length > 36)
                            {
                                allBills.Add(dataBill);
                            }
                        }
                        sqlDataReader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                SimpleLog.Log(ex);
                throw new Exception("error ocured in dal get bill method "+ex.Message);
            }
            return allBills;
        }


        public List<DataQrCode> GetBillForQrCodeRegen()
        {

            List<DataQrCode> allBills = new List<DataQrCode>();
            DateTime dateIsActive = DateTime.Now.AddDays(-120);
            try
            {
                using (SqlConnection sqlConnection = GetSqlConnection())
                {
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandText = "SELECT id, DateHeure,notes  from CaisseTicket where typetik in(1,2) and id>=@Date";
                    sqlCommand.Parameters.Add(new SqlParameter("@Date", AppLink.LongFromDate(dateIsActive)));
                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {

                            DataQrCode dataBill = new DataQrCode();
                            dataBill.IdTicket= (int)sqlDataReader["Id"];
                            dataBill.Notes = ((sqlDataReader["notes"] == DBNull.Value) ? "" : ((string)sqlDataReader["notes"]));
                            dataBill.TotalAmount_Bill = GetBillTotalAmount(dataBill.IdTicket);
                            dataBill.DateTimeIssue_Bill= AppLink.DateFromLong((int)sqlDataReader["DateHeure"]);

                            allBills.Add(dataBill);
                        }
                        sqlDataReader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                SimpleLog.Log(ex);
                throw new Exception("error ocured in GetBillForQrCodeRegen method " + ex.Message);
            }
            return allBills;



        }


        private string GetBillTotalAmount(int ticketId)
        {
            string dataToPay;
            //int num = 0;
            List<DataPay> list = new List<DataPay>();
            decimal num2 = default(decimal);
            int num3 = 0;
            int num4 = 0;
            try
            {
                using (SqlConnection sqlConnection = GetSqlConnection())
                {
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandText = "SELECT IdCaisseTicket, Id, (select idcaissetypemoypaiement from caissemoypaiement where \r\n\t\t\t\t\tid=caisseligpaiement.idCaisseMoyPaiement) TYPEPAI,IdCaisseMoyPaiement, prix, InfoMisc3 \r\n                    from CaisseLigPaiement where IdCaisseTicket=@IdCaisseTicket";
                    sqlCommand.Parameters.Add(new SqlParameter("@IdCaisseTicket", ticketId));
                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {
                            DataPay dataPay = new DataPay();
                            dataPay.IdCtick = (int)sqlDataReader["IdCaisseTicket"];
                            dataPay.TypePay = (int)sqlDataReader["TYPEPAI"];
                            dataPay.IdCmoyPay = (short)sqlDataReader["IdCaisseMoyPaiement"];
                            dataPay.Prix = (decimal)sqlDataReader["prix"];
                            dataPay.Tip = (((string)sqlDataReader["InfoMisc3"] == "TIP") ? true : false);
                            switch (sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("TYPEPAI")))
                            {
                                case 0:
                                    dataPay.MoyPayFinal = NacinPlacanjaType.G;
                                    break;
                                case 1:
                                    dataPay.MoyPayFinal = NacinPlacanjaType.C;
                                    break;
                                case 2:
                                    dataPay.MoyPayFinal = NacinPlacanjaType.K;
                                    break;
                                case 3:
                                    dataPay.MoyPayFinal = NacinPlacanjaType.C;
                                    break;
                                case 12:
                                    dataPay.MoyPayFinal = NacinPlacanjaType.T;
                                    break;
                                default:
                                    dataPay.MoyPayFinal = NacinPlacanjaType.O;
                                    break;
                            }
                            list.Add(dataPay);
                        }
                        sqlDataReader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                SimpleLog.Log(ex);
                throw new Exception("error ocured in GetBillFollow " + ex.Message);
            }
            try
            {

                    foreach (DataPay item in list)
                    {
                        switch (item.MoyPayFinal)
                        {
                            case NacinPlacanjaType.G:
                                num2 += item.Prix;
                                num3++;
                                break;
                            case NacinPlacanjaType.C:
                                num2 += item.Prix;
                                num3++;
                                break;
                            case NacinPlacanjaType.K:
                                num2 += item.Prix;
                                num3++;
                                break;
                            case NacinPlacanjaType.T:
                                num2 += item.Prix;
                                num3++;
                                break;
                            case NacinPlacanjaType.O:
                                num4++;
                                break;
                        }
                        
                    }
                
                dataToPay = (num2 / 100m).ToString("0.00");
                return dataToPay;
            }
            catch (Exception ex2)
            {
                SimpleLog.Log(ex2);
                throw new Exception("error ocured in GetBillTotalAmount " + ex2.Message);
            }

        }



        /// <summary>
        /// dohvat detalja računA
        /// </summary>
        /// <param name="Bill"></param>
        /// <returns></returns>
        public DataBill GetBillFollow(DataBill Bill)
        {
            //int num = 0;
            List<DataPay> list = new List<DataPay>();
            decimal num2 = default(decimal);
            int num3 = 0;
            int num4 = 0;
            try
            {
                using (SqlConnection sqlConnection = GetSqlConnection())
                {
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandText = "SELECT IdCaisseTicket, Id, (select idcaissetypemoypaiement from caissemoypaiement where \r\n\t\t\t\t\tid=caisseligpaiement.idCaisseMoyPaiement) TYPEPAI,IdCaisseMoyPaiement, prix, InfoMisc3 \r\n                    from CaisseLigPaiement where IdCaisseTicket=@IdCaisseTicket";
                    sqlCommand.Parameters.Add(new SqlParameter("@IdCaisseTicket", Bill.IdTicket));
                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {
                            DataPay dataPay = new DataPay();
                            dataPay.IdCtick = (int)sqlDataReader["IdCaisseTicket"];
                            Bill.BillDate_Bill = AppLink.DateFromLong((int)sqlDataReader["Id"]);
                            dataPay.TypePay = (int)sqlDataReader["TYPEPAI"];
                            dataPay.IdCmoyPay = (short)sqlDataReader["IdCaisseMoyPaiement"];
                            dataPay.Prix = (decimal)sqlDataReader["prix"];
                            dataPay.Tip = (((string)sqlDataReader["InfoMisc3"] == "TIP") ? true : false);
                            switch (sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("TYPEPAI")))
                            {
                                case 0:
                                    dataPay.MoyPayFinal = NacinPlacanjaType.G;
                                    break;
                                case 1:
                                    dataPay.MoyPayFinal = NacinPlacanjaType.C;
                                    break;
                                case 2:
                                    dataPay.MoyPayFinal = NacinPlacanjaType.K;
                                    break;
                                case 3:
                                    dataPay.MoyPayFinal = NacinPlacanjaType.C;
                                    break;
                                case 12:
                                    dataPay.MoyPayFinal = NacinPlacanjaType.T;
                                    break;
                                default:
                                    dataPay.MoyPayFinal = NacinPlacanjaType.O;
                                    break;
                            }
                            list.Add(dataPay);
                        }
                        sqlDataReader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                SimpleLog.Log(ex);
                throw new Exception("error ocured in GetBillFollow "+ex.Message);
            }
            try
            {
                if (list.Count == 0)
                {
                    Bill.PaymentMethod_Bill = NacinPlacanjaType.O;
                }
                else
                {
                    foreach (DataPay item in list)
                    {
                        switch (item.MoyPayFinal)
                        {
                            case NacinPlacanjaType.G:
                                num2 += item.Prix;
                                num3++;
                                break;
                            case NacinPlacanjaType.C:
                                num2 += item.Prix;
                                num3++;
                                break;
                            case NacinPlacanjaType.K:
                                num2 += item.Prix;
                                num3++;
                                break;
                            case NacinPlacanjaType.T:
                                num2 += item.Prix;
                                num3++;
                                break;
                            case NacinPlacanjaType.O:
                                num4++;
                                break;
                        }
                        if (num3 == 1 && item.MoyPayFinal != NacinPlacanjaType.O)
                        {
                            Bill.PaymentMethod_Bill = item.MoyPayFinal;
                        }
                        if (num3 > 1 && item.MoyPayFinal != NacinPlacanjaType.O)
                        {
                            Bill.PaymentMethod_Bill = NacinPlacanjaType.O;
                        }
                        if (num4 >= 1 && num3 == 0)
                        {
                            Bill.PaymentMethod_Bill = NacinPlacanjaType.O;
                        }
                        if (item.TypePay == 9)
                        {
                            item.Prix *= decimal.MinusOne;
                            num2 += item.Prix;
                        }
                    }
                }
                Bill.TotalAmount_Bill = (num2 / 100m).ToString("0.00");
                switch (Bill.TypeTik)
                {
                    case 1:
                        if (Convert.ToDecimal(Bill.VATBase_Bill) == decimal.Zero || num2 == decimal.Zero)
                        {
                            Bill.VATAmount_Bill = 0.ToString("0.00");
                        }
                        else
                        {
                            Bill.VATAmount_Bill = (Convert.ToDecimal(num2 / 100m) - Convert.ToDecimal(Bill.VATBase_Bill)).ToString("0.00");
                        }
                        break;
                    case 2:
                        {
                            decimal d = Convert.ToDecimal(num2);
                            decimal d2 = Convert.ToDecimal(Bill.VATTaxRate_Bill);
                            decimal d3 = (d - d / (decimal.One + d2 / 100m)) / 100m;
                            d3 = -d3;
                            Bill.VATAmount_Bill = d3.ToString("-0.00");
                            Bill.VATBase_Bill = ((num2 + d3 * 100m) / 100m).ToString("0.00");
                            if (!Bill.TaxPayer_Bill)
                            {
                                Bill.VATAmount_Bill = "0.00";
                                Bill.VATBase_Bill = "0.00";
                            }
                            break;
                        }
                }
                return Bill;
            }
            catch (Exception ex2)
            {
                SimpleLog.Log(ex2);
                throw new Exception("error ocured in GetBillFollow " + ex2.Message);
            }
        }

        public int SaveTicketJir(int ticketid, string jir)
        {
            try
            {
                using (SqlConnection sqlConnection = GetSqlConnection())
                {
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandText = "UPDATE CaisseTicket set Hash=@jir where Id=@Id";
                    sqlCommand.Parameters.Add(new SqlParameter("@jir", jir));
                    sqlCommand.Parameters.Add(new SqlParameter("@Id", ticketid));
                    return sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex2)
            {
                SimpleLog.Log(ex2);
                throw new Exception("error ocured in SaveTicketJir " + ex2.Message);
            }
        }

        public int SaveNotes(int ticketid, string notes)
        {
            try
            {
                using (SqlConnection sqlConnection = GetSqlConnection())
                {
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandText = "UPDATE CaisseTicket set notes=@notes where Id=@Id";
                    sqlCommand.Parameters.Add(new SqlParameter("@notes", notes));
                    sqlCommand.Parameters.Add(new SqlParameter("@Id", ticketid));
                    return sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                SimpleLog.Log(ex);
                throw new Exception("error ocured in SaveNotes " + ex.Message);
            }
        }

        public List<DataPerso> GetPerso()
        {
            List<DataPerso> allPerso = new List<DataPerso>();
            try
            {
                using (SqlConnection sqlConnection = GetSqlConnection())
                {
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandText = "SELECT Id, Code, NumeroSecu from perso";
                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {
                            DataPerso dataPerso = new DataPerso();
                            dataPerso.idPerso = (int)sqlDataReader["Id"];
                            dataPerso.codePerso = (string)sqlDataReader["Code"];
                            dataPerso.numeroSecu = (string)sqlDataReader["NumeroSecu"];
                            allPerso.Add(dataPerso);
                        }
                        sqlDataReader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                SimpleLog.Log(ex);
                throw new Exception("error ocured in GetPerso " + ex.Message);
            }
            return allPerso;
        }

        public bool CheckForWeirdTaxesWhenNotInVAT()
        {
            SqlConnection sqlConnection = new SqlConnection(AppLink.ConnectionString);
            sqlConnection.Open();
            using (sqlConnection)
            {
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = "SELECT COUNT(taux) AS NumberOfTaxes FROM SysTauxTva WHERE taux != 0";
                int num = 0;
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    while (sqlDataReader.Read())
                    {
                        num = (int)sqlDataReader["NumberOfTaxes"];
                    }
                }
                if (num == 0)
                {
                    return false;
                }
            }
            return true;
        }

        public void UpdatePerso(string cmdSQL)
        {
            try
            {
                using (SqlConnection sqlConnection = GetSqlConnection())
                {
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandText = cmdSQL;
                    sqlCommand.ExecuteNonQuery();
                }
            }
            catch(Exception ex)
            {
                SimpleLog.Log(ex);
                throw new Exception("error ocured in UpdatePerso " + ex.Message);
            }
        }

        public bool UseNoTva()
        {
            try
            {
                using (SqlConnection sqlConnection = GetSqlConnection())
                {
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandText = "UPDATE systauxtva set libelle=@libelle, taux=@taux, taux3=@taux3";
                    sqlCommand.Parameters.Add(new SqlParameter("@libelle", "0 %"));
                    sqlCommand.Parameters.Add(new SqlParameter("@taux", "0"));
                    sqlCommand.Parameters.Add(new SqlParameter("@taux3", "0"));
                    sqlCommand.ExecuteNonQuery();
                }
            }
            catch
            {
            }
            return false;
        }

        public void FlushBillsWithNoJir()
        {
            try
            {
                using (SqlConnection sqlConnection = GetSqlConnection())
                {
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandText = "delete caisseticketoutput";
                    sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                SimpleLog.Log(ex);
                throw new Exception("error ocured in UpdatePerso " + ex.Message);
            }
        }

        public List<DataTax> GetTaxes(int id)
        {
            List<DataTax> list = new List<DataTax>();
            list.AddRange(GetMainTaxes(id));
            return list;
        }

        private List<DataTax> GetMainTaxes(int id)
        {
            List<DataTax> list = new List<DataTax>();
            using (SqlConnection sqlConnection = GetSqlConnection())
            {
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = "\r\n\t\t\t\t\tSELECT PrixDeBase, IdTauxTVA, IdExecutant, MontantHT, PrixPaye, NumArticle, EtatLigTik, Taux\r\n\t\t\t\t\tFROM CaisseLigTicket \r\n\t\t\t\t\tJOIN systauxtva ON systauxtva.id = IdTauxTVA\r\n\t\t\t\t\tWHERE idcaisseticket = @id AND InfoMisc = 0\r\n\t\t\t\t";
                sqlCommand.Parameters.Add(new SqlParameter("@id", id));
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    while (sqlDataReader.Read())
                    {
                        DataTax dataTax = new DataTax();
                        dataTax.OperatorId = (int)sqlDataReader["IdExecutant"];
                        dataTax.TaxableAmount = ((decimal)(int)sqlDataReader["MontantHT"] / 100m).ToString();
                        dataTax.TaxFull = ((decimal)(int)sqlDataReader["PrixPaye"] / 100m).ToString();
                        dataTax.TaxId = (int)sqlDataReader["IdTauxTVA"];
                        dataTax.NumArticle = (int)sqlDataReader["NumArticle"];
                        string text = dataTax.TaxRate = ((decimal)(int)sqlDataReader["Taux"] / 100m).ToString("0.00");
                        if (dataTax.TaxRate == "0.00")
                        {
                            dataTax.TaxAmount = "0.00";
                        }
                        short num = (short)sqlDataReader["EtatLigTik"];
                        if (num != 12)
                        {
                            list.Add(dataTax);
                        }
                    }
                    sqlDataReader.Close();
                }
            }
            List<DataTax> relatedTaxes = GetRelatedTaxes(id);
            if (relatedTaxes.Count > 0)
            {
                list.AddRange(relatedTaxes);
            }
            return list;
        }

        private List<DataTax> GetRelatedTaxes(int id)
        {
            List<DataTax> list = new List<DataTax>();
            using (SqlConnection sqlConnection = GetSqlConnection())
            {
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = "\r\n\t\t\t\t\tSELECT PrixDeBase, IdTauxTVA, IdExecutant, MontantHT, PrixPaye, NumArticle, TypeLigTik, InfoMisc, Taux\r\n          FROM CaisseLigTicket\r\n\t\t\t\t\tLEFT JOIN systauxtva ON systauxtva.id = IdTauxTVA\r\n          WHERE idcaisseticket = @id AND InfoMisc != 0 AND IdTauxTVA != -1\r\n\t\t\t\t";
                sqlCommand.Parameters.Add(new SqlParameter("@id", id));
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    while (sqlDataReader.Read())
                    {
                        if ((int)sqlDataReader["InfoMisc"] >= 1)
                        {
                            list.AddRange(GetMainTaxes((int)sqlDataReader["NumArticle"]));
                        }
                        else
                        {
                            DataTax dataTax = new DataTax();
                            dataTax.OperatorId = (int)sqlDataReader["IdExecutant"];
                            dataTax.TaxableAmount = ((decimal)(int)sqlDataReader["MontantHT"] / 100m).ToString();
                            dataTax.TaxFull = ((decimal)(int)sqlDataReader["PrixPaye"] / 100m).ToString();
                            dataTax.TaxId = (int)sqlDataReader["IdTauxTVA"];
                            dataTax.NumArticle = (int)sqlDataReader["NumArticle"];
                            dataTax.TaxRate = ((decimal)(int)sqlDataReader["Taux"] / 100m).ToString("0.00");
                            if (dataTax.TaxRate == "0.00")
                            {
                                dataTax.TaxAmount = "0.00";
                            }
                            list.Add(dataTax);
                        }
                    }
                    sqlDataReader.Close();
                }
            }
            return list;
        }

        public List<DataNewTax> GetNewTaxes(int id)
        {
            List<DataNewTax> list = new List<DataNewTax>();
            List<int> comboBillId = GetComboBillId(id);
            comboBillId.Add(id);
            string str = string.Join(",", comboBillId.ToArray());
            using (SqlConnection sqlConnection = GetSqlConnection())
            {
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = "\r\n\t\t\t\t\t\tSELECT SUM(CaisseTicketTaxe.MontantTVA) AS MontantTVA, SUM(CaisseTicketTaxe.MontantTTC) AS MontantTTC, SysTauxTva.Taux, CaisseTicketTaxe.IdTauxTVA\r\n\t\t\t\t\t\tFROM CaisseTicketTaxe\r\n\t\t\t\t\t\tJOIN SysTauxTva ON SysTauxTva.id = CaisseTicketTaxe.IdTauxTVA\r\n\t\t\t\t\t\tWHERE CaisseTicketTaxe.IdCaisseTicket IN (" + str + ")\r\n\t\t\t\t\t\tGROUP BY SysTauxTva.Taux, CaisseTicketTaxe.IdTauxTVA\r\n\t\t\t\t\t\tORDER BY SysTauxTva.Taux\r\n\t\t\t\t\t";
                /* sqlCommand.CommandText = @"SELECT SUM(CaisseTicketTaxe.MontantTVA) AS MontantTVA, 
     SUM(CaisseTicketTaxe.MontantTTC) AS MontantTTC,
     SUM(CaisseLigTicket.MontantHT) as MontantHT,
     SysTauxTva.Taux, 
     CaisseTicketTaxe.IdTauxTVA
     FROM CaisseTicketTaxe
     JOIN SysTauxTva ON SysTauxTva.id = CaisseTicketTaxe.IdTauxTVA
     join CaisseLigTicket on CaisseLigTicket.IdTauxTVA = CaisseTicketTaxe.IdTauxTVA
     WHERE CaisseTicketTaxe.IdCaisseTicket IN("+ str + ") and CaisseLigTicket.IdCaisseTicket IN("+ str + ") GROUP BY SysTauxTva.Taux, CaisseTicketTaxe.IdTauxTVA ORDER BY SysTauxTva.Taux";
     */
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    while (sqlDataReader.Read())
                    {
                        DataNewTax dataNewTax = new DataNewTax();
                        dataNewTax.TaxId = sqlDataReader["IdTauxTVA"].ToString();
                        dataNewTax.TaxAmount = ((decimal)sqlDataReader["MontantTVA"] / 100m).ToString();
                        dataNewTax.TaxableAmount = ((decimal)sqlDataReader["MontantTTC"] / 100m - (decimal)sqlDataReader["MontantTVA"] / 100m).ToString();
                        dataNewTax.TaxRate = ((decimal)(int)sqlDataReader["Taux"] / 100m).ToString();
                        list.Add(dataNewTax);
                    }
                    sqlDataReader.Close();
                }
            }
            if (list.Count == 1)
            {
                using (SqlConnection sqlConnection2 = GetSqlConnection())
                {
                    SqlCommand sqlCommand2 = sqlConnection2.CreateCommand();
                    sqlCommand2.CommandText = "\r\n\t\t\t\t\t\tSELECT SUM(CaisseLigPaiement.Prix) AS BaseReduction\r\n\t\t\t\t\t\tFROM CaisseLigPaiement\r\n\t\t\t\t\t\tWHERE CaisseLigPaiement.IdCaisseTicket IN (" + str + ")\r\n\t\t\t\t\t\tAND CaisseLigPaiement.IdCaisseMoyPaiement IN (4, 6)\r\n\t\t\t\t\t\tGROUP BY CaisseLigPaiement.IdCaisseTicket\r\n\t\t\t\t\t";
                    using (SqlDataReader sqlDataReader2 = sqlCommand2.ExecuteReader())
                    {
                        while (sqlDataReader2.Read())
                        {
                            DataNewTax dataNewTax2 = list.First();
                            string value = ((decimal)sqlDataReader2["BaseReduction"] / 100m).ToString();
                            decimal d = Convert.ToDecimal(value);
                            decimal d2 = Convert.ToDecimal(dataNewTax2.TaxableAmount);
                            decimal num = d2 - d;
                            list.First().TaxableAmount = num.ToString();
                        }
                        sqlDataReader2.Close();
                    }
                }
            }
            return list;
        }

        public List<int> GetComboBillId(int mainBillId)
        {
            List<int> list = new List<int>();
            using (SqlConnection sqlConnection = GetSqlConnection())
            {
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = "\r\n\t\t\t\t\tSELECT CaisseTicket.id\r\n\t\t\t\t\tFROM CaisseTicket\r\n\t\t\t\t\tWHERE CaisseTicket.InfoSuiv = @id\r\n\t\t\t\t";
                sqlCommand.Parameters.Add(new SqlParameter("@id", mainBillId));
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    while (sqlDataReader.Read())
                    {
                        list.Add((int)sqlDataReader["id"]);
                    }
                    sqlDataReader.Close();
                }
            }
            return list;
        }

        public void Dispose()
        {
            if (_sqlConnection.State == ConnectionState.Open)
            {
                _sqlConnection.Close();
            }
        }

        public List<DataBill> GetTicket(int dticket)
        {
            List<DataBill> allBills = new List<DataBill>();
            DataSalon dataSalon = new DataSalon();
            try
            {
                using (SqlConnection sqlConnection = GetSqlConnection())
                {
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandText = "SELECT DateHeure, MontantHT, PrixFacture, code, id, Hash, notes, idsysmachine, typetik from caisseticket where id="+ dticket;
                    //sqlCommand.Parameters.Add(new SqlParameter("@Date", dticket));
                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {
                            DataBill dataBill = new DataBill();
                            dataBill.VATNumber_Salon_Bill = "xxxxxxxx";
                            dataBill.TaxPayer_Bill = true;
                            dataBill.BillDate_Bill = AppLink.DateFromLong((int)sqlDataReader["DateHeure"]);
                            dataBill.SequenceMark_Bill = OznakaSlijednostiType.P;
                            dataBill.PremiseMark_Bill = AppLink.PremiseMark;
                            dataBill.BillingDeviceMark_Bill = AppLink.BillingDeviceMark;
                            //dataBill.BillNumberMark_Bill = Convert.ToString(sqlDataReader["Code"]);
                            dataBill.TypeTik = (short)sqlDataReader["typetik"];
                            //dataBill.CashierVATNumber_Bill = ((sqlDataReader["OIBPerso"] == DBNull.Value) ? "OIBPERSOERROR" : Convert.ToString(sqlDataReader["OIBPerso"]));
                            //dataBill.CountLigPay_Bill = (int)sqlDataReader["CountLigPay"];
                            dataBill.IdTicket = (int)sqlDataReader["Id"];
                            dataBill.HashStatus = ((sqlDataReader["Hash"] == DBNull.Value) ? "0" : ((string)sqlDataReader["Hash"]));
                            dataBill.Notes = ((sqlDataReader["notes"] == DBNull.Value) ? "" : ((string)sqlDataReader["notes"]));
     
                             allBills.Add(dataBill);
                            
                        }
                        sqlDataReader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                SimpleLog.Log(ex);
                throw new Exception("error ocured in dal get bill method " + ex.Message);
            }
            return allBills;
        }

        public int GetPersoByOib(string vATNumber_Salon)
        {
            try
            {
                using (SqlConnection sqlConnection = GetSqlConnection())
                {
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandText = "SELECT Id from perso where NumeroSecu='" + vATNumber_Salon+"'";
                    var result = sqlCommand.ExecuteScalar();

                    if (result == null)
                        return -1;
                    
                    return (int)result;


                }
            }
            catch (Exception ex)
            {
                SimpleLog.Log(ex);
                return -1;
            }
        }

        public int UpdateTicketWithOib(int idTicket, int idCashier)
        {
            try
            {
                using (SqlConnection sqlConnection = GetSqlConnection())
                {
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandText = "UPDATE CaisseTicket set idpersoencaiss=@idCashier where Id=@Id";
                    sqlCommand.Parameters.Add(new SqlParameter("@idCashier", idCashier));
                    sqlCommand.Parameters.Add(new SqlParameter("@Id", idTicket));
                    return sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex2)
            {
                SimpleLog.Log(ex2);
                throw new Exception("error ocured in SaveTicketJir " + ex2.Message);
            }
        }
    }
}
