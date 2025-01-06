using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace 简单关系图_测试_
{
    /**
     * 这个文件用来：把输入的“规则文本”转换为“关系网结构”
     * 父+母=兄-姐-我-弟-妹
     * 父↓ 祖父+祖母=伯父-姑母-父-叔父-姑母
     * 母↓ 外祖父+外祖母=舅父-姨母-母-舅父-姨母
     **/
    // 下面是角色对象，每一个称呼都是一个角色
    public class Person
    {
        // 属性 (Properties)
        public int code { get; set; } // ***编号
        public string Name { get; set; } // *称呼(唯一)
        public int Sex { get; set; } // *性别：0未知，1男，2女
        // 关系属性：
        public Person Up_father { get; set; } // 父
        public Person Up_mother { get; set; } // 母
        public Person Lv_half { get; set; } // 夫/妻
        public List<Person> Down_child { get; set; } // 子女

        // 构造函数 (Constructor) - 可选，用于初始化对象
        public Person(string name)
        {
            Name = name;// *称呼(唯一)
            Sex = getSex(name);// *性别：0未知，1男，2女
            Up_father = null;// 父
            Up_mother = null;// 母
            Lv_half = null;// 夫/妻
            Down_child = new List<Person>();// 子女
            code = 0;// ***编号
        }
        // 方法 (Methods) - 可选，用于定义对象的行为
        public int getSex(string name)
        {
            if (name.EndsWith("父") || name.EndsWith("兄") || name.EndsWith("弟"))
                return 1;
            else if (name.EndsWith("母") || name.EndsWith("姐") || name.EndsWith("妹"))
                return 2;
            else
                return 0;
        }

    }
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
                            .ToArray(); // 转换为 string[]
            }
            else
            {
                return null; // 字符串不符合格式
            }
        }
    }
}
