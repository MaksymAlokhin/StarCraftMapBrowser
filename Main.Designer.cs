namespace StarCraftMapBrowser
{
    public partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        public System.ComponentModel.IContainer components = null;

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
        public void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.selectFolderBtn = new System.Windows.Forms.Button();
            this.infoBox = new System.Windows.Forms.TextBox();
            this.renameAllBtn = new System.Windows.Forms.Button();
            this.MapImage = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ThumbMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.DupeMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.bgWorker1 = new System.ComponentModel.BackgroundWorker();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.progressLBL = new System.Windows.Forms.Label();
            this.grid = new System.Windows.Forms.DataGridView();
            this.DescrTextBox = new System.Windows.Forms.TextBox();
            this.NameTextBox = new System.Windows.Forms.TextBox();
            this.FilenameFilter = new System.Windows.Forms.TextBox();
            this.MapnameFilter = new System.Windows.Forms.TextBox();
            this.FilenameTextBox = new System.Windows.Forms.TextBox();
            this.ClearMissingBtn = new System.Windows.Forms.Button();
            this.ClearAllBtn = new System.Windows.Forms.Button();
            this.bgWorkerBigJPEGs = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.MapImage)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.SuspendLayout();
            // 
            // selectFolderBtn
            // 
            this.selectFolderBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.selectFolderBtn.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectFolderBtn.Location = new System.Drawing.Point(1001, 585);
            this.selectFolderBtn.Name = "selectFolderBtn";
            this.selectFolderBtn.Size = new System.Drawing.Size(129, 36);
            this.selectFolderBtn.TabIndex = 4;
            this.selectFolderBtn.Text = "Select Folder";
            this.selectFolderBtn.UseVisualStyleBackColor = true;
            this.selectFolderBtn.Click += new System.EventHandler(this.selectFolderBtn_Click);
            // 
            // infoBox
            // 
            this.infoBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.infoBox.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.infoBox.Location = new System.Drawing.Point(12, 585);
            this.infoBox.Multiline = true;
            this.infoBox.Name = "infoBox";
            this.infoBox.ReadOnly = true;
            this.infoBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.infoBox.Size = new System.Drawing.Size(841, 79);
            this.infoBox.TabIndex = 8;
            // 
            // renameAllBtn
            // 
            this.renameAllBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.renameAllBtn.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.renameAllBtn.Location = new System.Drawing.Point(869, 585);
            this.renameAllBtn.Name = "renameAllBtn";
            this.renameAllBtn.Size = new System.Drawing.Size(129, 36);
            this.renameAllBtn.TabIndex = 3;
            this.renameAllBtn.Text = "Rename All";
            this.renameAllBtn.UseVisualStyleBackColor = true;
            this.renameAllBtn.Click += new System.EventHandler(this.renameAllBtn_Click);
            // 
            // MapImage
            // 
            this.MapImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MapImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.MapImage.Location = new System.Drawing.Point(869, 63);
            this.MapImage.Name = "MapImage";
            this.MapImage.Size = new System.Drawing.Size(256, 256);
            this.MapImage.TabIndex = 6;
            this.MapImage.TabStop = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1142, 26);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuToolStripMenuItem
            // 
            this.menuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.menuToolStripMenuItem.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            this.menuToolStripMenuItem.Size = new System.Drawing.Size(58, 22);
            this.menuToolStripMenuItem.Text = "Menu";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ThumbMenu,
            this.DupeMenu});
            this.optionsToolStripMenuItem.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(74, 22);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // ThumbMenu
            // 
            this.ThumbMenu.Checked = true;
            this.ThumbMenu.CheckOnClick = true;
            this.ThumbMenu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ThumbMenu.Name = "ThumbMenu";
            this.ThumbMenu.Size = new System.Drawing.Size(309, 22);
            this.ThumbMenu.Text = "Generate Thumbnails";
            // 
            // DupeMenu
            // 
            this.DupeMenu.CheckOnClick = true;
            this.DupeMenu.Name = "DupeMenu";
            this.DupeMenu.Size = new System.Drawing.Size(309, 22);
            this.DupeMenu.Text = "\'Rename All\' Removes Duplicates";
            // 
            // bgWorker1
            // 
            this.bgWorker1.WorkerReportsProgress = true;
            this.bgWorker1.WorkerSupportsCancellation = true;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(12, 553);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(814, 22);
            this.progressBar.TabIndex = 7;
            // 
            // progressLBL
            // 
            this.progressLBL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressLBL.AutoSize = true;
            this.progressLBL.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.progressLBL.Location = new System.Drawing.Point(832, 555);
            this.progressLBL.Name = "progressLBL";
            this.progressLBL.Size = new System.Drawing.Size(22, 18);
            this.progressLBL.TabIndex = 6;
            this.progressLBL.Text = "%";
            // 
            // grid
            // 
            this.grid.AllowUserToAddRows = false;
            this.grid.AllowUserToDeleteRows = false;
            this.grid.AllowUserToOrderColumns = true;
            this.grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.grid.DefaultCellStyle = dataGridViewCellStyle2;
            this.grid.Location = new System.Drawing.Point(12, 70);
            this.grid.Name = "grid";
            this.grid.ReadOnly = true;
            this.grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grid.Size = new System.Drawing.Size(840, 476);
            this.grid.TabIndex = 0;
            this.grid.CurrentCellChanged += new System.EventHandler(this.grid_CurrentCellChanged);
            // 
            // DescrTextBox
            // 
            this.DescrTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DescrTextBox.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DescrTextBox.Location = new System.Drawing.Point(864, 325);
            this.DescrTextBox.Multiline = true;
            this.DescrTextBox.Name = "DescrTextBox";
            this.DescrTextBox.ReadOnly = true;
            this.DescrTextBox.Size = new System.Drawing.Size(266, 221);
            this.DescrTextBox.TabIndex = 11;
            // 
            // NameTextBox
            // 
            this.NameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.NameTextBox.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.NameTextBox.Location = new System.Drawing.Point(864, 38);
            this.NameTextBox.Name = "NameTextBox";
            this.NameTextBox.ReadOnly = true;
            this.NameTextBox.Size = new System.Drawing.Size(266, 19);
            this.NameTextBox.TabIndex = 10;
            this.NameTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // FilenameFilter
            // 
            this.FilenameFilter.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FilenameFilter.Location = new System.Drawing.Point(12, 38);
            this.FilenameFilter.Name = "FilenameFilter";
            this.FilenameFilter.Size = new System.Drawing.Size(148, 26);
            this.FilenameFilter.TabIndex = 1;
            this.FilenameFilter.TextChanged += new System.EventHandler(this.FilenameTextBox_TextChanged);
            // 
            // MapnameFilter
            // 
            this.MapnameFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MapnameFilter.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MapnameFilter.Location = new System.Drawing.Point(705, 38);
            this.MapnameFilter.Name = "MapnameFilter";
            this.MapnameFilter.Size = new System.Drawing.Size(148, 26);
            this.MapnameFilter.TabIndex = 2;
            this.MapnameFilter.TextChanged += new System.EventHandler(this.MapnameTextBox_TextChanged);
            // 
            // FilenameTextBox
            // 
            this.FilenameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilenameTextBox.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FilenameTextBox.Location = new System.Drawing.Point(12, 553);
            this.FilenameTextBox.Name = "FilenameTextBox";
            this.FilenameTextBox.ReadOnly = true;
            this.FilenameTextBox.Size = new System.Drawing.Size(840, 26);
            this.FilenameTextBox.TabIndex = 5;
            // 
            // ClearMissingBtn
            // 
            this.ClearMissingBtn.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ClearMissingBtn.Location = new System.Drawing.Point(266, 31);
            this.ClearMissingBtn.Name = "ClearMissingBtn";
            this.ClearMissingBtn.Size = new System.Drawing.Size(156, 32);
            this.ClearMissingBtn.TabIndex = 13;
            this.ClearMissingBtn.Text = "Clear Missing";
            this.ClearMissingBtn.UseVisualStyleBackColor = true;
            this.ClearMissingBtn.Click += new System.EventHandler(this.ClearMissingBtn_Click);
            // 
            // ClearAllBtn
            // 
            this.ClearAllBtn.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ClearAllBtn.Location = new System.Drawing.Point(443, 31);
            this.ClearAllBtn.Name = "ClearAllBtn";
            this.ClearAllBtn.Size = new System.Drawing.Size(156, 32);
            this.ClearAllBtn.TabIndex = 14;
            this.ClearAllBtn.Text = "Clear All";
            this.ClearAllBtn.UseVisualStyleBackColor = true;
            this.ClearAllBtn.Click += new System.EventHandler(this.ClearAllBtn_Click);
            // 
            // bgWorkerBigJPEGs
            // 
            this.bgWorkerBigJPEGs.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorkerBigJPEGs_DoWork);
            this.bgWorkerBigJPEGs.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorkerBigJPEGs_RunWorkerCompleted);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1142, 671);
            this.Controls.Add(this.ClearAllBtn);
            this.Controls.Add(this.ClearMissingBtn);
            this.Controls.Add(this.FilenameTextBox);
            this.Controls.Add(this.MapnameFilter);
            this.Controls.Add(this.FilenameFilter);
            this.Controls.Add(this.NameTextBox);
            this.Controls.Add(this.DescrTextBox);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.progressLBL);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.MapImage);
            this.Controls.Add(this.renameAllBtn);
            this.Controls.Add(this.infoBox);
            this.Controls.Add(this.selectFolderBtn);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "StarCraft Map Browser";
            ((System.ComponentModel.ISupportInitialize)(this.MapImage)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button selectFolderBtn;
        public System.Windows.Forms.TextBox infoBox;
        public System.Windows.Forms.Button renameAllBtn;
        public System.Windows.Forms.PictureBox MapImage;
        public System.Windows.Forms.MenuStrip menuStrip1;
        public System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        public System.ComponentModel.BackgroundWorker bgWorker1;
        public System.Windows.Forms.ProgressBar progressBar;
        public System.Windows.Forms.Label progressLBL;
        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.TextBox DescrTextBox;
        private System.Windows.Forms.TextBox NameTextBox;
        private System.Windows.Forms.TextBox FilenameFilter;
        private System.Windows.Forms.TextBox MapnameFilter;
        private System.Windows.Forms.TextBox FilenameTextBox;
        private System.Windows.Forms.Button ClearMissingBtn;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ThumbMenu;
        private System.Windows.Forms.ToolStripMenuItem DupeMenu;
        private System.Windows.Forms.Button ClearAllBtn;
        private System.ComponentModel.BackgroundWorker bgWorkerBigJPEGs;
    }
}

