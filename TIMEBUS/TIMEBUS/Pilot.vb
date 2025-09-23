Public Class Pilot
    Private itinerary As List(Of Tuple(Of String, Date, Integer)) ' List of (Location, Date, Duration)
    Private currentIndex As Integer = 0 ' To track the current location in the itinerary
    Private countdownTimer As Timer ' Timer for location countdown
    Private travelTimer As Timer ' Timer for simulating travel between locations
    Private remainingSeconds As Integer ' Tracks remaining seconds for the current location

    Private tourTimer As Timer ' Timer for overall tour countdown
    Private tourRemainingSeconds As Integer = 360 ' Total tour time in seconds (6 minutes)
    Private travelDuration As Double ' How long the travel animation should run (in seconds)
    Private travelSpeed As Double = 1 ' Normal travel speed (1 is normal speed) - Variable To Keep Track of Speed For Speed Up and Speed Down 

    ' Labels for displaying location, date, timer, and tour time remaining
    Private lblLocation As Label
    Private lblDate As Label
    Private lblTimer As Label
    Private lblTourTime As Label
    Private lblTravelTimer As Label ' New label for travel countdown timer
    Private lblSpeed As Label ' Label to display Speed (Normal, Fast, Slow)
    Private lblPrevNextLocations As Label

    ' Buttons for navigation and controls
    Private btnNext As Button
    Private btnPrevious As Button
    Private btnEnd As Button
    Private btnStop As Button
    Private btnSpeedUp As Button
    Private btnSlowDown As Button

    'Pilot Mode: Autopilot or Manual 
    Private lblMode As Label ' Label to display Autopilot/Manual mode
    Private btnToggleMode As Button ' Button to switch between Autopilot and Manual mode
    Private isAutopilot As Boolean = True ' Flag to track if the pilot is in Autopilot mode

    ' Add a Pause button to control travelDuration
    Private btnPauseTravel As Button
    Private btnTakePicture As Button

    ' PictureBox to display location images and travel image to simulate travel
    Private pictureBox As PictureBox
    Private travelImage As Image

    ' Declare the Stopwatch as a class-level variable
    Private travelStopwatch As New Stopwatch()

    ' Constructor that accepts the itinerary list
    Public Sub New(itinerary As List(Of Tuple(Of String, Date, Integer)))
        InitializeComponent()
        Me.itinerary = itinerary
        InitializeLabels()
        InitializeButtons()
        InitializePictureBox()
        LoadTravelImage()
    End Sub

    ' Method to load the travel image for simulating the journey
    Private Sub LoadTravelImage()
        travelImage = My.Resources.banner ' Example: Plane image, you can change to any other image
    End Sub

    ' Method to set up labels for location, date, timers
    Private Sub InitializeLabels()
        lblLocation = New Label()
        lblLocation.Location = New Point(10, 10)
        lblLocation.Size = New Size(660, 30)
        lblLocation.Font = New Font("Arial", 14, FontStyle.Bold)
        lblLocation.TextAlign = ContentAlignment.MiddleCenter
        Me.Controls.Add(lblLocation)

        lblDate = New Label()
        lblDate.Location = New Point(10, 50)
        lblDate.Size = New Size(660, 25)
        lblDate.Font = New Font("Arial", 12, FontStyle.Regular)
        lblDate.TextAlign = ContentAlignment.MiddleCenter
        Me.Controls.Add(lblDate)

        lblTourTime = New Label()
        lblTourTime.Location = New Point(10, 80)
        lblTourTime.Size = New Size(660, 25)
        lblTourTime.Font = New Font("Arial", 12, FontStyle.Italic)
        lblTourTime.TextAlign = ContentAlignment.MiddleCenter
        Me.Controls.Add(lblTourTime)

        lblTimer = New Label()
        lblTimer.Location = New Point(10, 110)
        lblTimer.Size = New Size(660, 25)
        lblTimer.Font = New Font("Arial", 12, FontStyle.Italic)
        lblTimer.TextAlign = ContentAlignment.MiddleCenter
        Me.Controls.Add(lblTimer)

        lblTravelTimer = New Label() ' Label for the travel countdown
        lblTravelTimer.Location = New Point(10, 140)
        lblTravelTimer.Size = New Size(660, 25)
        lblTravelTimer.Font = New Font("Arial", 12, FontStyle.Italic)
        lblTravelTimer.TextAlign = ContentAlignment.MiddleCenter
        Me.Controls.Add(lblTravelTimer)

        lblSpeed = New Label()  ' Label for Travel Speed
        lblSpeed.Location = New Point(10, 170)
        lblSpeed.Size = New Size(660, 25)
        lblSpeed.Font = New Font("Arial", 12, FontStyle.Regular)
        lblSpeed.TextAlign = ContentAlignment.MiddleCenter
        lblSpeed.Text = "Travel Speed: Normal" ' Default speed label
        Me.Controls.Add(lblSpeed)

        lblPrevNextLocations = New Label()
        lblPrevNextLocations.Location = New Point(10, 140)
        lblPrevNextLocations.Size = New Size(660, 25)
        lblPrevNextLocations.Font = New Font("Arial", 12, FontStyle.Regular)
        lblPrevNextLocations.TextAlign = ContentAlignment.MiddleCenter
        Me.Controls.Add(lblPrevNextLocations)

        ' Label for displaying Autopilot/Manual mode
        lblMode = New Label()
        lblMode.Location = New Point(500, 170)
        lblMode.Size = New Size(660, 25)
        lblMode.Font = New Font("Arial", 12, FontStyle.Regular)
        lblMode.TextAlign = ContentAlignment.MiddleCenter
        Me.Controls.Add(lblMode)
        UpdateModeLabel() ' Initialize the label text based on the current mode
    End Sub

    ' Method to set up buttons
    Private Sub InitializeButtons()
        btnNext = New Button() With {
            .Text = "Next",
            .Location = New Point(10, 170),
            .Size = New Size(100, 40)
        }
        AddHandler btnNext.Click, AddressOf BtnNext_Click
        Me.Controls.Add(btnNext)

        btnPrevious = New Button() With {
            .Text = "Previous",
            .Location = New Point(120, 170),
            .Size = New Size(100, 40)
        }
        AddHandler btnPrevious.Click, AddressOf BtnPrevious_Click
        Me.Controls.Add(btnPrevious)

        btnEnd = New Button() With {
            .Text = "End",
            .Location = New Point(230, 170),
            .Size = New Size(100, 40)
        }
        AddHandler btnEnd.Click, AddressOf BtnEnd_Click
        Me.Controls.Add(btnEnd)

        btnStop = New Button() With {
            .Text = "Stop",
            .Location = New Point(10, 220),
            .Size = New Size(100, 40)
        }
        AddHandler btnStop.Click, AddressOf BtnStop_Click
        Me.Controls.Add(btnStop)

        btnSpeedUp = New Button() With {
            .Text = "Speed Up",
            .Location = New Point(120, 220),
            .Size = New Size(100, 40)
        }
        AddHandler btnSpeedUp.Click, AddressOf BtnSpeedUp_Click
        Me.Controls.Add(btnSpeedUp)

        btnSlowDown = New Button() With {
            .Text = "Slow Down",
            .Location = New Point(230, 220),
            .Size = New Size(100, 40)
        }
        AddHandler btnSlowDown.Click, AddressOf BtnSlowDown_Click
        Me.Controls.Add(btnSlowDown)

        ' Button to toggle between Autopilot and Manual mode
        btnToggleMode = New Button() With {
            .Text = "Switch to Manual",
            .Location = New Point(750, 200),
            .Size = New Size(150, 40)
        }
        AddHandler btnToggleMode.Click, AddressOf BtnToggleMode_Click
        Me.Controls.Add(btnToggleMode)

        Me.Size = New Size(800, 800)
        Me.WindowState = FormWindowState.Maximized
    End Sub

    ' Initialize the Pause button
    Private Sub InitializePauseButton()
        btnPauseTravel = New Button() With {
        .Text = "Pause",
        .Location = New Point(340, 220),
        .Size = New Size(100, 40)
    }
        AddHandler btnPauseTravel.Click, AddressOf BtnPauseTravel_Click
        Me.Controls.Add(btnPauseTravel)
        btnPauseTravel.Visible = False ' Initially hidden
    End Sub

    ' Initialize the Take Picture button
    Private Sub InitializeTakePictureButton()
        btnTakePicture = New Button() With {
        .Text = "Take Picture",
        .Location = New Point(pictureBox.Location.X + 20, pictureBox.Location.Y + pictureBox.Height + 10),
        .Size = New Size(120, 40)
    }
        AddHandler btnTakePicture.Click, AddressOf BtnTakePicture_Click
        Me.Controls.Add(btnTakePicture)
        btnTakePicture.Visible = False ' Initially hidden
    End Sub

    ' Handle Take Picture button click
    Private Sub BtnTakePicture_Click(sender As Object, e As EventArgs)
        ' Show a prompt indicating the picture has been taken
        MessageBox.Show("Picture Taken", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    ' Method to initialize PictureBox for image display
    Private Sub InitializePictureBox()
        pictureBox = New PictureBox()
        pictureBox.Location = New Point(10, 270)
        pictureBox.Size = New Size(600, 400)
        pictureBox.SizeMode = PictureBoxSizeMode.StretchImage
        Me.Controls.Add(pictureBox)
    End Sub

    ' Form Load event to initialize and start timers
    Private Sub Pilot_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitializePauseButton()
        InitializeTakePictureButton()
        countdownTimer = New Timer()
        AddHandler countdownTimer.Tick, AddressOf Countdown_Tick
        countdownTimer.Interval = 1000

        tourTimer = New Timer()
        AddHandler tourTimer.Tick, AddressOf TourCountdown_Tick
        tourTimer.Interval = 1000 ' 1-second intervals for tour timer

        travelTimer = New Timer()
        AddHandler travelTimer.Tick, AddressOf Travel_Tick
        travelTimer.Interval = 1000 ' Travel image changes every 1 second

        tourTimer.Start()
        ShowNextLocation()
    End Sub

    ' Method to display the current location in the itinerary and initialize the timer
    ' Modify ShowNextLocation to update previous and next locations
    Private Sub ShowNextLocation()
        If currentIndex < itinerary.Count Then
            Dim item = itinerary(currentIndex)
            lblLocation.Text = $"Location: {item.Item1}"
            lblDate.Text = $"Date: {item.Item2.ToShortDateString()}"
            remainingSeconds = item.Item3 * 60
            lblTimer.Text = $"Time Remaining: {remainingSeconds} seconds"

            ' Update previous and next locations
            Dim prevLocation As String = If(currentIndex > 0, itinerary(currentIndex - 1).Item1, String.Empty)
            Dim nextLocation As String = If(currentIndex < itinerary.Count - 1, itinerary(currentIndex + 1).Item1, String.Empty)
            lblPrevNextLocations.Text = $"Previous Location: {prevLocation}    Next Location: {nextLocation}"

            ' Reset travel duration based on current speed
            travelDuration = 10 ' Default travel duration (adjusted later)
            lblTravelTimer.Text = $"Travel Time Remaining: {travelDuration} seconds"

            pictureBox.Image = travelImage

            btnTakePicture.Visible = True

            ' Start the travel simulation (show travel image)
            HideUIElements(True)
            travelTimer.Start()
            btnPauseTravel.Visible = True ' Show pause button during travel
        Else
            EndTour("Tour complete!")
        End If
        UpdateButtonStates()
    End Sub

    ' Modify the Travel_Tick method to respect the pause/resume functionality
    Private Sub Travel_Tick(sender As Object, e As EventArgs)
        ' If the stopwatch hasn't started, start it (this starts when the travel starts)
        If Not travelStopwatch.IsRunning Then
            travelStopwatch.Start()
        End If

        ' Reduce travel time based on speed (travelSpeed adjusts the rate)
        travelDuration -= 1 ' Adjust travel time based on speed

        ' Update the travel timer display
        lblTravelTimer.Text = $"Travel Time Remaining: {Math.Ceiling(travelDuration)} seconds"

        ' Once travel duration reaches zero, stop the travel timer and update the location
        If travelDuration <= 0 Then
            travelTimer.Stop()
            ' Stop the stopwatch to measure the elapsed time
            travelStopwatch.Stop()

            ' Get the elapsed time in seconds
            Dim elapsedTime As Double = travelStopwatch.Elapsed.TotalSeconds + 1

            ' Subtract the elapsed time from the remainingSeconds
            remainingSeconds -= elapsedTime
            lblTimer.Text = $"Time Remaining: {remainingSeconds} seconds"

            ' Reset the stopwatch for the next travel
            travelStopwatch.Reset()

            ' Set the image for the next location after travel
            Dim item = itinerary(currentIndex)
            ' Check if the date of the location is in the future, present, or past
            If item.Item2 >= DateTime.Now Then
                ' The location date is in the future
                pictureBox.Image = CType(My.Resources.ResourceManager.GetObject("futurecity"), Image)
            ElseIf item.Item2 < DateTime.Now Then
                ' The location date is in the past
                pictureBox.Image = CType(My.Resources.ResourceManager.GetObject("past"), Image)
            End If
            'pictureBox.Image = CType(My.Resources.ResourceManager.GetObject(item.Item1.Replace(" ", "").ToLower()), Image)

            ' Hide the travel UI elements and show the location UI
            HideUIElements(False)
            countdownTimer.Start()
            btnPauseTravel.Visible = False ' Hide pause button after travel
            ' Reset travelSpeed to 1 and Update Speed Label (Ensure its Back to Normal)
            travelSpeed = 1
            UpdateSpeedLabel()
        End If
    End Sub

    Private Sub TourCountdown_Tick(sender As Object, e As EventArgs)
        tourRemainingSeconds -= 1
        lblTourTime.Text = $"Tour Time Remaining: {tourRemainingSeconds} seconds"

        If tourRemainingSeconds <= 0 Then
            tourTimer.Stop()
            EndTour("Tour ended by time limit.")
        End If
    End Sub

    Private Sub Countdown_Tick(sender As Object, e As EventArgs)
        remainingSeconds -= 1
        lblTimer.Text = $"Time Remaining: {remainingSeconds} seconds"

        If remainingSeconds <= 0 Then
            countdownTimer.Stop()
            ShowNextLocation()
        End If
    End Sub

    Private Sub BtnSpeedUp_Click(sender As Object, e As EventArgs)
        If travelSpeed = 1 Then
            travelSpeed = 2 ' Double the speed
            travelDuration = travelDuration / 2
        ElseIf travelSpeed = 0.5 Then
            travelSpeed = 1 ' Back to Normal Speed
            travelDuration = travelDuration / 2
        ElseIf travelSpeed = 2 Then
            MessageBox.Show("Cannot speed up further.")
            Return
        End If

        ' Update the speed label
        UpdateSpeedLabel()

        SwitchToManualMode()
    End Sub

    Private Sub BtnSlowDown_Click(sender As Object, e As EventArgs)
        If travelSpeed = 1 Then
            travelSpeed = 0.5 ' Half the speed
            travelDuration = travelDuration * 2
        ElseIf travelSpeed = 2 Then
            travelSpeed = 1 ' Back to Normal Speed
            travelDuration = travelDuration * 2
        ElseIf travelSpeed = 0.5 Then
            MessageBox.Show("Cannot slow down further.")
            Return
        End If

        ' Update the speed label
        UpdateSpeedLabel()

        SwitchToManualMode()
    End Sub

    ' Method to update the speed label
    Private Sub UpdateSpeedLabel()
        If travelSpeed = 1 Then
            lblSpeed.Text = "Travel Speed: Normal (1x)"
        ElseIf travelSpeed = 2 Then
            lblSpeed.Text = "Travel Speed: Fast (2x)"
        ElseIf travelSpeed = 0.5 Then
            lblSpeed.Text = "Travel Speed: Slow (0.5x)"
        End If
    End Sub

    ' Method to hide or show the UI elements based on the "hide" argument
    Private Sub HideUIElements(hide As Boolean)
        lblLocation.Visible = Not hide
        lblDate.Visible = Not hide
        lblTimer.Visible = Not hide
        lblTourTime.Visible = Not hide
        lblTravelTimer.Visible = hide
        lblSpeed.Visible = hide
        btnNext.Visible = Not hide
        btnPrevious.Visible = Not hide
        btnEnd.Visible = Not hide
        btnStop.Visible = Not hide
        btnSpeedUp.Visible = hide
        btnSlowDown.Visible = hide
    End Sub

    ' Method to end the tour with a message
    Private Sub EndTour(message As String)
        countdownTimer.Stop()
        tourTimer.Stop()
        MessageBox.Show(message)
        Me.Close()
    End Sub

    ' Method to update the button states based on the current location
    Private Sub UpdateButtonStates()
        btnPrevious.Enabled = currentIndex > 0
        btnNext.Enabled = currentIndex < itinerary.Count - 1
    End Sub

    ' Event handler for Next button
    Private Sub BtnNext_Click(sender As Object, e As EventArgs)
        If currentIndex < itinerary.Count - 1 Then
            countdownTimer.Stop()
            currentIndex += 1
            ShowNextLocation()
        End If

        SwitchToManualMode()
    End Sub

    ' Event handler for Previous button
    Private Sub BtnPrevious_Click(sender As Object, e As EventArgs)
        If currentIndex > 0 Then
            countdownTimer.Stop()
            currentIndex -= 1
            ShowNextLocation()
        End If

        SwitchToManualMode()
    End Sub

    ' Event handler for End button
    Private Sub BtnEnd_Click(sender As Object, e As EventArgs)
        EndTour("Tour ended by user.")

        SwitchToManualMode()
    End Sub

    ' Event handler for Stop button
    Private Sub BtnStop_Click(sender As Object, e As EventArgs)
        If countdownTimer.Enabled Then
            countdownTimer.Stop()
            btnStop.Text = "Resume"
        Else
            countdownTimer.Start()
            btnStop.Text = "Stop"
        End If

        SwitchToManualMode()
    End Sub

    ' Add event handler for Pause button
    Private Sub BtnPauseTravel_Click(sender As Object, e As EventArgs)
        If travelTimer.Enabled Then
            travelTimer.Stop()
            btnPauseTravel.Text = "Resume"
        Else
            travelTimer.Start()
            btnPauseTravel.Text = "Pause"
        End If

        SwitchToManualMode()
    End Sub

    ' Event handler for the Mode toggle button
    Private Sub BtnToggleMode_Click(sender As Object, e As EventArgs)
        isAutopilot = Not isAutopilot ' Toggle the Autopilot flag

        If isAutopilot Then
            btnToggleMode.Text = "Switch to Manual" ' Update button text
        Else
            btnToggleMode.Text = "Switch to Autopilot" ' Update button text
        End If

        UpdateModeLabel() ' Update the mode label text
    End Sub

    ' Method to update the Mode label
    Private Sub UpdateModeLabel()
        If isAutopilot Then
            lblMode.Text = "TimeBus Mode: Autopilot" ' Display Autopilot mode
        Else
            lblMode.Text = "TimeBus Mode: Manual" ' Display Manual mode
        End If
    End Sub

    ' Helper method to switch to Manual Mode
    Private Sub SwitchToManualMode()
        isAutopilot = False ' Switch to manual mode
        lblMode.Text = "TimeBus Mode: Manual" ' Update label
        btnToggleMode.Text = "Switch to Autopilot" ' Update button text
    End Sub

End Class