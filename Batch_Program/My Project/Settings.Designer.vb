﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Namespace My
    
    <Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),  _
     Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0"),  _
     Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
    Partial Friend NotInheritable Class MySettings
        Inherits Global.System.Configuration.ApplicationSettingsBase
        
        Private Shared defaultInstance As MySettings = CType(Global.System.Configuration.ApplicationSettingsBase.Synchronized(New MySettings()),MySettings)
        
#Region "My.Settings Auto-Save Functionality"
#If _MyType = "WindowsForms" Then
    Private Shared addedHandler As Boolean

    Private Shared addedHandlerLockObject As New Object

    <Global.System.Diagnostics.DebuggerNonUserCodeAttribute(), Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)> _
    Private Shared Sub AutoSaveSettings(ByVal sender As Global.System.Object, ByVal e As Global.System.EventArgs)
        If My.Application.SaveMySettingsOnExit Then
            My.Settings.Save()
        End If
    End Sub
#End If
#End Region
        
        Public Shared ReadOnly Property [Default]() As MySettings
            Get
                
#If _MyType = "WindowsForms" Then
               If Not addedHandler Then
                    SyncLock addedHandlerLockObject
                        If Not addedHandler Then
                            AddHandler My.Application.Shutdown, AddressOf AutoSaveSettings
                            addedHandler = True
                        End If
                    End SyncLock
                End If
