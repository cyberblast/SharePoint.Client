using System.Net;

namespace Authentication {
    public class DefaultAuthenticator : IAuthenticator
    {
        public event ExceptionHandler OnException = (e) => { };
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