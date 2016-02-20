using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(1.To<StringComparison>());


            CodeTimer.Initialize();
            "1,2,3,4,5,6".To<int[]>();
            Console.WriteLine("www.baidu.com".To<Uri>().AbsoluteUri);
            //"1,2,3,4,5,6".To<int[]>();
            CodeTimer.Time("a", 100000, () => {
                "1,2,3,4,5,6".To<int[]>();
            });
            CodeTimer.Time("a", 100000, () => {
                "1,2,3,4,5,6".Split(',').Select(int.Parse).ToArray();
            });
            var user = new { id = "1", name = 123 }.To<User>();

            Console.WriteLine("1,2,3,4,5,6".To<int[]>());
            Console.WriteLine("".To<int?>());

            var dict = new Dictionary<string, string[]>()
            {
                { " 2015-1-1", new[]{ "1", "2", "3", "4" }}
            };

            var list = dict.To<Dictionary<DateTime, List<int>>>();

            //var my = "{'id':1,'name':'blqw'}".To<MyClass>();

            Console.WriteLine("1,2,3,4,5".To<ArrayList>());

            var table = new DataTable();
            table.Columns.Add("id", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Sex", typeof(bool));
            table.Rows.Add(1, "blqw1", true);
            table.Rows.Add(2, "blqw2", false);
            table.Rows.Add(3, "blqw3", true);
            table.Rows.Add(4, "blqw4", false);
            CodeTimer.Time("x", 10000, () => {

                var list1 = table.To<List<NameValueCollection>>();
                var list2 = table.To<List<User>>();
                var list3 = table.To<List<Dictionary<string, object>>>();
                var list4 = table.To<List<Hashtable>>();

                var tb1 = list1.To<DataTable>();
                var tb2 = list2.To<DataTable>();
                var tb3 = list3.To<DataTable>();
                var tb4 = list4.To<DataTable>();
            });



            Console.WriteLine();
        }

        class User
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public DateTime Birthday { get; set; }
            public bool Sex { get; set; }
        }

        class MyClass
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }
    }
}
