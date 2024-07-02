Imports System.ComponentModel

Public Class Preferences
    Private Sub BTN_HANDLE_Click(sender As Object, e As EventArgs) Handles BTN_HANDLE.Click
        Try
            MainUI.KeyWrite("THREAD_MAX", NUM_THREADS.Value)
            MainUI.KeyWrite("TS_ID", NUM_TS_ID.Value)
            MainUI.KeyWrite("PMT_PID", NUM_PMT_PID.Value)
            MainUI.KeyWrite("PCR_OFFSET", NUM_PCR_OFFSET.Value)
            MainUI.KeyWrite("PTS_DELAY", NUM_PTS_DELAY.Value)
            MainUI.KeyWrite("GOP_MIN", NUM_GOP_MIN.Value)
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
            Dim _loc_1 As String = MainUI.KeyRead("THREAD_MAX")
            If IsNumeric(_loc_1) Then NUM_THREADS.Value = Int(_loc_1)

            _loc_1 = MainUI.KeyRead("TS_ID")
            If IsNumeric(_loc_1) Then NUM_TS_ID.Value = Int(_loc_1)

            _loc_1 = MainUI.KeyRead("PMT_PID")
            If IsNumeric(_loc_1) Then NUM_PMT_PID.Value = Int(_loc_1)

            _loc_1 = MainUI.KeyRead("PCR_OFFSET")
            If IsNumeric(_loc_1) Then NUM_PCR_OFFSET.Value = Int(_loc_1)

            _loc_1 = MainUI.KeyRead("PTS_DELAY")
            If IsNumeric(_loc_1) Then NUM_PTS_DELAY.Value = Int(_loc_1)

            _loc_1 = MainUI.KeyRead("GOP_MIN")
            If IsNumeric(_loc_1) Then NUM_GOP_MIN.Value = Int(_loc_1)
        Catch ex As Exception

        End Try
    End Sub

End Class