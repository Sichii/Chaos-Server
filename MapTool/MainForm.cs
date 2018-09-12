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
using System.Windows.Forms;
#pragma warning disable IDE0007 // Use implicit type
#pragma warning disable IDE0003 // Remove qualification

namespace ChaosTool
{

    internal class MainForm : Form
    {
        internal System.ComponentModel.IContainer components = null;
        internal TreeView MapTree;
        private TabControl mainTabControl;
        private TabPage mapsTab;
        private TabPage worldMapsTab;
        private Label addWorldMapNodeLbl;
        private Label mapIdLbl;
        private Label mapSizeXLbl;
        private Label mapSizeYLbl;
        private Label mapFlagsLbl;
        private Label mapNameLbl;
        private Label mapMusicLbl;
        private GroupBox addMapGbox;
        private CheckBox noSpellsCbox;
        private CheckBox noSkillsCbox;
        private CheckBox snowCbox;
        private CheckBox hostileCbox;
        private NumericUpDown musicNum;
        private NumericUpDown sizeYNum;
        private NumericUpDown sizeXNum;
        private NumericUpDown mapIdNum;
        private Button addMapBtn;
        private TextBox mapNameTbox;
        private Label flagsSumLbl;
        private GroupBox addWarpGbox;
        private GroupBox addWorldMapGbox;
        private CheckBox pvpCbox;
        private Button changeBtn;
        private Button deleteMapBtn;
        private Button changeWarpBtn;
        private Button addWarpBtn;
        private Button deleteWarpBtn;
        private NumericUpDown targetIDNum;
        private Label TargetIDLbl;
        private NumericUpDown targetYNum;
        private Label targetYLbl;
        private NumericUpDown targetXNum;
        private Label targetXLbl;
        private NumericUpDown sourceYNum;
        private Label sourceYLbl;
        private NumericUpDown sourceXNum;
        private Label sourceXLbl;
        internal TreeView WorldMapTree;
        internal MapsCache MapsCache { get; }

