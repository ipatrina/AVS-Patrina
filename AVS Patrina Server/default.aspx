<% @Page Language="VB" Debug="false" ResponseEncoding="utf-8" %>
<% @Import Namespace="System" %>
<% @Import Namespace="System.Diagnostics" %>
<% @Import Namespace="System.Security" %>
<% @Import Namespace="System.Security.Cryptography" %>
<% @Import Namespace="System.Text" %>
<% @Import Namespace="System.IO" %>
<% @Import Namespace="System.Net" %>
<% @Import Namespace="System.Net.Sockets" %>
<% @Import Namespace="System.Web" %>
<% @Import Namespace="System.Web.Security" %>

<Script runat=server>
	
    ' AVS Patrina Server
    ' Version: 2.0.1
    ' Date: 2022.08

    Dim ES_DECODER As String = "C:\Program Files (x86)\AVS\ldecod.exe"
    Dim MAINCONCEPT_ENC_AVC As String = "C:\Program Files (x86)\AVS\sample_enc_avc.exe"
    Dim MAINCONCEPT_ENC_AVC_CONFIG As String = "sample_enc_avc.ini"
    Dim Key As String = "00000000000000000000000000000000"
    Dim Threading As Integer = 10

    Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim ThreadID As Integer = Int32.MaxValue
            Int32.TryParse(GetRequestHeader("X-Thread-ID"), ThreadID)
            If (Not Key = "" And Not Key = GetQueryString("Key")) Or (ThreadID < 0 Or ThreadID >= Threading) Then
                Response.StatusCode = 403
                Exit Sub
            End If

            Dim AVS_VIDEO_INFO As String() = GetRequestHeader("X-Video-Parameter").Split(" ")
            Dim ENCODE_PARAMETERS As String = GetRequestHeader("X-Encode-Parameter")

            If GetQueryString("Profile").Length > 0 Then
                If My.Computer.FileSystem.FileExists(Path.GetDirectoryName(MAINCONCEPT_ENC_AVC) & "\" & GetQueryString("Profile") & ".ini") Then
                    MAINCONCEPT_ENC_AVC_CONFIG = GetQueryString("Profile") & ".ini"
                End If
            Else
                If AVS_VIDEO_INFO(0) = "720" Then
                    MAINCONCEPT_ENC_AVC_CONFIG = "sd.ini"
                End If
            End If

            Dim AVC_CONFIG_CACHE As String = ""
            For Each AVC_CONFIG_LINE In My.Computer.FileSystem.ReadAllText(Path.GetDirectoryName(MAINCONCEPT_ENC_AVC) & "\" & MAINCONCEPT_ENC_AVC_CONFIG).Split(vbLf)
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
            My.Computer.FileSystem.WriteAllText(AVC_CONFIG_CACHE_FILE, AVC_CONFIG_CACHE, False, Encoding.Default)

            If Request.ContentLength <= 0 Or AVS_VIDEO_INFO.Length = 0 Or ENCODE_PARAMETERS.Length = 0 Then
                Response.StatusCode = 204
                Exit Sub
            End If

            Dim ES_BUFFER As Byte() = StreamToBytes(Request.InputStream)
            If Not ES_BUFFER.Length = Request.ContentLength Then
                Response.StatusCode = 204
                Exit Sub
            End If

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

            Dim AVC_CACHE_FILE As String = GetTempFile() & ".avc"

            If My.Computer.FileSystem.FileExists(YUV_CACHE_FILE) Then
                If New FileInfo(YUV_CACHE_FILE).Length > 1024 Then
                    Dim RAW_ENCODER_SHELL As New Process()
                    RAW_ENCODER_SHELL.StartInfo.FileName = MAINCONCEPT_ENC_AVC
                    RAW_ENCODER_SHELL.StartInfo.WorkingDirectory = Path.GetDirectoryName(YUV_CACHE_FILE)
                    RAW_ENCODER_SHELL.StartInfo.Arguments = "-I420 -w " & AVS_VIDEO_INFO(0) & " -h " & AVS_VIDEO_INFO(1) & " -v " & Chr(34) & YUV_CACHE_FILE & Chr(34) & " -o " & Chr(34) & AVC_CACHE_FILE & Chr(34) & " -c " & Chr(34) & AVC_CONFIG_CACHE_FILE & Chr(34)
                    RAW_ENCODER_SHELL.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                    RAW_ENCODER_SHELL.Start()
                    RAW_ENCODER_SHELL.WaitForExit()
                    RAW_ENCODER_SHELL.Close()
                    RAW_ENCODER_SHELL.Dispose()
                End If
            End If

            Dim AVC_CACHE_FILE_LENGTH As Long = 0
            If My.Computer.FileSystem.FileExists(AVC_CACHE_FILE) Then
                AVC_CACHE_FILE_LENGTH = New FileInfo(AVC_CACHE_FILE).Length
                If AVC_CACHE_FILE_LENGTH > 0 Then
                    Response.ContentType = "application/octet-stream"
                    Response.AddHeader("Content-Length", AVC_CACHE_FILE_LENGTH.ToString)
                    Dim CONVERSION_AVC_BUFFER As Byte() = My.Computer.FileSystem.ReadAllBytes(AVC_CACHE_FILE)
                    Try
                        My.Computer.FileSystem.DeleteFile(ES_CACHE_FILE)
                        My.Computer.FileSystem.DeleteFile(YUV_CACHE_FILE)
                        My.Computer.FileSystem.DeleteFile(AVC_CACHE_FILE)
                        My.Computer.FileSystem.DeleteFile(AVC_CONFIG_CACHE_FILE)
                    Catch ex As Exception

                    End Try
                    Response.OutputStream.Write(CONVERSION_AVC_BUFFER, 0, CONVERSION_AVC_BUFFER.Length)
                    Response.Flush()
                End If
            End If

            If AVC_CACHE_FILE_LENGTH = 0 Then Response.StatusCode = 204
        Catch ex As Exception
            Response.StatusCode = 503
        End Try
    End Sub

    Private Function BytesToHex(param1 As Byte()) As String
        Return BitConverter.ToString(param1).Replace("-", "").ToUpper
    End Function

    Private Function GetQueryString(Parameter As String) As String
        Try
            If Request.QueryString(Parameter) = "" Then Return ""
            Return Request.QueryString(Parameter)
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Private Function GetRequestHeader(ByVal param1 As String) As String
        If Not String.IsNullOrEmpty(Request.Headers(param1)) Then
            Return Request.Headers(param1).Replace("+", " ")
        End If
        Return ""
    End Function

    Private Function GetRndString(StringLength As Long) As String
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

    Private Function GetSeed() As Integer
        Dim _loc_1 As Byte() = New Byte(3) {}
        Dim _loc_2 As New RNGCryptoServiceProvider()
        _loc_2.GetBytes(_loc_1)
        Return System.Math.Abs(BitConverter.ToInt32(_loc_1, 0))
    End Function

    Private Function GetTempFile() As String
        Dim _loc_1 As String = Path.GetTempPath() & "\AVS\"
        If Not Directory.Exists(_loc_1) Then My.Computer.FileSystem.CreateDirectory(_loc_1)
        Return _loc_1 & Time() & "_" & GetRndString(10)
    End Function

    Private Function HexToBytes(param1 As String) As Byte()
        Return Enumerable.Range(0, param1.Length).Where(Function(x) x Mod 2 = 0).[Select](Function(x) Convert.ToByte(param1.Substring(x, 2), 16)).ToArray()
    End Function

    Private Function StreamToBytes(ByVal param1 As Stream) As Byte()
        Dim _loc_1 As Byte() = New Byte(1048575) {}
        Using _loc_2 As MemoryStream = New MemoryStream()
            Dim _loc_3 As Integer
            Do
                _loc_3 = param1.Read(_loc_1, 0, _loc_1.Length)
                If _loc_3 > 0 Then _loc_2.Write(_loc_1, 0, _loc_3)
            Loop Until _loc_3 = 0
            Return _loc_2.ToArray()
        End Using
    End Function

    Private Function Time() As Long
        Return (DateTime.UtcNow - New DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds
    End Function
</Script>