﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Natasha.CSharp.Component.Domain;
using Natasha.CSharp.Component.Exception;
using Natasha.CSharp.Extension.Inner;
using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Reflection;

/// <summary>
/// 程序集编译构建器 - 编译选项
/// </summary>
public partial class AssemblyCSharpBuilder 
{

    private LoadBehaviorEnum _compileReferenceBehavior;
    private LoadBehaviorEnum _compileAssemblyBehavior;
    private Func<AssemblyName, AssemblyName, LoadVersionResultEnum>? _referencePickFunc;

    public AssemblyCSharpBuilder CompileWithReferenceLoadBehavior(LoadBehaviorEnum loadBehavior)
    {
        _compileReferenceBehavior = loadBehavior;
        return this;
    }
    public AssemblyCSharpBuilder CompileWithAssemblyLoadBehavior(LoadBehaviorEnum loadBehavior)
    {
        _compileAssemblyBehavior = loadBehavior;
        return this;
    }

    public AssemblyCSharpBuilder CompileWithReferencesFilter(Func<AssemblyName, AssemblyName, LoadVersionResultEnum>? useAssemblyNameFunc = null)
    {
        _referencePickFunc = useAssemblyNameFunc;
        return this;
    }

    /// <summary>
    /// 流编译成功之后触发的事件
    /// </summary>
    public event Action<CSharpCompilation, Assembly>? CompileSucceedEvent;


    /// <summary>
    /// 流编译失败之后触发的事件
    /// </summary>
    public event Action<CSharpCompilation, ImmutableArray<Diagnostic>>? CompileFailedEvent;


    /// <summary>
    /// 将 SyntaxTrees 中的语法树编译到程序集.如果不成功会抛出 NatashaException.
    /// </summary>
    /// <remarks>
    /// <example>
    /// <code>
    /// 
    ///     //程序集的域加载行为, 该行为决定了编译后的程序集随着依赖加载到域中的处理结果.
    ///     //和加载插件原理相同.
    ///     builder.CompileWithAssemblyLoadBehavior(enum);
    ///     
    ///     //编译单元的引用加载行为, 遇到同名不同版本的引用该如何处理.
    ///     builder.CompileWithReferenceLoadBehavior(enum);
    ///     builder.CompileWithReferencesFilter(func);
    /// 
    /// </code>
    /// </example>
    /// </remarks>
    public Assembly GetAssembly()
    {
#if DEBUG
        Stopwatch stopwatch = new();
        stopwatch.Start();
#endif

        //Mark : 26ms
        if (_compileReferenceBehavior == LoadBehaviorEnum.None)
        {
            _compilerOptions.SetSupersedeLowerVersions(true);
        }

        var options = _compilerOptions.GetCompilationOptions();
        var references = Domain.GetReferences(_compileReferenceBehavior, _referencePickFunc);
        var compilation = CSharpCompilation.Create(AssemblyName, SyntaxTrees, references, options);

#if DEBUG
        stopwatch.RestartAndShowCategoreInfo("[Compiler]", "获取编译单元", 2);
#endif


        if (EnableSemanticHandler)
        {
            foreach (var item in _semanticAnalysistor)
            {
                compilation = item(this, compilation);
            }
            lock (SyntaxTrees)
            {
                SyntaxTrees.Clear();
                SyntaxTrees.AddRange(compilation.SyntaxTrees);
            }
        }

#if DEBUG
        stopwatch.RestartAndShowCategoreInfo("[Semantic]", "语义处理", 2);
#endif

        Stream dllStream;
        Stream pdbStream;
        Stream? xmlStream = null;
        if (DllFilePath != string.Empty)
        {
            dllStream = File.Create(DllFilePath);
        }
        else
        {
            dllStream = new MemoryStream();
        }

        if (PdbFilePath != string.Empty)
        {
            pdbStream = File.Create(PdbFilePath);
        }
        else
        {
            pdbStream = new MemoryStream();
        }

        if (XmlFilePath != string.Empty)
        {
            xmlStream = File.Create(XmlFilePath);
        }

        var compileResult = compilation.Emit(
           dllStream,
           pdbStream: pdbStream,
           xmlDocumentationStream: xmlStream,
           options: new EmitOptions(pdbFilePath: PdbFilePath == string.Empty ? null : PdbFilePath, debugInformationFormat: DebugInformationFormat.PortablePdb));


        LogCompilationEvent?.Invoke(compilation.GetNatashaLog());

        Assembly? assembly = null;
        if (compileResult.Success)
        {
            dllStream.Seek(0, SeekOrigin.Begin);
            pdbStream?.Seek(0, SeekOrigin.Begin);
            Domain.SetAssemblyLoadBehavior(_compileAssemblyBehavior);
            assembly = Domain.LoadAssemblyFromStream(dllStream, pdbStream);
            CompileSucceedEvent?.Invoke(compilation, assembly!);
        }
        dllStream.Dispose();
        pdbStream?.Dispose();
        xmlStream?.Dispose();

#if DEBUG 
        stopwatch.StopAndShowCategoreInfo("[  Emit  ]", "编译时长", 2);
#endif

        if (!compileResult.Success)
        {
            CompileFailedEvent?.Invoke(compilation, compileResult.Diagnostics);
            throw NatashaExceptionAnalyzer.GetCompileException(compilation, compileResult.Diagnostics);
        }

        return assembly!;
    }

}



