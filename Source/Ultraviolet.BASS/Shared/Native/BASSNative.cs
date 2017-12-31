﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using Ultraviolet.Core;
using Ultraviolet.Core.Native;

namespace Ultraviolet.BASS.Native
{
    internal delegate UInt32 StreamProc(UInt32 handle, IntPtr buffer, UInt32 length, IntPtr user);

    internal delegate void SyncProc(UInt32 handle, UInt32 channel, UInt32 data, IntPtr user);

    [SuppressUnmanagedCodeSecurity]
    internal static unsafe class BASSNative
    {
        // NOTE: The #ifdefs everywhere are necessary because I haven't yet found a way to make
        // the new dynamic loader work on mobile platforms, particularly Android, where dlopen()
        // sometimes maps the same library to multiple address spaces for reasons that I haven't
        // yet been able to discern. My hope is that if the proposed .NET Standard API for dynamic
        // library loading ever makes it to Xamarin Android/iOS, we can standardize all supported
        // platforms on a single declaration type. For now, though, this nonsense seems necessary.

#if ANDROID
        const String LIBRARY = "bass";
#elif IOS
        const String LIBRARY = "__Internal";
#else
        private static readonly NativeLibrary lib = new NativeLibrary(
            UltravioletPlatformInfo.CurrentPlatform == UltravioletPlatform.Windows ? "bass" : "libbass");
#endif

        public static readonly IntPtr STREAMPROC_DUMMY = new IntPtr(0);
        public static readonly IntPtr STREAMPROC_PUSH = new IntPtr(-1);

        public const UInt32 BASS_OK = 0;
        public const UInt32 BASS_ERROR_MEM = 1;
        public const UInt32 BASS_ERROR_FILEOPEN = 2;
        public const UInt32 BASS_ERROR_DRIVER = 3;
        public const UInt32 BASS_ERROR_BUFLOST = 4;
        public const UInt32 BASS_ERROR_HANDLE = 5;
        public const UInt32 BASS_ERROR_FORMAT = 6;
        public const UInt32 BASS_ERROR_POSITION = 7;
        public const UInt32 BASS_ERROR_INIT = 8;
        public const UInt32 BASS_ERROR_START = 9;
        public const UInt32 BASS_ERROR_ALREADY = 14;
        public const UInt32 BASS_ERROR_NOCHAN = 18;
        public const UInt32 BASS_ERROR_ILLTYPE = 19;
        public const UInt32 BASS_ERROR_ILLPARAM = 20;
        public const UInt32 BASS_ERROR_NO3D = 21;
        public const UInt32 BASS_ERROR_NOEAX = 22;
        public const UInt32 BASS_ERROR_DEVICE = 23;
        public const UInt32 BASS_ERROR_NOPLAY = 24;
        public const UInt32 BASS_ERROR_FREQ = 25;
        public const UInt32 BASS_ERROR_NOTFILE = 27;
        public const UInt32 BASS_ERROR_NOHW = 29;
        public const UInt32 BASS_ERROR_EMPTY = 31;
        public const UInt32 BASS_ERROR_NONET = 32;
        public const UInt32 BASS_ERROR_CREATE = 33;
        public const UInt32 BASS_ERROR_NOFX = 34;
        public const UInt32 BASS_ERROR_NOTAVAIL = 37;
        public const UInt32 BASS_ERROR_DECODE = 38;
        public const UInt32 BASS_ERROR_DX = 39;
        public const UInt32 BASS_ERROR_TIMEOUT = 40;
        public const UInt32 BASS_ERROR_FILEFORM = 41;
        public const UInt32 BASS_ERROR_SPEAKER = 42;
        public const UInt32 BASS_ERROR_VERSION = 43;
        public const UInt32 BASS_ERROR_CODEC = 44;
        public const UInt32 BASS_ERROR_ENDED = 45;
        public const UInt32 BASS_ERROR_BUSY = 46;
        public const UInt32 BASS_ERROR_UNKNOWN = unchecked((UInt32)(-1));

