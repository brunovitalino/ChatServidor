namespace ChatServidor.VIEW
{
    partial class frmServidor
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.mskIp = new System.Windows.Forms.MaskedTextBox();
            this.btnConectar = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Black;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP:";
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Location = new System.Drawing.Point(12, 68);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(307, 220);
            this.txtLog.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Black;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(12, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Status";
            // 
            // mskIp
            // 
            this.mskIp.BeepOnError = true;
            this.mskIp.HidePromptOnLeave = true;
            this.mskIp.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this.mskIp.Location = new System.Drawing.Point(56, 12);
            this.mskIp.Mask = "099\\.099\\.099\\.099";
            this.mskIp.Name = "mskIp";
            this.mskIp.Size = new System.Drawing.Size(140, 20);
            this.mskIp.TabIndex = 1;
            this.mskIp.Text = "1270  0  1";
            this.mskIp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.mskIp, "Digite o endereço IP no qual o servidor ficará hospedado.");
            this.mskIp.MaskInputRejected += new System.Windows.Forms.MaskInputRejectedEventHandler(this.mskIp_MaskInputRejected);
            this.mskIp.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mskIp_KeyDown);
            this.mskIp.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.mskIp_KeyPress);
            this.mskIp.MouseHover += new System.EventHandler(this.mskIp_MouseHover);
            // 
            // btnConectar
            // 
            this.btnConectar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConectar.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnConectar.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.btnConectar.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.btnConectar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConectar.ForeColor = System.Drawing.Color.White;
            this.btnConectar.Location = new System.Drawing.Point(244, 10);
            this.btnConectar.Name = "btnConectar";
            this.btnConectar.Size = new System.Drawing.Size(75, 23);
            this.btnConectar.TabIndex = 2;
            this.btnConectar.Text = "Conectar";
            this.btnConectar.UseVisualStyleBackColor = false;
            this.btnConectar.Click += new System.EventHandler(this.btnConectar_Click);
            // 
            // frmServidor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(331, 300);
            this.Controls.Add(this.btnConectar);
            this.Controls.Add(this.mskIp);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.label1);
            this.Name = "frmServidor";
            this.Text = "Servidor";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmServidor_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.MaskedTextBox mskIp;
        private System.Windows.Forms.Button btnConectar;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

