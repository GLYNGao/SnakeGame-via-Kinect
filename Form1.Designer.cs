namespace SnakeGame
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cbNearMode = new System.Windows.Forms.CheckBox();
            this.imageBoxSkeleton = new Emgu.CV.UI.ImageBox();
            this.cbSeat = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.imageBoxSkeleton)).BeginInit();
            this.SuspendLayout();
            // 
            // cbNearMode
            // 
            this.cbNearMode.AutoSize = true;
            this.cbNearMode.Location = new System.Drawing.Point(12, 276);
            this.cbNearMode.Name = "cbNearMode";
            this.cbNearMode.Size = new System.Drawing.Size(60, 16);
            this.cbNearMode.TabIndex = 4;
            this.cbNearMode.Text = "近模式";
            this.cbNearMode.UseVisualStyleBackColor = true;
            this.cbNearMode.CheckedChanged += new System.EventHandler(this.cbNearMode_CheckedChanged);
            // 
            // imageBoxSkeleton
            // 
            this.imageBoxSkeleton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imageBoxSkeleton.Location = new System.Drawing.Point(29, 12);
            this.imageBoxSkeleton.Name = "imageBoxSkeleton";
            this.imageBoxSkeleton.Size = new System.Drawing.Size(293, 258);
            this.imageBoxSkeleton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imageBoxSkeleton.TabIndex = 5;
            this.imageBoxSkeleton.TabStop = false;
            // 
            // cbSeat
            // 
            this.cbSeat.AutoSize = true;
            this.cbSeat.Location = new System.Drawing.Point(121, 276);
            this.cbSeat.Name = "cbSeat";
            this.cbSeat.Size = new System.Drawing.Size(48, 16);
            this.cbSeat.TabIndex = 6;
            this.cbSeat.Text = "坐姿";
            this.cbSeat.UseVisualStyleBackColor = true;
            this.cbSeat.CheckedChanged += new System.EventHandler(this.cbSeat_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(346, 302);
            this.Controls.Add(this.cbSeat);
            this.Controls.Add(this.imageBoxSkeleton);
            this.Controls.Add(this.cbNearMode);
            this.Name = "骨骼图";
            this.Text = "骨骼图";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imageBoxSkeleton)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbNearMode;
        private Emgu.CV.UI.ImageBox imageBoxSkeleton;
        private System.Windows.Forms.CheckBox cbSeat;
    }
}

