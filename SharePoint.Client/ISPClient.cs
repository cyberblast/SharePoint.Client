using Microsoft.SharePoint.Client;
using System;
using System.Linq.Expressions;
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
        void IterateItems(ClientContext ctx, string listName, CamlQuery query, ItemMethod iterator, params Expression<Func<ListItemCollection, object>>[] retrievals);
        void IterateItems(string listName, CamlQuery query, ItemMethod iterator, params Expression<Func<ListItemCollection, object>>[] retrievals);
        void IterateItems(ClientContext ctx, string listName, QueryBuilder.Query query, ItemMethod iterator, params Expression<Func<ListItemCollection, object>>[] retrievals);
        void IterateItems(string listName, QueryBuilder.Query query, ItemMethod iterator, params Expression<Func<ListItemCollection, object>>[] retrievals);
    }
}
