namespace ChaosLauncher
{
    partial class MessageDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MessageDialog));
            this.messageLbl = new System.Windows.Forms.Label();
            this.gitLink = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // messageLbl
            // 
            this.messageLbl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.messageLbl.Location = new System.Drawing.Point(0, 28);
            this.messageLbl.Margin = new System.Windows.Forms.Padding(0);
            this.messageLbl.Name = "messageLbl";
            this.messageLbl.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.messageLbl.Size = new System.Drawing.Size(234, 33);
            this.messageLbl.TabIndex = 7;
            this.messageLbl.Text = "Put the launcher in your Dark Ages folder";
            this.messageLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gitLink
            // 
            this.gitLink.AutoSize = true;
            this.gitLink.Dock = System.Windows.Forms.DockStyle.Top;
            this.gitLink.Location = new System.Drawing.Point(0, 0);
            this.gitLink.Margin = new System.Windows.Forms.Padding(0);
            this.gitLink.Name = "gitLink";
            this.gitLink.Padding = new System.Windows.Forms.Padding(12, 10, 10, 5);
            this.gitLink.Size = new System.Drawing.Size(230, 28);
            this.gitLink.TabIndex = 8;
            this.gitLink.TabStop = true;
            this.gitLink.Text = "https://github.com/zahdjinn/Chaos-Server";
            this.gitLink.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.gitLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.GitLink_LinkClicked);
            // 
            // MessageDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(234, 61);
            this.Controls.Add(this.gitLink);
            this.Controls.Add(this.messageLbl);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MessageDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Chaos Launcher";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label messageLbl;
        private System.Windows.Forms.LinkLabel gitLink;
    }
}