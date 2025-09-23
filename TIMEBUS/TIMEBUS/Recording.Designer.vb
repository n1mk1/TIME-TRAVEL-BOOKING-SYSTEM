<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Recording
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Recording))
        mediaPlayer = New AxWMPLib.AxWindowsMediaPlayer()
        CType(mediaPlayer, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' mediaPlayer
        ' 
        mediaPlayer.Enabled = True
        mediaPlayer.Location = New Point(257, 80)
        mediaPlayer.Name = "mediaPlayer"
        mediaPlayer.OcxState = CType(resources.GetObject("mediaPlayer.OcxState"), AxHost.State)
        mediaPlayer.Size = New Size(250, 250)
        mediaPlayer.TabIndex = 0
        ' 
        ' Recording
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(800, 450)
        Controls.Add(mediaPlayer)
        Name = "Recording"
        Text = "Recording"
        CType(mediaPlayer, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents mediaPlayer As AxWMPLib.AxWindowsMediaPlayer
End Class
