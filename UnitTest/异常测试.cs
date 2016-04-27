using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Collections.Generic;

namespace blqw
{
    class user
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime date { get; set; }
    }

    [TestClass]
    public class 异常测试
    {
        [TestMethod]
        public void 异常栈展示()
        {
            var table1 = new DataTable("a");
            table1.Columns.Add("id", typeof(int));
            table1.Columns.Add("name", typeof(string));
            table1.Rows.Add(1, "赵");
            table1.Rows.Add(2, "钱");
            table1.Rows.Add(3, "孙");
            table1.Rows.Add(4, "李");

            var table2 = new DataTable("b");
            table2.Columns.Add("id", typeof(int));
            table2.Columns.Add("date", typeof(string));
            table2.Rows.Add(1, "2016-04-20");
            table2.Rows.Add(2, "2016-04-21");
            table2.Rows.Add(3, "2016-04-22A");
            table2.Rows.Add(4, "2016-04-23");
            
            var dataset = new DataSet();
            dataset.Tables.Add(table1);
            dataset.Tables.Add(table2);

            dataset.To<Dictionary<string, List<user>>>();
        }

        
        
    }
}
