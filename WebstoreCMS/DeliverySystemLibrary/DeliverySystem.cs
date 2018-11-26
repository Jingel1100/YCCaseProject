using System.Net;
using System.Net.Mail;
using System.Text;

namespace DeliverySystemLibrary
{
    public class DeliverySystem
    {
        public void SendMessage(string productString,string amountString, string guid)
        {
            string username = "Erik";
            string emailadress = "SET EMAIL";
            string htmlMessage = ComposeMessage(username, productString, amountString);
            string subject = ComposeSubject(guid);
            SendEmailAsync(emailadress, subject, htmlMessage);
        }

        public string ComposeMessage(string name, string productsOrderedString, string amountsOrderedString)
        {
            StringBuilder messageSb = new StringBuilder();
            messageSb.AppendLine($"Dear {name}");
            messageSb.AppendLine();
            messageSb.AppendLine("You have ordered the following products:");

            string[] productsOrdered = productsOrderedString.Split(',');
            string[] amountsOrdered = amountsOrderedString.Split(',');
            for (int i = 0; i < productsOrdered.Length-1; i++)
            {
                messageSb.Append("\t");
                messageSb.AppendLine($"{productsOrdered[i]}\t{amountsOrdered[i]}");
            }

            messageSb.AppendLine("Thank you and we hope to welcome you soon again.");
            messageSb.AppendLine("Regards:");
            messageSb.AppendLine("WebstoreMVC");
            messageSb.AppendLine();
            messageSb.AppendLine("This email has been automatically generated, replies will not be read");

            return messageSb.ToString();
        }

        public string ComposeSubject(string orderNumber)
        {
            return $"Confirmation for order: {orderNumber}"; ;
        }

        public void SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com")
            {
                EnableSsl = true,
                Port = 587,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("ErikITPH@gmail.com", "mjloferzqcdinock")
            };
            var mailMessage = new MailMessage
            {
                From = new MailAddress("ErikITPH@gmail.com")
            };
            mailMessage.To.Add(email);
            mailMessage.Subject = subject;
            mailMessage.Body = htmlMessage;
            //client.Send(mailMessage);
        }
    }
}