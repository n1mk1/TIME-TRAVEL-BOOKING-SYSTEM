Public Class visitor
    Private addDefaultsButton As New Button()
    Private addLocationButton As New Button()
    Private removeLocationButton As New Button()

    Private pictureBoxes As New List(Of PictureBox)()
    Private imageNames As New List(Of String)()
    Private adventureDescriptions As New Dictionary(Of String, String)()
    Private itinerary As New List(Of Tuple(Of String, Date, Integer))()
    Private allItineraries As New List(Of List(Of Tuple(Of String, Date, Integer)))()
    Private locationEvents As New Dictionary(Of String, List(Of Tuple(Of String, Date)))()
    Private descriptionPanel As Panel
    Private descriptionLabel As Label
    Private scrollablePanel As Panel
    Private itineraryPanel As Panel
    Private notificationLabel As Label
    Private newToursPanel As Panel
    Private tourHistoryPanel As Panel
    Private originalFormWidth As Integer
    Private originalFormHeight As Integer
    Private remainingHours As Integer = 6
    Private confirmedTourCount As Integer = 0

    Public Event ItineraryConfirmed(itinerary As List(Of Tuple(Of String, Date, Integer)))

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Set the initial form size to 1920x1080
        Me.Size = New Size(1920, 1080)
        originalFormWidth = Me.Width
        originalFormHeight = Me.Height


        ' Set dark mode theme for the form
        Me.BackColor = Color.Black
        Me.ForeColor = Color.White

        ' Initialize main UI components
        InitializeItineraryPanel()
        InitializeBannerAndButtons()
        InitializeScrollablePanel()
        InitializeDescriptionPanel()
        InitializeNotificationLabel()
        InitializeAddDefaultsButton()
        InitializeAddLocationButton()
        InitializeRemoveLocationButton()
        InitializeNewToursPanel()
        InitializeTourHistoryPanel()
        InitializeContent() 'put up location picture boxes
        InitializeCuratedTour()

        ' Add event handler for resizing the form
        AddHandler Me.Resize, AddressOf Form1_Resize
    End Sub

    Private Sub InitializeContent()
        addDefaultsButton.PerformClick()
    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs)
        ' Calculate scaling factors
        Dim widthScale As Double = Me.Width / originalFormWidth
        Dim heightScale As Double = Me.Height / originalFormHeight

        ' Resize and reposition all controls proportionally
        ResizeControl(itineraryPanel, widthScale, heightScale)
        ResizeControl(scrollablePanel, widthScale, heightScale)
        ResizeControl(descriptionPanel, widthScale, heightScale)
        ResizeControl(newToursPanel, widthScale, heightScale)
        ResizeControl(tourHistoryPanel, widthScale, heightScale)
        ResizeControl(notificationLabel, widthScale, heightScale)
        ResizeControl(addDefaultsButton, widthScale, heightScale)
        ResizeControl(addLocationButton, widthScale, heightScale)
        ResizeControl(removeLocationButton, widthScale, heightScale)

        For Each control As Control In Me.Controls
            If TypeOf control Is Button Or TypeOf control Is Label Or TypeOf control Is Panel Then
                ResizeControl(control, widthScale, heightScale)
            End If
        Next

        ' Update original form size to the new size
        originalFormWidth = Me.Width
        originalFormHeight = Me.Height
    End Sub

    Private Sub ResizeControl(control As Control, widthScale As Double, heightScale As Double)
        control.Location = New Point(CInt(control.Location.X * widthScale), CInt(control.Location.Y * heightScale))
        control.Size = New Size(CInt(control.Size.Width * widthScale), CInt(control.Size.Height * heightScale))


        If TypeOf control Is Label Or TypeOf control Is Button Then
            Dim ctrlFontSize As Single = control.Font.Size * CSng(Math.Min(widthScale, heightScale))
            control.Font = New Font(control.Font.FontFamily, ctrlFontSize, control.Font.Style)
        End If
    End Sub

    Private Sub InitializeBannerAndButtons()
        ' Add the sort button below the itinerary panel
        Dim sortButton As New Button()
        sortButton.Text = "Sort Alphabetically"
        sortButton.Location = New Point(755, 10)
        sortButton.Size = New Size(225, 30)
        sortButton.BackColor = Color.FromArgb(30, 30, 60) ' Dark blue with a hint of space feel
        sortButton.ForeColor = Color.Cyan
        sortButton.FlatStyle = FlatStyle.Flat
        sortButton.FlatAppearance.BorderSize = 0
        sortButton.Region = New Region(New Rectangle(0, 0, sortButton.Width, sortButton.Height))
        AddHandler sortButton.Click, AddressOf SortButton_Click


        ' Add the recommended button below the sort button
        Dim recommendButton As New Button()
        recommendButton.Text = "Recommended"
        recommendButton.Location = New Point(989, 10)
        recommendButton.Size = New Size(225, 30)
        recommendButton.BackColor = Color.FromArgb(30, 60, 30) ' Dark green with a futuristic touch
        recommendButton.ForeColor = Color.Lime
        recommendButton.FlatStyle = FlatStyle.Flat
        recommendButton.FlatAppearance.BorderSize = 0
        recommendButton.Region = New Region(New Rectangle(0, 0, recommendButton.Width, recommendButton.Height))
        AddHandler recommendButton.Click, AddressOf RecommendButton_Click


        ' Add a label below the recommended button
        Dim chooseLocationLabel As New Label()
        chooseLocationLabel.Text = "Click To Choose Location"
        chooseLocationLabel.Font = New Font("Arial", 11, FontStyle.Bold) ' Updated font size and style
        chooseLocationLabel.Location = New Point(1525, 80)
        chooseLocationLabel.Size = New Size(300, 20)
        chooseLocationLabel.TextAlign = ContentAlignment.MiddleCenter
        chooseLocationLabel.ForeColor = Color.Cyan ' Set the text color to cyan
        Me.Controls.Add(chooseLocationLabel)
    End Sub

    Private Sub InitializeScrollablePanel()
        ' Create a panel for displaying picture boxes
        scrollablePanel = New Panel()
        scrollablePanel.Location = New Point(1440, 120)
        scrollablePanel.Size = New Size(440, 900)
        scrollablePanel.AutoScroll = True
        scrollablePanel.BackColor = Color.FromArgb(20, 20, 40) ' Dark space-like background
        Me.Controls.Add(scrollablePanel)
    End Sub

    Private Sub InitializeDescriptionPanel()
        ' Create a panel for displaying descriptions
        descriptionPanel = New Panel()
        descriptionPanel.Size = New Size(800, 1050) ' Adjust the size to fit both the description and the image
        descriptionPanel.BackColor = Color.FromArgb(50, 50, 70) ' Dark gray with a hint of blue
        descriptionPanel.Visible = False
        descriptionPanel.BorderStyle = BorderStyle.FixedSingle
        Me.Controls.Add(descriptionPanel)

        ' Create a label to show the description inside the panel
        descriptionLabel = New Label()
        descriptionLabel.AutoSize = False
        descriptionLabel.Dock = DockStyle.Bottom
        descriptionLabel.TextAlign = ContentAlignment.MiddleCenter
        descriptionLabel.Font = New Font("Arial", 10, FontStyle.Regular)
        descriptionLabel.ForeColor = Color.LightGray
        descriptionLabel.Height = 100 ' Adjust height for text area
        descriptionPanel.Controls.Add(descriptionLabel)

        ' Create a PictureBox to show the news image inside the panel
        Dim newsPictureBox As New PictureBox()
        newsPictureBox.Dock = DockStyle.Fill
        newsPictureBox.SizeMode = PictureBoxSizeMode.StretchImage
        newsPictureBox.BackColor = Color.FromArgb(10, 10, 30) ' Background color for picture box
        descriptionPanel.Controls.Add(newsPictureBox)

        ' Store the PictureBox reference in the panel's Tag for easier access
        descriptionPanel.Tag = newsPictureBox
    End Sub

    Private Sub InitializeItineraryPanel()
        ' Create a panel for displaying the itinerary
        itineraryPanel = New Panel()
        itineraryPanel.Location = New Point(20, 20)
        itineraryPanel.Size = New Size(1400, 500)
        itineraryPanel.AutoScroll = True
        itineraryPanel.BorderStyle = BorderStyle.FixedSingle
        itineraryPanel.BackColor = Color.FromArgb(20, 20, 40) ' Dark background to match the space theme
        Me.Controls.Add(itineraryPanel)

        ' Create a PictureBox and add the idle.gif from resources
        Dim pictureBoxIdle As New PictureBox()
        pictureBoxIdle.Image = My.Resources.idle
        pictureBoxIdle.SizeMode = PictureBoxSizeMode.StretchImage ' Make it fill the panel
        pictureBoxIdle.Dock = DockStyle.Fill ' Make it fit the entire panel

        ' Add PictureBox to the panel
        itineraryPanel.Controls.Add(pictureBoxIdle)
    End Sub



    Private Sub InitializeNotificationLabel()
        ' Create a label for notifications
        notificationLabel = New Label()
        notificationLabel.Size = New Size(600, 30) ' Increase the size to fit more text
        notificationLabel.Location = New Point(450, itineraryPanel.Bottom + 10) ' Position it right below the itinerary panel
        notificationLabel.TextAlign = ContentAlignment.MiddleCenter
        notificationLabel.Font = New Font("Arial", 10, FontStyle.Bold)
        notificationLabel.ForeColor = Color.OrangeRed
        notificationLabel.Visible = False ' Initially set to hidden until updated
        Me.Controls.Add(notificationLabel)
    End Sub

    Private Sub InitializeAddDefaultsButton()
        ' Add a button for adding default locations
        addDefaultsButton.Text = "Add Defaults"
        addDefaultsButton.Location = New Point(1000, 14)
        addDefaultsButton.Size = New Size(300, 30)
        addDefaultsButton.BackColor = Color.FromArgb(40, 40, 60) ' Dark blue with a hint of space feel
        addDefaultsButton.ForeColor = Color.Cyan
        addDefaultsButton.FlatStyle = FlatStyle.Flat
        addDefaultsButton.FlatAppearance.BorderSize = 0
        AddHandler addDefaultsButton.Click, AddressOf AddDefaultsButton_Click
    End Sub

    Private Sub InitializeAddLocationButton()
        ' Add a button for adding a new location
        addLocationButton.Text = "Add Custom Location"
        addLocationButton.Location = New Point(1440, 20)
        addLocationButton.Size = New Size(440, 50)
        addLocationButton.BackColor = Color.FromArgb(40, 40, 60) ' Dark blue with a hint of space feel
        addLocationButton.ForeColor = Color.Cyan
        addLocationButton.FlatStyle = FlatStyle.Flat
        addLocationButton.FlatAppearance.BorderSize = 0
        AddHandler addLocationButton.Click, AddressOf AddLocationButton_Click
        Me.Controls.Add(addLocationButton)
    End Sub

    Private Sub InitializeRemoveLocationButton()
        ' Add a button for removing a location
        removeLocationButton.Text = "Remove Location"
        removeLocationButton.Location = New Point(10, 590)
        removeLocationButton.Size = New Size(300, 30)
        removeLocationButton.BackColor = Color.FromArgb(60, 30, 30) ' Dark red with a hint of space feel
        removeLocationButton.ForeColor = Color.Cyan
        removeLocationButton.FlatStyle = FlatStyle.Flat
        removeLocationButton.FlatAppearance.BorderSize = 0
        AddHandler removeLocationButton.Click, AddressOf RemoveLocationButton_Click
    End Sub

    Private Sub InitializeNewToursPanel()
        ' Create a panel for displaying new tours information
        newToursPanel = New Panel()
        newToursPanel.Location = New Point(20, itineraryPanel.Bottom + 50)
        newToursPanel.Size = New Size(680, 450)
        newToursPanel.AutoScroll = True
        newToursPanel.BorderStyle = BorderStyle.FixedSingle
        newToursPanel.BackColor = Color.FromArgb(20, 20, 40) ' Dark background to match the space theme

        ' Add title label "New Tours" at the top of the panel
        Dim newToursTitle As New Label()
        newToursTitle.Text = "New Tours"
        newToursTitle.Font = New Font("Arial", 12, FontStyle.Bold)
        newToursTitle.ForeColor = Color.White
        newToursTitle.Location = New Point(10, 10)
        newToursTitle.AutoSize = True ' Enable AutoSize to ensure the label fits the text
        newToursPanel.Controls.Add(newToursTitle)

        Me.Controls.Add(newToursPanel)
    End Sub

    Private Sub InitializeTourHistoryPanel()
        ' Create a panel for displaying tour history information
        tourHistoryPanel = New Panel()
        tourHistoryPanel.Location = New Point(750, itineraryPanel.Bottom + 50)
        tourHistoryPanel.Size = New Size(680, 450)
        tourHistoryPanel.AutoScroll = True
        tourHistoryPanel.BorderStyle = BorderStyle.FixedSingle
        tourHistoryPanel.BackColor = Color.FromArgb(20, 20, 40) ' Dark background to match the space theme

        ' Add title label "Tour History" at the top of the panel
        Dim tourHistoryTitle As New Label()
        tourHistoryTitle.Text = "Curated Tours"
        tourHistoryTitle.Font = New Font("Arial", 12, FontStyle.Bold)
        tourHistoryTitle.ForeColor = Color.White
        tourHistoryTitle.Location = New Point(10, 10)
        tourHistoryTitle.AutoSize = True ' Enable AutoSize to ensure the label fits the text
        tourHistoryPanel.Controls.Add(tourHistoryTitle)
        Me.Controls.Add(tourHistoryPanel)
    End Sub

    Private Sub InitializeCuratedTour()
        ' Define the curated tour details
        Dim tourName As String = "European Tour"
        Dim curatedItinerary As New List(Of Tuple(Of String, Date, Integer)) From {
        Tuple.Create("Paris", New Date(1763, 4, 1), 2),
        Tuple.Create("Hamburg", New Date(1842, 5, 8), 2),
        Tuple.Create("Meribel", New Date(2060, 1, 14), 2)
    }

        ' Create a panel to display the tour
        Dim tourPanel As New Panel()
        tourPanel.Width = tourHistoryPanel.Width - 20
        tourPanel.BorderStyle = BorderStyle.FixedSingle
        tourPanel.BackColor = Color.FromArgb(30, 30, 60) ' Dark theme

        ' Add a title label for the curated tour
        Dim titleLabel As New Label()
        titleLabel.Text = $"{tourName}"
        titleLabel.Font = New Font("Arial", 14, FontStyle.Bold)
        titleLabel.ForeColor = Color.White
        titleLabel.Location = New Point(10, 10)
        titleLabel.AutoSize = True
        tourPanel.Controls.Add(titleLabel)

        ' Display each location in the curated tour
        Dim yOffset As Integer = 50
        For Each item In curatedItinerary
            Dim locationLabel As New Label()
            locationLabel.Text = $"Location: {item.Item1}{vbCrLf}" &
                             $"Date: {item.Item2.ToShortDateString()}{vbCrLf}" &
                             $"Time: {item.Item3} hour(s)"
            locationLabel.Font = New Font("Arial", 10, FontStyle.Regular)
            locationLabel.ForeColor = Color.LightGray
            locationLabel.Location = New Point(10, yOffset)
            locationLabel.AutoSize = True
            tourPanel.Controls.Add(locationLabel)
            yOffset += 60 ' Adjust for spacing

            ' Description label for each location
            Dim descriptionLabel As New Label()
            descriptionLabel.Text = GetLocationDescription(item.Item1)
            descriptionLabel.Font = New Font("Arial", 9, FontStyle.Italic)
            descriptionLabel.ForeColor = Color.LightGray
            descriptionLabel.Location = New Point(10, yOffset)
            descriptionLabel.Size = New Size(tourPanel.Width - 20, 40) ' Width adjustment for description
            descriptionLabel.AutoSize = False
            tourPanel.Controls.Add(descriptionLabel)
            yOffset += 50 ' Adjust for spacing

            ' Add a "View Recording" button for each location
            Dim viewRecordingButton As New Button()
            viewRecordingButton.Text = "View Recording"
            viewRecordingButton.Size = New Size(120, 30)
            viewRecordingButton.Location = New Point(10, yOffset)
            AddHandler viewRecordingButton.Click, Sub()
                                                      ' Get the location name (used for video path)
                                                      Dim locationName As String = item.Item1

                                                      ' Get the description from the GetLocationDescription function
                                                      Dim description As String = GetLocationDescription(locationName)

                                                      ' Define the path to the video file (adjust the path based on your video storage)
                                                      Dim videoPath As String = System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.StartupPath, "..", "..", "..", "Resources", locationName & ".mp4"))

                                                      ' Show the Recording form with the description and video path
                                                      Dim videoForm As New Recording(description, videoPath)
                                                      videoForm.ShowDialog() ' Display the video form as a modal dialog
                                                  End Sub
            tourPanel.Controls.Add(viewRecordingButton)

            yOffset += 40 ' Adjust for button spacing
        Next

        ' Adjust the panel height dynamically based on the content
        tourPanel.Height = yOffset + 60 ' Extra space for buttons

        ' Add a "Copy" button to duplicate the curated tour
        Dim copyButton As New Button()
        copyButton.Text = "Copy"
        copyButton.Font = New Font(copyButton.Font.FontFamily, 12, FontStyle.Bold)
        copyButton.Size = New Size(150, 40)
        copyButton.BackColor = Color.FromArgb(50, 150, 50) ' Dark blue for the button
        copyButton.ForeColor = Color.White
        copyButton.FlatStyle = FlatStyle.Flat
        copyButton.FlatAppearance.BorderSize = 0
        'Dim buttonYPosition As Integer = (tourPanel.Height) / 2.5
        'copyButton.Location = New Point(tourPanel.Width - copyButton.Width - 10, buttonYPosition) ' Adjusted position

        'New Position Based On Updates of Adding Description Label and Recording Button
        copyButton.Location = New Point(tourPanel.Width - copyButton.Width - 10, 10) ' Top right position
        AddHandler copyButton.Click, Sub()
                                         AddCuratedTourToNewTours(tourName, curatedItinerary)
                                     End Sub
        tourPanel.Controls.Add(copyButton)

        ' Position the panel dynamically in the tourHistoryPanel
        Dim totalHeight As Integer = 10 ' Start from a slight offset
        For Each control As Control In tourHistoryPanel.Controls
            totalHeight += control.Height + 10 ' Add spacing
        Next
        tourPanel.Location = New Point(5, totalHeight)

        ' Add the panel to the tourHistoryPanel
        tourHistoryPanel.Controls.Add(tourPanel)

        ' Adjust scrollable area
        Dim cumulativeHeight As Integer = 0
        For Each control As Control In tourHistoryPanel.Controls
            cumulativeHeight += control.Height + 10
        Next
        tourHistoryPanel.AutoScrollMinSize = New Size(tourHistoryPanel.Width, cumulativeHeight)
    End Sub

    Private Function GetLocationDescription(location As String) As String
        ' Return a description for each location
        Select Case location.ToLower()
            Case "hamburg"
                Return "Hamburg: Showcasing the Great Fire of Hamburg tragedy leaving over a third of the city in ruins."
            Case "meribel"
                Return "Meribel: Futuristic (2060) charming ski resort in the French Alps, popular for winter sports and alpine scenery."
            Case "paris"
                Return "Paris: In the 18th century, second-largest city of Europe. Showing view of the city from the Pont Neuf (1763)."
            Case Else
                Return "No description available."
        End Select
    End Function

    Private Sub AddCuratedTourToNewTours(tourName As String, curatedItinerary As List(Of Tuple(Of String, Date, Integer)))
        ' Create a panel for the curated tour in New Tours
        Dim tourPanel As New Panel()
        tourPanel.Width = newToursPanel.Width - 20
        tourPanel.BorderStyle = BorderStyle.FixedSingle
        tourPanel.BackColor = Color.FromArgb(20, 40, 60) ' Different shade for New Tours

        ' Add a title label
        Dim titleLabel As New Label()
        titleLabel.Text = $"{tourName} (Copied)"
        titleLabel.Font = New Font("Arial", 14, FontStyle.Bold)
        titleLabel.ForeColor = Color.White
        titleLabel.Location = New Point(10, 10)
        titleLabel.AutoSize = True
        tourPanel.Controls.Add(titleLabel)

        ' Display the itinerary
        Dim yOffset As Integer = 50
        For Each item In curatedItinerary
            Dim locationLabel As New Label()
            locationLabel.Text = $"Location: {item.Item1}{vbCrLf}" &
                             $"Date: {item.Item2.ToShortDateString()}{vbCrLf}" &
                             $"Time: {item.Item3} hour(s)"
            locationLabel.Font = New Font("Arial", 10, FontStyle.Regular)
            locationLabel.ForeColor = Color.LightGray
            locationLabel.Location = New Point(10, yOffset)
            locationLabel.AutoSize = True
            tourPanel.Controls.Add(locationLabel)
            yOffset += 60
        Next

        ' Adjust the panel height
        tourPanel.Height = yOffset + 60 ' Extra space for buttons

        ' Add "Board TimeBus" button
        Dim boardTimeBusButton As New Button()
        boardTimeBusButton.Text = "Board TimeBus"
        boardTimeBusButton.Font = New Font(boardTimeBusButton.Font.FontFamily, 12, FontStyle.Bold)
        boardTimeBusButton.Size = New Size(150, 40)
        boardTimeBusButton.BackColor = Color.FromArgb(0, 100, 200) ' Dark blue for the button
        boardTimeBusButton.ForeColor = Color.White
        boardTimeBusButton.FlatStyle = FlatStyle.Flat
        boardTimeBusButton.FlatAppearance.BorderSize = 0
        Dim buttonYPosition As Integer = (tourPanel.Height - boardTimeBusButton.Height) / 2

        ' Add "Delete Tour" button
        Dim deleteTourButton As New Button()
        deleteTourButton.Text = "Delete Tour"
        deleteTourButton.Font = New Font(deleteTourButton.Font.FontFamily, 12, FontStyle.Bold)
        deleteTourButton.Size = New Size(150, 40)
        deleteTourButton.BackColor = Color.FromArgb(200, 0, 0) ' Dark red for the button
        deleteTourButton.ForeColor = Color.White
        deleteTourButton.FlatStyle = FlatStyle.Flat
        deleteTourButton.FlatAppearance.BorderSize = 0

        ' Position the "Delete Tour" button to the right
        deleteTourButton.Location = New Point(tourPanel.Width - deleteTourButton.Width - 10, buttonYPosition) ' Adjusted position

        ' Position the "Board TimeBus" button to the left of the "Delete Tour" button
        boardTimeBusButton.Location = New Point(deleteTourButton.Left - boardTimeBusButton.Width - 15, buttonYPosition) ' Adjusted position

        AddHandler boardTimeBusButton.Click, Sub()
                                                 Dim details As String = $"{tourName}{vbCrLf}{vbCrLf}"
                                                 For Each item In curatedItinerary
                                                     details &= $"Location: {item.Item1}{vbCrLf}" &
                                                    $"Date: {item.Item2.ToShortDateString()}{vbCrLf}" &
                                                    $"Time: {item.Item3} hour(s){vbCrLf}{vbCrLf}"
                                                 Next
                                                 Dim pilot = New Pilot(curatedItinerary)
                                                 pilot.Show()
                                             End Sub
        tourPanel.Controls.Add(boardTimeBusButton)

        AddHandler deleteTourButton.Click, Sub()
                                               newToursPanel.Controls.Remove(tourPanel) ' Remove the tour panel
                                               UpdateNewToursPanelHeight() ' Update the scrollable area
                                           End Sub
        tourPanel.Controls.Add(deleteTourButton)



        ' Position the panel dynamically in the New Tours Panel
        Dim totalHeight As Integer = 10
        For Each control As Control In newToursPanel.Controls
            totalHeight += control.Height + 10
        Next
        tourPanel.Location = New Point(5, totalHeight)

        newToursPanel.Controls.Add(tourPanel)

        ' Adjust the scrollable area
        UpdateNewToursPanelHeight()
    End Sub

    Private Sub UpdateNewToursPanelHeight()
        ' Adjust the scrollable content height for New Tours Panel
        Dim cumulativeHeight As Integer = 0
        For Each control As Control In newToursPanel.Controls
            cumulativeHeight += control.Height + 10 ' Add spacing
        Next
        newToursPanel.AutoScrollMinSize = New Size(newToursPanel.Width, cumulativeHeight)
    End Sub

    Private Sub AddDefaultsButton_Click(sender As Object, e As EventArgs)
        ' Add preset locations directly to the visitor instance
        AddLocation("bali", "Click To Add" & vbCr & "Bali: A popular island destination known for its forested volcanic mountains, beaches, and coral reefs.")
        AddLocation("agra", "Click To Add" & vbCrLf & "Agra: Home to the iconic Taj Mahal, a symbol of love and one of the Seven Wonders of the World.")
        AddLocation("toronto", "Click To Add" & vbCrLf & "Toronto: A vibrant Canadian city known for its CN Tower and multicultural neighborhoods.")
        AddLocation("hamburg", "Click To Add" & vbCrLf & "Hamburg: A major port city in northern Germany, known for its maritime heritage and modern architecture.")
        AddLocation("istanbul", "Click To Add" & vbCrLf & "Istanbul: A historic city straddling Europe and Asia, known for its cultural sites and the Bosphorus Strait.")
        AddLocation("lisbon", "Click To Add" & vbCrLf & "Lisbon: The hilly, coastal capital city of Portugal known for its colorful buildings, Fado music, and tram rides.")
        AddLocation("meribel", "Click To Add" & vbCrLf & "Meribel: A charming ski resort in the French Alps, popular for winter sports and alpine scenery.")
        AddLocation("newyork", "Click To Add" & vbCrLf & "New York: A bustling metropolitan city known for Times Square, Central Park, and the Statue of Liberty.")
        AddLocation("paris", "Click To Add" & vbCrLf & "Paris: The capital of France, famous for its art, fashion, the Eiffel Tower, and café culture.")

        ' Add events for each location
        AddEventsForLocations()
    End Sub

    Private Sub AddEventsForLocations()
        ' Initialize events for each location
        ' Each event is a Tuple of event name and event date

        ' Bali events
        locationEvents("bali") = New List(Of Tuple(Of String, Date))()
        locationEvents("bali").Add(Tuple.Create("Bali Arts Festival", New Date(1738, 6, 13))) ' Past event
        locationEvents("bali").Add(Tuple.Create("Bali Kite Festival", New Date(2060, 7, 15)))  ' Future event

        ' Agra events
        locationEvents("agra") = New List(Of Tuple(Of String, Date))()
        locationEvents("agra").Add(Tuple.Create("Taj Mahotsav", New Date(1640, 2, 18)))       ' Past event
        locationEvents("agra").Add(Tuple.Create("Agra Cultural Fair", New Date(2100, 11, 5))) ' Future event

        ' Toronto events
        locationEvents("toronto") = New List(Of Tuple(Of String, Date))()
        locationEvents("toronto").Add(Tuple.Create("Toronto Film Festival", New Date(1675, 9, 10))) ' Past event
        locationEvents("toronto").Add(Tuple.Create("Winter Carnival", New Date(2085, 1, 20)))        ' Future event

        ' Hamburg events
        locationEvents("hamburg") = New List(Of Tuple(Of String, Date))()
        locationEvents("hamburg").Add(Tuple.Create("Hamburg Port Anniversary", New Date(1550, 5, 7))) ' Past event
        locationEvents("hamburg").Add(Tuple.Create("Christmas Market", New Date(2140, 12, 1)))         ' Future event

        ' Istanbul events
        locationEvents("istanbul") = New List(Of Tuple(Of String, Date))()
        locationEvents("istanbul").Add(Tuple.Create("Istanbul Tulip Festival", New Date(1480, 4, 10))) ' Past event
        locationEvents("istanbul").Add(Tuple.Create("Istanbul Music Festival", New Date(2125, 6, 1)))  ' Future event

        ' Lisbon events
        locationEvents("lisbon") = New List(Of Tuple(Of String, Date))()
        locationEvents("lisbon").Add(Tuple.Create("Lisbon Marathon", New Date(1600, 10, 20)))  ' Past event
        locationEvents("lisbon").Add(Tuple.Create("Sardine Festival", New Date(2098, 6, 12)))  ' Future event

        ' Meribel events
        locationEvents("meribel") = New List(Of Tuple(Of String, Date))()
        locationEvents("meribel").Add(Tuple.Create("Ski Season Opening", New Date(1620, 12, 1)))  ' Past event
        locationEvents("meribel").Add(Tuple.Create("Winter Music Festival", New Date(2084, 3, 5))) ' Future event

        ' New York events
        locationEvents("newyork") = New List(Of Tuple(Of String, Date))()
        locationEvents("newyork").Add(Tuple.Create("New Year's Eve Ball Drop", New Date(1580, 12, 31))) ' Past event
        locationEvents("newyork").Add(Tuple.Create("Thanksgiving Parade", New Date(2107, 11, 28)))      ' Future event

        ' Paris events
        locationEvents("paris") = New List(Of Tuple(Of String, Date))()
        locationEvents("paris").Add(Tuple.Create("Bastille Day", New Date(1590, 7, 14))) ' Past event
        locationEvents("paris").Add(Tuple.Create("Paris Fashion Week", New Date(2088, 9, 25))) ' Future event
    End Sub

    Public Sub AddDefaultsExternal()
        AddDefaultsButton_Click(addDefaultsButton, EventArgs.Empty)
    End Sub

    Public Sub AddLocationExternal()
        AddLocationButton_Click(addLocationButton, EventArgs.Empty)
    End Sub

    Public Sub RemoveLocationExternal()
        RemoveLocationButton_Click(removeLocationButton, EventArgs.Empty)
    End Sub

    Private Sub AddLocationButton_Click(sender As Object, e As EventArgs)
        ' Open a dialog to add a new location
        Dim locationName As String = InputBox("Enter the name of the new location:")
        If String.IsNullOrWhiteSpace(locationName) Then Return

        Dim locationDescription As String = InputBox("Enter the description for " & locationName & ":")
        If String.IsNullOrWhiteSpace(locationDescription) Then Return

        AddLocation(locationName, locationDescription)
    End Sub

    Private Sub RemoveLocationButton_Click(sender As Object, e As EventArgs)
        ' Open a dialog to remove a location
        Dim locationName As String = InputBox("Enter the name of the location to remove:")
        If String.IsNullOrWhiteSpace(locationName) Then Return

        If adventureDescriptions.ContainsKey(locationName) Then
            adventureDescriptions.Remove(locationName)
            imageNames.Remove(locationName)
            Dim pictureBoxToRemove = pictureBoxes.FirstOrDefault(Function(pb) pb.Tag.ToString() = locationName)
            If pictureBoxToRemove IsNot Nothing Then
                scrollablePanel.Controls.Remove(pictureBoxToRemove)
                pictureBoxes.Remove(pictureBoxToRemove)
                RearrangePictureBoxes()
                MessageBox.Show($"Location '{locationName}' has been removed.")
            End If
        Else
            MessageBox.Show($"Location '{locationName}' does not exist.")
        End If
    End Sub

    Private Sub PictureBox_MouseEnter(sender As Object, e As EventArgs)
        Dim hoveredPictureBox As PictureBox = DirectCast(sender, PictureBox)
        Dim locationName As String = hoveredPictureBox.Tag.ToString()

        ' Construct the news image name using the locationName with "_news" suffix
        Dim newsImageName As String = locationName & "_news"

        ' Display the description panel as a tooltip near the cursor and enlarge the news image in the description panel
        If adventureDescriptions.ContainsKey(locationName) Then
            descriptionLabel.Text = adventureDescriptions(locationName)

            ' Set the news image in the description panel's PictureBox
            Dim newsPictureBox As PictureBox = DirectCast(descriptionPanel.Tag, PictureBox)
            Dim newsImage As Image = CType(My.Resources.ResourceManager.GetObject(newsImageName), Image)

            If newsImage IsNot Nothing Then
                newsPictureBox.Image = newsImage
                newsPictureBox.Size = New Size(newsPictureBox.Width, newsPictureBox.Height) ' Fit the news image perfectly
                newsPictureBox.BringToFront()
            Else
                newsPictureBox.Image = Nothing
            End If

            descriptionPanel.Visible = True
            descriptionPanel.BringToFront()
            descriptionPanel.Location = New Point(350, 10)
        End If
    End Sub

    Private Sub PictureBox_MouseMove(sender As Object, e As MouseEventArgs)
        ' Update the description panel position to follow the cursor
        Dim hoveredPictureBox As PictureBox = DirectCast(sender, PictureBox)
        Dim cursorPosition As Point = Me.PointToClient(Cursor.Position)
        descriptionPanel.Location = New Point(350, 10)
    End Sub

    Private Sub PictureBox_MouseLeave(sender As Object, e As EventArgs)
        ' Hide the description panel and reset the PictureBox size when the mouse leaves the PictureBox
        Dim hoveredPictureBox As PictureBox = DirectCast(sender, PictureBox)
        descriptionPanel.Visible = False
        hoveredPictureBox.Size = New Size(140, 200) ' Reset to original size
        Dim newsPictureBox As PictureBox = DirectCast(descriptionPanel.Tag, PictureBox)
        newsPictureBox.Size = New Size(300, 150) ' Reset news image to original size
    End Sub

    Private Sub PictureBox_Click(sender As Object, e As EventArgs)
        Dim clickedPictureBox As PictureBox = DirectCast(sender, PictureBox)
        Dim locationName As String = clickedPictureBox.Tag.ToString()

        ' Create a popup form to select date and time
        Dim popupForm As New Form()
        popupForm.Text = "Select Date and Time or Event"
        popupForm.Size = New Size(400, 550)
        popupForm.StartPosition = FormStartPosition.CenterParent
        popupForm.BackColor = Color.FromArgb(10, 10, 30) ' Darker background with a futuristic feel
        popupForm.ForeColor = Color.White

        ' Add a panel for content
        Dim contentPanel As New Panel()
        contentPanel.Dock = DockStyle.Fill
        contentPanel.BackColor = Color.Transparent
        popupForm.Controls.Add(contentPanel)

        ' Add label to indicate past or future
        Dim dateLabel As New Label()
        dateLabel.Text = "Choose Date or Event"
        dateLabel.Font = New Font("Segoe UI", 14, FontStyle.Bold)
        dateLabel.ForeColor = Color.White
        dateLabel.BackColor = Color.Transparent
        dateLabel.Location = New Point(100, 20) ' Position above the radio buttons
        dateLabel.AutoSize = True
        contentPanel.Controls.Add(dateLabel)

        ' Add radio buttons for selecting date or event
        Dim radioSelectDate As New RadioButton()
        radioSelectDate.Text = "Select Date"
        radioSelectDate.Font = New Font("Segoe UI", 12, FontStyle.Regular)
        radioSelectDate.ForeColor = Color.White
        radioSelectDate.BackColor = Color.Transparent
        radioSelectDate.Location = New Point(50, 60)
        radioSelectDate.AutoSize = True
        radioSelectDate.Checked = True ' Default selection

        Dim radioSelectEvent As New RadioButton()
        radioSelectEvent.Text = "Select Event"
        radioSelectEvent.Font = New Font("Segoe UI", 12, FontStyle.Regular)
        radioSelectEvent.ForeColor = Color.White
        radioSelectEvent.BackColor = Color.Transparent
        radioSelectEvent.Location = New Point(200, 60)
        radioSelectEvent.AutoSize = True

        contentPanel.Controls.Add(radioSelectDate)
        contentPanel.Controls.Add(radioSelectEvent)

        ' Show a calendar to choose the date
        Dim calendar As New MonthCalendar()
        calendar.Location = New Point(90, 100)
        calendar.MaxSelectionCount = 1 ' Restrict selection to a single date
        calendar.TitleBackColor = Color.FromArgb(20, 20, 40) ' Darker title background for a modern touch
        calendar.TitleForeColor = Color.Cyan
        calendar.TrailingForeColor = Color.LightGray
        calendar.BackColor = Color.FromArgb(30, 30, 50)
        calendar.ForeColor = Color.White
        calendar.Font = New Font("Segoe UI", 12, FontStyle.Bold)
        calendar.Size = New Size(350, 200) ' Larger size for better user interaction
        calendar.Visible = True ' Initially visible
        ' Set the date to January 1, 2000
        calendar.SelectionStart = New DateTime(2000, 1, 1)
        calendar.SelectionEnd = New DateTime(2000, 1, 1)
        ' Add the calendar to the content panel
        contentPanel.Controls.Add(calendar)


        ' Panel to display events
        Dim eventsPanel As New Panel()
        eventsPanel.Location = New Point(50, 100)
        eventsPanel.Size = New Size(300, 200)
        eventsPanel.BackColor = Color.Transparent
        eventsPanel.Visible = False
        contentPanel.Controls.Add(eventsPanel)

        ' Event handler to toggle visibility
        AddHandler radioSelectDate.CheckedChanged, Sub(senderObj, eventArgs)
                                                       calendar.Visible = radioSelectDate.Checked
                                                       eventsPanel.Visible = Not radioSelectDate.Checked
                                                   End Sub

        ' If 'Select Event' is chosen, display events for the location
        If locationEvents.ContainsKey(locationName) Then
            Dim events = locationEvents(locationName)
            Dim yOffset As Integer = 0
            Dim eventRadioButtons As New List(Of RadioButton)()

            For Each evt In events
                Dim eventRadio As New RadioButton()
                eventRadio.Text = $"{evt.Item1} ({evt.Item2.ToShortDateString()})"
                eventRadio.Font = New Font("Segoe UI", 10, FontStyle.Regular)
                eventRadio.ForeColor = Color.White
                eventRadio.BackColor = Color.Transparent
                eventRadio.Location = New Point(10, yOffset)
                eventRadio.AutoSize = True
                eventsPanel.Controls.Add(eventRadio)
                eventRadioButtons.Add(eventRadio)
                yOffset += 30
            Next

            ' Ensure at least one event is selected by default if available
            If eventRadioButtons.Count > 0 Then
                eventRadioButtons(0).Checked = True
            End If
        End If

        ' Create a dropdown list for hours
        Dim hoursDropdown As New ComboBox()
        hoursDropdown.Location = New Point(50, 360)
        hoursDropdown.Size = New Size(300, 40)
        hoursDropdown.BackColor = Color.FromArgb(40, 40, 60) ' Dark dropdown with a spacey touch
        hoursDropdown.ForeColor = Color.White
        hoursDropdown.Font = New Font("Segoe UI", 12, FontStyle.Bold)
        For i As Integer = 1 To remainingHours
            hoursDropdown.Items.Add(i.ToString() & " hour(s)")
        Next
        hoursDropdown.SelectedIndex = 0
        contentPanel.Controls.Add(hoursDropdown)

        ' Create a confirm button
        Dim confirmButton As New Button()
        confirmButton.Text = "Confirm"
        confirmButton.Location = New Point(50, 405)
        confirmButton.Size = New Size(300, 50)
        confirmButton.BackColor = Color.FromArgb(30, 30, 60) ' Dark blue with a sci-fi feel
        confirmButton.ForeColor = Color.Cyan
        confirmButton.FlatStyle = FlatStyle.Flat
        confirmButton.FlatAppearance.BorderSize = 0
        confirmButton.Font = New Font("Segoe UI", 14, FontStyle.Bold)
        confirmButton.Region = New Region(New Rectangle(0, 0, confirmButton.Width, confirmButton.Height))
        AddHandler confirmButton.Click, Sub()
                                            If radioSelectDate.Checked Then
                                                Dim selectedDate As Date = calendar.SelectionRange.Start
                                                Dim selectedHours As Integer = Integer.Parse(hoursDropdown.SelectedItem.ToString().Split(" "c)(0))
                                                AddToItinerary(locationName, selectedDate, selectedHours)
                                            ElseIf radioSelectEvent.Checked Then
                                                ' Handle event selection logic here
                                                Dim selectedEventRadio = eventsPanel.Controls.OfType(Of RadioButton)().FirstOrDefault(Function(rb) rb.Checked)
                                                If selectedEventRadio IsNot Nothing Then
                                                    Dim eventText As String = selectedEventRadio.Text
                                                    ' Extract the event name and date
                                                    Dim eventNameAndDate = eventText.Split("("c)
                                                    Dim eventName As String = eventNameAndDate(0).Trim()
                                                    Dim eventDate As Date = Date.Parse(eventNameAndDate(1).Replace(")", "").Trim())
                                                    Dim selectedHours As Integer = Integer.Parse(hoursDropdown.SelectedItem.ToString().Split(" "c)(0))
                                                    AddToItinerary(locationName & " - " & eventName, eventDate, selectedHours)
                                                Else
                                                    MessageBox.Show("Please select an event.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                    Return
                                                End If
                                            End If
                                            popupForm.Close()
                                        End Sub
        contentPanel.Controls.Add(confirmButton)

        ' Show the popup form
        popupForm.ShowDialog()
    End Sub



    Private Sub AddToItinerary(locationName As String, selectedDate As Date, selectedHours As Integer)
        ' Add the selected location, date, and time to the itinerary
        itinerary.Add(Tuple.Create(locationName, selectedDate, selectedHours))
        remainingHours -= selectedHours
        UpdateItineraryPanel()
        UpdateNotificationLabel()

        ' Disable all PictureBoxes if no remaining hours
        If remainingHours <= 0 Then
            scrollablePanel.Enabled = False
            ShowNotification("All hours are planned. Remove an item to add more.")
        End If
    End Sub

    Private Sub UpdateItineraryPanel()
        ' Clear existing itinerary controls
        itineraryPanel.Controls.Clear()

        ' Scaling factor from 1280x720 to 1920x1080
        Dim scaleFactor As Double = 1.5

        ' Add each itinerary item to the panel with increased sizes
        For index As Integer = 0 To itinerary.Count - 1
            Dim item = itinerary(index)

            ' Extract the location name
            Dim locationName As String = item.Item1.Split("-"c)(0).Trim()

            ' Add the location picture (larger version)
            Dim smallPictureBox As New PictureBox()
            smallPictureBox.Size = New Size(CInt(100 * scaleFactor), CInt(100 * scaleFactor))
            smallPictureBox.Location = New Point(CInt(index * 160 * scaleFactor + 10 * scaleFactor), CInt(10 * scaleFactor))
            smallPictureBox.Image = CType(My.Resources.ResourceManager.GetObject(locationName), Image)
            smallPictureBox.SizeMode = PictureBoxSizeMode.Zoom
            itineraryPanel.Controls.Add(smallPictureBox)

            ' Add the itinerary details
            Dim itineraryLabel As New Label()
            itineraryLabel.Text = $"Location: {item.Item1}
Date: {item.Item2.ToShortDateString()}
Time: {item.Item3} hour(s)"
            itineraryLabel.Size = New Size(CInt(150 * scaleFactor), CInt(60 * scaleFactor))
            itineraryLabel.Location = New Point(CInt(index * 160 * scaleFactor + 10 * scaleFactor), CInt(120 * scaleFactor))
            itineraryLabel.ForeColor = Color.White
            itineraryPanel.Controls.Add(itineraryLabel)

            ' Create a dropdown to allow changing the hours
            Dim hoursDropdown As New ComboBox()
            hoursDropdown.Size = New Size(CInt(65 * scaleFactor), CInt(50 * scaleFactor))
            hoursDropdown.Location = New Point(CInt(index * 160 * scaleFactor + 10 * scaleFactor), CInt(185 * scaleFactor))
            hoursDropdown.BackColor = Color.FromArgb(40, 40, 60) ' Dark dropdown for consistency
            hoursDropdown.ForeColor = Color.White
            For i As Integer = 1 To remainingHours + itinerary(index).Item3
                hoursDropdown.Items.Add(i.ToString() & " hour(s)")
            Next
            hoursDropdown.SelectedItem = $"{item.Item3} hour(s)"
            Dim currentIndex As Integer = index
            AddHandler hoursDropdown.SelectedIndexChanged, Sub()
                                                               Dim newHours As Integer = Integer.Parse(hoursDropdown.SelectedItem.ToString().Split(" "c)(0))
                                                               If newHours <= 6 Then
                                                                   remainingHours += itinerary(currentIndex).Item3 - newHours
                                                                   itinerary(currentIndex) = Tuple.Create(itinerary(currentIndex).Item1, itinerary(currentIndex).Item2, newHours)
                                                                   UpdateItineraryPanel()
                                                                   UpdateNotificationLabel()

                                                                   ' Re-enable scrollable panel if there are remaining hours
                                                                   If remainingHours > 0 Then
                                                                       scrollablePanel.Enabled = True
                                                                       HideNotification()
                                                                   End If
                                                                   If remainingHours = 0 Then
                                                                       scrollablePanel.Enabled = False
                                                                   End If
                                                               Else
                                                                   MessageBox.Show("Cannot plan more than 6 hours.")
                                                               End If
                                                           End Sub
            itineraryPanel.Controls.Add(hoursDropdown)

            ' Create a delete button for each itinerary item
            Dim deleteButton As New Button()
            deleteButton.Text = "X"
            deleteButton.Font = New Font(deleteButton.Font.FontFamily, CSng(deleteButton.Font.Size * scaleFactor), FontStyle.Bold)
            deleteButton.Size = New Size(CInt(40 * scaleFactor), CInt(40 * scaleFactor))
            deleteButton.Location = New Point(CInt(index * 160 * scaleFactor + 80 * scaleFactor), CInt(180 * scaleFactor))
            deleteButton.BackColor = Color.FromArgb(255, 112, 112) ' Dark red for a warning feel
            deleteButton.ForeColor = Color.White
            deleteButton.FlatStyle = FlatStyle.Flat
            deleteButton.FlatAppearance.BorderSize = 0
            deleteButton.Region = New Region(New Rectangle(0, 0, deleteButton.Width, deleteButton.Height))
            AddHandler deleteButton.Click, Sub()
                                               remainingHours += itinerary(currentIndex).Item3
                                               itinerary.RemoveAt(currentIndex)
                                               UpdateItineraryPanel()
                                               UpdateNotificationLabel()

                                               ' Re-enable scrollable panel if there are remaining hours
                                               If remainingHours > 0 Then
                                                   scrollablePanel.Enabled = True
                                                   HideNotification()
                                               End If
                                           End Sub
            itineraryPanel.Controls.Add(deleteButton)

            ' Create a "Move Up" button for each itinerary item
            If index > 0 Then
                Dim moveUpButton As New Button()
                moveUpButton.Text = "<"
                moveUpButton.Font = New Font(moveUpButton.Font.FontFamily, CSng(14 * scaleFactor), FontStyle.Bold)
                moveUpButton.Size = New Size(CInt(40 * scaleFactor), CInt(40 * scaleFactor))
                moveUpButton.Location = New Point(CInt(index * 160 * scaleFactor + 10 * scaleFactor), CInt(225 * scaleFactor))
                moveUpButton.BackColor = Color.FromArgb(30, 30, 60) ' Matching dark blue with space theme
                moveUpButton.ForeColor = Color.Cyan
                moveUpButton.FlatStyle = FlatStyle.Flat
                moveUpButton.FlatAppearance.BorderSize = 0
                moveUpButton.Region = New Region(New Rectangle(0, 0, moveUpButton.Width, moveUpButton.Height))
                AddHandler moveUpButton.Click, Sub()
                                                   ' Move the item up in the list
                                                   Dim tempItem = itinerary(currentIndex)
                                                   itinerary.RemoveAt(currentIndex)
                                                   itinerary.Insert(currentIndex - 1, tempItem)
                                                   UpdateItineraryPanel() ' Re-render the panel
                                               End Sub
                itineraryPanel.Controls.Add(moveUpButton)
            End If

            ' Create a "Move Down" button for each itinerary item
            If index < itinerary.Count - 1 Then
                Dim moveDownButton As New Button()
                moveDownButton.Text = ">"
                moveDownButton.Font = New Font(moveDownButton.Font.FontFamily, CSng(14 * scaleFactor), FontStyle.Bold)
                moveDownButton.TextAlign = ContentAlignment.MiddleCenter
                moveDownButton.Size = New Size(CInt(40 * scaleFactor), CInt(40 * scaleFactor))
                moveDownButton.Location = New Point(CInt(index * 160 * scaleFactor + 60 * scaleFactor), CInt(225 * scaleFactor))
                moveDownButton.BackColor = Color.FromArgb(30, 30, 60) ' Matching dark blue with space theme
                moveDownButton.ForeColor = Color.Cyan
                moveDownButton.FlatStyle = FlatStyle.Flat
                moveDownButton.FlatAppearance.BorderSize = 0
                moveDownButton.Region = New Region(New Rectangle(0, 0, moveDownButton.Width, moveDownButton.Height))
                AddHandler moveDownButton.Click, Sub()
                                                     ' Move the item down in the list
                                                     Dim tempItem = itinerary(currentIndex)
                                                     itinerary.RemoveAt(currentIndex)
                                                     itinerary.Insert(currentIndex + 1, tempItem)
                                                     UpdateItineraryPanel() ' Re-render the panel
                                                 End Sub
                itineraryPanel.Controls.Add(moveDownButton)
            End If
        Next

        ' Add the confirm button to the itinerary panel if there is at least one item
        If itinerary.Count > 0 Then
            Dim confirmButton As New Button()
            confirmButton.Text = "Confirm Tour"
            confirmButton.Font = New Font(confirmButton.Font.FontFamily, CSng(14 * scaleFactor), FontStyle.Bold)
            confirmButton.Size = New Size(CInt(200 * scaleFactor), CInt(50 * scaleFactor))
            confirmButton.Location = New Point(CInt((itineraryPanel.Width - confirmButton.Width) / 2 - 110 * scaleFactor), CInt(270 * scaleFactor))
            confirmButton.BackColor = Color.FromArgb(0, 100, 0) ' Dark green with a sci-fi feel
            confirmButton.ForeColor = Color.GreenYellow
            confirmButton.FlatStyle = FlatStyle.Flat
            confirmButton.FlatAppearance.BorderSize = 0
            confirmButton.Region = New Region(New Rectangle(0, 0, confirmButton.Width, confirmButton.Height))
            AddHandler confirmButton.Click, AddressOf ConfirmButton_Click
            itineraryPanel.Controls.Add(confirmButton)

            ' Add the cancel button beside the confirm button
            Dim cancelButton As New Button()
            cancelButton.Text = "Cancel Tour"
            cancelButton.Font = New Font(cancelButton.Font.FontFamily, CSng(14 * scaleFactor), FontStyle.Bold)
            cancelButton.Size = New Size(CInt(200 * scaleFactor), CInt(50 * scaleFactor))
            cancelButton.Location = New Point(CInt((itineraryPanel.Width - cancelButton.Width) / 2 + 110 * scaleFactor) + 10, CInt(270 * scaleFactor))
            cancelButton.BackColor = Color.FromArgb(100, 0, 0) ' Dark red for cancel action
            cancelButton.ForeColor = Color.LightPink
            cancelButton.FlatStyle = FlatStyle.Flat
            cancelButton.FlatAppearance.BorderSize = 0
            cancelButton.Region = New Region(New Rectangle(0, 0, cancelButton.Width, cancelButton.Height))
            AddHandler cancelButton.Click, AddressOf CancelButton_Click
            itineraryPanel.Controls.Add(cancelButton)
        End If
    End Sub

    Private Sub SortButton_Click(sender As Object, e As EventArgs)
        ' Sort the PictureBoxes alphabetically based on their tags
        pictureBoxes = pictureBoxes.OrderBy(Function(pb) pb.Tag.ToString()).ToList()
        scrollablePanel.Controls.Clear()
        For Each pb In pictureBoxes
            scrollablePanel.Controls.Add(pb)
        Next
        RearrangePictureBoxes()
    End Sub

    Private Sub RecommendButton_Click(sender As Object, e As EventArgs)
        ' Shuffle the PictureBoxes randomly
        pictureBoxes = pictureBoxes.OrderBy(Function(pb) Guid.NewGuid()).ToList()
        scrollablePanel.Controls.Clear()
        For Each pb In pictureBoxes
            scrollablePanel.Controls.Add(pb)
        Next
        RearrangePictureBoxes()
    End Sub

    Private Sub RearrangePictureBoxes()
        ' Define the grid parameters
        Dim rowCount As Integer = 5
        Dim columnCount As Integer = 3
        Dim pictureBoxSize As Size = New Size(140, 200)
        Dim spacing As Integer = 10

        ' Arrange PictureBoxes in a grid layout
        For index As Integer = 0 To scrollablePanel.Controls.Count - 1
            Dim row As Integer = index \ columnCount
            Dim col As Integer = index Mod columnCount

            Dim x As Integer = col * (pictureBoxSize.Width + spacing)
            Dim y As Integer = row * (pictureBoxSize.Height + spacing)

            Dim pictureBox As PictureBox = DirectCast(scrollablePanel.Controls(index), PictureBox)
            pictureBox.Location = New Point(x, y)
        Next
    End Sub

    ' Public method to add a new location from Form1 (admin form)
    Public Sub AddLocation(newLocationName As String, newLocationDescription As String)
        ' Check if the location already exists
        If adventureDescriptions.ContainsKey(newLocationName) Then
            MessageBox.Show("Location already exists!")
            Return
        End If

        ' Add the new location to the lists
        adventureDescriptions.Add(newLocationName, newLocationDescription)
        imageNames.Add(newLocationName)

        ' Create and configure a new PictureBox
        Dim newPictureBox As New PictureBox()
        newPictureBox.Size = New Size(140, 200)
        newPictureBox.BackColor = Color.FromArgb(10, 10, 30) ' Dark background for picture box
        newPictureBox.SizeMode = PictureBoxSizeMode.StretchImage
        newPictureBox.Image = CType(My.Resources.ResourceManager.GetObject(newLocationName), Image)
        newPictureBox.Tag = newLocationName

        ' Add event handlers for the PictureBox
        AddHandler newPictureBox.Click, AddressOf PictureBox_Click
        AddHandler newPictureBox.MouseEnter, AddressOf PictureBox_MouseEnter
        AddHandler newPictureBox.MouseMove, AddressOf PictureBox_MouseMove
        AddHandler newPictureBox.MouseLeave, AddressOf PictureBox_MouseLeave

        ' Add the PictureBox to the list and the panel
        pictureBoxes.Add(newPictureBox)
        scrollablePanel.Controls.Add(newPictureBox)

        ' RearrangePictureBoxes to keep layout consistent
        RearrangePictureBoxes()
    End Sub

    Private Sub ConfirmButton_Click(sender As Object, e As EventArgs)
        ' Logic for confirming the itinerary
        allItineraries.Add(New List(Of Tuple(Of String, Date, Integer))(itinerary))
        Dim confirmedItinerary As New List(Of Tuple(Of String, Date, Integer))(itinerary) ' Create copy of itinerary
        RaiseEvent ItineraryConfirmed(confirmedItinerary) ' Send copy information
        itinerary.Clear() ' Clear itinerary
        remainingHours = 6
        UpdateItineraryPanel()
        UpdateNotificationLabel()
        scrollablePanel.Enabled = True
        ShowToolTipNotification("Itinerary confirmed! You can now create a new tour.")

        ' Increment the confirmed tour count
        confirmedTourCount += 1

        ' Create a new panel for the tour
        Dim tourPanel As New Panel()
        tourPanel.Width = newToursPanel.Width - 20 ' Adjust width dynamically
        tourPanel.BackColor = Color.MediumSlateBlue ' Set background color
        tourPanel.BorderStyle = BorderStyle.FixedSingle

        ' Create the label for the tour details
        Dim tourLabel As New Label()
        tourLabel.Text = $"Tour {confirmedTourCount} Confirmed:"
        tourLabel.Font = New Font("Arial", 12, FontStyle.Bold) ' Bold subtitle
        tourLabel.AutoSize = True
        tourLabel.ForeColor = Color.LightPink
        tourLabel.BackColor = Color.Transparent
        tourLabel.Location = New Point(5, 5) ' Place the label at the top-left corner of the panel
        tourPanel.Controls.Add(tourLabel)

        ' Add details for each location with a slight gap
        Dim currentYPosition As Integer = tourLabel.Bottom + 5 ' Start below the title
        For Each item In confirmedItinerary
            Dim locationLabel As New Label()
            locationLabel.Text = $"{vbCrLf} Location: {item.Item1}{vbCrLf}" &
                             $"Date: {item.Item2.ToShortDateString()}{vbCrLf}" &
                             $"Time: {item.Item3} hour(s)"
            locationLabel.AutoSize = True
            locationLabel.Font = New Font("Arial", 10, FontStyle.Bold) ' Bold labels for "Location", "Date", "Time"
            locationLabel.ForeColor = Color.LightPink
            locationLabel.Location = New Point(5, currentYPosition)
            tourPanel.Controls.Add(locationLabel)
            currentYPosition = locationLabel.Bottom + 5 ' Update position for the next label
        Next

        ' Dynamically set the panel height based on content
        Dim totalContentHeight As Integer = currentYPosition + 10 ' Add padding below labels
        tourPanel.Height = Math.Max(80, totalContentHeight) ' Ensure a minimum height of 80

        ' Create the "Board TimeBus" button
        Dim boardTimeBusButton As New Button()
        boardTimeBusButton.Text = "Board TimeBus"
        boardTimeBusButton.Font = New Font(boardTimeBusButton.Font.FontFamily, 12, FontStyle.Bold)
        boardTimeBusButton.Size = New Size(150, 40)
        boardTimeBusButton.BackColor = Color.FromArgb(0, 100, 200) ' Dark blue for the button
        boardTimeBusButton.ForeColor = Color.White
        boardTimeBusButton.FlatStyle = FlatStyle.Flat
        boardTimeBusButton.FlatAppearance.BorderSize = 0
        Dim buttonYPosition As Integer = (tourPanel.Height - boardTimeBusButton.Height) / 2
        boardTimeBusButton.Location = New Point(tourPanel.Width - boardTimeBusButton.Width - 170, buttonYPosition) ' Position adjusted

        ' AddHandler to display itinerary details
        AddHandler boardTimeBusButton.Click, Sub()
                                                 Dim itineraryDetails As String = $"Pilot Information for Tour {confirmedTourCount}:{vbCrLf}"
                                                 For Each item In confirmedItinerary
                                                     itineraryDetails &= $"Location: {item.Item1}{vbCrLf}" &
                                                                 $"Date: {item.Item2.ToShortDateString()}{vbCrLf}" &
                                                                 $"Time: {item.Item3} hour(s){vbCrLf}{vbCrLf}"
                                                 Next
                                                 Dim pilot = New Pilot(confirmedItinerary)
                                                 pilot.Show()
                                             End Sub

        tourPanel.Controls.Add(boardTimeBusButton)


        ' Create the "Delete Tour" button
        Dim deleteTourButton As New Button()
        deleteTourButton.Text = "Delete Tour"
        deleteTourButton.Font = New Font(deleteTourButton.Font.FontFamily, 12, FontStyle.Bold)
        deleteTourButton.Size = New Size(150, 40)
        deleteTourButton.BackColor = Color.FromArgb(200, 0, 0) ' Dark red for the button
        deleteTourButton.ForeColor = Color.White
        deleteTourButton.FlatStyle = FlatStyle.Flat
        deleteTourButton.FlatAppearance.BorderSize = 0
        deleteTourButton.Location = New Point(tourPanel.Width - deleteTourButton.Width - 10, buttonYPosition) ' Position beside "Board TimeBus"
        AddHandler deleteTourButton.Click, Sub()
                                               newToursPanel.Controls.Remove(tourPanel) ' Remove the panel
                                               UpdateTourPanelHeight() ' Adjust scrollable panel height
                                           End Sub
        tourPanel.Controls.Add(deleteTourButton)

        ' Position the panel dynamically in newToursPanel
        Dim totalPanelHeight As Integer = 0
        For Each control As Control In newToursPanel.Controls
            totalPanelHeight += control.Height + 10 ' Add spacing between panels
        Next
        tourPanel.Location = New Point(5, totalPanelHeight)

        ' Add the panel to the New Tours Panel
        newToursPanel.Controls.Add(tourPanel)

        ' Adjust the scrollable content height to accommodate all panels
        Dim cumulativeHeight As Integer = 0
        For Each control As Control In newToursPanel.Controls
            cumulativeHeight += control.Height + 10 ' Add spacing
        Next
        newToursPanel.AutoScrollMinSize = New Size(newToursPanel.Width, cumulativeHeight)

        ' Refresh the itinerary panel
        Me.Controls.Remove(itineraryPanel)
        InitializeItineraryPanel()
    End Sub

    Private Sub UpdateTourPanelHeight()
        ' Adjust the scrollable content height to accommodate all panels
        Dim cumulativeHeight As Integer = 0
        For Each control As Control In newToursPanel.Controls
            cumulativeHeight += control.Height + 10 ' Add spacing
        Next
        newToursPanel.AutoScrollMinSize = New Size(newToursPanel.Width, cumulativeHeight)
    End Sub



    Private Sub CancelButton_Click(sender As Object, e As EventArgs)
        ' Logic for canceling the itinerary
        itinerary.Clear() ' Clear the itinerary
        remainingHours = 6
        UpdateItineraryPanel()
        UpdateNotificationLabel()
        scrollablePanel.Enabled = True
        ShowToolTipNotification("Itinerary has been reset.")
        Me.Controls.Remove(itineraryPanel)
        InitializeItineraryPanel()
    End Sub

    Private Sub ShowNotification(message As String)
        ' Show the notification label with a message
        notificationLabel.Text = message
        notificationLabel.Visible = True
    End Sub

    Private Sub HideNotification()
        ' Hide the notification label
        notificationLabel.Visible = False
    End Sub

    Private Sub UpdateNotificationLabel()
        ' Update the notification label to show hours planned and remaining
        notificationLabel.Text = $"Total Hours Planned: {6 - remainingHours}, Hours Remaining: {remainingHours}"
        notificationLabel.Visible = True
    End Sub

    Private Sub ShowToolTipNotification(message As String)
        ' Display a tool tip notification at the top of the page
        Dim toolTipLabel As New Label()
        toolTipLabel.Text = message
        toolTipLabel.Size = New Size(Me.Width, 30)
        toolTipLabel.Font = New Font("Arial", 10, FontStyle.Bold)
        toolTipLabel.ForeColor = Color.White
        toolTipLabel.BackColor = Color.FromArgb(50, 20, 40) ' Dark background for tooltip

        toolTipLabel.TextAlign = ContentAlignment.MiddleCenter
        toolTipLabel.Location = New Point(0, 0)
        toolTipLabel.BringToFront()
        Me.Controls.Add(toolTipLabel)
        toolTipLabel.BringToFront()

        ' Set a timer to remove the tool tip after a few seconds
        Dim timer As New Timer()
        timer.Interval = 3000 ' Display for 3 seconds
        AddHandler timer.Tick, Sub()
                                   Me.Controls.Remove(toolTipLabel)
                                   timer.Stop()
                               End Sub
        timer.Start()
    End Sub
End Class
