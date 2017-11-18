using cyberblast.Common;
using System.Net;

namespace cyberblast.SharePoint.Client.Authentication {
    public class DefaultAuthenticator : IAuthenticator
    {
        public event ExceptionHandler OnException = (sender, args) => { };
        public bool _ThrowExceptions = false;

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
        public bool ThrowExceptions
        {
            get { return _ThrowExceptions; }
            set { _ThrowExceptions = value; }
        }        

        public CookieCollection Authenticate()
        {
            return new CookieCollection();
        }
    }
}