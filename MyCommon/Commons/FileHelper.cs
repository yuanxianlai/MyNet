using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MyProject.Common
{
    /// <summary>
    /// 目录、文件操作  操作文件编码格式：UTF-8
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// 锁
        /// 用于多线程时，预防同事操作同一份文件
        /// 如果该变量被锁，其他
        /// </summary>
        private readonly static object LockStatus = new object();

        #region 目录操作
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="DirPathStr">目录物理路径(允许文件物理路径)</param>
        public static void DirCreate(string DirPathStr)
        {
            //判断是该路劲是否为文件物理路劲
            if (DirPathStr.LastIndexOf('.') > DirPathStr.LastIndexOf('\\'))
            {
                int index = DirPathStr.LastIndexOf('\\');
                DirPathStr = DirPathStr.Substring(0, index);
            }

            DirectoryInfo dir = new DirectoryInfo(DirPathStr);
            if(!dir.Exists)
            {
                dir.Create();
            }
        }


        /// <summary>
        /// 目录拷贝
        /// </summary>
        /// <param name="CopyDirPathStr">拷贝目录物理路劲</param>
        /// <param name="CopyToDirPathStr">拷贝至目录物理路劲</param>
        public static void DirCopy(string CopyDirPathStr, string CopyToDirPathStr)
        {
            DirectoryInfo source = new DirectoryInfo(CopyDirPathStr);
            DirectoryInfo target = new DirectoryInfo(CopyToDirPathStr);

            if(target.FullName.StartsWith(source.FullName, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new Exception("父目录不能拷贝到子目录！");
            }

            //检查是否存在目的目录  
            if(!Directory.Exists(CopyToDirPathStr))
            {
                Directory.CreateDirectory(CopyToDirPathStr);
            }

            //先来复制文件  
            DirectoryInfo directoryInfo = new DirectoryInfo(CopyDirPathStr);
            FileInfo[] files = directoryInfo.GetFiles();

            //复制所有文件  
            foreach(FileInfo file in files)
            {
                file.CopyTo(Path.Combine(CopyToDirPathStr, file.Name));
            }

            //最后复制目录  
            DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();
            foreach(DirectoryInfo dir in directoryInfoArray)
            {
                DirCopy(Path.Combine(CopyDirPathStr, dir.Name), Path.Combine(CopyToDirPathStr, dir.Name));
            }
        }


        /// <summary>
        /// 目录移动
        /// </summary>
        /// <param name="DirPathStr">原始目录物理路劲</param>
        /// <param name="MoveToDir">移动至目录物理路径</param>
        public static void DirMoveTo(string DirPathStr, string MoveToDir)
        {
            lock(LockStatus)
            {
                //检查是否存在目的目录  
                if(!Directory.Exists(MoveToDir))
                {
                    Directory.CreateDirectory(MoveToDir);
                }

                //先来移动文件  
                DirectoryInfo directoryInfo = new DirectoryInfo(DirPathStr);
                FileInfo[] files = directoryInfo.GetFiles();

                //移动所有文件  
                foreach(FileInfo file in files)
                {
                    //如果自身文件在运行，不能直接覆盖，需要重命名之后再移动  
                    if(File.Exists(Path.Combine(MoveToDir, file.Name)))
                    {
                        if(File.Exists(Path.Combine(MoveToDir, file.Name + ".bak")))
                        {
                            File.Delete(Path.Combine(MoveToDir, file.Name + ".bak"));
                        }
                        File.Move(Path.Combine(MoveToDir, file.Name), Path.Combine(MoveToDir, file.Name + ".bak"));
                    }

                    file.MoveTo(Path.Combine(MoveToDir, file.Name));
                }

                //最后移动目录
                DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();
                foreach(DirectoryInfo dir in directoryInfoArray)
                {
                    DirMoveTo(Path.Combine(DirPathStr, dir.Name), Path.Combine(MoveToDir, dir.Name));
                }

                directoryInfo.Delete();
            }
        }


        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="DirPathStr">目录物理路径(允许文件物理路径)</param>
        public static void DirDel(string DirPathStr)
        {
            lock(LockStatus)
            {
                if(!Directory.Exists(DirPathStr))
                {
                    Directory.Delete(DirPathStr);
                }
            }
        }


        /// <summary>
        /// 获取目录文件信息
        /// </summary>
        /// <param name="DirPathStr">目录物理路径(允许文件文件物理路径)</param>
        /// <returns>文件信息集合</returns>
        public static List<FileInfo> DirFileList(string DirPathStr)
        {
            //判断是该路劲是否为文件物理路劲
            if (DirPathStr.LastIndexOf('.') > DirPathStr.LastIndexOf('\\'))
            {
                int index = DirPathStr.LastIndexOf('/');
                DirPathStr = DirPathStr.Substring(0, index);
            }

            DirectoryInfo dir = new DirectoryInfo(DirPathStr);
            if(dir.Exists)
            {
                return dir.GetFiles().ToList();
            }
            else
            {
                return new List<FileInfo>();
            }
        }
        #endregion

        #region 文件操作
        #region 创建文件
        /// <summary>
        /// 检查文件是否存在
        /// </summary>
        /// <param name="FilePathStr"></param>
        /// <returns></returns>
        public static bool FileExists(string FilePathStr)
        {
            return File.Exists(FilePathStr);
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="FilePathStr">文件物理路劲</param>
        /// <param name="IsCoverFile">是否覆盖文件 true:覆盖  false:不覆盖</param>
        public static void FileCreate(string FilePathStr, bool IsCoverFile = false)
        {
            //获取目录地址
            //如果不存在需创建该目录
            DirCreate(FilePathStr);

            if(!File.Exists(FilePathStr) || IsCoverFile)
            {
                File.CreateText(FilePathStr).Close();
            }
        }
        #endregion

        #region 写入内容
        /// <summary>
        /// 写入内容
        /// </summary>
        /// <param name="FilePathStr">文件物理路劲</param>
        /// <param name="Contents">要写入的内容</param>
        public static void FileWrite(string FilePathStr, string Contents)
        {
            //获取目录地址
            //如果不存在需创建该目录
            DirCreate(FilePathStr);

            lock(LockStatus)
            {
                File.WriteAllText(FilePathStr, Contents);
            }
        }


        /// <summary>
        /// 追加写入内容
        /// </summary>
        /// <param name="FilePathStr">文件物理路劲</param>
        /// <param name="Contents">要追加写入的内容</param>
        public static void FileWriteAppend(string FilePathStr, string Contents)
        {
            lock(LockStatus)
            {
                //获取目录地址
                //如果不存在需创建该目录
                DirCreate(FilePathStr);

                File.AppendAllText(FilePathStr, Contents);
            }
        }
        #endregion

        #region 读取文件内容
        /// <summary>
        /// 读取文件所有内容
        /// </summary>
        /// <param name="FilePathStr">文件物理路劲</param>
        /// <returns>文件内容</returns>
        public static string FileRead(string FilePathStr)
        {
            if(File.Exists(FilePathStr))
            {
                lock(LockStatus)
                {
                    return File.ReadAllText(FilePathStr, Encoding.UTF8);
                }
            }
            else
            {
                return "";
            }
        }


        /// <summary>
        /// 读取文件内容
        /// </summary>
        /// <param name="FilePathStr">文件物理路劲</param>
        /// <returns>文件内容</returns>
        public static string[] FileReadLines(string FilePathStr)
        {
            if(File.Exists(FilePathStr))
            {
                lock(LockStatus)
                {
                    return File.ReadAllLines(FilePathStr, Encoding.UTF8);
                }
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 删除文件
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="FilePathStr">文件物理地址</param>
        /// <returns></returns>
        public static void FileDel(string FilePathStr)
        {
            lock(LockStatus)
            {
                if(File.Exists(FilePathStr))
                {
                    File.Delete(FilePathStr);
                }
            }
        }
        #endregion
        #endregion
    }
}