using Microsoft.SharePoint.Client;
using System;
using System.Linq.Expressions;
using System.Net;

namespace SharePoint.Client {
    public interface ISPClient
    {
        event ExceptionHandler OnException;
        NetworkCredential Credentials { get; set; }
        bool ThrowExceptions { get; set; }
        bool FormsBasedAuthAccepted { get; set; }
        CookieCollection Cookies { get; set; }
        bool Authenticate();
        void Execute(Call action);
        void IterateItems(ClientContext ctx, string listName, CamlQuery query, ItemMethod iterator, params Expression<Func<ListItemCollection, object>>[] retrievals);
    }
}
