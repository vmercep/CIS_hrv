using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
public class SignedInfoType : EntityBase<SignedInfoType> {
  [EditorBrowsable(EditorBrowsableState.Never)]
  private CanonicalizationMethodType _canonicalizationMethod;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private SignatureMethodType _signatureMethod;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private List<ReferenceType> _reference;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _id;

  public CanonicalizationMethodType CanonicalizationMethod {
    get {
      return _canonicalizationMethod;
    }
    set {
      _canonicalizationMethod = value;
    }
  }

  public SignatureMethodType SignatureMethod {
    get {
      return _signatureMethod;
    }
    set {
      _signatureMethod = value;
    }
  }

  [XmlElement("Reference")]
  public List<ReferenceType> Reference {
    get {
      return _reference;
    }
    set {
      _reference = value;
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

  public SignedInfoType () {
    _reference = new List<ReferenceType>();
    _signatureMethod = new SignatureMethodType();
    _canonicalizationMethod = new CanonicalizationMethodType();
  }
}
