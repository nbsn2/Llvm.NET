﻿// <copyright file="DINodeArray.cs" company=".NET Foundation">
// Copyright (c) .NET Foundation. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace Llvm.NET.DebugInfo
{
    /// <summary>Array of <see cref="DIMacroNode"/> debug information nodes for use with <see cref="DebugInfoBuilder"/> methods</summary>
    [SuppressMessage( "Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This matches the wrapped native type" )]
    public class DIMacroNodeArray
        : TupleTypedArrayWrapper<DINode>
    {
        internal DIMacroNodeArray( MDTuple tuple )
            : base( tuple )
        {
        }
    }
}
