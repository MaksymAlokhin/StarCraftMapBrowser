namespace StarCraftMapBrowser
{
    partial class About
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.AboutBox = new System.Windows.Forms.TextBox();
            this.StormLink = new System.Windows.Forms.LinkLabel();
            this.OKbtn = new System.Windows.Forms.Button();
            this.Mail = new System.Windows.Forms.LinkLabel();
            this.GitLink = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // AboutBox
            // 
            this.AboutBox.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AboutBox.Location = new System.Drawing.Point(13, 13);
            this.AboutBox.Multiline = true;
            this.AboutBox.Name = "AboutBox";
            this.AboutBox.ReadOnly = true;
            this.AboutBox.Size = new System.Drawing.Size(377, 223);
            this.AboutBox.TabIndex = 1;
            this.AboutBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // StormLink
            // 
            this.StormLink.AutoSize = true;
            this.StormLink.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StormLink.Location = new System.Drawing.Point(150, 202);
            this.StormLink.Name = "StormLink";
            this.StormLink.Size = new System.Drawing.Size(112, 18);
            this.StormLink.TabIndex = 2;
            this.StormLink.TabStop = true;
            this.StormLink.Text = "Uses StormLib";
            this.StormLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.StormLink_LinkClicked);
            // 
            // OKbtn
            // 
            this.OKbtn.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OKbtn.Location = new System.Drawing.Point(168, 254);
            this.OKbtn.Name = "OKbtn";
            this.OKbtn.Size = new System.Drawing.Size(75, 23);
            this.OKbtn.TabIndex = 0;
            this.OKbtn.Text = "OK";
            this.OKbtn.UseVisualStyleBackColor = true;
            this.OKbtn.Click += new System.EventHandler(this.OKbtn_Click);
            // 
            // Mail
            // 
            this.Mail.AutoSize = true;
            this.Mail.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Mail.Location = new System.Drawing.Point(140, 161);
            this.Mail.Name = "Mail";
            this.Mail.Size = new System.Drawing.Size(134, 18);
            this.Mail.TabIndex = 3;
            this.Mail.TabStop = true;
            this.Mail.Text = "Contact the author";
            this.Mail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Mail_LinkClicked);
            // 
            // GitLink
            // 
            this.GitLink.AutoSize = true;
            this.GitLink.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GitLink.Location = new System.Drawing.Point(160, 124);
            this.GitLink.Name = "GitLink";
            this.GitLink.Size = new System.Drawing.Size(89, 18);
            this.GitLink.TabIndex = 4;
            this.GitLink.TabStop = true;
            this.GitLink.Text = "GitHub Link";
            this.GitLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.GitLink_LinkClicked);
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(403, 289);
            this.Controls.Add(this.GitLink);
            this.Controls.Add(this.Mail);
            this.Controls.Add(this.OKbtn);
            this.Controls.Add(this.StormLink);
            this.Controls.Add(this.AboutBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "About";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox AboutBox;
        private System.Windows.Forms.LinkLabel StormLink;
        private System.Windows.Forms.Button OKbtn;
        private System.Windows.Forms.LinkLabel Mail;
        private System.Windows.Forms.LinkLabel GitLink;
    }
}