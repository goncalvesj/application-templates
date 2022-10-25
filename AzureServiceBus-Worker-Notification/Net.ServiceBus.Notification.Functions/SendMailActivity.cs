using Azure.Communication.Email;
using Azure.Communication.Email.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Net.ServiceBus.Notification.Functions
{
    public class SendMailActivity
    {
        private readonly ILogger<SendMailActivity> _log;
        protected IConfiguration Configuration { get; }
        
        public SendMailActivity(ILogger<SendMailActivity> injectedLogger, IConfiguration configuration)
        {
            _log = injectedLogger;
            Configuration = configuration;
        }

        [FunctionName(nameof(SendMailActivity))]
        public async Task Run([ActivityTrigger] string myQueueItem)
        {
            var connString = Configuration["CommsServiceConnectionString"];
            var sender = Configuration["CommsServiceSender"];
            var destinationEmail = Configuration["DestinationEmail"];
            
            _log.LogInformation("Mock Email sent");

            return;

            var emailClient = new EmailClient(connString);

            var subject = "Send email plain text sample";
            var emailContent = new EmailContent(subject)
            {
                PlainText = myQueueItem,
                Html = ""
            };

            var emailRecipients = new EmailRecipients(new List<EmailAddress> {
                new EmailAddress(destinationEmail) { DisplayName = "" }
            });

            var emailMessage = new EmailMessage(sender, emailContent, emailRecipients)
            {
                Importance = EmailImportance.Low
            };

            try
            {
                SendEmailResult sendEmailResult = emailClient.Send(emailMessage);

                string messageId = sendEmailResult.MessageId;
                if (!string.IsNullOrEmpty(messageId))
                {
                    _log.LogInformation($"Email sent, MessageId = {messageId}");
                }
                else
                {
                    _log.LogInformation($"Failed to send email.");
                    return;
                }

                // wait max 2 minutes to check the send status for mail.
                var cancellationToken = new CancellationTokenSource(TimeSpan.FromMinutes(2));
                do
                {
                    SendStatusResult sendStatus = emailClient.GetSendStatus(messageId);
                    _log.LogInformation($"Send mail status for MessageId : <{messageId}>, Status: [{sendStatus.Status}]");

                    if (sendStatus.Status != SendStatus.Queued)
                    {
                        break;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(10));

                } while (!cancellationToken.IsCancellationRequested);

                if (cancellationToken.IsCancellationRequested)
                {
                    _log.LogInformation($"Looks like we timed out for email");
                }
            }
            catch (Exception ex)
            {
                _log.LogInformation($"Error in sending email, {ex}");
            }
        }
    }
}