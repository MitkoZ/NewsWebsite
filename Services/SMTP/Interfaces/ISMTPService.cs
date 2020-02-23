namespace Services.SMTP.Interfaces
{
    public interface ISMTPService
    {
        public void SendEmail(string subject, string textEmail, string receiverEmail);
    }
}
