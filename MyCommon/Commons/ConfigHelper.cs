using System;
using System.Collections.Generic;
using System.Linq;

using System.Configuration;

namespace MyProject.Common {
    /// <summary>
    /// 系统配置信息
    /// </summary>
    public class ConfigHelper {
        #region 系统常用参数（配置信息）
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static string ConnStr {
            get { return GetConn("ConnStr"); }
        }

        /// <summary>
        /// 日志目录
        /// </summary>
        public static string LogsDir {
            get {
                string ret = GetApp("LogDir");
                return ret ?? "LogDir";
            }
            set { SetApp("LogDir", value); }
        }

        /// <summary>
        /// 上传文件目录
        /// </summary>
        public static string UploadDir {
            get {
                string ret = GetApp("UploadDir");
                return ret ?? "UploadDir";
            }
            set { SetApp("UploadDir", value); }
        }

        /// <summary>
        /// 临时文件目录
        /// </summary>
        public static string FreeDir {
            get {
                string ret = GetApp("FreeDir");
                return ret ?? "FreeDir";
            }
            set { SetApp("FreeDir", value); }
        }

        /// <summary>
        /// 分页记录数
        /// </summary>
        public static int PageSize {
            get {
                string ret = GetApp("PageSize");
                return ret == null ? 15 : int.Parse(ret);
            }
        }

        /// <summary>
        /// 初始化密码
        /// </summary>
        public static string IniPwd {
            get {
                string ret = GetApp("IniPwd");
                return ret == null ? "123456" : ret;
            }
        }
        #endregion

        #region 私有方法（获取配置信息，修改配置信息）
        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="Name">配置信息节点名称</param>
        /// <returns></returns>
        private static string GetApp(string Name) {
            if (ConfigurationManager.AppSettings[Name] == null) {
                return null;
            }
            else {
                return ConfigurationManager.AppSettings[Name];
            }
        }

        /// <summary>
        /// 修改配置信息
        /// </summary>
        /// <param name="Name">配置信息节点名称</param>
        /// <param name="Value"></param>
        private static void SetApp(string Name, string Value) {
            Configuration cga = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cga.AppSettings.Settings[Name].Value = Value;
            cga.Save();
        }

        /// <summary>
        /// 获取数据库连接字符串
        /// </summary>
        /// <param name="Name">数据库连接节点名称</param>
        /// <returns></returns>
        private static string GetConn(string Name) {
            try {
                return ConfigurationManager.ConnectionStrings[Name].ToString();
            }
            catch { return ""; }
        }
        #endregion
    }
}
