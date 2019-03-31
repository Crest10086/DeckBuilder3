using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;

namespace MyTools
{
    public static class EnumChange
    {
        //枚举值转字符串
        public static string GetDesc<T>(this T enumType) where T : struct, IConvertible
        {
            try
            {
                FieldInfo fi = enumType.GetType().GetField(enumType.ToString());
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                return (attributes.Length > 0) ? attributes[0].Description : enumType.ToString();
            }
            catch
            {
                return null;
            }
        }

        //位域枚举值转字符串列表
        public static string[] GetDescList<T>(this T enumType) where T: struct, IConvertible
        {
            var ss = new List<string>();
            var values = Enum.GetValues(typeof(T));
            foreach (T value in values)
            {
                Enum et = (Enum)(object)enumType;
                Enum ev = (Enum)(object)value;
                if (et.HasFlag(ev))
                    ss.Add(GetDesc<T>(value));
            }

            return ss.ToArray<string>();
        }

        //字符串转枚举值
        public static T ParseEnum<T>(this string description) where T : struct, IConvertible
        {
            Type _type = typeof(T);
            foreach (FieldInfo field in _type.GetFields())
            {
                DescriptionAttribute[] _curDesc = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (_curDesc != null && _curDesc.Length > 0)
                {
                    if (_curDesc[0].Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException($"{description} 未能找到对应的枚举.", "Description");
        }

        //字符串转枚举值
        public static T ParseEnumOrDefault<T>(this string description, T defValue = default(T)) where T : struct, IConvertible
        {
            try
            {
                return ParseEnum<T>(description);
            }
            catch
            {
                return Enum.TryParse<T>(description, out var enumType) ? enumType : defValue;
            }
        }

        //字符串转枚举值
        public static bool TryParseEnum<T>(this string description, out T enumType) where T : struct, IConvertible
        {
            try
            {
                enumType = ParseEnum<T>(description);
                return true;
            }
            catch
            {
                enumType = default(T);
                return Enum.TryParse<T>(description, out enumType);
            }
        }
    }
}
