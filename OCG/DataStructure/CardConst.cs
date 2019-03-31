using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;

namespace OCG.DataStructure
{

    [Flags]
    public enum CardRule
    {
        [Description("")] TYPE_NONE = 0,
        [Description("OCG")] TYPE_OCG = 1,
        [Description("TCG")] TYPE_TCG = 2,
        [Description("O&T")] TYPE_OT = 3,
        [Description("DIY")] TYPE_DIY = 4
    };

    [Flags]
    public enum CardElement
    {
        [Description("")] TYPE_NONE = 0,
        [Description("地")] TYPE_EARTH = 1,
        [Description("水")] TYPE_WATER = 2,
        [Description("火")] TYPE_FIRE = 4,
        [Description("风")] TYPE_WIND = 8,
        [Description("光")] TYPE_LIGHT = 16,
        [Description("暗")] TYPE_DARK = 32,
        [Description("神")] TYPE_DEVINE = 64
    };

    [Flags]
    public enum CardRace
    {
        [Description("")] TYPE_NONE = 0,
        [Description("战士")] TYPE_WARRIOR = 1,
        [Description("魔法师")] TYPE_SPELLCASTER = 2,
        [Description("天使")] TYPE_FAIRY = 4,
        [Description("恶魔")] TYPE_FIEND = 8,
        [Description("不死")] TYPE_ZOMBIE = 16,
        [Description("机械")] TYPE_MACHINE = 32,
        [Description("水")] TYPE_AQUA = 64,
        [Description("炎")] TYPE_PYRO = 128,
        [Description("岩石")] TYPE_ROCK = 256,
        [Description("鸟兽")] TYPE_WINDBEAST = 512,
        [Description("植物")] TYPE_PLANT = 1024,
        [Description("昆虫")] TYPE_INSECT = 2048,
        [Description("雷")] TYPE_THUNDER = 4096,
        [Description("龙")] TYPE_DRAGON = 8192,
        [Description("兽")] TYPE_BEAST = 16384,
        [Description("兽战士")] TYPE_BEASTWARRIOR = 32768,
        [Description("恐龙")] TYPE_DINOSAUR = 65536,
        [Description("鱼")] TYPE_FISH = 131072,
        [Description("海龙")] TYPE_SEASERPENT = 262144,
        [Description("爬虫")] TYPE_REPTILE = 524288,
        [Description("念动力")] TYPE_PSYCHO = 1048576,
        [Description("幻神兽")] TYPE_DEVINE = 2097152,
        [Description("创造神")] TYPE_CREATORGOD = 4194304,
        [Description("幻龙")] TYPE_WYRM = 8388608,
        [Description("电子界")] TYPE_CYBERS = 16777216
    };

    [Flags]
    public enum LINK_MARK
    {
        [Description("")] TYPE_BOTTOM_NONE = 0,
        [Description("↙")] TYPE_BOTTOM_LEFT = 1,
        [Description("↓")] TYPE_BOTTOM = 2,
        [Description("↘")] TYPE_BOTTOM_RIGHT = 4,
        [Description("←")] TYPE_LEFT = 8,
        [Description("→")] TYPE_RIGHT = 32,
        [Description("↖")] TYPE_TOP_LEFT = 64,
        [Description("↑")] TYPE_TOP = 128,
        [Description("↗")] TYPE_TOP_RIGHT = 256
    }
}
