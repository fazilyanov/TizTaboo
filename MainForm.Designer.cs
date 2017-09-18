namespace TizTaboo {
    partial class MainForm {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.pnl = new System.Windows.Forms.Panel();
            this.tbAlias = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnContextMenu = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.списокПсевдонимовToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.настройкиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.выходToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.синхронизироватьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.btnContextMenu)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnl
            // 
            this.pnl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnl.ForeColor = System.Drawing.Color.White;
            this.pnl.Location = new System.Drawing.Point(0, 23);
            this.pnl.Margin = new System.Windows.Forms.Padding(2);
            this.pnl.Name = "pnl";
            this.pnl.Size = new System.Drawing.Size(736, 434);
            this.pnl.TabIndex = 2;
            // 
            // tbAlias
            // 
            this.tbAlias.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbAlias.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.tbAlias.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbAlias.Font = new System.Drawing.Font("Lucida Console", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbAlias.ForeColor = System.Drawing.Color.White;
            this.tbAlias.Location = new System.Drawing.Point(17, 3);
            this.tbAlias.Margin = new System.Windows.Forms.Padding(2);
            this.tbAlias.Name = "tbAlias";
            this.tbAlias.Size = new System.Drawing.Size(638, 16);
            this.tbAlias.TabIndex = 1;
            this.tbAlias.TabStop = false;
            this.tbAlias.TextChanged += new System.EventHandler(this.tbAlias_TextChanged);
            this.tbAlias.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbAlias_KeyDown);
            this.tbAlias.Leave += new System.EventHandler(this.tbAlias_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(2, 4);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(15, 14);
            this.label1.TabIndex = 4;
            this.label1.Text = ">";
            // 
            // btnContextMenu
            // 
            this.btnContextMenu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnContextMenu.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnContextMenu.BackgroundImage")));
            this.btnContextMenu.Location = new System.Drawing.Point(719, 2);
            this.btnContextMenu.Name = "btnContextMenu";
            this.btnContextMenu.Size = new System.Drawing.Size(16, 16);
            this.btnContextMenu.TabIndex = 7;
            this.btnContextMenu.TabStop = false;
            this.btnContextMenu.Click += new System.EventHandler(this.btnContextMenu_Click);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.синхронизироватьToolStripMenuItem,
            this.списокПсевдонимовToolStripMenuItem,
            this.настройкиToolStripMenuItem,
            this.toolStripMenuItem1,
            this.выходToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.ShowImageMargin = false;
            this.contextMenuStrip.Size = new System.Drawing.Size(168, 120);
            // 
            // списокПсевдонимовToolStripMenuItem
            // 
            this.списокПсевдонимовToolStripMenuItem.Name = "списокПсевдонимовToolStripMenuItem";
            this.списокПсевдонимовToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.списокПсевдонимовToolStripMenuItem.Text = "Список псевдонимов";
            this.списокПсевдонимовToolStripMenuItem.Click += new System.EventHandler(this.списокПсевдонимовToolStripMenuItem_Click);
            // 
            // настройкиToolStripMenuItem
            // 
            this.настройкиToolStripMenuItem.Name = "настройкиToolStripMenuItem";
            this.настройкиToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.настройкиToolStripMenuItem.Text = "Настройки";
            this.настройкиToolStripMenuItem.Click += new System.EventHandler(this.настройкиToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(164, 6);
            // 
            // выходToolStripMenuItem
            // 
            this.выходToolStripMenuItem.Name = "выходToolStripMenuItem";
            this.выходToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.выходToolStripMenuItem.Text = "Выход";
            this.выходToolStripMenuItem.Click += new System.EventHandler(this.выходToolStripMenuItem_Click);
            // 
            // синхронизироватьToolStripMenuItem
            // 
            this.синхронизироватьToolStripMenuItem.Name = "синхронизироватьToolStripMenuItem";
            this.синхронизироватьToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.синхронизироватьToolStripMenuItem.Text = "Синхронизировать";
            this.синхронизироватьToolStripMenuItem.Click += new System.EventHandler(this.синхронизироватьToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(739, 458);
            this.ContextMenuStrip = this.contextMenuStrip;
            this.Controls.Add(this.btnContextMenu);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbAlias);
            this.Controls.Add(this.pnl);
            this.Font = new System.Drawing.Font("Lucida Console", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.ShowInTaskbar = false;
            this.Text = "TizTaboo";
            this.TopMost = true;
            this.Deactivate += new System.EventHandler(this.MainForm_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.btnContextMenu)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnl;
        private System.Windows.Forms.TextBox tbAlias;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox btnContextMenu;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem списокПсевдонимовToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem настройкиToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem выходToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem синхронизироватьToolStripMenuItem;
    }
}

