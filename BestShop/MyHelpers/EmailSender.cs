using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;

namespace BestShop.MyHelpers
{
    public class EmailSender
    {
        public static async void SendEmail(string toEmail, string username,
            string subject, string message)
        {
            string apiKey = "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";



            Configuration.Default.ApiKey["api-key"] = apiKey;
            var apiInstance = new TransactionalEmailsApi();

            string SenderName = username;
            string SenderEmail = "BestShop@Bestshop.com";
            SendSmtpEmailSender emailSender = new SendSmtpEmailSender(SenderName, SenderEmail);

            SendSmtpEmailTo emailReciver1 = new SendSmtpEmailTo(toEmail, username);
            List<SendSmtpEmailTo> To = new List<SendSmtpEmailTo>();
            To.Add(emailReciver1);

            string HtmlContent = null;
            string TextContent = message;
            try
            {
                var sendSmtpEmail = new SendSmtpEmail(emailSender, To, null, null, HtmlContent, TextContent, subject);
                CreateSmtpEmail result = await apiInstance.SendTransacEmailAsync(sendSmtpEmail);
                Console.WriteLine("Response: \n" + result.ToJson());
            }
            catch (Exception e)
            {
                Console.WriteLine("We have an Exception: \n" + e.Message);
            }
        }
    }
}
