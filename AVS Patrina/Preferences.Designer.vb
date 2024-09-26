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
        Me.LBL_ABOUT_SUMMARY = New System.Windows.Forms.Label()
        Me.PIC_ABOUT_BADGE = New System.Windows.Forms.PictureBox()
        Me.NUM_THREADS = New System.Windows.Forms.NumericUpDown()
        Me.LBL_THREADS = New System.Windows.Forms.Label()
        Me.LBL_TS_ID = New System.Windows.Forms.Label()
        Me.NUM_TS_ID = New System.Windows.Forms.NumericUpDown()
        Me.LBL_PMT_PID = New System.Windows.Forms.Label()
        Me.NUM_PMT_PID = New System.Windows.Forms.NumericUpDown()
        Me.BTN_HANDLE = New System.Windows.Forms.Button()
        Me.LBL_PCR_OFFSET = New System.Windows.Forms.Label()
        Me.NUM_PCR_OFFSET = New System.Windows.Forms.NumericUpDown()
        Me.LBL_PTS_DELAY = New System.Windows.Forms.Label()
        Me.NUM_PTS_DELAY = New System.Windows.Forms.NumericUpDown()
        Me.LBL_GOP_MIN = New System.Windows.Forms.Label()
        Me.NUM_GOP_MIN = New System.Windows.Forms.NumericUpDown()
        CType(Me.PIC_ABOUT_BADGE, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NUM_THREADS, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NUM_TS_ID, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NUM_PMT_PID, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NUM_PCR_OFFSET, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NUM_PTS_DELAY, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NUM_GOP_MIN, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'LBL_ABOUT_SUMMARY
        '
        Me.LBL_ABOUT_SUMMARY.Location = New System.Drawing.Point(100, 12)
        Me.LBL_ABOUT_SUMMARY.Name = "LBL_ABOUT_SUMMARY"
        Me.LBL_ABOUT_SUMMARY.Size = New System.Drawing.Size(400, 90)
        Me.LBL_ABOUT_SUMMARY.TabIndex = 101
        Me.LBL_ABOUT_SUMMARY.Text = "AVS Patrina 是一款适用于将 MPEG-2 传输流中采用 AVS1-P16 广播视频 (AVS+) 编码标准的图像转换为 MPEG-4 Part 10 " &
    "(H.264) 编码标准，并保留 MPEG-2 传输流中原始音频数据的非实时视频编码转换器。"
        '
        'PIC_ABOUT_BADGE
        '
        Me.PIC_ABOUT_BADGE.Image = CType(resources.GetObject("PIC_ABOUT_BADGE.Image"), System.Drawing.Image)
        Me.PIC_ABOUT_BADGE.Location = New System.Drawing.Point(12, 18)
        Me.PIC_ABOUT_BADGE.Name = "PIC_ABOUT_BADGE"
        Me.PIC_ABOUT_BADGE.Size = New System.Drawing.Size(75, 75)
        Me.PIC_ABOUT_BADGE.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PIC_ABOUT_BADGE.TabIndex = 10013
        Me.PIC_ABOUT_BADGE.TabStop = False
        '
        'NUM_THREADS
        '
        Me.NUM_THREADS.Location = New System.Drawing.Point(104, 111)
        Me.NUM_THREADS.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
        Me.NUM_THREADS.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.NUM_THREADS.Name = "NUM_THREADS"
        Me.NUM_THREADS.Size = New System.Drawing.Size(70, 29)
        Me.NUM_THREADS.TabIndex = 211
        Me.NUM_THREADS.Value = New Decimal(New Integer() {10, 0, 0, 0})
        '
        'LBL_THREADS
        '
        Me.LBL_THREADS.AutoSize = True
        Me.LBL_THREADS.Font = New System.Drawing.Font("微软雅黑", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.LBL_THREADS.Location = New System.Drawing.Point(8, 114)
        Me.LBL_THREADS.Name = "LBL_THREADS"
        Me.LBL_THREADS.Size = New System.Drawing.Size(90, 21)
        Me.LBL_THREADS.TabIndex = 201
        Me.LBL_THREADS.Text = "工作线程数"
        '
        'LBL_TS_ID
        '
        Me.LBL_TS_ID.AutoSize = True
        Me.LBL_TS_ID.Font = New System.Drawing.Font("微软雅黑", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.LBL_TS_ID.Location = New System.Drawing.Point(192, 114)
        Me.LBL_TS_ID.Name = "LBL_TS_ID"
        Me.LBL_TS_ID.Size = New System.Drawing.Size(71, 21)
        Me.LBL_TS_ID.TabIndex = 221
        Me.LBL_TS_ID.Text = "TS 流 ID"
        '
        'NUM_TS_ID
        '
        Me.NUM_TS_ID.Location = New System.Drawing.Point(269, 111)
        Me.NUM_TS_ID.Maximum = New Decimal(New Integer() {65535, 0, 0, 0})
        Me.NUM_TS_ID.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.NUM_TS_ID.Name = "NUM_TS_ID"
        Me.NUM_TS_ID.Size = New System.Drawing.Size(75, 29)
        Me.NUM_TS_ID.TabIndex = 231
        Me.NUM_TS_ID.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'LBL_PMT_PID
        '
        Me.LBL_PMT_PID.AutoSize = True
        Me.LBL_PMT_PID.Font = New System.Drawing.Font("微软雅黑", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.LBL_PMT_PID.Location = New System.Drawing.Point(359, 114)
        Me.LBL_PMT_PID.Name = "LBL_PMT_PID"
        Me.LBL_PMT_PID.Size = New System.Drawing.Size(77, 21)
        Me.LBL_PMT_PID.TabIndex = 241
        Me.LBL_PMT_PID.Text = "PMT PID"
        '
        'NUM_PMT_PID
        '
        Me.NUM_PMT_PID.Location = New System.Drawing.Point(442, 111)
        Me.NUM_PMT_PID.Maximum = New Decimal(New Integer() {8190, 0, 0, 0})
        Me.NUM_PMT_PID.Minimum = New Decimal(New Integer() {32, 0, 0, 0})
        Me.NUM_PMT_PID.Name = "NUM_PMT_PID"
        Me.NUM_PMT_PID.Size = New System.Drawing.Size(70, 29)
        Me.NUM_PMT_PID.TabIndex = 251
        Me.NUM_PMT_PID.Value = New Decimal(New Integer() {32, 0, 0, 0})
        '
        'BTN_HANDLE
        '
        Me.BTN_HANDLE.Font = New System.Drawing.Font("微软雅黑", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.BTN_HANDLE.Location = New System.Drawing.Point(192, 192)
        Me.BTN_HANDLE.Name = "BTN_HANDLE"
        Me.BTN_HANDLE.Size = New System.Drawing.Size(140, 50)
        Me.BTN_HANDLE.TabIndex = 901
        Me.BTN_HANDLE.Text = "保存"
        Me.BTN_HANDLE.UseVisualStyleBackColor = True
        '
        'LBL_PCR_OFFSET
        '
        Me.LBL_PCR_OFFSET.AutoSize = True
        Me.LBL_PCR_OFFSET.Font = New System.Drawing.Font("微软雅黑", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.LBL_PCR_OFFSET.Location = New System.Drawing.Point(8, 149)
        Me.LBL_PCR_OFFSET.Name = "LBL_PCR_OFFSET"
        Me.LBL_PCR_OFFSET.Size = New System.Drawing.Size(89, 21)
        Me.LBL_PCR_OFFSET.TabIndex = 301
        Me.LBL_PCR_OFFSET.Text = "PCR偏移帧"
        '
        'NUM_PCR_OFFSET
        '
        Me.NUM_PCR_OFFSET.Location = New System.Drawing.Point(104, 146)
        Me.NUM_PCR_OFFSET.Maximum = New Decimal(New Integer() {250, 0, 0, 0})
        Me.NUM_PCR_OFFSET.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.NUM_PCR_OFFSET.Name = "NUM_PCR_OFFSET"
        Me.NUM_PCR_OFFSET.Size = New System.Drawing.Size(70, 29)
        Me.NUM_PCR_OFFSET.TabIndex = 311
        Me.NUM_PCR_OFFSET.Value = New Decimal(New Integer() {8, 0, 0, 0})
        '
        'LBL_PTS_DELAY
        '
        Me.LBL_PTS_DELAY.AutoSize = True
        Me.LBL_PTS_DELAY.Font = New System.Drawing.Font("微软雅黑", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.LBL_PTS_DELAY.Location = New System.Drawing.Point(192, 149)
        Me.LBL_PTS_DELAY.Name = "LBL_PTS_DELAY"
        Me.LBL_PTS_DELAY.Size = New System.Drawing.Size(70, 21)
        Me.LBL_PTS_DELAY.TabIndex = 321
        Me.LBL_PTS_DELAY.Text = "PTS延迟"
        '
        'NUM_PTS_DELAY
        '
        Me.NUM_PTS_DELAY.Increment = New Decimal(New Integer() {3600, 0, 0, 0})
        Me.NUM_PTS_DELAY.Location = New System.Drawing.Point(269, 146)
        Me.NUM_PTS_DELAY.Maximum = New Decimal(New Integer() {900000, 0, 0, 0})
        Me.NUM_PTS_DELAY.Minimum = New Decimal(New Integer() {900000, 0, 0, -2147483648})
        Me.NUM_PTS_DELAY.Name = "NUM_PTS_DELAY"
        Me.NUM_PTS_DELAY.Size = New System.Drawing.Size(75, 29)
        Me.NUM_PTS_DELAY.TabIndex = 331
        '
        'LBL_GOP_MIN
        '
        Me.LBL_GOP_MIN.AutoSize = True
        Me.LBL_GOP_MIN.Font = New System.Drawing.Font("微软雅黑", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.LBL_GOP_MIN.Location = New System.Drawing.Point(359, 149)
        Me.LBL_GOP_MIN.Name = "LBL_GOP_MIN"
        Me.LBL_GOP_MIN.Size = New System.Drawing.Size(77, 21)
        Me.LBL_GOP_MIN.TabIndex = 341
        Me.LBL_GOP_MIN.Text = "最小GOP"
        '
        'NUM_GOP_MIN
        '
        Me.NUM_GOP_MIN.Location = New System.Drawing.Point(442, 146)
        Me.NUM_GOP_MIN.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
        Me.NUM_GOP_MIN.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.NUM_GOP_MIN.Name = "NUM_GOP_MIN"
        Me.NUM_GOP_MIN.Size = New System.Drawing.Size(70, 29)
        Me.NUM_GOP_MIN.TabIndex = 351
        Me.NUM_GOP_MIN.Value = New Decimal(New Integer() {25, 0, 0, 0})
        '
        'Preferences
        '
        Me.AcceptButton = Me.BTN_HANDLE
        Me.AutoScaleDimensions = New System.Drawing.SizeF(10.0!, 21.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(524, 261)
        Me.Controls.Add(Me.LBL_GOP_MIN)
        Me.Controls.Add(Me.NUM_GOP_MIN)
        Me.Controls.Add(Me.LBL_PTS_DELAY)
        Me.Controls.Add(Me.NUM_PTS_DELAY)
        Me.Controls.Add(Me.LBL_PCR_OFFSET)
        Me.Controls.Add(Me.NUM_PCR_OFFSET)
        Me.Controls.Add(Me.BTN_HANDLE)
        Me.Controls.Add(Me.LBL_PMT_PID)
        Me.Controls.Add(Me.NUM_PMT_PID)
        Me.Controls.Add(Me.LBL_TS_ID)
        Me.Controls.Add(Me.NUM_TS_ID)
        Me.Controls.Add(Me.LBL_THREADS)
        Me.Controls.Add(Me.NUM_THREADS)
        Me.Controls.Add(Me.PIC_ABOUT_BADGE)
        Me.Controls.Add(Me.LBL_ABOUT_SUMMARY)
        Me.Font = New System.Drawing.Font("微软雅黑", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(5)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Preferences"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "偏好设置"
        CType(Me.PIC_ABOUT_BADGE, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NUM_THREADS, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NUM_TS_ID, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NUM_PMT_PID, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NUM_PCR_OFFSET, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NUM_PTS_DELAY, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NUM_GOP_MIN, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents LBL_ABOUT_SUMMARY As Label
    Friend WithEvents PIC_ABOUT_BADGE As PictureBox
    Friend WithEvents NUM_THREADS As NumericUpDown
    Friend WithEvents LBL_THREADS As Label
    Friend WithEvents LBL_TS_ID As Label
    Friend WithEvents NUM_TS_ID As NumericUpDown
    Friend WithEvents LBL_PMT_PID As Label
    Friend WithEvents NUM_PMT_PID As NumericUpDown
    Friend WithEvents BTN_HANDLE As Button
    Friend WithEvents LBL_PCR_OFFSET As Label
    Friend WithEvents NUM_PCR_OFFSET As NumericUpDown
    Friend WithEvents LBL_PTS_DELAY As Label
    Friend WithEvents NUM_PTS_DELAY As NumericUpDown
    Friend WithEvents LBL_GOP_MIN As Label
    Friend WithEvents NUM_GOP_MIN As NumericUpDown
End Class
