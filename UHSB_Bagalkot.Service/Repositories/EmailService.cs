using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHSB_Bagalkot.Service.Common;
using UHSB_Bagalkot.Service.Interface;

namespace UHSB_Bagalkot.Service.Repositories
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string otp)
        {
            CommonEnum.WriteLog($"SendEmailAsync called. To: {toEmail}, Subject: {subject}");
            try
            {
                if (string.IsNullOrWhiteSpace(toEmail))
                {
                    CommonEnum.WriteLog("SendEmailAsync failed: Recipient email is null or empty.");
                    throw new ArgumentException("Recipient email cannot be null or empty.", nameof(toEmail));
                }

                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("UHSB Horti Guide", "naveenv4500@gmail.com"));
                emailMessage.To.Add(MailboxAddress.Parse(toEmail));
                emailMessage.Subject = subject;

                string htmlBody = $@"
<html>
    <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;'>
        <table align='center' width='100%' cellpadding='0' cellspacing='0' style='max-width: 600px; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 0 10px rgba(0,0,0,0.1);'>
            <tr>
                <td style='padding: 30px; text-align: center;'>
                    <h2 style='color: #004d40;'>Your OTP Code</h2>
                    <p style='font-size: 16px; color: #333;'>Use the following OTP to login to <strong>UHSB Horti Guide</strong>:</p>
                    <p style='font-size: 24px; font-weight: bold; margin: 20px 0; color: #004d40;'>{otp}</p>
                    <p style='font-size: 14px; color: #555;'>This OTP is valid for 5 minutes. If you did not request it, please ignore this email.</p>
                </td>
            </tr>
            <tr>
                <td style='padding: 20px; text-align: center; background-color: #eeeeee; font-size: 12px; color: #777;'>
                    &copy; 2025 University of Horticultural Sciences, Bagalkot. All rights reserved.
                </td>
            </tr>
        </table>
    </body>
</html>";

                emailMessage.Body = new TextPart("html") { Text = htmlBody };

                using var client = new SmtpClient();
                CommonEnum.WriteLog("Connecting to SMTP server...");
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                CommonEnum.WriteLog("Authenticating SMTP client...");
                await client.AuthenticateAsync("naveenv4500@gmail.com", "awvi zukp txyp vyyr"); // Use App Password

                CommonEnum.WriteLog($"Sending email to {toEmail}...");
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);

                CommonEnum.WriteLog($"OTP email successfully sent to {toEmail}");
            }
            catch (Exception ex)
            {
                CommonEnum.WriteLog($"SendEmailAsync failed for {toEmail}: {ex.Message}\n{ex.StackTrace}");
                throw; // Optional: rethrow if you want the caller to handle
            }
            finally
            {
                CommonEnum.WriteLog("SendEmailAsync finished execution.");
            }
        }



    }
}