using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://www.apis-it.hr/fin/2012/types/f73")]
public class PrateciDokumentType : EntityBase<PrateciDokumentType>
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    private string _jirPd;



    public string JirPD
    {
        get
        {
            return _jirPd;
        }
        set
        {
            _jirPd = value;
        }
    }

  
}
