using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
public class KeyValueType : EntityBase<KeyValueType> {
  [EditorBrowsable(EditorBrowsableState.Never)]
  private object _item;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private List<string> _text;

  [XmlAnyElement]
  [XmlElement("DSAKeyValue", typeof(DSAKeyValueType))]
  [XmlElement("RSAKeyValue", typeof(RSAKeyValueType))]
  public object Item {
    get {
      return _item;
    }
    set {
      _item = value;
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

  public KeyValueType () {
    _text = new List<string>();
  }
}
