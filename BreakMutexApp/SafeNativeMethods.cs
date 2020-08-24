using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace BreakMutexApp
{
    internal static class SafeNativeMethods
    {
        [DllImport("BreakMutexLib.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseRemote(
            uint dwProcessId,
            [MarshalAs(UnmanagedType.LPWStr), In] string Name);
    }
}
