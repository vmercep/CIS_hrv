using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{



    public static class LogFile
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public static void createFileForSysLog()
        {
            FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
            string directoryName = fileInfo.DirectoryName;
            string path = directoryName + "//PaymentChangeInfo";
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
                File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden);
                
                using (var tw = new StreamWriter(path, true))
                {
                    tw.WriteLine(DateTime.Now);
                }
            }
        }

        public static DateTime ReturnDateFromFile()
        {
            DateTime today = new DateTime();
            FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
            string directoryName = fileInfo.DirectoryName;
            string path = directoryName + "//PaymentChangeInfo";
            const Int32 BufferSize = 128;
            using (var fileStream = File.OpenRead(path))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                String line;
                while ((line = streamReader.ReadLine()) != null) {
                    today = Convert.ToDateTime(line);
                }
 
            }

            return today;
        }


        /*
        public static void createSimpleLog()
        {
            FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
            string directoryName = fileInfo.DirectoryName;
            Console.WriteLine(directoryName);
            directoryName += "\\CISLogFiles";
            SimpleLog.SetLogDir(directoryName, createIfNotExisting: true);
            SimpleLog.Prefix = "CISLogFile_";
        }
        */

        public static void InitConfig()
        {
            try
            {

                log.Debug("Creating initial config file for MERLIN");
                ConfigFile configFile = new ConfigFile();
                configFile.ConnectionString = "server=(local); uid=sa; pwd=_PWD4sa_; Database=Merlin";
                configFile.ServerUrl = "https://cis.porezna-uprava.hr:8449/FiskalizacijaService";
                configFile.ConnectionEncryption = "TLS 1.2";
                configFile.LogFileActive = "True";
                configFile.SaveXMLActive = "True";
                configFile.XMLSavePath = "C:\\Ikosoft";
                configFile.DateIsActive = $"{DateTime.Today:yyyy/MM/dd}";
                configFile.ActiveLanguage = "HR";
                configFile.VATNumber = "";
                configFile.InVATsystem = "False";
                configFile.PremiseMark = "";
                configFile.BillingDeviceMark = "";
                configFile.OIBSoftware = "15259334060";
                configFile.OperatorOIB = "";
                configFile.OperatorCode = "";
                //configFile.PCeasedOperation = Convert.ToString(chkPCeasedOperation.Checked);
                configFile.Certifikat = "";
                configFile.SendTestReceipts = "True";
                configFile.UseCertificateFile = "False";
                configFile.CertificatePassword = "Demo03";
                configFile.IgnoreSSLCertificates = "True";
                configFile.SendTestReceipts = "True";
                configFile.SendPonudaToFisk = "False";

                configFile.QrCodeMessage = @"https://porezna.gov.hr/rn?zki={0}&datv={1}&izn={2}";

                configFile.QrCodeLocation = @"C:\\Ikosoft\\MerlinX2\\Dat\\Merlin_Files\\Images\\QRCode";

                configFile.QrCodeSize = "180";

                configFile.LogLevel = "INFO";


                FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
                string directoryName = fileInfo.DirectoryName;

                string filePath = Path.Combine(directoryName, Helper.Globals.Name);

                if (!File.Exists(filePath))
                {
                    //LogToFile("Creating cfg file for merlin");
                    StreamWriter streamWriter = new StreamWriter(Path.Combine(directoryName, Helper.Globals.Name), true, Encoding.UTF8);
                    streamWriter.WriteLine(configFile.SecGeneral);
                    streamWriter.WriteLine(configFile.ConnectionString);
                    streamWriter.WriteLine(configFile.ConnectionEncryption);
                    streamWriter.WriteLine(configFile.ServerUrl);
                    streamWriter.WriteLine(configFile.LogFileActive);
                    streamWriter.WriteLine(configFile.SaveXMLActive);
                    streamWriter.WriteLine(configFile.XMLSavePath);
                    streamWriter.WriteLine(configFile.DateIsActive);
                    streamWriter.WriteLine(configFile.ActiveLanguage);
                    streamWriter.WriteLine(configFile.Lvide);
                    streamWriter.WriteLine(configFile.SecSalon);
                    streamWriter.WriteLine(configFile.VATNumber);
                    streamWriter.WriteLine(configFile.InVATsystem);
                    streamWriter.WriteLine(configFile.PremiseMark);
                    streamWriter.WriteLine(configFile.BillingDeviceMark);
                    streamWriter.WriteLine(configFile.OIBSoftware);
                    streamWriter.WriteLine(configFile.PCeasedOperation);
                    streamWriter.WriteLine(configFile.Lvide);
                    streamWriter.WriteLine(configFile.SecSoloconfig);
                    streamWriter.WriteLine(configFile.OperatorOIB);
                    streamWriter.WriteLine(configFile.OperatorCode);
                    streamWriter.WriteLine(configFile.Lvide);
                    streamWriter.WriteLine(configFile.SecCertificate);
                    streamWriter.WriteLine(configFile.Certifikat);
                    streamWriter.WriteLine(configFile.Lvide);
                    streamWriter.WriteLine(configFile.SecCertificateFile);
                    streamWriter.WriteLine(configFile.UseCertificateFile);
                    streamWriter.WriteLine(configFile.CertificatePassword);
                    streamWriter.WriteLine(configFile.Lvide);
                    streamWriter.WriteLine(configFile.SecTestCertificate);
                    streamWriter.WriteLine(configFile.SendTestReceipts);
                    streamWriter.WriteLine(configFile.TestServerUrl);
                    streamWriter.WriteLine(configFile.IgnoreSSLCertificates);
                    streamWriter.WriteLine(configFile.SendPonudaToFisk);
                    streamWriter.WriteLine(configFile.QrCode);
                    streamWriter.WriteLine(configFile.QrCodeMessage);
                    streamWriter.WriteLine(configFile.QrCodeLocation);
                    streamWriter.WriteLine(configFile.QrCodeSize);
                    streamWriter.WriteLine(configFile.LogLevel);
                    streamWriter.Close();


                }
            }
            catch (Exception ex)
            {
                log.Error("Error in creating initial config file",ex);
            }




        }



        public static bool CreateConfigFile(ConfigFile fConfig, bool isConfigMod)
        {
            log.Debug("Creating config file");

            FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
            string directoryName = fileInfo.DirectoryName;

            if (isConfigMod && File.Exists(Path.Combine(directoryName, Helper.Globals.Name)))
            {
                File.Delete(Path.Combine(directoryName, Helper.Globals.Name));

                try
                {
                    string filePath = Path.Combine(directoryName, Helper.Globals.Name);

                    StreamWriter streamWriter = new StreamWriter(filePath, true, Encoding.UTF8);
                    streamWriter.WriteLine(fConfig.SecGeneral);
                    streamWriter.WriteLine(fConfig.ConnectionString);
                    streamWriter.WriteLine(fConfig.ConnectionEncryption);
                    streamWriter.WriteLine(fConfig.ServerUrl);
                    streamWriter.WriteLine(fConfig.LogFileActive);
                    streamWriter.WriteLine(fConfig.SaveXMLActive);
                    streamWriter.WriteLine(fConfig.XMLSavePath);
                    streamWriter.WriteLine(fConfig.DateIsActive);
                    streamWriter.WriteLine(fConfig.DateTipIsActive);
                    streamWriter.WriteLine(fConfig.ActiveLanguage);
                    streamWriter.WriteLine(fConfig.Lvide);
                    streamWriter.WriteLine(fConfig.SecSalon);
                    streamWriter.WriteLine(fConfig.VATNumber);
                    streamWriter.WriteLine(fConfig.InVATsystem);
                    streamWriter.WriteLine(fConfig.PremiseMark);
                    streamWriter.WriteLine(fConfig.BillingDeviceMark);
                    streamWriter.WriteLine(fConfig.OIBSoftware);
                    streamWriter.WriteLine(fConfig.PCeasedOperation);
                    streamWriter.WriteLine(fConfig.Lvide);
                    streamWriter.WriteLine(fConfig.SecSoloconfig);
                    streamWriter.WriteLine(fConfig.OperatorOIB);
                    streamWriter.WriteLine(fConfig.OperatorCode);
                    streamWriter.WriteLine(fConfig.Lvide);
                    streamWriter.WriteLine(fConfig.SecCertificate);
                    streamWriter.WriteLine(fConfig.Certifikat);
                    streamWriter.WriteLine(fConfig.Lvide);
                    streamWriter.WriteLine(fConfig.SecCertificateFile);
                    streamWriter.WriteLine(fConfig.UseCertificateFile);
                    streamWriter.WriteLine(fConfig.CertificatePassword);
                    streamWriter.WriteLine(fConfig.Lvide);
                    streamWriter.WriteLine(fConfig.SecTestCertificate);
                    streamWriter.WriteLine(fConfig.SendTestReceipts);
                    streamWriter.WriteLine(fConfig.TestServerUrl);
                    streamWriter.WriteLine(fConfig.IgnoreSSLCertificates);
                    streamWriter.WriteLine(fConfig.SendPonudaToFisk);
                    streamWriter.WriteLine(fConfig.QrCodeMessage);
                    streamWriter.WriteLine(fConfig.QrCodeLocation);
                    streamWriter.WriteLine(fConfig.QrCodeSize);
                    streamWriter.WriteLine(fConfig.LogLevel);
                    streamWriter.WriteLine(fConfig.SendTip);
                    streamWriter.Close();


                    log.Debug("Config file created");
                    return true;
                }
                catch (Exception ex)
                {
                   
                    log.Error("Error in creating config file",ex);
                    return false;
                }
            }
            log.Debug("Config file created");
            return true;


        }
    }
}
