<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Visitor1 = New visitor()
        SuspendLayout()
        ' 
        ' Visitor1
        ' 
        Visitor1.BackColor = Color.Black
        Visitor1.Dock = DockStyle.Fill
        Visitor1.ForeColor = Color.White
        Visitor1.Location = New Point(0, 0)
        Visitor1.Name = "Visitor1"
        Visitor1.Size = New Size(1904, 1041)
        Visitor1.TabIndex = 0
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1904, 1041)
        Controls.Add(Visitor1)
        Name = "Form1"
        Text = "TimeBus"
        ResumeLayout(False)
    End Sub

    Friend WithEvents Visitor1 As visitor

End Class
