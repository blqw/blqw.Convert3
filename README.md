# blqw.Convert3
全能转换器

> 对象转换，从未如此简单  
```csharp
obj.To<T>();                //转换失败,抛出异常
obj.To<T>(T defaultValue);  //转换失败,返回默认值
obj.To<T>(out succeed);     //输出转换是否成功
//下面3个是非泛型方法 
obj.To(Type outputType);
obj.To(Type outputType, object defaultValue);
obj.To(Type outputType, out succeed);
```

## 代码展示
更多示例代码: [demo](https://github.com/blqw/blqw.Convert3/blob/master/Demo/Program.cs) , [单元测试1](https://github.com/blqw/blqw.Convert3/blob/master/UnitTest/%E5%9F%BA%E6%9C%AC%E5%8A%9F%E8%83%BD%E6%B5%8B%E8%AF%95.cs)  
```csharp
//最基本
"1".To<int>();
"a".To<int>(0); //转换失败返回 0
"是".To<bool>(); //支持 "是/否" "真/假" "对/错" "t/f" "true/false" 等
byte[].To<Guid>();

//进阶
"1,2,3,4,5,6".To<int[]>();
"{\"id\":\"name\":\"blqw\"}".To<User>();
Dictionary.To<Entity>(); //键值对转实体
DataRow.To<Entity>(); //数据行转实体
DataTable.To<List<Entity>>; //数据表转实体集合

//更复杂
DataTable.To<List<NameValueCollection>>(); 
List<Dictionary<string, object>>.To<DataTable>(); 
new { ID=1, Name="blqw"}.To<User>(); //匿名类转换

//变态嵌套
Dictionary<Guid, Dictionary<int, User>>
    .To<Dictionary<string, Dictionary<DateTime, NameValueCollection>>>(); //不能理解就算了
```
## 扩展自定义转换器
> 扩展转换行为是以**目标类型**为参照的  
> 例如要转换为自定义类型`MyClass`就需要**实现`IConvertor<MyClass>`**接口  
> 或者直接**继承基类 `BaseConvertor<MyClass>`**,并保留默认**无参构造函数**  
> IOC组件会自动装载实现`IConvertor`的全部类型

#### 1. `MyClass`定义
```csharp
class MyClass
{
     int Number { get; set; }
}
```
#### 2. 转换器代码
```csharp
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
```
#### 3. 测试代码
```csharp
var x = "1234".To<MyClass>(null);
Console.WriteLine(x?.Number); //1234
x = "abcd".To<MyClass>(null);
Console.WriteLine(x?.Number); //null
```
## 其他功能
```csharp
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

//摘要/加密
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
```


## 更新说明 
#### [3.0.5.4] 2017.04.12
+ 动态类型增加`DebuggerDispaly`特性

#### [3.0.5.2] 2016.12.19
* 更新ioc
* 修复ioc初始化日志中有一条错误的问题

#### [3.0.5.1] 2016.12.02
* 取消对`CJsonObject`的密封类属性
* 修改对`IBuilder<out TInstance, in TValue>`及其子类的私有属性,改为``

#### [3.0.5] 2016.11.30
* 优化单值转数组的体验，如：`string s = "abc"; s.To<string[]>();` 将得到一个`new string[]{"abc"}`
* 优化接口、抽象类、静态类、泛型定义类的错误信息

> [单元测试](UnitTest/3.0.5.cs)

#### [3.0.4] 2016.11.16
* 优化动态类型的处理

#### [3.0.3.2] 2016.11.15
* 修复`Mapper`类的一个bug

#### [3.0.3.1] 2016.11.14
* 修复`IDataReader`转基础类型时会出现错误的问题

#### [3.0.3-beta] 2016.10.07
* 修复bug
* 更新`blqw.IOC`

#### [3.0.2-beta] 2016.10.05
* 修复汉字转拼音时二级汉字转换错误的问题
* 修复其他bug

#### [3.0.0-beta] 2016.09.30
* 重构代码,优化代码逻辑
* 完善注释
* 优化异常栈
 
#### 2016.08.04
* 修复`DataReader`转`String`错误的问题

#### 2016.08.03
* 修正空字符串转数组时会抛出异常的错误

#### 2016.07.20
* 覆盖老版IOC  

#### 2016.07.18
* 修复嵌套类型中获取类名错误的问题  

#### 2016.07.05
* 修复一个无限循环的bug  
* 修复-1转枚举失败的问题  
* 修复一个转型失败可能抛出异常的问题  
* 修复一个可空枚举转型失败的问题  
* 修复当某个类继承泛型类时,但本身不是泛型时抛出异常的问题  
* 更新IOC模块  

#### 2016.06.27
* 更新到2.1  
* 发布到nuget  

#### 2016.04.28  
* 发布2.0新版  
* 重构内部实现,代码更精简,可读性更高  
* 调整部分方法签名,更清晰  
* 优化加载模式,使用MEF注入更简单  
* 优化错误信息,更明确  

#### 2016.03.16
* 修复ioc插件bug  
  
#### 2016.02.23  
* 修复bug  

#### 2016.02.21  
* 优化获取属性值的逻辑，优先使用Literacy插件  

#### 2016.02.20  
* 重构ioc逻辑，修复部分bug  

#### 2015.11.06
* 正式发布并提供后期维护
