// MainForm
using AutoUpdaterDotNET;
using CisDal;
using DataObjects;
using Helper;
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
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Xml;

public class MainForm : Form {
  

  private readonly Stopwatch stopWatch = new Stopwatch();

  private DataSalon DataSalonToSend = new DataSalon();

  private List<DataBill> m_ListBills = new List<DataBill>();

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

  public MainForm () {
    AutoUpdater.DownloadPath = Environment.CurrentDirectory;
    InitializeComponent();
    if (_385_fisk.Properties.Settings.Default.TestBills.Length > 0) {
      numberOfFailedAttempts = serializer.Deserialize<Dictionary<string, int>>(_385_fisk.Properties.Settings.Default.TestBills);
    }
    TranslateContent();
  }

  private void TranslateContent () {
    Text = Translations.Translate(Text);
    grpInfo.Text = Translations.Translate(grpInfo.Text);
    lblFiskalisation.Text = Translations.Translate(lblFiskalisation.Text);
    lblExecutionTime.Text = Translations.Translate(lblExecutionTime.Text);
  }

  private void ErrorMessVerifData (string myMessage) {
    MessageAlert(Translations.Translate(myMessage), Translations.Translate("Greška"));
    Log.WriteLog(NumLog.MissingSetting, 0, "", placeholders, ErrorCode, ErrorMessage);
  }

  private void cis_SoapMessageSent (object sender, EventArgs e) {
    stopWatch.Stop();
    string text = Translations.Translate("Vrijeme izvršenja :") + string.Format("{0} s", stopWatch.Elapsed.TotalSeconds.ToString("0.00"));
    lblExecutionTime.Text = text;
    Application.DoEvents();
    stopWatch.Reset();
    Cursor.Current = Cursors.Default;
  }

  private void cis_SoapMessageSending (object sender, CentralniInformacijskiSustavEventArgs e) {
    Cursor.Current = Cursors.WaitCursor;
    stopWatch.Start();
    Application.DoEvents();
  }


  private decimal calculateTax(string totalBill, string taxRate)
    {
        decimal ammount=0;
        ammount = Convert.ToDecimal(totalBill) - Convert.ToDecimal(taxRate);
        return ammount;
    }

