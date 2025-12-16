using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes; // Use WPF Rectangle

namespace FrequencyVisualizer
{
    public partial class MainWindow : Window
    {
        private SerialPortHandler serialPortHandler;

        public MainWindow()
        {
            InitializeComponent();

            serialPortHandler = new SerialPortHandler("COM6", 9600);
            serialPortHandler.DataReceived += OnDataReceived;
            serialPortHandler.Open();

            this.Closed += Window_Closed; // Ensure event is hooked up
        }

        private void OnDataReceived(string data)
        {
            try
            {
                string[] values = data.Split(',');
                int[] frequencies = values.Select(v => int.Parse(v)).ToArray();

                Dispatcher.Invoke(() => UpdateCanvas(frequencies));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing data: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateCanvas(int[] frequencies)
        {
            FrequencyCanvas.Children.Clear();

            double canvasWidth = FrequencyCanvas.ActualWidth;
            double canvasHeight = FrequencyCanvas.ActualHeight;

            if (frequencies.Length == 0 || canvasWidth == 0) return;

            double barWidth = canvasWidth / frequencies.Length;

            for (int i = 0; i < frequencies.Length; i++)
            {
                double normalizedHeight = (frequencies[i] / 1023.0) * canvasHeight;

                var bar = new Rectangle
                {
                    Width = Math.Max(1, barWidth - 5),
                    Height = normalizedHeight,
                    Fill = Brushes.LimeGreen
                };

                Canvas.SetLeft(bar, i * barWidth);
                Canvas.SetTop(bar, canvasHeight - normalizedHeight);

                FrequencyCanvas.Children.Add(bar);
            }
            }

        private void Window_Closed(object sender, EventArgs e)
        {
            serialPortHandler.Close();
        }
    }
