using cyberblast.Common;
using Microsoft.SharePoint.Client;
using System.Net;

namespace cyberblast.SharePoint.Client {
    public interface ISPClient {
        event ExceptionHandler OnException;
        NetworkCredential Credentials { get; set; }
        bool ThrowExceptions { get; set; }
        bool FormsBasedAuthAccepted { get; set; }
        CookieCollection Cookies { get; set; }
        bool Authenticate();
        void Execute(Call action);
        void GetFileStream(string serverRelativeUrl, StreamCall handler, ClientContext ctx);
        void GetFileStream(string serverRelativeUrl, StreamCall handler);
    }
}
