using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xaml;

namespace Something1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// 
    /// структура интерфейса программы:
    /// 
    /// 
    /// Header - шапка окна, на ней расположилась надпись "Сотовая связь" и элементы меню теория, практика, тест и кнопки управления
    /// 
    /// в элементе меню теория есть 4 подменю теория 1-4 каждая из которых вызывает свой метод
    /// в элементе меню практика есть пока что 1 подменю практика1 
    /// в элементе меню тест сразу же вызывается метод "начать тест"
    /// 
    /// кнопки управления это кнопка "закрыть", "развернуть" и "свернуть"
    /// каждая из кнопок состоит из кружочка и уникальной "текстуры", например у кнопки закрыть это крестик
    /// 
    /// 
    /// в "теле" интерфейса, ниже шапки были определены 3 вида панелей: 
    /// панель с теорией "ContentTheory1";
    /// панель с практикой "ContentPractice"; 
    /// панель с тестом "ContentTest".
    /// 
    /// ContentTheory1 состоит только из браузера типа WebView2 в котором отображается теоретический материал в файлах формата типа .htm
    /// 
    /// в ContentPractice заключены все элементы, необходимые для практических задач, такие как:
    /// Practice1Image - изображение для первого п.з.
    /// кнопки Start- , ReStart- и CheckPracticeButton для начала, перезапуска и проверки п.з. соответственно
    /// полотно PracticeCanvas на котором можно перемещать картинки
    /// картинки MyDraggableImage[1-10] которые были заключены в элемент типа Border для отображения рамки
    /// 
    /// 
    /// 
    /// ContentTest состоит из нескольких типов элементов:
    /// текстовые поля scoreText и txtQuestion для отображения прогресса теста и вопроса соответственно
    /// кнопки ans[1-4], ButtonStartTest, ButtonEndTest отвечающие за выбор ответа [1-4], начало и конец теста соответственно
    /// и изображение qImage для отображение картинок в вопросе
    /// 
    /// </summary>

    public partial class MainWindow : Window
    {
        bool practice1done = false, practice2done = false, practice3done = false;

        int Ful = 2535;
        int Fdl = 2655;
        double Lul, Ldl, Pout_ay = 23, G_ay = 0, Pin_ay = -104, G_bs, L_bs, Pin_bs, G, Pout_bs, ht, hr;


        // вариант студента, необходим для певой практической задачи
        string studentsVariant = "";

        // Список номеров вопросов который в последствии рандомизируется => случайные номера вопросов
        List<int> testquestionNumbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 };

        // activePractice - номер текущего практического задания
        int activePractice = 0;

        ComboBox prevSelected = new();

        // questionNum - номер текущего вопроса
        // i - индекс вопроса в testquestionNumbers
        // score - количество правильных ответов в тесте
        int questionNum, i, score;
        int testTimes = 0;
        DispatcherTimer timer = new DispatcherTimer();
        // myDraggableImage - изображение, которые впоследствии надо будет передвигать
        Border myDraggableImage = new Border();

        public MainWindow()
        {

            Process proc = Process.GetCurrentProcess();
            int count = Process.GetProcesses().Where(p =>
                p.ProcessName == proc.ProcessName).Count();

            if (count > 1)
            {
                MessageBox.Show("Приложение уже запущено");
                App.Current.Shutdown();
            }

            InitializeComponent();

            foreach (UIElement element in Practice2Canvas.Children)
            {

                if (element is ComboBox)
                {
                    ComboBox myElemt = element as ComboBox;

                    myElemt.Items.Add("FTCW OD Cable");
                    myElemt.Items.Add("FOSC Flexi Optical");
                    myElemt.Items.Add("7/16 connector");
                    myElemt.Items.Add("Power Cable");
                }
            }
        }

        // Метод для перетаскивания окна программы мышкой (метод присвоен заголовку окна (Header))
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // При нажатии левой кнопки мыши
            base.OnMouseLeftButtonDown(e);

            DragMove();
        }


        // Метод для изменения ширины и высоты окна
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // костыль для того, что бы меню на заголовке не ездило туда-сюда
            Header.Width = Width - 8;

            if (Practice2ScrollViewer.Visibility == Visibility.Visible)
            {
                Practice2ScrollViewer.Height = Height-120;

                if (Practice2ScrollViewer.Height >= Practice2Image.Height)
                {
                    Practice2ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                }
                else
                {
                    Practice2ScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                }
            }
        }

        // Метод нажатия на кнопку "закрыть" -> закрывает программу 0_о
        private void ButtonCloseClick(object sender, RoutedEventArgs e) => Close();

        // Метод наведения курсора на кнопку "закрыть" показывает "текстуру" кнопки
        private void OnButtonCloseMouseHover(object sender, MouseEventArgs e) => ButtonClosePath.Visibility = Visibility.Visible;

        // Метод ухода курсора с кнопки "закрыть" скрывает "текстуру" кнопки
        private void OnButtonCloseMouseLeave(object sender, MouseEventArgs e) => ButtonClosePath.Visibility = Visibility.Hidden;

        // Метод нажатия на кнопку "развернуть" делает окно на весь экран / в нормальное состояние
        private void ButtonMaximizeClick(object sender, RoutedEventArgs e)
        {
            // Если состояние окна == развернутое (на весь экран), тогда
        if (WindowState == WindowState.Maximized)
                
                // сделать состояение нормальным
                WindowState = WindowState.Normal;
            else
               
                // иначе сделать состояение развернутым
                WindowState = WindowState.Maximized;
        }

        // Метод наведения курсора на кнопку "развернуть" показывает "текстуру" кнопки
        private void OnButtonMaximizeMouseHover(object sender, MouseEventArgs e) => ButtonMaximizePath.Visibility = Visibility.Visible;

        // Метод ухода курсора с кнопки "развернуть" скрывает "текстуру" кнопки
        private void OnButtonMaximizeMouseLeave(object sender, MouseEventArgs e) => ButtonMaximizePath.Visibility = Visibility.Hidden;

        // Метод нажатия на кнопку "скрыть" меняет состояние окна на "свернутое"
        private void ButtonMinimizeClick(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        // Метод наведения курсора на кнопку "скрыть" показывает "текстуру" кнопки
        private void OnButtonMinimizeMouseHover(object sender, MouseEventArgs e) => ButtonMinimizePath.Visibility = Visibility.Visible;

        // Метод ухода курсора с кнопки "скрыть" скрывает "текстуру" кнопки
        private void OnButtonMinimizeMouseLeave(object sender, MouseEventArgs e) => ButtonMinimizePath.Visibility = Visibility.Hidden;

        // Метод нажатия на элемент меню "Теория1", в котором включается отображение окна браузера
        // с загружением в него теоретического материала
        private void Theory1Click(object sender, RoutedEventArgs e)
        {
            // Если браузер TheoryView не виден (скрыт)
            if (TheoryView.Visibility != Visibility.Visible)

                // тогда показать
                TheoryView.Visibility = Visibility.Visible;
            else

            // Если открыт текущий раздел теории
                if (TheoryView.Source == (new Uri(Environment.CurrentDirectory + "\\theory\\Theory1.htm")))
               
                // тогда скрыть браузер
                TheoryView.Visibility = Visibility.Hidden;

            // Если открыт не текущий раздел теории
            if (TheoryView.Source != (new Uri(Environment.CurrentDirectory + "\\theory\\Theory1.htm")))
                
                // тогда открыть нужный
                TheoryView.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\Theory1.htm"));
        }

        // Метод нажатия на элемент меню "Теория2", в котором включается отображение окна браузера
        // с загружением в него теоретического материала
        private void Theory2Click(object sender, RoutedEventArgs e)
        {
            // тоже самое что и в первом случае
            if (TheoryView.Visibility != Visibility.Visible) 
                TheoryView.Visibility = Visibility.Visible;
            else
                if (TheoryView.Source == (new Uri(Environment.CurrentDirectory + "\\theory\\Theory2.htm"))) 
                TheoryView.Visibility = Visibility.Hidden;

            if (TheoryView.Source != (new Uri(Environment.CurrentDirectory + "\\theory\\Theory2.htm"))) 
                TheoryView.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\Theory2.htm"));

        }

        // Метод нажатия на элемент меню "Теория3", в котором включается отображение окна браузера
        // с загружением в него теоретического материала
        private void Theory3Click(object sender, RoutedEventArgs e)
        {
            // тоже самое что и в первом случае

            if (TheoryView.Visibility != Visibility.Visible) 
                TheoryView.Visibility = Visibility.Visible;
            else
                if (TheoryView.Source == (new Uri(Environment.CurrentDirectory + "\\theory\\Theory3.htm"))) 
                TheoryView.Visibility = Visibility.Hidden;

            if (TheoryView.Source != (new Uri(Environment.CurrentDirectory + "\\theory\\Theory3.htm"))) 
                TheoryView.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\Theory3.htm"));
        }

        // Метод нажатия на элемент меню "Теория4", в котором включается отображение окна браузера
        // с загружением в него теоретического материала
        private void Theory4Click(object sender, RoutedEventArgs e)
        {
            // тоже самое что и в первом случае

            if (TheoryView.Visibility != Visibility.Visible) 
                TheoryView.Visibility = Visibility.Visible;
            else
                if (TheoryView.Source == (new Uri(Environment.CurrentDirectory + "\\theory\\Theory4.htm"))) 
                TheoryView.Visibility = Visibility.Hidden;

            if (TheoryView.Source != (new Uri(Environment.CurrentDirectory + "\\theory\\Theory4.htm"))) 
                TheoryView.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\Theory4.htm"));
        }

        // Метод для перетаскивания изображения для п.з.
       private void PracticeImage_Dragging(object sender, DragEventArgs e)
        {
            // dropPosition - получает координаты на PracticeCanvas которые соответствуют 
            // положению указателя мыши
            Point dropPosition = e.GetPosition(Practice1Canvas);

             // Если источником вызова метода является не Полотно (как класс), тогда
            if (e.Source is not Canvas)
            {
                // сохраняем элемент класса Grid во временную переменную tg
                Grid tg = (Grid)((Image)e.Source).Parent;

                // если у myDraggableImage нет имени, то есть он не определен
                if (myDraggableImage.Name == "")

                    // определяем myDraggableImage как элемент класса Border
                    myDraggableImage = (Border)tg.Parent;
                else
                
                // если источником является (навели мышь на другой объект)
                // другой элемент, тогда выходим из метода
                if ((Border)tg.Parent != myDraggableImage)
                    return;

                // задаем положение myDraggableImage относительно курсора
                Canvas.SetLeft(myDraggableImage, dropPosition.X);
                Canvas.SetTop(myDraggableImage, dropPosition.Y);
            }
          else
            {
                // иначе задаем положение верхнего-левого угла элемента сохраненного ранее
                Canvas.SetLeft(myDraggableImage, dropPosition.X);
                Canvas.SetTop(myDraggableImage, dropPosition.Y);
            }
        }

        private void PracticeSelectorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (prevSelected == sender as ComboBox)
                prevSelected = new();

            ComboBox nowSelected = sender as ComboBox;
            Line line = new Line();
            Ellipse ellipse1 = new Ellipse();
            Ellipse ellipse2 = new Ellipse();
            
            ellipse1.Height = ellipse1.Width = 12;
            ellipse2.Height = ellipse2.Width = 12;

            line.StrokeThickness = 5;

            if (nowSelected.SelectedValue == null)
                return;

            if (nowSelected.Tag is null)
                nowSelected.Tag = "selected";

            if (prevSelected.Name == "")
                prevSelected = sender as ComboBox;
            else
            {
                line.X1 = Canvas.GetLeft(prevSelected) + 10;
                line.Y1 = Canvas.GetTop(prevSelected) + 10;
                line.X2 = Canvas.GetLeft(nowSelected) + 10;
                line.Y2 = Canvas.GetTop(nowSelected) + 10;

                if (prevSelected.Tag.ToString() == "selected" && nowSelected.Tag.ToString() == "selected"
                    && prevSelected.SelectedValue.ToString() == "FTCW OD Cable" && nowSelected.SelectedValue.ToString() == "FTCW OD Cable")
                {
                    line.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD800"));
                    ellipse1.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD800"));
                    ellipse2.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD800"));

                    prevSelected.Tag = "connected to " + nowSelected.Name;
                    nowSelected.Tag = "connected to " + prevSelected.Name;

                    //prevSelected.Visibility = Visibility.Collapsed;
                    //nowSelected.Visibility = Visibility.Collapsed;

                    Practice2Canvas.Children.Add(line);
                    
                    Practice2Canvas.Children.Add(ellipse1);
                    Practice2Canvas.Children.Add(ellipse2);
                    Canvas.SetLeft(ellipse1, Canvas.GetLeft(prevSelected) + 5);
                    Canvas.SetTop(ellipse1, Canvas.GetTop(prevSelected) + 5);

                    Canvas.SetLeft(ellipse2, Canvas.GetLeft(nowSelected) + 5);
                    Canvas.SetTop(ellipse2, Canvas.GetTop(nowSelected) + 5);

                    prevSelected = new();
                }

                else
                    if (prevSelected.Tag.ToString() == "selected" && nowSelected.Tag.ToString() == "selected"
                        && prevSelected.SelectedValue.ToString() == "FOSC Flexi Optical" && nowSelected.SelectedValue.ToString() == "FOSC Flexi Optical")
                {
                    line.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B200FF"));
                    ellipse1.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B200FF"));
                    ellipse2.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B200FF"));

                    prevSelected.Tag = "connected to " + nowSelected.Name;
                    nowSelected.Tag = "connected to " + prevSelected.Name;

                    //prevSelected.Visibility = Visibility.Collapsed;
                    //nowSelected.Visibility = Visibility.Collapsed;

                    Practice2Canvas.Children.Add(line);

                    Practice2Canvas.Children.Add(ellipse1);
                    Practice2Canvas.Children.Add(ellipse2);
                    Canvas.SetLeft(ellipse1, Canvas.GetLeft(prevSelected) + 5);
                    Canvas.SetTop(ellipse1, Canvas.GetTop(prevSelected) + 5);

                    Canvas.SetLeft(ellipse2, Canvas.GetLeft(nowSelected) + 5);
                    Canvas.SetTop(ellipse2, Canvas.GetTop(nowSelected) + 5);

                    prevSelected = new();
                }

                else
                    if (prevSelected.Tag.ToString() == "selected" && nowSelected.Tag.ToString() == "selected"
                        && prevSelected.SelectedValue.ToString() == "7/16 connector" && nowSelected.SelectedValue.ToString() == "7/16 connector")
                {
                    line.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0026FF"));
                    ellipse1.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0026FF"));
                    ellipse2.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0026FF"));

                    prevSelected.Tag = "connected to " + nowSelected.Name;
                    nowSelected.Tag = "connected to " + prevSelected.Name;

                    //prevSelected.Visibility = Visibility.Collapsed;
                    //nowSelected.Visibility = Visibility.Collapsed;

                    Practice2Canvas.Children.Add(line);

                    Practice2Canvas.Children.Add(ellipse1);
                    Practice2Canvas.Children.Add(ellipse2);
                    Canvas.SetLeft(ellipse1, Canvas.GetLeft(prevSelected) + 5);
                    Canvas.SetTop(ellipse1, Canvas.GetTop(prevSelected) + 5);

                    Canvas.SetLeft(ellipse2, Canvas.GetLeft(nowSelected) + 5);
                    Canvas.SetTop(ellipse2, Canvas.GetTop(nowSelected) + 5);

                    prevSelected = new();
                }

                else
                    if (prevSelected.Tag.ToString() == "selected" && nowSelected.Tag.ToString() == "selected"
                        && prevSelected.SelectedValue.ToString() == "Power Cable" && nowSelected.SelectedValue.ToString() == "Power Cable")
                {
                    line.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#262626"));
                    ellipse1.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#262626"));
                    ellipse2.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#262626"));

                    prevSelected.Tag = "connected to " + nowSelected.Name;
                    nowSelected.Tag = "connected to " + prevSelected.Name;

                    //prevSelected.Visibility = Visibility.Collapsed;
                    //nowSelected.Visibility = Visibility.Collapsed;

                    Practice2Canvas.Children.Add(line);

                    Practice2Canvas.Children.Add(ellipse1);
                    Practice2Canvas.Children.Add(ellipse2);
                    Canvas.SetLeft(ellipse1, Canvas.GetLeft(prevSelected) + 5);
                    Canvas.SetTop(ellipse1, Canvas.GetTop(prevSelected) + 5);

                    Canvas.SetLeft(ellipse2, Canvas.GetLeft(nowSelected) + 5);
                    Canvas.SetTop(ellipse2, Canvas.GetTop(nowSelected) + 5);

                    prevSelected = new();
                }
                else 
                { 
                    prevSelected = nowSelected;
                }
            }
        }

        // Метод который вызывается при наведении мыши на объект, которому присвоен этот метод
        private void PracticeDraggableImage_MouseMove(object sender, MouseEventArgs e)
        {
            // Если на этот объект нажали левой кнопкой мыши, тогда
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // обнуляем данные в myDraggableImage
                myDraggableImage = new Border();

                // сохраняем этот элемент в локальную переменную myElement
                var myElement = (Border)FindName(((Border)sender).Name);

                // вызываем метод DoDragDrop для начала перетаскивания с параметрами
                // myElement - источник события и данные этого события
                // DragDropEffects.Move - эффект плавного следования за курсором мыши
                DragDrop.DoDragDrop(myElement, myElement, DragDropEffects.Move);
            }
        }

       private void VariantBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ConfirmVariant(sender, e);
        }

        // метод, вызываемый нажатием кнопки "подтвердить"
        // на панели выбора варианта
        private void ConfirmVariant(object sender, RoutedEventArgs e)
        {
            int t;

            // задаем вариант студента равный вписанному
            studentsVariant = VariantBox.Text;

            // если ничего не вписали, оповещаем
            if (studentsVariant == "") 
            { 
                MessageBox.Show("Пустой вариант!");
                return;
            }

            if (! int.TryParse(studentsVariant, out t))
            {
                VariantBox.Text = "";
                MessageBox.Show("Вариант может состоять только из \nцелых чисел от 1 до 3");
                return;
            }

            if (t > 3 || t<1)
            {
                VariantBox.Text = "";
                MessageBox.Show("Вариант может состоять только из \nцелых чисел от 1 до 3");
                return;
            }

            // если вписали, скрываем кнопку "подтвердить"
            // и панель с выбором варианта
            VariantPicker.Visibility = Visibility.Hidden;
            ConfirmVariantButton.Visibility = Visibility.Hidden;

            // и показываем кнопку начала практики
            StartPracticeButton.Visibility = Visibility.Visible;
           
        }

        // Метод вызываемый нажатием по элементу меню "Практика1"
        private void Practice1Click(object sender, RoutedEventArgs e)
        {
            whoLabel.Visibility = Visibility.Hidden;
            ghImage.Visibility = Visibility.Hidden;
            ContentTest.Visibility = Visibility.Hidden;
            Practice2ScrollViewer.Visibility = Visibility.Collapsed;
            Practice3Panel.Visibility = Visibility.Hidden;
            Practice1Canvas.Visibility = Visibility.Hidden;
            
            if (practice1done)
            {
                Practice1Image.Source = new BitmapImage(new Uri("pack://application:,,,/Practice1." + studentsVariant + "Image.jpg"));
                //Practice2Image.Source = new BitmapImage(new Uri("pack://application:,,,/Practice1.1.10.png"));
                Practice1Image.Visibility = Visibility.Visible;
                Practice1Canvas.Visibility = Visibility.Visible;
                Practice1Canvas.IsEnabled = false;
                StartPracticeButton.Visibility= Visibility.Hidden;
                CheckPracticeButton.Visibility = Visibility.Hidden;
                ReStartPracticeButton.Visibility = Visibility.Hidden;
                StartNextPracticeButton.Visibility = Visibility.Visible;
                return;
            }
            
            if (studentsVariant == "") 
            {
                VariantPicker.Visibility = Visibility.Visible;
                ConfirmVariantButton.Visibility = Visibility.Visible;
                StartPracticeButton.Visibility = Visibility.Hidden;
                CheckPracticeButton.Visibility = Visibility.Hidden;
                ReStartPracticeButton.Visibility = Visibility.Hidden;
            }

            // проверяем кто вызвал текущий метод
            // если это кнопка (в будущем кнопка перезапуска), тогда
            if (sender is Button)
            {
                // показываем содержимое панели с п.з.
                ContentPractice.Visibility = Visibility.Visible;
                StartPracticeButton.Visibility = Visibility.Visible;
            }
            else
            {
                // если панель с п.з. скрыта - показываем
                if (ContentPractice.Visibility == Visibility.Hidden)
                    ContentPractice.Visibility = Visibility.Visible;

            }

            // переменной activePractice задаем значение 1 
            // что бы понимать, что сейчас решается 1-ое п.з.
            activePractice = 1;
            
            // Если браузер с теорией отображен на экране, тогда
            // его следует скрыть
            if (TheoryView.Visibility == Visibility.Visible)
                TheoryView.Visibility = Visibility.Hidden;
        }
        
        // Метод вызываемый нажатием по элементу меню "Практика2"
        private void Practice2Click(object sender, RoutedEventArgs e)
        {
            whoLabel.Visibility = Visibility.Hidden;
            ghImage.Visibility = Visibility.Hidden;
            Practice2ScrollViewer.Height = 455;
            ContentTest.Visibility = Visibility.Hidden;
            Practice1Canvas.Visibility = Visibility.Collapsed;
            Practice1Image.Visibility = Visibility.Hidden;
            Practice1Image.Source = new BitmapImage(new Uri("pack://application:,,,/Practice1.1.10.png"));
            Practice3Panel.Visibility = Visibility.Hidden;

            if (practice2done)
            {
                Practice1Image.Source = new BitmapImage(new Uri("pack://application:,,,/Practice1.1.10.png"));
                Practice2Image.Source = new BitmapImage(new Uri("pack://application:,,,/pracScheme.png"));
                Practice2Image.Visibility = Visibility.Visible;
                Practice2Canvas.Visibility = Visibility.Visible;
                Practice2Canvas.IsEnabled = false;
                StartPracticeButton.Visibility = Visibility.Hidden;
                CheckPracticeButton.Visibility = Visibility.Hidden;
                ReStartPracticeButton.Visibility = Visibility.Hidden;
                StartNextPracticeButton.Visibility = Visibility.Visible;
                return;
            }


            VariantPicker.Visibility = Visibility.Hidden;
            ConfirmVariantButton.Visibility = Visibility.Hidden;
            
            // проверяем кто вызвал текущий метод
            // если это кнопка (в будущем кнопка перезапуска), тогда
            if (sender is Button)
                // показываем содержимое панели с п.з.
                ContentPractice.Visibility = Visibility.Visible;
            else
            {
                // если панель с п.з. скрыта - показываем
                if (ContentPractice.Visibility == Visibility.Hidden)
                    ContentPractice.Visibility = Visibility.Visible;
            }
           
            // переменной activePractice задаем значение 2 
            // что бы понимать, что сейчас решается 2-ое п.з.
            activePractice = 2;

            // Если браузер с теорией отображен на экране, тогда
            // его следует скрыть
            if (TheoryView.Visibility == Visibility.Visible)
                TheoryView.Visibility = Visibility.Hidden;

            if (StartPracticeButton.Visibility == Visibility.Hidden)
                StartPracticeButton.Visibility = Visibility.Visible;
        }

        List<int> GetRandomPosition(Random r)
        {
            // создаем локаольную переменную r класса Random

            int rIntTop, rIntLeft;

            // так как размер картинок у каждого варианта разный
            // укажем диапазон значений
            if (studentsVariant == "1")
            {
                // относительно верхней границы
                rIntTop = r.Next(0, 375);

                // относительно левой границы
                rIntLeft = r.Next(0, 400);
            }
            else
            {
                if (studentsVariant == "2")
                {
                    rIntTop = r.Next(0, 375);
                    rIntLeft = r.Next(0, 800);
                }
                else
                {
                    rIntTop = r.Next(0, 375);
                    rIntLeft = r.Next(0, 650);
                }
            }
            List<int> rInts = new() { rIntTop, rIntLeft };

            return rInts;
        }

        private void ShowImagesPractice1Variant1(Border cb, Label cl, Image ci)
        {
            List<string> Labels = new List<string> { "База данных" + "\n" + " VLR", "Аутентификация" + "\n" + " AUC", "Шлюз GGSN", "База данных" + "\n" + " HLR", "Медиа шлюз", "Коммутатор " + "\n" + " MSC",
                                                                "Базовая"+ "\n" +"станция", "Базовая" + "\n" + "станция", "Контроллер " + "\n" + " BSC", "Коммутатор" + "\n" + "SGSN"};

            // установим максимальную высоту 80 пикселей
            cb.MaxHeight = 80;

            cl.Content = Labels[int.Parse(ci.Name.ToString().Last().ToString())];

            // если последний символ в имени элемента ci 
            // равен '0', тогда это десятый элемент
            if (ci.Name.ToString().Last() == '0')

                // поэтому источником изображения будет 1.1.10
                ci.Source = new BitmapImage(new Uri("pack://application:,,,/Practice1.1.10.png"));

            else
                //иначе просто берем последнюю цифру
                ci.Source = new BitmapImage(new Uri("pack://application:,,,/Practice1.1." + ci.Name.ToString().Last() + ".png"));

            // так как картинки 6 и 7 не стандартного размера
            // зададим им параметр высоты отдельно
            if (ci.Name.ToString().Last() == '6' || ci.Name.ToString().Last() == '7')
            {
                cl.Margin = new Thickness(0, 0, 0, -30);
                cb.MaxHeight = 95;
            }

            if (ci.Name.ToString().Last() == '2')
            {
                cb.MaxHeight = 75;
            }

            if (ci.Name.ToString().Last() == '3' || ci.Name.ToString().Last() == '9' || ci.Name.ToString().Last() == '0'
                || ci.Name.ToString().Last() == '1' || ci.Name.ToString().Last() == '5' || ci.Name.ToString().Last() == '8')
            {
                cl.Margin = new Thickness(0, 0, 0, -30);
            }
        }

        private void ShowImagesPractice1Variant2(Border cb, Label cl, Image ci) 
        {
            List<string> Labels = new List<string> { "",
                                                                "Сервер" + "\n" + "аутентификации", "Управление" + "\n" + "данными", "Управление" + "\n" + "доступом", "Управление" + "\n" + "сеансом",
                                                                "Управление" + "\n" +"политикой", "Прикладная" + "\n" +"функция", "Плоскость" + "\n" + "пользователя", "",
                                                                ""};

            cl.Content = Labels[int.Parse(ci.Name.ToString().Last().ToString())];
            cl.Margin = new Thickness(0, 0, 0, -30);

            cb.MaxHeight = 60;

            if (ci.Name.ToString().Last() == '9')
            {
                ci.Source = new BitmapImage(new Uri("pack://application:,,,/Practice1.2.9.png"));
                cb.MaxHeight = 100;
            }
            else
                ci.Source = new BitmapImage(new Uri("pack://application:,,,/Practice1.2." + ci.Name.ToString().Last() + ".png"));

            if (ci.Name.ToString().Last() == '8')
            {
                cb.MaxHeight = 120;
            }
        }

        private void ShowImagesPractice1Variant3(Border cb, Label cl, Image ci)
        {

            List<string> Labels = new List<string> { "", "Базовая" + "\n" + "станция", "Узел управления" + "\n" + "мобильностью", "Обслуживающий" + "\n" + " шлюз",
                                                                    "Публичный" + "\n" + " шлюз", "Узел выставления" + "\n" + "счетов", "Абонентские"+ "\n" +"данные"};

            cl.Content = Labels[int.Parse(ci.Name.ToString().Last().ToString())];
            cl.Margin = new Thickness(0, 0, 0, -30);

            cb.MaxHeight = 110;

            if (ci.Name.ToString().Last() == '6')
            {
                cb.MaxHeight = 90;
                ci.Source = new BitmapImage(new Uri("pack://application:,,,/Practice1.3.6.png"));
            }
            else
                ci.Source = new BitmapImage(new Uri("pack://application:,,,/Practice1.3." + ci.Name.ToString().Last() + ".png"));

            if (ci.Name.ToString().Last() == '1')
            {
                cb.MaxHeight = 130;
            }
        }

        private void ShowImagesPractice1()
        {
            // создаем локаольную переменную r класса Random
            Random r = new();

            // выставляем свойство полотна с картинками 
            // "разрешить перетаскивание"
            Practice1Canvas.AllowDrop = true;

            // показываем необходимые элементы

            Practice1Image.Source = new BitmapImage(new Uri("pack://application:,,,/Practice1." + studentsVariant + "Image.jpg"));

            Practice1Image.Visibility = Visibility.Visible;
            Practice1Canvas.Visibility = Visibility.Visible;

            // так как интресенее когда картинки разбросаны случайно
            // напишем рандомайзер положения картинок на полотне
            foreach (UIElement element in Practice1Canvas.Children)
            {
                List<int> rInts = GetRandomPosition(r);

                // если текущий элемент - типа Border
                if (element is Border)
                {
                    // тогда задаем его положение с помощью rIntTop и rIntLeft
                    Canvas.SetTop(element, rInts[0]);
                    Canvas.SetLeft(element, rInts[1]);

                    // создаем локальную переменную cb класса Border с параметрами найденного элемента
                    Border cb = (Border)element;

                    // у этого элемента меняем видимость на "Visible"
                    // и делаем прозрачным границы (на случай если уже начиналась практика)
                    cb.Visibility = Visibility.Visible;
                    cb.BorderBrush = null;

                    // получаем текущий элемент типа Image 
                    Grid cg = (Grid)cb.Child;
                    Image ci = (Image)cg.Children[0];
                    Label cl = (Label)cg.Children[1];

                    // у каждого варианта свои параметры изображений
                    if (studentsVariant == "1")
                    {
                        ShowImagesPractice1Variant1(cb, cl, ci);
                    }

                    if (studentsVariant == "2")
                    {
                        if (ci.Name.ToString().Last() == '0')
                            break;

                        ShowImagesPractice1Variant2(cb, cl, ci);
                    }

                    if (studentsVariant == "3")
                    {
                        if (ci.Name.ToString().Last() == '7')
                            break;

                        ShowImagesPractice1Variant3(cb, cl, ci);
                    }
                }
            }
        }

        private void ShowThingsPractice2()
        {
            Practice2Image.Source = new BitmapImage(new Uri("pack://application:,,,/pracScheme.png"));

            Practice2Image.Visibility = Visibility.Visible;
            Practice2Canvas.Visibility = Visibility.Visible;
            Practice2Canvas.Width = Practice2Image.Width;
            Practice2ScrollViewer.Visibility = Visibility.Visible;

            foreach (UIElement element in Practice2Canvas.Children)
            {
                if (element is ComboBox)
                {
                    element.Visibility = Visibility.Visible;
                }
            }
        }

        // Метод нажатия по кнопке "начать практику"
        private void StartPracticeClick(object sender, RoutedEventArgs e)
        {
            // если сейчас решается 1-ое п.з.
            if (activePractice == 1)
                ShowImagesPractice1();

            if (activePractice == 2)
            {
                ShowThingsPractice2();
            }
            // так как уже начали выполнять п.з. скрываем кнопку старта
            // и показываем кнопку "проверить"
            StartPracticeButton.Visibility = Visibility.Hidden;
            CheckPracticeButton.Visibility = Visibility.Visible;
        }

        int CalculatePractice1ScoreVariant1()
        {
            // локальная переменная score - счетчик правильных ответов
            var score = 0;

            // если картинка расположена на определенном участке полотна - выделяем зеленым, засчитываем как верно
            if (342 <= Canvas.GetLeft(PracticeDraggableImage1) && (Canvas.GetLeft(PracticeDraggableImage1) <= 415)
                && (163 <= Canvas.GetTop(PracticeDraggableImage1)) && (Canvas.GetTop(PracticeDraggableImage1) <= 200)
                || (370 <= Canvas.GetLeft(PracticeDraggableImage1) && (Canvas.GetLeft(PracticeDraggableImage1) <= 420)
                && (65 <= Canvas.GetTop(PracticeDraggableImage1)) && (Canvas.GetTop(PracticeDraggableImage1) <= 120))
                || (300 <= Canvas.GetLeft(PracticeDraggableImage1) && (Canvas.GetLeft(PracticeDraggableImage1) <= 360)
                && (5 <= Canvas.GetTop(PracticeDraggableImage1)) && (Canvas.GetTop(PracticeDraggableImage1) <= 60)))
            {
                PracticeDraggableImage1.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                score++;
            }

            // иначе выделяем красным
            else
                PracticeDraggableImage1.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

            // ниже все тоже самое
            if (0 <= Canvas.GetLeft(PracticeDraggableImage2) && (Canvas.GetLeft(PracticeDraggableImage2) <= 40)
                && (105 <= Canvas.GetTop(PracticeDraggableImage2)) && (Canvas.GetTop(PracticeDraggableImage2) <= 135))
            {
                PracticeDraggableImage2.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                score++;
            }
            else
                PracticeDraggableImage2.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

            if (342 <= Canvas.GetLeft(PracticeDraggableImage3) && (Canvas.GetLeft(PracticeDraggableImage3) <= 415)
                && (163 <= Canvas.GetTop(PracticeDraggableImage3)) && (Canvas.GetTop(PracticeDraggableImage3) <= 200)
                || (370 <= Canvas.GetLeft(PracticeDraggableImage3) && (Canvas.GetLeft(PracticeDraggableImage3) <= 420)
                && (65 <= Canvas.GetTop(PracticeDraggableImage3)) && (Canvas.GetTop(PracticeDraggableImage3) <= 120))
                || (300 <= Canvas.GetLeft(PracticeDraggableImage3) && (Canvas.GetLeft(PracticeDraggableImage3) <= 360)
                && (5 <= Canvas.GetTop(PracticeDraggableImage3)) && (Canvas.GetTop(PracticeDraggableImage3) <= 60)))
            {
                PracticeDraggableImage3.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                score++;
            }
            else
                PracticeDraggableImage3.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

            if (165 <= Canvas.GetLeft(PracticeDraggableImage4) && (Canvas.GetLeft(PracticeDraggableImage4) <= 216)
                && (129 <= Canvas.GetTop(PracticeDraggableImage4)) && (Canvas.GetTop(PracticeDraggableImage4) <= 164))
            {
                PracticeDraggableImage4.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                score++;
            }
            else
                PracticeDraggableImage4.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

            if (276 <= Canvas.GetLeft(PracticeDraggableImage5) && (Canvas.GetLeft(PracticeDraggableImage5) <= 315)
                && (133 <= Canvas.GetTop(PracticeDraggableImage5)) && (Canvas.GetTop(PracticeDraggableImage5) <= 156))
            {
                PracticeDraggableImage5.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                score++;
            }
            else
                PracticeDraggableImage5.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

            if (240 <= Canvas.GetLeft(PracticeDraggableImage6) && (Canvas.GetLeft(PracticeDraggableImage6) <= 270)
                && (280 <= Canvas.GetTop(PracticeDraggableImage6)) && (Canvas.GetTop(PracticeDraggableImage6) <= 356)
                || (110 <= Canvas.GetLeft(PracticeDraggableImage6) && (Canvas.GetLeft(PracticeDraggableImage6) <= 152)
                && (240 <= Canvas.GetTop(PracticeDraggableImage6)) && (Canvas.GetTop(PracticeDraggableImage6) <= 341)))
            {
                PracticeDraggableImage6.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                score++;
            }
            else
                PracticeDraggableImage6.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

            if (240 <= Canvas.GetLeft(PracticeDraggableImage7) && (Canvas.GetLeft(PracticeDraggableImage7) <= 270)
                && (280 <= Canvas.GetTop(PracticeDraggableImage7)) && (Canvas.GetTop(PracticeDraggableImage7) <= 356)
                || (110 <= Canvas.GetLeft(PracticeDraggableImage7) && (Canvas.GetLeft(PracticeDraggableImage7) <= 152)
                && (240 <= Canvas.GetTop(PracticeDraggableImage7)) && (Canvas.GetTop(PracticeDraggableImage7) <= 341)))
            {
                PracticeDraggableImage7.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                score++;
            }
            else
                PracticeDraggableImage7.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

            if (162 <= Canvas.GetLeft(PracticeDraggableImage8) && (Canvas.GetLeft(PracticeDraggableImage8) <= 210)
                && (222 <= Canvas.GetTop(PracticeDraggableImage8)) && (Canvas.GetTop(PracticeDraggableImage8) <= 260))
            {
                PracticeDraggableImage8.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                score++;
            }
            else
                PracticeDraggableImage8.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

            if (75 <= Canvas.GetLeft(PracticeDraggableImage9) && (Canvas.GetLeft(PracticeDraggableImage9) <= 105)
                && (160 <= Canvas.GetTop(PracticeDraggableImage9)) && (Canvas.GetTop(PracticeDraggableImage9) <= 200))
            {
                PracticeDraggableImage9.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                score++;
            }
            else
                PracticeDraggableImage9.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

            if (342 <= Canvas.GetLeft(PracticeDraggableImage10) && (Canvas.GetLeft(PracticeDraggableImage10) <= 415)
                && (163 <= Canvas.GetTop(PracticeDraggableImage10)) && (Canvas.GetTop(PracticeDraggableImage10) <= 200)
                || (370 <= Canvas.GetLeft(PracticeDraggableImage10) && (Canvas.GetLeft(PracticeDraggableImage10) <= 420)
                && (65 <= Canvas.GetTop(PracticeDraggableImage10)) && (Canvas.GetTop(PracticeDraggableImage10) <= 120))
                || (300 <= Canvas.GetLeft(PracticeDraggableImage10) && (Canvas.GetLeft(PracticeDraggableImage10) <= 360)
                && (5 <= Canvas.GetTop(PracticeDraggableImage10)) && (Canvas.GetTop(PracticeDraggableImage10) <= 60)))
            {
                PracticeDraggableImage10.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                score++;
            }
            else
                PracticeDraggableImage10.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

            return score;
        }

        int CalculatePractice1ScoreVariant2()
        {
            // локальная переменная score - счетчик правильных ответов
            var score = 0;

            // если картинка расположена на определенном участке полотна
            if (231 <= Canvas.GetLeft(PracticeDraggableImage1) && (Canvas.GetLeft(PracticeDraggableImage1) <= 276)
                && (0 <= Canvas.GetTop(PracticeDraggableImage1)) && (Canvas.GetTop(PracticeDraggableImage1) <= 15))
            {
                // выделяем зеленым
                PracticeDraggableImage1.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));

                // увеличиваем очки
                score++;
            }

            else
                // иначе выделяем красным
                PracticeDraggableImage1.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

            // ниже все тоже самое
            if (342 <= Canvas.GetLeft(PracticeDraggableImage2) && (Canvas.GetLeft(PracticeDraggableImage2) <= 372)
                && (0 <= Canvas.GetTop(PracticeDraggableImage2)) && (Canvas.GetTop(PracticeDraggableImage2) <= 15))
            {
                PracticeDraggableImage2.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                score++;
            }
            else
                PracticeDraggableImage2.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

            if (240 <= Canvas.GetLeft(PracticeDraggableImage3) && (Canvas.GetLeft(PracticeDraggableImage3) <= 270)
                && (112 <= Canvas.GetTop(PracticeDraggableImage3)) && (Canvas.GetTop(PracticeDraggableImage3) <= 132))
            {
                PracticeDraggableImage3.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                score++;
            }
            else
                PracticeDraggableImage3.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

            if (343 <= Canvas.GetLeft(PracticeDraggableImage4) && (Canvas.GetLeft(PracticeDraggableImage4) <= 377)

                && (112 <= Canvas.GetTop(PracticeDraggableImage4)) && (Canvas.GetTop(PracticeDraggableImage4) <= 132))
            {
                PracticeDraggableImage4.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                score++;
            }
            else
                PracticeDraggableImage4.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

            if (462 <= Canvas.GetLeft(PracticeDraggableImage5) && (Canvas.GetLeft(PracticeDraggableImage5) <= 492)
                && (112 <= Canvas.GetTop(PracticeDraggableImage5)) && (Canvas.GetTop(PracticeDraggableImage5) <= 132))
            {
                PracticeDraggableImage5.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                score++;
            }
            else
                PracticeDraggableImage5.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

            if (560 <= Canvas.GetLeft(PracticeDraggableImage6) && (Canvas.GetLeft(PracticeDraggableImage6) <= 590)
                && (112 <= Canvas.GetTop(PracticeDraggableImage6)) && (Canvas.GetTop(PracticeDraggableImage6) <= 132))
            {
                PracticeDraggableImage6.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                score++;
            }
            else
                PracticeDraggableImage6.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

            if (330 <= Canvas.GetLeft(PracticeDraggableImage7) && (Canvas.GetLeft(PracticeDraggableImage7) <= 364)
                && (226 <= Canvas.GetTop(PracticeDraggableImage7)) && (Canvas.GetTop(PracticeDraggableImage7) <= 246))
            {
                PracticeDraggableImage7.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                score++;
            }
            else
                PracticeDraggableImage7.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

            if (708 <= Canvas.GetLeft(PracticeDraggableImage8) && (Canvas.GetLeft(PracticeDraggableImage8) <= 773)
                && (255 <= Canvas.GetTop(PracticeDraggableImage8)) && (Canvas.GetTop(PracticeDraggableImage8) <= 318))
            {
                PracticeDraggableImage8.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                score++;
            }
            else
                PracticeDraggableImage8.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

            if (86 <= Canvas.GetLeft(PracticeDraggableImage9) && (Canvas.GetLeft(PracticeDraggableImage9) <= 142)
                && (258 <= Canvas.GetTop(PracticeDraggableImage9)) && (Canvas.GetTop(PracticeDraggableImage9) <= 335))
            {
                PracticeDraggableImage9.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                score++;
            }
            else
                PracticeDraggableImage9.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

            return score;
        }

        int CalculatePractice1ScoreVariant3()
        {
            // локальная переменная score - счетчик правильных ответов
            var score = 0;

            // если картинка расположена на определенном участке полотна

            if (42 <= Canvas.GetLeft(PracticeDraggableImage1) && (Canvas.GetLeft(PracticeDraggableImage1) <= 103)
                && (228 <= Canvas.GetTop(PracticeDraggableImage1)) && (Canvas.GetTop(PracticeDraggableImage1) <= 290))
            {
                // выделяем зеленым
                PracticeDraggableImage1.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));

                // увеличиваем счетчик верных ответов
                score++;
            }

            else
                // иначе выделяем красным
                PracticeDraggableImage1.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

            // ниже все тоже самое
            if (475<= Canvas.GetLeft(PracticeDraggableImage2) && (Canvas.GetLeft(PracticeDraggableImage2) <= 521)
                && (74 <= Canvas.GetTop(PracticeDraggableImage2)) && (Canvas.GetTop(PracticeDraggableImage2) <= 132))
            {
                PracticeDraggableImage2.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                score++;
            }
            else
                PracticeDraggableImage2.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

            if (265 <= Canvas.GetLeft(PracticeDraggableImage3) && (Canvas.GetLeft(PracticeDraggableImage3) <= 340)
                && (186 <= Canvas.GetTop(PracticeDraggableImage3)) && (Canvas.GetTop(PracticeDraggableImage3) <= 269))
            {
                PracticeDraggableImage3.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                score++;
            }
            else
                PracticeDraggableImage3.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

            if (362 <= Canvas.GetLeft(PracticeDraggableImage4) && (Canvas.GetLeft(PracticeDraggableImage4) <= 422)
                && (190 <= Canvas.GetTop(PracticeDraggableImage4)) && (Canvas.GetTop(PracticeDraggableImage4) <= 277))
            {
                PracticeDraggableImage4.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                score++;
            }
            else
                PracticeDraggableImage4.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

            if (163 <= Canvas.GetLeft(PracticeDraggableImage5) && (Canvas.GetLeft(PracticeDraggableImage5) <= 218)
                && (69<= Canvas.GetTop(PracticeDraggableImage5)) && (Canvas.GetTop(PracticeDraggableImage5) <= 125))
            {
                PracticeDraggableImage5.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                score++;
            }
            else
                PracticeDraggableImage5.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

            if (334 <= Canvas.GetLeft(PracticeDraggableImage6) && (Canvas.GetLeft(PracticeDraggableImage6) <= 410)
                && (14 <= Canvas.GetTop(PracticeDraggableImage6)) && (Canvas.GetTop(PracticeDraggableImage6) <= 75))
            {
                PracticeDraggableImage6.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                score++;
            }
            else
                PracticeDraggableImage6.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

            return score;
        }

        private void CalculatePractice1TotalScore()
        {
            // далле по вариантам
            if (studentsVariant == "1")
            {
                // запрещаем пользователю передвигать картинки
                Practice1Canvas.AllowDrop = false;

                // если после проверки все 10 картинок оказались на правильных местах
                // засчитываем п.з. 1
                if (CalculatePractice1ScoreVariant1() == 10)
                {
                    practice1done = true;
                    activePractice++;
                    StartNextPracticeButton.Visibility = Visibility.Visible;
                }
                else
                {
                    ReStartPracticeButton.Visibility = Visibility.Visible;
                }
            }

            if (studentsVariant == "2")
            {
                // запрещаем пользователю передвигать картинки
                Practice1Canvas.AllowDrop = false;

                // если после проверки все 9 картинок оказались на правильных местах
                // засчитываем п.з. 1

                if (CalculatePractice1ScoreVariant2() == 9)
                {
                    practice1done = true;
                    activePractice++;
                    StartNextPracticeButton.Visibility = Visibility.Visible;
                }
                else
                {
                    ReStartPracticeButton.Visibility = Visibility.Visible;
                }
            }

            if (studentsVariant == "3")
            {
                // запрещаем пользователю передвигать картинки
                Practice1Canvas.AllowDrop = false;

                // если после проверки все 6 картинок оказались на правильных местах
                // засчитываем п.з. 1
                if (CalculatePractice1ScoreVariant3() == 6)
                {
                    practice1done = true;
                    activePractice++;
                    StartNextPracticeButton.Visibility = Visibility.Visible;
                }
                else
                {
                    ReStartPracticeButton.Visibility = Visibility.Visible;
                }
            }

            if (practice1done && practice2done && practice3done)
            {
                TestMenu.Visibility = Visibility.Visible;
            }
        }

        private void CalculatePractice2Score()
        {
            // вот эта абоминация снизу - 
            // страшная сказка на ночь

            Ellipse RWellipse1 = new Ellipse();
            Ellipse RWellipse2 = new Ellipse();
            Ellipse RWellipse3 = new Ellipse();
            Ellipse RWellipse4 = new Ellipse();
            Ellipse RWellipse5 = new Ellipse();
            Ellipse RWellipse6 = new Ellipse();
            Ellipse RWellipse7 = new Ellipse();
            Ellipse RWellipse8 = new Ellipse();
            Ellipse RWellipse9 = new Ellipse();
            Ellipse RWellipseA = new Ellipse();
            Ellipse RWellipseB = new Ellipse();
            Ellipse RWellipseC = new Ellipse();
            Ellipse RWellipseD = new Ellipse();
            Ellipse RWellipseE = new Ellipse();
            Ellipse RWellipseDC1 = new Ellipse();
            Ellipse RWellipseDC2 = new Ellipse();
            Ellipse RWellipseDC3 = new Ellipse();
            Ellipse RWellipseDC4 = new Ellipse();
            Ellipse RWellipseEq1 = new Ellipse();
            Ellipse RWellipseEq2 = new Ellipse();
            Ellipse RWellipseEq3 = new Ellipse();
            Ellipse RWellipseEq4 = new Ellipse();
            Ellipse RWellipseIBP1 = new Ellipse();
            Ellipse RWellipseIBP2 = new Ellipse();
            Ellipse RWellipseIBP3 = new Ellipse();
            Ellipse RWellipseIBP4 = new Ellipse();
            Ellipse RWellipsePilot1 = new Ellipse();
            Ellipse RWellipsePilot2 = new Ellipse();
            Ellipse RWellipsePilot3 = new Ellipse();
            Ellipse RWellipsePilot4 = new Ellipse();

            if (true) 
            { 

                RWellipse1.Width = RWellipse1.Height = RWellipse2.Width = RWellipse2.Height =
                RWellipse3.Width = RWellipse3.Height = RWellipse4.Width = RWellipse4.Height =
                RWellipse5.Width = RWellipse5.Height = RWellipse6.Width = RWellipse6.Height =
                RWellipse7.Width = RWellipse7.Height = RWellipse8.Width = RWellipse8.Height =
                RWellipse9.Width = RWellipse9.Height = RWellipseA.Width = RWellipseA.Height =
                RWellipseB.Width = RWellipseB.Height = RWellipseC.Width = RWellipseC.Height =
                RWellipseD.Width = RWellipseD.Height = RWellipseE.Width = RWellipseE.Height =
                RWellipseDC1.Width = RWellipseDC1.Height = RWellipseDC2.Width = RWellipseDC2.Height =
                RWellipseDC3.Width = RWellipseDC3.Height = RWellipseDC4.Width = RWellipseDC4.Height =
                RWellipseEq1.Width = RWellipseEq1.Height = RWellipseEq2.Width = RWellipseEq2.Height =
                RWellipseEq3.Width = RWellipseEq3.Height = RWellipseEq4.Width = RWellipseEq4.Height =
                RWellipseIBP1.Width = RWellipseIBP1.Height = RWellipseIBP2.Width = RWellipseIBP2.Height =
                RWellipseIBP3.Width = RWellipseIBP3.Height = RWellipseIBP4.Width = RWellipseIBP4.Height =
                RWellipsePilot1.Width = RWellipsePilot1.Height = RWellipsePilot2.Width = RWellipsePilot2.Height =
                RWellipsePilot3.Width = RWellipsePilot3.Height = RWellipsePilot4.Width = RWellipsePilot4.Height = 20;

                RWellipse1.StrokeThickness = RWellipse2.StrokeThickness = RWellipse3.StrokeThickness = RWellipse4.StrokeThickness =
                RWellipse5.StrokeThickness = RWellipse6.StrokeThickness = RWellipse7.StrokeThickness = RWellipse8.StrokeThickness =
                RWellipse9.StrokeThickness = RWellipseA.StrokeThickness = RWellipseB.StrokeThickness = RWellipseC.StrokeThickness =
                RWellipseDC1.StrokeThickness = RWellipseDC2.StrokeThickness = RWellipseDC3.StrokeThickness = RWellipseDC4.StrokeThickness =
                RWellipseEq1.StrokeThickness = RWellipseEq2.StrokeThickness = RWellipseEq3.StrokeThickness = RWellipseEq4.StrokeThickness =
                RWellipseIBP1.StrokeThickness = RWellipseIBP2.StrokeThickness = RWellipseIBP3.StrokeThickness = RWellipseIBP4.StrokeThickness =
                RWellipsePilot1.StrokeThickness = RWellipsePilot2.StrokeThickness = RWellipsePilot3.StrokeThickness = RWellipsePilot4.StrokeThickness =
                RWellipseD.StrokeThickness = RWellipseE.StrokeThickness = 2;

                Practice2Canvas.Children.Add(RWellipse1);
                Practice2Canvas.Children.Add(RWellipse2);
                Practice2Canvas.Children.Add(RWellipse3);
                Practice2Canvas.Children.Add(RWellipse4);
                Practice2Canvas.Children.Add(RWellipse5);
                Practice2Canvas.Children.Add(RWellipse6);
                Practice2Canvas.Children.Add(RWellipse7);
                Practice2Canvas.Children.Add(RWellipse8);
                Practice2Canvas.Children.Add(RWellipse9);
                Practice2Canvas.Children.Add(RWellipseA);
                Practice2Canvas.Children.Add(RWellipseB);
                Practice2Canvas.Children.Add(RWellipseC);
                Practice2Canvas.Children.Add(RWellipseD);
                Practice2Canvas.Children.Add(RWellipseE);
                Practice2Canvas.Children.Add(RWellipseDC1);
                Practice2Canvas.Children.Add(RWellipseDC2);
                Practice2Canvas.Children.Add(RWellipseDC3);
                Practice2Canvas.Children.Add(RWellipseDC4);
                Practice2Canvas.Children.Add(RWellipseEq1);
                Practice2Canvas.Children.Add(RWellipseEq2);
                Practice2Canvas.Children.Add(RWellipseEq3);
                Practice2Canvas.Children.Add(RWellipseEq4);
                Practice2Canvas.Children.Add(RWellipseIBP1);
                Practice2Canvas.Children.Add(RWellipseIBP2);
                Practice2Canvas.Children.Add(RWellipseIBP3);
                Practice2Canvas.Children.Add(RWellipseIBP4);
                Practice2Canvas.Children.Add(RWellipsePilot1);
                Practice2Canvas.Children.Add(RWellipsePilot2);
                Practice2Canvas.Children.Add(RWellipsePilot3);
                Practice2Canvas.Children.Add(RWellipsePilot4);

                Canvas.SetLeft(RWellipse1, Canvas.GetLeft(PracticeSelector1) );
                Canvas.SetLeft(RWellipse2, Canvas.GetLeft(PracticeSelector2));
                Canvas.SetLeft(RWellipse3, Canvas.GetLeft(PracticeSelector3));
                Canvas.SetLeft(RWellipse4, Canvas.GetLeft(PracticeSelector4));
                Canvas.SetLeft(RWellipse5, Canvas.GetLeft(PracticeSelector5));
                Canvas.SetLeft(RWellipse6, Canvas.GetLeft(PracticeSelector6));
                Canvas.SetLeft(RWellipse7, Canvas.GetLeft(PracticeSelector7));
                Canvas.SetLeft(RWellipse8, Canvas.GetLeft(PracticeSelector8));
                Canvas.SetLeft(RWellipse9, Canvas.GetLeft(PracticeSelector9));
                Canvas.SetLeft(RWellipseA, Canvas.GetLeft(PracticeSelectorA));
                Canvas.SetLeft(RWellipseB, Canvas.GetLeft(PracticeSelectorB));
                Canvas.SetLeft(RWellipseC, Canvas.GetLeft(PracticeSelectorC));
                Canvas.SetLeft(RWellipseD, Canvas.GetLeft(PracticeSelectorD));
                Canvas.SetLeft(RWellipseE, Canvas.GetLeft(PracticeSelectorE));
                Canvas.SetLeft(RWellipseDC1, Canvas.GetLeft(PracticeSelectorVoltageDC1));
                Canvas.SetLeft(RWellipseDC2, Canvas.GetLeft(PracticeSelectorVoltageDC2));
                Canvas.SetLeft(RWellipseDC3, Canvas.GetLeft(PracticeSelectorVoltageDC3));
                Canvas.SetLeft(RWellipseDC4, Canvas.GetLeft(PracticeSelectorVoltageDC4));
                Canvas.SetLeft(RWellipseEq1, Canvas.GetLeft(PracticeSelectorVoltageEq1));
                Canvas.SetLeft(RWellipseEq2, Canvas.GetLeft(PracticeSelectorVoltageEq2));
                Canvas.SetLeft(RWellipseEq3, Canvas.GetLeft(PracticeSelectorVoltageEq3));
                Canvas.SetLeft(RWellipseEq4, Canvas.GetLeft(PracticeSelectorVoltageEq4));
                Canvas.SetLeft(RWellipseIBP1, Canvas.GetLeft(PracticeSelectorVoltageIBP1));
                Canvas.SetLeft(RWellipseIBP2, Canvas.GetLeft(PracticeSelectorVoltageIBP2));
                Canvas.SetLeft(RWellipseIBP3, Canvas.GetLeft(PracticeSelectorVoltageIBP3));
                Canvas.SetLeft(RWellipseIBP4, Canvas.GetLeft(PracticeSelectorVoltageIBP4));
                Canvas.SetLeft(RWellipsePilot1, Canvas.GetLeft(PracticeSelectorVoltagePilot1));
                Canvas.SetLeft(RWellipsePilot2, Canvas.GetLeft(PracticeSelectorVoltagePilot2));
                Canvas.SetLeft(RWellipsePilot3, Canvas.GetLeft(PracticeSelectorVoltagePilot3));
                Canvas.SetLeft(RWellipsePilot4, Canvas.GetLeft(PracticeSelectorVoltagePilot4));

                Canvas.SetTop(RWellipse1, Canvas.GetTop(PracticeSelector1));
                Canvas.SetTop(RWellipse2, Canvas.GetTop(PracticeSelector2));
                Canvas.SetTop(RWellipse3, Canvas.GetTop(PracticeSelector3));
                Canvas.SetTop(RWellipse4, Canvas.GetTop(PracticeSelector4));
                Canvas.SetTop(RWellipse5, Canvas.GetTop(PracticeSelector5));
                Canvas.SetTop(RWellipse6, Canvas.GetTop(PracticeSelector6));
                Canvas.SetTop(RWellipse7, Canvas.GetTop(PracticeSelector7));
                Canvas.SetTop(RWellipse8, Canvas.GetTop(PracticeSelector8));
                Canvas.SetTop(RWellipse9, Canvas.GetTop(PracticeSelector9));
                Canvas.SetTop(RWellipseA, Canvas.GetTop(PracticeSelectorA));
                Canvas.SetTop(RWellipseB, Canvas.GetTop(PracticeSelectorB));
                Canvas.SetTop(RWellipseC, Canvas.GetTop(PracticeSelectorC));
                Canvas.SetTop(RWellipseD, Canvas.GetTop(PracticeSelectorD));
                Canvas.SetTop(RWellipseE, Canvas.GetTop(PracticeSelectorE));
                Canvas.SetTop(RWellipseDC1, Canvas.GetTop(PracticeSelectorVoltageDC1));
                Canvas.SetTop(RWellipseDC2, Canvas.GetTop(PracticeSelectorVoltageDC2));
                Canvas.SetTop(RWellipseDC3, Canvas.GetTop(PracticeSelectorVoltageDC3));
                Canvas.SetTop(RWellipseDC4, Canvas.GetTop(PracticeSelectorVoltageDC4));
                Canvas.SetTop(RWellipseEq1, Canvas.GetTop(PracticeSelectorVoltageEq1));
                Canvas.SetTop(RWellipseEq2, Canvas.GetTop(PracticeSelectorVoltageEq2));
                Canvas.SetTop(RWellipseEq3, Canvas.GetTop(PracticeSelectorVoltageEq3));
                Canvas.SetTop(RWellipseEq4, Canvas.GetTop(PracticeSelectorVoltageEq4));
                Canvas.SetTop(RWellipseIBP1, Canvas.GetTop(PracticeSelectorVoltageIBP1));
                Canvas.SetTop(RWellipseIBP2, Canvas.GetTop(PracticeSelectorVoltageIBP2));
                Canvas.SetTop(RWellipseIBP3, Canvas.GetTop(PracticeSelectorVoltageIBP3));
                Canvas.SetTop(RWellipseIBP4, Canvas.GetTop(PracticeSelectorVoltageIBP4));
                Canvas.SetTop(RWellipsePilot1, Canvas.GetTop(PracticeSelectorVoltagePilot1));
                Canvas.SetTop(RWellipsePilot2, Canvas.GetTop(PracticeSelectorVoltagePilot2));
                Canvas.SetTop(RWellipsePilot3, Canvas.GetTop(PracticeSelectorVoltagePilot3));
                Canvas.SetTop(RWellipsePilot4, Canvas.GetTop(PracticeSelectorVoltagePilot4));
            }
            
            int score = 0;

            if (PracticeSelector1.Tag != null) 
            { 
                if ( PracticeSelector1.Text == "FTCW OD Cable" && PracticeSelector1.Tag.ToString() == "connected to PracticeSelector2")
                {

                    RWellipse1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    RWellipse2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                    if (PracticeSelector1.Text == "FTCW OD Cable" && PracticeSelector1.Tag.ToString() == "connected to PracticeSelector4")
                    {
                        RWellipse1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                        RWellipse4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                        score++;
                    }
                else
                {
                    RWellipse1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector1.Tag.ToString() == "connected to PracticeSelector2")
                        RWellipse2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector1.Tag.ToString() == "connected to PracticeSelector3")
                        RWellipse3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector1.Tag.ToString() == "connected to PracticeSelector4")
                        RWellipse4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector1.Tag.ToString() == "connected to PracticeSelector5")
                        RWellipse5.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector1.Tag.ToString() == "connected to PracticeSelector6")
                        RWellipse6.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector1.Tag.ToString() == "connected to PracticeSelector7")
                        RWellipse7.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector1.Tag.ToString() == "connected to PracticeSelector8")
                        RWellipse8.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector1.Tag.ToString() == "connected to PracticeSelector9")
                        RWellipse9.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector1.Tag.ToString() == "connected to PracticeSelectorA")
                        RWellipseA.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector1.Tag.ToString() == "connected to PracticeSelectorB")
                        RWellipseB.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector1.Tag.ToString() == "connected to PracticeSelectorC")
                        RWellipseC.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector1.Tag.ToString() == "connected to PracticeSelectorD")
                        RWellipseD.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector1.Tag.ToString() == "connected to PracticeSelectorE")
                        RWellipseE.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector1.Tag.ToString() == "connected to PracticeSelectorVoltageEq1")
                        RWellipseEq1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector1.Tag.ToString() == "connected to PracticeSelectorVoltageEq2")
                        RWellipseEq2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector1.Tag.ToString() == "connected to PracticeSelectorVoltageEq3")
                        RWellipseEq3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector1.Tag.ToString() == "connected to PracticeSelectorVoltageEq4")
                        RWellipseEq4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector1.Tag.ToString() == "connected to PracticeSelectorVoltageDC1")
                        RWellipseDC1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector1.Tag.ToString() == "connected to PracticeSelectorVoltageDC2")
                        RWellipseDC2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector1.Tag.ToString() == "connected to PracticeSelectorVoltageDC3")
                        RWellipseDC3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector1.Tag.ToString() == "connected to PracticeSelectorVoltageDC4")
                        RWellipseDC4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector1.Tag.ToString() == "connected to PracticeSelectorVoltagePilot1")
                        RWellipsePilot1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector1.Tag.ToString() == "connected to PracticeSelectorVoltagePilot2")
                        RWellipsePilot2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector1.Tag.ToString() == "connected to PracticeSelectorVoltagePilot3")
                        RWellipsePilot3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector1.Tag.ToString() == "connected to PracticeSelectorVoltagePilot4")
                        RWellipsePilot4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector1.Tag.ToString() == "connected to PracticeSelectorVoltageIBP1")
                        RWellipseIBP1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector1.Tag.ToString() == "connected to PracticeSelectorVoltageIBP2")
                        RWellipseIBP2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector1.Tag.ToString() == "connected to PracticeSelectorVoltageIBP3")
                        RWellipseIBP3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector1.Tag.ToString() == "connected to PracticeSelectorVoltageIBP4")
                        RWellipseIBP4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));
                }
            }

            if (PracticeSelector2.Tag != null)
            {
                if (PracticeSelector2.Text == "FTCW OD Cable" && PracticeSelector2.Tag.ToString() == "connected to PracticeSelector3")
                {
                }
                else
                    if (PracticeSelector2.Text == "FTCW OD Cable" && PracticeSelector2.Tag.ToString() == "connected to PracticeSelector1")
                {
                }
                else
                {
                    RWellipse2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector2.Tag.ToString() == "connected to PracticeSelector1")
                        RWellipse1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector2.Tag.ToString() == "connected to PracticeSelector3")
                        RWellipse3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector2.Tag.ToString() == "connected to PracticeSelector4")
                        RWellipse4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector2.Tag.ToString() == "connected to PracticeSelector5")
                        RWellipse5.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector2.Tag.ToString() == "connected to PracticeSelector6")
                        RWellipse6.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector2.Tag.ToString() == "connected to PracticeSelector7")
                        RWellipse7.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector2.Tag.ToString() == "connected to PracticeSelector8")
                        RWellipse8.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector2.Tag.ToString() == "connected to PracticeSelector9")
                        RWellipse9.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector2.Tag.ToString() == "connected to PracticeSelectorA")
                        RWellipseA.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector2.Tag.ToString() == "connected to PracticeSelectorB")
                        RWellipseB.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector2.Tag.ToString() == "connected to PracticeSelectorC")
                        RWellipseC.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector2.Tag.ToString() == "connected to PracticeSelectorD")
                        RWellipseD.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector2.Tag.ToString() == "connected to PracticeSelectorE")
                        RWellipseE.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector2.Tag.ToString() == "connected to PracticeSelectorVoltageEq1")
                        RWellipseEq1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector2.Tag.ToString() == "connected to PracticeSelectorVoltageEq2")
                        RWellipseEq2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector2.Tag.ToString() == "connected to PracticeSelectorVoltageEq3")
                        RWellipseEq3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector2.Tag.ToString() == "connected to PracticeSelectorVoltageEq4")
                        RWellipseEq4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector2.Tag.ToString() == "connected to PracticeSelectorVoltageDC1")
                        RWellipseDC1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector2.Tag.ToString() == "connected to PracticeSelectorVoltageDC2")
                        RWellipseDC2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector2.Tag.ToString() == "connected to PracticeSelectorVoltageDC3")
                        RWellipseDC3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector2.Tag.ToString() == "connected to PracticeSelectorVoltageDC4")
                        RWellipseDC4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector2.Tag.ToString() == "connected to PracticeSelectorVoltagePilot1")
                        RWellipsePilot1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector2.Tag.ToString() == "connected to PracticeSelectorVoltagePilot2")
                        RWellipsePilot2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector2.Tag.ToString() == "connected to PracticeSelectorVoltagePilot3")
                        RWellipsePilot3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector2.Tag.ToString() == "connected to PracticeSelectorVoltagePilot4")
                        RWellipsePilot4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector2.Tag.ToString() == "connected to PracticeSelectorVoltageIBP1")
                        RWellipseIBP1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector2.Tag.ToString() == "connected to PracticeSelectorVoltageIBP2")
                        RWellipseIBP2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector2.Tag.ToString() == "connected to PracticeSelectorVoltageIBP3")
                        RWellipseIBP3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector2.Tag.ToString() == "connected to PracticeSelectorVoltageIBP4")
                        RWellipseIBP4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));
                }
            }

            if (PracticeSelector3.Tag != null)
            {
                if (PracticeSelector3.Text == "FTCW OD Cable" && PracticeSelector3.Tag.ToString() == "connected to PracticeSelector2")
                {
                    RWellipse3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    RWellipse2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                    if (PracticeSelector3.Text == "FTCW OD Cable" && PracticeSelector3.Tag.ToString() == "connected to PracticeSelector4")
                {
                    RWellipse3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    RWellipse4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                {
                    RWellipse3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector3.Tag.ToString() == "connected to PracticeSelector1")
                        RWellipse1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector3.Tag.ToString() == "connected to PracticeSelector5")
                        RWellipse5.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector3.Tag.ToString() == "connected to PracticeSelector6")
                        RWellipse6.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector3.Tag.ToString() == "connected to PracticeSelector7")
                        RWellipse7.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector3.Tag.ToString() == "connected to PracticeSelector8")
                        RWellipse8.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector3.Tag.ToString() == "connected to PracticeSelector9")
                        RWellipse9.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector3.Tag.ToString() == "connected to PracticeSelectorA")
                        RWellipseA.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector3.Tag.ToString() == "connected to PracticeSelectorB")
                        RWellipseB.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector3.Tag.ToString() == "connected to PracticeSelectorC")
                        RWellipseC.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector3.Tag.ToString() == "connected to PracticeSelectorD")
                        RWellipseD.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector3.Tag.ToString() == "connected to PracticeSelectorE")
                        RWellipseE.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector3.Tag.ToString() == "connected to PracticeSelectorVoltageEq1")
                        RWellipseEq1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector3.Tag.ToString() == "connected to PracticeSelectorVoltageEq2")
                        RWellipseEq2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector3.Tag.ToString() == "connected to PracticeSelectorVoltageEq3")
                        RWellipseEq3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector3.Tag.ToString() == "connected to PracticeSelectorVoltageEq4")
                        RWellipseEq4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector3.Tag.ToString() == "connected to PracticeSelectorVoltageDC1")
                        RWellipseDC1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector3.Tag.ToString() == "connected to PracticeSelectorVoltageDC2")
                        RWellipseDC2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector3.Tag.ToString() == "connected to PracticeSelectorVoltageDC3")
                        RWellipseDC3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector3.Tag.ToString() == "connected to PracticeSelectorVoltageDC4")
                        RWellipseDC4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector3.Tag.ToString() == "connected to PracticeSelectorVoltagePilot1")
                        RWellipsePilot1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector3.Tag.ToString() == "connected to PracticeSelectorVoltagePilot2")
                        RWellipsePilot2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector3.Tag.ToString() == "connected to PracticeSelectorVoltagePilot3")
                        RWellipsePilot3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector3.Tag.ToString() == "connected to PracticeSelectorVoltagePilot4")
                        RWellipsePilot4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector3.Tag.ToString() == "connected to PracticeSelectorVoltageIBP1")
                        RWellipseIBP1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector3.Tag.ToString() == "connected to PracticeSelectorVoltageIBP2")
                        RWellipseIBP2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector3.Tag.ToString() == "connected to PracticeSelectorVoltageIBP3")
                        RWellipseIBP3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector3.Tag.ToString() == "connected to PracticeSelectorVoltageIBP4")
                        RWellipseIBP4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));
                }
            }

            if (PracticeSelector4.Tag != null)
            {
                if (PracticeSelector4.Text == "FTCW OD Cable" && PracticeSelector4.Tag.ToString() == "connected to PracticeSelector3")
                {
                }
                else
                    if (PracticeSelector4.Text == "FTCW OD Cable" && PracticeSelector4.Tag.ToString() == "connected to PracticeSelector1")
                {
                }
                else
                {
                    RWellipse4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector4.Tag.ToString() == "connected to PracticeSelector1")
                        RWellipse1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector4.Tag.ToString() == "connected to PracticeSelector2")
                        RWellipse2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector4.Tag.ToString() == "connected to PracticeSelector3")
                        RWellipse3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector4.Tag.ToString() == "connected to PracticeSelector5")
                        RWellipse5.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector4.Tag.ToString() == "connected to PracticeSelector6")
                        RWellipse6.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector4.Tag.ToString() == "connected to PracticeSelector7")
                        RWellipse7.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector4.Tag.ToString() == "connected to PracticeSelector8")
                        RWellipse8.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector4.Tag.ToString() == "connected to PracticeSelector9")
                        RWellipse9.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector4.Tag.ToString() == "connected to PracticeSelectorA")
                        RWellipseA.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector4.Tag.ToString() == "connected to PracticeSelectorB")
                        RWellipseB.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector4.Tag.ToString() == "connected to PracticeSelectorC")
                        RWellipseC.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector4.Tag.ToString() == "connected to PracticeSelectorD")
                        RWellipseD.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector4.Tag.ToString() == "connected to PracticeSelectorE")
                        RWellipseE.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector4.Tag.ToString() == "connected to PracticeSelectorVoltageEq1")
                        RWellipseEq1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector4.Tag.ToString() == "connected to PracticeSelectorVoltageEq2")
                        RWellipseEq2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector4.Tag.ToString() == "connected to PracticeSelectorVoltageEq3")
                        RWellipseEq3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector4.Tag.ToString() == "connected to PracticeSelectorVoltageEq4")
                        RWellipseEq4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector4.Tag.ToString() == "connected to PracticeSelectorVoltageDC1")
                        RWellipseDC1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector4.Tag.ToString() == "connected to PracticeSelectorVoltageDC2")
                        RWellipseDC2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector4.Tag.ToString() == "connected to PracticeSelectorVoltageDC3")
                        RWellipseDC3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector4.Tag.ToString() == "connected to PracticeSelectorVoltageDC4")
                        RWellipseDC4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector4.Tag.ToString() == "connected to PracticeSelectorVoltagePilot1")
                        RWellipsePilot1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector4.Tag.ToString() == "connected to PracticeSelectorVoltagePilot2")
                        RWellipsePilot2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector4.Tag.ToString() == "connected to PracticeSelectorVoltagePilot3")
                        RWellipsePilot3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector4.Tag.ToString() == "connected to PracticeSelectorVoltagePilot4")
                        RWellipsePilot4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector4.Tag.ToString() == "connected to PracticeSelectorVoltageIBP1")
                        RWellipseIBP1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector4.Tag.ToString() == "connected to PracticeSelectorVoltageIBP2")
                        RWellipseIBP2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector4.Tag.ToString() == "connected to PracticeSelectorVoltageIBP3")
                        RWellipseIBP3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector4.Tag.ToString() == "connected to PracticeSelectorVoltageIBP4")
                        RWellipseIBP4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));
                }
            }
            
            if (PracticeSelector5.Tag != null)
            {
                if (PracticeSelector5.Text == "7/16 connector" && PracticeSelector5.Tag.ToString() == "connected to PracticeSelector6")
                {
                    RWellipse5.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    RWellipse6.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                {
                    RWellipse5.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector5.Tag.ToString() == "connected to PracticeSelector1")
                        RWellipse1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector5.Tag.ToString() == "connected to PracticeSelector2")
                        RWellipse2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector5.Tag.ToString() == "connected to PracticeSelector3")
                        RWellipse3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector5.Tag.ToString() == "connected to PracticeSelector4")
                        RWellipse4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector5.Tag.ToString() == "connected to PracticeSelector6")
                        RWellipse6.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector5.Tag.ToString() == "connected to PracticeSelector7")
                        RWellipse7.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector5.Tag.ToString() == "connected to PracticeSelector8")
                        RWellipse8.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector5.Tag.ToString() == "connected to PracticeSelector9")
                        RWellipse9.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector5.Tag.ToString() == "connected to PracticeSelectorA")
                        RWellipseA.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector5.Tag.ToString() == "connected to PracticeSelectorB")
                        RWellipseB.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector5.Tag.ToString() == "connected to PracticeSelectorC")
                        RWellipseC.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector5.Tag.ToString() == "connected to PracticeSelectorD")
                        RWellipseD.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector5.Tag.ToString() == "connected to PracticeSelectorE")
                        RWellipseE.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector5.Tag.ToString() == "connected to PracticeSelectorVoltageEq1")
                        RWellipseEq1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector5.Tag.ToString() == "connected to PracticeSelectorVoltageEq2")
                        RWellipseEq2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector5.Tag.ToString() == "connected to PracticeSelectorVoltageEq3")
                        RWellipseEq3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector5.Tag.ToString() == "connected to PracticeSelectorVoltageEq4")
                        RWellipseEq4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector5.Tag.ToString() == "connected to PracticeSelectorVoltageDC1")
                        RWellipseDC1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector5.Tag.ToString() == "connected to PracticeSelectorVoltageDC2")
                        RWellipseDC2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector5.Tag.ToString() == "connected to PracticeSelectorVoltageDC3")
                        RWellipseDC3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector5.Tag.ToString() == "connected to PracticeSelectorVoltageDC4")
                        RWellipseDC4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector5.Tag.ToString() == "connected to PracticeSelectorVoltagePilot1")
                        RWellipsePilot1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector5.Tag.ToString() == "connected to PracticeSelectorVoltagePilot2")
                        RWellipsePilot2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector5.Tag.ToString() == "connected to PracticeSelectorVoltagePilot3")
                        RWellipsePilot3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector5.Tag.ToString() == "connected to PracticeSelectorVoltagePilot4")
                        RWellipsePilot4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector5.Tag.ToString() == "connected to PracticeSelectorVoltageIBP1")
                        RWellipseIBP1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector5.Tag.ToString() == "connected to PracticeSelectorVoltageIBP2")
                        RWellipseIBP2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector5.Tag.ToString() == "connected to PracticeSelectorVoltageIBP3")
                        RWellipseIBP3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector5.Tag.ToString() == "connected to PracticeSelectorVoltageIBP4")
                        RWellipseIBP4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));
                }
            }

            if (PracticeSelector6.Tag != null)
            {
                if (PracticeSelector6.Text == "7/16 connector" && PracticeSelector6.Tag.ToString() == "connected to PracticeSelector5")
                {
                }
                else
                {
                    RWellipse6.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector6.Tag.ToString() == "connected to PracticeSelector1")
                        RWellipse1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector6.Tag.ToString() == "connected to PracticeSelector2")
                        RWellipse2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector6.Tag.ToString() == "connected to PracticeSelector3")
                        RWellipse3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector6.Tag.ToString() == "connected to PracticeSelector4")
                        RWellipse4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector6.Tag.ToString() == "connected to PracticeSelector5")
                        RWellipse5.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector6.Tag.ToString() == "connected to PracticeSelector7")
                        RWellipse7.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector6.Tag.ToString() == "connected to PracticeSelector8")
                        RWellipse8.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector6.Tag.ToString() == "connected to PracticeSelector9")
                        RWellipse9.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector6.Tag.ToString() == "connected to PracticeSelectorA")
                        RWellipseA.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector6.Tag.ToString() == "connected to PracticeSelectorB")
                        RWellipseB.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector6.Tag.ToString() == "connected to PracticeSelectorC")
                        RWellipseC.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector6.Tag.ToString() == "connected to PracticeSelectorD")
                        RWellipseD.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector6.Tag.ToString() == "connected to PracticeSelectorE")
                        RWellipseE.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector6.Tag.ToString() == "connected to PracticeSelectorVoltageEq1")
                        RWellipseEq1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector6.Tag.ToString() == "connected to PracticeSelectorVoltageEq2")
                        RWellipseEq2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector6.Tag.ToString() == "connected to PracticeSelectorVoltageEq3")
                        RWellipseEq3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector6.Tag.ToString() == "connected to PracticeSelectorVoltageEq4")
                        RWellipseEq4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector6.Tag.ToString() == "connected to PracticeSelectorVoltageDC1")
                        RWellipseDC1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector6.Tag.ToString() == "connected to PracticeSelectorVoltageDC2")
                        RWellipseDC2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector6.Tag.ToString() == "connected to PracticeSelectorVoltageDC3")
                        RWellipseDC3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector6.Tag.ToString() == "connected to PracticeSelectorVoltageDC4")
                        RWellipseDC4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector6.Tag.ToString() == "connected to PracticeSelectorVoltagePilot1")
                        RWellipsePilot1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector6.Tag.ToString() == "connected to PracticeSelectorVoltagePilot2")
                        RWellipsePilot2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector6.Tag.ToString() == "connected to PracticeSelectorVoltagePilot3")
                        RWellipsePilot3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector6.Tag.ToString() == "connected to PracticeSelectorVoltagePilot4")
                        RWellipsePilot4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector6.Tag.ToString() == "connected to PracticeSelectorVoltageIBP1")
                        RWellipseIBP1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector6.Tag.ToString() == "connected to PracticeSelectorVoltageIBP2")
                        RWellipseIBP2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector6.Tag.ToString() == "connected to PracticeSelectorVoltageIBP3")
                        RWellipseIBP3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector6.Tag.ToString() == "connected to PracticeSelectorVoltageIBP4")
                        RWellipseIBP4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));
                }
            }

            if (PracticeSelector7.Tag != null)
            {
                if (PracticeSelector7.Text == "7/16 connector" && PracticeSelector7.Tag.ToString() == "connected to PracticeSelector8")
                {
                    RWellipse7.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    RWellipse8.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                {
                    RWellipse7.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector7.Tag.ToString() == "connected to PracticeSelector1")
                        RWellipse1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector7.Tag.ToString() == "connected to PracticeSelector2")
                        RWellipse2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector7.Tag.ToString() == "connected to PracticeSelector3")
                        RWellipse3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector7.Tag.ToString() == "connected to PracticeSelector4")
                        RWellipse4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector7.Tag.ToString() == "connected to PracticeSelector5")
                        RWellipse5.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector7.Tag.ToString() == "connected to PracticeSelector6")
                        RWellipse6.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector7.Tag.ToString() == "connected to PracticeSelector8")
                        RWellipse8.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector7.Tag.ToString() == "connected to PracticeSelector9")
                        RWellipse9.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector7.Tag.ToString() == "connected to PracticeSelectorA")
                        RWellipseA.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector7.Tag.ToString() == "connected to PracticeSelectorB")
                        RWellipseB.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector7.Tag.ToString() == "connected to PracticeSelectorC")
                        RWellipseC.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector7.Tag.ToString() == "connected to PracticeSelectorD")
                        RWellipseD.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector7.Tag.ToString() == "connected to PracticeSelectorE")
                        RWellipseE.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector7.Tag.ToString() == "connected to PracticeSelectorVoltageEq1")
                        RWellipseEq1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector7.Tag.ToString() == "connected to PracticeSelectorVoltageEq2")
                        RWellipseEq2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector7.Tag.ToString() == "connected to PracticeSelectorVoltageEq3")
                        RWellipseEq3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector7.Tag.ToString() == "connected to PracticeSelectorVoltageEq4")
                        RWellipseEq4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector7.Tag.ToString() == "connected to PracticeSelectorVoltageDC1")
                        RWellipseDC1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector7.Tag.ToString() == "connected to PracticeSelectorVoltageDC2")
                        RWellipseDC2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector7.Tag.ToString() == "connected to PracticeSelectorVoltageDC3")
                        RWellipseDC3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector7.Tag.ToString() == "connected to PracticeSelectorVoltageDC4")
                        RWellipseDC4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector7.Tag.ToString() == "connected to PracticeSelectorVoltagePilot1")
                        RWellipsePilot1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector7.Tag.ToString() == "connected to PracticeSelectorVoltagePilot2")
                        RWellipsePilot2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector7.Tag.ToString() == "connected to PracticeSelectorVoltagePilot3")
                        RWellipsePilot3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector7.Tag.ToString() == "connected to PracticeSelectorVoltagePilot4")
                        RWellipsePilot4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector7.Tag.ToString() == "connected to PracticeSelectorVoltageIBP1")
                        RWellipseIBP1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector7.Tag.ToString() == "connected to PracticeSelectorVoltageIBP2")
                        RWellipseIBP2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector7.Tag.ToString() == "connected to PracticeSelectorVoltageIBP3")
                        RWellipseIBP3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector7.Tag.ToString() == "connected to PracticeSelectorVoltageIBP4")
                        RWellipseIBP4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));
                }
            }

            if (PracticeSelector8.Tag != null)
            {
                if (PracticeSelector8.Text == "7/16 connector" && PracticeSelector8.Tag.ToString() == "connected to PracticeSelector7")
                {
                }
                else
                {
                    RWellipse8.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector8.Tag.ToString() == "connected to PracticeSelector1")
                        RWellipse1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector8.Tag.ToString() == "connected to PracticeSelector2")
                        RWellipse2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector8.Tag.ToString() == "connected to PracticeSelector3")
                        RWellipse3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector8.Tag.ToString() == "connected to PracticeSelector4")
                        RWellipse4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector8.Tag.ToString() == "connected to PracticeSelector5")
                        RWellipse5.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector8.Tag.ToString() == "connected to PracticeSelector6")
                        RWellipse6.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector8.Tag.ToString() == "connected to PracticeSelector7")
                        RWellipse7.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector8.Tag.ToString() == "connected to PracticeSelector9")
                        RWellipse9.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector8.Tag.ToString() == "connected to PracticeSelectorA")
                        RWellipseA.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector8.Tag.ToString() == "connected to PracticeSelectorB")
                        RWellipseB.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector8.Tag.ToString() == "connected to PracticeSelectorC")
                        RWellipseC.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector8.Tag.ToString() == "connected to PracticeSelectorD")
                        RWellipseD.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector8.Tag.ToString() == "connected to PracticeSelectorE")
                        RWellipseE.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector8.Tag.ToString() == "connected to PracticeSelectorVoltageEq1")
                        RWellipseEq1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector8.Tag.ToString() == "connected to PracticeSelectorVoltageEq2")
                        RWellipseEq2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector8.Tag.ToString() == "connected to PracticeSelectorVoltageEq3")
                        RWellipseEq3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector8.Tag.ToString() == "connected to PracticeSelectorVoltageEq4")
                        RWellipseEq4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector8.Tag.ToString() == "connected to PracticeSelectorVoltageDC1")
                        RWellipseDC1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector8.Tag.ToString() == "connected to PracticeSelectorVoltageDC2")
                        RWellipseDC2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector8.Tag.ToString() == "connected to PracticeSelectorVoltageDC3")
                        RWellipseDC3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector8.Tag.ToString() == "connected to PracticeSelectorVoltageDC4")
                        RWellipseDC4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector8.Tag.ToString() == "connected to PracticeSelectorVoltagePilot1")
                        RWellipsePilot1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector8.Tag.ToString() == "connected to PracticeSelectorVoltagePilot2")
                        RWellipsePilot2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector8.Tag.ToString() == "connected to PracticeSelectorVoltagePilot3")
                        RWellipsePilot3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector8.Tag.ToString() == "connected to PracticeSelectorVoltagePilot4")
                        RWellipsePilot4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector8.Tag.ToString() == "connected to PracticeSelectorVoltageIBP1")
                        RWellipseIBP1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector8.Tag.ToString() == "connected to PracticeSelectorVoltageIBP2")
                        RWellipseIBP2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector8.Tag.ToString() == "connected to PracticeSelectorVoltageIBP3")
                        RWellipseIBP3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector8.Tag.ToString() == "connected to PracticeSelectorVoltageIBP4")
                        RWellipseIBP4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));
                }
            }

            if (PracticeSelector9.Tag != null)
            {
                if (PracticeSelector9.Text == "FOSC Flexi Optical" && PracticeSelector9.Tag.ToString() == "connected to PracticeSelectorА")
                {
                }
                else
                {
                    RWellipse9.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector9.Tag.ToString() == "connected to PracticeSelector1")
                        RWellipse1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector9.Tag.ToString() == "connected to PracticeSelector2")
                        RWellipse2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector9.Tag.ToString() == "connected to PracticeSelector3")
                        RWellipse3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector9.Tag.ToString() == "connected to PracticeSelector4")
                        RWellipse4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector9.Tag.ToString() == "connected to PracticeSelector5")
                        RWellipse5.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector9.Tag.ToString() == "connected to PracticeSelector6")
                        RWellipse6.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector9.Tag.ToString() == "connected to PracticeSelector7")
                        RWellipse7.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector9.Tag.ToString() == "connected to PracticeSelector8")
                        RWellipse8.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector9.Tag.ToString() == "connected to PracticeSelectorА")
                        RWellipseA.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector9.Tag.ToString() == "connected to PracticeSelectorB")
                        RWellipseB.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector9.Tag.ToString() == "connected to PracticeSelectorC")
                        RWellipseC.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector9.Tag.ToString() == "connected to PracticeSelectorD")
                        RWellipseD.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector9.Tag.ToString() == "connected to PracticeSelectorE")
                        RWellipseE.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector9.Tag.ToString() == "connected to PracticeSelectorVoltageEq1")
                        RWellipseEq1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector9.Tag.ToString() == "connected to PracticeSelectorVoltageEq2")
                        RWellipseEq2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector9.Tag.ToString() == "connected to PracticeSelectorVoltageEq3")
                        RWellipseEq3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector9.Tag.ToString() == "connected to PracticeSelectorVoltageEq4")
                        RWellipseEq4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector9.Tag.ToString() == "connected to PracticeSelectorVoltageDC1")
                        RWellipseDC1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector9.Tag.ToString() == "connected to PracticeSelectorVoltageDC2")
                        RWellipseDC2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector9.Tag.ToString() == "connected to PracticeSelectorVoltageDC3")
                        RWellipseDC3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector9.Tag.ToString() == "connected to PracticeSelectorVoltageDC4")
                        RWellipseDC4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector9.Tag.ToString() == "connected to PracticeSelectorVoltagePilot1")
                        RWellipsePilot1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector9.Tag.ToString() == "connected to PracticeSelectorVoltagePilot2")
                        RWellipsePilot2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector9.Tag.ToString() == "connected to PracticeSelectorVoltagePilot3")
                        RWellipsePilot3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector9.Tag.ToString() == "connected to PracticeSelectorVoltagePilot4")
                        RWellipsePilot4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector9.Tag.ToString() == "connected to PracticeSelectorVoltageIBP1")
                        RWellipseIBP1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector9.Tag.ToString() == "connected to PracticeSelectorVoltageIBP2")
                        RWellipseIBP2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector9.Tag.ToString() == "connected to PracticeSelectorVoltageIBP3")
                        RWellipseIBP3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelector9.Tag.ToString() == "connected to PracticeSelectorVoltageIBP4")
                        RWellipseIBP4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                }
            }

            if (PracticeSelectorA.Tag != null)
            {
                if (PracticeSelectorA.Text == "FOSC Flexi Optical" && PracticeSelectorA.Tag.ToString() == "connected to PracticeSelector9")
                {
                    RWellipseA.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    RWellipse9.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                {
                    RWellipseA.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorA.Tag.ToString() == "connected to PracticeSelector1")
                        RWellipse1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorA.Tag.ToString() == "connected to PracticeSelector2")
                        RWellipse2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorA.Tag.ToString() == "connected to PracticeSelector3")
                        RWellipse3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorA.Tag.ToString() == "connected to PracticeSelector4")
                        RWellipse4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorA.Tag.ToString() == "connected to PracticeSelector5")
                        RWellipse5.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorA.Tag.ToString() == "connected to PracticeSelector6")
                        RWellipse6.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorA.Tag.ToString() == "connected to PracticeSelector7")
                        RWellipse7.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorA.Tag.ToString() == "connected to PracticeSelector8")
                        RWellipse8.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorA.Tag.ToString() == "connected to PracticeSelector9")
                        RWellipse9.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorA.Tag.ToString() == "connected to PracticeSelectorB")
                        RWellipseB.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorA.Tag.ToString() == "connected to PracticeSelectorC")
                        RWellipseC.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorA.Tag.ToString() == "connected to PracticeSelectorD")
                        RWellipseD.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorA.Tag.ToString() == "connected to PracticeSelectorE")
                        RWellipseE.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorA.Tag.ToString() == "connected to PracticeSelectorVoltageEq1")
                        RWellipseEq1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorA.Tag.ToString() == "connected to PracticeSelectorVoltageEq2")
                        RWellipseEq2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorA.Tag.ToString() == "connected to PracticeSelectorVoltageEq3")
                        RWellipseEq3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorA.Tag.ToString() == "connected to PracticeSelectorVoltageEq4")
                        RWellipseEq4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorA.Tag.ToString() == "connected to PracticeSelectorVoltageDC1")
                        RWellipseDC1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorA.Tag.ToString() == "connected to PracticeSelectorVoltageDC2")
                        RWellipseDC2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorA.Tag.ToString() == "connected to PracticeSelectorVoltageDC3")
                        RWellipseDC3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorA.Tag.ToString() == "connected to PracticeSelectorVoltageDC4")
                        RWellipseDC4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorA.Tag.ToString() == "connected to PracticeSelectorVoltagePilot1")
                        RWellipsePilot1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorA.Tag.ToString() == "connected to PracticeSelectorVoltagePilot2")
                        RWellipsePilot2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorA.Tag.ToString() == "connected to PracticeSelectorVoltagePilot3")
                        RWellipsePilot3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorA.Tag.ToString() == "connected to PracticeSelectorVoltagePilot4")
                        RWellipsePilot4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorA.Tag.ToString() == "connected to PracticeSelectorVoltageIBP1")
                        RWellipseIBP1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorA.Tag.ToString() == "connected to PracticeSelectorVoltageIBP2")
                        RWellipseIBP2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorA.Tag.ToString() == "connected to PracticeSelectorVoltageIBP3")
                        RWellipseIBP3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorA.Tag.ToString() == "connected to PracticeSelectorVoltageIBP4")
                        RWellipseIBP4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                }
            }

            if (PracticeSelectorB.Tag != null)
            {
                if (PracticeSelectorB.Text == "7/16 connector" && PracticeSelectorB.Tag.ToString() == "connected to PracticeSelectorC")
                {
                    RWellipseB.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    RWellipseC.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                {
                    RWellipseB.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorB.Tag.ToString() == "connected to PracticeSelector1")
                        RWellipse1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorB.Tag.ToString() == "connected to PracticeSelector2")
                        RWellipse2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorB.Tag.ToString() == "connected to PracticeSelector3")
                        RWellipse3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorB.Tag.ToString() == "connected to PracticeSelector4")
                        RWellipse4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorB.Tag.ToString() == "connected to PracticeSelector5")
                        RWellipse5.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorB.Tag.ToString() == "connected to PracticeSelector6")
                        RWellipse6.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorB.Tag.ToString() == "connected to PracticeSelector7")
                        RWellipse7.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorB.Tag.ToString() == "connected to PracticeSelector8")
                        RWellipse8.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorB.Tag.ToString() == "connected to PracticeSelector9")
                        RWellipse9.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorB.Tag.ToString() == "connected to PracticeSelectorA")
                        RWellipseA.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorB.Tag.ToString() == "connected to PracticeSelectorC")
                        RWellipseC.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorB.Tag.ToString() == "connected to PracticeSelectorD")
                        RWellipseD.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorB.Tag.ToString() == "connected to PracticeSelectorE")
                        RWellipseE.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorB.Tag.ToString() == "connected to PracticeSelectorVoltageEq1")
                        RWellipseEq1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorB.Tag.ToString() == "connected to PracticeSelectorVoltageEq2")
                        RWellipseEq2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorB.Tag.ToString() == "connected to PracticeSelectorVoltageEq3")
                        RWellipseEq3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorB.Tag.ToString() == "connected to PracticeSelectorVoltageEq4")
                        RWellipseEq4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorB.Tag.ToString() == "connected to PracticeSelectorVoltageDC1")
                        RWellipseDC1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorB.Tag.ToString() == "connected to PracticeSelectorVoltageDC2")
                        RWellipseDC2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorB.Tag.ToString() == "connected to PracticeSelectorVoltageDC3")
                        RWellipseDC3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorB.Tag.ToString() == "connected to PracticeSelectorVoltageDC4")
                        RWellipseDC4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorB.Tag.ToString() == "connected to PracticeSelectorVoltagePilot1")
                        RWellipsePilot1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorB.Tag.ToString() == "connected to PracticeSelectorVoltagePilot2")
                        RWellipsePilot2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorB.Tag.ToString() == "connected to PracticeSelectorVoltagePilot3")
                        RWellipsePilot3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorB.Tag.ToString() == "connected to PracticeSelectorVoltagePilot4")
                        RWellipsePilot4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorB.Tag.ToString() == "connected to PracticeSelectorVoltageIBP1")
                        RWellipseIBP1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorB.Tag.ToString() == "connected to PracticeSelectorVoltageIBP2")
                        RWellipseIBP2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorB.Tag.ToString() == "connected to PracticeSelectorVoltageIBP3")
                        RWellipseIBP3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorB.Tag.ToString() == "connected to PracticeSelectorVoltageIBP4")
                        RWellipseIBP4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));
                }
            }

            if (PracticeSelectorC.Tag != null)
            {
                if (PracticeSelectorC.Text == "7/16 connector" && PracticeSelectorC.Tag.ToString() == "connected to PracticeSelectorB")
                {
                }
                else
                {
                    RWellipseC.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorC.Tag.ToString() == "connected to PracticeSelector1")
                        RWellipse1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorC.Tag.ToString() == "connected to PracticeSelector2")
                        RWellipse2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorC.Tag.ToString() == "connected to PracticeSelector3")
                        RWellipse3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorC.Tag.ToString() == "connected to PracticeSelector4")
                        RWellipse4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorC.Tag.ToString() == "connected to PracticeSelector5")
                        RWellipse5.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorC.Tag.ToString() == "connected to PracticeSelector6")
                        RWellipse6.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorC.Tag.ToString() == "connected to PracticeSelector7")
                        RWellipse7.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorC.Tag.ToString() == "connected to PracticeSelector8")
                        RWellipse8.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorC.Tag.ToString() == "connected to PracticeSelector9")
                        RWellipse9.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorC.Tag.ToString() == "connected to PracticeSelectorA")
                        RWellipseA.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorC.Tag.ToString() == "connected to PracticeSelectorB")
                        RWellipseB.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorC.Tag.ToString() == "connected to PracticeSelectorD")
                        RWellipseD.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorC.Tag.ToString() == "connected to PracticeSelectorE")
                        RWellipseE.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorC.Tag.ToString() == "connected to PracticeSelectorVoltageEq1")
                        RWellipseEq1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorC.Tag.ToString() == "connected to PracticeSelectorVoltageEq2")
                        RWellipseEq2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorC.Tag.ToString() == "connected to PracticeSelectorVoltageEq3")
                        RWellipseEq3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorC.Tag.ToString() == "connected to PracticeSelectorVoltageEq4")
                        RWellipseEq4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorC.Tag.ToString() == "connected to PracticeSelectorVoltageDC1")
                        RWellipseDC1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorC.Tag.ToString() == "connected to PracticeSelectorVoltageDC2")
                        RWellipseDC2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorC.Tag.ToString() == "connected to PracticeSelectorVoltageDC3")
                        RWellipseDC3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorC.Tag.ToString() == "connected to PracticeSelectorVoltageDC4")
                        RWellipseDC4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorC.Tag.ToString() == "connected to PracticeSelectorVoltagePilot1")
                        RWellipsePilot1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorC.Tag.ToString() == "connected to PracticeSelectorVoltagePilot2")
                        RWellipsePilot2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorC.Tag.ToString() == "connected to PracticeSelectorVoltagePilot3")
                        RWellipsePilot3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorC.Tag.ToString() == "connected to PracticeSelectorVoltagePilot4")
                        RWellipsePilot4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorC.Tag.ToString() == "connected to PracticeSelectorVoltageIBP1")
                        RWellipseIBP1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorC.Tag.ToString() == "connected to PracticeSelectorVoltageIBP2")
                        RWellipseIBP2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorC.Tag.ToString() == "connected to PracticeSelectorVoltageIBP3")
                        RWellipseIBP3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorC.Tag.ToString() == "connected to PracticeSelectorVoltageIBP4")
                        RWellipseIBP4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));
                }
            }

            if (PracticeSelectorD.Tag != null)
            {
                if (PracticeSelectorD.Text == "7/16 connector" && PracticeSelectorD.Tag.ToString() == "connected to PracticeSelectorE")
                {
                    RWellipseD.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    RWellipseE.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++; 
                }
                else
                {
                    RWellipseD.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorD.Tag.ToString() == "connected to PracticeSelector1")
                        RWellipse1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorD.Tag.ToString() == "connected to PracticeSelector2")
                        RWellipse2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorD.Tag.ToString() == "connected to PracticeSelector3")
                        RWellipse3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorD.Tag.ToString() == "connected to PracticeSelector4")
                        RWellipse4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorD.Tag.ToString() == "connected to PracticeSelector5")
                        RWellipse5.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorD.Tag.ToString() == "connected to PracticeSelector6")
                        RWellipse6.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorD.Tag.ToString() == "connected to PracticeSelector7")
                        RWellipse7.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorD.Tag.ToString() == "connected to PracticeSelector8")
                        RWellipse8.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorD.Tag.ToString() == "connected to PracticeSelector9")
                        RWellipse9.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorD.Tag.ToString() == "connected to PracticeSelectorA")
                        RWellipseA.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorD.Tag.ToString() == "connected to PracticeSelectorB")
                        RWellipseB.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorD.Tag.ToString() == "connected to PracticeSelectorC")
                        RWellipseC.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorD.Tag.ToString() == "connected to PracticeSelectorE")
                        RWellipseE.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorD.Tag.ToString() == "connected to PracticeSelectorVoltageEq1")
                        RWellipseEq1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorD.Tag.ToString() == "connected to PracticeSelectorVoltageEq2")
                        RWellipseEq2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorD.Tag.ToString() == "connected to PracticeSelectorVoltageEq3")
                        RWellipseEq3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorD.Tag.ToString() == "connected to PracticeSelectorVoltageEq4")
                        RWellipseEq4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorD.Tag.ToString() == "connected to PracticeSelectorVoltageDC1")
                        RWellipseDC1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorD.Tag.ToString() == "connected to PracticeSelectorVoltageDC2")
                        RWellipseDC2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorD.Tag.ToString() == "connected to PracticeSelectorVoltageDC3")
                        RWellipseDC3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorD.Tag.ToString() == "connected to PracticeSelectorVoltageDC4")
                        RWellipseDC4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorD.Tag.ToString() == "connected to PracticeSelectorVoltagePilot1")
                        RWellipsePilot1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorD.Tag.ToString() == "connected to PracticeSelectorVoltagePilot2")
                        RWellipsePilot2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorD.Tag.ToString() == "connected to PracticeSelectorVoltagePilot3")
                        RWellipsePilot3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorD.Tag.ToString() == "connected to PracticeSelectorVoltagePilot4")
                        RWellipsePilot4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorD.Tag.ToString() == "connected to PracticeSelectorVoltageIBP1")
                        RWellipseIBP1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorD.Tag.ToString() == "connected to PracticeSelectorVoltageIBP2")
                        RWellipseIBP2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorD.Tag.ToString() == "connected to PracticeSelectorVoltageIBP3")
                        RWellipseIBP3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorD.Tag.ToString() == "connected to PracticeSelectorVoltageIBP4")
                        RWellipseIBP4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));
                }
            }

            if (PracticeSelectorE.Tag != null)
            {
                if (PracticeSelectorE.Text == "7/16 connector" && PracticeSelectorE.Tag.ToString() == "connected to PracticeSelectorD")
                {
                }
                else
                {
                    RWellipseE.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorE.Tag.ToString() == "connected to PracticeSelector1")
                        RWellipse1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorE.Tag.ToString() == "connected to PracticeSelector2")
                        RWellipse2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorE.Tag.ToString() == "connected to PracticeSelector3")
                        RWellipse3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorE.Tag.ToString() == "connected to PracticeSelector4")
                        RWellipse4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorE.Tag.ToString() == "connected to PracticeSelector5")
                        RWellipse5.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorE.Tag.ToString() == "connected to PracticeSelector6")
                        RWellipse6.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorE.Tag.ToString() == "connected to PracticeSelector7")
                        RWellipse7.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorE.Tag.ToString() == "connected to PracticeSelector8")
                        RWellipse8.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorE.Tag.ToString() == "connected to PracticeSelector9")
                        RWellipse9.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorE.Tag.ToString() == "connected to PracticeSelectorA")
                        RWellipseA.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorE.Tag.ToString() == "connected to PracticeSelectorB")
                        RWellipseB.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorE.Tag.ToString() == "connected to PracticeSelectorC")
                        RWellipseC.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorE.Tag.ToString() == "connected to PracticeSelectorD")
                        RWellipseD.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorE.Tag.ToString() == "connected to PracticeSelectorVoltageEq1")
                        RWellipseEq1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorE.Tag.ToString() == "connected to PracticeSelectorVoltageEq2")
                        RWellipseEq2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorE.Tag.ToString() == "connected to PracticeSelectorVoltageEq3")
                        RWellipseEq3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorE.Tag.ToString() == "connected to PracticeSelectorVoltageEq4")
                        RWellipseEq4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorE.Tag.ToString() == "connected to PracticeSelectorVoltageDC1")
                        RWellipseDC1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorE.Tag.ToString() == "connected to PracticeSelectorVoltageDC2")
                        RWellipseDC2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorE.Tag.ToString() == "connected to PracticeSelectorVoltageDC3")
                        RWellipseDC3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorE.Tag.ToString() == "connected to PracticeSelectorVoltageDC4")
                        RWellipseDC4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorE.Tag.ToString() == "connected to PracticeSelectorVoltagePilot1")
                        RWellipsePilot1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorE.Tag.ToString() == "connected to PracticeSelectorVoltagePilot2")
                        RWellipsePilot2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorE.Tag.ToString() == "connected to PracticeSelectorVoltagePilot3")
                        RWellipsePilot3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorE.Tag.ToString() == "connected to PracticeSelectorVoltagePilot4")
                        RWellipsePilot4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorE.Tag.ToString() == "connected to PracticeSelectorVoltageIBP1")
                        RWellipseIBP1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorE.Tag.ToString() == "connected to PracticeSelectorVoltageIBP2")
                        RWellipseIBP2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorE.Tag.ToString() == "connected to PracticeSelectorVoltageIBP3")
                        RWellipseIBP3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorE.Tag.ToString() == "connected to PracticeSelectorVoltageIBP4")
                        RWellipseIBP4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));
                }
            }

            if (PracticeSelectorVoltageEq1.Tag != null)
            {
                if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelectorVoltageDC1")
                {
                    RWellipseDC1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    RWellipseEq1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelectorVoltageDC2")
                    {
                    RWellipseDC2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    RWellipseEq1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelectorVoltageDC3")
                {
                    RWellipseDC3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    RWellipseEq1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelectorVoltageDC4")
                {
                    RWellipseDC4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    RWellipseEq1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                {
                    RWellipseEq1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelector1")
                        RWellipse1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelector2")
                        RWellipse2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelector3")
                        RWellipse3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelector4")
                        RWellipse4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelector5")
                        RWellipse5.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelector6")
                        RWellipse6.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelector7")
                        RWellipse7.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelector8")
                        RWellipse8.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelector9")
                        RWellipse9.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelectorA")
                        RWellipseA.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelectorB")
                        RWellipseB.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelectorC")
                        RWellipseC.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelectorD")
                        RWellipseD.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelectorE")
                        RWellipseE.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelectorVoltageEq2")
                        RWellipseEq2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelectorVoltageEq3")
                        RWellipseEq3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelectorVoltageEq4")
                        RWellipseEq4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelectorVoltageDC1")
                        RWellipseDC1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelectorVoltageDC2")
                        RWellipseDC2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelectorVoltageDC3")
                        RWellipseDC3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelectorVoltageDC4")
                        RWellipseDC4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelectorVoltagePilot1")
                        RWellipsePilot1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelectorVoltagePilot2")
                        RWellipsePilot2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelectorVoltagePilot3")
                        RWellipsePilot3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelectorVoltagePilot4")
                        RWellipsePilot4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelectorVoltageIBP1")
                        RWellipseIBP1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelectorVoltageIBP2")
                        RWellipseIBP2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelectorVoltageIBP3")
                        RWellipseIBP3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelectorVoltageIBP4")
                        RWellipseIBP4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));
                }
            }

            if (PracticeSelectorVoltageEq2.Tag != null)
            {
                if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelectorVoltageDC1")
                {
                    RWellipseDC1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    RWellipseEq2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelectorVoltageDC2")
                {
                    RWellipseDC2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    RWellipseEq2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelectorVoltageDC3")
                {
                    RWellipseDC3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    RWellipseEq2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelectorVoltageDC4")
                {
                    RWellipseDC4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    RWellipseEq2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                {
                    RWellipseEq2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelector1")
                        RWellipse1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq1.Tag.ToString() == "connected to PracticeSelector2")
                        RWellipse2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelector3")
                        RWellipse3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelector4")
                        RWellipse4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelector5")
                        RWellipse5.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelector6")
                        RWellipse6.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelector7")
                        RWellipse7.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelector8")
                        RWellipse8.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelector9")
                        RWellipse9.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelectorA")
                        RWellipseA.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelectorB")
                        RWellipseB.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelectorC")
                        RWellipseC.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelectorD")
                        RWellipseD.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelectorE")
                        RWellipseE.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelectorVoltageEq1")
                        RWellipseEq1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelectorVoltageEq3")
                        RWellipseEq3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelectorVoltageEq4")
                        RWellipseEq4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelectorVoltageDC1")
                        RWellipseDC1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelectorVoltageDC2")
                        RWellipseDC2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelectorVoltageDC3")
                        RWellipseDC3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelectorVoltageDC4")
                        RWellipseDC4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelectorVoltagePilot1")
                        RWellipsePilot1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelectorVoltagePilot2")
                        RWellipsePilot2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelectorVoltagePilot3")
                        RWellipsePilot3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelectorVoltagePilot4")
                        RWellipsePilot4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelectorVoltageIBP1")
                        RWellipseIBP1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelectorVoltageIBP2")
                        RWellipseIBP2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelectorVoltageIBP3")
                        RWellipseIBP3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq2.Tag.ToString() == "connected to PracticeSelectorVoltageIBP4")
                        RWellipseIBP4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));
                }
            }

            if (PracticeSelectorVoltageEq3.Tag != null)
            {
                if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelectorVoltageDC1")
                {
                    RWellipseDC1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    RWellipseEq3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelectorVoltageDC2")
                {
                    RWellipseDC2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    RWellipseEq3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelectorVoltageDC3")
                {
                    RWellipseDC3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    RWellipseEq3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelectorVoltageDC4")
                {
                    RWellipseDC4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    RWellipseEq3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                {
                    RWellipseEq3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelector1")
                        RWellipse1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelector2")
                        RWellipse2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelector3")
                        RWellipse3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelector4")
                        RWellipse4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelector5")
                        RWellipse5.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelector6")
                        RWellipse6.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelector7")
                        RWellipse7.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelector8")
                        RWellipse8.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelector9")
                        RWellipse9.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelectorA")
                        RWellipseA.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelectorB")
                        RWellipseB.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelectorC")
                        RWellipseC.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelectorD")
                        RWellipseD.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelectorE")
                        RWellipseE.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelectorVoltageEq1")
                        RWellipseEq1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelectorVoltageEq2")
                        RWellipseEq2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelectorVoltageEq4")
                        RWellipseEq4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelectorVoltageDC1")
                        RWellipseDC1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelectorVoltageDC2")
                        RWellipseDC2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelectorVoltageDC3")
                        RWellipseDC3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelectorVoltageDC4")
                        RWellipseDC4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelectorVoltagePilot1")
                        RWellipsePilot1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelectorVoltagePilot2")
                        RWellipsePilot2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelectorVoltagePilot3")
                        RWellipsePilot3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelectorVoltagePilot4")
                        RWellipsePilot4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelectorVoltageIBP1")
                        RWellipseIBP1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelectorVoltageIBP2")
                        RWellipseIBP2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelectorVoltageIBP3")
                        RWellipseIBP3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq3.Tag.ToString() == "connected to PracticeSelectorVoltageIBP4")
                        RWellipseIBP4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));
                }
            }

            if (PracticeSelectorVoltageEq4.Tag != null)
            {
                if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelectorVoltageDC1")
                {
                    RWellipseDC1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    RWellipseEq4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelectorVoltageDC2")
                {
                    RWellipseDC2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    RWellipseEq4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelectorVoltageDC3")
                {
                    RWellipseDC3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    RWellipseEq4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelectorVoltageDC4")
                {
                    RWellipseDC4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    RWellipseEq4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                    score++;
                }
                else
                {
                    RWellipseEq4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelector1")
                        RWellipse1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelector2")
                        RWellipse2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelector3")
                        RWellipse3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelector4")
                        RWellipse4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelector5")
                        RWellipse5.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelector6")
                        RWellipse6.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelector7")
                        RWellipse7.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelector8")
                        RWellipse8.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelector9")
                        RWellipse9.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelectorA")
                        RWellipseA.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelectorB")
                        RWellipseB.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelectorC")
                        RWellipseC.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelectorD")
                        RWellipseD.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelectorE")
                        RWellipseE.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelectorVoltageEq1")
                        RWellipseEq1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelectorVoltageEq2")
                        RWellipseEq2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelectorVoltageEq3")
                        RWellipseEq3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelectorVoltageDC1")
                        RWellipseDC1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelectorVoltageDC2")
                        RWellipseDC2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelectorVoltageDC3")
                        RWellipseDC3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelectorVoltageDC4")
                        RWellipseDC4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelectorVoltagePilot1")
                        RWellipsePilot1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelectorVoltagePilot2")
                        RWellipsePilot2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelectorVoltagePilot3")
                        RWellipsePilot3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelectorVoltagePilot4")
                        RWellipsePilot4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelectorVoltageIBP1")
                        RWellipseIBP1.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelectorVoltageIBP2")
                        RWellipseIBP2.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelectorVoltageIBP3")
                        RWellipseIBP3.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

                    if (PracticeSelectorVoltageEq4.Tag.ToString() == "connected to PracticeSelectorVoltageIBP4")
                        RWellipseIBP4.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));
                }
            }

            if (score == 11)
            {
                practice2done = true;
                activePractice++;
                StartNextPracticeButton.Visibility = Visibility.Visible;
            }
            else
            {
                ReStartPracticeButton.Visibility = Visibility.Visible;
            }
            
            if (practice1done && practice2done && practice3done)
            {
                TestMenu.Visibility = Visibility.Visible;
            }
        }

        List<double> CalculatePractice3Radius()
        {
            double d_ul, d_dl;
            double prelog_du_ul, prelog_du_dl;

            Lul = Pout_ay + G_ay + G_bs - L_bs - Pin_bs + G - 18;
            Ldl = Pout_bs + G_bs - L_bs + G_ay - Pin_ay - 18;

            double A_ht_ul = Math.Round((1.11 * Math.Log10(Ful) - 0.7) * hr - (1.56 * Math.Log10(Ful) - 0.8), 4);
            double A_ht_dl = Math.Round((1.11 * Math.Log10(Fdl) - 0.7) * hr - (1.56 * Math.Log10(Fdl) - 0.8),4);

            prelog_du_ul = (double) ( -46.3 - 33.9 * Math.Log10(Ful) + 13.82 * Math.Log10(ht) + A_ht_ul + Lul ) / ( 44.9 - 6.55 * Math.Log10(ht) );
            prelog_du_dl = (double) ( -46.3 - 33.9 * Math.Log10(Fdl) + 13.82 * Math.Log10(ht) + A_ht_dl + Ldl) / (44.9 - 6.55 * Math.Log10(ht) );

            d_ul = Math.Round(Math.Pow(10, prelog_du_ul),4);
            d_dl = Math.Round(Math.Pow(10, prelog_du_dl),4);
            
            return new List<double> { d_ul, d_dl };
        }

        private void RenderRadiusesPractice3(object sender, RoutedEventArgs e)
        {
            foreach (UIElement element in Practice3Grid.Children)
            {
                if (element is TextBox)
                {
                    double t;
                    TextBox textBox = (TextBox) element;

                    if (! double.TryParse(textBox.Text,out t))
                    {
                        MessageBox.Show("Значения могуть быть только цифрами, \nдробные значения записываются через запятую");
                        return;
                    }

                    if ((textBox.Name == "htValue" || textBox.Name == "hrValue")&& t < 0)
                    {
                        textBox.Text = (-t).ToString();
                        MessageBox.Show("Значения высоты подвеса антенн \nне могут иметь отрицательные значения");
                        return;
                    }

                    if (Math.Abs(t) > 999)
                    {
                        if (textBox.Name == "CalculatedValue")
                        {
                            continue;
                        }

                        textBox.Text = "999";
                    }
                }
            }

            G_bs = Convert.ToDouble(G_bsValue.Text);
            L_bs = Convert.ToDouble(L_bsValue.Text);
            Pin_bs = Convert.ToDouble(Pin_bsValue.Text);
            Pout_bs = Convert.ToDouble(Pout_bsValue.Text);
            G = Convert.ToDouble(GValue.Text);
            hr = Convert.ToDouble(hrValue.Text);
            ht = Convert.ToDouble(htValue.Text);

            List<double> doubles = CalculatePractice3Radius();
            double calculatedvalue;
            if (doubles[0] < doubles[1])
            {     
                DownLink.Width = doubles[0] * 1000 / 5;
                dlLabel.Content = Math.Round(doubles[0], 4) * 1000 + " м";
                calculatedvalue = double.Parse((Math.Round(doubles[0], 4) * 1000).ToString());
            }
            else
            {
                DownLink.Width = doubles[1] * 1000 / 5;
                dlLabel.Content = Math.Round(doubles[1], 4) * 1000 + " м";
                calculatedvalue = double.Parse((Math.Round(doubles[0], 4) * 1000).ToString());
            }
            double cv;
            if (! double.TryParse(CalculatedValue.Text, out cv))
            {
                MessageBox.Show("Значения могуть быть только цифрами, \nдробные значения записываются через запятую");
                return;
            }

            if (cv == calculatedvalue)
            {
                rwLabel.Content = "Рассчеты \nверны";
                practice3done = true;
            }
            else
            {
                rwLabel.Content = "Рассчеты \nне верны";
            }

            if (practice1done && practice2done && practice3done)
            {
                TestMenu.Visibility = Visibility.Visible;
            }
        }

        private void ShowPractice3(object sender, RoutedEventArgs e)
        {
            whoLabel.Visibility = Visibility.Hidden;
            ghImage.Visibility = Visibility.Hidden;
            ContentTest.Visibility = Visibility.Hidden;
            Practice2Canvas.Visibility = Visibility.Hidden;
            Practice2ScrollViewer.Visibility = Visibility.Hidden;
            Practice2Image.Visibility = Visibility.Hidden;
            Practice1Canvas.Visibility = Visibility.Hidden;
            Practice1Image.Visibility = Visibility.Hidden;
            ConfirmVariantButton.Visibility = Visibility.Hidden;
            StartNextPracticeButton.Visibility = Visibility.Hidden;
            StartPracticeButton.Visibility = Visibility.Hidden;    
            ReStartPracticeButton.Visibility = Visibility.Hidden;
            CheckPracticeButton.Visibility = Visibility.Hidden;
            VariantPicker.Visibility = Visibility.Hidden;
            TheoryView.Visibility = Visibility.Hidden;

            ContentPractice.Visibility = Visibility.Visible;
            Practice3Panel.Visibility = Visibility.Visible;
        }

        private void ghImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var destinationurl = "https://github.com/KysTT/uisiDiploma";
            var sInfo = new System.Diagnostics.ProcessStartInfo(destinationurl)
            {
                UseShellExecute = true,
            };
            System.Diagnostics.Process.Start(sInfo);
        }

        // метод при нажатии на кнопку "проверить"
        private void CheckPracticeClick(object sender, RoutedEventArgs e)
        {
            // скрываем кнопку "проверить" и показываем кнопку "заново"
            CheckPracticeButton.Visibility = Visibility.Hidden;

            // если решали 1-ую п.з.
            if (activePractice == 1)
            {
                CalculatePractice1TotalScore();
                return;
            }
            if (activePractice == 2)
            { 
                CalculatePractice2Score(); 
                return;
            }
        }

        private void ContinuePracticeClick(object sender, RoutedEventArgs e)
        {

            if (activePractice == 2) 
            {
                Practice2Click(sender, e);
            }

            if (activePractice == 3)
            {
                ShowPractice3(sender, e);
            }
            
            StartNextPracticeButton.Visibility = Visibility.Hidden;
            Practice1Canvas.Visibility = Visibility.Hidden;
            Practice2Canvas.Visibility = Visibility.Hidden;
        }

        private void RestartPractice2()
        {
            List<Line> toRemoveLines = new List<Line>();
            List<Ellipse> toRemoveCircles = new List<Ellipse>();

            foreach (var element in Practice2Canvas.Children)
            {

                if (element is Line)
                {
                    toRemoveLines.Add((Line)element);
                }

                if (element is Ellipse)
                {
                    toRemoveCircles.Add((Ellipse)element);
                }
            }

            for (int i = 0; i < toRemoveLines.Count; i++)
            {
                Practice2Canvas.Children.Remove(toRemoveLines[i]);
            }

            for (int i = 0; i < toRemoveCircles.Count; i++)
            {
                Practice2Canvas.Children.Remove(toRemoveCircles[i]);
            }

            foreach (var element in Practice2Canvas.Children)
            {
                if (element is ComboBox) 
                { 
                    var ccb = (ComboBox)element;
                    ccb.SelectedItem = null;
                    ccb.Tag = null;
                }
            }
        }

        // метод для перезапуска п.з. 
        // вызывается нажатием на кнопку "перезапуска"
        private void ReStartPracticeClick(object sender, RoutedEventArgs e)
        {
            // показываем кнопку начать практику
            // скрываем кнопку "перезапуска"
            StartPracticeButton.Visibility = Visibility.Visible;
            ReStartPracticeButton.Visibility = Visibility.Hidden;

            // если текущая п.з. == 1 , тогда
            // запускаем метода для отображения 
            // и начала соотвествующей практике

            if (activePractice == 1)
            {
                Practice1Click(sender, e);
            }

            if (activePractice == 2)
            {
                RestartPractice2();
            }
        }

        // метод для запуска теста по кнопке "начать тест"

        private void TestClick(object sender, RoutedEventArgs e)
        {
            whoLabel.Visibility = Visibility.Hidden;
            ghImage.Visibility = Visibility.Hidden;

            // ChangeVisibilityTest - метод, который скрывает 
            // те или иные элементы интерфейса в зависимости
            // от видимости панели с тестом

            ChangeVisibilityTest();

            // RestartGame перезапускает тест

            RestartGame();
        }

        // метод для изменения видимости элементов

        private void ChangeVisibilityTest()
        {
            ContentPractice.Visibility = Visibility.Hidden;
            Practice3Panel.Visibility = Visibility.Hidden;

            // скрыть кнопки с вариантами ответов

            ans1.Visibility = Visibility.Hidden;
            ans2.Visibility = Visibility.Hidden;
            ans3.Visibility = Visibility.Hidden;
            ans4.Visibility = Visibility.Hidden;

            // если видна панель с тестом

            if (ContentTest.Visibility == Visibility.Visible)
            {
                // скрыть все относящееся к тесту и показать 
                // меню "тест" и браузер с теорией

                ContentTheory1.Visibility = Visibility.Visible;
                TestMenu.Visibility = Visibility.Visible;

                ContentTest.Visibility = Visibility.Hidden;
                ButtonStartTest.Visibility = Visibility.Hidden;
            }
            else
            {
                // иначе показать меню "тест" 
                // и кнопку "начать тест"
                ContentTest.Visibility = Visibility.Visible;
                ButtonStartTest.Visibility = Visibility.Visible;

                ContentTheory1.Visibility = Visibility.Hidden;
                ButtonEndTest.Visibility = Visibility.Hidden;
            }
        }

        // метод перезапуска теста

        private void RestartGame()
        {
            StartingLabel.Visibility = Visibility.Visible;

            // правильных ответов нет
            score = 0;
            
            // начальный вопрос 0
            questionNum = 0;

            // индекс вопроса 0
            i = 0;
            
            // запуск теста
         
            StartGame();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            TestMenu.Visibility = Visibility.Visible;
            timer.Stop();
            testTimes = 0;
        }

        // метод запуска теста

        private void StartGame()
        {

            // Список номеров вопросов который в последствии рандомизируется => случайные номера вопросов
            testquestionNumbers = new() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 };
            
            // перемешиваем элементы массива (номера вопросов)
            var randomList = testquestionNumbers.OrderBy(a => Guid.NewGuid()).ToList();
            testquestionNumbers = randomList.Take(15).ToList();
        }

        // метод проверки ответов, вызывается нажатием
        // любой из ans[1-4]

        private void CheckAnswer(object sender, RoutedEventArgs e)
        {
            // получаем кнопку, на которую нажал пользователь

            Button? senderButton = sender as Button;

            // если у этой кнопки метка == 1 (верный вариант ответа)
            // засчитываем ответ

            if (senderButton.Tag.ToString() == "1") 
                score++;

            // увеличиваем номер вопроса на 1

            questionNum++;

            // в текстовом поле scoreText отображаем информацию
            // о том, на сколько вопросов из их количества был дан ответ

            scoreText.Content = "Текущий вопрос " + (questionNum + 1) + " из " + testquestionNumbers.Count + "\nПопытка номер " + testTimes;
            
            // следующий вопрос
            
            NextQuestion();
        }

        // метод для отображения вопросов

        private void NextQuestion()
        {
            StartingLabel.Visibility = Visibility.Hidden;

            // показываем на экране
            // txtQuestion - текст вопроса
            // qImage - картинка для вопроса

            txtQuestion.Visibility = Visibility.Visible;
           
            // метки кнопок обнуляем
            ans1.Tag = ""; ans2.Tag = ""; ans3.Tag = ""; ans4.Tag = "";

            // если номер текущего вопроса меньше количества вопросов
            // тогда i приравниваем номеру вопроса из 
            // массива testquestionNumbers с индексом questionNum

            if (questionNum < testquestionNumbers.Count)
                i = testquestionNumbers[questionNum];

            // иначе считаем результат и выводим в 
            // отдельном окне
            else
            {
                // локальная переменная totalScore типа double (с плавающей запятой)
                // для подсчёта процентов
                double totalScore;
                
                // задали ей значение равное количество правильных ответов 
                // поделенное на число всех вопросов 
                
                totalScore = (float)score / testquestionNumbers.Count;

                // получили ответ типа 0.66666(6)
                // и округлили до 2х знаков после запятой

                totalScore = Math.Round(totalScore, 2);
                
                // в текстовой строке вывели результат
                scoreText.Content = "Правильных ответов " + score + "/" + testquestionNumbers.Count + " " + (totalScore * 100) + "%" + "\nПопытка номер " + testTimes;

                // скрыли все кнопки с вариантами ответов 
                
                ans1.Visibility = Visibility.Hidden;
                ans2.Visibility = Visibility.Hidden;
                ans3.Visibility = Visibility.Hidden;
                ans4.Visibility = Visibility.Hidden;
                
                // скрыли текст вопроса
                txtQuestion.Visibility = Visibility.Hidden;
                
                // вывели на экран всплывающее окно с результатом
                MessageBox.Show("Правильных ответов " + score + "/" + testquestionNumbers.Count + " " + (totalScore * 100) + "%" + "\nПопытка номер " + testTimes);
            }

            // конструкция switch (i) содержит в себе всю информацию о вопросе

            switch (i)
            {
                // case [1-10] - случаи, или вопросы (case 1 - вопрос 1)

                case 1:
                   
                    // задали необходимый текст вопроса
                    txtQuestion.Text = "На каком стандарте были основаны сети первого поколения?";

                    // задали варианты ответов
                    ans1.Content = "NMT";
                    ans2.Content = "UMTS";
                    ans3.Content = "LTE";
                    ans4.Content = "GSM";

                    // присвоили метку правильного кнопке
                    ans2.Tag = "1";

                    // конец случая
                    break;

                    // ниже все по той же схеме
                case 2:
                    txtQuestion.Text = "На каком стандарте были основаны сети второго поколения?";

                    ans1.Content = "LTE";
                    ans2.Content = "NMT";
                    ans3.Content = "UMTS";
                    ans4.Content = "GSM";
                    ans4.Tag = "1";
                    break;

                case 3:

                    txtQuestion.Text = "На каком стандарте были основаны сети третьего поколения?";

                    ans1.Content = "NMT";
                    ans2.Content = "GSM";
                    ans3.Content = "LTE";
                    ans4.Content = "UMTS";
                    ans4.Tag = "1";
                    break;

                case 4:
                    txtQuestion.Text = "На каком стандарте были основаны сети четвертого поколения?";

                    ans1.Content = "LTE";
                    ans2.Content = "GSM";
                    ans3.Content = "NMT";
                    ans4.Content = "UMTS";
                    ans1.Tag = "1";
                    break;

                case 5:
                    txtQuestion.Text = "Какой блок у базовой станции включает в себя все необходимые функции для клиппирования сигнала, цифро-аналогового преобразования, модуляции и усиления радиочастоты для тракта передатчика?";

                    ans1.Content = "блок фильтрации";
                    ans2.Content = "плата передатчика";
                    ans3.Content = "радио блок";
                    ans4.Content = "цифровая стойка";
                    ans3.Tag = "1";
                    break;

                case 6:
                    txtQuestion.Text = "Какой блок у базовой станции содержит малошумящий усилитель (LNA) и выполняет разделение несущей радиочастоты?";

                    ans1.Content = "плата передатчика";
                    ans2.Content = "блок фильтрации";
                    ans3.Content = "цифровая стойка";
                    ans4.Content = "радио блок";
                    ans2.Tag = "1";
                    break;

                case 7:
                    txtQuestion.Text = "Какой блок у базовой станции обеспечивает разделение ячеек, объединение каналов, кодирование, модуляцию и распространение, а также обработку транспортных каналов?";

                    ans1.Content = "радио блок";
                    ans2.Content = "блок фильтрации";
                    ans3.Content = "цифровая стойка";
                    ans4.Content = "плата передатчика";
                    ans4.Tag = "1";
                    break;

                case 8:
                    txtQuestion.Text = "Какая характеристика антенны определяет, какие частоты данная антенна может пропускать и принимать?";

                    ans1.Content = "рабочий частотный диапазон";
                    ans2.Content = "коэффициент направленного действия";
                    ans3.Content = "диаграмма направленности";
                    ans4.Content = "поляризация";
                    ans1.Tag = "1";
                    break;

                case 9:
                    txtQuestion.Text = "Какая характеристика антенны определяет, в какой плоскости будет излучаться сигнал?";

                    ans1.Content = "диаграмма направленности";
                    ans2.Content = "коэффициент направленного действия";
                    ans3.Content = "рабочий частотный диапазон";
                    ans4.Content = "поляризация";
                    ans4.Tag = "1";
                    break;

                case 10:

                    txtQuestion.Text = "Какая характеристика антенны отражает, в каких направлениях будет излучаться сигнал, и с какой силой?";

                    ans1.Content = "рабочий частотный диапазон";
                    ans2.Content = "диаграмма направленности";
                    ans3.Content = "коэффициент направленного действия";
                    ans4.Content = "поляризация";
                    ans2.Tag = "1";
                    break;

                case 11:

                    txtQuestion.Text = "Какая характеристика антенны показывает отношение мощности излучаемого сигнала в заданном направлении к мощности сигнала, если бы антенна излучала по всем направлениям?";

                    ans1.Content = "поляризация";
                    ans2.Content = "диаграмма направленности";
                    ans3.Content = "коэффициент направленного действия";
                    ans4.Content = "рабочий частотный диапазон";
                    ans3.Tag = "1";
                    break;

                case 12:

                    txtQuestion.Text = "Какой тип антенн является всенаправленной?";

                    ans1.Content = "параболическая антенна";
                    ans2.Content = "омни-антенна";
                    ans3.Content = "панельная антенна";
                    ans4.Content = "логопериодическая антенна";
                    ans2.Tag = "1";
                    break;

                case 13:

                    txtQuestion.Text = "Какой тип антенн использует кросс-поляризационные диполи для передачи сигнала?";

                    ans1.Content = "панельная антенна";
                    ans2.Content = "логопериодическая антенна";
                    ans3.Content = "параболическая антенна";
                    ans4.Content = "омни-антенна";
                    ans1.Tag = "1";
                    break;

                case 14:

                    txtQuestion.Text = "Какой тип антенн используется как в спутниковой, так и в радиорелейной связи?";

                    ans1.Content = "логопериодическая антенна";
                    ans2.Content = "омни-антенна";
                    ans3.Content = "панельная антенна";
                    ans4.Content = "параболическая антенна";
                    ans4.Tag = "1";
                    break;

                case 15:

                    txtQuestion.Text = "Какой основной элемент сети стандарта LTE отвечает за обработку и маршрутизации пакетных данных поступающих из/в подсистему базовых станций?";

                    ans1.Content = "обслуживающий шлюз";
                    ans2.Content = "публичный шлюз";
                    ans3.Content = "узел выставления счетов";
                    ans4.Content = "узел управления мобильностью";
                    ans1.Tag = "1";
                    break;

                case 16:

                    txtQuestion.Text = "Какой основной элемент сети стандарта LTE отвечает за управление мобильностью абонентов сети?";

                    ans1.Content = "обслуживающий шлюз";
                    ans2.Content = "узел управления мобильностью";
                    ans3.Content = "узел выставления счетов";
                    ans4.Content = "публичный шлюз";
                    ans2.Tag = "1";
                    break;

                case 17:

                    txtQuestion.Text = "Какой основной элемент сети стандарта LTE отвечает за подключение к сети других операторов?";

                    ans1.Content = "публичный шлюз";
                    ans2.Content = "обслуживающий шлюз";
                    ans3.Content = "узел управления мобильностью";
                    ans4.Content = "узел выставления счетов";
                    ans1.Tag = "1";
                    break;

                case 18:

                    txtQuestion.Text = "Какой основной элемент сети стандарта LTE отвечает за управление начислением платы за оказанные услуги связи?";

                    ans1.Content = "узел управления мобильностью";
                    ans2.Content = "публичный шлюз";
                    ans3.Content = "обслуживающий шлюз";
                    ans4.Content = "узел выставления счетов";
                    ans4.Tag = "1";
                    break;

                case 19:

                    txtQuestion.Text = "Какова скорость передачи сетей поколения 5G?";

                    ans1.Content = "5 Гбит/с";
                    ans2.Content = "326,4 Мбит/с";
                    ans3.Content = "500 Мбит/с";
                    ans4.Content = "622,080 Мбит/с";
                    ans1.Tag = "1";
                    break;

                case 20:

                    txtQuestion.Text = "В каком диапазоне частот происходит наибольшее поглощение частицами воздуха?";

                    ans1.Content = "57 – 64 ГГц";
                    ans2.Content = "3 – 57 ГГц";
                    ans3.Content = "164 – 200 ГГц";
                    ans4.Content = "64 – 164 ГГц";
                    ans1.Tag = "1";
                    break;

                case 21:

                    txtQuestion.Text = "В каком диапазоне частот происходит наибольшее поглощение частицами водяного пара?";

                    ans1.Content = "57 – 64 ГГц";
                    ans2.Content = "3 – 57 ГГц";
                    ans3.Content = "164 – 200 ГГц";
                    ans4.Content = "64 – 164 ГГц";
                    ans3.Tag = "1";
                    break;

                case 22:

                    txtQuestion.Text = "Как называют зону покрытия отдельной базовой станции в сотовой связи?";

                    ans1.Content = "сота";
                    ans2.Content = "радиус действия";
                    ans3.Content = "дальность связи";
                    ans4.Content = "кольцо";
                    ans1.Tag = "1";
                    break;

                case 23:

                    txtQuestion.Text = "Тип распространения сигнала, при котором на приемную антенну практически всегда приходит множество копий сигнала с разными уровнями и разными задержками по времени?";

                    ans1.Content = "с замиранием";
                    ans2.Content = "обычное";
                    ans3.Content = "мультилучевое";
                    ans4.Content = "многолучевое";
                    ans4.Tag = "1";
                    break;

                case 24:

                    txtQuestion.Text = "Что не входят в состав новых сервисов пятого поколения?";

                    ans1.Content = "интернет вещей";
                    ans2.Content = "интернет машин";
                    ans3.Content = "видеотрафик";
                    ans4.Content = "финансовые технологии";
                    ans3.Tag = "1";
                    break;

                case 25:

                    txtQuestion.Text = "Какой диапазон длин волн используется в сетях пятого поколения?";

                    ans1.Content = "миллиметровый";
                    ans2.Content = "сантиметровый";
                    ans3.Content = "нанометровый";
                    ans4.Content = "метровый";
                    ans1.Tag = "1";
                    break;

                case 26:

                    txtQuestion.Text = "Как называется технология формирования луча определенном направлении?";

                    ans1.Content = "beamforming";
                    ans2.Content = "MIMO";
                    ans3.Content = "SDMA";
                    ans4.Content = "all-spectrum access";
                    ans1.Tag = "1";
                    break;

                case 27:

                    txtQuestion.Text = "Задачами какого сервиса являются анализ среды, сбор данных, моделирование, обоснование и автоматическая настройка датчиков?";

                    ans1.Content = "IoT";
                    ans2.Content = "D2D";
                    ans3.Content = "IoV";
                    ans4.Content = "M2M";
                    ans1.Tag = "1";
                    break;

                case 28:

                    txtQuestion.Text = "Какой сервис представляет собой сеть взаимосвязанных транспортных средств для надежного управления дорожным движением и снижения вероятности столкновений?";

                    ans1.Content = "D2D";
                    ans2.Content = "IoT";
                    ans3.Content = "M2M";
                    ans4.Content = "IoV";
                    ans4.Tag = "1";
                    break;

                case 29:

                    txtQuestion.Text = "Какой сервис представляет собой прямую связь между двумя мобильными пользователями минуя базовую станцию (BS) или базовую сеть?";

                    ans1.Content = "M2M";
                    ans2.Content = "IoT";
                    ans3.Content = "D2D";
                    ans4.Content = "IoV";
                    ans3.Tag = "1";
                    break;

                case 30:

                    txtQuestion.Text = "Задачами какого сервиса являются автоматизированную генерацию данных, обработку, передачу и обмен между интеллектуальными устройствами с минимальным вмешательством человека?";

                    ans1.Content = "D2D";
                    ans2.Content = "M2M";
                    ans3.Content = "IoT";
                    ans4.Content = "IoV";
                    ans2.Tag = "1";
                    break;
            }
        }

        // метод который показывет 
        // панель меню после теста

        private void ShowAfterTest()
        {
            NavMenu.Visibility = Visibility.Visible;
            ContentTheory1.Visibility = Visibility.Visible;
        }

        // метод который скрывает 
        // панель меню пока идет тест

        private void HideWhileTest()
        {
            NavMenu.Visibility = Visibility.Collapsed;
            ContentTheory1.Visibility = Visibility.Hidden;
        }


        // метод который вызывается нажатием кноки "начать тест"

        private void ButtonStartTestClick(object sender, RoutedEventArgs e)
        {
            testTimes++;
            // скрываем меню "тест"

            TestMenu.Visibility = Visibility.Hidden;

            // показываем кнопки с вариантом ответа
            
            ans1.Visibility = Visibility.Visible;
            ans2.Visibility = Visibility.Visible;
            ans3.Visibility = Visibility.Visible;
            ans4.Visibility = Visibility.Visible;

            // скрываем остальные элементы
            
            HideWhileTest();

            // скрываем кнопку "начать тест"

            ButtonStartTest.Visibility = Visibility.Hidden;
            
            // начинаем тест
            StartGame();
            
            // показываем кнопку закончить тест

            ButtonEndTest.Visibility = Visibility.Visible;
            
            // отображаем информацию о текущем вопросе
            
            scoreText.Content = "Текущий вопрос " + 1 + " из " + testquestionNumbers.Count + "\nПопытка номер " + testTimes;
            
            // следующий вопрос

            NextQuestion();
        }

        // метод который вызывается нажатием кнопки "закончить тест"

        private void ButtonEndTestClick(object sender, RoutedEventArgs e)
        {
            // очищаем весь интерфейс
            txtQuestion.Text = "";
            scoreText.Content = "";
            
            // изменяем видимость элементов теста

            ChangeVisibilityTest();
            
            // показываем меню 
            ShowAfterTest();
            
            // скрываем кнопку "закончить тест"
            ButtonEndTest.Visibility = Visibility.Hidden;

            if (testTimes >= 3)
            {
                timer.Tick += new EventHandler(timer_Tick);
                timer.Interval = new TimeSpan(0, 10, 0);
                timer.Start();
                TestMenu.Visibility = Visibility.Collapsed;
                ContentTest.Visibility = Visibility.Collapsed;
                MessageBox.Show("Вы израсходовали 3 поптыки\nтеперь придется подождать :)");
                Theory1Click(sender, e);
                return;
            }
        }
    }
}
