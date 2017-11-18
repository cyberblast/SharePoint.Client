using cyberblast.Common;
using System.Net;

namespace cyberblast.SharePoint.Client.Authentication {
    public class DefaultAuthenticator : IAuthenticator {
        public event ExceptionHandler OnException = (sender, args) => { };

        public NetworkCredential Credentials { get; set; }
        public string Url { get; set; }
        public bool ThrowExceptions { get; set; }

        public CookieCollection Authenticate() {
            return new CookieCollection();
        }
    }
}