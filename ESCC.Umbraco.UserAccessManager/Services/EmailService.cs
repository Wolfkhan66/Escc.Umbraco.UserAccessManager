﻿using System.Configuration;
using System.Net.Mail;
using System.Text;
using ESCC.Umbraco.UserAccessManager.Models;
using ESCC.Umbraco.UserAccessManager.Services.Interfaces;
using Exceptionless;
using Exceptionless.Extensions;

namespace ESCC.Umbraco.UserAccessManager.Services
{
    public class EmailService : IEmailService
    {
        /// <summary>
        /// Sends and email to the given address
        /// </summary>
        /// <param name="emailTo">Address of user you wish to email</param>
        /// <param name="emailSubject">Subject line of email </param>
        /// <param name="emailBody">Body text of email</param>
        private void SmtpSendEmail(string emailTo, string emailSubject, string emailBody)
        {
            using (var client = new SmtpClient
            {
                UseDefaultCredentials = true
            })
            {
                using (var message = new MailMessage())
                {
                    message.To.Add(emailTo);
                    message.IsBodyHtml = true;
                    message.BodyEncoding = Encoding.UTF8;
                    message.Subject = emailSubject;
                    message.Body = emailBody;

                    try
                    {
                        // send the email
                        client.Send(message);
                    }
                    catch (SmtpException exception)
                    {
                        exception.ToExceptionless().Submit();
                    }
                }
            }
        }

        /// <summary>
        /// Email the password reset URL to the requestor
        /// </summary>
        /// <param name="model">PasswordResetModel - EmailAddress, UserId and UniqueResetId</param>
        /// <param name="url">root url for the site eg http://localhost:53201/ </param>
        public void PasswordResetEmail(PasswordResetModel model, string url)
        {
            const string subject = "ESCC website – web author password";

            var body = new StringBuilder();

            body.AppendLine("<p>Hello,</p>");
            body.AppendLine("<p>This link takes you to the screen where you can set or change your password for the ESCC website content management system. Please note, this link will expire in 24 hours.</p>");
            body.AppendFormatLine("<p>{0}/Admin/PasswordResetVerification?userId={1}&uniqueResetId={2}</p>", url, model.UserId.ToString(), model.UniqueResetId);
            body.AppendLine("<p>If you need any help using the system, please refer to the user guides on the intranet.</p>");
            body.AppendLine("<p>Kind regards,<br/>Digital Services</p>");
            body.AppendFormatLine("<p>Guidance for web authors: <a href=\"{0}\">{0}</a>", ConfigurationManager.AppSettings["WebAuthorsGuidanceUrl"]);
            body.AppendFormatLine("<br/>Yammer: <a href=\"{0}\">{0}</a></p>", ConfigurationManager.AppSettings["WebAuthorsYammerUrl"]);

            //var link = String.Format("<p>Click this link, or paste into a browser to reset your password:</p><p>{0}/Admin/PasswordResetVerification?userId={1}&uniqueResetId={2}</p>", url, model.UserId, model.UniqueResetId);

            SmtpSendEmail(model.EmailAddress, subject, body.ToString());
        }

        /// <summary>
        /// Send confirmation email to let the user know the password reset was successful
        /// </summary>
        /// <param name="model">PasswordResetModel - EmailAddress</param>
        public void PasswordResetConfirmationEmail(PasswordResetModel model)
        {
            const string subject = "ESCC website – web author password successfully reset";

            var body = new StringBuilder();

            body.AppendLine("<p>Hello,</p>");
            body.AppendLine("<p>Your web author password has been successfully changed. You are now ready to start updating the ESCC website. Below is the website back office link where you need to login to edit your content.</p>");
            body.AppendFormatLine("<p><a href=\"{0}\">{0}</a>", ConfigurationManager.AppSettings["UmbracoBackOfficeUrl"]);
            body.AppendLine("<p>If you need any help using the system, please refer to the user guides on the intranet.</p>");
            body.AppendLine("<p>Kind regards,<br/>Digital Services</p>");
            body.AppendFormatLine("<p>Guidance for web authors: <a href=\"{0}\">{0}</a>", ConfigurationManager.AppSettings["WebAuthorsGuidanceUrl"]);
            body.AppendFormatLine("<br/>Yammer: <a href=\"{0}\">{0}</a></p>", ConfigurationManager.AppSettings["WebAuthorsYammerUrl"]);

            SmtpSendEmail(model.EmailAddress, subject, body.ToString());
        }

        /// <summary>
        /// Email the admin team to inform them of the new account so they can setup permissions
        /// </summary>
        /// <param name="model">UmbracoUserModel - FullName, UserName and EmailAddress</param>
        public void CreateNewUserEmail(UmbracoUserModel model)
        {
            var subject = string.Format("ESCC website - new web author account created for {0}", model.FullName);

            var body = new StringBuilder();

            body.AppendLine("<p>Hello Web Staff,</p>");
            body.AppendFormatLine("<p>A new web author account has been created for {0}:</P>", model.FullName);
            body.AppendFormatLine("<p>Username: {0}<br/>Email address: {1}</p>", model.UserName, model.EmailAddress);
            body.AppendLine("<p>You can now go to the User Access Manager to set up the pages that this web author will be responsible for.</p>");

            var emailTo = ConfigurationManager.AppSettings["EmailTo"];

            SmtpSendEmail(emailTo, subject, body.ToString());
        }

        public void PageExpiryWarningEmail(string emailTo, ExpiringPageModel contentNode, UmbracoUserModel pageUser)
        {
            var subject = string.Format("Page expiry - {0}", contentNode.PageName);
            var body = new StringBuilder();

            var expiryDate = contentNode.ExpiryDate;

            body.AppendFormatLine("<p>Hello {0},</p>", pageUser.FullName);
            body.AppendFormatLine("<p>Page {0} is due to expire on {1} at {2}</p>", contentNode.PageName, expiryDate.ToShortDateString(), expiryDate.ToShortTimeString());

            SmtpSendEmail(emailTo, subject, body.ToString());
        }
    }
}