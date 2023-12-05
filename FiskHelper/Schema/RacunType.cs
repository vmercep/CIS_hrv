using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://www.apis-it.hr/fin/2012/types/f73")]
public class RacunType : EntityBase<RacunType> {
  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _oib;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private bool _uSustPdv;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _datVrijeme;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private OznakaSlijednostiType _oznSlijed;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private BrojRacunaType _brRac;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private List<PorezType> _pdv;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private List<PorezType> _pnp;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private List<PorezOstaloType> _ostaliPor;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _iznosOslobPdv;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _iznosMarza;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _iznosNePodlOpor;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private List<NaknadaType> _naknade;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _iznosUkupno;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private NacinPlacanjaType _nacinPlac;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _oibOper;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _zastKod;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private bool _nakDost;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _paragonBrRac;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _specNamj;

    [EditorBrowsable(EditorBrowsableState.Never)]
    private NapojnicaType _napojnica;

    public string Oib {
    get {
      return _oib;
    }
    set {
      _oib = value;
    }
  }

  public bool USustPdv {
    get {
      return _uSustPdv;
    }
    set {
      _uSustPdv = value;
    }
  }

  public string DatVrijeme {
    get {
      return _datVrijeme;
    }
    set {
      _datVrijeme = value;
    }
  }

  public OznakaSlijednostiType OznSlijed {
    get {
      return _oznSlijed;
    }
    set {
      _oznSlijed = value;
    }
  }

  public BrojRacunaType BrRac {
    get {
      return _brRac;
    }
    set {
      _brRac = value;
    }
  }

  [XmlArrayItem("Porez", IsNullable = false)]
  public List<PorezType> Pdv {
    get {
      return _pdv;
    }
    set {
      _pdv = value;
    }
  }

  [XmlArrayItem("Porez", IsNullable = false)]
  public List<PorezType> Pnp {
    get {
      return _pnp;
    }
    set {
      _pnp = value;
    }
  }

  [XmlArrayItem("Porez", IsNullable = false)]
  public List<PorezOstaloType> OstaliPor {
    get {
      return _ostaliPor;
    }
    set {
      _ostaliPor = value;
    }
  }

  public string IznosOslobPdv {
    get {
      return _iznosOslobPdv;
    }
    set {
      _iznosOslobPdv = value;
    }
  }

  public string IznosMarza {
    get {
      return _iznosMarza;
    }
    set {
      _iznosMarza = value;
    }
  }

  public string IznosNePodlOpor {
    get {
      return _iznosNePodlOpor;
    }
    set {
      _iznosNePodlOpor = value;
    }
  }

  [XmlArrayItem("Naknada", IsNullable = false)]
  public List<NaknadaType> Naknade {
    get {
      return _naknade;
    }
    set {
      _naknade = value;
    }
  }

  public string IznosUkupno {
    get {
      return _iznosUkupno;
    }
    set {
      _iznosUkupno = value;
    }
  }

  public NacinPlacanjaType NacinPlac {
    get {
      return _nacinPlac;
    }
    set {
      _nacinPlac = value;
    }
  }

  public string OibOper {
    get {
      return _oibOper;
    }
    set {
      _oibOper = value;
    }
  }

  public string ZastKod {
    get {
      return _zastKod;
    }
    set {
      _zastKod = value;
    }
  }

  public bool NakDost {
    get {
      return _nakDost;
    }
    set {
      _nakDost = value;
    }
  }

  public string ParagonBrRac {
    get {
      return _paragonBrRac;
    }
    set {
      _paragonBrRac = value;
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

    public NapojnicaType Napojnica
    {
        get
        {
            return _napojnica;
        }
        set
        {
            _napojnica = value;
        }
    }

    public RacunType () {
    _naknade = new List<NaknadaType>();
    _ostaliPor = new List<PorezOstaloType>();
    _pnp = new List<PorezType>();
    _pdv = new List<PorezType>();
    _brRac = new BrojRacunaType();
        _napojnica = new NapojnicaType();
  }
}
