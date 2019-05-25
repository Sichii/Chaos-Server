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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
#pragma warning disable IDE0007 // Use implicit type
#pragma warning disable IDE0003 // Remove qualification

namespace ChaosTool
{

    internal class MainForm : Form
    {
        internal System.ComponentModel.IContainer components = null;
        internal TreeView MapTree;
        private TabControl MainTabControl;
        private TabPage MapsTab;
        private TabPage WorldMapsTab;
        private Label MapIdLbl;
        private Label MapSizeXLbl;
        private Label MapSizeYLbl;
        private Label MapFlagsLbl;
        private Label MapNameLbl;
        private Label MapMusicLbl;
        private GroupBox MapGbox;
        private NumericUpDown MusicNum;
        private NumericUpDown MapSizeYNum;
        private NumericUpDown MapSizeXNum;
        private NumericUpDown MapIdNum;
        private Button AddMapBtn;
        private TextBox MapNameTbox;
        private Label FlagSumLbl;
        private GroupBox WarpGbox;
        private GroupBox WorldMapGbox;
        private Button ChangeBtn;
        private Button DeleteMapBtn;
        private Button ChangeWarpBtn;
        private Button AddWarpBtn;
        private Button DeleteWarpBtn;
        private NumericUpDown WarpTargetMapIDNum;
        private Label WarpTargetMapIDLbl;
        private NumericUpDown WarpTargetYNum;
        private Label WarpTargetYLbl;
        private NumericUpDown WarpTargetXNum;
        private Label WarpTargetXLbl;
        private NumericUpDown WarpSourceYNum;
        private Label WarpSourceYLbl;
        private NumericUpDown WarpSourceXNum;
        private Label WarpSourceXLbl;
        private Button ChangeWMapBtn;
        private Button DeleteWMapBtn;
        private Button AddWMapBtn;
        private Label WMapSourceXLbl;
        private NumericUpDown WMapSourceXNum;
        private Label WMapSourceYLbl;
        private NumericUpDown WMapSourceYNum;
        private Label WMapFieldNameLbl;
        private GroupBox WorldMapNodeGbox;
        private Label NodeTargetXLbl;
        private NumericUpDown NodeTargetXNum;
        private Label NodeTargetYLbl;
        private NumericUpDown NodeTargetYNum;
        private Button ChangeNodeBtn;
        private Button DeleteNodeBtn;
        private Button AddNodeBtn;
        private Label NodePositionXLbl;
        internal NumericUpDown NodePositionXNum;
        private Label NodePositionYLbl;
        internal NumericUpDown NodePositionYNum;
        private NumericUpDown NodeTargetMapIDNum;
        private Label NodeTargetMapIDLbl;
        private Label WorldMapFieldNoteLbl;
        private ComboBox WMapFieldNameCombox;
        private Button NodePositionSelectorBtn;
        internal TreeView WorldMapTree;
        internal MapsCache MapsCache { get; }

        internal MainForm()
        {
            InitializeComponent();
            MapsCache = new MapsCache(this);
            MapTree.TreeViewNodeSorter = new TreeNodeSorter();
            WorldMapTree.TreeViewNodeSorter = new TreeNodeSorter();
            MapsCache.Load();
            LoadTrees();
        }

