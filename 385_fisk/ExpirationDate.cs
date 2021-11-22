using Helper;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

public class ExpirationDate : Form {
  private IContainer components = null;

  private Button button1;
    public DateTimePicker dateTimePicker1;
    private Label label1;

  public ExpirationDate () {
    InitializeComponent();
  }

  

  private void button1_Click (object sender, EventArgs e) {
        Close();
  }

  protected override void Dispose (bool disposing) {
    if (disposing && components != null) {
      components.Dispose();
    }
    base.Dispose(disposing);
  }

  private void InitializeComponent () {
            this.button1 = new System.Windows.Forms.Button();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(200, 131);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(152, 35);
            this.button1.TabIndex = 5;
            this.button1.Text = "Spremi izmjenu";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(34, 58);
            this.dateTimePicker1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(298, 26);
            this.dateTimePicker1.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 23);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Datum od:";
            // 
            // ExpirationDate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(369, 185);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::_385_fisk.Properties.Resources.icon;
            this.MaximizeBox = false;
            this.Name = "ExpirationDate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "QrRegenDatum";
            this.Load += new System.EventHandler(this.ExpirationDate_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

  }

    private void ExpirationDate_Load(object sender, EventArgs e)
    {
        dateTimePicker1.Value = DateTime.Today.AddMonths(-1);
    }
}
