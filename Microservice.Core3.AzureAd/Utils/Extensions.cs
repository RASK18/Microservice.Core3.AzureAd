using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// ReSharper disable UnusedMember.Global
namespace Microservice.Core3.AzureAd.Utils
{
    public static class Extensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> iEnum) => iEnum == null || !iEnum.Any();

        public static string ToBase64(this string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(bytes);
        }

        public static string FromBase64(this string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}