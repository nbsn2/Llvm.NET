﻿#define TARGET_CORTEX_M3
using Llvm.NET;
using Llvm.NET.DebugInfo;
using Llvm.NET.Instructions;
using Llvm.NET.Types;
using Llvm.NET.Values;
using System;
using System.IO;

namespace TestDebugInfo
{
    /// <summary>Program to test/demonstrate Aspects of debug information generation with Llvm.NET</summary>
    class Program
    {
        static AttributeSet TargetDependentAttributes { get; set; }

#if TARGET_X86
        const string ModuleName = "test_x86.bc";
        const string Triple = "x86_64-pc-windows-msvc18.0.0";
        const string Cpu = "x86-64";
        const string Features = "+sse,+sse2";

        static AttributeSet BuildTargetDependentAttributes(Context ctx )
        {
            var bldr = new AttributeBuilder();
            
            bldr.Add( "disable-tail-calls", "false" );
            bldr.Add( "less-precise-fpmad", "false" );
            bldr.Add( "no-frame-pointer-elim", "false" );
            bldr.Add( "no-infs-fp-math", "false" );
            bldr.Add( "no-nans-fp-math", "false" );
            bldr.Add( "stack-protector-buffer-size", "8" );
            bldr.Add( "target-cpu", Cpu );
            bldr.Add( "target-features", Features );
            bldr.Add( "unsafe-fp-math", "false" );
            bldr.Add( "use-soft-float", "false" );
        };
#elif TARGET_CORTEX_M3
        const string ModuleName = "test_M3.bc";
        const string Triple = "thumbv7m-none--eabi";
        const string Cpu = "cortex-m3";
        const string Features = "+hwdiv";

        static AttributeSet BuildTargetDependentAttributes(Context ctx )
        {
            var bldr = new AttributeBuilder();
            
            bldr.Add( "disable-tail-calls", "false" );
            bldr.Add( "less-precise-fpmad", "false" );
            bldr.Add( "no-frame-pointer-elim", "true" );
            bldr.Add( "no-frame-pointer-elim-non-leaf" );
            bldr.Add( "no-infs-fp-math", "false" );
            bldr.Add( "no-nans-fp-math", "false" );
            bldr.Add( "stack-protector-buffer-size", "8" );
            bldr.Add( "target-cpu", Cpu );
            bldr.Add( "target-features", Features );
            bldr.Add( "unsafe-fp-math", "false" );
            bldr.Add( "use-soft-float", "false" );
            return bldr.ToAttributeSet( FunctionAttributeIndex.Function, ctx );
        }
#endif
        // obviously this is not clang but using an identical name helps in diff with actual clang output
        const string VersionIdentString = "clang version 3.7.0 (tags/RELEASE_370/final)";

