using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace WpfApp2
{
    public partial class MainWindow : Window
    {
        List<int> testquestionNumbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        int activePractice = 0;
        int totalPracticeScore = 0;

        int questionNum, i, score;
        Border myDraggableImage = new Border();

        public MainWindow()
        {
            InitializeComponent();

        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            DragMove();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Header.Width = Width - 8;
            //NavBar.Height = this.Height - 53;
            //ContentTheory1.Width = this.Width - 367; ContentTest.Width = this.Width - 367;
        }

        private void ButtonCloseClick(object sender, RoutedEventArgs e) => Close();

        private void OnButtonCloseMouseHover(object sender, MouseEventArgs e) => ButtonClosePath.Visibility = Visibility.Visible;

        private void OnButtonCloseMouseLeave(object sender, MouseEventArgs e) => ButtonClosePath.Visibility = Visibility.Hidden;

        private void ButtonMaximizeClick(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized) WindowState = WindowState.Normal;
            else
            {
                MaxHeight = SystemParameters.VirtualScreenHeight - 34; MaxWidth = SystemParameters.VirtualScreenWidth + 14;
                WindowState = WindowState.Maximized;
            }
        }

        private void OnButtonMaximizeMouseHover(object sender, MouseEventArgs e) => ButtonMaximizePath.Visibility = Visibility.Visible;

        private void OnButtonMaximizeMouseLeave(object sender, MouseEventArgs e) => ButtonMaximizePath.Visibility = Visibility.Hidden;

        private void ButtonMinimizeClick(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void OnButtonMinimizeMouseHover(object sender, MouseEventArgs e) => ButtonMinimizePath.Visibility = Visibility.Visible;

        private void OnButtonMinimizeMouseLeave(object sender, MouseEventArgs e) => ButtonMinimizePath.Visibility = Visibility.Hidden;

        private void Theory1Click(object sender, RoutedEventArgs e)
        {
            if (TheoryView.Visibility != Visibility.Visible) TheoryView.Visibility = Visibility.Visible;
            else
                if (TheoryView.Source == (new Uri(Environment.CurrentDirectory + "\\theory\\Отличия поколений.htm"))) 
                    TheoryView.Visibility = Visibility.Hidden;

            if (TheoryView.Source != (new Uri(Environment.CurrentDirectory + "\\theory\\Отличия поколений.htm"))) TheoryView.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\Отличия поколений.htm"));
        }

        private void Theory2Click(object sender, RoutedEventArgs e)
        {
            if (TheoryView.Visibility != Visibility.Visible) TheoryView.Visibility = Visibility.Visible;
            else 
                if (TheoryView.Source == (new Uri(Environment.CurrentDirectory + "\\theory\\Оборудование мобильных сетей.htm"))) TheoryView.Visibility = Visibility.Hidden;

            if (TheoryView.Source != (new Uri(Environment.CurrentDirectory + "\\theory\\Оборудование мобильных сетей.htm"))) TheoryView.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\Оборудование мобильных сетей.htm"));

        }

        private void Theory3Click(object sender, RoutedEventArgs e)
        {
            if (TheoryView.Visibility != Visibility.Visible) TheoryView.Visibility = Visibility.Visible;
            else
                if (TheoryView.Source == (new Uri(Environment.CurrentDirectory + "\\theory\\Антенны.htm"))) TheoryView.Visibility = Visibility.Hidden;

            if (TheoryView.Source != (new Uri(Environment.CurrentDirectory + "\\theory\\Антенны.htm"))) TheoryView.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\Антенны.htm"));
        }

        private void Theory4Click(object sender, RoutedEventArgs e)
        {
            if (TheoryView.Visibility != Visibility.Visible) TheoryView.Visibility = Visibility.Visible;
            else
                if (TheoryView.Source == (new Uri(Environment.CurrentDirectory + "\\theory\\Распространение радиосигнала.htm"))) TheoryView.Visibility = Visibility.Hidden;

            if (TheoryView.Source != (new Uri(Environment.CurrentDirectory + "\\theory\\Распространение радиосигнала.htm"))) TheoryView.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\Распространение радиосигнала.htm"));
        }

        private void PracticeImage_Dragging(object sender, DragEventArgs e)
        {

            Point dropPosition = e.GetPosition(PracticeCanvas);

            if (e.Source is not Canvas)
            {
                //myDraggableImage = (Image)FindName( ((Image)e.Source).Name);
                myDraggableImage = (Border)((Image)e.Source).Parent;
                Canvas.SetLeft(myDraggableImage, dropPosition.X);
                Canvas.SetTop(myDraggableImage, dropPosition.Y);
            }
            else
            {
                Canvas.SetLeft(myDraggableImage, dropPosition.X);
                Canvas.SetTop(myDraggableImage, dropPosition.Y);
            }
        }

        private void PracticeDraggableImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var myElement = (Border)FindName(((Border)sender).Name);
                DragDrop.DoDragDrop(myElement, myElement, DragDropEffects.Move);
            }
        }

        private  void Practice1Click(object sender, RoutedEventArgs e) 
        {
            activePractice = 1;

            foreach (UIElement element in PracticeCanvas.Children)
            {
                if (element is Border)
                {
                    Border cb = (Border)element;
                    cb.Visibility = Visibility.Visible;
                    cb.BorderBrush = null;

                    //if (cb.Name.EndsWith("1")) cb.Margin = new Thickness(13, -65, 0, 0);
                }
            }

            if (sender is Button)
                PracticeContent.Visibility = Visibility.Visible;
            else
            {
                if (PracticeContent.Visibility == Visibility.Hidden) PracticeContent.Visibility = Visibility.Visible;
                else PracticeContent.Visibility = Visibility.Hidden;
            }
        }
        
        private void StartPracticeClick(object sender, RoutedEventArgs e)
        {
            if (activePractice == 1) 
            {
                Random r = new Random();

                PracticeCanvas.AllowDrop = true;
                Practice1Image.Visibility = Visibility.Visible;
                PracticeCanvas.Visibility = Visibility.Visible;

                foreach (UIElement element in PracticeCanvas.Children)
                {
                    int rIntTop = r.Next(0, 400);
                    int rIntLeft = r.Next(0, 400);
                    if (element is Border)
                    {
                        Canvas.SetTop(element, rIntTop);
                        Canvas.SetLeft(element, rIntLeft);
                    }
                }
            }
            StartPracticeButton.Visibility = Visibility.Hidden;
            CheckPracticeButton.Visibility = Visibility.Visible;
        }

        private void CheckPracticeClick(object sender, RoutedEventArgs e)
        {
            CheckPracticeButton.Visibility = Visibility.Hidden;
            ReStartPracticeButton.Visibility = Visibility.Visible;
            if (activePractice == 1)
            {
                var score = 0;
                PracticeCanvas.AllowDrop = false;

                if (384 <= Canvas.GetLeft(PracticeDraggableImage1) && (Canvas.GetLeft(PracticeDraggableImage1) <= 415)
                    && (163 <= Canvas.GetTop(PracticeDraggableImage1)) && (Canvas.GetTop(PracticeDraggableImage1) <= 200))
                { 
                    PracticeDraggableImage1.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else 
                    PracticeDraggableImage1.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                if (0 <= Canvas.GetLeft(PracticeDraggableImage2) && (Canvas.GetLeft(PracticeDraggableImage2) <= 40)
                    && (115 <= Canvas.GetTop(PracticeDraggableImage2)) && (Canvas.GetTop(PracticeDraggableImage2) <= 135))
                {
                    PracticeDraggableImage2.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                    PracticeDraggableImage2.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                if (398 <= Canvas.GetLeft(PracticeDraggableImage3) && (Canvas.GetLeft(PracticeDraggableImage3) <= 420)
                    && (65 <= Canvas.GetTop(PracticeDraggableImage3)) && (Canvas.GetTop(PracticeDraggableImage3) <= 120))
                {
                    PracticeDraggableImage3.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                    PracticeDraggableImage3.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                if (185 <= Canvas.GetLeft(PracticeDraggableImage4) && (Canvas.GetLeft(PracticeDraggableImage4) <= 216)

                    && (129 <= Canvas.GetTop(PracticeDraggableImage4)) && (Canvas.GetTop(PracticeDraggableImage4) <= 159))
                {
                    PracticeDraggableImage4.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                    PracticeDraggableImage4.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                if (290 <= Canvas.GetLeft(PracticeDraggableImage5) && (Canvas.GetLeft(PracticeDraggableImage5) <= 315)
                    && (133 <= Canvas.GetTop(PracticeDraggableImage5)) && (Canvas.GetTop(PracticeDraggableImage5) <= 156))
                {
                    PracticeDraggableImage5.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                    PracticeDraggableImage5.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                if ( 240 <= Canvas.GetLeft(PracticeDraggableImage6) && (Canvas.GetLeft(PracticeDraggableImage6) <= 270)
                    && (300 <= Canvas.GetTop(PracticeDraggableImage6)) && (Canvas.GetTop(PracticeDraggableImage6) <= 356)
                    || (125 <= Canvas.GetLeft(PracticeDraggableImage6) && (Canvas.GetLeft(PracticeDraggableImage6) <= 152)
                    && (290 <= Canvas.GetTop(PracticeDraggableImage6)) && (Canvas.GetTop(PracticeDraggableImage6) <= 341)))
                {
                    PracticeDraggableImage6.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                    PracticeDraggableImage6.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                if (240 <= Canvas.GetLeft(PracticeDraggableImage7) && (Canvas.GetLeft(PracticeDraggableImage7) <= 270)
                    && (300 <= Canvas.GetTop(PracticeDraggableImage7)) && (Canvas.GetTop(PracticeDraggableImage7) <= 356)
                    || (125 <= Canvas.GetLeft(PracticeDraggableImage7) && (Canvas.GetLeft(PracticeDraggableImage7) <= 152)
                    && (290 <= Canvas.GetTop(PracticeDraggableImage7)) && (Canvas.GetTop(PracticeDraggableImage7) <= 341)))
                {
                    PracticeDraggableImage7.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                    PracticeDraggableImage7.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                if (183 <= Canvas.GetLeft(PracticeDraggableImage8) && (Canvas.GetLeft(PracticeDraggableImage8) <= 210)
                    && (228 <= Canvas.GetTop(PracticeDraggableImage8)) && (Canvas.GetTop(PracticeDraggableImage8) <= 260))
                {
                    PracticeDraggableImage8.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                    PracticeDraggableImage8.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                if (75 <= Canvas.GetLeft(PracticeDraggableImage9) && (Canvas.GetLeft(PracticeDraggableImage9) <= 105)
                    && (180 <= Canvas.GetTop(PracticeDraggableImage9)) && (Canvas.GetTop(PracticeDraggableImage9) <= 200))
                {
                    PracticeDraggableImage9.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                    PracticeDraggableImage9.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                if (320 <= Canvas.GetLeft(PracticeDraggableImage10) && (Canvas.GetLeft(PracticeDraggableImage10) <= 360)
                    && (25 <= Canvas.GetTop(PracticeDraggableImage10)) && (Canvas.GetTop(PracticeDraggableImage10) <= 60))
                {
                    PracticeDraggableImage10.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                    PracticeDraggableImage10.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                if (score == 10) totalPracticeScore++;
            }


        }

        private void ReStartPracticeClick(object sender, RoutedEventArgs e)
        {
            StartPracticeButton.Visibility = Visibility.Visible;
            ReStartPracticeButton.Visibility = Visibility.Hidden;
            if (activePractice == 1)
            {
                Practice1Click(sender, e);
            }
        }

        private void TestClick(object sender, RoutedEventArgs e)
        {
            ChangeVisibilityTest();
            RestartGame();
        }

        private void ChangeVisibilityTest()
        {
            if (ContentTest.Visibility == Visibility.Visible)
            {
                ContentTest.Visibility = Visibility.Hidden;
                ContentTheory1.Visibility = Visibility.Visible;
                TestMenu.Visibility = Visibility.Visible;
                ButtonStartTest.Visibility = Visibility.Hidden; 
                ans1.Visibility = Visibility.Hidden; 
                ans2.Visibility = Visibility.Hidden; 
                ans3.Visibility = Visibility.Hidden; 
                ans4.Visibility = Visibility.Hidden;
            }
            else {
                ContentTheory1.Visibility = Visibility.Hidden;
                ContentTest.Visibility = Visibility.Visible;
                ButtonStartTest.Visibility = Visibility.Visible; 
                ButtonEndTest.Visibility = Visibility.Hidden;
                ans1.Visibility = Visibility.Hidden; 
                ans2.Visibility = Visibility.Hidden; 
                ans3.Visibility = Visibility.Hidden; 
                ans4.Visibility = Visibility.Hidden;
            }
        }        
        
        private void RestartGame()
        {
            score = 0;
            questionNum = 0;
            i = 0; 
            StartGame(); 
        }

        private void StartGame()
        {
            var randomList = testquestionNumbers.OrderBy(a => Guid.NewGuid()).ToList();
            testquestionNumbers = randomList;
        }

        private void CheckAnswer(object sender, RoutedEventArgs e)
        {
            Button? senderButton = sender as Button;
            if (senderButton.Tag.ToString() == "1") score++;

            questionNum++;
            scoreText.Content = "Текущий вопрос " + (questionNum + 1) + " из " + testquestionNumbers.Count;
            NextQuestion();
        }

        private void NextQuestion()
        {
            txtQuestion.Visibility = Visibility.Visible;
            qImage.Visibility = Visibility.Hidden;
            qImage.Source = new BitmapImage(new Uri("pack://application:,,,/form1.png"));
            ans1.Tag = ""; ans2.Tag = ""; ans3.Tag = ""; ans4.Tag = "";
            if (questionNum < testquestionNumbers.Count)
                i = testquestionNumbers[questionNum];
            else
            {
                double totalScore;
                totalScore = (float)score / testquestionNumbers.Count;
                totalScore = Math.Round(totalScore, 2);
                MessageBox.Show("Правильных ответов " + score + "/" + testquestionNumbers.Count + " " + (totalScore * 100) + "%");
                scoreText.Content = "Правильных ответов " + score + "/" + testquestionNumbers.Count + " " + (totalScore * 100) + "%";
                ans1.Visibility = Visibility.Hidden; 
                ans2.Visibility = Visibility.Hidden; 
                ans3.Visibility = Visibility.Hidden; 
                ans4.Visibility = Visibility.Hidden; 
                txtQuestion.Visibility = Visibility.Hidden;
            }

            switch (i)
            {
                case 1:
                    qImage.Visibility = Visibility.Visible;
                    txtQuestion.Text = "Вопрос 1"; 

                    ans1.Content = "Ответ 1"; 
                    ans2.Content = "Ответ 2 (Правильный)";
                    ans3.Content = "Ответ 3";
                    ans4.Content = "Ответ 4";
                    ans2.Tag = "1";

                    qImage.Source = new BitmapImage(new Uri("pack://application:,,,/form1.png"));
                    break;

                case 2:
                    txtQuestion.Text = "Вопрос 2";

                    ans1.Content = "Ответ 1 (Правильный)";
                    ans2.Content = "Ответ 2";
                    ans3.Content = "Ответ 3";
                    ans4.Content = "Ответ 4";
                    ans1.Tag = "1";
                    break;

                case 3:
                    
                    txtQuestion.Text = "Вопрос 3";

                    ans1.Content = "Ответ 1";
                    ans2.Content = "Ответ 2";
                    ans3.Content = "Ответ 3 (Правильный)";
                    ans4.Content = "Ответ 4";
                    ans3.Tag = "1";
                    break;

                case 4:
                    txtQuestion.Text = "Вопрос 4";

                    ans1.Content = "Ответ 1";
                    ans2.Content = "Ответ 2";
                    ans3.Content = "Ответ 3";
                    ans4.Content = "Ответ 4 (Правильный)";
                    ans4.Tag = "1";
                    break;

                case 5:
                    txtQuestion.Text = "Вопрос 5";

                    ans1.Content = "Ответ 1 (Правильный)";
                    ans2.Content = "Ответ 2";
                    ans3.Content = "Ответ 3";
                    ans4.Content = "Ответ 4";
                    ans1.Tag = "1";
                    break;

                case 6:
                    txtQuestion.Text = "Вопрос 6";

                    ans1.Content = "Ответ 1";
                    ans2.Content = "Ответ 2";
                    ans3.Content = "Ответ 3 (Правильный)";
                    ans4.Content = "Ответ 4";
                    ans3.Tag = "1";
                    break;

                case 7:
                    txtQuestion.Text = "Вопрос 7";

                    ans1.Content = "Ответ 1";
                    ans2.Content = "Ответ 2 (Правильный)";
                    ans3.Content = "Ответ 3";
                    ans4.Content = "Ответ 4";
                    ans2.Tag = "1";
                    break;

                case 8:
                    txtQuestion.Text = "Вопрос 8";

                    ans1.Content = "Ответ 1";
                    ans2.Content = "Ответ 2";
                    ans3.Content = "Ответ 3";
                    ans4.Content = "Ответ 4 (Правильный)";
                    ans4.Tag = "1";
                    break;

                case 9:
                    txtQuestion.Text = "Вопрос 9";

                    ans1.Content = "Ответ 1";
                    ans2.Content = "Ответ 2";
                    ans3.Content = "Ответ 3 (Правильный)";
                    ans4.Content = "Ответ 4";
                    ans3.Tag = "1";
                    break;

                case 10:
                    
                    txtQuestion.Text = "Вопрос 10";

                    ans1.Content = "Ответ 1 (Правильный)";
                    ans2.Content = "Ответ 2";
                    ans3.Content = "Ответ 3";
                    ans4.Content = "Ответ 4";
                    ans1.Tag = "1";
                    break;
            }
        }

        private void ShowAfterTest()
        {
            TheoryMenu.Visibility = Visibility.Visible;
            PracticeMenu.Visibility = Visibility.Visible;
            ContentTheory1.Visibility = Visibility.Visible;
        }

        private void HideWhileTest()
        {
            TheoryMenu.Visibility = Visibility.Hidden;
            PracticeMenu.Visibility = Visibility.Hidden;
            ContentTheory1.Visibility = Visibility.Hidden;
        }

        private void ButtonStartTestClick(object sender, RoutedEventArgs e)
        {
            TestMenu.Visibility = Visibility.Hidden;
            ans1.Visibility = Visibility.Visible; 
            ans2.Visibility = Visibility.Visible; 
            ans3.Visibility = Visibility.Visible; 
            ans4.Visibility = Visibility.Visible;
            
            HideWhileTest();

            ButtonStartTest.Visibility = Visibility.Hidden;
            StartGame();
            ButtonEndTest.Visibility = Visibility.Visible;
            scoreText.Content = "Текущий вопрос " + 1 + " из " + testquestionNumbers.Count;
            NextQuestion();
        }

        private void ButtonEndTestClick(object sender, RoutedEventArgs e)
        {
            txtQuestion.Text = "";
            qImage.Visibility = Visibility.Hidden;
            scoreText.Content = "";
            ChangeVisibilityTest();
            ShowAfterTest();
            ButtonEndTest.Visibility = Visibility.Hidden;
        }
    }
}
