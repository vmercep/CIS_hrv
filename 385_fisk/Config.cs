using _385_fisk.Exceptions;
using AutoUpdaterDotNET;
using CisBl;
using Helper;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

public class Config : Form {
    public bool runNormalMode = false;

    private bool state = false;

    private readonly Stopwatch stopWatch = new Stopwatch();

    private string folderPath = string.Empty;

    private ToolTip tooltip1 = new ToolTip();

    private IContainer components = null;

    private Button btnSaveAndQuit;

    private Label lblConnectionString;

    private TextBox txtConnectString;

    private TextBox txtURL;

    private Label lblURL;

    private CheckBox chkLogFileActive;

    private DateTimePicker dtpDateActive;

    private Label lblDateActive;

    private GroupBox grpBasic;

    private GroupBox grpPremise;

    private Label lblOibSoftware;

    private TextBox txtOibSoftware;

    private Label lblBillingDeviceMark;

    private TextBox txtBillingDeviceMark;

    private Label lblPremiseMark;

    private TextBox txtPremiseMark;

    private CheckBox chkVatActive;

    private Label lblOIB;

    private TextBox txtVatNumber;

    private GroupBox grpSolo;

    private Label lblCodeOperator;

    private TextBox txtCodeOperator;

    private Label lblOibOperator;

    private TextBox txtOibOperator;

    private Button btnUnlockSettings;

    private PictureBox pictureBox1;

    private Button btnQuit;

    private GroupBox grpCertificate;

    private Label lblCertificate;

    private TextBox txtCertificate;

    private Button btnXmlPathChoose;

    private Label lblXmlPath;

    private CheckBox chkSaveXmlActive;

    private ComboBox cmbConnectionEncryption;

    private Label lblConnectionEncryption;

    private Label lblErrorMessage;

    private Label lblActiveLanguage;

    private ComboBox cmbActiveLanguage;

    private GroupBox grpExtraOptions;

    private CheckBox chkSendTest;

    private Button btnProcessBills;

    private GroupBox grpCertificateFile;

    private CheckBox chkUseCertificateFile;

    private Label lbCertLocation;

    private Button btnCertificateFile;

    private Label lbCertPassword;
    private CheckBox cbCheckSSL;
    private Button btUpdate;
    private Label lbVerzija;
    private CheckBox cbFiskPonude;
    private TextBox txtQrMessage;
    private Label lbQrMessage;
    private GroupBox grpQrCode;
    private TextBox txtQrSaveLocation;
    private Label lbQrSaveLocation;
    private TextBox txtQrSize;
    private Label lbQrSize;
    private ComboBox cmbLogLevel;
    private Label lbloglevel;
    private Button btQrRegen;
    private ProgressBar pgBarQr;
    private TextBox txtCertificatePassword;

    QrCodeRegen regen = new QrCodeRegen();
    private TabControl tabControl;
    private TabPage tabOsnovno;
    private TabPage tabPostavke;
    private TabPage tabSolo;
    private TabPage tabExtraOpcije;
    private TabPage tabCertifikati;
    private TabPage tabQr;
    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


    public Config()
    {
        InitializeComponent();
    }

    private void populateTranslations()
    {
        Text = Translations.Translate(Text);
        tooltip1.SetToolTip(btnUnlockSettings, Translations.Translate("Zaštićene postavke"));
        tooltip1.SetToolTip(btnProcessBills, Translations.Translate("Pokreni fiskalizaciju"));
        tooltip1.SetToolTip(btnSaveAndQuit, Translations.Translate("Spremi / Izađi"));
        tooltip1.SetToolTip(btnQuit, Translations.Translate("Izađi bez spremanja"));
        tooltip1.SetToolTip(btUpdate, Translations.Translate("Preuzmi najnoviju verziju CIS-a"));
        lblErrorMessage.Text = Translations.Translate(lblErrorMessage.Text);
        grpBasic.Text = Translations.Translate(grpBasic.Text);
        chkSaveXmlActive.Text = Translations.Translate(chkSaveXmlActive.Text);
        chkLogFileActive.Text = Translations.Translate(chkLogFileActive.Text);
        lblXmlPath.Text = Translations.Translate(lblXmlPath.Text);
        btnXmlPathChoose.Text = Translations.Translate(btnXmlPathChoose.Text);
        lblDateActive.Text = Translations.Translate(lblDateActive.Text);
        lblActiveLanguage.Text = Translations.Translate(lblActiveLanguage.Text);
        grpPremise.Text = Translations.Translate(grpPremise.Text);
        lblOIB.Text = Translations.Translate(lblOIB.Text);
        chkVatActive.Text = Translations.Translate(chkVatActive.Text);
        lblPremiseMark.Text = Translations.Translate(lblPremiseMark.Text);
        lblBillingDeviceMark.Text = Translations.Translate(lblBillingDeviceMark.Text);
        lblOibSoftware.Text = Translations.Translate(lblOibSoftware.Text);
        grpSolo.Text = Translations.Translate(grpSolo.Text);
        lblOibOperator.Text = Translations.Translate(lblOibOperator.Text);
        lblCodeOperator.Text = Translations.Translate(lblCodeOperator.Text);
        grpCertificate.Text = Translations.Translate(grpCertificate.Text);
        lblCertificate.Text = Translations.Translate(lblCertificate.Text);
        grpExtraOptions.Text = Translations.Translate(grpExtraOptions.Text);
        chkSendTest.Text = Translations.Translate(chkSendTest.Text);

        //tab translate
        tabOsnovno.Text= Translations.Translate(grpBasic.Text);
        tabSolo.Text = Translations.Translate(grpSolo.Text);
        tabPostavke.Text = Translations.Translate(grpPremise.Text);
        tabExtraOpcije.Text= Translations.Translate("Dodatne opcije:");
        tabCertifikati.Text= Translations.Translate(grpCertificate.Text);
        

        //extras missing translation
        grpExtraOptions.Text = Translations.Translate("Dodatne opcije:");
        cbFiskPonude.Text= Translations.Translate("Fiskaliziraj ponude");
        grpCertificateFile.Text= Translations.Translate("Certifikat iz datoteke :");
        chkUseCertificateFile.Text = Translations.Translate("Koristi certifikat iz datoteke:");
        lbCertLocation.Text = Translations.Translate("Putanja datoteke s certifikatom :");
        lbCertPassword.Text= Translations.Translate("Lozinka certifikata :");
        btnCertificateFile.Text= Translations.Translate("Odabir");
        cbCheckSSL.Text= Translations.Translate("Ignoriraj SSL certifikate");

        lbQrMessage.Text = Translations.Translate("QR poruka");
        lbQrSaveLocation.Text = Translations.Translate("QR lokacija spremanja");
        lbQrSize.Text = Translations.Translate("QR Velicina");
        btQrRegen.Text = Translations.Translate("QR generiranje");
        grpQrCode.Text= Translations.Translate("QR postavke");
        lblConnectionString.Text= Translations.Translate("Poveznica s Merlin bazom");
        lblConnectionEncryption.Text = Translations.Translate("Connection encryption");
        lbloglevel.Text = Translations.Translate("Log level");
        lblURL.Text= Translations.Translate("Server URL");
        lblDateActive.Text = Translations.Translate("Date active");







        lbVerzija.Text = Translations.Translate("Verzija programa: CIS_HRV_ ") + Assembly.GetEntryAssembly().GetName().Version;

    }

    private void btnSaveAndQuitClick(object sender, EventArgs e)
    {
        if (saveConfig())
        {
            Application.Exit();
        }
    }

    private void verifyData()
    {
        state = false;
        lblErrorMessage.Visible = false;
        if (txtConnectString.TextLength < 52 || txtConnectString.Text.Contains(":"))
        {
            displayFieldError(txtConnectString, error: true);
        }
        else
        {
            displayFieldError(txtConnectString, error: false);
        }
        if (txtURL.TextLength < 54)
        {
            displayFieldError(txtURL, error: true);
        }
        else
        {
            displayFieldError(txtURL, error: false);
        }
        if (txtVatNumber.TextLength < 1 || txtVatNumber.TextLength > 11)
        {
            displayFieldError(txtVatNumber, error: true);
        }
        else
        {
            displayFieldError(txtVatNumber, error: false);
        }
        if (txtPremiseMark.TextLength < 1)
        {
            displayFieldError(txtPremiseMark, error: true);
        }
        else
        {
            displayFieldError(txtPremiseMark, error: false);
        }
        if (txtBillingDeviceMark.TextLength < 1)
        {
            displayFieldError(txtBillingDeviceMark, error: true);
        }
        else
        {
            displayFieldError(txtBillingDeviceMark, error: false);
        }
        if (txtOibSoftware.TextLength < 1)
        {
            displayFieldError(txtOibSoftware, error: true);
        }
        else
        {
            displayFieldError(txtOibSoftware, error: false);
        }
        if (chkUseCertificateFile.CheckState == CheckState.Unchecked)
        {
            if (txtCertificate.TextLength < 1)
            {
                displayFieldError(txtCertificate, error: true);
            }
            else
            {
                displayFieldError(txtCertificate, error: false);
            }
            displayFieldError(txtCertificatePassword, error: false);
        }
        if (txtVatNumber.TextLength != 11)
        {
            displayFieldError(txtVatNumber, error: true);
        }
        else
        {
            displayFieldError(txtVatNumber, error: false);
        }

        if (txtOibSoftware.TextLength != 11)
        {
            displayFieldError(txtOibSoftware, error: true);
        }
        else
        {
            displayFieldError(txtOibSoftware, error: false);
        }
        if (txtOibOperator.TextLength > 0)
        {
            if (txtOibOperator.TextLength != 11)
            {
                displayFieldError(txtOibOperator, error: true);
            }
            else
            {
                displayFieldError(txtOibOperator, error: false);
            }
        }
        else
        {
            displayFieldError(txtOibOperator, error: false);
        }
        if (chkUseCertificateFile.CheckState == CheckState.Checked && txtCertificatePassword.TextLength < 1)
        {
            displayFieldError(txtCertificatePassword, error: true);
            displayFieldError(txtCertificate, error: false);
        }
    }