        /// <summary>Creates a test LLVM module with debug information</summary>
        /// <param name="args">ignored</param>
        /// <remarks>
        /// <code language="c" title="Example code generated" source="test.c" />
        /// </remarks>
        static void Main( string[ ] args )
        {
            var srcPath = args[0];
            if( !File.Exists( srcPath ) )
            {
                Console.Error.WriteLine( "Src file not found: '{0}'", srcPath );
                return;
            }
            srcPath = Path.GetFullPath( srcPath );

            StaticState.RegisterAll( );
            var target = Target.FromTriple( Triple );
            using( var context = new Context( ) )
            using( var targetMachine = target.CreateTargetMachine( context, Triple, Cpu, Features, CodeGenOpt.Aggressive, Reloc.Default, CodeModel.Small ) )
            using( var module = new NativeModule( ModuleName, context ) )
            {
                TargetDependentAttributes = BuildTargetDependentAttributes( context );
                var targetData = targetMachine.TargetData;

                module.TargetTriple = targetMachine.Triple;
                module.DataLayoutString = targetMachine.TargetData.ToString( );

                // create compile unit and file as the top level scope for everything
                var cu = module.DIBuilder.CreateCompileUnit( SourceLanguage.C99
                                                           , Path.GetFileName( srcPath )
                                                           , Path.GetDirectoryName( srcPath )
                                                           , VersionIdentString
                                                           , false
                                                           , ""
                                                           , 0
                                                           );
                var diFile = module.DIBuilder.CreateFile( srcPath );

                // Create basic types used in this compilation
                var i32 = new DebugBasicType( module.Context.Int32Type, module, "int", DiTypeKind.Signed );
                var f32 = new DebugBasicType( module.Context.FloatType, module, "float", DiTypeKind.Float );
                var voidType = DebugType.Create( module.Context.VoidType, ( DIType )null );
                var i32Array_0_32 = i32.CreateArrayType( module, 0, 32 );

                // create the LLVM structure type and body
                // The full body is used with targetMachine.TargetData to determine size, alignment and element offsets
                // in a target independent manner.
                var fooType = new DebugStructType( module, "struct.foo", cu, "foo" );

                // Create concrete debug type with full debug information
                var fooBody = new[ ]
                    { new DebugMemberInfo { File = diFile, Line = 3, Name = "a", DebugType = i32, Index = 0 }
                    , new DebugMemberInfo { File = diFile, Line = 4, Name = "b", DebugType = f32, Index = 1 }
                    , new DebugMemberInfo { File = diFile, Line = 5, Name = "c", DebugType = i32Array_0_32, Index = 2 }
                    };
                fooType.SetBody( false, module, cu, diFile, 1, 0, fooBody );

                // add global variables and constants
                var constArray = ConstantArray.From( i32, 32, module.Context.CreateConstant( 3 ), module.Context.CreateConstant( 4 ) );
                var barValue = module.Context.CreateNamedConstantStruct( fooType
                                                                       , module.Context.CreateConstant( 1 )
                                                                       , module.Context.CreateConstant( 2.0f )
                                                                       , constArray
                                                                       );

                var bar = module.AddGlobal( fooType, false, 0, barValue, "bar" );
                bar.Alignment = targetData.AbiAlignmentOf( fooType );
                module.DIBuilder.CreateGlobalVariable( cu, "bar", string.Empty, diFile, 8, fooType.DIType, false, bar );

                var baz = module.AddGlobal( fooType, false, Linkage.Common, Constant.NullValueFor( fooType ), "baz" );
                baz.Alignment = targetData.AbiAlignmentOf( fooType );
                module.DIBuilder.CreateGlobalVariable( cu, "baz", string.Empty, diFile, 9, fooType.DIType, false, baz );

                // add module flags and compiler identifiers...
                // this can technically occur at any point, though placing it here makes
                // comparing against clang generated files possible
                AddModuleFlags( module );

                // create types for function args
                var constFoo = module.DIBuilder.CreateQualifiedType( fooType.DIType, QualifiedTypeTag.Const );
                var fooPtr = new DebugPointerType( fooType, module );

                Function doCopyFunc = DeclareDoCopyFunc( module, diFile, voidType );

                Function copyFunc = DeclareCopyFunc( module, diFile, voidType, constFoo, fooPtr );

                CreateCopyFunctionBody( module, targetData, copyFunc, diFile, fooType, fooPtr, constFoo );
                CreateDoCopyFunctionBody( module, targetData, doCopyFunc, fooType, bar, baz, copyFunc );

                // finalize the debug information
                // all temporaries must be replaced by now, this resolves any remaining
                // forward declarations and marks the builder to prevent adding any
                // nodes that are not completely resolved.
                module.DIBuilder.Finish( );

                // verify the module is still good and print any errors found
                string msg;
                if( !module.Verify( out msg ) )
                {
                    Console.Error.WriteLine( "ERROR: {0}", msg );
                }
                else
                {
                    // Module is good, so generate the output files
                    module.WriteToFile( "test.bc" );
                    File.WriteAllText( "test.ll", module.AsString( ) );
                    targetMachine.EmitToFile( module, "test.o", CodeGenFileType.ObjectFile );
                    targetMachine.EmitToFile( module, "test.s", CodeGenFileType.AssemblySource );
                }
            }
        }

        private static Function DeclareDoCopyFunc( NativeModule module, DIFile diFile, IDebugType<ITypeRef, DIType> voidType )
        {
            var doCopySig = module.Context.CreateFunctionType( module.DIBuilder, voidType );

            // Create the functions
            // NOTE: The declaration ordering is reversed from that of the sample code file (test.c)
            //       However, this is what Clang ends up doing for some reason so it is
            //       replicated here to aid in comparing the generated LL files.
            var doCopyFunc = module.CreateFunction( scope: diFile
                                                  , name: "DoCopy"
                                                  , linkageName: null
                                                  , file: diFile
                                                  , line: 23
                                                  , signature: doCopySig
                                                  , isLocalToUnit: false
                                                  , isDefinition: true
                                                  , scopeLine: 24
                                                  , debugFlags: DebugInfoFlags.None
                                                  , isOptimized: false
                                                  ).AddAttributes( AttributeKind.NoUnwind )
#if TARGET_X86
                                                   .AddAttributes( AttributeKind.UWTable )
#endif
                                                   .AddAttributes(FunctionAttributeIndex.Function, TargetDependentAttributes );
            return doCopyFunc;
        }

