﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Natasha.CSharp.Component.Exception;
using Natasha.CSharp.Core;
using System;
using System.Collections.Generic;

/// <summary>
/// 程序集编译构建器 - 语法树相关
/// </summary>
public partial class AssemblyCSharpBuilder 
{

    public readonly List<SyntaxTree> SyntaxTrees;

    private CSharpParseOptions? _options;
    /// <summary>
    /// 配置语法树选项
    /// </summary>
    /// <param name="cSharpParseOptions"></param>
    /// <returns></returns>
    public AssemblyCSharpBuilder ConfigSyntaxOptions(CSharpParseOptions cSharpParseOptions)
    {
        _options = cSharpParseOptions;
        return this;
    }
    /// <summary>
    /// 配置语法树选项
    /// </summary>
    /// <param name="cSharpParseOptionsAction"></param>
    /// <returns></returns>
    public AssemblyCSharpBuilder ConfigSyntaxOptions(Func<CSharpParseOptions, CSharpParseOptions> cSharpParseOptionsAction)
    {
        _options = cSharpParseOptionsAction(new CSharpParseOptions());
        return this;
    }
    /// <summary>
    /// 添加脚本
    /// </summary>
    /// <param name="script"></param>
    public AssemblyCSharpBuilder Add(string script)
    {
        var tree = NatashaCSharpSyntax.ParseTree(script, _options);
        var exception = NatashaExceptionAnalyzer.GetSyntaxException(tree);
        if (exception != null)
        {
            throw exception;
        }
        else
        {
            lock (SyntaxTrees)
            {
                SyntaxTrees.Add(tree);
            }
        }
        return this;
    }
    /// <summary>
    /// 添加语法树
    /// </summary>
    /// <param name="tree"></param>
    public AssemblyCSharpBuilder Add(SyntaxTree tree)
    {
        tree = NatashaCSharpSyntax.FormartTree(tree, _options);
        var exception = NatashaExceptionAnalyzer.GetSyntaxException(tree);
        if (exception != null)
        {
            throw exception;
        }
        else
        {
            lock (SyntaxTrees)
            {
                SyntaxTrees.Add(tree);
            }
        }
        return this;
    }

}



