using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ChaosLauncher
{
    public partial class MessageDialog : Form
    {
        internal static DialogResult Show(Form owner, IWin32Window location = null)
        {
            if (owner.InvokeRequired)
                return (DialogResult)owner.Invoke((Action)(() => Show(owner, location)));
            else
                using (MessageDialog message = new MessageDialog())
                    return message.ShowDialog(location ?? owner);
        }

        public MessageDialog()
        {
            InitializeComponent();
        }

        private void gitLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start((sender as LinkLabel).Text);
        }
    }
}
