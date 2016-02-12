'MultipleObjectTrackingVB.sln
'Blob.vb

'Emgu CV 3.1.0

Option Explicit On      'require explicit declaration of variables, this is NOT Python !!
Option Strict On        'restrict implicit data type conversions to only widening conversions

Imports System.Math

Imports Emgu.CV                     '
Imports Emgu.CV.CvEnum              'Emgu Cv imports
Imports Emgu.CV.Structure           '
Imports Emgu.CV.UI                  '
Imports Emgu.CV.Util                '

'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Public Class Blob

    Public currentContour As New VectorOfPoint()

    Public currentBoundingRect As Rectangle

    Public centerPositions As New List(Of Point)

    Public dblCurrentDiagonalSize As Double
    Public dblCurrentAspectRatio As Double

    Public intCurrentRectArea As Integer

    Public blnTrackedCurrently As Boolean
    Public blnCurrentMatchFoundOrNewBlob As Boolean

    Public blnStillBeingTracked As Boolean

    Public intNumOfConsecutiveFramesWithoutAMatch As Integer

    Public predictedNextPosition As Point

    ' constructor '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Sub New(_contour As VectorOfPoint)

        currentContour = _contour

        currentBoundingRect = CvInvoke.BoundingRectangle(currentContour)
        
        Dim currentCenter As New Point()

        currentCenter.X = CInt(CDbl(currentBoundingRect.X + currentBoundingRect.X + currentBoundingRect.Width) / 2.0)
        currentCenter.Y = CInt(CDbl(currentBoundingRect.Y + currentBoundingRect.Y + currentBoundingRect.Height) / 2.0)

        centerPositions.Add(currentCenter)

        dblCurrentDiagonalSize = Math.Sqrt((currentBoundingRect.Width ^ 2) + (currentBoundingRect.Height ^ 2))

        dblCurrentAspectRatio = CDbl(currentBoundingRect.Width) / CDbl(currentBoundingRect.Height)

        intCurrentRectArea = currentBoundingRect.Width * currentBoundingRect.Height

        blnStillBeingTracked = True
        blnCurrentMatchFoundOrNewBlob = True

        intNumOfConsecutiveFramesWithoutAMatch = 0

    End Sub

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    Sub predictNextPosition()

        Dim numPositions As Integer = centerPositions.Count()

        If (numPositions = 1) Then

            predictedNextPosition.X = centerPositions.Last().X
            predictedNextPosition.Y = centerPositions.Last().Y

        ElseIf (numPositions = 2) Then

            Dim deltaX As Integer = centerPositions(1).X - centerPositions(0).X
            Dim deltaY As Integer = centerPositions(1).Y - centerPositions(0).Y

            predictedNextPosition.X = centerPositions.Last().X + deltaX
            predictedNextPosition.Y = centerPositions.Last().Y + deltaY

        ElseIf (numPositions = 3) Then

            Dim sumOfXChanges As Integer = ((centerPositions(2).X - centerPositions(1).X) * 2) + _
                                           ((centerPositions(1).X - centerPositions(0).X) * 1)

            Dim deltaX As Integer = CInt(Math.Round(CDbl(sumOfXChanges / 3.0)))

            Dim sumOfYChanges As Integer = ((centerPositions(2).Y - centerPositions(1).Y) * 2) + _
                                           ((centerPositions(1).Y - centerPositions(0).Y) * 1)

            Dim deltaY As Integer = CInt(Math.Round(CDbl(sumOfYChanges / 3.0)))

            predictedNextPosition.X = centerPositions.Last().X + deltaX
            predictedNextPosition.Y = centerPositions.Last().Y + deltaY
            
        ElseIf (numPositions = 4) Then

            Dim sumOfXChanges As Integer = ((centerPositions(3).X - centerPositions(2).X) * 3) + _
                                           ((centerPositions(2).X - centerPositions(1).X) * 2) + _
                                           ((centerPositions(1).X - centerPositions(0).X) * 1)

            Dim deltaX As Integer = CInt(Math.Round(CDbl(sumOfXChanges / 6.0)))

            Dim sumOfYChanges As Integer = ((centerPositions(3).Y - centerPositions(2).Y) * 3) + _
                                           ((centerPositions(2).Y - centerPositions(1).Y) * 2) + _
                                           ((centerPositions(1).Y - centerPositions(0).Y) * 1)

            Dim deltaY As Integer = CInt(Math.Round(CDbl(sumOfYChanges / 6.0)))

            predictedNextPosition.X = centerPositions.Last().X + deltaX
            predictedNextPosition.Y = centerPositions.Last().Y + deltaY
            
        ElseIf (numPositions >= 5) Then

            Dim sumOfXChanges As Integer = ((centerPositions(numPositions - 1).X - centerPositions(numPositions - 2).X) * 4) + _
                                           ((centerPositions(numPositions - 2).X - centerPositions(numPositions - 3).X) * 3) + _
                                           ((centerPositions(numPositions - 3).X - centerPositions(numPositions - 4).X) * 2) + _
                                           ((centerPositions(numPositions - 4).X - centerPositions(numPositions - 5).X) * 1)

            Dim deltaX As Integer = CInt(Math.Round(CDbl(sumOfXChanges / 10.0)))

            Dim sumOfYChanges As Integer = ((centerPositions(numPositions - 1).Y - centerPositions(numPositions - 2).Y) * 4) + _
                                           ((centerPositions(numPositions - 2).Y - centerPositions(numPositions - 3).Y) * 3) + _
                                           ((centerPositions(numPositions - 3).Y - centerPositions(numPositions - 4).Y) * 2) + _
                                           ((centerPositions(numPositions - 4).Y - centerPositions(numPositions - 5).Y) * 1)

            Dim deltaY As Integer = CInt(Math.Round(CDbl(sumOfYChanges / 10.0)))

            predictedNextPosition.X = centerPositions.Last().X + deltaX
            predictedNextPosition.Y = centerPositions.Last().Y + deltaY

        Else
            'should never get here
        End If
        
    End Sub
    
End Class



