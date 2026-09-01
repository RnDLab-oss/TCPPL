using System.Data;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ERP_API.Helpers
{
    public class pvtUtilitiy
    {
        public static string CleanString(string input)
        {
            return string.IsNullOrWhiteSpace(input) ? "" : input.Trim();
        }
        public static string GetLocalIPAddress()
        {
            string localIP = "Not Found";

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                // Only take IPv4 address, not IPv6
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }


        public static string ConvertNumberToWords(long number)
        {
            if (number == 0)
                return "Zero";

            if (number < 0)
                return "Minus " + ConvertNumberToWords(Math.Abs(number));

            string[] unitsMap = new string[]
            {
                "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine",
                "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen",
                "Seventeen", "Eighteen", "Nineteen"
            };

            string[] tensMap = new string[]
            {
                "Zero","Ten","Twenty","Thirty","Forty","Fifty","Sixty","Seventy","Eighty","Ninety"
            };

            string words = "";

            if ((number / 10000000) > 0)
            {
                words += ConvertNumberToWords(number / 10000000) + " Crore ";
                number %= 10000000;
            }

            if ((number / 100000) > 0)
            {
                words += ConvertNumberToWords(number / 100000) + " Lakh ";
                number %= 100000;
            }

            if ((number / 1000) > 0)
            {
                words += ConvertNumberToWords(number / 1000) + " Thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += ConvertNumberToWords(number / 100) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }
            return words.Trim();
        }

        // For Nullable TimeSpan (TimeSpan?)
        public static string ToTimeNull(TimeSpan? time)
        {
            if (time.HasValue)
                return time.Value.ToString(@"hh\:mm\:ss");

            return null;
        }

        // For Non-Nullable TimeSpan
        public static string ToTimeOnly(TimeSpan time)
        {
            return time.ToString(@"hh\:mm\:ss");
        }

        public static TimeSpan ToTimeSpanSafe(object value)
        {
            if (value == null || value == DBNull.Value)
                return TimeSpan.Zero;

            // Agar already TimeSpan hai
            if (value is TimeSpan timeSpanValue)
                return timeSpanValue;

            // Agar string hai
            if (TimeSpan.TryParse(value.ToString(), out TimeSpan result))
                return result;

            return TimeSpan.Zero;
        }

        public static int ToInt(object value)
        {
            return value == DBNull.Value ? 0 : Convert.ToInt32(value);
        }
        public static decimal ToDecimal(object value)
        {
            return value == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(value), 2);
        }

        public static double ToDouble(object value)
        {
            return value == DBNull.Value ? 0 : Convert.ToDouble(value);
        }

        public static string ToString(object value)
        {
            return value == DBNull.Value ? "" : value.ToString().Trim();
        }
        public static DateTime ToDateTime(object value)
        {
            return value == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(value);
        }

        public static DateTime? ToDateTimeNull(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;

            DateTime result;
            if (DateTime.TryParse(value.ToString().Trim(), out result))
                return result;

            return null;
        }

        public static string ToDateOnly(object value)
        {
            if (value == null || value == DBNull.Value)
                return string.Empty;

            DateTime dateValue;

            if (DateTime.TryParse(value.ToString(), out dateValue))
            {
                return dateValue.ToString("dd-MM-yyyy");
            }
            return string.Empty;
        }

        public static bool ToBool(object value)
        {
            if (value == DBNull.Value || value == null)
                return false;

            if (value is bool)
                return (bool)value;

            bool result;
            if (bool.TryParse(value.ToString(), out result))
                return result;

            // Handle 1/0 as true/false
            if (value.ToString() == "1") return true;
            if (value.ToString() == "0") return false;

            return false;
        }

    
    }
}
