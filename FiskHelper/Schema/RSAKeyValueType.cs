using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
public class RSAKeyValueType : EntityBase<RSAKeyValueType> {
  [EditorBrowsable(EditorBrowsableState.Never)]
  private byte[] _modulus;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private byte[] _exponent;

  [XmlElement(DataType = "base64Binary")]
  public byte[] Modulus {
    get {
      return _modulus;
    }
    set {
      _modulus = value;
    }
  }

  [XmlElement(DataType = "base64Binary")]
  public byte[] Exponent {
    get {
      return _exponent;
    }
    set {
      _exponent = value;
    }
  }
}
