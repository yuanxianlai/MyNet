
using MyProject.DataBase;
using MyProject.DbModule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyMvc.Controllers {
    public class Config_ListController : Controller {
        public ActionResult List(string SMI_Code = "Sys0001") {
            string sql = string.Format(@"
select clt.CLT_Table,
       clt.CLT_Where,
       clt.CLT_OrderBy,
       clc.CLC_Column,
       clc.CLC_ColumnZH,
       clc.CLC_ColumnType,
       clc.CLC_JoinTable,
       clc.CLC_JoinColumnID,
       clc.CLC_JoinColumnValue,
       clc.CLC_JoinWhere
  from Config_List_Table clt, Config_List_Column clc
 where clt.CLT_ID = clc.CLT_ID
   and clt.CLT_Status = '001'
   and clc.CLC_Status = '001'
   and clt.SMI_Code = '{0}'
", SMI_Code);
            DbSqlServer db = new DbSqlServer();
            db.ConnOpen(new MyProjectContainer().Database.Connection.ConnectionString);
            DataTable dtConfigList = db.RunSqlRetDT(sql, "ConfigList");
            foreach (DataRow dr in dtConfigList.Rows) {

            }
            return View();
        }
    }
}
