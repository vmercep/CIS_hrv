﻿using DataObjects;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

public static class MerlinData {
  private static SqlConnection GetSqlConnection () {
    SqlConnection sqlConnection = new SqlConnection(AppLink.ConnectionString);
    sqlConnection.Open();
    return sqlConnection;
  }

  public static bool GetBill (List<DataBill> allBills, string OIB, bool vatIsActive) {
    DataSalon dataSalon = new DataSalon();
    dataSalon.DateIsActive = Convert.ToDateTime(AppLink.DateIsActive);
    try {
      using (SqlConnection sqlConnection = GetSqlConnection()) {
        SqlCommand sqlCommand = sqlConnection.CreateCommand();
        sqlCommand.CommandText = "SELECT DateHeure, MontantHT, PrixFacture, code, id, Hash, idsysmachine, typetik, notes, \r\n                    (select top 1 taux from systauxtva where flagarchive=0 order by taux desc) TauxTva, \r\n                    (select count(idcaisseticket) from caisseligpaiement where idcaisseticket=caisseticket.id) CountLigPay, \r\n                    (select numerosecu from perso where id=caisseticket.idpersoencaiss) OIBPerso from caisseticket\r\n                    where (Hash like @HashE or Hash like @Hash0 or Hash like @Hash1 or Hash is null or datalength(Hash) > 36) and typetik in(1,2) and id>=@Date";
        sqlCommand.Parameters.Add(new SqlParameter("@HashE", "ERROR_%"));
        sqlCommand.Parameters.Add(new SqlParameter("@Hash0", "0"));
        sqlCommand.Parameters.Add(new SqlParameter("@Hash1", ""));
        sqlCommand.Parameters.Add(new SqlParameter("@Date", AppLink.LongFromDate(dataSalon.DateIsActive)));
        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader()) {
          while (sqlDataReader.Read()) {
            DataBill dataBill = new DataBill();
            dataBill.VATNumber_Salon_Bill = OIB;
            dataBill.TaxPayer_Bill = vatIsActive;
            dataBill.BillDate_Bill = AppLink.DateFromLong((int) sqlDataReader["DateHeure"]);
            dataBill.SequenceMark_Bill = OznakaSlijednostiType.P;
            dataBill.PremiseMark_Bill = AppLink.PremiseMark;
            dataBill.BillingDeviceMark_Bill = AppLink.BillingDeviceMark;
            dataBill.BillNumberMark_Bill = Convert.ToString(sqlDataReader["Code"]);
            if (vatIsActive) {
              dataBill.VATTaxRate_Bill = ((int) sqlDataReader["TauxTVA"] / 100).ToString("0.00");
              dataBill.VATBase_Bill = ((decimal) sqlDataReader["MontantHT"] / 100m).ToString("0.00");
            } else {
              dataBill.VATTaxRate_Bill = "0.00";
              dataBill.VATBase_Bill = "0.00";
            }
            dataBill.TypeTik = (short) sqlDataReader["typetik"];
            dataBill.CashierVATNumber_Bill = ((sqlDataReader["OIBPerso"] == DBNull.Value) ? "OIBPERSOERROR" : Convert.ToString(sqlDataReader["OIBPerso"]));
            dataBill.CountLigPay_Bill = (int) sqlDataReader["CountLigPay"];
            dataBill.IdTicket = (int) sqlDataReader["Id"];
            dataBill.HashStatus = ((sqlDataReader["Hash"] == DBNull.Value) ? "0" : ((string) sqlDataReader["Hash"]));
            dataBill.Notes = ((sqlDataReader["notes"] == DBNull.Value) ? "" : ((string) sqlDataReader["notes"]));
            if (dataBill.HashStatus.Length < 36 || dataBill.HashStatus.Length > 36) {
              allBills.Add(dataBill);
            }
          }
          sqlDataReader.Close();
        }
      }
    } catch (Exception ex) {
      SimpleLog.Log(ex);
      return false;
    }
    return true;
  }

  public static bool GetBillFollow (DataBill Bill) {
    //int num = 0;
    List<DataPay> list = new List<DataPay>();
    decimal num2 = default(decimal);
    int num3 = 0;
    int num4 = 0;
    try {
      using (SqlConnection sqlConnection = GetSqlConnection()) {
        SqlCommand sqlCommand = sqlConnection.CreateCommand();
        sqlCommand.CommandText = "SELECT IdCaisseTicket, Id, (select idcaissetypemoypaiement from caissemoypaiement where \r\n\t\t\t\t\tid=caisseligpaiement.idCaisseMoyPaiement) TYPEPAI,IdCaisseMoyPaiement, prix, InfoMisc3 \r\n                    from CaisseLigPaiement where IdCaisseTicket=@IdCaisseTicket";
        sqlCommand.Parameters.Add(new SqlParameter("@IdCaisseTicket", Bill.IdTicket));
        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader()) {
          while (sqlDataReader.Read()) {
            DataPay dataPay = new DataPay();
            dataPay.IdCtick = (int) sqlDataReader["IdCaisseTicket"];
            Bill.BillDate_Bill = AppLink.DateFromLong((int) sqlDataReader["Id"]);
            dataPay.TypePay = (int) sqlDataReader["TYPEPAI"];
            dataPay.IdCmoyPay = (short) sqlDataReader["IdCaisseMoyPaiement"];
            dataPay.Prix = (decimal) sqlDataReader["prix"];
            dataPay.Tip = (((string) sqlDataReader["InfoMisc3"] == "TIP") ? true : false);
            switch (sqlDataReader.GetInt32(sqlDataReader.GetOrdinal("TYPEPAI"))) {
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
    } catch (Exception ex) {
      SimpleLog.Log(ex);
      Console.WriteLine(ex.Message);
      return false;
    }
    try {
      if (list.Count == 0) {
        Bill.PaymentMethod_Bill = NacinPlacanjaType.O;
      } else {
        foreach (DataPay item in list) {
          switch (item.MoyPayFinal) {
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
          if (num3 == 1 && item.MoyPayFinal != NacinPlacanjaType.O) {
            Bill.PaymentMethod_Bill = item.MoyPayFinal;
          }
          if (num3 > 1 && item.MoyPayFinal != NacinPlacanjaType.O) {
            Bill.PaymentMethod_Bill = NacinPlacanjaType.O;
          }
          if (num4 >= 1 && num3 == 0) {
            Bill.PaymentMethod_Bill = NacinPlacanjaType.O;
          }
          if (item.TypePay == 9) {
            item.Prix *= decimal.MinusOne;
            num2 += item.Prix;
          }
        }
      }
      Bill.TotalAmount_Bill = (num2 / 100m).ToString("0.00");
      switch (Bill.TypeTik) {
        case 1:
          if (Convert.ToDecimal(Bill.VATBase_Bill) == decimal.Zero || num2 == decimal.Zero) {
            Bill.VATAmount_Bill = 0.ToString("0.00");
          } else {
            Bill.VATAmount_Bill = (Convert.ToDecimal(num2 / 100m) - Convert.ToDecimal(Bill.VATBase_Bill)).ToString("0.00");
          }
          break;
        case 2: {
            decimal d = Convert.ToDecimal(num2);
            decimal d2 = Convert.ToDecimal(Bill.VATTaxRate_Bill);
            decimal d3 = (d - d / (decimal.One + d2 / 100m)) / 100m;
            d3 = -d3;
            Bill.VATAmount_Bill = d3.ToString("-0.00");
            Bill.VATBase_Bill = ((num2 + d3 * 100m) / 100m).ToString("0.00");
            if (!Bill.TaxPayer_Bill) {
              Bill.VATAmount_Bill = "0.00";
              Bill.VATBase_Bill = "0.00";
            }
            break;
          }
      }
      return true;
    } catch (Exception ex2) {
      SimpleLog.Log(ex2);
      Console.WriteLine(ex2.Message);
      return false;
    }
  }

  public static bool SaveTicketJir (int ticketid, string jir) {
    try {
      using (SqlConnection sqlConnection = GetSqlConnection()) {
        SqlCommand sqlCommand = sqlConnection.CreateCommand();
        sqlCommand.CommandText = "UPDATE CaisseTicket set Hash=@jir where Id=@Id";
        sqlCommand.Parameters.Add(new SqlParameter("@jir", jir));
        sqlCommand.Parameters.Add(new SqlParameter("@Id", ticketid));
        sqlCommand.ExecuteNonQuery();
      }
    } catch {
    }
    return false;
  }

  public static bool SaveNotes (int ticketid, string notes) {
    try {
      using (SqlConnection sqlConnection = GetSqlConnection()) {
        SqlCommand sqlCommand = sqlConnection.CreateCommand();
        sqlCommand.CommandText = "UPDATE CaisseTicket set notes=@notes where Id=@Id";
        sqlCommand.Parameters.Add(new SqlParameter("@notes", notes));
        sqlCommand.Parameters.Add(new SqlParameter("@Id", ticketid));
        sqlCommand.ExecuteNonQuery();
      }
    } catch {
    }
    return false;
  }

  public static bool GetPerso (List<DataPerso> allPerso) {
    try {
      using (SqlConnection sqlConnection = GetSqlConnection()) {
        SqlCommand sqlCommand = sqlConnection.CreateCommand();
        sqlCommand.CommandText = "SELECT Id, Code, NumeroSecu from perso";
        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader()) {
          while (sqlDataReader.Read()) {
            DataPerso dataPerso = new DataPerso();
            dataPerso.idPerso = (int) sqlDataReader["Id"];
            dataPerso.codePerso = (string) sqlDataReader["Code"];
            dataPerso.numeroSecu = (string) sqlDataReader["NumeroSecu"];
            allPerso.Add(dataPerso);
          }
          sqlDataReader.Close();
        }
      }
    } catch (Exception ex) {
      SimpleLog.Log(ex);
      Console.WriteLine(ex.Message);
      return false;
    }
    return true;
  }

  public static bool UpdatePerso (string cmdSQL) {
    try {
      using (SqlConnection sqlConnection = GetSqlConnection()) {
        SqlCommand sqlCommand = sqlConnection.CreateCommand();
        sqlCommand.CommandText = cmdSQL;
        sqlCommand.ExecuteNonQuery();
      }
    } catch {
    }
    return false;
  }

  public static bool UseNoTva () {
    try {
      using (SqlConnection sqlConnection = GetSqlConnection()) {
        SqlCommand sqlCommand = sqlConnection.CreateCommand();
        sqlCommand.CommandText = "UPDATE systauxtva set libelle=@libelle, taux=@taux, taux3=@taux3";
        sqlCommand.Parameters.Add(new SqlParameter("@libelle", "0 %"));
        sqlCommand.Parameters.Add(new SqlParameter("@taux", "0"));
        sqlCommand.Parameters.Add(new SqlParameter("@taux3", "0"));
        sqlCommand.ExecuteNonQuery();
      }
    } catch {
    }
    return false;
  }

  public static bool FlushBillsWithNoJir () {
    try {
      using (SqlConnection sqlConnection = GetSqlConnection()) {
        SqlCommand sqlCommand = sqlConnection.CreateCommand();
        sqlCommand.CommandText = "delete caisseticketoutput";
        sqlCommand.ExecuteNonQuery();
        return true;
      }
    } catch {
    }
    return false;
  }

  public static List<DataTax> GetTaxes (int id) {
    List<DataTax> list = new List<DataTax>();
    list.AddRange(GetMainTaxes(id));
    return list;
  }

  private static List<DataTax> GetMainTaxes (int id) {
    List<DataTax> list = new List<DataTax>();
    using (SqlConnection sqlConnection = GetSqlConnection()) {
      SqlCommand sqlCommand = sqlConnection.CreateCommand();
      sqlCommand.CommandText = "\r\n\t\t\t\t\tSELECT PrixDeBase, IdTauxTVA, IdExecutant, MontantHT, PrixPaye, NumArticle, EtatLigTik, Taux\r\n\t\t\t\t\tFROM CaisseLigTicket \r\n\t\t\t\t\tJOIN systauxtva ON systauxtva.id = IdTauxTVA\r\n\t\t\t\t\tWHERE idcaisseticket = @id AND InfoMisc = 0\r\n\t\t\t\t";
      sqlCommand.Parameters.Add(new SqlParameter("@id", id));
      using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader()) {
        while (sqlDataReader.Read()) {
          DataTax dataTax = new DataTax();
          dataTax.OperatorId = (int) sqlDataReader["IdExecutant"];
          dataTax.TaxableAmount = ((decimal) (int) sqlDataReader["MontantHT"] / 100m).ToString();
          dataTax.TaxFull = ((decimal) (int) sqlDataReader["PrixPaye"] / 100m).ToString();
          dataTax.TaxId = (int) sqlDataReader["IdTauxTVA"];
          dataTax.NumArticle = (int) sqlDataReader["NumArticle"];
          string text = dataTax.TaxRate = ((decimal) (int) sqlDataReader["Taux"] / 100m).ToString("0.00");
          if (dataTax.TaxRate == "0.00") {
            dataTax.TaxAmount = "0.00";
          }
          short num = (short) sqlDataReader["EtatLigTik"];
          if (num != 12) {
            list.Add(dataTax);
          }
        }
        sqlDataReader.Close();
      }
    }
    List<DataTax> relatedTaxes = GetRelatedTaxes(id);
    if (relatedTaxes.Count > 0) {
      list.AddRange(relatedTaxes);
    }
    return list;
  }

  private static List<DataTax> GetRelatedTaxes (int id) {
    List<DataTax> list = new List<DataTax>();
    using (SqlConnection sqlConnection = GetSqlConnection()) {
      SqlCommand sqlCommand = sqlConnection.CreateCommand();
      sqlCommand.CommandText = "\r\n\t\t\t\t\tSELECT PrixDeBase, IdTauxTVA, IdExecutant, MontantHT, PrixPaye, NumArticle, TypeLigTik, InfoMisc, Taux\r\n          FROM CaisseLigTicket\r\n\t\t\t\t\tLEFT JOIN systauxtva ON systauxtva.id = IdTauxTVA\r\n          WHERE idcaisseticket = @id AND InfoMisc != 0 AND IdTauxTVA != -1\r\n\t\t\t\t";
      sqlCommand.Parameters.Add(new SqlParameter("@id", id));
      using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader()) {
        while (sqlDataReader.Read()) {
          if ((int) sqlDataReader["InfoMisc"] >= 1) {
            list.AddRange(GetMainTaxes((int) sqlDataReader["NumArticle"]));
          } else {
            DataTax dataTax = new DataTax();
            dataTax.OperatorId = (int) sqlDataReader["IdExecutant"];
            dataTax.TaxableAmount = ((decimal) (int) sqlDataReader["MontantHT"] / 100m).ToString();
            dataTax.TaxFull = ((decimal) (int) sqlDataReader["PrixPaye"] / 100m).ToString();
            dataTax.TaxId = (int) sqlDataReader["IdTauxTVA"];
            dataTax.NumArticle = (int) sqlDataReader["NumArticle"];
            dataTax.TaxRate = ((decimal) (int) sqlDataReader["Taux"] / 100m).ToString("0.00");
            if (dataTax.TaxRate == "0.00") {
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

  public static List<DataNewTax> GetNewTaxes (int id) {
    List<DataNewTax> list = new List<DataNewTax>();
    List<int> comboBillId = GetComboBillId(id);
    comboBillId.Add(id);
    string str = string.Join(",", comboBillId.ToArray());
    using (SqlConnection sqlConnection = GetSqlConnection()) {
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
      using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader()) {
        while (sqlDataReader.Read()) {
          DataNewTax dataNewTax = new DataNewTax();
          dataNewTax.TaxId = sqlDataReader["IdTauxTVA"].ToString();
          dataNewTax.TaxAmount = ((decimal) sqlDataReader["MontantTVA"] / 100m).ToString();
          dataNewTax.TaxableAmount = ((decimal) sqlDataReader["MontantTTC"] / 100m - (decimal) sqlDataReader["MontantTVA"] / 100m).ToString();
          dataNewTax.TaxRate = ((decimal) (int) sqlDataReader["Taux"] / 100m).ToString();
          list.Add(dataNewTax);
        }
        sqlDataReader.Close();
      }
    }
    if (list.Count == 1) {
      using (SqlConnection sqlConnection2 = GetSqlConnection()) {
        SqlCommand sqlCommand2 = sqlConnection2.CreateCommand();
        sqlCommand2.CommandText = "\r\n\t\t\t\t\t\tSELECT SUM(CaisseLigPaiement.Prix) AS BaseReduction\r\n\t\t\t\t\t\tFROM CaisseLigPaiement\r\n\t\t\t\t\t\tWHERE CaisseLigPaiement.IdCaisseTicket IN (" + str + ")\r\n\t\t\t\t\t\tAND CaisseLigPaiement.IdCaisseMoyPaiement IN (4, 6)\r\n\t\t\t\t\t\tGROUP BY CaisseLigPaiement.IdCaisseTicket\r\n\t\t\t\t\t";
        using (SqlDataReader sqlDataReader2 = sqlCommand2.ExecuteReader()) {
          while (sqlDataReader2.Read()) {
            DataNewTax dataNewTax2 = list.First();
            string value = ((decimal) sqlDataReader2["BaseReduction"] / 100m).ToString();
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

  public static List<int> GetComboBillId (int mainBillId) {
    List<int> list = new List<int>();
    using (SqlConnection sqlConnection = GetSqlConnection()) {
      SqlCommand sqlCommand = sqlConnection.CreateCommand();
      sqlCommand.CommandText = "\r\n\t\t\t\t\tSELECT CaisseTicket.id\r\n\t\t\t\t\tFROM CaisseTicket\r\n\t\t\t\t\tWHERE CaisseTicket.InfoSuiv = @id\r\n\t\t\t\t";
      sqlCommand.Parameters.Add(new SqlParameter("@id", mainBillId));
      using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader()) {
        while (sqlDataReader.Read()) {
          list.Add((int) sqlDataReader["id"]);
        }
        sqlDataReader.Close();
      }
    }
    return list;
  }

  public static bool checkIfNewTaxes () {
    using (SqlConnection sqlConnection = GetSqlConnection()) {
      SqlCommand sqlCommand = sqlConnection.CreateCommand();
      sqlCommand.CommandText = "\r\n\t\t\t\t\tselect case when exists((select * from information_schema.tables where table_name = 'CaisseTicketTaxe')) then 1 else 0 end\r\n\t\t\t\t";
      return (int) sqlCommand.ExecuteScalar() == 1;
    }
  }
}
