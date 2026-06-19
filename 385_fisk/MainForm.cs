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
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Web.Management;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Xml;

public class MainForm : Form
{

    #region declarations
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

    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


    #endregion

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
        log.Error("Error in config data verification " + myMessage);
    }

    private void InitializeMerlinData(out IMerlinData dalMerlin)
    {
        dalMerlin = new MerlinData();
    }
    private bool ValidateConfiguration()
    {
        if (AppLink.TestFileCfg()) return true;

        MessageAlert(Translations.Translate("Nedostaje konfiguracijska datoteka!"),
                     Translations.Translate("Greška"), NumLog.CfgMissing, 0, "");
        log.Error("Missing config file, closing application");
        Close();
        return false;
    }

    private void LoadBusinessUnitData()
    {
        log.Debug("Loading data for business unit");
        lblInfo.Text = Translations.Translate("Učitavanje podataka o poslovnom prostoru...");

        DataSalonToSend.VATNumber_Salon = AppLink.VATNumber;
        DataSalonToSend.LogFileIsActive = Convert.ToInt16(AppLink.LogFileActive);
        DataSalonToSend.DateIsActive = Convert.ToDateTime(AppLink.DateIsActive);
        DataSalonToSend.BillingDeviceMark = AppLink.BillingDeviceMark;
        DataSalonToSend.OIBSoftware = AppLink.OIBSoftware;
        CertificateName = AppLink.Certificate;

        Log.WriteLog(NumLog.AppLaunch, 0, "", placeholders, ErrorCode, ErrorMessage);

        int num2 = 0;
        CheckCertificate(ref num2);
        log.Debug("Business unit data loaded");
    }

    private bool VerifyBusinessUnitData()
    {
        if (Helper.DataVerification.VerifDataOk(DataSalonToSend, out vatActif, out string errorMessage))
            return true;

        ErrorMessVerifData(errorMessage);
        log.Debug("Closing application");
        Close();
        return false;
    }

    private void ProcessOffers(IMerlinData dalMerlin)
    {
        log.Debug("Offer processing START");
        var offers = dalMerlin.GetOffer(DataSalonToSend.VATNumber_Salon, vatActif);

        if (offers.Count > 0)
        {
            log.Debug($"Found {offers.Count} offers in database, continuing with application");
            foreach (var offer in offers)
            {
                log.Debug($"For offer id {offer.IdTicket}/{JsonConvert.SerializeObject(offer)} update Hash");
                SaveErrorOnBill(offer.IdTicket, "OVO NIJE FISKALIZIRANI RAČUN", dalMerlin);
            }
        }
        log.Debug("Offer processing END");
    }

    private void ProcessBills(IMerlinData dalMerlin)
    {
        lblInfo.Text = Translations.Translate("Obrada računa (korak 1)...");
        log.Debug("Fetching bills for fiscalization");

        var bills = dalMerlin.GetAllBills(DataSalonToSend.VATNumber_Salon, vatActif);
        log.Debug($"Found {bills.Count} bills for fiscalization, continuing with batch");

        foreach (var bill in bills)
        {
            log.Debug("Processing seperate bill " + bill.IdTicket);
            ProcessBill(bill, dalMerlin);
        }
    }

    private void ProcessBill(DataBill bill, IMerlinData dalMerlin)
    {
        log.Debug($"Fetching details for bill id {bill.IdTicket}/{JsonConvert.SerializeObject(bill)}");

        var billDetails = dalMerlin.GetBillFollow(bill);
        billDetails.MarkSubseqBillDelivery_Bill =
            billDetails.Notes.Contains("ZKI:") && billDetails.HashStatus.Length != 36;

        billDetails.IsPro = bill.IsPro;
        billDetails.BuyerTaxNumber = bill.BuyerTaxNumber;

        if (bill.CashierVATNumber_Bill == "OIBPERSOERROR")
        {
            if (!AddCachierToTicket(dalMerlin, bill)) return;
            bill.CashierVATNumber_Bill = bill.VATNumber_Salon_Bill;
        }

        if(billDetails.IsPro && billDetails.PaymentMethod_Bill==NacinPlacanjaType.T)
        {
            log.Error($"Bill id {bill.IdTicket} is forbiden for T payment type");
            ErrorMessVerifData("Nije moguće odabrai transakcijsko plaćanje za pravni osobu! Ovaj račun biti će fiskaliziran kao za običnog klijenta. STORNIRAJTE GA ODMAH i izdajte račun kroz fisklizaciju 2.0 preko vašeg posrednika ili knjigovodstva.");
            return;
        }

        if (AppLink.SendTestReceipts.Equals("1") && !SendBillToTestCis(billDetails, dalMerlin)) return;

        int ret = SendBillToProdCis(billDetails, dalMerlin);
        if (AppLink.SendTip.Equals("1") && ret == 0) ProcessTip(billDetails, dalMerlin);
    }

    private void ProcessTip(DataBill billDetails, IMerlinData dalMerlin)
    {
        var tip = dalMerlin.GetTip(billDetails.IdTicket);

        if (tip.Amount == 0)
        {
            log.Debug($"Tip amount for ticket {tip.IdTicket} is 0");
            return;
        }

        log.Debug($"Processing tip for ticket {billDetails.IdTicket}, amount {tip.Amount:F}");
        SendTipToProdCis(billDetails, tip, dalMerlin);
    }

    private void ProcessFailedTips(IMerlinData dalMerlin)
    {
        if (!AppLink.SendTip.Equals("1")) return;

        var tipsFailed = dalMerlin.GetFailedTips();
        if (tipsFailed.Count == 0) return;

        log.Debug($"Failed tips fetched, total count: {tipsFailed.Count}");

        foreach (var tip in tipsFailed)
        {
            if (tip.Amount == 0)
            {
                log.Debug($"Tip amount for tip {tip.IdTicket} is 0");
                continue;
            }

            log.Debug($"Reprocessing tip {tip.IdTicket} with previous comment {tip.Comment}");
            var oneBill = dalMerlin.GetOneBill(DataSalonToSend.VATNumber_Salon, vatActif, tip.IdTicket);
            var billDetails = dalMerlin.GetBillFollow(oneBill);
            SendTipToProdCis(billDetails, tip, dalMerlin);
        }
    }

    private void FinalizeProcessing(IMerlinData dalMerlin)
    {
        FlushBillsWithNoJir(dalMerlin);
        lblInfo.Text = Translations.Translate("Zatvaranje aplikacije");
        log.Debug("Closing application");

        CheckForUpdateEvent();
        Close();
    }

    private void HandleCriticalError(Exception e)
    {
        log.Error("Critical error in principal", e);
        MessageAlert($"Kritična greška u principalu {e.Message}, izlazim iz aplikacije", "Greška u principalu");
        Close();
    }

    private void Principal()
    {
        try
        {
            // Initialize necessary objects and logging
            InitializeMerlinData(out IMerlinData dalMerlin);

            // Validate the application configuration file
            if (!ValidateConfiguration()) return;

            // Load business unit details (VAT number, certificate, etc.)
            LoadBusinessUnitData();

            // Verify if the loaded data is valid; exit if not
            if (!VerifyBusinessUnitData()) return;

            // Remove any bills that lack a JIR (fiscalization ID)
            FlushBillsWithNoJir(dalMerlin);

            // Process available offers
            ProcessOffers(dalMerlin);

            // Process all bills for fiscalization
            ProcessBills(dalMerlin);

            // Retry sending any failed tips
            ProcessFailedTips(dalMerlin);

            // Final application cleanup and closing
            FinalizeProcessing(dalMerlin);
        }
        catch (Exception e)
        {
            // Handle unexpected errors gracefully
            HandleCriticalError(e);
        }
    }

    /// <summary>
    /// Sends a tip transaction to the CIS (Fiscalization System).
    /// </summary>
    /// <param name="billDetails">Details of the bill associated with the tip.</param>
    /// <param name="dataTip">Tip data to be sent.</param>
    /// <param name="dalMerlin">Data access layer for database interactions.</param>
    private void SendTipToProdCis(DataBill billDetails, DataTip dataTip, IMerlinData dalMerlin)
    {
        log.Debug($"Bill ID {billDetails.IdTicket} - Sending tip to production CIS");

        try
        {
            // Initialize the CIS business logic handler
            CisBussines cisBl = new CisBussines(DataSalonToSend);

            log.Debug($"Sending tip to production CIS for Bill ID {billDetails.IdTicket}, Data: {JsonConvert.SerializeObject(billDetails)}");

            // Update UI to indicate CIS connection
            lblInfo.Text = Translations.Translate("Povezivanje s uslugom CIS, molimo pričekajte...");
            log.Debug("Connecting with CIS, please stand by...");

            // Send tip request to CIS and receive response as XML
            XmlDocument xmlDocument = cisBl.SendTip(billDetails, dataTip, dalMerlin, CertificateName, true);

            // If response is null, CIS did not return any data
            if (xmlDocument == null)
            {
                throw new Exception(Translations.Translate("Nije primljen odgovor od CIS-a tokom slanja napojnice"));
            }

            // Verify CIS response
            string message = "";
            if (!cisBl.CheckTipAnswer(xmlDocument, out message))
            {
                MessageAlert(message, Translations.Translate("Greška"));
                SaveErrorOnTip(dataTip, "GRESKA " + message.Substring(0, 10), dalMerlin);
                log.Error($"Error sending to production CIS for Tip ID {dataTip.IdTicket}: {message}");
                throw new Exception("Error on CIS " + message);
            }

            // Log and update UI with CIS response
            txtResponse.Text = xmlDocument.OuterXml;
            log.Debug($"Tip successfully sent to CIS for Tip ID {dataTip.IdTicket}, Response: {JsonConvert.SerializeObject(xmlDocument)}");

            Log.WriteLog(NumLog.SendBillOK, 0, "", placeholders, ErrorCode, ErrorMessage);

            // Handle different response cases
            if (string.IsNullOrEmpty(txtResponse.Text))
            {
                // Empty XML response
                Log.WriteLog(NumLog.EmptyXMLResponse, 0, "", placeholders, ErrorCode, ErrorMessage);
                SaveErrorOnTip(dataTip, "GRESKA Empty", dalMerlin);
            }
            else if (VerifAndSaveTip(txtResponse.Text, dataTip.IdTicket, "SifraPoruke", State: false, dalMerlin))
            {
                // Process successful response
                Log.WriteLog(NumLog.NextBill, 0, "", placeholders, ErrorCode, ErrorMessage);
            }
            else if (VerifAndSaveTip(txtResponse.Text, dataTip.IdTicket, "SifraGreske", State: true, dalMerlin))
            {
                // Process error response
                Log.WriteLog(NumLog.NextBill, 0, "", placeholders, ErrorCode, ErrorMessage);
            }
            else
            {
                // Unknown error case
                log.Debug($"Unexpected error while sending tip for Tip ID {dataTip.IdTicket}");
                SaveErrorOnTip(dataTip, "GRESKA Unknown", dalMerlin);
            }
        }
        catch (Exception ex)
        {
            // Log and handle exceptions
            log.Error(ex);

            if (ex.Message.ToLowerInvariant().Contains("network password"))
            {
                // Certificate password error
                MessageAlert(Translations.Translate("Lozinka certifikata nije ispravna!"), Translations.Translate("Greška"));
            }
            else
            {
                // General CIS connection failure
                MessageAlert(
                    Translations.Translate("Trenutno nije moguće spajanje na CIS, samo nastavite s izdavanjem računa klikom na OK")
                    + Environment.NewLine + Translations.Translate("Kontaktirajte tehničku podršku!"),
                    Translations.Translate("Greška")
                );
            }

            // Log HTTP error
            Log.WriteLog(NumLog.ErrorHttp, billDetails.IdTicket, ex.Message, placeholders, ErrorCode, ErrorMessage);
        }
    }
    private void SaveErrorOnTip(DataTip dataTip, string m_Text, IMerlinData dalMerlin)
    {
        try
        {
            log.Debug(String.Format("Adding error on ticket id {0}  error: {1}", dataTip, m_Text));
            dalMerlin.UpdateTip(dataTip.IdTicket, m_Text);
            log.Debug(String.Format("Updated ticket id {0} done", dataTip.IdTicket));
            Log.WriteLog(NumLog.BillUpdateOK, dataTip.IdTicket, m_Text, placeholders, ErrorCode, ErrorMessage);
        }
        catch (Exception e)
        {
            log.Error("Error ocured in SaveErrorOnBill", e);

        }
    }

    /// <summary>
    /// Sends a bill to the test CIS (Fiscalization System) for validation.
    /// </summary>
    /// <param name="billDetails">Bill data to be sent.</param>
    /// <param name="dalMerlin">Data access layer for database interactions.</param>
    /// <returns>True if the bill passes validation or needs to be sent to production, otherwise false.</returns>
    private bool SendBillToTestCis(DataBill billDetails, IMerlinData dalMerlin)
    {
        log.Debug($"Bill ID {billDetails.IdTicket} - Sending to test CIS");

        // Check if the bill has already failed 3 times; if so, proceed to production CIS
        if (GetBillFailedAttempts(billDetails.IdTicket) >= 3)
        {
            log.Debug($"Bill ID {billDetails.IdTicket} checked 3 times, sending to production CIS");
            return true;
        }

        try
        {
            // Initialize the CIS business logic handler
            CisBussines cisBl = new CisBussines(DataSalonToSend);

            Log.WriteLog(NumLog.SendingTest, billDetails.IdTicket, "", placeholders, ErrorCode, ErrorMessage);

            // Update UI to indicate CIS connection
            lblInfo.Text = Translations.Translate("Slanje zahtjeva za provjerom računa, molimo pričekajte...");
            log.Debug("Sending request to test CIS...");

            // Send the bill request to test CIS and receive the response as XML
            XmlDocument xmlDocument = cisBl.SendBill(billDetails, dalMerlin, CertificateName, true);

            // Handle missing response from CIS
            if (xmlDocument == null)
            {
                throw new Exception(Translations.Translate("Nije primljen odgovor od CIS-a tokom slanja testnog računa"));
            }

            // Validate CIS response
            string message = "";
            if (!cisBl.CheckBillAnswer(xmlDocument, true, out message))
            {
                MessageAlert(message, Translations.Translate("Greška"));
                log.Debug($"Error returned from bill check for ticket ID {billDetails.IdTicket}: {message}");

                var messageCodes = AppLink.GetFailedMessageCodes
    .Split(',')
    .Select(code => code.Trim())
    .Where(code => !string.IsNullOrWhiteSpace(code))
    .ToList();

                // Handle non-critical errors that allow retrying
                if (messageCodes.Any(code => message.StartsWith(code, StringComparison.OrdinalIgnoreCase)))
                {
                    log.Debug($"Non-critical error detected on ticket ID {billDetails.IdTicket}: {message}");
                    AddBillToFailedAttempts(billDetails.IdTicket);
                }

                // Log error and return failure
                SaveErrorOnBill(billDetails.IdTicket, "GRESKA_CIS " + message.Substring(0, 10), dalMerlin);
                return false;
            }

            // Bill successfully verified in test mode
            return true;
        }
        catch (NoOibException e)
        {
            // Handle missing OIB (tax number) error
            log.Error($"Error sending bill ID {billDetails.IdTicket} to test CIS", e);
            AddBillToFailedAttempts(billDetails.IdTicket);
            SaveErrorOnBill(billDetails.IdTicket, "GRESKA_OIB", dalMerlin);
            MessageAlert(Translations.Translate("Nedostaje OIB zaposlenika!"), Translations.Translate("Greška"));
            return false;
        }
        catch (Exception ex)
        {
            // Handle specific errors
            log.Error($"General error for bill ID {billDetails.IdTicket}", ex);

            if (ex.Message.ToLowerInvariant().Contains("network password"))
            {
                // Incorrect certificate password
                MessageAlert(Translations.Translate("Lozinka certifikata nije ispravna!"), Translations.Translate("Greška"));
            }
            else if (ex.Message.ToLowerInvariant().Contains("error occurred on a send"))
            {
                // If error suggests a sending issue, proceed to production CIS
                return true;
            }
            else
            {
                // General connectivity failure
                MessageAlert(
                    Translations.Translate("Trenutno nije moguće spajanje na CIS, samo nastavite s izdavanjem računa klikom na OK") +
                    Environment.NewLine + Translations.Translate("Kontaktirajte tehničku podršku!"),
                    Translations.Translate("Greška")
                );

                SaveErrorOnBill(billDetails.IdTicket, "GRESKA_CIS", dalMerlin);
                Log.WriteLog(NumLog.ErrorHttp, billDetails.IdTicket, ex.Message, placeholders, ErrorCode, ErrorMessage);
            }

            return false;
        }
    }




    private bool AddCachierToTicket(IMerlinData dalMerlin, DataBill bill)
    {
        try
        {
            log.Debug(String.Format("For bill id {0} missing OIB", bill.IdTicket));
            var idCashier = dalMerlin.GetPersoByOib(DataSalonToSend.VATNumber_Salon);
            if (idCashier == -1)
            {
                MessageAlert(String.Format("Nedostaje OIB zaposlenika za račun {0}, defaultni zaposlenik ne postoji", bill.IdTicket), "Nedostaje OIB zaposlenika");
                log.Debug(String.Format("Missing OIB for bill id {0}, default cachier dose not exist in database", bill.IdTicket));
                return false;
            }
            int ret = dalMerlin.UpdateTicketWithOib(bill.IdTicket, idCashier);
            log.Debug(String.Format("Bill id {0} filled with OIB id {1}/db return {2}", bill.IdTicket, idCashier, ret));

            return true;

        }
        catch (Exception e)
        {
            log.Error(String.Format("Error ocured for bill ticket id {0} ", bill.IdTicket), e);
            MessageAlert(String.Format("Greška u ažuriranju računa {0} sa OIB-om, poruka greške {1}", bill.IdTicket, e.Message), "Nedostaje OIB zaposlenika");
            return false;
        }
    }

    public void AddBillToFailedAttempts(int idTicket)
    {
        log.Debug(String.Format("Ticket id: {0} added to counting", idTicket));
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

        log.Debug(String.Format("Ticket id: {0} added to counting {1}", idTicket, serializer.Serialize(numberOfFailedAttempts)));

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

    private void CheckDemoCertificate(bool useTestServer = true)
    {
        X509Certificate2 x509Certificate = null;
        try
        {
            if (useTestServer)
            {
                log.Debug("Fetching demo certificate to check validity and etc.");
                x509Certificate = Potpisivanje.DohvatiCertifikat(AppLink.DatotekaDemoCertifikata(), "Demo03");
                if (x509Certificate == null) throw new Exception("No demo certificate");
                log.Debug("Fetched test certificate " + x509Certificate.Subject);
                int num3 = (DateTime.Parse(x509Certificate.GetExpirationDateString()) - DateTime.Now.Date).Days;
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
                        MessageAlert("Vaš demo certifikat ističe za " + num3 + " dana. Podsjetite se obnoviti ga!", "Upozorenje");
                        _385_fisk.Properties.Settings.Default.CriticalCertDays = num3;
                        _385_fisk.Properties.Settings.Default.Save();
                    }
                }

            }
        }
        catch (Exception ex)
        {
            log.Error("Error in fetching demo certificate " + ex.Message);
        }


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
                log.Debug(String.Format("Certificate {0} dose not exist", CertificateName));
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
                    log.Debug(String.Format("Certificate {0} dose not exist", CertificateName)); Close();
                    break;
                case 1:
                    placeholders = new Dictionary<string, string>();
                    placeholders["{{%certificate_title%}}"] = CertificateName;
                    log.Debug("Certificate loaded succsessfully");
                    break;
                case 2:
                    placeholders = new Dictionary<string, string>();
                    placeholders["{{%certificate_title%}}"] = CertificateName;
                    MessageAlert(Translations.Translate("Pronađeno je više osobnih certifikata s nazivom {{%certificate_title%}}!", placeholders), Translations.Translate("Greška"));
                    log.Debug(String.Format("Multiple Certificate {0} found", CertificateName)); Close();
                    break;
            }
        }
        if (AppLink.UseCertificateFile == "1")
        {
            X509Certificate2 x509Certificate = null;
            if (string.IsNullOrEmpty(AppLink.DatotekaCertifikata()))
            {
                log.Debug("Certificate file path not defined!!!");
                throw new Exception("Certificate file path not defined!!!");
            }
            log.Debug("Fetching certificate from file " + AppLink.DatotekaCertifikata());
            x509Certificate = Potpisivanje.DohvatiCertifikat(AppLink.DatotekaCertifikata(), AppLink.CertificatePassword);
            if (x509Certificate == null) throw new Exception("No certificate");
            log.Debug("Fetched certificate " + x509Certificate.Subject);
            int num3 = (DateTime.Parse(x509Certificate.GetExpirationDateString()) - DateTime.Now.Date).Days;
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
                    MessageAlert("Vaš demo certifikat ističe za " + num3 + " dana. Podsjetite se obnoviti ga!", "Upozorenje");
                    _385_fisk.Properties.Settings.Default.CriticalCertDays = num3;
                    _385_fisk.Properties.Settings.Default.Save();
                }
            }

        }
    }

    /// <summary>
    /// Sends a bill to the production CIS (Fiscalization System).
    /// </summary>
    /// <param name="listBill">Bill data to be sent.</param>
    /// <param name="dalMerlin">Data access layer for database interactions.</param>
    /// <returns>
    /// Integer return codes:
    /// 0 - Success
    /// 1 - Empty response or error in verification
    /// 2 - Incorrect certificate password
    /// 3 - Network connection issue
    /// 4 - General error
    /// </returns>
    private int SendBillToProdCis(DataBill listBill, IMerlinData dalMerlin)
    {
        int returnValue = 0; // Default return value (0 means success)

        try
        {
            // Initialize the CIS business logic handler
            CisBussines cisBl = new CisBussines(DataSalonToSend);

            log.Debug($"Sending bill ID {listBill.IdTicket} to production CIS: {JsonConvert.SerializeObject(listBill)}");
            Log.WriteLog(NumLog.SendBill, listBill.IdTicket, "", placeholders, ErrorCode, ErrorMessage);

            // Update UI to indicate CIS connection
            lblInfo.Text = Translations.Translate("Povezivanje s uslugom CIS, molimo pričekajte...");
            log.Debug("Connecting with CIS, please stand by...");

            // Send bill request to CIS and receive response as XML
            XmlDocument xmlDocument = cisBl.SendBill(listBill, dalMerlin, CertificateName, false);

            // Handle missing response from CIS
            if (xmlDocument == null)
            {
                throw new Exception(Translations.Translate("Nije primljen odgovor od CIS-a tokom slanja računa"));
            }

            string message = "";

            // Validate CIS response
            if (!cisBl.CheckBillAnswer(xmlDocument, false, out message))
            {
                MessageAlert(message, Translations.Translate("Greška"));
                SaveErrorOnBill(listBill.IdTicket, "GRESKA_CIS " + message.Substring(0, 10), dalMerlin);
                log.Error($"Error sending bill ID {listBill.IdTicket} to production CIS: {message}");
                throw new Exception($"Error on CIS: {message}");
            }

            // Save response from CIS
            txtResponse.Text = xmlDocument.OuterXml;
            log.Debug($"Bill ID {listBill.IdTicket} successfully sent to CIS, response: {JsonConvert.SerializeObject(xmlDocument)}");
            Log.WriteLog(NumLog.SendBillOK, 0, "", placeholders, ErrorCode, ErrorMessage);

            // Handle empty response
            if (string.IsNullOrEmpty(txtResponse.Text))
            {
                Log.WriteLog(NumLog.EmptyXMLResponse, 0, "", placeholders, ErrorCode, ErrorMessage);
                SaveErrorOnBill(listBill.IdTicket, "GRESKA_RESP Empty", dalMerlin);
                returnValue = 1;
            }
            // Verify and save JIR (Unique Invoice Identifier) for valid response
            else if (VerifAndSaveJir(txtResponse.Text, listBill.IdTicket, "Jir", State: false, dalMerlin))
            {
                Log.WriteLog(NumLog.NextBill, 0, "", placeholders, ErrorCode, ErrorMessage);
                returnValue = 0; // Successfully processed
            }
            // Handle error code from CIS response
            else if (VerifAndSaveJir(txtResponse.Text, listBill.IdTicket, "SifraGreske", State: true, dalMerlin))
            {
                Log.WriteLog(NumLog.NextBill, 0, "", placeholders, ErrorCode, ErrorMessage);
                returnValue = 1;
            }
            else
            {
                log.Error($"Unexpected error occurred while processing bill ID {listBill.IdTicket}");
                SaveErrorOnBill(listBill.IdTicket, "GRESKA_RESP", dalMerlin);
                returnValue = 1;
            }

            return returnValue;
        }
        catch (Exception ex)
        {
            log.Error($"Exception occurred while sending bill ID {listBill.IdTicket}: {ex}");

            if (ex.Message.ToLowerInvariant().Contains("network password"))
            {
                returnValue = 2;
                MessageAlert(Translations.Translate("Lozinka certifikata nije ispravna!"), Translations.Translate("Greška"));
            }
            else
            {
                returnValue = 3;
                MessageAlert(
                    Translations.Translate("Trenutno nije moguće spajanje na CIS, samo nastavite s izdavanjem računa klikom na OK") +
                    Environment.NewLine + Translations.Translate("Kontaktirajte tehničku podršku!"),
                    Translations.Translate("Greška")
                );
            }

            Log.WriteLog(NumLog.ErrorHttp, listBill.IdTicket, ex.Message, placeholders, ErrorCode, ErrorMessage);

            // Mark as general error
            returnValue = 4;
            SaveErrorOnBill(listBill.IdTicket, "GRESKA_eSJ", dalMerlin);

            return returnValue;
        }
    }

    #region AutoUpdater
    private void CheckForUpdateEvent()
    {
        log.Debug("Checking for update of CIS application");
        //if (!lastCheck.Equals(now.ToString("d.M.y")))
        //{

        //log.Debug("Checking for new version of CIS application");
        //_385_fisk.Properties.Settings.Default.LastCheck = now.ToString("d.M.y");
        // _385_fisk.Properties.Settings.Default.Save();
        //base.TopMost = false;
        //AutoUpdater.Start("https://www.dropbox.com/s/l86kf0sochnqnh6/CisUpdateList.xml?dl=1");
        //AutoUpdater.DownloadPath = Environment.CurrentDirectory;
        //AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
        //}
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
                        if (AutoUpdater.DownloadUpdate(args))
                        {
                            Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
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
            log.Debug(String.Format("Adding error on ticket id {0}  error: {1}", Idticket, m_Text));
            var ret = dalMerlin.SaveTicketJir(Idticket, m_Text);
            log.Debug(String.Format("Updated ticket id {0}  return code {1}", Idticket, ret));
            Log.WriteLog(NumLog.BillUpdateOK, Idticket, m_Text, placeholders, ErrorCode, ErrorMessage);
        }
        catch (Exception e)
        {
            log.Error("Error ocured in SaveErrorOnBill", e);

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
            MessageAlert(ex.Message + Environment.NewLine + Translations.Translate("Kontaktirajte tehničku podršku!"), Translations.Translate("Greška"));
            Log.WriteLog(NumLog.XMLerror, IDticket, ex.Message, placeholders, ErrorCode, ErrorMessage);
            log.Error("Error ocured in line 670", ex);
            flag = false;
        }
        if (!flag)
        {
            return false;
        }
        var ret = dalMerlin.SaveTicketJir(IDticket, text);

        log.Debug(String.Format("ticket updated {0} return {1} ", IDticket, ret));
        var ticket = dalMerlin.GetTicket(IDticket);

        log.Debug("Number of Ticket fetched from database " + ticket.Count);
        log.Debug("Ticket fetched from database " + JsonConvert.SerializeObject(ticket));




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


    public bool VerifAndSaveTip(string XmlText, int IDticket, string XmlTag, bool State, IMerlinData dalMerlin)
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
            MessageAlert(ex.Message + Environment.NewLine + Translations.Translate("Kontaktirajte tehničku podršku!"), Translations.Translate("Greška"));
            Log.WriteLog(NumLog.XMLerror, IDticket, ex.Message, placeholders, ErrorCode, ErrorMessage);
            log.Error("Error ocured in line 670", ex);
            flag = false;
        }
        if (!flag)
        {
            return false;
        }
        dalMerlin.UpdateTip(IDticket, text);

        log.Debug(String.Format("TIP updated {0} ", IDticket));
        //var ticket = dalMerlin.GetTicket(IDticket);

        //log.Debug("Number of Ticket fetched from database " + ticket.Count);
        //log.Debug("Ticket fetched from database " + JsonConvert.SerializeObject(ticket));




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
        errorMessageBox.tbMessage.Text = content;
        errorMessageBox.ShowDialog();

        Log.WriteLog(logerror, ticket, extendmessage, placeholders, ErrorCode, ErrorMessage);
        log.Debug(String.Format("Message alert with content {0}", content));
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
            log.Debug("Config file creation START");
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
            configFile.MessageCodes = "V100,v101";
            LogFile.CreateConfigFile(configFile, isConfigMod: false);
            Config config = new Config();
            config.Show();
            config.TopMost = true;
            log.Debug("Config file creation END");
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
        catch (Exception e)
        {

            log.Error("Error in solo mode ", e);
            Close();
        }

    }

    private void FlushBillsWithNoJir(IMerlinData dalMerlin)
    {
        try
        {
            log.Debug("Flush database for bills without JIR");
            DateTime euroDate = new DateTime(2023, 1, 1);
            if (DateTime.Now > euroDate)
            {
                log.Debug("Flush partial, euro date is in place");
                dalMerlin.FlushBillsWithNoJirPartial();
            }
            else
            {
                log.Debug("Flush full, no euro date set");
                dalMerlin.FlushBillsWithNoJir();
            }

        }
        catch (Exception e)
        {

            log.Error("Error in FlushBillsWithNoJir", e);
        }

    }




    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        //AutoUpdater.Start("https://www.dropbox.com/s/l86kf0sochnqnh6/CisUpdateList.xml?dl=1");
        //AutoUpdater.DownloadPath = Environment.CurrentDirectory;
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
