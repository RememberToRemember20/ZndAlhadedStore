using System.ComponentModel.DataAnnotations;

namespace ZndAlhadedStore.Settings
{
    public class SmtpSettings
    {
        [Required] public string Host { get; set; } = string.Empty;
        [Range(1, 65535)] public int Port { get; set; } = 587;
        public bool UseImplicitTls { get; set; }   // false = STARTTLS (منفذ 587) ، true = SSL مباشر (منفذ 465)
        [Required] public string Username { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
        [Required, EmailAddress] public string FromAddress { get; set; } = string.Empty;
        [Required] public string FromName { get; set; } = string.Empty;
    }
}
