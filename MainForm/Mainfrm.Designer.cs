namespace MainForm
{
    partial class Mainfrm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Mainfrm));
            this.superTabControl1 = new DevComponents.DotNetBar.SuperTabControl();
            this.superTabControlPanel1 = new DevComponents.DotNetBar.SuperTabControlPanel();
            this.collapsibleSplitContainer3 = new DevComponents.DotNetBar.Controls.CollapsibleSplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.btLoadDxftoviewer = new DevComponents.DotNetBar.ButtonX();
            this.btLoaddxfViwer = new DevComponents.DotNetBar.ButtonX();
            this.groupPanel1 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.sliderR = new DevComponents.DotNetBar.Controls.Slider();
            this.labelXB = new DevComponents.DotNetBar.LabelX();
            this.sliderB = new DevComponents.DotNetBar.Controls.Slider();
            this.labelXG = new DevComponents.DotNetBar.LabelX();
            this.sliderG = new DevComponents.DotNetBar.Controls.Slider();
            this.labelXR = new DevComponents.DotNetBar.LabelX();
            this.图纸显示 = new DevComponents.DotNetBar.SuperTabItem();
            this.superTabControlPanel3 = new DevComponents.DotNetBar.SuperTabControlPanel();
            this.collapsibleSplitContainer1 = new DevComponents.DotNetBar.Controls.CollapsibleSplitContainer();
            this.collapsibleSplitContainer2 = new DevComponents.DotNetBar.Controls.CollapsibleSplitContainer();
            this.fCTBCode = new FastColoredTextBoxNS.FastColoredTextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btInformationdxf = new DevComponents.DotNetBar.ButtonX();
            this.btCreateDxfToTable = new DevComponents.DotNetBar.ButtonX();
            this.btCreateDxfToGcode = new DevComponents.DotNetBar.ButtonX();
            this.btCreatNewDxf = new DevComponents.DotNetBar.ButtonX();
            this.btDrawgcode = new DevComponents.DotNetBar.ButtonX();
            this.btMakeGcode = new DevComponents.DotNetBar.ButtonX();
            this.BtopenDXF = new DevComponents.DotNetBar.ButtonX();
            this.superTabItem1 = new DevComponents.DotNetBar.SuperTabItem();
            this.superTabControlPanel2 = new DevComponents.DotNetBar.SuperTabControlPanel();
            this.superTabItem2 = new DevComponents.DotNetBar.SuperTabItem();
            this.btSettings = new DevComponents.DotNetBar.ButtonItem();
            this.panelEx1 = new DevComponents.DotNetBar.PanelEx();
            this.panellog = new DevComponents.DotNetBar.PanelEx();
            ((System.ComponentModel.ISupportInitialize)(this.superTabControl1)).BeginInit();
            this.superTabControl1.SuspendLayout();
            this.superTabControlPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.collapsibleSplitContainer3)).BeginInit();
            this.collapsibleSplitContainer3.Panel1.SuspendLayout();
            this.collapsibleSplitContainer3.Panel2.SuspendLayout();
            this.collapsibleSplitContainer3.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.groupPanel1.SuspendLayout();
            this.superTabControlPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.collapsibleSplitContainer1)).BeginInit();
            this.collapsibleSplitContainer1.Panel1.SuspendLayout();
            this.collapsibleSplitContainer1.Panel2.SuspendLayout();
            this.collapsibleSplitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.collapsibleSplitContainer2)).BeginInit();
            this.collapsibleSplitContainer2.Panel1.SuspendLayout();
            this.collapsibleSplitContainer2.Panel2.SuspendLayout();
            this.collapsibleSplitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fCTBCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panelEx1.SuspendLayout();
            this.SuspendLayout();
            // 
            // superTabControl1
            // 
            this.superTabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            // 
            // 
            // 
            this.superTabControl1.ControlBox.CloseBox.Name = "";
            // 
            // 
            // 
            this.superTabControl1.ControlBox.MenuBox.Name = "";
            this.superTabControl1.ControlBox.Name = "";
            this.superTabControl1.ControlBox.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.superTabControl1.ControlBox.MenuBox,
            this.superTabControl1.ControlBox.CloseBox});
            this.superTabControl1.Controls.Add(this.superTabControlPanel1);
            this.superTabControl1.Controls.Add(this.superTabControlPanel3);
            this.superTabControl1.Controls.Add(this.superTabControlPanel2);
            this.superTabControl1.Location = new System.Drawing.Point(3, 3);
            this.superTabControl1.Name = "superTabControl1";
            this.superTabControl1.ReorderTabsEnabled = true;
            this.superTabControl1.SelectedTabFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
            this.superTabControl1.SelectedTabIndex = 0;
            this.superTabControl1.Size = new System.Drawing.Size(1228, 562);
            this.superTabControl1.TabFont = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.superTabControl1.TabIndex = 0;
            this.superTabControl1.Tabs.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.superTabItem1,
            this.图纸显示,
            this.superTabItem2,
            this.btSettings});
            this.superTabControl1.Text = "superTabControl1";
            // 
            // superTabControlPanel1
            // 
            this.superTabControlPanel1.Controls.Add(this.collapsibleSplitContainer3);
            this.superTabControlPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.superTabControlPanel1.Location = new System.Drawing.Point(0, 28);
            this.superTabControlPanel1.Name = "superTabControlPanel1";
            this.superTabControlPanel1.Size = new System.Drawing.Size(1228, 534);
            this.superTabControlPanel1.TabIndex = 1;
            this.superTabControlPanel1.TabItem = this.图纸显示;
            // 
            // collapsibleSplitContainer3
            // 
            this.collapsibleSplitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.collapsibleSplitContainer3.Location = new System.Drawing.Point(0, 0);
            this.collapsibleSplitContainer3.Name = "collapsibleSplitContainer3";
            // 
            // collapsibleSplitContainer3.Panel1
            // 
            this.collapsibleSplitContainer3.Panel1.Controls.Add(this.panel1);
            // 
            // collapsibleSplitContainer3.Panel2
            // 
            this.collapsibleSplitContainer3.Panel2.Controls.Add(this.btLoadDxftoviewer);
            this.collapsibleSplitContainer3.Panel2.Controls.Add(this.btLoaddxfViwer);
            this.collapsibleSplitContainer3.Panel2.Controls.Add(this.groupPanel1);
            this.collapsibleSplitContainer3.Size = new System.Drawing.Size(1228, 534);
            this.collapsibleSplitContainer3.SplitterDistance = 937;
            this.collapsibleSplitContainer3.SplitterWidth = 20;
            this.collapsibleSplitContainer3.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Controls.Add(this.elementHost1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(937, 534);
            this.panel1.TabIndex = 1;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.Location = new System.Drawing.Point(0, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(937, 534);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // elementHost1
            // 
            this.elementHost1.Location = new System.Drawing.Point(0, 500);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(279, 34);
            this.elementHost1.TabIndex = 0;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.ChildChanged += new System.EventHandler<System.Windows.Forms.Integration.ChildChangedEventArgs>(this.elementHost1_ChildChanged);
            this.elementHost1.Child = null;
            // 
            // btLoadDxftoviewer
            // 
            this.btLoadDxftoviewer.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btLoadDxftoviewer.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btLoadDxftoviewer.Location = new System.Drawing.Point(3, 198);
            this.btLoadDxftoviewer.Name = "btLoadDxftoviewer";
            this.btLoadDxftoviewer.Size = new System.Drawing.Size(75, 23);
            this.btLoadDxftoviewer.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btLoadDxftoviewer.TabIndex = 8;
            this.btLoadDxftoviewer.Text = "绘画显示dxf";
            this.btLoadDxftoviewer.Click += new System.EventHandler(this.btLoadDxftoviewer_Click);
            // 
            // btLoaddxfViwer
            // 
            this.btLoaddxfViwer.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btLoaddxfViwer.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btLoaddxfViwer.Location = new System.Drawing.Point(3, 159);
            this.btLoaddxfViwer.Name = "btLoaddxfViwer";
            this.btLoaddxfViwer.Size = new System.Drawing.Size(75, 23);
            this.btLoaddxfViwer.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btLoaddxfViwer.TabIndex = 7;
            this.btLoaddxfViwer.Text = "加载dxf文件";
            this.btLoaddxfViwer.Click += new System.EventHandler(this.btLoaddxfViwer_Click);
            // 
            // groupPanel1
            // 
            this.groupPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupPanel1.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2010;
            this.groupPanel1.Controls.Add(this.sliderR);
            this.groupPanel1.Controls.Add(this.labelXB);
            this.groupPanel1.Controls.Add(this.sliderB);
            this.groupPanel1.Controls.Add(this.labelXG);
            this.groupPanel1.Controls.Add(this.sliderG);
            this.groupPanel1.Controls.Add(this.labelXR);
            this.groupPanel1.DisabledBackColor = System.Drawing.Color.Empty;
            this.groupPanel1.Location = new System.Drawing.Point(3, 3);
            this.groupPanel1.Name = "groupPanel1";
            this.groupPanel1.Size = new System.Drawing.Size(265, 132);
            // 
            // 
            // 
            this.groupPanel1.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.groupPanel1.Style.BackColorGradientAngle = 90;
            this.groupPanel1.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.groupPanel1.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderBottomWidth = 1;
            this.groupPanel1.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.groupPanel1.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderLeftWidth = 1;
            this.groupPanel1.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderRightWidth = 1;
            this.groupPanel1.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderTopWidth = 1;
            this.groupPanel1.Style.CornerDiameter = 4;
            this.groupPanel1.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.groupPanel1.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center;
            this.groupPanel1.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.groupPanel1.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.groupPanel1.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.groupPanel1.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.groupPanel1.TabIndex = 6;
            this.groupPanel1.Text = "背景颜色设置";
            this.groupPanel1.TitleImagePosition = DevComponents.DotNetBar.eTitleImagePosition.Center;
            // 
            // sliderR
            // 
            this.sliderR.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.sliderR.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.sliderR.Location = new System.Drawing.Point(3, 3);
            this.sliderR.Maximum = 255;
            this.sliderR.Name = "sliderR";
            this.sliderR.Size = new System.Drawing.Size(187, 23);
            this.sliderR.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.sliderR.TabIndex = 0;
            this.sliderR.Text = "R";
            this.sliderR.Value = 0;
            this.sliderR.ValueChanged += new System.EventHandler(this.sliderR_ValueChanged);
            // 
            // labelXB
            // 
            this.labelXB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.labelXB.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelXB.Location = new System.Drawing.Point(208, 83);
            this.labelXB.Name = "labelXB";
            this.labelXB.Size = new System.Drawing.Size(52, 23);
            this.labelXB.TabIndex = 5;
            this.labelXB.Text = "labelX3";
            // 
            // sliderB
            // 
            this.sliderB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.sliderB.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.sliderB.Location = new System.Drawing.Point(3, 83);
            this.sliderB.Maximum = 255;
            this.sliderB.Name = "sliderB";
            this.sliderB.Size = new System.Drawing.Size(187, 23);
            this.sliderB.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.sliderB.TabIndex = 1;
            this.sliderB.Text = "B";
            this.sliderB.Value = 0;
            this.sliderB.ValueChanged += new System.EventHandler(this.sliderB_ValueChanged);
            // 
            // labelXG
            // 
            this.labelXG.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.labelXG.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelXG.Location = new System.Drawing.Point(208, 42);
            this.labelXG.Name = "labelXG";
            this.labelXG.Size = new System.Drawing.Size(52, 23);
            this.labelXG.TabIndex = 4;
            this.labelXG.Text = "labelX2";
            // 
            // sliderG
            // 
            this.sliderG.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.sliderG.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.sliderG.Location = new System.Drawing.Point(3, 44);
            this.sliderG.Maximum = 255;
            this.sliderG.Name = "sliderG";
            this.sliderG.Size = new System.Drawing.Size(187, 23);
            this.sliderG.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.sliderG.TabIndex = 2;
            this.sliderG.Text = "G";
            this.sliderG.Value = 0;
            this.sliderG.ValueChanged += new System.EventHandler(this.sliderG_ValueChanged);
            // 
            // labelXR
            // 
            this.labelXR.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.labelXR.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelXR.Location = new System.Drawing.Point(208, 3);
            this.labelXR.Name = "labelXR";
            this.labelXR.Size = new System.Drawing.Size(52, 23);
            this.labelXR.TabIndex = 3;
            this.labelXR.Text = "labelX1";
            // 
            // 图纸显示
            // 
            this.图纸显示.AttachedControl = this.superTabControlPanel1;
            this.图纸显示.GlobalItem = false;
            this.图纸显示.Name = "图纸显示";
            this.图纸显示.Text = "图纸查看";
            // 
            // superTabControlPanel3
            // 
            this.superTabControlPanel3.Controls.Add(this.collapsibleSplitContainer1);
            this.superTabControlPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.superTabControlPanel3.Location = new System.Drawing.Point(0, 28);
            this.superTabControlPanel3.Name = "superTabControlPanel3";
            this.superTabControlPanel3.Size = new System.Drawing.Size(1228, 534);
            this.superTabControlPanel3.TabIndex = 0;
            this.superTabControlPanel3.TabItem = this.superTabItem1;
            // 
            // collapsibleSplitContainer1
            // 
            this.collapsibleSplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.collapsibleSplitContainer1.Location = new System.Drawing.Point(0, 0);
            this.collapsibleSplitContainer1.Name = "collapsibleSplitContainer1";
            // 
            // collapsibleSplitContainer1.Panel1
            // 
            this.collapsibleSplitContainer1.Panel1.Controls.Add(this.collapsibleSplitContainer2);
            // 
            // collapsibleSplitContainer1.Panel2
            // 
            this.collapsibleSplitContainer1.Panel2.Controls.Add(this.btInformationdxf);
            this.collapsibleSplitContainer1.Panel2.Controls.Add(this.btCreateDxfToTable);
            this.collapsibleSplitContainer1.Panel2.Controls.Add(this.btCreateDxfToGcode);
            this.collapsibleSplitContainer1.Panel2.Controls.Add(this.btCreatNewDxf);
            this.collapsibleSplitContainer1.Panel2.Controls.Add(this.btDrawgcode);
            this.collapsibleSplitContainer1.Panel2.Controls.Add(this.btMakeGcode);
            this.collapsibleSplitContainer1.Panel2.Controls.Add(this.BtopenDXF);
            this.collapsibleSplitContainer1.Size = new System.Drawing.Size(1228, 534);
            this.collapsibleSplitContainer1.SplitterDistance = 990;
            this.collapsibleSplitContainer1.SplitterWidth = 20;
            this.collapsibleSplitContainer1.TabIndex = 0;
            // 
            // collapsibleSplitContainer2
            // 
            this.collapsibleSplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.collapsibleSplitContainer2.Location = new System.Drawing.Point(0, 0);
            this.collapsibleSplitContainer2.Name = "collapsibleSplitContainer2";
            // 
            // collapsibleSplitContainer2.Panel1
            // 
            this.collapsibleSplitContainer2.Panel1.Controls.Add(this.fCTBCode);
            // 
            // collapsibleSplitContainer2.Panel2
            // 
            this.collapsibleSplitContainer2.Panel2.Controls.Add(this.pictureBox1);
            this.collapsibleSplitContainer2.Size = new System.Drawing.Size(990, 534);
            this.collapsibleSplitContainer2.SplitterDistance = 456;
            this.collapsibleSplitContainer2.SplitterWidth = 20;
            this.collapsibleSplitContainer2.TabIndex = 0;
            // 
            // fCTBCode
            // 
            this.fCTBCode.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.fCTBCode.AutoIndent = false;
            this.fCTBCode.AutoIndentCharsPatterns = "\r\n^\\s*[\\w\\.]+\\s*(?<range>=)\\s*(?<range>[^;]+);";
            this.fCTBCode.AutoScrollMinSize = new System.Drawing.Size(207, 12);
            this.fCTBCode.BackBrush = null;
            this.fCTBCode.CharHeight = 12;
            this.fCTBCode.CharWidth = 7;
            this.fCTBCode.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fCTBCode.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.fCTBCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fCTBCode.Font = new System.Drawing.Font("Courier New", 8F);
            this.fCTBCode.IsReplaceMode = false;
            this.fCTBCode.Location = new System.Drawing.Point(0, 0);
            this.fCTBCode.Name = "fCTBCode";
            this.fCTBCode.Paddings = new System.Windows.Forms.Padding(0);
            this.fCTBCode.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.fCTBCode.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("fCTBCode.ServiceColors")));
            this.fCTBCode.Size = new System.Drawing.Size(456, 534);
            this.fCTBCode.TabIndex = 25;
            this.fCTBCode.Text = "(Paste GCode or load file)";
            this.fCTBCode.ToolTip = null;
            this.fCTBCode.Zoom = 100;
            this.fCTBCode.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.fCTBCode_TextChanged);
            this.fCTBCode.TextChangedDelayed += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.fCTBCode_TextChangedDelayed);
            this.fCTBCode.Click += new System.EventHandler(this.fCTBCode_Click);
            this.fCTBCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.fCTBCode_KeyDown);
            this.fCTBCode.MouseHover += new System.EventHandler(this.fCTBCode_MouseHover);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Cross;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(514, 534);
            this.pictureBox1.TabIndex = 19;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.SizeChanged += new System.EventHandler(this.pictureBox1_SizeChanged);
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseDown);
            this.pictureBox1.MouseHover += new System.EventHandler(this.pictureBox1_MouseHover);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseUp);
            this.pictureBox1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseWheel);
            // 
            // btInformationdxf
            // 
            this.btInformationdxf.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btInformationdxf.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btInformationdxf.Location = new System.Drawing.Point(18, 182);
            this.btInformationdxf.Name = "btInformationdxf";
            this.btInformationdxf.Size = new System.Drawing.Size(82, 30);
            this.btInformationdxf.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btInformationdxf.TabIndex = 4;
            this.btInformationdxf.Text = "DXF信息显示";
            this.btInformationdxf.Click += new System.EventHandler(this.btInformationdxf_Click);
            // 
            // btCreateDxfToTable
            // 
            this.btCreateDxfToTable.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btCreateDxfToTable.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btCreateDxfToTable.Location = new System.Drawing.Point(18, 254);
            this.btCreateDxfToTable.Name = "btCreateDxfToTable";
            this.btCreateDxfToTable.Size = new System.Drawing.Size(82, 30);
            this.btCreateDxfToTable.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btCreateDxfToTable.TabIndex = 3;
            this.btCreateDxfToTable.Text = "图表dxf";
            this.btCreateDxfToTable.Click += new System.EventHandler(this.btCreateDxfToTable_Click);
            // 
            // btCreateDxfToGcode
            // 
            this.btCreateDxfToGcode.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btCreateDxfToGcode.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btCreateDxfToGcode.Location = new System.Drawing.Point(18, 218);
            this.btCreateDxfToGcode.Name = "btCreateDxfToGcode";
            this.btCreateDxfToGcode.Size = new System.Drawing.Size(82, 30);
            this.btCreateDxfToGcode.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btCreateDxfToGcode.TabIndex = 2;
            this.btCreateDxfToGcode.Text = "G代码dxf";
            this.btCreateDxfToGcode.Click += new System.EventHandler(this.btCreateDxfToGcode_Click);
            // 
            // btCreatNewDxf
            // 
            this.btCreatNewDxf.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btCreatNewDxf.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btCreatNewDxf.Location = new System.Drawing.Point(18, 11);
            this.btCreatNewDxf.Name = "btCreatNewDxf";
            this.btCreatNewDxf.Size = new System.Drawing.Size(82, 33);
            this.btCreatNewDxf.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btCreatNewDxf.TabIndex = 1;
            this.btCreatNewDxf.Text = "DXF文件提取";
            this.btCreatNewDxf.Click += new System.EventHandler(this.btCreatNewDxf_Click);
            // 
            // btDrawgcode
            // 
            this.btDrawgcode.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btDrawgcode.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btDrawgcode.Location = new System.Drawing.Point(18, 145);
            this.btDrawgcode.Name = "btDrawgcode";
            this.btDrawgcode.Size = new System.Drawing.Size(82, 33);
            this.btDrawgcode.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btDrawgcode.TabIndex = 0;
            this.btDrawgcode.Text = "提取数据";
            // 
            // btMakeGcode
            // 
            this.btMakeGcode.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btMakeGcode.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btMakeGcode.Location = new System.Drawing.Point(18, 106);
            this.btMakeGcode.Name = "btMakeGcode";
            this.btMakeGcode.Size = new System.Drawing.Size(82, 33);
            this.btMakeGcode.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btMakeGcode.TabIndex = 0;
            this.btMakeGcode.Text = "生成G代码";
            this.btMakeGcode.Click += new System.EventHandler(this.btMakeGcode_Click);
            // 
            // BtopenDXF
            // 
            this.BtopenDXF.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.BtopenDXF.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.BtopenDXF.Location = new System.Drawing.Point(18, 54);
            this.BtopenDXF.Name = "BtopenDXF";
            this.BtopenDXF.Size = new System.Drawing.Size(82, 33);
            this.BtopenDXF.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.BtopenDXF.TabIndex = 0;
            this.BtopenDXF.Text = "打开DXF文件";
            this.BtopenDXF.Click += new System.EventHandler(this.BtopenDXF_Click);
            // 
            // superTabItem1
            // 
            this.superTabItem1.AttachedControl = this.superTabControlPanel3;
            this.superTabItem1.GlobalItem = false;
            this.superTabItem1.Name = "superTabItem1";
            this.superTabItem1.Text = "G代码生成单元";
            // 
            // superTabControlPanel2
            // 
            this.superTabControlPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.superTabControlPanel2.Location = new System.Drawing.Point(0, 28);
            this.superTabControlPanel2.Name = "superTabControlPanel2";
            this.superTabControlPanel2.Size = new System.Drawing.Size(1228, 534);
            this.superTabControlPanel2.TabIndex = 0;
            this.superTabControlPanel2.TabItem = this.superTabItem2;
            // 
            // superTabItem2
            // 
            this.superTabItem2.AttachedControl = this.superTabControlPanel2;
            this.superTabItem2.GlobalItem = false;
            this.superTabItem2.Name = "superTabItem2";
            this.superTabItem2.Text = "运动控制单元";
            // 
            // btSettings
            // 
            this.btSettings.Name = "btSettings";
            this.btSettings.Text = "参数设置";
            this.btSettings.Click += new System.EventHandler(this.btSettings_Click);
            // 
            // panelEx1
            // 
            this.panelEx1.CanvasColor = System.Drawing.SystemColors.Control;
            this.panelEx1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.panelEx1.Controls.Add(this.panellog);
            this.panelEx1.Controls.Add(this.superTabControl1);
            this.panelEx1.DisabledBackColor = System.Drawing.Color.Empty;
            this.panelEx1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelEx1.Location = new System.Drawing.Point(0, 0);
            this.panelEx1.Name = "panelEx1";
            this.panelEx1.Size = new System.Drawing.Size(1231, 729);
            this.panelEx1.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.panelEx1.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.panelEx1.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.panelEx1.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.panelEx1.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.panelEx1.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.panelEx1.Style.GradientAngle = 90;
            this.panelEx1.TabIndex = 1;
            this.panelEx1.Text = "panelEx1";
            // 
            // panellog
            // 
            this.panellog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panellog.CanvasColor = System.Drawing.SystemColors.Control;
            this.panellog.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.panellog.DisabledBackColor = System.Drawing.Color.Empty;
            this.panellog.Location = new System.Drawing.Point(0, 571);
            this.panellog.Name = "panellog";
            this.panellog.Size = new System.Drawing.Size(1231, 155);
            this.panellog.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.panellog.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.panellog.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.panellog.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.panellog.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.panellog.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.panellog.Style.GradientAngle = 90;
            this.panellog.TabIndex = 1;
            this.panellog.Text = "panelEx2";
            // 
            // Mainfrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1231, 729);
            this.Controls.Add(this.panelEx1);
            this.Name = "Mainfrm";
            this.Text = "REMAC_堆模机械手系统";
            this.Load += new System.EventHandler(this.Mainfrm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.superTabControl1)).EndInit();
            this.superTabControl1.ResumeLayout(false);
            this.superTabControlPanel1.ResumeLayout(false);
            this.collapsibleSplitContainer3.Panel1.ResumeLayout(false);
            this.collapsibleSplitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.collapsibleSplitContainer3)).EndInit();
            this.collapsibleSplitContainer3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.groupPanel1.ResumeLayout(false);
            this.superTabControlPanel3.ResumeLayout(false);
            this.collapsibleSplitContainer1.Panel1.ResumeLayout(false);
            this.collapsibleSplitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.collapsibleSplitContainer1)).EndInit();
            this.collapsibleSplitContainer1.ResumeLayout(false);
            this.collapsibleSplitContainer2.Panel1.ResumeLayout(false);
            this.collapsibleSplitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.collapsibleSplitContainer2)).EndInit();
            this.collapsibleSplitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fCTBCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panelEx1.ResumeLayout(false);
            this.ResumeLayout(false);

        }



        #endregion

        private DevComponents.DotNetBar.SuperTabControl superTabControl1;
        private DevComponents.DotNetBar.SuperTabControlPanel superTabControlPanel2;
        private DevComponents.DotNetBar.SuperTabItem superTabItem2;
        private DevComponents.DotNetBar.SuperTabControlPanel superTabControlPanel1;
        private DevComponents.DotNetBar.SuperTabItem 图纸显示;
        private DevComponents.DotNetBar.PanelEx panelEx1;
        private DevComponents.DotNetBar.PanelEx panellog;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private DevComponents.DotNetBar.Controls.Slider sliderR;
        private DevComponents.DotNetBar.Controls.Slider sliderG;
        private DevComponents.DotNetBar.Controls.Slider sliderB;
        private DevComponents.DotNetBar.LabelX labelXB;
        private DevComponents.DotNetBar.LabelX labelXG;
        private DevComponents.DotNetBar.LabelX labelXR;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel1;
        private DevComponents.DotNetBar.SuperTabControlPanel superTabControlPanel3;
        private DevComponents.DotNetBar.SuperTabItem superTabItem1;
        private DevComponents.DotNetBar.Controls.CollapsibleSplitContainer collapsibleSplitContainer1;
        private DevComponents.DotNetBar.Controls.CollapsibleSplitContainer collapsibleSplitContainer2;
        private DevComponents.DotNetBar.Controls.CollapsibleSplitContainer collapsibleSplitContainer3;
        private DevComponents.DotNetBar.ButtonX btDrawgcode;
        private DevComponents.DotNetBar.ButtonX btMakeGcode;
        private DevComponents.DotNetBar.ButtonX BtopenDXF;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private FastColoredTextBoxNS.FastColoredTextBox fCTBCode;
        private DevComponents.DotNetBar.ButtonItem btSettings;
        private DevComponents.DotNetBar.ButtonX btCreatNewDxf;
        private DevComponents.DotNetBar.ButtonX btLoaddxfViwer;
        private System.Windows.Forms.PictureBox pictureBox2;
        private DevComponents.DotNetBar.ButtonX btLoadDxftoviewer;
        private DevComponents.DotNetBar.ButtonX btCreateDxfToTable;
        private DevComponents.DotNetBar.ButtonX btCreateDxfToGcode;
        private DevComponents.DotNetBar.ButtonX btInformationdxf;
    }
}

