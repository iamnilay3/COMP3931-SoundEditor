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

namespace SoundEditor
{
    struct Complex
    {
        public double real;
        public double imaginary;
    }
    class FrequencyGraph
    {
        //grid to paint on
        Grid frequencyGraph;
        //Sound Data
        float[] soundData;
        //Number of samples
        int N;
        //stores real and imaginary numbers of a frequency bin
        
        public FrequencyGraph(Grid frequencyGraph, float[] soundData)
        {
            this.frequencyGraph = frequencyGraph;
            this.soundData = soundData;
            N = 100;
            drawGraph();
        }

        public void drawGraph()
        {
            int canvasWidth = (int)frequencyGraph.Width;
            float barWidth = (float)canvasWidth / (float)N;
            //Console.WriteLine(N);
            //Console.WriteLine(canvasWidth);
            //Console.WriteLine(barWidth);
            double[] amplitudes = getMaxAmplitudes();
            double maxAmplitude = amplitudes.Max(element => Math.Abs(element));
            double amplitudeRatio = frequencyGraph.Height / maxAmplitude;
            for (int i = 0; i < amplitudes.Length; i++)
            {
                drawBar(barWidth, (int)(amplitudes[i]*amplitudeRatio), barWidth * i);
            }
        }

        public void drawBar(float width, int height, float x)
        {
            Rectangle freqBar = new Rectangle();
            freqBar.Width = width;
            freqBar.Height = height;
            freqBar.Fill = System.Windows.Media.Brushes.Blue;
            freqBar.HorizontalAlignment = HorizontalAlignment.Left;
            freqBar.VerticalAlignment = VerticalAlignment.Bottom;
            freqBar.Margin = new System.Windows.Thickness(x, 0, 0, 0);
            frequencyGraph.Children.Add(freqBar);
        }

        public Complex[] discreteFourierTransformation(double[] S)
        {
            //Imaginary values related to frequency bin's amplitude
            Complex[] A = new Complex[N]; 
            Complex temp;
            for (int f = 0; f < N; f++)
            {
                temp.real = 0;
                temp.imaginary = 0;
                for (int t = 0; t < N; t++)
                {
                    temp.real += S[t] * Math.Cos(2 * Math.PI * t * f / N);
                    temp.imaginary -= S[t] * Math.Sin(2 * Math.PI * t * f / N);
                }
                A[f] = temp;
            }
            return A;
        }

        public double[] inverseFourier(Complex[] A)
        {
            //Sample Value
            double[] S = new double[N];
            double temp;
            for (int t = 0; t < N; t++)
            {
                temp = 0;
                for (int f = 0; f < N; f++)
                {
                    temp += A[f].real * Math.Cos(2 * Math.PI * t * f / N)
                        - A[f].imaginary * Math.Sin(2 * Math.PI * t * f / N);
                }
                S[t] = temp / N;
            }
            return S;
        }

        public double getFrequencyBinAmplitude(Complex A)
        {
            double a, b, c;
            a = A.real * A.real;
            b = A.imaginary * A.imaginary;
            c = Math.Sqrt(a + b);
            return c;
        }

        public double[] getMaxAmplitudes()
        {
            Complex[] A;
            double[] tempAmplitudes = new double[N];
            double tempAmplitude;
            double[] tempSamples = new double[N];
            for (int i = 0; i < soundData.Length-N; i += N)
            {
                for(int j = 0; j < N; j++)
                {
                    tempSamples[j] = soundData[i+j];
                    //Console.WriteLine("soundData:" + soundData[i+j] + " i+j:" + (i+j));
                }
                A = discreteFourierTransformation(tempSamples);
                for (int k = 0; k < A.Length; k++)
                {
                    tempAmplitude = getFrequencyBinAmplitude(A[k]);
                    //Console.WriteLine("freqBinAmp:" + tempAmplitude + " k:" + k);
                    //Console.WriteLine(tempAmplitude);
                    if (tempAmplitudes[k] < tempAmplitude)
                    {
                        tempAmplitudes[k] = tempAmplitude;
                    }
                }
            }
            return tempAmplitudes;
        }
    }
}