        private static Function DeclareCopyFunc( NativeModule module
                                               , DIFile diFile
                                               , IDebugType<ITypeRef, DIType> voidType
                                               , DIDerivedType constFoo
                                               , DebugPointerType fooPtr
                                               )
        {
            // Since the first parameter is passed by value 
            // using the pointer + alloca + memcopy pattern, the actual
            // source, and therefore debug, signature is NOT a pointer.
            // However, that usage would create a signature with two
            // pointers as the arguments, which doesn't match the source
            // To get the correct debug info signature this inserts an
            // explicit DebugType<> that overrides the default behavior
            // to pair the LLVM pointer type with the original source type.
            var copySig = module.Context.CreateFunctionType( module.DIBuilder
                                                           , voidType
                                                           , DebugType.Create( fooPtr, constFoo )
                                                           , fooPtr
                                                           );

            var copyFunc = module.CreateFunction( scope: diFile
                                                , name: "copy"
                                                , linkageName: null
                                                , file: diFile
                                                , line: 11
                                                , signature: copySig
                                                , isLocalToUnit: true
                                                , isDefinition: true
                                                , scopeLine: 14
                                                , debugFlags: DebugInfoFlags.Prototyped
                                                , isOptimized: false
                                                ).Linkage( Linkage.Internal ) // static function
                                                 .AddAttributes( AttributeKind.NoUnwind, AttributeKind.InlineHint )
#if TARGET_X86
                                                 .AddAttributes( AttributeKind.UWTable )
#endif
                                                 .AddAttributes( FunctionAttributeIndex.Function, TargetDependentAttributes );

            // ByVal pointers indicate by value semantics. The actual semantics are along the lines of
            // "pass the arg as copy on the arguments stack and set parameter implicitly to that copy's address"
            // (src: https://github.com/ldc-developers/ldc/issues/937 )
            //
            // LLVM recognizes this pattern and has a pass to map to an efficient register usage whenever plausible.
            // Though it seems Clang doesn't apply the attribute in all cases, for x86 it doesn't appear to ever use
            // it, for Cortex-Mx it seems to use it only for larger structs, otherwise it uses an [ n x i32]. Thus,
            // on cortex-m the function parameters are handled quite differently by clang, which seems odd...
#if TARGET_CORTEX_M3            
            copyFunc.AddAttributes( FunctionAttributeIndex.Parameter0
                                  , AttributeKind.ByVal
                                  , new AttributeValue(AttributeKind.Alignment, module.Layout.AbiAlignmentOf( fooPtr.ElementType ) )
                                  );
#endif

            return copyFunc;
        }

        private static void AddModuleFlags( NativeModule module )
        {
            module.AddModuleFlag( ModuleFlagBehavior.Warning, NativeModule.DwarfVersionValue, 4 );
            module.AddModuleFlag( ModuleFlagBehavior.Warning, NativeModule.DebugVersionValue, NativeModule.DebugMetadataVersion );
#if TARGET_X86
                module.AddModuleFlag( ModuleFlagBehavior.Error, "PIC Level", 2 );
#elif TARGET_CORTEX_M3
            // Specify ABI const sizes so linker can detect mismatches
            module.AddModuleFlag( ModuleFlagBehavior.Error, "wchar_size", 4 );
            module.AddModuleFlag( ModuleFlagBehavior.Error, "min_enum_size", 4 );
#endif
            module.AddVersionIdentMetadata( VersionIdentString );
        }

