using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System {
    /// <summary>
    /// 集合处理方法
    /// </summary>
    public static class StringHelper {
        #region 字符串集合互转
        /// <summary>
        /// 将字此符串转字符串数组
        /// </summary>
        /// <param name="Value">被转换字符串</param>
        /// <param name="SplitSymbol">分隔符号</param>
        /// <returns>分隔后数组</returns>
        public static string[] _ToArray(this string Value, string[] SplitSymbol) {
            return Value.Split(SplitSymbol, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 将字符串转字符串数组
        /// </summary>
        /// <param name="Value">被转换字符串</param>
        /// <param name="SplitSymbol">分隔符号</param>
        /// <returns>分隔后数组</returns>
        public static string[] _ToArray(this string Value, string SplitSymbol) {
            return Value._ToArray(new string[] { SplitSymbol });
        }

        /// <summary>
        /// 字符串数组转换成字符串
        /// </summary>
        /// <param name="Value">字符串数组</param>
        /// <param name="SplitSymbol">分隔符</param>
        /// <returns>转换后字符串</returns>
        public static string _ToString(this string[] Value, string SplitSymbol) {
            StringBuilder sb = new StringBuilder();
            foreach (var item in Value) {
                sb.AppendFormat("{0}{1}", item, SplitSymbol);
            }

            return sb.ToString();
        }
        #endregion

        #region 字符串转日期
        /// <summary>
        /// 将字符串转换成日期
        /// </summary>
        /// <param name="Value">字符串</param>
        /// <returns>日期</returns>
        public static DateTime _ToDateTime(this string Value) {
            try {
                return DateTime.Parse(Value);
            }
            catch {
                return DateTime.MinValue;
            }
        }
        /// <summary>
        /// 字符串转换成日期
        /// </summary>
        /// <param name="Value">字符串</param>
        /// <returns>日期</returns>
        public static DateTime _ToDateTimeNow(this string Value) {
            try {
                return DateTime.Parse(Value);
            }
            catch {
                return DateTime.Now;
            }
        }
        /// <summary>
        /// 字符串转换成日期
        /// </summary>
        /// <param name="Value">字符串</param>
        /// <returns>日期</returns>
        public static DateTime _ToDateNow(this string Value) {
            try {
                return DateTime.Parse(Value);
            }
            catch {
                return DateTime.Now.ToShortDateString()._ToDateTime();
            }
        }
        #endregion

        #region 字符串转数值类型
        /// <summary>
        /// 字符串转换成数字
        /// </summary>
        /// <param name="Value">字符串</param>
        /// <returns>数字</returns>
        public static int _ToInt32(this string Value) {
            try { return Convert.ToInt32(Value); }
            catch { return 0; }
        }

        /// <summary>
        /// 字符串转换成双精度数字
        /// </summary>
        /// <param name="Value">字符串</param>
        /// <returns>双精度数字</returns>
        public static double _ToDouble(this string Value) {
            try { return Convert.ToDouble(Value); }
            catch { return 0; }
        }

        /// <summary>
        /// 将此数据转换为 decimal：异常则返回0
        /// </summary>
        /// <param name="Value">字符串</param>
        /// <returns>数字</returns>
        public static decimal _ToDecimal(this object Value) {
            try { return Convert.ToDecimal(Value); }
            catch { return 0; }
        }
        #endregion

        #region 扩展Substring
        /// <summary>
        /// 字符串截取：从尾部开始截取
        /// </summary>
        /// <param name="Val">需要截取的字符串</param>
        /// <param name="EndLen">截取长度</param>
        /// <returns></returns>
        public static string _Substring(this string Val, int EndLen) {
            return Val.Substring(0, Val.Length - EndLen);
        }
        #endregion
    }
}
