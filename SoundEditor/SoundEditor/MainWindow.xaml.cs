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

using NAudio.Wave;

namespace SoundEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WaveGraph waveGraph;
        private FrequencyGraph freqGraph;
        private Recorder recorder;
        private Canvas defaultCanvas;

        public MainWindow()
        {
            InitializeComponent();
            recorder = new Recorder();
        }

        /**
         * Placeholder for creating a new wave
         */
        private void MenuItem_New(object sender, RoutedEventArgs e)
        {
            float[] soundData = new float[10000];
            Random random = new Random();
            for (int i = 0; i < 10000; i++)
            {
                soundData[i] = random.Next(-10, 11);
            }
            waveGraph = new WaveGraph(WaveCanvas, WaveBorder, soundData, 1000);
            freqGraph = new FrequencyGraph(FrequencyCanvas, soundData);
        }
        
        //record a wave file
        private void MenuItem_Record(object sender, RoutedEventArgs e)
        {
            recorder.StartRecord();
            //change button/menu switches
            record_MI.IsEnabled = false;
            record_BTN.IsEnabled = false;
            stop_MI.IsEnabled = true;
            stop_BTN.IsEnabled = true;

        }


        private void MenuItem_StopRecord(object sender, RoutedEventArgs e)
        {
            recorder.StopRecord();
            float[] recordedSamples = recorder.getRecordingData();
            waveGraph = new WaveGraph(WaveCanvas, WaveBorder, recordedSamples, 44100);
            freqGraph = new FrequencyGraph(FrequencyCanvas, recordedSamples);
            
            //change button/menu switches
            record_MI.IsEnabled = true;
            record_BTN.IsEnabled = true;
            play_BTN.IsEnabled = true;
            stop_MI.IsEnabled = false;
            stop_BTN.IsEnabled = false;
        }
        private void MenuItem_PlayRecord(object sender, RoutedEventArgs e)
        {
            recorder.Playback();
        }
        private void MenuItem_Exit(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


    }
}
