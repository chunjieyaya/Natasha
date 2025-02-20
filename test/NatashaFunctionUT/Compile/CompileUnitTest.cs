﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace NatashaFunctionUT.Compile
{
    [Trait("基础功能测试", "编译")]
    public class CompileUnitTest : DomainPrepare
    {

        [Fact(DisplayName = "基础编译测试 - 程序集")]
        public void TestAssembly()
        {
            string code = @"public class A{public string Name=""HelloWorld"";}";
            using (DomainManagement.Random().CreateScope())
            {
                AssemblyCSharpBuilder builder = new();
                builder.Add(code);

                var assembly = builder.GetAssembly();
                Assert.NotNull(assembly);

                var type = assembly.GetTypes().Where(item => item.Name == "A").First();
                var info = type.GetField("Name");

                Assert.Equal("HelloWorld", info!.GetValue(Activator.CreateInstance(type)));
                Assert.NotEqual("Default", builder.Domain.Name);

            }
        }

        [Fact(DisplayName = "基础编译测试 - 类型")]
        public void TestType()
        {
            string code = @"public class A{public string Name=""HelloWorld"";}";
            using (DomainManagement.Random().CreateScope())
            {
                AssemblyCSharpBuilder builder = new();
                builder.Add(code);

                var type = builder.GetTypeFromShortName("A");
                Assert.Equal("A", type.Name);

                var info = type.GetField("Name");
                Assert.Equal("HelloWorld", info!.GetValue(Activator.CreateInstance(type)));


                Assert.NotEqual("Default", builder.Domain.Name);
               
            }
        }


        [Fact(DisplayName = "基础编译测试 - 委托")]
        public void TestDelegate()
        {
            string code = @"public class A{public string Name=""HelloWorld""; public static string Get(){  return (new A()).Name; }}";
            using (DomainManagement.Create("compileUntiTestFoDelegate").CreateScope())
            {
                AssemblyCSharpBuilder builder = new("compileUntiTestFoDelegateAssembly");
                builder.Add(code);
                var func = builder.GetDelegateFromShortName<Func<string>>("A","Get");
                Assert.Equal("compileUntiTestFoDelegateAssembly", func.Method.Module.Assembly.GetName().Name!);
                Assert.Equal("HelloWorld", func());
                Assert.Equal("compileUntiTestFoDelegate", builder.Domain.Name);

            }
        }

    }
}
