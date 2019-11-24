using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
public class DigestMethodType : EntityBase<DigestMethodType> {
  [EditorBrowsable(EditorBrowsableState.Never)]
  private List<XmlNode> _any;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _algorithm;

  [XmlText]
  [XmlAnyElement]
  public List<XmlNode> Any {
    get {
      return _any;
    }
    set {
      _any = value;
    }
  }

  [XmlAttribute(DataType = "anyURI")]
  public string Algorithm {
    get {
      return _algorithm;
    }
    set {
      _algorithm = value;
    }
  }

  public DigestMethodType () {
    _any = new List<XmlNode>();
  }
}
