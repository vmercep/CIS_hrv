using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

public static class Razno {
  public static string FormatirajDatumVrijeme (DateTime datumVrijeme) {
    return string.Format("{0:dd.MM.yyyy}T{1}", datumVrijeme, datumVrijeme.ToString("HH:mm:ss"));
  }

  public static string FormatirajDatum (DateTime datum) {
    return $"{datum:dd.MM.yyyy}";
  }

  public static string DohvatiFormatiranoTrenutnoDatumVrijeme () {
    return FormatirajDatumVrijeme(DateTime.Now);
  }

  public static void FormatirajEchoPoruku (ref string poruka) {
    if (string.IsNullOrEmpty(poruka)) {
      poruka = "echo test";
    }
  }

  public static string ZastitniKodIzracun (X509Certificate2 certifikat, string oibObveznika, string datumVrijemeIzdavanjaRacuna, string brojcanaOznakaRacuna, string oznakaPoslovnogProstora, string oznakaNaplatnogUredaja, string ukupniIznosRacuna) {
    if (certifikat == null || string.IsNullOrEmpty(oibObveznika) || datumVrijemeIzdavanjaRacuna == null || string.IsNullOrEmpty(brojcanaOznakaRacuna) || string.IsNullOrEmpty(oznakaPoslovnogProstora) || string.IsNullOrEmpty(oznakaNaplatnogUredaja)) {
      throw new ArgumentNullException();
    }
    return ZKI(certifikat, oibObveznika, datumVrijemeIzdavanjaRacuna, brojcanaOznakaRacuna, oznakaPoslovnogProstora, oznakaNaplatnogUredaja, ukupniIznosRacuna);
  }

  public static string ZastitniKodIzracun (string certificateSubject, string oibObveznika, string datumVrijemeIzdavanjaRacuna, string brojcanaOznakaRacuna, string oznakaPoslovnogProstora, string oznakaNaplatnogUredaja, string ukupniIznosRacuna) {
    if (string.IsNullOrEmpty(certificateSubject) || string.IsNullOrEmpty(oibObveznika) || datumVrijemeIzdavanjaRacuna == null || string.IsNullOrEmpty(brojcanaOznakaRacuna) || string.IsNullOrEmpty(oznakaPoslovnogProstora) || string.IsNullOrEmpty(oznakaNaplatnogUredaja)) {
      throw new ArgumentNullException();
    }
    X509Certificate2 certifikat = Potpisivanje.DohvatiCertifikat(certificateSubject);
    return ZKI(certifikat, oibObveznika, datumVrijemeIzdavanjaRacuna, brojcanaOznakaRacuna, oznakaPoslovnogProstora, oznakaNaplatnogUredaja, ukupniIznosRacuna);
  }

  public static string ZastitniKodIzracun (string certifikatDatoteka, string zaporka, string oibObveznika, string datumVrijemeIzdavanjaRacuna, string brojcanaOznakaRacuna, string oznakaPoslovnogProstora, string oznakaNaplatnogUredaja, string ukupniIznosRacuna) {
    if (string.IsNullOrEmpty(certifikatDatoteka) || string.IsNullOrEmpty(zaporka) || string.IsNullOrEmpty(oibObveznika) || datumVrijemeIzdavanjaRacuna == null || string.IsNullOrEmpty(brojcanaOznakaRacuna) || string.IsNullOrEmpty(oznakaPoslovnogProstora) || string.IsNullOrEmpty(oznakaNaplatnogUredaja)) {
      throw new ArgumentNullException();
    }
    X509Certificate2 certifikat = Potpisivanje.DohvatiCertifikat(certifikatDatoteka, zaporka);
    return ZKI(certifikat, oibObveznika, datumVrijemeIzdavanjaRacuna, brojcanaOznakaRacuna, oznakaPoslovnogProstora, oznakaNaplatnogUredaja, ukupniIznosRacuna);
  }

  public static DirectoryInfo GenerirajProvjeriMapu (string mapa) {
    DirectoryInfo directoryInfo = null;
    try {
      directoryInfo = new DirectoryInfo(mapa);
      if (!directoryInfo.Exists) {
        directoryInfo.Create();
      }
    } catch (Exception ex) {
      SimpleLog.Log(ex);
      Trace.TraceError($"Greška kod kreiranja mape:{ex.Message}");
      throw;
    }
    return directoryInfo;
  }

  private static string ComputeHash (byte[] objectAsBytes) {
    MD5 mD = MD5.Create();
    try {
      byte[] array = mD.ComputeHash(objectAsBytes);
      StringBuilder stringBuilder = new StringBuilder();
      for (int i = 0; i < array.Length; i++) {
        stringBuilder.Append(array[i].ToString("x2"));
      }
      return stringBuilder.ToString();
    } catch (ArgumentNullException) {
      Console.WriteLine("Hash has not been generated.");
      return null;
    }
  }

  private static string ZKI (X509Certificate2 certifikat, string oibObveznika, string datumVrijemeIzdavanjaRacuna, string brojcanaOznakaRacuna, string oznakaPoslovnogProstora, string oznakaNaplatnogUredaja, string ukupniIznosRacuna) {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append(oibObveznika);
    stringBuilder.Append(datumVrijemeIzdavanjaRacuna);
    stringBuilder.Append(brojcanaOznakaRacuna);
    stringBuilder.Append(oznakaPoslovnogProstora);
    stringBuilder.Append(oznakaNaplatnogUredaja);
    stringBuilder.Append(ukupniIznosRacuna.Replace(',', '.'));
    byte[] array = Potpisivanje.PotpisiTekst(stringBuilder.ToString(), certifikat);
    if (array != null) {
      return ComputeHash(array);
    }
    return "";
  }
}
