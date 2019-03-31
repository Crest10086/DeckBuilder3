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
        public string Element { get; set; }           //属性
        public string Race { get; set; }             //种族
        public string EffectType { get; set; }        //效果归类
        public string Atk { get; set; }               //攻击
        public int AtkValue { get; set; }             //攻击力
        public string Def { get; set; }               //防御
        public int DefValue { get; set; }             //防御力
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
        public DateTime Createtime { get; set; }      //创建时间
        public DateTime Updatetime { get; set; }      //更新时间
    }
}
