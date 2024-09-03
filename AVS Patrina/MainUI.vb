Imports System.ComponentModel
Imports System.IO
Imports System.Media
Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography
Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web
Imports Microsoft.Win32

Public Class MainUI

    Public AVS_FRAME_TYPE_PREVIOUS As Integer = 0
    Public AVS_PID As Integer = -1
    Public AVS_PID_COUNTER As Integer = -1
    Public AVS_VIDEO_INFO As New List(Of String)
    Public CACHE_REMOVE As Boolean = True
    Public CLOUD_TRANSCODING As String = ""
    Public CONVERSION_STREAM As New List(Of MemoryStream)
    Public DRA_PID As Integer = -1
    Public ENCODE_PARAMETERS As String = ""
    Public ERROR_MESSAGE As String = ""
    Public ES_STREAM As New List(Of MemoryStream)
    Public GOP_MIN As Integer = 25
    Public INPUT_GOP As New List(Of Integer)
    Public INPUT_READ As FileStream
    Public INPUT_READER As BinaryReader
    Public INTRA_PTS As New List(Of Long)
    Public MT_ABORT As Integer = 1
    Public MT_ACTIVE As Boolean = False
    Public MT_STATUS As New List(Of Integer)
    Public MT_THREADS As Integer = 10
    Public OUTPUT_WRITE As FileStream
    Public OUTPUT_WRITER As BinaryWriter
    Public PASSTHROUGH_ACTIVE As Boolean = False
    Public PASSTHROUGH_PID As New List(Of Integer)
    Public PASSTHROUGH_PID_TYPE As New List(Of Integer)
    Public PAT_PMT As Byte()
    Public PAT_PMT_COUNTER As Integer = -1
    Public PCR_OFFSET As Integer = 8
    Public PMT_PID As Integer = 32
    Public PMT_PROGRAM_NUMBER As Integer = 1
    Public PTS_DELAY As Integer = 0
    Public PROCESSOR_STOPWATCH As New Stopwatch
    Public PROCESSOR_STOPWATCH_PROGRESS As Integer = -1
    Public TS_ID As Integer = 1
    ReadOnly TS_PACKET_HEADER_SIZE As Integer = 4
    ReadOnly TS_PACKET_SIZE As Integer = 188
    Public TS_STREAM As New List(Of MemoryStream)

    Private Sub BGW_PROCESS_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BGW_PROCESS.DoWork
        Try
            Dim BGW_PROCESS_CONTROL As BackgroundWorker = TryCast(sender, BackgroundWorker)
            PROCESSOR_STOPWATCH.Start()

            Dim MT_ERROR As Integer = 0
            Dim MT_IDLE As Integer = 0
            Dim MT_QUEUE As New List(Of Integer)
            While MT_ACTIVE
                Try
                    For ThreadID = 0 To MT_THREADS - 1
                        If MT_STATUS(ThreadID) = 0 Then
                            If INPUT_READER.BaseStream.Position >= INPUT_READ.Length - TS_PACKET_SIZE - 1 Then
                                MT_IDLE += 1
                                MT_STATUS(ThreadID) = Int32.MaxValue
                            Else
                                MT_STATUS(ThreadID) = 1
                                GetStream(ThreadID)
                                If MT_STATUS(ThreadID) = 2 Then
                                    If ES_STREAM(ThreadID).Length <= 0 Or INTRA_PTS(ThreadID) <= 0 Then
                                        ResetThread(ThreadID)
                                    Else
                                        MT_QUEUE.Add(ThreadID)
                                        MT_STATUS(ThreadID) = 3
                                        Dim _loc_2 As New Threading.Thread(AddressOf ConvertStream)
                                        _loc_2.Start(ThreadID)
                                    End If
                                    BGW_PROCESS_CONTROL.ReportProgress(Int(INPUT_READER.BaseStream.Position / INPUT_READ.Length * 100))
                                ElseIf MT_STATUS(ThreadID) = -1 Then
                                    ResetThread(ThreadID)
                                    MT_ERROR += 1
                                End If
                            End If
                        ElseIf MT_STATUS(ThreadID) = 4 Then
                            If MT_QUEUE(0) = ThreadID Then
                                MT_STATUS(ThreadID) = 5
                                WriteStream(ThreadID)
                                If MT_STATUS(ThreadID) = 6 Then
                                    MT_QUEUE.RemoveAt(0)
                                    ResetThread(ThreadID)
                                    MT_ERROR = 0
                                    ERROR_MESSAGE = ""
                                ElseIf MT_STATUS(ThreadID) = -5 Then
                                    MT_ERROR += 1
                                End If
                            End If
                        ElseIf MT_STATUS(ThreadID) = -3 Then
                            CONVERSION_STREAM(ThreadID).Close()
                            CONVERSION_STREAM(ThreadID) = New MemoryStream
                            MT_ERROR += 1
                        End If
                    Next

                    If MT_IDLE >= MT_THREADS Then
                        MT_ACTIVE = False
                        MT_ABORT = 0
                    ElseIf MT_ERROR >= MT_THREADS * 2 Then
                        MT_ACTIVE = False
                        MT_ABORT = -1
                    End If
                Catch ex As Exception

                End Try

                Threading.Thread.Sleep(10)
            End While
        Catch ex As Exception

        End Try
    End Sub

    Private Sub BGW_PROCESS_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles BGW_PROCESS.ProgressChanged
        Try
            If e.ProgressPercentage > PROCESSOR_STOPWATCH_PROGRESS And e.ProgressPercentage < 100 Then
                PGB_OUTPUT_PROGRESS.Value = e.ProgressPercentage
                PROCESSOR_STOPWATCH_PROGRESS = e.ProgressPercentage
                Text = e.ProgressPercentage & "% - " & Path.GetFileNameWithoutExtension(OUTPUT_WRITE.Name)

                If PROCESSOR_STOPWATCH.IsRunning Then
                    Dim _LOC_1 As TimeSpan = TimeSpan.FromTicks(PROCESSOR_STOPWATCH.Elapsed.Ticks / e.ProgressPercentage * (100 - e.ProgressPercentage))
                    If Not PROCESSOR_STOPWATCH_PROGRESS >= 100 And _LOC_1.Days < 1 And OUTPUT_WRITER.BaseStream.Position > 0 Then Text += "  (" & String.Format("{0}h {1}m {2}s", _LOC_1.Hours, _LOC_1.Minutes, _LOC_1.Seconds) & ")"
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub BGW_PROCESS_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BGW_PROCESS.RunWorkerCompleted
        Try
            PROCESSOR_STOPWATCH.Stop()
            PROCESSOR_STOPWATCH.Reset()

            If MT_ABORT = 0 Then
                PGB_OUTPUT_PROGRESS.Value = 100
                Text = "100% - " & Path.GetFileNameWithoutExtension(OUTPUT_WRITE.Name) & " [完成]"
                PLAY_SOUND_OK()
            ElseIf MT_ABORT = 1 Then
                Text &= " [中止]"
            Else
                Text &= " [错误]"
                If ERROR_MESSAGE.Length > 0 Then
                    MsgBox(ERROR_MESSAGE, vbExclamation, "任务失败")
                End If
            End If

            BTN_HANDLE.Text = "开始"
            BTN_HANDLE.Enabled = True
        Catch ex As Exception

        End Try

        Try
            INPUT_READ.Close()
            INPUT_READER.Close()
            INPUT_READ.Dispose()
            INPUT_READER.Dispose()
            OUTPUT_WRITE.Close()
            OUTPUT_WRITER.Close()
            OUTPUT_WRITE.Dispose()
            OUTPUT_WRITER.Dispose()
        Catch ex As Exception

        End Try

        Try
            If CACHE_REMOVE Then
                My.Computer.FileSystem.DeleteFile(AVS_VIDEO_INFO(3))
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub BTN_HANDLE_Click(sender As Object, e As EventArgs) Handles BTN_HANDLE.Click
        Try
            BTN_HANDLE.Enabled = False
            If BTN_HANDLE.Text = "开始" Then
                If BGW_PROCESS.IsBusy = False Then
                    If MT_STATUS.Count > 0 Then
                        For ThreadID = 0 To MT_STATUS.Count - 1
                            If MT_STATUS(ThreadID) = 3 Then
                                MsgBox("请等待编码器线程回收后继续。", vbExclamation, "任务失败")
                                BTN_HANDLE.Enabled = True
                                Exit Sub
                            End If
                        Next
                    End If
                    ResetStream()
                    If Not IsFFmpegExist() Then
                        MsgBox("请您安装 FFmpeg 程序后继续。", vbExclamation, "任务失败")
                        BTN_HANDLE.Enabled = True
                        Exit Sub
                    End If
                    If Not My.Computer.FileSystem.FileExists(TXT_INPUT_SOURCE_FILE.Text) Then
                        MsgBox("您的输入文件无效！", vbExclamation, "任务失败")
                        BTN_HANDLE.Enabled = True
                        Exit Sub
                    End If
                    INPUT_READ = New FileStream(TXT_INPUT_SOURCE_FILE.Text, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                    INPUT_READER = New BinaryReader(INPUT_READ)
                    Dim OUTPUT_PATH As String = RegulateFilePath(TXT_OUTPUT_DEST_FILE.Text, ".ts")
                    If Not My.Computer.FileSystem.DirectoryExists(Path.GetDirectoryName(OUTPUT_PATH)) Then My.Computer.FileSystem.CreateDirectory(Path.GetDirectoryName(OUTPUT_PATH))
                    GetStreamInfo()
                    If AVS_VIDEO_INFO.Count = 0 Then
                        MsgBox("无法从您的输入文件中读取码流参数！", vbExclamation, "任务失败")
                        BTN_HANDLE.Enabled = True
                        Exit Sub
                    Else
                        Dim AVC_CONFIG_FILE As String = Application.StartupPath & "\" & CBO_PRESET.Text & ".ini"
                        If My.Computer.FileSystem.FileExists(AVC_CONFIG_FILE) Then
                            Dim AVC_CONFIG_CACHE As String = ""
                            For Each AVC_CONFIG_LINE In My.Computer.FileSystem.ReadAllText(AVC_CONFIG_FILE).Split(vbLf)
                                Dim AVC_CONFIG_LINE_TRIM As String = AVC_CONFIG_LINE.Trim()
                                If Not (AVC_CONFIG_LINE_TRIM = "" Or AVC_CONFIG_LINE_TRIM.StartsWith("#")) Then
                                    If AVC_CONFIG_LINE_TRIM.StartsWith("def_horizontal_size") Then
                                        AVC_CONFIG_CACHE &= AVC_CONFIG_LINE_TRIM.Split("=")(0) & " = " & AVS_VIDEO_INFO(0) & vbCrLf
                                    ElseIf AVC_CONFIG_LINE_TRIM.StartsWith("def_vertical_size") Then
                                        AVC_CONFIG_CACHE &= AVC_CONFIG_LINE_TRIM.Split("=")(0) & " = " & AVS_VIDEO_INFO(1) & vbCrLf
                                    ElseIf AVC_CONFIG_LINE_TRIM.StartsWith("frame_rate") Then
                                        AVC_CONFIG_CACHE &= AVC_CONFIG_LINE_TRIM.Split("=")(0) & " = " & AVS_VIDEO_INFO(2) & vbCrLf
                                    Else
                                        AVC_CONFIG_CACHE &= AVC_CONFIG_LINE_TRIM & vbCrLf
                                    End If
                                End If
                            Next
                            Dim AVC_CONFIG_CACHE_FILE As String = GetTempFile() & ".ini"
                            AVS_VIDEO_INFO.Add(AVC_CONFIG_CACHE_FILE)
                            AVS_VIDEO_INFO.Add(AVC_CONFIG_CACHE)
                            My.Computer.FileSystem.WriteAllText(AVC_CONFIG_CACHE_FILE, AVC_CONFIG_CACHE, False, Encoding.Default)
                        Else
                            MsgBox("无法获取AVC编码器配置文件！", vbExclamation, "任务失败")
                            BTN_HANDLE.Enabled = True
                            Exit Sub
                        End If
                    End If
                    BUILD_PAT_PMT()
                    OUTPUT_WRITE = New FileStream(OUTPUT_PATH, FileMode.Create)
                    OUTPUT_WRITER = New BinaryWriter(OUTPUT_WRITE)
                    BTN_HANDLE.Text = "停止"
                    BGW_PROCESS.RunWorkerAsync()
                    BTN_HANDLE.Enabled = True
                End If
            Else
                If KeyRead("ABORT_CONFIRM").Length > 0 Then
                    If MsgBox("任务正在进行。您确定结束任务吗？", vbQuestion + vbYesNo, "停止") = vbYes Then
                        MT_ACTIVE = False
                    Else
                        BTN_HANDLE.Enabled = True
                    End If
                Else
                    MT_ACTIVE = False
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.ToString, vbExclamation, "任务失败")
            BTN_HANDLE.Enabled = True
        End Try
    End Sub

    Private Sub BTN_INPUT_BROWSE_Click(sender As Object, e As EventArgs) Handles BTN_INPUT_BROWSE.Click
        Try
            If OFD_INPUT_SOURCE_FILE.ShowDialog = DialogResult.OK Then
                TXT_INPUT_SOURCE_FILE.Text = OFD_INPUT_SOURCE_FILE.FileName
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub BTN_OUTPUT_BROWSE_Click(sender As Object, e As EventArgs) Handles BTN_OUTPUT_BROWSE.Click
        Try
            If SFD_OUTPUT_DEST_FILE.ShowDialog = DialogResult.OK Then
                TXT_OUTPUT_DEST_FILE.Text = SFD_OUTPUT_DEST_FILE.FileName
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub BTN_MENU_Click(sender As Object, e As EventArgs) Handles BTN_MENU.Click
        Try
            CMS_MENU.Show(System.Windows.Forms.Control.MousePosition)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub BTN_NEW_TASK_Click(sender As Object, e As EventArgs) Handles BTN_NEW_TASK.Click
        Try
            Process.Start(Application.ExecutablePath)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub LBL_INPUT_SOURCE_FILE_Click(sender As Object, e As EventArgs) Handles LBL_INPUT_SOURCE_FILE.Click
        Try
            Process.Start("explorer.exe", New IO.FileInfo(TXT_INPUT_SOURCE_FILE.Text).DirectoryName)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub LBL_OUTPUT_DEST_FILE_Click(sender As Object, e As EventArgs) Handles LBL_OUTPUT_DEST_FILE.Click
        Try
            If LBL_OUTPUT_DEST_FILE.Text.Contains("\") Then
                Process.Start("explorer.exe", New IO.FileInfo(TXT_OUTPUT_DEST_FILE.Text).DirectoryName)
            Else
                LBL_INPUT_SOURCE_FILE_Click(sender, e)
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub MainUI_DragDrop(sender As Object, e As DragEventArgs) Handles Me.DragDrop
        Try
            Dim DragFilePath As String() = e.Data.GetData(DataFormats.FileDrop)
            If My.Computer.FileSystem.FileExists(DragFilePath(0)) Then
                TXT_INPUT_SOURCE_FILE.Text = DragFilePath(0)
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub MainUI_DragEnter(sender As Object, e As DragEventArgs) Handles Me.DragEnter
        Try
            If e.Data.GetDataPresent(DataFormats.FileDrop) = True Then
                e.Effect = DragDropEffects.Copy
            Else
                e.Effect = DragDropEffects.None
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub MainUI_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Try
            e.Cancel = 1
            If BGW_PROCESS.IsBusy Then
                If MsgBox("任务正在进行。您确定结束任务并退出程序吗？", vbQuestion + vbYesNo, "退出") = vbNo Then
                    Exit Sub
                End If
            End If
            Dispose()
            End
        Catch ex As Exception
            Dispose()
            End
        End Try
    End Sub

    Private Sub MainUI_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            For Each PresetFile In Directory.GetFiles(Application.StartupPath, "*.ini")
                Dim PresetFileName As String = Path.GetFileNameWithoutExtension(PresetFile)
                CBO_PRESET.Items.Add(PresetFileName)
                CBO_PRESET.Text = PresetFileName
            Next
        Catch ex As Exception

        End Try
    End Sub

    Private Sub PIC_ABOUT_BADGE_Click(sender As Object, e As EventArgs) Handles PIC_ABOUT_BADGE.Click
        Try
            TSI_ABOUT_Click(sender, e)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub TSI_ABOUT_Click(sender As Object, e As EventArgs) Handles TSI_ABOUT.Click
        Try
            Dim VersionStrings As String() = Application.ProductVersion.ToString.Split(".")
            MsgBox("AVS Patrina" & vbCrLf & vbCrLf & "AVS1-P16广播视频编码转换器" & vbCrLf & vbCrLf & "软件版本：" & VersionStrings(0) & "." & VersionStrings(1) & "." & VersionStrings(2) & vbCrLf & "更新时间：20" & VersionStrings(3).Substring(0, 2) & "年" & Int(VersionStrings(3).Substring(2, 2)) & "月" & vbCrLf & vbCrLf & "Copyright © 2021-2024 版权所有", vbInformation, TSI_ABOUT.Text.Split("(")(0))
        Catch ex As Exception

        End Try
    End Sub

    Private Sub TSI_OPTIONS_Click(sender As Object, e As EventArgs) Handles TSI_OPTIONS.Click
        Try
            Preferences.Show()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub TSI_WIPE_CACHE_Click(sender As Object, e As EventArgs) Handles TSI_WIPE_CACHE.Click
        Try
            If MsgBox("您即将清理缓存文件夹并释放磁盘空间。如果您有正在进行的任务，执行此操作将可能导致这些任务失败。请确认您没有活动的任务。" & vbCrLf & "您无法撤销本操作！执行清理？", vbQuestion + vbOKCancel, "清除缓存") = vbOK Then
                My.Computer.FileSystem.DeleteDirectory(Path.GetDirectoryName(GetTempFile()), FileIO.DeleteDirectoryOption.DeleteAllContents)
                MsgBox("缓存已清除。", vbInformation, "清除缓存")
            End If
        Catch ex As Exception
            MsgBox("缓存文件夹清理失败！", vbExclamation, "清除缓存")
        End Try
    End Sub

    Public Function AppendTo(param1 As Byte(), param2 As Byte()) As Byte()
        Dim _loc_1 As Byte() = New Byte(param1.Length + param2.Length - 1) {}
        If param2.Length > 0 Then
            Array.Copy(param2, _loc_1, param2.Length)
        End If
        Array.Copy(param1, 0, _loc_1, param2.Length, param1.Length)
        Return _loc_1
    End Function

    Public Function BinToBytes(param1 As String) As Byte()
        Dim _loc_1 As Integer = param1.Length / 8
        Dim _loc_2 As Byte() = New Byte(_loc_1 - 1) {}
        For _loc_3 As Integer = 0 To _loc_1 - 1
            _loc_2(_loc_3) = Convert.ToByte(param1.Substring(8 * _loc_3, 8), 2)
        Next
        Return _loc_2
    End Function

    Private Function BUILD_PACKET_BIN_PADDING(Input As String)
        Try
            Input = Input.Trim
            If Input.Length < TS_PACKET_SIZE * 8 Then
                For i = 1 To TS_PACKET_SIZE * 8 - Input.Length
                    Input += "1"
                Next
            End If
            If Input.Length > TS_PACKET_SIZE * 8 Then
                Input = Input.Substring(0, TS_PACKET_SIZE * 8)
            End If
            Return Input
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Private Sub BUILD_PAT_PMT()
        Try
            'BEGIN BUILD PAT
            'BEGIN BUILD PAT TS PACKET HEADER
            'Byte 1
            Dim PACKET_DATA_BIN As String = "01000111"  'sync_byte 0x47

            'Byte 2 and Byte 3
            PACKET_DATA_BIN += "0"                      'transport_error_indicator
            PACKET_DATA_BIN += "1"                      'payload_unit_start_indicator
            PACKET_DATA_BIN += "0"                      'transport_priority
            PACKET_DATA_BIN += "0000000000000"          'PID 0x00

            'Byte 4
            PACKET_DATA_BIN += "00"                     'transport_scrambling_control
            PACKET_DATA_BIN += "01"                     'adaptation_field_control
            PACKET_DATA_BIN += "0000"                   'continuity_counte
            'END BUILD PAT TS PACKET HEADER

            'EMPTY BYTE
            PACKET_DATA_BIN += "00000000"

            'BEGIN BUILD PAT TABLE
            'Byte 1
            PACKET_DATA_BIN += "00000000"               'table_id

            'Byte 2 and Byte 3
            PACKET_DATA_BIN += "1"                      'section_syntax_indicator
            PACKET_DATA_BIN += "0"                      'zero
            PACKET_DATA_BIN += "11"                     'reserved
            PACKET_DATA_BIN += "XXXXXXXXXXXX"           'section_length

            Dim _LOC_1 As Integer = PACKET_DATA_BIN.Length

            'Byte 4 and Byte 5
            PACKET_DATA_BIN += ID_BIN(TS_ID, 16)        'transport_stream_id

            'Byte 6
            PACKET_DATA_BIN += "11"                     'reserved
            PACKET_DATA_BIN += "00000"                  'version_number
            PACKET_DATA_BIN += "1"                      'current_next_indicator

            'Byte 7
            PACKET_DATA_BIN += "00000000"               'section_number 0x00

            'Byte 8
            PACKET_DATA_BIN += "00000000"               'last_section_number 0x00

            'Byte 9 and Byte 10
            PACKET_DATA_BIN += ID_BIN(PMT_PROGRAM_NUMBER, 16)        'program_number

            'Byte 11 to Byte 12
            PACKET_DATA_BIN += "111"                    'reserved
            PACKET_DATA_BIN += ID_BIN(PMT_PID, 13)      'program_map_PID

            PACKET_DATA_BIN = PACKET_DATA_BIN.Replace("XXXXXXXXXXXX", ID_BIN(Int((PACKET_DATA_BIN.Length - _LOC_1) / 8 + 4), 12))

            'Byte 13 to Byte 16
            PACKET_DATA_BIN += BytesToBin(HexToBytes(CALC_CRC32_MPEG_2_STRING(BytesToHex(BinToBytes(PACKET_DATA_BIN.Substring((TS_PACKET_HEADER_SIZE + 1) * 8, PACKET_DATA_BIN.Length - (TS_PACKET_HEADER_SIZE + 1) * 8))))))

            PACKET_DATA_BIN = BUILD_PACKET_BIN_PADDING(PACKET_DATA_BIN)
            Dim PAT_DATA As String = PACKET_DATA_BIN
            'END BUILD PAT

            'BEGIN BUILD PMT
            'BEGIN BUILD PMT TS PACKET HEADER
            'Byte 1
            PACKET_DATA_BIN = "01000111"                'sync_byte 0x47

            'Byte 2 and Byte 3
            PACKET_DATA_BIN += "0"                      'transport_error_indicator
            PACKET_DATA_BIN += "1"                      'payload_unit_start_indicator
            PACKET_DATA_BIN += "0"                      'transport_priority
            PACKET_DATA_BIN += ID_BIN(PMT_PID, 13)      'PID

            'Byte 4
            PACKET_DATA_BIN += "00"                     'transport_scrambling_control
            PACKET_DATA_BIN += "01"                     'adaptation_field_control
            PACKET_DATA_BIN += "0000"                   'continuity_counte
            'END BUILD PMT TS PACKET HEADER

            'EMPTY BYTE
            PACKET_DATA_BIN += "00000000"

            'BEGIN BUILD PMT TABLE
            'Byte 1
            PACKET_DATA_BIN += "00000010"               'table_id 0x02

            'Byte 2 and Byte 3
            PACKET_DATA_BIN += "1"                      'section_syntax_indicator
            PACKET_DATA_BIN += "0"                      'zero
            PACKET_DATA_BIN += "11"                     'reserved
            PACKET_DATA_BIN += "XXXXXXXXXXXX"           'section_length

            Dim _LOC_2 As Integer = PACKET_DATA_BIN.Length

            'Byte 4 and Byte 5
            PACKET_DATA_BIN += ID_BIN(PMT_PROGRAM_NUMBER, 16)        'program_number

            'Byte 6
            PACKET_DATA_BIN += "11"                     'reserved
            PACKET_DATA_BIN += "00000"                  'version_number
            PACKET_DATA_BIN += "1"                      'current_next_indicator

            'Byte 7
            PACKET_DATA_BIN += "00000000"               'section_number 0x00

            'Byte 8
            PACKET_DATA_BIN += "00000000"               'last_section_number 0x00

            'Byte 9 and Byte 10
            PACKET_DATA_BIN += "111"                    'reserved
            PACKET_DATA_BIN += "PPPPPPPPPPPPP"          'PCR_PID

            'Byte 11 and Byte 12
            PACKET_DATA_BIN += "1111"                   'reserved
            PACKET_DATA_BIN += "000000000000"           'program_info_length

            'From Byte 13
            Dim _LOC_3 As Integer = 0
            For _LOC_4 = 0 To PASSTHROUGH_PID.Count - 1
                Dim _LOC_5 As Integer = PASSTHROUGH_PID(_LOC_4)
                If _LOC_5 >= 32 And _LOC_5 <= 8190 Then
                    Dim INSERT_CODEC_PAT_PMT As String = ""
                    If PASSTHROUGH_PID_TYPE(_LOC_4) = 6 Then
                        INSERT_CODEC_PAT_PMT = "AC3"
                    ElseIf PASSTHROUGH_PID_TYPE(_LOC_4) = 1006 Then
                        PASSTHROUGH_PID_TYPE(_LOC_4) = 6
                        INSERT_CODEC_PAT_PMT = "DRA"
                    ElseIf PASSTHROUGH_PID_TYPE(_LOC_4) = 2006 Then
                        PASSTHROUGH_PID_TYPE(_LOC_4) = 6
                        INSERT_CODEC_PAT_PMT = "EAC3"
                    End If

                    PACKET_DATA_BIN += ID_BIN(PASSTHROUGH_PID_TYPE(_LOC_4), 8)   'stream_type
                    PACKET_DATA_BIN += "111"        'reserved
                    PACKET_DATA_BIN += ID_BIN(_LOC_5, 13)                        'elementary_PID
                    PACKET_DATA_BIN += "1111"       'reserved

                    If PASSTHROUGH_PID_TYPE(_LOC_4) = 6 Then
                        If INSERT_CODEC_PAT_PMT.Replace("-", "").ToUpper.Contains("EAC3") Then
                            'BEGIN BUILD E-AC-3 ES INFO
                            PACKET_DATA_BIN += "000000000011"                        'ES_info_length 0x03

                            PACKET_DATA_BIN += "01111010"                            'descriptor_tag 0x7A

                            PACKET_DATA_BIN += "00000001"                            'descriptor_length 0x01

                            PACKET_DATA_BIN += "00000000"
                            'END BUILD E-AC-3 ES INFO
                        ElseIf INSERT_CODEC_PAT_PMT.Replace("-", "").ToUpper.Contains("AC3") Then
                            'BEGIN BUILD AC-3 ES INFO
                            PACKET_DATA_BIN += "000000000011"                        'ES_info_length 0x03

                            PACKET_DATA_BIN += "01101010"                            'descriptor_tag 0x6A

                            PACKET_DATA_BIN += "00000001"                            'descriptor_length 0x01

                            PACKET_DATA_BIN += "00000000"
                            'END BUILD AC-3 ES INFO
                        ElseIf INSERT_CODEC_PAT_PMT.ToUpper.Contains("DRA") Then
                            'BEGIN BUILD DRA ES INFO
                            PACKET_DATA_BIN += "000000001010"                        'ES_info_length 0x0A

                            'DRA registration descriptor
                            PACKET_DATA_BIN += "00000101"                            'descriptor_tag 0x05

                            PACKET_DATA_BIN += "00000100"                            'descriptor_length 0x04

                            PACKET_DATA_BIN += "01000100010100100100000100110001"    'format_identifier 0x44524131

                            'DRA audio stream descriptor
                            PACKET_DATA_BIN += "10100000"                            'descriptor_tag 0xA0

                            PACKET_DATA_BIN += "00000010"                            'descriptor_length 0x02

                            PACKET_DATA_BIN += "1000"                                'sample_rate_index 0x08

                            PACKET_DATA_BIN += "000010"                              'num_normal_channels 0x02

                            PACKET_DATA_BIN += "00"                                  'num_lfe_channels 0x00

                            PACKET_DATA_BIN += "0"                                   'dra_version_flag

                            PACKET_DATA_BIN += "0"                                   'text_present_flag

                            PACKET_DATA_BIN += "0"                                   'language_present_flag

                            PACKET_DATA_BIN += "0"                                   'reversed
                            'END BUILD DRA ES INFO
                        Else
                            PACKET_DATA_BIN += "000000000000"                        'ES_info_length
                        End If
                    Else
                        PACKET_DATA_BIN += "000000000000"                        'ES_info_length
                    End If

                    If _LOC_3 = 0 Then PACKET_DATA_BIN = PACKET_DATA_BIN.Replace("PPPPPPPPPPPPP", ID_BIN(_LOC_5, 13))
                    _LOC_3 += 1
                End If

                Dim MAX_PID_NUMBER As Integer = 32
                If _LOC_3 >= MAX_PID_NUMBER Then Exit For
            Next

            PACKET_DATA_BIN = PACKET_DATA_BIN.Replace("XXXXXXXXXXXX", ID_BIN(Int((PACKET_DATA_BIN.Length - _LOC_2) / 8 + 4), 12))

            'Last 4 Bytes
            PACKET_DATA_BIN += BytesToBin(HexToBytes(CALC_CRC32_MPEG_2_STRING(BytesToHex(BinToBytes(PACKET_DATA_BIN.Substring((TS_PACKET_HEADER_SIZE + 1) * 8, PACKET_DATA_BIN.Length - (TS_PACKET_HEADER_SIZE + 1) * 8))))))

            PACKET_DATA_BIN = BUILD_PACKET_BIN_PADDING(PACKET_DATA_BIN)
            Dim PMT_DATA As String = PACKET_DATA_BIN
            'END BUILD PMT

            PAT_PMT = AppendTo(BinToBytes(PMT_DATA), BinToBytes(PAT_DATA))
        Catch ex As Exception

        End Try
    End Sub

    Public Function BytesToBin(param1() As Byte) As String
        Dim _loc_1 As New StringBuilder
        For Each _loc_2 In param1
            _loc_1.Append(Convert.ToString(_loc_2, 2).PadLeft(8, "0"))
        Next
        Return _loc_1.ToString
    End Function

    Public Function BytesToHex(param1 As Byte()) As String
        Return BitConverter.ToString(param1).Replace("-", "").ToUpper
    End Function

    Private Function CALC_CRC32_MPEG_2(Data As UInteger(), Length As Integer) As UInteger
        Dim i As UInteger
        Dim crc As UInteger = &HFFFFFFFFL, j As UInteger = 0
        While Math.Max(Threading.Interlocked.Decrement(Length), Length + 1) <> 0
            crc = crc Xor Data(j) << 24
            j += 1
            For i = 0 To 7
                If Not (crc And &H80000000L) = 0 Then
                    crc = (crc << 1) Xor &H4C11DB7
                Else
                    crc <<= 1
                End If
            Next
        End While
        Return crc
    End Function

    Public Function CALC_CRC32_MPEG_2_STRING(Input As String) As String
        Dim CRC32_Data As UInteger()
        CRC32_Data = HexToUInt(Input)
        Dim CRC32_Result As UInteger = CALC_CRC32_MPEG_2(CRC32_Data, CRC32_Data.Length)
        Dim crcStr As String = CRC32_Result.ToString("X4")
        crcStr = If(crcStr.Length = 8, crcStr, "0" & crcStr)
        Return crcStr.Trim
    End Function

    Public Function CheckValidationResult(sender As Object, certificate As X509Certificate, chain As X509Chain, errors As SslPolicyErrors) As Boolean
        Return True
    End Function

    Private Sub ConvertStream(ThreadID As Integer)
        Try
            Dim CONVERSION_ERROR As Integer = 0
            While CONVERSION_ERROR < 10
                Dim ES_BUFFER As Byte() = New Byte(ES_STREAM(ThreadID).Length - 1) {}
                ES_STREAM(ThreadID).Position = 0
                ES_STREAM(ThreadID).Read(ES_BUFFER, 0, ES_STREAM(ThreadID).Length)
                Dim CONVERSION_TS_BUFFER As Byte() = New Byte() {}

                Dim AVC_CACHE_FILE As String = GetTempFile() & ".avc"
                Dim ES_DECODER As String = Environment.CurrentDirectory & "\ldecod.exe"
                If My.Computer.FileSystem.FileExists(ES_DECODER) Then
                    Dim ES_CACHE_FILE As String = GetTempFile() & ".avs"
                    My.Computer.FileSystem.WriteAllBytes(ES_CACHE_FILE, ES_BUFFER, False)
                    Dim YUV_CACHE_FILE As String = GetTempFile() & ".yuv"
                    Dim ES_DECODER_SHELL As New Process()
                    ES_DECODER_SHELL.StartInfo.FileName = ES_DECODER
                    ES_DECODER_SHELL.StartInfo.WorkingDirectory = Path.GetDirectoryName(ES_CACHE_FILE)
                    ES_DECODER_SHELL.StartInfo.Arguments = "nul " & Path.GetFileName(ES_CACHE_FILE) & " " & Path.GetFileName(YUV_CACHE_FILE) & " nul 2 0 0 0 0"
                    ES_DECODER_SHELL.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                    ES_DECODER_SHELL.Start()
                    ES_DECODER_SHELL.WaitForExit()
                    ES_DECODER_SHELL.Close()
                    ES_DECODER_SHELL.Dispose()

                    Dim MAINCONCEPT_ENC_AVC As String = Application.StartupPath & "\sample_enc_avc.exe"
                    If Not My.Computer.FileSystem.FileExists(MAINCONCEPT_ENC_AVC) Then
                        ERROR_MESSAGE = "未找到MainConcept® AVC/H.264 Video Encoder！"
                        MT_STATUS(ThreadID) = -3
                        Exit Sub
                    End If

                    Dim YUV_FRAME_SIZE As Long = Int(AVS_VIDEO_INFO(0)) * Int(AVS_VIDEO_INFO(1)) * 1.5
                    Dim YUV_FILE_SIZE As Long = 0
                    If My.Computer.FileSystem.FileExists(YUV_CACHE_FILE) Then YUV_FILE_SIZE = New FileInfo(YUV_CACHE_FILE).Length
                    If Not YUV_FILE_SIZE = INPUT_GOP(ThreadID) * YUV_FRAME_SIZE And Not INPUT_GOP(ThreadID) = 0 Then
                        Dim YUV_FILE_STREAM As New IO.FileStream(YUV_CACHE_FILE, IO.FileMode.OpenOrCreate, IO.FileAccess.Write, IO.FileShare.None)
                        YUV_FILE_STREAM.SetLength(INPUT_GOP(ThreadID) * YUV_FRAME_SIZE)
                        If YUV_FILE_SIZE < YUV_FILE_STREAM.Length Then
                            Try
                                Dim YUV_STUFF As Byte() = New Byte() {}
                                Dim YUV_STUFF_FILE As String = ""
                                For Each YUV_STUFF_FILE_SELECT As String In Directory.GetFiles(Application.StartupPath, "*.yuv")
                                    If New IO.FileInfo(YUV_STUFF_FILE_SELECT).Length = YUV_FRAME_SIZE Then
                                        YUV_STUFF_FILE = YUV_STUFF_FILE_SELECT
                                        Exit For
                                    End If
                                Next
                                If YUV_STUFF_FILE.Length > 0 Then YUV_STUFF = My.Computer.FileSystem.ReadAllBytes(YUV_STUFF_FILE)

                                Dim YUV_STUFF_OFFSET As Long = Math.Ceiling(YUV_FILE_SIZE / YUV_FRAME_SIZE) * YUV_FRAME_SIZE
                                If YUV_STUFF.Length > 0 And YUV_STUFF_OFFSET < YUV_FILE_STREAM.Length Then
                                    YUV_FILE_STREAM.Seek(YUV_STUFF_OFFSET, 0)
                                    While YUV_FILE_STREAM.Position < YUV_FILE_STREAM.Length
                                        YUV_FILE_STREAM.Write(YUV_STUFF, 0, YUV_STUFF.Length)
                                    End While
                                End If
                            Catch ex As Exception

                            End Try
                        End If
                        YUV_FILE_STREAM.Close()
                    End If

                    Dim RAW_ENCODER_SHELL As New Process()
                    RAW_ENCODER_SHELL.StartInfo.FileName = MAINCONCEPT_ENC_AVC
                    RAW_ENCODER_SHELL.StartInfo.WorkingDirectory = Path.GetDirectoryName(YUV_CACHE_FILE)
                    RAW_ENCODER_SHELL.StartInfo.Arguments = "-I420 -w " & AVS_VIDEO_INFO(0) & " -h " & AVS_VIDEO_INFO(1) & " -v " & Chr(34) & YUV_CACHE_FILE & Chr(34) & " -o " & Chr(34) & AVC_CACHE_FILE & Chr(34) & " -c " & Chr(34) & AVS_VIDEO_INFO(3) & Chr(34)
                    RAW_ENCODER_SHELL.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                    RAW_ENCODER_SHELL.Start()
                    RAW_ENCODER_SHELL.WaitForExit()
                    RAW_ENCODER_SHELL.Close()
                    RAW_ENCODER_SHELL.Dispose()

                    Try
                        If CACHE_REMOVE Then
                            My.Computer.FileSystem.DeleteFile(ES_CACHE_FILE)
                            My.Computer.FileSystem.DeleteFile(YUV_CACHE_FILE)
                        End If
                    Catch ex As Exception

                    End Try
                ElseIf CLOUD_TRANSCODING.ToLower.StartsWith("http://") Or CLOUD_TRANSCODING.ToLower.StartsWith("https://") Then
                    If ES_BUFFER.Length > 0 Then
                        Do
                            Try
                                My.Computer.FileSystem.WriteAllBytes(AVC_CACHE_FILE, PostWebpage(ES_BUFFER, ThreadID), False)
                                If MT_STATUS(ThreadID) = -31 Then
                                    ERROR_MESSAGE = "云转码失败！"
                                    MT_STATUS(ThreadID) = -3
                                    Exit Sub
                                End If
                                Exit Do
                            Catch ex As Exception

                            End Try
                        Loop
                    End If
                Else
                    ERROR_MESSAGE = "未找到AVS1-P16广播视频(AVS+)解码器！"
                    MT_STATUS(ThreadID) = -3
                    Exit Sub
                End If

                Dim TS_CACHE_FILE As String = GetTempFile() & ".ts"
                Dim MAINCONCEPT_MUX_MP2 As String = Application.StartupPath & "\sample_mux_mp2_file.exe"
                If Not My.Computer.FileSystem.FileExists(MAINCONCEPT_MUX_MP2) Then
                    ERROR_MESSAGE = "未找到MainConcept® MPEG-2 Multiplexer！"
                    MT_STATUS(ThreadID) = -3
                    Exit Sub
                End If
                If My.Computer.FileSystem.FileExists(AVC_CACHE_FILE) Then
                    If New FileInfo(AVC_CACHE_FILE).Length > 16 Then
                        Dim MP2T_MUXER_SHELL As New Process()
                        MP2T_MUXER_SHELL.StartInfo.FileName = MAINCONCEPT_MUX_MP2
                        MP2T_MUXER_SHELL.StartInfo.WorkingDirectory = Path.GetDirectoryName(AVC_CACHE_FILE)
                        MP2T_MUXER_SHELL.StartInfo.Arguments = "-i " & Chr(34) & AVC_CACHE_FILE & Chr(34) & " -o " & Chr(34) & TS_CACHE_FILE & Chr(34)
                        MP2T_MUXER_SHELL.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                        MP2T_MUXER_SHELL.Start()
                        MP2T_MUXER_SHELL.WaitForExit()
                        MP2T_MUXER_SHELL.Close()
                        MP2T_MUXER_SHELL.Dispose()
                    End If
                End If

                If My.Computer.FileSystem.FileExists(TS_CACHE_FILE) Then
                    CONVERSION_TS_BUFFER = My.Computer.FileSystem.ReadAllBytes(TS_CACHE_FILE)
                End If

                Try
                    If CACHE_REMOVE Then
                        My.Computer.FileSystem.DeleteFile(AVC_CACHE_FILE)
                        My.Computer.FileSystem.DeleteFile(TS_CACHE_FILE)
                    End If
                Catch ex As Exception

                End Try

                If CONVERSION_TS_BUFFER.Length >= TS_PACKET_SIZE Then
                    Dim CONVERSION_READ As New MemoryStream()
                    CONVERSION_READ.Write(CONVERSION_TS_BUFFER, 0, CONVERSION_TS_BUFFER.Length)
                    CONVERSION_READ.Position = 0
                    Dim CONVERSION_READER As New BinaryReader(CONVERSION_READ)
                    Dim PES_PTS_REFERENCE As Long = Int64.MinValue
                    Dim TS_PID_PCR_COUNTER As Integer = 0
                    Dim TS_PID_PCR_REFERENCE As Long = 0
                    Do
                        Dim TS_PACKET_READ_AVAILABLE As Integer = TS_PACKET_SIZE
                        TS_PACKET_READ_AVAILABLE -= TS_PACKET_HEADER_SIZE
                        Dim TS_PACKET_HEADER As Byte() = CONVERSION_READER.ReadBytes(TS_PACKET_HEADER_SIZE)
                        If TS_PACKET_HEADER(0) = &H47 Then
                            Dim TS_PACKET_HEADER_BIT As New BitArray(TS_PACKET_HEADER)
                            Dim TS_PACKET_PID As Integer = (TS_PACKET_HEADER(1) And &H1F) << 8 Or TS_PACKET_HEADER(2)
                            If TS_PACKET_PID = 256 Or TS_PACKET_PID = 2064 Then
                                Dim TS_PACKET_DATA As Byte() = New Byte(TS_PACKET_SIZE - 1) {}
                                Array.Copy(TS_PACKET_HEADER, 0, TS_PACKET_DATA, 0, TS_PACKET_HEADER.Length)

                                Dim TS_PACKET_PCR_INCLUSIVE As Boolean = False
                                If TS_PACKET_HEADER_BIT(29) Then
                                    Dim TS_ADAPTATION_FIELD As Byte() = CONVERSION_READER.ReadBytes(1)
                                    TS_PACKET_DATA(TS_PACKET_SIZE - TS_PACKET_READ_AVAILABLE) = TS_ADAPTATION_FIELD(0)
                                    TS_PACKET_READ_AVAILABLE -= 1

                                    Dim TS_ADAPTATION_FIELD_LENGTH As Integer = Int(TS_ADAPTATION_FIELD(0))
                                    If TS_ADAPTATION_FIELD_LENGTH > 0 Then
                                        Try
                                            TS_ADAPTATION_FIELD = CONVERSION_READER.ReadBytes(TS_ADAPTATION_FIELD_LENGTH)
                                            Array.Copy(TS_ADAPTATION_FIELD, 0, TS_PACKET_DATA, TS_PACKET_SIZE - TS_PACKET_READ_AVAILABLE, TS_ADAPTATION_FIELD.Length)
                                            TS_PACKET_READ_AVAILABLE -= TS_ADAPTATION_FIELD_LENGTH

                                            Dim TS_PACKET_ADAPTATION_BIT As New BitArray(TS_ADAPTATION_FIELD)
                                            TS_PACKET_PCR_INCLUSIVE = TS_PACKET_ADAPTATION_BIT(4)
                                        Catch ex As Exception

                                        End Try
                                    End If
                                End If

                                Dim PES_DATA As Byte() = CONVERSION_READER.ReadBytes(TS_PACKET_READ_AVAILABLE)
                                Array.Copy(PES_DATA, 0, TS_PACKET_DATA, TS_PACKET_SIZE - TS_PACKET_READ_AVAILABLE, PES_DATA.Length)
                                If TS_PACKET_HEADER_BIT(14) Then
                                    Dim PES_PAYLOAD_OFFSET As Integer = 9 + Int(PES_DATA(8))
                                    Dim PES_PAYLOAD As Byte() = New Byte(PES_DATA.Length - PES_PAYLOAD_OFFSET - 1) {}
                                    Array.Copy(PES_DATA, PES_PAYLOAD_OFFSET, PES_PAYLOAD, 0, PES_DATA.Length - PES_PAYLOAD_OFFSET)
                                    Dim PES_HEADER As Byte() = New Byte(PES_PAYLOAD_OFFSET - 1) {}
                                    Array.Copy(PES_DATA, PES_HEADER, PES_PAYLOAD_OFFSET)

                                    Dim PES_HEADER_BIT As New BitArray(PES_HEADER)
                                    Dim PES_PTS As Long = 0
                                    Dim PES_DTS As Long = 0
                                    Dim PES_PTS_OUTPUT As Long = 0
                                    Dim PES_DTS_OUTPUT As Long = 0
                                    If PES_HEADER_BIT(63) Then
                                        PES_PTS = GetPTS(PES_HEADER, 10)
                                        If PES_PTS_REFERENCE = Int64.MinValue Then PES_PTS_REFERENCE = INTRA_PTS(ThreadID) - PES_PTS + PTS_DELAY
                                        Dim PES_PTS_BUFFER As Byte() = New Byte(4) {}
                                        Array.Copy(PES_HEADER, 9, PES_PTS_BUFFER, 0, 5)
                                        PES_PTS_OUTPUT = PES_PTS + PES_PTS_REFERENCE
                                        PES_PTS_BUFFER = SetPTS(PES_PTS_OUTPUT, PES_PTS_BUFFER)
                                        Array.Copy(PES_PTS_BUFFER, 0, TS_PACKET_DATA, TS_PACKET_SIZE - TS_PACKET_READ_AVAILABLE + 9, 5)
                                        If PES_HEADER_BIT(62) Then
                                            PES_DTS = GetPTS(PES_HEADER, 15)
                                            Array.Copy(PES_HEADER, 14, PES_PTS_BUFFER, 0, 5)
                                            PES_DTS_OUTPUT = PES_DTS + PES_PTS_REFERENCE
                                            PES_PTS_BUFFER = SetPTS(PES_DTS_OUTPUT, PES_PTS_BUFFER)
                                            Array.Copy(PES_PTS_BUFFER, 0, TS_PACKET_DATA, TS_PACKET_SIZE - TS_PACKET_READ_AVAILABLE + 14, 5)

                                            If PES_PTS < PES_DTS Then
                                                CONVERSION_STREAM(ThreadID).Close()
                                                CONVERSION_STREAM(ThreadID) = New MemoryStream
                                                Exit Do
                                            End If
                                        End If

                                        If PES_DTS_OUTPUT = 0 Then PES_DTS_OUTPUT = PES_PTS_OUTPUT
                                        TS_PID_PCR_REFERENCE = PES_DTS_OUTPUT - GetPCRBase() * PCR_OFFSET
                                        TS_PID_PCR_COUNTER = 0
                                    End If
                                End If

                                If TS_PACKET_PCR_INCLUSIVE Then
                                    'Dim TS_PID_PCR As Long = GetPCR(TS_PACKET_DATA, 7)
                                    Dim TS_PID_PCR_BUFFER As Byte() = SetPCR(TS_PID_PCR_REFERENCE + TS_PID_PCR_COUNTER * Math.Floor(GetPCRBase() / 9))
                                    Array.Copy(TS_PID_PCR_BUFFER, 0, TS_PACKET_DATA, 6, 6)
                                    TS_PID_PCR_COUNTER += 1
                                End If

                                If Not TS_PACKET_PID = AVS_PID Then
                                    Dim AVS_PID_BIT As New BitArray(New Integer() {AVS_PID})
                                    TS_PACKET_HEADER_BIT(12) = AVS_PID_BIT(12)
                                    TS_PACKET_HEADER_BIT(11) = AVS_PID_BIT(11)
                                    TS_PACKET_HEADER_BIT(10) = AVS_PID_BIT(10)
                                    TS_PACKET_HEADER_BIT(9) = AVS_PID_BIT(9)
                                    TS_PACKET_HEADER_BIT(8) = AVS_PID_BIT(8)
                                    TS_PACKET_HEADER_BIT(23) = AVS_PID_BIT(7)
                                    TS_PACKET_HEADER_BIT(22) = AVS_PID_BIT(6)
                                    TS_PACKET_HEADER_BIT(21) = AVS_PID_BIT(5)
                                    TS_PACKET_HEADER_BIT(20) = AVS_PID_BIT(4)
                                    TS_PACKET_HEADER_BIT(19) = AVS_PID_BIT(3)
                                    TS_PACKET_HEADER_BIT(18) = AVS_PID_BIT(2)
                                    TS_PACKET_HEADER_BIT(17) = AVS_PID_BIT(1)
                                    TS_PACKET_HEADER_BIT(16) = AVS_PID_BIT(0)
                                    TS_PACKET_HEADER_BIT.CopyTo(TS_PACKET_DATA, 0)
                                End If

                                CONVERSION_STREAM(ThreadID).Write(TS_PACKET_DATA, 0, TS_PACKET_SIZE)
                            Else
                                CONVERSION_READER.ReadBytes(TS_PACKET_SIZE - TS_PACKET_HEADER_SIZE)
                            End If
                        End If
                    Loop Until CONVERSION_READER.BaseStream.Position >= CONVERSION_READ.Length - 1
                    Exit While
                Else
                    CONVERSION_STREAM(ThreadID).Close()
                    CONVERSION_STREAM(ThreadID) = New MemoryStream
                    CONVERSION_ERROR += 1
                    If INPUT_READER.BaseStream.Position >= INPUT_READ.Length - TS_PACKET_SIZE - 1 Then CONVERSION_ERROR += Short.MaxValue
                End If
            End While
            MT_STATUS(ThreadID) = 4
        Catch ex As Exception
            ERROR_MESSAGE = ex.ToString
            MT_STATUS(ThreadID) = -3
        End Try
    End Sub

    'GB/T 20090.16-2016
    Public Function GetFrameType(param1 As Byte()) As Integer
        Try
            Dim _loc_1 As String = BytesToHex(param1)
            Dim _loc_2 As Integer = _loc_1.IndexOf("000001B3")
            If _loc_2 Mod 2 = 0 Then Return 1    'I frame
            _loc_2 = _loc_1.IndexOf("000001B6")
            If _loc_2 Mod 2 = 0 Then
                _loc_1 = BytesToBin(New Byte() {param1(_loc_2 / 2 + 7)}).Substring(0, 2)
                If _loc_1 = "01" Then Return 2   'P frame
                If _loc_1 = "10" Then Return 3   'B frame
            End If
            Return 0                             'Undefined
        Catch ex As Exception
            Return 0
        End Try
    End Function

    Public Function GetPCR(param1 As Byte(), param2 As Integer) As Long
        Try
            Dim _loc_1 As Integer = (param2 - 1) * 8
            Dim _loc_2 As New BitArray(param1)
            Dim _loc_3 As New BitArray(New Byte(7) {})
            _loc_3(0) = _loc_2(_loc_1 + 39)
            _loc_3(1) = _loc_2(_loc_1 + 24)
            _loc_3(2) = _loc_2(_loc_1 + 25)
            _loc_3(3) = _loc_2(_loc_1 + 26)
            _loc_3(4) = _loc_2(_loc_1 + 27)
            _loc_3(5) = _loc_2(_loc_1 + 28)
            _loc_3(6) = _loc_2(_loc_1 + 29)
            _loc_3(7) = _loc_2(_loc_1 + 30)
            _loc_3(8) = _loc_2(_loc_1 + 31)
            _loc_3(9) = _loc_2(_loc_1 + 16)
            _loc_3(10) = _loc_2(_loc_1 + 17)
            _loc_3(11) = _loc_2(_loc_1 + 18)
            _loc_3(12) = _loc_2(_loc_1 + 19)
            _loc_3(13) = _loc_2(_loc_1 + 20)
            _loc_3(14) = _loc_2(_loc_1 + 21)
            _loc_3(15) = _loc_2(_loc_1 + 22)
            _loc_3(16) = _loc_2(_loc_1 + 23)
            _loc_3(17) = _loc_2(_loc_1 + 8)
            _loc_3(18) = _loc_2(_loc_1 + 9)
            _loc_3(19) = _loc_2(_loc_1 + 10)
            _loc_3(20) = _loc_2(_loc_1 + 11)
            _loc_3(21) = _loc_2(_loc_1 + 12)
            _loc_3(22) = _loc_2(_loc_1 + 13)
            _loc_3(23) = _loc_2(_loc_1 + 14)
            _loc_3(24) = _loc_2(_loc_1 + 15)
            _loc_3(25) = _loc_2(_loc_1 + 0)
            _loc_3(26) = _loc_2(_loc_1 + 1)
            _loc_3(27) = _loc_2(_loc_1 + 2)
            _loc_3(28) = _loc_2(_loc_1 + 3)
            _loc_3(29) = _loc_2(_loc_1 + 4)
            _loc_3(30) = _loc_2(_loc_1 + 5)
            _loc_3(31) = _loc_2(_loc_1 + 6)
            _loc_3(32) = _loc_2(_loc_1 + 7)
            Dim _loc_4 As Byte() = New Byte(7) {}
            _loc_3.CopyTo(_loc_4, 0)
            Return BitConverter.ToInt64(_loc_4, 0)
        Catch ex As Exception
            Return 0
        End Try
    End Function

    Public Function GetPCRBase() As Integer
        Try
            Return Math.Floor(90000 / Convert.ToDouble(AVS_VIDEO_INFO(2)))
        Catch ex As Exception
            Return 3600
        End Try
    End Function

    Public Function GetPTS(param1 As Byte(), param2 As Integer) As Long
        Try
            Dim _loc_1 As Integer = (param2 - 1) * 8
            Dim _loc_2 As New BitArray(param1)
            Dim _loc_3 As New BitArray(New Byte(7) {})
            _loc_3(0) = _loc_2(_loc_1 + 33)
            _loc_3(1) = _loc_2(_loc_1 + 34)
            _loc_3(2) = _loc_2(_loc_1 + 35)
            _loc_3(3) = _loc_2(_loc_1 + 36)
            _loc_3(4) = _loc_2(_loc_1 + 37)
            _loc_3(5) = _loc_2(_loc_1 + 38)
            _loc_3(6) = _loc_2(_loc_1 + 39)
            _loc_3(7) = _loc_2(_loc_1 + 24)
            _loc_3(8) = _loc_2(_loc_1 + 25)
            _loc_3(9) = _loc_2(_loc_1 + 26)
            _loc_3(10) = _loc_2(_loc_1 + 27)
            _loc_3(11) = _loc_2(_loc_1 + 28)
            _loc_3(12) = _loc_2(_loc_1 + 29)
            _loc_3(13) = _loc_2(_loc_1 + 30)
            _loc_3(14) = _loc_2(_loc_1 + 31)
            _loc_3(15) = _loc_2(_loc_1 + 17)
            _loc_3(16) = _loc_2(_loc_1 + 18)
            _loc_3(17) = _loc_2(_loc_1 + 19)
            _loc_3(18) = _loc_2(_loc_1 + 20)
            _loc_3(19) = _loc_2(_loc_1 + 21)
            _loc_3(20) = _loc_2(_loc_1 + 22)
            _loc_3(21) = _loc_2(_loc_1 + 23)
            _loc_3(22) = _loc_2(_loc_1 + 8)
            _loc_3(23) = _loc_2(_loc_1 + 9)
            _loc_3(24) = _loc_2(_loc_1 + 10)
            _loc_3(25) = _loc_2(_loc_1 + 11)
            _loc_3(26) = _loc_2(_loc_1 + 12)
            _loc_3(27) = _loc_2(_loc_1 + 13)
            _loc_3(28) = _loc_2(_loc_1 + 14)
            _loc_3(29) = _loc_2(_loc_1 + 15)
            _loc_3(30) = _loc_2(_loc_1 + 1)
            _loc_3(31) = _loc_2(_loc_1 + 2)
            _loc_3(32) = _loc_2(_loc_1 + 3)
            Dim _loc_4 As Byte() = New Byte(7) {}
            _loc_3.CopyTo(_loc_4, 0)
            Return BitConverter.ToInt64(_loc_4, 0)
        Catch ex As Exception
            Return 0
        End Try
    End Function

    Public Function GetRandomFileName() As String
        Return "H264_" & Format(Now(), "yyyyMMddHHmmss") & "_" & GetRandomString(4)
    End Function

    Public Function GetRandomString(StringLength As Long) As String
        Try
            Randomize()
            Dim _loc_1 As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
            Dim _loc_2 As New Random(GetSeed)
            Dim _loc_3 As New StringBuilder
            For _loc_4 As Integer = 1 To StringLength
                _loc_3.Append(_loc_1.Substring(_loc_2.Next(0, _loc_1.Length - 1), 1))
            Next
            Return _loc_3.ToString
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Function GetSeed() As Integer
        Dim _loc_1 As Byte() = New Byte(3) {}
        Dim _loc_2 As New RNGCryptoServiceProvider()
        _loc_2.GetBytes(_loc_1)
        Return System.Math.Abs(BitConverter.ToInt32(_loc_1, 0))
    End Function

    Private Sub GetStream(ThreadID As Integer)
        Try
            Dim MT_THREAD_ID As Integer = ThreadID
            Dim INPUT_FRAME_GROUP As Integer = 10000000
            Dim AVS_I_FRAME_FLUSH_FLAG As Integer = 0
            Do
                Dim TS_PACKET_READ_AVAILABLE As Integer = TS_PACKET_SIZE
                TS_PACKET_READ_AVAILABLE -= TS_PACKET_HEADER_SIZE
                Dim TS_PACKET_HEADER As Byte() = INPUT_READER.ReadBytes(TS_PACKET_HEADER_SIZE)
                If TS_PACKET_HEADER(0) = &H47 Then
                    Dim TS_PACKET_HEADER_BIT As New BitArray(TS_PACKET_HEADER)
                    Dim TS_PACKET_PID As Integer = (TS_PACKET_HEADER(1) And &H1F) << 8 Or TS_PACKET_HEADER(2)

                    If TS_PACKET_PID = AVS_PID Or TS_PACKET_PID = DRA_PID Then
                        If TS_PACKET_PID = DRA_PID Then MT_THREAD_ID += MT_THREADS
                        Dim TS_PACKET_DATA As Byte() = New Byte(TS_PACKET_SIZE - 1) {}
                        Array.Copy(TS_PACKET_HEADER, 0, TS_PACKET_DATA, 0, TS_PACKET_HEADER.Length)

                        If TS_PACKET_HEADER_BIT(29) Then
                            Dim TS_ADAPTATION_FIELD As Byte() = INPUT_READER.ReadBytes(1)
                            TS_PACKET_DATA(TS_PACKET_SIZE - TS_PACKET_READ_AVAILABLE) = TS_ADAPTATION_FIELD(0)
                            TS_PACKET_READ_AVAILABLE -= 1

                            Dim TS_ADAPTATION_FIELD_LENGTH As Integer = Int(TS_ADAPTATION_FIELD(0))
                            If TS_ADAPTATION_FIELD_LENGTH > 0 Then
                                Try
                                    TS_ADAPTATION_FIELD = INPUT_READER.ReadBytes(TS_ADAPTATION_FIELD_LENGTH)
                                    Array.Copy(TS_ADAPTATION_FIELD, 0, TS_PACKET_DATA, TS_PACKET_SIZE - TS_PACKET_READ_AVAILABLE, TS_ADAPTATION_FIELD.Length)
                                    TS_PACKET_READ_AVAILABLE -= TS_ADAPTATION_FIELD_LENGTH
                                Catch ex As Exception

                                End Try
                            End If
                        End If

                        Dim PES_DATA As Byte() = INPUT_READER.ReadBytes(TS_PACKET_READ_AVAILABLE)
                        Array.Copy(PES_DATA, 0, TS_PACKET_DATA, TS_PACKET_SIZE - TS_PACKET_READ_AVAILABLE, PES_DATA.Length)
                        If TS_PACKET_HEADER_BIT(14) Then
                            Dim PES_PAYLOAD_OFFSET As Integer = 9 + Int(PES_DATA(8))
                            If PES_PAYLOAD_OFFSET > PES_DATA.Length - 1 Then PES_PAYLOAD_OFFSET = PES_DATA.Length - 1
                            Dim PES_PAYLOAD As Byte() = New Byte(PES_DATA.Length - PES_PAYLOAD_OFFSET - 1) {}
                            Array.Copy(PES_DATA, PES_PAYLOAD_OFFSET, PES_PAYLOAD, 0, PES_DATA.Length - PES_PAYLOAD_OFFSET)
                            Dim PES_HEADER As Byte() = New Byte(PES_PAYLOAD_OFFSET - 1) {}
                            Array.Copy(PES_DATA, PES_HEADER, PES_PAYLOAD_OFFSET)
                            If TS_PACKET_PID = AVS_PID Then
                                Dim AVS_FRAME_TYPE_CURRENT As Integer = GetFrameType(PES_PAYLOAD)
                                If AVS_FRAME_TYPE_CURRENT > 0 Then
                                    If AVS_I_FRAME_FLUSH_FLAG > 0 Then    'Pending flush, end read the next I frame
                                        INPUT_READER.BaseStream.Position -= AVS_I_FRAME_FLUSH_FLAG
                                        AVS_I_FRAME_FLUSH_FLAG = 0
                                        AVS_FRAME_TYPE_PREVIOUS = 4
                                        Exit Do
                                    End If
                                    INPUT_FRAME_GROUP += 1
                                    INPUT_GOP(MT_THREAD_ID) = INPUT_FRAME_GROUP
                                    If AVS_FRAME_TYPE_CURRENT = 1 And INPUT_FRAME_GROUP >= GOP_MIN Then
                                        If AVS_FRAME_TYPE_PREVIOUS = 3 Then    'Last frame is a B frame, so need to read the entire next I frame to flush
                                            AVS_I_FRAME_FLUSH_FLAG += TS_PACKET_SIZE
                                        Else
                                            AVS_FRAME_TYPE_PREVIOUS = AVS_FRAME_TYPE_CURRENT
                                            PASSTHROUGH_ACTIVE = True
                                            If ES_STREAM(MT_THREAD_ID).Length <= 0 Then    'Is this the GOP starting I frame, or the GOP ending I frame
                                                INTRA_PTS(MT_THREAD_ID) = GetPTS(PES_HEADER, 10)
                                                INPUT_FRAME_GROUP = 0
                                            Else
                                                Exit Do
                                            End If
                                        End If
                                    Else
                                        AVS_FRAME_TYPE_PREVIOUS = AVS_FRAME_TYPE_CURRENT
                                    End If
                                    ES_STREAM(MT_THREAD_ID).Write(PES_PAYLOAD, 0, PES_PAYLOAD.Length)
                                Else
                                    If AVS_I_FRAME_FLUSH_FLAG > 0 Then AVS_I_FRAME_FLUSH_FLAG += TS_PACKET_SIZE
                                    ES_STREAM(MT_THREAD_ID).Write(PES_PAYLOAD, 0, PES_PAYLOAD.Length)
                                End If
                            End If
                        Else
                            If AVS_I_FRAME_FLUSH_FLAG > 0 Then AVS_I_FRAME_FLUSH_FLAG += TS_PACKET_SIZE
                            ES_STREAM(MT_THREAD_ID).Write(PES_DATA, 0, PES_DATA.Length)
                        End If
                    Else
                        Dim TS_PACKET_PAYLOAD As Byte() = INPUT_READER.ReadBytes(TS_PACKET_SIZE - TS_PACKET_HEADER_SIZE)
                        If AVS_I_FRAME_FLUSH_FLAG > 0 Then
                            AVS_I_FRAME_FLUSH_FLAG += TS_PACKET_SIZE
                        ElseIf PASSTHROUGH_ACTIVE And PASSTHROUGH_PID.Contains(TS_PACKET_PID) Then
                            TS_STREAM(MT_THREAD_ID).Write(TS_PACKET_HEADER, 0, TS_PACKET_HEADER.Length)
                            TS_STREAM(MT_THREAD_ID).Write(TS_PACKET_PAYLOAD, 0, TS_PACKET_PAYLOAD.Length)
                        End If
                    End If
                Else
                    INPUT_READER.BaseStream.Position -= TS_PACKET_HEADER_SIZE - 1
                End If
            Loop Until INPUT_READER.BaseStream.Position >= INPUT_READ.Length - 1
            INPUT_READER.BaseStream.Position -= TS_PACKET_SIZE    'Move origin to current packet
            MT_STATUS(MT_THREAD_ID) = 2
        Catch ex As Exception
            ERROR_MESSAGE = ex.ToString
            MT_STATUS(ThreadID) = -1
        End Try
    End Sub

    Private Sub GetStreamInfo()
        Try
            Dim _loc_1 As New Process()
            _loc_1.StartInfo.FileName = "ffmpeg.exe"
            _loc_1.StartInfo.Arguments = "-i " & Chr(34) & INPUT_READ.Name & Chr(34)
            _loc_1.StartInfo.UseShellExecute = False
            _loc_1.StartInfo.CreateNoWindow = True
            _loc_1.StartInfo.RedirectStandardError = True
            _loc_1.Start()
            Dim Errorstr As String = _loc_1.StandardError.ReadToEnd()
            _loc_1.Close()

            For Each _loc_2 In Errorstr.Split(vbCrLf)
                Dim _loc_3 As String = _loc_2.Trim.Replace(" ", "").ToLower()
                Dim _loc_4 As String = Regex.Match(_loc_3, "program([0-9]*)", RegexOptions.IgnoreCase).Groups(1).Value.Trim()
                If _loc_4.Length > 0 Or _loc_3.Contains("noprogram") Then
                    If AVS_PID >= 32 Then
                        Exit For
                    Else
                        PMT_PROGRAM_NUMBER = Int(_loc_4)
                        AVS_PID = -1
                        PASSTHROUGH_PID.Clear()
                        PASSTHROUGH_PID_TYPE.Clear()
                    End If
                End If

                Dim _loc_5 As Match = Regex.Match(_loc_3, "stream#([0-9]*):([0-9]*)\[0x([0-9a-z]*)\](.*)0x([0-9a-z]*)\)", RegexOptions.IgnoreCase)

                If _loc_5.Groups(3).Value.Length > 0 And _loc_5.Groups(3).Value.Length > 0 Then
                    Dim _loc_6 As Integer = HexToInt(_loc_5.Groups(3).Value)
                    Dim _loc_7 As Integer = HexToInt(_loc_5.Groups(5).Value)
                    If _loc_7 = 858604353 Then _loc_7 = 6
                    If _loc_7 = 826364484 Then _loc_7 = 1006
                    If _loc_3.Contains("audio:eac3") Then _loc_7 = 2006
                    If _loc_7 = 66 Or _loc_7 = 1448302145 Then
                        AVS_PID = _loc_6
                        PASSTHROUGH_PID.Insert(0, AVS_PID)
                        PASSTHROUGH_PID_TYPE.Insert(0, 27)
                        Dim _loc_8 As String = Regex.Match(_loc_3, "([0-9]*)x([0-9]*),", RegexOptions.IgnoreCase).Value
                        Dim _loc_9 As String = Regex.Match(_loc_3, "([0-9.]*)fps,", RegexOptions.IgnoreCase).Groups(1).Value
                        If _loc_8 = "" Then _loc_8 = "1920x1080,"
                        If _loc_9 = "" Then _loc_9 = "25"
                        AVS_VIDEO_INFO.Add(_loc_8.Substring(0, _loc_8.Length - 1).Split("x")(0))
                        AVS_VIDEO_INFO.Add(_loc_8.Substring(0, _loc_8.Length - 1).Split("x")(1))
                        AVS_VIDEO_INFO.Add(_loc_9)
                    Else
                        PASSTHROUGH_PID.Add(_loc_6)
                        PASSTHROUGH_PID_TYPE.Add(_loc_7)
                    End If
                End If
            Next
        Catch ex As Exception

        End Try
    End Sub

    Private Function GetTempFile() As String
        Dim _loc_1 As String = Path.GetTempPath() & "\AVS\"
        Dim _loc_2 As String = KeyRead("TEMP_PATH")
        If _loc_2.Length > 1 Then _loc_1 = _loc_2 & "\"
        If Not Directory.Exists(_loc_1) Then My.Computer.FileSystem.CreateDirectory(_loc_1)
        Return _loc_1 & Time() & "_" & GetRandomString(10)
    End Function

    Public Function HexToBytes(param1 As String) As Byte()
        Return Enumerable.Range(0, param1.Length).Where(Function(x) x Mod 2 = 0).[Select](Function(x) Convert.ToByte(param1.Substring(x, 2), 16)).ToArray()
    End Function

    Public Function HexToInt(param1 As String) As Integer
        If param1.Length Mod 2 = 1 Then param1 = "0" & param1
        Return Int(CLng("&H" & param1))
    End Function

    Private Function HexToUInt(Input As String) As UInteger()
        Input = Input.Replace(" ", "")
        If (Input.Length Mod 2) <> 0 Then Input += " "
        Dim returnBytes As UInteger() = New UInteger(Input.Length / 2 - 1) {}
        For i As Integer = 0 To returnBytes.Length - 1
            returnBytes(i) = Convert.ToByte(Input.Substring(i * 2, 2), 16)
        Next
        Return returnBytes
    End Function

    Private Function ID_BIN(ID As String, RequiredLength As Integer)
        Try
            ID = ID.Trim
            Dim ReturnBIN As String = ""
            If ID.ToUpper.StartsWith("0X") Then
                Dim _LOC_1 As String = ID.ToUpper.Split("X")(1)
                If _LOC_1.Length Mod 2 = 1 Then _LOC_1 = "0" & _LOC_1
                ID = Int(CLng("&H" & _LOC_1))
            End If
            Dim Number As Long = Convert.ToInt64(ID)
            Do While Number > 0
                ReturnBIN = Number Mod 2 & ReturnBIN
                Number \= 2
            Loop
            If RequiredLength < ReturnBIN.Length Then
                ReturnBIN = ReturnBIN.Substring(0, RequiredLength)
            End If
            If RequiredLength > ReturnBIN.Length Then
                For _LOC_2 = 1 To RequiredLength - ReturnBIN.Length
                    ReturnBIN = "0" + ReturnBIN
                Next
            End If
            Return ReturnBIN
        Catch ex As Exception
            Dim ReturnBIN As String = ""
            For _LOC_3 = 1 To RequiredLength
                ReturnBIN += "0"
            Next
            Return ReturnBIN
        End Try
    End Function

    Public Function IsFFmpegExist() As Boolean
        Try
            Dim _loc_1 As New Process()
            _loc_1.StartInfo.FileName = "ffmpeg.exe"
            _loc_1.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            _loc_1.Start()
            _loc_1.Close()
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function KeyRead(KeyName As String) As String
        Try
            Dim _loc_1 As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\AVS Patrina", True)
            Return _loc_1.GetValue(KeyName, "")
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Sub KeyWrite(KeyName As String, KeyValue As String)
        Try
            Dim _loc_1 As RegistryKey = Registry.CurrentUser.CreateSubKey("Software\AVS Patrina")
            _loc_1.OpenSubKey("Software\HmDX", True)
            _loc_1.SetValue(KeyName, KeyValue, RegistryValueKind.String)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub PAT_PMT_COUNTER_STEP()
        Try
            If PAT_PMT_COUNTER < 15 Then
                PAT_PMT_COUNTER += 1
            Else
                PAT_PMT_COUNTER = 0
            End If
            Dim PAT_PMT_COUNTER_BIT As New BitArray(New Byte() {PAT_PMT_COUNTER})
            Dim PAT_PMT_HEADER_BYTE_3_BIT As New BitArray(New Byte() {PAT_PMT(3)})
            PAT_PMT_HEADER_BYTE_3_BIT(0) = PAT_PMT_COUNTER_BIT(0)
            PAT_PMT_HEADER_BYTE_3_BIT(1) = PAT_PMT_COUNTER_BIT(1)
            PAT_PMT_HEADER_BYTE_3_BIT(2) = PAT_PMT_COUNTER_BIT(2)
            PAT_PMT_HEADER_BYTE_3_BIT(3) = PAT_PMT_COUNTER_BIT(3)
            PAT_PMT_HEADER_BYTE_3_BIT.CopyTo(PAT_PMT, 3)
            PAT_PMT_HEADER_BYTE_3_BIT = New BitArray(New Byte() {PAT_PMT(TS_PACKET_SIZE + 3)})
            PAT_PMT_HEADER_BYTE_3_BIT(0) = PAT_PMT_COUNTER_BIT(0)
            PAT_PMT_HEADER_BYTE_3_BIT(1) = PAT_PMT_COUNTER_BIT(1)
            PAT_PMT_HEADER_BYTE_3_BIT(2) = PAT_PMT_COUNTER_BIT(2)
            PAT_PMT_HEADER_BYTE_3_BIT(3) = PAT_PMT_COUNTER_BIT(3)
            PAT_PMT_HEADER_BYTE_3_BIT.CopyTo(PAT_PMT, TS_PACKET_SIZE + 3)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub PLAY_SOUND_OK()
        Try
            Dim _LOC_1 As System.Reflection.Assembly = System.Reflection.Assembly.GetExecutingAssembly()
            Dim _LOC_2 As System.IO.Stream = _LOC_1.GetManifestResourceStream("AVS_Patrina.OK.wav")
            Dim _LOC_3 As New SoundPlayer(_LOC_2)
            _LOC_3.Play()
        Catch ex As Exception

        End Try
    End Sub

    Public Function PostWebpage(PostData As Byte(), ThreadID As Integer) As Byte()
        Try
            ServicePointManager.ServerCertificateValidationCallback = New Security.RemoteCertificateValidationCallback(AddressOf CheckValidationResult)
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 Or SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls Or SecurityProtocolType.Ssl3

            Dim httpReq As System.Net.HttpWebRequest
            Dim httpResp As System.Net.HttpWebResponse
            Dim httpURL As New System.Uri(CLOUD_TRANSCODING)
            httpReq = CType(WebRequest.Create(httpURL), HttpWebRequest)
            httpReq.Method = "POST"
            httpReq.Timeout = 300000
            httpReq.KeepAlive = True
            httpReq.ServicePoint.ConnectionLimit = Int32.MaxValue
            httpReq.ServicePoint.Expect100Continue = False
            httpReq.ContentLength = PostData.Length
            httpReq.ContentType = "application/octet-stream"
            httpReq.Headers.Add("X-Encode-Parameter", HttpUtility.UrlEncode(ENCODE_PARAMETERS))
            httpReq.Headers.Add("X-Thread-ID", ThreadID.ToString())
            httpReq.Headers.Add("X-Video-Parameter", HttpUtility.UrlEncode(AVS_VIDEO_INFO(0) & " " & AVS_VIDEO_INFO(1) & " " & AVS_VIDEO_INFO(2)))
            Dim PostStream As Stream = httpReq.GetRequestStream()
            PostStream.Write(PostData, 0, PostData.Length)
            PostStream.Close()
            httpResp = CType(httpReq.GetResponse(), HttpWebResponse)
            Dim reader As New BinaryReader(httpResp.GetResponseStream)
            If Not httpResp.Headers("Content-Length") = "" Then
                Dim ContentLength As Integer = Int(httpResp.Headers("Content-Length"))
                Dim returnBuffer As Byte() = reader.ReadBytes(ContentLength)
                If returnBuffer.Length = ContentLength And ContentLength > 0 Then
                    Return returnBuffer
                Else
                    Return New Byte() {}
                End If
                Return returnBuffer
            Else
                Return New Byte() {}
            End If
        Catch ex As WebException
            Dim httpResp As HttpWebResponse = ex.Response
            If httpResp.StatusCode >= 400 Then
                MT_STATUS(ThreadID) = -31
            End If
            Return New Byte() {}
        Catch ex As Exception
            MT_STATUS(ThreadID) = -31
            Return New Byte() {}
        End Try
    End Function

    Private Function RegulateFileName(FileName As String)
        Try
            Return FileName.Replace("/", "_").Replace("\", "_").Replace("*", "_").Replace("?", "_").Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace(Chr(34), "_").Replace("|", "_")
        Catch ex As Exception
            Return FileName
        End Try
    End Function

    Private Function RegulateFilePath(Input As String, Optional DefaultExtension As String = "")
        Try
            Dim DirectoryName As String = Path.GetDirectoryName(INPUT_READ.Name) & "\"
            Dim FileName As String = ""
            Input = Input.Replace("/", "\")
            If Input.EndsWith(":") Then Input &= "\"
            Dim _loc_1 As String() = Input.Split("\")
            If (_loc_1(0) = "" And Not Input.StartsWith("\\")) Or (Not _loc_1(0) = "" And Not _loc_1(0).Contains(":")) Then
                Input = Path.GetDirectoryName(INPUT_READ.Name) & "\" & Input
            End If
            If Input.EndsWith("\") Then
                DirectoryName = Input
            Else
                DirectoryName = Path.GetDirectoryName(Input) & "\"
                FileName = RegulateFileName(_loc_1(_loc_1.Length - 1))
            End If
            If FileName = "" Then FileName = GetRandomFileName()
            If Not FileName.Contains(".") Then FileName &= DefaultExtension
            Return DirectoryName & FileName
        Catch ex As Exception
            Return Input
        End Try
    End Function

    Private Sub ResetStream()
        Try
            AVS_FRAME_TYPE_PREVIOUS = 0
            AVS_PID = -1
            AVS_PID_COUNTER = -1
            AVS_VIDEO_INFO.Clear()
            CONVERSION_STREAM.Clear()
            DRA_PID = -1
            ERROR_MESSAGE = ""
            ES_STREAM.Clear()
            GOP_MIN = 25
            INPUT_GOP.Clear()
            INTRA_PTS.Clear()
            MT_ABORT = 1
            MT_ACTIVE = True
            MT_STATUS.Clear()
            MT_THREADS = 10
            PASSTHROUGH_ACTIVE = False
            PASSTHROUGH_PID.Clear()
            PASSTHROUGH_PID_TYPE.Clear()
            PAT_PMT = Nothing
            PAT_PMT_COUNTER = -1
            PCR_OFFSET = 8
            PTS_DELAY = 0
            PMT_PID = 32
            PMT_PROGRAM_NUMBER = 1
            PROCESSOR_STOPWATCH_PROGRESS = -1
            TS_ID = 1
            TS_STREAM.Clear()
        Catch ex As Exception

        End Try

        Try
            Dim _loc_1 As String = KeyRead("THREAD_MAX")
            If IsNumeric(_loc_1) Then MT_THREADS = Int(_loc_1)

            _loc_1 = KeyRead("TS_ID")
            If IsNumeric(_loc_1) Then TS_ID = Int(_loc_1)

            _loc_1 = KeyRead("PMT_PID")
            If IsNumeric(_loc_1) Then PMT_PID = Int(_loc_1)

            _loc_1 = KeyRead("PCR_OFFSET")
            If IsNumeric(_loc_1) Then PCR_OFFSET = Int(_loc_1)

            _loc_1 = KeyRead("PTS_DELAY")
            If IsNumeric(_loc_1) Then PTS_DELAY = Int(_loc_1)

            _loc_1 = KeyRead("GOP_MIN")
            If IsNumeric(_loc_1) Then GOP_MIN = Int(_loc_1)

            '_loc_1 = KeyRead("Encoding")
            'If _loc_1.Length > 0 And _loc_1.Contains("-") Then
            '    ENCODE_PARAMETERS = _loc_1
            'Else
            '    ENCODE_PARAMETERS = "-I420"
            'End If

            'CLOUD_TRANSCODING = KeyRead("Transcoding")
        Catch ex As Exception

        End Try

        Try
            For _loc_1 = 1 To MT_THREADS
                CONVERSION_STREAM.Add(New MemoryStream)
                INPUT_GOP.Add(0)
                INTRA_PTS.Add(0)
                MT_STATUS.Add(0)
                ES_STREAM.Add(New MemoryStream)
                TS_STREAM.Add(New MemoryStream)
            Next
        Catch ex As Exception

        End Try
    End Sub

    Private Sub ResetThread(ThreadID)
        Try
            CONVERSION_STREAM(ThreadID).Close()
            CONVERSION_STREAM(ThreadID) = New MemoryStream
            ES_STREAM(ThreadID).Close()
            ES_STREAM(ThreadID) = New MemoryStream
            INPUT_GOP(ThreadID) = 0
            INTRA_PTS(ThreadID) = 0
            MT_STATUS(ThreadID) = 0
            TS_STREAM(ThreadID).Close()
            TS_STREAM(ThreadID) = New MemoryStream
        Catch ex As Exception

        End Try
    End Sub

    Public Function SetPCR(param1 As Long) As Byte()
        Try
            Dim _loc_2 As New BitArray(New Byte(5) {})
            Dim _loc_3 As New BitArray(BitConverter.GetBytes(param1))
            _loc_2(39) = _loc_3(0)
            _loc_2(24) = _loc_3(1)
            _loc_2(25) = _loc_3(2)
            _loc_2(26) = _loc_3(3)
            _loc_2(27) = _loc_3(4)
            _loc_2(28) = _loc_3(5)
            _loc_2(29) = _loc_3(6)
            _loc_2(30) = _loc_3(7)
            _loc_2(31) = _loc_3(8)
            _loc_2(16) = _loc_3(9)
            _loc_2(17) = _loc_3(10)
            _loc_2(18) = _loc_3(11)
            _loc_2(19) = _loc_3(12)
            _loc_2(20) = _loc_3(13)
            _loc_2(21) = _loc_3(14)
            _loc_2(22) = _loc_3(15)
            _loc_2(23) = _loc_3(16)
            _loc_2(8) = _loc_3(17)
            _loc_2(9) = _loc_3(18)
            _loc_2(10) = _loc_3(19)
            _loc_2(11) = _loc_3(20)
            _loc_2(12) = _loc_3(21)
            _loc_2(13) = _loc_3(22)
            _loc_2(14) = _loc_3(23)
            _loc_2(15) = _loc_3(24)
            _loc_2(0) = _loc_3(25)
            _loc_2(1) = _loc_3(26)
            _loc_2(2) = _loc_3(27)
            _loc_2(3) = _loc_3(28)
            _loc_2(4) = _loc_3(29)
            _loc_2(5) = _loc_3(30)
            _loc_2(6) = _loc_3(31)
            _loc_2(7) = _loc_3(32)
            For _loc_5 = 33 To 38
                _loc_2(_loc_5) = 1
            Next
            Dim _loc_4 As Byte() = New Byte(5) {}
            _loc_2.CopyTo(_loc_4, 0)
            Return _loc_4
        Catch ex As Exception
            Return New Byte(5) {}
        End Try
    End Function

    Public Function SetPTS(param1 As Long, param2 As Byte()) As Byte()
        Try
            Dim _loc_2 As New BitArray(param2)
            Dim _loc_3 As New BitArray(BitConverter.GetBytes(param1))
            _loc_2(33) = _loc_3(0)
            _loc_2(34) = _loc_3(1)
            _loc_2(35) = _loc_3(2)
            _loc_2(36) = _loc_3(3)
            _loc_2(37) = _loc_3(4)
            _loc_2(38) = _loc_3(5)
            _loc_2(39) = _loc_3(6)
            _loc_2(24) = _loc_3(7)
            _loc_2(25) = _loc_3(8)
            _loc_2(26) = _loc_3(9)
            _loc_2(27) = _loc_3(10)
            _loc_2(28) = _loc_3(11)
            _loc_2(29) = _loc_3(12)
            _loc_2(30) = _loc_3(13)
            _loc_2(31) = _loc_3(14)
            _loc_2(17) = _loc_3(15)
            _loc_2(18) = _loc_3(16)
            _loc_2(19) = _loc_3(17)
            _loc_2(20) = _loc_3(18)
            _loc_2(21) = _loc_3(19)
            _loc_2(22) = _loc_3(20)
            _loc_2(23) = _loc_3(21)
            _loc_2(8) = _loc_3(22)
            _loc_2(9) = _loc_3(23)
            _loc_2(10) = _loc_3(24)
            _loc_2(11) = _loc_3(25)
            _loc_2(12) = _loc_3(26)
            _loc_2(13) = _loc_3(27)
            _loc_2(14) = _loc_3(28)
            _loc_2(15) = _loc_3(29)
            _loc_2(1) = _loc_3(30)
            _loc_2(2) = _loc_3(31)
            _loc_2(3) = _loc_3(32)
            _loc_2(0) = 1
            _loc_2(16) = 1
            _loc_2(32) = 1
            Dim _loc_4 As Byte() = New Byte(4) {}
            _loc_2.CopyTo(_loc_4, 0)
            Return _loc_4
        Catch ex As Exception
            Return New Byte(4) {}
        End Try
    End Function

    Public Function Time() As Long
        Return (DateTime.UtcNow - New DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds
    End Function

    Private Sub WriteStream(ThreadID As Integer)
        Try
            If TS_STREAM(ThreadID).Length + CONVERSION_STREAM(ThreadID).Length > 0 Then
                TS_STREAM(ThreadID).Position = 0
                CONVERSION_STREAM(ThreadID).Position = 0
                Dim OUTPUT_BUFFER As Byte() = New Byte(TS_STREAM(ThreadID).Length + CONVERSION_STREAM(ThreadID).Length - 1) {}
                Dim OUTPUT_BUFFER_OFFSET As Integer = 0
                If CONVERSION_STREAM(ThreadID).Length > 0 Then
                    Dim CONVERSION_COUNTER As Integer = 0
                    Dim CONVERSION_TOTAL As Integer = Math.Floor(CONVERSION_STREAM(ThreadID).Length / TS_PACKET_SIZE)
                    Dim ROUND_TOTAL As Integer = Math.Floor(TS_STREAM(ThreadID).Length / TS_PACKET_SIZE)
                    If ROUND_TOTAL = 0 Then ROUND_TOTAL = CONVERSION_TOTAL
                    Dim CONVERSION_ROUND As Double = CONVERSION_TOTAL / ROUND_TOTAL
                    For ROUND_CURRENT = 1 To ROUND_TOTAL
                        Do
                            If CONVERSION_COUNTER < CONVERSION_TOTAL Then
                                CONVERSION_STREAM(ThreadID).Read(OUTPUT_BUFFER, OUTPUT_BUFFER_OFFSET, TS_PACKET_SIZE)

                                Dim TS_PACKET_HEADER_BYTE_3_BIT As New BitArray(New Byte() {OUTPUT_BUFFER(OUTPUT_BUFFER_OFFSET + 3)})
                                If TS_PACKET_HEADER_BYTE_3_BIT(4) Then
                                    If AVS_PID_COUNTER < 15 Then
                                        AVS_PID_COUNTER += 1
                                    Else
                                        AVS_PID_COUNTER = 0
                                    End If
                                End If
                                Dim AVS_PID_COUNTER_BIT As New BitArray(New Byte() {AVS_PID_COUNTER})
                                TS_PACKET_HEADER_BYTE_3_BIT(0) = AVS_PID_COUNTER_BIT(0)
                                TS_PACKET_HEADER_BYTE_3_BIT(1) = AVS_PID_COUNTER_BIT(1)
                                TS_PACKET_HEADER_BYTE_3_BIT(2) = AVS_PID_COUNTER_BIT(2)
                                TS_PACKET_HEADER_BYTE_3_BIT(3) = AVS_PID_COUNTER_BIT(3)
                                TS_PACKET_HEADER_BYTE_3_BIT.CopyTo(OUTPUT_BUFFER, OUTPUT_BUFFER_OFFSET + 3)

                                OUTPUT_BUFFER_OFFSET += TS_PACKET_SIZE
                                CONVERSION_COUNTER += 1
                            Else
                                Exit Do
                            End If
                        Loop Until CONVERSION_COUNTER >= ROUND_CURRENT * CONVERSION_ROUND

                        If TS_STREAM(ThreadID).Length > 0 Then
                            TS_STREAM(ThreadID).Read(OUTPUT_BUFFER, OUTPUT_BUFFER_OFFSET, TS_PACKET_SIZE)
                            OUTPUT_BUFFER_OFFSET += TS_PACKET_SIZE
                        End If
                    Next
                Else
                    TS_STREAM(ThreadID).Read(OUTPUT_BUFFER, 0, OUTPUT_BUFFER.Length)
                End If

                Dim OUTPUT_ROUND_LENGTH As Integer = Math.Ceiling(OUTPUT_BUFFER.Length / TS_PACKET_SIZE / INPUT_GOP(ThreadID) * 5) * TS_PACKET_SIZE
                Dim OUTPUT_ROUND_CURRENT As Integer = OUTPUT_ROUND_LENGTH
                OUTPUT_BUFFER_OFFSET = 0
                While OUTPUT_BUFFER_OFFSET < OUTPUT_BUFFER.Length
                    If OUTPUT_BUFFER.Length - OUTPUT_BUFFER_OFFSET < OUTPUT_ROUND_LENGTH Then OUTPUT_ROUND_CURRENT = OUTPUT_BUFFER.Length - OUTPUT_BUFFER_OFFSET
                    PAT_PMT_COUNTER_STEP()
                    OUTPUT_WRITER.Write(PAT_PMT)
                    OUTPUT_WRITER.Write(OUTPUT_BUFFER, OUTPUT_BUFFER_OFFSET, OUTPUT_ROUND_CURRENT)
                    OUTPUT_BUFFER_OFFSET += OUTPUT_ROUND_CURRENT
                End While
                OUTPUT_WRITER.Flush()
            End If

            MT_STATUS(ThreadID) = 6
        Catch ex As Exception
            ERROR_MESSAGE = ex.ToString
            MT_STATUS(ThreadID) = -5
        End Try
    End Sub

End Class
