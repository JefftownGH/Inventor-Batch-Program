﻿Imports System.Windows.Forms

Public Class Settings
    Dim Main As Main
    Dim Settings As Settings

    Public Sub New()
        InitializeComponent()

        If My.Settings.PDFSaveNewLoc = True Then
            txtPDFSaveLoc.Text = My.Settings.PDFSaveLoc
            txtPDFSaveLoc.Enabled = True
            rdoPDFSaveLoc.Checked = True
        ElseIf My.Settings.PDFSaveTag = True Then
            txtPDFTag.Enabled = True
            rdoPDFTag.Checked = True
            txtPDFTag.Text = My.Settings.PDFTag
        End If
        If My.Settings.DXFSaveNewLoc = True Then
            txtDXFSaveLoc.Text = My.Settings.DXFSaveLoc
            txtDXFSaveLoc.Enabled = True
            rdoDXFSaveLoc.Checked = True
        ElseIf My.Settings.DXFSaveTag = True Then
            txtDXFTag.Enabled = True
            rdoDXFTag.Checked = True
            txtDXFTag.Text = My.Settings.DXFTag
        End If
        If My.Settings.DWGSaveNewLoc = True Then
            txtDWGSaveLoc.Text = My.Settings.DWGSaveLoc
            txtDWGSaveLoc.Enabled = True
            rdoDWGSaveLoc.Checked = True
        ElseIf My.Settings.DWGsavetag = True Then
            txtDWGTag.Enabled = True
            rdoDWGTag.Checked = True
            txtDWGTag.Text = My.Settings.DWGTag
        End If
        If My.Settings.PDFRev = False Then chkPDFRev.Checked = False
        If My.Settings.DXFRev = False Then chkDXFRev.Checked = False
        If My.Settings.DWGRev = False Then chkDWGRev.Checked = False

    End Sub
    Private Sub rdoPDFSaveLoc_CheckedChanged(sender As Object, e As EventArgs) Handles rdoPDFSaveLoc.CheckedChanged
        If rdoPDFSaveLoc.Checked = True Then
            rdoPDFChoose.Checked = False
            rdoPDFTag.Checked = False
            txtPDFSaveLoc.Enabled = True
            txtPDFTag.Enabled = False
            txtPDFTag.Text = "ex: \Drawings\PDF"
        End If
    End Sub

    Private Sub rdoPDFChoose_CheckedChanged(sender As Object, e As EventArgs) Handles rdoPDFChoose.CheckedChanged
        If rdoPDFChoose.Checked = True Then
            rdoPDFTag.Checked = False
            rdoPDFSaveLoc.Checked = False
            txtPDFSaveLoc.Enabled = False
            txtPDFTag.Enabled = False
            txtPDFTag.Text = "ex: \Drawings\PDF"
        End If
    End Sub
    Private Sub rdoPDFSub_CheckedChanged(sender As Object, e As EventArgs) Handles rdoPDFTag.CheckedChanged
        If rdoPDFTag.Checked = True Then
            rdoPDFChoose.Checked = False
            rdoPDFSaveLoc.Checked = False
            txtPDFSaveLoc.Enabled = False
            txtPDFTag.Enabled = True
            txtPDFTag.Text = ""
        End If
    End Sub
    Private Sub rdoDXFChoose_CheckedChanged(sender As Object, e As EventArgs) Handles rdoDXFChoose.CheckedChanged
        If rdoDXFChoose.Checked = True Then
            rdoDXFSaveLoc.Checked = False
            rdoDXFTag.Checked = False
            txtDXFTag.Enabled = False
            txtDXFSaveLoc.Enabled = False
            txtDXFTag.Text = "ex: \Drawings\DXF"
        End If
    End Sub
    Private Sub rdoDXFSaveLoc_CheckedChanged(sender As Object, e As EventArgs) Handles rdoDXFSaveLoc.CheckedChanged
        If rdoDXFSaveLoc.Checked = True Then
            rdoDXFChoose.Checked = False
            rdoDXFTag.Checked = False
            txtDXFSaveLoc.Enabled = True
            txtDXFTag.Enabled = False
            txtDXFTag.Text = "ex: \Drawings\DXF"
        End If
    End Sub

    Private Sub rdoDXFSub_CheckedChanged(sender As Object, e As EventArgs) Handles rdoDXFTag.CheckedChanged
        If rdoDXFTag.Checked = True Then
            rdoDXFChoose.Checked = False
            rdoDXFSaveLoc.Checked = False
            txtDXFSaveLoc.Enabled = False
            txtDXFTag.Enabled = True
            txtDXFTag.Text = ""
        End If
    End Sub

    Private Sub rdoDWGChoose_CheckedChanged(sender As Object, e As EventArgs) Handles rdoDWGChoose.CheckedChanged
        If rdoDWGChoose.Checked = True Then
            rdoDWGSaveLoc.Checked = False
            rdoDWGTag.Checked = False
            txtDWGTag.Enabled = False
            txtDWGSaveLoc.Enabled = False
            txtDWGTag.Text = "ex: \Drawings\DWG"
        End If
    End Sub
    Private Sub rdoDWGSaveLoc_CheckedChanged(sender As Object, e As EventArgs) Handles rdoDWGSaveLoc.CheckedChanged
        If rdoDWGSaveLoc.Checked = True Then
            rdoDWGChoose.Checked = False
            rdoDWGTag.Checked = False
            txtDWGSaveLoc.Enabled = True
            txtDWGTag.Enabled = False
            txtDWGTag.Text = "ex: \Drawings\DWG"
        End If
    End Sub

    Private Sub rdoDWGSub_CheckedChanged(sender As Object, e As EventArgs) Handles rdoDWGTag.CheckedChanged
        If rdoDWGTag.Checked = True Then
            rdoDWGChoose.Checked = False
            rdoDWGSaveLoc.Checked = False
            txtDWGSaveLoc.Enabled = False
            txtDWGTag.Enabled = True
            txtDWGTag.Text = ""
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        If rdoPDFSaveLoc.Checked = True Then
            My.Settings.PDFSaveLoc = txtPDFSaveLoc.Text
            My.Settings.PDFSaveNewLoc = True
            My.Settings.PDFSaveTag = False
        ElseIf rdoPDFTag.Checked = True Then
            If IsAlphaNum(txtPDFTag.Text) = False Then
                MsgBox("The PDF location you have selected is invalid")
                Exit Sub
            End If
            If Strings.Left(txtPDFTag.Text, 1) <> "\" Then
                txtPDFTag.Text = "\" & txtPDFTag.Text
            End If
            If Strings.Right(txtPDFTag.Text, 1) = "\" Then
                txtPDFTag.Text = Strings.Left(txtPDFTag.Text, Len(txtPDFTag.Text) - 1)
            End If
            My.Settings.PDFTag = txtPDFTag.Text
            My.Settings.PDFSaveNewLoc = False
            My.Settings.PDFSaveTag = True
        Else
            My.Settings.PDFSaveNewLoc = False
            My.Settings.PDFSaveTag = False
        End If
        If chkPDFRev.Checked = True Then
            My.Settings.PDFRev = True
        Else
            My.Settings.PDFRev = False
        End If


        If rdoDXFSaveLoc.Checked = True Then
            My.Settings.DXFSaveLoc = txtDXFSaveLoc.Text
            My.Settings.DXFSaveNewLoc = True
            My.Settings.DXFSaveTag = False
        ElseIf rdoDXFTag.Checked = True Then
            If IsAlphaNum(txtDXFTag.Text) = False Then
                MsgBox("The DXF location you have selected is invalid")
                Exit Sub
            End If
            If Strings.Left(txtDXFTag.Text, 1) <> "\" Then
                txtDXFTag.Text = "\" & txtDXFTag.Text
            End If
            If Strings.Right(txtDXFTag.Text, 1) = "\" Then
                txtDXFTag.Text = Strings.Left(txtDXFTag.Text, Len(txtDXFTag.Text) - 1)
            End If
            My.Settings.DXFTag = txtDXFTag.Text
            My.Settings.DXFSaveNewLoc = False
            My.Settings.DXFSaveTag = True
        Else
            My.Settings.DXFSaveNewLoc = False
            My.Settings.DXFSaveTag = False
        End If
        If chkDXFRev.Checked = True Then
            My.Settings.DXFRev = True
        Else
            My.Settings.DXFRev = False
        End If

        If rdoDWGSaveLoc.Checked = True Then
            My.Settings.DWGSaveLoc = txtDWGSaveLoc.Text
            My.Settings.DWGSaveNewLoc = True
            My.Settings.DWGSaveTag = False
        ElseIf rdoDWGTag.Checked = True Then
            If IsAlphaNum(txtDWGTag.Text) = False Then
                MsgBox("The DWG location you have selected is invalid")
                Exit Sub
            End If
            If Strings.Left(txtDWGTag.Text, 1) <> "\" Then
                txtDWGTag.Text = "\" & txtDWGTag.Text
            End If
            If Strings.Right(txtDWGTag.Text, 1) = "\" Then
                txtDWGTag.Text = Strings.Left(txtDWGTag.Text, Len(txtDWGTag.Text) - 1)
            End If
            My.Settings.DWGTag = txtDWGTag.Text
            My.Settings.DWGSaveNewLoc = False
            My.Settings.DWGSaveTag = True
        Else
            My.Settings.DWGSaveNewLoc = False
            My.Settings.DWGSaveTag = False
        End If
        If chkDWGRev.Checked = True Then
            My.Settings.DWGRev = True
        Else
            My.Settings.DWGRev = False
        End If

        Me.Close()
    End Sub

    Private Sub btnPDFLocBrowse_Click(sender As Object, e As EventArgs) Handles btnPDFLocBrowse.Click
        Dim Folder As FolderBrowserDialog = New FolderBrowserDialog
        Folder.Description = "Choose the location you wish to save to"
        Folder.RootFolder = System.Environment.SpecialFolder.Desktop
        Try
            If Folder.ShowDialog() = Windows.Forms.DialogResult.OK Then
                txtPDFSaveLoc.Text = Folder.SelectedPath
            Else
                Exit Sub
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Exception Details", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try
    End Sub

    Private Sub btnDXFLocBrowse_Click(sender As Object, e As EventArgs) Handles btnDXFLocBrowse.Click
        Dim Folder As FolderBrowserDialog = New FolderBrowserDialog
        Folder.Description = "Choose the location you wish to save to"
        Folder.RootFolder = System.Environment.SpecialFolder.Desktop
        Try
            If Folder.ShowDialog() = Windows.Forms.DialogResult.OK Then
                txtDXFSaveLoc.Text = Folder.SelectedPath
            Else
                Exit Sub
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Exception Details", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try
    End Sub

    Private Sub btnDWGBrowse_Click(sender As Object, e As EventArgs) Handles btnDWGBrowse.Click
        Dim Folder As FolderBrowserDialog = New FolderBrowserDialog
        Folder.Description = "Choose the location you wish to save to"
        Folder.RootFolder = System.Environment.SpecialFolder.Desktop
        Try
            If Folder.ShowDialog() = Windows.Forms.DialogResult.OK Then
                txtDWGSaveLoc.Text = Folder.SelectedPath
            Else
                Exit Sub
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Exception Details", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        End Try
    End Sub
    Private Function IsAlphaNum(ByVal strInputText As String) As Boolean
        Dim IsAlpha As Boolean = False
        If System.Text.RegularExpressions.Regex.IsMatch(strInputText, "^[a-zA-Z0-9-\\]+$") Then
            IsAlpha = True
        Else
            IsAlpha = False
        End If
        Return IsAlpha
    End Function
End Class