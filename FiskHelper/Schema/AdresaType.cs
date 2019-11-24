using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://www.apis-it.hr/fin/2012/types/f73")]
public class AdresaType : EntityBase<AdresaType> {
  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _ulica;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _kucniBroj;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _kucniBrojDodatak;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _brojPoste;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _naselje;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _opcina;

  public string Ulica {
    get {
      return _ulica;
    }
    set {
      _ulica = value;
    }
  }

  public string KucniBroj {
    get {
      return _kucniBroj;
    }
    set {
      _kucniBroj = value;
    }
  }

  public string KucniBrojDodatak {
    get {
      return _kucniBrojDodatak;
    }
    set {
      _kucniBrojDodatak = value;
    }
  }

  public string BrojPoste {
    get {
      return _brojPoste;
    }
    set {
      _brojPoste = value;
    }
  }

  public string Naselje {
    get {
      return _naselje;
    }
    set {
      _naselje = value;
    }
  }

  public string Opcina {
    get {
      return _opcina;
    }
    set {
      _opcina = value;
    }
  }
}
