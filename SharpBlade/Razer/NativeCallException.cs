﻿// ---------------------------------------------------------------------------------------
// <copyright file="NativeCallException.cs" company="SharpBlade">
//     Copyright © 2013-2014 by Adam Hellberg and Brandon Scott.
//
//     Permission is hereby granted, free of charge, to any person obtaining a copy of
//     this software and associated documentation files (the "Software"), to deal in
//     the Software without restriction, including without limitation the rights to
//     use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
//     of the Software, and to permit persons to whom the Software is furnished to do
//     so, subject to the following conditions:
//
//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
//
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
//     WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//     CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//     Disclaimer: SharpBlade is in no way affiliated with Razer and/or any of
//     its employees and/or licensors. Adam Hellberg and/or Brandon Scott do not
//     take responsibility for any harm caused, direct or indirect, to any Razer
//     peripherals via the use of SharpBlade.
//
//     "Razer" is a trademark of Razer USA Ltd.
// </copyright>
// ---------------------------------------------------------------------------------------

namespace SharpBlade.Razer
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    using SharpBlade.Annotations;
    using SharpBlade.Native;

    /// <summary>
    /// Exception for failures in native code provided by Razer.
    /// </summary>
    [Serializable]
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors",
        Justification = "This is a special exception type.")]
    public class NativeCallException : SwitchbladeException
    {
        /// <summary>
        /// The name of the native function that failed.
        /// </summary>
        private readonly string _function;

        /// <summary>
        /// <see cref="HRESULT" /> obtained from calling the native function.
        /// </summary>
        private readonly HRESULT _hresult;

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeCallException" /> class.
        /// </summary>
        /// <param name="function">The name of the function that failed.</param>
        /// <param name="hresult"><see cref="HRESULT" /> returned from the native function.</param>
        internal NativeCallException(string function, HRESULT hresult)
            : base(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Call to native RazerAPI function {0} failed with error message: {1}",
                    function,
                    HelperMethods.GetErrorMessage(hresult)),
                HelperMethods.GetWin32Exception(hresult))
        {
            _function = function;
            _hresult = hresult;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeCallException" /> class
        /// from serialization data.
        /// </summary>
        /// <param name="info">Serialization info object.</param>
        /// <param name="context">Streaming context.</param>
        protected NativeCallException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _function = info.GetString("Function");
            _hresult = info.GetInt32("Hresult");
        }

        /// <summary>
        /// Gets the name of the native function that failed.
        /// </summary>
        [PublicAPI]
        public string Function
        {
            get
            {
                return _function;
            }
        }

        /// <summary>
        /// Gets the <see cref="HRESULT" /> obtained from calling the native function.
        /// </summary>
        [CLSCompliant(false)]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Hresult",
            Justification = "This name is derived from native code, so there's not much to do about it.")]
        public new HRESULT HResult
        {
            get
            {
                return _hresult;
            }
        }

        /// <summary>
        /// Adds object data to serialization object.
        /// </summary>
        /// <param name="info">Serialization info object.</param>
        /// <param name="context">Streaming context.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Function", _function);
            info.AddValue("HResult", (int)HResult);
        }
    }
}
