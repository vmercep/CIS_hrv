using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://www.apis-it.hr/fin/2012/types/f73")]
public class PoslovniProstorType : EntityBase<PoslovniProstorType> {
  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _oib;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _oznPoslProstora;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private AdresniPodatakType _adresniPodatak;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _radnoVrijeme;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _datumPocetkaPrimjene;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private OznakaZatvaranjaType? _oznakaZatvaranja;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _specNamj;

  public string Oib {
    get {
      return _oib;
    }
    set {
      _oib = value;
    }
  }

  public string OznPoslProstora {
    get {
      return _oznPoslProstora;
    }
    set {
      _oznPoslProstora = value;
    }
  }

  public AdresniPodatakType AdresniPodatak {
    get {
      return _adresniPodatak;
    }
    set {
      _adresniPodatak = value;
    }
  }

  public string RadnoVrijeme {
    get {
      return _radnoVrijeme;
    }
    set {
      _radnoVrijeme = value;
    }
  }

  public string DatumPocetkaPrimjene {
    get {
      return _datumPocetkaPrimjene;
    }
    set {
      _datumPocetkaPrimjene = value;
    }
  }

  public OznakaZatvaranjaType OznakaZatvaranja {
    get {
      if (_oznakaZatvaranja.HasValue) {
        return _oznakaZatvaranja.Value;
      }
      return OznakaZatvaranjaType.Z;
    }
    set {
      _oznakaZatvaranja = value;
    }
  }

  [XmlIgnore]
  public bool OznakaZatvaranjaSpecified {
    get {
      return _oznakaZatvaranja.HasValue;
    }
    set {
      if (!value) {
        _oznakaZatvaranja = null;
      }
    }
  }

  public string SpecNamj {
    get {
      return _specNamj;
    }
    set {
      _specNamj = value;
    }
  }

  public PoslovniProstorType () {
    _adresniPodatak = new AdresniPodatakType();
  }
}
