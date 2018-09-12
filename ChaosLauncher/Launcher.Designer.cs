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

namespace ChaosLauncher
{
    partial class Launcher
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
                components.Dispose();

            pfc.Dispose();
            token.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Launcher));
            launchBtn = new System.Windows.Forms.Button();
            serverStatusLbl = new System.Windows.Forms.Label();
            serverStatusImg = new System.Windows.Forms.PictureBox();
            infoImg = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(serverStatusImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(infoImg)).BeginInit();
            SuspendLayout();
            // 
            // launchBtn
            // 
            launchBtn.BackColor = System.Drawing.Color.Transparent;
            launchBtn.Enabled = false;
            launchBtn.FlatAppearance.BorderColor = System.Drawing.Color.SaddleBrown;
            launchBtn.FlatAppearance.BorderSize = 2;
            launchBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Sienna;
            launchBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Orange;
            launchBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            launchBtn.Font = new System.Drawing.Font("SWTOR Trajan", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            launchBtn.ForeColor = System.Drawing.Color.DarkRed;
            launchBtn.Location = new System.Drawing.Point(127, 132);
            launchBtn.Name = "launchBtn";
            launchBtn.Size = new System.Drawing.Size(148, 44);
            launchBtn.TabIndex = 1;
            launchBtn.Text = "Launch";
            launchBtn.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            launchBtn.UseVisualStyleBackColor = false;
            launchBtn.Click += new System.EventHandler(LaunchBtn_Click);
            // 
            // serverStatusLbl
            // 
            serverStatusLbl.AutoSize = true;
            serverStatusLbl.BackColor = System.Drawing.Color.Transparent;
            serverStatusLbl.Font = new System.Drawing.Font("SWTOR Trajan", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            serverStatusLbl.Location = new System.Drawing.Point(12, 140);
            serverStatusLbl.Name = "serverStatusLbl";
            serverStatusLbl.Size = new System.Drawing.Size(69, 34);
            serverStatusLbl.TabIndex = 2;
            serverStatusLbl.Text = "Server\r\nStatus";
            // 
            // serverStatusImg
            // 
            serverStatusImg.BackColor = System.Drawing.Color.Transparent;
            serverStatusImg.Location = new System.Drawing.Point(93, 146);
            serverStatusImg.Name = "serverStatusImg";
            serverStatusImg.Size = new System.Drawing.Size(16, 16);
            serverStatusImg.TabIndex = 3;
            serverStatusImg.TabStop = false;
            // 
            // infoImg
            // 
            infoImg.BackColor = System.Drawing.Color.Transparent;
            infoImg.Image = global::ChaosLauncher.Properties.Resources.info;
            infoImg.InitialImage = global::ChaosLauncher.Properties.Resources.info;
            infoImg.Location = new System.Drawing.Point(9, 9);
            infoImg.Margin = new System.Windows.Forms.Padding(0);
            infoImg.MaximumSize = new System.Drawing.Size(16, 16);
            infoImg.MinimumSize = new System.Drawing.Size(16, 16);
            infoImg.Name = "infoImg";
            infoImg.Size = new System.Drawing.Size(16, 16);
            infoImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            infoImg.TabIndex = 5;
            infoImg.TabStop = false;
            infoImg.Click += new System.EventHandler(InfoImg_Click);
            // 
            // Launcher
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            AutoSize = true;
            BackColor = System.Drawing.Color.White;
            BackgroundImage = global::ChaosLauncher.Properties.Resources.launcherImg;
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            ClientSize = new System.Drawing.Size(287, 188);
            Controls.Add(infoImg);
            Controls.Add(serverStatusImg);
            Controls.Add(serverStatusLbl);
            Controls.Add(launchBtn);
            DoubleBuffered = true;
            ForeColor = System.Drawing.Color.Black;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Icon = ((System.Drawing.Icon)(resources.GetObject("$Icon")));
            MaximizeBox = false;
            Name = "Launcher";
            Text = "Chaos Launcher";
            ((System.ComponentModel.ISupportInitialize)(serverStatusImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(infoImg)).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button launchBtn;
        private System.Windows.Forms.Label serverStatusLbl;
        private System.Windows.Forms.PictureBox serverStatusImg;
        private System.Windows.Forms.PictureBox infoImg;
    }
}