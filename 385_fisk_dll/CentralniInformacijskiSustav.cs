using Helper;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Net.Security;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

[SecuritySafeCritical]
public class CentralniInformacijskiSustav {
  private const string cisUrl = "https://cis.porezna-uprava.hr:8449/FiskalizacijaService";

    private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public string NazivMapeZahtjev {
    get;
    set;
  }

  public string NazivMapeOdgovor {
    get;
    set;
  }

  public bool NazivAutoGeneriranje {
    get;
    set;
  }

  public XmlDocument OdgovorGreska {
    get;
    set;
  }

  public WebExceptionStatus? OdgovorGreskaStatus {
    get;
    set;
  }

  public int TimeOut {
    get;
    set;
  }

  public string CisUrl {
    get;
    set;
  }

  public event EventHandler<CentralniInformacijskiSustavEventArgs> SoapMessageSending;

  public event EventHandler<EventArgs> SoapMessageSent;

    public XmlDocument PosaljiPromjenuNacinaplacanja(RacunPNPType racun, string certificateSubject)
    {
        XmlDocument racunOdgovor = null;
        PromijeniNacPlacZahtjev promjenaNacinaPlacanjaZahtjev = XmlDokumenti.KreirajPromjeniNacinPlacanjazahtjev(racun);
        XmlDocument zahtjevXml = XmlDokumenti.SerijalizirajPromjenuNacinaPlacanjaZahtjev(promjenaNacinaPlacanjaZahtjev);
        if (AppLink.UseCertificateFile == "1")
        {
            PosaljiZahtjev(certificateSubject, ref racunOdgovor, zahtjevXml, useTestServer: false, useImportedCertificate: true);
        }
        else
        {
            PosaljiZahtjev(certificateSubject, ref racunOdgovor, zahtjevXml);
        }
        return racunOdgovor;
    }

   
    public XmlDocument PosaljiRacun(RacunType racun, string certificateSubject)
    {
        XmlDocument racunOdgovor = null;
        RacunZahtjev racunZahtjev = XmlDokumenti.KreirajRacunZahtjev(racun);
        XmlDocument zahtjevXml = XmlDokumenti.SerijalizirajRacunZahtjev(racunZahtjev);
        if (AppLink.UseCertificateFile == "1")
        {
            PosaljiZahtjev(certificateSubject, ref racunOdgovor, zahtjevXml, useTestServer: false, useImportedCertificate: true);
        }
        else
        {
            PosaljiZahtjev(certificateSubject, ref racunOdgovor, zahtjevXml);
        }
        return racunOdgovor;
    }
    // remove in next releases since is not in use
    /*
    public XmlDocument PosaljiRacun(RacunType racun, X509Certificate2 certifikat)
    {
        //XmlDocument xmlDocument = null;
        RacunZahtjev racunZahtjev = XmlDokumenti.KreirajRacunZahtjev(racun);
        XmlDocument dokument = XmlDokumenti.SerijalizirajRacunZahtjev(racunZahtjev);
        Potpisivanje.PotpisiXmlDokument(dokument, certifikat);
        XmlDokumenti.DodajSoapEnvelope(ref dokument);
        return SendSoapMessage(dokument);
    }

    public XmlDocument PosaljiRacun(RacunType racun, string certificateSubject, StoreLocation storeLocation, StoreName storeName)
    {
        XmlDocument racunOdgovor = null;
        RacunZahtjev racunZahtjev = XmlDokumenti.KreirajRacunZahtjev(racun);
        XmlDocument zahtjevXml = XmlDokumenti.SerijalizirajRacunZahtjev(racunZahtjev);
        PosaljiZahtjev(certificateSubject, storeLocation, storeName, ref racunOdgovor, zahtjevXml);
        return racunOdgovor;
    }

    public XmlDocument PosaljiRacun(RacunType racun)
    {
        XmlDocument racunOdgovor = null;
        RacunZahtjev racunZahtjev = XmlDokumenti.KreirajRacunZahtjev(racun);
        XmlDocument zahtjevXml = XmlDokumenti.SerijalizirajRacunZahtjev(racunZahtjev);
        PosaljiZahtjev("", ref racunOdgovor, zahtjevXml, useTestServer: false, useImportedCertificate: true);
        return racunOdgovor;
    }
    */

