using System;
using System.Diagnostics;
using System.Text;
using System.Xml;

public static class XmlDokumenti {
  public static XmlDocument SerijalizirajRacunZahtjev (RacunZahtjev racunZahtjev) {
    string text = "";
    try {
      text = racunZahtjev.Serialize();
      text = text.Replace("<tns:Pdv />", "");
      text = text.Replace("<tns:Pnp />", "");
      text = text.Replace("<tns:OstaliPor />", "");
      text = text.Replace("<tns:Naknade />", "");
    } catch (Exception ex) {
      SimpleLog.Log(ex);
      Trace.TraceError($"Greška kod serijalizacije zahtjeva za račun: {ex.Message}");
      throw;
    }
    return UcitajXml(text);
  }

  public static XmlDocument SerijalizirajRacun (RacunType racun) {
    string text = "";
    try {
      text = racun.Serialize();
    } catch (Exception ex) {
      SimpleLog.Log(ex);
      Trace.TraceError($"Greška kod serijalizacije računa: {ex.Message}");
      throw;
    }
    return UcitajXml(text);
  }

  public static RacunZahtjev KreirajRacunZahtjev (RacunType racun) {
    RacunZahtjev racunZahtjev = new RacunZahtjev {
      Id = "signXmlId",
      Racun = racun
    };
    ZaglavljeType zaglavljeType2 = racunZahtjev.Zaglavlje = new ZaglavljeType {
      DatumVrijeme = Razno.DohvatiFormatiranoTrenutnoDatumVrijeme(),
      IdPoruke = Guid.NewGuid().ToString()
    };
    return racunZahtjev;
  }

  public static RacunZahtjev KreirajRacunZahtjev (string ID, RacunType racun) {
    RacunZahtjev racunZahtjev = new RacunZahtjev {
      Id = ID,
      Racun = racun
    };
    ZaglavljeType zaglavljeType2 = racunZahtjev.Zaglavlje = new ZaglavljeType {
      DatumVrijeme = Razno.DohvatiFormatiranoTrenutnoDatumVrijeme(),
      IdPoruke = Guid.NewGuid().ToString()
    };
    return racunZahtjev;
  }

  public static string DohvatiJir (XmlDocument dokument) {
    string result = "";
    if (dokument != null) {
      DodajNamespace(dokument, out XmlNamespaceManager nsmgr);
      XmlElement documentElement = dokument.DocumentElement;
      XmlNode xmlNode = documentElement.SelectSingleNode("soap:Body/tns:RacunOdgovor/tns:Jir", nsmgr);
      if (xmlNode != null) {
        result = xmlNode.InnerText;
      }
    }
    return result;
  }

    /*
  public static Tuple<string, string> DohvatiStatusProvjere (XmlDocument dokument) {
    Tuple<string, string> result = null;
    if (dokument != null) {
      DodajNamespace(dokument, out XmlNamespaceManager nsmgr);
      XmlElement documentElement = dokument.DocumentElement;
      XmlNodeList xmlNodeList = documentElement.SelectNodes("soap:Body/tns:ProvjeraOdgovor/f73:Greske", nsmgr);
      foreach (XmlNode item3 in xmlNodeList) {
        string item = "";
        string item2 = "";
        XmlNode xmlNode2 = item3.SelectSingleNode("f73:Greska/f73:SifraGreske", nsmgr);
        if (xmlNode2 != null) {
          item = xmlNode2.InnerText.Trim();
        }
        XmlNode xmlNode3 = item3.SelectSingleNode("f73:Greska/f73:PorukaGreske", nsmgr);
        if (xmlNode3 != null) {
          item2 = xmlNode3.InnerText.Trim();
        }
        result = new Tuple<string, string>(item, item2);
      }
    }
    return result;
  }
    */

