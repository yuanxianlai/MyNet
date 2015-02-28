using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace System {
    /// <summary>
    ///  日期扩展
    /// </summary>
    public static class DateTimeHelper {
        /// <summary>
        /// 日期格式化字符串
        /// </summary>
        private static string DateFromatStr = "yyyy-MM-dd";
        private static string TimeFromatStr = "HH:mm:ss";

        #region DateTime?
        /// <summary>
        /// 将 DateTime? 转换为 DateTime 类型：如果为null则返回最小日期
        /// </summary>
        /// <param name="Value">被转换日期</param>
        /// <returns></returns>
        public static DateTime _ToDateTime(this DateTime? Value) {
            if (Value != null) {
                return Value.Value;
            }
            else {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// 将 DateTime? 转换为日期常用格式（yyyy-MM-dd HH:mm:ss）
        /// </summary>
        /// <param name="Value">被转换日期</param>
        /// <returns></returns>
        public static string _ToString(this DateTime? Value) {
            return Value._ToDateTime().ToString(string.Format("{0} {1}", DateFromatStr, TimeFromatStr));
        }

        /// <summary>
        /// 将 DateTime? 转换为日期常用格式（yyyy-MM-dd）
        /// </summary>
        /// <param name="Value">被转换日期</param>
        /// <returns></returns>
        public static string _ToShortString(this DateTime? Value) {
            return Value._ToDateTime().ToString(DateFromatStr);
        }
        #endregion

        #region DateTime
        /// <summary>
        /// 转换常用日期格式(yyyy-MM-dd HH:mm:ss)
        /// </summary>
        /// <param name="Value">被转换日期</param>
        /// <returns></returns>
        public static string _ToString(this DateTime Value) {
            return Value.ToString(string.Format("{0} {1}", DateFromatStr, TimeFromatStr));
        }

        /// <summary>
        /// 转换常用日期格式(yyyy-MM-dd)
        /// </summary>
        /// <param name="Value">被转换日期</param>
        /// <returns></returns>
        public static string _ToShortString(this DateTime Value) {
            return Value.ToString(DateFromatStr);
        }

        /// <summary>
        /// 获取英文星期
        /// </summary>
        /// <param name="Value">被转换日期</param>
        /// <returns></returns>
        public static string _GetWeekEN(this DateTime Value) {
            Calendar myCal = CultureInfo.InvariantCulture.Calendar;
            return myCal.GetDayOfWeek(Value).ToString();
        }

        /// <summary>
        /// 获取中文星期
        /// </summary>
        /// <param name="Value">被转换日期</param>
        /// <returns></returns>
        public static string _GetWeekZH(this DateTime Value) {
            switch (_GetWeekEN(Value)) {
                case "Sunday":
                    return "星期日";
                case "Monday":
                    return "星期一";
                case "Tuesday":
                    return "星期二";
                case "Wednesday":
                    return "星期三";
                case "Thursday":
                    return "星期四";
                case "Friday":
                    return "星期五";
                case "Saturday":
                    return "星期六";
            }
            return null;
        }
        #endregion
    }
}
