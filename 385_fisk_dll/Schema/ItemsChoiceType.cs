using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[XmlType(Namespace = "http://www.w3.org/2000/09/xmldsig#", IncludeInSchema = false)]
public enum ItemsChoiceType {
  [XmlEnum("##any:")]
  Item,
  X509CRL,
  X509Certificate,
  X509IssuerSerial,
  X509SKI,
  X509SubjectName
}
