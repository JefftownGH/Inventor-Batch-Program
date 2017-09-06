﻿Imports System
Imports System.Type
Imports System.Activator
Imports System.Runtime.InteropServices
Imports Inventor
Imports System.Windows.Forms
Imports Microsoft.Office.Interop
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Diagnostics
Imports System.Threading

Public Class Main
    Dim _invApp As Inventor.Application
    'Dim _ExcelApp As New Excel.Application 'Microsoft.Office.Interop.Excel.Application
    Dim _started As Boolean = False
    Public iProperties As iProperties
    Public Shared RevTable As New RevTable
    Public Print_Size As New Print_Size
    Public CheckNeeded As CheckNeeded
    Public Settings As New Settings
    Dim RenameTable As New List(Of List(Of String))
    Dim ThumbList As New List(Of Image)
    Dim OpenFiles As New List(Of KeyValuePair(Of String, String))
    Dim SubFiles As New List(Of KeyValuePair(Of String, String))
    Dim AlphaSub As SortedList(Of String, String) = New SortedList(Of String, String)
    Dim OpenDocs As New ArrayList
    Dim VBAFlag As String = "False"
    Dim InvRef(0 To 9) As Inventor.Property
    Dim CustomPropSet As PropertySet
    Dim Time As System.DateTime = Now

    Public Sub writeDebug(ByVal x As String)
        Dim path As String = My.Computer.FileSystem.SpecialDirectories.Temp
        Dim FILE_NAME As String = path & "\Debug.txt"
        'MsgBox(FILE_NAME)
        If System.IO.File.Exists(FILE_NAME) = False Then
            System.IO.File.Create(FILE_NAME).Dispose()
        End If
        Dim objWriter As New System.IO.StreamWriter(FILE_NAME, True)
        objWriter.WriteLine(DateTime.Now.ToLongTimeString & " " & x & vbNewLine)
        objWriter.Close()

    End Sub
    Public Sub New()
        If My.Computer.FileSystem.FileExists(My.Computer.FileSystem.SpecialDirectories.Temp & "\Debug.txt") Then
            Kill(My.Computer.FileSystem.SpecialDirectories.Temp & "\Debug.txt")
        End If
        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
        Try
            _invApp = Marshal.GetActiveObject("Inventor.Application")
        Catch ex As Exception
            Try
                Dim invAppType As Type =
                  GetTypeFromProgID("Inventor.Application")
                _invApp = CreateInstance(invAppType)
                _invApp.Visible = True
                'Note: if you shut down the Inventor session that was started
                'this(way) there is still an Inventor.exe running. We will use
                'this Boolean to test whether or not the Inventor App  will
                'need to be shut down.
                _started = True
            Catch ex2 As Exception
                MsgBox(ex2.ToString())
                MsgBox("Unable to get or start Inventor")
            End Try
        End Try
        writeDebug("Inventor Accessed")

    End Sub
    Public Function PopiProperties(CalledFunction As iProperties)
        iProperties = CalledFunction
        Return Nothing
    End Function
    Public Sub UpdateForm()
        'Clear the Open Documents listbox
        OpenFiles.Clear()
        lstOpenfiles.Items.Clear()
        Dim oDoc As Document
        'Iterate through each document open in Inventor and retrieve the display name
        Try
            For Each oDoc In _invApp.Documents.VisibleDocuments
                If oDoc.FullDocumentName <> Nothing Then
                    'Compare file type to the files chosen to display and only display the selected documents.
                    'Add the document name to key & location to value for faster recall
                    If chkAssy.Checked = True And oDoc.DocumentType = DocumentTypeEnum.kAssemblyDocumentObject Then
                        OpenFiles.Add(New KeyValuePair(Of String, String)(Strings.Right(oDoc.FullDocumentName, Len(oDoc.FullDocumentName) - InStrRev(oDoc.FullDocumentName, "\")), oDoc.FullDocumentName))
                        'lstOpenfiles.Items.Add(Strings.Right(oDoc.FullDocumentName, len(oDoc.FullDocumentName)-InStrRev(oDoc.FullDocumentName, "\")))
                    ElseIf chkParts.Checked = True And oDoc.DocumentType = DocumentTypeEnum.kPartDocumentObject Then
                        OpenFiles.Add(New KeyValuePair(Of String, String)(Strings.Right(oDoc.FullDocumentName, Len(oDoc.FullDocumentName) - InStrRev(oDoc.FullDocumentName, "\")), oDoc.FullDocumentName))
                        'lstOpenfiles.Items.Add(Strings.Right(oDoc.FullDocumentName, len(oDoc.FullDocumentName)-InStrRev(oDoc.FullDocumentName, "\")))
                    ElseIf chkDrawings.Checked = True And oDoc.DocumentType = DocumentTypeEnum.kDrawingDocumentObject Then
                        OpenFiles.Add(New KeyValuePair(Of String, String)(Strings.Right(oDoc.FullDocumentName, Len(oDoc.FullDocumentName) - InStrRev(oDoc.FullDocumentName, "\")), oDoc.FullDocumentName))
                        'lstOpenfiles.Items.Add(Strings.Right(oDoc.FullDocumentName, len(oDoc.FullDocumentName)-InStrRev(oDoc.FullDocumentName, "\")))
                    ElseIf chkPres.Checked = True And oDoc.DocumentType = DocumentTypeEnum.kPresentationDocumentObject Then
                        OpenFiles.Add(New KeyValuePair(Of String, String)(Strings.Right(oDoc.FullDocumentName, Len(oDoc.FullDocumentName) - InStrRev(oDoc.FullDocumentName, "\")), oDoc.FullDocumentName))
                        'lstOpenfiles.Items.Add(Strings.Right(oDoc.FullDocumentName, len(oDoc.FullDocumentName)-InStrRev(oDoc.FullDocumentName, "\")))
                    End If
                End If
            Next
            writeDebug("Open document list refreshed")
        Catch
            If Strings.Len(Err.ToString) > 0 Then
                writeDebug("***ERROR***" & vbNewLine & Err.Description & vbNewLine & "***ERROR***" & vbNewLine)
                Err.Clear()
            End If

        End Try
        If OpenFiles.Count = 0 Then
            OpenFiles.Clear()
        Else
            'Add an entry for each item in the keyvalue pair list
            For Each Pair As KeyValuePair(Of String, String) In OpenFiles
                lstOpenfiles.Items.Add(Pair.Key)
            Next
        End If

        If LVSubFiles.Items.Count = 0 Then
            'change display style to simulate an unusable entity
            LVSubFiles.Items.Add("No Drawings Found")
            'LVSubFiles.ThreeDCheckBoxes = False
            LVSubFiles.ForeColor = Drawing.Color.Gray
            For x = 0 To LVSubFiles.Items.Count - 1
                LVSubFiles.Items(x).Checked = False
            Next
        End If
        If lstOpenfiles.Items.Count > 0 Then
            If InStr(lstOpenfiles.Items(0), "ipt") > 0 Or
                InStr(lstOpenfiles.Items(0), "idw") > 0 Or
                InStr(lstOpenfiles.Items(0), "iam") > 0 Or
                InStr(lstOpenfiles.Items(0), "ipn") > 0 Then
            Else
                MsgBox("This program needs to access the file extensions to operate properly" & vbNewLine &
                   "Open file explorer and enable file name extensions")
                End
            End If
        End If
    End Sub
    Private Sub Main_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        UpdateForm()
    End Sub
    Private Sub btnExit_Click(sender As System.Object, e As System.EventArgs) Handles btnExit.Click
        If btnExit.Text = "Exit" Then
            Me.Close()
        End If
    End Sub
    Private Sub chkDrawings_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkDrawings.CheckedChanged
        UpdateForm()
    End Sub
    Private Sub chkParts_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkParts.CheckedChanged
        UpdateForm()
        CheckboxReorder()
    End Sub
    Private Sub chkAssy_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkAssy.CheckedChanged
        UpdateForm()
        CheckboxReorder()
    End Sub
    Private Sub CheckboxReorder()
        If chkAssy.Checked = True And chkParts.Checked = False Then
            chkDerived.Visible = True
            chkAssy.Top = chkParts.Location.Y + 15
            chkDerived.Top = chkAssy.Location.Y + 15
            chkPres.Top = chkDerived.Location.Y + 15
            GroupBox1.Height = 94
        ElseIf chkAssy.Checked = False And chkParts.Checked = True Then
            chkDerived.Visible = True
            chkDerived.Top = chkParts.Location.Y + 15
            chkAssy.Top = chkDerived.Location.Y + 15
            chkPres.Top = chkAssy.Location.Y + 15
            GroupBox1.Height = 94
        ElseIf chkAssy.Checked = True And chkParts.Checked = True Then
            chkDerived.Visible = True
            chkAssy.Top = chkParts.Location.Y + 15
            chkDerived.Top = chkAssy.Location.Y + 15
            chkPres.Top = chkDerived.Location.Y + 15
            GroupBox1.Height = 94
        Else
            chkAssy.Checked = False And chkParts.Checked = False
            chkDerived.Visible = False
            chkDerived.Checked = False
            chkDerived.Top = chkParts.Location.Y + 15
            chkAssy.Top = chkParts.Location.Y + 15
            chkPres.Top = chkAssy.Location.Y + 15
            GroupBox1.Height = 79
        End If
    End Sub
    Private Sub chkPres_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkPres.CheckedChanged
        UpdateForm()
    End Sub
    Private Sub lstOpenfiles_ItemCheck(sender As Object, e As System.Windows.Forms.ItemCheckEventArgs) Handles lstOpenfiles.ItemCheck
        'Run checkbox click routine through a timer so that the most recent item will show up in the selection
        tmr.Enabled = False
        tmr.Enabled = True
    End Sub
    Private Sub tmr_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmr.Tick
        MsVistaProgressBar1.Visible = True
        LVSubFiles.Enabled = False
        'MsVistaProgressBar1.ProgressBarStyle = MSVistaProgressBar.BarStyle.Marquee
        tmr.Enabled = False
        'set the colour and style of the subfiles window
        SubFiles.Clear()
        AlphaSub.Clear()
        LVSubFiles.Items.Clear()
        RenameTable.Clear()
        'LVSubFiles.ThreeDCheckBoxes = True
        LVSubFiles.ForeColor = Drawing.Color.Black
        Dim watch As Stopwatch = Stopwatch.StartNew
        Dim oDoc As Document
        Dim Path As Documents
        Dim PartSource As String
        Dim Level As Integer = 0
        Dim Total As Integer = 1
        Dim Counter As Integer = 1
        Dim Title As String = "Found: "
        ProgressBar(Total, Counter, Title, "")
        CreateOpenDocs(OpenDocs)
        Dim Elog As String = ""
        'iterate through the open files and display the available drawings in the Subfiles window
        Dim X As Integer
        For X = 0 To lstOpenfiles.CheckedItems.Count - 1
            'for drawing documents add name and location from openfiles list
            If Strings.InStr(lstOpenfiles.CheckedItems(X), "idw") <> 0 Then
                SubFiles.Add(New KeyValuePair(Of String, String)(lstOpenfiles.CheckedItems(X), OpenFiles.Item(lstOpenfiles.Items.IndexOf(lstOpenfiles.CheckedItems(X))).Value))
                AlphaSub.Add(lstOpenfiles.CheckedItems(X), OpenFiles.Item(lstOpenfiles.Items.IndexOf(lstOpenfiles.CheckedItems(X))).Value)
                Elog = SubFiles.Item(SubFiles.Count - 1).Value & ": Added to Open File list" & vbNewLine
            Else
                'iterate through open documents looking for the item corresponding to the checked item, if it exists, display it in the subfiles window.
                For J = 1 To _invApp.Documents.Count
                    Path = _invApp.Documents
                    oDoc = Path.Item(J)
                    'get the source path of the targeted file
                    PartSource = oDoc.FullDocumentName
                    'If the targeted document is the checked document, identify its type
                    If lstOpenfiles.CheckedItems.Item(X) = Strings.Right(PartSource, Strings.Len(PartSource) - InStrRev(PartSource, "\")) Then
                        'Drawing documents can be directly sent to the subfiles list
                        If oDoc.DocumentType = DocumentTypeEnum.kDrawingDocumentObject Then
                            TestForDrawing(PartSource, 0, Total, Counter, OpenDocs, Elog)
                            'Part/Presentation documents require a search for the associated drawings
                        ElseIf oDoc.DocumentType = DocumentTypeEnum.kPartDocumentObject Or oDoc.DocumentType = DocumentTypeEnum.kPresentationDocumentObject Then
                            'Test to see if assumed drawing name exists
                            TestForDrawing(PartSource, 0, Total, Counter, OpenDocs, Elog)
                            CheckForDev(PartSource, Total, Counter, OpenDocs, Elog, Level + 1)
                            'Assembly documents require a search for subfiles
                        ElseIf oDoc.DocumentType = DocumentTypeEnum.kAssemblyDocumentObject Then
                            Total = Total + oDoc.ReferencedDocuments.Count
                            TraverseAssemblyLoad(PartSource, 0, Total, Counter, OpenDocs, Elog, Dev:=False)
                        End If
                    End If
                Next
            End If
        Next
        'update the listbox when complete
        writeDebug(Elog)
        RunCompleted()
    End Sub
    Private Sub TestForDrawing(Partsource As String, ByVal Level As Integer, ByRef Total As Integer,
                               ByRef Counter As Integer, OpenDocs As ArrayList, ByRef Elog As String)
        Dim Dup As Boolean = False
        Dim strFile, DrawingName As String
        'Extract the file name from the full file path
        strFile = Strings.Right(Partsource, Strings.Len(Partsource) - InStrRev(Partsource, "\"))
        'Assume drawing is stored in same location as part/assembly (same inherent procedure as Inventor)
        'Create drawing name for search purposes
        DrawingName = Strings.Left(strFile, (Strings.Len(strFile) - 3)) & "idw"
        'Test to see if the file exists in the expected
        If Not My.Computer.FileSystem.FileExists(Strings.Left(Partsource, (Strings.Len(Partsource) - 3)) & "idw") Then
            DrawingName = DrawingName & "(DNE)"
        End If
        Counter += 1
        Elog = Elog & DrawingName & ": "
        'Check to see if the part has already been added to the list
        If SubFiles.Count = 0 Then
            SubFiles.Add(New KeyValuePair(Of String, String)((Space(Level * 3) & DrawingName).ToString, Partsource))
            AlphaSub.Add((DrawingName).ToString, Partsource)
            Elog = Elog & "Added to Open File list" & vbNewLine
            AddtoRenameTable(Partsource, DrawingName, strFile, True, "")
        Else
            For x = 0 To SubFiles.Count - 1
                If Trim(SubFiles.Item(x).Key) = DrawingName Then
                    Elog = Elog & "Skipped in Open File list" & vbNewLine
                    Exit Sub
                End If
            Next
            'Tab over part to show it is a sub-component
            SubFiles.Add(New KeyValuePair(Of String, String)((Space(Level * 3) & DrawingName).ToString, Partsource))
            AlphaSub.Add((DrawingName).ToString, Partsource)
            Elog = Elog & "Added to Open File list" & vbNewLine
        End If
        ProgressBar(Total, Counter, "Found: ", DrawingName)
        'End If

        'For Each pair As KeyValuePair(Of String, String) In AlphaSub

        'Next
        ' AlphaSub = SubFiles.ToList
        ' AlphaSub.Sort(Function(firstpair As KeyValuePair(Of String, String), nextpair As KeyValuePair(Of String, String)) firstpair.Key.CompareTo(nextpair.Key))
        'AlphaSub = AlphaSub.Sort()
    End Sub
    Private Sub CheckForDev(PartSource As String, ByRef Total As Integer, ByRef Counter As Integer, OpenDocs As ArrayList, Elog As String, ByVal Level As Integer)
        If chkDerived.Checked = False Then Exit Sub
        Dim oAsmDoc As AssemblyDocument = Nothing
        Dim oDoc, oDerived As Document
        Dim DerDrawing As String
        oDoc = _invApp.Documents.Open(PartSource, False)
        If oDoc.DocumentType = DocumentTypeEnum.kPartDocumentObject And oDoc.ReferencedDocuments.Count > 0 Then
            For X = 1 To oDoc.ReferencedDocuments.Count
                oDerived = oDoc.ReferencedDocuments.Item(X)
                DerDrawing = oDerived.PropertySets.Item("{32853F0F-3444-11D1-9E93-0060B03C1CA6}").ItemByPropId("5").Value.ToString & ".idw"
                PartSource = oDerived.FullDocumentName
                If oDerived.DocumentType = DocumentTypeEnum.kAssemblyDocumentObject Then
                    Total = Total + oDoc.ReferencedDocuments.Count - 1
                    If _invApp.ActiveDocument.FullFileName = PartSource Then
                        oAsmDoc = _invApp.ActiveDocument
                    Else
                        Try
                            oAsmDoc = _invApp.Documents.Open(PartSource, False)
                        Catch ex As Exception
                            MsgBox("Something went wrong accessing " & PartSource & vbNewLine &
                           "Please verify the file exists And/Or check" & vbNewLine &
                           "if there are invisible documents open")
                            Exit Sub
                        End Try
                    End If

                    TraverseAssembly(oAsmDoc.ComponentDefinition.Occurrences, PartSource, Level, Total, Counter, OpenDocs, Elog, True)
                Else
                    TestForDrawing(PartSource, Level, Total, Counter, OpenDocs, Elog)
                End If
            Next
        End If
        'CloseLater(Strings.Right(PartSource, (Strings.Len(PartSource) - InStrRev(PartSource, "\"))), oDoc)
    End Sub
    Private Sub TraverseAssemblyLoad(PartSource As String, Level As Integer, ByRef Total As Integer,
                                     ByRef Counter As Integer, Opendocs As ArrayList, ByRef Elog As String, ByRef Dev As Boolean)
        Dim oAsmDoc As AssemblyDocument = Nothing
        Dim strFile, DrawingName, ID As String
        Dim Add As String = "True"
        Dim RenameList As New List(Of String)
        'Open assembly in background
        If _invApp.ActiveDocument.FullFileName = PartSource Then
            oAsmDoc = _invApp.ActiveDocument
        Else
            Try
                oAsmDoc = _invApp.Documents.Open(PartSource, False)
            Catch ex As Exception
                MsgBox("Something went wrong accessing " & PartSource & vbNewLine &
                           "Please verify the file exists And/Or check" & vbNewLine &
                           "if there are invisible documents open")
                Exit Sub
            End Try
        End If
        PartSource = oAsmDoc.FullDocumentName
        Rename.PopMain(Me)
        'Create a drawing name assuming the name/location is the same as the part
        strFile = Strings.Right(PartSource, Strings.Len(PartSource) - InStrRev(PartSource, "\"))
        DrawingName = Strings.Left(strFile, (Strings.Len(strFile) - 3)) & "idw"
        'If Strings.InStr(PartSource, "Content Center") = 0 Then
        If Dev = False Then
            If Strings.InStr(oAsmDoc.File.FullFileName, "Purchased Parts") > 0 Then
                ID = "PP"
            ElseIf Strings.InStr(oAsmDoc.File.FullFileName, "Content Center") > 0 Then
                ID = "CC"
            Else
                ID = oAsmDoc.PropertySets.Item("{F29F85E0-4FF9-1068-AB91-08002B27B3D9}").ItemByPropId("5").Value
            End If
        Else
            ID = "DP"
        End If

        AddtoRenameTable(PartSource, DrawingName, strFile, Add, ID)
        'End If
        'Check to see if the drawing exists and add to the list
        TestForDrawing(PartSource, Level, Total, Counter, Opendocs, Elog)
        'If the part is an assembly, go through recursively and retrieve the sub-components
        TraverseAssembly(oAsmDoc.ComponentDefinition.Occurrences, PartSource, Level + 1, Total, Counter, Opendocs, Elog, Dev)
        CloseLater(strFile, oAsmDoc)
    End Sub
    Private Sub TraverseAssembly(Occurrences As ComponentOccurrences, PartSource As String, Level As Integer, ByRef Total As Integer, ByRef Counter As Integer,
                                 OpenDocs As ArrayList, ByRef Elog As String, ByVal Dev As Boolean)
        'Count the level of the sub-component
        Dim Add As Boolean = True
        Dim strFile, DrawingName, ID As String
        Dim oOcc As ComponentOccurrence
        'iterate through each occurrence in the assembly
        Total = Total + Occurrences.Count
        For Each oOcc In Occurrences
            Try
                PartSource = oOcc.Definition.Document.fullfilename
                'setting the mass to something updates the mass property in the drawing
                'create drawing name
                strFile = Strings.Right(PartSource, Strings.Len(PartSource) - InStrRev(PartSource, "\"))
                DrawingName = Strings.Left(strFile, (Strings.Len(strFile) - 3)) & "idw"
                ProgressBar(Total, Counter, "Found:  ", DrawingName)
                'FindWorker.ReportProgress(CInt((Counter / Total) * 100))
                'Add to list if the drawing exists
                TestForDrawing(PartSource, Level, Total, Counter, OpenDocs, Elog)
                'iterate through again for each sub-assembly found 
                Counter += 1
                'Create a list of sub parts for use in the renaming section. this saves time having to recreate it again later.
                'If VBAFlag <> "NA" And Strings.InStr(PartSource, "Content Center") = 0 Then 
                If Dev = False Then
                    If Strings.InStr(PartSource, "Purchased Parts") > 0 Then
                        ID = "PP"
                    ElseIf Strings.InStr(PartSource, "Content Center") > 0 Then
                        ID = "CC"
                    Else
                        ID = oOcc.Definition.Document.propertysets.Item("{F29F85E0-4FF9-1068-AB91-08002B27B3D9}").ItemByPropId("5").Value
                    End If
                Else
                    ID = "DP"
                End If
                AddtoRenameTable(PartSource, DrawingName, strFile, Add, ID)
                'If the sub assembly is an assembly itself, redo the iteration until all parts have been found.
                If oOcc.DefinitionDocumentType = DocumentTypeEnum.kAssemblyDocumentObject Then
                    'Level += 1
                    TraverseAssembly(oOcc.SubOccurrences, PartSource, Level + 1, Total, Counter, OpenDocs, Elog, Dev)
                ElseIf oOcc.DefinitionDocumentType = DocumentTypeEnum.kPartDocumentObject Then
                    CheckForDev(PartSource, Total, Counter, OpenDocs, Elog, Level + 1)
                End If
            Catch

            End Try

        Next
    End Sub
    Private Sub AddtoRenameTable(ByVal PartSource As String, ByVal DrawingName As String, ByVal strFile As String,
                                 ByVal add As Boolean, ByVal ID As String)
        Dim oDoc As Document = Nothing
        Dim exists As Boolean
        Dim RenameList As New List(Of String)
        RenameList.Add(Strings.Left(PartSource, InStrRev(PartSource, "\")))
        If My.Computer.FileSystem.FileExists(Strings.Left(PartSource, InStrRev(PartSource, "\")) & DrawingName) Then
            RenameList.Add(DrawingName)
        Else
            RenameList.Add("")
        End If
        RenameList.Add(strFile)
        For X = 0 To RenameTable.Count - 1
            If RenameTable(X).Item(2).ToString = strFile Then
                add = False
                Exit For
            Else
                add = True
            End If
        Next
        'pull the thumbnail from Inventor. This must be done through inventor's 32-bit api. 
        If add = True Then
            RenameTable.Add(RenameList)
            Dim oP As Inventor.InventorVBAProject
            For Each oP In _invApp.VBAProjects
                If oP.Name = "ApplicationProject" Then
                    Dim oC As Inventor.InventorVBAComponent
                    For Each oC In oP.InventorVBAComponents
                        If oC.Name = "Get64BitPicture" Then
                            exists = True
                            Dim oM As Inventor.InventorVBAMember
                            For Each oM In oC.InventorVBAMembers
                                If oM.Name = "Thumbnail" Then
                                    If My.Computer.FileSystem.FileExists(My.Computer.FileSystem.SpecialDirectories.Temp & "\Thumbnail.jpg") Then
                                        Kill(My.Computer.FileSystem.SpecialDirectories.Temp & "\Thumbnail.jpg")
                                    End If

                                    IO.File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\PartSource.txt", PartSource)
                                    oM.Execute()
                                    Kill(My.Computer.FileSystem.SpecialDirectories.Temp & "\PartSource.txt")
                                    Exit For
                                End If
                            Next oM
                        End If
                    Next oC
                End If
            Next

            If IO.File.Exists(My.Computer.FileSystem.SpecialDirectories.Temp & "\Thumbnail.jpg") Then
                Dim Thumbnail As Image = Image.FromFile(My.Computer.FileSystem.SpecialDirectories.Temp & "\Thumbnail.jpg", False)
                ThumbList.Add(Thumbnail)
                Kill(My.Computer.FileSystem.SpecialDirectories.Temp & "\Thumbnail.jpg")
            Else
                ThumbList.Add(Nothing)
            End If
        End If
        RenameList.Add(ID)
        If exists = True And add = True Then
            VBAFlag = "True"
        End If
    End Sub
    Private Sub CreateVBA()
        If Not IO.File.Exists(My.Computer.FileSystem.SpecialDirectories.Temp & "\Get64BitPicture.bas") Then
            IO.File.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "\Get64BitPicture.txt", My.Resources.Get64BitPicture)
            FileSystem.Rename(My.Computer.FileSystem.SpecialDirectories.Temp & "\Get64BitPicture.txt", My.Computer.FileSystem.SpecialDirectories.Temp & "\Get64BitPicture.bas")
        End If
        writeDebug("VBA Code missing. Generated VBA code in temp directory")
        MsgBox("The thumbnail script is missing from Inventor" & vbNewLine & vbNewLine &
               "If you no longer wish to receive this error" & vbNewLine &
               "Open VBA Editor in Inventor and import" & vbNewLine &
               My.Computer.FileSystem.SpecialDirectories.Temp & "\Get64BitPicture.bas into the ""modules list""")
    End Sub
    Private Sub chkPartSelect_CheckedChanged(sender As Object, e As EventArgs) Handles chkPartSelect.CheckedChanged
        If chkPartSelect.CheckState = CheckState.Checked Then
            For X = 0 To lstOpenfiles.Items.Count - 1
                lstOpenfiles.SetItemChecked(X, True)
            Next
        Else
            For X = 0 To lstOpenfiles.Items.Count - 1
                lstOpenfiles.SetItemChecked(X, False)
            Next
        End If
    End Sub
    Private Sub chkDWGSelect_CheckedChanged(sender As Object, e As EventArgs) Handles chkDWGSelect.CheckedChanged
        If chkDWGSelect.CheckState = CheckState.Checked Then
            For X = 0 To LVSubFiles.Items.Count - 1
                LVSubFiles.Items(X).Checked = True
            Next
        Else
            For X = 0 To LVSubFiles.Items.Count - 1
                LVSubFiles.Items(X).Checked = False
            Next
        End If
    End Sub
    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        For X = 0 To LVSubFiles.Items.Count - 1
            If LVSubFiles.Items(X).Checked = True Then
                LVSubFiles.Items(X).Checked = False
            Else
                LVSubFiles.Items(X).Checked = True
            End If
        Next
    End Sub
    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        For X = 0 To lstOpenfiles.Items.Count - 1
            If LVSubFiles.Items(X).Checked = True Then
                LVSubFiles.Items(X).Checked = False
            Else
                LVSubFiles.Items(X).Checked = True
            End If
        Next
    End Sub
    Private Sub chkNoDrawing_CheckedChanged(sender As System.Object, e As System.EventArgs)
        'If lstOpenfiles.Items.Count > 0 Then
        'If lstOpenfiles.GetItemChecked(0) = True Then
        'lstOpenfiles.SetItemChecked(0, False)
        'lstOpenfiles.SetItemChecked(0, True)
        'Else
        'lstOpenfiles.SetItemChecked(0, True)
        'lstOpenfiles.SetItemChecked(0, False)
        'End If
        'End If
    End Sub
    Private Sub lstSubfiles_SelectedIndexChanged(sender As Object, e As System.EventArgs)
        'Dim Running As Boolean
        'Dim Y As Integer
        'For X = 0 To lstSubfiles.Items.Count - 1
        'MsgBox(lstSubfiles.GetItemCheckState(X))
        'If lstSubfiles.GetItemCheckState(X) = CheckState.Indeterminate Then
        'Y = X
        'Running = True
        'End If
        'Next
        'If Running = True Then
        'Running = False
        'lstSubfiles.SetItemCheckState(1, CheckState.Indeterminate)
        'End If
    End Sub
    Public Sub CreateOpenDocs(OpenDocs As ArrayList)
        OpenDocs.Clear()
        Dim Archive, DocSource, DocName As String
        For Each oDoc In _invApp.Documents.VisibleDocuments
            Archive = oDoc.FullDocumentName
            'Use the Partsource file to create the drawingsource file
            DocSource = Strings.Left(Archive, Strings.Len(Archive))
            DocName = Strings.Right(DocSource, Strings.Len(DocSource) - Strings.InStrRev(DocSource, "\"))
            OpenDocs.Add(DocName)
        Next
    End Sub

    Private Sub RunCompleted()
        If SubFiles.Count > 10 Then
            LVSubFiles.Height = lstOpenfiles.Height - 25
            txtSearch.Visible = True
            txtSearch.Text = "Search"
            txtSearch.ForeColor = Drawing.Color.Gray
        Else
            LVSubFiles.Height = lstOpenfiles.Height
            txtSearch.Visible = False
        End If

        LVSubFiles.Enabled = True
        'if no drawings are found, notify the user
        If SubFiles.Count = 0 Then
            'change display style to simulate an unusable entity
            LVSubFiles.Items.Add("No Drawings Found")
            'LVSubFiles.ThreeDCheckBoxes = False
            LVSubFiles.ForeColor = Drawing.Color.Gray
        ElseIf SubFiles.Count <> 0 And LVSubFiles.Items.Count = 0 Then
            For Each pair As KeyValuePair(Of String, String) In SubFiles
                If InStr(pair.Key, "(DNE)") <> 0 Then
                    LVSubFiles.Items.Add(Strings.Replace(pair.Key, "(DNE)", ""))
                    LVSubFiles.Items(LVSubFiles.Items.Count - 1).ForeColor = Drawing.Color.Gray
                Else
                    LVSubFiles.Items.Add(pair.Key)
                    LVSubFiles.Items(LVSubFiles.Items.Count - 1).Checked = True
                End If
            Next
        End If
        MsVistaProgressBar1.Visible = False
        Me.Update()
    End Sub
    Public Sub btnOK_Click(sender As System.Object, e As System.EventArgs) Handles btnOK.Click
        MsVistaProgressBar1.ProgressBarStyle = MSVistaProgressBar.BarStyle.Continuous
        Dim NoPart, NoDraw As Boolean
        Dim Path As Documents = _invApp.Documents
        Dim oDoc As Document = Nothing
        Dim Archive As String = ""
        Dim DrawSource As String = ""
        Dim DrawingName As String = ""
        Dim DocSource, DocName As String
        Dim CheckNeeded As New CheckNeeded
        CreateOpenDocs(OpenDocs)
        If lstOpenfiles.CheckedItems.Count = 0 Then
            NoPart = True
        End If
        If LVSubFiles.CheckedItems.Count = 0 Then
            NoDraw = True
        End If
        If NoPart = True Then
            MsgBox("Please Select a Part/Assembly")
            Exit Sub
        ElseIf NoDraw = True Then
            MsgBox("Please Select a Drawing")
            Exit Sub
        End If
        If chkiProp.Checked = True Then
            Dim iProperties As New iProperties
            iProperties.PopMain(Me)
            iProperties.PopulateiProps(Path, oDoc, Archive, DrawingName, DrawSource, OpenDocs, True)
            If DrawSource = "" And DrawingName = "" Then
                MsgBox("Could not locate drawing" & vbNewLine & "Ensure the drawing and model are saved to the same directory")
                Exit Sub
            End If
            iProperties.ShowDialog()
            chkiProp.CheckState = CheckState.Unchecked
        End If
        If ChkRevType.Checked = True Then
            Call ChangeRev()
            ChkRevType.Checked = False
        End If

        If chkCheck.Checked = True Then
            CheckNeeded.PopMain(Me)
            CheckNeeded.PopulateCheckNeeded(Path, oDoc, Archive, DrawingName, DrawSource, OpenDocs)
            If CheckNeeded.lstCheckNeeded.Items.Count > 0 Then
                CheckNeeded.ShowDialog()
            ElseIf CheckNeeded.lstCheckNeeded.Items.Count > 0 Then
                CheckNeeded.btnIgnore.Visible = True
                CheckNeeded.ShowDialog()
            Else
                MsgBox("All drawings have been checked.")
            End If
            chkCheck.Checked = False
        End If
        'If chkExport.Checked = True Then
        '    CheckNeeded.PopMain(Me)
        '    CheckNeeded.PopulateCheckNeeded(Path, oDoc, Archive, DrawingName, DrawSource, OpenDocs)
        '    If CheckNeeded.lstCheckNeeded.Items.Count > 0 Then
        '        chkCheck.CheckState = CheckState.Indeterminate
        '        CheckNeeded.ShowDialog()
        '    Else
        '        chkCheck.CheckState = CheckState.Unchecked
        '    End If
        'End If
        If chkRRev.Checked = True Then
            CheckNeeded.PopMain(Me)
            CheckNeeded.btnIgnore.Visible = False
            CheckNeeded.Label1.Text = "These drawings have revisions that can be removed:"
            CheckNeeded.Label2.Visible = False
            CheckNeeded.Refresh()
            CheckNeeded.btnOK.Visible = False
            CheckNeeded.btnCancel.Location = CheckNeeded.btnOK.Location
            CheckNeeded.btnCancel.Text = "Finished"
            CheckNeeded.PopulateCheckNeeded(Path, oDoc, Archive, DrawingName, DrawSource, OpenDocs)
            'CheckNeeded.lstCheckNeeded
            CheckNeeded.ShowDialog()
            'CheckNeeded.btnIgnore.Visible = True
            'CheckNeeded.Label1.Text = "The following files have not been checked:"
            'CheckNeeded.Label2.Visible = True
            'CheckNeeded.Refresh()
            chkRRev.Checked = False
        End If
        If chkPrint.Checked = True Then
            Print.PopMain(Me)
            Print.PopulatePrint(Path, oDoc, Archive, DrawingName, DrawSource, OpenDocs, SubFiles)
            Print.ShowDialog()
            'PrintDrawings(Path, oDoc, Archive, DrawingName, DrawSource, OpenDocs)
            chkPrint.Checked = False
        End If
        If chkDXF.Checked = True Then
            ExportCheck(Path, oDoc, Archive, DrawingName, DrawSource, "DXF")
            chkDXF.Checked = False
        End If
        If chkPDF.Checked = True Then
            ExportCheck(Path, oDoc, Archive, DrawingName, DrawSource, "PDF")
        End If
        If chkDWG.Checked = True Then
            ExportCheck(Path, oDoc, Archive, DrawingName, DrawSource, "DWG")
            chkDWG.Checked = False
        End If
        If chkOpen.Checked = True Then
            OpenSelected(Path, oDoc, Archive, DrawingName, DrawSource)
            chkOpen.Checked = False
        End If
        If chkClose.Checked = True Then
            CloseDrawings(Path, oDoc, Archive, DrawingName, DrawSource, OpenDocs)
            chkClose.Checked = False
        End If
        RunCompleted()
        OpenDocs.Clear()
        On Error Resume Next

        For Each oDoc In _invApp.Documents.VisibleDocuments
            Archive = oDoc.FullDocumentName
            'Use the Partsource file to create the drawingsource file
            DocSource = Strings.Left(Archive, Strings.Len(Archive))
            DocName = Strings.Right(DocSource, Strings.Len(DocSource) - Strings.InStrRev(DocSource, "\"))
            OpenDocs.Add(DocName)
        Next

        If OpenDocs.Count > 0 Then
        Else
            _invApp.SilentOperation = True
            _invApp.Documents.CloseAll()
            _invApp.SilentOperation = False
        End If
        Err.Clear()
    End Sub
    Public Sub MatchDrawing(ByRef DrawSource As String, ByRef DrawingName As String, Y As Integer)
        'Path As Documents, ByRef oDoc As Document, ByRef Archive As String _
        '  , ByRef DrawingName As String, ByRef DrawSource As String, X As Integer)
        If SubFiles.Count > LVSubFiles.Items.Count Then
            For Each item In SubFiles
                If item.Key.Contains(Trim(LVSubFiles.Items(Y).ToString)) Then
                    DrawSource = Strings.Left(item.Value, Len(item.Value) - 3) & "idw"
                    DrawingName = Trim(item.Key)
                    Exit For
                End If
            Next
        Else
            DrawSource = Strings.Left(SubFiles.Item(Y).Value, Len(SubFiles.Item(Y).Value) - 3) & "idw"
            DrawingName = SubFiles.Item(Y).Key
        End If
        'Look for selected item
        '        For J = 1 To _invApp.Documents.Count
        '            oDoc = Path.Item(J)
        '            Archive = oDoc.FullDocumentName
        '            If Archive = Nothing Then
        '                GoTo Skip
        '            ElseIf Strings.Right(Archive, 3) <> "idw" Then
        '                Dim mass As Double = oDoc.ComponentDefinition.MassProperties.Mass
        '            End If
        '            'Use the Partsource file to create the drawingsource file
        '            DrawSource = Strings.Left(Archive, Strings.Len(Archive) - 3) & "idw"
        '            DrawingName = Strings.Right(DrawSource, Strings.Len(DrawSource) - Strings.InStrRev(DrawSource, "\"))
        '            'If the drawing file is checked, open the drawing in Inventor
        '            If Trim(lstSubfiles.Items.Item(X).ToString) = DrawingName Then
        '                Exit Sub
        '            End If
        'Skip:
        '        Next
    End Sub
    Public Sub MatchPart(ByRef DrawSource As String, ByRef DrawingName As String, Y As Integer)
        'Path As Documents, ByRef oDoc As Document, ByRef Archive As String _
        ' , ByRef PartName As String, ByRef CheckedFile As String,
        'ByRef PartSource As String, X As Integer)

        If SubFiles.Count > LVSubFiles.Items.Count Then
            For Each item In SubFiles
                If item.Key.Contains(Trim(LVSubFiles.Items(Y).ToString)) Then
                    DrawSource = Strings.Left(item.Value, Len(item.Value))
                    DrawingName = Trim(item.Key)
                    Exit For
                End If
            Next
        ElseIf System.IO.File.Exists(Strings.Left(SubFiles.Item(Y).Value, Len(SubFiles.Item(Y).Value) - 3) & "ipt") And
            Not System.IO.File.Exists(Strings.Left(SubFiles.Item(Y).Value, Len(SubFiles.Item(Y).Value) - 3) & "iam") Then
            DrawSource = Strings.Left(SubFiles.Item(Y).Value, Len(SubFiles.Item(Y).Value) - 3) & "ipt"
            DrawingName = Strings.Left(SubFiles.Item(Y).Key, Len(SubFiles.Item(Y).Key) - 3) & "ipt"
        ElseIf System.IO.File.Exists(Strings.Left(SubFiles.Item(Y).Value, Len(SubFiles.Item(Y).Value) - 3) & "iam") And
            Not System.IO.File.Exists(Strings.Left(SubFiles.Item(Y).Value, Len(SubFiles.Item(Y).Value) - 3) & "ipt") Then
            DrawSource = Strings.Left(SubFiles.Item(Y).Value, Len(SubFiles.Item(Y).Value) - 3) & "iam"
            DrawingName = Strings.Left(SubFiles.Item(Y).Key, Len(SubFiles.Item(Y).Key) - 3) & "iam"
        ElseIf System.IO.File.Exists(Strings.Left(SubFiles.Item(Y).Value, Len(SubFiles.Item(Y).Value) - 3) & "ipt") And
            System.IO.File.Exists(Strings.Left(SubFiles.Item(Y).Value, Len(SubFiles.Item(Y).Value) - 3) & "iam") Then
            If My.Settings.DupName = "Model" Then
                DrawSource = Strings.Left(SubFiles.Item(Y).Value, Len(SubFiles.Item(Y).Value) - 3) & "ipt"
                DrawingName = Strings.Left(SubFiles.Item(Y).Key, Len(SubFiles.Item(Y).Key) - 3) & "ipt"
            Else
                DrawSource = Strings.Left(SubFiles.Item(Y).Value, Len(SubFiles.Item(Y).Value) - 3) & "iam"
                DrawingName = Strings.Left(SubFiles.Item(Y).Key, Len(SubFiles.Item(Y).Key) - 3) & "iam"
            End If
        Else
            MsgBox("Couldn't locate model data for " & DrawingName)
            Exit Sub
        End If

        'Look for selected item
        'Dim CheckedFile As String = Trim(lstSubfiles.Items.Item(X))
        '        For J = 1 To _invApp.Documents.Count
        '            oDoc = Path.Item(J)
        '            Archive = oDoc.FullDocumentName
        '            If Archive = Nothing Then GoTo skip
        '            If Strings.Right(Archive, 1) <> ">" Then
        '                If Strings.Right(Archive, 3) <> "idw" Then
        '                    Dim mass As Double = oDoc.ComponentDefinition.MassProperties.Mass
        '                End If
        '                'Use the Partsource file to create the drawingsource file
        '                If System.IO.File.Exists(Strings.Left(Archive, Strings.Len(Archive) - 3) & "ipt") Then
        '                    PartSource = Strings.Left(Archive, Strings.Len(Archive) - 3) & "ipt"
        '                ElseIf System.IO.File.Exists(Strings.Left(Archive, Strings.Len(Archive) - 3) & "iam") Then
        '                    PartSource = Strings.Left(Archive, Strings.Len(Archive) - 3) & "iam"
        '                End If
        '                If PartSource = "" Then
        '                    MsgBox("The drawing " & CheckedFile & " has a model with a different name" & vbNewLine _
        '                                               & "or has been saved in a different project location." & vbNewLine _
        '                                               & "This model's properties could not be located.")
        '                    GoTo skip
        '                End If

        '                PartName = Strings.Right(PartSource, Strings.Len(PartSource) - Strings.InStrRev(PartSource, "\"))
        '                'If the drawing file is checked, open the drawing in Inventor
        '                If Strings.Left(CheckedFile, Len(CheckedFile) - 4) = Strings.Left(PartName, Len(PartName) - 4) Then
        '                    Archive = PartSource
        '                    Exit Sub
        '                End If
        '            End If
        'skip:
        '        Next
    End Sub
    Public Sub OpenSelected(Path As Documents, ByRef odoc As Document, ByRef Archive As String _
                             , ByRef DrawingName As String, ByRef DrawSource As String)
        'Go through drawings to see which ones are selected
        For X = 0 To LVSubFiles.Items.Count - 1
            'Look through all sub files in open documents to get the part sourcefile
            If LVSubFiles.Items(X).Checked = True Then
                MatchDrawing(DrawSource, DrawingName, X)
                'DrawSource = Strings.Left(SubFiles.Item(X).Value, Len(SubFiles.Item(X).Value) - 3) & "idw"
                odoc = _invApp.Documents.Open(DrawSource, True)
            End If
        Next
    End Sub
    'Private Sub PrintDrawings(Path As Documents, ByRef odoc As Document, ByRef Archive As String _
    '                         , ByRef DrawingName As String, ByRef DrawSource As String, OpenDocs As ArrayList)
    '    Dim Z, Y As Integer
    '    Dim Ans As Integer
    '    Dim Size, Compare As Double
    '    Dim ScaleSelect, MSheetQ, PrintMSheets, First, ScaleCheck, Flag As Boolean
    '    Dim dDoc As DrawingDocument
    '    Size = 0
    '    Compare = 0
    '    'Create loop. First loop creates a list of files to be printed getting spec's
    '    'Second loop print the selected files according to the selected size
    '    'Check to see if the drawings should be scaled
    '    Ans = MsgBox("Would you like to print full size?", vbYesNoCancel, "Scale Drawings")
    '    If Ans = vbYes Then
    '        ScaleSelect = False
    '    ElseIf Ans = vbNo Then
    '        ScaleSelect = True
    '    Else
    '        Exit Sub
    '    End If
    '    For Y = 1 To 2
    '        'Go through drawings to see which ones are selected
    '        For X = 0 To lstSubfiles.Items.Count - 1
    '            'Look through all sub files in open documents to get the part sourcefile
    '            If lstSubfiles.GetItemCheckState(X) = CheckState.Checked Then
    '                'MatchDrawing(Path, odoc, Archive, DrawingName, DrawSource, X)
    '                DrawSource = Strings.Left(SubFiles.Item(X).Value, Len(SubFiles.Item(X).Value) - 3) & "idw"
    '                If Y = 1 Then
    '                    dDoc = _invApp.Documents.Open(DrawSource, False)
    '                Else
    '                    dDoc = _invApp.Documents.Open(DrawSource, True)
    '                End If
    '                DrawingName = Strings.Right(dDoc.FullDocumentName, Len(dDoc.FullDocumentName) - InStrRev(dDoc.FullDocumentName, "\"))
    '                'Check to see if there are multiple pages to be printed
    '                For Z = 1 To dDoc.Sheets.Count
    '                    If Z > 1 And Y = 1 And MSheetQ = False Then
    '                        'If there are multiple sheets check if they should all be printed
    '                        Ans = MsgBox("Some drawings have multiple sheets." _
    '                                     & vbNewLine & "Do you wish to print these as well?",
    '                                    vbYesNoCancel, "Print Multiple Sheets")
    '                        MSheetQ = True
    '                        'Record whether to print single or multiple sheets
    '                        If Ans = vbYes Then
    '                            PrintMSheets = True
    '                        ElseIf Ans = vbNo Then
    '                            PrintMSheets = False
    '                        Else
    '                            Exit Sub
    '                        End If
    '                    End If
    '                    'Record sheet size for the first drawing
    '                    Size = dDoc.Sheets.Item(Z).Size
    '                    dDoc.Sheets.Item(Z).Activate()
    '                    If First = False Then
    '                        Compare = Size
    '                        First = True
    '                    End If
    '                    'On second iteration print the selected sheet
    '                    If Y = 2 Then
    '                        PrintSheets(DrawingName, ScaleSelect, Size)
    '                        If PrintMSheets = False Then
    '                            Exit For
    '                        End If
    '                        'Printed = True
    '                    End If
    '                Next
    '                'Compare curent sheet size to first sheet size to check if the are different
    '                'If Size <> Compare Then
    '                ' ScaleCheck = True
    '                ' 'Notify the user of differing sheet sizes and ask for input
    '                ' If Y = 1 And Flag = True Then
    '                ' Ans = MsgBox("The sheets to be printed are of different sizes." _
    '                '              & vbNewLine & "Do you wish to scale the drawings to the same size?", _
    '                '             vbYesNo, "Scale Drawings")
    '                ' If Ans = vbYes Then
    '                ' 'Scale all drawings to the same size
    '                ' ScaleCheck = True
    '                'End If
    '                'End If
    '                'End If
    '                'Close drawings that have been opened by the program
    '                CloseLater(DrawingName, dDoc)
    '                'If Printed = True Then
    '                'Exit For
    '                'End If
    '            End If
    '        Next
    '    Next
    'End Sub

    Public Sub ExportCheck(Path As Documents, ByRef odoc As Document, ByRef Archive As String _
                             , ByRef DrawingName As String, ByRef DrawSource As String, ExportType As String)
        Dim OpenDocs As New ArrayList
        CreateOpenDocs(OpenDocs)
        Dim X, Ans, Total, Counter, OWCounter As Integer
        Counter = 1
        Dim Overwrite, Title, PDFSource, DXFSource, DWGSource, RevNo As String
        Overwrite = ""
        PDFSource = ""
        DXFSource = ""
        DWGSource = ""
        RevNo = ""
        'Get total amount of documents for the process bar
        Total = LVSubFiles.CheckedItems.Count
        'Go through drawings to see which ones are selected
        Dim Destin As String = ""

        For X = 0 To LVSubFiles.Items.Count - 1
            'Look through all sub files in open documents to get the part sourcefile
            If LVSubFiles.Items(X).Checked = True Then
                'iterate through opend documents to find the selected file
                DrawSource = Strings.Left(SubFiles.Item(X).Value, Len(SubFiles.Item(X).Value) - 3) & "idw"
                DrawingName = Trim(SubFiles.Item(X).Key)
                If My.Settings(ExportType & "SaveNewLoc") = False And My.Settings(ExportType & "SaveTag") = False Then
                    Dim Folder As FolderBrowserDialog = New FolderBrowserDialog
                    Folder.Description = "Choose the location you wish to save to"
                    Folder.RootFolder = System.Environment.SpecialFolder.Desktop
                    Folder.SelectedPath = Strings.Left(DrawSource, InStrRev(DrawSource, "\"))
                    Try
                        If Folder.ShowDialog() = Windows.Forms.DialogResult.OK Then
                            Destin = Folder.SelectedPath
                        Else
                            Exit Sub
                        End If
                    Catch ex As Exception
                        MessageBox.Show(ex.Message, "Exception Details", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    End Try
                ElseIf My.Settings(ExportType & "SaveNewLoc") = True Then
                    Destin = My.Settings(ExportType & "SaveLoc")
                Else
                    Destin = Strings.Left(DrawSource, InStrRev(DrawSource, "\") - 1) & My.Settings(ExportType & "Tag")
                End If
                'open drawing
                odoc = _invApp.Documents.Open(DrawSource, False)
                'Get revision number
                If My.Settings(ExportType & "Rev") = True Then
                    RevNo = "-R" & odoc.PropertySets.Item("{F29F85E0-4FF9-1068-AB91-08002B27B3D9}").ItemByPropId("9").Value
                Else
                    RevNo = ""
                End If
                'Check for files that will be overwritten
                'Set the filename to be saved along with the revision

                'Set the PDF/DXF name and search to see if it exists
                DXFSource = Destin & "\" & DrawingName
                PDFSource = Destin & "\" & DrawingName
                DXFSource = DXFSource.Insert(DXFSource.LastIndexOf("."), RevNo)
                PDFSource = PDFSource.Insert(PDFSource.LastIndexOf("."), RevNo)
                DXFSource = Replace(DXFSource, "idw", "dxf")
                PDFSource = Replace(PDFSource, "idw", "pdf")
                DWGSource = Replace(DXFSource, "dxf", "dwg")
                'If the file exists, save it for display later
                If ExportType = "PDF" Then
                    If chkPDF.CheckState = CheckState.Checked Then
                        If My.Computer.FileSystem.FileExists(PDFSource) Then
                            Overwrite = Overwrite & Strings.Left(DrawingName, Strings.Len(DrawingName) - 4) & RevNo & ".pdf" & vbNewLine
                            OWCounter += 1
                        End If
                    End If
                ElseIf ExportType = "DWG" Then
                    If chkDWG.CheckState = CheckState.Checked Then
                        If My.Computer.FileSystem.FileExists(DWGSource) Or
                            My.Computer.FileSystem.FileExists(DWGSource.Insert(DWGSource.LastIndexOf("."), "_Sheet_1")) Then
                            Overwrite = Overwrite & Strings.Left(DrawingName, Strings.Len(DrawingName) - 4) & RevNo & ".dwg" & vbNewLine
                            OWCounter += 1
                        End If
                    End If
                ElseIf ExportType = "DXF" Then
                    If My.Computer.FileSystem.FileExists(DXFSource) Or
                    My.Computer.FileSystem.FileExists(DXFSource.Insert(DXFSource.LastIndexOf("."), "_Sheet_1")) Then
                        Overwrite = Overwrite & Strings.Left(DrawingName, Strings.Len(DrawingName) - 4) & RevNo & ".dxf" & vbNewLine
                        OWCounter += 1
                    End If
                End If

                CloseLater(DrawingName, odoc)
                Counter += 1
                Title = "Checking"
                ProgressBar(Total, Counter, Title, DrawingName)
                'Display any files that will be overwritten
            End If
        Next
        Counter = 1
        If Overwrite = "" And ExportType = "PDF" Then
            PDFCreator(Path, odoc, PDFSource, Archive, DrawingName, DrawSource, Destin, OpenDocs, Total, Counter)
        ElseIf Overwrite = "" And ExportType = "DXF" Then
            DXFCreator(Path, odoc, Destin, Archive, DrawingName, DrawSource, OpenDocs, Total, Counter, DXFSource)
        ElseIf Overwrite = "" And ExportType = "DWG" Then
            ExportPart(DrawSource, Archive, False, Destin, DrawingName, OpenDocs, "DWG", RevNo)
        ElseIf OWCounter > 10 Then
            Ans = MsgBox("More than 10 files" & vbNewLine & "will be overwritten." _
                       & vbNewLine & " Do you wish to continue?", vbOKCancel, "Overwrite")
            If Ans = vbOK And ExportType = "PDF" Then
                PDFCreator(Path, odoc, PDFSource, Archive, DrawingName, DrawSource, Destin, OpenDocs, Total, Counter)
            ElseIf Ans = vbOK And ExportType = "DXF" Then
                DXFCreator(Path, odoc, Destin, Archive, DrawingName, DrawSource, OpenDocs, Total, Counter, DXFSource)
            Else
                chkCheck.CheckState = CheckState.Unchecked
                MsVistaProgressBar1.Visible = False
                Exit Sub
            End If
        Else
            Ans = MsgBox("The following files will be overwritten:" _
                         & vbNewLine & vbNewLine & Overwrite, vbOKCancel, "Overwrite")
            If Ans = vbOK And ExportType = "PDF" Then
                PDFCreator(Path, odoc, PDFSource, Archive, DrawingName, DrawSource, Destin, OpenDocs, Total, Counter)
            ElseIf Ans = vbOK And ExportType = "DXF" Then
                DXFCreator(Path, odoc, Destin, Archive, DrawingName, DrawSource, OpenDocs, Total, Counter, DXFSource)
            ElseIf Ans = vbOK And ExportType = "DWG" Then
                ExportPart(DrawSource, Archive, False, Destin, DrawingName, OpenDocs, "DWG", RevNo)
            Else
                chkCheck.CheckState = CheckState.Unchecked
                Exit Sub
            End If
        End If
    End Sub
    Private Sub PDFCreator(Path As Documents, ByRef oDoc As Document, ByRef PDFSource As String, ByRef Archive As String _
                             , ByRef DrawingName As String, ByRef DrawSource As String, ByRef Destin As String, OpenDocs As ArrayList _
                             , Total As Integer, Counter As Integer)
        ' Get the PDF translator Add-In.
        Dim ErrorReport As String = ""
        Dim Title, RevNo As String
        Dim oPDFTrans As Inventor.ApplicationAddIn
        oPDFTrans = _invApp.ApplicationAddIns.ItemById("{0AC6FD96-2F4D-42CE-8BE0-8AEA580399E4}")
        If chkPDF.Checked = True And oPDFTrans Is Nothing Then
            MsgBox("Could Not access PDF translator.")
            Exit Sub
        End If
        ' Create some objects that are used to pass information to the translator Add-In.   
        Dim oContext As TranslationContext
        oContext = _invApp.TransientObjects.CreateTranslationContext
        Dim oOptions As NameValueMap
        oOptions = _invApp.TransientObjects.CreateNameValueMap
        'Go through drawings to see which ones are selected
        _invApp.SilentOperation = True
        For X = 0 To LVSubFiles.Items.Count - 1
            'Look through all sub files in open documents to get the part sourcefile
            If LVSubFiles.Items(X).Checked = True Then
                'iterate through opend documents to find the selected file
                DrawSource = Strings.Left(SubFiles.Item(X).Value, Len(SubFiles.Item(X).Value) - 3) & "idw"
                DrawingName = Trim(SubFiles.Item(X).Key)
                'open drawing
                oDoc = _invApp.Documents.Open(DrawSource, True)
                If oPDFTrans.HasSaveCopyAsOptions(_invApp.ActiveDocument, oContext, oOptions) Then
                    For P = 1 To oDoc.sheets.count
                        oDoc.sheets.item(P).activate()
                    Next
                    Dim Mass As Double = oDoc.ReferencedFiles.Item(1).componentdefinition.massproperties.mass
                    'Next
                    Call oDoc.Update()
                    On Error Resume Next
                    oOptions.Value("Sheet_Range") = Inventor.PrintRangeEnum.kPrintAllSheets
                    oOptions.Value("All_Color_AS_Black") = False
                    oOptions.Value("Remove_Line_Weights") = True
                    oOptions.Value("Vector_Resolution") = 5000
                    ' Define various settings and input to provide the translator.  
                    oContext.Type = IOMechanismEnum.kFileBrowseIOMechanism
                    Dim oData As DataMedium
                    oData = _invApp.TransientObjects.CreateDataMedium
                    'create save locations
                    If My.Settings.PDFRev = True Then
                        RevNo = "-R" & oDoc.PropertySets.Item("{F29F85E0-4FF9-1068-AB91-08002B27B3D9}").ItemByPropId("9").Value
                    Else
                        RevNo = ""
                    End If
                    PDFSource = Destin & "\" & DrawingName
                    PDFSource = PDFSource.Insert(PDFSource.LastIndexOf("."), RevNo)
                    'PDFSource = Destin & "\" & DrawingName & "-R" & RevNo
                    PDFSource = Replace(PDFSource, "idw", "pdf")
                    'check if the file exists. If the directory is missing, create a new one
                    If chkPDF.CheckState = CheckState.Checked Then
                        If My.Computer.FileSystem.DirectoryExists(Strings.Left(PDFSource, InStrRev(PDFSource, "\"))) = False Then
                            MkDir(Strings.Left(PDFSource, InStrRev(PDFSource, "\")))
                        End If
                        If My.Computer.FileSystem.DirectoryExists(Strings.Left(PDFSource, InStrRev(PDFSource, "\")) & "Archived\") = False Then
                            MkDir(Strings.Left(PDFSource, InStrRev(PDFSource, "\")) & "Archived\")
                        End If
                        'if an older revision exists move it into the archive folder
                        'if the archive folder doesn't exist create a new one
                        For Each file As IO.FileInfo In Get_Files(Strings.Left(PDFSource, InStrRev(PDFSource, "\")), IO.SearchOption.TopDirectoryOnly, "pdf", Strings.Left(DrawingName, Len(DrawingName) - (Len(RevNo) + 4)))
                            If file.FullName <> PDFSource Then
                                If My.Computer.FileSystem.FileExists(file.FullName.Insert(file.FullName.LastIndexOf("\"), "\Archived")) Then
                                    Kill(file.FullName)
                                Else
                                    System.IO.File.Move(file.FullName, file.FullName.Insert(file.FullName.LastIndexOf("\"), "\Archived"))
                                End If
                            End If
                        Next
                        Dim oDocument As Document
                        oDocument = _invApp.ActiveDocument
                        ' Call the translator.  
                        oData.FileName = PDFSource
                        Call oPDFTrans.SaveCopyAs(oDocument, oContext, oOptions, oData)
                    End If
                    'If the drawingcreation causes an error, save drawing name and inform user when process is complete
                    CloseLater(DrawingName, oDoc)
                    If Err.Number = -2147467259 Then
                        ErrorReport = DrawingName & vbNewLine
                        Err.Clear()
                    End If
                End If
                Title = "Saving: "
                ProgressBar(Total, Counter, Title, DrawingName)
                Counter += 1
            End If
        Next
        _invApp.SilentOperation = False
        MsVistaProgressBar1.Visible = False
        'notify user of drawings that caused problems
        If Len(ErrorReport) > 0 Then
            MsgBox("The following files were Not created: " & vbNewLine & vbNewLine & ErrorReport & vbNewLine &
                   "Check to make sure the drawing isn't open or write-protected.")
        End If
    End Sub
    Private Function Get_Files(ByVal directory As String,
                           ByVal recursive As IO.SearchOption,
                           ByVal ext As String,
                           ByVal with_word_in_filename As String) As List(Of IO.FileInfo)
        'Check the directory for older revisions that have the same base name and select them to be moved to the archive folder
        Return IO.Directory.GetFiles(directory, "*" & If(ext.StartsWith("*"), ext.Substring(1), ext), recursive) _
                           .Where(Function(o) o.ToLower.Contains(with_word_in_filename.ToLower)) _
                           .Select(Function(p) New IO.FileInfo(p)).ToList

    End Function
    Public Sub DXFCreator(Path As Documents, ByRef oDoc As Document, ByRef Destin As String, ByRef Archive As String _
                             , ByRef DrawingName As String, ByRef DrawSource As String, OpenDocs As ArrayList _
                             , Total As Integer, Counter As Integer, DXFSource As String)
        Dim sReadableType As String = ""
        Dim Title, RevNo As String
        Dim NotMade As String = ""
        _invApp.SilentOperation = True
        'Go through drawings to see which ones are selected
        For X = 0 To LVSubFiles.Items.Count - 1
            'Look through all sub files in open documents to get the part sourcefile
            If LVSubFiles.Items(X).Checked = True Then
                'iterate through opend documents to find the selected file
                DrawingName = Trim(LVSubFiles.Items(X).Text)
                DrawSource = Strings.Left(SubFiles.Item(X).Value, Len(SubFiles.Item(X).Value) - 3) & "idw"
                'open drawing
                oDoc = _invApp.Documents.Open(DrawSource, False)
                If My.Settings.DXFRev = True Then
                    RevNo = "-R" & oDoc.PropertySets.Item("{F29F85E0-4FF9-1068-AB91-08002B27B3D9}").ItemByPropId("9").Value
                Else
                    RevNo = ""
                End If
                DXFSource = Destin & "\" & DrawingName
                DXFSource = DXFSource.Insert(DXFSource.LastIndexOf("."), RevNo)
                'PDFSource = Destin & "\" & DrawingName & "-R" & RevNo
                DXFSource = Replace(DXFSource, "idw", "dxf")
                'DXFSource = DrawSource.Insert(DrawSource.LastIndexOf("\"), "\DWG_DXF")
                'DXFSource = DXFSource.Insert(DXFSource.LastIndexOf("."), "-R" & RevNo)
                'DXFSource = Replace(DXFSource, "idw", "dxf")

                If My.Computer.FileSystem.FileExists(Strings.Replace(SubFiles.Item(X).Value, "idw", "ipt")) = True Then
                    Archive = Strings.Replace(SubFiles.Item(X).Value, "idw", "ipt")
                    oDoc = _invApp.Documents.Open(Archive, True)
                    SheetMetalTest(Archive, oDoc, sReadableType)
                    If sReadableType = "P" And chkDWG.Checked = False Then
                        Call ExportPart(DrawSource, Archive, False, Destin, DrawingName, OpenDocs, "DXF", RevNo)
                    ElseIf sReadableType = "S" And chkDWG.Checked = False And chkUseDrawings.Checked = False Then
                        Call SMDXF(oDoc, DXFSource, True)
                        CloseLater(Strings.Left(DrawingName, Len(DrawingName) - 3) & "ipt", oDoc)
                    ElseIf sReadableType = "S" And chkDWG.Checked = False And chkUseDrawings.Checked = True Then
                        Call ExportPart(DrawSource, Archive, False, Destin, DrawingName, OpenDocs, "DXF", RevNo)
                    ElseIf sReadableType = "P" And chkDWG.Checked = True Then
                        Call ExportPart(DrawSource, Archive, False, Destin, DrawingName, OpenDocs, "DWG", RevNo)
                    ElseIf sReadableType = "" Then
                        CloseLater(Strings.Right(oDoc.FullDocumentName, Len(oDoc.FullDocumentName) - InStrRev(oDoc.FullDocumentName, "\")), oDoc)
                    End If
                    If chkCheck.CheckState = CheckState.Indeterminate Then
                        chkCheck.CheckState = CheckState.Unchecked
                    End If
                ElseIf My.Computer.FileSystem.FileExists(Strings.Replace(SubFiles.Item(X).Value, "idw", "iam")) = False Then
                    NotMade = DrawingName & vbNewLine
                ElseIf My.Computer.FileSystem.FileExists(Strings.Replace(SubFiles.Item(X).Value, "idw", "iam")) = True Then
                    Call ExportPart(DrawSource, Archive, False, Destin, DrawingName, OpenDocs, "DXF", RevNo)
                End If
                Title = "Saving:  "
                ProgressBar(Total, Counter, Title, DrawingName)
                Counter += 1
            End If
        Next
        If NotMade <> "" Then
            MsgBox("There was a problem generating the following files:" & vbNewLine & vbNewLine & NotMade)
        End If
        _invApp.SilentOperation = False
    End Sub
    Private Sub btnFlatPattern_Click(sender As Object, e As EventArgs) Handles btnFlatPattern.Click
        Dim Archive As String = ""
        Dim PartName As String = ""
        Dim PartSource As String = ""
        Dim oDoc As Document = Nothing
        Dim sReadableType As String = ""
        Dim Path As Documents = _invApp.Documents
        Dim RevNo As String = ""
        Dim DXFSource As String = ""
        Dim Title As String = "Saving: "
        Dim Total As String = lstOpenfiles.CheckedItems.Count
        Dim Flag As Boolean = False
        For X = 0 To lstOpenfiles.Items.Count - 1
            If InStr(lstOpenfiles.Items(X), "ipt") <> 0 Then
                Flag = True
                Exit For

            End If
        Next
        If Flag = False Then
            MsgBox("No part selected." & vbNewLine & "For drawings use the drawing action list.")
        Else
            For X = 0 To lstOpenfiles.Items.Count - 1
                'Look through all sub files in open documents to get the part sourcefile
                If lstOpenfiles.GetItemCheckState(X) = CheckState.Checked Then
                    PartName = lstOpenfiles.Items(X)
                    PartSource = OpenFiles.Item(X).Value
                    oDoc = _invApp.Documents.Open(PartSource, False)
                    If My.Computer.FileSystem.FileExists(Strings.Replace(PartSource, "ipt", "idw")) Then
                        Dim dDoc As DrawingDocument = _invApp.Documents.Open(Strings.Replace(PartSource, "ipt", "idw"), False)
                        RevNo = dDoc.PropertySets.Item("{F29F85E0-4FF9-1068-AB91-08002B27B3D9}").ItemByPropId("9").Value
                        dDoc.Close()
                    Else
                        RevNo = 0
                    End If

                    DXFSource = PartSource.Insert(PartSource.LastIndexOf("\"), "\DWG_DXF")
                    If My.Settings.DXFRev = True Then
                        DXFSource = DXFSource.Insert(DXFSource.LastIndexOf("."), "-R" & RevNo)
                    End If
                    DXFSource = Replace(DXFSource, "ipt", "dxf")
                    SheetMetalTest(Archive, oDoc, sReadableType)
                    If sReadableType = "S" Then
                        Call SMDXF(oDoc, DXFSource, True)
                    End If
                    ProgressBar(Total, X + 1, "Saving: ", PartName)
                End If
            Next
        End If
        MsVistaProgressBar1.Visible = False
    End Sub
    Private Sub SheetMetalTest(ByRef Archive As String, oDoc As Document, ByRef sReadableType As String)
        Dim sDocumentSubType As String = oDoc.SubType
        'Get document type
        Dim eDocumentType As DocumentTypeEnum = oDoc.DocumentType
        'Check document type for sheet metal
        If chkSkipAssy.CheckState = CheckState.Checked Then
            If sDocumentSubType = "{28EC8354-9024-440F-A8A2-0E0E55D635B0}" Or sDocumentSubType = "{E60F81E1-49B3-11D0-93C3-7E0706000000}" Then
                sReadableType = ""
                Exit Sub
            End If
        ElseIf chkSkipAssy.CheckState = CheckState.Unchecked And sDocumentSubType = "{E60F81E1-49B3-11D0-93C3-7E0706000000}" Then
            sReadableType = "P"
        End If
        If chkUseDrawings.CheckState = CheckState.Checked And chkSkipAssy.CheckState = CheckState.Unchecked Then
            sReadableType = "P"
            Exit Sub
        End If
        Select Case sDocumentSubType
            Case "{4D29B490-49B2-11D0-93C3-7E0706000000}"
                'Part
                sReadableType = "P"
            Case "{28EC8354-9024-440F-A8A2-0E0E55D635B0}"
                sReadableType = "P"
            Case "{9C464203-9BAE-11D3-8BAD-0060B0CE6BB4}"
                sReadableType = "S"
                'Sheet Metal
        End Select
    End Sub
    Private Sub ExportPart(DrawSource As String, Archive As String, FlatPattern As Boolean, Destin As String, DrawingName As String,
             OpenDocs As ArrayList, Output As String, RevNo As String)



        Dim odoc As Document = _invApp.ActiveDocument
        If FlatPattern = False Then
            odoc = _invApp.Documents.Open(DrawSource, True)
        Else
            odoc = _invApp.Documents.Open(Archive, True)
        End If
        Dim oDWGAddIn As TranslatorAddIn = Nothing
        Dim i As Long
        Dim strIniFile As String
        Dim DwgName, DWGSource, ExportName As String
        Archive = _invApp.ActiveDocument.FullDocumentName
        DwgName = Strings.Right(Archive, Len(Archive) - InStrRev(Archive, "\"))
        DWGSource = Destin & "\" & DrawingName
        DWGSource = DWGSource.Insert(DWGSource.LastIndexOf("."), RevNo)
        'PDFSource = Destin & "\" & DrawingName & "-R" & RevNo
        ExportName = Replace(DWGSource, "idw", LCase(Output))
        'DWGSource = Replace(DWGSource, "idw", "dwg")
        'DXFSource = Replace(DWGSource, "dwg", "dxf")

        'DXFSource = Strings.Left(Archive, InStrRev(Archive, "\")) & "DWG_DXF\" & Strings.Left(DwgName, Len(DwgName) - 4) & "-R" & RevNo & ".dxf"

        'DWGSource = Strings.Left(Archive, InStrRev(Archive, "\")) & "DWG_DXF\" & Strings.Left(DwgName, Len(DwgName) - 4) & "-R" & RevNo & ".dwg"
        ' Create a name-value map to
        ' supply information
        ' to the translator.
        'ExportName = Strings.Right(ExportName, Len(ExportName) - InStrRev(ExportName, "\"))
        If My.Computer.FileSystem.DirectoryExists(Strings.Left(ExportName, InStrRev(ExportName, "\"))) = False Then
            MkDir(Strings.Left(ExportName, InStrRev(ExportName, "\")))
        End If
        If My.Computer.FileSystem.DirectoryExists(Strings.Left(ExportName, InStrRev(ExportName, "\")) & "Archived\") = False Then
            MkDir(Strings.Left(ExportName, InStrRev(ExportName, "\")) & "Archived\")
        End If
        For Each file As IO.FileInfo In Get_Files(Strings.Left(ExportName, InStrRev(ExportName, "\")), IO.SearchOption.TopDirectoryOnly, "dxf", Strings.Left(DrawingName, Len(DrawingName) - (Len(RevNo) + 4)))
            If file.FullName <> ExportName Then
                If Not My.Computer.FileSystem.FileExists(file.FullName.Insert(file.FullName.LastIndexOf("\"), "\Archived")) Then
                    System.IO.File.Move(file.FullName, file.FullName.Insert(file.FullName.LastIndexOf("\"), "\Archived"))
                End If
            End If
        Next
        Dim oNameValueMap As NameValueMap
        oNameValueMap = _invApp.TransientObjects.CreateNameValueMap
        ' Define the type of output by
        ' specifying the filename.
        Dim oOutputFile As DataMedium
        oOutputFile = _invApp.TransientObjects.CreateDataMedium
        For i = 1 To _invApp.ApplicationAddIns.Count
            If Output = "DXF" Then
                If _invApp.ApplicationAddIns.Item(i).ClassIdString = "{C24E3AC2-122E-11D5-8E91-0010B541CD80}" Then
                    oDWGAddIn = _invApp.ApplicationAddIns.Item(i)
                    strIniFile = My.Resources.DXF
                    Call oNameValueMap.Add("Export_Acad_IniFile", strIniFile)
                    oOutputFile.FileName = ExportName
                    Exit For
                End If
            ElseIf Output = "DWG" Then
                If _invApp.ApplicationAddIns.Item(i).ClassIdString = "{C24E3AC2-122E-11D5-8E91-0010B541CD80}" Then
                    oDWGAddIn = _invApp.ApplicationAddIns.Item(i)
                    strIniFile = My.Resources.dwg
                    Call oNameValueMap.Add("Export_Acad_IniFile", strIniFile)
                    oOutputFile.FileName = ExportName
                    Exit For
                End If
            Else
                MsgBox("Error: No export type selected")
                Exit Sub
            End If
        Next


        If oDWGAddIn Is Nothing Then
            MsgBox("DWG add-in not found.")
            Exit Sub
        End If

        ' Check to make sure the add-in
        ' is activated.
        If Not oDWGAddIn.Activated Then
            oDWGAddIn.Activate()
        End If

        ' Create a translation context and define
        ' that we want to output to a file.
        Dim oContext As TranslationContext
        oContext = _invApp.TransientObjects.CreateTranslationContext
        oContext.Type = IOMechanismEnum.kFileBrowseIOMechanism


        ' Call the SaveCopyAs method of the add-in.
        Call oDWGAddIn.SaveCopyAs(_invApp.ActiveDocument, oContext, oNameValueMap, oOutputFile)
        CloseLater(Strings.Right(DrawSource, Strings.Len(DrawSource) - Strings.InStrRev(DrawSource, "\")), _invApp.Documents.Open(DrawSource, True))
        CloseLater(Strings.Right(Archive, Strings.Len(Archive) - Strings.InStrRev(Archive, "\")), _invApp.Documents.Open(Archive, True))
        Me.Focus()
    End Sub
    Private Sub SMDXF(oDoc As Document, DXFSource As String, Flatpattern As Boolean)
        'Dim oPartDoc As Document = _invApp.ActiveDocument
        _invApp.SilentOperation = True
        Dim oCompDef As ComponentDefinition = oDoc.ComponentDefinition
        Dim oDef As ControlDefinition
        On Error Resume Next
        If oCompDef.HasFlatPattern = False Then
            'go to flat pattern or create it if it doesn't exist
            oCompDef.Unfold()
            oDef = _invApp.CommandManager.ControlDefinitions.Item("PartSwitchRepresentationCmd")
            oDef.Execute()
            'Return to folded model
            oDef = _invApp.CommandManager.ControlDefinitions.Item("PartConvertToSheetMetalCmd")
            oDef.Execute()
            'Get active document
            If Len(Err.Description) <> 0 Then
                MsgBox("An Error occurred during the flat pattern creation Of " & Strings.Right(oDoc.FullDocumentName, Len(oDoc.FullDocumentName) - InStrRev(oDoc.FullDocumentName, "\")) & "." & vbNewLine _
                      & "The .dxf was Not created" & vbNewLine & "Some parts require the flat patterns To be created manually." _
                      & vbNewLine & vbNewLine & Err.Description)
                Err.Clear()
            End If
        End If
        If My.Computer.FileSystem.DirectoryExists(Strings.Left(DXFSource, InStrRev(DXFSource, "\"))) = False Then
            MkDir(Strings.Left(DXFSource, InStrRev(DXFSource, "\")))
        End If
        If My.Computer.FileSystem.DirectoryExists(Strings.Left(DXFSource, InStrRev(DXFSource, "\")) & "Archived\") = False Then
            MkDir(Strings.Left(DXFSource, InStrRev(DXFSource, "\")) & "Archived\")
        End If
        Dim oDataIO As DataIO
        oDataIO = oCompDef.DataIO
        'Build the string that defines the format of the DXF file
        Dim sOut As String
        '&OuterProfileLayer=IV_INTERIOR_PROFILES"
        sOut = "FLAT PATTERN DXF?AcadVersion=2000" &
        "&OuterProfileLayer=IV_OUTER_PROFILE&OuterProfileLayerLineType=37633&OuterProfileLayerLineWeight=0,0500&OuterProfileLayerColor=0;0;0" &
        "&InteriorProfilesLayer=IV_INTERIOR_PROFILES&InteriorProfilesLayerLineType=37633&InteriorProfilesLayerLineWeight=0,0500&InteriorProfilesLayerColor=0;0;0" &
        "&InvisibleLayers=IV_TANGENT;IV_BEND;IV_Bend_Down;IV_Bend​_Up;IV_ARC_CENTERS"
        '
        '
        oDataIO.WriteDataToFile(sOut, DXFSource)
        If Len(Err.Description) <> 0 Then
            MsgBox("An error occurred while saving " & Strings.Right(oDoc.FullDocumentName, Len(oDoc.FullDocumentName) - InStrRev(oDoc.FullDocumentName, "\")) & "." & vbNewLine _
                  & "Ensure the dxf is not open or write protected." _
                  & vbNewLine & vbNewLine & Err.Description)
            Err.Clear()
        End If
        _invApp.SilentOperation = False
        ' "&TangentLayer=IV_TANGENT&deletelayer=TrueTangentLayerLineType=37633&TangentLayerLineWeight=0,0500&TangentLayerColor=0;0;0" & _
        '"&BendLayer=BEND&BendLayerLineType=37633&BendLayerLineWeight=0,0500&BendLayerColor=0;0;0" & _
        '"&BendDownLayer=BENDD&BendDownLayerLineType=37633&BendDownLayerLineWeight=0,0500&BendDownLayerColor=0;0;0" & _
        ' "&ArcCentersLayer=ARC&ArcCentersLayerLineType=37633&ArcCentersLayerLineWeight=0,0500&ArcCentersLayerColor=0;0;0" & _
        ' "&OuterProfileLayer=IV_OUTER_PROFILE&OuterProfileLayerLineType=37633&OuterProfileLayerLineWeight=0,0500&OuterProfileLayerColor=0;0;0" & _
        ' "&InteriorProfilesLayer=IV_INTERIOR_PROFILES&InteriorProfilesLayerLineType=37633&InteriorProfilesLayerLineWeight=0,0500&InteriorProfilesLayerColor=0;0;0"

        '' ---- Creating DXF from flat pattern through the .INI file doesn't seem to be possible
        '' ---- the code below is kept for reference only
        'Dim oDXFAddin As TranslatorAddIn = Nothing
        'For i As Long = 1 To _invApp.ApplicationAddIns.Count
        ' If _invApp.ApplicationAddIns.Item(i).ClassIdString = "{C24E3AC4-122E-11D5-8E91-0010B541CD80}" Then
        ' oDXFAddin = _invApp.ApplicationAddIns.Item(i)
        ' Exit For
        ' End If
        ' Next
        ' If oDXFAddin Is Nothing Then
        ' MsgBox("The DXF Add-in could not be found")
        ' Exit Sub
        ' End If
        ' If Not oDXFAddin.Activated Then oDXFAddin.Activate()
        ' Dim oContext As TranslationContext = _invApp.TransientObjects.CreateTranslationContext
        ' oContext.Type = IOMechanismEnum.kFileBrowseIOMechanism
        ' Dim oNameValueMap As NameValueMap = _invApp.TransientObjects.CreateNameValueMap
        ' If oDXFAddin.HasSaveCopyAsOptions(oPartDoc, oContext, oNameValueMap) Then
        ' For x = 1 To oNameValueMap.Count
        ' Next
        ' oNameValueMap.Value("DwgVersion") = 25
        ' Dim strIniFile As String = "P:\Inventor System Files\Inventor Add Ins and Programs\IO Profile.ini"
        ' oNameValueMap.Value("Export_Acad_IniFile") = strIniFile
        ' End If

        ' Dim oOutputFile As DataMedium = _invApp.TransientObjects.CreateDataMedium
        ' oOutputFile.FileName = DXFSource

        'Call oDXFAddin.SaveCopyAs(oPartDoc, oContext, oNameValueMap, oOutputFile)

    End Sub
    Private Sub CloseDrawings(Path As Documents, ByRef odoc As Document, ByRef Archive As String _
                                 , ByRef DrawingName As String, ByRef DrawSource As String, OpenDocs As ArrayList)
        Dim Ans As String
        Dim Checkin As Boolean
        Ans = MsgBox("Do you wish to save these drawings?", vbYesNoCancel, "Close Drawings")
        If Ans = vbYes Then
            Checkin = True
        ElseIf Ans = vbNo Then
            Checkin = False
        Else
            Exit Sub
        End If
        'Go through drawings to see which ones are selected
        For x = 0 To LVSubFiles.CheckedItems.Count - 1
            'iterate through all open documents in inventor to find selected document
            For Each item As Document In _invApp.Documents
                If Strings.Right(item.FullDocumentName, Len(item.FullDocumentName) - InStrRev(item.FullDocumentName, "\")) = Trim(LVSubFiles.CheckedItems(x).Text) Then
                    'save document if user selected save
                    If Checkin = True Then
                        item.Save()
                        item.Close(True)
                        Exit For
                        ' otherwise close without save
                    Else
                        item.Close(True)
                        Exit For
                    End If
                End If
            Next
        Next
        UpdateForm()
    End Sub
    Public Sub ProgressBar(Total As Integer, Counter As Integer, Title As String, FileName As String)
        Dim Percent As Integer = (Counter / Total) * 100
        MsVistaProgressBar1.Visible = True
        If Percent > 100 Then Percent = 100
        Me.MsVistaProgressBar1.Value = Percent
        If Title = "Found: " Or Title = "Getting References: " Then
            Me.MsVistaProgressBar1.DisplayText = Title & " " & FileName
        Else
            Me.MsVistaProgressBar1.DisplayText = Title & " " & FileName & " " & Percent & "%"
        End If
    End Sub
    Public Sub CloseLater(Name As String, oDoc As Document)
        Dim CloseLater As Boolean = True
        'Go through string of originally open documents to see if document has been opened by the program
        For Each Str As String In OpenDocs
            If Str.Contains(Name) Then
                'If found, don't close
                CloseLater = False
                Exit For
            End If
        Next
        'If the drawing wasn't found close it. This keeps the system from having too many open documents in memory.
        If CloseLater = True Then
            oDoc.Close(True)
        End If
    End Sub
    Private Sub btnSpreadsheet_Click(sender As System.Object, e As System.EventArgs) Handles btnSpreadsheet.Click
        VBAFlag = "NA"
        Dim C, Total, Ans As Integer
        Dim Counter As Integer = 1
        Total = 0
        Dim AssyName, SaveName, PreSave As String
        SaveName = "" : PreSave = ""
        Dim AsmDoc As AssemblyDocument
        Dim oDoc As Document = Nothing
        Dim AsmDef As AssemblyComponentDefinition
        Dim PropSets As PropertySets
        Dim ExcelDoc As Excel.Workbook = Nothing
        Dim OpenDocs As New ArrayList
        CreateOpenDocs(OpenDocs)
        Dim Elog As String = ""
        Dim Start As Date = Now()
        Dim _ExcelApp As New Excel.Application
        For X = 0 To lstOpenfiles.Items.Count - 1
            If lstOpenfiles.GetItemCheckState(X) = CheckState.Checked Then
                If Strings.Right(lstOpenfiles.Items.Item(X).ToString, 3) = "iam" Then
                    C += 1
                End If
            End If
        Next
        If C = 0 Then
            MsgBox("Please select an assembly")
            Exit Sub
        End If
        If C > 1 Then
            MsgBox("Please only select one assembly")
            Exit Sub
        End If
        For X = 0 To lstOpenfiles.Items.Count - 1
            If lstOpenfiles.GetItemCheckState(X) = CheckState.Checked Then
                AssyName = Trim(lstOpenfiles.Items.Item(X).ToString)
                For Each oDoc In _invApp.Documents
                    If InStr(oDoc.FullDocumentName, AssyName) <> 0 Then
                        oDoc.Activate()
                        If oDoc.DocumentType = DocumentTypeEnum.kAssemblyDocumentObject Then
                            AsmDoc = _invApp.ActiveDocument
                        Else
                            MsgBox("The assembly must be active.")
                            Exit Sub
                        End If
                        Try
                            AsmDef = AsmDoc.ComponentDefinition
                            TraverseAssembly(AsmDoc.ComponentDefinition.Occurrences, "", 0, Total, Counter, OpenDocs, Elog, False)
                            If My.Computer.FileSystem.FileExists(IO.Path.Combine(IO.Path.GetTempPath, "List-Blank.xlsm")) Then
                                Kill(IO.Path.Combine(IO.Path.GetTempPath, "List-Blank.xlsm"))
                            End If
                            If My.Computer.FileSystem.FileExists(IO.Path.Combine(IO.Path.GetTempPath, "Quote-Blank.xlsm")) Then
                                Kill(IO.Path.Combine(IO.Path.GetTempPath, "Quote-Blank.xlsm"))
                            End If
                            IO.File.WriteAllBytes(IO.Path.Combine(IO.Path.GetTempPath, "List-Blank.xlsm"), My.Resources.List_Blank)
                            IO.File.WriteAllBytes(IO.Path.Combine(IO.Path.GetTempPath, "Quote-Blank.xlsm"), My.Resources.Quote_Blank)
                            Dim xlPath = IO.Path.Combine(IO.Path.GetTempPath, "List-Blank.xlsm")
                            _ExcelApp.Workbooks.Open(xlPath)
                            _ExcelApp.Visible = False
                            ExcelDoc = _ExcelApp.ActiveWorkbook
                            'ShowParameters(oDoc)
                            GetProperties(oDoc, AsmDef.Occurrences, 0, 0, ExcelDoc, Total)
                            MsVistaProgressBar1.Visible = False
                            ExcelDoc.Worksheets("Saw Cut Lengths").visible = False
                            'UpdateCustomiProperty(oDoc, "", "")
                            PropSets = AsmDoc.PropertySets
                            'SaveName = Strings.Left(AsmDoc.FullDocumentName, InStrRev(AsmDoc.FullDocumentName, "\") - 1)
                            SaveName = Strings.Left(AsmDoc.FullDocumentName, InStrRev(AsmDoc.FullDocumentName, "\"))
                            PreSave = Strings.Left(Strings.Right(AsmDoc.FullDocumentName, Len(AsmDoc.FullDocumentName) - InStrRev(AsmDoc.FullDocumentName, "\")),
                                                   Strings.Len(Strings.Right(AsmDoc.FullDocumentName, Len(AsmDoc.FullDocumentName) - InStrRev(AsmDoc.FullDocumentName, "\"))) - 4) & "-List.xlsm"
                            If My.Computer.FileSystem.DirectoryExists(SaveName & "Vendor Quotes\") Then
                            Else
                                My.Computer.FileSystem.CreateDirectory(SaveName & "Vendor Quotes\")
                            End If
                            _ExcelApp.ActiveWorkbook.SaveCopyAs(SaveName & "Vendor Quotes\" & PreSave)
                            _ExcelApp.DisplayAlerts = False
                            ExcelDoc.Close()

                        Catch ex As Exception
                            MessageBox.Show(ex.Message, "Exception Details", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        Finally
                            Ans = MsgBox("Do you wish to open the file?", vbYesNo, "Finished")
                            If Ans = vbYes Then
                                _ExcelApp.Workbooks.Open(SaveName & "Vendor Quotes\" & PreSave)
                                _ExcelApp.Visible = True
                            Else
                                _ExcelApp.Visible = True

                                _ExcelApp.Quit()
                                _ExcelApp.DisplayAlerts = True
                                _ExcelApp = Nothing
                            End If
                        End Try
                        Exit Sub
                    End If
                Next
            End If
        Next
    End Sub
    Private Sub GetProperties(ByRef oDoc As Inventor.Document, ByVal Occurrences As ComponentOccurrences, ByRef Properties As Double, ByRef Counter As Integer, ExcelDoc As Excel.Workbook, Total As Integer)
        Dim Occ As ComponentOccurrence
        Dim oPartDef As ComponentDefinition
        Dim PropSets, customPropSet, UserDefProps As Inventor.PropertySets
        Dim Process, StockNo, Material, PartNo, Description, Vendor, Length, Width, Thk, Area, Title As String
        Dim Offset As Integer = 1
        Title = "Extracting"
        For Each Occ In Occurrences
            Counter += 1
            'Check if the document is a part or assembly
            If Occ.DefinitionDocumentType = DocumentTypeEnum.kPartDocumentObject Or
                DocumentTypeEnum.kAssemblyDocumentObject And Occ.BOMStructure = BOMStructureEnum.kPurchasedBOMStructure Then
                'Get iProperties for this document
                PropSets = Occ.Definition.Document.PropertySets()
                Process = PropSets.Item("{D5CDD502-2E9C-101B-9397-08002B2CF9AE}").ItemByPropId("2").Value.ToString
                If Occ.BOMStructure = BOMStructureEnum.kPurchasedBOMStructure Then
                    Process = "PP"
                ElseIf Occ.BOMStructure = BOMStructureEnum.kReferenceBOMStructure Then
                    Process = "Ref"
                ElseIf Occ.BOMStructure = BOMStructureEnum.kPhantomBOMStructure Then
                    Process = "PH"
                End If
                StockNo = PropSets.Item("{32853F0F-3444-11D1-9E93-0060B03C1CA6}").ItemByPropId("55").Value.ToString
                Material = PropSets.Item("{32853F0F-3444-11D1-9E93-0060B03C1CA6}").ItemByPropId("20").Value.ToString
                PartNo = PropSets.Item("{32853F0F-3444-11D1-9E93-0060B03C1CA6}").ItemByPropId("5").Value.ToString
                Description = PropSets.Item("{32853F0F-3444-11D1-9E93-0060B03C1CA6}").ItemByPropId("29").Value.ToString
                Vendor = PropSets.Item("{32853F0F-3444-11D1-9E93-0060B03C1CA6}").ItemByPropId("30").Value.ToString

                oPartDef = Occ.Definition
                On Error Resume Next
                'Dim ExcelDoc As Excel.Workbook
                Offset += 1

                Length = oPartDef.Parameters.Item("Length").Value / 30.48
                Width = oPartDef.Parameters.Item("Width").Value / 30.48
                Thk = oPartDef.Parameters.Item("Thickness").Value / 2.54
                Area = oPartDef.MassProperties.Area / 1858
                If Process = "SC" Or Process = "EX" Then
                    ExcelDoc.Worksheets("Saw Cut").activate()
                    Do Until ExcelDoc.ActiveSheet.Range("F" & Offset).Value = ""
                        Offset = Offset + 1
                        If CStr(ExcelDoc.ActiveSheet.Range("F" & Offset).Value) = CStr(StockNo) And
                            CStr(ExcelDoc.ActiveSheet.Range("E" & Offset).Value) = CStr(Material) Then
                            ExcelDoc.ActiveSheet.Range("A" & Offset).Value = ExcelDoc.ActiveSheet.Range("A" & Offset).Value + (Length)
                            If InStrRev(ExcelDoc.ActiveSheet.Range("G" & Offset).Value, PartNo) = "" And
                                ExcelDoc.ActiveSheet.Range("G" & Offset).Value <> "" And
                                InStr(ExcelDoc.ActiveSheet.range("G" & Offset).value, PartNo) = 0 Then
                                ExcelDoc.ActiveSheet.Range("G" & Offset).Value = ExcelDoc.ActiveSheet.Range("G" & Offset).Value & ", " & PartNo
                            End If
                            Exit Do
                        ElseIf ExcelDoc.ActiveSheet.Range("F" & Offset).Value = Nothing Then
                            ExcelDoc.ActiveSheet.Range("A" & Offset).Value = ExcelDoc.ActiveSheet.Range("A" & Offset).Value + (Length)
                            ExcelDoc.ActiveSheet.Range("B" & Offset).Value = "Ft."
                            ExcelDoc.ActiveSheet.Range("F" & Offset).Value = StockNo
                            ExcelDoc.ActiveSheet.Range("E" & Offset).Value = Material
                            ExcelDoc.ActiveSheet.Range("G" & Offset).Value = PartNo
                            Exit Do
                        Else
                            'MsgBox "Jump to next line"
                        End If
                    Loop
                    Offset = 1
                    ExcelDoc.Worksheets("Saw Cut Lengths").Activate()
                    Do Until ExcelDoc.ActiveSheet.Range("C" & Offset).Value = ""
                        Offset = Offset + 1

                        If ExcelDoc.ActiveSheet.Range("C" & Offset).Value = Nothing Then
                            ExcelDoc.ActiveSheet.Range("A" & Offset).Value = Length
                            ExcelDoc.ActiveSheet.Range("B" & Offset).Value = "Ft."
                            ExcelDoc.ActiveSheet.Range("D" & Offset).Value = StockNo
                            ExcelDoc.ActiveSheet.Range("C" & Offset).Value = Material
                            ExcelDoc.ActiveSheet.Range("E" & Offset).Value = PartNo
                            Exit Do
                        End If
                    Loop
                ElseIf Process = "LC" Then
                    ExcelDoc.Worksheets("Profile Cut").Activate()
                    Do Until ExcelDoc.ActiveSheet.Range("D" & Offset).Value = ""
                        Offset = Offset + 1
                        If CStr(ExcelDoc.ActiveSheet.Range("D" & Offset).Value) = CStr(StockNo) Then
                            ExcelDoc.ActiveSheet.Range("A" & Offset).Value = ExcelDoc.ActiveSheet.Range("A" & Offset).Value + Area
                            If InStrRev(ExcelDoc.ActiveSheet.Range("E" & Offset).Value, PartNo) = "" And
                                ExcelDoc.ActiveSheet.Range("E" & Offset).Value <> "" Then
                                ExcelDoc.ActiveSheet.Range("E" & Offset).Value = ExcelDoc.ActiveSheet.Range("E" & Offset).Value & ", " & PartNo
                            End If
                            Exit Do
                        ElseIf ExcelDoc.ActiveSheet.Range("C" & Offset).Value = Nothing Then

                            ExcelDoc.ActiveSheet.Range("A" & Offset).Value = ExcelDoc.ActiveSheet.Range("A" & Offset).Value + Area
                            ExcelDoc.ActiveSheet.Range("B" & Offset).Value = "Sq Ft."
                            ExcelDoc.ActiveSheet.Range("D" & Offset).Value = StockNo
                            ExcelDoc.ActiveSheet.Range("C" & Offset).Value = Material
                            ExcelDoc.ActiveSheet.Range("E" & Offset).Value = PartNo
                            Exit Do
                        Else
                            'MsgBox "Jump to next line"
                        End If
                    Loop
                ElseIf Process = "HC" Then
                    ExcelDoc.Worksheets("Torch Cut").Activate()
                    Do Until ExcelDoc.ActiveSheet.Range("D" & Offset).Value = ""
                        Offset = Offset + 1
                        If CStr(ExcelDoc.ActiveSheet.Range("D" & Offset).Value) = CStr(StockNo) Then
                            ExcelDoc.ActiveSheet.Range("A" & Offset).Value = ExcelDoc.ActiveSheet.Range("A" & Offset).Value + (Length * Width)
                            If InStrRev(ExcelDoc.ActiveSheet.Range("E" & Offset).Value, PartNo) = "" Then
                                ExcelDoc.ActiveSheet.Range("E" & Offset).Value = ExcelDoc.ActiveSheet.Range("E" & Offset).Value & ", " & PartNo
                            End If
                            Exit Do
                        ElseIf ExcelDoc.ActiveSheet.Range("D" & Offset).Value = Nothing Then
                            ExcelDoc.ActiveSheet.Range("A" & Offset).Value = ExcelDoc.ActiveSheet.Range("A" & Offset).Value + (Length * Width)
                            ExcelDoc.ActiveSheet.Range("B" & Offset).Value = "Sq Ft."
                            ExcelDoc.ActiveSheet.Range("D" & Offset).Value = StockNo
                            ExcelDoc.ActiveSheet.Range("C" & Offset).Value = Material
                            ExcelDoc.ActiveSheet.Range("E" & Offset).Value = PartNo
                            Exit Do
                        Else
                            'MsgBox "Jump to next line"
                        End If
                    Loop
                ElseIf Process = "PP" Then
                    ExcelDoc.Worksheets("Purchased Parts").Activate()
                    Do Until ExcelDoc.ActiveSheet.Range("B" & Offset).Value = ""
                        Offset = Offset + 1
                        If CStr(ExcelDoc.ActiveSheet.Range("B" & Offset).Value) = CStr(Description) And
                            CStr(ExcelDoc.ActiveSheet.Range("D" & Offset).Value) = CStr(PartNo) Then
                            ExcelDoc.ActiveSheet.Range("A" & Offset).Value = ExcelDoc.ActiveSheet.Range("A" & Offset).Value + 1
                            Exit Do
                        ElseIf ExcelDoc.ActiveSheet.Range("B" & Offset).Value = Nothing Then
                            ExcelDoc.ActiveSheet.Range("A" & Offset).Value = 1
                            ExcelDoc.ActiveSheet.Range("D" & Offset).Value = PartNo
                            ExcelDoc.ActiveSheet.Range("B" & Offset).Value = Description
                            ExcelDoc.ActiveSheet.Range("C" & Offset).Value = Vendor
                            Exit Do
                        Else
                            'MsgBox "Jump to next line"
                        End If
                    Loop
                ElseIf Process = "" And Occ.DefinitionDocumentType = DocumentTypeEnum.kPartDocumentObject Then
                    ExcelDoc.Worksheets("Other Parts").Activate()
                    Do Until ExcelDoc.ActiveSheet.Range("B" & Offset).Value = ""
                        Offset = Offset + 1
                        If CStr(ExcelDoc.ActiveSheet.Range("E" & Offset).Value) = CStr(Description) Then
                            ExcelDoc.ActiveSheet.Range("A" & Offset).Value = ExcelDoc.ActiveSheet.Range("A" & Offset).Value + 1
                            ExcelDoc.ActiveSheet.Range("B" & Offset).Value = ExcelDoc.ActiveSheet.Range("B" & Offset).Value + (Length)
                            'ExcelDoc.ActiveSheet.Range("D" & Offset).Value = ExcelDoc.ActiveSheet.Range("D" & Offset).Value + (Length * Width)
                            If InStrRev(ExcelDoc.ActiveSheet.Range("G" & Offset).Value, PartNo) = "" And
                                ExcelDoc.ActiveSheet.Range("G" & Offset).Value <> "" Then
                                ExcelDoc.ActiveSheet.Range("G" & Offset).Value = ExcelDoc.ActiveSheet.Range("G" & Offset).Value & ", " & PartNo
                            End If
                            Exit Do
                        ElseIf ExcelDoc.ActiveSheet.Range("B" & Offset).Value = Nothing Then
                            ExcelDoc.ActiveSheet.Range("A" & Offset).Value = 1
                            'ExcelDoc.ActiveSheet.Range("B" & Offset).Value = ExcelDoc.ActiveSheet.Range("B" & Offset).Value + (Length)
                            ExcelDoc.ActiveSheet.Range("B" & Offset).Value = "Ea."
                            'ExcelDoc.ActiveSheet.Range("D" & Offset).Value = ExcelDoc.ActiveSheet.Range("D" & Offset).Value + (Length * Width)
                            'ExcelDoc.ActiveSheet.Range("E" & Offset).Value = "Sq Ft."
                            ExcelDoc.ActiveSheet.Range("E" & Offset).Value = Description
                            ExcelDoc.ActiveSheet.Range("C" & Offset).Value = Material
                            ExcelDoc.ActiveSheet.Range("D" & Offset).Value = StockNo
                            ExcelDoc.ActiveSheet.Range("G" & Offset).Value = PartNo
                            ExcelDoc.ActiveSheet.Range("F" & Offset).Value = Vendor
                            Exit Do
                        Else
                            'MsgBox "Jump to next line"
                        End If
                    Loop
                End If
                Offset = 1
                ProgressBar(Total, Counter, Title, Occ.Name)
                If Err.Number = 0 Then
                    Properties = Properties + Length
                End If
                On Error GoTo 0
            Else
                GetProperties(oDoc, Occ.SubOccurrences, Properties, Counter, ExcelDoc, Total)
            End If
        Next
        ExcelDoc.Worksheets("Saw Cut").activate()
    End Sub
    Private Sub btnRename_Click(sender As System.Object, e As System.EventArgs) Handles btnRename.Click
        If lstOpenfiles.CheckedItems.Count = 0 Then
            MsgBox("Please select an item to rename")
            Exit Sub
        ElseIf lstOpenfiles.CheckedItems.Count > 1 Then
            MsgBox("Only select only one item to be renamed")
            Exit Sub
        End If

        CreateOpenDocs(OpenDocs)
        Dim ErrList As String = ""
        Dim Elog As String = ""
        For Each Document In _invApp.Documents
            CloseLater(Strings.Right(Document.FullDocumentName, Len(Document.FullDocumentName) - InStrRev(Document.FullDocumentName, "\")), Document)
        Next
        If VBAFlag = "False" Then CreateVBA()
        Dim Rename As New Rename
        Dim X As Integer = 0
        Rename.PopMain(Me)
        Try
            For Each Entry As List(Of String) In RenameTable
                If X = 0 Then
                    Rename.txtParent.Text = Entry.Item(2).ToString
                    Rename.txtParentSource.Text = Entry.Item(0).ToString
                End If
                Rename.DGVRename.Rows.Add(Entry.Item(0).ToString, Entry.Item(1).ToString, Entry.Item(2).ToString, "", ThumbList(X), Entry.Item(3).ToString)
                Elog = Elog & Entry.Item(2).ToString & ": Added to Rename Table" & vbNewLine
                X += 1
            Next
        Catch
            ErrList = Err.Description & vbNewLine
            Err.Clear()
        Finally
            If ErrList.Length > 0 Then
                MsgBox("The following occurred during the creation of the rename table" & vbNewLine & ErrList)
            End If
            writeDebug(Elog)
        End Try
        Rename.chkCCParts.Checked = False
        Rename.Show()
        'RenameTable.Clear()
    End Sub
    Public Sub ExtractThumb(ByRef PartName As String, ByRef Thumbnail As Image)
        Dim X As Integer = 0
        For Each entry As List(Of String) In RenameTable
            If entry.Item(2).ToString = PartName Then
                Thumbnail = ThumbList(X)
                Exit For
            End If
            X += 1
        Next
    End Sub
    Private Sub ChangeRev()
        Dim Ans As String
        Ans = MsgBox("This will reset all current revisions" _
                     & vbNewLine & "Continue?", MsgBoxStyle.OkCancel, "Change Revision")
        If Ans = vbCancel Then Exit Sub
        Dim Rev_Switch As New Rev_Switch
        Dim RevType As Integer = 0
        Rev_Switch.PopMain(Me)
        Rev_Switch.First(RevType)
    End Sub
    Public Sub ChangeRev(ByRef RevType As Integer)
        Dim oDoc As Document = Nothing
        Dim dDoc As DrawingDocument = Nothing
        Dim Path As Documents = _invApp.Documents
        Dim DrawingName As String
        Dim Archive As String = Nothing
        Dim DrawSource As String = Nothing
        Dim OpenDocs As New ArrayList
        Dim Y, Q As Integer
        Q = 0
        CreateOpenDocs(OpenDocs)
        _invApp.SilentOperation = True
        For Y = 0 To LVSubFiles.Items.Count - 1
            If LVSubFiles.Items(Y).Checked = True Then
                Q += 1
                DrawingName = Trim(LVSubFiles.Items.Item(Y).Text)
                'MatchDrawing(Path, oDoc, Archive, DrawingName, DrawSource, Y)
                'open drawing
                DrawSource = Strings.Left(SubFiles.Item(Y).Value, Len(SubFiles.Item(Y).Value) - 3) & "idw"
                oDoc = _invApp.Documents.Open(DrawSource, True)
                Dim Sheets As Sheets
                Dim Sheet As Sheet
                Dim Rev, RevNo As String
                Dim oPoint(0 To oDoc.sheets.count * 2) As String
                Dim Tx(0 To oDoc.sheets.count) As Decimal
                Dim Ty(0 To oDoc.sheets.count) As Decimal
                Dim Adjust As String = ""
                Dim Point As Point2d
                Dim oRevTable As RevisionTable
                Dim oTitleblock As TitleBlock
                Dim C, R, i As Integer
                MsVistaProgressBar1.Visible = True
                'Check the titleblock to see if it is the IMM TitleBlock, inform user if it is not
                Dim oTitleBlockDef As TitleBlockDefinition
                oTitleBlockDef = oDoc.TitleBlockDefinitions.Item(1)
                'If Strings.InStr(oTitleBlockDef.Name, "IMM") = 0 Then
                '    MsgBox("This operation cannot function on" & vbNewLine &
                '           "non-IMM title blocks")
                '    Exit Sub
                'End If
                'Set the point at which the rev table should be inserted
                ProgressBar(LVSubFiles.CheckedItems.Count, Q, "Changing Rev: ", Strings.Right(oDoc.FullDocumentName, Len(oDoc.FullDocumentName) - InStrRev(oDoc.FullDocumentName, "\")))
                RevNo = oDoc.PropertySets.Item("{F29F85E0-4FF9-1068-AB91-08002B27B3D9}").ItemByPropId("9").Value
                oRevTable = oDoc.activesheet.RevisionTables(1)
                'oRevTable.RevisionTableRows.Add()
                If IsNumeric(RevNo) Then
                    Rev = RevNo
                Else
                    Rev = Asc(RevNo) - Asc("A")
                End If
                Dim X As Integer = 0
                'Iterate through each sheet of the open drawing
                Sheets = oDoc.Sheets
                Sheets.Item(1).Activate()
                Dim S As Integer = 0
                'Delete each rev table in each sheet
                Sheets.Item(1).Activate()
                For Each Sheet In Sheets
                    Sheet.Activate()

                    oRevTable = oDoc.activesheet.RevisionTables(1)
                    Tx(S) = oRevTable.Position.X
                    Ty(S) = oRevTable.Position.Y
                    S += 1
                Next
                S = 0
                For Each Sheet In Sheets
                    If S = 0 Then

                        Sheet.Activate()
                        oRevTable = oDoc.activesheet.RevisionTables(1)
                        oRevTable.RevisionTableRows.Add()

                        For Each oRow As RevisionTableRow In oRevTable.RevisionTableRows
                            If oRow.IsActiveRow Then
                            Else
                                oRow.Delete()
                            End If
                        Next
                        oTitleblock = oDoc.activesheet.TitleBlock
                        If S = 0 Then
                            Adjust = Ty(S) - oRevTable.Position.Y
                        End If

                        oRevTable.Delete()
                        S += 1
                    ElseIf S > 0 Then
                        Sheet.Activate()
                        oRevTable = oDoc.activesheet.RevisionTables(1)
                        oRevTable.Delete()
                    End If
                Next Sheet
                S = 0
                Sheets.Item(1).Activate()
                For Each Sheet In Sheets
                    Sheet.Activate()
                    'Set counter to numeric or alpha based on previous RevTable
                    Point = _invApp.TransientGeometry.CreatePoint2d(Tx(S), Ty(S) - Adjust)

                    If Not IsNumeric(RevNo) Then
                        oRevTable = oDoc.ActiveSheet.RevisionTables.add2(Point, False, True, False, 0)
                    Else
                        oRevTable = oDoc.ActiveSheet.RevisionTables.Add2(Point, False, True, True, "A")
                    End If
                    'Add items corresponding to which table form is being populated
                    S += 1
                Next Sheet
                Sheet = oDoc.activesheet
                Sheet.Activate()
                oRevTable = Sheet.RevisionTables(1)
                'Iterate through the new revision table and insert the standard information for the respective table
                C = oRevTable.RevisionTableColumns.Count
                R = oRevTable.RevisionTableRows.Count
                Dim Contents(0 To C * R) As String
                Dim Rtr As RevisionTableRow
                Dim RTCell As RevisionTableCell
                i = 1
                For Each Rtr In oRevTable.RevisionTableRows
                    For Each RTCell In Rtr
                        If i = 3 And RevType = 1 Then
                            RTCell.Text = "ISSUED FOR CONSTRUCTION"
                            Exit For
                        ElseIf i = 3 And RevType = 0 Then
                            RTCell.Text = "ISSUED FOR REVIEW"
                            Exit For
                        End If
                        i += 1
                    Next
                Next
                oDoc.sheets.item(1).activate()
                _invApp.SilentOperation = True
                Try
                    oDoc.Save()
                Catch
                End Try
                CloseLater(DrawingName, oDoc)
                _invApp.SilentOperation = False
            End If
        Next
        _invApp.SilentOperation = False
        MsVistaProgressBar1.Visible = False
    End Sub
    Private Sub RRev()
        Dim OpenDocs As New ArrayList
        CreateOpenDocs(OpenDocs)
        RevTable.PopMain(Me)
        Dim oDoc As Document
        Dim Path As Documents
        'Dim oRevTable As RevisionTable
        'Dim Point As Point2d
        Path = _invApp.Documents
        'Dim Loc As Drawing.Point
        'Dim Sheets As Sheets
        'Dim Sheet As Sheet
        Dim X As Integer = 0
        'Dim oTitleblock As TitleBlock
        Dim DwgName, strPath, strFile, RevNo As String
        RevNo = ""
        DwgName = ""
        RevTable.btnIgnore.Visible = False
        chkiProp.CheckState = CheckState.Unchecked
        'iterate through the files in subfiles
        For X = 0 To LVSubFiles.Items.Count - 1
            'select the files that have been checked
            If LVSubFiles.Items(X).Checked = True Then
                'save the name of the checked file
                DwgName = LVSubFiles.Items.Item(X).Text
                Exit For
            End If
        Next
        'Iterate through all the open documents in inventor
        For j = 1 To Path.Count
            oDoc = Path.Item(j)

            'Parse the path from the document
            strPath = Strings.Left(oDoc.FullDocumentName, Strings.Len(oDoc.FullDocumentName) - 3) & "idw"
            'Parse out the filename from the document
            strFile = Strings.Right(strPath, Strings.Len(strPath) - InStrRev(strPath, "\"))
            'Compare the selected file to the current document, if they are the same proceed
            If Trim(DwgName) = strFile Then
                'Modify the revtable layout to give instruction for removing revisions
                'Open the selected document in the background
                oDoc = _invApp.Documents.Open(strPath, False)
                'Go through each sheet to check for revisiontable
                'Do Until X > 1
                'Dim S As Integer = 0
                'Sheets = oDoc.Sheets
                'Dim oPoint(0 To oDoc.sheets.count * 2) As String
                'For Each Sheet In Sheets
                ' 'For each sheet, on the first cycle, delete the table, and create a new table on the second round
                ' If X <> 1 Then
                ' Sheet.Activate()
                ' On Error Resume Next
                ' oRevTable = oDoc.activesheet.RevisionTables(1)
                ' oTitleblock = oDoc.activesheet.TitleBlock
                ' oPoint(S) = oRevTable.Position.X
                ' oPoint(S + 1) = oRevTable.RangeBox.MinPoint.Y + 1.14359999263287
                ' oRevTable.Delete()
                ' S += 2
                'Else
                '   Sheet.Activate()
                '  Point = _invApp.TransientGeometry.CreatePoint2d(oPoint(S), oPoint(S + 1))
                ' oRevTable = oDoc.ActiveSheet.Revisiontables.Add2(Point, False, True, False, 0)
                'S += 2
                'End If
                'Next
                'X += 1
                'Loop
                Call RevTable.PopulateRevTable(oDoc, RevNo, 1, False)
                Exit For
            End If
        Next
        'Show the userform
        RevTable.Show()
    End Sub
    Private Sub btnRef_Click(sender As System.Object, e As System.EventArgs) Handles btnRef.Click
        Dim Written As Boolean = False
        Dim Warning As Boolean = False
        Dim Warning2 As Boolean = False
        Dim PropExists As Boolean = True
        Dim Fix As String = "The following files have more references" & vbNewLine &
                            "than the titleblock permits: " & vbNewLine & vbNewLine
        Dim C, X, Y, R, P, Maxlayer, Layer, Col As Integer
        Col = R = P = 0
        Layer = 0
        'Dim Parent As New ArrayList
        Dim Parent As New List(Of List(Of String))
        Dim Total As Integer = LVSubFiles.CheckedItems.Count
        Dim Counter As Integer = 1
        Parent.Clear()
        Dim oDoc As Document
        Dim OpenDocs As New ArrayList
        Dim AsmDoc As AssemblyDocument
        Dim DocType As DocumentTypeEnum
        Dim Ans, strFile, PartName, Archive, DrawSource As String
        Dim DrawingName As String = ""
        Dim test, ChildFile, ParentFile As String
        ChildFile = ""
        Dim Path As Documents
        Path = _invApp.Documents
        'Go through drawings to see which ones are selected
        For X = 0 To lstOpenfiles.Items.Count - 1
            'Look through all sub files in open documents to get the part sourcefile
            If lstOpenfiles.GetItemCheckState(X) = CheckState.Checked Then
                'iterate through opend documents to find the selected file
                If Strings.Right(lstOpenfiles.Items.Item(X), 3) = "iam" Then
                    C += 1
                End If
            End If
        Next
        If C = 0 Then
            MsgBox("Please select an assembly")
            Exit Sub
        End If
        If C > 1 Then
            MsgBox("Please only select one assembly")
            Exit Sub
        End If
        'Go through drawings to see which ones are selected
        For X = 0 To lstOpenfiles.Items.Count - 1
            'Look through all sub files in open documents to get the part sourcefile
            If lstOpenfiles.GetItemCheckState(X) = CheckState.Checked Then
                'iterate through opend documents to find the selected file
                For Y = 1 To _invApp.Documents.Count
                    'If selected document = Found document
                    If InStr(_invApp.Documents.Item(Y).FullFileName, lstOpenfiles.Items.Item(X)) <> 0 Then
                        oDoc = _invApp.Documents.Open(_invApp.Documents.Item(Y).FullFileName)
                        Ans = MsgBox("This action will overwrite" & vbNewLine & "all previous references." _
                                     & vbNewLine & "Do you wish to continue?", vbYesNo, "Overwrite References")
                        If Ans = vbNo Then
                            Exit Sub
                        Else
                            ' activate matching document
                            AsmDoc = _invApp.ActiveDocument
                            DocType = AsmDoc.DocumentType
                            strFile = Strings.Left(AsmDoc.FullDocumentName, Strings.Len(AsmDoc.FullDocumentName) - 4)
                            strFile = Strings.Right(strFile, Strings.Len(strFile) - InStrRev(strFile, "\"))
                            'Add list to the array for new Parent
                            Parent.Add(New List(Of String))
                            Parent(Layer).Add(strFile)
                            'call recursive sub to go through all sub files
                            WriteChildren(AsmDoc.ComponentDefinition.Occurrences, Layer + 1, R, Col, Maxlayer, Parent, strFile, Total, Counter)
                            Exit For
                        End If
                    End If
                Next
            End If
        Next
        Counter = 1
        For X = 0 To LVSubFiles.Items.Count - 1
            Counter += 1
            For Y = 1 To _invApp.Documents.Count - 1
                oDoc = Path.Item(Y)
                Archive = oDoc.FullDocumentName
                'Use the Partsource file to create the drawingsource file
                DrawSource = Strings.Left(Archive, Strings.Len(Archive) - 3) & "idw"
                If My.Computer.FileSystem.FileExists(DrawSource) Then
                    DrawingName = Strings.Right(DrawSource, Strings.Len(DrawSource) - Strings.InStrRev(DrawSource, "\"))
                    'If the drawing file is listed, open the drawing in Inventor
                    If Trim(LVSubFiles.Items.Item(X).Text) = DrawingName Then
                        oDoc = _invApp.Documents.Open(DrawSource, False)
                        CustomPropSet = oDoc.PropertySets.Item("{D5CDD505-2E9C-101B-9397-08002B2CF9AE}")
                        PartName = Strings.Right(oDoc.FullDocumentName, Len(oDoc.FullDocumentName) - InStrRev(oDoc.FullDocumentName, "\"))


                        For P = 0 To 9
                            Try
                                InvRef(P) = CustomPropSet.Item("Reference" & P)
                            Catch
                                SetiProp(InvRef(P), "Reference" & P)
                                InvRef(P) = CustomPropSet.Item("Reference" & P)
                            End Try
                        Next
                        For P = 0 To 9
                            InvRef(P).Value = ""
                        Next
                        P = 0
                        Dim Fullstring As New List(Of String)
                        For Each l As List(Of String) In Parent
                            For M = 0 To l.Count - 1
                                test = l(M)
                                If InStr(test, ",") = 0 Then
                                    ParentFile = test
                                    ChildFile = ""
                                Else
                                    ChildFile = Strings.Right(test, Len(test) - (InStrRev(test, ",")))
                                    ParentFile = Strings.Left(test, (InStr(test, ",") - 1))
                                End If
                                If (Strings.Left(PartName, Len(PartName) - 4) = ChildFile) And ChildFile <> "" Then
                                    If P = 7 Then
                                        Warning2 = True
                                        If InStr(Fix, ChildFile) = 0 Then
                                            Fix = Fix & ChildFile & vbNewLine
                                        End If
                                    ElseIf P = 6 Then
                                        MsgBox("The current titleblock holds 6 references." & vbNewLine &
                                            "All following references will be added, but the Titleblock needs to be updated before they can be shown.")
                                    ElseIf P = 10 Then
                                        MsgBox("Currently the program can only keep 10 references." & vbNewLine &
                                            "All following references will not be added.")
                                    End If
                                    Try
                                        InvRef(P) = CustomPropSet.Item("Reference" & P)
                                        InvRef(P).Value = ParentFile
                                    Catch
                                        InvRef(P) = CustomPropSet.Add("", "Reference" & P)
                                        InvRef(P).Value = ParentFile
                                    End Try
                                    P += 1
                                    Written = True
                                End If
                            Next
                        Next
                        _invApp.SilentOperation = True
                        oDoc.Update()
                        Try
                            oDoc.Save2()
                        Catch ex As Exception
                            MsgBox(ex.Message)
                        End Try
                        CloseLater(DrawingName, oDoc)
                        _invApp.SilentOperation = False
                    End If
                End If
                'MsVistaProgressBar1.ProgressBarStyle = MSVistaProgressBar.BarStyle.Marquee
                ProgressBar(Total, Counter, "Writing Reference: ", DrawingName)
                If Written = True Then
                    Written = False
                    Exit For
                End If
            Next
        Next
        MsVistaProgressBar1.Visible = False
        If Warning2 = True Then
            MsgBox(Fix & vbNewLine & "These drawings need to be updated manually.")
        End If

        MsgBox("All the references have been updated")
    End Sub
    Private Sub SetiProp(InvRef As Inventor.Property, Ref As String)
        InvRef = CustomPropSet.Add("", Ref)
    End Sub
    Private Sub WriteChildren(Occurrences As ComponentOccurrences, Layer As Integer,
                          ByRef R As Integer, ByRef Col As Integer, ByRef Maxlayer As Integer,
                          Parent As List(Of List(Of String)), ParentName As String, ByRef Total As Integer, ByRef Counter As Integer)
        Dim Occ As ComponentOccurrence
        Dim Title As String = "Getting References: "
        'Dim oDoc As Document
        'Dim ColVal As Integer
        'Dim Dup, Fresh As Boolean
        Dim Match As Boolean
        Dim PartName As String
        'Parent(Layer, Col) = Occ.Name
        ParentName = ParentName
        'go through each sub-file
        For Each Occ In Occurrences
            If Occ.Name <> "" Then
                'Parse out part name from the occurance name
                If InStr(Occ.Name, ":") <> 0 Then
                    PartName = Strings.Left(Occ.Name, InStrRev(Occ.Name, ":") - 1)
                Else
                    PartName = Occ.Name
                End If
                'Match the case of the name to IMM Standard
                Match = (PartName Like "?###[-.]#####") Or (PartName Like "?##[-.]#####") Or (PartName Like "?###[-.]#####[-.]##") Or (PartName Like "?##[-.]#####[-.]##")
                'If Match = True Then
                Col += 1
                ProgressBar(Total, Counter, Title, Col)
                'For Parents with more children, add another list to the current list
                If Layer > Maxlayer Then
                    Maxlayer = Layer
                    Parent.Add(New List(Of String))
                End If
                'search for repetitions in the current list
                If Parent(Layer).Contains(ParentName & "," & PartName) Then
                Else
                    'add the new part to the current list



                    Parent(Layer).Add(ParentName & "," & PartName)
                End If
                'End If
                If Occ.DefinitionDocumentType = DocumentTypeEnum.kAssemblyDocumentObject Then
                    'Go through for each assembly found
                    Counter += 1
                    WriteChildren(Occ.SubOccurrences, Layer + 1, R, Col, Maxlayer, Parent, PartName, Total, Counter)
                End If
            End If
        Next
    End Sub
    Private Sub chkDXF_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkDXF.CheckedChanged
        If chkDWG.Checked = True Then Exit Sub
        If chkDXF.CheckState = CheckState.Checked Then
            chkUseDrawings.CheckState = CheckState.Unchecked
            chkUseDrawings.Visible = True
            chkSkipAssy.CheckState = CheckState.Checked
            chkSkipAssy.Visible = True
            chkClose.Top = chkClose.Top + 30
            GroupBox4.Height = GroupBox4.Size.Height.ToString + 30
        Else
            chkUseDrawings.CheckState = CheckState.Checked
            chkUseDrawings.Visible = False
            chkSkipAssy.Visible = False
            chkClose.Top = chkClose.Top - 30
            GroupBox4.Height = GroupBox4.Size.Height.ToString - 30
        End If
    End Sub
    Private Sub ToolTip1_Popup(sender As System.Object, e As System.Windows.Forms.PopupEventArgs) Handles ToolTip1.Popup
        Dim ctrl As Control = GetChildAtPoint(Me.PointToClient(MousePosition))
        Dim tt As String = ToolTip1.GetToolTip(ctrl)
        If Not tt = Nothing Then
            If tt.Length > 75 Then ToolTip1.SetToolTip(ctrl, SplitToolTip(tt))
        End If
    End Sub
    Friend Function SplitToolTip(ByVal strOrig As String) As String
        Dim strArray As String()
        Dim SPACE As String = " "
        Dim strOneWord As String
        Dim strBuilder As String = ""
        Dim strReturn As String = ""
        strArray = strOrig.Split(SPACE)
        For Each strOneWord In strArray
            strBuilder = strOneWord & SPACE
            If Len(strBuilder) > 70 Then
                strReturn = strReturn & strBuilder & vbNewLine &
                strBuilder = ""
            End If
        Next
        Return strReturn & strBuilder
    End Function
    Private Sub txtSearch_Click(sender As Object, e As System.EventArgs) Handles txtSearch.Click
        If txtSearch.ForeColor = Drawing.Color.Gray Then
            txtSearch.ForeColor = Drawing.Color.Black
            txtSearch.Text = ""
        End If
    End Sub
    Private Sub txtSearch_LostFocus(sender As Object, e As EventArgs) Handles txtSearch.LostFocus
        If txtSearch.Text = "" Then
            txtSearch.ForeColor = Drawing.Color.Gray
            txtSearch.Text = "Search"
        End If
    End Sub
    Private Sub txtSearch_TextChanged(sender As Object, e As System.EventArgs) Handles txtSearch.TextChanged
        If txtSearch.ForeColor = Drawing.Color.Gray Then Exit Sub
        LVSubFiles.Items.Clear()

        For Each item As KeyValuePair(Of String, String) In SubFiles
            If InStr(UCase(item.Key), UCase(txtSearch.Text)) <> 0 Then
                LVSubFiles.Items.Add(item.Key)
            End If
        Next
    End Sub
    Private Sub chkExport_CheckedChanged(sender As Object, e As EventArgs) Handles chkExport.CheckedChanged
        If chkExport.CheckState = CheckState.Checked Then
            chkDWG.Visible = True
            chkPDF.Visible = True
            chkDXF.Visible = True
            chkClose.Top = chkClose.Top + 15
            GroupBox4.Height = GroupBox4.Height + 15
            chkPDF.CheckState = CheckState.Checked
            chkDWG.CheckState = CheckState.Unchecked
            chkDXF.CheckState = CheckState.Unchecked
        Else
            chkDWG.Visible = False
            chkPDF.Visible = False
            chkDXF.Visible = False
            chkDXF.CheckState = CheckState.Unchecked
            chkDWG.CheckState = CheckState.Unchecked
            chkPDF.CheckState = CheckState.Unchecked
            chkClose.Top = chkClose.Top - 15
            GroupBox4.Height = GroupBox4.Height - 15
        End If
    End Sub

    Private Sub chkDWG_CheckedChanged(sender As Object, e As EventArgs) Handles chkDWG.CheckedChanged
        If chkDXF.CheckState = CheckState.Checked Then Exit Sub
        If chkDWG.CheckState = CheckState.Checked Then
            chkUseDrawings.CheckState = CheckState.Unchecked
            chkUseDrawings.Visible = True
            chkSkipAssy.CheckState = CheckState.Checked
            chkSkipAssy.Visible = True
            chkClose.Top = chkClose.Top + 30
            GroupBox4.Height = GroupBox4.Size.Height.ToString + 30
        Else
            chkUseDrawings.CheckState = CheckState.Checked
            chkUseDrawings.Visible = False
            chkSkipAssy.Visible = False
            chkClose.Top = chkClose.Top - 30
            GroupBox4.Height = GroupBox4.Size.Height.ToString - 30
        End If
    End Sub
    Private Sub lstOpenfiles_MouseUp(sender As Object, e As MouseEventArgs) Handles lstOpenfiles.MouseUp
        If e.Button = Windows.Forms.MouseButtons.Right Then
            CMSOpenFiles.Show(Cursor.Position)
        End If
    End Sub
    Private Sub LVSubFiles_MouseUp(sender As Object, e As MouseEventArgs) Handles LVSubFiles.MouseUp
        If e.Button = Windows.Forms.MouseButtons.Right Then
            CMSSubFiles.Show(Cursor.Position)
        End If
    End Sub
    Private Sub CMSAlphabetical_Click(sender As Object, e As EventArgs) Handles CMSAlphabetical.Click
        LVSubFiles.Items.Clear()

        For Each pair As KeyValuePair(Of String, String) In AlphaSub

            If CMSShow.Checked = True Then
                If InStr(pair.Key, "(DNE)") <> 0 Then
                    LVSubFiles.Items.Add(Strings.Replace(pair.Key, "(DNE)", ""))
                    LVSubFiles.Items(LVSubFiles.Items.Count - 1).ForeColor = Drawing.Color.Gray
                Else
                    LVSubFiles.Items.Add(Strings.Trim(pair.Key))
                    LVSubFiles.Items(LVSubFiles.Items.Count - 1).Checked = True
                End If
            Else
                If InStr(pair.Key, "(DNE)") = 0 Then
                    LVSubFiles.Items.Add(Strings.Trim(pair.Key))
                    LVSubFiles.Items(LVSubFiles.Items.Count - 1).Checked = True
                End If
            End If
        Next
        CMSAlphabetical.Checked = True
        CMSHeirarchical.Checked = False
    End Sub
    Private Sub CMSHeirarchical_Click(sender As Object, e As EventArgs) Handles CMSHeirarchical.Click
        LVSubFiles.Items.Clear()
        For Each pair As KeyValuePair(Of String, String) In SubFiles

            If CMSShow.Checked = True Then
                If InStr(pair.Key, "(DNE)") <> 0 Then
                    LVSubFiles.Items.Add(Strings.Replace(pair.Key, "(DNE)", ""))
                    LVSubFiles.Items(LVSubFiles.Items.Count - 1).ForeColor = Drawing.Color.Gray
                Else
                    LVSubFiles.Items.Add(pair.Key)
                    LVSubFiles.Items(LVSubFiles.Items.Count - 1).Checked = True
                End If
            Else
                If InStr(pair.Key, "(DNE)") = 0 Then
                    LVSubFiles.Items.Add(pair.Key)
                    LVSubFiles.Items(LVSubFiles.Items.Count - 1).Checked = True
                End If
            End If
        Next
        CMSAlphabetical.Checked = False
        CMSHeirarchical.Checked = True
    End Sub

    Private Sub CMSSpreadsheet_Click(sender As Object, e As EventArgs) Handles CMSSpreadsheet.Click
        ExportText(CMSHeirarchical.Checked, CMSShow.Checked, False, True)
    End Sub
    Private Sub ExportList(ByRef oDoc As Inventor.Document, ByVal Occurrences As ComponentOccurrences,
                           ByRef Properties As Double, ByRef Counter As Integer, ExcelDoc As Excel.Workbook, Total As Integer)
        Dim Occ As ComponentOccurrence
        Dim PropSets As Inventor.PropertySets
        Dim StockNo, Material, PartNo, Description, Mass, Title As String
        Dim Offset As Integer = 1
        Title = "Extracting"
        For Each Occ In Occurrences
            Counter += 1
            'Check if the document is a part or assembly
            If Occ.DefinitionDocumentType = DocumentTypeEnum.kPartDocumentObject Or
                DocumentTypeEnum.kAssemblyDocumentObject And Occ.BOMStructure = BOMStructureEnum.kPurchasedBOMStructure Then
                'Get iProperties for this document
                PropSets = Occ.Definition.Document.PropertySets()
                StockNo = PropSets.Item("{32853F0F-3444-11D1-9E93-0060B03C1CA6}").ItemByPropId("55").Value.ToString
                Material = PropSets.Item("{32853F0F-3444-11D1-9E93-0060B03C1CA6}").ItemByPropId("20").Value.ToString
                PartNo = PropSets.Item("{32853F0F-3444-11D1-9E93-0060B03C1CA6}").ItemByPropId("5").Value.ToString
                Description = PropSets.Item("{32853F0F-3444-11D1-9E93-0060B03C1CA6}").ItemByPropId("29").Value.ToString
                Mass = Math.Round(Occ.MassProperties.Mass, 2)
                ExcelDoc.ActiveSheet.Range("A" & Offset).Value = StockNo
                ExcelDoc.ActiveSheet.Range("B" & Offset).Value = Material
                ExcelDoc.ActiveSheet.Range("C" & Offset).Value = PartNo
                ExcelDoc.ActiveSheet.Range("D" & Offset).Value = Description
                ExcelDoc.ActiveSheet.Range("E" & Offset).Value = Mass
                ProgressBar(Total, Counter, Title, Occ.Name)
                Offset += 1
            Else
                ExportList(oDoc, Occ.SubOccurrences, Properties, Counter, ExcelDoc, Total)
            End If
        Next
    End Sub

    Private Sub CMSTextFile_Click(sender As Object, e As EventArgs) Handles CMSTextFile.Click
        ExportText(CMSHeirarchical.Checked, CMSHide.Checked, True, True)
    End Sub

    Private Sub DefaultSettingsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DefaultSettingsToolStripMenuItem.Click
        Settings.ShowDialog()
    End Sub

    Private Sub HideMissingDrawingsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CMSHide.Click
        LVSubFiles.Items.Clear()

        If CMSHeirarchical.Checked = True Then
            For Each pair As KeyValuePair(Of String, String) In SubFiles
                If InStr(pair.Key, "(DNE)") = 0 Then
                    LVSubFiles.Items.Add(pair.Key)
                End If
            Next
            For X = 0 To LVSubFiles.Items.Count - 1
                LVSubFiles.Items(X).Checked = True
            Next
        Else
            For Each pair As KeyValuePair(Of String, String) In AlphaSub
                If InStr(pair.Key, "(DNE)") = 0 Then
                    LVSubFiles.Items.Add(Strings.Trim(pair.Key))
                End If
            Next
            For X = 0 To LVSubFiles.Items.Count - 1
                LVSubFiles.Items(X).Checked = True
            Next
        End If
        CMSShow.Checked = False
        CMSHide.Checked = True

    End Sub

    Private Sub ShowMissingDrawingsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CMSShow.Click
        LVSubFiles.Items.Clear()

        If CMSHeirarchical.Checked = True Then
            For Each pair As KeyValuePair(Of String, String) In SubFiles
                If Strings.InStr(pair.Key, "(DNE)") <> 0 Then
                    LVSubFiles.Items.Add(Strings.Replace(pair.Key, "(DNE)", ""))
                    LVSubFiles.Items(LVSubFiles.Items.Count - 1).Checked = False
                    LVSubFiles.Items(LVSubFiles.Items.Count - 1).ForeColor = Drawing.Color.Gray
                Else
                    LVSubFiles.Items.Add(pair.Key)
                    LVSubFiles.Items(LVSubFiles.Items.Count - 1).Checked = True
                End If
            Next
        Else
            For Each pair As KeyValuePair(Of String, String) In AlphaSub
                If Strings.InStr(pair.Key, "(DNE)") <> 0 Then
                    LVSubFiles.Items.Add(Strings.Replace(Strings.Trim(pair.Key), "(DNE)", ""))
                    LVSubFiles.Items(LVSubFiles.Items.Count - 1).Checked = False
                    LVSubFiles.Items(LVSubFiles.Items.Count - 1).ForeColor = Drawing.Color.Gray
                Else
                    LVSubFiles.Items.Add(Strings.Trim(pair.Key))
                    LVSubFiles.Items(LVSubFiles.Items.Count - 1).Checked = True
                End If
            Next
        End If
        CMSShow.Checked = True
        CMSHide.Checked = False
    End Sub
    Private Sub ExportText(Heirarchical As Boolean, Hidden As Boolean, Text As Boolean, OpenFiles As Boolean)
        MsgBox("Heirarchical: " & Heirarchical & vbNewLine &
               "Show: " & Hidden & vbNewLine &
               "Text: " & Text & vbNewLine &
               "Openfiles: " & OpenFiles)
    End Sub

    Private Sub CMSSubSpreadsheet_Click(sender As Object, e As EventArgs) Handles CMSSubSpreadsheet.Click
        ExportText(CMSHeirarchical.Checked, CMSShow.Checked, False, False)
    End Sub

    Private Sub CMSSubText_Click(sender As Object, e As EventArgs) Handles CMSSubText.Click
        ExportText(CMSHeirarchical.Checked, CMSShow.Checked, True, False)
    End Sub

    Private Sub LVSubFiles_Click(sender As Object, e As EventArgs) Handles LVSubFiles.Click
        If LVSubFiles.Items(LVSubFiles.FocusedItem.Index).ForeColor <> Drawing.Color.Gray Then
            If LVSubFiles.Items(LVSubFiles.FocusedItem.Index).Checked = True Then
                LVSubFiles.Items(LVSubFiles.FocusedItem.Index).Checked = False
            Else
                LVSubFiles.Items(LVSubFiles.FocusedItem.Index).Checked = True
            End If
        End If
    End Sub

    Private Sub AboutBatchProgramToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutBatchProgramToolStripMenuItem.Click
        About.ShowDialog()
    End Sub

    Private Sub IPropertySettingsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles IPropertySettingsToolStripMenuItem.Click
        iPropertySettings.ShowDialog()
    End Sub

    Private Sub LVSubFiles_ItemChecked(sender As Object, e As ItemCheckedEventArgs) Handles LVSubFiles.ItemChecked
        If LVSubFiles.Items(e.Item.Index).ForeColor = Drawing.Color.Gray Then
            e.Item.Checked = False
        End If
        LVSubFiles.FocusedItem = Nothing
    End Sub

    Private Sub LVSubFiles_ItemActivate(sender As Object, e As EventArgs) Handles LVSubFiles.ItemActivate

    End Sub
End Class