        ~MainForm()
        {
            MapsCache.Save();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        internal void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.MapTree = new System.Windows.Forms.TreeView();
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.MapsTab = new System.Windows.Forms.TabPage();
            this.WorldMapsTab = new System.Windows.Forms.TabPage();
            this.WorldMapTree = new System.Windows.Forms.TreeView();
            this.MapIdLbl = new System.Windows.Forms.Label();
            this.MapSizeXLbl = new System.Windows.Forms.Label();
            this.MapSizeYLbl = new System.Windows.Forms.Label();
            this.MapFlagsLbl = new System.Windows.Forms.Label();
            this.MapNameLbl = new System.Windows.Forms.Label();
            this.MapMusicLbl = new System.Windows.Forms.Label();
            this.MapGbox = new System.Windows.Forms.GroupBox();
            this.DeleteMapBtn = new System.Windows.Forms.Button();
            this.ChangeBtn = new System.Windows.Forms.Button();
            this.AddMapBtn = new System.Windows.Forms.Button();
            this.MapNameTbox = new System.Windows.Forms.TextBox();
            this.FlagSumLbl = new System.Windows.Forms.Label();
            this.MusicNum = new System.Windows.Forms.NumericUpDown();
            this.MapSizeYNum = new System.Windows.Forms.NumericUpDown();
            this.MapSizeXNum = new System.Windows.Forms.NumericUpDown();
            this.MapIdNum = new System.Windows.Forms.NumericUpDown();
            this.WarpGbox = new System.Windows.Forms.GroupBox();
            this.ChangeWarpBtn = new System.Windows.Forms.Button();
            this.AddWarpBtn = new System.Windows.Forms.Button();
            this.DeleteWarpBtn = new System.Windows.Forms.Button();
            this.WarpTargetMapIDNum = new System.Windows.Forms.NumericUpDown();
            this.WarpTargetMapIDLbl = new System.Windows.Forms.Label();
            this.WarpTargetYNum = new System.Windows.Forms.NumericUpDown();
            this.WarpTargetYLbl = new System.Windows.Forms.Label();
            this.WarpTargetXNum = new System.Windows.Forms.NumericUpDown();
            this.WarpTargetXLbl = new System.Windows.Forms.Label();
            this.WarpSourceYNum = new System.Windows.Forms.NumericUpDown();
            this.WarpSourceYLbl = new System.Windows.Forms.Label();
            this.WarpSourceXNum = new System.Windows.Forms.NumericUpDown();
            this.WarpSourceXLbl = new System.Windows.Forms.Label();
            this.WorldMapGbox = new System.Windows.Forms.GroupBox();
            this.WMapFieldNameCombox = new System.Windows.Forms.ComboBox();
            this.WorldMapFieldNoteLbl = new System.Windows.Forms.Label();
            this.WMapFieldNameLbl = new System.Windows.Forms.Label();
            this.ChangeWMapBtn = new System.Windows.Forms.Button();
            this.DeleteWMapBtn = new System.Windows.Forms.Button();
            this.AddWMapBtn = new System.Windows.Forms.Button();
            this.WMapSourceXLbl = new System.Windows.Forms.Label();
            this.WMapSourceXNum = new System.Windows.Forms.NumericUpDown();
            this.WMapSourceYLbl = new System.Windows.Forms.Label();
            this.WMapSourceYNum = new System.Windows.Forms.NumericUpDown();
            this.WorldMapNodeGbox = new System.Windows.Forms.GroupBox();
            this.NodePositionSelectorBtn = new System.Windows.Forms.Button();
            this.NodeTargetMapIDNum = new System.Windows.Forms.NumericUpDown();
            this.NodeTargetXLbl = new System.Windows.Forms.Label();
            this.NodeTargetMapIDLbl = new System.Windows.Forms.Label();
            this.NodeTargetXNum = new System.Windows.Forms.NumericUpDown();
            this.NodeTargetYLbl = new System.Windows.Forms.Label();
            this.NodeTargetYNum = new System.Windows.Forms.NumericUpDown();
            this.ChangeNodeBtn = new System.Windows.Forms.Button();
            this.DeleteNodeBtn = new System.Windows.Forms.Button();
            this.AddNodeBtn = new System.Windows.Forms.Button();
            this.NodePositionXLbl = new System.Windows.Forms.Label();
            this.NodePositionXNum = new System.Windows.Forms.NumericUpDown();
            this.NodePositionYLbl = new System.Windows.Forms.Label();
            this.NodePositionYNum = new System.Windows.Forms.NumericUpDown();
            this.MainTabControl.SuspendLayout();
            this.MapsTab.SuspendLayout();
            this.WorldMapsTab.SuspendLayout();
            this.MapGbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.MusicNum).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.MapSizeYNum).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.MapSizeXNum).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.MapIdNum).BeginInit();
            this.WarpGbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.WarpTargetMapIDNum).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.WarpTargetYNum).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.WarpTargetXNum).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.WarpSourceYNum).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.WarpSourceXNum).BeginInit();
            this.WorldMapGbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.WMapSourceXNum).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.WMapSourceYNum).BeginInit();
            this.WorldMapNodeGbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.NodeTargetMapIDNum).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.NodeTargetXNum).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.NodeTargetYNum).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.NodePositionXNum).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.NodePositionYNum).BeginInit();
            this.SuspendLayout();
            // 
            // MapTree
            // 
            this.MapTree.BackColor = System.Drawing.Color.White;
            this.MapTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MapTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MapTree.ForeColor = System.Drawing.Color.Black;
            this.MapTree.FullRowSelect = true;
            this.MapTree.HideSelection = false;
            this.MapTree.HotTracking = true;
            this.MapTree.Location = new System.Drawing.Point(4, 4);
            this.MapTree.Margin = new System.Windows.Forms.Padding(4);
            this.MapTree.Name = "MapTree";
            this.MapTree.Size = new System.Drawing.Size(418, 486);
            this.MapTree.TabIndex = 0;
            this.MapTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.MapTree_NodeMouseClick);
            // 
            // MainTabControl
            // 
            this.MainTabControl.Controls.Add(this.MapsTab);
            this.MainTabControl.Controls.Add(this.WorldMapsTab);
            this.MainTabControl.Dock = System.Windows.Forms.DockStyle.Left;
            this.MainTabControl.Location = new System.Drawing.Point(0, 0);
            this.MainTabControl.Margin = new System.Windows.Forms.Padding(4);
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.SelectedIndex = 0;
            this.MainTabControl.Size = new System.Drawing.Size(434, 523);
            this.MainTabControl.TabIndex = 1;
            // 
            // MapsTab
            // 
            this.MapsTab.Controls.Add(this.MapTree);
            this.MapsTab.Location = new System.Drawing.Point(4, 25);
            this.MapsTab.Margin = new System.Windows.Forms.Padding(4);
            this.MapsTab.Name = "MapsTab";
            this.MapsTab.Padding = new System.Windows.Forms.Padding(4);
            this.MapsTab.Size = new System.Drawing.Size(426, 494);
            this.MapsTab.TabIndex = 0;
            this.MapsTab.Text = "Maps";
            this.MapsTab.UseVisualStyleBackColor = true;
            // 
            // WorldMapsTab
            // 
            this.WorldMapsTab.Controls.Add(this.WorldMapTree);
            this.WorldMapsTab.Location = new System.Drawing.Point(4, 25);
            this.WorldMapsTab.Margin = new System.Windows.Forms.Padding(4);
            this.WorldMapsTab.Name = "WorldMapsTab";
            this.WorldMapsTab.Padding = new System.Windows.Forms.Padding(4);
            this.WorldMapsTab.Size = new System.Drawing.Size(426, 494);
            this.WorldMapsTab.TabIndex = 1;
            this.WorldMapsTab.Text = "WorldMaps";
            this.WorldMapsTab.UseVisualStyleBackColor = true;
            // 
            // WorldMapTree
            // 
            this.WorldMapTree.BackColor = System.Drawing.Color.White;
            this.WorldMapTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WorldMapTree.ForeColor = System.Drawing.Color.Black;
            this.WorldMapTree.Location = new System.Drawing.Point(4, 4);
            this.WorldMapTree.Margin = new System.Windows.Forms.Padding(4);
            this.WorldMapTree.Name = "WorldMapTree";
            this.WorldMapTree.Size = new System.Drawing.Size(418, 486);
            this.WorldMapTree.TabIndex = 1;
            this.WorldMapTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.WorldMapTree_NodeMouseClick);
            // 
            // MapIdLbl
            // 
            this.MapIdLbl.AutoSize = true;
            this.MapIdLbl.Location = new System.Drawing.Point(42, 24);
            this.MapIdLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.MapIdLbl.Name = "MapIdLbl";
            this.MapIdLbl.Size = new System.Drawing.Size(27, 16);
            this.MapIdLbl.TabIndex = 8;
            this.MapIdLbl.Text = "ID: ";
            // 
            // MapSizeXLbl
            // 
            this.MapSizeXLbl.AutoSize = true;
            this.MapSizeXLbl.Location = new System.Drawing.Point(21, 58);
            this.MapSizeXLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.MapSizeXLbl.Name = "MapSizeXLbl";
            this.MapSizeXLbl.Size = new System.Drawing.Size(48, 16);
            this.MapSizeXLbl.TabIndex = 9;
            this.MapSizeXLbl.Text = "SizeX: ";
            // 
            // MapSizeYLbl
            // 
            this.MapSizeYLbl.AutoSize = true;
            this.MapSizeYLbl.Location = new System.Drawing.Point(21, 92);
            this.MapSizeYLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.MapSizeYLbl.Name = "MapSizeYLbl";
            this.MapSizeYLbl.Size = new System.Drawing.Size(49, 16);
            this.MapSizeYLbl.TabIndex = 10;
            this.MapSizeYLbl.Text = "SizeY: ";
            // 
            // MapFlagsLbl
            // 
            this.MapFlagsLbl.AutoSize = true;
            this.MapFlagsLbl.Location = new System.Drawing.Point(163, 58);
            this.MapFlagsLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.MapFlagsLbl.Name = "MapFlagsLbl";
            this.MapFlagsLbl.Size = new System.Drawing.Size(48, 16);
            this.MapFlagsLbl.TabIndex = 11;
            this.MapFlagsLbl.Text = "Flags: ";
            // 
            // MapNameLbl
            // 
            this.MapNameLbl.AutoSize = true;
            this.MapNameLbl.Location = new System.Drawing.Point(160, 24);
            this.MapNameLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.MapNameLbl.Name = "MapNameLbl";
            this.MapNameLbl.Size = new System.Drawing.Size(51, 16);
            this.MapNameLbl.TabIndex = 12;
            this.MapNameLbl.Text = "Name: ";
            // 
            // MapMusicLbl
            // 
            this.MapMusicLbl.AutoSize = true;
            this.MapMusicLbl.Location = new System.Drawing.Point(163, 92);
            this.MapMusicLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.MapMusicLbl.Name = "MapMusicLbl";
            this.MapMusicLbl.Size = new System.Drawing.Size(49, 16);
            this.MapMusicLbl.TabIndex = 13;
            this.MapMusicLbl.Text = "Music: ";
            // 
            // MapGbox
            // 
            this.MapGbox.Controls.Add(this.DeleteMapBtn);
            this.MapGbox.Controls.Add(this.ChangeBtn);
            this.MapGbox.Controls.Add(this.AddMapBtn);
            this.MapGbox.Controls.Add(this.MapNameTbox);
            this.MapGbox.Controls.Add(this.FlagSumLbl);
            this.MapGbox.Controls.Add(this.MusicNum);
            this.MapGbox.Controls.Add(this.MapSizeYNum);
            this.MapGbox.Controls.Add(this.MapSizeXNum);
            this.MapGbox.Controls.Add(this.MapIdNum);
            this.MapGbox.Controls.Add(this.MapIdLbl);
            this.MapGbox.Controls.Add(this.MapMusicLbl);
            this.MapGbox.Controls.Add(this.MapNameLbl);
            this.MapGbox.Controls.Add(this.MapSizeXLbl);
            this.MapGbox.Controls.Add(this.MapFlagsLbl);
            this.MapGbox.Controls.Add(this.MapSizeYLbl);
            this.MapGbox.Dock = System.Windows.Forms.DockStyle.Top;
            this.MapGbox.Location = new System.Drawing.Point(434, 0);
            this.MapGbox.Margin = new System.Windows.Forms.Padding(4);
            this.MapGbox.Name = "MapGbox";
            this.MapGbox.Padding = new System.Windows.Forms.Padding(4);
            this.MapGbox.Size = new System.Drawing.Size(652, 130);
            this.MapGbox.TabIndex = 14;
            this.MapGbox.TabStop = false;
            this.MapGbox.Text = "Maps";
            // 
            // DeleteMapBtn
            // 
            this.DeleteMapBtn.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.DeleteMapBtn.AutoSize = true;
            this.DeleteMapBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.DeleteMapBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DeleteMapBtn.Location = new System.Drawing.Point(523, 18);
            this.DeleteMapBtn.Name = "DeleteMapBtn";
            this.DeleteMapBtn.Size = new System.Drawing.Size(117, 28);
            this.DeleteMapBtn.TabIndex = 29;
            this.DeleteMapBtn.Text = "Delete Selected";
            this.DeleteMapBtn.UseVisualStyleBackColor = true;
            this.DeleteMapBtn.Click += new System.EventHandler(this.DeleteMapBtn_Click);
            // 
            // ChangeBtn
            // 
            this.ChangeBtn.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ChangeBtn.AutoSize = true;
            this.ChangeBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ChangeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChangeBtn.Location = new System.Drawing.Point(516, 52);
            this.ChangeBtn.Name = "ChangeBtn";
            this.ChangeBtn.Size = new System.Drawing.Size(124, 28);
            this.ChangeBtn.TabIndex = 28;
            this.ChangeBtn.Text = "Change Selected";
            this.ChangeBtn.UseVisualStyleBackColor = true;
            this.ChangeBtn.Click += new System.EventHandler(this.ChangeBtn_Click);
            // 
            // AddMapBtn
            // 
            this.AddMapBtn.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.AddMapBtn.AutoSize = true;
            this.AddMapBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AddMapBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AddMapBtn.Location = new System.Drawing.Point(565, 86);
            this.AddMapBtn.Name = "AddMapBtn";
            this.AddMapBtn.Size = new System.Drawing.Size(75, 28);
            this.AddMapBtn.TabIndex = 26;
            this.AddMapBtn.Text = "Add Map";
            this.AddMapBtn.UseVisualStyleBackColor = true;
            this.AddMapBtn.Click += new System.EventHandler(this.AddMapBtn_Click);
            // 
            // MapNameTbox
            // 
            this.MapNameTbox.BackColor = System.Drawing.Color.White;
            this.MapNameTbox.ForeColor = System.Drawing.Color.Black;
            this.MapNameTbox.Location = new System.Drawing.Point(218, 21);
            this.MapNameTbox.Name = "MapNameTbox";
            this.MapNameTbox.Size = new System.Drawing.Size(120, 22);
            this.MapNameTbox.TabIndex = 25;
            // 
            // FlagSumLbl
            // 
            this.FlagSumLbl.AutoSize = true;
            this.FlagSumLbl.Location = new System.Drawing.Point(219, 58);
            this.FlagSumLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.FlagSumLbl.Name = "FlagSumLbl";
            this.FlagSumLbl.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.FlagSumLbl.Size = new System.Drawing.Size(15, 16);
            this.FlagSumLbl.TabIndex = 24;
            this.FlagSumLbl.Text = "0";
            this.FlagSumLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MusicNum
            // 
            this.MusicNum.BackColor = System.Drawing.Color.White;
            this.MusicNum.ForeColor = System.Drawing.Color.Black;
            this.MusicNum.Location = new System.Drawing.Point(219, 90);
            this.MusicNum.Maximum = new decimal(new int[] {
            127,
            0,
            0,
            0});
            this.MusicNum.Minimum = new decimal(new int[] {
            128,
            0,
            0,
            -2147483648});
            this.MusicNum.Name = "MusicNum";
            this.MusicNum.Size = new System.Drawing.Size(44, 22);
            this.MusicNum.TabIndex = 19;
            this.MusicNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.MusicNum.Value = new decimal(new int[] {
            127,
            0,
            0,
            0});
            // 
            // MapSizeYNum
            // 
            this.MapSizeYNum.BackColor = System.Drawing.Color.White;
            this.MapSizeYNum.ForeColor = System.Drawing.Color.Black;
            this.MapSizeYNum.Location = new System.Drawing.Point(77, 90);
            this.MapSizeYNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.MapSizeYNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.MapSizeYNum.Name = "MapSizeYNum";
            this.MapSizeYNum.Size = new System.Drawing.Size(44, 22);
            this.MapSizeYNum.TabIndex = 18;
            this.MapSizeYNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.MapSizeYNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // MapSizeXNum
            // 
            this.MapSizeXNum.BackColor = System.Drawing.Color.White;
            this.MapSizeXNum.ForeColor = System.Drawing.Color.Black;
            this.MapSizeXNum.Location = new System.Drawing.Point(76, 56);
            this.MapSizeXNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.MapSizeXNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.MapSizeXNum.Name = "MapSizeXNum";
            this.MapSizeXNum.Size = new System.Drawing.Size(44, 22);
            this.MapSizeXNum.TabIndex = 17;
            this.MapSizeXNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.MapSizeXNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // MapIdNum
            // 
            this.MapIdNum.BackColor = System.Drawing.Color.White;
            this.MapIdNum.ForeColor = System.Drawing.Color.Black;
            this.MapIdNum.Location = new System.Drawing.Point(76, 22);
            this.MapIdNum.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.MapIdNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.MapIdNum.Name = "MapIdNum";
            this.MapIdNum.Size = new System.Drawing.Size(58, 22);
            this.MapIdNum.TabIndex = 14;
            this.MapIdNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.MapIdNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // WarpGbox
            // 
            this.WarpGbox.Controls.Add(this.ChangeWarpBtn);
            this.WarpGbox.Controls.Add(this.AddWarpBtn);
            this.WarpGbox.Controls.Add(this.DeleteWarpBtn);
            this.WarpGbox.Controls.Add(this.WarpTargetMapIDNum);
            this.WarpGbox.Controls.Add(this.WarpTargetMapIDLbl);
            this.WarpGbox.Controls.Add(this.WarpTargetYNum);
            this.WarpGbox.Controls.Add(this.WarpTargetYLbl);
            this.WarpGbox.Controls.Add(this.WarpTargetXNum);
            this.WarpGbox.Controls.Add(this.WarpTargetXLbl);
            this.WarpGbox.Controls.Add(this.WarpSourceYNum);
            this.WarpGbox.Controls.Add(this.WarpSourceYLbl);
            this.WarpGbox.Controls.Add(this.WarpSourceXNum);
            this.WarpGbox.Controls.Add(this.WarpSourceXLbl);
            this.WarpGbox.Dock = System.Windows.Forms.DockStyle.Top;
            this.WarpGbox.Location = new System.Drawing.Point(434, 130);
            this.WarpGbox.Name = "WarpGbox";
            this.WarpGbox.Size = new System.Drawing.Size(652, 130);
            this.WarpGbox.TabIndex = 15;
            this.WarpGbox.TabStop = false;
            this.WarpGbox.Text = "Warps";
            // 
            // ChangeWarpBtn
            // 
            this.ChangeWarpBtn.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ChangeWarpBtn.AutoSize = true;
            this.ChangeWarpBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ChangeWarpBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChangeWarpBtn.Location = new System.Drawing.Point(516, 55);
            this.ChangeWarpBtn.Name = "ChangeWarpBtn";
            this.ChangeWarpBtn.Size = new System.Drawing.Size(124, 28);
            this.ChangeWarpBtn.TabIndex = 30;
            this.ChangeWarpBtn.Text = "Change Selected";
            this.ChangeWarpBtn.UseVisualStyleBackColor = true;
            this.ChangeWarpBtn.Click += new System.EventHandler(this.ChangeWarpBtn_Click);
            // 
            // AddWarpBtn
            // 
            this.AddWarpBtn.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.AddWarpBtn.AutoSize = true;
            this.AddWarpBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AddWarpBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AddWarpBtn.Location = new System.Drawing.Point(458, 89);
            this.AddWarpBtn.Name = "AddWarpBtn";
            this.AddWarpBtn.Size = new System.Drawing.Size(182, 28);
            this.AddWarpBtn.TabIndex = 30;
            this.AddWarpBtn.Text = "Add Warp to Selected Map";
            this.AddWarpBtn.UseVisualStyleBackColor = true;
            this.AddWarpBtn.Click += new System.EventHandler(this.AddWarpBtn_Click);
            // 
            // DeleteWarpBtn
            // 
            this.DeleteWarpBtn.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.DeleteWarpBtn.AutoSize = true;
            this.DeleteWarpBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.DeleteWarpBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DeleteWarpBtn.Location = new System.Drawing.Point(523, 21);
            this.DeleteWarpBtn.Name = "DeleteWarpBtn";
            this.DeleteWarpBtn.Size = new System.Drawing.Size(117, 28);
            this.DeleteWarpBtn.TabIndex = 30;
            this.DeleteWarpBtn.Text = "Delete Selected";
            this.DeleteWarpBtn.UseVisualStyleBackColor = true;
            this.DeleteWarpBtn.Click += new System.EventHandler(this.DeleteWarpBtn_Click);
            // 
            // WarpTargetMapIDNum
            // 
            this.WarpTargetMapIDNum.BackColor = System.Drawing.Color.White;
            this.WarpTargetMapIDNum.ForeColor = System.Drawing.Color.Black;
            this.WarpTargetMapIDNum.Location = new System.Drawing.Point(219, 93);
            this.WarpTargetMapIDNum.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.WarpTargetMapIDNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.WarpTargetMapIDNum.Name = "WarpTargetMapIDNum";
            this.WarpTargetMapIDNum.Size = new System.Drawing.Size(60, 22);
            this.WarpTargetMapIDNum.TabIndex = 38;
            this.WarpTargetMapIDNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.WarpTargetMapIDNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // WarpTargetMapIDLbl
            // 
            this.WarpTargetMapIDLbl.AutoSize = true;
            this.WarpTargetMapIDLbl.Location = new System.Drawing.Point(148, 95);
            this.WarpTargetMapIDLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.WarpTargetMapIDLbl.Name = "WarpTargetMapIDLbl";
            this.WarpTargetMapIDLbl.Size = new System.Drawing.Size(64, 16);
            this.WarpTargetMapIDLbl.TabIndex = 37;
            this.WarpTargetMapIDLbl.Text = "TargetID:";
            // 
            // WarpTargetYNum
            // 
            this.WarpTargetYNum.BackColor = System.Drawing.Color.White;
            this.WarpTargetYNum.ForeColor = System.Drawing.Color.Black;
            this.WarpTargetYNum.Location = new System.Drawing.Point(218, 59);
            this.WarpTargetYNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.WarpTargetYNum.Name = "WarpTargetYNum";
            this.WarpTargetYNum.Size = new System.Drawing.Size(44, 22);
            this.WarpTargetYNum.TabIndex = 36;
            this.WarpTargetYNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.WarpTargetYNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // WarpTargetYLbl
            // 
            this.WarpTargetYLbl.AutoSize = true;
            this.WarpTargetYLbl.Location = new System.Drawing.Point(151, 61);
            this.WarpTargetYLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.WarpTargetYLbl.Name = "WarpTargetYLbl";
            this.WarpTargetYLbl.Size = new System.Drawing.Size(60, 16);
            this.WarpTargetYLbl.TabIndex = 35;
            this.WarpTargetYLbl.Text = "TargetY:";
            // 
            // WarpTargetXNum
            // 
            this.WarpTargetXNum.BackColor = System.Drawing.Color.White;
            this.WarpTargetXNum.ForeColor = System.Drawing.Color.Black;
            this.WarpTargetXNum.Location = new System.Drawing.Point(218, 25);
            this.WarpTargetXNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.WarpTargetXNum.Name = "WarpTargetXNum";
            this.WarpTargetXNum.Size = new System.Drawing.Size(44, 22);
            this.WarpTargetXNum.TabIndex = 34;
            this.WarpTargetXNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.WarpTargetXNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // WarpTargetXLbl
            // 
            this.WarpTargetXLbl.AutoSize = true;
            this.WarpTargetXLbl.Location = new System.Drawing.Point(152, 27);
            this.WarpTargetXLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.WarpTargetXLbl.Name = "WarpTargetXLbl";
            this.WarpTargetXLbl.Size = new System.Drawing.Size(59, 16);
            this.WarpTargetXLbl.TabIndex = 33;
            this.WarpTargetXLbl.Text = "TargetX:";
            // 
            // WarpSourceYNum
            // 
            this.WarpSourceYNum.BackColor = System.Drawing.Color.White;
            this.WarpSourceYNum.ForeColor = System.Drawing.Color.Black;
            this.WarpSourceYNum.Location = new System.Drawing.Point(77, 59);
            this.WarpSourceYNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.WarpSourceYNum.Name = "WarpSourceYNum";
            this.WarpSourceYNum.Size = new System.Drawing.Size(44, 22);
            this.WarpSourceYNum.TabIndex = 32;
            this.WarpSourceYNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.WarpSourceYNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // WarpSourceYLbl
            // 
            this.WarpSourceYLbl.AutoSize = true;
            this.WarpSourceYLbl.Location = new System.Drawing.Point(8, 61);
            this.WarpSourceYLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.WarpSourceYLbl.Name = "WarpSourceYLbl";
            this.WarpSourceYLbl.Size = new System.Drawing.Size(63, 16);
            this.WarpSourceYLbl.TabIndex = 31;
            this.WarpSourceYLbl.Text = "SourceY:";
            // 
            // WarpSourceXNum
            // 
            this.WarpSourceXNum.BackColor = System.Drawing.Color.White;
            this.WarpSourceXNum.ForeColor = System.Drawing.Color.Black;
            this.WarpSourceXNum.Location = new System.Drawing.Point(77, 25);
            this.WarpSourceXNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.WarpSourceXNum.Name = "WarpSourceXNum";
            this.WarpSourceXNum.Size = new System.Drawing.Size(44, 22);
            this.WarpSourceXNum.TabIndex = 30;
            this.WarpSourceXNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.WarpSourceXNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // WarpSourceXLbl
            // 
            this.WarpSourceXLbl.AutoSize = true;
            this.WarpSourceXLbl.Location = new System.Drawing.Point(8, 27);
            this.WarpSourceXLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.WarpSourceXLbl.Name = "WarpSourceXLbl";
            this.WarpSourceXLbl.Size = new System.Drawing.Size(62, 16);
            this.WarpSourceXLbl.TabIndex = 9;
            this.WarpSourceXLbl.Text = "SourceX:";
            // 
            // WorldMapGbox
            // 
            this.WorldMapGbox.Controls.Add(this.WMapFieldNameCombox);
            this.WorldMapGbox.Controls.Add(this.WorldMapFieldNoteLbl);
            this.WorldMapGbox.Controls.Add(this.WMapFieldNameLbl);
            this.WorldMapGbox.Controls.Add(this.ChangeWMapBtn);
            this.WorldMapGbox.Controls.Add(this.DeleteWMapBtn);
            this.WorldMapGbox.Controls.Add(this.AddWMapBtn);
            this.WorldMapGbox.Controls.Add(this.WMapSourceXLbl);
            this.WorldMapGbox.Controls.Add(this.WMapSourceXNum);
            this.WorldMapGbox.Controls.Add(this.WMapSourceYLbl);
            this.WorldMapGbox.Controls.Add(this.WMapSourceYNum);
            this.WorldMapGbox.Dock = System.Windows.Forms.DockStyle.Top;
            this.WorldMapGbox.Location = new System.Drawing.Point(434, 260);
            this.WorldMapGbox.Name = "WorldMapGbox";
            this.WorldMapGbox.Size = new System.Drawing.Size(652, 129);
            this.WorldMapGbox.TabIndex = 16;
            this.WorldMapGbox.TabStop = false;
            this.WorldMapGbox.Text = "WorldMaps";
            // 
            // WMapFieldNameCombox
            // 
            this.WMapFieldNameCombox.FormattingEnabled = true;
            this.WMapFieldNameCombox.Location = new System.Drawing.Point(218, 24);
            this.WMapFieldNameCombox.Name = "WMapFieldNameCombox";
            this.WMapFieldNameCombox.Size = new System.Drawing.Size(120, 24);
            this.WMapFieldNameCombox.TabIndex = 51;
            // 
            // WorldMapFieldNoteLbl
            // 
            this.WorldMapFieldNoteLbl.AutoSize = true;
            this.WorldMapFieldNoteLbl.Location = new System.Drawing.Point(133, 51);
            this.WorldMapFieldNoteLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.WorldMapFieldNoteLbl.Name = "WorldMapFieldNoteLbl";
            this.WorldMapFieldNoteLbl.Size = new System.Drawing.Size(121, 48);
            this.WorldMapFieldNoteLbl.TabIndex = 50;
            this.WorldMapFieldNoteLbl.Text = "field001 = Temuair\r\nfield002 = Medenia\r\nfield003 = Mythosia";
            this.WorldMapFieldNoteLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // WMapFieldNameLbl
            // 
            this.WMapFieldNameLbl.AutoSize = true;
            this.WMapFieldNameLbl.Location = new System.Drawing.Point(133, 27);
            this.WMapFieldNameLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.WMapFieldNameLbl.Name = "WMapFieldNameLbl";
            this.WMapFieldNameLbl.Size = new System.Drawing.Size(78, 16);
            this.WMapFieldNameLbl.TabIndex = 46;
            this.WMapFieldNameLbl.Text = "FieldName:";
            // 
            // ChangeWMapBtn
            // 
            this.ChangeWMapBtn.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ChangeWMapBtn.AutoSize = true;
            this.ChangeWMapBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ChangeWMapBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChangeWMapBtn.Location = new System.Drawing.Point(516, 55);
            this.ChangeWMapBtn.Name = "ChangeWMapBtn";
            this.ChangeWMapBtn.Size = new System.Drawing.Size(124, 28);
            this.ChangeWMapBtn.TabIndex = 40;
            this.ChangeWMapBtn.Text = "Change Selected";
            this.ChangeWMapBtn.UseVisualStyleBackColor = true;
            this.ChangeWMapBtn.Click += new System.EventHandler(this.ChangeWMapBtn_Click);
            // 
            // DeleteWMapBtn
            // 
            this.DeleteWMapBtn.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.DeleteWMapBtn.AutoSize = true;
            this.DeleteWMapBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.DeleteWMapBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DeleteWMapBtn.Location = new System.Drawing.Point(523, 21);
            this.DeleteWMapBtn.Name = "DeleteWMapBtn";
            this.DeleteWMapBtn.Size = new System.Drawing.Size(117, 28);
            this.DeleteWMapBtn.TabIndex = 42;
            this.DeleteWMapBtn.Text = "Delete Selected";
            this.DeleteWMapBtn.UseVisualStyleBackColor = true;
            this.DeleteWMapBtn.Click += new System.EventHandler(this.DeleteWMapBtn_Click);
            // 
            // AddWMapBtn
            // 
            this.AddWMapBtn.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.AddWMapBtn.AutoSize = true;
            this.AddWMapBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AddWMapBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AddWMapBtn.Location = new System.Drawing.Point(373, 89);
            this.AddWMapBtn.Name = "AddWMapBtn";
            this.AddWMapBtn.Size = new System.Drawing.Size(267, 28);
            this.AddWMapBtn.TabIndex = 41;
            this.AddWMapBtn.Text = "Add WMap / Add WMap to Selected Map";
            this.AddWMapBtn.UseVisualStyleBackColor = true;
            this.AddWMapBtn.Click += new System.EventHandler(this.AddWMapBtn_Click);
            // 
            // WMapSourceXLbl
            // 
            this.WMapSourceXLbl.AutoSize = true;
            this.WMapSourceXLbl.Location = new System.Drawing.Point(8, 27);
            this.WMapSourceXLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.WMapSourceXLbl.Name = "WMapSourceXLbl";
            this.WMapSourceXLbl.Size = new System.Drawing.Size(62, 16);
            this.WMapSourceXLbl.TabIndex = 39;
            this.WMapSourceXLbl.Text = "SourceX:";
            // 
            // WMapSourceXNum
            // 
            this.WMapSourceXNum.BackColor = System.Drawing.Color.White;
            this.WMapSourceXNum.ForeColor = System.Drawing.Color.Black;
            this.WMapSourceXNum.Location = new System.Drawing.Point(77, 25);
            this.WMapSourceXNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.WMapSourceXNum.Name = "WMapSourceXNum";
            this.WMapSourceXNum.Size = new System.Drawing.Size(44, 22);
            this.WMapSourceXNum.TabIndex = 43;
            this.WMapSourceXNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.WMapSourceXNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // WMapSourceYLbl
            // 
            this.WMapSourceYLbl.AutoSize = true;
            this.WMapSourceYLbl.Location = new System.Drawing.Point(8, 61);
            this.WMapSourceYLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.WMapSourceYLbl.Name = "WMapSourceYLbl";
            this.WMapSourceYLbl.Size = new System.Drawing.Size(63, 16);
            this.WMapSourceYLbl.TabIndex = 44;
            this.WMapSourceYLbl.Text = "SourceY:";
            // 
            // WMapSourceYNum
            // 
            this.WMapSourceYNum.BackColor = System.Drawing.Color.White;
            this.WMapSourceYNum.ForeColor = System.Drawing.Color.Black;
            this.WMapSourceYNum.Location = new System.Drawing.Point(77, 59);
            this.WMapSourceYNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.WMapSourceYNum.Name = "WMapSourceYNum";
            this.WMapSourceYNum.Size = new System.Drawing.Size(44, 22);
            this.WMapSourceYNum.TabIndex = 45;
            this.WMapSourceYNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.WMapSourceYNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // WorldMapNodeGbox
            // 
            this.WorldMapNodeGbox.Controls.Add(this.NodePositionSelectorBtn);
            this.WorldMapNodeGbox.Controls.Add(this.NodeTargetMapIDNum);
            this.WorldMapNodeGbox.Controls.Add(this.NodeTargetXLbl);
            this.WorldMapNodeGbox.Controls.Add(this.NodeTargetMapIDLbl);
            this.WorldMapNodeGbox.Controls.Add(this.NodeTargetXNum);
            this.WorldMapNodeGbox.Controls.Add(this.NodeTargetYLbl);
            this.WorldMapNodeGbox.Controls.Add(this.NodeTargetYNum);
            this.WorldMapNodeGbox.Controls.Add(this.ChangeNodeBtn);
            this.WorldMapNodeGbox.Controls.Add(this.DeleteNodeBtn);
            this.WorldMapNodeGbox.Controls.Add(this.AddNodeBtn);
            this.WorldMapNodeGbox.Controls.Add(this.NodePositionXLbl);
            this.WorldMapNodeGbox.Controls.Add(this.NodePositionXNum);
            this.WorldMapNodeGbox.Controls.Add(this.NodePositionYLbl);
            this.WorldMapNodeGbox.Controls.Add(this.NodePositionYNum);
            this.WorldMapNodeGbox.Dock = System.Windows.Forms.DockStyle.Top;
            this.WorldMapNodeGbox.Location = new System.Drawing.Point(434, 389);
            this.WorldMapNodeGbox.Name = "WorldMapNodeGbox";
            this.WorldMapNodeGbox.Size = new System.Drawing.Size(652, 129);
            this.WorldMapNodeGbox.TabIndex = 47;
            this.WorldMapNodeGbox.TabStop = false;
            this.WorldMapNodeGbox.Text = "WorldMapNodes";
            // 
            // NodePositionSelectorBtn
            // 
            this.NodePositionSelectorBtn.AutoSize = true;
            this.NodePositionSelectorBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.NodePositionSelectorBtn.Location = new System.Drawing.Point(11, 89);
            this.NodePositionSelectorBtn.Name = "NodePositionSelectorBtn";
            this.NodePositionSelectorBtn.Size = new System.Drawing.Size(123, 28);
            this.NodePositionSelectorBtn.TabIndex = 50;
            this.NodePositionSelectorBtn.Text = "Position Selector";
            this.NodePositionSelectorBtn.UseVisualStyleBackColor = true;
            this.NodePositionSelectorBtn.Click += new System.EventHandler(this.NodePositionSelectorBtn_Click);
            // 
            // NodeTargetMapIDNum
            // 
            this.NodeTargetMapIDNum.BackColor = System.Drawing.Color.White;
            this.NodeTargetMapIDNum.ForeColor = System.Drawing.Color.Black;
            this.NodeTargetMapIDNum.Location = new System.Drawing.Point(210, 93);
            this.NodeTargetMapIDNum.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.NodeTargetMapIDNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NodeTargetMapIDNum.Name = "NodeTargetMapIDNum";
            this.NodeTargetMapIDNum.Size = new System.Drawing.Size(60, 22);
            this.NodeTargetMapIDNum.TabIndex = 40;
            this.NodeTargetMapIDNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.NodeTargetMapIDNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // NodeTargetXLbl
            // 
            this.NodeTargetXLbl.AutoSize = true;
            this.NodeTargetXLbl.Location = new System.Drawing.Point(144, 27);
            this.NodeTargetXLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.NodeTargetXLbl.Name = "NodeTargetXLbl";
            this.NodeTargetXLbl.Size = new System.Drawing.Size(59, 16);
            this.NodeTargetXLbl.TabIndex = 46;
            this.NodeTargetXLbl.Text = "TargetX:";
            // 
            // NodeTargetMapIDLbl
            // 
            this.NodeTargetMapIDLbl.AutoSize = true;
            this.NodeTargetMapIDLbl.Location = new System.Drawing.Point(139, 95);
            this.NodeTargetMapIDLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.NodeTargetMapIDLbl.Name = "NodeTargetMapIDLbl";
            this.NodeTargetMapIDLbl.Size = new System.Drawing.Size(64, 16);
            this.NodeTargetMapIDLbl.TabIndex = 39;
            this.NodeTargetMapIDLbl.Text = "TargetID:";
            // 
            // NodeTargetXNum
            // 
            this.NodeTargetXNum.BackColor = System.Drawing.Color.White;
            this.NodeTargetXNum.ForeColor = System.Drawing.Color.Black;
            this.NodeTargetXNum.Location = new System.Drawing.Point(210, 25);
            this.NodeTargetXNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.NodeTargetXNum.Name = "NodeTargetXNum";
            this.NodeTargetXNum.Size = new System.Drawing.Size(44, 22);
            this.NodeTargetXNum.TabIndex = 47;
            this.NodeTargetXNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.NodeTargetXNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // NodeTargetYLbl
            // 
            this.NodeTargetYLbl.AutoSize = true;
            this.NodeTargetYLbl.Location = new System.Drawing.Point(143, 61);
            this.NodeTargetYLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.NodeTargetYLbl.Name = "NodeTargetYLbl";
            this.NodeTargetYLbl.Size = new System.Drawing.Size(60, 16);
            this.NodeTargetYLbl.TabIndex = 48;
            this.NodeTargetYLbl.Text = "TargetY:";
            // 
            // NodeTargetYNum
            // 
            this.NodeTargetYNum.BackColor = System.Drawing.Color.White;
            this.NodeTargetYNum.ForeColor = System.Drawing.Color.Black;
            this.NodeTargetYNum.Location = new System.Drawing.Point(210, 59);
            this.NodeTargetYNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.NodeTargetYNum.Name = "NodeTargetYNum";
            this.NodeTargetYNum.Size = new System.Drawing.Size(44, 22);
            this.NodeTargetYNum.TabIndex = 49;
            this.NodeTargetYNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.NodeTargetYNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // ChangeNodeBtn
            // 
            this.ChangeNodeBtn.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ChangeNodeBtn.AutoSize = true;
            this.ChangeNodeBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ChangeNodeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChangeNodeBtn.Location = new System.Drawing.Point(516, 55);
            this.ChangeNodeBtn.Name = "ChangeNodeBtn";
            this.ChangeNodeBtn.Size = new System.Drawing.Size(124, 28);
            this.ChangeNodeBtn.TabIndex = 40;
            this.ChangeNodeBtn.Text = "Change Selected";
            this.ChangeNodeBtn.UseVisualStyleBackColor = true;
            this.ChangeNodeBtn.Click += new System.EventHandler(this.ChangeNodeBtn_Click);
            // 
            // DeleteNodeBtn
            // 
            this.DeleteNodeBtn.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.DeleteNodeBtn.AutoSize = true;
            this.DeleteNodeBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.DeleteNodeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DeleteNodeBtn.Location = new System.Drawing.Point(523, 21);
            this.DeleteNodeBtn.Name = "DeleteNodeBtn";
            this.DeleteNodeBtn.Size = new System.Drawing.Size(117, 28);
            this.DeleteNodeBtn.TabIndex = 42;
            this.DeleteNodeBtn.Text = "Delete Selected";
            this.DeleteNodeBtn.UseVisualStyleBackColor = true;
            this.DeleteNodeBtn.Click += new System.EventHandler(this.DeleteNodeBtn_Click);
            // 
            // AddNodeBtn
            // 
            this.AddNodeBtn.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.AddNodeBtn.AutoSize = true;
            this.AddNodeBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AddNodeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AddNodeBtn.Location = new System.Drawing.Point(444, 89);
            this.AddNodeBtn.Name = "AddNodeBtn";
            this.AddNodeBtn.Size = new System.Drawing.Size(196, 28);
            this.AddNodeBtn.TabIndex = 41;
            this.AddNodeBtn.Text = "Add Node to Selected WMap";
            this.AddNodeBtn.UseVisualStyleBackColor = true;
            this.AddNodeBtn.Click += new System.EventHandler(this.AddNodeBtn_Click);
            // 
            // NodePositionXLbl
            // 
            this.NodePositionXLbl.AutoSize = true;
            this.NodePositionXLbl.Location = new System.Drawing.Point(8, 27);
            this.NodePositionXLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.NodePositionXLbl.Name = "NodePositionXLbl";
            this.NodePositionXLbl.Size = new System.Drawing.Size(67, 16);
            this.NodePositionXLbl.TabIndex = 39;
            this.NodePositionXLbl.Text = "PositionX:";
            // 
            // NodePositionXNum
            // 
            this.NodePositionXNum.BackColor = System.Drawing.Color.White;
            this.NodePositionXNum.ForeColor = System.Drawing.Color.Black;
            this.NodePositionXNum.Location = new System.Drawing.Point(82, 25);
            this.NodePositionXNum.Maximum = new decimal(new int[] {
            640,
            0,
            0,
            0});
            this.NodePositionXNum.Name = "NodePositionXNum";
            this.NodePositionXNum.Size = new System.Drawing.Size(44, 22);
            this.NodePositionXNum.TabIndex = 43;
            this.NodePositionXNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.NodePositionXNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // NodePositionYLbl
            // 
            this.NodePositionYLbl.AutoSize = true;
            this.NodePositionYLbl.Location = new System.Drawing.Point(8, 61);
            this.NodePositionYLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.NodePositionYLbl.Name = "NodePositionYLbl";
            this.NodePositionYLbl.Size = new System.Drawing.Size(68, 16);
            this.NodePositionYLbl.TabIndex = 44;
            this.NodePositionYLbl.Text = "PositionY:";
            // 
            // NodePositionYNum
            // 
            this.NodePositionYNum.BackColor = System.Drawing.Color.White;
            this.NodePositionYNum.ForeColor = System.Drawing.Color.Black;
            this.NodePositionYNum.Location = new System.Drawing.Point(82, 59);
            this.NodePositionYNum.Maximum = new decimal(new int[] {
            480,
            0,
            0,
            0});
            this.NodePositionYNum.Name = "NodePositionYNum";
            this.NodePositionYNum.Size = new System.Drawing.Size(44, 22);
            this.NodePositionYNum.TabIndex = 45;
            this.NodePositionYNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.NodePositionYNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1086, 523);
            this.Controls.Add(this.WorldMapNodeGbox);
            this.Controls.Add(this.WorldMapGbox);
            this.Controls.Add(this.WarpGbox);
            this.Controls.Add(this.MapGbox);
            this.Controls.Add(this.MainTabControl);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ChaosTool";
            this.MainTabControl.ResumeLayout(false);
            this.MapsTab.ResumeLayout(false);
            this.WorldMapsTab.ResumeLayout(false);
            this.MapGbox.ResumeLayout(false);
            this.MapGbox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.MusicNum).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.MapSizeYNum).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.MapSizeXNum).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.MapIdNum).EndInit();
            this.WarpGbox.ResumeLayout(false);
            this.WarpGbox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.WarpTargetMapIDNum).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.WarpTargetYNum).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.WarpTargetXNum).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.WarpSourceYNum).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.WarpSourceXNum).EndInit();
            this.WorldMapGbox.ResumeLayout(false);
            this.WorldMapGbox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.WMapSourceXNum).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.WMapSourceYNum).EndInit();
            this.WorldMapNodeGbox.ResumeLayout(false);
            this.WorldMapNodeGbox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.NodeTargetMapIDNum).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.NodeTargetXNum).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.NodeTargetYNum).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.NodePositionXNum).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.NodePositionYNum).EndInit();
            this.ResumeLayout(false);

        }

        #region Trees
        internal void LoadTrees()
        {
            if (InvokeRequired)
                Invoke((Action)LoadTrees);
            else
            {
                MapTree.Nodes.Clear();
                WorldMapTree.Nodes.Clear();

                //for each map
                foreach (Chaos.Map map in MapsCache.Maps.Values)
                {
                    //each map gets a Node
                    var Map = new MapTreeNode(map, $@"{map.Name} - {map.Id} ({map.SizeX},{map.SizeY})");

                    var Warps = new TreeNode("Warps")
                    {
                        Name = "Warps"
                    };
                    var Doors = new TreeNode("Doors")
                    {
                        Name = "Doors"
                    };
                    var WorldMaps = new TreeNode("WorldMaps")
                    {
                        Name = "WorldMaps"
                    };
                    Map.Nodes.Add(Warps);
                    Map.Nodes.Add(Doors);
                    Map.Nodes.Add(WorldMaps);

                    //each Warp gets a subNode
                    foreach (Chaos.Warp Warp in map.Warps.Values)
                        try
                        {
                            Warps.Nodes.Add(new WarpTreeNode(Warp, $@"{MapsCache.Maps[Warp.TargetLocation.MapID].Name} - {Warp}"));
                        }
                        catch
                        {
                            MessageBox.Show($@"A Warp's target map doesn't exist. TargetMapID={Warp.TargetLocation.MapID}", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                    //each worldmap gets a subNode
                    foreach (KeyValuePair<Chaos.Point, Chaos.WorldMap> kvp in map.WorldMaps)
                        WorldMaps.Nodes.Add(new WorldMapTreeNode(kvp, $@"{kvp.Key} => {kvp.Value.CheckSum}"));

                    //add this map to the map tree
                    MapTree.Nodes.Add(Map);
                }

                WMapFieldNameCombox.Items.Clear();
                WMapFieldNameCombox.Items.AddRange(new string[] { "field001", "field002", "field003" });

                //for each worldmap
                foreach (KeyValuePair<uint, Chaos.WorldMap> kvp in MapsCache.WorldMaps)
                {
                    //each worldmap gets a Node
                    var WorldMap = new WorldMapTreeNode(kvp.Value, $@"{kvp.Key} => {kvp.Value.Field}");

                    //each worldmapNode gets a subNode
                    foreach (Chaos.WorldMapNode wmn in kvp.Value.Nodes)
                        WorldMap.Nodes.Add(new WorldMapNodeTreeNode(wmn, $@"{wmn.Position} - {wmn.Location} - ({MapsCache.Maps[wmn.MapId].Name})"));

                    //add this worldmap to the worldmap tree
                    WorldMapTree.Nodes.Add(WorldMap);

                    WMapFieldNameCombox.Items.Add(kvp.Key.ToString());
                }
            }
        }

        private void MapTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node is MapTreeNode tMapTreeNode)
            {
                Chaos.Map map = tMapTreeNode.Map;

                MapIdNum.Value = map.Id;
                MapSizeXNum.Value = map.SizeX;
                MapSizeYNum.Value = map.SizeY;
                MapNameTbox.Text = map.Name;
                FlagSumLbl.Text = map.Flags.ToString();
                MusicNum.Value = map.Music;
            }
            else if (e.Node is WarpTreeNode tWarpTreeNode)
            {
                Chaos.Warp warp = tWarpTreeNode.Warp;

                WarpSourceXNum.Value = warp.Point.X;
                WarpSourceYNum.Value = warp.Point.Y;
                WarpTargetXNum.Value = warp.TargetPoint.X;
                WarpTargetYNum.Value = warp.TargetPoint.Y;
                WarpTargetMapIDNum.Value = warp.TargetLocation.MapID;
            }
            else if (e.Node is WorldMapTreeNode tWorldMapTreeNode)
            {
                Chaos.Point point = tWorldMapTreeNode.Point;
                Chaos.WorldMap worldMap = tWorldMapTreeNode.WorldMap;

                WMapSourceXNum.Value = point.X;
                WMapSourceYNum.Value = point.Y;
                WMapFieldNameCombox.Text = worldMap.Field;
            }
        }

        private void WorldMapTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node is WorldMapTreeNode tWorldMapTreeNode)
            {
                Chaos.WorldMap worldMap = tWorldMapTreeNode.WorldMap;

                WMapSourceXNum.Value = 0;
                WMapSourceYNum.Value = 0;
                WMapFieldNameCombox.Text = worldMap.Field;
            }
            else if (e.Node is WorldMapNodeTreeNode tWorldMapNodeTreeNode)
            {
                Chaos.WorldMapNode worldMapNode = tWorldMapNodeTreeNode.WorldMapNode;

                NodePositionXNum.Value = worldMapNode.Position.X;
                NodePositionYNum.Value = worldMapNode.Position.Y;
                NodeTargetXNum.Value = worldMapNode.Point.X;
                NodeTargetYNum.Value = worldMapNode.Point.Y;
                NodeTargetMapIDNum.Value = worldMapNode.MapId;
            }
        }
        #endregion

        #region Maps
        private void AddMapBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string message = "";
                string mapName = MapNameTbox.Text;
                ushort mapId = decimal.ToUInt16(MapIdNum.Value);
                byte sizeX = decimal.ToByte(MapSizeXNum.Value);
                byte sizeY = decimal.ToByte(MapSizeYNum.Value);
                sbyte music = decimal.ToSByte(MusicNum.Value);

                message = MapsCache.Maps.ContainsKey(mapId) ? $@"Map ID:{MapIdNum.Value} is already in use. Overwrite? Will Delete doors and Warps."
                    : $@"Add this as a new map? Make sure info is correct.";

                if (uint.TryParse(FlagSumLbl.Text, out uint flags))
                {
                    if (MessageBox.Show(message, "Chaos MapTool", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        MapsCache.Maps[mapId] = new Chaos.Map(mapId, sizeX, sizeY, (Chaos.MapFlags)flags, mapName, music);
                        MapsCache.Save();
                        LoadTrees();
                    }
                }
                else
                    MessageBox.Show("Flag error.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {
                MessageBox.Show("Exception, check values.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChangeBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (MapTree.SelectedNode is MapTreeNode tMapTreeNode)
                {
                    string message = "";
                    string mapName = MapNameTbox.Text;
                    ushort mapId = tMapTreeNode.Map.Id;
                    byte sizeX = decimal.ToByte(MapSizeXNum.Value);
                    byte sizeY = decimal.ToByte(MapSizeYNum.Value);
                    sbyte music = decimal.ToSByte(MusicNum.Value);
                    uint flags = 0;

                    if (!MapsCache.Maps.ContainsKey(mapId))
                        MessageBox.Show("Map doesn't exist, please select a map.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        message = $@"Change map info related to Map ID: {mapId}? Doors and Warps will stay.";

                        if (uint.TryParse(FlagSumLbl.Text, out flags))
                        {
                            if (MessageBox.Show(message, "Chaos MapTool", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                            {
                                MapsCache.Maps[mapId].Name = mapName;
                                MapsCache.Maps[mapId].SizeX = sizeX;
                                MapsCache.Maps[mapId].SizeY = sizeY;
                                MapsCache.Maps[mapId].Music = music;
                                MapsCache.Maps[mapId].Flags = (Chaos.MapFlags)flags;
                                MapsCache.Save();
                                LoadTrees();
                                MapTree.SelectedNode = MapTree.Nodes[mapName];
                            }
                        }
                        else
                            MessageBox.Show("Flag error.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Exception, check values.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteMapBtn_Click(object sender, EventArgs e)
        {
            try
            {;
                if (MapTree.SelectedNode is MapTreeNode tMapTreeNode)
                {
                    string message = "";
                    ushort mapId = tMapTreeNode.Map.Id;

                    if (!MapsCache.Maps.ContainsKey(mapId))
                        MessageBox.Show("Map doesn't exist, please select a map.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        message = $@"Delete Map ID: {mapId}? This will destroy doors, Warps, and the info.";
                        if (MessageBox.Show(message, "Chaos MapTool", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                        {
                            MapsCache.Maps.Remove(mapId);
                            MapsCache.Save();
                            LoadTrees();
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Exception, check values.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Warps
        private void AddWarpBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (MapTree.SelectedNode is MapTreeNode tMapTreeNode)
                {
                    Chaos.Map map = tMapTreeNode.Map;
                    var newWarp = new Chaos.Warp(map.Id, (ushort)WarpSourceXNum.Value, (ushort)WarpSourceYNum.Value, (ushort)WarpTargetMapIDNum.Value, (ushort)WarpTargetXNum.Value, (ushort)WarpTargetYNum.Value);

                    if (map.Warps.ContainsKey(newWarp.Point))
                        MessageBox.Show("Map already contains Warp on that point.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        map.Warps.Add(newWarp.Point, newWarp);

                    MapsCache.Save();
                    LoadTrees();

                    MapTree.Nodes[map.Name].Expand();
                    MapTree.Nodes[map.Name].Nodes["Warps"].Expand();
                    MapTree.SelectedNode = MapTree.Nodes[map.Name];
                }
            }
            catch
            {
                MessageBox.Show("Exception, check values.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChangeWarpBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (MapTree.SelectedNode is WarpTreeNode tWarpTreeNode)
                {
                    Chaos.Warp oldWarp = tWarpTreeNode.Warp;
                    Chaos.Map map = MapsCache.Maps[oldWarp.Location.MapID];
                    var newWarp = new Chaos.Warp(oldWarp.Location.MapID, (ushort)WarpSourceXNum.Value, (ushort)WarpSourceYNum.Value, (ushort)WarpTargetMapIDNum.Value, (ushort)WarpTargetXNum.Value, (ushort)WarpTargetYNum.Value);

                    if (!map.Warps.ContainsKey(oldWarp.Point))
                        MessageBox.Show("Map does not contain that Warp.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        map.Warps.Remove(oldWarp.Point);
                        map.Warps.Add(newWarp.Point, newWarp);

                        MapsCache.Save();
                        LoadTrees();
                        MapTree.Nodes[map.Name].Expand();
                        MapTree.Nodes[map.Name].Nodes["Warps"].Expand();
                        MapTree.SelectedNode = MapTree.Nodes[map.Name].Nodes["Warps"].Nodes[newWarp.Point.ToString()];
                    }
                }
            }
            catch
            {
                MessageBox.Show("Exception, check values.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteWarpBtn_Click(object sender, EventArgs e)
        {
            if (MapTree.SelectedNode is WarpTreeNode tWarpTreeNode)
            {
                Chaos.Warp Warp = tWarpTreeNode.Warp;
                Chaos.Map map = MapsCache.Maps[Warp.Location.MapID];

                if (!map.Warps.ContainsKey(Warp.Point))
                    MessageBox.Show("Map does not contain that Warp.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    map.Warps.Remove(Warp.Point);

                    MapsCache.Save();
                    LoadTrees();
                    MapTree.Nodes[map.Name].Expand();
                    MapTree.Nodes[map.Name].Nodes["Warps"].Expand();
                    MapTree.SelectedNode = MapTree.Nodes[map.Name];
                }
            }
        }
        #endregion

        #region WorldMaps
        private void AddWMapBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (MapTree.SelectedNode is MapTreeNode tMapTreeNode && MainTabControl.SelectedTab == MapsTab)
                {
                    Chaos.Map map = tMapTreeNode.Map;
                    Chaos.WorldMap newWmap = MapsCache.WorldMaps.FirstOrDefault(kvp => WMapFieldNameCombox.Text == kvp.Key.ToString()).Value;
                    Chaos.Point sourcePoint = ((int)WMapSourceXNum.Value, (int)WMapSourceYNum.Value);

                    if (newWmap == null)
                        MessageBox.Show("WorldMap does not exist.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else if (map.WorldMaps.ContainsKey(sourcePoint))
                        MessageBox.Show("Map already contains WorldMap on that point.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        map.WorldMaps.Add(sourcePoint, newWmap);

                        MapsCache.Save();
                        LoadTrees();

                        MapTree.Nodes[map.Name].Expand();
                        MapTree.Nodes[map.Name].Nodes["WorldMaps"].Expand();
                        MapTree.SelectedNode = MapTree.Nodes[map.Name].Nodes[newWmap.CheckSum.ToString()];
                    }
                }
                else if (MainTabControl.SelectedTab == WorldMapsTab)
                {
                    Chaos.WorldMap newWmap = new Chaos.WorldMap(WMapFieldNameCombox.Text);

                    if (newWmap == null)
                        MessageBox.Show("Check data.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else if (MapsCache.WorldMaps.ContainsKey(newWmap.CheckSum))
                        MessageBox.Show("That worldmap already exists.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        MapsCache.WorldMaps.Add(newWmap.CheckSum, newWmap);

                        MapsCache.Save();
                        LoadTrees();

                        WorldMapTree.SelectedNode = WorldMapTree.Nodes[newWmap.CheckSum.ToString()];
                    }
                }
            }
            catch
            {
                MessageBox.Show("Exception, check values.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChangeWMapBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (MapTree.SelectedNode is WorldMapTreeNode tWorldMapTreeNode && MainTabControl.SelectedTab == MapsTab)
                {
                    Chaos.Map map = (tWorldMapTreeNode.Parent.Parent as MapTreeNode).Map;
                    Chaos.WorldMap newWmap = MapsCache.WorldMaps.FirstOrDefault(kvp => WMapFieldNameCombox.Text == kvp.Key.ToString() || WMapFieldNameCombox.Text == kvp.Value.Field).Value;
                    Chaos.Point newPoint = ((int)WMapSourceXNum.Value, (int)WMapSourceYNum.Value);


                    if(newWmap == null)
                        MessageBox.Show("WorldMap does not exist.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else if (map.WorldMaps.ContainsKey(newPoint))
                        MessageBox.Show("Map already contains WorldMap on that point.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        map.WorldMaps.Remove(tWorldMapTreeNode.Point);
                        map.WorldMaps.Add(newPoint, newWmap);

                        MapsCache.Save();
                        LoadTrees();

                        MapTree.Nodes[map.Name].Expand();
                        MapTree.Nodes[map.Name].Nodes["WorldMaps"].Expand();
                        MapTree.SelectedNode = MapTree.Nodes[map.Name].Nodes[newWmap.CheckSum.ToString()];
                    }
                }
                else if (WorldMapTree.SelectedNode is WorldMapTreeNode uWorldMapTreeNode && MainTabControl.SelectedTab == WorldMapsTab)
                {
                    Chaos.WorldMap oldMap = uWorldMapTreeNode.WorldMap;
                    Chaos.WorldMap newWmap = new Chaos.WorldMap(WMapFieldNameCombox.Text, oldMap.Nodes.ToArray());

                    if (newWmap == null || oldMap == null)
                        MessageBox.Show("Error, check values.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else if (MapsCache.WorldMaps.ContainsKey(newWmap.CheckSum) && oldMap.CheckSum != newWmap.CheckSum)
                        MessageBox.Show("That worldmap already exists.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        MapsCache.WorldMaps.Remove(oldMap.CheckSum);
                        MapsCache.WorldMaps.Add(newWmap.CheckSum, newWmap);

                        //correct instanced map data
                        foreach (Chaos.Map map in MapsCache.Maps.Values.ToList())
                            foreach (KeyValuePair<Chaos.Point, Chaos.WorldMap> kvp in map.WorldMaps.ToList())
                                if (kvp.Value.CheckSum == oldMap.CheckSum)
                                    MapsCache.Maps[map.Id].WorldMaps[kvp.Key] = newWmap;

                        MapsCache.Save();
                        LoadTrees();

                        WorldMapTree.SelectedNode = WorldMapTree.Nodes[newWmap.CheckSum.ToString()];
                    }
                }
            }
            catch
            {
                MessageBox.Show("Exception, check values.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteWMapBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (MapTree.SelectedNode is WorldMapTreeNode tWorldMapTreeNode && MainTabControl.SelectedTab == MapsTab)
                {
                    Chaos.Map map = (tWorldMapTreeNode.Parent.Parent as MapTreeNode).Map;

                    if (!map.WorldMaps.ContainsKey(tWorldMapTreeNode.Point))
                        MessageBox.Show("No WorldMap exists on that point.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        map.WorldMaps.Remove(tWorldMapTreeNode.Point);

                        MapsCache.Save();
                        LoadTrees();

                        MapTree.Nodes[map.Name].Expand();
                        MapTree.Nodes[map.Name].Nodes["WorldMaps"].Expand();
                    }
                }
                else if (WorldMapTree.SelectedNode is WorldMapTreeNode nWorldMapTreeNode && MainTabControl.SelectedTab == WorldMapsTab)
                {
                    if (!MapsCache.WorldMaps.ContainsKey(nWorldMapTreeNode.WorldMap.CheckSum))
                        MessageBox.Show("That worldmap doesn't exist.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        MapsCache.WorldMaps.Remove(nWorldMapTreeNode.WorldMap.CheckSum);

                        MapsCache.Save();
                        LoadTrees();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Exception, check values.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region WorldMapNodes
        private void AddNodeBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (WorldMapTree.SelectedNode is WorldMapTreeNode tWorldMapTreeNode && MainTabControl.SelectedTab == WorldMapsTab)
                {
                    Chaos.WorldMap wMap = tWorldMapTreeNode.WorldMap;
                    Chaos.WorldMapNode newNode = new Chaos.WorldMapNode(((int)NodePositionXNum.Value, (int)NodePositionYNum.Value), MapsCache.Maps[(ushort)NodeTargetMapIDNum.Value].Name, (ushort)NodeTargetMapIDNum.Value, ((int)NodeTargetXNum.Value, (int)NodeTargetYNum.Value));

                    if (wMap.Nodes.Contains(newNode))
                        MessageBox.Show("WorldMap already contains that Node.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        wMap.Nodes.Add(newNode);

                        MapsCache.Save();
                        LoadTrees();

                        string wMapNum = wMap.CheckSum.ToString();
                        WorldMapTree.Nodes[wMapNum].Expand();
                        WorldMapTree.SelectedNode = WorldMapTree.Nodes[wMapNum].Nodes[newNode.CheckSum.ToString()];
                    }
                }
            }
            catch
            {
                MessageBox.Show("Exception, check values.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChangeNodeBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (WorldMapTree.SelectedNode is WorldMapNodeTreeNode tWorldMapNodeTreeNode && MainTabControl.SelectedTab == WorldMapsTab)
                {
                    Chaos.WorldMapNode node = tWorldMapNodeTreeNode.WorldMapNode;
                    Chaos.WorldMap wMap = (tWorldMapNodeTreeNode.Parent as WorldMapTreeNode).WorldMap;
                    Chaos.WorldMapNode newNode = new Chaos.WorldMapNode(((int)NodePositionXNum.Value, (int)NodePositionYNum.Value), MapsCache.Maps[(ushort)NodeTargetMapIDNum.Value].Name, (ushort)NodeTargetMapIDNum.Value, ((int)NodeTargetXNum.Value, (int)NodeTargetYNum.Value));

                    if (!wMap.Nodes.Contains(node))
                        MessageBox.Show("WorldMap does not contain that Node.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        wMap.Nodes.Remove(node);
                        wMap.Nodes.Add(newNode);

                        MapsCache.Save();
                        LoadTrees();

                        string wMapNum = wMap.CheckSum.ToString();
                        WorldMapTree.Nodes[wMapNum].Expand();
                        WorldMapTree.SelectedNode = WorldMapTree.Nodes[wMapNum].Nodes[newNode.CheckSum.ToString()];
                    }
                }
            }
            catch
            {
                MessageBox.Show("Exception, check values.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteNodeBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (WorldMapTree.SelectedNode is WorldMapNodeTreeNode tWorldMapNodeTreeNode && MainTabControl.SelectedTab == WorldMapsTab)
                {
                    Chaos.WorldMapNode node = tWorldMapNodeTreeNode.WorldMapNode;
                    Chaos.WorldMap wMap = (tWorldMapNodeTreeNode.Parent as WorldMapTreeNode).WorldMap;

                    if (!wMap.Nodes.Contains(node))
                        MessageBox.Show("WorldMap does not contain that Node.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        wMap.Nodes.Remove(node);

                        MapsCache.Save();
                        LoadTrees();

                        string wMapNum = wMap.CheckSum.ToString();
                        WorldMapTree.Nodes[wMapNum].Expand();
                        WorldMapTree.SelectedNode = WorldMapTree.Nodes[wMapNum];
                    }
                }
            }
            catch
            {
                MessageBox.Show("Exception, check values.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void NodePositionSelectorBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (MainTabControl.SelectedTab == WorldMapsTab)
                {
                    Chaos.WorldMap wMap = null;

                    if (WorldMapTree.SelectedNode is WorldMapNodeTreeNode tWorldMapNodeTreeNode)
                        wMap = (tWorldMapNodeTreeNode.Parent as WorldMapTreeNode).WorldMap;
                    else if (WorldMapTree.SelectedNode is WorldMapTreeNode tWorldMapTreeNode)
                        wMap = tWorldMapTreeNode.WorldMap;

                    PositionSelector newSelector = new PositionSelector(this, wMap.Field);
                    newSelector.Show();
                }
            }
            catch { }
        }
        #endregion
    }
}
