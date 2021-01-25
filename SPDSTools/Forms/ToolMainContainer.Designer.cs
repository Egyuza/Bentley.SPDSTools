namespace SPDSTools
{
    partial class ToolMainContainer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToolMainContainer));
            this.tlpContainer = new System.Windows.Forms.TableLayoutPanel();
            this.flpMenu = new System.Windows.Forms.FlowLayoutPanel();
            this.chbxDraw = new System.Windows.Forms.CheckBox();
            this.chbxLocateDraw = new System.Windows.Forms.CheckBox();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.tlpApplyReset = new System.Windows.Forms.TableLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.tlpSelection = new System.Windows.Forms.TableLayoutPanel();
            this.lbSelCount = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.markToInsert = new System.Windows.Forms.Label();
            this.pnlProgress = new System.Windows.Forms.Panel();
            this.progBar = new System.Windows.Forms.ProgressBar();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tlpContainer.SuspendLayout();
            this.flpMenu.SuspendLayout();
            this.tlpApplyReset.SuspendLayout();
            this.tlpSelection.SuspendLayout();
            this.pnlProgress.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpContainer
            // 
            resources.ApplyResources(this.tlpContainer, "tlpContainer");
            this.tlpContainer.Controls.Add(this.flpMenu, 0, 0);
            this.tlpContainer.Controls.Add(this.tlpApplyReset, 0, 4);
            this.tlpContainer.Controls.Add(this.tlpSelection, 0, 3);
            this.tlpContainer.Controls.Add(this.markToInsert, 0, 2);
            this.tlpContainer.Controls.Add(this.pnlProgress, 0, 1);
            this.tlpContainer.Name = "tlpContainer";
            // 
            // flpMenu
            // 
            resources.ApplyResources(this.flpMenu, "flpMenu");
            this.flpMenu.Controls.Add(this.chbxDraw);
            this.flpMenu.Controls.Add(this.chbxLocateDraw);
            this.flpMenu.Controls.Add(this.btnUpdate);
            this.flpMenu.Name = "flpMenu";
            // 
            // chbxDraw
            // 
            resources.ApplyResources(this.chbxDraw, "chbxDraw");
            this.chbxDraw.BackColor = System.Drawing.Color.Transparent;
            this.chbxDraw.BackgroundImage = global::SPDSTools.Properties.Resources.icons8_compass_48;
            this.chbxDraw.FlatAppearance.BorderSize = 0;
            this.chbxDraw.FlatAppearance.CheckedBackColor = System.Drawing.Color.CornflowerBlue;
            this.chbxDraw.Name = "chbxDraw";
            this.toolTip1.SetToolTip(this.chbxDraw, resources.GetString("chbxDraw.ToolTip"));
            this.chbxDraw.UseVisualStyleBackColor = false;
            this.chbxDraw.CheckedChanged += new System.EventHandler(this.chbxDraw_CheckedChanged);
            // 
            // chbxLocateDraw
            // 
            resources.ApplyResources(this.chbxLocateDraw, "chbxLocateDraw");
            this.chbxLocateDraw.BackColor = System.Drawing.Color.Transparent;
            this.chbxLocateDraw.BackgroundImage = global::SPDSTools.Properties.Resources.icons8_hunt_30;
            this.chbxLocateDraw.FlatAppearance.BorderSize = 0;
            this.chbxLocateDraw.FlatAppearance.CheckedBackColor = System.Drawing.Color.CornflowerBlue;
            this.chbxLocateDraw.Name = "chbxLocateDraw";
            this.toolTip1.SetToolTip(this.chbxLocateDraw, resources.GetString("chbxLocateDraw.ToolTip"));
            this.chbxLocateDraw.UseVisualStyleBackColor = false;
            this.chbxLocateDraw.CheckedChanged += new System.EventHandler(this.chbxLocateDraw_CheckedChanged);
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.Color.Transparent;
            this.btnUpdate.BackgroundImage = global::SPDSTools.Properties.Resources.icons8_available_updates_48;
            resources.ApplyResources(this.btnUpdate, "btnUpdate");
            this.btnUpdate.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.btnUpdate.FlatAppearance.BorderSize = 0;
            this.btnUpdate.Name = "btnUpdate";
            this.toolTip1.SetToolTip(this.btnUpdate, resources.GetString("btnUpdate.ToolTip"));
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // tlpApplyReset
            // 
            resources.ApplyResources(this.tlpApplyReset, "tlpApplyReset");
            this.tlpApplyReset.Controls.Add(this.btnCancel, 1, 0);
            this.tlpApplyReset.Controls.Add(this.btnApply, 0, 0);
            this.tlpApplyReset.Name = "tlpApplyReset";
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnCancel.Name = "btnCancel";
            this.toolTip1.SetToolTip(this.btnCancel, resources.GetString("btnCancel.ToolTip"));
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnApply
            // 
            resources.ApplyResources(this.btnApply, "btnApply");
            this.btnApply.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnApply.Name = "btnApply";
            this.toolTip1.SetToolTip(this.btnApply, resources.GetString("btnApply.ToolTip"));
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // tlpSelection
            // 
            resources.ApplyResources(this.tlpSelection, "tlpSelection");
            this.tlpSelection.Controls.Add(this.lbSelCount, 1, 0);
            this.tlpSelection.Controls.Add(this.label1, 0, 0);
            this.tlpSelection.Name = "tlpSelection";
            // 
            // lbSelCount
            // 
            resources.ApplyResources(this.lbSelCount, "lbSelCount");
            this.lbSelCount.Name = "lbSelCount";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // markToInsert
            // 
            resources.ApplyResources(this.markToInsert, "markToInsert");
            this.markToInsert.Name = "markToInsert";
            // 
            // pnlProgress
            // 
            resources.ApplyResources(this.pnlProgress, "pnlProgress");
            this.pnlProgress.Controls.Add(this.progBar);
            this.pnlProgress.Name = "pnlProgress";
            // 
            // progBar
            // 
            this.progBar.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.progBar, "progBar");
            this.progBar.ForeColor = System.Drawing.Color.CornflowerBlue;
            this.progBar.Name = "progBar";
            // 
            // toolTip1
            // 
            this.toolTip1.ShowAlways = true;
            // 
            // ToolMainContainer
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.tlpContainer);
            this.Name = "ToolMainContainer";
            this.DockChanged += new System.EventHandler(this.ToolMainContainer_DockChanged);
            this.tlpContainer.ResumeLayout(false);
            this.tlpContainer.PerformLayout();
            this.flpMenu.ResumeLayout(false);
            this.tlpApplyReset.ResumeLayout(false);
            this.tlpApplyReset.PerformLayout();
            this.tlpSelection.ResumeLayout(false);
            this.tlpSelection.PerformLayout();
            this.pnlProgress.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpContainer;
        private System.Windows.Forms.FlowLayoutPanel flpMenu;
        private System.Windows.Forms.TableLayoutPanel tlpApplyReset;
        public System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.Button btnApply;
        public System.Windows.Forms.Button btnUpdate;
        public System.Windows.Forms.CheckBox chbxDraw;
        public System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TableLayoutPanel tlpSelection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbSelCount;
        private System.Windows.Forms.ProgressBar progBar;
        private System.Windows.Forms.Label markToInsert;
        private System.Windows.Forms.Panel pnlProgress;
        public System.Windows.Forms.CheckBox chbxLocateDraw;
    }
}
