using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MyProject.Common {
    /// <summary>
    /// 操作 Enum
    /// </summary>
    public class EnumHelper {
        /// <summary>
        /// 获取enum选择列表
        /// </summary>
        /// <typeparam name="T">enum对象</typeparam>
        /// <returns>enum选择列表</returns>
        public static List<SelectListItem> Get_EnumSelectListItem<T>() {
            List<SelectListItem> ret = new List<SelectListItem>();
            foreach (T item in (T[])Enum.GetValues(typeof(T))) {
                ret.Add(new SelectListItem() { Text = item.ToString(), Value = Convert.ToInt32(item).ToString() });
            }

            return ret;
        }

        /// <summary>
        /// 获取emun值对应描述
        /// </summary>
        /// <typeparam name="T">enum</typeparam>
        /// <param name="Value">enum值</param>
        /// <returns>enum描述</returns>
        public static string Get_Enum<T>(int Value) {
            foreach (T item in (T[])Enum.GetValues(typeof(T))) {
                if (Convert.ToInt32(item) == Value) {
                    return item.ToString();
                }
            }
            return "未知";
        }
    }
}