#End If
                Return defaultInstance
            End Get
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property PDFSaveLoc() As String
            Get
                Return CType(Me("PDFSaveLoc"),String)
            End Get
            Set
                Me("PDFSaveLoc") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property DXFSaveLoc() As String
            Get
                Return CType(Me("DXFSaveLoc"),String)
            End Get
            Set
                Me("DXFSaveLoc") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property DWGSaveLoc() As String
            Get
                Return CType(Me("DWGSaveLoc"),String)
            End Get
            Set
                Me("DWGSaveLoc") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property PDFTag() As String
            Get
                Return CType(Me("PDFTag"),String)
            End Get
            Set
                Me("PDFTag") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property DXFTag() As String
            Get
                Return CType(Me("DXFTag"),String)
            End Get
            Set
                Me("DXFTag") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("")>  _
        Public Property DWGTag() As String
            Get
                Return CType(Me("DWGTag"),String)
            End Get
            Set
                Me("DWGTag") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property PDFSaveNewLoc() As Boolean
            Get
                Return CType(Me("PDFSaveNewLoc"),Boolean)
            End Get
            Set
                Me("PDFSaveNewLoc") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property PDFSaveTag() As Boolean
            Get
                Return CType(Me("PDFSaveTag"),Boolean)
            End Get
            Set
                Me("PDFSaveTag") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property DXFSaveNewLoc() As Boolean
            Get
                Return CType(Me("DXFSaveNewLoc"),Boolean)
            End Get
            Set
                Me("DXFSaveNewLoc") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property DXFSaveTag() As Boolean
            Get
                Return CType(Me("DXFSaveTag"),Boolean)
            End Get
            Set
                Me("DXFSaveTag") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property DWGSaveNewLoc() As Boolean
            Get
                Return CType(Me("DWGSaveNewLoc"),Boolean)
            End Get
            Set
                Me("DWGSaveNewLoc") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property DWGSaveTag() As Boolean
            Get
                Return CType(Me("DWGSaveTag"),Boolean)
            End Get
            Set
                Me("DWGSaveTag") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
        Public Property PDFRev() As Boolean
            Get
                Return CType(Me("PDFRev"),Boolean)
            End Get
            Set
                Me("PDFRev") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
        Public Property DXFRev() As Boolean
            Get
                Return CType(Me("DXFRev"),Boolean)
            End Get
            Set
                Me("DXFRev") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
        Public Property DWGRev() As Boolean
            Get
                Return CType(Me("DWGRev"),Boolean)
            End Get
            Set
                Me("DWGRev") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Model")>  _
        Public Property DupName() As String
            Get
                Return CType(Me("DupName"),String)
            End Get
            Set
                Me("DupName") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Model")>  _
        Public Property Title() As String
            Get
                Return CType(Me("Title"),String)
            End Get
            Set
                Me("Title") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Model")>  _
        Public Property Subject() As String
            Get
                Return CType(Me("Subject"),String)
            End Get
            Set
                Me("Subject") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Model")>  _
        Public Property Author() As String
            Get
                Return CType(Me("Author"),String)
            End Get
            Set
                Me("Author") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Model")>  _
        Public Property Manager() As String
            Get
                Return CType(Me("Manager"),String)
            End Get
            Set
                Me("Manager") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Model")>  _
        Public Property Company() As String
            Get
                Return CType(Me("Company"),String)
            End Get
            Set
                Me("Company") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Model")>  _
        Public Property Category() As String
            Get
                Return CType(Me("Category"),String)
            End Get
            Set
                Me("Category") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Model")>  _
        Public Property Keywords() As String
            Get
                Return CType(Me("Keywords"),String)
            End Get
            Set
                Me("Keywords") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Model")>  _
        Public Property Comments() As String
            Get
                Return CType(Me("Comments"),String)
            End Get
            Set
                Me("Comments") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Model")>  _
        Public Property Location() As String
            Get
                Return CType(Me("Location"),String)
            End Get
            Set
                Me("Location") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Model")>  _
        Public Property Subtype() As String
            Get
                Return CType(Me("Subtype"),String)
            End Get
            Set
                Me("Subtype") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Model")>  _
        Public Property PPartNumber() As String
            Get
                Return CType(Me("PPartNumber"),String)
            End Get
            Set
                Me("PPartNumber") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Model")>  _
        Public Property PStockNumber() As String
            Get
                Return CType(Me("PStockNumber"),String)
            End Get
            Set
                Me("PStockNumber") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Model")>  _
        Public Property Description() As String
            Get
                Return CType(Me("Description"),String)
            End Get
            Set
                Me("Description") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Drawing")>  _
        Public Property Revision() As String
            Get
                Return CType(Me("Revision"),String)
            End Get
            Set
                Me("Revision") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Model")>  _
        Public Property Project() As String
            Get
                Return CType(Me("Project"),String)
            End Get
            Set
                Me("Project") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Model")>  _
        Public Property Designer() As String
            Get
                Return CType(Me("Designer"),String)
            End Get
            Set
                Me("Designer") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Model")>  _
        Public Property Engineer() As String
            Get
                Return CType(Me("Engineer"),String)
            End Get
            Set
                Me("Engineer") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Model")>  _
        Public Property Vendor() As String
            Get
                Return CType(Me("Vendor"),String)
            End Get
            Set
                Me("Vendor") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Model")>  _
        Public Property SPartNumber() As String
            Get
                Return CType(Me("SPartNumber"),String)
            End Get
            Set
                Me("SPartNumber") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Model")>  _
        Public Property SStockNumber() As String
            Get
                Return CType(Me("SStockNumber"),String)
            End Get
            Set
                Me("SStockNumber") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Drawing")>  _
        Public Property Status() As String
            Get
                Return CType(Me("Status"),String)
            End Get
            Set
                Me("Status") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Model")>  _
        Public Property ModelDate() As String
            Get
                Return CType(Me("ModelDate"),String)
            End Get
            Set
                Me("ModelDate") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Drawing")>  _
        Public Property DrawingDate() As String
            Get
                Return CType(Me("DrawingDate"),String)
            End Get
            Set
                Me("DrawingDate") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Drawing")>  _
        Public Property DrawingBy() As String
            Get
                Return CType(Me("DrawingBy"),String)
            End Get
            Set
                Me("DrawingBy") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Drawing")>  _
        Public Property DesignState() As String
            Get
                Return CType(Me("DesignState"),String)
            End Get
            Set
                Me("DesignState") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Drawing")>  _
        Public Property CheckedBy() As String
            Get
                Return CType(Me("CheckedBy"),String)
            End Get
            Set
                Me("CheckedBy") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Drawing")>  _
        Public Property CheckedDate() As String
            Get
                Return CType(Me("CheckedDate"),String)
            End Get
            Set
                Me("CheckedDate") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Drawing")>  _
        Public Property Custom() As String
            Get
                Return CType(Me("Custom"),String)
            End Get
            Set
                Me("Custom") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
        Public Property RenameShowMe() As Boolean
            Get
                Return CType(Me("RenameShowMe"),Boolean)
            End Get
            Set
                Me("RenameShowMe") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
        Public Property DonateShowMe() As Boolean
            Get
                Return CType(Me("DonateShowMe"),Boolean)
            End Get
            Set
                Me("DonateShowMe") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("False")>  _
        Public Property Donated() As Boolean
            Get
                Return CType(Me("Donated"),Boolean)
            End Get
            Set
                Me("Donated") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("0")>  _
        Public Property DonateCount() As Integer
            Get
                Return CType(Me("DonateCount"),Integer)
            End Get
            Set
                Me("DonateCount") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
        Public Property FirstRun() As Boolean
            Get
                Return CType(Me("FirstRun"),Boolean)
            End Get
            Set
                Me("FirstRun") = value
            End Set
        End Property
    End Class
End Namespace

Namespace My
    
    <Global.Microsoft.VisualBasic.HideModuleNameAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>  _
    Friend Module MySettingsProperty
        
        <Global.System.ComponentModel.Design.HelpKeywordAttribute("My.Settings")>  _
        Friend ReadOnly Property Settings() As Global.My.MySettings
            Get
                Return Global.My.MySettings.Default
            End Get
        End Property
    End Module
End Namespace
