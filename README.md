# cyberblast SharePoint Client

<a href="https://github.com/cyberblast" border="0"><img align="right" title="logo" src="https://avatars2.githubusercontent.com/u/33760031?s=64"></a>

[![Build Status](https://travis-ci.org/cyberblast/SharePoint.Client.svg?branch=master)](https://travis-ci.org/cyberblast/SharePoint.Client)

[Nuget Package](https://www.nuget.org/packages/cyberblast.SharePoint.Client)

## Sample Usage

```C#
using System;
using cyberblast.SharePoint.Client;
namespace ConsoleApp1 {
    class Program {
        static void Main(string[] args) {

            ISPClient client = new SPClient("http://yourSharePointUrl");
            client.Execute(ctx => {
                var web = ctx.Web;
                ctx.Load(web, w => w.Title);
                ctx.ExecuteQuery();
                Console.Write(web.Title);
            });
        }
    }
}
```

## Features

* Set Name/Password 
```C#
ISPClient client = new SPClient(
    "http://yourSharePointUrl", 
    "domain", 
    "loginName", 
    "password");
```

* Use different authentication procedures
```C#
using cyberblast.SharePoint.Client.Authentication;
[...]
ISPClient client = new SPClient<TMGAuthenticator>(
    "http://yourSharePointUrl", 
    "domain", 
    "loginName", 
    "password");
client.Authenticate();
```

* Create CAML queries & iterate list items
```C#
using cyberblast.SharePoint.Client;
using cyberblast.SharePoint.Client.Authentication;
using Microsoft.SharePoint.Client;
namespace ConsoleApp1 {
    class Program {
        const int ROW_LIMIT = 100;
        static void Main(string[] args) {
            ISPClient client = new SPClient<TMGAuthenticator>(
                "http://yourSharePointUrl", 
                "domain", 
                "loginName", 
                "password");
            client.Authenticate();

            var filter = QueryBuilder.Equals(
                new QueryBuilder.Field("Id"),
                new QueryBuilder.Value(7, FieldType.Number));
            var query = QueryBuilder.Query(filter, ROW_LIMIT);

            void Callback(ListItem item) {
                Console.WriteLine(item.Id);
            }

            client.Execute(ctx => 
                ctx.IterateItems("Documents", query, Callback));
        }
    }
}
```

* Retrieve Field Values
```C#
void Callback(ListItem item) {
    int number = item.GetValue<int>("numberField");
    string author = item.GetValue<FieldUserValue, string>(
        "Author", 
        (fieldUserValue) => fieldUserValue.LookupValue);
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
  Contained in separate namespace `cyberblast.Claims.WinForm` 
