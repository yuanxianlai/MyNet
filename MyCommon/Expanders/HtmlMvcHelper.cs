using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace System.Web.Mvc.Html {
    /// <summary>
    /// Asp.Net Mvc  控件集成
    /// </summary>
    public static class HtmlMvcHelper {
        #region 常用函数

        #endregion

        #region Email
        /// <summary>
        /// 
        /// </summary>
        private static string OldStr = "type=\"text\"", NewStr = "type=\"Email\"";
        public static MvcHtmlString Email(this HtmlHelper htmlHelper, string name) {
            string ret = InputExtensions.TextBox(htmlHelper, name).ToHtmlString().Replace(OldStr, NewStr);
            return new MvcHtmlString(ret);
        }
        public static MvcHtmlString Email(this HtmlHelper htmlHelper, string name, object value) {
            string ret = InputExtensions.TextBox(htmlHelper, name, value).ToHtmlString().Replace(OldStr, NewStr);
            return new MvcHtmlString(ret);
        }
        public static MvcHtmlString Email(this HtmlHelper htmlHelper, string name, object value, IDictionary<string, object> htmlAttributes) {
            string ret = InputExtensions.TextBox(htmlHelper, name, value, htmlAttributes).ToHtmlString().Replace(OldStr, NewStr);
            return new MvcHtmlString(ret);
        }
        public static MvcHtmlString Email(this HtmlHelper htmlHelper, string name, object value, object htmlAttributes) {
            string ret = InputExtensions.TextBox(htmlHelper, name, value, htmlAttributes).ToHtmlString().Replace(OldStr, NewStr);
            return new MvcHtmlString(ret);
        }
        public static MvcHtmlString EmailFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression) {
            string ret = InputExtensions.TextBoxFor(htmlHelper, expression).ToHtmlString().Replace(OldStr, NewStr);
            return new MvcHtmlString(ret);
        }
        public static MvcHtmlString EmailFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes) {
            string ret = InputExtensions.TextBoxFor(htmlHelper, expression, htmlAttributes).ToHtmlString().Replace(OldStr, NewStr);
            return new MvcHtmlString(ret);
        }
        public static MvcHtmlString EmailFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes) {
            string ret = InputExtensions.TextBoxFor(htmlHelper, expression, htmlAttributes).ToHtmlString().Replace(OldStr, NewStr);
            return new MvcHtmlString(ret);
        }
        #endregion
    }
}