using System;
using System.Net;

namespace Authentication {
    public delegate void ExceptionHandler(Exception e);
    public interface IAuthenticator 
    {
        event ExceptionHandler OnException;
        bool ThrowExceptions { get; set; }
        NetworkCredential Credentials { get; set; }
        string Url { get; set; }
        CookieCollection Authenticate();
    }
}
