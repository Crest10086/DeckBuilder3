using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using System.Drawing;
using MyTools;

namespace OCG.DataStructure
{
    [Flags]
    public enum BaseCardTypes
    {
        [Description("怪兽")] TYPE_MONSTER = 1,
        [Description("魔法")] TYPE_SPELL = 2,
        [Description("陷阱")] TYPE_TRAP = 4,
    };

    [Flags]
    public enum SubCardTypes
    {
        [Description("通常")] TYPE_NORMAL = 16,
        [Description("效果")] TYPE_EFFECT = 32,
        [Description("融合")] TYPE_FUSION = 64,
        [Description("仪式")] TYPE_RITUAL = 128,
        [Description("同调")] TYPE_SYNCHRO = 8192,
        [Description("超量")] TYPE_XYZ = 8388608,
        [Description("连接")] TYPE_LINK = 67108864,
        [Description("速攻")] TYPE_QUICKPLAY = 65536,
        [Description("永续")] TYPE_CONTINUOUS = 131072,
        [Description("装备")] TYPE_EQUIP = 262144,
        [Description("场地")] TYPE_FIELD = 524288,
        [Description("反击")] TYPE_COUNTER = 1048576
    }

    [Flags]
    public enum FullCardTypes
    {
        [Description("怪兽")] TYPE_MONSTER = 1,
        [Description("魔法")] TYPE_SPELL = 2,
        [Description("陷阱")] TYPE_TRAP = 4,
        [Description("通常")] TYPE_NORMAL = 16,
        [Description("效果")] TYPE_EFFECT = 32,
        [Description("融合")] TYPE_FUSION = 64,
        [Description("仪式")] TYPE_RITUAL = 128,
        [Description("陷阱怪兽")] TYPE_TRAPMONSTER = 256,
        [Description("灵魂")] TYPE_SPIRIT = 512,
        [Description("同盟")] TYPE_UNION = 1024,
        [Description("二重")] TYPE_DUAL = 2048,
        [Description("调整")] TYPE_TUNER = 4096,
        [Description("同调")] TYPE_SYNCHRO = 8192,
        [Description("衍生物")] TYPE_TOKEN = 16384,
        [Description("速攻")] TYPE_QUICKPLAY = 65536,
        [Description("永续")] TYPE_CONTINUOUS = 131072,
        [Description("装备")] TYPE_EQUIP = 262144,
        [Description("场地")] TYPE_FIELD = 524288,
        [Description("反击")] TYPE_COUNTER = 1048576,
        [Description("反转")] TYPE_FLIP = 2097152,
        [Description("卡通")] TYPE_TOON = 4194304,
        [Description("超量")] TYPE_XYZ = 8388608,
        [Description("灵摆")] TYPE_PENDULUM = 16777216,
        [Description("特殊召唤")] TYPE_SPSUMMON = 33554432,
        [Description("连接")] TYPE_LINK = 67108864
    }

    public class CardType
    {
        BaseCardTypes baseType;
        SubCardTypes subType;
        FullCardTypes fullType;

        public string BaseText { get; private set; }
        public string SubText { get; private set; }
        public string FullText { get; private set; }
        public string ShortText { get; private set; }

        public BaseCardTypes BaseType
        {
            get => baseType;
            set
            {
                baseType = value;
                BaseText = value.GetDesc<BaseCardTypes>();
                SetColor();
                SetShortText();
            }
        }
        public SubCardTypes SubType
        {
            get => subType;
            set
            {
                subType = value;
                SubText = value.GetDesc<SubCardTypes>();
                SetColor();
                SetShortText();
            }
        }
        public FullCardTypes FullType
        {
            get => fullType;
            set
            {
                fullType = value;
                var ss = value.GetDescList<FullCardTypes>();
                FullText = string.Join("|", ss);

                var value2 = (BaseCardTypes)value;
                foreach (BaseCardTypes btvalue in Enum.GetValues(typeof(BaseCardTypes)))
                {
                    if (value2.HasFlag(btvalue))
                    {
                        BaseType = btvalue;
                    }
                }

                var value3 = (SubCardTypes)value;
                var st = SubCardTypes.TYPE_NORMAL;
                foreach (SubCardTypes stvalue in Enum.GetValues(typeof(SubCardTypes)))
                {
                    if (value3.HasFlag(stvalue))
                    {
                        st = stvalue;
                    }
                }
                SubType = st;
            }
        }

        public Color CardColor { get; private set; } = default(Color);

        private void SetColor()
        {
            switch (BaseType)
            {
                case BaseCardTypes.TYPE_MONSTER:
                    switch (SubType)
                    {
                        case SubCardTypes.TYPE_EFFECT:
                            CardColor = Color.OrangeRed;
                            break;
                        case SubCardTypes.TYPE_NORMAL:
                            CardColor = Color.SandyBrown;
                            break;
                        case SubCardTypes.TYPE_FUSION:
                            CardColor = Color.DarkOrchid;
                            break;
                        case SubCardTypes.TYPE_RITUAL:
                            CardColor = Color.DodgerBlue;
                            break;
                        case SubCardTypes.TYPE_SYNCHRO:
                            CardColor = Color.DarkSlateGray;
                            break;
                        case SubCardTypes.TYPE_XYZ:
                            CardColor = Color.Black;
                            break;
                        case SubCardTypes.TYPE_LINK:
                            CardColor = Color.Aqua;
                            break;
                        default:
                            CardColor = Color.Gainsboro;
                            break;
                    }
                    break;
                case BaseCardTypes.TYPE_SPELL:
                    CardColor = Color.Green;
                    break;
                case BaseCardTypes.TYPE_TRAP:
                    CardColor = Color.Fuchsia;
                    break;
            }
            CardColor = default(Color);
        }

        private void SetShortText()
        {
            if (string.IsNullOrWhiteSpace(SubText))
                ShortText = "通常" + BaseText;
            else
                ShortText = SubText + BaseText;
        }
    }
}