  private XmlDocument sendBill (DataBill DataBillToSend) {
    CultureInfo cultureInfo = new CultureInfo("hr-HR");
    RacunType racunType = new RacunType {
      Oib = DataBillToSend.VATNumber_Salon_Bill,
      USustPdv = DataBillToSend.TaxPayer_Bill,
      DatVrijeme = DataBillToSend.DateTimeIssue_Bill(DataBillToSend.BillDate_Bill),
      OznSlijed = DataBillToSend.SequenceMark_Bill
    };
    BrojRacunaType brojRacunaType2 = racunType.BrRac = new BrojRacunaType {
      BrOznRac = DataBillToSend.BillNumberMark_Bill,
      OznPosPr = DataBillToSend.PremiseMark_Bill,
      OznNapUr = DataBillToSend.BillingDeviceMark_Bill
    };
    racunType.IznosUkupno = ProperNumber(DataBillToSend.TotalAmount_Bill);
    racunType.NacinPlac = DataBillToSend.PaymentMethod_Bill;
    racunType.OibOper = DataBillToSend.CashierVATNumber_Bill;
    string notes = DataBillToSend.Notes;
    if (AppLink.InVATsystem == "1") {
      if (Convert.ToDecimal(DataBillToSend.TotalAmount_Bill) != decimal.Zero) {
        if (MerlinData.checkIfNewTaxes()) {
          List<DataNewTax> newTaxes = MerlinData.GetNewTaxes(DataBillToSend.IdTicket);
          foreach (DataNewTax item2 in newTaxes) {
                        decimal num = 0;
                        if (newTaxes.Count > 1)
                        {
                            num = Convert.ToDecimal(item2.TaxableAmount);
                        }
                        else num = calculateTax(DataBillToSend.TotalAmount_Bill, item2.TaxAmount);  //Convert.ToDecimal(item2.TaxableAmount);
                        decimal num2 = Convert.ToDecimal(item2.TaxAmount);
                        decimal num3 = Convert.ToDecimal(item2.TaxRate);
            racunType.Pdv.Add(new PorezType {
              Osnovica = ProperNumber(num.ToString("0.00")),
              Iznos = ProperNumber(num2.ToString("0.00")),
              Stopa = ProperNumber(num3.ToString("0.00"))
            });
          }
        } else {
          PorezType item = new PorezType {
            Stopa = ProperNumber(DataBillToSend.VATTaxRate_Bill),
            Osnovica = ProperNumber(DataBillToSend.VATBase_Bill),
            Iznos = ProperNumber(DataBillToSend.VATAmount_Bill)
          };
          racunType.Pdv.Add(item);
        }
      }
    } else {
            /*
      racunType.Pdv.Add(new PorezType {
        Osnovica = ProperNumber("0.00"),
        Iznos = ProperNumber("0.00"),
        Stopa = ProperNumber("0.00")
      });*/
    }
    if (notes.Length > 0) {
      int count = Regex.Matches(notes, "/", RegexOptions.IgnoreCase).Count;
      if (count > 1 && notes.Contains(";")) {
        string[] array = notes.Split(';', '\r', '\n');
        racunType.ParagonBrRac = array.GetValue(0).ToString();
      }
    }
    string text = (!(AppLink.UseCertificateFile == "1")) ? Razno.ZastitniKodIzracun(CertificateName, racunType.Oib, racunType.DatVrijeme.Replace('T', ' '), racunType.BrRac.BrOznRac, racunType.BrRac.OznPosPr, racunType.BrRac.OznNapUr, racunType.IznosUkupno.ToString()) : Razno.ZastitniKodIzracun(AppLink.DatotekaCertifikata(), AppLink.CertificatePassword, racunType.Oib, racunType.DatVrijeme.Replace('T', ' '), racunType.BrRac.BrOznRac, racunType.BrRac.OznPosPr, racunType.BrRac.OznNapUr, racunType.IznosUkupno.ToString());
    string text2 = DataBillToSend.Notes;
    bool flag = false;
    if (text2.Length == 0) {
      text2 = "ZKI: " + text;
      flag = true;
    } else if (!text2.Contains("ZKI:")) {
      text2 = text2 + "\r\nZKI: " + text;
      flag = true;
    }
    if (flag) {
      MerlinData.SaveNotes(DataBillToSend.IdTicket, text2);
    }
    racunType.ZastKod = text;
    racunType.NakDost = DataBillToSend.MarkSubseqBillDelivery_Bill;
    CentralniInformacijskiSustav centralniInformacijskiSustav = new CentralniInformacijskiSustav();
    centralniInformacijskiSustav.SoapMessageSending += cis_SoapMessageSending;
    centralniInformacijskiSustav.SoapMessageSent += cis_SoapMessageSent;
    XmlDocument xmlDocument = centralniInformacijskiSustav.PosaljiRacun(racunType, CertificateName);
    if (xmlDocument != null) {
      bool flag2 = Potpisivanje.ProvjeriPotpis(xmlDocument);
    } else {
      stopWatch.Stop();
    }
    return xmlDocument;
  }

