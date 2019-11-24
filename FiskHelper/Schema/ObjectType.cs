using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
public class ObjectType : EntityBase<ObjectType> {
  [EditorBrowsable(EditorBrowsableState.Never)]
  private List<XmlNode> _any;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _id;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _mimeType;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private string _encoding;

  [XmlText]
  [XmlAnyElement]
  public List<XmlNode> Any {
    get {
      return _any;
    }
    set {
      _any = value;
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

  [XmlAttribute]
  public string MimeType {
    get {
      return _mimeType;
    }
    set {
      _mimeType = value;
    }
  }

  [XmlAttribute(DataType = "anyURI")]
  public string Encoding {
    get {
      return _encoding;
    }
    set {
      _encoding = value;
    }
  }

  public ObjectType () {
    _any = new List<XmlNode>();
  }
}
