using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#", IncludeInSchema = false)]
public enum ItemsChoiceType1 {
  [XmlEnum("##any:")]
  Item,
  PGPKeyID,
  PGPKeyPacket
}