  public XmlDocument sendBillCheck (DataBill dataBillToSend) {
    CultureInfo cultureInfo = new CultureInfo("hr-HR");
    RacunType racunType = new RacunType {
      Oib = dataBillToSend.VATNumber_Salon_Bill,
      USustPdv = dataBillToSend.TaxPayer_Bill,
      DatVrijeme = dataBillToSend.DateTimeIssue_Bill(dataBillToSend.BillDate_Bill),
      OznSlijed = dataBillToSend.SequenceMark_Bill
    };
    BrojRacunaType brojRacunaType2 = racunType.BrRac = new BrojRacunaType {
      BrOznRac = dataBillToSend.BillNumberMark_Bill,
      OznPosPr = dataBillToSend.PremiseMark_Bill,
      OznNapUr = dataBillToSend.BillingDeviceMark_Bill
    };
    racunType.IznosUkupno = ProperNumber(dataBillToSend.TotalAmount_Bill);
    racunType.NacinPlac = dataBillToSend.PaymentMethod_Bill;
    racunType.OibOper = dataBillToSend.CashierVATNumber_Bill;
    if (AppLink.InVATsystem == "1") {
      if (Convert.ToDecimal(dataBillToSend.TotalAmount_Bill) != decimal.Zero) {
        if (MerlinData.checkIfNewTaxes()) {
          List<DataNewTax> newTaxes = MerlinData.GetNewTaxes(dataBillToSend.IdTicket);
          foreach (DataNewTax item2 in newTaxes) {
                        decimal num = 0;
                        if (newTaxes.Count>1)
                        {
                            num = Convert.ToDecimal(item2.TaxableAmount);
                        }
                        else num = calculateTax(dataBillToSend.TotalAmount_Bill, item2.TaxAmount);  //Convert.ToDecimal(item2.TaxableAmount);
                        decimal num2 = Convert.ToDecimal(item2.TaxAmount);
                        decimal num3 = Convert.ToDecimal(item2.TaxRate);
            racunType.Pdv.Add(new PorezType {
              Osnovica = ProperNumber(num.ToString("0.00")),
              Iznos = ProperNumber(num2.ToString("0.00")),
              Stopa = ProperNumber(num3.ToString("0.00"))
            });
          }
        } else {
          PorezType item = new PorezType {
            Stopa = ProperNumber(dataBillToSend.VATTaxRate_Bill),
            Osnovica = ProperNumber(dataBillToSend.VATBase_Bill),
            Iznos = ProperNumber(dataBillToSend.VATAmount_Bill)
          };
          racunType.Pdv.Add(item);
        }
      }
    } else {
            /*
      racunType.Pdv.Add(new PorezType {
        Osnovica = ProperNumber("0.00"),
        Iznos = ProperNumber("0.00"),
        Stopa = ProperNumber("0.00")
      });*/
    }
    string notes = dataBillToSend.Notes;
    if (notes.Length > 0) {
      int count = Regex.Matches(notes, "/", RegexOptions.IgnoreCase).Count;
      if (count > 1 && notes.Contains(";")) {
        string[] array = notes.Split(';', '\r', '\n');
        racunType.ParagonBrRac = array.GetValue(0).ToString();
      }
    }
    string text = (!(AppLink.UseCertificateFile == "1")) ? Razno.ZastitniKodIzracun(CertificateName, racunType.Oib, racunType.DatVrijeme.Replace('T', ' '), racunType.BrRac.BrOznRac, racunType.BrRac.OznPosPr, racunType.BrRac.OznNapUr, racunType.IznosUkupno.ToString()) : Razno.ZastitniKodIzracun(AppLink.DatotekaCertifikata(), AppLink.CertificatePassword, racunType.Oib, racunType.DatVrijeme.Replace('T', ' '), racunType.BrRac.BrOznRac, racunType.BrRac.OznPosPr, racunType.BrRac.OznNapUr, racunType.IznosUkupno.ToString());
    string text2 = dataBillToSend.Notes;
    bool flag = false;
    if (text2.Length == 0) {
      text2 = "ZKI: " + text;
      flag = true;
    } else if (!text2.Contains("ZKI:")) {
      text2 = text2 + "\r\nZKI: " + text;
      flag = true;
    }
    if (flag) {
      MerlinData.SaveNotes(dataBillToSend.IdTicket, text2);
    }
    racunType.ZastKod = text;
    racunType.NakDost = dataBillToSend.MarkSubseqBillDelivery_Bill;
    racunType.Oib = DataSalonToSend.OIBSoftware;
    text = (racunType.ZastKod = ((!(AppLink.UseCertificateFile == "1")) ? Razno.ZastitniKodIzracun(CertificateName, racunType.Oib, racunType.DatVrijeme.Replace('T', ' '), racunType.BrRac.BrOznRac, racunType.BrRac.OznPosPr, racunType.BrRac.OznNapUr, racunType.IznosUkupno.ToString()) : Razno.ZastitniKodIzracun(AppLink.DatotekaCertifikata(), AppLink.CertificatePassword, racunType.Oib, racunType.DatVrijeme.Replace('T', ' '), racunType.BrRac.BrOznRac, racunType.BrRac.OznPosPr, racunType.BrRac.OznNapUr, racunType.IznosUkupno.ToString())));
    CentralniInformacijskiSustav centralniInformacijskiSustav = new CentralniInformacijskiSustav();
    centralniInformacijskiSustav.SoapMessageSending += cis_SoapMessageSending;
    centralniInformacijskiSustav.SoapMessageSent += cis_SoapMessageSent;
    return centralniInformacijskiSustav.PosaljiProvjeru(racunType);
  }

