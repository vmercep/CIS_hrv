using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public static class Log
    {
        public static void WriteLog(NumLog numLog, int IDticket, string extendMess, Dictionary<string, string> placeholders,string ErrorCode, string ErrorMessage)
        {

                switch (numLog)
                {
                    case NumLog.AppLaunch:
                        LogFile.LogToFile("------------------------- " + Translations.Translate("POČETAK APLIKACIJE") + " -------------------------");
                        break;
                    case NumLog.MissingSetting:
                        LogFile.LogToFile(Translations.Translate("Nedostaju postavke"));
                        break;
                    case NumLog.SettingsOk:
                        LogFile.LogToFile(Translations.Translate("Postavke su u redu"));
                        break;
                    case NumLog.ConnectionCISFailed:
                        LogFile.LogToFile(Translations.Translate("Povezivanje s CIS servisom nije uspijelo"));
                        break;
                    case NumLog.ConnectionCISOk:
                        LogFile.LogToFile(Translations.Translate("Uspiješno povezivanje s CIS servisom"));
                        break;
                    case NumLog.PbBillProcSt1:
                        LogFile.LogToFile(Translations.Translate("Problem prilikom obrade računa (korak 1)"));
                        break;
                    case NumLog.BillProcSt1ok:
                        LogFile.LogToFile(Translations.Translate("Uspiješna obrada računa (korak 1)"));
                        break;
                    case NumLog.NoBill:
                        LogFile.LogToFile("-- " + Translations.Translate("Ne postoji račun za obradu. ZATVRANJE APLIKACIJE") + " --");
                        break;
                    case NumLog.StartBillProcSt2:
                        LogFile.LogToFile(Translations.Translate("Započinje obrada računa (korak 2)"));
                        break;
                    case NumLog.PbBillProcSt2:
                        LogFile.LogToFile(Translations.Translate("Problem prilikom obrade računa (korak 2)") + " :" + IDticket);
                        break;
                    case NumLog.MissingOIBEmp:
                        LogFile.LogToFile(Translations.Translate("Nedostaje OIB zaposlenika"));
                        break;
                    case NumLog.SendBill:
                        LogFile.LogToFile(">" + Translations.Translate("Šaljem račun") + ": " + IDticket);
                        break;
                    case NumLog.SendBillOK:
                        LogFile.LogToFile(">>>" + Translations.Translate("Uspiješno slanje"));
                        break;
                    case NumLog.ErrorHttp:
                        LogFile.LogToFile(extendMess + " - " + Translations.Translate("Greška prilikom slanja računa"));
                        break;
                    case NumLog.EmptyXMLResponse:
                        LogFile.LogToFile(Translations.Translate("XML odgovor je prazan"));
                        break;
                    case NumLog.XMLerror:
                        LogFile.LogToFile(extendMess + " " + Translations.Translate("na računu") + " " + IDticket);
                        break;
                    case NumLog.JirNodeNotFount:
                        LogFile.LogToFile(Translations.Translate("Na odgovoru nije pronađen JIR"));
                        break;
                    case NumLog.NodeJirIsNull:
                        LogFile.LogToFile(Translations.Translate("Čvor JIR na odgovoru nema sadržaja"));
                        break;
                    case NumLog.JirIsNull:
                        LogFile.LogToFile(Translations.Translate("Element JIR na odgovoru nema sadržaja"));
                        break;
                    case NumLog.NextBill:
                        LogFile.LogToFile("-" + Translations.Translate("Slijedeći račun") + "-");
                        break;
                    case NumLog.StartSearchJir:
                        LogFile.LogToFile(Translations.Translate("Započinje traženje JIR-a"));
                        break;
                    case NumLog.StartSearchError:
                        LogFile.LogToFile(Translations.Translate("Započinje traženje greške"));
                        break;
                    case NumLog.s001:
                        LogFile.LogToFile(Translations.Translate("Greška") + " s001");
                        break;
                    case NumLog.s002:
                        LogFile.LogToFile(Translations.Translate("Greška") + " s002");
                        break;
                    case NumLog.s003:
                        LogFile.LogToFile(Translations.Translate("Greška") + " s003");
                        break;
                    case NumLog.s004:
                        LogFile.LogToFile(Translations.Translate("Greška") + " s004");
                        break;
                    case NumLog.s005:
                        LogFile.LogToFile(Translations.Translate("Greška") + " s005");
                        break;
                    case NumLog.s006:
                        LogFile.LogToFile(Translations.Translate("Greška") + " s006");
                        break;
                    case NumLog.sERR:
                        LogFile.LogToFile(Translations.Translate("Greška") + " sERR");
                        break;
                    case NumLog.BillUpdateOK:
                        placeholders = new Dictionary<string, string>();
                        placeholders["{{%receipt_number%}}"] = IDticket.ToString();
                        placeholders["{{%jir_number%}}"] = extendMess;
                        LogFile.LogToFile(Translations.Translate("Račun s brojem {{%receipt_number%}} uspiješno nadopunjen s {{%jir_number%}}", placeholders));
                        break;
                    case NumLog.ErrorHttp2:
                        LogFile.LogToFile(extendMess + " " + Translations.Translate("Greška tokom slanja podataka o poslovnom prostoru"));
                        break;
                    case NumLog.SendBP:
                        LogFile.LogToFile(">" + Translations.Translate("Slanje podataka o poslovnom prostoru"));
                        break;
                    case NumLog.CfgMissing:
                        LogFile.LogToFile(">" + Translations.Translate("Nedostaje konfiguracijska datoteka"));
                        break;
                    case NumLog.SendingTest:
                        LogFile.LogToFile(Translations.Translate("Slanje testnog računa s brojem: ") + IDticket);
                        break;
                    case NumLog.TestFailed:
                        LogFile.LogToFile(Translations.Translate("Slanje testnog računa vratilo je grešku s brojem: ") + ErrorCode + " - " + ErrorMessage);
                        break;
                    case NumLog.TestSuccess:
                        LogFile.LogToFile(Translations.Translate("Slanje testnog računa je bilo uspiješno"));
                        break;
                    case NumLog.WeirdTaxRatesWhenNotInVAT:
                        LogFile.LogToFile(Translations.Translate("Označena je postavka da je korisnik izvan sustava PDV-a no postoje uneseni porezne stope koje su različite od 0%"));
                        break;
                }
            }
        

    }
    
    public enum NumLog
    {
        None,
        AppLaunch,
        MissingSetting,
        SettingsOk,
        ConnectionCISFailed,
        ConnectionCISOk,
        PbBillProcSt1,
        BillProcSt1ok,
        NoBill,
        StartBillProcSt2,
        PbBillProcSt2,
        MissingOIBEmp,
        SendBill,
        SendBillOK,
        ErrorHttp,
        EmptyXMLResponse,
        XMLerror,
        JirNodeNotFount,
        NodeJirIsNull,
        JirIsNull,
        NextBill,
        StartSearchJir,
        StartSearchError,
        s001,
        s002,
        s003,
        s004,
        s005,
        s006,
        sERR,
        BillUpdateOK,
        ErrorHttp2,
        SendBP,
        CfgMissing,
        SendingTest,
        TestSuccess,
        TestFailed,
        WeirdTaxRatesWhenNotInVAT
    }
}
