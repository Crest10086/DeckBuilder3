﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCG;

namespace MyTools
{
    /// <summary>
    /// ini文件类
    /// </summary>
    public class IniFile
    {
        public string FileName { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="aFileName">Ini文件路径</param>
        public IniFile(string aFileName = "") => FileName = aFileName;       

        /// <summary>
        /// [扩展]读Int数值
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="name">键</param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public int ReadInt(string section, string name, int def)
        {
            return NativeMethods.GetPrivateProfileInt(section, name, def, this.FileName);
        }

        /// <summary>
        /// [扩展]读取string字符串
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="name">键</param>
        /// <param name="def">默认值</param>
        /// <returns></returns>
        public string ReadString(string section, string name, string def)
        {
            StringBuilder vRetSb = new StringBuilder(2048);
            NativeMethods.GetPrivateProfileString(section, name, def, vRetSb, 2048, this.FileName);
            return vRetSb.ToString();
        }

        /// <summary>
        /// [扩展]写入Int数值，如果不存在 节-键，则会自动创建
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="name">键</param>
        /// <param name="Ival">写入值</param>
        public void WriteInt(string section, string name, int Ival)
        {
            NativeMethods.WritePrivateProfileString(section, name, Ival.ToString(), this.FileName);
        }

        /// <summary>
        /// [扩展]写入String字符串，如果不存在 节-键，则会自动创建
        /// </summary>
        /// <param name="section">节</param>
        /// <param name="name">键</param>
        /// <param name="strVal">写入值</param>
        public void WriteString(string section, string name, string strVal)
        {
            NativeMethods.WritePrivateProfileString(section, name, strVal, this.FileName);
        }

        /// <summary>
        /// 删除指定的 节
        /// </summary>
        /// <param name="section"></param>
        public void DeleteSection(string section)
        {
            NativeMethods.WritePrivateProfileString(section, null, null, this.FileName);
        }

        /// <summary>
        /// 删除全部 节
        /// </summary>
        public void DeleteAllSection()
        {
            NativeMethods.WritePrivateProfileString(null, null, null, this.FileName);
        }

        /// <summary>
        /// 读取指定 节-键 的值
        /// </summary>
        /// <param name="section"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public string IniReadValue(string section, string name)
        {
            StringBuilder strSb = new StringBuilder(256);
            NativeMethods.GetPrivateProfileString(section, name, "", strSb, 256, this.FileName);
            return strSb.ToString();
        }

        /// <summary>
        /// 写入指定值，如果不存在 节-键，则会自动创建
        /// </summary>
        /// <param name="section"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void IniWriteValue(string section, string name, string value)
        {
            NativeMethods.WritePrivateProfileString(section, name, value, this.FileName);
        }
    }
}
