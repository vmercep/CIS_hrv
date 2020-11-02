using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _385_fisk.Exceptions
{
    public partial class ErrorMessageBox : Form
    {
        public ErrorMessageBox()
        {
            InitializeComponent();
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btSendToSupport_Click(object sender, EventArgs e)
        {
            try
            {



                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("mail.ikosoft.com.hr", 587);

                mail.From = new MailAddress("info@ikosoft.com.hr");
                mail.To.Add("info@ikosoft.com.hr");
                mail.Subject = "Greška na CIS aplikaciji "+AppLink.VATNumber;
                mail.Body = lbMessageText.Text + "\r\n" + lbMessageDetails.Text;

                //SmtpServer.Port = 465;
                SmtpServer.Credentials = new System.Net.NetworkCredential("info@ikosoft.com.hr", "1J7dTROd0u");
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpServer.EnableSsl = true;
                SmtpServer.Timeout = 10000;
                SmtpServer.Send(mail);
                //MessageBox.Show("Mail Send");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Close();
            }
        }
    }
}
