using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
public class X509DataType : EntityBase<X509DataType> {
  [EditorBrowsable(EditorBrowsableState.Never)]
  private object[] _items;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private ItemsChoiceType[] _itemsElementName;

  [XmlAnyElement]
  [XmlElement("X509CRL", typeof(byte[]), DataType = "base64Binary")]
  [XmlElement("X509Certificate", typeof(byte[]), DataType = "base64Binary")]
  [XmlElement("X509IssuerSerial", typeof(X509IssuerSerialType))]
  [XmlElement("X509SKI", typeof(byte[]), DataType = "base64Binary")]
  [XmlElement("X509SubjectName", typeof(string))]
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
  public ItemsChoiceType[] ItemsElementName {
    get {
      return _itemsElementName;
    }
    set {
      _itemsElementName = value;
    }
  }
}
