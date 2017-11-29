using Microsoft.SharePoint.Client;
using System.IO;

namespace cyberblast.SharePoint.Client {
    public enum State { Offline, Connecting, Connected, Error, ConnectionFailed, AuthenticationFailed }
    public delegate void Call(ClientContext ctx);
    public delegate void StreamCall(Stream stream);
    public delegate void ItemMethod(ListItem item);
    public delegate S Transformer<T, S>(T initialValue);
}
