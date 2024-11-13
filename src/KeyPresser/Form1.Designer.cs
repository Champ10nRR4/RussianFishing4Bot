namespace KeyPresser
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnStart = new Button();
            btnStop = new Button();
            listBox1 = new ListBox();
            tbSkipMax = new TextBox();
            tbSkipMin = new TextBox();
            label1 = new Label();
            tbHoldMax = new TextBox();
            tbHoldMin = new TextBox();
            label2 = new Label();
            gbActionType = new GroupBox();
            rbPull = new RadioButton();
            rbSpin = new RadioButton();
            tbCoordX = new TextBox();
            tbCoordY = new TextBox();
            pnlColourIndicator = new Panel();
            label3 = new Label();
            label4 = new Label();
            cbSeaFishing = new CheckBox();
            cbFloat = new CheckBox();
            cbAuto = new CheckBox();
            tbActionDelay = new TextBox();
            label5 = new Label();
            menuStrip1 = new MenuStrip();
            menuToolStripMenuItem = new ToolStripMenuItem();
            actionsToolStripMenuItem = new ToolStripMenuItem();
            initializeCoordinatesToolStripMenuItem = new ToolStripMenuItem();
            pickCoordinatesToolStripMenuItem = new ToolStripMenuItem();
            configurationToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            label6 = new Label();
            cbBrewing = new CheckBox();
            cbDig = new CheckBox();
            lblStatus = new Label();
            btnTest = new Button();
            cbSpinning = new CheckBox();
            cbJigging = new CheckBox();
            gbActionType.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Location = new Point(423, 447);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(85, 34);
            btnStart.TabIndex = 0;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Location = new Point(514, 447);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(88, 34);
            btnStop.TabIndex = 0;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 25;
            listBox1.Location = new Point(12, 37);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(590, 404);
            listBox1.TabIndex = 1;
            // 
            // tbSkipMax
            // 
            tbSkipMax.Location = new Point(13, 519);
            tbSkipMax.Name = "tbSkipMax";
            tbSkipMax.Size = new Size(92, 31);
            tbSkipMax.TabIndex = 2;
            // 
            // tbSkipMin
            // 
            tbSkipMin.Location = new Point(12, 480);
            tbSkipMin.Name = "tbSkipMin";
            tbSkipMin.Size = new Size(92, 31);
            tbSkipMin.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 451);
            label1.Name = "label1";
            label1.Size = new Size(91, 25);
            label1.TabIndex = 5;
            label1.Text = "Pause(ms)";
            // 
            // tbHoldMax
            // 
            tbHoldMax.Location = new Point(110, 519);
            tbHoldMax.Name = "tbHoldMax";
            tbHoldMax.Size = new Size(72, 31);
            tbHoldMax.TabIndex = 4;
            // 
            // tbHoldMin
            // 
            tbHoldMin.Location = new Point(110, 481);
            tbHoldMin.Name = "tbHoldMin";
            tbHoldMin.Size = new Size(72, 31);
            tbHoldMin.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(110, 451);
            label2.Name = "label2";
            label2.Size = new Size(72, 25);
            label2.TabIndex = 5;
            label2.Text = "Act(ms)";
            // 
            // gbActionType
            // 
            gbActionType.Controls.Add(rbPull);
            gbActionType.Controls.Add(rbSpin);
            gbActionType.Location = new Point(339, 452);
            gbActionType.Name = "gbActionType";
            gbActionType.Size = new Size(78, 99);
            gbActionType.TabIndex = 6;
            gbActionType.TabStop = false;
            gbActionType.Text = "Action Type";
            // 
            // rbPull
            // 
            rbPull.AutoSize = true;
            rbPull.Location = new Point(6, 65);
            rbPull.Name = "rbPull";
            rbPull.Size = new Size(65, 29);
            rbPull.TabIndex = 1;
            rbPull.TabStop = true;
            rbPull.Text = "Pull";
            rbPull.UseVisualStyleBackColor = true;
            // 
            // rbSpin
            // 
            rbSpin.AutoSize = true;
            rbSpin.Location = new Point(6, 31);
            rbSpin.Name = "rbSpin";
            rbSpin.Size = new Size(72, 29);
            rbSpin.TabIndex = 0;
            rbSpin.TabStop = true;
            rbSpin.Text = "Spin";
            rbSpin.UseVisualStyleBackColor = true;
            // 
            // tbCoordX
            // 
            tbCoordX.Location = new Point(216, 479);
            tbCoordX.Name = "tbCoordX";
            tbCoordX.Size = new Size(56, 31);
            tbCoordX.TabIndex = 10;
            // 
            // tbCoordY
            // 
            tbCoordY.Location = new Point(216, 518);
            tbCoordY.Name = "tbCoordY";
            tbCoordY.Size = new Size(56, 31);
            tbCoordY.TabIndex = 10;
            // 
            // pnlColourIndicator
            // 
            pnlColourIndicator.BorderStyle = BorderStyle.FixedSingle;
            pnlColourIndicator.Location = new Point(423, 489);
            pnlColourIndicator.Name = "pnlColourIndicator";
            pnlColourIndicator.Size = new Size(175, 33);
            pnlColourIndicator.TabIndex = 11;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(192, 482);
            label3.Name = "label3";
            label3.Size = new Size(27, 25);
            label3.TabIndex = 12;
            label3.Text = "X:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(192, 521);
            label4.Name = "label4";
            label4.Size = new Size(26, 25);
            label4.TabIndex = 13;
            label4.Text = "Y:";
            // 
            // cbSeaFishing
            // 
            cbSeaFishing.AutoSize = true;
            cbSeaFishing.Location = new Point(13, 556);
            cbSeaFishing.Name = "cbSeaFishing";
            cbSeaFishing.Size = new Size(127, 29);
            cbSeaFishing.TabIndex = 15;
            cbSeaFishing.Text = "Sea Fishing";
            cbSeaFishing.UseVisualStyleBackColor = true;
            cbSeaFishing.CheckedChanged += cbSeaFishing_CheckedChanged;
            // 
            // cbFloat
            // 
            cbFloat.AutoSize = true;
            cbFloat.Location = new Point(141, 556);
            cbFloat.Name = "cbFloat";
            cbFloat.Size = new Size(77, 29);
            cbFloat.TabIndex = 17;
            cbFloat.Text = "Float";
            cbFloat.UseVisualStyleBackColor = true;
            cbFloat.CheckedChanged += cbFloat_CheckedChanged;
            // 
            // cbAuto
            // 
            cbAuto.AutoSize = true;
            cbAuto.Location = new Point(439, 556);
            cbAuto.Name = "cbAuto";
            cbAuto.Size = new Size(77, 29);
            cbAuto.TabIndex = 18;
            cbAuto.Text = "Auto";
            cbAuto.UseVisualStyleBackColor = true;
            cbAuto.CheckedChanged += cbAuto_CheckedChanged;
            // 
            // tbActionDelay
            // 
            tbActionDelay.Location = new Point(279, 519);
            tbActionDelay.Name = "tbActionDelay";
            tbActionDelay.Size = new Size(55, 31);
            tbActionDelay.TabIndex = 20;
            // 
            // label5
            // 
            label5.Location = new Point(279, 452);
            label5.Name = "label5";
            label5.Size = new Size(55, 52);
            label5.TabIndex = 21;
            label5.Text = "Delay (s)";
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(24, 24);
            menuStrip1.Items.AddRange(new ToolStripItem[] { menuToolStripMenuItem, exitToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(610, 33);
            menuStrip1.TabIndex = 22;
            menuStrip1.Text = "menuStrip1";
            // 
            // menuToolStripMenuItem
            // 
            menuToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { actionsToolStripMenuItem, configurationToolStripMenuItem });
            menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            menuToolStripMenuItem.Size = new Size(73, 29);
            menuToolStripMenuItem.Text = "Menu";
            // 
            // actionsToolStripMenuItem
            // 
            actionsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { initializeCoordinatesToolStripMenuItem, pickCoordinatesToolStripMenuItem });
            actionsToolStripMenuItem.Name = "actionsToolStripMenuItem";
            actionsToolStripMenuItem.Size = new Size(223, 34);
            actionsToolStripMenuItem.Text = "Actions";
            // 
            // initializeCoordinatesToolStripMenuItem
            // 
            initializeCoordinatesToolStripMenuItem.Name = "initializeCoordinatesToolStripMenuItem";
            initializeCoordinatesToolStripMenuItem.Size = new Size(278, 34);
            initializeCoordinatesToolStripMenuItem.Text = "Initialize Coordinates";
            initializeCoordinatesToolStripMenuItem.Click += initializeCoordinatesToolStripMenuItem_Click;
            // 
            // pickCoordinatesToolStripMenuItem
            // 
            pickCoordinatesToolStripMenuItem.Name = "pickCoordinatesToolStripMenuItem";
            pickCoordinatesToolStripMenuItem.Size = new Size(278, 34);
            pickCoordinatesToolStripMenuItem.Text = "Pick Coordinates";
            pickCoordinatesToolStripMenuItem.Click += pickCoordinatesToolStripMenuItem_Click;
            // 
            // configurationToolStripMenuItem
            // 
            configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
            configurationToolStripMenuItem.Size = new Size(223, 34);
            configurationToolStripMenuItem.Text = "Configuration";
            configurationToolStripMenuItem.Click += configurationToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(55, 29);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(210, 451);
            label6.Name = "label6";
            label6.Size = new Size(62, 25);
            label6.TabIndex = 23;
            label6.Text = "Coord";
            // 
            // cbBrewing
            // 
            cbBrewing.AutoSize = true;
            cbBrewing.Location = new Point(13, 591);
            cbBrewing.Name = "cbBrewing";
            cbBrewing.Size = new Size(101, 29);
            cbBrewing.TabIndex = 24;
            cbBrewing.Text = "Brewing";
            cbBrewing.UseVisualStyleBackColor = true;
            cbBrewing.CheckedChanged += cbBrewing_CheckedChanged;
            // 
            // cbDig
            // 
            cbDig.AutoSize = true;
            cbDig.Location = new Point(120, 591);
            cbDig.Name = "cbDig";
            cbDig.Size = new Size(66, 29);
            cbDig.TabIndex = 25;
            cbDig.Text = "Dig";
            cbDig.UseVisualStyleBackColor = true;
            // 
            // lblStatus
            // 
            lblStatus.Location = new Point(423, 525);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(175, 25);
            lblStatus.TabIndex = 26;
            lblStatus.Text = "Status";
            // 
            // btnTest
            // 
            btnTest.Location = new Point(548, 574);
            btnTest.Name = "btnTest";
            btnTest.Size = new Size(50, 34);
            btnTest.TabIndex = 27;
            btnTest.Text = "Test";
            btnTest.UseVisualStyleBackColor = true;
            btnTest.Click += btnTest_Click;
            // 
            // cbSpinning
            // 
            cbSpinning.AutoSize = true;
            cbSpinning.Location = new Point(224, 556);
            cbSpinning.Name = "cbSpinning";
            cbSpinning.Size = new Size(108, 29);
            cbSpinning.TabIndex = 28;
            cbSpinning.Text = "Spinning";
            cbSpinning.UseVisualStyleBackColor = true;
            // 
            // cbJigging
            // 
            cbJigging.AutoSize = true;
            cbJigging.Location = new Point(338, 556);
            cbJigging.Name = "cbJigging";
            cbJigging.Size = new Size(95, 29);
            cbJigging.TabIndex = 29;
            cbJigging.Text = "Jigging";
            cbJigging.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(610, 620);
            Controls.Add(cbJigging);
            Controls.Add(cbSpinning);
            Controls.Add(btnTest);
            Controls.Add(lblStatus);
            Controls.Add(cbDig);
            Controls.Add(cbBrewing);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(tbActionDelay);
            Controls.Add(cbAuto);
            Controls.Add(cbFloat);
            Controls.Add(cbSeaFishing);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(pnlColourIndicator);
            Controls.Add(tbCoordY);
            Controls.Add(tbCoordX);
            Controls.Add(gbActionType);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(tbHoldMin);
            Controls.Add(tbHoldMax);
            Controls.Add(tbSkipMin);
            Controls.Add(tbSkipMax);
            Controls.Add(listBox1);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            Controls.Add(menuStrip1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Kids love play puzzles";
            Load += Form1_Load;
            gbActionType.ResumeLayout(false);
            gbActionType.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnStart;
        private Button btnStop;
        private ListBox listBox1;
        private TextBox tbSkipMax;
        private TextBox tbSkipMin;
        private Label label1;
        private TextBox tbHoldMax;
        private TextBox tbHoldMin;
        private Label label2;
        private GroupBox gbActionType;
        private RadioButton rbPull;
        private RadioButton rbSpin;
        private TextBox tbCoordX;
        private TextBox tbCoordY;
        private Panel pnlColourIndicator;
        private Label label3;
        private Label label4;
        private CheckBox cbSeaFishing;
        private CheckBox cbFloat;
        private CheckBox cbAuto;
        private TextBox tbActionDelay;
        private Label label5;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem menuToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem actionsToolStripMenuItem;
        private ToolStripMenuItem initializeCoordinatesToolStripMenuItem;
        private ToolStripMenuItem pickCoordinatesToolStripMenuItem;
        private ToolStripMenuItem configurationToolStripMenuItem;
        private Label label6;
        private CheckBox cbBrewing;
        private CheckBox cbDig;
        private Label lblStatus;
        private Button btnTest;
        private CheckBox cbSpinning;
        private CheckBox cbJigging;
    }
}