  public static Tuple<string, string> DohvatiStatusGreške (XmlDocument dokument, bool provjera) {
    Tuple<string, string> result = null;
    if (dokument != null) {
      DodajNamespace(dokument, out XmlNamespaceManager nsmgr);
      XmlElement documentElement = dokument.DocumentElement;
      XmlNodeList xmlNodeList = documentElement.SelectNodes("soap:Body/tns:RacunOdgovor/f73:Greske", nsmgr);
      if (provjera) xmlNodeList = documentElement.SelectNodes("soap:Body/tns:ProvjeraOdgovor/f73:Greske", nsmgr);

      foreach (XmlNode item3 in xmlNodeList) {
        string item = "";
        string item2 = "";
        XmlNode xmlNode2 = item3.SelectSingleNode("f73:Greska/f73:SifraGreske", nsmgr);
        if (xmlNode2 != null) {
          item = xmlNode2.InnerText.Trim();
        }
        XmlNode xmlNode3 = item3.SelectSingleNode("f73:Greska/f73:PorukaGreske", nsmgr);
        if (xmlNode3 != null) {
          item2 = xmlNode3.InnerText.Trim();
        }
        result = new Tuple<string, string>(item, item2);
      }
    }
    return result;
  }

  public static string DohvatiUuid (XmlDocument dokument, TipDokumentaEnum tipDokumenta) {
    string result = "";
    if (dokument != null) {
      DodajNamespace(dokument, out XmlNamespaceManager nsmgr);
      XmlElement documentElement = dokument.DocumentElement;
      XmlNode xmlNode = documentElement.SelectSingleNode($"soap:Body/tns:{tipDokumenta}/tns:Zaglavlje/tns:IdPoruke", nsmgr);
      if (xmlNode != null) {
        result = xmlNode.InnerText;
      }
    }
    return result;
  }

  public static string DohvatiSifruGreske (XmlDocument dokument, TipDokumentaEnum tipDokumenta) {
    string result = "";
    if (dokument != null) {
      DodajNamespace(dokument, out XmlNamespaceManager nsmgr);
      XmlElement documentElement = dokument.DocumentElement;
      XmlNode xmlNode = documentElement.SelectSingleNode($"soap:Body/tns:{tipDokumenta}/tns:Greske/tns:Greska/tns:SifraGreske", nsmgr);
      if (xmlNode != null) {
        result = xmlNode.InnerText;
      }
    }
    return result;
  }

  public static string DohvatiPorukuGreske (XmlDocument dokument, TipDokumentaEnum tipDokumenta) {
    string result = "";
    if (dokument != null) {
      DodajNamespace(dokument, out XmlNamespaceManager nsmgr);
      XmlElement documentElement = dokument.DocumentElement;
      XmlNode xmlNode = documentElement.SelectSingleNode($"soap:Body/tns:{tipDokumenta}/tns:Greske/tns:Greska/tns:PorukaGreske", nsmgr);
      if (xmlNode != null) {
        result = xmlNode.InnerText;
      }
    }
    return result;
  }

  public static string DohvatiGreskuRezultataZahtjeva (XmlDocument OdgovorGreska) {
    TipDokumentaEnum tipDokumentaEnum = OdrediTipDokumenta(OdgovorGreska);
    DodajNamespace(OdgovorGreska, out XmlNamespaceManager nsmgr);
    XmlElement documentElement = OdgovorGreska.DocumentElement;
    XmlNode xmlNode = documentElement.SelectSingleNode("soap:Body", nsmgr);
    XmlNode xmlNode2 = null;
    if (xmlNode.HasChildNodes) {
      switch (tipDokumentaEnum) {
        case TipDokumentaEnum.EchoOdgovor:
          xmlNode2 = xmlNode.SelectSingleNode("tns:EchoResponse", nsmgr);
          break;
        case TipDokumentaEnum.RacunOdgovor:
          xmlNode2 = xmlNode.SelectSingleNode("tns:RacunOdgovor", nsmgr);
          break;
        case TipDokumentaEnum.PoslovniProstorOdgovor:
          xmlNode2 = xmlNode.SelectSingleNode("tns:PoslovniProstorOdgovor", nsmgr);
          break;
      }
    }
    string text = "";
    if (xmlNode2 != null) {
      XmlNodeList xmlNodeList = xmlNode2.SelectNodes("tns:Greske", nsmgr);
      foreach (XmlNode item in xmlNodeList) {
        text += string.Format("{0} Šifra greške: {1}\n", item.FirstChild.SelectSingleNode("tns:PorukaGreske", nsmgr).InnerText, item.FirstChild.SelectSingleNode("tns:SifraGreske", nsmgr).InnerText);
      }
      return text;
    }
    return "Nepoznata greška";
  }

  public static void SnimiXmlDokumentDatoteka (XmlDocument dokument, string nazivDatoteke) {
    if (dokument != null) {
      try {
        dokument.Save(nazivDatoteke);
      } catch (Exception ex) {
        SimpleLog.Log(ex);
        Trace.TraceError($"Greška kod snimanja XML dokumenta u datoteku: {ex.Message}");
        throw;
      }
    }
  }

