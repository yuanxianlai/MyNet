using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Text;
namespace MyProject.Common {
    /// <summary>
    /// 日志类型实体类
    /// </summary>
    public class LogInfo {
        /// <summary>
        /// 日志类型
        /// </summary>
        public MyProject.Common.LogHelper.LogType LogType { get; set; }
        /// <summary>
        /// 日志日期
        /// </summary>
        public string DateTime { get; set; }
        /// <summary>
        /// 日志信息
        /// </summary>
        public string Info { get; set; }
        /// <summary>
        /// 日志代码
        /// </summary>
        public string CodeLine { get; set; }
        /// <summary>
        /// 程序运行时间
        /// </summary>
        public string RunTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }

    /// <summary>
    /// 日志记录
    /// </summary>
    public class LogHelper {
        /// <summary>
        /// 日志类型
        /// </summary>
        public enum LogType {
            /// <summary>
            /// Debug
            /// </summary>
            Debug,
            /// <summary>
            /// Info
            /// </summary>
            Info,
            /// <summary>
            /// Error
            /// </summary>
            Error,
            /// <summary>
            /// 自定义
            /// </summary>
            Custom
        }

        /// <summary>
        /// 日志目录
        /// </summary>
        public static string LogDirStr {
            get { return PathHelper.GetFullPath(ConfigHelper.LogsDir); }
        }
        #region 记录日志信息
        #region 公共方法
        /// <summary>
        /// Debug
        /// </summary>
        /// <param name="Message">Message</param>        
        /// <returns>日志信息</returns>
        public static LogInfo Debug(string Message) {
            LogInfo li = new LogInfo() {
                DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"),
                Info = Message.Replace("\n", "<br />"),
                LogType = LogType.Debug
            };

            Write_LogInfo(LogType.Debug, li);

            return li;
        }

        /// <summary>
        /// Debug
        /// </summary>
        /// <param name="Ex">Exception</param>        
        /// <returns>日志信息</returns>
        public static LogInfo Debug(Exception Ex) {
            Ex = InnerException(Ex);

            LogInfo li = new LogInfo() {
                DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"),
                Info = Ex.Message.Replace("\n", "<br />"),
                CodeLine = Ex.StackTrace,
                LogType = LogType.Debug
            };

            Write_LogInfo(LogType.Debug, li);

            return li;
        }

        /// <summary>
        /// Info
        /// </summary>
        /// <param name="Message">日志内容</param>
        /// <returns>日志信息</returns>
        public static LogInfo Info(string Message) {
            LogInfo li = new LogInfo() {
                DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"),
                Info = Message.Replace("\n", "<br />"),
                LogType = LogType.Info
            };
            Write_LogInfo(LogType.Info, li);

            return li;
        }

        /// <summary>
        /// Info
        /// </summary>
        /// <param name="Message">日志内容</param>
        /// <param name="RunTime">程序测量时间</param>        
        /// <returns>日志信息</returns>
        public static LogInfo Info(string Message, Stopwatch RunTime) {
            LogInfo li = new LogInfo() {
                DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"),
                Info = Message.Replace("\n", "<br />"),
                LogType = LogType.Info
            };

            if (RunTime != null) {
                li.RunTime = string.Format("{0}时{1}分{2}秒{3}毫秒", RunTime.Elapsed.Hours, RunTime.Elapsed.Minutes, RunTime.Elapsed.Seconds, RunTime.Elapsed.Milliseconds);
            }

            Write_LogInfo(LogType.Info, li);

            return li;
        }

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="Ex">Exception</param>        
        /// <returns>日志信息</returns>
        public static LogInfo Error(Exception Ex) {
            Ex = InnerException(Ex);

            LogInfo li = new LogInfo() {
                DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"),
                Info = Ex.Message.Replace("\n", "<br />"),
                CodeLine = Ex.StackTrace,
                LogType = LogType.Error
            };

            Write_LogInfo(LogType.Debug, li);

            return li;
        }

        /// <summary>
        /// Error
        /// </summary>
        /// <param name="Message">日志内容</param>        
        /// <returns>日志信息</returns>
        public static LogInfo Error(string Message) {
            LogInfo li = new LogInfo() {
                DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"),
                Info = Message.Replace("\n", "<br />"),
                LogType = LogType.Error
            };
            Write_LogInfo(LogType.Error, li);

            return li;
        }

        /// <summary>
        /// 写入日志（可自定义日志目录）
        /// </summary>
        /// <param name="CustomDir">目录名称</param>
        /// <param name="Message">日志内容</param>        
        /// <returns>日志信息</returns>
        public static LogInfo Custom(string CustomDir, string Message) {
            if (CustomDir == null || CustomDir.Equals("")) {
                throw new Exception("使用自定义日至目录，CustomDir 不能为空值！");
            }

            LogInfo li = new LogInfo() {
                DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"),
                Info = Message.Replace("\n", "<br />"),
                LogType = LogType.Custom
            };
            Write_LogInfo(CustomDir, li);

            return li;
        }

