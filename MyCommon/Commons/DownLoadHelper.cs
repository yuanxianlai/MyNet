using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web;

namespace MyProject.Common {
    /// <summary>
    /// 下载文件
    /// </summary>
    public class DownLoadHelper {
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="Buffur">字节数</param>
        /// <param name="DownLoadName">文件名称</param>
        /// <param name="Context">HTTP 请求的信息</param>
        /// <returns></returns>
        private static void DownLoadFile(byte[] Buffur, string DownLoadName, ControllerContext Context) {
            try {
                DownLoadName = HttpUtility.UrlEncode(System.Text.UTF8Encoding.UTF8.GetBytes(DownLoadName));
                HttpResponseBase Response = Context.HttpContext.Response;
                Response.Clear();
                Response.ClearContent();
                Response.Buffer = true;
                Response.Charset = "utf-8";
                Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
                Response.ContentType = "application/x-excel";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + DownLoadName);
                Response.AppendHeader("Content-Length", Buffur.Length.ToString());
                Response.BinaryWrite(Buffur);
                Response.End();
            }
            catch (Exception Ex) {
                LogHelper.Debug(Ex);
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="S">文件流</param>
        /// <param name="DownLoadName">下载文件名称</param>
        /// <param name="Context">HTTP 请求的信息</param>
        /// <returns></returns>
        public static void DownLoadFile(Stream S, string DownLoadName, ControllerContext Context) {
            try {
                byte[] buffur = new byte[S.Length];
                S.Read(buffur, 0, (int)S.Length);
                DownLoadFile(buffur, DownLoadName, Context);
            }
            catch (Exception Ex) {
                LogHelper.Debug(Ex);
            }
            finally {
                if (S != null) {
                    //关闭资源  
                    S.Close();
                }
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="PathStr">文件路径（含文件名称）</param>
        /// <param name="DownLoadName">重命名文件名（含后缀）</param>
        /// <param name="Context">http请求</param>
        /// <returns></returns>
        public static void DownLoadFile(string PathStr, string DownLoadName, ControllerContext Context) {
            FileStream fs = new FileStream(PathStr, FileMode.Open, FileAccess.Read);
            try {
                byte[] buffur = new byte[fs.Length];
                fs.Read(buffur, 0, (int)fs.Length);

                DownLoadFile(buffur, DownLoadName, Context);
            }
            catch (Exception Ex) {
                LogHelper.Debug(Ex);
            }
            finally {
                if (fs != null) {
                    //关闭资源  
                    fs.Close();
                }
            }
        }
    }
}
