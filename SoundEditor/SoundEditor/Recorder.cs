using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NAudio.Wave;


namespace SoundEditor
{
    //TODO Naudio.wave.waveout.finalize
    class Recorder
    {
        //Recorded data
        private List<float> recordingData;
        private WaveIn recordingStream;
        private WaveFileWriter waveWriter;
        private String waveFileName;
        private WaveOut waveReader;

        public Recorder()
        {
            recordingStream = new WaveIn();
            recordingStream.WaveFormat = new WaveFormat(44100, 2);
            recordingData = new List<float>();

            
        }

        public void StartRecord()
        {
            //creates temporary wave file
            waveFileName = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString() + ".wav");
            waveWriter = new WaveFileWriter(waveFileName, recordingStream.WaveFormat);
            //Record data
            recordingStream.DataAvailable += new EventHandler<WaveInEventArgs>(recordingStream_DataAvailable);
            recordingStream.StartRecording();
        }

        //credit: http://channel9.msdn.com/coding4fun/articles/NET-Voice-Recorder
        private void recordingStream_DataAvailable(object sender, WaveInEventArgs e)
        {
            waveWriter.Write(e.Buffer, 0, e.BytesRecorded);
            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = (short)((e.Buffer[index + 1] << 8) |
                                        e.Buffer[index + 0]);
                float sample32 = sample / 32768f;
                recordingData.Add(sample32);
            }
        }

        public void StopRecord()
        {
            //stop recording
            recordingStream.StopRecording();
            recordingStream.Dispose();
            recordingStream = null;

            //stop writing
            waveWriter.Dispose();
            WaveStream streamOut = new WaveFileReader(waveFileName);
            waveReader = new WaveOut();
            waveReader.Init(streamOut);
        }

        public float[] getRecordingData()
        {
            //display data
            float[] sampleData = recordingData.ToArray<float>();
            return sampleData;
        }

        public void Playback()
        {
            waveReader.Play();
        }
    }
}
