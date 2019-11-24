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
public class SignatureMethodType : EntityBase<SignatureMethodType> {
  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _hMACOutputLength;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private List<XmlNode> _any;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _algorithm;

  [XmlElement(DataType = "integer")]
  public string HMACOutputLength {
    get {
      return _hMACOutputLength;
    }
    set {
      _hMACOutputLength = value;
    }
  }

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

  public SignatureMethodType () {
    _any = new List<XmlNode>();
  }
}
