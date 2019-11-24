using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
public class PGPDataType : EntityBase<PGPDataType> {
  [EditorBrowsable(EditorBrowsableState.Never)]
  private object[] _items;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private ItemsChoiceType1[] _itemsElementName;

  [XmlAnyElement]
  [XmlElement("PGPKeyID", typeof(byte[]), DataType = "base64Binary")]
  [XmlElement("PGPKeyPacket", typeof(byte[]), DataType = "base64Binary")]
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
  public ItemsChoiceType1[] ItemsElementName {
    get {
      return _itemsElementName;
    }
    set {
      _itemsElementName = value;
    }
  }
}
