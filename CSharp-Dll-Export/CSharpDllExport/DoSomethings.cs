using System;
using System.Runtime.InteropServices;

namespace CSharpDllExport
{
    public class DoSomethings
    {
        [UnmanagedCallersOnly(EntryPoint = "Add")]
        public static int Add(int a, int b)
        {
            return a + b;
        }

        [UnmanagedCallersOnly(EntryPoint = "ConcatString")]
        public static IntPtr ConcatString(IntPtr first, IntPtr second)
        {
            // 从指针转换为string
            string my1String = Marshal.PtrToStringAnsi(first);
            string my2String = Marshal.PtrToStringAnsi(second);

            // 连接两个string 
            string concat = my1String + my2String;

            // 将申请非托管内存string转换为指针
            IntPtr concatPointer = Marshal.StringToHGlobalAnsi(concat);

            // 返回指针
            return concatPointer;
        }
    }
}