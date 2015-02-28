using MyProject.Common;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Data.Common;
using System.Text;

namespace MyProject.DataBase.OracleHelper {
    /// <summary>
    /// Oracle 数据库访问
    /// </summary>
    public class DbOracle : IDbBase {
        #region   全局变量
        private OracleConnection Conn = null;
        private OracleCommand Comm = null;
        private OracleTransaction Tran = null;
        private OracleDataAdapter Da = null;
        #endregion

        #region  接口实现
        public void ConnOpen(string ConnStr = null) {
            if (ConnStr == null || ConnStr.Equals("")) {
                ConnStr = ConfigHelper.ConnStr;
            }
            Conn = new OracleConnection(ConnStr);
            Conn.Open();
        }

        public void TranBegin() {
            Tran = Conn.BeginTransaction();
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
            using (Comm = new OracleCommand(Sql, Conn) { CommandType = CommandType.Text }) {
                try {
                    return Comm.ExecuteScalar();
                }
                catch (OracleException Ex) {
                    LogHelper.Debug(Sql);
                    throw Ex;
                }
                finally {
                    ConnClose();
                }
            }
        }

        public string InsertRetIdentity(string Sql) {
            throw new NotImplementedException();
        }

        public int RunSqlRetNum(string Sql) {
            using (Comm = new OracleCommand(Sql, Conn) { CommandType = CommandType.Text }) {
                try {
                    return Comm.ExecuteNonQuery();
                }
                catch (OracleException Ex) {
                    LogHelper.Debug(Sql);
                    throw Ex;
                }
            }
        }

        public DataSet RunSqlRetDS(string[] LstSql, string[] TableNames) {
            using (OracleDataAdapter da = new OracleDataAdapter(LstSql._ToString(";"), Conn)) {
                DataSet ds = new DataSet();
                da.Fill(ds);
                for (int i = 0; i < TableNames.Length; i++) {
                    if (!string.IsNullOrEmpty(TableNames[i])) {
                        ds.Tables[i].TableName = TableNames[i];
                    }
                }
                ConnClose();
                return ds;
            }
        }

        public DataTable RunSqlRetDT(string Sql, string TableName) {
            return RunSqlRetDS(Sql._ToArray(";"), new string[] { TableName }).Tables[0];
        }

        public int RunSqlTranRetNum(string[] LstSql) {
            using (Tran = Conn.BeginTransaction()) {
                using (Comm = new OracleCommand(LstSql._ToString(";"), Conn) {
                    CommandType = CommandType.Text,
                    Transaction = Tran
                }) {
                    try {
                        int ret = Comm.ExecuteNonQuery();
                        Tran.Commit();
                        return ret;
                    }
                    catch (OracleException Ex) {
                        Tran.Rollback();

                        LogHelper.Debug(LstSql._ToString(";"));
                        throw Ex;
                    }
                    finally {
                        ConnClose();
                    }
                }
            }
        }

        public DataSet RunSqlTranRetDS(string[] LstSql, string[] TableNames) {
            return RunSqlRetDS(LstSql, TableNames);
        }

        public void RunProcedure(string ProName, DbParameter[] Par) {
            using (Comm = new OracleCommand(ProName, Conn) { CommandType = CommandType.StoredProcedure }) {
                foreach (OracleParameter par in Par) {
                    Comm.Parameters.Add(par);
                }

                try {
                    Comm.ExecuteNonQuery();
                }
                catch (OracleException Ex) {
                    LogHelper.Debug("ProName:" + ProName);
                    throw Ex;
                }
                finally {
                    ConnClose();
                }
            }
        }

