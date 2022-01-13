using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using FederatedIdentity.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FederatedIdentity.Web.Controllers
{
    public class HomeController : Controller
    {
        private static Lazy<X509SigningCredentials> _signingCredentials;
        private readonly AppSettingsModel _appSettings;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IOptions<AppSettingsModel> appSettings,
            IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;

            _appSettings = appSettings.Value;
            _hostingEnvironment = hostingEnvironment;

            // Sample: Load the certificate with a private key (must be pfx file)
            _signingCredentials = new Lazy<X509SigningCredentials>(() =>
            {

	            var certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                certStore.Open(OpenFlags.ReadOnly);
                var certCollection = certStore.Certificates.Find(
                    X509FindType.FindByThumbprint,
                    _appSettings.SigningCertThumbprint,
                    false);
                // Get the first cert with the thumb-print
                if (certCollection.Count > 0) return new X509SigningCredentials(certCollection[0]);

                throw new Exception("Certificate not found");
            });
        }

        public IActionResult Index(string email, string phone)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewData["Message"] = "";
                return View();
            }

            var token = BuildIdToken(phone, email);
            var link = BuildUrl(token);

            //var Body = string.Empty;

            //var htmlTemplate =
            //    System.IO.File.ReadAllText(Path.Combine(_hostingEnvironment.ContentRootPath,
            //        "App_Data\\Template.html"));

            //try
            //{
            //    var mailMessage = new MailMessage();
            //    mailMessage.To.Add(email);
            //    mailMessage.From = new MailAddress(_appSettings.SMTPFromAddress);
            //    mailMessage.Subject = _appSettings.SMTPSubject;
            //    mailMessage.Body = string.Format(htmlTemplate, email, link);
            //    mailMessage.IsBodyHtml = true;

            //    var smtpClient = new SmtpClient(_appSettings.SMTPServer, _appSettings.SMTPPort)
            //    {
            //        Credentials = new NetworkCredential(_appSettings.SMTPUsername, _appSettings.SMTPPassword),
            //        EnableSsl = _appSettings.SMTPUseSSL,
            //        DeliveryMethod = SmtpDeliveryMethod.Network
            //    };

            //    smtpClient.Send(mailMessage);

            //    ViewData["Message"] = $"Email sent to {email}";
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            ViewData["Link"] = link;
            ViewData["Message"] = $"Email sent to {email}, {phone}";
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        private string BuildIdToken(string phone, string email)
        {
            var issuer = $"{Request.Scheme}://{Request.Host}{Request.PathBase.Value}/";

            // All parameters send to Azure AD B2C needs to be sent as claims
            IList<Claim> claims = new List<Claim>();
            claims.Add(new Claim("email", email, ClaimValueTypes.String, issuer));
            claims.Add(new Claim("strongAuthenticationPhoneNumber", phone, ClaimValueTypes.String, issuer));

            // Create the token
            var token = new JwtSecurityToken(
                issuer,
                _appSettings.B2CClientId,
                claims,
                DateTime.Now,
                DateTime.Now.AddDays(7),
                _signingCredentials.Value);

            // Get the representation of the signed token
            var jwtHandler = new JwtSecurityTokenHandler();

            return jwtHandler.WriteToken(token);
        }

        private string BuildUrl(string token)
        {
            var nonce = Guid.NewGuid().ToString("n");

            return string.Format(_appSettings.B2CSignUpUrl,
                _appSettings.B2CTenant,
                _appSettings.B2CPolicy,
                _appSettings.B2CClientId,
                Uri.EscapeDataString(_appSettings.B2CRedirectUri),
                nonce) + "&id_token_hint=" + token;
        }
    }

    public class AppSettingsModel
    {
        public string B2CTenant { get; set; }
        public string B2CPolicy { get; set; }
        public string B2CClientId { get; set; }
        public string B2CRedirectUri { get; set; }
        public string B2CSignUpUrl { get; set; }
        public string SigningCertAlgorithm { get; set; }
        public string SigningCertThumbprint { get; set; }
        public string SMTPServer { get; set; }
        public int SMTPPort { get; set; }
        public string SMTPUsername { get; set; }
        public string SMTPPassword { get; set; }
        public bool SMTPUseSSL { get; set; }
        public string SMTPFromAddress { get; set; }
        public string SMTPSubject { get; set; }
        public int LinkExpiresAfterMinutes { get; set; }
    }
}
