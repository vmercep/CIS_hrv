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
            this.lbMessageDetails = new System.Windows.Forms.Label();
            this.btSendToSupport = new System.Windows.Forms.Button();
            this.pbErrorBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbErrorBox)).BeginInit();
            this.SuspendLayout();
            // 
            // lbMessageText
            // 
            this.lbMessageText.AutoSize = true;
            this.lbMessageText.BackColor = System.Drawing.Color.Transparent;
            this.lbMessageText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMessageText.Location = new System.Drawing.Point(53, 26);
            this.lbMessageText.Name = "lbMessageText";
            this.lbMessageText.Size = new System.Drawing.Size(45, 16);
            this.lbMessageText.TabIndex = 0;
            this.lbMessageText.Text = "label1";
            // 
            // btCancel
            // 
            this.btCancel.BackColor = System.Drawing.Color.Transparent;
            this.btCancel.Location = new System.Drawing.Point(12, 122);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 1;
            this.btCancel.Text = "Otkaži";
            this.btCancel.UseVisualStyleBackColor = false;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // lbMessageDetails
            // 
            this.lbMessageDetails.AutoSize = true;
            this.lbMessageDetails.BackColor = System.Drawing.Color.Transparent;
            this.lbMessageDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMessageDetails.Location = new System.Drawing.Point(13, 62);
            this.lbMessageDetails.Name = "lbMessageDetails";
            this.lbMessageDetails.Size = new System.Drawing.Size(51, 16);
            this.lbMessageDetails.TabIndex = 2;
            this.lbMessageDetails.Text = "label1";
            // 
            // btSendToSupport
            // 
            this.btSendToSupport.Location = new System.Drawing.Point(284, 122);
            this.btSendToSupport.Name = "btSendToSupport";
            this.btSendToSupport.Size = new System.Drawing.Size(136, 23);
            this.btSendToSupport.TabIndex = 4;
            this.btSendToSupport.Text = "Pošalji grešku podršci";
            this.btSendToSupport.UseVisualStyleBackColor = true;
            this.btSendToSupport.Click += new System.EventHandler(this.btSendToSupport_Click);
            // 
            // pbErrorBox
            // 
            this.pbErrorBox.Image = global::_385_fisk.Properties.Resources.symbolError;
            this.pbErrorBox.Location = new System.Drawing.Point(16, 13);
            this.pbErrorBox.Name = "pbErrorBox";
            this.pbErrorBox.Size = new System.Drawing.Size(31, 29);
            this.pbErrorBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbErrorBox.TabIndex = 5;
            this.pbErrorBox.TabStop = false;
            // 
            // ErrorMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackgroundImage = global::_385_fisk.Properties.Resources.background2;
            this.ClientSize = new System.Drawing.Size(432, 160);
            this.Controls.Add(this.pbErrorBox);
            this.Controls.Add(this.btSendToSupport);
            this.Controls.Add(this.lbMessageDetails);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.lbMessageText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
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
        public System.Windows.Forms.Label lbMessageDetails;
        private System.Windows.Forms.PictureBox pbErrorBox;
    }
}