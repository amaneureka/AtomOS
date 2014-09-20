namespace Debug_Window
{
    partial class MainFrm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbl_ram = new System.Windows.Forms.Label();
            this.lbl_1 = new System.Windows.Forms.Label();
            this.RAMMeter = new System.Windows.Forms.ProgressBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lbl_System = new System.Windows.Forms.Label();
            this.lbl_03 = new System.Windows.Forms.Label();
            this.lbl_connection = new System.Windows.Forms.Label();
            this.lbl_02 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbl_ram);
            this.groupBox1.Controls.Add(this.lbl_1);
            this.groupBox1.Controls.Add(this.RAMMeter);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(415, 300);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Memory Window";
            // 
            // lbl_ram
            // 
            this.lbl_ram.AutoSize = true;
            this.lbl_ram.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_ram.Location = new System.Drawing.Point(232, 30);
            this.lbl_ram.Name = "lbl_ram";
            this.lbl_ram.Size = new System.Drawing.Size(67, 17);
            this.lbl_ram.TabIndex = 2;
            this.lbl_ram.Text = "0/0 Bytes";
            // 
            // lbl_1
            // 
            this.lbl_1.AutoSize = true;
            this.lbl_1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_1.Location = new System.Drawing.Point(18, 30);
            this.lbl_1.Name = "lbl_1";
            this.lbl_1.Size = new System.Drawing.Size(98, 17);
            this.lbl_1.TabIndex = 1;
            this.lbl_1.Text = "Physical RAM:";
            // 
            // RAMMeter
            // 
            this.RAMMeter.Location = new System.Drawing.Point(21, 50);
            this.RAMMeter.Name = "RAMMeter";
            this.RAMMeter.Size = new System.Drawing.Size(370, 42);
            this.RAMMeter.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lbl_System);
            this.groupBox2.Controls.Add(this.lbl_03);
            this.groupBox2.Controls.Add(this.lbl_connection);
            this.groupBox2.Controls.Add(this.lbl_02);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(433, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(171, 113);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Status";
            // 
            // lbl_System
            // 
            this.lbl_System.AutoSize = true;
            this.lbl_System.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_System.ForeColor = System.Drawing.Color.Red;
            this.lbl_System.Location = new System.Drawing.Point(107, 52);
            this.lbl_System.Name = "lbl_System";
            this.lbl_System.Size = new System.Drawing.Size(31, 17);
            this.lbl_System.TabIndex = 5;
            this.lbl_System.Text = "N/A";
            // 
            // lbl_03
            // 
            this.lbl_03.AutoSize = true;
            this.lbl_03.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_03.Location = new System.Drawing.Point(18, 52);
            this.lbl_03.Name = "lbl_03";
            this.lbl_03.Size = new System.Drawing.Size(86, 17);
            this.lbl_03.TabIndex = 4;
            this.lbl_03.Text = "System       :";
            // 
            // lbl_connection
            // 
            this.lbl_connection.AutoSize = true;
            this.lbl_connection.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_connection.ForeColor = System.Drawing.Color.Red;
            this.lbl_connection.Location = new System.Drawing.Point(107, 30);
            this.lbl_connection.Name = "lbl_connection";
            this.lbl_connection.Size = new System.Drawing.Size(46, 17);
            this.lbl_connection.TabIndex = 3;
            this.lbl_connection.Text = "Failed";
            // 
            // lbl_02
            // 
            this.lbl_02.AutoSize = true;
            this.lbl_02.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_02.Location = new System.Drawing.Point(18, 30);
            this.lbl_02.Name = "lbl_02";
            this.lbl_02.Size = new System.Drawing.Size(87, 17);
            this.lbl_02.TabIndex = 2;
            this.lbl_02.Text = "Connection :";
            // 
            // MainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(616, 421);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "MainFrm";
            this.Text = "Debugging Window";
            this.Load += new System.EventHandler(this.MainFrm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ProgressBar RAMMeter;
        private System.Windows.Forms.Label lbl_ram;
        private System.Windows.Forms.Label lbl_1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lbl_03;
        private System.Windows.Forms.Label lbl_connection;
        private System.Windows.Forms.Label lbl_02;
        private System.Windows.Forms.Label lbl_System;
    }
}

