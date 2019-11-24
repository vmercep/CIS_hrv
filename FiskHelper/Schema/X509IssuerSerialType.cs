using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
public class X509IssuerSerialType : EntityBase<X509IssuerSerialType> {
  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _x509IssuerName;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _x509SerialNumber;

  public string X509IssuerName {
    get {
      return _x509IssuerName;
    }
    set {
      _x509IssuerName = value;
    }
  }

  [XmlElement(DataType = "integer")]
  public string X509SerialNumber {
    get {
      return _x509SerialNumber;
    }
    set {
      _x509SerialNumber = value;
    }
  }
}
