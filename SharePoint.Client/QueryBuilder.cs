using Microsoft.SharePoint.Client;

namespace cyberblast.SharePoint.Client
{
    public static class QueryBuilder
    {
        public static QueryExpression Query(Expression filter, int rowlimit, params Order[] orderByFields)
        {
            return string.Format("<View><Query>{0}{1}</Query><RowLimit>{2}</RowLimit></View>", Where(filter), OrderBy(orderByFields), rowlimit);
        }

        #region Expressions

        public static Expression Or(Expression condition1, Expression condition2) {
            if (string.IsNullOrEmpty(condition1))
                return condition2;
            if (string.IsNullOrEmpty(condition2))
                return condition1;
            return string.Format("<Or>{0}{1}</Or>", condition1, condition2);
        }
        public static Expression And(Expression condition1, Expression condition2) {
            if (string.IsNullOrEmpty(condition1))
                return condition2;
            if (string.IsNullOrEmpty(condition2))
                return condition1;
            return string.Format("<And>{0}{1}</And>", condition1, condition2);
        }

        public static Expression Geq(Field a, Value b) {
            return string.Format(@"<Geq>{0}{1}</Geq>", a, b);
        }
        public static Expression Eq(Field a, Value b) {
            return string.Format(@"<Eq>{0}{1}</Eq>", a, b);
        }

        public static Expression IsNullOrEmpty(Field field, FieldType type) {
            return string.Format(@"<Or><IsNull>{0}</IsNull><Eq>{0}<Value Type=""{1}""></Value></Eq></Or>", field, type.ToString());
        }

        // Add more Expressions here...

        #endregion

        #region Privates

        private static string Where(Expression condition) {
            return string.Concat("<Where>", condition, "</Where>");
        }
        private static string OrderBy(params Order[] fieldRef) {
            if (fieldRef == null || fieldRef.Length == 0)
                return string.Empty;
            return string.Concat("<OrderBy>", string.Join<Order>(string.Empty, fieldRef), "</OrderBy>");
        }

        #endregion

        #region Types
        
        public class Expression {
            readonly string _value;
            public Expression(string value) {
                this._value = value;
            }
            public static implicit operator string(Expression d) {
                return d.ToString();
            }
            public static implicit operator Expression(string d) {
                return new Expression(d);
            }
        }
        public class Value {
            readonly string _value;
            public Value(string value) {
                this._value = value;
            }
            public Value(object value, FieldType type, bool? includeTimeValue = null) {
                if (includeTimeValue.HasValue)
                    this._value = string.Format(@"<Value Type=""{1}"" IncludeTimeValue=""{2}"">{0}</Value>", value, type.ToString(), includeTimeValue.ToString().ToUpper());
                else
                    this._value = string.Format(@"<Value Type=""{1}"">{0}</Value>", value, type.ToString());
            }
            public static implicit operator string(Value d) {
                return d.ToString();
            }
            public static implicit operator Value(string d) {
                return new Value(d);
            }
        }
        public class Field {
            readonly string _value;

            public Field(string fieldname, bool? lookupId = null) {
                if (lookupId.HasValue)
                    _value = string.Format(@"<FieldRef Name=""{0}"" LookupId=""{1}"" />", fieldname, lookupId.ToString().ToUpper());
                else if (fieldname.StartsWith("<"))
                    _value = fieldname;
                else
                    _value = string.Format(@"<FieldRef Name=""{0}"" />", fieldname);
            }

            public static implicit operator string(Field d) {
                return d.ToString();
            }
            public static implicit operator Field(string d) {
                return new Field(d);
            }
        }
        public class QueryExpression {
            readonly string _value;

            public QueryExpression(string value) {
                this._value = value;
            }

            public static implicit operator string(QueryExpression d) {
                return d.ToString();
            }
            public static implicit operator QueryExpression(string d) {
                return new QueryExpression(d);
            }
        }
        public class Order {
            readonly string _value;

            public Order(string fieldname, bool? ascending = null) {
                if (ascending.HasValue)
                    _value = string.Format(@"<FieldRef Name=""{0}"" Ascending=""{1}"" />", fieldname, ascending.ToString().ToUpper());
                else if (fieldname.StartsWith("<"))
                    _value = fieldname;
                else
                    _value = string.Format(@"<FieldRef Name=""{0}"" />", fieldname);
            }

            public static implicit operator string(Order d) {
                return d.ToString();
            }
            public static implicit operator Order(string d) {
                return new Order(d);
            }
        }

        #endregion
    }
}
