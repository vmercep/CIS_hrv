using System;
using System.IO;
using System.Reflection;


public static class AppLink {
  public static string ConnectionString => GetParams("ConnectionString", "");

  public static string ConnectionEncryption => GetParams("ConnectionEncryption", "TLS 1.2");

  public static string URL => GetParams("ServerUrl", "https://cis.porezna-uprava.hr:8449/FiskalizacijaService");

  public static string SaveXMLActive => GetParams("SaveXMLActive", "1");

  public static string XMLSavePath => GetParams("XMLSavePath", "");

  public static string VATNumber => GetParams("VATNumber", "");

  public static string UseCertificateFile => GetParams("UseCertificateFile", "0");

  public static string CertificatePassword => GetParams("CertificatePassword", "");

  public static string Certificate => GetParams("Certificate", "");

  public static string PremiseMark => GetParams("PremiseMark", "");

  public static string LogFileActive => GetParams("LogFileActive", "1");

  public static string DateIsActive => GetParams("DateIsActive", "2013/04/01");

  public static string BillingDeviceMark => GetParams("BillingDeviceMark", "");

  public static string OIBSoftware => GetParams("OIBSoftware", "");

  public static string OperatorOIB => GetParams("OperatorOIB", "");

  public static string OperatorCode => GetParams("OperatorCode", "");

  public static string TestMode => GetParams("TestMode", "");

  public static string InVATsystem => GetParams("InVATsystem", "1");

  public static string ActiveLanguage => GetParams("ActiveLanguage", "hr");

  public static string SendTestReceipts => GetParams("SendTestReceipts", "0");

  public static string TestServerUrl => GetParams("TestServerUrl", "https://cistest.apis-it.hr:8449/FiskalizacijaServiceTest");

  public static string PCeasedOperation => GetParams("PCeasedOperation", "0");

  public static string IgnoreSSLCertificates => GetParams("IgnoreSSLCertificates", "0");

  public static string SendPonudaToFisk => GetParams("SendPonudaToFisk", "0");

  public static string QrCodeMessage => GetParams("QrCodeMessage", "https://porezna.gov.hr/rn?zki={0}&datv={1}&izn={2}");

    public static string QrCodeLocation => GetParams("QrCodeLocation", "C:\\Ikosoft\\MerlinX2\\Dat\\Merlin_Files\\Images\\QRCode");

    public static string QrCodeSize => GetParams("QrCodeSize", "180");

    public static string LogLevel => GetParams("LogLevel", "INFO");

    private static string GetParams (string name, string defaultValue) {
    FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
    string directoryName = fileInfo.DirectoryName;
    string[] array = File.ReadAllLines(Path.Combine(directoryName, Helper.Globals.Name));
    string[] array2 = array;
    foreach (string text in array2) {
      if (text.StartsWith(name + "=")) {
        return text.Substring(name.Length + 1);
      }
    }
    return defaultValue;
  }

  public static DateTime DateFromLong (long longDate) {
    DateTime dateTime = new DateTime(1990, 1, 1).AddDays((double) ((longDate - longDate % 100000) / 100000));
    if (longDate % 100000 < 86400) {
      return dateTime.AddSeconds((double) (longDate % 100000));
    }
    return dateTime.AddSeconds(86399.0);
  }

  public static long LongFromDate (DateTime date) {
    return date.Subtract(new DateTime(1990, 1, 1)).Days * 100000;
  }




    public static bool TestFileCfg () {
    FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
    string directoryName = fileInfo.DirectoryName;
    return File.Exists(Path.Combine(directoryName, Helper.Globals.Name));
  }

  public static string GetTechCode () {
        //return (DateTime.Now.Year - 1900 + DateTime.Now.Month).ToString() + DateTime.Now.Day;
        return "Merlinka1994";
  }

  public static string DatotekaCertifikata () {
    FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
    string directoryName = fileInfo.DirectoryName;
    return directoryName + "/Certificates/certificate.p12";
  }

  public static string DatotekaDemoCertifikata () {
    FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
    string directoryName = fileInfo.DirectoryName;
    return directoryName + "/Certificates/demo.p12";
  }

    public static string GetPayementType(int typpai)
    {
        switch (typpai)
        {
            case 0:
                return "G";              
            case 1:
                return "C";
            case 2:
                return "K";
            case 3:
                return "C";
            case 12:
                return "T";
            default:
                return "O";
        }
    }
}
