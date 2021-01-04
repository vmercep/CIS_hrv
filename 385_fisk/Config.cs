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

    private GroupBox grpTestCertificate;

    private CheckBox chkSendTest;

    private Button btnProcessBills;

    private GroupBox grpCertificateFile;

    private CheckBox chkUseCertificateFile;

    private Label label1;

    private Button btnCertificateFile;

    private Label label2;
    private CheckBox cbCheckSSL;
    private Button btUpdate;
    private Label lbVerzija;
    private CheckBox cbFiskPonude;
    private TextBox txtQrMessage;
    private Label label3;
    private GroupBox groupBox1;
    private TextBox txtQrSaveLocation;
    private Label label4;
    private TextBox txtQrSize;
    private Label label5;
    private ComboBox cmbLogLevel;
    private Label lbloglevel;
    private Button btQrRegen;
    private ProgressBar pgBarQr;
    private TextBox txtCertificatePassword;

    QrCodeRegen regen = new QrCodeRegen();

    public Config()
    {
        InitializeComponent();
    }

    private void populateTranslations()
    {
        Text = Translations.Translate(Text);
        tooltip1.SetToolTip(btnUnlockSettings, Translations.Translate("Zaštićene postavke"));
        //tooltip1.SetToolTip(btnSendData, Translations.Translate("Pošalji u poreznu upravu"));
        tooltip1.SetToolTip(btnSaveAndQuit, Translations.Translate("Spremi / Izađi"));
        tooltip1.SetToolTip(btnQuit, Translations.Translate("Izađi bez spremanja"));
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
        grpTestCertificate.Text = Translations.Translate(grpTestCertificate.Text);
        chkSendTest.Text = Translations.Translate(chkSendTest.Text);
        lbVerzija.Text = "Verzija programa: CIS_HRV_ " + Assembly.GetEntryAssembly().GetName().Version;
        LogFile.LogToFile("Config screen loaded!", LogLevel.Debug);
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
        LogFile.LogToFile("Config screen loading!", LogLevel.Debug);
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
        LogFile.LogToFile("Unlock setting screen!", LogLevel.Debug);
        string text = inputBox(Translations.Translate("Unesite tehničku šifru"), Translations.Translate("Tehnička šifra"), "");
        string techCode = AppLink.GetTechCode();

        if (text == techCode) 
        {
                      
            LogFile.LogToFile("Settings unlocked!", LogLevel.Debug);          
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
                LogFile.LogToFile("Wrong password for setting screen entered!", LogLevel.Debug);
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


            directoryInfo = new DirectoryInfo(Environment.CurrentDirectory + "\\FiskXMLMessages\\Invoice\\Requests\\");
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            directoryInfo = new DirectoryInfo(Environment.CurrentDirectory + "\\FiskXMLMessages\\Invoice\\Response\\");
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
        LogFile.LogToFile("Config saved!",LogLevel.Debug);
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
        errorMessageBox.lbMessageDetails.Text = content;
        errorMessageBox.ShowDialog();

        LogFile.LogToFile("Error in QR code regen " + content, LogLevel.Debug);
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
            MessageAlert("Qr kodovi generirani", "QrCode generator");
            pgBarQr.Visible = false;
        }
        catch(Exception ex)
        {
            LogFile.LogToFile("Error in QR code regen " +ex.Message, LogLevel.Debug);
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
            this.label3 = new System.Windows.Forms.Label();
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
            this.grpTestCertificate = new System.Windows.Forms.GroupBox();
            this.cbFiskPonude = new System.Windows.Forms.CheckBox();
            this.chkSendTest = new System.Windows.Forms.CheckBox();
            this.btnProcessBills = new System.Windows.Forms.Button();
            this.grpCertificateFile = new System.Windows.Forms.GroupBox();
            this.cbCheckSSL = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtCertificatePassword = new System.Windows.Forms.TextBox();
            this.btnCertificateFile = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.chkUseCertificateFile = new System.Windows.Forms.CheckBox();
            this.btUpdate = new System.Windows.Forms.Button();
            this.lbVerzija = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pgBarQr = new System.Windows.Forms.ProgressBar();
            this.btQrRegen = new System.Windows.Forms.Button();
            this.txtQrSize = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtQrSaveLocation = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.grpBasic.SuspendLayout();
            this.grpPremise.SuspendLayout();
            this.grpSolo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.grpCertificate.SuspendLayout();
            this.grpTestCertificate.SuspendLayout();
            this.grpCertificateFile.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSaveAndQuit
            // 
            this.btnSaveAndQuit.BackColor = System.Drawing.Color.Transparent;
            this.btnSaveAndQuit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveAndQuit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveAndQuit.Image = ((System.Drawing.Image)(resources.GetObject("btnSaveAndQuit.Image")));
            this.btnSaveAndQuit.Location = new System.Drawing.Point(1502, 15);
            this.btnSaveAndQuit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSaveAndQuit.Name = "btnSaveAndQuit";
            this.btnSaveAndQuit.Size = new System.Drawing.Size(62, 63);
            this.btnSaveAndQuit.TabIndex = 0;
            this.btnSaveAndQuit.UseVisualStyleBackColor = false;
            this.btnSaveAndQuit.Click += new System.EventHandler(this.btnSaveAndQuitClick);
            // 
            // lblConnectionString
            // 
            this.lblConnectionString.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnectionString.Location = new System.Drawing.Point(14, 31);
            this.lblConnectionString.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblConnectionString.Name = "lblConnectionString";
            this.lblConnectionString.Size = new System.Drawing.Size(350, 20);
            this.lblConnectionString.TabIndex = 1;
            this.lblConnectionString.Text = "Connection string :";
            this.lblConnectionString.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtConnectString
            // 
            this.txtConnectString.BackColor = System.Drawing.SystemColors.Window;
            this.txtConnectString.Enabled = false;
            this.txtConnectString.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConnectString.Location = new System.Drawing.Point(372, 28);
            this.txtConnectString.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtConnectString.Name = "txtConnectString";
            this.txtConnectString.Size = new System.Drawing.Size(448, 26);
            this.txtConnectString.TabIndex = 2;
            // 
            // txtURL
            // 
            this.txtURL.BackColor = System.Drawing.SystemColors.Window;
            this.txtURL.Enabled = false;
            this.txtURL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtURL.Location = new System.Drawing.Point(372, 112);
            this.txtURL.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtURL.Name = "txtURL";
            this.txtURL.Size = new System.Drawing.Size(448, 26);
            this.txtURL.TabIndex = 4;
            // 
            // lblURL
            // 
            this.lblURL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblURL.Location = new System.Drawing.Point(14, 117);
            this.lblURL.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblURL.Name = "lblURL";
            this.lblURL.Size = new System.Drawing.Size(350, 20);
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
            this.chkLogFileActive.Location = new System.Drawing.Point(14, 158);
            this.chkLogFileActive.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkLogFileActive.Name = "chkLogFileActive";
            this.chkLogFileActive.Size = new System.Drawing.Size(378, 25);
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
            this.dtpDateActive.Location = new System.Drawing.Point(372, 306);
            this.dtpDateActive.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dtpDateActive.Name = "dtpDateActive";
            this.dtpDateActive.Size = new System.Drawing.Size(166, 26);
            this.dtpDateActive.TabIndex = 6;
            this.dtpDateActive.Value = new System.DateTime(2013, 4, 4, 0, 0, 0, 0);
            // 
            // lblDateActive
            // 
            this.lblDateActive.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDateActive.Location = new System.Drawing.Point(14, 311);
            this.lblDateActive.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDateActive.Name = "lblDateActive";
            this.lblDateActive.Size = new System.Drawing.Size(350, 20);
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
            this.grpBasic.Location = new System.Drawing.Point(18, 65);
            this.grpBasic.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpBasic.Name = "grpBasic";
            this.grpBasic.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpBasic.Size = new System.Drawing.Size(832, 406);
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
            this.cmbLogLevel.Location = new System.Drawing.Point(370, 188);
            this.cmbLogLevel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbLogLevel.Name = "cmbLogLevel";
            this.cmbLogLevel.Size = new System.Drawing.Size(166, 28);
            this.cmbLogLevel.TabIndex = 17;
            // 
            // lbloglevel
            // 
            this.lbloglevel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbloglevel.Location = new System.Drawing.Point(14, 192);
            this.lbloglevel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbloglevel.Name = "lbloglevel";
            this.lbloglevel.Size = new System.Drawing.Size(350, 20);
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
            this.cmbActiveLanguage.Location = new System.Drawing.Point(372, 345);
            this.cmbActiveLanguage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbActiveLanguage.Name = "cmbActiveLanguage";
            this.cmbActiveLanguage.Size = new System.Drawing.Size(166, 28);
            this.cmbActiveLanguage.TabIndex = 15;
            // 
            // lblActiveLanguage
            // 
            this.lblActiveLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblActiveLanguage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblActiveLanguage.Location = new System.Drawing.Point(14, 346);
            this.lblActiveLanguage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblActiveLanguage.Name = "lblActiveLanguage";
            this.lblActiveLanguage.Size = new System.Drawing.Size(350, 25);
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
            this.cmbConnectionEncryption.Location = new System.Drawing.Point(372, 69);
            this.cmbConnectionEncryption.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbConnectionEncryption.Name = "cmbConnectionEncryption";
            this.cmbConnectionEncryption.Size = new System.Drawing.Size(166, 28);
            this.cmbConnectionEncryption.TabIndex = 13;
            // 
            // lblConnectionEncryption
            // 
            this.lblConnectionEncryption.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnectionEncryption.Location = new System.Drawing.Point(14, 75);
            this.lblConnectionEncryption.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblConnectionEncryption.Name = "lblConnectionEncryption";
            this.lblConnectionEncryption.Size = new System.Drawing.Size(350, 20);
            this.lblConnectionEncryption.TabIndex = 12;
            this.lblConnectionEncryption.Text = "Connection encryption :";
            this.lblConnectionEncryption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnXmlPathChoose
            // 
            this.btnXmlPathChoose.AutoSize = true;
            this.btnXmlPathChoose.Enabled = false;
            this.btnXmlPathChoose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnXmlPathChoose.Location = new System.Drawing.Point(370, 258);
            this.btnXmlPathChoose.Name = "btnXmlPathChoose";
            this.btnXmlPathChoose.Size = new System.Drawing.Size(168, 46);
            this.btnXmlPathChoose.TabIndex = 11;
            this.btnXmlPathChoose.Text = "Odabir";
            this.btnXmlPathChoose.UseVisualStyleBackColor = true;
            this.btnXmlPathChoose.Click += new System.EventHandler(this.btnChooseClick);
            // 
            // lblXmlPath
            // 
            this.lblXmlPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblXmlPath.Location = new System.Drawing.Point(14, 266);
            this.lblXmlPath.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblXmlPath.Name = "lblXmlPath";
            this.lblXmlPath.Size = new System.Drawing.Size(350, 20);
            this.lblXmlPath.TabIndex = 10;
            this.lblXmlPath.Text = "Putanja spremanja XML datoteka :";
            this.lblXmlPath.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chkSaveXmlActive
            // 
            this.chkSaveXmlActive.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkSaveXmlActive.Enabled = false;
            this.chkSaveXmlActive.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.chkSaveXmlActive.Location = new System.Drawing.Point(14, 225);
            this.chkSaveXmlActive.Name = "chkSaveXmlActive";
            this.chkSaveXmlActive.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkSaveXmlActive.Size = new System.Drawing.Size(378, 26);
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
            this.txtQrMessage.Location = new System.Drawing.Point(370, 32);
            this.txtQrMessage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtQrMessage.Name = "txtQrMessage";
            this.txtQrMessage.Size = new System.Drawing.Size(392, 26);
            this.txtQrMessage.TabIndex = 17;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(14, 37);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(350, 20);
            this.label3.TabIndex = 16;
            this.label3.Text = "QR poruka :";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            this.grpPremise.Location = new System.Drawing.Point(18, 468);
            this.grpPremise.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpPremise.Name = "grpPremise";
            this.grpPremise.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpPremise.Size = new System.Drawing.Size(832, 246);
            this.grpPremise.TabIndex = 9;
            this.grpPremise.TabStop = false;
            this.grpPremise.Text = "Postavke salona :";
            // 
            // lblOibSoftware
            // 
            this.lblOibSoftware.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOibSoftware.Location = new System.Drawing.Point(14, 202);
            this.lblOibSoftware.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOibSoftware.Name = "lblOibSoftware";
            this.lblOibSoftware.Size = new System.Drawing.Size(350, 20);
            this.lblOibSoftware.TabIndex = 27;
            this.lblOibSoftware.Text = "OIB proizvođača programa :";
            this.lblOibSoftware.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtOibSoftware
            // 
            this.txtOibSoftware.Enabled = false;
            this.txtOibSoftware.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOibSoftware.Location = new System.Drawing.Point(372, 195);
            this.txtOibSoftware.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtOibSoftware.Name = "txtOibSoftware";
            this.txtOibSoftware.Size = new System.Drawing.Size(448, 26);
            this.txtOibSoftware.TabIndex = 28;
            // 
            // lblBillingDeviceMark
            // 
            this.lblBillingDeviceMark.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBillingDeviceMark.Location = new System.Drawing.Point(14, 165);
            this.lblBillingDeviceMark.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBillingDeviceMark.Name = "lblBillingDeviceMark";
            this.lblBillingDeviceMark.Size = new System.Drawing.Size(350, 20);
            this.lblBillingDeviceMark.TabIndex = 25;
            this.lblBillingDeviceMark.Text = "Oznaka naplatnog uređaja :";
            this.lblBillingDeviceMark.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtBillingDeviceMark
            // 
            this.txtBillingDeviceMark.Enabled = false;
            this.txtBillingDeviceMark.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBillingDeviceMark.Location = new System.Drawing.Point(372, 158);
            this.txtBillingDeviceMark.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtBillingDeviceMark.Name = "txtBillingDeviceMark";
            this.txtBillingDeviceMark.Size = new System.Drawing.Size(448, 26);
            this.txtBillingDeviceMark.TabIndex = 26;
            // 
            // lblPremiseMark
            // 
            this.lblPremiseMark.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPremiseMark.Location = new System.Drawing.Point(14, 128);
            this.lblPremiseMark.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPremiseMark.Name = "lblPremiseMark";
            this.lblPremiseMark.Size = new System.Drawing.Size(350, 20);
            this.lblPremiseMark.TabIndex = 23;
            this.lblPremiseMark.Text = "Oznaka prostora :";
            this.lblPremiseMark.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPremiseMark
            // 
            this.txtPremiseMark.Enabled = false;
            this.txtPremiseMark.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPremiseMark.Location = new System.Drawing.Point(372, 122);
            this.txtPremiseMark.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtPremiseMark.Name = "txtPremiseMark";
            this.txtPremiseMark.Size = new System.Drawing.Size(448, 26);
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
            this.chkVatActive.Location = new System.Drawing.Point(18, 89);
            this.chkVatActive.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkVatActive.Name = "chkVatActive";
            this.chkVatActive.Size = new System.Drawing.Size(374, 23);
            this.chkVatActive.TabIndex = 6;
            this.chkVatActive.Text = "U sustavu PDV :";
            this.chkVatActive.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkVatActive.UseVisualStyleBackColor = false;
            // 
            // lblOIB
            // 
            this.lblOIB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOIB.Location = new System.Drawing.Point(14, 51);
            this.lblOIB.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOIB.Name = "lblOIB";
            this.lblOIB.Size = new System.Drawing.Size(350, 20);
            this.lblOIB.TabIndex = 3;
            this.lblOIB.Text = "OIB :";
            this.lblOIB.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtVatNumber
            // 
            this.txtVatNumber.Enabled = false;
            this.txtVatNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVatNumber.Location = new System.Drawing.Point(372, 48);
            this.txtVatNumber.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtVatNumber.Name = "txtVatNumber";
            this.txtVatNumber.Size = new System.Drawing.Size(448, 26);
            this.txtVatNumber.TabIndex = 4;
            // 
            // btnUnlockSettings
            // 
            this.btnUnlockSettings.BackColor = System.Drawing.Color.Transparent;
            this.btnUnlockSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUnlockSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUnlockSettings.Image = ((System.Drawing.Image)(resources.GetObject("btnUnlockSettings.Image")));
            this.btnUnlockSettings.Location = new System.Drawing.Point(1292, 15);
            this.btnUnlockSettings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnUnlockSettings.Name = "btnUnlockSettings";
            this.btnUnlockSettings.Size = new System.Drawing.Size(62, 63);
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
            this.grpSolo.Location = new System.Drawing.Point(18, 723);
            this.grpSolo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpSolo.Name = "grpSolo";
            this.grpSolo.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpSolo.Size = new System.Drawing.Size(832, 122);
            this.grpSolo.TabIndex = 10;
            this.grpSolo.TabStop = false;
            this.grpSolo.Text = "Solo postavke :";
            // 
            // lblCodeOperator
            // 
            this.lblCodeOperator.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCodeOperator.Location = new System.Drawing.Point(18, 78);
            this.lblCodeOperator.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCodeOperator.Name = "lblCodeOperator";
            this.lblCodeOperator.Size = new System.Drawing.Size(345, 20);
            this.lblCodeOperator.TabIndex = 7;
            this.lblCodeOperator.Text = "Šifra operatera :";
            this.lblCodeOperator.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtCodeOperator
            // 
            this.txtCodeOperator.BackColor = System.Drawing.SystemColors.Window;
            this.txtCodeOperator.Enabled = false;
            this.txtCodeOperator.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCodeOperator.Location = new System.Drawing.Point(370, 74);
            this.txtCodeOperator.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtCodeOperator.Name = "txtCodeOperator";
            this.txtCodeOperator.Size = new System.Drawing.Size(450, 26);
            this.txtCodeOperator.TabIndex = 8;
            // 
            // lblOibOperator
            // 
            this.lblOibOperator.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOibOperator.Location = new System.Drawing.Point(14, 34);
            this.lblOibOperator.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOibOperator.Name = "lblOibOperator";
            this.lblOibOperator.Size = new System.Drawing.Size(350, 20);
            this.lblOibOperator.TabIndex = 5;
            this.lblOibOperator.Text = "OIB operatera :";
            this.lblOibOperator.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtOibOperator
            // 
            this.txtOibOperator.BackColor = System.Drawing.SystemColors.Window;
            this.txtOibOperator.Enabled = false;
            this.txtOibOperator.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOibOperator.Location = new System.Drawing.Point(370, 29);
            this.txtOibOperator.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtOibOperator.Name = "txtOibOperator";
            this.txtOibOperator.Size = new System.Drawing.Size(450, 26);
            this.txtOibOperator.TabIndex = 6;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 5);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(50, 15);
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
            this.btnQuit.Location = new System.Drawing.Point(1431, 15);
            this.btnQuit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(62, 63);
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
            this.grpCertificate.Location = new System.Drawing.Point(860, 428);
            this.grpCertificate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpCertificate.Name = "grpCertificate";
            this.grpCertificate.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpCertificate.Size = new System.Drawing.Size(774, 89);
            this.grpCertificate.TabIndex = 203;
            this.grpCertificate.TabStop = false;
            this.grpCertificate.Text = "Certifikat :";
            // 
            // lblCertificate
            // 
            this.lblCertificate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCertificate.Location = new System.Drawing.Point(22, 40);
            this.lblCertificate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCertificate.Name = "lblCertificate";
            this.lblCertificate.Size = new System.Drawing.Size(340, 20);
            this.lblCertificate.TabIndex = 5;
            this.lblCertificate.Text = "Naziv certifikata :";
            this.lblCertificate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtCertificate
            // 
            this.txtCertificate.BackColor = System.Drawing.SystemColors.Window;
            this.txtCertificate.Enabled = false;
            this.txtCertificate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCertificate.Location = new System.Drawing.Point(370, 35);
            this.txtCertificate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtCertificate.Name = "txtCertificate";
            this.txtCertificate.Size = new System.Drawing.Size(392, 26);
            this.txtCertificate.TabIndex = 6;
            // 
            // lblErrorMessage
            // 
            this.lblErrorMessage.AutoSize = true;
            this.lblErrorMessage.BackColor = System.Drawing.Color.Transparent;
            this.lblErrorMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblErrorMessage.ForeColor = System.Drawing.Color.Red;
            this.lblErrorMessage.Location = new System.Drawing.Point(18, 29);
            this.lblErrorMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblErrorMessage.Name = "lblErrorMessage";
            this.lblErrorMessage.Size = new System.Drawing.Size(476, 29);
            this.lblErrorMessage.TabIndex = 204;
            this.lblErrorMessage.Text = "Neke postavke su prazne ili neispravne!";
            this.lblErrorMessage.Visible = false;
            // 
            // grpTestCertificate
            // 
            this.grpTestCertificate.BackColor = System.Drawing.Color.Transparent;
            this.grpTestCertificate.Controls.Add(this.cbFiskPonude);
            this.grpTestCertificate.Controls.Add(this.chkSendTest);
            this.grpTestCertificate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpTestCertificate.Location = new System.Drawing.Point(856, 75);
            this.grpTestCertificate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpTestCertificate.Name = "grpTestCertificate";
            this.grpTestCertificate.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpTestCertificate.Size = new System.Drawing.Size(777, 132);
            this.grpTestCertificate.TabIndex = 205;
            this.grpTestCertificate.TabStop = false;
            this.grpTestCertificate.Text = "Dodatne opcije :";
            // 
            // cbFiskPonude
            // 
            this.cbFiskPonude.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.cbFiskPonude.BackColor = System.Drawing.Color.Transparent;
            this.cbFiskPonude.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbFiskPonude.Enabled = false;
            this.cbFiskPonude.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbFiskPonude.Location = new System.Drawing.Point(18, 80);
            this.cbFiskPonude.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbFiskPonude.Name = "cbFiskPonude";
            this.cbFiskPonude.Size = new System.Drawing.Size(374, 38);
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
            this.chkSendTest.Location = new System.Drawing.Point(18, 32);
            this.chkSendTest.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkSendTest.Name = "chkSendTest";
            this.chkSendTest.Size = new System.Drawing.Size(374, 38);
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
            this.btnProcessBills.Location = new System.Drawing.Point(1360, 15);
            this.btnProcessBills.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnProcessBills.Name = "btnProcessBills";
            this.btnProcessBills.Size = new System.Drawing.Size(62, 63);
            this.btnProcessBills.TabIndex = 206;
            this.btnProcessBills.UseVisualStyleBackColor = false;
            this.btnProcessBills.Click += new System.EventHandler(this.btnProcessBills_Click);
            // 
            // grpCertificateFile
            // 
            this.grpCertificateFile.BackColor = System.Drawing.Color.Transparent;
            this.grpCertificateFile.Controls.Add(this.cbCheckSSL);
            this.grpCertificateFile.Controls.Add(this.label2);
            this.grpCertificateFile.Controls.Add(this.txtCertificatePassword);
            this.grpCertificateFile.Controls.Add(this.btnCertificateFile);
            this.grpCertificateFile.Controls.Add(this.label1);
            this.grpCertificateFile.Controls.Add(this.chkUseCertificateFile);
            this.grpCertificateFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpCertificateFile.Location = new System.Drawing.Point(856, 223);
            this.grpCertificateFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpCertificateFile.Name = "grpCertificateFile";
            this.grpCertificateFile.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grpCertificateFile.Size = new System.Drawing.Size(777, 195);
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
            this.cbCheckSSL.Location = new System.Drawing.Point(16, 140);
            this.cbCheckSSL.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cbCheckSSL.Name = "cbCheckSSL";
            this.cbCheckSSL.Size = new System.Drawing.Size(374, 55);
            this.cbCheckSSL.TabIndex = 16;
            this.cbCheckSSL.Text = "Ignoriraj SSL certifikate:";
            this.cbCheckSSL.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbCheckSSL.UseVisualStyleBackColor = false;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(22, 115);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(340, 20);
            this.label2.TabIndex = 14;
            this.label2.Text = "Lozinka certifikata :";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtCertificatePassword
            // 
            this.txtCertificatePassword.BackColor = System.Drawing.SystemColors.Window;
            this.txtCertificatePassword.Enabled = false;
            this.txtCertificatePassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCertificatePassword.Location = new System.Drawing.Point(370, 111);
            this.txtCertificatePassword.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtCertificatePassword.Name = "txtCertificatePassword";
            this.txtCertificatePassword.Size = new System.Drawing.Size(396, 26);
            this.txtCertificatePassword.TabIndex = 15;
            // 
            // btnCertificateFile
            // 
            this.btnCertificateFile.AutoSize = true;
            this.btnCertificateFile.Enabled = false;
            this.btnCertificateFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnCertificateFile.Location = new System.Drawing.Point(370, 66);
            this.btnCertificateFile.Name = "btnCertificateFile";
            this.btnCertificateFile.Size = new System.Drawing.Size(168, 46);
            this.btnCertificateFile.TabIndex = 13;
            this.btnCertificateFile.Text = "Odabir";
            this.btnCertificateFile.UseVisualStyleBackColor = true;
            this.btnCertificateFile.Click += new System.EventHandler(this.btnCertificateFile_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, 74);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(350, 20);
            this.label1.TabIndex = 12;
            this.label1.Text = "Putanja datoteke s certifikatom :";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chkUseCertificateFile
            // 
            this.chkUseCertificateFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.chkUseCertificateFile.BackColor = System.Drawing.Color.Transparent;
            this.chkUseCertificateFile.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkUseCertificateFile.Enabled = false;
            this.chkUseCertificateFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkUseCertificateFile.Location = new System.Drawing.Point(18, 17);
            this.chkUseCertificateFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chkUseCertificateFile.Name = "chkUseCertificateFile";
            this.chkUseCertificateFile.Size = new System.Drawing.Size(374, 55);
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
            this.btUpdate.Location = new System.Drawing.Point(1569, 15);
            this.btUpdate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btUpdate.Name = "btUpdate";
            this.btUpdate.Size = new System.Drawing.Size(62, 63);
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
            this.lbVerzija.Location = new System.Drawing.Point(14, 872);
            this.lbVerzija.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbVerzija.Name = "lbVerzija";
            this.lbVerzija.Size = new System.Drawing.Size(79, 25);
            this.lbVerzija.TabIndex = 209;
            this.lbVerzija.Text = "Verzija";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.pgBarQr);
            this.groupBox1.Controls.Add(this.btQrRegen);
            this.groupBox1.Controls.Add(this.txtQrSize);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtQrSaveLocation);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtQrMessage);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(860, 526);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(774, 318);
            this.groupBox1.TabIndex = 29;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Postavke QR Coda :";
            // 
            // pgBarQr
            // 
            this.pgBarQr.Location = new System.Drawing.Point(370, 205);
            this.pgBarQr.Name = "pgBarQr";
            this.pgBarQr.Size = new System.Drawing.Size(392, 46);
            this.pgBarQr.TabIndex = 22;
            this.pgBarQr.Visible = false;
            // 
            // btQrRegen
            // 
            this.btQrRegen.AutoSize = true;
            this.btQrRegen.Enabled = false;
            this.btQrRegen.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btQrRegen.Location = new System.Drawing.Point(370, 152);
            this.btQrRegen.Name = "btQrRegen";
            this.btQrRegen.Size = new System.Drawing.Size(300, 46);
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
            this.txtQrSize.Location = new System.Drawing.Point(369, 108);
            this.txtQrSize.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtQrSize.Name = "txtQrSize";
            this.txtQrSize.Size = new System.Drawing.Size(80, 26);
            this.txtQrSize.TabIndex = 21;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(12, 112);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(350, 20);
            this.label5.TabIndex = 20;
            this.label5.Text = "Veličina QR koda :";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtQrSaveLocation
            // 
            this.txtQrSaveLocation.BackColor = System.Drawing.SystemColors.Window;
            this.txtQrSaveLocation.Enabled = false;
            this.txtQrSaveLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtQrSaveLocation.Location = new System.Drawing.Point(370, 69);
            this.txtQrSaveLocation.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtQrSaveLocation.Name = "txtQrSaveLocation";
            this.txtQrSaveLocation.Size = new System.Drawing.Size(392, 26);
            this.txtQrSaveLocation.TabIndex = 19;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(14, 74);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(350, 20);
            this.label4.TabIndex = 18;
            this.label4.Text = "Putanja spremanja kodova :";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Config
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1646, 909);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lbVerzija);
            this.Controls.Add(this.btUpdate);
            this.Controls.Add(this.grpCertificateFile);
            this.Controls.Add(this.btnProcessBills);
            this.Controls.Add(this.grpTestCertificate);
            this.Controls.Add(this.lblErrorMessage);
            this.Controls.Add(this.grpCertificate);
            this.Controls.Add(this.btnQuit);
            this.Controls.Add(this.btnSaveAndQuit);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnUnlockSettings);
            this.Controls.Add(this.grpSolo);
            this.Controls.Add(this.grpPremise);
            this.Controls.Add(this.grpBasic);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
            this.grpTestCertificate.ResumeLayout(false);
            this.grpCertificateFile.ResumeLayout(false);
            this.grpCertificateFile.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

  }



    #endregion

   
}
