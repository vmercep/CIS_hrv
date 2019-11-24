using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
public class ReferenceType : EntityBase<ReferenceType> {
  [EditorBrowsable(EditorBrowsableState.Never)]
  private List<TransformType> _transforms;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private DigestMethodType _digestMethod;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private byte[] _digestValue;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _id;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _uRI;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _type;

  [XmlArrayItem("Transform", IsNullable = false)]
  public List<TransformType> Transforms {
    get {
      return _transforms;
    }
    set {
      _transforms = value;
    }
  }

  public DigestMethodType DigestMethod {
    get {
      return _digestMethod;
    }
    set {
      _digestMethod = value;
    }
  }

  [XmlElement(DataType = "base64Binary")]
  public byte[] DigestValue {
    get {
      return _digestValue;
    }
    set {
      _digestValue = value;
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

  [XmlAttribute(DataType = "anyURI")]
  public string URI {
    get {
      return _uRI;
    }
    set {
      _uRI = value;
    }
  }

  [XmlAttribute(DataType = "anyURI")]
  public string Type {
    get {
      return _type;
    }
    set {
      _type = value;
    }
  }

  public ReferenceType () {
    _digestMethod = new DigestMethodType();
    _transforms = new List<TransformType>();
  }
}
