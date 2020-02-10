namespace Helper
{

   


    public class ConfigFile
    {
        public string Lvide;

        public string SecGeneral;

        private string _ConnectionString;

        private string _ConnectionEncryption;

        private string _ServerUrl;

        private string _LogFileActive;

        private string _SaveXMLActive;

        private string _XMLSavePath;

        private string _DateIsActive;

        public string SecSalon;

        private string _VATNumber;

        private string _InVATsystem;

        private string _PremiseMark;

        private string _BillingDeviceMark;

        private string _OIBSoftware;

        public string SecSoloconfig;

        public string SecCertificate;

        private string _OperatorOIB;

        private string _OperatorCode;

        private string _PCeasedOperation;

        private string _Certifikat;

        public string SecCertificateFile;

        private string _UseCertificateFile;

        private string _CertificatePassword;

        private string _ActiveLanguage;

        public string SecTestCertificate;

        private string _SendTestReceipts;

        private string _TestServerUrl;

        private string _SendPonudaToFisk;

        private string _IgnoreSSLCertificates;

        public string ConnectionString
        {
            get
            {
                return _ConnectionString;
            }
            set
            {
                _ConnectionString = "ConnectionString=" + value;
            }
        }

        public string ConnectionEncryption
        {
            get
            {
                return _ConnectionEncryption;
            }
            set
            {
                _ConnectionEncryption = "ConnectionEncryption=" + value;
            }
        }

        public string ServerUrl
        {
            get
            {
                return _ServerUrl;
            }
            set
            {
                _ServerUrl = "ServerUrl=" + value;
            }
        }

        public string LogFileActive
        {
            get
            {
                return _LogFileActive;
            }
            set
            {
                if (value == "True")
                {
                    _LogFileActive = "LogFileActive=1";
                }
                else
                {
                    _LogFileActive = "LogFileActive=0";
                }
            }
        }

        public string SaveXMLActive
        {
            get
            {
                return _SaveXMLActive;
            }
            set
            {
                if (value == "True")
                {
                    _SaveXMLActive = "SaveXMLActive=1";
                }
                else
                {
                    _SaveXMLActive = "SaveXMLActive=0";
                }
            }
        }

        public string XMLSavePath
        {
            get
            {
                return _XMLSavePath;
            }
            set
            {
                _XMLSavePath = "XMLSavePath=" + value;
            }
        }

        public string DateIsActive
        {
            get
            {
                return _DateIsActive;
            }
            set
            {
                _DateIsActive = "DateIsActive=" + value;
            }
        }

        public string VATNumber
        {
            get
            {
                return _VATNumber;
            }
            set
            {
                _VATNumber = "VATNumber=" + value;
            }
        }

        public string InVATsystem
        {
            get
            {
                return _InVATsystem;
            }
            set
            {
                if (value == "True")
                {
                    _InVATsystem = "InVATsystem=1";
                }
                else
                {
                    _InVATsystem = "InVATsystem=0";
                }
            }
        }

        public string PremiseMark
        {
            get
            {
                return _PremiseMark;
            }
            set
            {
                _PremiseMark = "PremiseMark=" + value;
            }
        }

        public string BillingDeviceMark
        {
            get
            {
                return _BillingDeviceMark;
            }
            set
            {
                _BillingDeviceMark = "BillingDeviceMark=" + value;
            }
        }

        public string OIBSoftware
        {
            get
            {
                return _OIBSoftware;
            }
            set
            {
                _OIBSoftware = "OIBSoftware=" + value;
            }
        }

        public string OperatorOIB
        {
            get
            {
                return _OperatorOIB;
            }
            set
            {
                _OperatorOIB = "OperatorOIB=" + value;
            }
        }

        public string OperatorCode
        {
            get
            {
                return _OperatorCode;
            }
            set
            {
                _OperatorCode = "OperatorCode=" + value;
            }
        }

        public string PCeasedOperation
        {
            get
            {
                return _PCeasedOperation;
            }
            set
            {
                if (value == "True")
                {
                    _PCeasedOperation = "PCeasedOperation=1";
                }
                else
                {
                    _PCeasedOperation = "PCeasedOperation=0";
                }
            }
        }

        public string Certifikat
        {
            get
            {
                return _Certifikat;
            }
            set
            {
                _Certifikat = "Certificate=" + value;
            }
        }

        public string UseCertificateFile
        {
            get
            {
                return _UseCertificateFile;
            }
            set
            {
                if (value == "True")
                {
                    _UseCertificateFile = "UseCertificateFile=1";
                }
                else
                {
                    _UseCertificateFile = "UseCertificateFile=0";
                }
            }
        }

        public string CertificatePassword
        {
            get
            {
                return _CertificatePassword;
            }
            set
            {
                _CertificatePassword = "CertificatePassword=" + value;
            }
        }

        public string ActiveLanguage
        {
            get
            {
                return _ActiveLanguage;
            }
            set
            {
                _ActiveLanguage = "ActiveLanguage=" + value;
            }
        }

        public string SendTestReceipts
        {
            get
            {
                return _SendTestReceipts;
            }
            set
            {
                if (value == "True")
                {
                    _SendTestReceipts = "SendTestReceipts=1";
                }
                else
                {
                    _SendTestReceipts = "SendTestReceipts=0";
                }
            }
        }

        public string TestServerUrl
        {
            get
            {
                return _TestServerUrl;
            }
            set
            {
                _TestServerUrl = "TestServerUrl=" + value;
            }
        }

        public string SendPonudaToFisk
        {
            get
            {
                return _SendPonudaToFisk;
            }
            set
            {
                if (value == "True")
                {
                    _SendPonudaToFisk = "SendPonudaToFisk=1";
                }
                else
                {
                    _SendPonudaToFisk = "SendPonudaToFisk=0";
                }
            }
        }

        /// <summary>
        /// _IgnoreSSLCertificates
        /// </summary>
        public string IgnoreSSLCertificates
        {
            get
            {
                return _IgnoreSSLCertificates;
            }
            set
            {
                if (value == "True")
                {
                    _IgnoreSSLCertificates = "IgnoreSSLCertificates=1";
                }
                else
                {
                    _IgnoreSSLCertificates = "IgnoreSSLCertificates=0";
                }
            }
        }
        public ConfigFile()
        {
            Lvide = "";
            SecGeneral = "[General]";
            SecSalon = "[Salon]";
            SecSoloconfig = "[SoloConfig]";
            SecCertificate = "[Certificate]";
            SecCertificateFile = "[CertificateFile]";
            SecTestCertificate = "[TestCertificate]";
            TestServerUrl = "https://cistest.apis-it.hr:8449/FiskalizacijaServiceTest";
        }
    }

}
