using cyberblast.Common;
using System.Net;

namespace cyberblast.SharePoint.Client.Authentication {
    public interface IAuthenticator 
    {
        event ExceptionHandler OnException;
        bool ThrowExceptions { get; set; }
        NetworkCredential Credentials { get; set; }
        string Url { get; set; }
        CookieCollection Authenticate();
    }
}
