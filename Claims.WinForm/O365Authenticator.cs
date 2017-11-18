using cyberblast.Common;
using cyberblast.SharePoint.Client.Authentication;
using System;
using System.Net;

namespace cyberblast.Claims.WinForm {
    public class O365Authenticator : IAuthenticator {
        public event ExceptionHandler OnException = (sender, args) => { };

        public bool ThrowExceptions { get; set; }

        public NetworkCredential Credentials { get; set; }

        public string Url { get; set; }
        
        internal CookieCollection Cookies { get; set; }

        public O365Authenticator(){}
        
        public CookieCollection Authenticate() {
            try {
                Cookies = ClaimClientContext.GetAuthenticatedCookies(Url, 0, 0);
            }
            catch(Exception ex) {
                OnException(this, new ExceptionArgs {
                    Exception = ex
                });
                if (ThrowExceptions) throw;
            }
            return Cookies;
        }        
    }
}