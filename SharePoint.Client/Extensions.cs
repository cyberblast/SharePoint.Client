using Microsoft.SharePoint.Client;

namespace SharePoint.Client {
    public static class Extensions
    {
        public static T GetValue<T>(this ListItem item, string name)
        {
            if (!item.FieldValues.ContainsKey(name)) return default(T);
            var rawValue = item[name];
            if (rawValue == null) return default(T);
            if (!(rawValue is T)) return default(T);
                return (T)rawValue;
        }
        public static S GetValue<T,S>(this ListItem item, string name, Transformer<T,S> converter)
        {
            T itemValue = item.GetValue<T>(name);
            if (itemValue == null) return default(S);
            return converter(itemValue);
        }
    }
}
