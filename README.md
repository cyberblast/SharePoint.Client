# cyberblast SharePoint Client

<a href="https://github.com/cyberblast" border="0"><img align="right" title="Open Collaboration Society" src="https://raw.githubusercontent.com/cyberblast/SharePoint.Client/master/cyberblast64.png"></a>

[![Build Status](https://travis-ci.org/cyberblast/SharePoint.Client.svg?branch=master)](https://travis-ci.org/cyberblast/SharePoint.Client)

[Nuget Package](https://www.nuget.org/packages/cyberblast.SharePoint.Client)

## Sample Usage: 
```C#
using System;
using cyberblast.SharePoint.Client;
using cyberblast.Authentication;
namespace ConsoleApp1 {
    class Program {
        static void Main(string[] args) {

            ISPClient client = new SPClient("http://yourSharePointUrl");
            client.Execute(ctx => {
                var web = ctx.Web;
                ctx.Load(web, w => w.Title);
                ctx.ExecuteQuery();
                Console.Write(web.Title);
                Console.ReadLine();
            });
        }
    }
}
```

## Options

* Enter Name/Password 
```C#
ISPClient client = new SPClient("http://yourSharePointUrl", "domain", "loginName", "password");
client.Authenticate();
```

* Use different authentication procedures
```C#
ISPClient client = new SPClient<TMGAuthenticator>("http://yourSharePointUrl", "domain", "loginName", "password");
client.Authenticate();
```

* Create CAML queries & iterate list items
```C#
using System;
using cyberblast.SharePoint.Client;
using Microsoft.SharePoint.Client;
namespace ConsoleApp1 {
    class Program {
        static void Main(string[] args) {
            SPClient client = new SPClient("http://yourSharePointUrl");

            var filter = QueryBuilder.Eq(
                QueryBuilder.Static(7, FieldType.Number), 
                QueryBuilder.FieldRef("Id"));
            var rowLimit = 100;

            CamlQuery query = new CamlQuery() {
                ViewXml = QueryBuilder.Query(filter, rowLimit)
            };
            void Callback(ListItem item) {
                Console.WriteLine(item.Id);
            }
            client.IterateItems("Documents", query, Callback);
        }
    }
}
```

## Currently implemented Authenticators

* DefaultAuthentication

  Authenticate using NTLM/Kerberos
  This one is also used when no Authenticator is specified
  
* TMGAuthentication

  Form based authentication against a TMG Gateway
  
* O365Authenticator

  Opens a browser window requesting for O365 credentials.
  Currently only working when using Windows Forms.
