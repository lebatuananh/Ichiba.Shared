using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Shared.Common
{
    public static class Utility
    {
        public static string ToUnsign(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            text = text.Normalize(NormalizationForm.FormD);
            var regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            text = regex.Replace(text, string.Empty).Replace('đ', 'd').Replace('Đ', 'D');

            return text;
        }

        public static string ToFriendlyUrl(string text)
        {
            var regex = new Regex("[^\\d\\w]+");
            text = regex.Replace(text.ToLower(), "-").Trim(new[]
            {
                '-'
            });
            text = ToUnsign(text);

            return text;
        }

        public static string GenFriendlyUrl(params object[] values)
        {
            if (values == null || values.Length == 0) return string.Empty;

            const string SEPERATOR = "-";
            var seperator = string.Empty;
            var result = string.Empty;

            foreach (var item in values)
            {
                if (item == null) continue;

                var itemString = item.ToString();

                if (string.IsNullOrWhiteSpace(itemString)) continue;

                itemString = ToFriendlyUrl(itemString);
                result = $"{result}{seperator}{itemString}";
                seperator = SEPERATOR;
            }

            return result;
        }

        public static string SubString(string input, int length, bool appendDot)
        {
            string result;

            if (string.IsNullOrEmpty(input))
            {
                result = string.Empty;
            }
            else if (input.Length < length)
            {
                result = input;
            }
            else
            {
                if (appendDot) length -= 3;

                input = input.Substring(0, length);
                var num = input.LastIndexOf(" ");

                if (num > -1) input = input.Substring(0, num);

                if (appendDot) input += "...";

                result = input;
            }

            return result;
        }

        public static string SubStringWithDot(string input, int length)
        {
            return SubString(input, length, true);
        }

        private static string CreateHashInput(string input, string pepper)
        {
            var inputSHA512 = GenerateSHA512String(input);

            return $"{inputSHA512}{pepper}";
        }

        public static string BCryptHash(string input, string pepper)
        {
            var pepperInput = CreateHashInput(input, pepper);
            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var hash = BCrypt.Net.BCrypt.HashPassword(pepperInput, salt);

            return hash;
        }

        public static bool BCryptIsMatch(string input, string pepper, string hash)
        {
            var pepperInput = CreateHashInput(input, pepper);

            return BCrypt.Net.BCrypt.Verify(pepperInput, hash);
        }

        public static string GenerateSHA512String(string inputString)
        {
            var sha512 = SHA512.Create();
            var bytes = Encoding.UTF8.GetBytes(inputString);
            var hash = sha512.ComputeHash(bytes);

            return GetStringFromHash(hash);
        }

        private static string GetStringFromHash(byte[] hash)
        {
            var result = new StringBuilder();

            for (var i = 0; i < hash.Length; i++) result.Append(hash[i].ToString("X2"));

            return result.ToString();
        }

        public static string Md5Hash(string input)
        {
            using (var md5Hash = MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                var sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (var i = 0; i < data.Length; i++) sBuilder.Append(data[i].ToString("x2"));

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
        }

        public static string GenerateString(int length)
        {
            if (length <= 0) return string.Empty;

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return result;
        }

        /// <summary>
        ///     Generate unique string (maybe :))=]])
        /// </summary>
        /// <param name="length">length must greater than 9</param>
        /// <returns></returns>
        public static string GenerateUniqueString(int length = 10)
        {
            const int STRING_MIN_LENGTH = 10;

            if (length < STRING_MIN_LENGTH) length = STRING_MIN_LENGTH;

            var hash = Guid.NewGuid()
                .ToString()
                .GetHashCode()
                .ToString("x")
                .ToUpper();
            var randomLength = length - hash.Length;
            var random = GenerateString(randomLength);

            var result = $"{hash}{random}";

            return result;
        }

        public static DateTime DateTimeWithoutMilisecond(DateTime dateTime)
        {
            return new DateTime(dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                dateTime.Second);
        }

        public static DateTime UtcNowWithoutMilisecond()
        {
            return DateTimeWithoutMilisecond(DateTime.UtcNow);
        }

        #region ToEnum<T>

        public static T ToEnum<T>(object input, T _default)
        {
            try
            {
                if (input == null) return _default;

                if (typeof(string) == input.GetType())
                    _default = (T) Enum.Parse(typeof(T), input.ToString(), true);
                else
                    _default = (T) Enum.ToObject(typeof(T), input);

                return _default;
            }
            catch
            {
                return _default;
            }
        }

        public static T ToEnumOrDefault<T>(object input)
        {
            return ToEnum(input, default(T));
        }

        #endregion ToEnum<T>
    }
}