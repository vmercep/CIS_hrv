using Helper;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

public class ExpirationDate : Form {
  private IContainer components = null;

  private Button button1;

  private DateTimePicker dateTimePicker1;

  private Label label1;

  public ExpirationDate () {
    if (!File.Exists("95d6c3f32d0508ebce35724496382eb3")) {
      FileStream fileStream = File.Create("95d6c3f32d0508ebce35724496382eb3");
      fileStream.Flush();
      fileStream.Close();
      ValidateExpiryDate.Save(DateTime.Now.ToString("yyyy.MM.dd"));
    }
    InitializeComponent();
  }

  private void ExpirationDate_Load (object sender, EventArgs e) {
    string a = InputBox(Translations.Translate("Unesite tehničku šifru"), Translations.Translate("Tehnička šifra"), "");
    string techCode = AppLink.GetTechCode();
    if (a == techCode) {
      DateTime value = ValidateExpiryDate.Load();
      dateTimePicker1.Value = value;
    } else {
      MessageBox.Show(Translations.Translate("Neispravna šifra!"), Translations.Translate("Greška"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
      Application.Exit();
    }
    Text = Translations.Translate(Text);
    label1.Text = Translations.Translate(label1.Text);
    button1.Text = Translations.Translate(button1.Text);
  }

  public static string InputBox (string prompt, string title, string defaultValue) {
    InputBoxDialog inputBoxDialog = new InputBoxDialog();
    inputBoxDialog.FormPrompt = prompt;
    inputBoxDialog.FormCaption = title;
    inputBoxDialog.DefaultValue = defaultValue;
    inputBoxDialog.ShowDialog();
    string inputResponse = inputBoxDialog.InputResponse;
    inputBoxDialog.Close();
    return inputResponse;
  }

  private void button1_Click (object sender, EventArgs e) {
    string dateTime = dateTimePicker1.Value.ToString("yyyy.MM.dd");
    ValidateExpiryDate.Save(dateTime);
    Application.Exit();
  }

  protected override void Dispose (bool disposing) {
    if (disposing && components != null) {
      components.Dispose();
    }
    base.Dispose(disposing);
  }

  private void InitializeComponent () {
    System.ComponentModel.ComponentResourceManager componentResourceManager = new System.ComponentModel.ComponentResourceManager(typeof(ExpirationDate));
    button1 = new System.Windows.Forms.Button();
    dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
    label1 = new System.Windows.Forms.Label();
    SuspendLayout();
    button1.Location = new System.Drawing.Point(133, 85);
    button1.Name = "button1";
    button1.Size = new System.Drawing.Size(101, 23);
    button1.TabIndex = 5;
    button1.Text = "Spremi izmjenu";
    button1.UseVisualStyleBackColor = true;
    button1.Click += new System.EventHandler(button1_Click);
    dateTimePicker1.Location = new System.Drawing.Point(23, 38);
    dateTimePicker1.Name = "dateTimePicker1";
    dateTimePicker1.Size = new System.Drawing.Size(200, 20);
    dateTimePicker1.TabIndex = 4;
    label1.AutoSize = true;
    label1.Location = new System.Drawing.Point(16, 15);
    label1.Name = "label1";
    label1.Size = new System.Drawing.Size(215, 13);
    label1.TabIndex = 3;
    label1.Text = "Datum isteka opcije spremanja XML poruka:";
    base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
    base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
    base.ClientSize = new System.Drawing.Size(246, 120);
    base.Controls.Add(button1);
    base.Controls.Add(dateTimePicker1);
    base.Controls.Add(label1);
    base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
    base.Icon = _385_fisk.Properties.Resources.icon;
    base.Margin = new System.Windows.Forms.Padding(2);
    base.MaximizeBox = false;
    base.Name = "ExpirationDate";
    base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
    Text = "Validacija";
    base.Load += new System.EventHandler(ExpirationDate_Load);
    ResumeLayout(performLayout: false);
    PerformLayout();
  }
}
