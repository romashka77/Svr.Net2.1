using System;

namespace Svr.Infrastructure.Extensions
{
    public static class StringExtension
    {
        public static long? ToLong(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            try
            {
                return long.Parse(str);
            }
            catch (Exception)
            {

                return null;
            }
        }
        public static string ErrorFind(this string id)
        {
            return $"Ошибка: Не удалось найти ID = {id}.";
        }
        public static string GetCode(this string name, DateTime date, int? year, long? id)
        {
            return (id != null ? $"{id}-" : "") + (year != null ? $"{year}-" : "") + $"{name}/{date:D}";
        }
    }
}
