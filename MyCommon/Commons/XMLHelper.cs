using System;
using System.Data;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace MyProject.Common {
    /// <summary>
    /// 该类主要用于XML的处理
    /// 需要首先实例化，才能使用相关方法
    /// </summary>
    public class XMLHelper {
        #region 对象的序列化和反序列化(Xml格式) XmlSerialize/XmlDeserialize

        /// <summary>
        /// 将指定类型的对象序列化为XML
        /// </summary>
        /// <param name="Obj">事体对象</param>
        /// <param name="Extra"></param>
        /// <returns>XML文本</returns>
        public static string ObjectToXml(object Obj, params Type[] Extra) {
            XmlSerializer ser = new XmlSerializer(Obj.GetType(), Extra);
            MemoryStream mem = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mem, Encoding.Default);
            ser.Serialize(writer, Obj);
            string strtmp = Encoding.Default.GetString(mem.ToArray());
            return strtmp;
        }

        /// <summary>
        /// 将xml反序列化为对象
        /// </summary>
        /// <param name="xml">xml字符串</param>
        /// <param name="type">指定类型</param>
        /// <param name="Extra"></param>
        /// <returns></returns>
        /// <example>ArrayList arrExample = (ArrayList)XMLHelper.XmlDeserialize(value, typeof(ArrayList), typeof([ClassName]));</example>
        public static object XmlToObject(string xml, Type type, params Type[] Extra) {
            XmlSerializer serializer = new XmlSerializer(type, Extra);
            MemoryStream mem;
            try {
                mem = new MemoryStream(Encoding.Default.GetBytes(xml));
            }
            catch {
                return null;
            }
            return serializer.Deserialize(mem);
        }

        /// <summary>
        /// 将DataTable序列化为xml
        /// </summary>
        /// <param name="Dt">Datatable</param>
        /// <param name="TableName">设置DataTable的名称</param>
        /// <returns>xml文本</returns>
        public static string DataTableToXml(DataTable Dt, string TableName) {
            Dt.TableName = TableName;
            StringBuilder strbuilder = new StringBuilder();
            StringWriter writer = new StringWriter(strbuilder);
            Dt.WriteXml(writer, System.Data.XmlWriteMode.IgnoreSchema);
            return strbuilder.ToString();
        }

        /// <summary>
        /// 将xml反序列化为DataTable
        /// </summary>
        /// <param name="xml">xml文本</param>
        /// <returns>DataTable</returns>
        public static DataTable XmlToDataTable(string xml) {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();

            try {
                MemoryStream mem = new MemoryStream(Encoding.UTF8.GetBytes(xml));
                ds.ReadXml(mem);
                if (ds.Tables.Count > 0)
                    dt = ds.Tables[0];
            }
            catch {
                return null;
            }
            return dt;
        }
        #endregion


        #region 树状目录的解析和绑定

        /// <summary>
        /// 将xml数据绑定到树状目录
        /// 如果发生异常，则不会绑定
        /// </summary>
        /// <param name="Document">单个xml节点</param>
        /// <param name="TreeView">树状目录[TreeView]</param>
        /// <param name="LstPermission">允许访问树节点</param>
        public static void TreeViewParse(XmlNode Document, TreeView TreeView, List<string> LstPermission) {
            if (Document.Name != "TreeView")
                return;

            foreach (XmlNode node in Document.ChildNodes) {
                if (node.Name == "treenode") {
                    if (LstPermission.IndexOf(node.Attributes["PermissionCode"].Value) >= 0 || node.Attributes["PermissionCode"].Value == "")
                        TreeView.Nodes.Add(NodesParse(node, LstPermission));
                }
            }
        }

        /// <summary>
        /// 将xml数据绑定到树状目录
        /// </summary>
        /// <param name="Document">单个xml节点</param>
        /// <param name="TreeView">树状目录[TreeView]</param>
        public static void TreeViewParse(XmlNode Document, TreeView TreeView) {
            if (Document.Name != "TreeView")
                return;
            foreach (XmlNode node in Document.ChildNodes) {
                if (node.Name == "treenode")
                    TreeView.Nodes.Add(NodesParse(node));
            }
        }

        /// <summary>
        /// 根据传入的xml节点，设置树节点的各项属性和子节点
        /// 如果获取XmlNode节点的值发生异常，则该节点不会添加到树中
        /// </summary>
        /// <param name="Document">xml节点[XmlNode]</param>
        /// <returns>TreeNode</returns>
        private static TreeNode NodesParse(XmlNode Document) {
            TreeNode node = new TreeNode();

            try {
                node.Text = Document.Attributes["text"].Value;
                node.NavigateUrl = Document.Attributes["NavigateUrl"].Value;
                node.ToolTip = Document.Attributes["Description"].Value;
                node.Value = Document.Attributes["PermissionCode"].Value;
                if (node.NavigateUrl == "")
                    node.SelectAction = TreeNodeSelectAction.None;

                foreach (XmlNode node2 in Document.ChildNodes) {
                    TreeNode node3 = NodesParse(node2);
                    if (node3 != null) {
                        node.ChildNodes.Add(node3);
                    }
                }
            }
            catch {
                return null;
            }
            return node;
        }

        /// <summary>
        /// 根据传入的xml节点和允许访问节点列表，设置树节点的各项属性和子节点
        /// 如果获取XmlNode节点的值发生异常，则该节点不会添加到树中
        /// </summary>
        /// <param name="Document">xml节点[XmlNode]</param>
        /// <param name="LstPermission">允许访问列表</param>
        /// <returns>TreeNode</returns>
        private static TreeNode NodesParse(XmlNode Document, List<string> LstPermission) {
            TreeNode node = new TreeNode();
            try {
                node.NavigateUrl = Document.Attributes["NavigateUrl"].Value;
                node.Text = Document.Attributes["text"].Value;
                node.ToolTip = Document.Attributes["Description"].Value;
                node.Value = Document.Attributes["PermissionCode"].Value;
                if (node.NavigateUrl == "")
                    node.SelectAction = TreeNodeSelectAction.None;

                foreach (XmlNode node2 in Document.ChildNodes) {
                    TreeNode node3 = NodesParse(node2);
                    if (node3 != null) {
                        if (LstPermission.IndexOf(node2.Attributes["PermissionCode"].Value) >= 0 || node2.Attributes["PermissionCode"].Value == "")
                            node.ChildNodes.Add(node3);
                    }
                }
            }
            catch {
                return null;
            }
            return node;
        }
        #endregion

        /// <summary>
        /// 获取xml文件中指定节点的指定子节点的值
        /// </summary>
        /// <param name="FilePath">xml文件</param>
        /// <param name="FindNode">指定节点</param>
        /// <param name="strKeyName">指定节点的指定子节点</param>
        /// <returns>节点值</returns>
        public static string GetKeyValue(string FilePath, string FindNode, string strKeyName) {
            string _strKeyValue = "";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(FilePath);
            XmlNodeList nodes = xmlDoc.SelectNodes(FindNode);
            foreach (XmlNode node in nodes) {
                foreach (XmlNode childNode in node.ChildNodes) {
                    if (childNode.Name == strKeyName) {
                        _strKeyValue = childNode.InnerText;
                    }
                }
            }
            return _strKeyValue;
        }


        /// <summary>
        /// 获取xml文本
        /// </summary>
        /// <param name="FilePath">xml文件</param>
        /// <returns></returns>
        public static string GetXmlStr(string FilePath) {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(FilePath);
            return xmlDoc.InnerXml;
        }

        /// <summary>
        /// 获取xml文本
        /// </summary>
        /// <param name="FilePath">xml文件</param>
        /// <param name="NodeStr">xml节点</param>
        /// <returns></returns>
        public static string GetXmlStr(string FilePath, string NodeStr) {
            XmlDocument xmlDoc = new XmlDocument();
            try {
                xmlDoc.Load(FilePath);

                return xmlDoc.SelectSingleNode(NodeStr).OuterXml;
            }
            catch {
                return null;
            }
        }

        #region 其他XML序列化方法

        private static void XmlSerializeInternal(Stream stream, object o, Encoding encoding) {
            if (o == null)
                throw new ArgumentNullException("o");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            XmlSerializer serializer = new XmlSerializer(o.GetType());

            XmlWriterSettings settings = new XmlWriterSettings() {
                Indent = true,
                NewLineChars = Environment.NewLine,
                Encoding = encoding,
                IndentChars = "    "
            };

            using (XmlWriter writer = XmlWriter.Create(stream, settings)) {
                serializer.Serialize(writer, o);
                writer.Close();
            }
        }

        /// <summary>
        /// 将一个对象序列化为XML字符串
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>序列化产生的XML字符串</returns>
        public static string XmlSerialize(object o, Encoding encoding) {
            using (MemoryStream stream = new MemoryStream()) {
                XmlSerializeInternal(stream, o, encoding);

                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream, encoding)) {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// 将一个对象按XML序列化的方式写入到一个文件
        /// </summary>
        /// <param name="o">要序列化的对象</param>
        /// <param name="path">保存文件路径</param>
        /// <param name="encoding">编码方式</param>
        public static void XmlSerializeToFile(object o, string path, Encoding encoding) {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write)) {
                XmlSerializeInternal(file, o, encoding);
            }
        }

        /// <summary>
        /// 从XML字符串中反序列化对象
        /// </summary>
        /// <typeparam name="T">结果对象类型</typeparam>
        /// <param name="s">包含对象的XML字符串</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>反序列化得到的对象</returns>
        public static T XmlDeserialize<T>(string s, Encoding encoding) {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentNullException("s");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            XmlSerializer mySerializer = new XmlSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(encoding.GetBytes(s))) {
                using (StreamReader sr = new StreamReader(ms, encoding)) {
                    return (T)mySerializer.Deserialize(sr);
                }
            }
        }

        /// <summary>
        /// 读入一个文件，并按XML的方式反序列化对象。
        /// </summary>
        /// <typeparam name="T">结果对象类型</typeparam>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>反序列化得到的对象</returns>
        public static T XmlDeserializeFromFile<T>(string path, Encoding encoding) {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            string xml = File.ReadAllText(path, encoding);
            return XmlDeserialize<T>(xml, encoding);
        }

        #endregion

        #region Custom
        /// <summary>
        /// 获取xml文件中指定节点的指定子节点的属性值
        /// </summary>
        /// <param name="FilePath">xml文件</param>
        /// <param name="FindNode">指定节点</param>
        /// <returns></returns>
        public static XmlNodeList GetXmlNodeList(string FilePath, string FindNode) {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(FilePath);
            return xmlDoc.SelectNodes(FindNode);
        }
        #endregion
    }
}