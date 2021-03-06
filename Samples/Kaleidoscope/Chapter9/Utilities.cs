﻿// <copyright file="Utilities.cs" company=".NET Foundation">
// Copyright (c) .NET Foundation. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using static Kaleidoscope.Grammar.KaleidoscopeParser;

[assembly: SuppressMessage( "StyleCop.CSharp.DocumentationRules", "SA1652:Enable XML documentation output", Justification = "Sample application" )]

namespace Kaleidoscope
{
    internal static class Utilities
    {
        /// <summary>Waits for a debugger attach</summary>
        /// <remarks>
        /// This is useful for Mixed mode debugging CoreCLR builds as it doesn't directly support
        /// debug launch with mixed debugging. Instead you launch without debugging and call this
        /// at the start of Main() then use the Attach option to attach using both managed and
        /// native. This is a bit of a regression in the project system for the new "SDK" type
        /// .NET projects that will hopefully be resolved in the not too distant future.
        /// <note type="note">
        /// In release builds this is a NOP due to the use of the <see cref="ConditionalAttribute"/>
        /// </note>
        /// </remarks>
        [Conditional( "DEBUG" )]
        internal static void WaitForDebugger( )
        {
            if( !Debugger.IsAttached )
            {
                Console.WriteLine( "Waiting for Debugger attach..." );
            }

            while( !Debugger.IsAttached )
            {
                System.Threading.Thread.Sleep( 5000 );
            }
        }

        internal static string GetPrototypeName( this PrototypeContext prototype )
        {
            switch( prototype )
            {
            case FunctionPrototypeContext func:
                return func.Name;

            case UnaryPrototypeContext unaryOp:
                return $"$unary{unaryOp.Op}";

            case BinaryPrototypeContext binOp:
                return $"$binary{binOp.Op}";

            default:
                throw new ArgumentException( "unknown prototype" );
            }
        }
    }
}
