﻿using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NadekoBot.Extensions
{
    public static class Extensions
    {
        public static async Task<IMessage> Reply(this IMessage msg, string content, params object[] args) => await msg.Channel.SendMessageAsync(string.Format(content, args));

        public static async Task<IMessage[]> ReplyLong(this IMessage msg, string content, string breakOn = "\n", string addToEnd = "", string addToStart = "")
        {

            if (content.Length < 2000) return new[] { await msg.Channel.SendMessageAsync(content) };
            var list = new List<IMessage>();

            var temp = Regex.Split(content, breakOn).Select(x => x += breakOn).ToList();
            string toolong;
            //while ((toolong = temp.FirstOrDefault(x => x.Length > 2000)) != null)
            //{
            //    more desperate measures == split on whitespace?
            //}

            StringBuilder builder = new StringBuilder();
            //make this less crappy to look at, maybe it's bugged
            for (int i = 0; i < temp.Count; i++)
            {
                var addition = temp[i];
                //we append 

                if (builder.Length == 0 && i != 0) builder.Append(addToStart + addition);
                else builder.Append(addition);

                //Check if the next would have room
                if (i + 1 >= temp.Count || temp[i + 1].Length + builder.Length + addToEnd.Length > 2000)
                {
                    if (i + 1 < temp.Count) builder.Append(addToEnd);
                    list.Add(await msg.Channel.SendMessageAsync(builder.ToString()));
                    builder.Clear();
                }
            }

            return list.ToArray();
        }

        public static Task<IMessage> SendTableAsync<T>(this IMessageChannel ch, string seed, IEnumerable<T> items, Func<T, string> howToPrint, int columns = 3)
        {
            var i = 0;
            return ch.SendMessageAsync($@"{seed}```xl
{string.Join("\n", items.GroupBy(item => (i++) / columns)
                        .Select(ig => string.Concat(ig.Select(el => howToPrint(el)))))}
```");
        }

        public static Task<IMessage> SendTableAsync<T>(this IMessageChannel ch, IEnumerable<T> items, Func<T, string> howToPrint, int columns = 3)
        {
            return ch.SendTableAsync("", items, howToPrint, columns);
        }

        public static async Task<string> ShortenUrl(this string url)
        {
            if (string.IsNullOrWhiteSpace(NadekoBot.Credentials.GoogleApiKey)) return url;
            try
            {
                var httpWebRequest =
                    (HttpWebRequest)WebRequest.Create("https://www.googleapis.com/urlshortener/v1/url?key=" +
                                                       NadekoBot.Credentials.GoogleApiKey);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(await httpWebRequest.GetRequestStreamAsync().ConfigureAwait(false)))
                {
                    var json = "{\"longUrl\":\"" + Uri.EscapeDataString(url) + "\"}";
                    streamWriter.Write(json);
                }

                var httpResponse = (await httpWebRequest.GetResponseAsync().ConfigureAwait(false)) as HttpWebResponse;
                var responseStream = httpResponse.GetResponseStream();
                using (var streamReader = new StreamReader(responseStream))
                {
                    var responseText = await streamReader.ReadToEndAsync().ConfigureAwait(false);
                    return Regex.Match(responseText, @"""id"": ?""(?<id>.+)""").Groups["id"].Value;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Shortening of this url failed: " + url);
                Console.WriteLine(ex.ToString());
                return url;
            }
        }

        /// <summary>
        /// returns an IEnumerable with randomized element order
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> items)
        {
            // Thanks to @Joe4Evr for finding a bug in the old version of the shuffle
            using (var provider = RandomNumberGenerator.Create())
            {
                var list = items.ToList();
                var n = list.Count;
                while (n > 1)
                {
                    var box = new byte[(n / Byte.MaxValue) + 1];
                    int boxSum;
                    do
                    {
                        provider.GetBytes(box);
                        boxSum = box.Sum(b => b);
                    }
                    while (!(boxSum < n * ((Byte.MaxValue * box.Length) / n)));
                    var k = (boxSum % n);
                    n--;
                    var value = list[k];
                    list[k] = list[n];
                    list[n] = value;
                }
                return list;
            }
        }

        public static string TrimTo(this string str, int maxLength, bool hideDots = false)
        {
            if (maxLength < 0)
                throw new ArgumentOutOfRangeException(nameof(maxLength), $"Argument {nameof(maxLength)} can't be negative.");
            if (maxLength == 0)
                return string.Empty;
            if (maxLength <= 3)
                return string.Concat(str.Select(c => '.'));
            if (str.Length < maxLength)
                return str;
            return string.Concat(str.Take(maxLength - 3)) + (hideDots ? "" : "...");
        }

        /// <summary>
        /// Removes trailing S or ES (if specified) on the given string if the num is 1
        /// </summary>
        /// <param name="str"></param>
        /// <param name="num"></param>
        /// <param name="es"></param>
        /// <returns>String with the correct singular/plural form</returns>
        public static string SnPl(this string str, int? num, bool es = false)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            if (num == null)
                throw new ArgumentNullException(nameof(num));
            return num == 1 ? str.Remove(str.Length - 1, es ? 2 : 1) : str;
        }

        //http://www.dotnetperls.com/levenshtein
        public static int LevenshteinDistance(this string s, string t)
        {
            var n = s.Length;
            var m = t.Length;
            var d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (var i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (var j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (var i = 1; i <= n; i++)
            {
                //Step 4
                for (var j = 1; j <= m; j++)
                {
                    // Step 5
                    var cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }

        public static int KiB(this int value) => value * 1024;
        public static int KB(this int value) => value * 1000;

        public static int MiB(this int value) => value.KiB() * 1024;
        public static int MB(this int value) => value.KB() * 1000;

        public static int GiB(this int value) => value.MiB() * 1024;
        public static int GB(this int value) => value.MB() * 1000;

        public static ulong KiB(this ulong value) => value * 1024;
        public static ulong KB(this ulong value) => value * 1000;

        public static ulong MiB(this ulong value) => value.KiB() * 1024;
        public static ulong MB(this ulong value) => value.KB() * 1000;

        public static ulong GiB(this ulong value) => value.MiB() * 1024;
        public static ulong GB(this ulong value) => value.MB() * 1000;

    }
}