<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainUI
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainUI))
        Me.BGW_PROCESS = New System.ComponentModel.BackgroundWorker()
        Me.OFD_INPUT_SOURCE_FILE = New System.Windows.Forms.OpenFileDialog()
        Me.SFD_OUTPUT_DEST_FILE = New System.Windows.Forms.SaveFileDialog()
        Me.BTN_OUTPUT_BROWSE = New System.Windows.Forms.Button()
        Me.LBL_OUTPUT_DEST_FILE = New System.Windows.Forms.Label()
        Me.TXT_OUTPUT_DEST_FILE = New System.Windows.Forms.TextBox()
        Me.TXT_INPUT_SOURCE_FILE = New System.Windows.Forms.TextBox()
        Me.BTN_INPUT_BROWSE = New System.Windows.Forms.Button()
        Me.LBL_INPUT_SOURCE_FILE = New System.Windows.Forms.Label()
        Me.LBL_ABOUT_TITLE = New System.Windows.Forms.Label()
        Me.PIC_ABOUT_BADGE = New System.Windows.Forms.PictureBox()
        Me.BTN_HANDLE = New System.Windows.Forms.Button()
        Me.PGB_OUTPUT_PROGRESS = New System.Windows.Forms.ProgressBar()
        Me.BTN_NEW_TASK = New System.Windows.Forms.Button()
        Me.BTN_MENU = New System.Windows.Forms.Button()
        Me.CMS_MENU = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.TSI_WIPE_CACHE = New System.Windows.Forms.ToolStripMenuItem()
        Me.TSS_MENU = New System.Windows.Forms.ToolStripSeparator()
        Me.TSI_OPTIONS = New System.Windows.Forms.ToolStripMenuItem()
        Me.TSI_ABOUT = New System.Windows.Forms.ToolStripMenuItem()
        Me.CBO_PRESET = New System.Windows.Forms.ComboBox()
        CType(Me.PIC_ABOUT_BADGE, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.CMS_MENU.SuspendLayout()
        Me.SuspendLayout()
        '
        'BGW_PROCESS
        '
        Me.BGW_PROCESS.WorkerReportsProgress = True
        '
        'OFD_INPUT_SOURCE_FILE
        '
        Me.OFD_INPUT_SOURCE_FILE.Filter = "TS 传输流|*.ts|所有文件|*.*"
        '
        'SFD_OUTPUT_DEST_FILE
        '
        Me.SFD_OUTPUT_DEST_FILE.Filter = "TS 传输流|*.ts|所有文件|*.*"
        '
        'BTN_OUTPUT_BROWSE
        '
        Me.BTN_OUTPUT_BROWSE.Location = New System.Drawing.Point(478, 93)
        Me.BTN_OUTPUT_BROWSE.Name = "BTN_OUTPUT_BROWSE"
        Me.BTN_OUTPUT_BROWSE.Size = New System.Drawing.Size(35, 31)
        Me.BTN_OUTPUT_BROWSE.TabIndex = 421
        Me.BTN_OUTPUT_BROWSE.Text = "..."
        Me.BTN_OUTPUT_BROWSE.UseVisualStyleBackColor = True
        '
        'LBL_OUTPUT_DEST_FILE
        '
        Me.LBL_OUTPUT_DEST_FILE.AutoSize = True
        Me.LBL_OUTPUT_DEST_FILE.Font = New System.Drawing.Font("微软雅黑", 12.0!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.LBL_OUTPUT_DEST_FILE.Location = New System.Drawing.Point(100, 98)
        Me.LBL_OUTPUT_DEST_FILE.Name = "LBL_OUTPUT_DEST_FILE"
        Me.LBL_OUTPUT_DEST_FILE.Size = New System.Drawing.Size(90, 21)
        Me.LBL_OUTPUT_DEST_FILE.TabIndex = 401
        Me.LBL_OUTPUT_DEST_FILE.Text = "目标文件："
        '
        'TXT_OUTPUT_DEST_FILE
        '
        Me.TXT_OUTPUT_DEST_FILE.Location = New System.Drawing.Point(192, 94)
        Me.TXT_OUTPUT_DEST_FILE.MaxLength = 1073741824
        Me.TXT_OUTPUT_DEST_FILE.Name = "TXT_OUTPUT_DEST_FILE"
        Me.TXT_OUTPUT_DEST_FILE.Size = New System.Drawing.Size(280, 29)
        Me.TXT_OUTPUT_DEST_FILE.TabIndex = 411
        '
        'TXT_INPUT_SOURCE_FILE
        '
        Me.TXT_INPUT_SOURCE_FILE.Location = New System.Drawing.Point(192, 59)
        Me.TXT_INPUT_SOURCE_FILE.MaxLength = 1073741824
        Me.TXT_INPUT_SOURCE_FILE.Name = "TXT_INPUT_SOURCE_FILE"
        Me.TXT_INPUT_SOURCE_FILE.ReadOnly = True
        Me.TXT_INPUT_SOURCE_FILE.Size = New System.Drawing.Size(280, 29)
        Me.TXT_INPUT_SOURCE_FILE.TabIndex = 311
        '
        'BTN_INPUT_BROWSE
        '
        Me.BTN_INPUT_BROWSE.Location = New System.Drawing.Point(478, 58)
        Me.BTN_INPUT_BROWSE.Name = "BTN_INPUT_BROWSE"
        Me.BTN_INPUT_BROWSE.Size = New System.Drawing.Size(35, 31)
        Me.BTN_INPUT_BROWSE.TabIndex = 321
        Me.BTN_INPUT_BROWSE.Text = "..."
        Me.BTN_INPUT_BROWSE.UseVisualStyleBackColor = True
        '
        'LBL_INPUT_SOURCE_FILE
        '
        Me.LBL_INPUT_SOURCE_FILE.AutoSize = True
        Me.LBL_INPUT_SOURCE_FILE.Font = New System.Drawing.Font("微软雅黑", 12.0!, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.LBL_INPUT_SOURCE_FILE.Location = New System.Drawing.Point(100, 63)
        Me.LBL_INPUT_SOURCE_FILE.Name = "LBL_INPUT_SOURCE_FILE"
        Me.LBL_INPUT_SOURCE_FILE.Size = New System.Drawing.Size(90, 21)
        Me.LBL_INPUT_SOURCE_FILE.TabIndex = 301
        Me.LBL_INPUT_SOURCE_FILE.Text = "输入文件："
        '
        'LBL_ABOUT_TITLE
        '
        Me.LBL_ABOUT_TITLE.AutoSize = True
        Me.LBL_ABOUT_TITLE.Font = New System.Drawing.Font("微软雅黑", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.LBL_ABOUT_TITLE.Location = New System.Drawing.Point(99, 18)
        Me.LBL_ABOUT_TITLE.Name = "LBL_ABOUT_TITLE"
        Me.LBL_ABOUT_TITLE.Size = New System.Drawing.Size(125, 26)
        Me.LBL_ABOUT_TITLE.TabIndex = 10013
        Me.LBL_ABOUT_TITLE.Text = "AVS Patrina"
        '
        'PIC_ABOUT_BADGE
        '
        Me.PIC_ABOUT_BADGE.Image = CType(resources.GetObject("PIC_ABOUT_BADGE.Image"), System.Drawing.Image)
        Me.PIC_ABOUT_BADGE.Location = New System.Drawing.Point(12, 18)
        Me.PIC_ABOUT_BADGE.Name = "PIC_ABOUT_BADGE"
        Me.PIC_ABOUT_BADGE.Size = New System.Drawing.Size(75, 75)
        Me.PIC_ABOUT_BADGE.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PIC_ABOUT_BADGE.TabIndex = 10012
        Me.PIC_ABOUT_BADGE.TabStop = False
        '
        'BTN_HANDLE
        '
        Me.BTN_HANDLE.Font = New System.Drawing.Font("微软雅黑", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.BTN_HANDLE.Location = New System.Drawing.Point(192, 192)
        Me.BTN_HANDLE.Name = "BTN_HANDLE"
        Me.BTN_HANDLE.Size = New System.Drawing.Size(140, 50)
        Me.BTN_HANDLE.TabIndex = 801
        Me.BTN_HANDLE.Text = "开始"
        Me.BTN_HANDLE.UseVisualStyleBackColor = True
        '
        'PGB_OUTPUT_PROGRESS
        '
        Me.PGB_OUTPUT_PROGRESS.Location = New System.Drawing.Point(12, 150)
        Me.PGB_OUTPUT_PROGRESS.MarqueeAnimationSpeed = 10
        Me.PGB_OUTPUT_PROGRESS.Name = "PGB_OUTPUT_PROGRESS"
        Me.PGB_OUTPUT_PROGRESS.Size = New System.Drawing.Size(500, 25)
        Me.PGB_OUTPUT_PROGRESS.TabIndex = 501
        '
        'BTN_NEW_TASK
        '
        Me.BTN_NEW_TASK.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(172, Byte), Integer))
        Me.BTN_NEW_TASK.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BTN_NEW_TASK.Font = New System.Drawing.Font("微软雅黑", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.BTN_NEW_TASK.ForeColor = System.Drawing.SystemColors.Window
        Me.BTN_NEW_TASK.Location = New System.Drawing.Point(420, 10)
        Me.BTN_NEW_TASK.Name = "BTN_NEW_TASK"
        Me.BTN_NEW_TASK.Size = New System.Drawing.Size(45, 40)
        Me.BTN_NEW_TASK.TabIndex = 201
        Me.BTN_NEW_TASK.Text = "+"
        Me.BTN_NEW_TASK.UseVisualStyleBackColor = False
        '
        'BTN_MENU
        '
        Me.BTN_MENU.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(172, Byte), Integer))
        Me.BTN_MENU.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BTN_MENU.Font = New System.Drawing.Font("微软雅黑", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.BTN_MENU.ForeColor = System.Drawing.Color.White
        Me.BTN_MENU.Location = New System.Drawing.Point(469, 10)
        Me.BTN_MENU.Name = "BTN_MENU"
        Me.BTN_MENU.Size = New System.Drawing.Size(45, 40)
        Me.BTN_MENU.TabIndex = 211
        Me.BTN_MENU.Text = "···"
        Me.BTN_MENU.UseVisualStyleBackColor = False
        '
        'CMS_MENU
        '
        Me.CMS_MENU.Font = New System.Drawing.Font("微软雅黑", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.CMS_MENU.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TSI_WIPE_CACHE, Me.TSS_MENU, Me.TSI_OPTIONS, Me.TSI_ABOUT})
        Me.CMS_MENU.Name = "ContextMenuStrip1"
        Me.CMS_MENU.Size = New System.Drawing.Size(141, 76)
        '
        'TSI_WIPE_CACHE
        '
        Me.TSI_WIPE_CACHE.Name = "TSI_WIPE_CACHE"
        Me.TSI_WIPE_CACHE.Size = New System.Drawing.Size(140, 22)
        Me.TSI_WIPE_CACHE.Text = "清除缓存(&C)"
        '
        'TSS_MENU
        '
        Me.TSS_MENU.Name = "TSS_MENU"
        Me.TSS_MENU.Size = New System.Drawing.Size(137, 6)
        '
        'TSI_OPTIONS
        '
        Me.TSI_OPTIONS.Name = "TSI_OPTIONS"
        Me.TSI_OPTIONS.Size = New System.Drawing.Size(140, 22)
        Me.TSI_OPTIONS.Text = "偏好设置(&P)"
        '
        'TSI_ABOUT
        '
        Me.TSI_ABOUT.Name = "TSI_ABOUT"
        Me.TSI_ABOUT.Size = New System.Drawing.Size(140, 22)
        Me.TSI_ABOUT.Text = "关于(&A)"
        '
        'CBO_PRESET
        '
        Me.CBO_PRESET.Font = New System.Drawing.Font("微软雅黑", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.CBO_PRESET.FormattingEnabled = True
        Me.CBO_PRESET.Location = New System.Drawing.Point(362, 202)
        Me.CBO_PRESET.Name = "CBO_PRESET"
        Me.CBO_PRESET.Size = New System.Drawing.Size(150, 28)
        Me.CBO_PRESET.TabIndex = 901
        Me.CBO_PRESET.Text = "(无可用预设)"
        '
        'MainUI
        '
        Me.AcceptButton = Me.BTN_HANDLE
        Me.AllowDrop = True
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(524, 261)
        Me.Controls.Add(Me.CBO_PRESET)
        Me.Controls.Add(Me.TXT_OUTPUT_DEST_FILE)
        Me.Controls.Add(Me.TXT_INPUT_SOURCE_FILE)
        Me.Controls.Add(Me.BTN_NEW_TASK)
        Me.Controls.Add(Me.BTN_MENU)
        Me.Controls.Add(Me.PGB_OUTPUT_PROGRESS)
        Me.Controls.Add(Me.BTN_HANDLE)
        Me.Controls.Add(Me.LBL_ABOUT_TITLE)
        Me.Controls.Add(Me.PIC_ABOUT_BADGE)
        Me.Controls.Add(Me.BTN_OUTPUT_BROWSE)
        Me.Controls.Add(Me.LBL_OUTPUT_DEST_FILE)
        Me.Controls.Add(Me.BTN_INPUT_BROWSE)
        Me.Controls.Add(Me.LBL_INPUT_SOURCE_FILE)
        Me.Font = New System.Drawing.Font("微软雅黑", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(5)
        Me.MaximizeBox = False
        Me.Name = "MainUI"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "AVS Patrina"
        CType(Me.PIC_ABOUT_BADGE, System.ComponentModel.ISupportInitialize).EndInit()
        Me.CMS_MENU.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents BGW_PROCESS As System.ComponentModel.BackgroundWorker
    Friend WithEvents OFD_INPUT_SOURCE_FILE As OpenFileDialog
    Friend WithEvents SFD_OUTPUT_DEST_FILE As SaveFileDialog
    Friend WithEvents BTN_OUTPUT_BROWSE As Button
    Friend WithEvents LBL_OUTPUT_DEST_FILE As Label
    Friend WithEvents TXT_OUTPUT_DEST_FILE As TextBox
    Friend WithEvents TXT_INPUT_SOURCE_FILE As TextBox
    Friend WithEvents BTN_INPUT_BROWSE As Button
    Friend WithEvents LBL_INPUT_SOURCE_FILE As Label
    Friend WithEvents LBL_ABOUT_TITLE As Label
    Friend WithEvents PIC_ABOUT_BADGE As PictureBox
    Friend WithEvents BTN_HANDLE As Button
    Friend WithEvents PGB_OUTPUT_PROGRESS As ProgressBar
    Friend WithEvents BTN_NEW_TASK As Button
    Friend WithEvents BTN_MENU As Button
    Friend WithEvents CMS_MENU As ContextMenuStrip
    Friend WithEvents TSI_OPTIONS As ToolStripMenuItem
    Friend WithEvents TSI_ABOUT As ToolStripMenuItem
    Friend WithEvents TSI_WIPE_CACHE As ToolStripMenuItem
    Friend WithEvents TSS_MENU As ToolStripSeparator
    Friend WithEvents CBO_PRESET As ComboBox
End Class