    private void displayFieldError(Control ctrl, bool error)
    {
        if (error)
        {
            ctrl.BackColor = Color.LightCoral;
            state = true;
            lblErrorMessage.Visible = true;
        }
        else
        {
            ctrl.BackColor = Color.White;
        }
    }

    private void Config_Load(object sender, EventArgs e)
    {
        log.Debug("Config screen loading!");
        txtConnectString.Text = AppLink.ConnectionString;
        string connectionEncryption = AppLink.ConnectionEncryption;
        if (!string.IsNullOrEmpty(connectionEncryption))
        {
            cmbConnectionEncryption.SelectedItem = connectionEncryption;
        }
        else
        {
            cmbConnectionEncryption.SelectedItem = "TLS 1.2";
        }
        txtURL.Text = AppLink.URL;
        string logFileActive = AppLink.LogFileActive;
        string a = logFileActive;
        if (!(a == "0"))
        {
            if (a == "1")
            {
                chkLogFileActive.CheckState = CheckState.Checked;
            }
            else
            {
                chkLogFileActive.CheckState = CheckState.Checked;
            }
        }
        else
        {
            chkLogFileActive.CheckState = CheckState.Unchecked;
        }
        string saveXMLActive = AppLink.SaveXMLActive;
        string a2 = saveXMLActive;
        if (!(a2 == "0"))
        {
            if (a2 == "1")
            {
                chkSaveXmlActive.CheckState = CheckState.Checked;
            }
            else
            {
                chkSaveXmlActive.CheckState = CheckState.Checked;
            }
        }
        else
        {
            chkSaveXmlActive.CheckState = CheckState.Unchecked;
        }
        folderPath = AppLink.XMLSavePath;
        string dateIsActive = AppLink.DateIsActive;
        DateTime value = (!string.IsNullOrEmpty(dateIsActive)) ? Convert.ToDateTime(dateIsActive) : DateTime.Now;
        dtpDateActive.Value = value;
        string activeLanguage = AppLink.ActiveLanguage;
        if (!string.IsNullOrEmpty(activeLanguage))
        {
            cmbActiveLanguage.SelectedItem = activeLanguage;
        }
        else
        {
            cmbActiveLanguage.SelectedItem = "hr";
        }
        txtVatNumber.Text = AppLink.VATNumber;
        string inVATsystem = AppLink.InVATsystem;
        string a3 = inVATsystem;
        if (!(a3 == "0"))
        {
            if (a3 == "1")
            {
                chkVatActive.CheckState = CheckState.Checked;
            }
            else
            {
                chkVatActive.CheckState = CheckState.Checked;
            }
        }
        else
        {
            chkVatActive.CheckState = CheckState.Unchecked;
        }
        string pCeasedOperation = AppLink.PCeasedOperation;
        string a4 = pCeasedOperation;
        if (!(a4 == "0"))
        {
            if (a4 == "1")
            {
                //chkPCeasedOperation.CheckState = CheckState.Checked;
            }
            else
            {
                //chkPCeasedOperation.CheckState = CheckState.Unchecked;
            }
        }
        else
        {
            //chkPCeasedOperation.CheckState = CheckState.Unchecked;
        }
        txtPremiseMark.Text = AppLink.PremiseMark;
        txtBillingDeviceMark.Text = AppLink.BillingDeviceMark;
        txtOibSoftware.Text = AppLink.OIBSoftware;
        txtOibOperator.Text = AppLink.OperatorOIB;
        txtCodeOperator.Text = AppLink.OperatorCode;
        txtCertificate.Text = AppLink.Certificate;
        string sendTestReceipts = AppLink.SendTestReceipts;
        string a5 = sendTestReceipts;
        if (!(a5 == "0"))
        {
            if (a5 == "1")
            {
                chkSendTest.CheckState = CheckState.Checked;
            }
            else
            {
                chkSendTest.CheckState = CheckState.Unchecked;
            }
        }
        else
        {
            chkSendTest.CheckState = CheckState.Unchecked;
        }
        string useCertificateFile = AppLink.UseCertificateFile;
        string a6 = useCertificateFile;
        if (!(a6 == "0"))
        {
            if (a6 == "1")
            {
                chkUseCertificateFile.CheckState = CheckState.Checked;
            }
            else
            {
                chkUseCertificateFile.CheckState = CheckState.Unchecked;
            }
        }
        else
        {
            chkUseCertificateFile.CheckState = CheckState.Unchecked;
        }
        txtCertificatePassword.Text = AppLink.CertificatePassword;
        txtConnectString.BackColor = Color.Gainsboro;
        txtURL.BackColor = Color.Gainsboro;
        txtOibOperator.BackColor = Color.Gainsboro;
        txtCodeOperator.BackColor = Color.Gainsboro;
        txtCertificate.BackColor = Color.Gainsboro;
        tooltip1.AutoPopDelay = 5000;
        tooltip1.InitialDelay = 1000;
        tooltip1.ReshowDelay = 500;
        tooltip1.ShowAlways = true;
        txtQrMessage.Text = AppLink.QrCodeMessage;
        txtQrSize.Text = AppLink.QrCodeSize;
        txtQrSaveLocation.Text = AppLink.QrCodeLocation;
        populateTranslations();

        string useSSLValidation = AppLink.IgnoreSSLCertificates;
        if (!(useSSLValidation == "0"))
        {
            if (useSSLValidation == "1")
            {
                cbCheckSSL.CheckState = CheckState.Checked;
            }
            else
            {
                cbCheckSSL.CheckState = CheckState.Unchecked;
            }
        }
        else
        {
            cbCheckSSL.CheckState = CheckState.Unchecked;
        }

        string sendPonudaToFisk = AppLink.SendPonudaToFisk;
        if (!(sendPonudaToFisk == "0"))
        {
            if (sendPonudaToFisk == "1")
            {
                cbFiskPonude.CheckState = CheckState.Checked;
            }
            else
            {
                cbFiskPonude.CheckState = CheckState.Unchecked;
            }
        }
        else
        {
            cbFiskPonude.CheckState = CheckState.Unchecked;
        }


        string logLevel = AppLink.LogLevel;
        if (!string.IsNullOrEmpty(logLevel))
        {
            cmbLogLevel.SelectedItem = logLevel;
        }
        else
        {
            cmbLogLevel.SelectedItem = "INFO";
        }

    }



    public static string inputBox (string prompt, string title, string defaultValue) 
    {
        InputBoxDialog inputBoxDialog = new InputBoxDialog();
        inputBoxDialog.FormPrompt = prompt;
        inputBoxDialog.FormCaption = title;
        inputBoxDialog.DefaultValue = defaultValue;
        inputBoxDialog.ShowDialog();
        string inputResponse = inputBoxDialog.InputResponse;
        inputBoxDialog.Close();
        return inputResponse;
    }

