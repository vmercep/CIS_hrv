using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://www.apis-it.hr/fin/2012/types/f73")]
public class PorezType : EntityBase<PorezType> {
  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _stopa;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _osnovica;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _iznos;

  public string Stopa {
    get {
      return _stopa;
    }
    set {
      _stopa = value;
    }
  }

  public string Osnovica {
    get {
      return _osnovica;
    }
    set {
      _osnovica = value;
    }
  }

  public string Iznos {
    get {
      return _iznos;
    }
    set {
      _iznos = value;
    }
  }
}
