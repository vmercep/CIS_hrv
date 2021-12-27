using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{

    public static class Log
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void WriteLog(NumLog numLog, int IDticket, string extendMess, Dictionary<string, string> placeholders,string ErrorCode, string ErrorMessage)
        {

                switch (numLog)
                {
                    case NumLog.AppLaunch:
                        log.Debug("------------------------- " + Translations.Translate("POČETAK APLIKACIJE") + " -------------------------");
                        break;
                    case NumLog.MissingSetting:
                        log.Debug(Translations.Translate("Nedostaju postavke"));
                        break;
                    case NumLog.SettingsOk:
                        log.Debug(Translations.Translate("Postavke su u redu"));
                        break;
                    case NumLog.ConnectionCISFailed:
                        log.Debug(Translations.Translate("Povezivanje s CIS servisom nije uspijelo"));
                        break;
                    case NumLog.ConnectionCISOk:
                        log.Debug(Translations.Translate("Uspiješno povezivanje s CIS servisom"));
                        break;
                    case NumLog.PbBillProcSt1:
                        log.Debug(Translations.Translate("Problem prilikom obrade računa (korak 1)"));
                        break;
                    case NumLog.BillProcSt1ok:
                        log.Debug(Translations.Translate("Uspiješna obrada računa (korak 1)"));
                        break;
                    case NumLog.NoBill:
                        log.Debug("-- " + Translations.Translate("Ne postoji račun za obradu. ZATVRANJE APLIKACIJE") + " --");
                        break;
                    case NumLog.StartBillProcSt2:
                        log.Debug(Translations.Translate("Započinje obrada računa (korak 2)"));
                        break;
                    case NumLog.PbBillProcSt2:
                        log.Debug(Translations.Translate("Problem prilikom obrade računa (korak 2)") + " :" + IDticket);
                        break;
                    case NumLog.MissingOIBEmp:
                        log.Debug(Translations.Translate("Nedostaje OIB zaposlenika"));
                        break;
                    case NumLog.SendBill:
                        log.Debug(">" + Translations.Translate("Šaljem račun") + ": " + IDticket);
                        break;
                    case NumLog.SendBillOK:
                        log.Debug(">>>" + Translations.Translate("Uspiješno slanje"));
                        break;
                    case NumLog.ErrorHttp:
                        log.Debug(extendMess + " - " + Translations.Translate("Greška prilikom slanja računa"));
                        break;
                    case NumLog.EmptyXMLResponse:
                        log.Debug(Translations.Translate("XML odgovor je prazan"));
                        break;
                    case NumLog.XMLerror:
                        log.Debug(extendMess + " " + Translations.Translate("na računu") + " " + IDticket);
                        break;
                    case NumLog.JirNodeNotFount:
                        log.Debug(Translations.Translate("Na odgovoru nije pronađen JIR"));
                        break;
                    case NumLog.NodeJirIsNull:
                        log.Debug(Translations.Translate("Čvor JIR na odgovoru nema sadržaja"));
                        break;
                    case NumLog.JirIsNull:
                        log.Debug(Translations.Translate("Element JIR na odgovoru nema sadržaja"));
                        break;
                    case NumLog.NextBill:
                        log.Debug("-" + Translations.Translate("Slijedeći račun") + "-");
                        break;
                    case NumLog.StartSearchJir:
                        log.Debug(Translations.Translate("Započinje traženje JIR-a"));
                        break;
                    case NumLog.StartSearchError:
                        log.Debug(Translations.Translate("Započinje traženje greške"));
                        break;
                    case NumLog.s001:
                        log.Debug(Translations.Translate("Greška") + " s001");
                        break;
                    case NumLog.s002:
                        log.Debug(Translations.Translate("Greška") + " s002");
                        break;
                    case NumLog.s003:
                        log.Debug(Translations.Translate("Greška") + " s003");
                        break;
                    case NumLog.s004:
                        log.Debug(Translations.Translate("Greška") + " s004");
                        break;
                    case NumLog.s005:
                        log.Debug(Translations.Translate("Greška") + " s005");
                        break;
                    case NumLog.s006:
                        log.Debug(Translations.Translate("Greška") + " s006");
                        break;
                    case NumLog.sERR:
                        log.Debug(Translations.Translate("Greška") + " sERR");
                        break;
                    case NumLog.BillUpdateOK:
                        placeholders = new Dictionary<string, string>();
                        placeholders["{{%receipt_number%}}"] = IDticket.ToString();
                        placeholders["{{%jir_number%}}"] = extendMess;
                        log.Debug(Translations.Translate("Račun s brojem {{%receipt_number%}} uspiješno nadopunjen s {{%jir_number%}}", placeholders));
                        break;
                    case NumLog.ErrorHttp2:
                        log.Debug(extendMess + " " + Translations.Translate("Greška tokom slanja podataka o poslovnom prostoru"));
                        break;
                    case NumLog.SendBP:
                        log.Debug(">" + Translations.Translate("Slanje podataka o poslovnom prostoru"));
                        break;
                    case NumLog.CfgMissing:
                        log.Debug(">" + Translations.Translate("Nedostaje konfiguracijska datoteka"));
                        break;
                    case NumLog.SendingTest:
                        log.Debug(Translations.Translate("Slanje testnog računa s brojem: ") + IDticket);
                        break;
                    case NumLog.TestFailed:
                        log.Debug(Translations.Translate("Slanje testnog računa vratilo je grešku s brojem: ") + ErrorCode + " - " + ErrorMessage);
                        break;
                    case NumLog.TestSuccess:
                        log.Debug(Translations.Translate("Slanje testnog računa je bilo uspiješno"));
                        break;
                    case NumLog.WeirdTaxRatesWhenNotInVAT:
                        log.Debug(Translations.Translate("Označena je postavka da je korisnik izvan sustava PDV-a no postoje uneseni porezne stope koje su različite od 0%"));
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
