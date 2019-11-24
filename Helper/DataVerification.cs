
using DataObjects;

namespace Helper
{
    public static class DataVerification
    {

        public static bool VerifDataOk(DataSalon ds, out bool inVatActive, out string errorMessage)
        {
            errorMessage = "";
            inVatActive = false;
            if (string.IsNullOrEmpty(ds.VATNumber_Salon))
            {
                errorMessage="OIB poslovnog prostora je prazan!";
                return false;
            }
            if (ds.VATNumber_Salon.Length > 11)
            {
                errorMessage = "OIB poslovnog prostora sadrži previše znakova! (max. 11)";
                return false;
            }
            if (string.IsNullOrEmpty(AppLink.DateIsActive))
            {
                errorMessage = "Datum početka primjene je prazno!";
                return false;
            }
            if (string.IsNullOrEmpty(AppLink.URL))
            {
                errorMessage = "URL do CIS servisa je prazan!";
                return false;
            }
            if (string.IsNullOrEmpty(AppLink.PremiseMark))
            {
                errorMessage = "Oznaka poslovnog prostora je prazna!";
                return false;
            }
            if (string.IsNullOrEmpty(AppLink.BillingDeviceMark))
            {
                errorMessage = "Oznaka naplatnog uređaja je prazna!";
                return false;
            }
            if (string.IsNullOrEmpty(AppLink.OIBSoftware))
            {
                errorMessage = "OIB proizvođača software-a je prazan!";
                return false;
            }
            if (string.IsNullOrEmpty(AppLink.InVATsystem))
            {
                errorMessage = "Oznaka da li je poslovni prostor u sustavu PDV-a je prazna!";
                return false;
            }
            if (!(AppLink.InVATsystem == "0") && !(AppLink.InVATsystem == "1"))
            {
                errorMessage = "Oznaka da li je poslovni prostor u sustavu PDV-a je neispravna!";
                return false;
            }
            string inVATsystem = AppLink.InVATsystem;
            if (!(inVATsystem == "0"))
            {
                if (inVATsystem == "1")
                {
                    inVatActive = true;
                }
            }
            else
            {
                inVatActive = false;
            }
            if (string.IsNullOrEmpty(AppLink.ConnectionString))
            {
                errorMessage = "Postavka za konekciju sa SQL serverom je prazna! Kontaktirajte tehničku podršku!";
                return false;
            }
            if (AppLink.UseCertificateFile == "0" && string.IsNullOrEmpty(AppLink.Certificate))
            {
                errorMessage = "Naziv certifikata je prazan! Kontaktirajte tehničku podršku!";
                return false;
            }
            if (AppLink.UseCertificateFile == "1")
            {
                if (string.IsNullOrEmpty(AppLink.CertificatePassword))
                {
                    errorMessage = "Lozinka certifikata je prazna!";
                    return false;
                }
                if (!System.IO.File.Exists(AppLink.DatotekaCertifikata()))
                {
                    errorMessage = "Uključeno je korištenje certifikata iz datoteke no niste odabrali datotku u konfiguraciji!";
                    return false;
                }
            }
            return true;
        }

    }
}
