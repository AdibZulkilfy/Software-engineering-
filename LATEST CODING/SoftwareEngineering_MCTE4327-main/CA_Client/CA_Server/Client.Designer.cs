namespace CA_Server
{
    partial class Client
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Client));
            this.label1 = new System.Windows.Forms.Label();
            this.connectButton = new System.Windows.Forms.Button();
            this.addLabel = new System.Windows.Forms.Label();
            this.portLabel = new System.Windows.Forms.Label();
            this.unLabel = new System.Windows.Forms.Label();
            this.keyLabel = new System.Windows.Forms.Label();
            this.hideCheckBox = new System.Windows.Forms.CheckBox();
            this.addBox = new System.Windows.Forms.TextBox();
            this.portBox = new System.Windows.Forms.TextBox();
            this.unBox = new System.Windows.Forms.TextBox();
            this.keyBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.sendBox = new System.Windows.Forms.TextBox();
            this.clearButton = new System.Windows.Forms.Button();
            this.logLabel = new System.Windows.Forms.Label();
            this.Logo = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.imageButton = new System.Windows.Forms.Button();
            this.logBox = new System.Windows.Forms.RichTextBox();
            this.SendButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Logo)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(20, 18);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(170, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Welcome To Our Chat Application";
            // 
            // connectButton
            // 
            this.connectButton.BackColor = System.Drawing.Color.LightGreen;
            this.connectButton.Location = new System.Drawing.Point(20, 339);
            this.connectButton.Margin = new System.Windows.Forms.Padding(2);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(166, 31);
            this.connectButton.TabIndex = 1;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = false;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // addLabel
            // 
            this.addLabel.AutoSize = true;
            this.addLabel.Location = new System.Drawing.Point(20, 100);
            this.addLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.addLabel.Name = "addLabel";
            this.addLabel.Size = new System.Drawing.Size(58, 13);
            this.addLabel.TabIndex = 2;
            this.addLabel.Text = "IP Address";
            // 
            // portLabel
            // 
            this.portLabel.AutoSize = true;
            this.portLabel.Location = new System.Drawing.Point(20, 146);
            this.portLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(43, 13);
            this.portLabel.TabIndex = 3;
            this.portLabel.Text = "Port No";
            // 
            // unLabel
            // 
            this.unLabel.AutoSize = true;
            this.unLabel.Location = new System.Drawing.Point(20, 210);
            this.unLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.unLabel.Name = "unLabel";
            this.unLabel.Size = new System.Drawing.Size(55, 13);
            this.unLabel.TabIndex = 4;
            this.unLabel.Text = "Username";
            // 
            // keyLabel
            // 
            this.keyLabel.AutoSize = true;
            this.keyLabel.Location = new System.Drawing.Point(20, 262);
            this.keyLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.keyLabel.Name = "keyLabel";
            this.keyLabel.Size = new System.Drawing.Size(53, 13);
            this.keyLabel.TabIndex = 5;
            this.keyLabel.Text = "Password";
            // 
            // hideCheckBox
            // 
            this.hideCheckBox.AutoSize = true;
            this.hideCheckBox.Location = new System.Drawing.Point(23, 309);
            this.hideCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.hideCheckBox.Name = "hideCheckBox";
            this.hideCheckBox.Size = new System.Drawing.Size(97, 17);
            this.hideCheckBox.TabIndex = 6;
            this.hideCheckBox.Text = "Hide Password";
            this.hideCheckBox.UseVisualStyleBackColor = true;
            this.hideCheckBox.CheckedChanged += new System.EventHandler(this.hideCheckBox_CheckedChanged);
            // 
            // addBox
            // 
            this.addBox.Location = new System.Drawing.Point(20, 120);
            this.addBox.Margin = new System.Windows.Forms.Padding(2);
            this.addBox.Name = "addBox";
            this.addBox.Size = new System.Drawing.Size(168, 20);
            this.addBox.TabIndex = 7;
            // 
            // portBox
            // 
            this.portBox.Location = new System.Drawing.Point(23, 160);
            this.portBox.Margin = new System.Windows.Forms.Padding(2);
            this.portBox.Name = "portBox";
            this.portBox.Size = new System.Drawing.Size(168, 20);
            this.portBox.TabIndex = 8;
            // 
            // unBox
            // 
            this.unBox.Location = new System.Drawing.Point(23, 232);
            this.unBox.Margin = new System.Windows.Forms.Padding(2);
            this.unBox.Name = "unBox";
            this.unBox.Size = new System.Drawing.Size(168, 20);
            this.unBox.TabIndex = 9;
            // 
            // keyBox
            // 
            this.keyBox.Location = new System.Drawing.Point(23, 281);
            this.keyBox.Margin = new System.Windows.Forms.Padding(2);
            this.keyBox.Name = "keyBox";
            this.keyBox.Size = new System.Drawing.Size(168, 20);
            this.keyBox.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(71, 189);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Client Login";
            // 
            // sendBox
            // 
            this.sendBox.Location = new System.Drawing.Point(276, 384);
            this.sendBox.Margin = new System.Windows.Forms.Padding(2);
            this.sendBox.Name = "sendBox";
            this.sendBox.Size = new System.Drawing.Size(326, 20);
            this.sendBox.TabIndex = 13;
            this.sendBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SendBox_KeyDown);
            // 
            // clearButton
            // 
            this.clearButton.BackColor = System.Drawing.Color.MistyRose;
            this.clearButton.Location = new System.Drawing.Point(23, 376);
            this.clearButton.Margin = new System.Windows.Forms.Padding(2);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(82, 24);
            this.clearButton.TabIndex = 14;
            this.clearButton.Text = "Clear chat";
            this.clearButton.UseVisualStyleBackColor = false;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // logLabel
            // 
            this.logLabel.AutoSize = true;
            this.logLabel.Location = new System.Drawing.Point(396, 11);
            this.logLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.logLabel.Name = "logLabel";
            this.logLabel.Size = new System.Drawing.Size(25, 13);
            this.logLabel.TabIndex = 18;
            this.logLabel.Text = "Log";
            // 
            // Logo
            // 
            this.Logo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Logo.BackgroundImage")));
            this.Logo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Logo.Image = ((System.Drawing.Image)(resources.GetObject("Logo.Image")));
            this.Logo.Location = new System.Drawing.Point(23, 41);
            this.Logo.Margin = new System.Windows.Forms.Padding(2);
            this.Logo.Name = "Logo";
            this.Logo.Size = new System.Drawing.Size(166, 43);
            this.Logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Logo.TabIndex = 31;
            this.Logo.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(108, 376);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(78, 24);
            this.button1.TabIndex = 32;
            this.button1.Text = "Export";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.exportButton_Click);
            // 
            // imageButton
            // 
            this.imageButton.Location = new System.Drawing.Point(216, 378);
            this.imageButton.Margin = new System.Windows.Forms.Padding(2);
            this.imageButton.Name = "imageButton";
            this.imageButton.Size = new System.Drawing.Size(57, 31);
            this.imageButton.TabIndex = 34;
            this.imageButton.Text = "Upload";
            this.imageButton.UseVisualStyleBackColor = true;
            this.imageButton.Click += new System.EventHandler(this.imageButton_Click);
            // 
            // logBox
            // 
            this.logBox.Location = new System.Drawing.Point(216, 29);
            this.logBox.Margin = new System.Windows.Forms.Padding(2);
            this.logBox.Name = "logBox";
            this.logBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.logBox.Size = new System.Drawing.Size(466, 345);
            this.logBox.TabIndex = 12;
            this.logBox.Text = "";
            // 
            // SendButton
            // 
            this.SendButton.Location = new System.Drawing.Point(607, 379);
            this.SendButton.Name = "SendButton";
            this.SendButton.Size = new System.Drawing.Size(75, 33);
            this.SendButton.TabIndex = 35;
            this.SendButton.Text = "Send";
            this.SendButton.UseVisualStyleBackColor = true;
            this.SendButton.Click += new System.EventHandler(this.SendButton_Click);
            // 
            // Client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(704, 415);
            this.Controls.Add(this.SendButton);
            this.Controls.Add(this.imageButton);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Logo);
            this.Controls.Add(this.logLabel);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.sendBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.keyBox);
            this.Controls.Add(this.unBox);
            this.Controls.Add(this.portBox);
            this.Controls.Add(this.addBox);
            this.Controls.Add(this.hideCheckBox);
            this.Controls.Add(this.keyLabel);
            this.Controls.Add(this.unLabel);
            this.Controls.Add(this.portLabel);
            this.Controls.Add(this.addLabel);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.logBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Client";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Client";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Client_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.Logo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Label addLabel;
        private System.Windows.Forms.Label portLabel;
        private System.Windows.Forms.Label unLabel;
        private System.Windows.Forms.Label keyLabel;
        private System.Windows.Forms.CheckBox hideCheckBox;
        private System.Windows.Forms.TextBox addBox;
        private System.Windows.Forms.TextBox portBox;
        private System.Windows.Forms.TextBox unBox;
        private System.Windows.Forms.TextBox keyBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox sendBox;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Label logLabel;
        private System.Windows.Forms.PictureBox Logo;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button imageButton;
        private System.Windows.Forms.RichTextBox logBox;
        private System.Windows.Forms.Button SendButton;
    }
}