  public static XmlDocument SerijalizirajPoslovniProstorZahtjev (PoslovniProstorZahtjev poslovniProstorZahtjev) {
    string text = "";
    try {
      text = poslovniProstorZahtjev.Serialize();
    } catch (Exception ex) {
      SimpleLog.Log(ex);
      Trace.TraceError($"Greška kod serijalizacije zahtjeva za poslovni prostor: {ex.Message}");
      throw;
    }
    return UcitajXml(text);
  }

  public static XmlDocument SerijalizirajPoslovniProstor (PoslovniProstorType poslovniProstor) {
    string text = "";
    try {
      text = poslovniProstor.Serialize();
    } catch (Exception ex) {
      SimpleLog.Log(ex);
      Trace.TraceError($"Greška kod serijalizacije poslovnog prostora: {ex.Message}");
      throw;
    }
    return UcitajXml(text);
  }

  public static PoslovniProstorZahtjev KreirajPoslovniProstorZahtjev (PoslovniProstorType poslovniProstor) {
    PoslovniProstorZahtjev poslovniProstorZahtjev = new PoslovniProstorZahtjev {
      Id = "signXmlId",
      PoslovniProstor = poslovniProstor
    };
    ZaglavljeType zaglavljeType2 = poslovniProstorZahtjev.Zaglavlje = new ZaglavljeType {
      DatumVrijeme = Razno.DohvatiFormatiranoTrenutnoDatumVrijeme(),
      IdPoruke = Guid.NewGuid().ToString()
    };
    return poslovniProstorZahtjev;
  }

  public static PoslovniProstorZahtjev KreirajPoslovniProstorZahtjev (string ID, PoslovniProstorType poslovniProstor) {
    PoslovniProstorZahtjev poslovniProstorZahtjev = new PoslovniProstorZahtjev {
      Id = ID,
      PoslovniProstor = poslovniProstor
    };
    ZaglavljeType zaglavljeType2 = poslovniProstorZahtjev.Zaglavlje = new ZaglavljeType {
      DatumVrijeme = Razno.DohvatiFormatiranoTrenutnoDatumVrijeme(),
      IdPoruke = Guid.NewGuid().ToString()
    };
    return poslovniProstorZahtjev;
  }

  public static XmlDocument DohvatiPorukuEchoZahtjev (string poruka) {
    XmlDocument xmlDocument = null;
    Razno.FormatirajEchoPoruku(ref poruka);
    string xml = $"<tns:EchoRequest xmlns:tns=\"http://www.apis-it.hr/fin/2012/types/f73\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.apisit.hr/fin/2012/types/f73/FiskalizacijaSchema.xsd\">{poruka}</tns:EchoRequest>";
    try {
      xmlDocument = new XmlDocument();
      xmlDocument.LoadXml(xml);
      DodajSoapEnvelope(ref xmlDocument);
    } catch (Exception ex) {
      SimpleLog.Log(ex);
      Trace.TraceError($"Greška kod kreiranja poruke za ECHO zahtjev: {ex.Message}");
      throw;
    }
    return xmlDocument;
  }

