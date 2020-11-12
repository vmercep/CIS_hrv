// MainForm
using _385_fisk.Exceptions;
using AutoUpdaterDotNET;
using CisBl;
using CisDal;
using DataObjects;
using Helper;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Management;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Xml;

public class MainForm : Form
{


    private readonly Stopwatch stopWatch = new Stopwatch();

    private DataSalon DataSalonToSend = new DataSalon();

    private bool vatActif = true;

    private Dictionary<string, string> placeholders = new Dictionary<string, string>();

    private string ErrorCode = "";

    private string ErrorMessage = "";

    private string lastCheck = _385_fisk.Properties.Settings.Default.LastCheck;

    private Dictionary<string, int> numberOfFailedAttempts = new Dictionary<string, int>();

    private JavaScriptSerializer serializer = new JavaScriptSerializer();

    private DateTime now = DateTime.Now;

    public static string CertificateName;

    private IContainer components = null;

    private PictureBox pictureBox2;

    private Label lblFiskalisation;

    private GroupBox grpInfo;

    private Label lblServiceStatus;

    private Label lblExecutionTime;

    private ProgressBar progressBar1;

    private Label lblInfo;

    private PictureBox pictureBox3;

    private TextBox txtResponse;

    private string errorMessage;

    public MainForm()
    {
        AutoUpdater.DownloadPath = Environment.CurrentDirectory;
        InitializeComponent();
        if (_385_fisk.Properties.Settings.Default.TestBills.Length > 0)
        {
            numberOfFailedAttempts = serializer.Deserialize<Dictionary<string, int>>(_385_fisk.Properties.Settings.Default.TestBills);
        }
        TranslateContent();
    }

    private void TranslateContent()
    {
        Text = Translations.Translate(Text);
        grpInfo.Text = Translations.Translate(grpInfo.Text);
        lblFiskalisation.Text = Translations.Translate(lblFiskalisation.Text);
        lblExecutionTime.Text = Translations.Translate(lblExecutionTime.Text);
    }

    private void ErrorMessVerifData(string myMessage)
    {
        MessageAlert(Translations.Translate(myMessage), Translations.Translate("Greška"));
        Log.WriteLog(NumLog.MissingSetting, 0, "", placeholders, ErrorCode, ErrorMessage);
    }

    private void cis_SoapMessageSent(object sender, EventArgs e)
    {
        stopWatch.Stop();
        string text = Translations.Translate("Vrijeme izvršenja :") + string.Format("{0} s", stopWatch.Elapsed.TotalSeconds.ToString("0.00"));
        lblExecutionTime.Text = text;
        Application.DoEvents();
        stopWatch.Reset();
        Cursor.Current = Cursors.Default;
    }

    private void cis_SoapMessageSending(object sender, CentralniInformacijskiSustavEventArgs e)
    {
        Cursor.Current = Cursors.WaitCursor;
        stopWatch.Start();
        Application.DoEvents();
    }

