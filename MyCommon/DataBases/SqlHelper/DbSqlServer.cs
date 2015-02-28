using MyProject.Common;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace MyProject.DataBase {
    /// <summary>
    /// Sqlserver 数据库访问
    /// </summary>
    public class DbSqlServer : IDbBase {
        #region   全局变量
        private SqlConnection Conn = null;
        private SqlCommand Comm = null;
        private SqlTransaction Tran = null;
        private SqlDataAdapter Da = null;
        #endregion

        #region 接口实现
        public void ConnOpen(string ConnStr = null) {
            if (ConnStr == null || ConnStr.Equals("")) {
                ConnStr = ConfigHelper.ConnStr;
            }
            Conn = new SqlConnection(ConnStr);
            Conn.Open();

            Comm = new SqlCommand() {
                Connection = Conn
            };
        }

        public void TranBegin() {
            if (Tran == null) {
                Comm.Transaction = Tran = Conn.BeginTransaction();
            }
        }

        public void TranCommit() {
            Tran.Commit();
        }

        public void TranRollBack() {
            Tran.Rollback();
        }

        public void ConnClose() {
            if (Conn.State != ConnectionState.Closed) {
                Conn.Close();
            }
        }

        public object RunSqlRetScalar(string Sql) {
            Comm.CommandType = CommandType.Text;
            Comm.CommandText = Sql;
            try {
                return Comm.ExecuteScalar();
            }
            catch (SqlException Ex) {
                ConnClose();
                LogHelper.Debug(Sql);
                throw Ex;
            }
        }

        public string InsertRetIdentity(string Sql) {
            //拼接，获取自增字段值的sql语句
            if (Sql.LastIndexOf(";") == Sql.Length - 1)
                Sql += ";";
            Sql += "select @@identity;";

            return RunSqlRetScalar(Sql).ToString();
        }

        public int RunSqlRetNum(string Sql) {
            Comm.CommandType = CommandType.Text;
            Comm.CommandText = Sql;
            try {
                return Comm.ExecuteNonQuery();
            }
            catch (SqlException Ex) {
                ConnClose();
                LogHelper.Debug(Sql);
                throw Ex;
            }
        }

        public DataSet RunSqlRetDS(string[] LstSql, string[] TableNames) {
            using (SqlDataAdapter da = new SqlDataAdapter(LstSql._ToString(";"), Conn)) {
                DataSet ds = new DataSet();
                da.Fill(ds);
                for (int i = 0; i < TableNames.Length; i++) {
                    if (!string.IsNullOrEmpty(TableNames[i])) {
                        ds.Tables[i].TableName = TableNames[i];
                    }
                }
                return ds;
            }
        }

        public DataTable RunSqlRetDT(string Sql, string TableName) {
            return RunSqlRetDS(Sql._ToArray(";"), new string[] { TableName }).Tables[0];
        }

        public int RunSqlTranRetNum(string[] LstSql) {
            Comm.CommandType = CommandType.Text;
            Comm.CommandText = LstSql._ToString(";");
            try {
                return Comm.ExecuteNonQuery();
            }
            catch (SqlException Ex) {
                ConnClose();
                LogHelper.Debug(LstSql._ToString(";"));
                throw Ex;
            }
        }

        public DataSet RunSqlTranRetDS(string[] LstSql, string[] TableNames) {
            return RunSqlRetDS(LstSql, TableNames);
        }

        public void RunProcedure(string ProName, DbParameter[] Par) {
            Comm.CommandType = CommandType.StoredProcedure;
            Comm.CommandText = ProName;
            foreach (SqlParameter par in Par) {
                Comm.Parameters.Add(par);
            }

            try {
                Comm.ExecuteNonQuery();
            }
            catch (SqlException Ex) {
                ConnClose();
                LogHelper.Debug("ProName:" + ProName);
                throw Ex;
            }
        }

        public void BatchInsert(DataTable Dt, int BatchSize = 5000) {
            SqlBulkCopy bc = new SqlBulkCopy(Conn);
            try {
                bc.DestinationTableName = Dt.TableName;
                foreach (DataColumn dc in Dt.Columns) {
                    bc.ColumnMappings.Add(dc.ColumnName, dc.ColumnName);
                }
                bc.BatchSize = BatchSize;
                bc.WriteToServer(Dt);
                bc.Close();
            }
            catch (Exception Ex) {
                ConnClose();
                bc.Close();
                throw Ex;
            }
        }

        public int BatchUpdate(DataTable Dt, string PrimaryKeyName, int BatchSize = 5000) {
            int columnLenght = Dt.Columns.Count;
            StringBuilder sbInser = new StringBuilder();
            SqlParameter[] par = new SqlParameter[columnLenght];
            for (int i = 0; i < columnLenght; i++) {
                DataColumn dc = Dt.Columns[i];
                if (!dc.ColumnName.ToUpper().Equals(PrimaryKeyName.ToUpper())) {
                    sbInser.AppendFormat("{0}=@{0}", dc.ColumnName);
                    if (i < columnLenght - 1)
                        sbInser.Append(",");
                }
                par[i] = new SqlParameter(string.Format("@{0}", Dt.Columns[i].ColumnName), null);
                par[i].SourceColumn = dc.ColumnName;
            }
            using (Da = new SqlDataAdapter()) {
                Da.SelectCommand = new SqlCommand(string.Format("select top 0 * from {0}", Dt.TableName), Conn);
                Da.UpdateCommand = new SqlCommand(string.Format("update {0} set {1} where {2} = @{2}", Dt.TableName, sbInser, PrimaryKeyName), Conn);
                foreach (SqlParameter item in par) {
                    Da.UpdateCommand.Parameters.Add(item);
                }
                Da.UpdateBatchSize = BatchSize;
                return Da.Update(Dt);
            }
        }
        #endregion

        #region 扩展
        #region 存储过程
        /// <summary>
        ///  执行存储过程
        /// </summary>
        /// <param name="ProName">存储过程名称</param>
        /// <param name="Par">参数</param>
        /// <returns></returns>
        public DataSet RunProcedureRetDS(string ProName, SqlParameter[] Par) {
            using (Comm = new SqlCommand(ProName, Conn) { CommandType = CommandType.StoredProcedure }) {
                foreach (SqlParameter par in Par) {
                    Comm.Parameters.Add(par);
                }
                using (SqlDataAdapter adapter = new SqlDataAdapter(Comm)) {
                    try {
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        return ds;
                    }
                    catch (SqlException Ex) {
                        ConnClose();
                        LogHelper.Debug("ProName:" + ProName);
                        throw Ex;
                    }
                }
            }
        }
        #endregion
        #endregion

        #region 数据库操作（清除数据库日志信息、创建、备份、还原数据库）
        /// <summary>
        /// 清除数据库日志，
        /// </summary>
        /// <param name="DataName">数据库名称</param>
        public void DataClearLog(string DataName) {
            RunSqlRetNum(string.Format(@"
DUMP TRANSACTION[{0}] WITH NO_LOG
    BACKUP LOG[{0}] WITH NO_LOG
        DBCC SHRINKDATABASE([{0}])
", DataName));
        }

        //private string ConnStr {
        //    get {
        //        string str = ConfigHelper.ConnStr;
        //        if (str.LastIndexOf(';') < str.Length - 1) {
        //            str += ";";
        //        }
        //        Regex r = new Regex("initial catalog=[a-z0-9_]{0,};");
        //        return r.Replace(str, "initial catalog=master;");
        //    }
        //}

        /// <summary>
        /// 创建数据
        /// </summary>
        /// <param name="DBName">数据库名称</param>
        /// <param name="DBFilePath">数据库文件路径</param>
        /// <returns></returns>
        public int CreateDB(string DBName, string DBFilePath) {
            string sql = string.Format(@"
use master;
IF DB_ID(N'{0}') IS NOT NULL 
DROP DATABASE {0};
CREATE DATABASE {0} 
ON(NAME=MyDatabase_dat,FILENAME='{1}{0}.mdf',SIZE=5,MAXSIZE=10,FILEGROWTH=1) 
LOG ON(NAME=MyDatabase_log,FILENAME='{1}{0}.ldf',SIZE=2,MAXSIZE=5,FILEGROWTH=1)", DBName, DBFilePath);
            return RunSqlRetNum(sql);
        }

        /// <summary>
        /// 备份数据库
        /// </summary>
        /// <param name="DBName">要备份的数据源名称</param>
        /// <param name="DBBackupPath">备份到的数据库文件名称及路径</param>
        /// <returns></returns>
        public bool BackupDB(string DBName, string DBBackupPath) {
            string procname = "sp_dropdevice";
            string name = String.Format("{0}_{1}", DBName, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
            SqlParameter sqlpar = new SqlParameter();

            //删除逻辑备份设备，但不会删掉备份的数据库文件
            Comm.CommandType = CommandType.StoredProcedure;
            Comm.CommandText = procname;
            sqlpar = Comm.Parameters.Add("@logicalname", SqlDbType.VarChar, 20);
            sqlpar.Direction = ParameterDirection.Input;
            sqlpar.Value = DBName;

            //如果逻辑设备不存在，略去错误
            try {
                Comm.ExecuteNonQuery();
            }
            catch (Exception Ex) {
                ConnClose();
                throw Ex;
            }

            //创建逻辑备份设备
            Comm.CommandType = CommandType.StoredProcedure;
            procname = "sp_addumpdevice";
            sqlpar = Comm.Parameters.Add("@devtype", SqlDbType.VarChar, 20);
            sqlpar.Direction = ParameterDirection.Input;
            sqlpar.Value = "disk";

            sqlpar = Comm.Parameters.Add("@logicalname", SqlDbType.VarChar, 20);//逻辑设备名
            sqlpar.Direction = ParameterDirection.Input;
            sqlpar.Value = DBName;

            sqlpar = Comm.Parameters.Add("@physicalname", SqlDbType.NVarChar, 260);//物理设备名
            sqlpar.Direction = ParameterDirection.Input;
            sqlpar.Value = String.Format("{0}{1}.bak", DBBackupPath, name);

            try {
                Comm.ExecuteNonQuery();
            }
            catch (Exception Ex) {
                ConnClose();
                throw Ex;
            }

            //备份数据库到指定的数据库文件(完全备份)
            string sql = String.Format("BACKUP DATABASE {0} TO {0} WITH INIT", DBName);
            Comm.CommandType = CommandType.Text;
            Comm.CommandText = sql;
            try {
                Comm.ExecuteNonQuery();
            }
            catch (Exception Ex) {
                ConnClose();
                throw Ex;
            }
            return true;
        }

        /// <summary>
        /// 还原指定的数据库文件
        /// </summary>
        /// <param name="DataBaseName">要还原的数据库</param>
        /// <param name="DataBaseFilePath">数据库备份文件及路径</param>
        /// <returns></returns>
        public bool RestoreDB(string DBName, string DBFilePath) {
            //还原指定的数据库文件
            string sql = String.Format("RESTORE DATABASE {0} from DISK = '{1}' ", DBName, DBFilePath);
            using (Comm = new SqlCommand(sql, Conn) { CommandType = CommandType.Text }) {
                try {
                    Comm.ExecuteNonQuery();
                    return true;
                }
                catch (Exception Ex) {
                    ConnClose();
                    throw Ex;
                }
            }
        }
        #endregion
    }
}