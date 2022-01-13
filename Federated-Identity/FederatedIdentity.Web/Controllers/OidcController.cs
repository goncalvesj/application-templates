using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace FederatedIdentity.Web.Controllers
{
    public class OidcController : Controller
    {
        private static Lazy<X509SigningCredentials> _signingCredentials;
        private readonly AppSettingsModel AppSettings;
        private readonly IWebHostEnvironment _hostingEnvironment;

        // Sample: Inject an instance of an AppSettingsModel class into the constructor of the consuming class, 
        // and let dependency injection handle the rest
        public OidcController(IOptions<AppSettingsModel> appSettings, IWebHostEnvironment hostingEnvironment)
        {
            AppSettings = appSettings.Value;
            _hostingEnvironment = hostingEnvironment;

            // Sample: Load the certificate with a private key (must be pfx file)
            _signingCredentials = new Lazy<X509SigningCredentials>(() =>
            {
                var certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                certStore.Open(OpenFlags.ReadOnly);
                var certCollection = certStore.Certificates.Find(
                    X509FindType.FindByThumbprint,
                    AppSettings.SigningCertThumbprint,
                    false);
                // Get the first cert with the thumb-print
                if (certCollection.Count > 0) return new X509SigningCredentials(certCollection[0]);

                throw new Exception("Certificate not found");
            });
        }

        [Route(".well-known/openid-configuration", Name = "OIDCMetadata")]
        public ActionResult Metadata()
        {
            return Content(JsonConvert.SerializeObject(new OidcModel
            {
                // Sample: The issuer name is the application root path
                Issuer = $"{Request.Scheme}://{Request.Host}{Request.PathBase.Value}/",

                // Sample: Include the absolute URL to JWKs endpoint
                JwksUri = Url.Link("JWKS", null),

                // Sample: Include the supported signing algorithms
                IdTokenSigningAlgValuesSupported = new[] {_signingCredentials.Value.Algorithm}
            }), "application/json");
        }

        [Route(".well-known/keys", Name = "JWKS")]
        public ActionResult JwksDocument()
        {
            return Content(JsonConvert.SerializeObject(new JwksModel
            {
                Keys = new[] {JwksKeyModel.FromSigningCredentials(_signingCredentials.Value)}
            }), "application/json");
        }
    }

    public class OidcModel
    {
        [JsonProperty("issuer")] public string Issuer { get; set; }

        [JsonProperty("jwks_uri")] public string JwksUri { get; set; }

        [JsonProperty("id_token_signing_alg_values_supported")]
        public ICollection<string> IdTokenSigningAlgValuesSupported { get; set; }
    }

    public class JwksModel
    {
        [JsonProperty("keys")] public ICollection<JwksKeyModel> Keys { get; set; }
    }

    public class JwksKeyModel
    {
        [JsonProperty("kid")] public string Kid { get; set; }

        [JsonProperty("nbf")] public long Nbf { get; set; }

        [JsonProperty("use")] public string Use { get; set; }

        [JsonProperty("kty")] public string Kty { get; set; }

        [JsonProperty("alg")] public string Alg { get; set; }

        [JsonProperty("x5c")] public ICollection<string> X5C { get; set; }

        [JsonProperty("x5t")] public string X5T { get; set; }

        [JsonProperty("n")] public string N { get; set; }

        [JsonProperty("e")] public string E { get; set; }

        public static JwksKeyModel FromSigningCredentials(X509SigningCredentials signingCredentials)
        {
            var certificate = signingCredentials.Certificate;

            // JWK cert data must be base64 (not base64url) encoded
            var certData = Convert.ToBase64String(certificate.Export(X509ContentType.Cert));

            // JWK thumbprints must be base64url encoded (no padding or special chars)
            var thumbprint = Base64UrlEncoder.Encode(certificate.GetCertHash());

            // JWK must have the modulus and exponent explicitly defined
            var rsa = certificate.GetRSAPublicKey() as RSACng;

            if (rsa == null) throw new Exception("Certificate is not an RSA certificate.");

            var keyParams = rsa.ExportParameters(false);
            var keyModulus = Base64UrlEncoder.Encode(keyParams.Modulus);
            var keyExponent = Base64UrlEncoder.Encode(keyParams.Exponent);

            return new JwksKeyModel
            {
                Kid = signingCredentials.Kid,
                Kty = "RSA",
                Nbf = new DateTimeOffset(certificate.NotBefore).ToUnixTimeSeconds(),
                Use = "sig",
                Alg = signingCredentials.Algorithm,
                X5C = new[] {certData},
                X5T = thumbprint,
                N = keyModulus,
                E = keyExponent
            };
        }
    }
}