String.prototype.StrSubStrEnd = function (len) {
    /// <summary>
    /// 字符串截取：从尾部开始截取
    /// </summary>
    /// <param name="len">截取长度</param>
    /// <returns>出现次数</returns>
    if (this.length <= len) {
        return this;
    }
    else {
        this.substring(0, str.length - len);
    }
};
String.prototype.StrReplace = function (strOld, strNew) {
    /// <summary>
    /// 返回一个新字符串，其中当前实例中出现的所有指定字符串都替换为另一个指定的字符串。
    /// </summary>
    /// <param name="strOld">要被替换的字符串。</param>
    /// <param name="strNew">要替换出现的所有 oldValue 的字符串。</param>
    /// <returns>等效于当前字符串（除了 oldValue 的所有实例都已替换为 newValue 外）的字符串。</returns>
    eval("var regEx = /\s" + strOld + "/g");
    return this.replace(regEx, strNew);
}