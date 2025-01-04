using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 简单关系图_测试_
{
    public static class SomeDeal
    {
        // 以下是时间获取
        public static string GetCurrentDateTimeFormatted()
        {
            DateTime now = DateTime.Now; // 获取当前本地时间
            string formattedDateTime = now.ToString("yyyyMMdd_HHmmss_fff"); // 使用自定义格式

            return formattedDateTime;
        }

        public static string GetCurrentUtcDateTimeFormatted()
        {
            DateTime now = DateTime.UtcNow; // 获取当前UTC时间
            string formattedDateTime = now.ToString("yyyyMMdd_HHmmss_fff"); // 使用自定义格式

            return formattedDateTime;
        }

        public static string GetCurrentDateTimeFormatted(string format)
        {
            DateTime now = DateTime.Now; // 获取当前本地时间
            string formattedDateTime = now.ToString(format); // 使用自定义格式

            return formattedDateTime;
        }
        public static string GetCurrentUtcDateTimeFormatted(string format)
        {
            DateTime now = DateTime.UtcNow; // 获取当前UTC时间
            string formattedDateTime = now.ToString(format); // 使用自定义格式

            return formattedDateTime;
        }
    }
}
