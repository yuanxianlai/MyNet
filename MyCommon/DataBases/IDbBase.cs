using System.Data;
using System.Data.Common;

namespace MyProject.DataBase {
    /// <summary>
    /// 数据操作接口
    /// </summary>
    public interface IDbBase {
        #region 数据库连接与事务
        /// <summary>
        /// 打开数据库连接
        /// </summary>
        /// <param name="ConnStr">数据库连接字符串</param>
        void ConnOpen(string ConnStr);
        /// <summary>
        /// 开启事务
        /// </summary>
        void TranBegin();
        /// <summary>
        /// 提交事务
        /// </summary>
        void TranCommit();
        /// <summary>
        /// 回滚事务
        /// </summary>
        void TranRollBack();
        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        void ConnClose();
        #endregion
        #region 执行Sql
        /// <summary>
        /// 执行Sql,返回首行首列
        /// </summary>
        /// <param name="Sql">Sql语句</param>
        /// <returns></returns>
        object RunSqlRetScalar(string Sql);
        /// <summary>
        /// 执行Sql,返回id（用于对单表insert语句）
        /// </summary>
        /// <param name="Sql">Sql语句</param>
        /// <returns></returns>
        string InsertRetIdentity(string Sql);
        /// <summary>
        /// 执行Sql,返回影响行数
        /// </summary>
        /// <param name="Sql">Sql语句</param>
        /// <returns></returns>
        int RunSqlRetNum(string Sql);
        /// <summary>
        /// 执行Sql,查询DataSet（多条语句查询,使用;隔开）
        /// </summary>
        /// <param name="Sql">Sql语句</param>
        /// <returns></returns>
        DataSet RunSqlRetDS(string[] LstSql, string[] TableName);
        /// <summary>
        /// 执行Sql,查询DataTable(用于查询结果为单表语句)
        /// </summary>
        /// <param name="Sql">Sql语句</param>
        /// <returns></returns>
        DataTable RunSqlRetDT(string Sql, string TableName);
        /// <summary>
        /// 执行Sql事务,返回影响行数(允许sql)
        /// </summary>
        /// <param name="LstSql">Sql集合</param>
        /// <returns></returns>
        int RunSqlTranRetNum(string[] LstSql);
        /// <summary>
        /// 事务执行Sql,返回sql集合中的select语句查询结果
        /// </summary>
        /// <param name="LstSql">Sql集合</param>
        /// <returns></returns>
        DataSet RunSqlTranRetDS(string[] LstSql, string[] TableNames);
        #endregion
        #region 批量操作
        /// <summary>
        /// 批量数据写入
        /// </summary>
        /// <param name="Dt">需要写入的数据集</param>
        /// <param name="ConnStr">连接字符串</param>
        /// <param name="BatchSize">提交数据条数</param>
        void BatchInsert(DataTable Dt, int BatchSize = 5000);
        /// <summary>
        /// 批量数据修改
        /// </summary>
        /// <param name="Dt">需要修改的数据集</param>
        /// <param name="PrimaryKeyName">修改主键</param>
        /// <param name="BatchSize">提交数据条数</param>
        /// <returns></returns>
        int BatchUpdate(DataTable Dt, string PrimaryKeyName, int BatchSize = 5000);
        #endregion
        #region 存储过程
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="ProName">存储过程名称</param>
        /// <param name="Par">参数（数组）</param>
        /// <returns></returns>
        void RunProcedure(string ProName, DbParameter[] Par);
        #endregion
    }
}