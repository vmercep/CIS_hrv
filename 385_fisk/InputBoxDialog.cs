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
    lblPrompt = new System.Windows.Forms.Label();
    txtInput = new System.Windows.Forms.TextBox();
    btnOK = new System.Windows.Forms.Button();
    btnCancel = new System.Windows.Forms.Button();
    SuspendLayout();
    lblPrompt.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right);
    lblPrompt.BackColor = System.Drawing.SystemColors.Control;
    lblPrompt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
    lblPrompt.Location = new System.Drawing.Point(12, 9);
    lblPrompt.Name = "lblPrompt";
    lblPrompt.Size = new System.Drawing.Size(302, 83);
    lblPrompt.TabIndex = 3;
    txtInput.Location = new System.Drawing.Point(8, 100);
    txtInput.Name = "txtInput";
    txtInput.Size = new System.Drawing.Size(379, 20);
    txtInput.TabIndex = 0;
    btnOK.Location = new System.Drawing.Point(320, 10);
    btnOK.Name = "btnOK";
    btnOK.Size = new System.Drawing.Size(75, 25);
    btnOK.TabIndex = 4;
    btnOK.Text = "&OK";
    btnOK.UseVisualStyleBackColor = true;
    btnOK.Click += new System.EventHandler(BtnOKClick);
    btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
    btnCancel.Location = new System.Drawing.Point(320, 41);
    btnCancel.Name = "btnCancel";
    btnCancel.Size = new System.Drawing.Size(75, 25);
    btnCancel.TabIndex = 5;
    btnCancel.Text = "&Cancel";
    btnCancel.UseVisualStyleBackColor = true;
    btnCancel.Click += new System.EventHandler(BtnCancelClick);
    base.AcceptButton = btnOK;
    AutoScaleBaseSize = new System.Drawing.Size(5, 13);
    BackColor = System.Drawing.SystemColors.Control;
    base.CancelButton = btnCancel;
    base.ClientSize = new System.Drawing.Size(398, 128);
    base.Controls.Add(btnCancel);
    base.Controls.Add(btnOK);
    base.Controls.Add(txtInput);
    base.Controls.Add(lblPrompt);
    base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
    base.MaximizeBox = false;
    base.MinimizeBox = false;
    base.Name = "InputBoxDialog";
    base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
    Text = "InputBox";
    base.TopMost = true;
    base.Load += new System.EventHandler(InputBox_Load);
    ResumeLayout(performLayout: false);
    PerformLayout();
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
