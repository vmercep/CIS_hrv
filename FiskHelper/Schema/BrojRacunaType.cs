using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://www.apis-it.hr/fin/2012/types/f73")]
public class BrojRacunaType : EntityBase<BrojRacunaType> {
  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _brOznRac;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _oznPosPr;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _oznNapUr;

  public string BrOznRac {
    get {
      return _brOznRac;
    }
    set {
      _brOznRac = value;
    }
  }

  public string OznPosPr {
    get {
      return _oznPosPr;
    }
    set {
      _oznPosPr = value;
    }
  }

  public string OznNapUr {
    get {
      return _oznNapUr;
    }
    set {
      _oznNapUr = value;
    }
  }
}
