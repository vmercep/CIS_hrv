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
            this.btOK = new System.Windows.Forms.Button();
            this.btSendToSupport = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbMessageText
            // 
            this.lbMessageText.AutoSize = true;
            this.lbMessageText.BackColor = System.Drawing.Color.Transparent;
            this.lbMessageText.Location = new System.Drawing.Point(13, 30);
            this.lbMessageText.Name = "lbMessageText";
            this.lbMessageText.Size = new System.Drawing.Size(35, 13);
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
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = false;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // lbMessageDetails
            // 
            this.lbMessageDetails.AutoSize = true;
            this.lbMessageDetails.BackColor = System.Drawing.Color.Transparent;
            this.lbMessageDetails.Location = new System.Drawing.Point(13, 68);
            this.lbMessageDetails.Name = "lbMessageDetails";
            this.lbMessageDetails.Size = new System.Drawing.Size(35, 13);
            this.lbMessageDetails.TabIndex = 2;
            this.lbMessageDetails.Text = "label1";
            // 
            // btOK
            // 
            this.btOK.Location = new System.Drawing.Point(345, 122);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.TabIndex = 3;
            this.btOK.Text = "OK";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // btSendToSupport
            // 
            this.btSendToSupport.Location = new System.Drawing.Point(237, 122);
            this.btSendToSupport.Name = "btSendToSupport";
            this.btSendToSupport.Size = new System.Drawing.Size(102, 23);
            this.btSendToSupport.TabIndex = 4;
            this.btSendToSupport.Text = "Send to support";
            this.btSendToSupport.UseVisualStyleBackColor = true;
            this.btSendToSupport.Click += new System.EventHandler(this.btSendToSupport_Click);
            // 
            // ErrorMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackgroundImage = global::_385_fisk.Properties.Resources.background2;
            this.ClientSize = new System.Drawing.Size(432, 160);
            this.Controls.Add(this.btSendToSupport);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.lbMessageDetails);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.lbMessageText);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ErrorMessageBox";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Message";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Button btSendToSupport;
        public System.Windows.Forms.Label lbMessageText;
        public System.Windows.Forms.Label lbMessageDetails;
    }
}