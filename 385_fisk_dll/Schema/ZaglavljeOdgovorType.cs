using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://www.apis-it.hr/fin/2012/types/f73")]
public class ZaglavljeOdgovorType : EntityBase<ZaglavljeOdgovorType> {
  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _idPoruke;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _datumVrijeme;

  public string IdPoruke {
    get {
      return _idPoruke;
    }
    set {
      _idPoruke = value;
    }
  }

  public string DatumVrijeme {
    get {
      return _datumVrijeme;
    }
    set {
      _datumVrijeme = value;
    }
  }
}
