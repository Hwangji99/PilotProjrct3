using System.ComponentModel;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows;

namespace Proj3.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _filePath;
        private BitmapSource _displayImage;
        private Mat _currentImage;
        private VideoCapture _videoCapture;
        private Timer _timer;
        private int _currentFrameIndex = 0;


        public MainViewModel()
        {
            OpenFileBtn = new Command.Command(OpenFile);
            DrawingBtn = new Command.Command(SetDrawingMode);
            ErasorBtn = new Command.Command(SetErasingMode);
            SaveBtn = new Command.Command(SaveImage);

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
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image and Video Files|*.jpg;*.png;*.bmp;*.mp4;*.avi"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;

                try
                {
                    // 확장자를 기반으로 이미지와 동영상을 구분
                    string extension = System.IO.Path.GetExtension(FilePath).ToLower();
                    if (extension == ".jpg" || extension == ".png" || extension == ".bmp")
                    {
                        // 이미지 파일 로드
                        _currentImage = Cv2.ImRead(FilePath);

                        if (_currentImage != null && !_currentImage.Empty())
                        {
                            // 화면에 표시
                            UpdateDisplayImage();
                        }
                        else
                        {
                            throw new Exception("이미지를 로드하지 못했습니다.");
                        }
                    }
                    else if (extension == ".mp4" || extension == ".avi")
                    {
                        // 동영상 파일 로드
                        VideoCapture videoCapture = new VideoCapture(FilePath);
                        if (videoCapture.IsOpened())
                        {
                            // 동영상 첫 프레임 읽기
                            Mat firstFrame = new Mat();
                            videoCapture.Read(firstFrame);

                            if (firstFrame != null && !firstFrame.Empty())
                            {
                                _currentImage = firstFrame;
                                // 화면에 첫 프레임 표시
                                UpdateDisplayImage();
                            }
                            else
                            {
                                throw new Exception("동영상의 첫 프레임을 로드하지 못했습니다.");
                            }
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
                UpdateDisplayImage();
            }
        }

        private void SetErasingMode(object parameter)
        {
            if (_currentImage != null)
            {
                // OpenCV를 사용해 지우기 효과 (예: 전체 초기화)
                _currentImage.SetTo(new Scalar(255, 255, 255));
                UpdateDisplayImage();
            }
        }

        private void SaveImage(object parameter)
        {
            if (_currentImage != null)
            {
                // 저장 경로 설정
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Image Files|*.jpg;*.png;*.bmp",
                    DefaultExt = "jpg"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    Cv2.ImWrite(saveFileDialog.FileName, _currentImage);
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