        /// <summary>
        /// 批量数据写入
        /// </summary>
        /// <param name="Dt">需要写入的数据集</param>
        /// <param name="ConnStr">连接字符串</param>
        /// <param name="BatchSize">提交数据条数</param>
        public void BatchInsert(DataTable Dt, int BatchSize = 5000) {
            using (Comm = new OracleCommand() { Connection = Conn }) {
                //到此为止，还都是我们熟悉的代码，下面就要开始喽 
                //这个参数需要指定每次批插入的记录数 
                Comm.ArrayBindCount = Dt.Rows.Count;

                //在这个命令行中,用到了参数,参数我们很熟悉,但是这个参数在传值的时候 
                //用到的是数组,而不是单个的值,这就是它独特的地方 
                StringBuilder sbColumn = new StringBuilder();
                foreach (DataColumn dc in Dt.Columns) {
                    sbColumn.AppendFormat(":{0},", dc.ColumnName);
                }
                string columnStr = sbColumn.ToString().Substring(0, sbColumn.Length - 1);

                Comm.CommandText = string.Format("insert into {0} ({1}) values({2})", Dt.TableName, columnStr.Replace(":", ""), columnStr);

                foreach (DataColumn dc in Dt.Columns) {
                    OracleParameter op = new OracleParameter();
                    op.ParameterName = dc.ColumnName;
                    switch (dc.DataType.Name) {
                        case "String":
                            op.OracleDbType = OracleDbType.Varchar2;
                            string[] dataStr = new string[Dt.Rows.Count];
                            for (int i = 0; i < dataStr.Length; i++) {
                                dataStr[i] = Dt.Rows[i][dc.ColumnName].ToString();
                            }
                            op.Value = dataStr;
                            op.OracleDbType = OracleDbType.Varchar2;
                            break;
                        case "Int16":
                            int[] dataInt = new int[Dt.Rows.Count];
                            for (int i = 0; i < dataInt.Length; i++) {
                                dataInt[i] = Convert.ToInt16(Dt.Rows[i][dc.ColumnName]);
                            }
                            op.Value = dataInt;
                            op.OracleDbType = OracleDbType.Int16;
                            break;
                        case "Double":
                            Double[] dataDouble = new Double[Dt.Rows.Count];
                            for (int i = 0; i < dataDouble.Length; i++) {
                                dataDouble[i] = Convert.ToDouble(Dt.Rows[i][dc.ColumnName]);
                            }
                            op.Value = dataDouble;
                            op.OracleDbType = OracleDbType.Double;
                            break;
                    }

                    op.Direction = ParameterDirection.Input;
                    Comm.Parameters.Add(op);
                }

                Comm.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 批量数据修改
        /// </summary>
        /// <param name="Dt">需要修改的数据集</param>
        /// <param name="PrimaryKeyName">修改主键</param>
        /// <param name="BatchSize">提交数据条数</param>
        /// <returns></returns>
        public int BatchUpdate(DataTable Dt, string PrimaryKeyName, int BatchSize = 5000) {
            int columnLenght = Dt.Columns.Count;
            StringBuilder sbInser = new StringBuilder();
            OracleParameter[] par = new OracleParameter[columnLenght];
            for (int i = 0; i < columnLenght; i++) {
                DataColumn dc = Dt.Columns[i];
                if (!dc.ColumnName.ToUpper().Equals(PrimaryKeyName.ToUpper())) {
                    sbInser.AppendFormat("{0}=@{0}", dc.ColumnName);
                    if (i < columnLenght - 1)
                        sbInser.Append(",");
                }
                par[i] = new OracleParameter(string.Format("@{0}", Dt.Columns[i].ColumnName), null);
                par[i].SourceColumn = dc.ColumnName;
            }
            using (Da = new OracleDataAdapter()) {
                Da.SelectCommand = new OracleCommand(string.Format("select top 0 * from {0}", Dt.TableName), Conn);
                Da.UpdateCommand = new OracleCommand(string.Format("update {0} set {1} where {2} = @{2}", Dt.TableName, sbInser, PrimaryKeyName), Conn);
                foreach (OracleParameter item in par) {
                    Da.UpdateCommand.Parameters.Add(item);
                }
                Da.UpdateBatchSize = BatchSize;
                return Da.Update(Dt);
            }
        }
        #endregion
    }
}
