using cyberblast.Common;
using cyberblast.SharePoint.Client.Authentication;
using Microsoft.SharePoint.Client;
using System;
using System.Net;

namespace cyberblast.SharePoint.Client {
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
        public State State {
            get => _State; 
        }
        public NetworkCredential Credentials { get; set; } = null;
        public bool ThrowExceptions { get; set; } = false;
        public bool FormsBasedAuthAccepted { get; set; } = true;
        public CookieCollection Cookies { get; set; }

        public event ExceptionHandler OnException = (sender, args) => { };

        /// <summary>
        /// Erzeugt einen SharePoint Client mit definierten Credentials zur Authentifizierung
        /// </summary>
        /// <param name="url">Url der aufzurufenden SharePoint Site</param>
        /// <param name="domain">Domain Name für Authentifizierung</param>
        /// <param name="loginName">Login Name für Authentifizierung</param>
        /// <param name="password">Passwort für Authentifizierung</param>
        public SPClient(string url, string domain, string loginName, string password) : this(url) {
            Credentials = new NetworkCredential(loginName, password, domain);
        }

        /// <summary>
        /// Erzeugt einen SharePoint Client unter Verwendung des "current users"
        /// </summary>
        /// <param name="url">Url der aufzurufenden SharePoint Site</param>
        public SPClient(string url) {
            _url = url;
        }

        /// <summary>
        /// Versucht die SharePoint Url als FormBasedAuthentication LoginSeite aufzurufen, sich dort zu authentifizieren und die Authentifizierungscookies zu speichern.
        /// </summary>
        internal void AuthenticateFormBased() {
            var authenticator = new T() {
                Url = _url,
                Credentials = Credentials ?? CredentialCache.DefaultNetworkCredentials
            };
            authenticator.OnException += (sender, args) => OnException(sender, args);
            Cookies = authenticator.Authenticate();
        }

        /// <summary>
        /// Versucht zunächst einen direkten Call aufzubauen. Sofern dies mislingt wird versucht per Form Based Authentication zu authentifizieren. 
        /// </summary>
        /// <returns>Endergebnis der Authentifizierung</returns>
        public bool Authenticate() {
            _State = State.Connecting;
            // dont throw here, exception is expected
            bool throwPreference = ThrowExceptions;
            ThrowExceptions = false;

            bool connected = IsConnected();
            if (!connected && typeof(T) != typeof(DefaultAuthenticator)) {
                _State = State.Connecting;
                // try again
                // get cookies
                AuthenticateFormBased();
                connected = IsConnected();
            }

            ThrowExceptions = throwPreference;
            if( _State == State.Connecting) {
                if (connected)
                    _State = State.Connected;
                else
                    _State = State.Error;
            }
            return connected;
        }
        private bool IsConnected() {
            bool result = false;
            Execute(ctx => {
                Web web = ctx.Web;
                ctx.Load(web);
                ctx.ExecuteQuery();
                result = !string.IsNullOrEmpty(web.Title);
            });
            return result;
        }        
        public void Execute(Call action) {
            try {
                using (ClientContext ctx = new ClientContext(_url)) {
                    ctx.ExecutingWebRequest += new EventHandler<WebRequestEventArgs>(ExecutingWebRequest);
                    ctx.AuthenticationMode = ClientAuthenticationMode.Default;
                    if (Credentials != null) ctx.Credentials = Credentials;
                    action(ctx);
                }
            }
            catch (WebException e) {
                if (e.Status == WebExceptionStatus.ProtocolError)
                    _State = State.AuthenticationFailed;
                else if (e.Status == WebExceptionStatus.NameResolutionFailure || e.Status == WebExceptionStatus.ConnectFailure || e.Status == WebExceptionStatus.ConnectionClosed)
                    _State = State.ConnectionFailed;
                OnException(this, new ExceptionArgs {
                    Exception = e
                });
                if (ThrowExceptions) throw;
            }
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
