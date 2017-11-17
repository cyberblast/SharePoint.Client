using cyberblast.Authentication;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;

namespace cyberblast.SharePoint.Client {
    public enum State { Offline, Connecting, Connected, Error, ConnectionFailed, AuthenticationFailed }

    public class SPClient : SPClient<DefaultAuthenticator> {
        public SPClient(string url, string domain, string loginName, string password) 
            : base(url, domain, loginName, password) { }
        
        public SPClient(string url) 
            : base(url) { }
    }

    /// <summary>
    /// Kapselt Authentifizierung und Aufruf von SharePoint Calls
    /// </summary>
    public class SPClient<T> : ISPClient
        where T : IAuthenticator, new() {

        string _url;

        State _State = State.Offline;
        public State State
        {
            get { return _State; }
        }
        public NetworkCredential Credentials { get; set; } = null;
        public bool ThrowExceptions { get; set; } = false;
        public bool FormsBasedAuthAccepted { get; set; } = true;
        public CookieCollection Cookies { get; set; }

        public event ExceptionHandler OnException = (e) => { };
        
        /// <summary>
        /// Erzeugt einen SharePoint Client mit definierten Credentials zur Authentifizierung
        /// </summary>
        /// <param name="url">Url der aufzurufenden SharePoint Site</param>
        /// <param name="domain">Domain Name für Authentifizierung</param>
        /// <param name="loginName">Login Name für Authentifizierung</param>
        /// <param name="password">Passwort für Authentifizierung</param>
        public SPClient(string url, string domain, string loginName, string password) : this(url)
        {
            Credentials = new NetworkCredential(loginName, password, domain);
        }

        /// <summary>
        /// Erzeugt einen SharePoint Client unter Verwendung des "current users"
        /// </summary>
        /// <param name="url">Url der aufzurufenden SharePoint Site</param>
        public SPClient(string url)
        {
            _url = url;
        }

        /// <summary>
        /// Versucht die SharePoint Url als FormBasedAuthentication LoginSeite aufzurufen, sich dort zu authentifizieren und die Authentifizierungscookies zu speichern.
        /// </summary>
        internal void AuthenticateFormBased()
        {
            var authenticator = new T() {
                Url = _url,
                Credentials = Credentials ?? CredentialCache.DefaultNetworkCredentials
            };
            authenticator.OnException += (e) => OnException(e);
            Cookies = authenticator.Authenticate();
        }

        /// <summary>
        /// Versucht zunächst einen direkten Call aufzubauen. Sofern dies mislingt wird versucht per Form Based Authentication zu authentifizieren. 
        /// </summary>
        /// <returns>Endergebnis der Authentifizierung</returns>
        public bool Authenticate()
        {
            _State = State.Connecting;
            bool result;
            // dont throw here, exception is expected
            bool throwPreference = ThrowExceptions;
            ThrowExceptions = false;
                        
            result = IsConnected();
            if (!result && typeof(T) != typeof(DefaultAuthenticator)) {
                _State = State.Connecting;
                // try again
                // get cookies
                AuthenticateFormBased();
                result = IsConnected();
            }

            ThrowExceptions = throwPreference;
            if( _State == State.Connecting) {
                if (result == true)
                    _State = State.Connected;
                else
                    _State = State.Error;
            }
            return result;
        }
        private bool IsConnected()
        {
            bool result = false;
            Execute(ctx =>
            {
                Web web = ctx.Web;
                ctx.Load(web);
                ctx.ExecuteQuery();
                result = !string.IsNullOrEmpty(web.Title);
            });
            return result;
        }        
        public void Execute(Call action)
        {
            try
            {
                using (ClientContext ctx = new ClientContext(_url))
                {
                    ctx.ExecutingWebRequest += new EventHandler<WebRequestEventArgs>(ExecutingWebRequest);
                    ctx.AuthenticationMode = ClientAuthenticationMode.Default;
                    if (Credentials != null) ctx.Credentials = Credentials;

                    action(ctx);
                }
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                    _State = State.AuthenticationFailed;
                else if (e.Status == WebExceptionStatus.NameResolutionFailure || e.Status == WebExceptionStatus.ConnectFailure || e.Status == WebExceptionStatus.ConnectionClosed)
                    _State = State.ConnectionFailed;
                OnException(e);
                if (ThrowExceptions) throw;
            }
        }
        public void IterateItems(ClientContext ctx, string listName, CamlQuery query, ItemMethod iterator, params Expression<Func<ListItemCollection, object>>[] retrievals) {
            List list = ctx.Web.Lists.GetByTitle(listName);
            query.ListItemCollectionPosition = null;
            ListItemCollection items;
            do {
                items = list.GetItems(query);

                ctx.Load(items, retrievals);
                ctx.ExecuteQuery();

                query.ListItemCollectionPosition = items.ListItemCollectionPosition;
                if (items != null && items.Count > 0) {
                    using (IEnumerator<ListItem> listItemEnumerator = items.GetEnumerator()) {
                        while (listItemEnumerator.MoveNext()) {
                            iterator(listItemEnumerator.Current);
                        }
                    }
                }
            }
            while (query.ListItemCollectionPosition != null);

            items = null;
            list = null;
        }
        public void IterateItems(string listName, CamlQuery query, ItemMethod iterator, params Expression<Func<ListItemCollection, object>>[] retrievals) {
            Execute(ctx => IterateItems(ctx, listName, query, iterator, retrievals));
        }
        public void IterateItems(ClientContext ctx, string listName, QueryBuilder.Query query, ItemMethod iterator, params Expression<Func<ListItemCollection, object>>[] retrievals) {
            List list = ctx.Web.Lists.GetByTitle(listName);
            CamlQuery caml = new CamlQuery(){
                ViewXml = query
            };
            caml.ListItemCollectionPosition = null;
            ListItemCollection items;
            do {
                items = list.GetItems(caml);

                ctx.Load(items, retrievals);
                ctx.ExecuteQuery();

                caml.ListItemCollectionPosition = items.ListItemCollectionPosition;
                if (items != null && items.Count > 0) {
                    using (IEnumerator<ListItem> listItemEnumerator = items.GetEnumerator()) {
                        while (listItemEnumerator.MoveNext()) {
                            iterator(listItemEnumerator.Current);
                        }
                    }
                }
            }
            while (caml.ListItemCollectionPosition != null);

            items = null;
            list = null;
        }
        public void IterateItems(string listName, QueryBuilder.Query query, ItemMethod iterator, params Expression<Func<ListItemCollection, object>>[] retrievals) {
            Execute(ctx => IterateItems(ctx, listName, query, iterator, retrievals));
        }
        private void ExecutingWebRequest(object sender, WebRequestEventArgs e) {
            if (!FormsBasedAuthAccepted)
                e.WebRequestExecutor.WebRequest.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
            if (Cookies != null) {
                e.WebRequestExecutor.WebRequest.CookieContainer = new CookieContainer();
                foreach (Cookie cookie in Cookies) {
                    e.WebRequestExecutor.WebRequest.CookieContainer.Add(cookie);
                }
            }
        }
    }
}
