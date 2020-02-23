namespace Services.SMTP.DTOs
{
    public class SMTPConfigDTO
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string SenderEmail { get; set; }
        public string SenderPassword { get; set; }
    }
}
