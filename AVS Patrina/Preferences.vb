Imports System.ComponentModel

Public Class Preferences
    Private Sub BTN_HANDLE_Click(sender As Object, e As EventArgs) Handles BTN_HANDLE.Click
        Try
            MainUI.KeyWrite("Threading", NUM_THREADS.Value)
            MainUI.KeyWrite("TS_ID", NUM_TS_ID.Value)
            MainUI.KeyWrite("PMT_PID", NUM_PMT_PID.Value)
            MainUI.KeyWrite("Encoding", TXT_ENCODE.Text)
            MainUI.KeyWrite("Transcoding", TXT_TRANSCODE.Text)
            Dispose()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Preferences_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Try
            Dispose()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Preferences_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Dim _loc_1 As String = MainUI.KeyRead("Threading")
            If IsNumeric(_loc_1) Then
                NUM_THREADS.Value = Int(_loc_1)
            End If

            _loc_1 = MainUI.KeyRead("TS_ID")
            If IsNumeric(_loc_1) Then
                NUM_TS_ID.Value = Int(_loc_1)
            End If

            _loc_1 = MainUI.KeyRead("PMT_PID")
            If IsNumeric(_loc_1) Then
                NUM_PMT_PID.Value = Int(_loc_1)
            End If

            _loc_1 = MainUI.KeyRead("Encoding")
            If _loc_1.Length > 0 Then
                TXT_ENCODE.Text = _loc_1
            End If

            _loc_1 = MainUI.KeyRead("Transcoding")
            If _loc_1.Length > 0 Then
                TXT_TRANSCODE.Text = _loc_1
            End If
        Catch ex As Exception

        End Try
    End Sub

End Class