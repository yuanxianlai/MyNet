using System;
using System.Collections.Generic;
using System.Linq;

namespace MyProject.Common
{
    /// <summary>
    /// 路劲 -- 物理路径
    /// </summary>
    public class PathHelper
    {
        /// <summary>
        /// 获得系统所在物理路径
        /// </summary>
        /// <param name="RelativePath">物理目录</param>
        /// <returns>系统物理路径</returns>
        public static string GetFullPath(string RelativePath = "")
        {
            string AppDir = AppDomain.CurrentDomain.BaseDirectory;
            if (RelativePath.IndexOf(":") < 0)
            {
                string str = RelativePath.Replace("..\\", "");
                if (str != RelativePath)
                {
                    int Num = (RelativePath.Length - str.Length) / ("..\\").Length + 1;
                    for (int i = 0; i < Num; i++)
                    {
                        AppDir = AppDir.Substring(0, AppDir.LastIndexOf("\\"));
                    }
                    str = "\\" + str;
                }
                return AppDir + str;
            }
            else
            {
                return null;
            }
        }
    }
}
