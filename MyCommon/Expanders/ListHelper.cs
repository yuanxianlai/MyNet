using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MyProject.Common;
namespace System {
    /// <summary>
    /// 扩展类
    /// </summary>
    public static class ListHelper {
        /// <summary>
        /// 将字符串集合转换包含分隔的符字符串
        /// </summary>
        /// <param name="Lst">字符串集合</param>
        /// <param name="SplitSymbol">分隔符</param>
        /// <returns>包含分隔的符字符串</returns>
        public static string _ToString(this List<string> Lst, string SplitSymbol) {
            if (SplitSymbol == null && SplitSymbol.Equals(""))
                throw new Exception("分隔符不能为null或空值！");

            StringBuilder sql = new StringBuilder();
            foreach (string str in Lst) {
                sql.Append(str + SplitSymbol);
            }
            return sql.ToString();
        }

        /// <summary>
        /// 将字符串集合转换字符串数组
        /// </summary>
        /// <param name="Lst">字符串集合</param>
        /// <returns></returns>
        public static string[] _ToString(this List<string> Lst) {
            string[] ret = new string[Lst.Count];
            for (int i = 0; i < ret.Length; i++) {
                ret[i] = Lst[i];
            }
            return ret;
        }

        /// <summary>
        /// 将包含分隔的符字符串转换字符串集合
        /// </summary>
        /// <param name="Str">包含分隔的符字符串</param>
        /// <param name="SplitSymbol">分隔符</param>
        /// <returns>字符串集合</returns>
        public static List<string> _ToList(this string Str, string SplitSymbol) {
            if (SplitSymbol == null && SplitSymbol.Equals(""))
                throw new Exception("分隔符不能为null或空值！");
            List<string> ret = new List<string>();
            foreach (string str in Str.Split(new string[] { SplitSymbol }, StringSplitOptions.RemoveEmptyEntries)) {
                ret.Add(str);
            }

            return ret;
        }

        /// <summary>
        /// 集合转二维表
        /// </summary>
        /// <typeparam name="T">集合数据类型</typeparam>
        /// <param name="Value">集合</param>
        /// <param name="Lst">表头</param>
        /// <returns></returns>
        public static DataTable _ToDataTable<T>(this List<T> Value, DataTable Dt) {
            foreach (T item in Value) {
                DataRow dr = Dt.NewRow();
                foreach (System.Reflection.PropertyInfo p in item.GetType().GetProperties()) {
                    dr[p.Name] = p.GetValue(item, null);
                }
                Dt.Rows.Add(dr);
            }
            return Dt;
        }
    }
}
