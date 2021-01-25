namespace SPDSTools
{
    partial class HeightToolControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HeightToolControl));
            this.chbxAutoCalc = new System.Windows.Forms.CheckBox();
            this.chbxAsLeader = new System.Windows.Forms.CheckBox();
            this.lbTextStyle = new System.Windows.Forms.Label();
            this.cbxTextStyle = new System.Windows.Forms.ComboBox();
            this.txtText = new System.Windows.Forms.TextBox();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.grbxAdditions = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.chbxSectorArrow = new System.Windows.Forms.CheckBox();
            this.chbxUseFilling = new System.Windows.Forms.CheckBox();
            this.grbxType = new System.Windows.Forms.GroupBox();
            this.tlpType = new System.Windows.Forms.TableLayoutPanel();
            this.rbtnType_Planned = new System.Windows.Forms.RadioButton();
            this.rbtnType_Section = new System.Windows.Forms.RadioButton();
            this.grbxOrientation = new System.Windows.Forms.GroupBox();
            this.tlpOrientation = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.chbxOrient_Bottom = new System.Windows.Forms.CheckBox();
            this.chbxOrient_Top = new System.Windows.Forms.CheckBox();
            this.chbxOrient_Auto = new System.Windows.Forms.CheckBox();
            this.chbxOrient_Right = new System.Windows.Forms.CheckBox();
            this.chbxOrient_Left = new System.Windows.Forms.CheckBox();
            this.grbxValue = new System.Windows.Forms.GroupBox();
            this.tlpValue = new System.Windows.Forms.TableLayoutPanel();
            this.chbxCalcByItem = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.chbxPlusMinus = new System.Windows.Forms.CheckBox();
            this.chbxThikness = new System.Windows.Forms.CheckBox();
            this.tlpTextStyle = new System.Windows.Forms.TableLayoutPanel();
            this.txtThikness = new System.Windows.Forms.TextBox();
            this.tlpMain.SuspendLayout();
            this.grbxAdditions.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.grbxType.SuspendLayout();
            this.tlpType.SuspendLayout();
            this.grbxOrientation.SuspendLayout();
            this.tlpOrientation.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.grbxValue.SuspendLayout();
            this.tlpValue.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tlpTextStyle.SuspendLayout();
            this.SuspendLayout();
            // 
            // chbxAutoCalc
            // 
            resources.ApplyResources(this.chbxAutoCalc, "chbxAutoCalc");
            this.chbxAutoCalc.Checked = true;
            this.chbxAutoCalc.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbxAutoCalc.Name = "chbxAutoCalc";
            this.chbxAutoCalc.UseVisualStyleBackColor = true;
            this.chbxAutoCalc.CheckedChanged += new System.EventHandler(this.chbxAutoCalc_CheckedChanged);
            // 
            // chbxAsLeader
            // 
            resources.ApplyResources(this.chbxAsLeader, "chbxAsLeader");
            this.chbxAsLeader.Name = "chbxAsLeader";
            this.chbxAsLeader.UseVisualStyleBackColor = true;
            this.chbxAsLeader.CheckedChanged += new System.EventHandler(this.chbxAsLeader_CheckedChanged);
            // 
            // lbTextStyle
            // 
            resources.ApplyResources(this.lbTextStyle, "lbTextStyle");
            this.lbTextStyle.Name = "lbTextStyle";
            // 
            // cbxTextStyle
            // 
            resources.ApplyResources(this.cbxTextStyle, "cbxTextStyle");
            this.cbxTextStyle.Name = "cbxTextStyle";
            this.cbxTextStyle.SelectedIndexChanged += new System.EventHandler(this.cbxTextStyle_SelectedIndexChanged);
            // 
            // txtText
            // 
            resources.ApplyResources(this.txtText, "txtText");
            this.txtText.Name = "txtText";
            this.txtText.TextChanged += new System.EventHandler(this.txtText_TextChanged);
            this.txtText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtText_KeyDown);
            // 
            // tlpMain
            // 
            resources.ApplyResources(this.tlpMain, "tlpMain");
            this.tlpMain.Controls.Add(this.grbxAdditions, 0, 3);
            this.tlpMain.Controls.Add(this.grbxType, 0, 2);
            this.tlpMain.Controls.Add(this.grbxOrientation, 0, 1);
            this.tlpMain.Controls.Add(this.grbxValue, 0, 0);
            this.tlpMain.Name = "tlpMain";
            // 
            // grbxAdditions
            // 
            resources.ApplyResources(this.grbxAdditions, "grbxAdditions");
            this.grbxAdditions.Controls.Add(this.tableLayoutPanel1);
            this.grbxAdditions.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.grbxAdditions.Name = "grbxAdditions";
            this.grbxAdditions.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.chbxSectorArrow, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.chbxUseFilling, 0, 0);
            this.tableLayoutPanel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // chbxSectorArrow
            // 
            resources.ApplyResources(this.chbxSectorArrow, "chbxSectorArrow");
            this.chbxSectorArrow.Name = "chbxSectorArrow";
            this.chbxSectorArrow.UseVisualStyleBackColor = true;
            this.chbxSectorArrow.CheckedChanged += new System.EventHandler(this.chbxSectorArrow_CheckedChanged);
            // 
            // chbxUseFilling
            // 
            resources.ApplyResources(this.chbxUseFilling, "chbxUseFilling");
            this.chbxUseFilling.Checked = true;
            this.chbxUseFilling.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbxUseFilling.Name = "chbxUseFilling";
            this.chbxUseFilling.UseVisualStyleBackColor = true;
            this.chbxUseFilling.CheckedChanged += new System.EventHandler(this.chbxUseFilling_CheckedChanged);
            // 
            // grbxType
            // 
            resources.ApplyResources(this.grbxType, "grbxType");
            this.grbxType.Controls.Add(this.tlpType);
            this.grbxType.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.grbxType.Name = "grbxType";
            this.grbxType.TabStop = false;
            // 
            // tlpType
            // 
            resources.ApplyResources(this.tlpType, "tlpType");
            this.tlpType.Controls.Add(this.rbtnType_Planned, 0, 1);
            this.tlpType.Controls.Add(this.rbtnType_Section, 0, 0);
            this.tlpType.Controls.Add(this.chbxAsLeader, 1, 1);
            this.tlpType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tlpType.Name = "tlpType";
            // 
            // rbtnType_Planned
            // 
            resources.ApplyResources(this.rbtnType_Planned, "rbtnType_Planned");
            this.rbtnType_Planned.Checked = true;
            this.rbtnType_Planned.Name = "rbtnType_Planned";
            this.rbtnType_Planned.TabStop = true;
            this.rbtnType_Planned.UseVisualStyleBackColor = true;
            this.rbtnType_Planned.CheckedChanged += new System.EventHandler(this.rbtnType_CheckedChanged);
            // 
            // rbtnType_Section
            // 
            resources.ApplyResources(this.rbtnType_Section, "rbtnType_Section");
            this.rbtnType_Section.Name = "rbtnType_Section";
            this.rbtnType_Section.UseVisualStyleBackColor = true;
            this.rbtnType_Section.CheckedChanged += new System.EventHandler(this.rbtnType_CheckedChanged);
            // 
            // grbxOrientation
            // 
            resources.ApplyResources(this.grbxOrientation, "grbxOrientation");
            this.grbxOrientation.Controls.Add(this.tlpOrientation);
            this.grbxOrientation.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.grbxOrientation.Name = "grbxOrientation";
            this.grbxOrientation.TabStop = false;
            // 
            // tlpOrientation
            // 
            resources.ApplyResources(this.tlpOrientation, "tlpOrientation");
            this.tlpOrientation.Controls.Add(this.tableLayoutPanel2, 2, 0);
            this.tlpOrientation.Controls.Add(this.chbxOrient_Auto, 0, 1);
            this.tlpOrientation.Controls.Add(this.chbxOrient_Right, 3, 1);
            this.tlpOrientation.Controls.Add(this.chbxOrient_Left, 1, 1);
            this.tlpOrientation.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tlpOrientation.Name = "tlpOrientation";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.chbxOrient_Bottom, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.chbxOrient_Top, 0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tlpOrientation.SetRowSpan(this.tableLayoutPanel2, 3);
            // 
            // chbxOrient_Bottom
            // 
            resources.ApplyResources(this.chbxOrient_Bottom, "chbxOrient_Bottom");
            this.chbxOrient_Bottom.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.chbxOrient_Bottom.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gray;
            this.chbxOrient_Bottom.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.chbxOrient_Bottom.Name = "chbxOrient_Bottom";
            this.chbxOrient_Bottom.UseVisualStyleBackColor = true;
            this.chbxOrient_Bottom.CheckedChanged += new System.EventHandler(this.chbxOrient_CheckedChanged);
            // 
            // chbxOrient_Top
            // 
            resources.ApplyResources(this.chbxOrient_Top, "chbxOrient_Top");
            this.chbxOrient_Top.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.chbxOrient_Top.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gray;
            this.chbxOrient_Top.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.chbxOrient_Top.Name = "chbxOrient_Top";
            this.chbxOrient_Top.UseVisualStyleBackColor = true;
            this.chbxOrient_Top.CheckedChanged += new System.EventHandler(this.chbxOrient_CheckedChanged);
            // 
            // chbxOrient_Auto
            // 
            resources.ApplyResources(this.chbxOrient_Auto, "chbxOrient_Auto");
            this.chbxOrient_Auto.Checked = true;
            this.chbxOrient_Auto.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbxOrient_Auto.Name = "chbxOrient_Auto";
            this.chbxOrient_Auto.UseVisualStyleBackColor = true;
            this.chbxOrient_Auto.CheckedChanged += new System.EventHandler(this.chbxOrient_CheckedChanged);
            // 
            // chbxOrient_Right
            // 
            resources.ApplyResources(this.chbxOrient_Right, "chbxOrient_Right");
            this.chbxOrient_Right.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.chbxOrient_Right.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gray;
            this.chbxOrient_Right.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.chbxOrient_Right.Name = "chbxOrient_Right";
            this.chbxOrient_Right.UseVisualStyleBackColor = true;
            this.chbxOrient_Right.CheckedChanged += new System.EventHandler(this.chbxOrient_CheckedChanged);
            // 
            // chbxOrient_Left
            // 
            resources.ApplyResources(this.chbxOrient_Left, "chbxOrient_Left");
            this.chbxOrient_Left.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.chbxOrient_Left.FlatAppearance.CheckedBackColor = System.Drawing.Color.Gray;
            this.chbxOrient_Left.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.chbxOrient_Left.Name = "chbxOrient_Left";
            this.chbxOrient_Left.UseVisualStyleBackColor = true;
            this.chbxOrient_Left.CheckedChanged += new System.EventHandler(this.chbxOrient_CheckedChanged);
            // 
            // grbxValue
            // 
            resources.ApplyResources(this.grbxValue, "grbxValue");
            this.grbxValue.Controls.Add(this.tlpValue);
            this.grbxValue.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.grbxValue.Name = "grbxValue";
            this.grbxValue.TabStop = false;
            // 
            // tlpValue
            // 
            resources.ApplyResources(this.tlpValue, "tlpValue");
            this.tlpValue.Controls.Add(this.chbxCalcByItem, 0, 3);
            this.tlpValue.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tlpValue.Controls.Add(this.chbxThikness, 0, 2);
            this.tlpValue.Controls.Add(this.tlpTextStyle, 0, 0);
            this.tlpValue.Controls.Add(this.txtThikness, 1, 2);
            this.tlpValue.Controls.Add(this.txtText, 1, 1);
            this.tlpValue.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tlpValue.Name = "tlpValue";
            // 
            // chbxCalcByItem
            // 
            resources.ApplyResources(this.chbxCalcByItem, "chbxCalcByItem");
            this.chbxCalcByItem.Name = "chbxCalcByItem";
            this.chbxCalcByItem.UseVisualStyleBackColor = true;
            this.chbxCalcByItem.CheckedChanged += new System.EventHandler(this.chbxCalcByItem_CheckedChanged);
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.chbxAutoCalc, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.chbxPlusMinus, 1, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // chbxPlusMinus
            // 
            resources.ApplyResources(this.chbxPlusMinus, "chbxPlusMinus");
            this.chbxPlusMinus.BackColor = System.Drawing.Color.Transparent;
            this.chbxPlusMinus.FlatAppearance.BorderSize = 0;
            this.chbxPlusMinus.FlatAppearance.CheckedBackColor = System.Drawing.Color.CornflowerBlue;
            this.chbxPlusMinus.Name = "chbxPlusMinus";
            this.chbxPlusMinus.UseVisualStyleBackColor = false;
            this.chbxPlusMinus.CheckedChanged += new System.EventHandler(this.chbxPlusMinus_CheckedChanged);
            // 
            // chbxThikness
            // 
            resources.ApplyResources(this.chbxThikness, "chbxThikness");
            this.chbxThikness.Name = "chbxThikness";
            this.chbxThikness.UseVisualStyleBackColor = true;
            this.chbxThikness.CheckedChanged += new System.EventHandler(this.chbxItemHeight_CheckedChanged);
            // 
            // tlpTextStyle
            // 
            resources.ApplyResources(this.tlpTextStyle, "tlpTextStyle");
            this.tlpValue.SetColumnSpan(this.tlpTextStyle, 2);
            this.tlpTextStyle.Controls.Add(this.cbxTextStyle, 1, 0);
            this.tlpTextStyle.Controls.Add(this.lbTextStyle, 0, 0);
            this.tlpTextStyle.Name = "tlpTextStyle";
            // 
            // txtThikness
            // 
            resources.ApplyResources(this.txtThikness, "txtThikness");
            this.txtThikness.Name = "txtThikness";
            this.txtThikness.TextChanged += new System.EventHandler(this.txtThikness_TextChanged);
            // 
            // HeightToolControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlpMain);
            this.Name = "HeightToolControl";
            this.tlpMain.ResumeLayout(false);
            this.tlpMain.PerformLayout();
            this.grbxAdditions.ResumeLayout(false);
            this.grbxAdditions.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.grbxType.ResumeLayout(false);
            this.grbxType.PerformLayout();
            this.tlpType.ResumeLayout(false);
            this.tlpType.PerformLayout();
            this.grbxOrientation.ResumeLayout(false);
            this.grbxOrientation.PerformLayout();
            this.tlpOrientation.ResumeLayout(false);
            this.tlpOrientation.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.grbxValue.ResumeLayout(false);
            this.grbxValue.PerformLayout();
            this.tlpValue.ResumeLayout(false);
            this.tlpValue.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tlpTextStyle.ResumeLayout(false);
            this.tlpTextStyle.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.CheckBox chbxAutoCalc;
        public System.Windows.Forms.CheckBox chbxAsLeader;
        private System.Windows.Forms.Label lbTextStyle;
        public System.Windows.Forms.ComboBox cbxTextStyle;
        public System.Windows.Forms.TextBox txtText;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.GroupBox grbxType;
        private System.Windows.Forms.TableLayoutPanel tlpType;
        private System.Windows.Forms.RadioButton rbtnType_Planned;
        private System.Windows.Forms.RadioButton rbtnType_Section;
        private System.Windows.Forms.TableLayoutPanel tlpOrientation;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.CheckBox chbxOrient_Top;
        private System.Windows.Forms.CheckBox chbxOrient_Bottom;
        private System.Windows.Forms.CheckBox chbxOrient_Right;
        private System.Windows.Forms.CheckBox chbxOrient_Left;
        private System.Windows.Forms.CheckBox chbxOrient_Auto;
        private System.Windows.Forms.GroupBox grbxValue;
        private System.Windows.Forms.TableLayoutPanel tlpValue;
        private System.Windows.Forms.GroupBox grbxOrientation;
        private System.Windows.Forms.CheckBox chbxCalcByItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        public System.Windows.Forms.GroupBox grbxAdditions;
        private System.Windows.Forms.CheckBox chbxPlusMinus;
        public System.Windows.Forms.CheckBox chbxSectorArrow;
        public System.Windows.Forms.CheckBox chbxUseFilling;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.CheckBox chbxThikness;
        private System.Windows.Forms.TableLayoutPanel tlpTextStyle;
        private System.Windows.Forms.TextBox txtThikness;
    }
}
