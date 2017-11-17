using System;
using System.IO;
using System.Net;
using System.Text;

namespace Authentication {
    public abstract class CookieAuthenticator : IAuthenticator
    {
        public event ExceptionHandler OnException = (e) => { };
        public bool _ThrowExceptions = false;

        #region abstract Properties

        protected abstract string UserAgent
        {
            get;
        }

        protected abstract string ContentType
        {
            get;
        }

        protected abstract string ConnectionPath
        {
            get;
        }

        protected abstract string ValidationPath
        {
            get;
        }

        protected abstract string UserNameParam
        {
            get;
        }

        protected abstract string PasswordParam
        {
            get;
        }

        #endregion

        #region Properties

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

        CookieCollection _Cookies;
        internal CookieCollection Cookies
        {
            get { return _Cookies; }
        }

        protected string ConnectUrl
        {
            get
            {
                if (Uri.TryCreate(new Uri(Url), ConnectionPath, out Uri connectUri))
                    return connectUri.AbsoluteUri;
                else return Url + ConnectionPath;
            }
        }

        protected string ValidationUrl
        {
            get
            {
                if (Uri.TryCreate(new Uri(Url), ValidationPath, out Uri validationUri))
                    return validationUri.AbsoluteUri;
                else return Url + ValidationPath;
            }
        }

        protected string[] Queries
        {
            get;
            set;
        }

        #endregion

        public CookieAuthenticator(){}

        /*
        internal CookieAuthenticator(string url, NetworkCredential credentials)
        {
            this.Url = url;
            this.Credentials = credentials;
        }
         */

        public CookieCollection Authenticate()
        {
            try
            {
                this.Connect();
                this.ValidateCredentials();
            }
            catch (Exception ex)
            {
                OnException(ex);
                if (ThrowExceptions) throw;
            }
            return _Cookies;
        }

        private void Connect()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ConnectUrl);
            request.CookieContainer = new CookieContainer();
            request.UserAgent = this.UserAgent;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                _Cookies = response.Cookies;
                string query = response.ResponseUri.Query;
                Queries = query.Substring(query.LastIndexOf('?') + 1).Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        private void ValidateCredentials()
        {
            string postData = string.Empty;
            postData += string.Format("{0}={2}&{1}={3}&{4}",
                UserNameParam,
                PasswordParam,
                Credentials.UserName,
                Credentials.Password,
                string.Join("&", Queries));

            HttpWebRequest postRequest = (HttpWebRequest)WebRequest.Create(ValidationUrl);
            postRequest.Method = "POST";
            postRequest.ContentType = ContentType;
            postRequest.AllowAutoRedirect = true;

            postRequest.CookieContainer = new CookieContainer();
            foreach (Cookie cookie in this.Cookies)
            {
                postRequest.CookieContainer.Add(cookie);
            }

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(postData);
            postRequest.ContentLength = data.Length;
            using (Stream newStream = postRequest.GetRequestStream())
            {
                newStream.Write(data, 0, data.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)postRequest.GetResponse())
            {
                _Cookies = response.Cookies;
            }
        }
    }
}