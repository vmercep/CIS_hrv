using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(Namespace = "http://www.apis-it.hr/fin/2012/types/f73")]
public class NapojnicaType : EntityBase<NapojnicaType>
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    private string _iznosNapojnice;

    [EditorBrowsable(EditorBrowsableState.Never)]
    private string _nacinPlacanjaNapojnice;



    public string iznosNapojnice
    {
        get
        {
            return _iznosNapojnice;
        }
        set
        {
            _iznosNapojnice = value;
        }
    }

    public string nacinPlacanjaNapojnice
    {
        get
        {
            return _nacinPlacanjaNapojnice;
        }
        set
        {
            _nacinPlacanjaNapojnice = value;
        }
    }

   
}
