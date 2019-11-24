using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
public class SPKIDataType : EntityBase<SPKIDataType> {
  [EditorBrowsable(EditorBrowsableState.Never)]
  private byte[][] _sPKISexp;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private XmlElement _any;

  [XmlElement("SPKISexp", DataType = "base64Binary")]
  public byte[][] SPKISexp {
    get {
      return _sPKISexp;
    }
    set {
      _sPKISexp = value;
    }
  }

  [XmlAnyElement]
  public XmlElement Any {
    get {
      return _any;
    }
    set {
      _any = value;
    }
  }
}
