using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://www.apis-it.hr/fin/2012/types/f73")]
[XmlRoot(Namespace = "http://www.apis-it.hr/fin/2012/types/f73", IsNullable = false)]
public class PoslovniProstorZahtjev : EntityBase<PoslovniProstorZahtjev> {
  [EditorBrowsable(EditorBrowsableState.Never)]
  private ZaglavljeType _zaglavlje;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private PoslovniProstorType _poslovniProstor;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _id;

  public ZaglavljeType Zaglavlje {
    get {
      return _zaglavlje;
    }
    set {
      _zaglavlje = value;
    }
  }

  public PoslovniProstorType PoslovniProstor {
    get {
      return _poslovniProstor;
    }
    set {
      _poslovniProstor = value;
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

  public PoslovniProstorZahtjev () {
    _poslovniProstor = new PoslovniProstorType();
    _zaglavlje = new ZaglavljeType();
  }
}
