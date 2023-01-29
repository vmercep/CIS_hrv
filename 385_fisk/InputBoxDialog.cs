using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

public class InputBoxDialog : Form {
  private Label lblPrompt;

  private Button btnOK;

  private TextBox txtInput;

  private Container components = null;

  private Button btnCancel;

  private string formCaption = string.Empty;

  private string formPrompt = string.Empty;

  private string inputResponse = string.Empty;

  private string defaultValue = string.Empty;

  public string FormCaption {
    get {
      return formCaption;
    }
    set {
      formCaption = value;
    }
  }

  public string FormPrompt {
    get {
      return formPrompt;
    }
    set {
      formPrompt = value;
    }
  }

  public string InputResponse {
    get {
      return inputResponse;
    }
    set {
      inputResponse = value;
    }
  }

  public string DefaultValue {
    get {
      return defaultValue;
    }
    set {
      defaultValue = value;
    }
  }

  public InputBoxDialog () {
    InitializeComponent();
  }

  protected override void Dispose (bool disposing) {
    if (disposing && components != null) {
      components.Dispose();
    }
    base.Dispose(disposing);
  }

  private void InitializeComponent () {
            this.lblPrompt = new System.Windows.Forms.Label();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblPrompt
            // 
            this.lblPrompt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPrompt.BackColor = System.Drawing.SystemColors.Control;
            this.lblPrompt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrompt.Location = new System.Drawing.Point(14, 10);
            this.lblPrompt.Name = "lblPrompt";
            this.lblPrompt.Size = new System.Drawing.Size(372, 113);
            this.lblPrompt.TabIndex = 3;
            // 
            // txtInput
            // 
            this.txtInput.Location = new System.Drawing.Point(10, 115);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(454, 22);
            this.txtInput.TabIndex = 0;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(384, 12);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(90, 28);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.BtnOKClick);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(384, 47);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 29);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancelClick);
            // 
            // InputBoxDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(488, 166);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.lblPrompt);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputBoxDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "InputBox";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.InputBox_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

  }

  private void InputBox_Load (object sender, EventArgs e) {
    txtInput.Text = defaultValue;
    lblPrompt.Text = formPrompt;
    Text = formCaption;
    txtInput.SelectionStart = 0;
    txtInput.SelectionLength = txtInput.Text.Length;
    txtInput.Focus();
  }

  private void BtnOKClick (object sender, EventArgs e) {
    InputResponse = txtInput.Text;
    Close();
  }

  private void BtnCancelClick (object sender, EventArgs e) {
    Close();
  }
}
