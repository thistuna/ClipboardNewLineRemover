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

        private bool isListenerAdded = false;
        private void ClipboardFormatListenerEnable(bool enable)
        {
            if(enable && !isListenerAdded)
            {
                AddClipboardFormatListener(Handle);
                isListenerAdded = true;
            }
            else if(!enable && isListenerAdded)
            {
                RemoveClipboardFormatListener(Handle);
                isListenerAdded = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            setFunctionEnable(true);
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
                ClipboardFormatListenerEnable(false);
                Clipboard.SetText(input);
                ClipboardFormatListenerEnable(true);
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
            ClipboardFormatListenerEnable(false);
        }

        private void setFunctionEnable(bool enable)
        {
            if (enable)
            {
                buttonOn.Enabled = false;
                buttonOff.Enabled = true;
                ClipboardFormatListenerEnable(true);
            }
            else
            {
                buttonOn.Enabled = true;
                buttonOff.Enabled = false;
                ClipboardFormatListenerEnable(false);
            }
        }

        private void buttonOn_Click(object sender, EventArgs e)
        {
            setFunctionEnable(true);
        }

        private void buttonOff_Click(object sender, EventArgs e)
        {
            setFunctionEnable(false);
        }
    }
}
