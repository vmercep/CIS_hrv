using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#")]
public class DSAKeyValueType : EntityBase<DSAKeyValueType> {
  [EditorBrowsable(EditorBrowsableState.Never)]
  private byte[] _p;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private byte[] _q;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private byte[] _g;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private byte[] _y;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private byte[] _j;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private byte[] _seed;

  [EditorBrowsable(EditorBrowsableState.Never)]
  private byte[] _pgenCounter;

  [XmlElement(DataType = "base64Binary")]
  public byte[] P {
    get {
      return _p;
    }
    set {
      _p = value;
    }
  }

  [XmlElement(DataType = "base64Binary")]
  public byte[] Q {
    get {
      return _q;
    }
    set {
      _q = value;
    }
  }

  [XmlElement(DataType = "base64Binary")]
  public byte[] G {
    get {
      return _g;
    }
    set {
      _g = value;
    }
  }

  [XmlElement(DataType = "base64Binary")]
  public byte[] Y {
    get {
      return _y;
    }
    set {
      _y = value;
    }
  }

  [XmlElement(DataType = "base64Binary")]
  public byte[] J {
    get {
      return _j;
    }
    set {
      _j = value;
    }
  }

  [XmlElement(DataType = "base64Binary")]
  public byte[] Seed {
    get {
      return _seed;
    }
    set {
      _seed = value;
    }
  }

  [XmlElement(DataType = "base64Binary")]
  public byte[] PgenCounter {
    get {
      return _pgenCounter;
    }
    set {
      _pgenCounter = value;
    }
  }
}
