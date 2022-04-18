using System;

namespace GiantTextFileSorter.Common.Extensions
{
    public static class StringExtensions
    {
        public static string ToBytes(this long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB" };
            if (byteCount == 0)
            {
                return "0" + suf[0];
            }

            var bytes = Math.Abs(byteCount);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, place), 1);
            
            return (Math.Sign(byteCount) * num) + suf[place];
        }
    }
}