using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace 简单关系图_测试_
{
    // 下面是主类，调用里面的方法
    public static class RuleWXT
    {
        // 主方法
        public static void getRule(string[] alldata)
        {
            int paddingW = 20, paddingH = 20;


            
        }

        // ***方法
        public static string[] getNameByLine(string input) // 格式化单行字符串：父+母=兄-姐-我-弟-妹
        {
            if (input == null)
                return null;
            int index = input.IndexOf(' '); // 找到第一个空格的索引
            input = index == -1 ? input : input.Substring(index + 1); // 如果没有空格，保持原样；否则，截取空格后面的部分
            index = input.IndexOf('↑'); // 找到第一个索引
            input = index == -1 ? input : input.Substring(index + 1); // 如果没有，保持原样；否则，截取后面的部分
            index = input.IndexOf('↓'); // 找到第一个索引
            input = index == -1 ? input : input.Substring(index + 1); // 如果没有，保持原样；否则，截取后面的部分
            // 正则表达式模式
            string pattern = @"^([^+\s]+)\+([^=\s]+)=([^-=\s]+)-([^-=\s]+)(?:-([^-=\s]+))?(?:-([^-=\s]+))?(?:-([^-=\s]+))?$";
            Match match = Regex.Match(input, pattern);
            if (match.Success)
            {
                // 提取匹配的值，并跳过开头的整个匹配字符串
                return match.Groups.Cast<Group>()
                            .Select(g => g.Value)
                            .Skip(1)
                            .Where(s => !string.IsNullOrEmpty(s)) // 去除空字符串
                            .ToArray(); // 转换为 string[]
            }
            else
            {
                return null; // 字符串不符合格式
            }
        }
    }
}