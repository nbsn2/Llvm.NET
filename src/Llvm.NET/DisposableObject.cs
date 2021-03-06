﻿// <copyright file="DisposableObject.cs" company=".NET Foundation">
// Copyright (c) .NET Foundation. All rights reserved.
// </copyright>

using System;

namespace Llvm.NET
{
    /// <summary>Abstract base class for implementing the Disposable pattern</summary>
    public abstract class DisposableObject
        : IDisposable
    {
        /// <summary>Finalizes the object releasing any unmanaged resources it owns</summary>
        ~DisposableObject( )
        {
           Dispose(false);
        }

        /// <inheritdoc/>
        public void Dispose( )
        {
            Dispose( true );
            GC.SuppressFinalize(this);
        }

        /// <summary>Gets a value indicating whether the object is disposed or not</summary>
        public abstract bool IsDisposed { get; }

        /// <summary>Abstract method that is implemented by derived types to perform the dispose operation</summary>
        /// <param name="disposing">Indicates if this is a dispose or finalize operation</param>
        /// <remarks>
        /// This is guaranteed to only be called if <see cref="IsDisposed"/> returns <see langword="false"/>
        /// so the implementation should only be concerned with the actual release of resources. If <paramref name="disposing"/>
        /// is <see langword="true"/> then the implementation should release managed and unmanaged resources, otherwise it should
        /// only release the unmanaged resources
        /// </remarks>
        protected abstract void InternalDispose( bool disposing );

        private void Dispose( bool disposing )
        {
            if( !IsDisposed )
            {
                InternalDispose( disposing );
            }
        }
    }
}
