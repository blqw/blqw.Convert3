using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Data;
using System.Collections.Specialized;

namespace blqw
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void 基本功能()
        {
            
            Assert.AreEqual(1, "1".To<int>());
            Assert.AreEqual(null, "".To<int?>());

            var my = "{\"ID\":1,\"Name\":\"blqw\"}".To<MyClass>();
            Assert.IsNotNull(my);
            Assert.AreEqual(1, my.ID);
            Assert.AreEqual("blqw", my.Name);

            var arr = "[1,2,3,4,5,6]".To<int[]>();
            Assert.IsNotNull(arr);
            Assert.AreEqual(6, arr.Length);
            Assert.AreEqual(1, arr[0]);
            Assert.AreEqual(2, arr[1]);
            Assert.AreEqual(3, arr[2]);
            Assert.AreEqual(4, arr[3]);
            Assert.AreEqual(5, arr[4]);
            Assert.AreEqual(6, arr[5]);

            var user = new User()
            {
                ID = 1,
                Name = "blqw",
                Birthday = DateTime.Now,
                Sex = true
            };
            var my2 = user.To<MyClass>();
            Assert.IsNotNull(my2);
            Assert.AreEqual(1, my2.ID);
            Assert.AreEqual("blqw", my2.Name);


        }

        [TestMethod]
        public void 进阶功能()
        {
            //CodeTimer.Initialize();
            var arr = "1,2,3,4,5,6".To<int[]>();
            Assert.IsNotNull(arr);
            Assert.AreEqual(6, arr.Length);
            Assert.AreEqual(1, arr[0]);
            Assert.AreEqual(2, arr[1]);
            Assert.AreEqual(3, arr[2]);
            Assert.AreEqual(4, arr[3]);
            Assert.AreEqual(5, arr[4]);
            Assert.AreEqual(6, arr[5]);

            var arr2 = "1,2,3,4,5,6".To<ArrayList>();
            Assert.IsNotNull(arr2);
            Assert.AreEqual(6, arr2.Count);
            Assert.AreEqual("1", arr2[0]);
            Assert.AreEqual("2", arr2[1]);
            Assert.AreEqual("3", arr2[2]);
            Assert.AreEqual("4", arr2[3]);
            Assert.AreEqual("5", arr2[4]);
            Assert.AreEqual("6", arr2[5]);


            var uri = "www.baidu.com".To<Uri>();
            Assert.IsNotNull(uri);
            Assert.AreEqual("http://www.baidu.com/", uri.AbsoluteUri);


            var user = new { id = "1", name = 123 }.To<User>();
            Assert.IsNotNull(user);
            Assert.AreEqual(1, user.ID);
            Assert.AreEqual("123", user.Name);

            var dict = new Dictionary<string, string[]>()
            {
                [" 2015-1-1"] = new[] { "1", "2", "3", "4" }
            };
            var dict2 = dict.To<Dictionary<DateTime, List<int>>>();
            Assert.IsNotNull(dict2);
            Assert.AreEqual(1, dict2.Count);
            Assert.AreEqual(new DateTime(2015, 1, 1), dict2.Keys.First());
            Assert.AreEqual(4, dict2.Values.First().Count);
            Assert.AreEqual(1, dict2.Values.First()[0]);
            Assert.AreEqual(2, dict2.Values.First()[1]);
            Assert.AreEqual(3, dict2.Values.First()[2]);
            Assert.AreEqual(4, dict2.Values.First()[3]);


            Console.WriteLine("1,2,3,4,5".To<ArrayList>());
            var table = new DataTable("x");
            table.Columns.Add("id", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Sex", typeof(bool));
            table.Rows.Add(1, "blqw1", true);
            table.Rows.Add(2, "blqw2", false);
            table.Rows.Add(3, "blqw3", true);
            table.Rows.Add(4, "blqw4", false);

            var list1 = table.To<List<NameValueCollection>>();
            Assert.IsNotNull(list1);
            Assert.AreEqual(4, list1.Count);
            Action<NameValueCollection, string, string, string> assert1 =
                (actual, id, name, sex) =>
                {
                    Assert.IsNotNull(actual);
                    Assert.AreEqual(id, actual["id"]);
                    Assert.AreEqual(name, actual["Name"]);
                    Assert.AreEqual(sex, actual["Sex"]);
                };
            assert1(list1[0], "1", "blqw1", "true");
            assert1(list1[1], "2", "blqw2", "false");
            assert1(list1[2], "3", "blqw3", "true");
            assert1(list1[3], "4", "blqw4", "false");

            var list2 = table.To<List<User>>();
            Assert.IsNotNull(list2);
            Assert.AreEqual(4, list2.Count);
            Action<User, int, string, bool> assert2 =
                (actual, id, name, sex) =>
                {
                    Assert.IsNotNull(actual);
                    Assert.AreEqual(id, actual.ID);
                    Assert.AreEqual(name, actual.Name);
                    Assert.AreEqual(sex, actual.Sex);
                };
            assert2(list2[0], 1, "blqw1", true);
            assert2(list2[1], 2, "blqw2", false);
            assert2(list2[2], 3, "blqw3", true);
            assert2(list2[3], 4, "blqw4", false);


            var list3 = table.To<List<Dictionary<string, object>>>();
            Assert.IsNotNull(list3);
            Assert.AreEqual(4, list3.Count);
            Action<Dictionary<string, object>, object, object, object> assert3 =
                (actual, id, name, sex) =>
                {
                    Assert.IsNotNull(actual);
                    Assert.AreEqual(id, actual["id"]);
                    Assert.AreEqual(name, actual["Name"]);
                    Assert.AreEqual(sex, actual["Sex"]);
                };
            assert3(list3[0], 1, "blqw1", true);
            assert3(list3[1], 2, "blqw2", false);
            assert3(list3[2], 3, "blqw3", true);
            assert3(list3[3], 4, "blqw4", false);

            var list4 = table.To<List<Hashtable>>();
            Assert.IsNotNull(list4);
            Assert.AreEqual(4, list4.Count);
            Action<Hashtable, object, object, object> assert4 =
                (actual, id, name, sex) =>
                {
                    Assert.IsNotNull(actual);
                    Assert.AreEqual(id, actual["id"]);
                    Assert.AreEqual(name, actual["Name"]);
                    Assert.AreEqual(sex, actual["Sex"]);
                };
            assert4(list4[0], 1, "blqw1", true);
            assert4(list4[1], 2, "blqw2", false);
            assert4(list4[2], 3, "blqw3", true);
            assert4(list4[3], 4, "blqw4", false);

            Action<DataRow, string, string, string> assert5 =
                (actual, id, name, sex) =>
                {
                    Assert.IsNotNull(actual);
                    Assert.AreEqual(id, actual["id"] + "");
                    Assert.AreEqual(name, actual["Name"] + "");
                    Assert.AreEqual(sex, (actual["Sex"] + "").ToLowerInvariant());
                };
            Action<DataTable> assert6 =
                (actual) =>
                {
                    Assert.IsNotNull(actual);
                    Assert.AreEqual(4, actual.Rows.Count);
                    assert5(actual.Rows[0], "1", "blqw1", "true");
                    assert5(actual.Rows[1], "2", "blqw2", "false");
                    assert5(actual.Rows[2], "3", "blqw3", "true");
                    assert5(actual.Rows[3], "4", "blqw4", "false");
                };
            var tb1 = list1.To<DataTable>();
            assert6(tb1);
            var tb2 = list2.To<DataTable>();
            assert6(tb2);
            var tb3 = list3.To<DataTable>();
            assert6(tb3);
            var tb4 = list4.To<DataTable>();
            assert6(tb4);
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
