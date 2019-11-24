using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
public class TransformType : EntityBase<TransformType> {
  [EditorBrowsable(EditorBrowsableState.Never)]
  private List<object> _items;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private List<string> _text;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _algorithm;

  [XmlAnyElement]
  [XmlElement("XPath", typeof(string))]
  public List<object> Items {
    get {
      return _items;
    }
    set {
      _items = value;
    }
  }

  [XmlText]
  public List<string> Text {
    get {
      return _text;
    }
    set {
      _text = value;
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

  public TransformType () {
    _text = new List<string>();
    _items = new List<object>();
  }
}