        public const UInt32 BASS_ACTIVE_STOPPED = 0;
        public const UInt32 BASS_ACTIVE_PLAYING = 1;
        public const UInt32 BASS_ACTIVE_STALLED = 2;
        public const UInt32 BASS_ACTIVE_PAUSED = 3;

        public const UInt32 BASS_STREAMPROC_END = 0x80000000;

        public const UInt32 BASS_STREAM_AUTOFREE = 0x40000;
        public const UInt32 BASS_STREAM_DECODE = 0x200000;

        public const UInt32 BASS_FX_FREESOURCE = 0x10000;

        public const UInt32 BASS_SAMPLE_LOOP = 4;
        public const UInt32 BASS_SAMPLE_OVER_VOL = 0x10000;
        public const UInt32 BASS_SAMPLE_OVER_POS = 0x20000;
        public const UInt32 BASS_SAMPLE_OVER_DIST = 0x30000;

        public const UInt32 BASS_TAG_ID3 = 0;
        public const UInt32 BASS_TAG_ID3V2 = 1;
        public const UInt32 BASS_TAG_OGG = 2;
        public const UInt32 BASS_TAG_HTTP = 3;
        public const UInt32 BASS_TAG_ICY = 4;
        public const UInt32 BASS_TAG_META = 5;
        public const UInt32 BASS_TAG_APE = 6;
        public const UInt32 BASS_TAG_MP4 = 7;
        public const UInt32 BASS_TAG_WMA = 8;
        public const UInt32 BASS_TAG_VENDOR = 9;
        public const UInt32 BASS_TAG_LYRICS3 = 10;
        public const UInt32 BASS_TAG_CA_CODEC = 11;
        public const UInt32 BASS_TAG_MF = 13;
        public const UInt32 BASS_TAG_WAVEFORMAT = 14;
        public const UInt32 BASS_TAG_RIFF_INFO = 0x100;
        public const UInt32 BASS_TAG_RIFF_BEXT = 0x101;
        public const UInt32 BASS_TAG_RIFF_CART = 0x102;
        public const UInt32 BASS_TAG_RIFF_DISP = 0x103;
        public const UInt32 BASS_TAG_APE_BINARY = 0x1000;
        public const UInt32 BASS_TAG_MUSIC_NAME = 0x10000;
        public const UInt32 BASS_TAG_MUSIC_MESSAGE = 0x10001;
        public const UInt32 BASS_TAG_MUSIC_ORDERS = 0x10002;
        public const UInt32 BASS_TAG_MUSIC_AUTH = 0x10003;
        public const UInt32 BASS_TAG_MUSIC_INST = 0x10100;
        public const UInt32 BASS_TAG_MUSIC_SAMPLE = 0x10300;

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_ErrorGetCode", CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 ErrorGetCode();
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate Int32 BASS_ErrorGetCodeDelegate();
        private static readonly BASS_ErrorGetCodeDelegate pBASS_ErrorGetCode = lib.LoadFunction<BASS_ErrorGetCodeDelegate>("BASS_ErrorGetCode");
        public static Int32 ErrorGetCode() => pBASS_ErrorGetCode();
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_Init", CallingConvention = CallingConvention.StdCall)]
        public static extern Boolean Init(Int32 device, UInt32 freq, UInt32 flags, IntPtr win, IntPtr clsid);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate Boolean BASS_InitDelegate(Int32 device, UInt32 freq, UInt32 flags, IntPtr win, IntPtr clsid);
        private static readonly BASS_InitDelegate pBASS_Init = lib.LoadFunction<BASS_InitDelegate>("BASS_Init");
        public static Boolean Init(Int32 device, UInt32 freq, UInt32 flags, IntPtr win, IntPtr clsid) => pBASS_Init(device, freq, flags, win, clsid);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_Free", CallingConvention = CallingConvention.StdCall)]
        public static extern Boolean Free();
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate Boolean BASS_FreeDelegate();
        private static readonly BASS_FreeDelegate pBASS_Free = lib.LoadFunction<BASS_FreeDelegate>("BASS_Free");
        public static Boolean Free() => pBASS_Free();
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_Update", CallingConvention = CallingConvention.StdCall)]
        public static extern Boolean Update(UInt32 length);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate Boolean BASS_UpdateDelegate(UInt32 length);
        private static readonly BASS_UpdateDelegate pBASS_Update = lib.LoadFunction<BASS_UpdateDelegate>("BASS_Update");
        public static Boolean Update(UInt32 length) => pBASS_Update(length);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_PluginLoad", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 PluginLoad([MarshalAs(UnmanagedType.LPStr)] String file, UInt32 flags);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate UInt32 BASS_PluginLoadDelegate([MarshalAs(UnmanagedType.LPStr)] String file, UInt32 flags);
        private static readonly BASS_PluginLoadDelegate pBASS_PluginLoad = lib.LoadFunction<BASS_PluginLoadDelegate>("BASS_PluginLoad");
        public static UInt32 PluginLoad([MarshalAs(UnmanagedType.LPStr)] String file, UInt32 flags) => pBASS_PluginLoad(file, flags);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_PluginFree", CallingConvention = CallingConvention.StdCall)]
        public static extern Boolean PluginFree(UInt32 handle);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate Boolean BASS_PluginFreeDelegate(UInt32 handle);
        private static readonly BASS_PluginFreeDelegate pBASS_PluginFree = lib.LoadFunction<BASS_PluginFreeDelegate>("BASS_PluginFree");
        public static Boolean PluginFree(UInt32 handle) => pBASS_PluginFree(handle);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_GetConfig", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 GetConfig(BASSConfig option);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate UInt32 BASS_GetConfigDelegate(BASSConfig option);
        private static readonly BASS_GetConfigDelegate pBASS_GetConfig = lib.LoadFunction<BASS_GetConfigDelegate>("BASS_GetConfig");
        public static UInt32 GetConfig(BASSConfig option) => pBASS_GetConfig(option);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_SetConfig", CallingConvention = CallingConvention.StdCall)]
        public static extern Boolean SetConfig(BASSConfig option, UInt32 value);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate Boolean BASS_SetConfigDelegate(BASSConfig option, UInt32 value);
        private static readonly BASS_SetConfigDelegate pBASS_SetConfig = lib.LoadFunction<BASS_SetConfigDelegate>("BASS_SetConfig");
        public static Boolean SetConfig(BASSConfig option, UInt32 value) => pBASS_SetConfig(option, value);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_GetVolume", CallingConvention = CallingConvention.StdCall)]
        public static extern Single GetVolume();
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate Single BASS_GetVolumeDelegate();
        private static readonly BASS_GetVolumeDelegate pBASS_GetVolume = lib.LoadFunction<BASS_GetVolumeDelegate>("BASS_GetVolume");
        public static Single GetVolume() => pBASS_GetVolume();
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_SetVolume", CallingConvention = CallingConvention.StdCall)]
        public static extern Boolean SetVolume(Single volume);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate Boolean BASS_SetVolumeDelegate(Single volume);
        private static readonly BASS_SetVolumeDelegate pBASS_SetVolume = lib.LoadFunction<BASS_SetVolumeDelegate>("BASS_SetVolume");
        public static Boolean SetVolume(Single volume) => pBASS_SetVolume(volume);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_StreamCreate", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 StreamCreate(UInt32 freq, UInt32 chans, UInt32 flags, IntPtr proc, IntPtr user);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate UInt32 BASS_StreamCreateDelegate(UInt32 freq, UInt32 chans, UInt32 flags, IntPtr proc, IntPtr user);
        private static readonly BASS_StreamCreateDelegate pBASS_StreamCreate = lib.LoadFunction<BASS_StreamCreateDelegate>("BASS_StreamCreate");
        public static UInt32 StreamCreate(UInt32 freq, UInt32 chans, UInt32 flags, IntPtr proc, IntPtr user) => pBASS_StreamCreate(freq, chans, flags, proc, user);
#endif
        public static UInt32 StreamCreate(UInt32 freq, UInt32 chans, UInt32 flags, StreamProc proc, IntPtr user) => StreamCreate(freq, chans, flags, Marshal.GetFunctionPointerForDelegate(proc), user);

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_StreamCreateFile", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 StreamCreateFile(Boolean mem, [MarshalAs(UnmanagedType.LPStr)] String file, UInt64 offset, UInt64 length, UInt32 flags);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate UInt32 BASS_StreamCreateFileDelegate(Boolean mem, [MarshalAs(UnmanagedType.LPStr)] String file, UInt64 offset, UInt64 length, UInt32 flags);
        private static readonly BASS_StreamCreateFileDelegate pBASS_StreamCreateFile = lib.LoadFunction<BASS_StreamCreateFileDelegate>("BASS_StreamCreateFile");
        public static UInt32 StreamCreateFile(Boolean mem, [MarshalAs(UnmanagedType.LPStr)] String file, UInt64 offset, UInt64 length, UInt32 flags) => pBASS_StreamCreateFile(mem, file, offset, length, flags);
#endif
        public static UInt32 StreamCreateFile(String file, UInt32 flags) => StreamCreateFile(false, file, 0, 0, flags);

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_StreamCreateFileUser", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 StreamCreateFileUser(UInt32 system, UInt32 flags, BASS_FILEPROCS* procs, IntPtr user);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate UInt32 BASS_StreamCreateFileUserDelegate(UInt32 system, UInt32 flags, BASS_FILEPROCS* procs, IntPtr user);
        private static readonly BASS_StreamCreateFileUserDelegate pBASS_StreamCreateFileUser = lib.LoadFunction<BASS_StreamCreateFileUserDelegate>("BASS_StreamCreateFileUser");
        public static UInt32 StreamCreateFileUser(UInt32 system, UInt32 flags, BASS_FILEPROCS* procs, IntPtr user) => pBASS_StreamCreateFileUser(system, flags, procs, user);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_StreamPutData", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 StreamPutData(UInt32 handle, IntPtr buffer, UInt32 length);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate UInt32 BASS_StreamPutDataDelegate(UInt32 handle, IntPtr buffer, UInt32 length);
        private static readonly BASS_StreamPutDataDelegate pBASS_StreamPutData = lib.LoadFunction<BASS_StreamPutDataDelegate>("BASS_StreamPutData");
        public static UInt32 StreamPutData(UInt32 handle, IntPtr buffer, UInt32 length) => pBASS_StreamPutData(handle, buffer, length);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_StreamFree", CallingConvention = CallingConvention.StdCall)]
        public static extern Boolean StreamFree(UInt32 handle);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate Boolean BASS_StreamFreeDelegate(UInt32 handle);
        private static readonly BASS_StreamFreeDelegate pBASS_StreamFree = lib.LoadFunction<BASS_StreamFreeDelegate>("BASS_StreamFree");
        public static Boolean StreamFree(UInt32 handle) => pBASS_StreamFree(handle);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_ChannelIsActive", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 ChannelIsActive(UInt32 handle);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate UInt32 BASS_ChannelIsActiveDelegate(UInt32 handle);
        private static readonly BASS_ChannelIsActiveDelegate pBASS_ChannelIsActive = lib.LoadFunction<BASS_ChannelIsActiveDelegate>("BASS_ChannelIsActive");
        public static UInt32 ChannelIsActive(UInt32 handle) => pBASS_ChannelIsActive(handle);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_ChannelIsSliding", CallingConvention = CallingConvention.StdCall)]
        public static extern Boolean ChannelIsSliding(UInt32 handle, BASSAttrib attrib);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate Boolean BASS_ChannelIsSlidingDelegate(UInt32 handle, BASSAttrib attrib);
        private static readonly BASS_ChannelIsSlidingDelegate pBASS_ChannelIsSliding = lib.LoadFunction<BASS_ChannelIsSlidingDelegate>("BASS_ChannelIsSliding");
        public static Boolean ChannelIsSliding(UInt32 handle, BASSAttrib attrib) => pBASS_ChannelIsSliding(handle, attrib);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_ChannelFlags", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 ChannelFlags(UInt32 handle, UInt32 flags, UInt32 mask);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate UInt32 BASS_ChannelFlagsDelegate(UInt32 handle, UInt32 flags, UInt32 mask);
        private static readonly BASS_ChannelFlagsDelegate pBASS_ChannelFlags = lib.LoadFunction<BASS_ChannelFlagsDelegate>("BASS_ChannelFlags");
        public static UInt32 ChannelFlags(UInt32 handle, UInt32 flags, UInt32 mask) => pBASS_ChannelFlags(handle, flags, mask);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_ChannelGetInfo", CallingConvention = CallingConvention.StdCall)]
        public static extern Boolean ChannelGetInfo(UInt32 handle, out BASS_CHANNELINFO info);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate Boolean BASS_ChannelGetInfoDelegate(UInt32 handle, out BASS_CHANNELINFO info);
        private static readonly BASS_ChannelGetInfoDelegate pBASS_ChannelGetInfo = lib.LoadFunction<BASS_ChannelGetInfoDelegate>("BASS_ChannelGetInfo");
        public static Boolean ChannelGetInfo(UInt32 handle, out BASS_CHANNELINFO info) => pBASS_ChannelGetInfo(handle, out info);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_ChannelBytes2Seconds", CallingConvention = CallingConvention.StdCall)]
        public static extern Double ChannelBytes2Seconds(UInt32 handle, UInt64 pos);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate Double BASS_ChannelBytes2SecondsDelegate(UInt32 handle, UInt64 pos);
        private static readonly BASS_ChannelBytes2SecondsDelegate pBASS_ChannelBytes2Seconds = lib.LoadFunction<BASS_ChannelBytes2SecondsDelegate>("BASS_ChannelBytes2Seconds");
        public static Double ChannelBytes2Seconds(UInt32 handle, UInt64 pos) => pBASS_ChannelBytes2Seconds(handle, pos);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_ChannelSeconds2Bytes", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt64 ChannelSeconds2Bytes(UInt32 handle, Double pos);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate UInt64 BASS_ChannelSeconds2BytesDelegate(UInt32 handle, Double pos);
        private static readonly BASS_ChannelSeconds2BytesDelegate pBASS_ChannelSeconds2Bytes = lib.LoadFunction<BASS_ChannelSeconds2BytesDelegate>("BASS_ChannelSeconds2Bytes");
        public static UInt64 ChannelSeconds2Bytes(UInt32 handle, Double pos) => pBASS_ChannelSeconds2Bytes(handle, pos);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_ChannelUpdate", CallingConvention = CallingConvention.StdCall)]
        public static extern Boolean ChannelUpdate(UInt32 handle, UInt32 length);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate Boolean BASS_ChannelUpdateDelegate(UInt32 handle, UInt32 length);
        private static readonly BASS_ChannelUpdateDelegate pBASS_ChannelUpdate = lib.LoadFunction<BASS_ChannelUpdateDelegate>("BASS_ChannelUpdate");
        public static Boolean ChannelUpdate(UInt32 handle, UInt32 length) => pBASS_ChannelUpdate(handle, length);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_ChannelPlay", CallingConvention = CallingConvention.StdCall)]
        public static extern Boolean ChannelPlay(UInt32 handle, Boolean restart);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate Boolean BASS_ChannelPlayDelegate(UInt32 handle, Boolean restart);
        private static readonly BASS_ChannelPlayDelegate pBASS_ChannelPlay = lib.LoadFunction<BASS_ChannelPlayDelegate>("BASS_ChannelPlay");
        public static Boolean ChannelPlay(UInt32 handle, Boolean restart) => pBASS_ChannelPlay(handle, restart);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_ChannelStop", CallingConvention = CallingConvention.StdCall)]
        public static extern Boolean ChannelStop(UInt32 handle);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate Boolean BASS_ChannelStopDelegate(UInt32 handle);
        private static readonly BASS_ChannelStopDelegate pBASS_ChannelStop = lib.LoadFunction<BASS_ChannelStopDelegate>("BASS_ChannelStop");
        public static Boolean ChannelStop(UInt32 handle) => pBASS_ChannelStop(handle);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_ChannelPause", CallingConvention = CallingConvention.StdCall)]
        public static extern Boolean ChannelPause(UInt32 handle);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate Boolean BASS_ChannelPauseDelegate(UInt32 handle);
        private static readonly BASS_ChannelPauseDelegate pBASS_ChannelPause = lib.LoadFunction<BASS_ChannelPauseDelegate>("BASS_ChannelPause");
        public static Boolean ChannelPause(UInt32 handle) => pBASS_ChannelPause(handle);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_ChannelGetData", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 ChannelGetData(UInt32 handle, IntPtr buffer, UInt32 length);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate UInt32 BASS_ChannelGetDataDelegate(UInt32 handle, IntPtr buffer, UInt32 length);
        private static readonly BASS_ChannelGetDataDelegate pBASS_ChannelGetData = lib.LoadFunction<BASS_ChannelGetDataDelegate>("BASS_ChannelGetData");
        public static UInt32 ChannelGetData(UInt32 handle, IntPtr buffer, UInt32 length) => pBASS_ChannelGetData(handle, buffer, length);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_ChannelGetAttribute", CallingConvention = CallingConvention.StdCall)]
        public static extern Boolean ChannelGetAttribute(UInt32 handle, BASSAttrib attrib, Single* value);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate Boolean BASS_ChannelGetAttributeDelegate(UInt32 handle, BASSAttrib attrib, Single* value);
        private static readonly BASS_ChannelGetAttributeDelegate pBASS_ChannelGetAttribute = lib.LoadFunction<BASS_ChannelGetAttributeDelegate>("BASS_ChannelGetAttribute");
        public static Boolean ChannelGetAttribute(UInt32 handle, BASSAttrib attrib, Single* value) => pBASS_ChannelGetAttribute(handle, attrib, value);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_ChannelSetAttribute", CallingConvention = CallingConvention.StdCall)]
        public static extern Boolean ChannelSetAttribute(UInt32 handle, BASSAttrib attrib, Single value);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate Boolean BASS_ChannelSetAttributeDelegate(UInt32 handle, BASSAttrib attrib, Single value);
        private static readonly BASS_ChannelSetAttributeDelegate pBASS_ChannelSetAttribute = lib.LoadFunction<BASS_ChannelSetAttributeDelegate>("BASS_ChannelSetAttribute");
        public static Boolean ChannelSetAttribute(UInt32 handle, BASSAttrib attrib, Single value) => pBASS_ChannelSetAttribute(handle, attrib, value);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_ChannelSlideAttribute", CallingConvention = CallingConvention.StdCall)]
        public static extern Boolean ChannelSlideAttribute(UInt32 handle, BASSAttrib attrib, Single value, UInt32 time);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate Boolean BASS_ChannelSlideAttributeDelegate(UInt32 handle, BASSAttrib attrib, Single value, UInt32 time);
        private static readonly BASS_ChannelSlideAttributeDelegate pBASS_ChannelSlideAttribute = lib.LoadFunction<BASS_ChannelSlideAttributeDelegate>("BASS_ChannelSlideAttribute");
        public static Boolean ChannelSlideAttribute(UInt32 handle, BASSAttrib attrib, Single value, UInt32 time) => pBASS_ChannelSlideAttribute(handle, attrib, value, time);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_ChannelGetPosition", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt64 ChannelGetPosition(UInt32 handle, UInt32 mode);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate UInt64 BASS_ChannelGetPositionDelegate(UInt32 handle, UInt32 mode);
        private static readonly BASS_ChannelGetPositionDelegate pBASS_ChannelGetPosition = lib.LoadFunction<BASS_ChannelGetPositionDelegate>("BASS_ChannelGetPosition");
        public static UInt64 ChannelGetPosition(UInt32 handle, UInt32 mode) => pBASS_ChannelGetPosition(handle, mode);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_ChannelSetPosition", CallingConvention = CallingConvention.StdCall)]
        public static extern Boolean ChannelSetPosition(UInt32 handle, UInt64 pos, UInt32 mode);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate Boolean BASS_ChannelSetPositionDelegate(UInt32 handle, UInt64 pos, UInt32 mode);
        private static readonly BASS_ChannelSetPositionDelegate pBASS_ChannelSetPosition = lib.LoadFunction<BASS_ChannelSetPositionDelegate>("BASS_ChannelSetPosition");
        public static Boolean ChannelSetPosition(UInt32 handle, UInt64 pos, UInt32 mode) => pBASS_ChannelSetPosition(handle, pos, mode);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_ChannelGetLength", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt64 ChannelGetLength(UInt32 handle, UInt32 mode);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate UInt64 BASS_ChannelGetLengthDelegate(UInt32 handle, UInt32 mode);
        private static readonly BASS_ChannelGetLengthDelegate pBASS_ChannelGetLength = lib.LoadFunction<BASS_ChannelGetLengthDelegate>("BASS_ChannelGetLength");
        public static UInt64 ChannelGetLength(UInt32 handle, UInt32 mode) => pBASS_ChannelGetLength(handle, mode);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_ChannelSetSync", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 ChannelSetSync(UInt32 handle, BASSSync type, UInt64 param, SyncProc proc, IntPtr user);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate UInt32 BASS_ChannelSetSyncDelegate(UInt32 handle, BASSSync type, UInt64 param, SyncProc proc, IntPtr user);
        private static readonly BASS_ChannelSetSyncDelegate pBASS_ChannelSetSync = lib.LoadFunction<BASS_ChannelSetSyncDelegate>("BASS_ChannelSetSync");
        public static UInt32 ChannelSetSync(UInt32 handle, BASSSync type, UInt64 param, SyncProc proc, IntPtr user) => pBASS_ChannelSetSync(handle, type, param, proc, user);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_ChannelRemoveSync", CallingConvention = CallingConvention.StdCall)]
        public static extern Boolean ChannelRemoveSync(UInt32 handle, UInt32 sync);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate Boolean BASS_ChannelRemoveSyncDelegate(UInt32 handle, UInt32 sync);
        private static readonly BASS_ChannelRemoveSyncDelegate pBASS_ChannelRemoveSync = lib.LoadFunction<BASS_ChannelRemoveSyncDelegate>("BASS_ChannelRemoveSync");
        public static Boolean ChannelRemoveSync(UInt32 handle, UInt32 sync) => pBASS_ChannelRemoveSync(handle, sync);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_ChannelGetTags", CallingConvention = CallingConvention.StdCall)]
        public static extern void* ChannelGetTags(UInt32 handle, UInt32 tags);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void* BASS_ChannelGetTagsDelegate(UInt32 handle, UInt32 tags);
        private static readonly BASS_ChannelGetTagsDelegate pBASS_ChannelGetTags = lib.LoadFunction<BASS_ChannelGetTagsDelegate>("BASS_ChannelGetTags");
        public static void* ChannelGetTags(UInt32 handle, UInt32 tags) => pBASS_ChannelGetTags(handle, tags);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_SampleLoad", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 SampleLoad(Boolean mem, IntPtr file, UInt64 offset, UInt32 length, UInt32 max, UInt32 flags);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate UInt32 BASS_SampleLoadDelegate(Boolean mem, IntPtr file, UInt64 offset, UInt32 length, UInt32 max, UInt32 flags);
        private static readonly BASS_SampleLoadDelegate pBASS_SampleLoad = lib.LoadFunction<BASS_SampleLoadDelegate>("BASS_SampleLoad");
        public static UInt32 SampleLoad(Boolean mem, IntPtr file, UInt64 offset, UInt32 length, UInt32 max, UInt32 flags) => pBASS_SampleLoad(mem, file, offset, length, max, flags);
#endif
        public static UInt32 SampleLoad(String file, UInt32 max, UInt32 flags)
        {
            var pFile = IntPtr.Zero;
            try
            {
                pFile = Marshal.StringToHGlobalAnsi(file);
                return SampleLoad(false, pFile, 0, 0, max, flags);
            }
            finally
            {
                if (pFile != IntPtr.Zero)
                    Marshal.FreeHGlobal(pFile);
            }
        }
        public static UInt32 SampleLoad(Byte[] data, UInt64 offset, UInt32 length, UInt32 max, UInt32 flags)
        {
            fixed (Byte* pData = data)
            {
                return SampleLoad(true, (IntPtr)pData, offset, length, max, flags);
            }            
        }

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_SampleFree", CallingConvention = CallingConvention.StdCall)]
        public static extern Boolean SampleFree(UInt32 handle);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate Boolean BASS_SampleFreeDelegate(UInt32 handle);
        private static readonly BASS_SampleFreeDelegate pBASS_SampleFree = lib.LoadFunction<BASS_SampleFreeDelegate>("BASS_SampleFree");
        public static Boolean SampleFree(UInt32 handle) => pBASS_SampleFree(handle);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_SampleGetChannel", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 SampleGetChannel(UInt32 handle, Boolean onlynew);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate UInt32 BASS_SampleGetChannelDelegate(UInt32 handle, Boolean onlynew);
        private static readonly BASS_SampleGetChannelDelegate pBASS_SampleGetChannel = lib.LoadFunction<BASS_SampleGetChannelDelegate>("BASS_SampleGetChannel");
        public static UInt32 SampleGetChannel(UInt32 handle, Boolean onlynew) => pBASS_SampleGetChannel(handle, onlynew);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_SampleGetInfo", CallingConvention = CallingConvention.StdCall)]
        public static extern Boolean SampleGetInfo(UInt32 handle, out BASS_SAMPLE info);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate Boolean BASS_SampleGetInfoDelegate(UInt32 handle, out BASS_SAMPLE info);
        private static readonly BASS_SampleGetInfoDelegate pBASS_SampleGetInfo = lib.LoadFunction<BASS_SampleGetInfoDelegate>("BASS_SampleGetInfo");
        public static Boolean SampleGetInfo(UInt32 handle, out BASS_SAMPLE info) => pBASS_SampleGetInfo(handle, out info);
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_SampleGetData", CallingConvention = CallingConvention.StdCall)]
        public static extern Boolean SampleGetData(UInt32 handle, IntPtr buffer);
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate Boolean BASS_SampleGetDataDelegate(UInt32 handle, IntPtr buffer);
        private static readonly BASS_SampleGetDataDelegate pBASS_SampleGetData = lib.LoadFunction<BASS_SampleGetDataDelegate>("BASS_SampleGetData");
        public static Boolean SampleGetData(UInt32 handle, IntPtr buffer) => pBASS_SampleGetData(handle, buffer);
#endif
        public static Boolean SampleGetData(UInt32 handle, Byte[] buffer)
        {
            fixed (Byte* pBuffer = buffer)
            {
                return SampleGetData(handle, (IntPtr)pBuffer);
            }
        }

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_Pause", CallingConvention = CallingConvention.StdCall)]
        public static extern Boolean Pause();
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate Boolean BASS_PauseDelegate();
        private static readonly BASS_PauseDelegate pBASS_Pause = lib.LoadFunction<BASS_PauseDelegate>("BASS_Pause");
        public static Boolean Pause() => pBASS_Pause();
#endif

#if ANDROID || IOS
        [DllImport(LIBRARY, EntryPoint="BASS_Start", CallingConvention = CallingConvention.StdCall)]
        public static extern Boolean Start();
#else
        [MonoNativeFunctionWrapper]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate Boolean BASS_StartDelegate();
        private static readonly BASS_StartDelegate pBASS_Start = lib.LoadFunction<BASS_StartDelegate>("BASS_Start");
        public static Boolean Start() => pBASS_Start();
#endif
    }
}