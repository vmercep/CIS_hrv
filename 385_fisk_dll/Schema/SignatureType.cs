using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
public class SignatureType : EntityBase<SignatureType> {
  [EditorBrowsable(EditorBrowsableState.Never)]
  private SignedInfoType _signedInfo;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private SignatureValueType _signatureValue;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private KeyInfoType _keyInfo;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private List<ObjectType> _object;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _id;

  public SignedInfoType SignedInfo {
    get {
      return _signedInfo;
    }
    set {
      _signedInfo = value;
    }
  }

  public SignatureValueType SignatureValue {
    get {
      return _signatureValue;
    }
    set {
      _signatureValue = value;
    }
  }

  public KeyInfoType KeyInfo {
    get {
      return _keyInfo;
    }
    set {
      _keyInfo = value;
    }
  }

  [XmlElement("Object")]
  public List<ObjectType> Object {
    get {
      return _object;
    }
    set {
      _object = value;
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

  public SignatureType () {
    _object = new List<ObjectType>();
    _keyInfo = new KeyInfoType();
    _signatureValue = new SignatureValueType();
    _signedInfo = new SignedInfoType();
  }
}
