﻿using Natasha.CSharp.Error.Model;
using System;
using Xunit;

namespace NatashaFunctionUT.Syntax
{
    [Trait("基础功能测试", "Syntax")]
    public class ParseTest
    {
        private NatashaException? CatchTreeError(string code)
        {
            try
            {
                AssemblyCSharpBuilder builder = new();
                builder.Add(code);
            }
            catch (Exception ex)
            {
                return ex as NatashaException;
            }
            return new NatashaException("a");
        }


        [Fact(DisplayName = "语法树异常")]
        public void Formart1()
        {

            var source = @"unsafe class C
{
    delegate * < int,  int> functionPointer 1;
}";

            var expected = @"unsafe class C
{
    delegate*<int, int> functionPointer 1 ;
}";

            var ex = CatchTreeError(source)!;
            Assert.NotNull(ex);
            Assert.Equal(expected, ex.Formatter);
            Assert.Equal(ExceptionKind.Syntax, ex.ErrorKind);
        }
    }
}
