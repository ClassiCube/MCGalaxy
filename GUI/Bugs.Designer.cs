namespace MCForge.GUI
{
    partial class Bugs
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Bugs));
            this.ribbon1 = new System.Windows.Forms.Ribbon();
            this.ribbonTab1 = new System.Windows.Forms.RibbonTab();
            this.problem = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.email = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.name = new System.Windows.Forms.TextBox();
            this.ribbonPanel1 = new System.Windows.Forms.RibbonPanel();
            this.ribbonButton1 = new System.Windows.Forms.RibbonButton();
            this.SuspendLayout();
            // 
            // ribbon1
            // 
            this.ribbon1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ribbon1.Location = new System.Drawing.Point(0, 0);
            this.ribbon1.Minimized = false;
            this.ribbon1.Name = "ribbon1";
            // 
            // 
            // 
            this.ribbon1.OrbDropDown.BorderRoundness = 8;
            this.ribbon1.OrbDropDown.Location = new System.Drawing.Point(0, 0);
            this.ribbon1.OrbDropDown.Name = "";
            this.ribbon1.OrbDropDown.Size = new System.Drawing.Size(527, 447);
            this.ribbon1.OrbDropDown.TabIndex = 0;
            this.ribbon1.OrbImage = null;
            this.ribbon1.OrbVisible = false;
            // 
            // 
            // 
            this.ribbon1.QuickAcessToolbar.AltKey = null;
            this.ribbon1.QuickAcessToolbar.Image = null;
            this.ribbon1.QuickAcessToolbar.Tag = null;
            this.ribbon1.QuickAcessToolbar.Text = null;
            this.ribbon1.QuickAcessToolbar.ToolTip = null;
            this.ribbon1.QuickAcessToolbar.ToolTipImage = null;
            this.ribbon1.QuickAcessToolbar.ToolTipTitle = null;
            this.ribbon1.Size = new System.Drawing.Size(407, 138);
            this.ribbon1.TabIndex = 0;
            this.ribbon1.Tabs.Add(this.ribbonTab1);
            this.ribbon1.TabSpacing = 6;
            this.ribbon1.Text = "ribbon1";
            // 
            // ribbonTab1
            // 
            this.ribbonTab1.Panels.Add(this.ribbonPanel1);
            this.ribbonTab1.Tag = null;
            this.ribbonTab1.Text = "Bug Reports";
            // 
            // problem
            // 
            this.problem.Location = new System.Drawing.Point(0, 166);
            this.problem.Multiline = true;
            this.problem.Name = "problem";
            this.problem.Size = new System.Drawing.Size(407, 154);
            this.problem.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(134, 150);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Describe your problem";
            // 
            // email
            // 
            this.email.Location = new System.Drawing.Point(42, 330);
            this.email.Name = "email";
            this.email.Size = new System.Drawing.Size(177, 20);
            this.email.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 333);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Email";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(226, 330);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Name";
            // 
            // name
            // 
            this.name.Location = new System.Drawing.Point(267, 330);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(128, 20);
            this.name.TabIndex = 6;
            // 
            // ribbonPanel1
            // 
            this.ribbonPanel1.Items.Add(this.ribbonButton1);
            this.ribbonPanel1.Tag = null;
            this.ribbonPanel1.Text = "ribbonPanel1";
            // 
            // ribbonButton1
            // 
            this.ribbonButton1.AltKey = null;
            this.ribbonButton1.DropDownArrowDirection = System.Windows.Forms.RibbonArrowDirection.Down;
            this.ribbonButton1.DropDownArrowSize = new System.Drawing.Size(5, 3);
            this.ribbonButton1.Image = ((System.Drawing.Image)(resources.GetObject("ribbonButton1.Image")));
            this.ribbonButton1.SmallImage = ((System.Drawing.Image)(resources.GetObject("ribbonButton1.SmallImage")));
            this.ribbonButton1.Style = System.Windows.Forms.RibbonButtonStyle.Normal;
            this.ribbonButton1.Tag = null;
            this.ribbonButton1.Text = "Submit";
            this.ribbonButton1.ToolTip = null;
            this.ribbonButton1.ToolTipImage = null;
            this.ribbonButton1.ToolTipTitle = null;
            // 
            // Bugs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(407, 352);
            this.Controls.Add(this.name);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.email);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.problem);
            this.Controls.Add(this.ribbon1);
            this.Name = "Bugs";
            this.ShowInTaskbar = false;
            this.Text = "Bugs";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Ribbon ribbon1;
        private System.Windows.Forms.RibbonTab ribbonTab1;
        private System.Windows.Forms.RibbonPanel ribbonPanel1;
        private System.Windows.Forms.RibbonButton ribbonButton1;
        private System.Windows.Forms.TextBox problem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox email;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox name;

    }
}