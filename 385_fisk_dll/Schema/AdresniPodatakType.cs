using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://www.apis-it.hr/fin/2012/types/f73")]
public class AdresniPodatakType : EntityBase<AdresniPodatakType> {
  [EditorBrowsable(EditorBrowsableState.Never)]
  private object _item;

  [XmlElement("Adresa", typeof(AdresaType))]
  [XmlElement("OstaliTipoviPP", typeof(string))]
  public object Item {
    get {
      return _item;
    }
    set {
      _item = value;
    }
  }
}
