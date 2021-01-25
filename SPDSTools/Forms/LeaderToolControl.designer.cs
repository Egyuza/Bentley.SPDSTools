using System;

namespace SPDSTools
{
    partial class LeaderToolControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LeaderToolControl));
            this.labTextStyle = new System.Windows.Forms.Label();
            this.cbxTextStyle = new System.Windows.Forms.ComboBox();
            this.labLeaderType = new System.Windows.Forms.Label();
            this.cbxOrientation = new System.Windows.Forms.ComboBox();
            this.labPointer = new System.Windows.Forms.Label();
            this.chbxMultiShelf = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cbListViewCombo = new System.Windows.Forms.ComboBox();
            this.labOrientation = new System.Windows.Forms.Label();
            this.cbxLeaderType = new MyComboBox();
            this.cbxPointer = new MyComboBox();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.lvText = new MyListView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.tlpMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // labTextStyle
            // 
            resources.ApplyResources(this.labTextStyle, "labTextStyle");
            this.labTextStyle.Name = "labTextStyle";
            // 
            // cbxTextStyle
            // 
            resources.ApplyResources(this.cbxTextStyle, "cbxTextStyle");
            this.cbxTextStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTextStyle.FormattingEnabled = true;
            this.cbxTextStyle.Name = "cbxTextStyle";
            // 
            // labLeaderType
            // 
            resources.ApplyResources(this.labLeaderType, "labLeaderType");
            this.labLeaderType.Name = "labLeaderType";
            // 
            // cbxOrientation
            // 
            resources.ApplyResources(this.cbxOrientation, "cbxOrientation");
            this.cbxOrientation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxOrientation.FormattingEnabled = true;
            this.cbxOrientation.Items.AddRange(new object[] {
            resources.GetString("cbxOrientation.Items"),
            resources.GetString("cbxOrientation.Items1"),
            resources.GetString("cbxOrientation.Items2")});
            this.cbxOrientation.Name = "cbxOrientation";
            // 
            // labPointer
            // 
            resources.ApplyResources(this.labPointer, "labPointer");
            this.labPointer.Name = "labPointer";
            // 
            // chbxMultiShelf
            // 
            resources.ApplyResources(this.chbxMultiShelf, "chbxMultiShelf");
            this.chbxMultiShelf.Name = "chbxMultiShelf";
            this.chbxMultiShelf.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.cbListViewCombo, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.labOrientation, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.chbxMultiShelf, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.cbxLeaderType, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.labLeaderType, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.labPointer, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.cbxPointer, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.cbxOrientation, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.labTextStyle, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.cbxTextStyle, 0, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // cbListViewCombo
            // 
            resources.ApplyResources(this.cbListViewCombo, "cbListViewCombo");
            this.cbListViewCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbListViewCombo.FormattingEnabled = true;
            this.cbListViewCombo.Items.AddRange(new object[] {
            resources.GetString("cbListViewCombo.Items"),
            resources.GetString("cbListViewCombo.Items1"),
            resources.GetString("cbListViewCombo.Items2")});
            this.cbListViewCombo.Name = "cbListViewCombo";
            // 
            // labOrientation
            // 
            resources.ApplyResources(this.labOrientation, "labOrientation");
            this.labOrientation.Name = "labOrientation";
            // 
            // cbxLeaderType
            // 
            this.cbxLeaderType.AllowDrop = true;
            this.cbxLeaderType.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.cbxLeaderType, "cbxLeaderType");
            this.cbxLeaderType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxLeaderType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxLeaderType.ForeColor = System.Drawing.SystemColors.WindowText;
            this.cbxLeaderType.FormattingEnabled = true;
            this.cbxLeaderType.ImageList = null;
            this.cbxLeaderType.Name = "cbxLeaderType";
            // 
            // cbxPointer
            // 
            this.cbxPointer.AllowDrop = true;
            this.cbxPointer.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.cbxPointer, "cbxPointer");
            this.cbxPointer.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxPointer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxPointer.ForeColor = System.Drawing.SystemColors.WindowText;
            this.cbxPointer.FormattingEnabled = true;
            this.cbxPointer.ImageList = null;
            this.cbxPointer.Name = "cbxPointer";
            // 
            // tlpMain
            // 
            resources.ApplyResources(this.tlpMain, "tlpMain");
            this.tlpMain.Controls.Add(this.lvText, 0, 0);
            this.tlpMain.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.tlpMain.Name = "tlpMain";
            // 
            // lvText
            // 
            resources.ApplyResources(this.lvText, "lvText");
            this.lvText.GridLines = true;
            this.lvText.Name = "lvText";
            this.lvText.UseCompatibleStateImageBehavior = false;
            this.lvText.MouseUp += new System.Windows.Forms.MouseEventHandler(this.myListView_MouseUp);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth16Bit;
            resources.ApplyResources(this.imageList1, "imageList1");
            this.imageList1.TransparentColor = System.Drawing.SystemColors.ActiveBorder;
            // 
            // imageList2
            // 
            this.imageList2.ColorDepth = System.Windows.Forms.ColorDepth.Depth16Bit;
            resources.ApplyResources(this.imageList2, "imageList2");
            this.imageList2.TransparentColor = System.Drawing.SystemColors.ActiveBorder;
            // 
            // LeaderToolControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlpMain);
            this.Name = "LeaderToolControl";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tlpMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label labTextStyle;
        public System.Windows.Forms.ComboBox cbxTextStyle;
        private System.Windows.Forms.Label labLeaderType;
        private System.Windows.Forms.ComboBox cbxOrientation;
        private System.Windows.Forms.Label labPointer;
        private MyComboBox cbxLeaderType;
        private MyComboBox cbxPointer;
        public System.Windows.Forms.CheckBox chbxMultiShelf;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labOrientation;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        public MyListView lvText;
        public System.Windows.Forms.ImageList imageList1;
        public System.Windows.Forms.ImageList imageList2;
        private System.Windows.Forms.ComboBox cbListViewCombo;
    }
}
