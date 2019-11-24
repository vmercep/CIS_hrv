using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
public class KeyInfoType : EntityBase<KeyInfoType> {
  [EditorBrowsable(EditorBrowsableState.Never)]
  private object[] _items;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private ItemsChoiceType2[] _itemsElementName;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private List<string> _text;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _id;

  [XmlAnyElement]
  [XmlElement("KeyName", typeof(string))]
  [XmlElement("KeyValue", typeof(KeyValueType))]
  [XmlElement("MgmtData", typeof(string))]
  [XmlElement("PGPData", typeof(PGPDataType))]
  [XmlElement("RetrievalMethod", typeof(RetrievalMethodType))]
  [XmlElement("SPKIData", typeof(SPKIDataType))]
  [XmlElement("X509Data", typeof(X509DataType))]
  [XmlChoiceIdentifier("ItemsElementName")]
  public object[] Items {
    get {
      return _items;
    }
    set {
      _items = value;
    }
  }

  [XmlElement("ItemsElementName")]
  [XmlIgnore]
  public ItemsChoiceType2[] ItemsElementName {
    get {
      return _itemsElementName;
    }
    set {
      _itemsElementName = value;
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

  [XmlAttribute(DataType = "ID")]
  public string Id {
    get {
      return _id;
    }
    set {
      _id = value;
    }
  }

  public KeyInfoType () {
    _text = new List<string>();
  }
}
