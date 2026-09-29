namespace ApiTCC.Utils
{
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using MimeKit;
    using Microsoft.Extensions.Configuration;
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task EnviarCodigoAsync(string emailDestino, string codigo)
        {
            var mensagem = new MimeMessage();
            mensagem.From.Add(new MailboxAddress("TCC Pet App", _configuration["ConfiguracaoEmail:Email"]));
            mensagem.To.Add(new MailboxAddress("", emailDestino));
            mensagem.Subject = "Recuperação de senha";

            mensagem.Body = new TextPart("plain")
            {
                Text = $"Seu código de recuperação de senha é: {codigo}\n\nEle expira em 15 minutos."
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(
                _configuration["ConfiguracaoEmail:Servidor"],
                int.Parse(_configuration["ConfiguracaoEmail:Porta"]),
                SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(
                _configuration["ConfiguracaoEmail:Email"],
                _configuration["ConfiguracaoEmail:SenhaApp"]);

            await client.SendAsync(mensagem);
            await client.DisconnectAsync(true);

        }
}
