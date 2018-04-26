﻿using System;
using System.Runtime.InteropServices;
using Ultraviolet.Core;

namespace Ultraviolet.FreeType2.Native
{
#pragma warning disable 1591
    [Preserve(AllMembers = true)]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct FT_SizeRec32
    {
        public FT_FaceRec32* face;
        public FT_Generic generic;
        public FT_Size_Metrics32 metrics;
        public IntPtr @internal;
    }
#pragma warning restore 1591
}