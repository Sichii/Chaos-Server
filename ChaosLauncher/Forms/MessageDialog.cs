// ****************************************************************************
// This file belongs to the Chaos-Server project.
// 
// This project is free and open-source, provided that any alterations or
// modifications to any portions of this project adhere to the
// Affero General Public License (Version 3).
// 
// A copy of the AGPLv3 can be found in the project directory.
// You may also find a copy at <https://www.gnu.org/licenses/agpl-3.0.html>
// ****************************************************************************

using System;
using System.Diagnostics;
using System.Windows.Forms;

#pragma warning disable IDE0007 // Use implicit type
#pragma warning disable IDE0003 // Remove qualification


namespace ChaosLauncher.Forms
{
    public partial class MessageDialog : Form
    {
        internal static DialogResult Show(Form owner, IWin32Window location = null)
        {
            if (owner.InvokeRequired)
            {
                var result = DialogResult.None;
                owner.Invoke((Action)(() => result = Show(owner, location)));

                return result;
            } else
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