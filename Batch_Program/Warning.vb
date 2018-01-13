﻿
Imports System.Runtime.InteropServices
Public Class Warning
    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
    End Sub
    Public Sub Donate()
        Me.Height = 111
        Label1.Height = 30
        Label1.Text = "Developing takes time and money. " & vbNewLine & "If this program has saved you either, consider saying thanks."
        PicDonate.Visible = True
        If My.Settings.Donated = True Then
            chkDontShow.Visible = True
            chkDontShow.Text = "I already did"
        Else
            chkDontShow.Visible = False
            PicDonate.Left = chkDontShow.Left
        End If

        btnOK.Width = 200
        Label2.Text = "Donate"
        btnOK.Left = btnOK.Left - 120
        btnOK.Text = "I'll just keep using it for free thanks"
        My.Settings.Save()
        ' Add any initialization after the InitializeComponent() call.
        Me.ShowDialog()
    End Sub
    Public Sub FirstRun()
        btnOK.Text = "OK"
        PicDonate.Visible = False
        Me.Text = "First time information"
        Me.Height = 150
        chkDontShow.Location = New Drawing.Point(chkDontShow.Location.X, chkDontShow.Location.Y + 40)
        Label2.Text = "FirstRun"
        Label1.Height = 65
        btnOK.Location = New Drawing.Point(btnOK.Location.X, btnOK.Location.Y + 40)
        Label1.Text = "Batch Program is a program created to automate many tasks in Inventor. The program is in a constant state of developement so bugs are likely. If you find any errors/bugs let me know by emailing a description of how to replicate the error to flyinggardengnomestudios@gmail.com. Please include the error log which is saved in: " & My.Computer.FileSystem.SpecialDirectories.Temp & "\debug.txt"
    End Sub
    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        Select Case Label2.Text
            Case "Rename"
                If chkDontShow.Checked = True Then
                    My.Settings.RenameShowMe = False
                End If
            Case "Donate"
                If btnOK.Text = "I'll just keep using it for free thanks" Then
                    My.Settings.DonateCount = My.Settings.DonateCount + 1
                Else
                    My.Settings.DonateShowMe = False
                End If
            Case "FirstRun"
                If chkDontShow.Checked = True Then
                    My.Settings.FirstRun = False
                End If
            Case "Bad File Type"
                If chkDontShow.Checked = True Then
                    My.Settings.BadFileTypeWarning = False
                End If
        End Select
        My.Settings.Save()
        Me.Close()
    End Sub
    Public Sub BadFileType()
        If My.Settings.BadFileTypeWarning = True Then
            Label2.Text = "Bad File Type"
            Label1.Height = 60
            Label1.Top = Label1.Top - 10
            Label1.Text = "Some items were found that have an unsupported extension or the file extensions are invisible." &
                       " These files may Not work correctly With the program. " &
                       "Currently only the native Inventor extensions are supported (.ipt, .idw, .iam, & .ipn)" & vbNewLine &
                       "(You can enable file name extension visibility through the file explorer)"
            btnOK.Text = "OK"
            Me.Height = 150
            btnOK.Top = btnOK.Top + 35
            chkDontShow.Visible = True
            chkDontShow.Top = chkDontShow.Top + 35
            chkDontShow.Checked = False
            PicDonate.Visible = False
        End If

    End Sub
    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PicDonate.Click
        Try
            System.Diagnostics.Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=HXDSA8NWCRM2G")
        Catch
            'Code to handle the error.
        End Try
        Me.Close()
        My.Settings.Donated = True
        My.Settings.Save()
    End Sub

    Private Sub chkDontShow_CheckedChanged(sender As Object, e As EventArgs) Handles chkDontShow.CheckedChanged
        If Label2.Text = "FirstRun" Then
            My.Settings.FirstRun = False
        ElseIf Label2.Text = "Donate" Then
            PicDonate.Visible = False
            btnOK.Text = "My bad, carry on and disregard"
        End If
        My.Settings.Save()
    End Sub
End Class