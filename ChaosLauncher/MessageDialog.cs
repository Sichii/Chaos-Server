using System;
using System.Diagnostics;
using System.Windows.Forms;
#pragma warning disable IDE0007 // Use implicit type
#pragma warning disable IDE0003 // Remove qualification


namespace ChaosLauncher
{
    public partial class MessageDialog : Form
    {
        internal static DialogResult Show(Form owner, IWin32Window location = null)
        {
            if (owner.InvokeRequired)
                return (DialogResult)owner.Invoke((Action)(() => Show(owner, location)));
            else
                using (var message = new MessageDialog())
                    return message.ShowDialog(location ?? owner);
        }

        public MessageDialog()
        {
            InitializeComponent();
        }

        private void GitLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => Process.Start((sender as LinkLabel).Text);
    }
}
