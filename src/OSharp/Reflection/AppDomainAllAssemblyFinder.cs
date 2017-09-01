﻿// -----------------------------------------------------------------------
//  <copyright file="BinAssemblyFinder.cs" company="OSharp开源团队">
//      Copyright (c) 2014-2017 OSharp. All rights reserved.
//  </copyright>
//  <site>http://www.osharp.org</site>
//  <last-editor>郭明锋</last-editor>
//  <last-date>2017-08-15 23:33</last-date>
// -----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.DependencyModel;

using OSharp.Collections;
using OSharp.Dependency;
using OSharp.Finders;

namespace OSharp.Reflection
{
    /// <summary>
    /// 应用程序目录程序集查找器
    /// </summary>
    public class AppDomainAllAssemblyFinder : FinderBase<Assembly>, IAllAssemblyFinder, ISingletonDependency
    {
        private readonly bool _filterNetAssembly;

        /// <summary>
        /// 初始化一个<see cref="AppDomainAllAssemblyFinder"/>类型的新实例
        /// </summary>
        public AppDomainAllAssemblyFinder(bool filterNetAssembly = true)
        {
            _filterNetAssembly = filterNetAssembly;
        }
        
        /// <summary>
        /// 重写以实现程序集的查找
        /// </summary>
        /// <returns></returns>
        protected override Assembly[] FindAllItems()
        {
            string[] filters =
            {
                "System",
                "Microsoft",
                "netstandard",
                "dotnet",
                "Window",
                "mscorlib"
            };
            DependencyContext context = DependencyContext.Default;
            string[] dllNames = context.CompileLibraries.SelectMany(m => m.Assemblies).Distinct().Select(m => m.Replace(".dll", "")).ToArray();
            string[] names = (from name in dllNames
                let i = name.LastIndexOf('/') + 1
                select name.Substring(i, name.Length - i)).Distinct().ToArray();

            Assembly[] assemblies = names.WhereIf(name => !filters.Any(name.StartsWith), _filterNetAssembly)
                .Select(name => Assembly.Load(new AssemblyName(name)))
                .ToArray();
            return assemblies;
        }
    }
}