    public XmlDocument PosaljiProvjeru(RacunType racun)
    {
        XmlDocument racunOdgovor = null;
        ProvjeraZahtjev provjeraZahtjev = XmlDokumenti.KreirajProvjeraZahtjev(racun);
        XmlDocument zahtjevXml = XmlDokumenti.SerijalizirajProvjeraZahtjev(provjeraZahtjev);
        PosaljiZahtjev("", ref racunOdgovor, zahtjevXml, useTestServer: true);
        return racunOdgovor;
    }

    public XmlDocument PosaljiProvjeru(RacunType racun, X509Certificate2 certifikat)
    {
        return null;
    }

    public XmlDocument PosaljiProvjeru(RacunType racun, string certificateSubject, StoreLocation storeLocation, StoreName storeName)
    {
        return null;
    }

    public XmlDocument PosaljiEcho(string poruka)
    {
        XmlDocument result = null;
        XmlDocument xmlDocument = XmlDokumenti.DohvatiPorukuEchoZahtjev(poruka);
        if (xmlDocument != null)
        {
            result = new XmlDocument();
            result = SendSoapMessage(xmlDocument);
        }
        return result;
    }

    public bool Echo()
    {
        return Echo("");
    }

    public bool Echo(string poruka)
    {
        bool result = false;
        XmlDocument xmlDocument = PosaljiEcho(poruka);
        if (xmlDocument != null && xmlDocument.DocumentElement != null)
        {
            string b = xmlDocument.DocumentElement.InnerText.Trim();
            Razno.FormatirajEchoPoruku(ref poruka);
            if (poruka == b)
            {
                result = true;
            }
        }
        return result;
    }

    public XmlDocument PosaljiSoapPoruku(XmlDocument soapPoruka)
    {
        return SendSoapMessage(soapPoruka);
    }

    public XmlDocument PosaljiSoapPoruku(string soapPoruka)
    {
        return SendSoapMessage(XmlDokumenti.UcitajXml(soapPoruka));
    }

    public static void BypassCertificateError()
    {
        log.Debug("BypassCertificateError in ON");

        ServicePointManager.ServerCertificateValidationCallback +=

            delegate (
                Object sender1,
                X509Certificate certificate,
                X509Chain chain,
                SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };
    }

