<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Preferences
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。  
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Preferences))
        Me.LBL_ENCODE = New System.Windows.Forms.Label()
        Me.LBL_ABOUT_SUMMARY = New System.Windows.Forms.Label()
        Me.PIC_ABOUT_BADGE = New System.Windows.Forms.PictureBox()
        Me.TXT_ENCODE = New System.Windows.Forms.TextBox()
        Me.TXT_TRANSCODE = New System.Windows.Forms.TextBox()
        Me.LBL_TRANSCODE = New System.Windows.Forms.Label()
        Me.NUM_THREADS = New System.Windows.Forms.NumericUpDown()
        Me.LBL_THREADS = New System.Windows.Forms.Label()
        Me.LBL_TS_ID = New System.Windows.Forms.Label()
        Me.NUM_TS_ID = New System.Windows.Forms.NumericUpDown()
        Me.LBL_PMT_PID = New System.Windows.Forms.Label()
        Me.NUM_PMT_PID = New System.Windows.Forms.NumericUpDown()
        Me.BTN_HANDLE = New System.Windows.Forms.Button()
        CType(Me.PIC_ABOUT_BADGE, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NUM_THREADS, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NUM_TS_ID, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NUM_PMT_PID, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'LBL_ENCODE
        '
        Me.LBL_ENCODE.AutoSize = True
        Me.LBL_ENCODE.Font = New System.Drawing.Font("微软雅黑", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.LBL_ENCODE.Location = New System.Drawing.Point(9, 142)
        Me.LBL_ENCODE.Name = "LBL_ENCODE"
        Me.LBL_ENCODE.Size = New System.Drawing.Size(90, 21)
        Me.LBL_ENCODE.TabIndex = 401
        Me.LBL_ENCODE.Text = "编码器选项"
        '
        'LBL_ABOUT_SUMMARY
        '
        Me.LBL_ABOUT_SUMMARY.Location = New System.Drawing.Point(69, 11)
        Me.LBL_ABOUT_SUMMARY.Name = "LBL_ABOUT_SUMMARY"
        Me.LBL_ABOUT_SUMMARY.Size = New System.Drawing.Size(543, 71)
        Me.LBL_ABOUT_SUMMARY.TabIndex = 101
        Me.LBL_ABOUT_SUMMARY.Text = "AVS Patrina是一款适用于将MPEG-2传输流中采用AVS1-P16广播视频(AVS+)编码标准的图像转换为MPEG-4 Part 10 (H.264)编" &
    "码标准，并保留MPEG-2传输流中原始音频数据的非实时视频编码转换器。"
        '
        'PIC_ABOUT_BADGE
        '
        Me.PIC_ABOUT_BADGE.Image = CType(resources.GetObject("PIC_ABOUT_BADGE.Image"), System.Drawing.Image)
        Me.PIC_ABOUT_BADGE.Location = New System.Drawing.Point(13, 11)
        Me.PIC_ABOUT_BADGE.Name = "PIC_ABOUT_BADGE"
        Me.PIC_ABOUT_BADGE.Size = New System.Drawing.Size(50, 50)
        Me.PIC_ABOUT_BADGE.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PIC_ABOUT_BADGE.TabIndex = 10013
        Me.PIC_ABOUT_BADGE.TabStop = False
        '
        'TXT_ENCODE
        '
        Me.TXT_ENCODE.Font = New System.Drawing.Font("微软雅黑", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.TXT_ENCODE.Location = New System.Drawing.Point(12, 170)
        Me.TXT_ENCODE.MaxLength = 0
        Me.TXT_ENCODE.Multiline = True
        Me.TXT_ENCODE.Name = "TXT_ENCODE"
        Me.TXT_ENCODE.Size = New System.Drawing.Size(600, 68)
        Me.TXT_ENCODE.TabIndex = 411
        Me.TXT_ENCODE.Text = "-I420"
        '
        'TXT_TRANSCODE
        '
        Me.TXT_TRANSCODE.Font = New System.Drawing.Font("微软雅黑", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.TXT_TRANSCODE.Location = New System.Drawing.Point(12, 282)
        Me.TXT_TRANSCODE.MaxLength = 0
        Me.TXT_TRANSCODE.Multiline = True
        Me.TXT_TRANSCODE.Name = "TXT_TRANSCODE"
        Me.TXT_TRANSCODE.Size = New System.Drawing.Size(600, 68)
        Me.TXT_TRANSCODE.TabIndex = 511
        '
        'LBL_TRANSCODE
        '
        Me.LBL_TRANSCODE.AutoSize = True
        Me.LBL_TRANSCODE.Font = New System.Drawing.Font("微软雅黑", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.LBL_TRANSCODE.Location = New System.Drawing.Point(9, 254)
        Me.LBL_TRANSCODE.Name = "LBL_TRANSCODE"
        Me.LBL_TRANSCODE.Size = New System.Drawing.Size(90, 21)
        Me.LBL_TRANSCODE.TabIndex = 501
        Me.LBL_TRANSCODE.Text = "编码服务器"
        '
        'NUM_THREADS
        '
        Me.NUM_THREADS.Location = New System.Drawing.Point(105, 99)
        Me.NUM_THREADS.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.NUM_THREADS.Name = "NUM_THREADS"
        Me.NUM_THREADS.Size = New System.Drawing.Size(80, 29)
        Me.NUM_THREADS.TabIndex = 211
        Me.NUM_THREADS.Value = New Decimal(New Integer() {10, 0, 0, 0})
        '
        'LBL_THREADS
        '
        Me.LBL_THREADS.AutoSize = True
        Me.LBL_THREADS.Font = New System.Drawing.Font("微软雅黑", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.LBL_THREADS.Location = New System.Drawing.Point(9, 102)
        Me.LBL_THREADS.Name = "LBL_THREADS"
        Me.LBL_THREADS.Size = New System.Drawing.Size(90, 21)
        Me.LBL_THREADS.TabIndex = 201
        Me.LBL_THREADS.Text = "工作线程数"
        '
        'LBL_TS_ID
        '
        Me.LBL_TS_ID.AutoSize = True
        Me.LBL_TS_ID.Font = New System.Drawing.Font("微软雅黑", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.LBL_TS_ID.Location = New System.Drawing.Point(241, 102)
        Me.LBL_TS_ID.Name = "LBL_TS_ID"
        Me.LBL_TS_ID.Size = New System.Drawing.Size(71, 21)
        Me.LBL_TS_ID.TabIndex = 221
        Me.LBL_TS_ID.Text = "TS 流 ID"
        '
        'NUM_TS_ID
        '
        Me.NUM_TS_ID.Location = New System.Drawing.Point(318, 99)
        Me.NUM_TS_ID.Maximum = New Decimal(New Integer() {65535, 0, 0, 0})
        Me.NUM_TS_ID.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.NUM_TS_ID.Name = "NUM_TS_ID"
        Me.NUM_TS_ID.Size = New System.Drawing.Size(80, 29)
        Me.NUM_TS_ID.TabIndex = 231
        Me.NUM_TS_ID.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'LBL_PMT_PID
        '
        Me.LBL_PMT_PID.AutoSize = True
        Me.LBL_PMT_PID.Font = New System.Drawing.Font("微软雅黑", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.LBL_PMT_PID.Location = New System.Drawing.Point(449, 102)
        Me.LBL_PMT_PID.Name = "LBL_PMT_PID"
        Me.LBL_PMT_PID.Size = New System.Drawing.Size(77, 21)
        Me.LBL_PMT_PID.TabIndex = 241
        Me.LBL_PMT_PID.Text = "PMT PID"
        '
        'NUM_PMT_PID
        '
        Me.NUM_PMT_PID.Location = New System.Drawing.Point(532, 99)
        Me.NUM_PMT_PID.Maximum = New Decimal(New Integer() {8190, 0, 0, 0})
        Me.NUM_PMT_PID.Minimum = New Decimal(New Integer() {32, 0, 0, 0})
        Me.NUM_PMT_PID.Name = "NUM_PMT_PID"
        Me.NUM_PMT_PID.Size = New System.Drawing.Size(80, 29)
        Me.NUM_PMT_PID.TabIndex = 251
        Me.NUM_PMT_PID.Value = New Decimal(New Integer() {8190, 0, 0, 0})
        '
        'BTN_HANDLE
        '
        Me.BTN_HANDLE.Font = New System.Drawing.Font("微软雅黑", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.BTN_HANDLE.Location = New System.Drawing.Point(242, 371)
        Me.BTN_HANDLE.Name = "BTN_HANDLE"
        Me.BTN_HANDLE.Size = New System.Drawing.Size(140, 50)
        Me.BTN_HANDLE.TabIndex = 901
        Me.BTN_HANDLE.Text = "保存"
        Me.BTN_HANDLE.UseVisualStyleBackColor = True
        '
        'Preferences
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(10.0!, 21.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(624, 441)
        Me.Controls.Add(Me.BTN_HANDLE)
        Me.Controls.Add(Me.LBL_PMT_PID)
        Me.Controls.Add(Me.NUM_PMT_PID)
        Me.Controls.Add(Me.LBL_TS_ID)
        Me.Controls.Add(Me.NUM_TS_ID)
        Me.Controls.Add(Me.LBL_THREADS)
        Me.Controls.Add(Me.NUM_THREADS)
        Me.Controls.Add(Me.TXT_TRANSCODE)
        Me.Controls.Add(Me.LBL_TRANSCODE)
        Me.Controls.Add(Me.TXT_ENCODE)
        Me.Controls.Add(Me.PIC_ABOUT_BADGE)
        Me.Controls.Add(Me.LBL_ABOUT_SUMMARY)
        Me.Controls.Add(Me.LBL_ENCODE)
        Me.Font = New System.Drawing.Font("微软雅黑", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(5)
        Me.MaximizeBox = False
        Me.Name = "Preferences"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "偏好设置"
        CType(Me.PIC_ABOUT_BADGE, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NUM_THREADS, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NUM_TS_ID, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NUM_PMT_PID, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents LBL_ENCODE As Label
    Friend WithEvents LBL_ABOUT_SUMMARY As Label
    Friend WithEvents PIC_ABOUT_BADGE As PictureBox
    Friend WithEvents TXT_ENCODE As TextBox
    Friend WithEvents TXT_TRANSCODE As TextBox
    Friend WithEvents LBL_TRANSCODE As Label
    Friend WithEvents NUM_THREADS As NumericUpDown
    Friend WithEvents LBL_THREADS As Label
    Friend WithEvents LBL_TS_ID As Label
    Friend WithEvents NUM_TS_ID As NumericUpDown
    Friend WithEvents LBL_PMT_PID As Label
    Friend WithEvents NUM_PMT_PID As NumericUpDown
    Friend WithEvents BTN_HANDLE As Button
End Class