    private void Principal()
    {
        IMerlinData dalMerlin = new MerlinData();
        

        LogFile.createSimpleLog();
        progressBar1.Value = 0;
        LogFile.LogToFile("------------Startam sa provjerom principala----------------", LogLevel.Debug);

        if (!AppLink.TestFileCfg())
        {
            MessageAlert(Translations.Translate("Nedostaje konfiguracijska datoteka!"), Translations.Translate("Greška"), NumLog.CfgMissing, 0, "");
            Close();
        }

        try
        {
            lblInfo.Text = Translations.Translate("Učitavanje podataka o poslovnom prostoru...");
            LogFile.LogToFile(lblInfo.Text, LogLevel.Debug);
            DataSalonToSend.VATNumber_Salon = AppLink.VATNumber;
            DataSalonToSend.LogFileIsActive = Convert.ToInt16(AppLink.LogFileActive);
            DataSalonToSend.DateIsActive = Convert.ToDateTime(AppLink.DateIsActive);
            DataSalonToSend.BillingDeviceMark = AppLink.BillingDeviceMark;
            DataSalonToSend.OIBSoftware = AppLink.OIBSoftware;
            Log.WriteLog(NumLog.AppLaunch, 0, "", placeholders, ErrorCode, ErrorMessage);
            CertificateName = AppLink.Certificate;

            int num2 = 0;
            CheckCertificate(ref num2);



            if (!Helper.DataVerification.VerifDataOk(DataSalonToSend, out vatActif, out errorMessage))
            {
                ErrorMessVerifData(errorMessage);
                Close();
            }

            FlushBillsWithNoJir(dalMerlin);

            LogFile.LogToFile("Fatching offers", LogLevel.Debug);
            var offers = dalMerlin.GetOffer(DataSalonToSend.VATNumber_Salon, vatActif);
            if (offers.Count > 0)
            {
                LogFile.LogToFile(String.Format("Found {0} offers ", offers.Count), LogLevel.Debug);

                foreach (DataBill offer in offers)
                {
                    LogFile.LogToFile(String.Format("For offer id {0}/{1} update Hash", offer.IdTicket, JsonConvert.SerializeObject(offer)), LogLevel.Debug);
                    SaveErrorOnBill(offer.IdTicket, "OVO NIJE FISKALIZIRANI RAČUN    " , dalMerlin);

                }



            }


            lblInfo.Text = Translations.Translate("Obrada računa (korak 1)...");
            LogFile.LogToFile("Fatching bills for fiscalization", LogLevel.Debug);
            var bills = dalMerlin.GetBill(DataSalonToSend.VATNumber_Salon, vatActif);
            Log.WriteLog(NumLog.BillProcSt1ok, 0, "", placeholders, ErrorCode, ErrorMessage);
            if (bills.Count < 1)
            {
                LogFile.LogToFile("There are no bills for fiscalization", LogLevel.Debug);
                Log.WriteLog(NumLog.NoBill, 0, "", placeholders, ErrorCode, ErrorMessage);
                CheckForUpdateEvent();
                LogFile.LogToFile("Closing screen", LogLevel.Debug);
                Close();
            }





            lblInfo.Text = Translations.Translate("Obrada računa (korak 2)...");
            LogFile.LogToFile(String.Format("Found {0} bills for fiscal, continuing with batch", bills.Count), LogLevel.Debug);
            Log.WriteLog(NumLog.StartBillProcSt2, 0, "", placeholders, ErrorCode, ErrorMessage);
            foreach (DataBill bill in bills)
            {
                LogFile.LogToFile(String.Format("For bill id {0}/{1} fetching details", bill.IdTicket, JsonConvert.SerializeObject(bill)), LogLevel.Debug);
                var billDetails = dalMerlin.GetBillFollow(bill);

                LogFile.LogToFile(String.Format("Details fetched bill id {0}/{1}", bill.IdTicket, JsonConvert.SerializeObject(billDetails)), LogLevel.Debug);
                if (billDetails.Notes.Contains("ZKI:") && billDetails.HashStatus.Length != 36)
                {
                    billDetails.MarkSubseqBillDelivery_Bill = true;
                }
                else
                {
                    billDetails.MarkSubseqBillDelivery_Bill = false;
                }

                //provjera OIB-a zaposlenika. Ako je -1 onda stavljam defaultnog tj firmu
                if (bill.CashierVATNumber_Bill == "OIBPERSOERROR")
                {
                    if (!AddCachierToTicket(dalMerlin, bill)) continue;

                    //prepišem potake sa salona
                    bill.CashierVATNumber_Bill = bill.VATNumber_Salon_Bill;
                }

                
                //šaljem na test
                if (AppLink.SendTestReceipts.Equals("1"))
                {
                    if (!SendBillToTestCis(billDetails, dalMerlin)) continue;                  
                }
                
                //šaljem na produkciju
                SendBillToProdCis(billDetails, dalMerlin);

            }


            FlushBillsWithNoJir(dalMerlin);
            lblInfo.Text = Translations.Translate("Zatvaranje aplikacije");
            CheckForUpdateEvent();
            Close();



        }
        catch (Exception e)
        {
            LogFile.LogToFile(String.Format("Greška u principalu {0}", e.Message), LogLevel.Debug);
            MessageAlert("Kritična greška u principalu "+e.Message +" izlazim iz aplikacije","Greška u principalu");
            Close();

        }
    }

