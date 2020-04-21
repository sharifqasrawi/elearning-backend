using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace E_Learning.Emails
{
    public class Email
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public Email(string to, string subject, string body)
        {
            this.To = to;
            this.Subject = subject;
            this.Body = body;
        }
        public void Send()
        {
            //Creating an email
            MailMessage email = new MailMessage();
            email.From = new MailAddress("info@qasrawi.fr");
            email.To.Add(new MailAddress(To));
            email.Subject = Subject;
            email.Body = Body.Replace("\n", "<br />");
            email.IsBodyHtml = true;
            //Sending the email
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "mail.qasrawi.fr";
            System.Net.NetworkCredential networkCredential = new System.Net.NetworkCredential();
            networkCredential.UserName = "info@qasrawi.fr";
            networkCredential.Password = "Info@53421";
            smtpClient.UseDefaultCredentials = true;
            smtpClient.Credentials = networkCredential;
            smtpClient.Port = 587;
            smtpClient.EnableSsl = false;
            smtpClient.Send(email);
        }
    }
}
