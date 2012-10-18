using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

// 

namespace SoundEditor
{
    class WaveGraph
    {
        //Canvas to paint on
        private Canvas waveGraph;
        int canvasWidth = 756;
        int canvasHeight = 246;

        //Border Surrounding Canvas
        private Border waveBorder;
        
        //Amplitude Labels
        private TextBlock ampLabel1, ampLabel2;
        
        //seconds display
        private int numberOfSeconds;
        private int secondsDisplayY = 224;
        private int secondsDisplayX = 35;
        
        //Sound Data
        private float[] soundData;
        
        //Sample Rate
        private int sampleRate;

        //graphing coordinates
        private readonly int XSTART = 35;
        private readonly int YSTART = 100;
        private readonly int GRAPH_HEIGHT = 200;
        
        //Constant values
        private readonly int LINEHEIGHT = 10;
        private readonly int PIXEL_SECOND = 80;
        private readonly int HSCROLLSIZE = 15;

        double heightRatio;
        public WaveGraph(Canvas waveGraph, Border waveBorder, float[] soundData, int sampleRate)
        {
            float max;
            int i;
            this.waveGraph = waveGraph;
            this.waveBorder = waveBorder;
            this.soundData = soundData;
            this.sampleRate = sampleRate;

            max = soundData.Max(element => Math.Abs(element));
            createAmplitudeLabels(max);

            numberOfSeconds = (soundData.Length / sampleRate);
            heightRatio = GRAPH_HEIGHT / (max * 2);
            //DrawSecond(PIXEL_SECOND * 0, sampleRate * 0);
            for (i = 0; i < numberOfSeconds; i++)
            {
                createSecondLabel(i);
                DrawSecond(PIXEL_SECOND*i, sampleRate*i);
            }
            //create final second label
            createSecondLabel(i);

            //adjusts the canvas width based on number of seconds and space between end points
            int newCanvasWidth = (numberOfSeconds * PIXEL_SECOND) + (XSTART * 2);
            if (newCanvasWidth > canvasWidth)
            {
                waveBorder.Height += HSCROLLSIZE;
                Console.WriteLine(canvasHeight);
                waveGraph.Height = canvasHeight;
                extendCanvas(newCanvasWidth);
            }
            

            
            //DrawGraph();
        }

        private void createAmplitudeLabels(float max)
        {
            ampLabel1 = new TextBlock();
            ampLabel2 = new TextBlock();

            //label 1
            ampLabel1.HorizontalAlignment = HorizontalAlignment.Left;
            ampLabel1.VerticalAlignment = VerticalAlignment.Top;
            ampLabel1.Margin = new System.Windows.Thickness(10, (YSTART - GRAPH_HEIGHT/2), 0, 0);
            ampLabel1.Text = max.ToString();

            //label 2
            ampLabel2.HorizontalAlignment = HorizontalAlignment.Left;
            ampLabel2.VerticalAlignment = VerticalAlignment.Top;
            ampLabel2.Margin = new System.Windows.Thickness(10, (YSTART + GRAPH_HEIGHT/2 - LINEHEIGHT),0, 0);
            ampLabel2.Text = "-" + max.ToString();

            //add labels to canvas
            waveGraph.Children.Add(ampLabel1);
            waveGraph.Children.Add(ampLabel2);
        }

        public void createSecondLabel(int time)
        {
            TextBlock secondLabel = new TextBlock();
            secondLabel.HorizontalAlignment = HorizontalAlignment.Left;
            secondLabel.VerticalAlignment = VerticalAlignment.Top;
            secondLabel.Margin = new System.Windows.Thickness(secondsDisplayX, secondsDisplayY, 0, 0);
            secondLabel.Text = time.ToString();

            secondsDisplayX += PIXEL_SECOND;

            waveGraph.Children.Add(secondLabel);

        }

        public void DrawGraph()
        {
            //sampleIndex gives every index to take a sample from to display that sample per pixel
            int sampleIndex = soundData.Length / 711;
            //current x value being drawn on
            int x = XSTART + 1;
            PathFigure myPathFigure = new PathFigure();
            myPathFigure.StartPoint = new Point(XSTART, YSTART-soundData[0]);
            for (int i = sampleIndex; i < soundData.Length; i += sampleIndex)
            {
                myPathFigure.Segments.Add(
                    new LineSegment(new Point(x, YSTART-(soundData[i]*heightRatio)), true));
                x++;
            }
            /// Create a PathGeometry to contain the figure.
            PathGeometry myPathGeometry = new PathGeometry();
            myPathGeometry.Figures.Add(myPathFigure);

            // Display the PathGeometry. 
            Path myPath = new Path();
            myPath.Stroke = Brushes.Black;
            myPath.StrokeThickness = 1;
            myPath.Data = myPathGeometry;
            waveGraph.Children.Add(myPath);
        }

        public void DrawSecond(int x, int sampleIndex)
        {
            //ensure we are printing on graph
            x += XSTART;
            //displayIndex gives every index to take a sample from to display that sample per pixel
            int displayIndex = sampleRate / PIXEL_SECOND;
            //Pathfigure to draw lines onto
            PathFigure myPathFigure = new PathFigure();
            //set inital point
            myPathFigure.StartPoint = new Point(x, YSTART - soundData[sampleIndex]);
            x += 1; //increase x after inital point
            int secondCheck = 0; //ensures we do not go past 80 samples to display in a second
            for (int i = sampleIndex; i < (sampleIndex + sampleRate); i += displayIndex)
            {
                if (secondCheck >= 80)
                {
                    break;
                }
                myPathFigure.Segments.Add(
                    new LineSegment(new Point(x, YSTART - (soundData[i] * heightRatio)), true));
                x++;
                secondCheck++;
            }
            /// Create a PathGeometry to contain the figure.
            PathGeometry myPathGeometry = new PathGeometry();
            myPathGeometry.Figures.Add(myPathFigure);

            // Display the PathGeometry. 
            Path myPath = new Path();
            myPath.Stroke = Brushes.Black;
            myPath.StrokeThickness = 1;
            myPath.Data = myPathGeometry;
            waveGraph.Children.Add(myPath);
        }

        private void extendCanvas(int newCanvasWidth)
        {
            waveGraph.Width = newCanvasWidth;
            //create new middle line
            Line middleLine = new Line();
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = Colors.Orange;
            middleLine.Stroke = brush;
            middleLine.StrokeThickness = 2;
            middleLine.X1 = XSTART;
            middleLine.Y1 = YSTART;
            middleLine.Y2 = YSTART;
            middleLine.X2 = newCanvasWidth - XSTART;
            //add middle line
            waveGraph.Children.Add(middleLine);
        }

    }
    
}
