using System.Net;
using System.Net.Mail;

namespace ManejoPresupuesto.Servicios
{
    public class ServicioEmail : IServicioEmail
    {
        private readonly IConfiguration Configuration;
        public ServicioEmail(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public async Task EnviarEmail(string receptor, string enlace)
        {
            // Lógica para enviar el email utilizando la configuración proporcionada
            var email = Configuration.GetValue<string>("CONFIGURACIONES_EMAIL:EMAIL");
            var host = Configuration.GetValue<string>("CONFIGURACIONES_EMAIL:HOST");
            var puerto = Configuration.GetValue<int>("CONFIGURACIONES_EMAIL:PORT");
            var password = Configuration.GetValue<string>("CONFIGURACIONES_EMAIL:PASSWORD");

            var cliente = new SmtpClient(host, puerto);
            cliente.EnableSsl = true;
            cliente.UseDefaultCredentials = false;
            cliente.Credentials = new NetworkCredential(email, password);
            var emisor = email;
            var subject = "Olvido su contraseña";
            var body = $"<h1>Recuperación de contraseña</h1>" +
                       $"<p>Haga clic en el siguiente enlace para restablecer su contraseña:</p>" +
                       $"<a href='{enlace}'>Restablecer contraseña</a>";
            var mensaje = new MailMessage(emisor, receptor, subject, body);
            await cliente.SendMailAsync(mensaje);

        }
    }
}
