using System;
using System.Xml;

public class CentralniInformacijskiSustavEventArgs : EventArgs {
  public bool Cancel {
    get;
    set;
  }

  public XmlDocument SoapMessage {
    get;
    set;
  }
}
