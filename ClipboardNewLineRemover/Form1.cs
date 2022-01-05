using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

// 参考：https://lets-csharp.com/how-to-clipboard-listener/

namespace ClipboardNewLineRemover
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll", SetLastError = true)]
        private extern static void AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        private extern static void RemoveClipboardFormatListener(IntPtr hwnd);

        private void Form1_Load(object sender, EventArgs e)
        {
            AddClipboardFormatListener(Handle);
        }

        
        void OnClipboardUpdate()
        {
            // クリップボードのデータが変更された

            // クリップボードがテキストだったら
            if (Clipboard.ContainsText())
            {
                string input = Clipboard.GetText();
                input = input.Replace("\r\n", " ");
                input = input.Replace("\n", " ");
                input = input.Replace("\r", " ");
                bool end = false;
                while (!end)
                {
                    string replaced = input.Replace("  ", " ");
                    if (replaced == input)
                    {
                        end = true;
                    }
                    input = replaced;
                }
                RemoveClipboardFormatListener(Handle);
                Clipboard.SetText(input);
                AddClipboardFormatListener(Handle);
            }
        }

        private const int WM_CLIPBOARDUPDATE = 0x31D;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_CLIPBOARDUPDATE)
            {
                OnClipboardUpdate();
                m.Result = IntPtr.Zero;
            }
            else
                base.WndProc(ref m);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            RemoveClipboardFormatListener(Handle);
        }
    }
}