        internal MainForm()
        {
            InitializeComponent();
            MapsCache = new MapsCache(this);
            MapTree.TreeViewNodeSorter = new TreeNodeSorter();
            WorldMapTree.TreeViewNodeSorter = new TreeNodeSorter();
            MapsCache.Load();
            LoadTree();
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
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.mapsTab = new System.Windows.Forms.TabPage();
            this.worldMapsTab = new System.Windows.Forms.TabPage();
            this.WorldMapTree = new System.Windows.Forms.TreeView();
            this.addWorldMapNodeLbl = new System.Windows.Forms.Label();
            this.mapIdLbl = new System.Windows.Forms.Label();
            this.mapSizeXLbl = new System.Windows.Forms.Label();
            this.mapSizeYLbl = new System.Windows.Forms.Label();
            this.mapFlagsLbl = new System.Windows.Forms.Label();
            this.mapNameLbl = new System.Windows.Forms.Label();
            this.mapMusicLbl = new System.Windows.Forms.Label();
            this.addMapGbox = new System.Windows.Forms.GroupBox();
            this.deleteMapBtn = new System.Windows.Forms.Button();
            this.changeBtn = new System.Windows.Forms.Button();
            this.pvpCbox = new System.Windows.Forms.CheckBox();
            this.addMapBtn = new System.Windows.Forms.Button();
            this.mapNameTbox = new System.Windows.Forms.TextBox();
            this.flagsSumLbl = new System.Windows.Forms.Label();
            this.noSpellsCbox = new System.Windows.Forms.CheckBox();
            this.noSkillsCbox = new System.Windows.Forms.CheckBox();
            this.snowCbox = new System.Windows.Forms.CheckBox();
            this.hostileCbox = new System.Windows.Forms.CheckBox();
            this.musicNum = new System.Windows.Forms.NumericUpDown();
            this.sizeYNum = new System.Windows.Forms.NumericUpDown();
            this.sizeXNum = new System.Windows.Forms.NumericUpDown();
            this.mapIdNum = new System.Windows.Forms.NumericUpDown();
            this.addWarpGbox = new System.Windows.Forms.GroupBox();
            this.changeWarpBtn = new System.Windows.Forms.Button();
            this.addWarpBtn = new System.Windows.Forms.Button();
            this.deleteWarpBtn = new System.Windows.Forms.Button();
            this.targetIDNum = new System.Windows.Forms.NumericUpDown();
            this.TargetIDLbl = new System.Windows.Forms.Label();
            this.targetYNum = new System.Windows.Forms.NumericUpDown();
            this.targetYLbl = new System.Windows.Forms.Label();
            this.targetXNum = new System.Windows.Forms.NumericUpDown();
            this.targetXLbl = new System.Windows.Forms.Label();
            this.sourceYNum = new System.Windows.Forms.NumericUpDown();
            this.sourceYLbl = new System.Windows.Forms.Label();
            this.sourceXNum = new System.Windows.Forms.NumericUpDown();
            this.sourceXLbl = new System.Windows.Forms.Label();
            this.addWorldMapGbox = new System.Windows.Forms.GroupBox();
            this.mainTabControl.SuspendLayout();
            this.mapsTab.SuspendLayout();
            this.worldMapsTab.SuspendLayout();
            this.addMapGbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.musicNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sizeYNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sizeXNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mapIdNum)).BeginInit();
            this.addWarpGbox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.targetIDNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.targetYNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.targetXNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sourceYNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sourceXNum)).BeginInit();
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
            this.MapTree.Size = new System.Drawing.Size(418, 482);
            this.MapTree.TabIndex = 0;
            this.MapTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.MapTree_NodeMouseClick);
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add(this.mapsTab);
            this.mainTabControl.Controls.Add(this.worldMapsTab);
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Left;
            this.mainTabControl.Location = new System.Drawing.Point(0, 0);
            this.mainTabControl.Margin = new System.Windows.Forms.Padding(4);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(434, 519);
            this.mainTabControl.TabIndex = 1;
            // 
            // mapsTab
            // 
            this.mapsTab.Controls.Add(this.MapTree);
            this.mapsTab.Location = new System.Drawing.Point(4, 25);
            this.mapsTab.Margin = new System.Windows.Forms.Padding(4);
            this.mapsTab.Name = "mapsTab";
            this.mapsTab.Padding = new System.Windows.Forms.Padding(4);
            this.mapsTab.Size = new System.Drawing.Size(426, 490);
            this.mapsTab.TabIndex = 0;
            this.mapsTab.Text = "Maps";
            this.mapsTab.UseVisualStyleBackColor = true;
            // 
            // worldMapsTab
            // 
            this.worldMapsTab.Controls.Add(this.WorldMapTree);
            this.worldMapsTab.Location = new System.Drawing.Point(4, 25);
            this.worldMapsTab.Margin = new System.Windows.Forms.Padding(4);
            this.worldMapsTab.Name = "worldMapsTab";
            this.worldMapsTab.Padding = new System.Windows.Forms.Padding(4);
            this.worldMapsTab.Size = new System.Drawing.Size(426, 490);
            this.worldMapsTab.TabIndex = 1;
            this.worldMapsTab.Text = "WorldMaps";
            this.worldMapsTab.UseVisualStyleBackColor = true;
            // 
            // WorldMapTree
            // 
            this.WorldMapTree.BackColor = System.Drawing.Color.White;
            this.WorldMapTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WorldMapTree.ForeColor = System.Drawing.Color.Black;
            this.WorldMapTree.Location = new System.Drawing.Point(4, 4);
            this.WorldMapTree.Margin = new System.Windows.Forms.Padding(4);
            this.WorldMapTree.Name = "WorldMapTree";
            this.WorldMapTree.Size = new System.Drawing.Size(418, 482);
            this.WorldMapTree.TabIndex = 1;
            // 
            // addWorldMapNodeLbl
            // 
            this.addWorldMapNodeLbl.AutoSize = true;
            this.addWorldMapNodeLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addWorldMapNodeLbl.Location = new System.Drawing.Point(436, 559);
            this.addWorldMapNodeLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.addWorldMapNodeLbl.Name = "addWorldMapNodeLbl";
            this.addWorldMapNodeLbl.Size = new System.Drawing.Size(168, 20);
            this.addWorldMapNodeLbl.TabIndex = 5;
            this.addWorldMapNodeLbl.Text = "Add WorldMapNode";
            // 
            // mapIdLbl
            // 
            this.mapIdLbl.AutoSize = true;
            this.mapIdLbl.Location = new System.Drawing.Point(29, 29);
            this.mapIdLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mapIdLbl.Name = "mapIdLbl";
            this.mapIdLbl.Size = new System.Drawing.Size(27, 16);
            this.mapIdLbl.TabIndex = 8;
            this.mapIdLbl.Text = "ID: ";
            // 
            // mapSizeXLbl
            // 
            this.mapSizeXLbl.AutoSize = true;
            this.mapSizeXLbl.Location = new System.Drawing.Point(8, 57);
            this.mapSizeXLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mapSizeXLbl.Name = "mapSizeXLbl";
            this.mapSizeXLbl.Size = new System.Drawing.Size(48, 16);
            this.mapSizeXLbl.TabIndex = 9;
            this.mapSizeXLbl.Text = "SizeX: ";
            // 
            // mapSizeYLbl
            // 
            this.mapSizeYLbl.AutoSize = true;
            this.mapSizeYLbl.Location = new System.Drawing.Point(8, 89);
            this.mapSizeYLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mapSizeYLbl.Name = "mapSizeYLbl";
            this.mapSizeYLbl.Size = new System.Drawing.Size(49, 16);
            this.mapSizeYLbl.TabIndex = 10;
            this.mapSizeYLbl.Text = "SizeY: ";
            // 
            // mapFlagsLbl
            // 
            this.mapFlagsLbl.AutoSize = true;
            this.mapFlagsLbl.Location = new System.Drawing.Point(153, 57);
            this.mapFlagsLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mapFlagsLbl.Name = "mapFlagsLbl";
            this.mapFlagsLbl.Size = new System.Drawing.Size(48, 16);
            this.mapFlagsLbl.TabIndex = 11;
            this.mapFlagsLbl.Text = "Flags: ";
            // 
            // mapNameLbl
            // 
            this.mapNameLbl.AutoSize = true;
            this.mapNameLbl.Location = new System.Drawing.Point(153, 25);
            this.mapNameLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mapNameLbl.Name = "mapNameLbl";
            this.mapNameLbl.Size = new System.Drawing.Size(51, 16);
            this.mapNameLbl.TabIndex = 12;
            this.mapNameLbl.Text = "Name: ";
            // 
            // mapMusicLbl
            // 
            this.mapMusicLbl.AutoSize = true;
            this.mapMusicLbl.Location = new System.Drawing.Point(152, 91);
            this.mapMusicLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mapMusicLbl.Name = "mapMusicLbl";
            this.mapMusicLbl.Size = new System.Drawing.Size(49, 16);
            this.mapMusicLbl.TabIndex = 13;
            this.mapMusicLbl.Text = "Music: ";
            // 
            // addMapGbox
            // 
            this.addMapGbox.Controls.Add(this.deleteMapBtn);
            this.addMapGbox.Controls.Add(this.changeBtn);
            this.addMapGbox.Controls.Add(this.pvpCbox);
            this.addMapGbox.Controls.Add(this.addMapBtn);
            this.addMapGbox.Controls.Add(this.mapNameTbox);
            this.addMapGbox.Controls.Add(this.flagsSumLbl);
            this.addMapGbox.Controls.Add(this.noSpellsCbox);
            this.addMapGbox.Controls.Add(this.noSkillsCbox);
            this.addMapGbox.Controls.Add(this.snowCbox);
            this.addMapGbox.Controls.Add(this.hostileCbox);
            this.addMapGbox.Controls.Add(this.musicNum);
            this.addMapGbox.Controls.Add(this.sizeYNum);
            this.addMapGbox.Controls.Add(this.sizeXNum);
            this.addMapGbox.Controls.Add(this.mapIdNum);
            this.addMapGbox.Controls.Add(this.mapIdLbl);
            this.addMapGbox.Controls.Add(this.mapMusicLbl);
            this.addMapGbox.Controls.Add(this.mapNameLbl);
            this.addMapGbox.Controls.Add(this.mapSizeXLbl);
            this.addMapGbox.Controls.Add(this.mapFlagsLbl);
            this.addMapGbox.Controls.Add(this.mapSizeYLbl);
            this.addMapGbox.Dock = System.Windows.Forms.DockStyle.Top;
            this.addMapGbox.Location = new System.Drawing.Point(434, 0);
            this.addMapGbox.Margin = new System.Windows.Forms.Padding(4);
            this.addMapGbox.Name = "addMapGbox";
            this.addMapGbox.Padding = new System.Windows.Forms.Padding(4);
            this.addMapGbox.Size = new System.Drawing.Size(652, 130);
            this.addMapGbox.TabIndex = 14;
            this.addMapGbox.TabStop = false;
            this.addMapGbox.Text = "Add Map";
            // 
            // deleteMapBtn
            // 
            this.deleteMapBtn.AutoSize = true;
            this.deleteMapBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.deleteMapBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteMapBtn.Location = new System.Drawing.Point(528, 27);
            this.deleteMapBtn.Name = "deleteMapBtn";
            this.deleteMapBtn.Size = new System.Drawing.Size(117, 28);
            this.deleteMapBtn.TabIndex = 29;
            this.deleteMapBtn.Text = "Delete Selected";
            this.deleteMapBtn.UseVisualStyleBackColor = true;
            this.deleteMapBtn.Click += new System.EventHandler(this.DeleteMapBtn_Click);
            // 
            // changeBtn
            // 
            this.changeBtn.AutoSize = true;
            this.changeBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.changeBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.changeBtn.Location = new System.Drawing.Point(521, 61);
            this.changeBtn.Name = "changeBtn";
            this.changeBtn.Size = new System.Drawing.Size(124, 28);
            this.changeBtn.TabIndex = 28;
            this.changeBtn.Text = "Change Selected";
            this.changeBtn.UseVisualStyleBackColor = true;
            this.changeBtn.Click += new System.EventHandler(this.ChangeBtn_Click);
            // 
            // pvpCbox
            // 
            this.pvpCbox.AutoSize = true;
            this.pvpCbox.Location = new System.Drawing.Point(335, 64);
            this.pvpCbox.Name = "pvpCbox";
            this.pvpCbox.Size = new System.Drawing.Size(52, 20);
            this.pvpCbox.TabIndex = 27;
            this.pvpCbox.Text = "PvP";
            this.pvpCbox.UseVisualStyleBackColor = true;
            this.pvpCbox.CheckedChanged += new System.EventHandler(this.PvPCbox_CheckedChanged);
            // 
            // addMapBtn
            // 
            this.addMapBtn.AutoSize = true;
            this.addMapBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.addMapBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addMapBtn.Location = new System.Drawing.Point(570, 95);
            this.addMapBtn.Name = "addMapBtn";
            this.addMapBtn.Size = new System.Drawing.Size(75, 28);
            this.addMapBtn.TabIndex = 26;
            this.addMapBtn.Text = "Add Map";
            this.addMapBtn.UseVisualStyleBackColor = true;
            this.addMapBtn.Click += new System.EventHandler(this.AddMapBtn_Click);
            // 
            // mapNameTbox
            // 
            this.mapNameTbox.BackColor = System.Drawing.Color.White;
            this.mapNameTbox.ForeColor = System.Drawing.Color.Black;
            this.mapNameTbox.Location = new System.Drawing.Point(201, 22);
            this.mapNameTbox.Name = "mapNameTbox";
            this.mapNameTbox.Size = new System.Drawing.Size(120, 22);
            this.mapNameTbox.TabIndex = 25;
            // 
            // flagsSumLbl
            // 
            this.flagsSumLbl.AutoSize = true;
            this.flagsSumLbl.Location = new System.Drawing.Point(198, 57);
            this.flagsSumLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.flagsSumLbl.Name = "flagsSumLbl";
            this.flagsSumLbl.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.flagsSumLbl.Size = new System.Drawing.Size(15, 16);
            this.flagsSumLbl.TabIndex = 24;
            this.flagsSumLbl.Text = "0";
            this.flagsSumLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // noSpellsCbox
            // 
            this.noSpellsCbox.AutoSize = true;
            this.noSpellsCbox.Location = new System.Drawing.Point(420, 12);
            this.noSpellsCbox.Name = "noSpellsCbox";
            this.noSpellsCbox.Size = new System.Drawing.Size(83, 20);
            this.noSpellsCbox.TabIndex = 23;
            this.noSpellsCbox.Text = "NoSpells";
            this.noSpellsCbox.UseVisualStyleBackColor = true;
            this.noSpellsCbox.CheckedChanged += new System.EventHandler(this.NoSpellsCbox_CheckedChanged);
            // 
            // noSkillsCbox
            // 
            this.noSkillsCbox.AutoSize = true;
            this.noSkillsCbox.Location = new System.Drawing.Point(420, 38);
            this.noSkillsCbox.Name = "noSkillsCbox";
            this.noSkillsCbox.Size = new System.Drawing.Size(77, 20);
            this.noSkillsCbox.TabIndex = 22;
            this.noSkillsCbox.Text = "NoSkills";
            this.noSkillsCbox.UseVisualStyleBackColor = true;
            this.noSkillsCbox.CheckedChanged += new System.EventHandler(this.NoSkillsCbox_CheckedChanged);
            // 
            // snowCbox
            // 
            this.snowCbox.AutoSize = true;
            this.snowCbox.Location = new System.Drawing.Point(335, 38);
            this.snowCbox.Name = "snowCbox";
            this.snowCbox.Size = new System.Drawing.Size(60, 20);
            this.snowCbox.TabIndex = 21;
            this.snowCbox.Text = "Snow";
            this.snowCbox.UseVisualStyleBackColor = true;
            this.snowCbox.CheckedChanged += new System.EventHandler(this.SnowCbox_CheckedChanged);
            // 
            // hostileCbox
            // 
            this.hostileCbox.AutoSize = true;
            this.hostileCbox.Location = new System.Drawing.Point(335, 12);
            this.hostileCbox.Name = "hostileCbox";
            this.hostileCbox.Size = new System.Drawing.Size(69, 20);
            this.hostileCbox.TabIndex = 20;
            this.hostileCbox.Text = "Hostile";
            this.hostileCbox.UseVisualStyleBackColor = true;
            this.hostileCbox.CheckedChanged += new System.EventHandler(this.HostileCbox_CheckedChanged);
            // 
            // musicNum
            // 
            this.musicNum.BackColor = System.Drawing.Color.White;
            this.musicNum.ForeColor = System.Drawing.Color.Black;
            this.musicNum.Location = new System.Drawing.Point(201, 89);
            this.musicNum.Maximum = new decimal(new int[] {
            127,
            0,
            0,
            0});
            this.musicNum.Minimum = new decimal(new int[] {
            128,
            0,
            0,
            -2147483648});
            this.musicNum.Name = "musicNum";
            this.musicNum.Size = new System.Drawing.Size(44, 22);
            this.musicNum.TabIndex = 19;
            this.musicNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.musicNum.Value = new decimal(new int[] {
            127,
            0,
            0,
            0});
            // 
            // sizeYNum
            // 
            this.sizeYNum.BackColor = System.Drawing.Color.White;
            this.sizeYNum.ForeColor = System.Drawing.Color.Black;
            this.sizeYNum.Location = new System.Drawing.Point(64, 87);
            this.sizeYNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.sizeYNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.sizeYNum.Name = "sizeYNum";
            this.sizeYNum.Size = new System.Drawing.Size(44, 22);
            this.sizeYNum.TabIndex = 18;
            this.sizeYNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.sizeYNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // sizeXNum
            // 
            this.sizeXNum.BackColor = System.Drawing.Color.White;
            this.sizeXNum.ForeColor = System.Drawing.Color.Black;
            this.sizeXNum.Location = new System.Drawing.Point(63, 55);
            this.sizeXNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.sizeXNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.sizeXNum.Name = "sizeXNum";
            this.sizeXNum.Size = new System.Drawing.Size(44, 22);
            this.sizeXNum.TabIndex = 17;
            this.sizeXNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.sizeXNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // mapIdNum
            // 
            this.mapIdNum.BackColor = System.Drawing.Color.White;
            this.mapIdNum.ForeColor = System.Drawing.Color.Black;
            this.mapIdNum.Location = new System.Drawing.Point(63, 27);
            this.mapIdNum.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.mapIdNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.mapIdNum.Name = "mapIdNum";
            this.mapIdNum.Size = new System.Drawing.Size(58, 22);
            this.mapIdNum.TabIndex = 14;
            this.mapIdNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mapIdNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // addWarpGbox
            // 
            this.addWarpGbox.Controls.Add(this.changeWarpBtn);
            this.addWarpGbox.Controls.Add(this.addWarpBtn);
            this.addWarpGbox.Controls.Add(this.deleteWarpBtn);
            this.addWarpGbox.Controls.Add(this.targetIDNum);
            this.addWarpGbox.Controls.Add(this.TargetIDLbl);
            this.addWarpGbox.Controls.Add(this.targetYNum);
            this.addWarpGbox.Controls.Add(this.targetYLbl);
            this.addWarpGbox.Controls.Add(this.targetXNum);
            this.addWarpGbox.Controls.Add(this.targetXLbl);
            this.addWarpGbox.Controls.Add(this.sourceYNum);
            this.addWarpGbox.Controls.Add(this.sourceYLbl);
            this.addWarpGbox.Controls.Add(this.sourceXNum);
            this.addWarpGbox.Controls.Add(this.sourceXLbl);
            this.addWarpGbox.Dock = System.Windows.Forms.DockStyle.Top;
            this.addWarpGbox.Location = new System.Drawing.Point(434, 130);
            this.addWarpGbox.Name = "addWarpGbox";
            this.addWarpGbox.Size = new System.Drawing.Size(652, 130);
            this.addWarpGbox.TabIndex = 15;
            this.addWarpGbox.TabStop = false;
            this.addWarpGbox.Text = "Add Warp";
            // 
            // changeWarpBtn
            // 
            this.changeWarpBtn.AutoSize = true;
            this.changeWarpBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.changeWarpBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.changeWarpBtn.Location = new System.Drawing.Point(516, 62);
            this.changeWarpBtn.Name = "changeWarpBtn";
            this.changeWarpBtn.Size = new System.Drawing.Size(124, 28);
            this.changeWarpBtn.TabIndex = 30;
            this.changeWarpBtn.Text = "Change Selected";
            this.changeWarpBtn.UseVisualStyleBackColor = true;
            this.changeWarpBtn.Click += new System.EventHandler(this.ChangeWarpBtn_Click);
            // 
            // addWarpBtn
            // 
            this.addWarpBtn.AutoSize = true;
            this.addWarpBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.addWarpBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addWarpBtn.Location = new System.Drawing.Point(458, 96);
            this.addWarpBtn.Name = "addWarpBtn";
            this.addWarpBtn.Size = new System.Drawing.Size(182, 28);
            this.addWarpBtn.TabIndex = 30;
            this.addWarpBtn.Text = "Add Warp to Selected Map";
            this.addWarpBtn.UseVisualStyleBackColor = true;
            this.addWarpBtn.Click += new System.EventHandler(this.AddWarpBtn_Click);
            // 
            // deleteWarpBtn
            // 
            this.deleteWarpBtn.AutoSize = true;
            this.deleteWarpBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.deleteWarpBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteWarpBtn.Location = new System.Drawing.Point(523, 28);
            this.deleteWarpBtn.Name = "deleteWarpBtn";
            this.deleteWarpBtn.Size = new System.Drawing.Size(117, 28);
            this.deleteWarpBtn.TabIndex = 30;
            this.deleteWarpBtn.Text = "Delete Selected";
            this.deleteWarpBtn.UseVisualStyleBackColor = true;
            this.deleteWarpBtn.Click += new System.EventHandler(this.DeleteWarpBtn_Click);
            // 
            // targetIDNum
            // 
            this.targetIDNum.BackColor = System.Drawing.Color.White;
            this.targetIDNum.ForeColor = System.Drawing.Color.Black;
            this.targetIDNum.Location = new System.Drawing.Point(218, 86);
            this.targetIDNum.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.targetIDNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.targetIDNum.Name = "targetIDNum";
            this.targetIDNum.Size = new System.Drawing.Size(60, 22);
            this.targetIDNum.TabIndex = 38;
            this.targetIDNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.targetIDNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // TargetIDLbl
            // 
            this.TargetIDLbl.AutoSize = true;
            this.TargetIDLbl.Location = new System.Drawing.Point(147, 88);
            this.TargetIDLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.TargetIDLbl.Name = "TargetIDLbl";
            this.TargetIDLbl.Size = new System.Drawing.Size(64, 16);
            this.TargetIDLbl.TabIndex = 37;
            this.TargetIDLbl.Text = "TargetID:";
            // 
            // targetYNum
            // 
            this.targetYNum.BackColor = System.Drawing.Color.White;
            this.targetYNum.ForeColor = System.Drawing.Color.Black;
            this.targetYNum.Location = new System.Drawing.Point(218, 58);
            this.targetYNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.targetYNum.Name = "targetYNum";
            this.targetYNum.Size = new System.Drawing.Size(44, 22);
            this.targetYNum.TabIndex = 36;
            this.targetYNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.targetYNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // targetYLbl
            // 
            this.targetYLbl.AutoSize = true;
            this.targetYLbl.Location = new System.Drawing.Point(151, 60);
            this.targetYLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.targetYLbl.Name = "targetYLbl";
            this.targetYLbl.Size = new System.Drawing.Size(60, 16);
            this.targetYLbl.TabIndex = 35;
            this.targetYLbl.Text = "TargetY:";
            // 
            // targetXNum
            // 
            this.targetXNum.BackColor = System.Drawing.Color.White;
            this.targetXNum.ForeColor = System.Drawing.Color.Black;
            this.targetXNum.Location = new System.Drawing.Point(217, 28);
            this.targetXNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.targetXNum.Name = "targetXNum";
            this.targetXNum.Size = new System.Drawing.Size(44, 22);
            this.targetXNum.TabIndex = 34;
            this.targetXNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.targetXNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // targetXLbl
            // 
            this.targetXLbl.AutoSize = true;
            this.targetXLbl.Location = new System.Drawing.Point(151, 30);
            this.targetXLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.targetXLbl.Name = "targetXLbl";
            this.targetXLbl.Size = new System.Drawing.Size(59, 16);
            this.targetXLbl.TabIndex = 33;
            this.targetXLbl.Text = "TargetX:";
            // 
            // sourceYNum
            // 
            this.sourceYNum.BackColor = System.Drawing.Color.White;
            this.sourceYNum.ForeColor = System.Drawing.Color.Black;
            this.sourceYNum.Location = new System.Drawing.Point(89, 58);
            this.sourceYNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.sourceYNum.Name = "sourceYNum";
            this.sourceYNum.Size = new System.Drawing.Size(44, 22);
            this.sourceYNum.TabIndex = 32;
            this.sourceYNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.sourceYNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // sourceYLbl
            // 
            this.sourceYLbl.AutoSize = true;
            this.sourceYLbl.Location = new System.Drawing.Point(20, 60);
            this.sourceYLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.sourceYLbl.Name = "sourceYLbl";
            this.sourceYLbl.Size = new System.Drawing.Size(63, 16);
            this.sourceYLbl.TabIndex = 31;
            this.sourceYLbl.Text = "SourceY:";
            // 
            // sourceXNum
            // 
            this.sourceXNum.BackColor = System.Drawing.Color.White;
            this.sourceXNum.ForeColor = System.Drawing.Color.Black;
            this.sourceXNum.Location = new System.Drawing.Point(89, 28);
            this.sourceXNum.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.sourceXNum.Name = "sourceXNum";
            this.sourceXNum.Size = new System.Drawing.Size(44, 22);
            this.sourceXNum.TabIndex = 30;
            this.sourceXNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.sourceXNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // sourceXLbl
            // 
            this.sourceXLbl.AutoSize = true;
            this.sourceXLbl.Location = new System.Drawing.Point(20, 30);
            this.sourceXLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.sourceXLbl.Name = "sourceXLbl";
            this.sourceXLbl.Size = new System.Drawing.Size(62, 16);
            this.sourceXLbl.TabIndex = 9;
            this.sourceXLbl.Text = "SourceX:";
            // 
            // addWorldMapGbox
            // 
            this.addWorldMapGbox.Dock = System.Windows.Forms.DockStyle.Top;
            this.addWorldMapGbox.Location = new System.Drawing.Point(434, 260);
            this.addWorldMapGbox.Name = "addWorldMapGbox";
            this.addWorldMapGbox.Size = new System.Drawing.Size(652, 129);
            this.addWorldMapGbox.TabIndex = 16;
            this.addWorldMapGbox.TabStop = false;
            this.addWorldMapGbox.Text = "Add WorldMap";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1086, 519);
            this.Controls.Add(this.addWorldMapGbox);
            this.Controls.Add(this.addWarpGbox);
            this.Controls.Add(this.addMapGbox);
            this.Controls.Add(this.addWorldMapNodeLbl);
            this.Controls.Add(this.mainTabControl);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "ChaosTool";
            this.mainTabControl.ResumeLayout(false);
            this.mapsTab.ResumeLayout(false);
            this.worldMapsTab.ResumeLayout(false);
            this.addMapGbox.ResumeLayout(false);
            this.addMapGbox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.musicNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sizeYNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sizeXNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mapIdNum)).EndInit();
            this.addWarpGbox.ResumeLayout(false);
            this.addWarpGbox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.targetIDNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.targetYNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.targetXNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sourceYNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sourceXNum)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        internal void LoadTree()
        {
            if (InvokeRequired)
                Invoke((Action)(LoadTree));
            else
            {
                MapTree.Nodes.Clear();
                WorldMapTree.Nodes.Clear();

                //for each map
                foreach (Chaos.Map map in MapsCache.Maps.Values)
                {
                    //each map gets a node
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

                    //each warp gets a subnode
                    foreach (Chaos.Warp warp in map.Warps.Values)
                        try
                        {
                            Warps.Nodes.Add(new WarpTreeNode(warp, $@"{warp.Point} => {warp.TargetMapId} : {warp.TargetPoint} - ({MapsCache.Maps[warp.TargetMapId].Name})"));
                        }
                        catch
                        {
                            MessageBox.Show($@"A warp's target map doesn't exist. TargetMapID={warp.TargetMapId}", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                    //each worldmap gets a subnode
                    foreach (KeyValuePair<Chaos.Point, Chaos.WorldMap> kvp in map.WorldMaps)
                        WorldMaps.Nodes.Add(new WorldMapTreeNode(kvp.Value, $@"{kvp.Key.ToString()} => {kvp.Value.GetCheckSum()}"));

                    //add this map to the map tree
                    MapTree.Nodes.Add(Map);
                }

                //for each worldmap
                foreach (KeyValuePair<uint, Chaos.WorldMap> kvp in MapsCache.WorldMaps)
                {
                    //each worldmap gets a node
                    var WorldMap = new WorldMapTreeNode(kvp.Value, kvp.Key.ToString());

                    //each worldmapnode gets a subnode
                    foreach (Chaos.WorldMapNode wmn in kvp.Value.Nodes)
                        WorldMap.Nodes.Add(new WorldMapNodeTreeNode(wmn, $@"{wmn.Position} => {wmn.MapId} : {wmn.Point} - ({MapsCache.Maps[wmn.MapId].Name})"));

                    //add this worldmap to the worldmap tree
                    WorldMapTree.Nodes.Add(WorldMap);
                }
            }
        }

        private void AddMapBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string message = "";
                string mapName = mapNameTbox.Text;
                ushort mapId = decimal.ToUInt16(mapIdNum.Value);
                byte sizeX = decimal.ToByte(sizeXNum.Value);
                byte sizeY = decimal.ToByte(sizeYNum.Value);
                sbyte music = decimal.ToSByte(musicNum.Value);

                if (MapsCache.Maps.ContainsKey(mapId))
                    message = $@"Map ID:{mapIdNum.Value} is already in use. Overwrite? Will delete doors and warps.";
                else
                    message = $@"Add this as a new map? Make sure info is correct.";

                if (uint.TryParse(flagsSumLbl.Text, out uint flags))
                {
                    if (MessageBox.Show(message, "Chaos MapTool", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        MapsCache.Maps[mapId] = new Chaos.Map(mapId, sizeX, sizeY, (Chaos.MapFlags)flags, mapName, music);
                        MapsCache.Save();
                        LoadTree();
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
                    string mapName = mapNameTbox.Text;
                    ushort mapId = tMapTreeNode.Map.Id;
                    byte sizeX = decimal.ToByte(sizeXNum.Value);
                    byte sizeY = decimal.ToByte(sizeYNum.Value);
                    sbyte music = decimal.ToSByte(musicNum.Value);
                    uint flags = 0;

                    if (!MapsCache.Maps.ContainsKey(mapId))
                        MessageBox.Show("Map doesn't exist, please select a map.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        message = $@"Change map info related to Map ID: {mapId}? Doors and warps will stay.";

                        if (uint.TryParse(flagsSumLbl.Text, out flags))
                        {
                            if (MessageBox.Show(message, "Chaos MapTool", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                            {
                                MapsCache.Maps[mapId].Name = mapName;
                                MapsCache.Maps[mapId].SizeX = sizeX;
                                MapsCache.Maps[mapId].SizeY = sizeY;
                                MapsCache.Maps[mapId].Music = music;
                                MapsCache.Maps[mapId].Flags = (Chaos.MapFlags)flags;
                                MapsCache.Save();
                                LoadTree();
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
                        message = $@"Delete Map ID: {mapId}? This will destroy doors, warps, and the info.";
                        if (MessageBox.Show(message, "Chaos MapTool", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                        {
                            MapsCache.Maps.Remove(mapId);
                            MapsCache.Save();
                            LoadTree();
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Exception, check values.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddWarpBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (MapTree.SelectedNode is MapTreeNode tMapTreeNode)
                {
                    Chaos.Map map = tMapTreeNode.Map;
                    var warp = new Chaos.Warp((ushort)sourceXNum.Value, (ushort)sourceYNum.Value, (ushort)targetXNum.Value, (ushort)targetYNum.Value, map.Id, (ushort)targetIDNum.Value);

                    if (map.Warps.ContainsKey(warp.Point))
                        MessageBox.Show("Map already contains warp on that point.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        map.Warps.Add(warp.Point, warp);

                    MapsCache.Save();
                    LoadTree();
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
                    Chaos.Map map = MapsCache.Maps[oldWarp.MapId];
                    var newWarp = new Chaos.Warp((ushort)sourceXNum.Value, (ushort)sourceYNum.Value, (ushort)targetXNum.Value, (ushort)targetYNum.Value, oldWarp.MapId, (ushort)targetIDNum.Value);

                    if (!map.Warps.ContainsKey(oldWarp.Point))
                        MessageBox.Show("Map does not contain that warp.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        map.Warps.Remove(oldWarp.Point);
                        map.Warps.Add(newWarp.Point, newWarp);

                        MapsCache.Save();
                        LoadTree();
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
                Chaos.Warp warp = tWarpTreeNode.Warp;
                Chaos.Map map = MapsCache.Maps[warp.MapId];

                if (!map.Warps.ContainsKey(warp.Point))
                    MessageBox.Show("Map does not contain that warp.", "Chaos MapTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    map.Warps.Remove(warp.Point);

                    MapsCache.Save();
                    LoadTree();
                    MapTree.Nodes[map.Name].Expand();
                    MapTree.Nodes[map.Name].Nodes["Warps"].Expand();
                    MapTree.SelectedNode = MapTree.Nodes[map.Name];
                }
            }
        }

        private void HostileCbox_CheckedChanged(object sender, EventArgs e) => flagsSumLbl.Text = (uint.Parse(flagsSumLbl.Text) + (uint)Chaos.MapFlags.Hostile).ToString();

        private void SnowCbox_CheckedChanged(object sender, EventArgs e) => flagsSumLbl.Text = (uint.Parse(flagsSumLbl.Text) + (uint)Chaos.MapFlags.Snowing).ToString();

        private void PvPCbox_CheckedChanged(object sender, EventArgs e) => flagsSumLbl.Text = (uint.Parse(flagsSumLbl.Text) + (uint)Chaos.MapFlags.PvP).ToString();

        private void NoSpellsCbox_CheckedChanged(object sender, EventArgs e) => flagsSumLbl.Text = (uint.Parse(flagsSumLbl.Text) + (uint)Chaos.MapFlags.NoSpells).ToString();

        private void NoSkillsCbox_CheckedChanged(object sender, EventArgs e) => flagsSumLbl.Text = (uint.Parse(flagsSumLbl.Text) + (uint)Chaos.MapFlags.NoSkills).ToString();

        ~MainForm()
        {
            MapsCache.Save();
        }

        private void MapTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node is MapTreeNode tMapTreeNode)
            {
                Chaos.Map map = tMapTreeNode.Map;

                mapIdNum.Value = map.Id;
                sizeXNum.Value = map.SizeX;
                sizeYNum.Value = map.SizeY;
                mapNameTbox.Text = map.Name;
                flagsSumLbl.Text = map.Flags.ToString();
                musicNum.Value = map.Music;
            }
            else if (e.Node is WarpTreeNode tWarpTreeNode)
            {
                Chaos.Warp warp = tWarpTreeNode.Warp;

                sourceXNum.Value = warp.SourceX;
                sourceYNum.Value = warp.SourceY;
                targetXNum.Value = warp.TargetX;
                targetYNum.Value = warp.TargetY;
                targetIDNum.Value = warp.TargetMapId;
            }
            else
            {
                //others later
            }
        }
    }
}
