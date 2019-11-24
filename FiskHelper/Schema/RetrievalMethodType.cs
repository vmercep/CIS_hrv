using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
public class RetrievalMethodType : EntityBase<RetrievalMethodType> {
  [EditorBrowsable(EditorBrowsableState.Never)]
  private List<TransformType> _transforms;

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

  public RetrievalMethodType () {
    _transforms = new List<TransformType>();
  }
}
