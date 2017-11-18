using cyberblast.Common;
using cyberblast.SharePoint.Client.Authentication;
using System;
using System.Net;

namespace cyberblast.Claims.WinForm {
    public class O365Authenticator : IAuthenticator
    {
        public event ExceptionHandler OnException = (sender, args) => { };

        public bool _ThrowExceptions = false;
        public bool ThrowExceptions
        {
            get { return _ThrowExceptions; }
            set { _ThrowExceptions = value; }
        }

        public NetworkCredential Credentials
        {
            get;
            set;
        }

        public string Url
        {
            get;
            set;
        }
        
        CookieCollection _Cookies;
        internal CookieCollection Cookies
        {
            get { return _Cookies; }
        }
        
        public O365Authenticator(){}
        
        public CookieCollection Authenticate()
        {
            try
            {
                //_Cookies = ClaimsWebAuth.ExtractAuthCookiesFromUrl(Url);
                //if(_Cookies == null)
                _Cookies = ClaimClientContext.GetAuthenticatedCookies(Url, 0, 0);
            }
            catch(Exception ex)
            {
                OnException(this, new ExceptionArgs {
                    Exception = ex
                });
                if (ThrowExceptions) throw;
            }
            return _Cookies;
        }        
    }
}