
# cyberblast SharePoint Client. 

Sample usage:

ISPClient client = new SPClient("http://yourSharePointUrl");
client.Execute(context => {
    var web = context.Web;
    context.Load(web, w => w.Title);
    context.ExecuteQuery();
    Console.Write(web.Title);
});

For more information about features, examples, source code, contact ... - please visit: 
https://github.com/cyberblast/SharePoint.Client 
