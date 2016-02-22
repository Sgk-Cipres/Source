namespace Orias
{
    partial class ControleOrias
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
            this.components = new System.ComponentModel.Container();
            this.txtBoxCodeOrias = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.viewControleOriasBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.oRIASBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.jazzOrias = new Orias.JazzOrias();
            this.updateCategorieBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.updateCategorieTableAdapter = new Orias.JazzOriasTableAdapters.UpdateCategorieTableAdapter();
            this.tableAdapterManager1 = new Orias.JazzOriasTableAdapters.TableAdapterManager();
            this.updateCategorieDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.insertCategorieBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.insertCategorieTableAdapter = new Orias.JazzOriasTableAdapters.InsertCategorieTableAdapter();
            this.insertCategorieDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BtnMiseAJourJazz = new System.Windows.Forms.Button();
            this.checkBoxMettreAJour = new System.Windows.Forms.CheckBox();
            this.checkBoxInserer = new System.Windows.Forms.CheckBox();
            this.comboBoxEnvironnement = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.toolTipMettreAJourJazz = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.viewControleOriasBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.oRIASBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.jazzOrias)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updateCategorieBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updateCategorieDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.insertCategorieBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.insertCategorieDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // txtBoxCodeOrias
            // 
            this.txtBoxCodeOrias.Font = new System.Drawing.Font("Open Sans", 8.25F);
            this.txtBoxCodeOrias.Location = new System.Drawing.Point(416, 11);
            this.txtBoxCodeOrias.MaxLength = 8;
            this.txtBoxCodeOrias.Name = "txtBoxCodeOrias";
            this.txtBoxCodeOrias.Size = new System.Drawing.Size(311, 22);
            this.txtBoxCodeOrias.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Open Sans", 8.25F);
            this.label1.Location = new System.Drawing.Point(308, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Codes Orias à traiter :";
            // 
            // txtLog
            // 
            this.txtLog.Font = new System.Drawing.Font("Open Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLog.Location = new System.Drawing.Point(5, 66);
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(724, 295);
            this.txtLog.TabIndex = 3;
            this.txtLog.Text = "";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(10, 37);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(717, 23);
            this.progressBar1.TabIndex = 6;
            // 
            // jazzOrias
            // 
            this.jazzOrias.DataSetName = "JazzOrias";
            this.jazzOrias.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // updateCategorieBindingSource
            // 
            this.updateCategorieBindingSource.DataMember = "UpdateCategorie";
            this.updateCategorieBindingSource.DataSource = this.jazzOrias;
            // 
            // updateCategorieTableAdapter
            // 
            this.updateCategorieTableAdapter.ClearBeforeFill = true;
            // 
            // tableAdapterManager1
            // 
            this.tableAdapterManager1.BackupDataSetBeforeUpdate = false;
            this.tableAdapterManager1.Connection = null;
            this.tableAdapterManager1.UpdateOrder = Orias.JazzOriasTableAdapters.TableAdapterManager.UpdateOrderOption.InsertUpdateDelete;
            // 
            // updateCategorieDataGridView
            // 
            this.updateCategorieDataGridView.AutoGenerateColumns = false;
            this.updateCategorieDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.updateCategorieDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4});
            this.updateCategorieDataGridView.DataSource = this.updateCategorieBindingSource;
            this.updateCategorieDataGridView.Location = new System.Drawing.Point(5, 443);
            this.updateCategorieDataGridView.Name = "updateCategorieDataGridView";
            this.updateCategorieDataGridView.Size = new System.Drawing.Size(359, 307);
            this.updateCategorieDataGridView.TabIndex = 9;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "id";
            this.dataGridViewTextBoxColumn3.HeaderText = "id";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "ArgumentValue";
            this.dataGridViewTextBoxColumn4.HeaderText = "ArgumentValue";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            // 
            // insertCategorieBindingSource
            // 
            this.insertCategorieBindingSource.DataMember = "InsertCategorie";
            this.insertCategorieBindingSource.DataSource = this.jazzOrias;
            // 
            // insertCategorieTableAdapter
            // 
            this.insertCategorieTableAdapter.ClearBeforeFill = true;
            // 
            // insertCategorieDataGridView
            // 
            this.insertCategorieDataGridView.AutoGenerateColumns = false;
            this.insertCategorieDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.insertCategorieDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6});
            this.insertCategorieDataGridView.DataSource = this.insertCategorieBindingSource;
            this.insertCategorieDataGridView.Location = new System.Drawing.Point(381, 443);
            this.insertCategorieDataGridView.Name = "insertCategorieDataGridView";
            this.insertCategorieDataGridView.Size = new System.Drawing.Size(346, 307);
            this.insertCategorieDataGridView.TabIndex = 10;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.DataPropertyName = "id";
            this.dataGridViewTextBoxColumn5.HeaderText = "id";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.DataPropertyName = "ArgumentValue";
            this.dataGridViewTextBoxColumn6.HeaderText = "ArgumentValue";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            // 
            // BtnMiseAJourJazz
            // 
            this.BtnMiseAJourJazz.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnMiseAJourJazz.Font = new System.Drawing.Font("Open Sans", 8.25F);
            this.BtnMiseAJourJazz.Location = new System.Drawing.Point(5, 367);
            this.BtnMiseAJourJazz.Name = "BtnMiseAJourJazz";
            this.BtnMiseAJourJazz.Size = new System.Drawing.Size(724, 40);
            this.BtnMiseAJourJazz.TabIndex = 13;
            this.BtnMiseAJourJazz.Text = "Mettre à jour Jazz";
            this.toolTipMettreAJourJazz.SetToolTip(this.BtnMiseAJourJazz, "Pour mettre à jour les codes Orias de Jazz sélectioner l\'environnement cible.\r\nRe" +
        "c");
            this.BtnMiseAJourJazz.UseVisualStyleBackColor = true;
            this.BtnMiseAJourJazz.Click += new System.EventHandler(this.Click_BtnMiseAJourJazz);
            // 
            // checkBoxMettreAJour
            // 
            this.checkBoxMettreAJour.AutoSize = true;
            this.checkBoxMettreAJour.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxMettreAJour.Font = new System.Drawing.Font("Open Sans", 8.25F);
            this.checkBoxMettreAJour.Location = new System.Drawing.Point(5, 413);
            this.checkBoxMettreAJour.Name = "checkBoxMettreAJour";
            this.checkBoxMettreAJour.Size = new System.Drawing.Size(179, 19);
            this.checkBoxMettreAJour.TabIndex = 14;
            this.checkBoxMettreAJour.Text = "Mettre à jour les codes Orias :";
            this.checkBoxMettreAJour.UseVisualStyleBackColor = false;
            // 
            // checkBoxInserer
            // 
            this.checkBoxInserer.AutoSize = true;
            this.checkBoxInserer.Font = new System.Drawing.Font("Open Sans", 8.25F);
            this.checkBoxInserer.Location = new System.Drawing.Point(381, 413);
            this.checkBoxInserer.Name = "checkBoxInserer";
            this.checkBoxInserer.Size = new System.Drawing.Size(248, 19);
            this.checkBoxInserer.TabIndex = 15;
            this.checkBoxInserer.Text = "Inséré les catégories pours les codes Orias :";
            this.checkBoxInserer.UseVisualStyleBackColor = true;
            // 
            // comboBoxEnvironnement
            // 
            this.comboBoxEnvironnement.AutoCompleteCustomSource.AddRange(new string[] {
            "Recette",
            "Pré-Production",
            "Production"});
            this.comboBoxEnvironnement.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comboBoxEnvironnement.Font = new System.Drawing.Font("Open Sans", 8.25F);
            this.comboBoxEnvironnement.FormattingEnabled = true;
            this.comboBoxEnvironnement.Items.AddRange(new object[] {
            "Recette",
            "Pré-Production",
            "Production",
            "Klesia"});
            this.comboBoxEnvironnement.Location = new System.Drawing.Point(112, 8);
            this.comboBoxEnvironnement.Name = "comboBoxEnvironnement";
            this.comboBoxEnvironnement.Size = new System.Drawing.Size(192, 23);
            this.comboBoxEnvironnement.TabIndex = 16;
            this.comboBoxEnvironnement.SelectedIndexChanged += new System.EventHandler(this.comboBoxEnvironnement_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Open Sans", 8.25F);
            this.label2.Location = new System.Drawing.Point(7, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Environnement Jazz  :";
            // 
            // toolTipMettreAJourJazz
            // 
            this.toolTipMettreAJourJazz.IsBalloon = true;
            this.toolTipMettreAJourJazz.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTipMettreAJourJazz.ToolTipTitle = "Mettre à Jour Jazz";
            // 
            // ControleOrias
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(764, 762);
            this.Controls.Add(this.comboBoxEnvironnement);
            this.Controls.Add(this.checkBoxInserer);
            this.Controls.Add(this.checkBoxMettreAJour);
            this.Controls.Add(this.BtnMiseAJourJazz);
            this.Controls.Add(this.insertCategorieDataGridView);
            this.Controls.Add(this.updateCategorieDataGridView);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtBoxCodeOrias);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ControleOrias";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Contrôle Orias";
            this.Load += new System.EventHandler(this.ControleOrias_Load);
            ((System.ComponentModel.ISupportInitialize)(this.viewControleOriasBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.oRIASBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.jazzOrias)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updateCategorieBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updateCategorieDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.insertCategorieBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.insertCategorieDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtBoxCodeOrias;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.BindingSource viewControleOriasBindingSource;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.BindingSource oRIASBindingSource;

        private JazzOrias jazzOrias;
        private System.Windows.Forms.BindingSource updateCategorieBindingSource;
        private JazzOriasTableAdapters.UpdateCategorieTableAdapter updateCategorieTableAdapter;
        private JazzOriasTableAdapters.TableAdapterManager tableAdapterManager1;
        private System.Windows.Forms.DataGridView updateCategorieDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.BindingSource insertCategorieBindingSource;
        private JazzOriasTableAdapters.InsertCategorieTableAdapter insertCategorieTableAdapter;
        private System.Windows.Forms.DataGridView insertCategorieDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.Button BtnMiseAJourJazz;
        private System.Windows.Forms.CheckBox checkBoxMettreAJour;
        private System.Windows.Forms.CheckBox checkBoxInserer;
        private System.Windows.Forms.ComboBox comboBoxEnvironnement;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolTip toolTipMettreAJourJazz;
    }
}

