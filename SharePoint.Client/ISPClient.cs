using cyberblast.Common;
using System.Net;

namespace cyberblast.SharePoint.Client {
    public interface ISPClient
    {
        event ExceptionHandler OnException;
        NetworkCredential Credentials { get; set; }
        bool ThrowExceptions { get; set; }
        bool FormsBasedAuthAccepted { get; set; }
        CookieCollection Cookies { get; set; }
        bool Authenticate();
        void Execute(Call action);
    }
}
