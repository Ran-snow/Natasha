﻿using Natasha;
using Natasha.CSharp;
using Natasha.Error.Model;
using Natasha.Framework;
using NatashaUT.Model;
using System;
using Xunit;

namespace NatashaUT
{

    [Trait("快速构建", "函数")]
    public class NDelegateMethodTest
    {

        [Fact(DisplayName = "独立域函数1")]
        public static void RunDelegate1()
        {
            Func<string> func = NDelegate.CreateDomain("NDelegate1").Func<string>("return \"1\";");
            Assert.Equal("1", func());
        }




        [Fact(DisplayName = "独立域函数2")]
        public static void RunDelegate2()
        {
            //-----------创建一个域（方便卸载）---------------//-----创建Func方法--------//
            var func = NDelegate.CreateDomain("NDelegate2").Func<string,string>("return arg;");
            Assert.Equal("1", func("1"));
        }




        [Fact(DisplayName = "独立域函数3")]
        public static void RunDelegate3()
        {
            var func = NDelegate.CreateDomain("NDelegate3").Func<string,string, string>("return arg1+arg2;");
            Assert.Equal("12", func("1","2"));
        }



        [Fact(DisplayName = "独立域函数7")]
        public static void RunDelegate7()
        {
            var func = NDelegate.CreateDomain("NDelegate7").Func<string>("return OtherNameSpaceMethod.FromDate(DateTime.Now);");
            Assert.Equal(DateTime.Now.ToString("yyyy-MM"), func());
        }



        [Fact(DisplayName = "独立域函数4")]
        public static void RunDelegate4()
        {
            NormalTestModel model = new NormalTestModel();
            var func = NDelegate.CreateDomain("NDelegate4").Action<NormalTestModel>("obj.Age=1;");
            func(model);
            Assert.Equal(1, model.Age);
        }




        [Fact(DisplayName = "独立域函数5")]
        public static void RunDelegate5()
        {
            NormalTestModel model = new NormalTestModel();
            var func = NDelegate.CreateDomain("NDelegate5").Action<NormalTestModel,int>("arg1.Age=arg2;");
            func(model,1);
            Assert.Equal(1, model.Age);
        }




        
        public static int RunDelegate6()
        {
            NormalTestModel model = new NormalTestModel();
            var func = NDelegate.CreateDomain("NDelegate6").Action<NormalTestModel, int, int>("arg1.Age=arg2+arg3;");
            func(model,1,2);
            func.DisposeDomain();
            return model.Age;
            
        }

        [Fact(DisplayName = "卸载委托测试")]
        public static void UnloadDelegate()
        {
            Assert.Equal(3, RunDelegate6());
#if !NETCOREAPP2_2
            for (int i = 0; i < 6; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            Assert.True(DomainManagement.IsDeleted("NDelegate6"));
#endif
        }



        [Fact(DisplayName = "独立编译")]
        public void TestType1()
        {
            var type = NDelegate.CreateDomain("NDelegate8").GetType(
                @"public class  DomainTest1{
                        public string Name;
                        public int Operator;
                }");

            Assert.Equal("DomainTest1", type.Name);
        }


        [Fact(DisplayName = "类型比域")]
        public void TestTypeEqual()
        {
            var domain = DomainManagement.Random;
            var type = NDelegate.UseDomain(domain,builder => {
                builder
                .CustomUsing()                    //使用用户自定义的Using
                .SetAssemblyName("MyAssemblyName")  //设置程序集名
                .ThrowAndLogCompilerError()         //抛出并记录编译器的异常 
                .ThrowSyntaxError()                 //抛出语法树异常
                .UseStreamCompile();                //使用流编译
            }).GetType(
                @"public class  DomainTest1{
                        public string Name;
                        public int Operator;
                }");

            Assert.Equal(domain, type.GetDomain());
        }


        [Fact(DisplayName = "委托比域")]
        public void TestDelegateEqual()
        {
            var domain = DomainManagement.Random;
            var action = NDelegate.UseDomain(domain).Action(
                @"int i = 1+1;");
            Assert.Equal(domain, action.GetDomain());
        }
    }
}
