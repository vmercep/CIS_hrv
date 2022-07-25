using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Xml.Serialization;

[Serializable]
[GeneratedCode("System.Xml", "4.6.1055.0")]
[DesignerCategory("code")]
[XmlType(AnonymousType = true, Namespace = "http://www.apis-it.hr/fin/2012/types/f73")]
[XmlRoot(Namespace = "http://www.apis-it.hr/fin/2012/types/f73", IsNullable = false)]
public class PromijeniNacPlacZahtjev : EntityBase<PromijeniNacPlacZahtjev>
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    private ZaglavljeType _zaglavlje;

    [EditorBrowsable(EditorBrowsableState.Never)]
    private RacunPNPType _racun;

    [EditorBrowsable(EditorBrowsableState.Never)]
    private string _id;

    public ZaglavljeType Zaglavlje
    {
        get
        {
            return _zaglavlje;
        }
        set
        {
            _zaglavlje = value;
        }
    }

    public RacunPNPType Racun
    {
        get
        {
            return _racun;
        }
        set
        {
            _racun = value;
        }
    }

    [XmlAttribute]
    public string Id
    {
        get
        {
            return _id;
        }
        set
        {
            _id = value;
        }
    }

    

    public PromijeniNacPlacZahtjev()
    {
        _racun = new RacunPNPType();
        _zaglavlje = new ZaglavljeType();

    }
}
