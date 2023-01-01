namespace _385_fisk.Exceptions
{
    partial class ErrorMessageBox
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
            this.lbMessageText = new System.Windows.Forms.Label();
            this.btCancel = new System.Windows.Forms.Button();
            this.btSendToSupport = new System.Windows.Forms.Button();
            this.pbErrorBox = new System.Windows.Forms.PictureBox();
            this.tbMessage = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbErrorBox)).BeginInit();
            this.SuspendLayout();
            // 
            // lbMessageText
            // 
            this.lbMessageText.AutoSize = true;
            this.lbMessageText.BackColor = System.Drawing.Color.Transparent;
            this.lbMessageText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMessageText.Location = new System.Drawing.Point(71, 32);
            this.lbMessageText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbMessageText.Name = "lbMessageText";
            this.lbMessageText.Size = new System.Drawing.Size(53, 20);
            this.lbMessageText.TabIndex = 0;
            this.lbMessageText.Text = "label1";
            // 
            // btCancel
            // 
            this.btCancel.BackColor = System.Drawing.Color.Transparent;
            this.btCancel.Location = new System.Drawing.Point(16, 198);
            this.btCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(172, 63);
            this.btCancel.TabIndex = 1;
            this.btCancel.Text = "Nastavi s izdavanjem računa";
            this.btCancel.UseVisualStyleBackColor = false;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // btSendToSupport
            // 
            this.btSendToSupport.Location = new System.Drawing.Point(394, 198);
            this.btSendToSupport.Margin = new System.Windows.Forms.Padding(4);
            this.btSendToSupport.Name = "btSendToSupport";
            this.btSendToSupport.Size = new System.Drawing.Size(169, 63);
            this.btSendToSupport.TabIndex = 4;
            this.btSendToSupport.Text = "Pošalji grešku podršci";
            this.btSendToSupport.UseVisualStyleBackColor = true;
            this.btSendToSupport.Click += new System.EventHandler(this.btSendToSupport_Click);
            // 
            // pbErrorBox
            // 
            this.pbErrorBox.Image = global::_385_fisk.Properties.Resources.symbolError;
            this.pbErrorBox.Location = new System.Drawing.Point(21, 16);
            this.pbErrorBox.Margin = new System.Windows.Forms.Padding(4);
            this.pbErrorBox.Name = "pbErrorBox";
            this.pbErrorBox.Size = new System.Drawing.Size(41, 36);
            this.pbErrorBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbErrorBox.TabIndex = 5;
            this.pbErrorBox.TabStop = false;
            // 
            // tbMessage
            // 
            this.tbMessage.BackColor = System.Drawing.SystemColors.Control;
            this.tbMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbMessage.ForeColor = System.Drawing.Color.Black;
            this.tbMessage.Location = new System.Drawing.Point(16, 70);
            this.tbMessage.Multiline = true;
            this.tbMessage.Name = "tbMessage";
            this.tbMessage.ReadOnly = true;
            this.tbMessage.Size = new System.Drawing.Size(547, 121);
            this.tbMessage.TabIndex = 6;
            // 
            // ErrorMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackgroundImage = global::_385_fisk.Properties.Resources.background2;
            this.ClientSize = new System.Drawing.Size(576, 274);
            this.Controls.Add(this.tbMessage);
            this.Controls.Add(this.pbErrorBox);
            this.Controls.Add(this.btSendToSupport);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.lbMessageText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ErrorMessageBox";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Greška";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pbErrorBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btSendToSupport;
        public System.Windows.Forms.Label lbMessageText;
        private System.Windows.Forms.PictureBox pbErrorBox;
        public System.Windows.Forms.TextBox tbMessage;
    }
}