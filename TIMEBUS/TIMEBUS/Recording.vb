Imports AxWMPLib

Public Class Recording

    Private lblDescription As New Label()
    Private locationDescription As String
    Private videoPath As String
    Private WithEvents btnPlayPause As New Button() ' Declare Play/Pause button as WithEvents

    'Variables used for Playback Control: Skipping to a Specific Time in Video 
    Private lblSkipTo As New Label()
    Private txtSkipTo As New TextBox()
    Private btnSkipTo As New Button()
    Private lblVideoDuration As New Label() ' Total Video Duration 
    Private WithEvents timer As New Timer() ' Timer to periodically check the video duration

    Public Sub New(description As String, videoPath As String)
        Me.locationDescription = description
        Me.videoPath = videoPath

        'Set the size of the Form
        Me.Size = New Size(800, 800)

        ' Configure description label
        lblDescription.Text = description
        lblDescription.AutoSize = True
        lblDescription.Location = New Point(10, 10)
        Controls.Add(lblDescription)

        ' Configure Skip to time controls
        lblSkipTo.Text = "Skip to Time (seconds):"
        lblSkipTo.Location = New Point(10, 700)
        lblSkipTo.Size = New Size(150, 30)
        Controls.Add(lblSkipTo)

        txtSkipTo.Location = New Point(160, 700)
        txtSkipTo.Width = 100
        Controls.Add(txtSkipTo)

        btnSkipTo.Text = "Skip"
        btnSkipTo.Location = New Point(270, 700)
        btnSkipTo.Width = 80
        AddHandler btnSkipTo.Click, AddressOf SkipTo_Click
        Controls.Add(btnSkipTo)

        lblVideoDuration.Location = New Point(360, 700)
        lblVideoDuration.Size = New Size(150, 30)
        Controls.Add(lblVideoDuration)

        ' Playback control buttons
        'Dim btnPlayPause As New Button() With {.Text = "Play/Pause", .Location = New Point(10, 650), .Width = 135, .Height = 40}
        'AddHandler btnPlayPause.Click, AddressOf PlayPause_Click
        'Controls.Add(btnPlayPause)

        btnPlayPause.Text = "Play" ' Set the initial text as "Play"
        btnPlayPause.Location = New Point(10, 650)
        btnPlayPause.Width = 135
        btnPlayPause.Height = 40
        AddHandler btnPlayPause.Click, AddressOf PlayPause_Click
        Controls.Add(btnPlayPause)

        Dim btnRestart As New Button() With {.Text = "Restart", .Location = New Point(140, 650), .Width = 120, .Height = 40}
        AddHandler btnRestart.Click, AddressOf Restart_Click
        Controls.Add(btnRestart)

        Dim btnRewind As New Button() With {.Text = "Rewind 10s", .Location = New Point(250, 650), .Width = 120, .Height = 40}
        AddHandler btnRewind.Click, AddressOf Rewind_Click
        Controls.Add(btnRewind)

        Dim btnForward As New Button() With {.Text = "Forward 10s", .Location = New Point(360, 650), .Width = 120, .Height = 40}
        AddHandler btnForward.Click, AddressOf Forward_Click
        Controls.Add(btnForward)

        Dim btnSpeedUp As New Button() With {.Text = "Speed Up", .Location = New Point(470, 650), .Width = 120, .Height = 40}
        AddHandler btnSpeedUp.Click, AddressOf SpeedUp_Click
        Controls.Add(btnSpeedUp)

        Dim btnSlowDown As New Button() With {.Text = "Slow Down", .Location = New Point(580, 650), .Width = 120, .Height = 40}
        AddHandler btnSlowDown.Click, AddressOf SlowDown_Click
        Controls.Add(btnSlowDown)
    End Sub

    Private Sub Recording_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Create the media player control dynamically
        mediaPlayer = New AxWMPLib.AxWindowsMediaPlayer()

        ' Set properties for the media player
        mediaPlayer.Dock = DockStyle.Fill ' Make the media player fill the form

        ' Add the media player control to the form's controls
        Controls.Add(mediaPlayer)

        ' Set the video URL if the file exists
        If IO.File.Exists(videoPath) Then
            mediaPlayer.URL = videoPath
            mediaPlayer.Ctlcontrols.play() ' Automatically play the video to load Duration label 

            ' Add PlayStateChange event handler to detect when the video starts playing
            AddHandler mediaPlayer.PlayStateChange, AddressOf Pausing
            'mediaPlayer.Ctlcontrols.pause()  ' Stop the video in a thumbnail state
        Else
            MessageBox.Show("Video file not found: " & videoPath)
        End If

        If mediaPlayer.playState = WMPLib.WMPPlayState.wmppsPlaying Then
            mediaPlayer.Ctlcontrols.pause()
        End If

        ' Hide the default UI (no controls from the media player interface)
        mediaPlayer.uiMode = "none" ' Disables the default Windows Media Player controls

        ' Start the timer to periodically check if the media is loaded and get its duration
        timer.Interval = 1000 ' Check every second
        timer.Start()

        ' Add PlayStateChange event handler to detect when video finishes playing
        AddHandler mediaPlayer.PlayStateChange, AddressOf VideoFinish
    End Sub

    ' This method is triggered when the play state changes
    Private Sub Pausing(sender As Object, e As AxWMPLib._WMPOCXEvents_PlayStateChangeEvent)
        ' Check if the video has started playing
        If e.newState = WMPLib.WMPPlayState.wmppsPlaying Then
            ' Pause the video once it starts playing
            mediaPlayer.Ctlcontrols.pause()

            ' Remove the event handler to avoid pausing repeatedly
            RemoveHandler mediaPlayer.PlayStateChange, AddressOf Pausing
        End If
    End Sub

    ' This method is triggered when the play state changes
    Private Sub VideoFinish(sender As Object, e As AxWMPLib._WMPOCXEvents_PlayStateChangeEvent)
        ' Check if the media player has finished playing (wmppsMediaEnded)
        If e.newState = WMPLib.WMPPlayState.wmppsMediaEnded Then
            btnPlayPause.Text = "Play" ' Set the button text to "Play" when the video ends
        End If
    End Sub

    Private Sub Timer_Tick(sender As Object, e As EventArgs) Handles timer.Tick
        ' Check if media is loaded and the duration is available
        If mediaPlayer.currentMedia IsNot Nothing AndAlso mediaPlayer.currentMedia.duration > 0 Then
            ' Update the label with the video duration in seconds
            lblVideoDuration.Text = "Duration: " & mediaPlayer.currentMedia.durationString
            timer.Stop() ' Stop the timer after the duration is retrieved
        End If
    End Sub

    Private Sub PlayPause_Click(sender As Object, e As EventArgs)
        If mediaPlayer.playState = WMPLib.WMPPlayState.wmppsPlaying Then
            mediaPlayer.Ctlcontrols.pause()
            btnPlayPause.Text = "Play" ' Change text to "Play" when paused
        Else
            mediaPlayer.Ctlcontrols.play()
            btnPlayPause.Text = "Pause" ' Change text to "Pause" when playing
        End If
    End Sub

    ' Form Closing event handler
    Private Sub Recording_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        ' Stop the media player when the form is closing
        If mediaPlayer IsNot Nothing Then
            mediaPlayer.Ctlcontrols.stop()
        End If
    End Sub

    Private Sub Restart_Click(sender As Object, e As EventArgs)
        mediaPlayer.Ctlcontrols.currentPosition = 0
    End Sub

    Private Sub Rewind_Click(sender As Object, e As EventArgs)
        mediaPlayer.Ctlcontrols.currentPosition -= 10
    End Sub

    Private Sub Forward_Click(sender As Object, e As EventArgs)
        mediaPlayer.Ctlcontrols.currentPosition += 10
    End Sub

    Private Sub SpeedUp_Click(sender As Object, e As EventArgs)
        mediaPlayer.settings.rate = Math.Min(2.0, mediaPlayer.settings.rate + 0.5)
    End Sub

    Private Sub SlowDown_Click(sender As Object, e As EventArgs)
        mediaPlayer.settings.rate = Math.Max(0.5, mediaPlayer.settings.rate - 0.5)
    End Sub

    'Skip to a Specific Time
    Private Sub SkipTo_Click(sender As Object, e As EventArgs)
        Dim skipTime As Double

        ' Try to parse the entered time
        If Double.TryParse(txtSkipTo.Text, skipTime) Then
            ' Validate the skip time
            If skipTime >= 0 AndAlso skipTime <= mediaPlayer.currentMedia.duration Then
                mediaPlayer.Ctlcontrols.currentPosition = skipTime
            Else
                MessageBox.Show("Invalid time. Please enter a value between 0 and " & mediaPlayer.currentMedia.duration.ToString())
            End If
        Else
            MessageBox.Show("Please enter a valid number for the time.")
        End If
    End Sub

End Class