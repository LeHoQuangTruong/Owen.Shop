using _20T1020637.BusinessLayers;
using _20T1020637.DomainModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace _20T1020637.Web
{
    public static class Converter
    {
        public static DateTime? DMYStringToDateTime(string s, string format = "d/M/yyyy")
        {
            try
            {
                return DateTime.ParseExact(s, format, CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }

        public static UserAccount CookiToUserAccount(string cookie)
        {
            // Json -> obj để sử dụng
            return Newtonsoft.Json.JsonConvert.DeserializeObject<UserAccount>(cookie);
        }

        public static decimal? StringToDecimal(string s)
        {
            try
            {
                return decimal.Parse(s, CultureInfo.InvariantCulture);
            }catch (Exception)
            {
                return null;
            }
        }

    }
}