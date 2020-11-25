﻿using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public static class BarCode
    {

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

            LogFile.LogToFile("Generate QR code for bill id "+idTicket,LogLevel.Debug);
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
                                LogFile.LogToFile("QR code created "+ idTicket.ToString() + ".jpg and saved " + AppLink.QrCodeLocation, LogLevel.Debug);
                            }
                        }

                    }


                }

            }
            catch(Exception e)
            {
                LogFile.LogToFile("Greška u generiranju bar koda " + e.Message);
                throw new Exception("Error ocured in QR generation " + e.Message);
            }

        }
    }
}
