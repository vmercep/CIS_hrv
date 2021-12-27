using QRCoder;
using System;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

using System.Reflection;


namespace Helper
{
    public static class BarCode
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        private static System.Drawing.Image ResizeImage(System.Drawing.Image image, int size)
        {
            System.Drawing.Image resizedImage = new Bitmap(size, size);

            using (Graphics graphicsHandler = Graphics.FromImage(resizedImage))
            {
                graphicsHandler.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsHandler.DrawImage(image, 0, 0, size, size);
            }

            return resizedImage;
        }

        public static void GenerateQrCode(string zki, DateTime datVrijeme, string iznosUkupno, int idTicket)
        {

            log.Debug("Generate QR code for bill id "+idTicket);
            QRCodeGenerator.ECCLevel eccLevel = QRCodeGenerator.ECCLevel.L;
            int pixelsPerModule = 30;
            string foregroundColor = "#000000";
            string backgroundColor = "#FFFFFF";

            iznosUkupno = iznosUkupno.Replace(".", string.Empty);
            iznosUkupno = iznosUkupno.Replace(",", string.Empty);

            string payloadString = String.Format(AppLink.QrCodeMessage, zki, datVrijeme.ToString("yyyyMMdd_HHmm"), iznosUkupno);

            FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
            string directoryName = fileInfo.DirectoryName;

            try
            {
                using (var generator = new QRCodeGenerator())
                {
                    using (var data = generator.CreateQrCode(payloadString, eccLevel))
                    {
                        using (var code = new QRCode(data))
                        {
                            using (var bitmap = code.GetGraphic(pixelsPerModule, foregroundColor, backgroundColor, true))
                            {

                                var actualFormat = ImageFormat.Jpeg;

                                var resized = ResizeImage(bitmap, Convert.ToInt32(AppLink.QrCodeSize));

                                var directoryInfo = new DirectoryInfo(AppLink.QrCodeLocation+"\\");
                                if (!directoryInfo.Exists)
                                {
                                    directoryInfo.Create();
                                }
                                resized.Save(AppLink.QrCodeLocation + "\\" + idTicket.ToString() + ".jpg", actualFormat);
                                log.Debug("QR code created "+ idTicket.ToString() + ".jpg and saved " + AppLink.QrCodeLocation);
                            }
                        }

                    }


                }

            }
            catch(Exception e)
            {
                log.Error("Greška u generiranju bar koda ", e);
                throw new Exception("Error ocured in QR generation " + e.Message);
            }

        }
    }
}
