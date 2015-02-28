using System.Data;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace MyProject.Common {
    /// <summary>
    /// JSON（对象转换JSON串）
    /// </summary>
    public class JsonHelper {
        #region Json与实体类互转
        /// <summary>
        /// 从一个对象信息生成Json串
        /// </summary>
        /// <param name="Obj">实体类</param>
        /// <returns>JSON串</returns>
        public static string JsonToObject(object Obj) {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(Obj.GetType());
            MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, Obj);
            byte[] dataBytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(dataBytes, 0, (int)stream.Length);
            return Encoding.UTF8.GetString(dataBytes);
        }

        /// <summary>
        /// 从一个Json串生成对象信息
        /// </summary>
        /// <param name="JsonStr">JSON串</param>
        /// <param name="Obj">实体类</param>
        /// <returns>对象</returns>
        public static object ObjectToJson(string JsonStr, object Obj) {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(Obj.GetType());
            MemoryStream mStream = new MemoryStream(Encoding.UTF8.GetBytes(JsonStr));
            return serializer.ReadObject(mStream);
        }
        #endregion

        #region Json与DataTable互转
        /// <summary>
        /// DataTable转Json数据
        /// </summary>
        /// <param name="dt">数据</param>
        /// <param name="RemoveTableName">是否将转换成的json数据包含在DataTable表名称中</param>
        /// <returns></returns>
        public static string DataTableToJson(DataTable dt, bool RemoveTableName = false) {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{");
            if (!RemoveTableName) {
                jsonBuilder.AppendFormat("\"{0}\":", dt.TableName);
            }
            jsonBuilder.Append("[");
            foreach (DataRow dr in dt.Rows) {
                jsonBuilder.Append("{");
                foreach (DataColumn dc in dt.Columns) {
                    jsonBuilder.AppendFormat("\"{0}\":\"{1}\",", dc.ColumnName, dr[dc.ColumnName]);
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("]}");
            return jsonBuilder.ToString();
        }
        #endregion
    }
}
