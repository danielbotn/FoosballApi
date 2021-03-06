using FoosballApi.Helpers;
using FoosballApi.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace FoosballApi
{
    public interface IEmailService
    {
        void Send(string to, string subject, string html, string from = null);
        void SendVerificationEmail(VerificationModel vModel, User user, string origin);
        void SendPasswordResetEmail(VerificationModel vModel, User user, string origin);
    }

    public class EmailService : IEmailService
    {
        private readonly Secrets _secrets;

        public EmailService(Secrets secrets)
        {
            _secrets = secrets;
        }

        public void Send(string to, string subject, string html, string from = null)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from ?? _secrets.SmtpEmailFrom));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect(_secrets.SmtpHost, int.Parse(_secrets.SmtpPort), SecureSocketOptions.StartTls);
            smtp.Authenticate(_secrets.SmtpUser, _secrets.SmtpPass);
            smtp.Send(email);
            smtp.Disconnect(true);
        }

        public void SendPasswordResetEmail(VerificationModel vModel, User user, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                var resetUrl = $"{origin}/auth/reset-password?token={vModel.PasswordResetToken}";
                message = $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
                             <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
            }
            else
            {
                message = $@"<p>Please use the below token to reset your password with the <code>/accounts/reset-password</code> api route:</p>
                             <p><code>{vModel.PasswordResetToken}</code></p>";
            }

            Send(
                to: user.Email,
                subject: "Foosball - Reset Password",
                html: $@"<h4>Reset Password Email</h4>
                         {message}"
            );
        }

        public void SendVerificationEmail(VerificationModel vModel, User user, string origin)
        {
            string message;
            if (!string.IsNullOrEmpty(origin))
            {
                message = $@"<p>Your conformation code:</p>
                             <p>{vModel.VerificationToken}</p>";
            }
            else
            {
                message = $@"<p>Please use the below token to verify your email address with the <code>/accounts/verify-email</code> api route:</p>
                             <p><code>{vModel.VerificationToken}</code></p>";
            }

            Send(
                to: user.Email,
                subject: "Foosball - Verify Email",
                html: $@"<h4>Verify Email</h4>
                         <p>Thanks for registering!</p>
                         {message}"
            );
        }
    }
}