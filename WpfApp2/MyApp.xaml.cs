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

namespace WpfApp2
{
    public partial class MainWindow : Window
    {
        List<int> testquestionNumbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        int qNum = 0;
        int i;
        int score;

        public MainWindow()
        {
            InitializeComponent();
            TheoryGrid.Width = this.Width - 367;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            this.DragMove();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Header.Width = this.Width + 22;
            NavBar.Height = this.Height - 88;
            TheoryGrid.Width = this.Width - 367;
        }
        private void ButtonCloseClick(object sender, RoutedEventArgs e) => Close();

        private void OnButtonCloseMouseHover(object sender, MouseEventArgs e) => ButtonClosePath.Visibility = Visibility.Visible;

        private void OnButtonCloseMouseLeave(object sender, MouseEventArgs e) => ButtonClosePath.Visibility = Visibility.Hidden;

        private void ButtonMaximizeClick(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                MaxHeight = SystemParameters.VirtualScreenHeight - 34; MaxWidth = SystemParameters.VirtualScreenWidth+14;
                this.WindowState = WindowState.Maximized;
            }
        }

        private void OnButtonMaximizeMouseHover(object sender, MouseEventArgs e) => ButtonMaximizePath.Visibility = Visibility.Visible;

        private void OnButtonMaximizeMouseLeave(object sender, MouseEventArgs e) => ButtonMaximizePath.Visibility = Visibility.Hidden;

        private void ButtonMinimizeClick(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Minimized;

        private void OnButtonMinimizeMouseHover(object sender, MouseEventArgs e) => ButtonMinimizePath.Visibility = Visibility.Visible;

        private void OnButtonMinimizeMouseLeave(object sender, MouseEventArgs e) => ButtonMinimizePath.Visibility = Visibility.Hidden;

        private void Theory1ButtonClick(object sender, RoutedEventArgs e)
        {
            if (Theory1.Visibility == Visibility.Visible) { Theory1.Visibility = Visibility.Hidden; }
            else Theory1.Visibility = Visibility.Visible;
            Theory2.Visibility = Visibility.Hidden;
            Theory3.Visibility = Visibility.Hidden;
            TestPart1Canvas.Visibility = Visibility.Hidden;
            Theory1.IsReadOnly = true;
        }

        private void Theory2ButtonClick(object sender, RoutedEventArgs e)
        {
            if (Theory2.Visibility == Visibility.Visible) { Theory2.Visibility = Visibility.Hidden; }
            else Theory2.Visibility = Visibility.Visible;
            Theory1.Visibility = Visibility.Hidden;
            Theory3.Visibility = Visibility.Hidden;
            TestPart1Canvas.Visibility = Visibility.Hidden;
            Theory2.IsReadOnly = true;
        }

        private void Theory3ButtonClick(object sender, RoutedEventArgs e)
        {
            if (Theory3.Visibility == Visibility.Visible) { Theory3.Visibility = Visibility.Hidden; }
            else Theory3.Visibility = Visibility.Visible;
            Theory1.Visibility = Visibility.Hidden;
            Theory2.Visibility = Visibility.Hidden;
            TestPart1Canvas.Visibility = Visibility.Hidden;
            Theory3.IsReadOnly = true;
        }

        private void Testik_Click(object sender, RoutedEventArgs e)
        {
            ChangeVisibilityTest();
            RestartGame();
        }

        private void ChangeVisibilityTest()
        {
            if (TestPart1Canvas.Visibility == Visibility.Visible)
            {
                TestPart1Canvas.Visibility = Visibility.Hidden;
                Theory1Button.Visibility = Visibility.Visible; Theory2Button.Visibility = Visibility.Visible; Theory3Button.Visibility = Visibility.Visible;
                ButtonStartTest.Visibility = Visibility.Hidden;
                ans1.Visibility = Visibility.Hidden; ans2.Visibility = Visibility.Hidden; ans3.Visibility = Visibility.Hidden; ans4.Visibility = Visibility.Hidden;
            }
            else {
                TestPart1Canvas.Visibility = Visibility.Visible;
                Theory1.Visibility = Visibility.Hidden;
                Theory2.Visibility = Visibility.Hidden;
                Theory3.Visibility = Visibility.Hidden;
                ButtonStartTest.Visibility = Visibility.Visible;
                ans1.Visibility = Visibility.Hidden; ans2.Visibility = Visibility.Hidden; ans3.Visibility = Visibility.Hidden; ans4.Visibility = Visibility.Hidden;

            }
        }


        private void CheckAnswer(object sender, RoutedEventArgs e)
        {
            Button senderButton = sender as Button;
            if (senderButton.Tag.ToString() == "1") score++;

            if (qNum < 0) qNum = 0;
            else qNum++;
            scoreText.Content = "Текущий вопрос " + (qNum + 1) + " из " + testquestionNumbers.Count;
            NextQuestion();
        }

        private void RestartGame()
        {
            score = 0; 
            qNum = 0;
            i = 0; 
            StartGame(); 
        }

        private void NextQuestion()
        {
            ans1.Tag = ""; ans2.Tag = ""; ans3.Tag = ""; ans4.Tag = "";
            if (qNum < testquestionNumbers.Count)
                i = testquestionNumbers[qNum];
            else
            {
                scoreText.Content = "Правильных ответов " + score + "/" + testquestionNumbers.Count + " " + ((float)score / testquestionNumbers.Count) * 100 + "%";
                ans1.Visibility = Visibility.Hidden; ans2.Visibility = Visibility.Hidden; ans3.Visibility = Visibility.Hidden; ans4.Visibility = Visibility.Hidden; txtQuestion.Visibility = Visibility.Hidden;
            }

            switch (i)
            {
                case 1:
                    
                    txtQuestion.Text = "Вопрос 1"; 

                    ans1.Content = "Ответ 1"; 
                    ans2.Content = "Ответ 2 (Правильный)";
                    ans3.Content = "Ответ 3";
                    ans4.Content = "Ответ 4";
                    ans2.Tag = "1";

                    //qImage.Source = new BitmapImage(new Uri("pack://application:,,,/form1.png"));
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

        private void StartGame()
        {
            var randomList = testquestionNumbers.OrderBy(a => Guid.NewGuid()).ToList();
            testquestionNumbers = randomList;
            questionOrder.Content = null;
            ButtonEndTest.Visibility = Visibility.Hidden;

        }

        private void ButtonStartTestClick(object sender, RoutedEventArgs e)
        {
            ans1.Visibility = Visibility.Visible; ans2.Visibility = Visibility.Visible; ans3.Visibility = Visibility.Visible; ans4.Visibility = Visibility.Visible;
            Theory1Button.Visibility = Visibility.Hidden; Theory2Button.Visibility = Visibility.Hidden; Theory3Button.Visibility = Visibility.Hidden;

            ButtonStartTest.Visibility = Visibility.Hidden;
            StartGame();
            ButtonEndTest.Visibility = Visibility.Visible;
            scoreText.Content = "Текущий вопрос " + 1 + " из " + testquestionNumbers.Count;
            NextQuestion();
        }

        private void ButtonEndTestClick(object sender, RoutedEventArgs e)
        {
            ChangeVisibilityTest();
        }
    }
}
