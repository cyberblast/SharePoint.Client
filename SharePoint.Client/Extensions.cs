using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;

namespace cyberblast.SharePoint.Client {
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

        public static void IterateItems(this ClientContext ctx, string listName, CamlQuery query, ItemMethod iterator, params System.Linq.Expressions.Expression<Func<ListItemCollection, object>>[] retrievals) {
            List list = ctx.Web.Lists.GetByTitle(listName);
            query.ListItemCollectionPosition = null;
            ListItemCollection items;
            do {
                items = list.GetItems(query);

                ctx.Load(items, retrievals);
                ctx.ExecuteQuery();

                query.ListItemCollectionPosition = items.ListItemCollectionPosition;
                if (items != null && items.Count > 0) {
                    using (IEnumerator<ListItem> listItemEnumerator = items.GetEnumerator()) {
                        while (listItemEnumerator.MoveNext()) {
                            iterator(listItemEnumerator.Current);
                        }
                    }
                }
            }
            while (query.ListItemCollectionPosition != null);

            items = null;
            list = null;
        }
        public static void IterateItems(this ClientContext ctx, string listName, QueryBuilder.QueryExpression query, ItemMethod iterator, params System.Linq.Expressions.Expression<Func<ListItemCollection, object>>[] retrievals) {
            List list = ctx.Web.Lists.GetByTitle(listName);
            CamlQuery caml = new CamlQuery() {
                ViewXml = query
            };
            caml.ListItemCollectionPosition = null;
            ListItemCollection items;
            do {
                items = list.GetItems(caml);

                ctx.Load(items, retrievals);
                ctx.ExecuteQuery();

                caml.ListItemCollectionPosition = items.ListItemCollectionPosition;
                if (items != null && items.Count > 0) {
                    using (IEnumerator<ListItem> listItemEnumerator = items.GetEnumerator()) {
                        while (listItemEnumerator.MoveNext()) {
                            iterator(listItemEnumerator.Current);
                        }
                    }
                }
            }
            while (caml.ListItemCollectionPosition != null);

            items = null;
            list = null;
        }

    }
}
