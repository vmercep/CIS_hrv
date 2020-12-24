using Helper;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

public class Potpisivanje {
  public static X509Certificate2 DohvatiCertifikat (string certificateSubject) {
    return DohvatiCertifikat(certificateSubject, StoreLocation.CurrentUser, StoreName.My);
  }

  public static X509Certificate2 DohvatiCertifikat (string certificateSubject, StoreLocation storeLocation, StoreName storeName) {
    X509Certificate2 result = null;
    X509Store x509Store = new X509Store(storeName, storeLocation);
    x509Store.Open(OpenFlags.OpenExistingOnly);
    X509Certificate2Enumerator enumerator = x509Store.Certificates.GetEnumerator();
    while (enumerator.MoveNext()) {
      X509Certificate2 current = enumerator.Current;
      if (current.FriendlyName.StartsWith(certificateSubject)) {
                LogFile.LogToFile("Cert loaded details issuer "+ current.Issuer + ", subject "+current.Subject, LogLevel.Debug);
                result = current;
        break;
      }
    }
    return result;
  }

    public static X509Certificate2 DohvatiCertifikat(string certifikatDatoteka, string zaporka)
    {
        X509Certificate2 result = null;
        FileInfo fileInfo = new FileInfo(certifikatDatoteka);
        if (fileInfo.Exists)
        {
            try
            {
                result = new X509Certificate2(certifikatDatoteka, zaporka);
            }
            catch (Exception ex)
            {
                SimpleLog.Log(ex);
                Trace.TraceError($"Greška kod kreiranja certifikata: {ex.Message}");
                throw;
            }
        }
        return result;
    }

    public static XmlDocument PotpisiXmlDokument(XmlDocument dokument, X509Certificate2 certifikat)
    {
        RSACryptoServiceProvider signingKey = (RSACryptoServiceProvider)certifikat.PrivateKey;
        SignedXml signedXml = null;
        try
        {
            signedXml = new SignedXml(dokument);
            signedXml.SigningKey = signingKey;
            signedXml.SignedInfo.CanonicalizationMethod = "http://www.w3.org/2001/10/xml-exc-c14n#";
            KeyInfo keyInfo = new KeyInfo();
            KeyInfoX509Data keyInfoX509Data = new KeyInfoX509Data();
            keyInfoX509Data.AddCertificate(certifikat);
            keyInfoX509Data.AddIssuerSerial(certifikat.Issuer, certifikat.GetSerialNumberString());
            keyInfo.AddClause(keyInfoX509Data);
            signedXml.KeyInfo = keyInfo;
            Reference reference = new Reference("");
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform(includeComments: false));
            reference.AddTransform(new XmlDsigExcC14NTransform(includeComments: false));
            reference.Uri = "#signXmlId";
            signedXml.AddReference(reference);
            signedXml.ComputeSignature();
            XmlElement xml = signedXml.GetXml();
            dokument.DocumentElement.AppendChild(xml);
        }
        catch (Exception ex)
        {
            SimpleLog.Log(ex);
            Trace.TraceError($"Greška kod potpisivanja XML dokumenta: {ex.Message}");
            throw;
        }
        return dokument;
    }

    public static byte[] PotpisiTekst (string tekst, X509Certificate2 certifikat) {
    if (certifikat != null) {
      //byte[] array = null;
      RSACryptoServiceProvider rSACryptoServiceProvider = (RSACryptoServiceProvider) certifikat.PrivateKey;
      try {
        byte[] bytes = Encoding.ASCII.GetBytes(tekst);
        return rSACryptoServiceProvider.SignData(bytes, new SHA1CryptoServiceProvider());
      } catch (Exception ex) {
        SimpleLog.Log(ex);
        Trace.TraceError($"Greška kod potpisivanja teksta: {ex.Message}");
        throw;
      }
    }
    throw new ArgumentNullException();
  }

    public static bool ProvjeriPotpis(XmlDocument dokument)
    {
        if (dokument == null)
        {
            throw new ArgumentNullException();
        }
        SignedXml signedXml = new SignedXml(dokument);
        XmlNodeList elementsByTagName = dokument.GetElementsByTagName("Signature");
        if (elementsByTagName.Count <= 0)
        {
            Trace.TraceError("Verifikacija nije uspjela: U primljenom dokumentu nije pronadjen digitalni potpis.");
            throw new CryptographicException("Verifikacija nije uspjela: U primljenom dokumentu nije pronadjen digitalni potpis.");
        }
        signedXml.LoadXml((XmlElement)elementsByTagName[0]);
        X509Certificate2 x509Certificate = null;
        foreach (KeyInfoClause item in signedXml.KeyInfo)
        {
            if (item is KeyInfoX509Data && ((KeyInfoX509Data)item).Certificates.Count > 0)
            {
                x509Certificate = (X509Certificate2)((KeyInfoX509Data)item).Certificates[0];
            }
        }
        if (x509Certificate == null)
        {
            Trace.TraceError("U primljenom XMLu nema certifikata.");
            throw new Exception("U primljenom XMLu nema certifikata.");
        }
        return signedXml.CheckSignature(x509Certificate, verifySignatureOnly: true);
    }
}