    private bool SendBillToTestCis(DataBill billDetails, IMerlinData dalMerlin)
    {
        LogFile.LogToFile(String.Format("Bill id {0} sending to test CIS", billDetails.IdTicket), LogLevel.Debug);
        if (GetBillFailedAttempts(billDetails.IdTicket) >= 3)
        {
            LogFile.LogToFile(String.Format("Bill oid {0} checked 3 times send to production CIS", billDetails.IdTicket), LogLevel.Debug);
            return true;
        }
        try
        {
            CisBussines cisBl = new CisBussines();

            Log.WriteLog(NumLog.SendingTest, billDetails.IdTicket, "", placeholders, ErrorCode, ErrorMessage);

            lblInfo.Text = Translations.Translate("Slanje zahtjeva za provjerom racuna, molimo pričekajte...");
            LogFile.LogToFile("Sending request to test CIS", LogLevel.Debug);
            //XmlDocument xmlDocument = sendBillCheck(billDetails, dalMerlin);
            XmlDocument xmlDocument = cisBl.SendBill(billDetails, dalMerlin, CertificateName, true);
            if (xmlDocument == null)
            {
                throw new Exception(Translations.Translate("Nije primljen odgovor od CIS-a tokom slanja testnog računa"));
            }


            string message = "";
            if (!cisBl.CheckBillAnswer(xmlDocument, true, out message))
            {
                MessageAlert(message, Translations.Translate("Greška"));
                LogFile.LogToFile(String.Format("Error returned from bill check for ticket id: {0} error message: {1}", billDetails.IdTicket, message), LogLevel.Debug);
                if (message.Contains("v101") || message.Contains("v103") || message.Contains("v104") || message.Contains("v152") || message.Contains("v153"))
                {
                    LogFile.LogToFile(String.Format("Non critical error detected on ticket id: {0} error message: {1}", billDetails.IdTicket, message), LogLevel.Debug);
                    AddBillToFailedAttempts(billDetails.IdTicket);
                }
                SaveErrorOnBill(billDetails.IdTicket, "ERROR_CIS       "+message.Substring(0,10), dalMerlin);
                return false;
            }

            return true;
        }
        catch (NoOibException e)
        {
            LogFile.LogToFile(String.Format("Error for sending bill id to test CIS {0} / message: {1}", billDetails.IdTicket, e.Message), LogLevel.Debug);
            AddBillToFailedAttempts(billDetails.IdTicket);
            SaveErrorOnBill(billDetails.IdTicket, "ERROR_OIB                              ", dalMerlin);
            MessageAlert(Translations.Translate("Nedostaje OIB zaposlenika!"), Translations.Translate("Greška"));
            return false;
        }
        catch (Exception ex2)
        {
            if (ex2.Message.ToLowerInvariant().Contains("network password"))
            {
                MessageAlert(Translations.Translate("Lozinka certifikata nije ispravna!"), Translations.Translate("Greška"));
            }

            if (ex2.Message.ToLowerInvariant().Contains("error occurred on a send"))
            {
                //SendBillToProdCis(billDetails, dalMerlin);
                return true;
            }
            MessageAlert(Translations.Translate("Trenutno nije moguće spajanje na CIS, samo nastavite s izdavanjem računa klikom na OK") + Environment.NewLine + Translations.Translate("Kontaktirajte tehničku podršku!"), Translations.Translate("Greška"));
            SaveErrorOnBill(billDetails.IdTicket, "ERROR_CIS                              ", dalMerlin);
            LogFile.LogToFile(String.Format("General error for bill id {0} / message: {1}", billDetails.IdTicket, ex2.Message), LogLevel.Debug);
            Log.WriteLog(NumLog.ErrorHttp, billDetails.IdTicket, ex2.Message, placeholders, ErrorCode, ErrorMessage);
            return false;
        }
        
    }

    private void SendBillToProdCis(DataBill listBill, IMerlinData dalMerlin)
    {
        try
        {
            CisBussines cisBl = new CisBussines();
            LogFile.LogToFile(String.Format("Slanje računa na prod CIS {0} {1}", listBill.IdTicket, JsonConvert.SerializeObject(listBill)), LogLevel.Debug);
            Log.WriteLog(NumLog.SendBill, listBill.IdTicket, "", placeholders, ErrorCode, ErrorMessage);
            lblInfo.Text = Translations.Translate("Povezivanje s uslugom CIS, molimo pričekajte...");
            LogFile.LogToFile(lblInfo.Text, LogLevel.Debug);
            XmlDocument xmlDocument = cisBl.SendBill(listBill, dalMerlin, CertificateName, false); //sendBill(listBill, dalMerlin);

            if (xmlDocument == null)
            {
                throw new Exception(Translations.Translate("Nije primljen odgovor od CIS-a tokom slanja testnog računa"));
            }
            string message = "";
            
            if(!cisBl.CheckBillAnswer(xmlDocument, false, out message))
            {
                MessageAlert(message, Translations.Translate("Greška"));
                SaveErrorOnBill(listBill.IdTicket, "ERROR_CIS       " + message.Substring(0, 10), dalMerlin);
                LogFile.LogToFile(String.Format("Greška slanja računa na prod CIS {0} : {1}", listBill.IdTicket, message), LogLevel.Debug);
                throw new Exception("Error on CIS "+message);
            }

            txtResponse.Text = xmlDocument.OuterXml;
            LogFile.LogToFile(String.Format("Račun uspješno poslan na CIS {0}, odgovor: {1}", listBill.IdTicket, JsonConvert.SerializeObject(xmlDocument)), LogLevel.Debug);

            Log.WriteLog(NumLog.SendBillOK, 0, "", placeholders, ErrorCode, ErrorMessage);

            if (string.IsNullOrEmpty(txtResponse.Text))
            {
                Log.WriteLog(NumLog.EmptyXMLResponse, 0, "", placeholders, ErrorCode, ErrorMessage);
                SaveErrorOnBill(listBill.IdTicket, "ERROR_RESP Empty                        ", dalMerlin);
            }
            else if (VerifAndSaveJir(txtResponse.Text, listBill.IdTicket, "Jir", State: false, dalMerlin))
            {
                Log.WriteLog(NumLog.NextBill, 0, "", placeholders, ErrorCode, ErrorMessage);
            }
            else if (VerifAndSaveJir(txtResponse.Text, listBill.IdTicket, "SifraGreske", State: true, dalMerlin))
            {
                Log.WriteLog(NumLog.NextBill, 0, "", placeholders, ErrorCode, ErrorMessage);
            }
            else
            {
                LogFile.LogToFile(String.Format("Sranje u kodu  {0}", listBill.IdTicket), LogLevel.Debug);
                SaveErrorOnBill(listBill.IdTicket, "ERROR_RESP                             ", dalMerlin);
            }

        }
        catch (Exception ex3)
        {

            SimpleLog.Log(ex3);

            if (ex3.Message.ToLowerInvariant().Contains("network password"))
            {

                MessageAlert(Translations.Translate("Lozinka certifikata nije ispravna!"), Translations.Translate("Greška"));
            }
            else
            {
                MessageAlert(Translations.Translate("Trenutno nije moguće spajanje na CIS, samo nastavite s izdavanjem računa klikom na OK") + Environment.NewLine + Translations.Translate("Kontaktirajte tehničku podršku!"), Translations.Translate("Greška"));
            }

            Log.WriteLog(NumLog.ErrorHttp, listBill.IdTicket, ex3.Message, placeholders, ErrorCode, ErrorMessage);
            SaveErrorOnBill(listBill.IdTicket, "ERROR_eSJ", dalMerlin);

        }


    }

