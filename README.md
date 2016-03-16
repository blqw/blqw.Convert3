# blqw.Convert3
全能转换器

>从一个任意类型的对象转为另一个任意类型的对象  
>从未如此简单  
```csharp
object.To<Type>();  //转换失败抛出异常
object.To<Type>(defaultValue); //转换失败返回默认值
```

## 代码示例
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
Dictionary<Guid, Dictionary<int, User>>.To<Dictionary<string, Dictionary<DateTime, NameValueCollection>>>(); //不能理解就算了
```

## 更新说明  
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