  public static XmlDocument UcitajXml (string xml) {
    XmlDocument xmlDocument = null;
    if (!string.IsNullOrEmpty(xml)) {
      try {
        xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml);
      } catch (Exception ex) {
        SimpleLog.Log(ex);
        Trace.TraceError($"Greška kod učitavanja XML dokumenta: {ex.Message}");
        throw;
      }
    }
    return xmlDocument;
  }

  public static void DodajSoapEnvelope (ref XmlDocument dokument) {
    if (dokument != null && !string.IsNullOrEmpty(dokument.InnerXml) && dokument.DocumentElement != null) {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchemainstance\">");
      stringBuilder.Append("<soapenv:Body>");
      stringBuilder.Append($"<{dokument.DocumentElement.Name}");
      for (int i = 0; i < dokument.DocumentElement.Attributes.Count; i++) {
        stringBuilder.Append($" {dokument.DocumentElement.Attributes[i].Name}=\"{dokument.DocumentElement.Attributes[i].Value}\"");
      }
      stringBuilder.Append(">");
      stringBuilder.Append(dokument.DocumentElement.InnerXml);
      stringBuilder.Append($"</{dokument.DocumentElement.Name}>");
      stringBuilder.Append("</soapenv:Body>");
      stringBuilder.Append("</soapenv:Envelope>");
      string text = stringBuilder.ToString();
      text = text.Replace("<tns:Zaglavlje xmlns:tns=\"http://www.apis-it.hr/fin/2012/types/f73\">", "<tns:Zaglavlje>");
      text = text.Replace("<tns:Racun xmlns:tns=\"http://www.apis-it.hr/fin/2012/types/f73\">", "<tns:Racun>");
      try {
        dokument.LoadXml(text);
      } catch (Exception ex) {
        SimpleLog.Log(ex);
        Trace.TraceError($"Greška kod dodavanja SOAP envelop-a: {ex.Message}");
        throw;
      }
    }
  }

  public static void DodajNamespace (XmlDocument dokument, out XmlNamespaceManager nsmgr) {
    nsmgr = new XmlNamespaceManager(dokument.NameTable);
    nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
    nsmgr.AddNamespace("tns", "http://www.apis-it.hr/fin/2012/types/f73");
    nsmgr.AddNamespace("f73", "http://www.apis-it.hr/fin/2012/types/f73");
  }

  public static TipDokumentaEnum OdrediTipDokumenta (XmlDocument dokument) {
    TipDokumentaEnum result = TipDokumentaEnum.Nepoznato;
    if (dokument != null) {
      DodajNamespace(dokument, out XmlNamespaceManager nsmgr);
      if (dokument.DocumentElement != null) {
        XmlElement documentElement = dokument.DocumentElement;
        XmlNode xmlNode = documentElement.SelectSingleNode("soap:Body", nsmgr);
        if (xmlNode?.HasChildNodes ?? false) {
          switch (xmlNode.FirstChild.Name) {
            case "tns:EchoRequest":
              result = TipDokumentaEnum.EchoZahtjev;
              break;
            case "tns:EchoResponse":
              result = TipDokumentaEnum.EchoOdgovor;
              break;
            case "tns:RacunZahtjev":
              result = TipDokumentaEnum.RacunZahtjev;
              break;
            case "tns:RacunOdgovor":
              result = TipDokumentaEnum.RacunOdgovor;
              break;
            case "tns:PoslovniProstorZahtjev":
              result = TipDokumentaEnum.PoslovniProstorZahtjev;
              break;
            case "tns:PoslovniProstorOdgovor":
              result = TipDokumentaEnum.PoslovniProstorOdgovor;
              break;
          }
        }
      }
    }
    return result;
  }

  public static XmlDocument SerijalizirajProvjeraZahtjev (ProvjeraZahtjev provjeraZahtjev) {
    string text = "";
    try {
      text = provjeraZahtjev.Serialize();
      text = text.Replace("<tns:Pdv />", "");
      text = text.Replace("<tns:Pnp />", "");
      text = text.Replace("<tns:OstaliPor />", "");
      text = text.Replace("<tns:Naknade />", "");
    } catch (Exception ex) {
      SimpleLog.Log(ex);
      Trace.TraceError($"Greška kod serijalizacije zahtjeva za račun: {ex.Message}");
      throw;
    }
    return UcitajXml(text);
  }

  public static ProvjeraZahtjev KreirajProvjeraZahtjev (RacunType racun) {
    ProvjeraZahtjev provjeraZahtjev = new ProvjeraZahtjev {
      Id = "signXmlId",
      Racun = racun
    };
    ZaglavljeType zaglavljeType2 = provjeraZahtjev.Zaglavlje = new ZaglavljeType {
      DatumVrijeme = Razno.DohvatiFormatiranoTrenutnoDatumVrijeme(),
      IdPoruke = Guid.NewGuid().ToString()
    };
    return provjeraZahtjev;
  }

  public static ProvjeraZahtjev KreirajProvjeraZahtjev (string ID, RacunType racun) {
    ProvjeraZahtjev provjeraZahtjev = new ProvjeraZahtjev {
      Id = ID,
      Racun = racun
    };
    ZaglavljeType zaglavljeType2 = provjeraZahtjev.Zaglavlje = new ZaglavljeType {
      DatumVrijeme = Razno.DohvatiFormatiranoTrenutnoDatumVrijeme(),
      IdPoruke = Guid.NewGuid().ToString()
    };
    return provjeraZahtjev;
  }
}
