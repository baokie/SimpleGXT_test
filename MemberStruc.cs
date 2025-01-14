using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace 简单关系图_测试_
{
    // 分两个：1自动生成文本，2文本生成图像
    // 结构需要统一，0-1-2始终统一结构，结构到绘制还需要一个算法
    // 结构到文本；结构到图像；结构到绘制
    /**
     * 这个文件用来：把输入的“规则文本”转换为“关系网结构”
     * 父+母=兄-姐-我-弟-妹
     * 父↓ 祖父+祖母=伯父-姑母-父-叔父-姑母
     * 母↓ 外祖父+外祖母=舅父-姨母-母-舅父-姨母
     **/
    // 下面是角色对象，每一个称呼都是一个角色
    public class Person
    {
        // ***属性 (Properties)
        public int Self { get; set; }// ***自己的性别：0未知，1男，2女
        public List<int> CodeList { get; set; }// ***索引路线，从0(自己)开始，向上1父2母，向平0夫/妻，向下-1兄-2弟-3我-4姐-5妹
        /**
        * 规则：上直系五子-12345，上旁系四子-1245，下二子-15
        * 原则：有下加平：
        *   1级-直系向上X世；
        *   2级-直系向上X世，直系向下X世；
        *   3级-直系向下X世；
        *   4级-直系向上X世，直系向下2X世。
        * 补充：
        *   -本亲一套、男方姻亲一套、女方姻亲一套；
        *   -范围外取其直系到自己所在世对其称呼。
        * 位置：上(父母)，下(五子)；上下可互换；父母不互换，五子不互换。
        **/
        public string Name { get; set; } // *完整称呼(唯一，与路线对应)（实际上不唯一，比如叔伯子女：堂兄姐弟妹，可以灰色合并）
        public int Sex { get; set; } // *性别：0未知，1男，2女
        public string NameCut { get; set; } // 简化称呼(不唯一，俗称)
        // ***关系属性：
        public Person Up_father { get; set; } // 父
        public Person Up_mother { get; set; } // 母
        public Person Lv_half { get; set; } // 夫/妻
        public List<Person> Down_child { get; set; } // 子女

        // ***构造函数 (Constructor) - 可选，用于初始化对象
        public Person()
        {

        }
        // ***方法 (Methods) - 可选，用于定义对象的行为
        // 添加编号到路线
        /**
        * 可用：1父，2母，0夫/妻，-1兄，-2姐，-3我，-4弟，-5妹；
        * 性别：男：1，-1，-4；女：2，-2，-5；
        * 规则：
        *   (0,-x,self)=0；
        *   (x,-3)=0;(x,-1245,y)=y;(-x,12,-y)=-y；
        *   (男,-1245,1)=男，(女,-1245,2)=女；
        *   (x,-1245)=(y,-1245)，(-x,12)=(-y,12)；
        **/
        public List<int> addCode(List<int> codelist, int code)
        {
            List<int> newList = new List<int>(codelist); // 使用复制构造函数创建副本
            newList.Add(code);
            return newList;
        }
        // 获取性别：通过称呼、编号、路线
        public int getSex(string name) // 通过完整称呼获取性别：0未知，1男，2女
        {
            if (name.EndsWith("父") || name.EndsWith("兄") || name.EndsWith("弟"))
                return 1;
            else if (name.EndsWith("母") || name.EndsWith("姐") || name.EndsWith("妹"))
                return 2;
            else
                return 0;
        }
        public int getSex(int code) // 通过编号获取性别：0未知，1男，2女
        {
            if (code == 1 || code == -1 || code == -4)
                return 1;
            else if (code == 2 || code == -2 || code == -5)
                return 2;
            else
                return 0;
        }
        public int getSex(List<int> codeList) // 通过路线获取性别：0未知，1男，2女
        {
            if (codeList != null && codeList.Count > 1)
            {
                int code = codeList[codeList.Count - 1];
                if (code == 0)
                {
                    code = codeList[codeList.Count - 2];
                    return getSexReverse(getSex(code));
                }
                else 
                    return getSex(code);
            }
            else
                return 0;
        }
        // 获取反向性别
        public int getSexReverse(int sex)
        {
            if (sex == 1)
                return 2;
            else if (sex == 2)
                return 1;
            else
                return 0;
        }

    }
}
