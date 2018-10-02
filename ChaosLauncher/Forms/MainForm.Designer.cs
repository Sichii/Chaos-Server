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
            this.launchBtn = new System.Windows.Forms.Button();
            this.serverStatusLbl = new System.Windows.Forms.Label();
            this.serverStatusImg = new System.Windows.Forms.PictureBox();
            this.infoImg = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.serverStatusImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.infoImg)).BeginInit();
            this.SuspendLayout();
            // 
            // launchBtn
            // 
            this.launchBtn.BackColor = System.Drawing.Color.Transparent;
            this.launchBtn.Enabled = false;
            this.launchBtn.FlatAppearance.BorderColor = System.Drawing.Color.SaddleBrown;
            this.launchBtn.FlatAppearance.BorderSize = 2;
            this.launchBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Sienna;
            this.launchBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Orange;
            this.launchBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.launchBtn.Font = new System.Drawing.Font("SWTOR Trajan", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.launchBtn.ForeColor = System.Drawing.Color.DarkRed;
            this.launchBtn.Location = new System.Drawing.Point(127, 132);
            this.launchBtn.Name = "launchBtn";
            this.launchBtn.Size = new System.Drawing.Size(148, 44);
            this.launchBtn.TabIndex = 1;
            this.launchBtn.Text = "Launch";
            this.launchBtn.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.launchBtn.UseVisualStyleBackColor = false;
            this.launchBtn.Click += new System.EventHandler(this.LaunchBtn_Click);
            // 
            // serverStatusLbl
            // 
            this.serverStatusLbl.AutoSize = true;
            this.serverStatusLbl.BackColor = System.Drawing.Color.Transparent;
            this.serverStatusLbl.Font = new System.Drawing.Font("SWTOR Trajan", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.serverStatusLbl.Location = new System.Drawing.Point(12, 140);
            this.serverStatusLbl.Name = "serverStatusLbl";
            this.serverStatusLbl.Size = new System.Drawing.Size(69, 34);
            this.serverStatusLbl.TabIndex = 2;
            this.serverStatusLbl.Text = "Server\r\nStatus";
            // 
            // serverStatusImg
            // 
            this.serverStatusImg.BackColor = System.Drawing.Color.Transparent;
            this.serverStatusImg.Location = new System.Drawing.Point(93, 146);
            this.serverStatusImg.Name = "serverStatusImg";
            this.serverStatusImg.Size = new System.Drawing.Size(16, 16);
            this.serverStatusImg.TabIndex = 3;
            this.serverStatusImg.TabStop = false;
            // 
            // infoImg
            // 
            this.infoImg.BackColor = System.Drawing.Color.Transparent;
            this.infoImg.Image = global::ChaosLauncher.Properties.Resources.info;
            this.infoImg.InitialImage = global::ChaosLauncher.Properties.Resources.info;
            this.infoImg.Location = new System.Drawing.Point(9, 9);
            this.infoImg.Margin = new System.Windows.Forms.Padding(0);
            this.infoImg.MaximumSize = new System.Drawing.Size(16, 16);
            this.infoImg.MinimumSize = new System.Drawing.Size(16, 16);
            this.infoImg.Name = "infoImg";
            this.infoImg.Size = new System.Drawing.Size(16, 16);
            this.infoImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.infoImg.TabIndex = 5;
            this.infoImg.TabStop = false;
            this.infoImg.Click += new System.EventHandler(this.InfoImg_Click);
            // 
            // Launcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::ChaosLauncher.Properties.Resources.launcherImg;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(287, 188);
            this.Controls.Add(this.infoImg);
            this.Controls.Add(this.serverStatusImg);
            this.Controls.Add(this.serverStatusLbl);
            this.Controls.Add(this.launchBtn);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Launcher";
            this.Text = "Chaos Launcher";
            ((System.ComponentModel.ISupportInitialize)(this.serverStatusImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.infoImg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button launchBtn;
        private System.Windows.Forms.Label serverStatusLbl;
        private System.Windows.Forms.PictureBox serverStatusImg;
        private System.Windows.Forms.PictureBox infoImg;
    }
}