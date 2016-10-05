using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using blqw.Converts;

namespace blqw
{

    class MyClass
    {
        public int Number { get; set; }
    }

    class MyClassConvertor : BaseConvertor<MyClass>
    {
        protected override MyClass ChangeType(ConvertContext context, object input, Type outputType, out bool success)
        {
            var i = context.Get<int>().ChangeType(context, input, typeof(int), out success);
            return success ? new MyClass() { Number = i } : null;
        }

        protected override MyClass ChangeType(ConvertContext context, string input, Type outputType, out bool success)
        {
            var i = context.Get<int>().ChangeType(context, input, typeof(int), out success);
            return success ? new MyClass() { Number = i } : null;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
//数字转大写
//参数说明:1.需要转换的数字,2:是否是简体中文,3:是否需要加上圆角分(只保留两位),4:是否需要支持15位以上的数字
Console.WriteLine(Convert3.NumberToUpper("123456456.789", true, true, false)); //一亿二千三百四十五万六千四百五十六元七角八分

//汉字转拼音
Console.WriteLine(Convert3.ToPinyin("冰麟轻武", PinyinMode.AllFirst));      //BLQW
Console.WriteLine(Convert3.ToPinyin("冰麟轻武", PinyinMode.First));         //B
Console.WriteLine(Convert3.ToPinyin("冰麟轻武", PinyinMode.Full));          //BingLinQingWu
Console.WriteLine(Convert3.ToPinyin("冰麟轻武", PinyinMode.FullWithSplit)); //Bing Lin Qing Wu

//全半角转换
Console.WriteLine(Convert3.ToDBC("，１２３４５６７ａｋｓ"));//,1234567aks
Console.WriteLine(Convert3.ToSBC("!1f23d.?@"));         //！１ｆ２３ｄ．？＠

Console.WriteLine(Convert3.ToMD5("123456"));    //e10adc3949ba59abbe56e057f20f883e
Console.WriteLine(Convert3.ToSHA1("123456"));   //7c4a8d09ca3762af61e59520943dc26494f8941b

//转为动态类型
var a = new Dictionary<string, object>() { ["id"] = 1, ["name"] = "blqw" };
Console.WriteLine(Convert3.ToDynamic(a).name);   //blqw
Console.WriteLine(Convert3.ToDynamic(a).id == 1);//True

//随机加密
var arr = new[]
{
    Convert3.ToRandomMD5("123456"),
    Convert3.ToRandomMD5("123456"),
    Convert3.ToRandomMD5("123456"),
    Convert3.ToRandomMD5("123456"),
    Convert3.ToRandomMD5("123456"),
};

foreach (var g in arr)
{
    Console.WriteLine($"{g} : {Convert3.EqualsRandomMD5("123456", g)}");
}
/*
fa91eefc-e903-dbcf-394b-0b757987357b : True
27abd3e0-fe0e-2eeb-1ff7-a60b03876465 : True
6d911bf2-0c59-0e01-5e87-7527dd1ee699 : True
0af7905a-0b3b-4eb4-b82b-0340f3438924 : True
1e024253-6bb9-fb25-4b67-3e42c265af02 : True
*/

            Console.WriteLine(1.To<StringComparison>());





            CodeTimer.Initialize();
            "1,2,3,4,5,6".To<int[]>();
            Console.WriteLine("www.baidu.com".To<Uri>().AbsoluteUri);
            //"1,2,3,4,5,6".To<int[]>();
            CodeTimer.Time("a", 100000, () =>
            {
                "1,2,3,4,5,6".To<int[]>();
            });
            CodeTimer.Time("a", 100000, () =>
            {
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
            CodeTimer.Time("x", 10000, () =>
            {

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

    }
}
