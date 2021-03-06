﻿// <copyright file="VectorTransforms.cs" company=".NET Foundation">
// Copyright (c) .NET Foundation. All rights reserved.
// </copyright>

using System.Runtime.InteropServices;
using Llvm.NET.Native;

using static Llvm.NET.Native.NativeMethods;

namespace Llvm.NET.Transforms
{
    /// <summary>Extension methods for adding vector transform passes</summary>
    public static class VectorTransforms
    {
        /// <summary>Adds a loop vectorizer pass</summary>
        /// <typeparam name="T"><see cref="PassManager"/> type</typeparam>
        /// <param name="passManager">Pass manager to add the pass to</param>
        /// <returns><paramref name="passManager"/> for fluent style construction of a pass manager</returns>
        public static T AddLoopVectorizePass<T>( this T passManager )
            where T : PassManager
        {
            LLVMAddLoopVectorizePass( passManager.Handle );
            return passManager;
        }

        /// <summary>Adds a bottom-up SLP vectorizer pass</summary>
        /// <typeparam name="T"><see cref="PassManager"/> type</typeparam>
        /// <param name="passManager">Pass manager to add the pass to</param>
        /// <returns><paramref name="passManager"/> for fluent style construction of a pass manager</returns>
        public static T AddSLPVectorizePass<T>( this T passManager )
            where T : PassManager
        {
            LLVMAddSLPVectorizePass( passManager.Handle );
            return passManager;
        }

        [DllImport( LibraryPath, CallingConvention = CallingConvention.Cdecl )]
        private static extern void LLVMAddLoopVectorizePass( LLVMPassManagerRef @PM );

        [DllImport( LibraryPath, CallingConvention = CallingConvention.Cdecl )]
        private static extern void LLVMAddSLPVectorizePass( LLVMPassManagerRef @PM );
    }
}