        /// <summary>
        /// 写入日志（可自定义日志目录）
        /// </summary>
        /// <param name="CustomDir">目录名称</param>
        /// <param name="Ex">Exception</param>        
        /// <returns>日志信息</returns>
        public static LogInfo Custom(string CustomDir, Exception Ex) {
            Ex = InnerException(Ex);

            if (CustomDir == null || CustomDir.Equals("")) {
                throw new Exception("使用自定义日至目录，CustomDir 不能为空值！");
            }

            LogInfo li = new LogInfo() {
                DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"),
                Info = Ex.Message.Replace("\n", "<br />"),
                CodeLine = Ex.StackTrace,
                LogType = LogType.Custom
            };
            Write_LogInfo(CustomDir, li);

            return li;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="LT">日志类型</param>
        /// <param name="LI">日志内容</param>
        private static void Write_LogInfo(LogType LT, LogInfo LI) {
            Write_LogInfo(LT.ToString(), LI);
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="DirName">日志目录名称</param>
        /// <param name="LI">日志内容</param>
        private static void Write_LogInfo(string DirName, LogInfo LI) {
            //日志文件名称
            //日志目录路劲
            //日志文件路径
            string filePath = Path.Combine(new string[] { 
                LogDirStr, 
                DirName, string.Format("Log_{0}.txt", 
                DateTime.Now.ToString("yyyy-MM-dd")) 
            });

            StringBuilder str = new StringBuilder();
            str.AppendFormat("{0}\t{1}{2}", LI.DateTime, LI.Info, Environment.NewLine);
            if (LI.CodeLine != null && !LI.CodeLine.Equals(""))
                str.AppendFormat("{0}{1}", LI.CodeLine, Environment.NewLine);
            if (LI.RunTime != null && !LI.RunTime.Equals(""))
                str.AppendFormat("   运行时间：{0}{1}", LI.RunTime, Environment.NewLine);

            str.Append(Environment.NewLine);
            FileHelper.FileWriteAppend(filePath, str.ToString());
        }

        /// <summary>
        /// 获取详细的异常信息
        /// </summary>
        /// <param name="Ex">异常对象</param>
        /// <returns>异常信息</returns>
        private static Exception InnerException(Exception Ex) {
            if (Ex.InnerException == null) {
                return Ex;
            }
            else {
                return InnerException(Ex.InnerException);
            }
        }
        #endregion
        #endregion

        #region 删除日志文件

        /// <summary>
        /// 删除日志文件
        /// </summary>
        /// <param name="LT">日志类型</param>
        /// <param name="LogFileName">日志文件名</param>
        /// <returns></returns>
        public static bool Del(LogType LT, string LogFileName) {
            string filePath = String.Format("{0}/{1}/{2}", LogDirStr, LT, LogFileName);

            //删除日志文件

            return false;
        }

        #endregion

        #region 读取日志信息

        /// <summary>
        /// 获取日志文件列表
        /// </summary>
        /// <param name="LT">日志类型</param>
        /// <returns>日志文件名集合</returns>
        public static List<string> LogsFileNameList(LogType LT) {
            List<string> lst = new List<string>();
            foreach (FileInfo item in FileHelper.DirFileList(Path.Combine(LogDirStr, LT.ToString()))) {
                lst.Add(item.Name);
            }

            return lst;
        }

        /// <summary>
        /// 获取日志信息
        /// </summary>
        /// <param name="LT">日志类型</param>
        /// <param name="LogFileName">日志文件名称</param>
        /// <param name="EnterType">换行符：1文本换行 2Web换行></param>
        /// <param name="DirName">日志目录名称（日志类型为自定义时需要需要赋值）</param>
        /// <returns>日志信息集合</returns>
        public static List<LogInfo> LogInfoList(LogType LT, string LogFileName, int EnterType, string DirName = null) {
            string dirName = LT.ToString();
            if (LT == LogType.Custom) {
                dirName = DirName;
                if (dirName == null || dirName.Equals(""))
                    throw new Exception(String.Format("日志类型设置为{0},DirName 不能为null或空值！", LT));
            }

            string filePath = Path.Combine(LogDirStr, dirName, LogFileName);
            List<LogInfo> lst = new List<LogInfo>();
            if (File.Exists(filePath)) {
                string[] strG = FileHelper.FileReadLines(filePath);

                int LogFirstLine = 0;
                LogInfo li = null;
                StringBuilder codeLine = new StringBuilder();
                foreach (string str in strG) {
                    if (!string.IsNullOrEmpty(str.Trim())) {
                        if (LogFirstLine == 0) {
                            li = new LogInfo();
                            lst.Add(li);
                            //日志信息
                            li.DateTime = str.Substring(0, 23).Trim();
                            if (EnterType == 1) {
                                li.Info = str.Substring(23).Trim().Replace("<br />", Environment.NewLine);
                            }
                            else if (EnterType == 2) {
                                li.Info = str.Substring(23).Trim();
                            }
                        }
                        else if (str.IndexOf("运行时间：") > -1) {
                            li.RunTime = str.Replace("运行时间：", string.Empty).Trim();
                        }
                        else {
                            codeLine.Append(str.Trim());

                            if (EnterType == 1) {
                                codeLine.Append(Environment.NewLine);
                            }
                            else if (EnterType == 2) {
                                codeLine.Append("<br />");
                            }

                        }

                        LogFirstLine++;
                    }
                    else {
                        li.CodeLine = codeLine.ToString();
                        codeLine.Clear();
                        LogFirstLine = 0;
                    }
                }
            }

            return lst.OrderBy(m => m.DateTime).ToList();
        }
        #endregion
    }
}
