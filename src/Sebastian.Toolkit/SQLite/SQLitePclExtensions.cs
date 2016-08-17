using System;
using SQLitePCL;

namespace Sebastian.Toolkit.SQLite
{
    public static class SQLitePclExtensions
    {
        public static bool IsSuccess(this SQLiteResult result)
        {
            return result == SQLiteResult.OK || result == SQLiteResult.DONE;
        }

        public static void ThrowOnFailure(this SQLiteResult result, string exceptionMessage)
        {
            if (result != SQLiteResult.OK && result != SQLiteResult.DONE)
            {
                throw new Exception(string.Format("{0} | Result: {1}", exceptionMessage, result));
            }
        }

        public static void Binding(this ISQLiteStatement statement, string paramName, object value)
        {
            if (value is DateTime)
            {
                statement.Bind(paramName, ((DateTime)value).ToUniversalTime().Ticks);
                return;
            }
            if (value is bool)
            {
                statement.Bind(paramName, Convert.ToInt64((bool)value));
                return;
            }
            if (value is Enum)
            {
                statement.Bind(paramName, (int)value);
                return;
            }
            statement.Bind(paramName, value);
        }

        public static T GetValue<T>(this ISQLiteStatement statement, string key)
        {
            return GetValue<T>(statement[key]);
        }

        private static T GetValue<T>(object value)
        {
            var type = typeof(T);
            if (type == typeof(DateTime))
            {
                if (value != null)
                {
                    var ticks = (long)value;
                    if (ticks != 0)
                    {
                        return (T)(object)(new DateTime(ticks, DateTimeKind.Utc));
                    }
                }
                return (T)(object)(new DateTime());
            }
            if (type == typeof(DateTime?))
            {
                if (value != null)
                {
                    var ticks = (long)value;
                    if (ticks != 0)
                    {
                        return (T)(object)(new DateTime(ticks, DateTimeKind.Utc));
                    }
                }
            }
            else if (type == typeof(bool))
            {
                if (value != null)
                {
                    var result = (long)value;
                    if (result != 0)
                    {
                        return (T)(object)(true);
                    }
                }
                return (T)(object)(false);
            }
            else if (type == typeof(bool?))
            {
                if (value != null)
                {
                    var result = (long)value;
                    if (result != 0)
                    {
                        return (T)(object)(true);
                    }
                }
            }
            else if (type == typeof(decimal))
            {
                // Note: There might be a loss of precision since SQLite's conversion
                // from floating point to decimal is limited
                if (value != null)
                {
                    return (T)Convert.ChangeType(value, type);
                }
            }
            else if (type == typeof (int))
            {
                if (value != null)
                {
                    return (T) (object) (int) (long) value;
                }
                return (T) (object) 0;
            }

            return (T)value;
        }
    }
}