    private XmlDocument SendSoapMessage(XmlDocument soapMessage, bool useTestServer = false)
    {
        XmlDocument xmlDocument = null;
        OdgovorGreska = null;

        log.Debug("Sending soap message " + JsonConvert.SerializeObject(soapMessage));

        if (AppLink.IgnoreSSLCertificates == "1") BypassCertificateError();

        if (this.SoapMessageSending != null)
        {
            CentralniInformacijskiSustavEventArgs centralniInformacijskiSustavEventArgs = new CentralniInformacijskiSustavEventArgs
            {
                SoapMessage = soapMessage
            };
            this.SoapMessageSending(this, centralniInformacijskiSustavEventArgs);
            if (centralniInformacijskiSustavEventArgs.Cancel)
            {
                return xmlDocument;
            }
        }
        SnimanjeDatoteka(NazivMapeZahtjev, soapMessage);
        try
        {
            Uri requestUri = (!useTestServer) ? new Uri(AppLink.URL) : new Uri(AppLink.TestServerUrl);
            HttpWebRequest httpWebRequest = WebRequest.Create(requestUri) as HttpWebRequest;
            if (httpWebRequest != null)
            {
                ServicePointManager.Expect100Continue = true;
                string connectionEncryption = AppLink.ConnectionEncryption;
                if (!string.IsNullOrEmpty(connectionEncryption))
                {
                    if (connectionEncryption == "TLS 1.1")
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;
                    }
                    else if (connectionEncryption == "TLS 1.2")
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    }
                    else
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    }
                }
                else
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                }
                if (AppLink.UseCertificateFile == "1")
                {
                    httpWebRequest.ClientCertificates.Add(Potpisivanje.DohvatiCertifikat(AppLink.DatotekaCertifikata(), AppLink.CertificatePassword));
                }
                else
                {
                    httpWebRequest.ClientCertificates.Add(Potpisivanje.DohvatiCertifikat(AppLink.Certificate));
                }
                if (TimeOut > 0)
                {
                    httpWebRequest.Timeout = TimeOut;
                }
                httpWebRequest.ContentType = "text/xml";
                httpWebRequest.Method = "POST";
                httpWebRequest.Proxy = null;
                byte[] bytes = Encoding.UTF8.GetBytes(soapMessage.InnerXml);
                httpWebRequest.ProtocolVersion = HttpVersion.Version11;
                httpWebRequest.ContentLength = bytes.Length;
                using (Stream stream = httpWebRequest.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
                HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse;
                if (httpWebResponse != null)
                {
                    Stream responseStream = httpWebResponse.GetResponseStream();
                    Encoding encoding = Encoding.GetEncoding("utf-8");
                    StreamReader streamReader = new StreamReader(responseStream, encoding);
                    string xml = streamReader.ReadToEnd();
                    xmlDocument = new XmlDocument();
                    xmlDocument.PreserveWhitespace = true;
                    xmlDocument.LoadXml(xml);
                    //LogFile.LogToFile("Response " + JsonConvert.SerializeObject(xmlDocument), LogLevel.Debug);
                    log.Debug("Response " + JsonConvert.SerializeObject(xmlDocument));
                    if (this.SoapMessageSent != null)
                    {
                        EventArgs e = new EventArgs();
                        this.SoapMessageSent(this, e);
                    }
                }
            }
        }
        catch (WebException ex)
        {
            //LogFile.LogToFile("WebException Error in sending XML " + ex.Message, LogLevel.Debug);
            //LogFile.LogToFile("WebException Error in sending XML " + ex.InnerException, LogLevel.Debug);
            log.Error("WebException Error in sending XML",ex);
            SimpleLog.Log(ex);
            OdgovorGreskaStatus = ex.Status;
            WebResponse response = ex.Response;

            //LogFile.LogToFile("WebException Error in sending XML "+ JsonConvert.SerializeObject(response), LogLevel.Debug);
            if (response != null)
            {
                using (Stream stream2 = response.GetResponseStream())
                {
                    StreamReader txtReader = new StreamReader(stream2);
                    OdgovorGreska = new XmlDocument();
                    OdgovorGreska.Load(txtReader);
                    SimpleLog.Log(OdgovorGreska.OuterXml);
                    //LogFile.LogToFile("WebException Error in sending XML " + OdgovorGreska.OuterXml, LogLevel.Debug);
                    log.Debug("WebException Error in sending XML " + OdgovorGreska.OuterXml);
                    return OdgovorGreska;
                }
            }
            Trace.TraceError($"Greška kod slanja SOAP poruke. Status greške (prema http://msdn.microsoft.com/en-us/library/system.net.webexceptionstatus.aspx): {ex.Status}. Poruka greške: {ex.Message}");
            throw;
        }
        catch (Exception ex2)
        {
            //LogFile.LogToFile("Exception ex2 Error in sending XML " + ex2.Message, LogLevel.Debug);
            log.Error("Exception ex2 Error in sending XML", ex2);
            SimpleLog.Log(ex2);
            Trace.TraceError($"Greška kod slanja SOAP poruke: {ex2.Message}");
            throw;
        }
        SaveFile(soapMessage, xmlDocument);
        return xmlDocument;
    }

    private void SnimanjeDatoteka (string mapa, XmlDocument dokument) {
    if (!string.IsNullOrEmpty(mapa) && dokument != null) {
      TipDokumentaEnum tipDokumentaEnum = XmlDokumenti.OdrediTipDokumenta(dokument);
      if (tipDokumentaEnum != 0) {
        DirectoryInfo directoryInfo = Razno.GenerirajProvjeriMapu(mapa);
        if (directoryInfo != null) {
          string text = "";
          if (!NazivAutoGeneriranje) {
            text = Path.Combine(mapa, $"{tipDokumentaEnum}.xml");
          } else {
            string text2 = XmlDokumenti.DohvatiUuid(dokument, tipDokumentaEnum);
            if (!string.IsNullOrEmpty(text2)) {
              text = Path.Combine(mapa, $"{tipDokumentaEnum}_{text2}.xml");
            }
          }
          if (!string.IsNullOrEmpty(text)) {
            XmlDokumenti.SnimiXmlDokumentDatoteka(dokument, text);
          }
        }
      }
    }
  }

    private void PosaljiZahtjev(string certificateSubject, ref XmlDocument racunOdgovor, XmlDocument zahtjevXml, bool useTestServer = false, bool useImportedCertificate = false)
    {
        SaveFile(zahtjevXml, zahtjevXml);
        if (zahtjevXml != null && !string.IsNullOrEmpty(zahtjevXml.InnerXml))
        {
            X509Certificate2 x509Certificate = null;
            if (useTestServer)
            {
                x509Certificate = Potpisivanje.DohvatiCertifikat(AppLink.DatotekaDemoCertifikata(), "Demo02");
            }
            else if (useImportedCertificate)
            {
                try
                {
                    x509Certificate = Potpisivanje.DohvatiCertifikat(AppLink.DatotekaCertifikata(), AppLink.CertificatePassword);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                x509Certificate = Potpisivanje.DohvatiCertifikat(certificateSubject);
            }
            if (x509Certificate != null)
            {
                Potpisivanje.PotpisiXmlDokument(zahtjevXml, x509Certificate);
                XmlDokumenti.DodajSoapEnvelope(ref zahtjevXml);
                racunOdgovor = SendSoapMessage(zahtjevXml, useTestServer);
            }
        }
    }

    private void PosaljiZahtjev (string certificateSubject, StoreLocation storeLocation, StoreName storeName, ref XmlDocument racunOdgovor, XmlDocument zahtjevXml) {
    if (zahtjevXml != null && !string.IsNullOrEmpty(zahtjevXml.InnerXml)) {
      X509Certificate2 x509Certificate = Potpisivanje.DohvatiCertifikat(certificateSubject, storeLocation, storeName);
      if (x509Certificate != null) {
        Potpisivanje.PotpisiXmlDokument(zahtjevXml, x509Certificate);
        XmlDokumenti.DodajSoapEnvelope(ref zahtjevXml);
        racunOdgovor = SendSoapMessage(zahtjevXml);
      }
    }
  }

    public static void SaveFile(XmlDocument request, XmlDocument response)
    {
        //LogFile.LogToFile("SaveFile for XML messages ", LogLevel.Debug);
        log.Debug("SaveFile for XML messages ");
        TipDokumentaEnum tipDokumentaEnum = XmlDokumenti.OdrediTipDokumenta(request);
        if (tipDokumentaEnum != 0)
        {
            string text = "";
            CreateDirectories();
            string str = XmlDokumenti.DohvatiUuid(request, tipDokumentaEnum);
            switch (tipDokumentaEnum)
            {
                case TipDokumentaEnum.RacunZahtjev:
                    //LogFile.LogToFile("SaveFile for XML messages in folder ", LogLevel.Debug);
                    log.Debug("SaveFile for XML messages in folder ");
                    text = ((!string.IsNullOrEmpty(AppLink.XMLSavePath)) ? Path.Combine(AppLink.XMLSavePath + "\\FiskXMLMessages\\Invoice\\Requests\\" + (DateTime.Today.Year.ToString()) + "\\", str + ".xml") : Path.Combine(Environment.CurrentDirectory + "\\FiskXMLMessages\\Invoice\\Requests\\" + (DateTime.Today.Year.ToString()) + "\\", str + ".xml"));
                    request.Save(text);
                    text = ((!string.IsNullOrEmpty(AppLink.XMLSavePath)) ? Path.Combine(AppLink.XMLSavePath + "\\FiskXMLMessages\\Invoice\\Response\\" + (DateTime.Today.Year.ToString()) + "\\", str + ".xml") : Path.Combine(Environment.CurrentDirectory + "\\FiskXMLMessages\\Invoice\\Response\\" + (DateTime.Today.Year.ToString()) + "\\", str + ".xml"));
                    response?.Save(text);
                    break;
            }
        }
    }

    public static void CreateDirectories()
    {
        try
        {
            log.Debug("Create directories for XML messages start ");
            DirectoryInfo directoryInfo = null;
            if (string.IsNullOrEmpty(AppLink.XMLSavePath))
            {
                directoryInfo = new DirectoryInfo(Environment.CurrentDirectory + "\\FiskXMLMessages\\Invoice\\Requests\\" + (DateTime.Today.Year.ToString()) + "\\");
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
            else
            {
                directoryInfo = new DirectoryInfo(AppLink.XMLSavePath + "\\FiskXMLMessages\\Invoice\\Requests\\" + (DateTime.Today.Year.ToString()) + "\\");
                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }
                directoryInfo = new DirectoryInfo(AppLink.XMLSavePath + "\\FiskXMLMessages\\Invoice\\Response\\" + (DateTime.Today.Year.ToString()) + "\\");
                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }
            }
            
            log.Debug("Create directories for XML messages end ");
        }
        catch (Exception e)
        {
            log.Error("Error ocured in directory creation ",e);

        }

    }
}
