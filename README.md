# cyberblast.SharePoint.Client

[![Build Status](https://travis-ci.org/cyberblast/SharePoint.Client.svg?branch=master)](https://travis-ci.org/cyberblast/SharePoint.Client)

[NuGet package](https://www.nuget.org/packages/cyberblast.SharePoint.Client)  
[Sample usage](#sample-usage)  
[Features](#features)  
[Authenticators](#currently-implemented-authenticators)  
[Contact](#contact)  

Please use this one using NuGet. Simply search for "cyberblast".  
A successful release will atomatically update the nuget package. 

## Sample usage

```C#
using System;
using cyberblast.SharePoint.Client;
namespace ConsoleApp1 {
    class Program {
        static void Main(string[] args) {

            ISPClient client = new SPClient("http://yourSharePointUrl");
            client.Execute(ctx => {
                // now common CSOM
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

* Set name/password 
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
// using TMGAuthenticator (see below for a list of implemented Authenticators)
ISPClient client = new SPClient<TMGAuthenticator>(
    "http://yourSharePointUrl", 
    "domain", 
    "loginName", 
    "password");
client.Authenticate();
```

* CAML query builder & iterate list items
```C#
using cyberblast.SharePoint.Client;
using cyberblast.SharePoint.Client.Authentication;
using Microsoft.SharePoint.Client;
namespace ConsoleApp1 {
    class Program {
        static void Main(string[] args) {

            ISPClient client = new SPClient("http://yourSharePointUrl");

            var filter = QueryBuilder.Equals(
                new QueryBuilder.Field("Id"),
                new QueryBuilder.Value(7, FieldType.Number));
            var query = QueryBuilder.Query(filter);
			
            // C#7 Syntax. But any fitting delegate will do...
            void Callback(ListItem item) { 
                Console.WriteLine(item.Id);
            }

            client.Execute(ctx => 
                // ClientContext Extension
                ctx.IterateItems("Documents", query, Callback));
        }
    }
}
```

* Retrieve and convert field values
```C#
void Callback(ListItem item) {
    // ListItem Extension
    int number = item.GetValue<int>("numberField");
    string author = item.GetValue<FieldUserValue, string>(
        "Author", 
        (fieldUserValue) => fieldUserValue.LookupValue);
}
```

## Currently implemented Authenticators

* DefaultAuthentication

  Authenticate using Integrated Windows authentication (NTLM/Kerberos)  
  This one is also used when no Authenticator is specified
  
* TMGAuthentication

  Form based authentication against a TMG Gateway  
  Inherits CookieAuthenticator
  
* Claims.WinForm.O365Authenticator

  Opens a browser window requesting for O365 credentials.  
  Currently only working when using Windows Forms.  
  Contained in separate namespace `cyberblast.Claims.WinForm` 

* (CookieAuthenticator)
  
  Abstract for implementing form based Authenticators

I know, there is a lot missing regardig authentication. I will add things as I need them. 
If YOU have some additions at hand, I'd be greatful for your additions (merge requests).

## Contact

[Create an issue](https://github.com/cyberblast/SharePoint.Client/issues)  
[Send a mail](mailto://git@cyberblast.org)
