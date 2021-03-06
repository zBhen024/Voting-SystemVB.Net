﻿Public Class AdminHomeNotStarted

    Implements MainControl

    Dim VotersCount As Integer
    Dim PreviousElectionCount As Integer
    Dim PartiesCount As Integer
    Dim CandidatesCounts As Dictionary(Of Integer, Integer)

    'Create only one Instance for the whole Runtime
    Private Shared Instance As AdminHomeNotStarted
    Public Shared Function GetInstance() As AdminHomeNotStarted
        If IsNothing(Instance) Then
            Instance = New AdminHomeNotStarted
            Instance.DoubleBuffered = True
        End If
        Return Instance
    End Function

    Sub StartLoading()
        ButtonStartElection.Enabled = False
        LoadingPanel.Visible = True
        Guna2WinProgressIndicator1.Start()
    End Sub
    Sub StopLoading()
        ButtonStartElection.Enabled = True
        LoadingPanel.Visible = False
        Guna2WinProgressIndicator1.Stop()
    End Sub

    ''' <summary>
    ''' To Check if the Positions have enough candidates to start the election
    ''' </summary>
    ''' <returns>Returns True if Every Candidates has atleast</returns>
    ''' <remarks></remarks>
    Private Function CanStartElection() As Boolean
        For i = 1 To 6
            If CandidatesCounts(i) < 1 Then
                Alert.ShowAlert("There is still no candidate in " & Position.GetName(i) & " Position.", Alert.AlertType.Error)
                Return False
            ElseIf CandidatesCounts(i) = 1 Then
                Alert.ShowAlert("Warning, there is only 1 candidate in " & Position.GetName(i) & " Position.", Alert.AlertType.Warning)
            End If
        Next
        Return True
    End Function

    'Trigger when the control is loaden on Admin Panel
    Public Sub RefreshData() Implements MainControl.RefreshControl
        StartLoading()
        BackgroundWorkerFetchData.RunWorkerAsync()
    End Sub

    'Trigger when Start Button has click
    Private Sub ButtonStartElection_Click(sender As Object, e As EventArgs) Handles ButtonStartElection.Click
        If CanStartElection() Then
            StartElection.ShowPopup()
        End If
    End Sub

    'Background Workers - Fetching Data
    Private Sub BackgroundWorkerFetchData_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorkerFetchData.DoWork
        VotersCount = Student.CountAll()
        PreviousElectionCount = Election.CountAll() - 1
        PartiesCount = Party.CountAll()
        CandidatesCounts = Candidate.Count()
    End Sub
    Private Sub BackgroundWorkerFetchData_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorkerFetchData.RunWorkerCompleted
        StopLoading()
        LabelVotersCount.Text = VotersCount
        LabelParties.Text = PartiesCount
        LabelPreviousElection.Text = PreviousElectionCount
        LabelCandidates.Text = CandidatesCounts.Sum(Function(item As KeyValuePair(Of Integer, Integer))
                                                        Return item.Value
                                                    End Function)
    End Sub

    Private Sub ButtonImport_Click(sender As Object, e As EventArgs) Handles ButtonImport.Click
        UploadDatabase.ShowPopup()
    End Sub
End Class
