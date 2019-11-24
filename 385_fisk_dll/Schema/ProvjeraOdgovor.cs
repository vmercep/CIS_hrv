using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://www.apis-it.hr/fin/2012/types/f73")]
[XmlRoot(Namespace = "http://www.apis-it.hr/fin/2012/types/f73", IsNullable = false)]
public class ProvjeraOdgovor : EntityBase<ProvjeraOdgovor> {
  [EditorBrowsable(EditorBrowsableState.Never)]
  private ZaglavljeOdgovorType _zaglavlje;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private RacunType _racun;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private List<GreskaType> _greske;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _id;

  public ZaglavljeOdgovorType Zaglavlje {
    get {
      return _zaglavlje;
    }
    set {
      _zaglavlje = value;
    }
  }

  public RacunType Racun {
    get {
      return _racun;
    }
    set {
      _racun = value;
    }
  }

  [XmlArrayItem("Greska", IsNullable = false)]
  public List<GreskaType> Greske {
    get {
      return _greske;
    }
    set {
      _greske = value;
    }
  }

  [XmlAttribute]
  public string Id {
    get {
      return _id;
    }
    set {
      _id = value;
    }
  }

  public ProvjeraOdgovor () {
    _greske = new List<GreskaType>();
    _racun = new RacunType();
    _zaglavlje = new ZaglavljeOdgovorType();
  }
}
