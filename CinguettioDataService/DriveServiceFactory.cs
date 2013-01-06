using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Drive.v2;
using Google.Apis.Util;
using System.Web;

namespace CinguettioDataService
{
    public class DriveServiceFactory
    {
        private const string SERVICE_ACCOUNT_EMAIL = "599107819184@developer.gserviceaccount.com";
        private const string SERVICE_ACCOUNT_PKCS12_FILE_PATH = @"\App_Data\4cdbb3038252b4cc8a2e2799e402d1f99b0d15ad-privatekey.p12";

        /// <summary>
        /// Build a Drive service object authorized with the service account.
        /// </summary>
        /// <returns>Drive service object.</returns>
        public static DriveService BuildService()
        {
            string p = HttpContext.Current.Server.MapPath(SERVICE_ACCOUNT_PKCS12_FILE_PATH);
            bool b =System.IO.File.Exists(p);
            X509Certificate2 certificate = new X509Certificate2(p, "notasecret",
                X509KeyStorageFlags.Exportable);

            var provider = new AssertionFlowClient(GoogleAuthenticationServer.Description, certificate)
            {
                ServiceAccountId = SERVICE_ACCOUNT_EMAIL,
                Scope = DriveService.Scopes.Drive.GetStringValue(),
            };
            var auth = new OAuth2Authenticator<AssertionFlowClient>(provider, AssertionFlowClient.GetState);

            return new DriveService(auth);
        }
    }
}