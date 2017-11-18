using System;
using System.Text;
using System.Net;
using System.IO;

namespace cyberblast.SharePoint.Client.Authentication {
    public class TMGAuthenticator : CookieAuthenticator
    {
        string _UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13";
        string _ContentType = "application/x-www-form-urlencoded";
        string _ConnectionPath = "CookieAuth.dll?GetLogon?curl=Z2F&reason=0&formdir=3";
        string _ValidationPath = "CookieAuth.dll?Logon";
        string _UserNameParam = "username";
        string _PasswordParam = "password";

        protected override string UserAgent
        {
            get { return _UserAgent; } 
        }
        protected override string ContentType
        {
            get { return _ContentType; }
        }
        protected override string ConnectionPath
        {
            get { return _ConnectionPath; }
        }
        protected override string ValidationPath
        {
            get { return _ValidationPath; }
        }
        protected override string UserNameParam
        {
            get { return _UserNameParam; }
        }
        protected override string PasswordParam
        {
            get { return _PasswordParam; }
        }
    }
}