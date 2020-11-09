using CisDal;
using DataObjects;
using Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace CisBl
{
    public class CisBussines
    {

        private string ProperNumber(string myNumber)
        {
            string text = myNumber.Replace(" ", "");
            text = text.Replace("'", "");
            return text.Replace(",", ".");
        }

        private decimal calculateTax(string totalBill, string taxRate)
        {
            decimal ammount = 0;
            ammount = Convert.ToDecimal(totalBill) - Convert.ToDecimal(taxRate);
            return ammount;
        }

        private RacunType GetRacun(DataBill DataBillToSend, IMerlinData dalMerlin)
        {
            CultureInfo cultureInfo = new CultureInfo("hr-HR");
            RacunType racunType = new RacunType
            {
                Oib = DataBillToSend.VATNumber_Salon_Bill,
                USustPdv = DataBillToSend.TaxPayer_Bill,
                DatVrijeme = DataBillToSend.DateTimeIssue_Bill(DataBillToSend.BillDate_Bill),
                OznSlijed = DataBillToSend.SequenceMark_Bill
            };
            BrojRacunaType brojRacunaType2 = racunType.BrRac = new BrojRacunaType
            {
                BrOznRac = DataBillToSend.BillNumberMark_Bill,
                OznPosPr = DataBillToSend.PremiseMark_Bill,
                OznNapUr = DataBillToSend.BillingDeviceMark_Bill
            };
            racunType.IznosUkupno = ProperNumber(DataBillToSend.TotalAmount_Bill);
            racunType.NacinPlac = DataBillToSend.PaymentMethod_Bill;
            racunType.OibOper = DataBillToSend.CashierVATNumber_Bill;
            string notes = DataBillToSend.Notes;
            if (AppLink.InVATsystem == "1")
            {
                if (Convert.ToDecimal(DataBillToSend.TotalAmount_Bill) != decimal.Zero)
                {
                    if (dalMerlin.checkIfNewTaxes())
                    {
                        List<DataNewTax> newTaxes = dalMerlin.GetNewTaxes(DataBillToSend.IdTicket);
                        foreach (DataNewTax item2 in newTaxes)
                        {
                            decimal num = 0;
                            if (newTaxes.Count > 1)
                            {
                                num = Convert.ToDecimal(item2.TaxableAmount);
                            }
                            else num = calculateTax(DataBillToSend.TotalAmount_Bill, item2.TaxAmount);  //Convert.ToDecimal(item2.TaxableAmount);
                            decimal num2 = Convert.ToDecimal(item2.TaxAmount);
                            decimal num3 = Convert.ToDecimal(item2.TaxRate);
                            racunType.Pdv.Add(new PorezType
                            {
                                Osnovica = ProperNumber(num.ToString("0.00")),
                                Iznos = ProperNumber(num2.ToString("0.00")),
                                Stopa = ProperNumber(num3.ToString("0.00"))
                            });
                        }
                    }
                    else
                    {
                        PorezType item = new PorezType
                        {
                            Stopa = ProperNumber(DataBillToSend.VATTaxRate_Bill),
                            Osnovica = ProperNumber(DataBillToSend.VATBase_Bill),
                            Iznos = ProperNumber(DataBillToSend.VATAmount_Bill)
                        };
                        racunType.Pdv.Add(item);
                    }
                }
            }
            if (notes.Length > 0)
            {
                int count = Regex.Matches(notes, "/", RegexOptions.IgnoreCase).Count;
                if (count > 1 && notes.Contains(";"))
                {
                    string[] array = notes.Split(';', '\r', '\n');
                    racunType.ParagonBrRac = array.GetValue(0).ToString();
                }
            }

            return racunType;

        }

        public bool CheckBillAnswer(XmlDocument xmlDocument, bool test, out string message)
        {
            Tuple<string, string> tuple = XmlDokumenti.DohvatiStatusGreške(xmlDocument, test);

            if (tuple == null || tuple.Item1.Equals("v100"))
            {
                LogFile.LogToFile("Test OK! sending to production CIS", LogLevel.Debug);
                message = null;
                return true;
            }

            LogFile.LogToFile(String.Format("Error in sending bill to cis {0}", JsonConvert.SerializeObject(xmlDocument)), LogLevel.Debug);
            string ErrorCode = tuple.Item1;
            string ErrorMessage = Translations.Translate(tuple.Item2);
            message = ErrorCode + " " + ErrorMessage;
            return false;
        }


        public XmlDocument SendBill(DataBill dataBillToSend, IMerlinData dalMerlin, string CertificateName, bool test)
        {
            LogFile.LogToFile(String.Format("Bill id {0} sending to CIS, method SendBill", dataBillToSend.IdTicket), LogLevel.Debug);
            LogFile.LogToFile(String.Format("Bill content {0}", JsonConvert.SerializeObject(dataBillToSend)), LogLevel.Debug);

            try
            {
                var racunType = GetRacun(dataBillToSend, dalMerlin);
                LogFile.LogToFile(String.Format("Created bill to send content {0}", JsonConvert.SerializeObject(racunType)), LogLevel.Debug);

                
                string text = (!(AppLink.UseCertificateFile == "1")) ? Razno.ZastitniKodIzracun(CertificateName, racunType.Oib, racunType.DatVrijeme.Replace('T', ' '), racunType.BrRac.BrOznRac, racunType.BrRac.OznPosPr, racunType.BrRac.OznNapUr, racunType.IznosUkupno.ToString()) : Razno.ZastitniKodIzracun(AppLink.DatotekaCertifikata(), AppLink.CertificatePassword, racunType.Oib, racunType.DatVrijeme.Replace('T', ' '), racunType.BrRac.BrOznRac, racunType.BrRac.OznPosPr, racunType.BrRac.OznNapUr, racunType.IznosUkupno.ToString());
                
                //Barcode generiranje
                BarCode.GenerateQrCode(text, dataBillToSend.BillDate_Bill, racunType.IznosUkupno.ToString(), dataBillToSend.IdTicket);

                string text2 = dataBillToSend.Notes;
                bool flag = false;
                if (text2.Length == 0)
                {
                    text2 = "ZKI: " + text;
                    flag = true;
                }
                else if (!text2.Contains("ZKI:"))
                {
                    text2 = text2 + "\r\nZKI: " + text;
                    flag = true;
                }
                if (flag)
                {
                    LogFile.LogToFile(String.Format("Račun {0} poslan na potpisivanje spremam ZKI {1}", dataBillToSend.IdTicket, text), LogLevel.Debug);
                    int ret = dalMerlin.SaveNotes(dataBillToSend.IdTicket, text2);
                    LogFile.LogToFile(String.Format("Račun {0} Nadopunjen sa ZKI return {1}", dataBillToSend.IdTicket, ret), LogLevel.Debug);
                }
                racunType.ZastKod = text;
                racunType.NakDost = dataBillToSend.MarkSubseqBillDelivery_Bill;
                CentralniInformacijskiSustav centralniInformacijskiSustav = new CentralniInformacijskiSustav();
                if(test)
                {
                    return centralniInformacijskiSustav.PosaljiProvjeru(racunType);
                }
                XmlDocument xmlDocument = centralniInformacijskiSustav.PosaljiRacun(racunType, CertificateName);
                if (xmlDocument != null)
                {
                    bool flag2 = Potpisivanje.ProvjeriPotpis(xmlDocument);
                }

                return xmlDocument;

            }
            catch (Exception e)
            {
                LogFile.LogToFile("Greška se desila u fiskalizaciji računa " + e.Message);
                throw new Exception(e.Message);
            }

        }

    }
}