    private void btnUnlockSettingClick (object sender, EventArgs e) 
    {
        log.Debug("Unlock setting screen!");
        string text = inputBox(Translations.Translate("Unesite tehničku šifru"), Translations.Translate("Tehnička šifra"), "");
        string techCode = AppLink.GetTechCode();

        if (text == techCode) 
        {

            log.Debug("Settings unlocked!");          
            dtpDateActive.Enabled = true;            
            cmbActiveLanguage.Enabled = true;
            chkVatActive.Enabled = true;
            txtConnectString.Enabled = true;
            cmbConnectionEncryption.Enabled = true; 
            btnXmlPathChoose.Enabled = true;            
            txtURL.Enabled = true;            
            chkLogFileActive.Enabled = true;            
            chkSaveXmlActive.Enabled = true;            
            txtOibOperator.Enabled = true;            
            txtCodeOperator.Enabled = true;            
            txtVatNumber.Enabled = true;          
            txtPremiseMark.Enabled = true;            
            txtBillingDeviceMark.Enabled = true;            
            txtOibSoftware.Enabled = true;            
            txtCertificate.Enabled = true;            
            chkUseCertificateFile.Enabled = true;            
            btnCertificateFile.Enabled = true;            
            txtCertificatePassword.Enabled = true;            
            cbCheckSSL.Enabled = true;            
            txtCertificatePassword.BackColor = Color.White;            
            chkSendTest.Enabled = true;            
            txtConnectString.BackColor = Color.White;            
            txtURL.BackColor = Color.White;            
            txtOibOperator.BackColor = Color.White;            
            txtCodeOperator.BackColor = Color.White;            
            txtVatNumber.BackColor = Color.White;           
            txtPremiseMark.BackColor = Color.White;            
            txtBillingDeviceMark.BackColor = Color.White;            
            txtOibSoftware.BackColor = Color.White;            
            txtCertificate.BackColor = Color.White;
            txtQrMessage.Enabled = true;            
            txtQrMessage.BackColor = Color.White;            
            txtQrSaveLocation.Enabled = true;           
            txtQrSaveLocation.BackColor = Color.White;            
            txtQrSize.Enabled = true;            
            txtQrSize.BackColor = Color.White;
            cbFiskPonude.Enabled = true;                    
            cmbLogLevel.Enabled = true;
            btQrRegen.Enabled = true;
        } 
        else if (text.Length > 0) 
        {
                log.Debug("Wrong password for setting screen entered!");
                MessageBox.Show(Translations.Translate("Pogrešna šifra!"), Translations.Translate("Greška"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
    
        }
  }
   
    private void cis_SoapMessageSending (object sender, CentralniInformacijskiSustavEventArgs e) 
    {
        Cursor.Current = Cursors.WaitCursor;
        stopWatch.Start(); 
        pictureBox1.Visible = true;
        Application.DoEvents();
    }

  
    private void cis_SoapMessageSent (object sender, EventArgs e) 
    {
        stopWatch.Stop();
        pictureBox1.Visible = false;
        btnSaveAndQuit.Enabled = true;
        btnUnlockSettings.Enabled = true;
        //btnSendData.Enabled = true;
        Application.DoEvents();
        stopWatch.Reset();
        Cursor.Current = Cursors.Default;

    }

    private void btnQuitClick(object sender, EventArgs e)
    {
        Environment.Exit(0);
    }

    private void btnChooseClick(object sender, EventArgs e)
    {
        btnUnlockSettings.Enabled = false;
        btnQuit.Enabled = false;
        btnSaveAndQuit.Enabled = false;
        btnXmlPathChoose.Enabled = false;
        FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
        if (!string.IsNullOrEmpty(folderPath))
        {
            folderBrowserDialog.SelectedPath = folderPath;
        }
        folderBrowserDialog.Description = Translations.Translate("Odaberite mapu u koju će se spremati XML zahtjevi i odgovori.");
        DialogResult dialogResult = folderBrowserDialog.ShowDialog();
        if (dialogResult == DialogResult.OK)
        {
            DirectoryInfo directoryInfo = null;
            folderPath = folderBrowserDialog.SelectedPath;


            directoryInfo = new DirectoryInfo(Environment.CurrentDirectory + "\\FiskXMLMessages\\Invoice\\Requests\\"+(DateTime.Today.Year.ToString())+"\\");
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            directoryInfo = new DirectoryInfo(Environment.CurrentDirectory + "\\FiskXMLMessages\\Invoice\\Response\\" + (DateTime.Today.Year.ToString()) + "\\");
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }


        }
        btnUnlockSettings.Enabled = true;
        btnQuit.Enabled = true;
        btnSaveAndQuit.Enabled = true;
        btnXmlPathChoose.Enabled = true;
    }

    private bool saveConfig()
    {
        log.Debug("Config saved!");
        state = false;
        ConfigFile configFile = new ConfigFile();
        configFile.ConnectionString = txtConnectString.Text;
        configFile.ServerUrl = txtURL.Text;
        configFile.ConnectionEncryption = cmbConnectionEncryption.SelectedItem.ToString();
        configFile.LogFileActive = Convert.ToString(chkLogFileActive.Checked);
        configFile.SaveXMLActive = Convert.ToString(chkSaveXmlActive.Checked);
        configFile.XMLSavePath = folderPath;
        configFile.DateIsActive = $"{dtpDateActive.Value:yyyy/MM/dd}";
        configFile.ActiveLanguage = cmbActiveLanguage.SelectedItem.ToString();
        configFile.VATNumber = txtVatNumber.Text;
        configFile.InVATsystem = Convert.ToString(chkVatActive.Checked);
        configFile.PremiseMark = txtPremiseMark.Text;
        configFile.BillingDeviceMark = txtBillingDeviceMark.Text;
        configFile.OIBSoftware = txtOibSoftware.Text;
        configFile.OperatorOIB = txtOibOperator.Text;
        configFile.OperatorCode = txtCodeOperator.Text;
        configFile.Certifikat = txtCertificate.Text;
        configFile.SendTestReceipts = Convert.ToString(chkSendTest.Checked);
        configFile.UseCertificateFile = Convert.ToString(chkUseCertificateFile.Checked);
        configFile.CertificatePassword = txtCertificatePassword.Text;
        configFile.IgnoreSSLCertificates = Convert.ToString(cbCheckSSL.Checked);
        configFile.SendPonudaToFisk = Convert.ToString(cbFiskPonude.Checked);
        configFile.QrCodeMessage = txtQrMessage.Text;
        configFile.QrCodeLocation = txtQrSaveLocation.Text;
        configFile.QrCodeSize = txtQrSize.Text;
        configFile.LogLevel = cmbLogLevel.SelectedItem.ToString();

        verifyData();
        if (state)
        {
            return false;
        }
        LogFile.CreateConfigFile(configFile, isConfigMod: true);
        return true;
    }

    private void btnProcessBills_Click(object sender, EventArgs e)
    {
        using (Process process = new Process())
        {
            process.StartInfo.FileName = "C:\\Ikosoft\\MerlinX2\\385_fisk.exe";
            process.Start();
            process.WaitForExit();
        }
    }



    private void btnCertificateFile_Click(object sender, EventArgs e)
    {
        btnUnlockSettings.Enabled = false;
        //btnSendData.Enabled = false;
        btnQuit.Enabled = false;
        btnSaveAndQuit.Enabled = false;
        btnCertificateFile.Enabled = false;
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Certificate|*.p12";
        DialogResult dialogResult = openFileDialog.ShowDialog();
        if (dialogResult == DialogResult.OK)
        {
            string fileName = openFileDialog.FileName;
            FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
            string directoryName = fileInfo.DirectoryName;
            string destFileName = Path.Combine(directoryName + "/Certificates", "certificate.p12");
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            File.Copy(fileName, destFileName, overwrite: true);
        }
        btnUnlockSettings.Enabled = true;
        //btnSendData.Enabled = true;
        btnQuit.Enabled = true;
        btnSaveAndQuit.Enabled = true;
        btnCertificateFile.Enabled = true;
    }

    private void chkUseCertificateFile_CheckedChanged_1(object sender, EventArgs e)
    {
        if (chkUseCertificateFile.CheckState == CheckState.Checked && chkUseCertificateFile.Enabled)
        {
            txtCertificatePassword.Enabled = true;
            btnCertificateFile.Enabled = true;
        }
        else
        {
            txtCertificatePassword.Enabled = false;
            btnCertificateFile.Enabled = false;
        }
    }


    private void btUpdate_Click(object sender, EventArgs e)
    {
        //AutoUpdater.Mandatory = true;
        AutoUpdater.Start("https://www.dropbox.com/s/l86kf0sochnqnh6/CisUpdateList.xml?dl=1");
        AutoUpdater.DownloadPath = Environment.CurrentDirectory;
    }

    private void MessageAlert(string content, string title, NumLog logerror = NumLog.None, int ticket = 1, string extendmessage = "")
    {
        //MessageBox.Show(content, title, MessageBoxButtons.OK, MessageBoxIcon.Hand);
        ErrorMessageBox errorMessageBox = new ErrorMessageBox();
        errorMessageBox.lbMessageText.Text = title;
        //errorMessageBox.lbMessageDetails.Text = content;
        errorMessageBox.tbMessage.Text = content;
        errorMessageBox.ShowDialog();

        log.Debug("Error in QR code regen " + content);
    }

    /// <summary>
    /// regeneriranje barkodova
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void button1_Click(object sender, EventArgs e)
    {
        try
        {
            
            ExpirationDate expDate = new ExpirationDate();
            expDate.ShowDialog();
            DateTime fromDate = expDate.dateTimePicker1.Value;
            
            pgBarQr.Visible = true;
            pgBarQr.Minimum = 0;
            pgBarQr.Maximum = regen.GetDataCount(fromDate);
            var progressHandler = new Progress<int>(value =>
            {
                pgBarQr.Value = value;
            });
            var progress = progressHandler as IProgress<int>;
            await Task.Run(() =>
            {
                regen.QrCodeRegeneration(fromDate,progress);

            });
            MessageBox.Show("Qr kodovi generirani", "QrCode generator");
            pgBarQr.Visible = false;
        }
        catch(Exception ex)
        {
            log.Error("Error in QR code regen " ,ex);
            MessageAlert("Greška u generiranju barkodova, molim provjerite log ili kontaktirajte održavanje", "Error in qrcode regeneration");
        }

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
    private void InitializeComponent () {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Config));
            this.btnSaveAndQuit = new System.Windows.Forms.Button();
            this.lblConnectionString = new System.Windows.Forms.Label();
            this.txtConnectString = new System.Windows.Forms.TextBox();
            this.txtURL = new System.Windows.Forms.TextBox();
            this.lblURL = new System.Windows.Forms.Label();
            this.chkLogFileActive = new System.Windows.Forms.CheckBox();
            this.dtpDateActive = new System.Windows.Forms.DateTimePicker();
            this.lblDateActive = new System.Windows.Forms.Label();
            this.grpBasic = new System.Windows.Forms.GroupBox();
            this.cmbLogLevel = new System.Windows.Forms.ComboBox();
            this.lbloglevel = new System.Windows.Forms.Label();
            this.cmbActiveLanguage = new System.Windows.Forms.ComboBox();
            this.lblActiveLanguage = new System.Windows.Forms.Label();
            this.cmbConnectionEncryption = new System.Windows.Forms.ComboBox();
            this.lblConnectionEncryption = new System.Windows.Forms.Label();
            this.btnXmlPathChoose = new System.Windows.Forms.Button();
            this.lblXmlPath = new System.Windows.Forms.Label();
            this.chkSaveXmlActive = new System.Windows.Forms.CheckBox();
            this.txtQrMessage = new System.Windows.Forms.TextBox();
            this.lbQrMessage = new System.Windows.Forms.Label();
            this.grpPremise = new System.Windows.Forms.GroupBox();
            this.lblOibSoftware = new System.Windows.Forms.Label();
            this.txtOibSoftware = new System.Windows.Forms.TextBox();
            this.lblBillingDeviceMark = new System.Windows.Forms.Label();
            this.txtBillingDeviceMark = new System.Windows.Forms.TextBox();
            this.lblPremiseMark = new System.Windows.Forms.Label();
            this.txtPremiseMark = new System.Windows.Forms.TextBox();
            this.chkVatActive = new System.Windows.Forms.CheckBox();
            this.lblOIB = new System.Windows.Forms.Label();
            this.txtVatNumber = new System.Windows.Forms.TextBox();
            this.btnUnlockSettings = new System.Windows.Forms.Button();
            this.grpSolo = new System.Windows.Forms.GroupBox();
            this.lblCodeOperator = new System.Windows.Forms.Label();
            this.txtCodeOperator = new System.Windows.Forms.TextBox();
            this.lblOibOperator = new System.Windows.Forms.Label();
            this.txtOibOperator = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnQuit = new System.Windows.Forms.Button();
            this.grpCertificate = new System.Windows.Forms.GroupBox();
            this.lblCertificate = new System.Windows.Forms.Label();
            this.txtCertificate = new System.Windows.Forms.TextBox();
            this.lblErrorMessage = new System.Windows.Forms.Label();
            this.grpExtraOptions = new System.Windows.Forms.GroupBox();
            this.cbFiskPonude = new System.Windows.Forms.CheckBox();
            this.chkSendTest = new System.Windows.Forms.CheckBox();
            this.btnProcessBills = new System.Windows.Forms.Button();
            this.grpCertificateFile = new System.Windows.Forms.GroupBox();
            this.cbCheckSSL = new System.Windows.Forms.CheckBox();
            this.lbCertPassword = new System.Windows.Forms.Label();
            this.txtCertificatePassword = new System.Windows.Forms.TextBox();
            this.btnCertificateFile = new System.Windows.Forms.Button();
            this.lbCertLocation = new System.Windows.Forms.Label();
            this.chkUseCertificateFile = new System.Windows.Forms.CheckBox();
            this.btUpdate = new System.Windows.Forms.Button();
            this.lbVerzija = new System.Windows.Forms.Label();
            this.grpQrCode = new System.Windows.Forms.GroupBox();
            this.pgBarQr = new System.Windows.Forms.ProgressBar();
            this.btQrRegen = new System.Windows.Forms.Button();
            this.txtQrSize = new System.Windows.Forms.TextBox();
            this.lbQrSize = new System.Windows.Forms.Label();
            this.txtQrSaveLocation = new System.Windows.Forms.TextBox();
            this.lbQrSaveLocation = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabOsnovno = new System.Windows.Forms.TabPage();
            this.tabPostavke = new System.Windows.Forms.TabPage();
            this.tabSolo = new System.Windows.Forms.TabPage();
            this.tabExtraOpcije = new System.Windows.Forms.TabPage();
            this.tabCertifikati = new System.Windows.Forms.TabPage();
            this.tabQr = new System.Windows.Forms.TabPage();
            this.grpBasic.SuspendLayout();
            this.grpPremise.SuspendLayout();
            this.grpSolo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.grpCertificate.SuspendLayout();
            this.grpExtraOptions.SuspendLayout();
            this.grpCertificateFile.SuspendLayout();
            this.grpQrCode.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabOsnovno.SuspendLayout();
            this.tabPostavke.SuspendLayout();
            this.tabSolo.SuspendLayout();
            this.tabExtraOpcije.SuspendLayout();
            this.tabCertifikati.SuspendLayout();
            this.tabQr.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSaveAndQuit
            // 
            this.btnSaveAndQuit.BackColor = System.Drawing.Color.Transparent;
            this.btnSaveAndQuit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveAndQuit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveAndQuit.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveAndQuit.Image")));
            this.btnSaveAndQuit.Location = new System.Drawing.Point(681, 2);
            this.btnSaveAndQuit.Margin = new System.Windows.Forms.Padding(4);
            this.btnSaveAndQuit.Name = "btnSaveAndQuit";
            this.btnSaveAndQuit.Size = new System.Drawing.Size(55, 50);
            this.btnSaveAndQuit.TabIndex = 0;
            this.btnSaveAndQuit.UseVisualStyleBackColor = false;
            this.btnSaveAndQuit.Click += new System.EventHandler(this.btnSaveAndQuitClick);
            // 
            // lblConnectionString
            // 
            this.lblConnectionString.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnectionString.Location = new System.Drawing.Point(12, 25);
            this.lblConnectionString.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblConnectionString.Name = "lblConnectionString";
            this.lblConnectionString.Size = new System.Drawing.Size(311, 16);
            this.lblConnectionString.TabIndex = 1;
            this.lblConnectionString.Text = "Connection string :";
            this.lblConnectionString.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtConnectString
            // 
            this.txtConnectString.BackColor = System.Drawing.SystemColors.Window;
            this.txtConnectString.Enabled = false;
            this.txtConnectString.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConnectString.Location = new System.Drawing.Point(331, 22);
            this.txtConnectString.Margin = new System.Windows.Forms.Padding(4);
            this.txtConnectString.Name = "txtConnectString";
            this.txtConnectString.Size = new System.Drawing.Size(399, 23);
            this.txtConnectString.TabIndex = 2;
            // 
            // txtURL
            // 
            this.txtURL.BackColor = System.Drawing.SystemColors.Window;
            this.txtURL.Enabled = false;
            this.txtURL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtURL.Location = new System.Drawing.Point(331, 90);
            this.txtURL.Margin = new System.Windows.Forms.Padding(4);
            this.txtURL.Name = "txtURL";
            this.txtURL.Size = new System.Drawing.Size(399, 23);
            this.txtURL.TabIndex = 4;
            // 
            // lblURL
            // 
            this.lblURL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblURL.Location = new System.Drawing.Point(12, 94);
            this.lblURL.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblURL.Name = "lblURL";
            this.lblURL.Size = new System.Drawing.Size(311, 16);
            this.lblURL.TabIndex = 3;
            this.lblURL.Text = "Server URL :";
            this.lblURL.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chkLogFileActive
            // 
            this.chkLogFileActive.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.chkLogFileActive.BackColor = System.Drawing.Color.Transparent;
            this.chkLogFileActive.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkLogFileActive.Enabled = false;
            this.chkLogFileActive.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkLogFileActive.Location = new System.Drawing.Point(12, 126);
            this.chkLogFileActive.Margin = new System.Windows.Forms.Padding(4);
            this.chkLogFileActive.Name = "chkLogFileActive";
            this.chkLogFileActive.Size = new System.Drawing.Size(336, 20);
            this.chkLogFileActive.TabIndex = 5;
            this.chkLogFileActive.Text = "Log datoteka :";
            this.chkLogFileActive.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkLogFileActive.UseVisualStyleBackColor = false;
            // 
            // dtpDateActive
            // 
            this.dtpDateActive.Enabled = false;
            this.dtpDateActive.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpDateActive.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDateActive.Location = new System.Drawing.Point(331, 245);
            this.dtpDateActive.Margin = new System.Windows.Forms.Padding(4);
            this.dtpDateActive.Name = "dtpDateActive";
            this.dtpDateActive.Size = new System.Drawing.Size(148, 23);
            this.dtpDateActive.TabIndex = 6;
            this.dtpDateActive.Value = new System.DateTime(2013, 4, 4, 0, 0, 0, 0);
            // 
            // lblDateActive
            // 
            this.lblDateActive.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDateActive.Location = new System.Drawing.Point(12, 249);
            this.lblDateActive.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDateActive.Name = "lblDateActive";
            this.lblDateActive.Size = new System.Drawing.Size(311, 16);
            this.lblDateActive.TabIndex = 7;
            this.lblDateActive.Text = "Datum početka primjene :";
            this.lblDateActive.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // grpBasic
            // 
            this.grpBasic.BackColor = System.Drawing.Color.Transparent;
            this.grpBasic.Controls.Add(this.cmbLogLevel);
            this.grpBasic.Controls.Add(this.lbloglevel);
            this.grpBasic.Controls.Add(this.cmbActiveLanguage);
            this.grpBasic.Controls.Add(this.lblActiveLanguage);
            this.grpBasic.Controls.Add(this.cmbConnectionEncryption);
            this.grpBasic.Controls.Add(this.lblConnectionEncryption);
            this.grpBasic.Controls.Add(this.btnXmlPathChoose);
            this.grpBasic.Controls.Add(this.lblXmlPath);
            this.grpBasic.Controls.Add(this.chkSaveXmlActive);
            this.grpBasic.Controls.Add(this.txtURL);
            this.grpBasic.Controls.Add(this.lblDateActive);
            this.grpBasic.Controls.Add(this.lblConnectionString);
            this.grpBasic.Controls.Add(this.dtpDateActive);
            this.grpBasic.Controls.Add(this.txtConnectString);
            this.grpBasic.Controls.Add(this.chkLogFileActive);
            this.grpBasic.Controls.Add(this.lblURL);
            this.grpBasic.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpBasic.Location = new System.Drawing.Point(7, 7);
            this.grpBasic.Margin = new System.Windows.Forms.Padding(4);
            this.grpBasic.Name = "grpBasic";
            this.grpBasic.Padding = new System.Windows.Forms.Padding(4);
            this.grpBasic.Size = new System.Drawing.Size(740, 325);
            this.grpBasic.TabIndex = 8;
            this.grpBasic.TabStop = false;
            this.grpBasic.Text = "Osnovno :";
            // 
            // cmbLogLevel
            // 
            this.cmbLogLevel.Enabled = false;
            this.cmbLogLevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold);
            this.cmbLogLevel.FormattingEnabled = true;
            this.cmbLogLevel.Items.AddRange(new object[] {
            "INFO",
            "DEBUG",
            "ERROR"});
            this.cmbLogLevel.Location = new System.Drawing.Point(329, 150);
            this.cmbLogLevel.Margin = new System.Windows.Forms.Padding(4);
            this.cmbLogLevel.Name = "cmbLogLevel";
            this.cmbLogLevel.Size = new System.Drawing.Size(148, 24);
            this.cmbLogLevel.TabIndex = 17;
            // 
            // lbloglevel
            // 
            this.lbloglevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbloglevel.Location = new System.Drawing.Point(12, 154);
            this.lbloglevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbloglevel.Name = "lbloglevel";
            this.lbloglevel.Size = new System.Drawing.Size(311, 16);
            this.lbloglevel.TabIndex = 16;
            this.lbloglevel.Text = "Log level :";
            this.lbloglevel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbActiveLanguage
            // 
            this.cmbActiveLanguage.Enabled = false;
            this.cmbActiveLanguage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold);
            this.cmbActiveLanguage.FormattingEnabled = true;
            this.cmbActiveLanguage.Items.AddRange(new object[] {
            "HR",
            "EN"});
            this.cmbActiveLanguage.Location = new System.Drawing.Point(331, 276);
            this.cmbActiveLanguage.Margin = new System.Windows.Forms.Padding(4);
            this.cmbActiveLanguage.Name = "cmbActiveLanguage";
            this.cmbActiveLanguage.Size = new System.Drawing.Size(148, 24);
            this.cmbActiveLanguage.TabIndex = 15;
            // 
            // lblActiveLanguage
            // 
            this.lblActiveLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblActiveLanguage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblActiveLanguage.Location = new System.Drawing.Point(12, 277);
            this.lblActiveLanguage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblActiveLanguage.Name = "lblActiveLanguage";
            this.lblActiveLanguage.Size = new System.Drawing.Size(311, 20);
            this.lblActiveLanguage.TabIndex = 14;
            this.lblActiveLanguage.Text = "Aktivan jezik :";
            this.lblActiveLanguage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbConnectionEncryption
            // 
            this.cmbConnectionEncryption.Enabled = false;
            this.cmbConnectionEncryption.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold);
            this.cmbConnectionEncryption.FormattingEnabled = true;
            this.cmbConnectionEncryption.Items.AddRange(new object[] {
            "TLS 1.1",
            "TLS 1.2"});
            this.cmbConnectionEncryption.Location = new System.Drawing.Point(331, 55);
            this.cmbConnectionEncryption.Margin = new System.Windows.Forms.Padding(4);
            this.cmbConnectionEncryption.Name = "cmbConnectionEncryption";
            this.cmbConnectionEncryption.Size = new System.Drawing.Size(148, 24);
            this.cmbConnectionEncryption.TabIndex = 13;
            // 
            // lblConnectionEncryption
            // 
            this.lblConnectionEncryption.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnectionEncryption.Location = new System.Drawing.Point(12, 60);
            this.lblConnectionEncryption.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblConnectionEncryption.Name = "lblConnectionEncryption";
            this.lblConnectionEncryption.Size = new System.Drawing.Size(311, 16);
            this.lblConnectionEncryption.TabIndex = 12;
            this.lblConnectionEncryption.Text = "Connection encryption :";
            this.lblConnectionEncryption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnXmlPathChoose
            // 
            this.btnXmlPathChoose.AutoSize = true;
            this.btnXmlPathChoose.Enabled = false;
            this.btnXmlPathChoose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnXmlPathChoose.Location = new System.Drawing.Point(329, 206);
            this.btnXmlPathChoose.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnXmlPathChoose.Name = "btnXmlPathChoose";
            this.btnXmlPathChoose.Size = new System.Drawing.Size(149, 37);
            this.btnXmlPathChoose.TabIndex = 11;
            this.btnXmlPathChoose.Text = "Odabir";
            this.btnXmlPathChoose.UseVisualStyleBackColor = true;
            this.btnXmlPathChoose.Click += new System.EventHandler(this.btnChooseClick);
            // 
            // lblXmlPath
            // 
            this.lblXmlPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblXmlPath.Location = new System.Drawing.Point(12, 213);
            this.lblXmlPath.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblXmlPath.Name = "lblXmlPath";
            this.lblXmlPath.Size = new System.Drawing.Size(311, 16);
            this.lblXmlPath.TabIndex = 10;
            this.lblXmlPath.Text = "Putanja spremanja XML datoteka :";
            this.lblXmlPath.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chkSaveXmlActive
            // 
            this.chkSaveXmlActive.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkSaveXmlActive.Enabled = false;
            this.chkSaveXmlActive.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.chkSaveXmlActive.Location = new System.Drawing.Point(12, 180);
            this.chkSaveXmlActive.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkSaveXmlActive.Name = "chkSaveXmlActive";
            this.chkSaveXmlActive.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkSaveXmlActive.Size = new System.Drawing.Size(336, 21);
            this.chkSaveXmlActive.TabIndex = 9;
            this.chkSaveXmlActive.Text = "Spremanje XML poruka i odgovora :";
            this.chkSaveXmlActive.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkSaveXmlActive.UseVisualStyleBackColor = true;
            // 
            // txtQrMessage
            // 
            this.txtQrMessage.BackColor = System.Drawing.SystemColors.Window;
            this.txtQrMessage.Enabled = false;
            this.txtQrMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtQrMessage.Location = new System.Drawing.Point(329, 26);
            this.txtQrMessage.Margin = new System.Windows.Forms.Padding(4);
            this.txtQrMessage.Name = "txtQrMessage";
            this.txtQrMessage.Size = new System.Drawing.Size(349, 23);
            this.txtQrMessage.TabIndex = 17;
            // 
            // lbQrMessage
            // 
            this.lbQrMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbQrMessage.Location = new System.Drawing.Point(12, 30);
            this.lbQrMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbQrMessage.Name = "lbQrMessage";
            this.lbQrMessage.Size = new System.Drawing.Size(311, 16);
            this.lbQrMessage.TabIndex = 16;
            this.lbQrMessage.Text = "QR poruka :";
            this.lbQrMessage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // grpPremise
            // 
            this.grpPremise.BackColor = System.Drawing.Color.Transparent;
            this.grpPremise.Controls.Add(this.lblOibSoftware);
            this.grpPremise.Controls.Add(this.txtOibSoftware);
            this.grpPremise.Controls.Add(this.lblBillingDeviceMark);
            this.grpPremise.Controls.Add(this.txtBillingDeviceMark);
            this.grpPremise.Controls.Add(this.lblPremiseMark);
            this.grpPremise.Controls.Add(this.txtPremiseMark);
            this.grpPremise.Controls.Add(this.chkVatActive);
            this.grpPremise.Controls.Add(this.lblOIB);
            this.grpPremise.Controls.Add(this.txtVatNumber);
            this.grpPremise.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpPremise.Location = new System.Drawing.Point(7, 12);
            this.grpPremise.Margin = new System.Windows.Forms.Padding(4);
            this.grpPremise.Name = "grpPremise";
            this.grpPremise.Padding = new System.Windows.Forms.Padding(4);
            this.grpPremise.Size = new System.Drawing.Size(740, 197);
            this.grpPremise.TabIndex = 9;
            this.grpPremise.TabStop = false;
            this.grpPremise.Text = "Postavke salona :";
            // 
            // lblOibSoftware
            // 
            this.lblOibSoftware.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOibSoftware.Location = new System.Drawing.Point(12, 162);
            this.lblOibSoftware.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOibSoftware.Name = "lblOibSoftware";
            this.lblOibSoftware.Size = new System.Drawing.Size(311, 16);
            this.lblOibSoftware.TabIndex = 27;
            this.lblOibSoftware.Text = "OIB proizvođača programa :";
            this.lblOibSoftware.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtOibSoftware
            // 
            this.txtOibSoftware.Enabled = false;
            this.txtOibSoftware.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOibSoftware.Location = new System.Drawing.Point(331, 156);
            this.txtOibSoftware.Margin = new System.Windows.Forms.Padding(4);
            this.txtOibSoftware.Name = "txtOibSoftware";
            this.txtOibSoftware.Size = new System.Drawing.Size(399, 23);
            this.txtOibSoftware.TabIndex = 28;
            // 
            // lblBillingDeviceMark
            // 
            this.lblBillingDeviceMark.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBillingDeviceMark.Location = new System.Drawing.Point(12, 132);
            this.lblBillingDeviceMark.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBillingDeviceMark.Name = "lblBillingDeviceMark";
            this.lblBillingDeviceMark.Size = new System.Drawing.Size(311, 16);
            this.lblBillingDeviceMark.TabIndex = 25;
            this.lblBillingDeviceMark.Text = "Oznaka naplatnog uređaja :";
            this.lblBillingDeviceMark.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtBillingDeviceMark
            // 
            this.txtBillingDeviceMark.Enabled = false;
            this.txtBillingDeviceMark.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBillingDeviceMark.Location = new System.Drawing.Point(331, 126);
            this.txtBillingDeviceMark.Margin = new System.Windows.Forms.Padding(4);
            this.txtBillingDeviceMark.Name = "txtBillingDeviceMark";
            this.txtBillingDeviceMark.Size = new System.Drawing.Size(399, 23);
            this.txtBillingDeviceMark.TabIndex = 26;
            // 
            // lblPremiseMark
            // 
            this.lblPremiseMark.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPremiseMark.Location = new System.Drawing.Point(12, 102);
            this.lblPremiseMark.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPremiseMark.Name = "lblPremiseMark";
            this.lblPremiseMark.Size = new System.Drawing.Size(311, 16);
            this.lblPremiseMark.TabIndex = 23;
            this.lblPremiseMark.Text = "Oznaka prostora :";
            this.lblPremiseMark.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPremiseMark
            // 
            this.txtPremiseMark.Enabled = false;
            this.txtPremiseMark.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPremiseMark.Location = new System.Drawing.Point(331, 98);
            this.txtPremiseMark.Margin = new System.Windows.Forms.Padding(4);
            this.txtPremiseMark.Name = "txtPremiseMark";
            this.txtPremiseMark.Size = new System.Drawing.Size(399, 23);
            this.txtPremiseMark.TabIndex = 24;
            // 
            // chkVatActive
            // 
            this.chkVatActive.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.chkVatActive.BackColor = System.Drawing.Color.Transparent;
            this.chkVatActive.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkVatActive.Enabled = false;
            this.chkVatActive.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkVatActive.Location = new System.Drawing.Point(16, 71);
            this.chkVatActive.Margin = new System.Windows.Forms.Padding(4);
            this.chkVatActive.Name = "chkVatActive";
            this.chkVatActive.Size = new System.Drawing.Size(332, 18);
            this.chkVatActive.TabIndex = 6;
            this.chkVatActive.Text = "U sustavu PDV :";
            this.chkVatActive.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkVatActive.UseVisualStyleBackColor = false;
            // 
            // lblOIB
            // 
            this.lblOIB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOIB.Location = new System.Drawing.Point(12, 41);
            this.lblOIB.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOIB.Name = "lblOIB";
            this.lblOIB.Size = new System.Drawing.Size(311, 16);
            this.lblOIB.TabIndex = 3;
            this.lblOIB.Text = "OIB :";
            this.lblOIB.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtVatNumber
            // 
            this.txtVatNumber.Enabled = false;
            this.txtVatNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVatNumber.Location = new System.Drawing.Point(331, 38);
            this.txtVatNumber.Margin = new System.Windows.Forms.Padding(4);
            this.txtVatNumber.Name = "txtVatNumber";
            this.txtVatNumber.Size = new System.Drawing.Size(399, 23);
            this.txtVatNumber.TabIndex = 4;
            // 
            // btnUnlockSettings
            // 
            this.btnUnlockSettings.BackColor = System.Drawing.Color.Transparent;
            this.btnUnlockSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUnlockSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUnlockSettings.Image = ((System.Drawing.Image)(resources.GetObject("btnUnlockSettings.Image")));
            this.btnUnlockSettings.Location = new System.Drawing.Point(494, 2);
            this.btnUnlockSettings.Margin = new System.Windows.Forms.Padding(4);
            this.btnUnlockSettings.Name = "btnUnlockSettings";
            this.btnUnlockSettings.Size = new System.Drawing.Size(55, 50);
            this.btnUnlockSettings.TabIndex = 0;
            this.btnUnlockSettings.UseVisualStyleBackColor = false;
            this.btnUnlockSettings.Click += new System.EventHandler(this.btnUnlockSettingClick);
            // 
            // grpSolo
            // 
            this.grpSolo.BackColor = System.Drawing.Color.Transparent;
            this.grpSolo.Controls.Add(this.lblCodeOperator);
            this.grpSolo.Controls.Add(this.txtCodeOperator);
            this.grpSolo.Controls.Add(this.lblOibOperator);
            this.grpSolo.Controls.Add(this.txtOibOperator);
            this.grpSolo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpSolo.Location = new System.Drawing.Point(7, 12);
            this.grpSolo.Margin = new System.Windows.Forms.Padding(4);
            this.grpSolo.Name = "grpSolo";
            this.grpSolo.Padding = new System.Windows.Forms.Padding(4);
            this.grpSolo.Size = new System.Drawing.Size(740, 98);
            this.grpSolo.TabIndex = 10;
            this.grpSolo.TabStop = false;
            this.grpSolo.Text = "Solo postavke :";
            // 
            // lblCodeOperator
            // 
            this.lblCodeOperator.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCodeOperator.Location = new System.Drawing.Point(16, 62);
            this.lblCodeOperator.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCodeOperator.Name = "lblCodeOperator";
            this.lblCodeOperator.Size = new System.Drawing.Size(307, 16);
            this.lblCodeOperator.TabIndex = 7;
            this.lblCodeOperator.Text = "Šifra operatera :";
            this.lblCodeOperator.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtCodeOperator
            // 
            this.txtCodeOperator.BackColor = System.Drawing.SystemColors.Window;
            this.txtCodeOperator.Enabled = false;
            this.txtCodeOperator.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCodeOperator.Location = new System.Drawing.Point(329, 59);
            this.txtCodeOperator.Margin = new System.Windows.Forms.Padding(4);
            this.txtCodeOperator.Name = "txtCodeOperator";
            this.txtCodeOperator.Size = new System.Drawing.Size(400, 23);
            this.txtCodeOperator.TabIndex = 8;
            // 
            // lblOibOperator
            // 
            this.lblOibOperator.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOibOperator.Location = new System.Drawing.Point(12, 27);
            this.lblOibOperator.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOibOperator.Name = "lblOibOperator";
            this.lblOibOperator.Size = new System.Drawing.Size(311, 16);
            this.lblOibOperator.TabIndex = 5;
            this.lblOibOperator.Text = "OIB operatera :";
            this.lblOibOperator.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtOibOperator
            // 
            this.txtOibOperator.BackColor = System.Drawing.SystemColors.Window;
            this.txtOibOperator.Enabled = false;
            this.txtOibOperator.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOibOperator.Location = new System.Drawing.Point(329, 23);
            this.txtOibOperator.Margin = new System.Windows.Forms.Padding(4);
            this.txtOibOperator.Name = "txtOibOperator";
            this.txtOibOperator.Size = new System.Drawing.Size(400, 23);
            this.txtOibOperator.TabIndex = 6;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 4);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(44, 12);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 40;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // btnQuit
            // 
            this.btnQuit.BackColor = System.Drawing.Color.Transparent;
            this.btnQuit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnQuit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnQuit.Image = ((System.Drawing.Image)(resources.GetObject("btnQuit.Image")));
            this.btnQuit.Location = new System.Drawing.Point(618, 2);
            this.btnQuit.Margin = new System.Windows.Forms.Padding(4);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(55, 50);
            this.btnQuit.TabIndex = 202;
            this.btnQuit.UseVisualStyleBackColor = false;
            this.btnQuit.Click += new System.EventHandler(this.btnQuitClick);
            // 
            // grpCertificate
            // 
            this.grpCertificate.BackColor = System.Drawing.Color.Transparent;
            this.grpCertificate.Controls.Add(this.lblCertificate);
            this.grpCertificate.Controls.Add(this.txtCertificate);
            this.grpCertificate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpCertificate.Location = new System.Drawing.Point(10, 171);
            this.grpCertificate.Margin = new System.Windows.Forms.Padding(4);
            this.grpCertificate.Name = "grpCertificate";
            this.grpCertificate.Padding = new System.Windows.Forms.Padding(4);
            this.grpCertificate.Size = new System.Drawing.Size(688, 71);
            this.grpCertificate.TabIndex = 203;
            this.grpCertificate.TabStop = false;
            this.grpCertificate.Text = "Certifikat :";
            // 
            // lblCertificate
            // 
            this.lblCertificate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCertificate.Location = new System.Drawing.Point(20, 32);
            this.lblCertificate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCertificate.Name = "lblCertificate";
            this.lblCertificate.Size = new System.Drawing.Size(302, 16);
            this.lblCertificate.TabIndex = 5;
            this.lblCertificate.Text = "Naziv certifikata :";
            this.lblCertificate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtCertificate
            // 
            this.txtCertificate.BackColor = System.Drawing.SystemColors.Window;
            this.txtCertificate.Enabled = false;
            this.txtCertificate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCertificate.Location = new System.Drawing.Point(329, 28);
            this.txtCertificate.Margin = new System.Windows.Forms.Padding(4);
            this.txtCertificate.Name = "txtCertificate";
            this.txtCertificate.Size = new System.Drawing.Size(349, 23);
            this.txtCertificate.TabIndex = 6;
            // 
            // lblErrorMessage
            // 
            this.lblErrorMessage.AutoSize = true;
            this.lblErrorMessage.BackColor = System.Drawing.Color.Transparent;
            this.lblErrorMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblErrorMessage.ForeColor = System.Drawing.Color.Red;
            this.lblErrorMessage.Location = new System.Drawing.Point(7, 20);
            this.lblErrorMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblErrorMessage.Name = "lblErrorMessage";
            this.lblErrorMessage.Size = new System.Drawing.Size(396, 25);
            this.lblErrorMessage.TabIndex = 204;
            this.lblErrorMessage.Text = "Neke postavke su prazne ili neispravne!";
            this.lblErrorMessage.Visible = false;
            // 
            // grpExtraOptions
            // 
            this.grpExtraOptions.BackColor = System.Drawing.Color.Transparent;
            this.grpExtraOptions.Controls.Add(this.cbFiskPonude);
            this.grpExtraOptions.Controls.Add(this.chkSendTest);
            this.grpExtraOptions.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpExtraOptions.Location = new System.Drawing.Point(7, 7);
            this.grpExtraOptions.Margin = new System.Windows.Forms.Padding(4);
            this.grpExtraOptions.Name = "grpExtraOptions";
            this.grpExtraOptions.Padding = new System.Windows.Forms.Padding(4);
            this.grpExtraOptions.Size = new System.Drawing.Size(691, 106);
            this.grpExtraOptions.TabIndex = 205;
            this.grpExtraOptions.TabStop = false;
            this.grpExtraOptions.Text = "Dodatne opcije :";
            // 
            // cbFiskPonude
            // 
            this.cbFiskPonude.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.cbFiskPonude.BackColor = System.Drawing.Color.Transparent;
            this.cbFiskPonude.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbFiskPonude.Enabled = false;
            this.cbFiskPonude.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbFiskPonude.Location = new System.Drawing.Point(16, 64);
            this.cbFiskPonude.Margin = new System.Windows.Forms.Padding(4);
            this.cbFiskPonude.Name = "cbFiskPonude";
            this.cbFiskPonude.Size = new System.Drawing.Size(332, 30);
            this.cbFiskPonude.TabIndex = 31;
            this.cbFiskPonude.Text = "Fiskaliziraj ponude :";
            this.cbFiskPonude.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbFiskPonude.UseVisualStyleBackColor = false;
            // 
            // chkSendTest
            // 
            this.chkSendTest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.chkSendTest.BackColor = System.Drawing.Color.Transparent;
            this.chkSendTest.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkSendTest.Enabled = false;
            this.chkSendTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkSendTest.Location = new System.Drawing.Point(16, 26);
            this.chkSendTest.Margin = new System.Windows.Forms.Padding(4);
            this.chkSendTest.Name = "chkSendTest";
            this.chkSendTest.Size = new System.Drawing.Size(332, 30);
            this.chkSendTest.TabIndex = 30;
            this.chkSendTest.Text = "Slanje testnih računa :";
            this.chkSendTest.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkSendTest.UseVisualStyleBackColor = false;
            // 
            // btnProcessBills
            // 
            this.btnProcessBills.BackColor = System.Drawing.Color.Transparent;
            this.btnProcessBills.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProcessBills.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnProcessBills.Image = ((System.Drawing.Image)(resources.GetObject("btnProcessBills.Image")));
            this.btnProcessBills.Location = new System.Drawing.Point(555, 2);
            this.btnProcessBills.Margin = new System.Windows.Forms.Padding(4);
            this.btnProcessBills.Name = "btnProcessBills";
            this.btnProcessBills.Size = new System.Drawing.Size(55, 50);
            this.btnProcessBills.TabIndex = 206;
            this.btnProcessBills.UseVisualStyleBackColor = false;
            this.btnProcessBills.Click += new System.EventHandler(this.btnProcessBills_Click);
            // 
            // grpCertificateFile
            // 
            this.grpCertificateFile.BackColor = System.Drawing.Color.Transparent;
            this.grpCertificateFile.Controls.Add(this.cbCheckSSL);
            this.grpCertificateFile.Controls.Add(this.lbCertPassword);
            this.grpCertificateFile.Controls.Add(this.txtCertificatePassword);
            this.grpCertificateFile.Controls.Add(this.btnCertificateFile);
            this.grpCertificateFile.Controls.Add(this.lbCertLocation);
            this.grpCertificateFile.Controls.Add(this.chkUseCertificateFile);
            this.grpCertificateFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpCertificateFile.Location = new System.Drawing.Point(7, 7);
            this.grpCertificateFile.Margin = new System.Windows.Forms.Padding(4);
            this.grpCertificateFile.Name = "grpCertificateFile";
            this.grpCertificateFile.Padding = new System.Windows.Forms.Padding(4);
            this.grpCertificateFile.Size = new System.Drawing.Size(691, 156);
            this.grpCertificateFile.TabIndex = 207;
            this.grpCertificateFile.TabStop = false;
            this.grpCertificateFile.Text = "Certifikat iz datoteke :";
            // 
            // cbCheckSSL
            // 
            this.cbCheckSSL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.cbCheckSSL.BackColor = System.Drawing.Color.Transparent;
            this.cbCheckSSL.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbCheckSSL.Enabled = false;
            this.cbCheckSSL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbCheckSSL.Location = new System.Drawing.Point(14, 112);
            this.cbCheckSSL.Margin = new System.Windows.Forms.Padding(4);
            this.cbCheckSSL.Name = "cbCheckSSL";
            this.cbCheckSSL.Size = new System.Drawing.Size(332, 44);
            this.cbCheckSSL.TabIndex = 16;
            this.cbCheckSSL.Text = "Ignoriraj SSL certifikate:";
            this.cbCheckSSL.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbCheckSSL.UseVisualStyleBackColor = false;
            // 
            // lbCertPassword
            // 
            this.lbCertPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCertPassword.Location = new System.Drawing.Point(20, 92);
            this.lbCertPassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbCertPassword.Name = "lbCertPassword";
            this.lbCertPassword.Size = new System.Drawing.Size(302, 16);
            this.lbCertPassword.TabIndex = 14;
            this.lbCertPassword.Text = "Lozinka certifikata :";
            this.lbCertPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtCertificatePassword
            // 
            this.txtCertificatePassword.BackColor = System.Drawing.SystemColors.Window;
            this.txtCertificatePassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCertificatePassword.Location = new System.Drawing.Point(329, 89);
            this.txtCertificatePassword.Margin = new System.Windows.Forms.Padding(4);
            this.txtCertificatePassword.Name = "txtCertificatePassword";
            this.txtCertificatePassword.PasswordChar = 'x';
            this.txtCertificatePassword.Size = new System.Drawing.Size(352, 23);
            this.txtCertificatePassword.TabIndex = 15;
            // 
            // btnCertificateFile
            // 
            this.btnCertificateFile.AutoSize = true;
            this.btnCertificateFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnCertificateFile.Location = new System.Drawing.Point(329, 53);
            this.btnCertificateFile.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCertificateFile.Name = "btnCertificateFile";
            this.btnCertificateFile.Size = new System.Drawing.Size(149, 37);
            this.btnCertificateFile.TabIndex = 13;
            this.btnCertificateFile.Text = "Odabir";
            this.btnCertificateFile.UseVisualStyleBackColor = true;
            this.btnCertificateFile.Click += new System.EventHandler(this.btnCertificateFile_Click);
            // 
            // lbCertLocation
            // 
            this.lbCertLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCertLocation.Location = new System.Drawing.Point(12, 59);
            this.lbCertLocation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbCertLocation.Name = "lbCertLocation";
            this.lbCertLocation.Size = new System.Drawing.Size(311, 16);
            this.lbCertLocation.TabIndex = 12;
            this.lbCertLocation.Text = "Putanja datoteke s certifikatom :";
            this.lbCertLocation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chkUseCertificateFile
            // 
            this.chkUseCertificateFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.chkUseCertificateFile.BackColor = System.Drawing.Color.Transparent;
            this.chkUseCertificateFile.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkUseCertificateFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkUseCertificateFile.Location = new System.Drawing.Point(16, 14);
            this.chkUseCertificateFile.Margin = new System.Windows.Forms.Padding(4);
            this.chkUseCertificateFile.Name = "chkUseCertificateFile";
            this.chkUseCertificateFile.Size = new System.Drawing.Size(332, 44);
            this.chkUseCertificateFile.TabIndex = 7;
            this.chkUseCertificateFile.Text = "Koristi certifikat iz datoteke :";
            this.chkUseCertificateFile.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkUseCertificateFile.UseVisualStyleBackColor = false;
            this.chkUseCertificateFile.CheckedChanged += new System.EventHandler(this.chkUseCertificateFile_CheckedChanged_1);
            // 
            // btUpdate
            // 
            this.btUpdate.BackColor = System.Drawing.Color.Transparent;
            this.btUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btUpdate.ForeColor = System.Drawing.Color.Transparent;
            this.btUpdate.Image = ((System.Drawing.Image)(resources.GetObject("btUpdate.Image")));
            this.btUpdate.Location = new System.Drawing.Point(741, 2);
            this.btUpdate.Margin = new System.Windows.Forms.Padding(4);
            this.btUpdate.Name = "btUpdate";
            this.btUpdate.Size = new System.Drawing.Size(55, 50);
            this.btUpdate.TabIndex = 208;
            this.btUpdate.UseVisualStyleBackColor = false;
            this.btUpdate.Click += new System.EventHandler(this.btUpdate_Click);
            // 
            // lbVerzija
            // 
            this.lbVerzija.AutoSize = true;
            this.lbVerzija.BackColor = System.Drawing.Color.Transparent;
            this.lbVerzija.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.lbVerzija.ForeColor = System.Drawing.Color.Black;
            this.lbVerzija.Location = new System.Drawing.Point(6, 568);
            this.lbVerzija.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbVerzija.Name = "lbVerzija";
            this.lbVerzija.Size = new System.Drawing.Size(68, 20);
            this.lbVerzija.TabIndex = 209;
            this.lbVerzija.Text = "Verzija";
            // 
            // grpQrCode
            // 
            this.grpQrCode.BackColor = System.Drawing.Color.Transparent;
            this.grpQrCode.Controls.Add(this.pgBarQr);
            this.grpQrCode.Controls.Add(this.btQrRegen);
            this.grpQrCode.Controls.Add(this.txtQrSize);
            this.grpQrCode.Controls.Add(this.lbQrSize);
            this.grpQrCode.Controls.Add(this.txtQrSaveLocation);
            this.grpQrCode.Controls.Add(this.lbQrSaveLocation);
            this.grpQrCode.Controls.Add(this.txtQrMessage);
            this.grpQrCode.Controls.Add(this.lbQrMessage);
            this.grpQrCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpQrCode.Location = new System.Drawing.Point(7, 7);
            this.grpQrCode.Margin = new System.Windows.Forms.Padding(4);
            this.grpQrCode.Name = "grpQrCode";
            this.grpQrCode.Padding = new System.Windows.Forms.Padding(4);
            this.grpQrCode.Size = new System.Drawing.Size(688, 254);
            this.grpQrCode.TabIndex = 29;
            this.grpQrCode.TabStop = false;
            this.grpQrCode.Text = "Postavke QR Coda :";
            // 
            // pgBarQr
            // 
            this.pgBarQr.Location = new System.Drawing.Point(329, 164);
            this.pgBarQr.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pgBarQr.Name = "pgBarQr";
            this.pgBarQr.Size = new System.Drawing.Size(348, 37);
            this.pgBarQr.TabIndex = 22;
            this.pgBarQr.Visible = false;
            // 
            // btQrRegen
            // 
            this.btQrRegen.AutoSize = true;
            this.btQrRegen.Enabled = false;
            this.btQrRegen.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btQrRegen.Location = new System.Drawing.Point(329, 122);
            this.btQrRegen.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btQrRegen.Name = "btQrRegen";
            this.btQrRegen.Size = new System.Drawing.Size(267, 37);
            this.btQrRegen.TabIndex = 17;
            this.btQrRegen.Text = "Regeneriranje kodova";
            this.btQrRegen.UseVisualStyleBackColor = true;
            this.btQrRegen.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtQrSize
            // 
            this.txtQrSize.BackColor = System.Drawing.SystemColors.Window;
            this.txtQrSize.Enabled = false;
            this.txtQrSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtQrSize.Location = new System.Drawing.Point(328, 86);
            this.txtQrSize.Margin = new System.Windows.Forms.Padding(4);
            this.txtQrSize.Name = "txtQrSize";
            this.txtQrSize.Size = new System.Drawing.Size(72, 23);
            this.txtQrSize.TabIndex = 21;
            // 
            // lbQrSize
            // 
            this.lbQrSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbQrSize.Location = new System.Drawing.Point(11, 90);
            this.lbQrSize.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbQrSize.Name = "lbQrSize";
            this.lbQrSize.Size = new System.Drawing.Size(311, 16);
            this.lbQrSize.TabIndex = 20;
            this.lbQrSize.Text = "Veličina QR koda :";
            this.lbQrSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtQrSaveLocation
            // 
            this.txtQrSaveLocation.BackColor = System.Drawing.SystemColors.Window;
            this.txtQrSaveLocation.Enabled = false;
            this.txtQrSaveLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtQrSaveLocation.Location = new System.Drawing.Point(329, 55);
            this.txtQrSaveLocation.Margin = new System.Windows.Forms.Padding(4);
            this.txtQrSaveLocation.Name = "txtQrSaveLocation";
            this.txtQrSaveLocation.Size = new System.Drawing.Size(349, 23);
            this.txtQrSaveLocation.TabIndex = 19;
            // 
            // lbQrSaveLocation
            // 
            this.lbQrSaveLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbQrSaveLocation.Location = new System.Drawing.Point(12, 59);
            this.lbQrSaveLocation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbQrSaveLocation.Name = "lbQrSaveLocation";
            this.lbQrSaveLocation.Size = new System.Drawing.Size(311, 16);
            this.lbQrSaveLocation.TabIndex = 18;
            this.lbQrSaveLocation.Text = "Putanja spremanja kodova :";
            this.lbQrSaveLocation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabOsnovno);
            this.tabControl.Controls.Add(this.tabPostavke);
            this.tabControl.Controls.Add(this.tabSolo);
            this.tabControl.Controls.Add(this.tabExtraOpcije);
            this.tabControl.Controls.Add(this.tabCertifikati);
            this.tabControl.Controls.Add(this.tabQr);
            this.tabControl.Location = new System.Drawing.Point(6, 62);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(800, 493);
            this.tabControl.TabIndex = 210;
            // 
            // tabOsnovno
            // 
            this.tabOsnovno.Controls.Add(this.grpBasic);
            this.tabOsnovno.Location = new System.Drawing.Point(4, 25);
            this.tabOsnovno.Name = "tabOsnovno";
            this.tabOsnovno.Padding = new System.Windows.Forms.Padding(3);
            this.tabOsnovno.Size = new System.Drawing.Size(792, 464);
            this.tabOsnovno.TabIndex = 0;
            this.tabOsnovno.Text = "Osnovno";
            this.tabOsnovno.UseVisualStyleBackColor = true;
            // 
            // tabPostavke
            // 
            this.tabPostavke.Controls.Add(this.grpPremise);
            this.tabPostavke.Location = new System.Drawing.Point(4, 25);
            this.tabPostavke.Name = "tabPostavke";
            this.tabPostavke.Padding = new System.Windows.Forms.Padding(3);
            this.tabPostavke.Size = new System.Drawing.Size(792, 464);
            this.tabPostavke.TabIndex = 1;
            this.tabPostavke.Text = "Postavke salona";
            this.tabPostavke.UseVisualStyleBackColor = true;
            // 
            // tabSolo
            // 
            this.tabSolo.Controls.Add(this.grpSolo);
            this.tabSolo.Location = new System.Drawing.Point(4, 25);
            this.tabSolo.Name = "tabSolo";
            this.tabSolo.Padding = new System.Windows.Forms.Padding(3);
            this.tabSolo.Size = new System.Drawing.Size(792, 464);
            this.tabSolo.TabIndex = 2;
            this.tabSolo.Text = "Solo postavke";
            this.tabSolo.UseVisualStyleBackColor = true;
            // 
            // tabExtraOpcije
            // 
            this.tabExtraOpcije.Controls.Add(this.grpExtraOptions);
            this.tabExtraOpcije.Location = new System.Drawing.Point(4, 25);
            this.tabExtraOpcije.Name = "tabExtraOpcije";
            this.tabExtraOpcije.Padding = new System.Windows.Forms.Padding(3);
            this.tabExtraOpcije.Size = new System.Drawing.Size(792, 464);
            this.tabExtraOpcije.TabIndex = 3;
            this.tabExtraOpcije.Text = "Dodatne opcije";
            this.tabExtraOpcije.UseVisualStyleBackColor = true;
            // 
            // tabCertifikati
            // 
            this.tabCertifikati.Controls.Add(this.grpCertificateFile);
            this.tabCertifikati.Controls.Add(this.grpCertificate);
            this.tabCertifikati.Location = new System.Drawing.Point(4, 25);
            this.tabCertifikati.Name = "tabCertifikati";
            this.tabCertifikati.Padding = new System.Windows.Forms.Padding(3);
            this.tabCertifikati.Size = new System.Drawing.Size(792, 464);
            this.tabCertifikati.TabIndex = 4;
            this.tabCertifikati.Text = "Certifikati";
            this.tabCertifikati.UseVisualStyleBackColor = true;
            // 
            // tabQr
            // 
            this.tabQr.Controls.Add(this.grpQrCode);
            this.tabQr.Location = new System.Drawing.Point(4, 25);
            this.tabQr.Name = "tabQr";
            this.tabQr.Padding = new System.Windows.Forms.Padding(3);
            this.tabQr.Size = new System.Drawing.Size(792, 464);
            this.tabQr.TabIndex = 5;
            this.tabQr.Text = "QR code";
            this.tabQr.UseVisualStyleBackColor = true;
            // 
            // Config
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(827, 653);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.lbVerzija);
            this.Controls.Add(this.btUpdate);
            this.Controls.Add(this.btnProcessBills);
            this.Controls.Add(this.lblErrorMessage);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnSaveAndQuit);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnUnlockSettings);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "Config";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CIS Postavke";
            this.Load += new System.EventHandler(this.Config_Load);
            this.grpBasic.ResumeLayout(false);
            this.grpBasic.PerformLayout();
            this.grpPremise.ResumeLayout(false);
            this.grpPremise.PerformLayout();
            this.grpSolo.ResumeLayout(false);
            this.grpSolo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.grpCertificate.ResumeLayout(false);
            this.grpCertificate.PerformLayout();
            this.grpExtraOptions.ResumeLayout(false);
            this.grpCertificateFile.ResumeLayout(false);
            this.grpCertificateFile.PerformLayout();
            this.grpQrCode.ResumeLayout(false);
            this.grpQrCode.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabOsnovno.ResumeLayout(false);
            this.tabPostavke.ResumeLayout(false);
            this.tabSolo.ResumeLayout(false);
            this.tabExtraOpcije.ResumeLayout(false);
            this.tabCertifikati.ResumeLayout(false);
            this.tabQr.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

  }



    #endregion

   
}
