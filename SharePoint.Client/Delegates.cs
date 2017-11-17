using System;
using Microsoft.SharePoint.Client;

namespace cyberblast.SharePoint.Client {
    public delegate void ExceptionHandler(Exception e);
    public delegate void Call(ClientContext ctx);
    public delegate void ItemMethod(ListItem item);
    public delegate S Transformer<T, S>(T initialValue);
}