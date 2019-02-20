using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BinaryReader
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            EncodingCB.ItemsSource = Encoding.GetEncodings().Select(a => new Encodings
            {
                Name = a.DisplayName,
                Value = a
            });
            EncodingCB.DisplayMemberPath = "Name";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int stopNum = -1;
            if (AllCB.IsChecked ?? false)
            {
                do
                {
                    Decode();
                    stopNum = EncodingCB.SelectedIndex;
                    EncodingCB.SelectedIndex++;
                    if (EncodingCB.SelectedIndex >= EncodingCB.Items.Count - 1)
                    {
                        stopNum = EncodingCB.SelectedIndex;
                        break;
                    }
                }
                while (MessageBox.Show("Продолжаем?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes);
                EncodingCB.SelectedIndex = stopNum;
            }
            else
                Decode();
        }

        private void Decode()
        {
            var encoding = Encoding.Default;
            if (EncodingCB.SelectedIndex != -1)
                encoding = ((Encodings)EncodingCB.SelectedItem).Value.GetEncoding();
            var input = new TextRange(InputRTB.Document.ContentStart, InputRTB.Document.ContentEnd).Text.Replace(Environment.NewLine, "").Replace(new[] { ", ", ",", ".", ";" }, " ");
            try
            {
                var results = input.Split(' ').ToList();
                var specific = false;
                if (!byte.TryParse(results[0], out byte fb))
                {
                    try
                    {
                        var res = Convert.ToByte(results[0], 16);
                        specific = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Введены не байты.");
                        return;
                    }
                }
                var bytes = (CompressedCB.IsChecked ?? false) ? results.Select(a => specific ? Convert.ToByte(a, 16) : Convert.ToByte(a)).ToArray().DecompressData() : results.Select(a => specific ? Convert.ToByte(a, 16) : Convert.ToByte(a)).ToArray();
                OutputRTB.Document.Blocks.Clear();
                OutputRTB.Document.Blocks.Add(new Paragraph(new Run(encoding.GetString(bytes))));
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"При декодировании возникла ошибка, возможно поставлена галка ""Сжатый"", попробуйте снять и повторить попытку.");
                return;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int stopNum = -1;
            if (AllCB.IsChecked ?? false)
            {
                do
                {
                    Encode();
                    stopNum = EncodingCB.SelectedIndex;
                    EncodingCB.SelectedIndex++;
                    if (EncodingCB.SelectedIndex >= EncodingCB.Items.Count - 1)
                    {
                        stopNum = EncodingCB.SelectedIndex;
                        break;
                    }
                }
                while (MessageBox.Show("Продолжаем?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes);
                EncodingCB.SelectedIndex = stopNum;
            }
            else            
                Encode();
            
        }

        private void Encode()
        {
            var encoding = Encoding.Default;
            if (EncodingCB.SelectedIndex != -1)
                encoding = ((Encodings)EncodingCB.SelectedItem).Value.GetEncoding();
            var input = new TextRange(InputRTB.Document.ContentStart, InputRTB.Document.ContentEnd).Text.Replace(Environment.NewLine, "");
            var result = (CompressedCB.IsChecked ?? false) ? encoding.GetBytes(input).CompressData() : encoding.GetBytes(input);
            OutputRTB.Document.Blocks.Clear();
            OutputRTB.Document.Blocks.Add(new Paragraph(new Run(string.Join(" ", result))));
        }
    }
}
