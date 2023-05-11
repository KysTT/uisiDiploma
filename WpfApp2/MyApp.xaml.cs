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

        int questionNum, i, score;

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
            Header.Width = Width - 16;
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
                MaxHeight = SystemParameters.VirtualScreenHeight - 34; MaxWidth = SystemParameters.VirtualScreenWidth+14;
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
