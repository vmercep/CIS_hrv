using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://www.apis-it.hr/fin/2012/types/f73")]
public class NaknadaType : EntityBase<NaknadaType> {
  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _nazivN;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _iznosN;

  public string NazivN {
    get {
      return _nazivN;
    }
    set {
      _nazivN = value;
    }
  }

  public string IznosN {
    get {
      return _iznosN;
    }
    set {
      _iznosN = value;
    }
  }
}