    private bool AddCachierToTicket(IMerlinData dalMerlin, DataBill bill)
    {
        try
        {
            LogFile.LogToFile(String.Format("For bill id {0} missing OIB", bill.IdTicket), LogLevel.Debug);
            var idCashier = dalMerlin.GetPersoByOib(DataSalonToSend.VATNumber_Salon);
            if (idCashier == -1)
            {
                MessageAlert(String.Format("Nedostaje OIB zaposlenika za račun {0}, defaultni zaposlenik ne postoji", bill.IdTicket), "Nedostaje OIB zaposlenika");
                LogFile.LogToFile(String.Format("Missing OIB for bill id {0}, default cachier dose not exist in database", bill.IdTicket), LogLevel.Debug);
                return false;
            }
            int ret=dalMerlin.UpdateTicketWithOib(bill.IdTicket, idCashier);
            LogFile.LogToFile(String.Format("Bill id {0} filled with OIB id {1}/db return {2}", bill.IdTicket, idCashier, ret), LogLevel.Debug);

            return true;

        }
        catch(Exception e)
        {
            LogFile.LogToFile(String.Format("Error ocured for bill ticket id {0} message {1}", bill.IdTicket, e.Message), LogLevel.Debug);
            MessageAlert(String.Format("Greška u ažuriranju računa {0} sa OIB-om, poruka greške {1}", bill.IdTicket, e.Message), "Nedostaje OIB zaposlenika");
            return false;
        }
    }

    public void AddBillToFailedAttempts(int idTicket)
    {
        LogFile.LogToFile(String.Format("Ticket id: {0} added to counting", idTicket), LogLevel.Debug);
        if (numberOfFailedAttempts.ContainsKey(idTicket.ToString()))
        {
            int num5 = numberOfFailedAttempts[idTicket.ToString()];
            num5++;
            numberOfFailedAttempts[idTicket.ToString()] = num5;
        }
        else
        {
            numberOfFailedAttempts.Add(idTicket.ToString(), 1);
        }

        LogFile.LogToFile(String.Format("Ticket id: {0} added to counting {1}", idTicket, serializer.Serialize(numberOfFailedAttempts)), LogLevel.Debug);

        _385_fisk.Properties.Settings.Default.TestBills = serializer.Serialize(numberOfFailedAttempts);
        _385_fisk.Properties.Settings.Default.Save();
    }

    public int GetBillFailedAttempts(int idTicket)
    {
        if (numberOfFailedAttempts.ContainsKey(idTicket.ToString()))
        {
            int num4 = numberOfFailedAttempts[idTicket.ToString()];
            if (num4 >= 3)
            {
                numberOfFailedAttempts.Remove(idTicket.ToString());
                _385_fisk.Properties.Settings.Default.TestBills = serializer.Serialize(numberOfFailedAttempts);
                _385_fisk.Properties.Settings.Default.Save();
            }
            return num4;
        }
        return 0;
    }