        private static void CreateCopyFunctionBody( NativeModule module
                                                  , DataLayout layout
                                                  , Function copyFunc
                                                  , DIFile diFile
                                                  , ITypeRef foo
                                                  , DebugPointerType fooPtr
                                                  , DIType constFooType
                                                  )
        {
            var diBuilder = module.DIBuilder;

            copyFunc.Parameters[ 0 ].Name = "src";
            copyFunc.Parameters[ 1 ].Name = "pDst";

            // create block for the function body, only need one for this simple sample
            var blk = copyFunc.AppendBasicBlock( "entry" );

            // create instruction builder to build the body
            var instBuilder = new InstructionBuilder( blk );

            // create debug info locals for the arguments
            // NOTE: Debug parameter indices are 1 based!
            var paramSrc = diBuilder.CreateArgument( copyFunc.DISubProgram, "src", diFile, 11, constFooType, false, 0, 1 );
            var paramDst = diBuilder.CreateArgument( copyFunc.DISubProgram, "pDst", diFile, 12, fooPtr.DIType, false, 0, 2 );

            var ptrAlign = layout.CallFrameAlignmentOf( fooPtr );
            
            // create Locals
            // NOTE: There's no debug location attached to these instructions.
            //       The debug info will come from the declare intrinsic below.
            var dstAddr = instBuilder.Alloca( fooPtr )
                                     .RegisterName( "pDst.addr" )
                                     .Alignment( ptrAlign );
#if TARGET_CORTEX_M3
            diBuilder.InsertDeclare( copyFunc.Parameters[ 0 ]
                                   , paramSrc
                                   , new DILocation( module.Context, 11, 43, copyFunc.DISubProgram )
                                   , blk
                                   );
#endif
            instBuilder.Store( copyFunc.Parameters[ 1 ], dstAddr)
                       .Alignment( ptrAlign );

            // insert declare pseudo instruction to attach debug info to the local declarations
            diBuilder.InsertDeclare( dstAddr, paramDst, new DILocation( module.Context, 12, 38, copyFunc.DISubProgram ), blk );

#if TARGET_X86
            // since the function's LLVM signature uses a pointer, which is copied locally
            // inform the debugger to treat it as the value by dereferencing the pointer
            diBuilder.InsertDeclare( copyFunc.Parameters[ 0 ]
                                   , paramSrc
                                   , diBuilder.CreateExpression( ExpressionOp.deref )
                                   , new DILocation( module.Context, 11, 43, copyFunc.DISubProgram )
                                   , blk
                                   );
#endif

            var loadedDst = instBuilder.Load( dstAddr )
                                       .Alignment( ptrAlign )
                                       .SetDebugLocation( 15, 6, copyFunc.DISubProgram );

            var dstPtr = instBuilder.BitCast( loadedDst, module.Context.Int8Type.CreatePointerType( ) )
                                    .SetDebugLocation( 15, 13, copyFunc.DISubProgram );

            var srcPtr = instBuilder.BitCast( copyFunc.Parameters[ 0 ], module.Context.Int8Type.CreatePointerType( ) )
                                    .SetDebugLocation( 15, 13, copyFunc.DISubProgram );

#if TARGET_X86
            instBuilder.MemCpy( module
                              , dstPtr
                              , srcPtr
                              , module.Context.CreateConstant( layout.ByteSizeOf( foo ) )
                              , ( int )layout.AbiAlignmentOf( foo )
                              , false
                              ).SetDebugLocation( 15, 13, copyFunc.DISubProgram );
#elif TARGET_CORTEX_M3
            // Cortex-M uses 32bit count parameter (constant for parameter 3)
            instBuilder.MemCpy( module
                              , dstPtr
                              , srcPtr
                              , module.Context.CreateConstant( (int)layout.ByteSizeOf( foo ) )
                              , ( int )layout.AbiAlignmentOf( foo )
                              , false
                              ).SetDebugLocation( 15, 13, copyFunc.DISubProgram );
#endif
            instBuilder.Return( )
                       .SetDebugLocation( 16, 1, copyFunc.DISubProgram );
        }

        private static void CreateDoCopyFunctionBody( NativeModule module
                                                    , DataLayout layout
                                                    , Function doCopyFunc
                                                    , IStructType foo
                                                    , GlobalVariable bar
                                                    , GlobalVariable baz
                                                    , Function copyFunc
                                                    )
        {
            var bytePtrType = module.Context.Int8Type.CreatePointerType( );

            // create block for the function body, only need one for this simple sample
            var blk = doCopyFunc.AppendBasicBlock( "entry" );

            // create instruction builder to build the body
            var instBuilder = new InstructionBuilder( blk );

#if TARGET_x86
            // create a temp local copy of the global structure
            var dstAddr = instBuilder.Alloca( foo )
                                     .RegisterName( "agg.tmp" )
                                     .Alignment( layout.CallFrameAlignmentOf( foo ) );

            var bitCastDst = instBuilder.BitCast( dstAddr, bytePtrType )
                                        .SetDebugLocation( 25, 11, doCopyFunc.DISubProgram );

            var bitCastSrc = instBuilder.BitCast( bar, bytePtrType )
                                        .SetDebugLocation( 25, 11, doCopyFunc.DISubProgram );

            instBuilder.MemCpy( module
                              , bitCastDst
                              , bitCastSrc
                              , module.Context.CreateConstant( layout.ByteSizeOf( foo ) )
                              , ( int )layout.CallFrameAlignmentOf( foo )
                              , false
                              ).SetDebugLocation( 25, 11, doCopyFunc.DISubProgram );

            instBuilder.Call( copyFunc, dstAddr, baz )
                       .SetDebugLocation( 25, 5, doCopyFunc.DISubProgram );
#elif TARGET_CORTEX_M3
            instBuilder.Call( copyFunc, bar, baz )
                       .SetDebugLocation( 25, 5, doCopyFunc.DISubProgram )
                       .AddAttributes( FunctionAttributeIndex.Parameter0, copyFunc.Attributes.ParameterAttributes( 0 ) );
#endif
            instBuilder.Return( )
                       .SetDebugLocation( 26, 1, doCopyFunc.DISubProgram );
        }
    }
}
