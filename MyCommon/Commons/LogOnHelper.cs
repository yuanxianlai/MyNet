using System;
using System.Collections.Generic;
using System.Linq;

using System.Web;
using System.Web.Security;
using System.Web.Script.Serialization;

namespace MyProject {
    /// <summary>
    /// 用户实体
    /// </summary>
    public class LogOnInfo {
        /// <summary>
        /// 登陆用户ID
        /// </summary>
        public string LogOnID { get; set; }
        /// <summary>
        /// 登陆用户名称
        /// </summary>
        public string LogOnName { get; set; }
        /// <summary>
        /// 登陆用户类型
        /// </summary>
        public int LogOnType { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        public string LogOnPwd { get; set; }
    }

    /// <summary>
    /// 用户登陆（web）
    /// 存储登陆用户信息
    /// 获取登陆用户信息
    /// </summary>
    public class LogOnHelper {
        /// <summary>
        /// 获取用户登陆信息
        /// </summary>
        public static LogOnInfo GetLogOnInfo {
            get {
                string json = HttpContext.Current.User.Identity.IsAuthenticated ? HttpContext.Current.User.Identity.Name : null;
                return json != null ? new JavaScriptSerializer().Deserialize<LogOnInfo>(json) : null;
            }
        }

        /// <summary>
        /// 获取用户IP地址
        /// </summary>
        public static string GetLogOnIPAddress {
            get {
                return HttpContext.Current.Request.UserHostAddress;
            }
        }

        /// <summary>
        /// 创建登陆用户的票据信息
        /// </summary>
        /// <param name="Info">登陆用户信息</param>
        /// <returns></returns>
        public static HttpCookie SetLogOnInfo(LogOnInfo Info) {
            string info = new JavaScriptSerializer().Serialize(Info);

            FormsAuthenticationTicket tk = new FormsAuthenticationTicket(1,
                       info,
                       DateTime.Now,
                       DateTime.Now.AddDays(1),
                       true,
                       "",
                       FormsAuthentication.FormsCookiePath
                       );

            string key = FormsAuthentication.Encrypt(tk); //得到加密后的身份验证票字串 

            HttpCookie ck = new HttpCookie(System.Web.Security.FormsAuthentication.FormsCookieName, key) {
                /*这句话在部署网站后有用，此为关系到同一个域名下面的多个站点是否能共享Cookie*/
                Domain = System.Web.Security.FormsAuthentication.CookieDomain
            };
            //Response.Cookies.Add(ck);

            FormsAuthentication.SetAuthCookie(info, false);

            return ck;
        }

        /// <summary>
        /// 清楚登录信息
        /// </summary>
        public static void ClearLogOnInfo() {
            //FrameWorkPermission.UserOnlineList.Remove(Common.Get_UserID);
            //写退出日志
            //EventMessage.EventWriteDB(2, "用户退出", Common.Get_UserID);
            //退出操作
            System.Web.Security.FormsAuthentication.SignOut();
        }
    }
}
