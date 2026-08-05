using BlubLib.GUI.Controls.Extended;

namespace BlubLib.GUI.Controls
{
    partial class CreditBox
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
            this._moduleLabel = new System.Windows.Forms.Label();
            this._websiteLabel = new BlubLib.GUI.Controls.Extended.LinkLabelEx();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // _moduleLabel
            // 
            this._moduleLabel.AutoSize = true;
            this._moduleLabel.BackColor = System.Drawing.Color.Transparent;
            this._moduleLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._moduleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(84)))), ((int)(((byte)(84)))));
            this._moduleLabel.Location = new System.Drawing.Point(10, 10);
            this._moduleLabel.Name = "_moduleLabel";
            this._moduleLabel.Size = new System.Drawing.Size(90, 13);
            this._moduleLabel.TabIndex = 4;
            this._moduleLabel.Text = "AnimeWatch by";
            // 
            // _websiteLabel
            // 
            this._websiteLabel.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this._websiteLabel.AutoSize = true;
            this._websiteLabel.BackColor = System.Drawing.Color.Transparent;
            this._websiteLabel.Image = null;
            this._websiteLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._websiteLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this._websiteLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this._websiteLabel.HoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this._websiteLabel.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this._websiteLabel.Location = new System.Drawing.Point(50, 28);
            this._websiteLabel.Name = "_websiteLabel";
            this._websiteLabel.Size = new System.Drawing.Size(48, 15);
            this._websiteLabel.TabIndex = 6;
            this._websiteLabel.TabStop = true;
            this._websiteLabel.Text = "wtfblub";
            this._websiteLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this._websiteLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.websiteLabel_LinkClicked);
            // 
            // toolTip
            // 
            this.toolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip.ToolTipTitle = "Website";
            // 
            // CreditBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._websiteLabel);
            this.Controls.Add(this._moduleLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "CreditBox";
            this.Size = new System.Drawing.Size(165, 50);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _moduleLabel;
        private LinkLabelEx _websiteLabel;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
