using MailKit.Net.Smtp;
using MimeKit;
using Services.SMTP.DTOs;
using Services.SMTP.Interfaces;

namespace Services.SMTP
{
    public class SMTPService : ISMTPService
    {
        private readonly string server;
        private readonly int port;
        private readonly string senderEmail;
        private readonly string senderPassword;

        public SMTPService(SMTPConfigDTO smtpConfigDTO)
        {
            this.server = smtpConfigDTO.Server;
            this.port = smtpConfigDTO.Port;
            this.senderEmail = smtpConfigDTO.SenderEmail;
            this.senderPassword = smtpConfigDTO.SenderPassword;
        }

        /// <summary>
        /// For Gmail, Less secure apps should be turned on from https://myaccount.google.com/lesssecureapps 
        /// (If an app or site doesn't meet our security standards, Google might block anyone who's trying to sign in to your account from it.
        /// Less secure apps can make it easier for hackers to get in to your account, so blocking sign-ins from these apps helps keep your account safe. 
        /// For more info about less secure apps option: https://support.google.com/accounts/answer/6010255?hl=en)
        /// </summary>
        /// <param name="subject">The subject of the email</param>
        /// <param name="textEmail">The email in a text format</param>
        /// <param name="receiverEmail">The receiver's email</param>
        public void SendEmail(string subject, string textEmail, string receiverEmail)
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress(this.senderEmail));
            message.To.Add(new MailboxAddress(receiverEmail));
            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = textEmail
            };

            using (SmtpClient smtpClient = new SmtpClient())
            {
                smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;

                smtpClient.Connect(this.server, this.port, false);

                // Note: only needed if the SMTP server requires authentication
                smtpClient.Authenticate(this.senderEmail, this.senderPassword);

                smtpClient.Send(message);
                smtpClient.Disconnect(true);
            }
        }
    }
}
