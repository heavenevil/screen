using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace screen
{
    public partial class Form1 : Form
    {
        public static string leak;

        // The matching delegate for MessageBeep
        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        delegate bool ExtTextOutWDelegate(IntPtr hdc,
                                       int X,
                                       int Y,
                                       uint fuOptions,
                                       [In] ref Rectangle lprc,
                                       string lpString,
                                       int cbCount,
                                       [In] IntPtr lpDx);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, EntryPoint = "ExtTextOutW", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        static extern bool ExtTextOutW(IntPtr hdc,
                                       int X,
                                       int Y,
                                       uint fuOptions,
                                       [In] ref Rectangle lprc,
                                       string lpString,
                                       int cbCount,
                                       [In] IntPtr lpDx);



        static private bool ExtTextOutWHook(IntPtr hdc,
                                       int X,
                                       int Y,
                                       uint fuOptions,
                                       [In] ref Rectangle lprc,
                                       string lpString,
                                       int cbCount,
                                       [In] IntPtr lpDx)
        {
            // We aren't going to call the original at all
            // but we could using: return MessageBeep(uType);
            leak = lpString;
            MessageBox.Show("hooked!");  // if hook function get called, then a message box will be displayed!!!
            return ExtTextOutW(
                 hdc,
                 X,
                 Y,
                 fuOptions,
                 ref lprc,
                 lpString,
                 cbCount,
                 lpDx
                  );
        }


        public Form1()
        {
            InitializeComponent();

            IntPtr hdc = this.Handle;
           

            using (var hook = EasyHook.LocalHook.Create(
                   EasyHook.LocalHook.GetProcAddress("gdi32.dll", "ExtTextOutW"),
                   new ExtTextOutWDelegate(ExtTextOutWHook),
                   null))
            {
                // Only hook this thread (threadId == 0 == GetCurrentThreadId)
                hook.ThreadACL.SetInclusiveACL(new int[] { 0 });
              //  hook.ThreadACL.SetExclusiveACL(new int[] { 0 });
                //TextOutA(hdc, 563, 313, "resttt", 6);
            }

        }



    }
}
