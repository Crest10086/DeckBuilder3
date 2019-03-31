using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;

namespace OCG.DataStructure
{
    public class Card
    {
        private string link;

        public int Id { get; set; }                   //ID
        public string Name { get; set; }              //卡片名称
        public string JapName { get; set; }           //日文名
        public string EnName { get; set; }            //英文名
        public string OldName { get; set; }           //旧卡名
        public string ShortName { get; set; }         //简称
        public CardType CardType { get; set; } = new CardType();  //卡片类型
        public int Level { get; set; }                //星数
        public string LevelText => Level > 0 ? Level.ToString() : "";
        public int PendulumL { get; set; }            //左刻度
        public int PendulumR { get; set; }            //右刻度
        public bool IsPendulum => PendulumL > 0 || PendulumR > 0;   //是否灵摆
        public string Attribute { get; set; }           //属性
        public string Race { get; set; }             //种族
        public string EffectType { get; set; }        //效果归类
        public string Atk { get; set; }               //攻击
        public int AtkValue { get; set; }             //攻击力
        public string Def { get; set; }               //防御
        public int DefValue { get; set; }             //防御力
        public string DefText => CardType.SubType == SubCardTypes.TYPE_LINK ? "Link-" + LinkCount.ToString() : Def;
        public string Effect { get; set; }            //效果
        public string Infrequence { get; set; }       //稀罕度
        public string Package { get; set; }           //卡包
        public int Limit { get; set; }                //禁限类型
        public string Code { get; set; }              //8位密码
        public string CodeList { get; set; }          //8位密码列表
        public string Alias { get; set; }             //规则同名卡8位密码
        //public string AliasList { get; set; }         //规则同名卡密码列表
        public string Adjust { get; set; }            //调整
        public string Associate { get; set; }         //关联卡片
        public CardRule CardRule { get; set; }        //卡片归属
        public string Link
        {
            get => link;
            set
            {
                link = value;
                LinkCount = value?.Length ?? 0;
            }
        }             //LINK字符串
        public int LinkCount { get; private set; }    //LINK数
        public DateTime CreateTime { get; set; } = DateTime.MaxValue;     //创建时间
        public DateTime UpdateTime { get; set; } = DateTime.MaxValue;     //更新时间

        private string GetTextInfo()
        {
            StringBuilder info = new StringBuilder();

            switch (this.Limit)
            {
                case 0:
                    info.Append("禁止卡");
                    break;
                case 1:
                    info.Append("限制卡");
                    break;
                case 2:
                    info.Append("准限制卡");
                    break;
                case -4:
                    info.Append("观赏卡");
                    break;
            }

            if (info.Length > 0 && this.CardRule != CardRule.TYPE_OT)
                info.Append("、");

            switch (this.CardRule)
            {
                case CardRule.TYPE_TCG:
                    info.Append("TCG专有卡");
                    break;
                case CardRule.TYPE_OCG:
                    info.Append("OCG专有卡");
                    break;
                case CardRule.TYPE_DIY:
                    info.Append("DIY卡");
                    break;
            }

            if (info.Length > 0)
                info.Append("\r\n");

            info.Append("中文名：");
            info.Append(this.Name);
            info.Append("\r\n");

            /*if (this.oldName != "" && this.oldName != this.name)
            {
                info.Append("旧卡名：");
                info.Append(this.oldName);
                info.Append("\r\n");
            }*/

            if (!string.IsNullOrWhiteSpace(this.JapName))
            {
                info.Append("日文名：");
                info.Append(this.JapName);
                info.Append("\r\n");
            }

            if (!string.IsNullOrWhiteSpace(this.EnName))
            {
                info.Append("英文名：");
                info.Append(this.EnName);
                info.Append("\r\n");
            }

            if (!string.IsNullOrWhiteSpace(this.ShortName) && this.ShortName != this.Name)
            {
                info.Append("简称：");
                info.Append(this.ShortName);
                info.Append("\r\n");
            }

            if (!string.IsNullOrWhiteSpace(this.CodeList))
            {
                info.Append("卡片密码：");
                info.Append(this.CodeList);
                info.Append("\r\n");
            }

            info.Append("卡片种类：");
            info.Append(this.CardType.FullText);
            info.Append("\r\n");

            if (this.Level > 0)
            {
                if (this.CardType.SubType != SubCardTypes.TYPE_XYZ)
                    info.Append("星级：");
                else
                    info.Append("阶级：");
                info.Append(this.Level.ToString());
                info.Append("\r\n");
            }

            if (this.CardType.BaseType == BaseCardTypes.TYPE_MONSTER)   //是怪兽
            {
                info.Append("属性：");
                info.Append(this.Attribute);
                info.Append("\r\n");

                info.Append("种族：");
                info.Append(this.Race);
                info.Append("\r\n");

                info.Append("攻击：");
                info.Append(this.Atk);
                info.Append("\r\n");

                info.Append("防御：");
                info.Append(this.Def);
                info.Append("\r\n");
            }

            /*
            if (this.infrequence != "")
            {
                info.Append("罕见度：");
                info.Append(this.infrequence);
                info.Append("\r\n");
            }

            if (this.package != "")
            {
                info.Append("卡包：");
                info.Append(this.package);
                info.Append("\r\n");
            }
            */

            if (this.PendulumL > 0 || this.PendulumR > 0)
            {
                info.Append("灵摆刻度：左");
                info.Append(this.PendulumL.ToString());
                info.Append(" 右");
                info.Append(this.PendulumR.ToString());
                info.Append("\r\n");
            }

            info.Append("效果：");
            info.Append(this.Effect);
            info.Append("\r\n");

            return info.ToString();
        }
        public string Text => GetTextInfo();

    }
}
