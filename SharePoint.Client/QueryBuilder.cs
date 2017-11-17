using Microsoft.SharePoint.Client;

namespace SharePoint.Client
{
    public class QueryBuilder
    {
        public static string Query(Expression filter, int rowlimit, params Field[] orderByFields)
        {
            return string.Format("<View><Query>{0}{1}</Query><RowLimit>{2}</RowLimit></View>", Where(filter), OrderBy(orderByFields), rowlimit);
        }

        #region Expressions

        public static Expression Geq(Value a, Value b) {
            return string.Format(@"<Geq>{0}{1}</Geq>", a, b);
        }
        public static Expression Eq(Value a, Value b) {
            return string.Format(@"<Eq>{0}{1}</Eq>", a, b);
        }

        // Add more Expressions here...

        #endregion

        public static Value Static(object value, FieldType type, bool? includeTimeValue = null) {
            if(includeTimeValue.HasValue)
                return string.Format(@"<Value Type=""{1}"" IncludeTimeValue=""{2}"">{0}</Value>", value, type.ToString(), includeTimeValue.ToString().ToUpper());
            else
                return string.Format(@"<Value Type=""{1}"">{0}</Value>", value, type.ToString());
        }
        public static Field FieldRef(string fieldname, bool? lookupId = null) {
            if (lookupId.HasValue)
                return string.Format(@"<FieldRef Name=""{0}"" LookupId=""{1}"" />", fieldname, lookupId.ToString().ToUpper());
            else
                return string.Format(@"<FieldRef Name=""{0}"" />", fieldname);
        }

        #region Privates
        private static string Where(Expression condition) {
            return string.Concat("<Where>", condition, "</Where>");
        }
        private static string OrderBy(params Field[] fieldRef) {
            if (fieldRef == null || fieldRef.Length == 0)
                return string.Empty;
            return string.Concat("<OrderBy>", string.Join<Field>("", fieldRef), "</OrderBy>");
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
            public static implicit operator string(Value d) {
                return d.ToString();
            }
            public static implicit operator Value(string d) {
                return new Value(d);
            }
        }
        public class Field : Value {
            public Field(string value) : base(value) { }

            public static implicit operator string(Field d) {
                return d.ToString();
            }
            public static implicit operator Field(string d) {
                return new Field(d);
            }
        }

        #endregion
    }
}