    private void CheckCertificate(ref int num2)
    {
        if (AppLink.UseCertificateFile == "0")
        {
            if (CertificateName.Length > 0)
            {
                num2 = Certifs.FindCertif(CertificateName);
            }
            else
            {
                placeholders = new Dictionary<string, string>();
                placeholders["{{%certificate_title%}}"] = CertificateName;
                MessageAlert(Translations.Translate("Osobni certifikat s nazivom {{%certificate_title%}} ne postoji!", placeholders), Translations.Translate("Greška"));
                LogFile.LogToFile(Translations.Translate("Osobni certifikat s nazivom {{%certificate_title%}} ne postoji!", placeholders));
                Close();
            }
            if (CertificateName.Length > 1)
            {
                int num3 = Certifs.CriticalCertValidity(CertificateName);
                int criticalCertDays = _385_fisk.Properties.Settings.Default.CriticalCertDays;
                if (num3 <= 30)
                {
                    if (num3 < 0)
                    {
                        MessageAlert("Vaš certifikat je istekao prije " + Math.Abs(num3) + " dana. Obnovite ga!", "Upozorenje");
                        _385_fisk.Properties.Settings.Default.CriticalCertDays = num3;
                        _385_fisk.Properties.Settings.Default.Save();
                    }
                    else if (num3 != criticalCertDays)
                    {
                        MessageAlert("Vaš certifikat ističe za " + num3 + " dana. Podsjetite se obnoviti ga!", "Upozorenje");
                        _385_fisk.Properties.Settings.Default.CriticalCertDays = num3;
                        _385_fisk.Properties.Settings.Default.Save();
                    }
                }
            }
            switch (num2)
            {
                case 0:
                    placeholders = new Dictionary<string, string>();
                    placeholders["{{%certificate_title%}}"] = CertificateName;
                    MessageAlert(Translations.Translate("Osobni certifikat s nazivom {{%certificate_title%}} ne postoji!", placeholders), Translations.Translate("Greška"));
                    LogFile.LogToFile(Translations.Translate("Osobni certifikat s nazivom {{%certificate_title%}} ne postoji!", placeholders));
                    Close();
                    break;
                case 1:
                    placeholders = new Dictionary<string, string>();
                    placeholders["{{%certificate_title%}}"] = CertificateName;
                    LogFile.LogToFile(Translations.Translate("Pronađen je osobni certifikat s nazivom {{%certificate_title%}}!", placeholders));
                    break;
                case 2:
                    placeholders = new Dictionary<string, string>();
                    placeholders["{{%certificate_title%}}"] = CertificateName;
                    MessageAlert(Translations.Translate("Pronađeno je više osobnih certifikata s nazivom {{%certificate_title%}}!", placeholders), Translations.Translate("Greška"));
                    LogFile.LogToFile(Translations.Translate("Pronađeno je više osobnih certifikata s nazivom {{%certificate_title%}}!", placeholders));
                    Close();
                    break;
            }
        }
    }

    #region AutoUpdater
    private void CheckForUpdateEvent()
    {
        if (!lastCheck.Equals(now.ToString("d.M.y")))
        {
            LogFile.LogToFile("Provjeravam novu verziju", LogLevel.Debug);
            _385_fisk.Properties.Settings.Default.LastCheck = now.ToString("d.M.y");
            _385_fisk.Properties.Settings.Default.Save();
            base.TopMost = false;
            AutoUpdater.Start("https://www.dropbox.com/s/l86kf0sochnqnh6/CisUpdateList.xml?dl=1");
            AutoUpdater.DownloadPath = Environment.CurrentDirectory;
            AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
        }
    }