  public string MD5code (DataBill DataBillToSend) {
    CultureInfo cultureInfo = new CultureInfo("hr-HR");
    RacunType racunType = new RacunType {
      Oib = DataBillToSend.VATNumber_Salon_Bill,
      USustPdv = DataBillToSend.TaxPayer_Bill,
      DatVrijeme = DataBillToSend.DateTimeIssue_Bill(DataBillToSend.BillDate_Bill),
      OznSlijed = DataBillToSend.SequenceMark_Bill
    };
    BrojRacunaType brojRacunaType2 = racunType.BrRac = new BrojRacunaType {
      BrOznRac = DataBillToSend.BillNumberMark_Bill,
      OznPosPr = DataBillToSend.PremiseMark_Bill,
      OznNapUr = DataBillToSend.BillingDeviceMark_Bill
    };
    racunType.IznosUkupno = ProperNumber(DataBillToSend.TotalAmount_Bill);
    return Razno.ZastitniKodIzracun(CertificateName, racunType.Oib, racunType.DatVrijeme.Replace('T', ' '), racunType.BrRac.BrOznRac, racunType.BrRac.OznPosPr, racunType.BrRac.OznNapUr, racunType.IznosUkupno.ToString());
  }

  private static string ProperNumber (string myNumber) {
    string text = myNumber.Replace(" ", "");
    text = text.Replace("'", "");
    return text.Replace(",", ".");
  }

  private void Form1_Shown (object sender, EventArgs e) {
    Principal();
  }

  

