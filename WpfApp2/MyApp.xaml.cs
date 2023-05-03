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

        private void EnterButtonClick(object sender, RoutedEventArgs e)
        {
            if (Enter.Visibility == Visibility.Visible) { Enter.Visibility = Visibility.Hidden; }
            else Enter.Visibility = Visibility.Visible;

            Socr.Visibility = Visibility.Hidden;
            TestPart1Canvas.Visibility = Visibility.Hidden;

            Enter.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\Enter.htm"));
        }

        private void Theory1ButtonClick(object sender, RoutedEventArgs e)
        {
            Enter.Visibility = Visibility.Hidden;
            TestPart1Canvas.Visibility = Visibility.Hidden;
            Socr.Visibility = Visibility.Hidden;

            BackButton.Visibility = Visibility.Visible;
            Theory1_1Button.Visibility = Visibility.Visible;
            Theory1_2Button.Visibility = Visibility.Visible;
            Theory1_3Button.Visibility = Visibility.Visible;

            EnterButton.Visibility = Visibility.Hidden;
            Theory1Button.Visibility = Visibility.Hidden;
            Theory2Button.Visibility = Visibility.Hidden;
            Theory3Button.Visibility = Visibility.Hidden;
            Theory4Button.Visibility = Visibility.Hidden;
            Theory5Button.Visibility = Visibility.Hidden;
            Testik.Visibility = Visibility.Hidden;
        }

        private void Theory2ButtonClick(object sender, RoutedEventArgs e)
        {
            Enter.Visibility = Visibility.Hidden;
            TestPart1Canvas.Visibility = Visibility.Hidden;
            Socr.Visibility = Visibility.Hidden;

            BackButton.Visibility = Visibility.Visible;
            Theory2_1Button.Visibility = Visibility.Visible;
            Theory2_2Button.Visibility = Visibility.Visible;
            Theory2_3Button.Visibility = Visibility.Visible;

            EnterButton.Visibility = Visibility.Hidden;
            Theory1Button.Visibility = Visibility.Hidden;
            Theory2Button.Visibility = Visibility.Hidden;
            Theory3Button.Visibility = Visibility.Hidden;
            Theory4Button.Visibility = Visibility.Hidden;
            Theory5Button.Visibility = Visibility.Hidden;
            Testik.Visibility = Visibility.Hidden;
        }

        private void Theory3ButtonClick(object sender, RoutedEventArgs e)
        {
            Enter.Visibility = Visibility.Hidden;
            TestPart1Canvas.Visibility = Visibility.Hidden;
            Socr.Visibility = Visibility.Hidden;

            BackButton.Visibility = Visibility.Visible;
            Theory3_1Button.Visibility = Visibility.Visible;
            Theory3_2Button.Visibility = Visibility.Visible;
            Theory3_3Button.Visibility = Visibility.Visible;

            EnterButton.Visibility = Visibility.Hidden;
            Theory1Button.Visibility = Visibility.Hidden;
            Theory2Button.Visibility = Visibility.Hidden;
            Theory3Button.Visibility = Visibility.Hidden;
            Theory4Button.Visibility = Visibility.Hidden;
            Theory5Button.Visibility = Visibility.Hidden;
            Testik.Visibility = Visibility.Hidden;
        }

        private void Theory4ButtonClick(object sender, RoutedEventArgs e)
        {
            Enter.Visibility = Visibility.Hidden;
            TestPart1Canvas.Visibility = Visibility.Hidden;
            Socr.Visibility = Visibility.Hidden;

            BackButton.Visibility = Visibility.Visible;
            Theory4_1Button.Visibility = Visibility.Visible;
            Theory4_2Button.Visibility = Visibility.Visible;
            Theory4_3Button.Visibility = Visibility.Visible;

            EnterButton.Visibility = Visibility.Hidden;
            Theory1Button.Visibility = Visibility.Hidden;
            Theory2Button.Visibility = Visibility.Hidden;
            Theory3Button.Visibility = Visibility.Hidden;
            Theory4Button.Visibility = Visibility.Hidden;
            Theory5Button.Visibility = Visibility.Hidden;
            Testik.Visibility = Visibility.Hidden;
        }

        private void Theory5ButtonClick(object sender, RoutedEventArgs e)
        {
            Enter.Visibility = Visibility.Hidden;
            TestPart1Canvas.Visibility = Visibility.Hidden;
            Socr.Visibility = Visibility.Hidden;

            BackButton.Visibility = Visibility.Visible;
            Theory5_1Button.Visibility = Visibility.Visible;
            Theory5_2Button.Visibility = Visibility.Visible;
            Theory5_3Button.Visibility = Visibility.Visible;
            Theory5_4Button.Visibility = Visibility.Visible;

            EnterButton.Visibility = Visibility.Hidden;
            Theory1Button.Visibility = Visibility.Hidden;
            Theory2Button.Visibility = Visibility.Hidden;
            Theory3Button.Visibility = Visibility.Hidden;
            Theory4Button.Visibility = Visibility.Hidden;
            Theory5Button.Visibility = Visibility.Hidden;
            Testik.Visibility = Visibility.Hidden;
        }

        private void BackBottonClick(object sender, RoutedEventArgs e)
        {
            BackButton.Visibility = Visibility.Hidden;

            Theory1_1Button.Visibility = Visibility.Hidden;
            Theory1_2Button.Visibility = Visibility.Hidden;
            Theory1_3Button.Visibility = Visibility.Hidden;

            Theory2_1Button.Visibility = Visibility.Hidden;
            Theory2_2Button.Visibility = Visibility.Hidden;
            Theory2_3Button.Visibility = Visibility.Hidden;

            Theory3_1Button.Visibility = Visibility.Hidden;
            Theory3_2Button.Visibility = Visibility.Hidden;
            Theory3_3Button.Visibility = Visibility.Hidden;

            Theory4_1Button.Visibility = Visibility.Hidden;
            Theory4_2Button.Visibility = Visibility.Hidden;
            Theory4_3Button.Visibility = Visibility.Hidden;

            Theory5_1Button.Visibility = Visibility.Hidden;
            Theory5_2Button.Visibility = Visibility.Hidden;
            Theory5_3Button.Visibility = Visibility.Hidden;
            Theory5_4Button.Visibility = Visibility.Hidden;

            EnterButton.Visibility = Visibility.Visible;
            Theory1Button.Visibility = Visibility.Visible;
            Theory2Button.Visibility = Visibility.Visible;
            Theory3Button.Visibility = Visibility.Visible;
            Theory4Button.Visibility = Visibility.Visible;
            Theory5Button.Visibility = Visibility.Visible;
            Testik.Visibility = Visibility.Visible;

            Theory1_1.Visibility = Visibility.Hidden;
            Theory1_2.Visibility = Visibility.Hidden;
            Theory1_3.Visibility = Visibility.Hidden;
            Theory2_1.Visibility = Visibility.Hidden;
            Theory2_2.Visibility = Visibility.Hidden;
            Theory2_3.Visibility = Visibility.Hidden;
            Theory3_1.Visibility = Visibility.Hidden;
            Theory3_2.Visibility = Visibility.Hidden;
            Theory3_3.Visibility = Visibility.Hidden;
            Theory4_1.Visibility = Visibility.Hidden;
            Theory4_2.Visibility = Visibility.Hidden;
            Theory4_3.Visibility = Visibility.Hidden;
            Theory5_1.Visibility = Visibility.Hidden;
            Theory5_2.Visibility = Visibility.Hidden;
            Theory5_3.Visibility = Visibility.Hidden;
            Theory5_4.Visibility = Visibility.Hidden;

            Socr.Visibility = Visibility.Hidden;
            Enter.Visibility = Visibility.Hidden;
            TestPart1Canvas.Visibility = Visibility.Hidden;
        }

        private void SocrButtonClick(object sender, RoutedEventArgs e)
        {
            if (Socr.Visibility == Visibility.Visible) { Socr.Visibility = Visibility.Hidden; }
            else Socr.Visibility = Visibility.Visible;

            Theory1_1.Visibility = Visibility.Hidden;
            Theory1_2.Visibility = Visibility.Hidden;
            Theory1_3.Visibility = Visibility.Hidden;
            Theory2_1.Visibility = Visibility.Hidden;
            Theory2_2.Visibility = Visibility.Hidden;
            Theory2_3.Visibility = Visibility.Hidden;
            Theory3_1.Visibility = Visibility.Hidden;
            Theory3_2.Visibility = Visibility.Hidden;
            Theory3_3.Visibility = Visibility.Hidden;
            Theory4_1.Visibility = Visibility.Hidden;
            Theory4_2.Visibility = Visibility.Hidden;
            Theory4_3.Visibility = Visibility.Hidden;
            Theory5_1.Visibility = Visibility.Hidden;
            Theory5_2.Visibility = Visibility.Hidden;
            Theory5_3.Visibility = Visibility.Hidden;
            Theory5_4.Visibility = Visibility.Hidden;
            Enter.Visibility = Visibility.Hidden;
            TestPart1Canvas.Visibility = Visibility.Hidden;

            Socr.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\Socr.htm"));
        }

        private void Testik_Click(object sender, RoutedEventArgs e)
        {
            logo.Visibility = Visibility.Hidden;
            ChangeVisibilityTest();
            RestartGame();
        }

        private void ChangeVisibilityTest()
        {
            if (TestPart1Canvas.Visibility == Visibility.Visible)
            {
                TestPart1Canvas.Visibility = Visibility.Hidden;
                
                logo.Visibility = Visibility.Visible;
                Theory1Button.Visibility = Visibility.Visible; 
                Theory2Button.Visibility = Visibility.Visible; 
                Theory3Button.Visibility = Visibility.Visible;
                Theory4Button.Visibility = Visibility.Visible;
                Theory5Button.Visibility = Visibility.Visible;
                SocrButton.Visibility = Visibility.Visible;
                EnterButton.Visibility = Visibility.Visible;

                ButtonStartTest.Visibility = Visibility.Hidden;
                ans1.Visibility = Visibility.Hidden; 
                ans2.Visibility = Visibility.Hidden; 
                ans3.Visibility = Visibility.Hidden; 
                ans4.Visibility = Visibility.Hidden;
            }
            else {
                TestPart1Canvas.Visibility = Visibility.Visible;
                Enter.Visibility = Visibility.Hidden;
                Socr.Visibility = Visibility.Hidden;

                ButtonStartTest.Visibility = Visibility.Visible;
                ans1.Visibility = Visibility.Hidden; 
                ans2.Visibility = Visibility.Hidden; 
                ans3.Visibility = Visibility.Hidden; 
                ans4.Visibility = Visibility.Hidden;

            }
        }

        private void RestartGame()
        {
            score = 0;
            qNum = 0;
            i = 0;
            StartGame();
        }

        private void StartGame()
        {
            var randomList = testquestionNumbers.OrderBy(a => Guid.NewGuid()).ToList();
            testquestionNumbers = randomList;
            questionOrder.Content = null;
            ButtonEndTest.Visibility = Visibility.Hidden;

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


        private void NextQuestion()
        {
            txtQuestion.Visibility = Visibility.Visible;
            qImage.Visibility = Visibility.Hidden;
            qImage.Source = new BitmapImage(new Uri("pack://application:,,,/form1.png"));
            ans1.Tag = ""; ans2.Tag = ""; ans3.Tag = ""; ans4.Tag = "";
            if (qNum < testquestionNumbers.Count)
                i = testquestionNumbers[qNum];
            else
            {
                double totalScore;
                totalScore = (float)score / testquestionNumbers.Count;
                totalScore = Math.Round(totalScore, 2);
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

        private void ButtonStartTestClick(object sender, RoutedEventArgs e)
        {
            ans1.Visibility = Visibility.Visible; 
            ans2.Visibility = Visibility.Visible; 
            ans3.Visibility = Visibility.Visible; 
            ans4.Visibility = Visibility.Visible;

            logo.Visibility = Visibility.Hidden;
            Theory1Button.Visibility = Visibility.Hidden; 
            Theory2Button.Visibility = Visibility.Hidden; 
            Theory3Button.Visibility = Visibility.Hidden;
            Theory4Button.Visibility = Visibility.Hidden;
            Theory5Button.Visibility = Visibility.Hidden;
            EnterButton.Visibility = Visibility.Hidden;
            SocrButton.Visibility = Visibility.Hidden;

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
        }

        private void Theory1_1ButtonClick(object sender, RoutedEventArgs e)
        {
            if (Theory1_1.Visibility == Visibility.Visible) { Theory1_1.Visibility = Visibility.Hidden; }
            else Theory1_1.Visibility = Visibility.Visible;

            Theory1_2.Visibility = Visibility.Hidden;
            Theory1_3.Visibility = Visibility.Hidden;
            Socr.Visibility = Visibility.Hidden;

            Theory1_1.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\1_1.htm"));

        }

        private void Theory1_2ButtonClick(object sender, RoutedEventArgs e)
        {
            if (Theory1_2.Visibility == Visibility.Visible) { Theory1_2.Visibility = Visibility.Hidden; }
            else Theory1_2.Visibility = Visibility.Visible;

            Theory1_1.Visibility = Visibility.Hidden;
            Theory1_3.Visibility = Visibility.Hidden;
            Socr.Visibility = Visibility.Hidden;

            Theory1_2.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\1_2.htm"));
        }

        private void Theory1_3ButtonClick(object sender, RoutedEventArgs e)
        {
            if (Theory1_3.Visibility == Visibility.Visible) { Theory1_3.Visibility = Visibility.Hidden; }
            else Theory1_3.Visibility = Visibility.Visible;

            Theory1_2.Visibility = Visibility.Hidden;
            Theory1_1.Visibility = Visibility.Hidden;
            Socr.Visibility = Visibility.Hidden;

            Theory1_3.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\1_3.htm"));
        }

        private void Theory2_1ButtonClick(object sender, RoutedEventArgs e)
        {
            if (Theory2_1.Visibility == Visibility.Visible) { Theory2_1.Visibility = Visibility.Hidden; }
            else Theory2_1.Visibility = Visibility.Visible;

            Theory2_2.Visibility = Visibility.Hidden;
            Theory2_3.Visibility = Visibility.Hidden;
            Socr.Visibility = Visibility.Hidden;

            Theory2_1.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\2_1.htm"));
        }

        private void Theory2_2ButtonClick(object sender, RoutedEventArgs e)
        {
            if (Theory2_2.Visibility == Visibility.Visible) { Theory2_2.Visibility = Visibility.Hidden; }
            else Theory2_2.Visibility = Visibility.Visible;

            Theory2_1.Visibility = Visibility.Hidden;
            Theory2_3.Visibility = Visibility.Hidden;
            Socr.Visibility = Visibility.Hidden;

            Theory2_2.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\2_2.htm"));
        }

        private void Theory2_3ButtonClick(object sender, RoutedEventArgs e)
        {
            if (Theory2_3.Visibility == Visibility.Visible) { Theory2_3.Visibility = Visibility.Hidden; }
            else Theory2_3.Visibility = Visibility.Visible;

            Theory2_2.Visibility = Visibility.Hidden;
            Theory2_1.Visibility = Visibility.Hidden;
            Socr.Visibility = Visibility.Hidden;

            Theory2_3.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\2_3.htm"));
        }

        private void Theory3_1ButtonClick(object sender, RoutedEventArgs e)
        {
            if (Theory3_1.Visibility == Visibility.Visible) { Theory3_1.Visibility = Visibility.Hidden; }
            else Theory3_1.Visibility = Visibility.Visible;

            Theory3_2.Visibility = Visibility.Hidden;
            Theory3_3.Visibility = Visibility.Hidden;
            Socr.Visibility = Visibility.Hidden;

            Theory3_1.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\3_1.htm"));
        }

        private void Theory3_2ButtonClick(object sender, RoutedEventArgs e)
        {
            if (Theory3_2.Visibility == Visibility.Visible) { Theory3_2.Visibility = Visibility.Hidden; }
            else Theory3_2.Visibility = Visibility.Visible;

            Theory3_1.Visibility = Visibility.Hidden;
            Theory3_3.Visibility = Visibility.Hidden;
            Socr.Visibility = Visibility.Hidden;

            Theory3_2.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\3_2.htm"));
        }

        private void Theory3_3ButtonClick(object sender, RoutedEventArgs e)
        {
            if (Theory3_3.Visibility == Visibility.Visible) { Theory3_3.Visibility = Visibility.Hidden; }
            else Theory3_3.Visibility = Visibility.Visible;

            Theory3_2.Visibility = Visibility.Hidden;
            Theory3_1.Visibility = Visibility.Hidden;
            Socr.Visibility = Visibility.Hidden;

            Theory3_3.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\3_3.htm"));
        }

        private void Theory4_1ButtonClick(object sender, RoutedEventArgs e)
        {
            if (Theory4_1.Visibility == Visibility.Visible) { Theory4_1.Visibility = Visibility.Hidden; }
            else Theory4_1.Visibility = Visibility.Visible;

            Theory4_2.Visibility = Visibility.Hidden;
            Theory4_3.Visibility = Visibility.Hidden;
            Socr.Visibility = Visibility.Hidden;

            Theory4_1.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\4_1.htm"));
        }

        private void Theory4_2ButtonClick(object sender, RoutedEventArgs e)
        {
            if (Theory4_2.Visibility == Visibility.Visible) { Theory4_2.Visibility = Visibility.Hidden; }
            else Theory4_2.Visibility = Visibility.Visible;

            Theory4_1.Visibility = Visibility.Hidden;
            Theory4_3.Visibility = Visibility.Hidden;
            Socr.Visibility = Visibility.Hidden;

            Theory4_2.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\4_2.htm"));
        }

        private void Theory4_3ButtonClick(object sender, RoutedEventArgs e)
        {
            if (Theory4_3.Visibility == Visibility.Visible) { Theory4_3.Visibility = Visibility.Hidden; }
            else Theory4_3.Visibility = Visibility.Visible;

            Theory4_1.Visibility = Visibility.Hidden;
            Theory4_2.Visibility = Visibility.Hidden;
            Socr.Visibility = Visibility.Hidden;

            Theory4_3.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\4_3.htm"));
        }

        private void Theory5_1ButtonClick(object sender, RoutedEventArgs e)
        {
            if (Theory5_1.Visibility == Visibility.Visible) { Theory5_1.Visibility = Visibility.Hidden; }
            else Theory5_1.Visibility = Visibility.Visible;

            Theory5_2.Visibility = Visibility.Hidden;
            Theory5_3.Visibility = Visibility.Hidden;
            Theory5_4.Visibility = Visibility.Hidden;
            Socr.Visibility = Visibility.Hidden;

            Theory5_1.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\5_1.htm"));
        }

        private void Theory5_2ButtonClick(object sender, RoutedEventArgs e)
        {
            if (Theory5_2.Visibility == Visibility.Visible) { Theory5_2.Visibility = Visibility.Hidden; }
            else Theory5_2.Visibility = Visibility.Visible;

            Theory5_1.Visibility = Visibility.Hidden;
            Theory5_3.Visibility = Visibility.Hidden;
            Theory5_4.Visibility = Visibility.Hidden;
            Socr.Visibility = Visibility.Hidden;

            Theory5_2.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\5_2.htm"));
        }

        private void Theory5_3ButtonClick(object sender, RoutedEventArgs e)
        {
            if (Theory5_3.Visibility == Visibility.Visible) { Theory5_3.Visibility = Visibility.Hidden; }
            else Theory5_3.Visibility = Visibility.Visible;

            Theory5_1.Visibility = Visibility.Hidden;
            Theory5_2.Visibility = Visibility.Hidden;
            Theory5_4.Visibility = Visibility.Hidden;
            Socr.Visibility = Visibility.Hidden;

            Theory5_3.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\5_3.htm"));
        }

        private void Theory5_4ButtonClick(object sender, RoutedEventArgs e)
        {
            if (Theory5_4.Visibility == Visibility.Visible) { Theory5_4.Visibility = Visibility.Hidden; }
            else Theory5_4.Visibility = Visibility.Visible;

            Theory5_1.Visibility = Visibility.Hidden;
            Theory5_2.Visibility = Visibility.Hidden;
            Theory5_3.Visibility = Visibility.Hidden;
            Socr.Visibility = Visibility.Hidden;

            Theory5_4.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\5_4.htm"));
        }
    }
}
