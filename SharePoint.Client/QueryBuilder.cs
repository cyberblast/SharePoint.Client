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

        public static Expression Equals(Field field, Value value) {
            return string.Format(@"<Eq>{0}{1}</Eq>", field, value);
        }
        public static Expression GreaterThan(Field field, Value value) {
            return string.Format(@"<Gt>{0}{1}</Gt>", field, value);
        }
        public static Expression GreaterOrEqual(Field field, Value value) {
            return string.Format(@"<Geq>{0}{1}</Geq>", field, value);
        }
        public static Expression LesserThan(Field field, Value value) {
            return string.Format(@"<Lt>{0}{1}</Lt>", field, value);
        }
        public static Expression LesserOrEqual(Field field, Value value) {
            return string.Format(@"<Leq>{0}{1}</Leq>", field, value);
        }
        public static Expression NotEqual(Field field, Value value) {
            return string.Format(@"<Neq>{0}{1}</Neq>", field, value);
        }
        public static Expression BeginsWith(Field field, Value value) {
            return string.Format(@"<BeginsWith>{0}{1}</BeginsWith>", field, value);
        }
        /// <summary>
        /// Specifies whether the value of a list item for the field specified by the FieldRef element is equal to one of the values specified by the Values element.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression In(Field field, params Value[] value) {
            string values = string.Join<Value>(string.Empty, value);
            return string.Format(@"<In>{0}<Values>{1}</Values></In>", field, values);
        }
        /// <summary>
        /// Searches for a string anywhere within a column that holds Text or Note field type values.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Expression Contains(Field field, Value value) {
            return string.Format(@"<Contains>{0}{1}</Contains>", field, value);
        }
        /// <summary>
        /// If the specified field is a Lookup field that allows multiple values, specifies that the Value element is included in the list item for the field that is specified by the FieldRef element.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression Includes(Field field, Value value) {
            return string.Format(@"<Includes>{0}{1}</Includes>", field, value);
        }
        /// <summary>
        /// If the specified field is a Lookup field that allows multiple values, specifies that the Value element is excluded from the list item for the field that is specified by the FieldRef element.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression NotIncludes(Field field, Value value) {
            return string.Format(@"<NotIncludes>{0}{1}</NotIncludes>", field, value);
        }

        public static Expression Null(Field field) {
            return string.Format(@"<IsNull>{0}</IsNull>", field);
        }
        public static Expression NullOrEmpty(Field field, FieldType type) {
            return string.Format(@"<Or><IsNull>{0}</IsNull><Eq>{0}<Value Type=""{1}""></Value></Eq></Or>", field, type.ToString());
        }
        public static Expression NotNull(Field field) {
            return string.Format(@"<IsNotNull>{0}</IsNotNull>", field);
        }
        
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
