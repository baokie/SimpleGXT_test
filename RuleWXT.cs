using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 简单关系图_测试_
{
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
    public static class RuleWXT
    {
        public static void getRule(string[] alldata)
        {
            int paddingW = 20, paddingH = 20;


            
        }
    }
}