    private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
    {
        if (args != null)
        {
            if (args.IsUpdateAvailable)
            {
                if (MessageBox.Show("Postoji nova verzija aplikacije za fiskalizaciju. Želite li ažurirati aplikaciju sada?", "Ažuriranje dostupno", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk).Equals(DialogResult.Yes))
                {
                    try
                    {
                        if (AutoUpdater.DownloadUpdate())
                        {
                            Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        SimpleLog.Log(ex);
                        MessageAlert(ex.Message + Environment.NewLine + Translations.Translate("Kontaktirajte tehničku podršku!"), Translations.Translate("Greška"));
                        Close();
                    }
                }
                else
                {
                    Close();
                }
            }
            else
            {
                Close();
            }
        }
        else
        {
            Close();
        }
    }
    #endregion

    



    private void SaveErrorOnBill(int Idticket, string m_Text, IMerlinData dalMerlin)
    {
        try
        {
            LogFile.LogToFile(String.Format("Adding error on ticket id {0}  error: {1}", Idticket, m_Text), LogLevel.Debug);
            var ret=dalMerlin.SaveTicketJir(Idticket, m_Text);
            LogFile.LogToFile(String.Format("Updated ticket id {0}  return code {1}", Idticket, ret), LogLevel.Debug);
            Log.WriteLog(NumLog.BillUpdateOK, Idticket, m_Text, placeholders, ErrorCode, ErrorMessage);
        }
        catch(Exception e)
        {
            LogFile.LogToFile(String.Format("Greška u dodavanju Error an račun {0} , {1}", Idticket, e.Message), LogLevel.Debug);
        }

    }

    public bool VerifAndSaveJir(string XmlText, int IDticket, string XmlTag, bool State, IMerlinData dalMerlin)
    {
        string text = "";
        bool flag = false;
        if (!State)
        {
            Log.WriteLog(NumLog.StartSearchJir, 0, "", placeholders, ErrorCode, ErrorMessage);
        }
        else
        {
            Log.WriteLog(NumLog.StartSearchError, 0, "", placeholders, ErrorCode, ErrorMessage);
        }
        try
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(XmlText);
            XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName(XmlTag, "http://www.apis-it.hr/fin/2012/types/f73");
            string text2 = "";
            if (elementsByTagName.Count != 1)
            {
                flag = false;
                Log.WriteLog(NumLog.JirNodeNotFount, 0, "", placeholders, ErrorCode, ErrorMessage);
                return false;
            }
            IEnumerator enumerator = elementsByTagName.GetEnumerator();
            try
            {
                if (enumerator.MoveNext())
                {
                    XmlNode xmlNode = (XmlNode)enumerator.Current;
                    if (xmlNode.FirstChild == null)
                    {
                        flag = false;
                        Log.WriteLog(NumLog.NodeJirIsNull, 0, "", placeholders, ErrorCode, ErrorMessage);
                    }
                    else
                    {
                        text2 = xmlNode.FirstChild.Value;
                        if (State)
                        {
                            text = "ERROR_" + text2;
                            flag = true;
                        }
                        else
                        {
                            text = text2;
                            flag = true;
                        }
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            if (string.IsNullOrEmpty(text2))
            {
                flag = false;
                Log.WriteLog(NumLog.JirIsNull, 0, "", placeholders, ErrorCode, ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            SimpleLog.Log(ex);
            MessageAlert(ex.Message + Environment.NewLine + Translations.Translate("Kontaktirajte tehničku podršku!"), Translations.Translate("Greška"));
            Log.WriteLog(NumLog.XMLerror, IDticket, ex.Message, placeholders, ErrorCode, ErrorMessage);
            flag = false;
        }
        if (!flag)
        {
            return false;
        }
        var ret=dalMerlin.SaveTicketJir(IDticket, text);
        LogFile.LogToFile(String.Format( "ticket updated {0} return {1} ", IDticket, ret ));

        var ticket = dalMerlin.GetTicket(IDticket);

        LogFile.LogToFile("number of Ticket fetched from database "+ticket.Count);
        LogFile.LogToFile("Ticket fetched from database " + JsonConvert.SerializeObject(ticket));
        Log.WriteLog(NumLog.BillUpdateOK, IDticket, text, placeholders, ErrorCode, ErrorMessage);
        if (State)
        {
            string str = Environment.NewLine + Translations.Translate("Kontaktirajte tehničku podršku!");
            switch (text)
            {
                case "s001":
                    MessageAlert(Translations.Translate("Poruka nije u skladu s XML shemom: #element ili lista elemenata koji nisu ispravni po shemi.") + str, Translations.Translate("Greška") + " s001", NumLog.s001, IDticket, "");

                    break;
                case "s002":
                    MessageAlert(Translations.Translate("Certifikat nije izdan od strane FINA-e.") + str, Translations.Translate("Greška") + " s002", NumLog.s002, IDticket, "");

                    break;
                case "s003":
                    MessageAlert(Translations.Translate("Certifikat ne sadrži naziv 'FISKAL'.") + str, Translations.Translate("Greška") + " s003", NumLog.s003, IDticket, "");

                    break;
                case "s004":
                    MessageAlert(Translations.Translate("Neispravan digitalni potpis.") + str, Translations.Translate("Greška") + " s004", NumLog.s004, IDticket, "");

                    break;
                case "s005":
                    MessageAlert(Translations.Translate("OIB iz poruke nije jednak OIB-u iz certifikata.") + str, Translations.Translate("Greška") + " s005", NumLog.s005, IDticket, "");

                    break;
                case "s006":
                    MessageAlert(Translations.Translate("Sistemska pogreška prilikom obrade zahtjeva.") + str, Translations.Translate("Greška") + " s006", NumLog.s006, IDticket, "");

                    break;
            }
        }
        return true;
    }



    private void MessageAlert(string content, string title, NumLog logerror = NumLog.None, int ticket = 1, string extendmessage = "")
    {
        //MessageBox.Show(content, title, MessageBoxButtons.OK, MessageBoxIcon.Hand);
        ErrorMessageBox errorMessageBox = new ErrorMessageBox();
        errorMessageBox.lbMessageText.Text = title;
        errorMessageBox.lbMessageDetails.Text = content;
        errorMessageBox.ShowDialog();

        Log.WriteLog(logerror, ticket, extendmessage, placeholders, ErrorCode, ErrorMessage);
    }

    private void Form1_Shown_1(object sender, EventArgs e)
    {
        if (AppLink.TestFileCfg())
        {
            IsSoloMode();
            Principal();
        }
        else
        {
            ConfigFile configFile = new ConfigFile();
            configFile.ConnectionString = "server=(local); uid=sa; pwd=_PWD4sa_; Database=Merlin";
            configFile.ServerUrl = "https://cis.porezna-uprava.hr:8449/FiskalizacijaService";
            configFile.ConnectionEncryption = "TLS 1.2";
            configFile.LogFileActive = "True";
            configFile.DateIsActive = "2013/04/01";
            configFile.SaveXMLActive = "True";
            configFile.XMLSavePath = "";
            configFile.VATNumber = "";
            configFile.InVATsystem = "True";
            configFile.PremiseMark = "";
            configFile.BillingDeviceMark = "";
            configFile.OIBSoftware = "";
            configFile.OperatorOIB = "";
            configFile.OperatorCode = "";
            LogFile.CreateConfigFile(configFile, isConfigMod: false);
            Config config = new Config();
            config.Show();
            config.TopMost = true;
        }
    }

    private void IsSoloMode()
    {
        IMerlinData dalMerlin = new MerlinData();
        try
        {
            if (AppLink.OperatorOIB.Length == 11 && AppLink.OperatorCode.Length > 0)
            {
                string operatorOIB = AppLink.OperatorOIB;
                string operatorCode = AppLink.OperatorCode;
                string text = "";
                var perso = dalMerlin.GetPerso();
                foreach (DataPerso item in perso)
                {
                    if (operatorOIB != item.numeroSecu || operatorCode != item.codePerso)
                    {
                        text = text + "UPDATE perso set numerosecu='" + operatorOIB + "', code='" + operatorCode + "' where id=" + item.idPerso + "\r\n";
                    }
                }
                if (text.Length > 0)
                {
                    dalMerlin.UpdatePerso(text);
                }
            }

        }
        catch(Exception e)
        {
            LogFile.LogToFile("greška u IsSoloMode "+e.Message, LogLevel.Debug);
            Close();
        }
        
    }

    private void FlushBillsWithNoJir(IMerlinData dalMerlin)
    {
        try
        {
            LogFile.LogToFile("Flush računa u bazi bez JIR-a", LogLevel.Debug);
            dalMerlin.FlushBillsWithNoJir();
        }
        catch(Exception e)
        {
            LogFile.LogToFile("greške u FlushBillsWithNoJir " + e.Message, LogLevel.Debug);
        }

    }




    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        AutoUpdater.Start("https://www.dropbox.com/s/l86kf0sochnqnh6/CisUpdateList.xml?dl=1");
        AutoUpdater.DownloadPath = Environment.CurrentDirectory;
    }

    #region Inicijalizacija


    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }



    private void InitializeComponent()
    {
        this.pictureBox2 = new System.Windows.Forms.PictureBox();
        this.lblFiskalisation = new System.Windows.Forms.Label();
        this.grpInfo = new System.Windows.Forms.GroupBox();
        this.lblServiceStatus = new System.Windows.Forms.Label();
        this.lblExecutionTime = new System.Windows.Forms.Label();
        this.progressBar1 = new System.Windows.Forms.ProgressBar();
        this.lblInfo = new System.Windows.Forms.Label();
        this.pictureBox3 = new System.Windows.Forms.PictureBox();
        this.txtResponse = new System.Windows.Forms.TextBox();
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
        this.grpInfo.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
        this.SuspendLayout();
        // 
        // pictureBox2
        // 
        this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
        this.pictureBox2.Location = new System.Drawing.Point(224, 44);
        this.pictureBox2.Name = "pictureBox2";
        this.pictureBox2.Size = new System.Drawing.Size(27, 24);
        this.pictureBox2.TabIndex = 42;
        this.pictureBox2.TabStop = false;
        this.pictureBox2.Visible = false;
        // 
        // lblFiskalisation
        // 
        this.lblFiskalisation.AutoSize = true;
        this.lblFiskalisation.BackColor = System.Drawing.Color.Transparent;
        this.lblFiskalisation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
        this.lblFiskalisation.ForeColor = System.Drawing.Color.White;
        this.lblFiskalisation.Location = new System.Drawing.Point(1, 5);
        this.lblFiskalisation.Name = "lblFiskalisation";
        this.lblFiskalisation.Size = new System.Drawing.Size(95, 13);
        this.lblFiskalisation.TabIndex = 4;
        this.lblFiskalisation.Text = "FISKALIZACIJA";
        // 
        // grpInfo
        // 
        this.grpInfo.BackColor = System.Drawing.Color.Transparent;
        this.grpInfo.Controls.Add(this.lblServiceStatus);
        this.grpInfo.Controls.Add(this.lblExecutionTime);
        this.grpInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
        this.grpInfo.Location = new System.Drawing.Point(8, 23);
        this.grpInfo.Name = "grpInfo";
        this.grpInfo.Size = new System.Drawing.Size(400, 83);
        this.grpInfo.TabIndex = 5;
        this.grpInfo.TabStop = false;
        this.grpInfo.Text = "Informacije";
        // 
        // lblServiceStatus
        // 
        this.lblServiceStatus.AutoSize = true;
        this.lblServiceStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
        this.lblServiceStatus.Location = new System.Drawing.Point(100, 27);
        this.lblServiceStatus.Name = "lblServiceStatus";
        this.lblServiceStatus.Size = new System.Drawing.Size(114, 13);
        this.lblServiceStatus.TabIndex = 6;
        this.lblServiceStatus.Text = "CIS status usluge :";
        this.lblServiceStatus.Visible = false;
        // 
        // lblExecutionTime
        // 
        this.lblExecutionTime.AutoSize = true;
        this.lblExecutionTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
        this.lblExecutionTime.Location = new System.Drawing.Point(104, 57);
        this.lblExecutionTime.Name = "lblExecutionTime";
        this.lblExecutionTime.Size = new System.Drawing.Size(110, 13);
        this.lblExecutionTime.TabIndex = 43;
        this.lblExecutionTime.Text = "Vrijeme izvršenja :";
        // 
        // progressBar1
        // 
        this.progressBar1.BackColor = System.Drawing.Color.White;
        this.progressBar1.ForeColor = System.Drawing.Color.Black;
        this.progressBar1.Location = new System.Drawing.Point(8, 112);
        this.progressBar1.Name = "progressBar1";
        this.progressBar1.Size = new System.Drawing.Size(400, 20);
        this.progressBar1.TabIndex = 44;
        // 
        // lblInfo
        // 
        this.lblInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
        | System.Windows.Forms.AnchorStyles.Left)
        | System.Windows.Forms.AnchorStyles.Right)));
        this.lblInfo.AutoSize = true;
        this.lblInfo.BackColor = System.Drawing.Color.Transparent;
        this.lblInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
        this.lblInfo.Location = new System.Drawing.Point(22, 142);
        this.lblInfo.MinimumSize = new System.Drawing.Size(370, 0);
        this.lblInfo.Name = "lblInfo";
        this.lblInfo.Size = new System.Drawing.Size(370, 16);
        this.lblInfo.TabIndex = 45;
        this.lblInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        // 
        // pictureBox3
        // 
        this.pictureBox3.BackColor = System.Drawing.Color.Transparent;
        this.pictureBox3.Image = global::_385_fisk.Properties.Resources.ajaxLoader;
        this.pictureBox3.Location = new System.Drawing.Point(381, 2);
        this.pictureBox3.Name = "pictureBox3";
        this.pictureBox3.Size = new System.Drawing.Size(30, 17);
        this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
        this.pictureBox3.TabIndex = 47;
        this.pictureBox3.TabStop = false;
        // 
        // txtResponse
        // 
        this.txtResponse.Location = new System.Drawing.Point(22, 201);
        this.txtResponse.Name = "txtResponse";
        this.txtResponse.Size = new System.Drawing.Size(100, 20);
        this.txtResponse.TabIndex = 48;
        // 
        // MainForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackgroundImage = global::_385_fisk.Properties.Resources.background;
        this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
        this.ClientSize = new System.Drawing.Size(414, 171);
        this.Controls.Add(this.txtResponse);
        this.Controls.Add(this.pictureBox3);
        this.Controls.Add(this.lblInfo);
        this.Controls.Add(this.progressBar1);
        this.Controls.Add(this.pictureBox2);
        this.Controls.Add(this.grpInfo);
        this.Controls.Add(this.lblFiskalisation);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        this.Icon = global::_385_fisk.Properties.Resources.icon;
        this.Name = "MainForm";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "Fiskalizacija-DEV";
        this.TopMost = true;
        this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
        this.Shown += new System.EventHandler(this.Form1_Shown_1);
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
        this.grpInfo.ResumeLayout(false);
        this.grpInfo.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();

    }
    #endregion


}
