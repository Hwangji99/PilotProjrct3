using System.ComponentModel;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Proj3.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _filePath;
        private BitmapSource _displayImage;
        private Mat _currentImage;
        private VideoCapture _videoCapture;
        private System.Timers.Timer _timer;
        private InkCanvas _inkCanvas;


        public MainViewModel()
        {
            OpenFileBtn = new Command.Command(OpenFile);
            DrawingBtn = new Command.Command(SetDrawingMode);
            ErasorBtn = new Command.Command(SetErasingMode);
            SaveBtn = new Command.Command(SaveImage);
        }

        public InkCanvas InkCanvas
        {
            get => _inkCanvas;
            set
            {
                _inkCanvas = value;
                OnPropertyChanged(nameof(InkCanvas));
            }
        }

        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
                OnPropertyChanged(nameof(FilePath));
            }
        }

        public BitmapSource DisplayImage
        {
            get => _displayImage;
            set
            {
                _displayImage = value;
                OnPropertyChanged(nameof(DisplayImage));
            }
        }

        private BitmapSource MatToBitmapSource(Mat mat)
        {
            return BitmapSourceConverter.ToBitmapSource(mat);
        }

        public ICommand OpenFileBtn { get; set; }
        public ICommand DrawingBtn { get; set; }
        public ICommand ErasorBtn { get; set; }
        public ICommand SaveBtn { get; set; }

        private void OpenFile(object parameter)
        {
            // 기존 리소스 정리
            if (_videoCapture != null)
            {
                _videoCapture.Release(); // VideoCapture 해제
                _videoCapture = null;
            }
            if (_timer != null)
            {
                _timer.Stop(); // 타이머 중지
                _timer.Dispose(); // 타이머 해제
                _timer = null;
            }
            if (_currentImage != null)
            {
                _currentImage.Dispose(); // Mat 해제
                _currentImage = null;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image and Video Files|*.jpg;*.png;*.bmp;*.mp4;*.avi"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;

                try
                {
                    string extension = System.IO.Path.GetExtension(FilePath).ToLower();
                    if (extension == ".jpg" || extension == ".png" || extension == ".bmp")
                    {
                        // 이미지 파일 로드
                        Mat image = Cv2.ImRead(FilePath);

                        if (image != null && !image.Empty())
                        {
                            _currentImage = image;
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                DisplayImage = MatToBitmapSource(_currentImage);
                            });
                        }
                        else
                        {
                            throw new Exception("이미지를 로드하지 못했습니다.");
                        }
                    }
                    else if (extension == ".mp4" || extension == ".avi")
                    {
                        // 동영상 파일 로드
                        _videoCapture = new VideoCapture(FilePath);

                        if (_videoCapture.IsOpened())
                        {
                            Mat frame = new Mat();
                            _timer = new System.Timers.Timer(33); // 약 30FPS

                            _timer.Elapsed += (sender, e) =>
                            {
                                // 동영상의 프레임을 읽어 화면에 표시
                                if (_videoCapture.Read(frame) && !frame.Empty())
                                {
                                    _currentImage = frame;
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        DisplayImage = MatToBitmapSource(_currentImage);
                                    });
                                }
                                else
                                {
                                    // 동영상 끝났을 때 자동으로 처음으로 돌아가서 다시 재생
                                    _videoCapture.Set(VideoCaptureProperties.PosFrames, 0);  // 처음으로 돌아가기
                                }
                            };
                            _timer.Start();  // 타이머 시작
                        }
                        else
                        {
                            throw new Exception("동영상을 로드하지 못했습니다.");
                        }
                    }
                    else
                    {
                        throw new Exception("지원하지 않는 파일 형식입니다.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"파일 로드 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private InkCanvasEditingMode _inkCanvasEditingMode = InkCanvasEditingMode.None;

        public InkCanvasEditingMode InkCanvasEditingMode
        {
            get => _inkCanvasEditingMode;
            set
            {
                _inkCanvasEditingMode = value;
                OnPropertyChanged(nameof(InkCanvasEditingMode));
            }
        }

        private void UpdateDisplayImage()
        {
            if (_currentImage != null)
            {
                DisplayImage = MatToBitmapSource(_currentImage);
            }
        }

        private void SetDrawingMode(object parameter)
        {
            if (_currentImage != null)
            {
                InkCanvasEditingMode = InkCanvasEditingMode.Ink;
                UpdateDisplayImage();
            }
        }

        private void SetErasingMode(object parameter)
        {
            if (_currentImage != null)
            {
                InkCanvasEditingMode = InkCanvasEditingMode.EraseByPoint;
                UpdateDisplayImage();
            }
        }

        private void SaveImage(object parameter)
        {
            if (_currentImage != null)
            {
                // InkCanvas에 그린 내용을 비트맵으로 캡처
                RenderTargetBitmap inkCanvasBitmap = new RenderTargetBitmap((int)_currentImage.Width, (int)_currentImage.Height, 96, 96, PixelFormats.Pbgra32);
                inkCanvasBitmap.Render(_inkCanvas);  

                // InkCanvas의 내용을 Mat로 변환
                BitmapSource combinedImage = inkCanvasBitmap;
                Mat inkCanvasMat = BitmapSourceConverter.ToMat(combinedImage);

                // 원본 이미지와 InkCanvas 그림을 합침
                Mat finalImage = _currentImage.Clone();  // 원본 이미지 복사
                Cv2.AddWeighted(finalImage, 1.0, inkCanvasMat, 1.0, 0.0, finalImage);  // 그림 합치기

                // 저장 경로 설정
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Image Files|*.jpg;*.png;*.bmp",
                    DefaultExt = "jpg"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        string extension = System.IO.Path.GetExtension(saveFileDialog.FileName).ToLower();

                        // 최종 이미지를 저장
                        Cv2.ImWrite(saveFileDialog.FileName, finalImage);

                        MessageBox.Show("파일이 성공적으로 저장되었습니다.", "저장 완료", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"파일 저장 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
