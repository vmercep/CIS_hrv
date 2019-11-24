using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
public class SignatureValueType : EntityBase<SignatureValueType> {
  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _id;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private byte[] _value;

  [XmlAttribute(DataType = "ID")]
  public string Id {
    get {
      return _id;
    }
    set {
      _id = value;
    }
  }

  [XmlText(DataType = "base64Binary")]
  public byte[] Value {
    get {
      return _value;
    }
    set {
      _value = value;
    }
  }
}
