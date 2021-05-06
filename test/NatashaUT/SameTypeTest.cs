﻿using Natasha.CSharp;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace NatashaUT
{
    [Trait("程序集同类测试", "")]
    public class SameTypeTest : PrepareTest
    {
        public object obj;
        public SameTypeTest()
        {
            obj = new object();
        }
        [Fact(DisplayName = "同命名空间程序集1")]
        public void Test1()
        {

#if !NETCOREAPP2_2
            lock (obj)
            {
                using (DomainComponent.CreateAndLock("TestSame"))
                {

                    var domain = DomainComponent.CurrentDomain;
                    var assembly = domain.CreateAssembly("ababab");
                    assembly.AddScript("using System;namespace ClassLibrary1{ public class Class1{public string name;}}");
                    var result2 = assembly.GetAssembly();
                    var type2 = result2.GetTypes().First(item=>item.Name =="Class1");


                    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Lib", "Repeate", "ClassLibrary1.dll");
                    var result1 = domain.LoadPluginFromStream(path);
                    var type1 = result1.GetTypes().First(item => item.Name == "Class1");
                    domain.Remove(path);


                    Assert.Equal("TestSame", DomainComponent.CurrentDomain.Name);
                    Assert.NotEqual(result1, result2);
                    Assert.Equal(type1.Name, type2.Name);


                    var func = NDelegate.UseDomain(domain).Func<object>("return new Class1();", "ClassLibrary1");
                    Assert.Equal(result2, func().GetType().Assembly);
                }

            }
#endif

        }


        [Fact(DisplayName = "同命名空间程序集2")]
        public void Test2()
        {

#if !NETCOREAPP2_2
            using (DomainComponent.CreateAndLock("Default1"))
            {

                var domain = DomainComponent.CurrentDomain;
                var assembly = domain.CreateAssembly("ClassLibrary1");
                assembly.AddScript("using System;namespace ClassLibrary1{ public class Class1{public string name;}}");
                var result2 = assembly.GetAssembly();
                var type2 = result2.GetTypes().First(item => item.Name == "Class1");

                try
                {
                    var assembly1 = domain.CreateAssembly("AsmTest2");
                    assembly1.AddScript("using System;namespace ClassLibrary1{ public class Class1{public string name;}}");
                    var result1 = assembly1.GetAssembly();
                    var type1 = result1.GetTypes().First(item => item.Name == "Class1");

                    Assert.NotEqual(result1, result2);
                    Assert.Equal(type1.Name, type2.Name);
                    lock (obj)
                    {
                        var func = NDelegate.UseDomain(domain).Func<object>("return new Class1();", "ClassLibrary1");
                        Assert.Equal(result2, func().GetType().Assembly);
                    }
                }
                catch (Exception ex)
                {

                    Assert.NotNull(ex);
                }


            }
#endif

        }


        [Fact(DisplayName = "同命名空间程序集3")]
        public void Test3()
        {

#if !NETCOREAPP2_2
            NSucceedLog.Enabled = true;
            using (DomainComponent.CreateAndLock("Default2"))
            {

                var domain = DomainComponent.CurrentDomain;
                var assembly = domain.CreateAssembly("ClassLibrary1");
                assembly.AddScript("using System;namespace ClassLibrary1{ public class Class1{public string name;}}");
                var result2 = assembly.GetAssembly();
                var type2 = result2.GetTypes().First(item => item.Name == "Class1");
                //domain.RemoveAssembly(result2);


                var assembly1 = domain.CreateAssembly("AsmTest22");
                assembly1.AddScript("using System;namespace ClassLibrary1{ public class Class1{public string name;}}");
                var result1 = assembly1.GetAssembly();
                var type1 = result1.GetTypes().First(item => item.Name == "Class1");


                Assert.NotEqual(result1, result2);
                Assert.Equal(type1.Name, type2.Name);
                domain.Remove(result2);

                lock (obj)
                {
                    //Class1 同时存在于 ClassLibrary1 和 AsmTest22 中
                    var func = NDelegate.UseDomain(domain).Func<object>("return new Class1();", "ClassLibrary1");
                    Assert.Equal(result1, func().GetType().Assembly);
                }

            }
#endif

        }


        [Fact(DisplayName = "同命名空间程序集4")]
        public void Test4()
        {

#if !NETCOREAPP2_2
            lock (obj)
            {
                Assembly result1;
                //using (DomainManagment.Lock("Default"))
                //{

                    var domain = DomainComponent.CurrentDomain;
                    var assembly = domain.CreateAssembly("DAsmTest1");
                    assembly.AddScript("using System;namespace ClassLibrary1{ public class Class1{public string name;}}");
                    var result2 = assembly.GetAssembly();
                    var type2 = result2.GetTypes().First(item => item.Name == "Class1");
                    domain.Remove(result2);


                    var assembly1 = domain.CreateAssembly("DAsmTest2");
                    assembly1.AddScript("using System;namespace ClassLibrary1{ public class Class1{public string name;}}");
                    result1 = assembly1.GetAssembly();
                    var type1 = result1.GetTypes().First(item => item.Name == "Class1");


                    Assert.NotEqual(result1, result2);
                    Assert.Equal(type1.Name, type2.Name);


                //}
                var func = NDelegate.DefaultDomain().Func<object>("return new Class1();", "ClassLibrary1");
                Assert.Equal(result1, func().GetType().Assembly);
                DomainComponent.Default.Remove(result1);
            }
#endif

        }


        [Fact(DisplayName = "同命名空间程序集5")]
        public void Test5()
        {

#if !NETCOREAPP2_2
            lock (obj)
            {
                Assembly result1;


                    var domain = DomainComponent.Random;
                    //var assembly = domain.CreateAssembly("AsmTest1");
                    //assembly.AddScript("using System;namespace ClassLibrary1{ public class Class1{public string name;}}");
                    //var result2 = assembly.Compiler();
                    //var type2 = assembly.GetType("Class1");
                    //domain.RemoveAssembly(result2);


                    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Lib", "Repeate", "ClassLibrary1.dll");
                    result1 = domain.LoadPluginFromStream(path);
                    var type1 = result1.GetTypes().First(item => item.Name == "Class1");


                    //Assert.NotEqual(result1, result2);
                    //Assert.Equal(type1.Name, type2.Name);



                var func = NDelegate.UseDomain(domain).Func<object>("return new Class1();", "ClassLibrary1");
                Assert.Equal(result1, func().GetType().Assembly);
                domain.Remove(path);
            }
#endif

        }
    }
}