  private void Principal () {
        LogFile.createSimpleLog();
        int num = 0;
        bool flag = false;
        progressBar1.Value = 0;


        if (!AppLink.TestFileCfg())
        {
            MessageAlert(Translations.Translate("Nedostaje konfiguracijska datoteka!"), Translations.Translate("Greška"), NumLog.CfgMissing, 0, "");
            Close();
        }
        
        /*
        if (AppLink.InVATsystem == "0" &&  MerlinData.CheckForWeirdTaxesWhenNotInVAT())
        {
            DialogResult dr = MessageBox.Show(Translations.Translate("Označena je postavka da je korisnik izvan sustava PDV-a no postoje uneseni porezne stope koje su različite od 0%. Molimo vas da ispravite porezne stope prije fiskaliziranja računa!\r\nŽelite li nastaviti?"), Translations.Translate("Greška"),MessageBoxButtons.YesNo);

            if(dr==DialogResult.No) Close();

        }
        */

            lblInfo.Text = Translations.Translate("Učitavanje podataka o poslovnom prostoru...");
            DataSalonToSend.VATNumber_Salon = AppLink.VATNumber;
            DataSalonToSend.LogFileIsActive = Convert.ToInt16(AppLink.LogFileActive);
            DataSalonToSend.DateIsActive = Convert.ToDateTime(AppLink.DateIsActive);
            DataSalonToSend.BillingDeviceMark = AppLink.BillingDeviceMark;
            DataSalonToSend.OIBSoftware = AppLink.OIBSoftware;
            Log.WriteLog(NumLog.AppLaunch, 0, "",placeholders,ErrorCode,ErrorMessage);
            CertificateName = AppLink.Certificate;
            int num2 = 0;

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
        if (CertificateName.Length > 1) {
          int num3 = Certifs.CriticalCertValidity(CertificateName);
          int criticalCertDays = _385_fisk.Properties.Settings.Default.CriticalCertDays;
          if (num3 <= 30) {
            if (num3 < 0) {
              MessageAlert("Vaš certifikat je istekao prije " + Math.Abs(num3) + " dana. Obnovite ga!", "Upozorenje");
              _385_fisk.Properties.Settings.Default.CriticalCertDays = num3;
              _385_fisk.Properties.Settings.Default.Save();
            } else if (num3 != criticalCertDays) {
              MessageAlert("Vaš certifikat ističe za " + num3 + " dana. Podsjetite se obnoviti ga!", "Upozorenje");
              _385_fisk.Properties.Settings.Default.CriticalCertDays = num3;
              _385_fisk.Properties.Settings.Default.Save();
            }
          }
        }
        switch (num2) {
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

            if (!Helper.DataVerification.VerifDataOk(DataSalonToSend, out vatActif,out errorMessage))
            {
                ErrorMessVerifData(errorMessage);
                Close();
            }
            else
            {
        lblInfo.Text = Translations.Translate("Obrada računa (korak 1)...");
        FlushBillsWithNoJir();
        if (!CisDal.MerlinData.GetBill(m_ListBills, DataSalonToSend.VATNumber_Salon, vatActif)) {
          MessageAlert(Translations.Translate("Problem tijekom obrade računa (korak 1).") + Environment.NewLine + Translations.Translate("Prekid obrade!") + Environment.NewLine + Translations.Translate("Kontaktirajte tehničku podršku!"), Translations.Translate("Kritična greška"), NumLog.PbBillProcSt1, 0, "");
          Close();
        } else {
          Log.WriteLog(NumLog.BillProcSt1ok, 0, "", placeholders, ErrorCode, ErrorMessage);
          if (m_ListBills.Count < 1) {
            Log.WriteLog(NumLog.NoBill, 0, "", placeholders, ErrorCode, ErrorMessage);
            if (!lastCheck.Equals(now.ToString("d.M.y"))) {
              _385_fisk.Properties.Settings.Default.LastCheck = now.ToString("d.M.y");
              _385_fisk.Properties.Settings.Default.Save();
              base.TopMost = false;
                        AutoUpdater.Start("https://www.dropbox.com/s/l86kf0sochnqnh6/CisUpdateList.xml?dl=1");
                        AutoUpdater.DownloadPath = Environment.CurrentDirectory;
                        AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
            } else {
              Close();
            }
          } else {
            num = progressBar1.Maximum / (m_ListBills.Count * 2);
            lblInfo.Text = Translations.Translate("Obrada računa (korak 2)...");
            Log.WriteLog(NumLog.StartBillProcSt2, 0, "",placeholders,ErrorCode,ErrorMessage);
            foreach (DataBill listBill in m_ListBills) {
              flag = false;
              if (!MerlinData.GetBillFollow(listBill)) {
                MessageAlert(Translations.Translate("Problem tijekom obrade računa (korak 2).") + Environment.NewLine + Translations.Translate("Prekid obrade!") + Environment.NewLine + Translations.Translate("Kontaktirajte tehničku podršku!"), Translations.Translate("Greška"), NumLog.PbBillProcSt2, listBill.IdTicket, "");
                                SaveErrorOnBill(listBill.IdTicket, "ERROR_JIR:Nije fiskalizirano");
                                continue;
              }
              if (listBill.Notes.Contains("ZKI:") && listBill.HashStatus.Length != 36) {
                listBill.MarkSubseqBillDelivery_Bill = true;
              } else {
                listBill.MarkSubseqBillDelivery_Bill = false;
              }
              if (listBill.CashierVATNumber_Bill == "OIBPERSOERROR") {
                MessageAlert(Translations.Translate("Nedostaje OIB zaposlenika!"), Translations.Translate("Greška"), NumLog.MissingOIBEmp, 0, "");
                flag = true;
                //SaveErrorOnBill(listBill.IdTicket, "ERROR_sERR");
                SaveErrorOnBill(listBill.IdTicket, "ERROR_JIR:Nije fiskalizirano");
              }
              if (flag) {
                continue;
              }
              try {
                lblInfo.Text = Translations.Translate("Povezivanje s uslugom CIS, molimo pričekajte...)");
                progressBar1.Value += num;
              } catch (Exception ex) {
                SimpleLog.Log(ex);
                if (ex.Message.ToLowerInvariant().Contains("network password")) {
                  MessageAlert(Translations.Translate("Lozinka certifikata nije ispravna!"), Translations.Translate("Greška"));
                } else {
                  MessageAlert(Translations.Translate("Trenutno nije moguće spajanje na CIS, samo nastavite s izdavanjem računa klikom na OK") + Environment.NewLine + Translations.Translate("Kontaktirajte tehničku podršku!"), Translations.Translate("Greška"));
                }
                Log.WriteLog(NumLog.ErrorHttp2, 0, ex.Message, placeholders, ErrorCode, ErrorMessage);
                                SaveErrorOnBill(listBill.IdTicket, "ERROR_JIR:Nije fiskalizirano");
                            }
              string sendTestReceipts = AppLink.SendTestReceipts;
              if (sendTestReceipts.Equals("1")) {
                if (numberOfFailedAttempts.ContainsKey(listBill.IdTicket.ToString())) {
                  int num4 = numberOfFailedAttempts[listBill.IdTicket.ToString()];
                  if (num4 >= 3) {
                    numberOfFailedAttempts.Remove(listBill.IdTicket.ToString());
                    _385_fisk.Properties.Settings.Default.TestBills = serializer.Serialize(numberOfFailedAttempts);
                    _385_fisk.Properties.Settings.Default.Save();
                    goto IL_0b42;
                  }
                }
                try {
                  Log.WriteLog(NumLog.SendingTest, listBill.IdTicket, "", placeholders, ErrorCode, ErrorMessage);
                  lblInfo.Text = Translations.Translate("Slanje zahtjeva za provjerom racuna, molimo pričekajte...");
                  XmlDocument xmlDocument = sendBillCheck(listBill);
                  if (xmlDocument == null) {
                    throw new Exception(Translations.Translate("Nije primljen odgovor od CIS-a tokom slanja testnog računa"));
                  }
                  Tuple<string, string> tuple = XmlDokumenti.DohvatiStatusProvjere(xmlDocument);
                  if (tuple == null) {
                    goto IL_0a73;
                  }
                  if (tuple.Item1.Equals("v100")) {
                    Log.WriteLog(NumLog.TestSuccess, 0, "", placeholders, ErrorCode, ErrorMessage);
                    goto IL_0a73;
                  }
                  if (numberOfFailedAttempts.ContainsKey(listBill.IdTicket.ToString())) {
                    int num5 = numberOfFailedAttempts[listBill.IdTicket.ToString()];
                    num5++;
                    numberOfFailedAttempts[listBill.IdTicket.ToString()] = num5;
                  } else {
                    numberOfFailedAttempts.Add(listBill.IdTicket.ToString(), 1);
                  }
                  _385_fisk.Properties.Settings.Default.TestBills = serializer.Serialize(numberOfFailedAttempts);
                  _385_fisk.Properties.Settings.Default.Save();
                  ErrorCode = tuple.Item1;
                  ErrorMessage = Translations.Translate(tuple.Item2);
                  MessageAlert(ErrorCode + " - " + ErrorMessage, Translations.Translate("Greška"));
                  SimpleLog.Warning("Za račun: *" + listBill.IdTicket + "* " + ErrorCode + " - " + ErrorMessage);
                  Log.WriteLog(NumLog.TestFailed, 0, "", placeholders, ErrorCode, ErrorMessage);
                  goto end_IL_088e;
                  IL_0a73:
                  Log.WriteLog(NumLog.SendBillOK, 0, "", placeholders, ErrorCode, ErrorMessage);
                  goto IL_0b42;
                  end_IL_088e:;
                } catch (Exception ex2) {
                  if (ex2.Message.ToLowerInvariant().Contains("network password")) {
                    MessageAlert(Translations.Translate("Lozinka certifikata nije ispravna!"), Translations.Translate("Greška"));
                  } else {
                    if (ex2.Message.ToLowerInvariant().Contains("error occurred on a send")) {
                      goto IL_0b42;
                    }
                    MessageAlert(Translations.Translate("Trenutno nije moguće spajanje na CIS, samo nastavite s izdavanjem računa klikom na OK") + Environment.NewLine + Translations.Translate("Kontaktirajte tehničku podršku!"), Translations.Translate("Greška"));
                                        SaveErrorOnBill(listBill.IdTicket, "ERROR_JIR:Nije fiskalizirano");
                                    }
                  Log.WriteLog(NumLog.ErrorHttp, listBill.IdTicket, ex2.Message,placeholders,ErrorCode,ErrorMessage);
                  SimpleLog.Log(ex2);
                }
                continue;
              }
              goto IL_0b42;
              IL_0b42:
              try {
                                Log.WriteLog(NumLog.SendBill, listBill.IdTicket, "", placeholders, ErrorCode, ErrorMessage);
                lblInfo.Text = Translations.Translate("Povezivanje s uslugom CIS, molimo pričekajte...");
                XmlDocument xmlDocument2 = sendBill(listBill);
                if (xmlDocument2 != null) {
                  Tuple<string, string> tuple2 = XmlDokumenti.DohvatiStatusGreške(xmlDocument2);
                  if (tuple2 != null && !tuple2.Item1.Equals("v100")) {
                    ErrorCode = tuple2.Item1;
                    ErrorMessage = Translations.Translate(tuple2.Item2);
                    MessageAlert(ErrorCode + " - " + ErrorMessage, Translations.Translate("Greška"));
                    SimpleLog.Warning("Za račun: *" + listBill.IdTicket + "* " + ErrorCode + " - " + ErrorMessage);
                                        Log.WriteLog(NumLog.TestFailed, 0, "", placeholders, ErrorCode, ErrorMessage);
                    continue;
                  }
                  txtResponse.Text = xmlDocument2.OuterXml;
                  progressBar1.Value += num;
                                    Log.WriteLog(NumLog.SendBillOK, 0, "", placeholders, ErrorCode, ErrorMessage);
                }
              } catch (Exception ex3) {
                SimpleLog.Log(ex3);
                if (ex3.Message.ToLowerInvariant().Contains("network password")) {
                  MessageAlert(Translations.Translate("Lozinka certifikata nije ispravna!"), Translations.Translate("Greška"));
                } else {
                  MessageAlert(Translations.Translate("Trenutno nije moguće spajanje na CIS, samo nastavite s izdavanjem računa klikom na OK") + Environment.NewLine + Translations.Translate("Kontaktirajte tehničku podršku!"), Translations.Translate("Greška"));
                }
                                Log.WriteLog(NumLog.ErrorHttp, listBill.IdTicket, ex3.Message, placeholders, ErrorCode, ErrorMessage);
                                SaveErrorOnBill(listBill.IdTicket, "ERROR_JIR:Nije fiskalizirano");
                                continue;
              }
              if (string.IsNullOrEmpty(txtResponse.Text)) {
                                Log.WriteLog(NumLog.EmptyXMLResponse, 0, "", placeholders, ErrorCode, ErrorMessage);
                                SaveErrorOnBill(listBill.IdTicket, "ERROR_JIR:Nije fiskalizirano");
                            } else if (VerifAndSaveJir(txtResponse.Text, listBill.IdTicket, "Jir", State: false)) {
                                Log.WriteLog(NumLog.NextBill, 0, "", placeholders, ErrorCode, ErrorMessage);
              } else if (VerifAndSaveJir(txtResponse.Text, listBill.IdTicket, "SifraGreske", State: true)) {
                                Log.WriteLog(NumLog.NextBill, 0, "", placeholders, ErrorCode, ErrorMessage);
              } else {
                                SaveErrorOnBill(listBill.IdTicket, "ERROR_JIR:Nije fiskalizirano");
                            }
            }
            FlushBillsWithNoJir();
            lblInfo.Text = Translations.Translate("Zatvaranje aplikacije");
            if (!lastCheck.Equals(now.ToString("d.M.y"))) {
                        _385_fisk.Properties.Settings.Default.LastCheck = now.ToString("d.M.y");
                        _385_fisk.Properties.Settings.Default.Save();
                        base.TopMost = false;
                        AutoUpdater.Start("https://www.dropbox.com/s/l86kf0sochnqnh6/CisUpdateList.xml?dl=1");
                        AutoUpdater.DownloadPath = Environment.CurrentDirectory;
                        AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
            } else {
              Close();
            }
          }
        }
      }
    

  }

  private void AutoUpdaterOnCheckForUpdateEvent (UpdateInfoEventArgs args) {
    if (args != null) {
      if (args.IsUpdateAvailable) {
        if (MessageBox.Show("Postoji nova verzija aplikacije za fiskalizaciju. Želite li ažurirati aplikaciju sada?", "Ažuriranje dostupno", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk).Equals(DialogResult.Yes)) {
          try {
            if (AutoUpdater.DownloadUpdate()) {
              Close();
            }
          } catch (Exception ex) {
            SimpleLog.Log(ex);
            MessageAlert(ex.Message + Environment.NewLine + Translations.Translate("Kontaktirajte tehničku podršku!"), Translations.Translate("Greška"));
            Close();
          }
        } else {
          Close();
        }
      } else {
        Close();
      }
    } else {
      Close();
    }
  }

  private void SaveErrorOnBill (int Idticket, string m_Text) {
    MerlinData.SaveTicketJir(Idticket, m_Text);
    Log.WriteLog(NumLog.BillUpdateOK, Idticket, m_Text, placeholders, ErrorCode, ErrorMessage);
  }

  public bool VerifAndSaveJir (string XmlText, int IDticket, string XmlTag, bool State) {
    string text = "";
    bool flag = false;
    if (!State) {
      Log.WriteLog(NumLog.StartSearchJir, 0, "", placeholders, ErrorCode, ErrorMessage);
    } else {
      Log.WriteLog(NumLog.StartSearchError, 0, "", placeholders, ErrorCode, ErrorMessage);
    }
    try {
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.LoadXml(XmlText);
      XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName(XmlTag, "http://www.apis-it.hr/fin/2012/types/f73");
      string text2 = "";
      if (elementsByTagName.Count != 1) {
        flag = false;
        Log.WriteLog(NumLog.JirNodeNotFount, 0, "", placeholders, ErrorCode, ErrorMessage);
        return false;
      }
      IEnumerator enumerator = elementsByTagName.GetEnumerator();
      try {
        if (enumerator.MoveNext()) {
          XmlNode xmlNode = (XmlNode) enumerator.Current;
          if (xmlNode.FirstChild == null) {
            flag = false;
            Log.WriteLog(NumLog.NodeJirIsNull, 0, "", placeholders, ErrorCode, ErrorMessage);
          } else {
            text2 = xmlNode.FirstChild.Value;
            if (State) {
              text = "ERROR_" + text2;
              flag = true;
            } else {
              text = text2;
              flag = true;
            }
          }
        }
      } finally {
        IDisposable disposable = enumerator as IDisposable;
        if (disposable != null) {
          disposable.Dispose();
        }
      }
      if (string.IsNullOrEmpty(text2)) {
        flag = false;
        Log.WriteLog(NumLog.JirIsNull, 0, "",placeholders,ErrorCode, ErrorMessage);
      }
    } catch (Exception ex) {
      SimpleLog.Log(ex);
      MessageAlert(ex.Message + Environment.NewLine + Translations.Translate("Kontaktirajte tehničku podršku!"), Translations.Translate("Greška"));
      Log.WriteLog(NumLog.XMLerror, IDticket, ex.Message, placeholders, ErrorCode, ErrorMessage);
      flag = false;
    }
    if (!flag) {
      return false;
    }
    MerlinData.SaveTicketJir(IDticket, text);
    Log.WriteLog(NumLog.BillUpdateOK, IDticket, text,placeholders, ErrorCode, ErrorMessage);
    if (State) {
      string str = Environment.NewLine + Translations.Translate("Kontaktirajte tehničku podršku!");
      switch (text) {
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

  

  private void MessageAlert (string content, string title, NumLog logerror=NumLog.None,int ticket=1,string extendmessage="") {
        MessageBox.Show(content, title, MessageBoxButtons.OK, MessageBoxIcon.Hand);
        Log.WriteLog(logerror, ticket, extendmessage, placeholders, ErrorCode, ErrorMessage);
  }

  private void Form1_Shown_1 (object sender, EventArgs e) {
    if (AppLink.TestFileCfg()) {
      IsSoloMode();
      Principal();
    } else {
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

  private void IsSoloMode () {
    if (AppLink.OperatorOIB.Length == 11 && AppLink.OperatorCode.Length > 0) {
      string operatorOIB = AppLink.OperatorOIB;
      string operatorCode = AppLink.OperatorCode;
      string text = "";
      List<DataPerso> list = new List<DataPerso>();
      bool perso = MerlinData.GetPerso(list);
      foreach (DataPerso item in list) {
        if (operatorOIB != item.numeroSecu || operatorCode != item.codePerso) {
          text = text + "UPDATE perso set numerosecu='" + operatorOIB + "', code='" + operatorCode + "' where id=" + item.idPerso + "\r\n";
        }
      }
      if (text.Length > 0) {
        MerlinData.UpdatePerso(text);
      }
    }
  }

  private void FlushBillsWithNoJir () {
    MerlinData.FlushBillsWithNoJir();
  }


  protected override void Dispose (bool disposing) {
    if (disposing && components != null) {
      components.Dispose();
    }
    base.Dispose(disposing);
  }

  private void InitializeComponent () {
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

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        AutoUpdater.Start("https://www.dropbox.com/s/l86kf0sochnqnh6/CisUpdateList.xml?dl=1");
        AutoUpdater.DownloadPath = Environment.CurrentDirectory;
    }
}
