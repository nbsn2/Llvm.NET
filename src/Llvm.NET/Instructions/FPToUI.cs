﻿// <copyright file="FPToUI.cs" company=".NET Foundation">
// Copyright (c) .NET Foundation. All rights reserved.
// </copyright>

using Llvm.NET.Native;

namespace Llvm.NET.Instructions
{
    public class FPToUI : Cast
    {
        internal FPToUI( LLVMValueRef valueRef )
            : base( valueRef )
        {
        }
    }
}
