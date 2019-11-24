using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://www.apis-it.hr/fin/2012/types/f73")]
public class GreskaType : EntityBase<GreskaType> {
  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _sifraGreske;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _porukaGreske;

  public string SifraGreske {
    get {
      return _sifraGreske;
    }
    set {
      _sifraGreske = value;
    }
  }

  public string PorukaGreske {
    get {
      return _porukaGreske;
    }
    set {
      _porukaGreske = value;
    }
  }
}
