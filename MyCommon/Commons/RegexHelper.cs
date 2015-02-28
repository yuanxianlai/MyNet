using System;
using System.Collections.Generic;
using System.Linq;

using System.Text.RegularExpressions;
using System.Web;

namespace MyProject.Common {
    /// <summary>
    /// 常用验证
    /// </summary>
    public class RegexHelper {
        #region 正则表达式
        public static string Decimal = @"^[-]?\d+[.]?\d*$";
        public static string Int = "^[0-9]*$";
        public static string PhoneNum = @"\d{3,4}-\d{7,8}";
        public static string Fax = @"86-\d{2,3}-\d{7,8}";
        public static string Date = @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$";
        //获取IP的字符串 HttpContext.Current.Request.UserHostAddress
        public static string IP1 = @"^（（2[0-4]\d|25[0-5]|[01]?\d\d?）\.）{3}（2[0-4]\d|25[0-5]|[01]?\d\d?）$";
        public static string IP2 = @"^（（2[0-4]\d|25[0-5]|[01]?\d\d?）\.）{2}（（2[0-4]\d|25[0-5]|[01]?\d\d?|\*）\.）（2[0-4]\d|25[0-5]|[01]?\d\d?|\*）$";
        public static string Email = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        #endregion

        /// <summary>
        /// 检查是否存在，存在则返回
        /// </summary>
        /// <param name="Str">被检查字符串</param>
        /// <param name="MatchStr">正则表达式句</param>
        /// <returns></returns>
        public static bool IsMatch(string Str, string MatchStr) {
            return Regex.IsMatch(Str, MatchStr);
        }
    }
}
