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
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

        // вариант студента, необходим для певой практической задачи
        string studentsVariant = "";

        // Список номеров вопросов который в последствии рандомизируется => случайные номера вопросов
        List<int> testquestionNumbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        
        // activePractice - номер текущего практического задания
        int activePractice = 0;

        // totalPracticeScore - количество правльно выполненных практических заданий (п.з.)
        // если totalPracticeScore == n, где n - общее число п.з. , тогда открывается тест
        int totalPracticeScore = 0;

        // questionNum - номер текущего вопроса
        // i - индекс вопроса в testquestionNumbers
        // score - количество правильных ответов в тесте
        int questionNum, i, score;

        // myDraggableImage - изображение, которые впоследствии надо будет передвигать
        Border myDraggableImage = new Border();

        public MainWindow()
        {
            InitializeComponent();
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
                if (TheoryView.Source == (new Uri(Environment.CurrentDirectory + "\\theory\\Отличия поколений.htm")))
               
                // тогда скрыть браузер
                TheoryView.Visibility = Visibility.Hidden;

            // Если открыт не текущий раздел теории
            if (TheoryView.Source != (new Uri(Environment.CurrentDirectory + "\\theory\\Отличия поколений.htm")))
                
                // тогда открыть нужный
                TheoryView.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\Отличия поколений.htm"));
        }

        // Метод нажатия на элемент меню "Теория2", в котором включается отображение окна браузера
        // с загружением в него теоретического материала
        private void Theory2Click(object sender, RoutedEventArgs e)
        {
            // тоже самое что и в первом случае
            if (TheoryView.Visibility != Visibility.Visible) 
                TheoryView.Visibility = Visibility.Visible;
            else
                if (TheoryView.Source == (new Uri(Environment.CurrentDirectory + "\\theory\\Оборудование мобильных сетей.htm"))) 
                TheoryView.Visibility = Visibility.Hidden;

            if (TheoryView.Source != (new Uri(Environment.CurrentDirectory + "\\theory\\Оборудование мобильных сетей.htm"))) 
                TheoryView.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\Оборудование мобильных сетей.htm"));

        }

        // Метод нажатия на элемент меню "Теория3", в котором включается отображение окна браузера
        // с загружением в него теоретического материала
        private void Theory3Click(object sender, RoutedEventArgs e)
        {
            // тоже самое что и в первом случае

            if (TheoryView.Visibility != Visibility.Visible) 
                TheoryView.Visibility = Visibility.Visible;
            else
                if (TheoryView.Source == (new Uri(Environment.CurrentDirectory + "\\theory\\Антенны.htm"))) 
                TheoryView.Visibility = Visibility.Hidden;

            if (TheoryView.Source != (new Uri(Environment.CurrentDirectory + "\\theory\\Антенны.htm"))) 
                TheoryView.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\Антенны.htm"));
        }

        // Метод нажатия на элемент меню "Теория4", в котором включается отображение окна браузера
        // с загружением в него теоретического материала
        private void Theory4Click(object sender, RoutedEventArgs e)
        {
            // тоже самое что и в первом случае

            if (TheoryView.Visibility != Visibility.Visible) 
                TheoryView.Visibility = Visibility.Visible;
            else
                if (TheoryView.Source == (new Uri(Environment.CurrentDirectory + "\\theory\\Распространение радиосигнала.htm"))) 
                TheoryView.Visibility = Visibility.Hidden;

            if (TheoryView.Source != (new Uri(Environment.CurrentDirectory + "\\theory\\Распространение радиосигнала.htm"))) 
                TheoryView.Source = (new Uri(Environment.CurrentDirectory + "\\theory\\Распространение радиосигнала.htm"));
        }

        // Метод для перетаскивания изображения для п.з.
       private void PracticeImage_Dragging(object sender, DragEventArgs e)
        {
            // dropPosition - получает координаты на PracticeCanvas которые соответствуют 
            // положению указателя мыши
            Point dropPosition = e.GetPosition(PracticeCanvas);

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
            // задаем вариант студента равный вписанному
            studentsVariant = VariantBox.Text;

            // если ничего не вписали, оповещаем
            if (studentsVariant == "")
                MessageBox.Show("Пустой вариант!");
            else
            { 
                // если вписали, скрываем кнопку "подтвердить"
                // и панель с выбором варианта
                VariantPicker.Visibility = Visibility.Hidden;
                ConfirmVariantButton.Visibility = Visibility.Hidden;

                // и показываем кнопку начала практики
                StartPracticeButton.Visibility = Visibility.Visible;
            }
        }

        // Метод вызываемый нажатием по элементу меню "Практика1"
        private void Practice1Click(object sender, RoutedEventArgs e)
        {
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

                // иначе скрываем
                else
                    ContentPractice.Visibility = Visibility.Hidden;
            }

            // переменной activePractice задаем значение 1 
            // что бы понимать, что сейчас решается 1-ое п.з.

            activePractice = 1;
            
            // Если браузер с теорией отображен на экране, тогда
            // его следует скрыть

            if (TheoryView.Visibility == Visibility.Visible)
                TheoryView.Visibility = Visibility.Hidden;
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
            PracticeCanvas.AllowDrop = true;

            // показываем необходимые элементы

            PracticeImage.Source = new BitmapImage(new Uri("pack://application:,,,/Practice1." + studentsVariant + "Image.jpg"));

            PracticeImage.Visibility = Visibility.Visible;
            PracticeCanvas.Visibility = Visibility.Visible;

            // так как интресенее когда картинки разбросаны случайно
            // напишем рандомайзер положения картинок на полотне
            foreach (UIElement element in PracticeCanvas.Children)
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

        // Метод нажатия по кнопке "начать практику"
        private void StartPracticeClick(object sender, RoutedEventArgs e)
        {
            // если сейчас решается 1-ое п.з.
            if (activePractice == 1)
                ShowImagesPractice1();

            if (activePractice == 2)
            {

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
            if (362 <= Canvas.GetLeft(PracticeDraggableImage1) && (Canvas.GetLeft(PracticeDraggableImage1) <= 415)
                && (163 <= Canvas.GetTop(PracticeDraggableImage1)) && (Canvas.GetTop(PracticeDraggableImage1) <= 200))
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

            if (398 <= Canvas.GetLeft(PracticeDraggableImage3) && (Canvas.GetLeft(PracticeDraggableImage3) <= 420)
                && (65 <= Canvas.GetTop(PracticeDraggableImage3)) && (Canvas.GetTop(PracticeDraggableImage3) <= 120))
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
                && (180 <= Canvas.GetTop(PracticeDraggableImage9)) && (Canvas.GetTop(PracticeDraggableImage9) <= 200))
            {
                PracticeDraggableImage9.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00CA4E"));
                score++;
            }
            else
                PracticeDraggableImage9.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFF605C"));

            if (315 <= Canvas.GetLeft(PracticeDraggableImage10) && (Canvas.GetLeft(PracticeDraggableImage10) <= 360)
                && (5 <= Canvas.GetTop(PracticeDraggableImage10)) && (Canvas.GetTop(PracticeDraggableImage10) <= 60))
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
            if (163 <= Canvas.GetLeft(PracticeDraggableImage2) && (Canvas.GetLeft(PracticeDraggableImage2) <= 218)
                && (69 <= Canvas.GetTop(PracticeDraggableImage2)) && (Canvas.GetTop(PracticeDraggableImage2) <= 125))
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

            if (475 <= Canvas.GetLeft(PracticeDraggableImage5) && (Canvas.GetLeft(PracticeDraggableImage5) <= 521)
                && (74 <= Canvas.GetTop(PracticeDraggableImage5)) && (Canvas.GetTop(PracticeDraggableImage5) <= 132))
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
                PracticeCanvas.AllowDrop = false;

                // если после проверки все 10 картинок оказались на правильных местах
                // засчитываем п.з. 1
                if (CalculatePractice1ScoreVariant1() == 10)
                {
                    totalPracticeScore++;
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
                PracticeCanvas.AllowDrop = false;

                // если после проверки все 9 картинок оказались на правильных местах
                // засчитываем п.з. 1

                if (CalculatePractice1ScoreVariant2() == 9)
                {
                    totalPracticeScore++;
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
                PracticeCanvas.AllowDrop = false;

                // если после проверки все 6 картинок оказались на правильных местах
                // засчитываем п.з. 1
                if (CalculatePractice1ScoreVariant3() == 6)
                {
                    totalPracticeScore++;
                    StartNextPracticeButton.Visibility = Visibility.Visible;
                }
                else
                {
                    ReStartPracticeButton.Visibility = Visibility.Visible;
                }
            }
        }

        // метод при нажатии на кнопку "проверить"
        private void CheckPracticeClick(object sender, RoutedEventArgs e)
        {
            // скрываем кнопку "проверить" и показываем кнопку "заново"
            CheckPracticeButton.Visibility = Visibility.Hidden;
            
            // если решали 1-ую п.з.
            if (activePractice == 1)
                CalculatePractice1TotalScore();
            if (activePractice == 2)
            {

            }
        }

        private void ContinuePracticeClick(object sender, RoutedEventArgs e)
        {
            activePractice++;
            
            StartNextPracticeButton.Visibility = Visibility.Hidden;
            PracticeCanvas.Visibility = Visibility.Hidden;
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
        }

        // метод для запуска теста по кнопке "начать тест"

        private void TestClick(object sender, RoutedEventArgs e)
        {
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
            // правильных ответов нет
            score = 0;
            
            // начальный вопрос 0
            questionNum = 0;

            // индекс вопроса 0
            i = 0;
            
            // запуск теста

            StartGame();
        }

        // метод запуска теста

        private void StartGame()
        {
            // перемешиваем элементы массива (номера вопросов)

            var randomList = testquestionNumbers.OrderBy(a => Guid.NewGuid()).ToList();
            testquestionNumbers = randomList;
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

            scoreText.Content = "Текущий вопрос " + (questionNum + 1) + " из " + testquestionNumbers.Count;
            
            // следующий вопрос
            
            NextQuestion();
        }

        // метод для отображения вопросов

        private void NextQuestion()
        {
            // показываем на экране
            // txtQuestion - текст вопроса
            // qImage - картинка для вопроса

            txtQuestion.Visibility = Visibility.Visible;
            qImage.Visibility = Visibility.Hidden;
            
            // источник картнки - по необходимости
            qImage.Source = new BitmapImage(new Uri("pack://application:,,,/form1.png"));
            
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
                scoreText.Content = "Правильных ответов " + score + "/" + testquestionNumbers.Count + " " + (totalScore * 100) + "%";

                // скрыли все кнопки с вариантами ответов 
                
                ans1.Visibility = Visibility.Hidden;
                ans2.Visibility = Visibility.Hidden;
                ans3.Visibility = Visibility.Hidden;
                ans4.Visibility = Visibility.Hidden;
                
                // скрыли текст вопроса
                txtQuestion.Visibility = Visibility.Hidden;
                
                // вывели на экран всплывающее окно с результатом
                MessageBox.Show("Правильных ответов " + score + "/" + testquestionNumbers.Count + " " + (totalScore * 100) + "%");
            }

            // конструкция switch (i) содержит в себе всю информацию о вопросе

            switch (i)
            {
                // case [1-10] - случаи, или вопросы (case 1 - вопрос 1)

                case 1:

                    // показали картинку
                    qImage.Visibility = Visibility.Visible;
                    
                    // задали необходимый текст вопроса
                    txtQuestion.Text = "Вопрос 1";

                    // задали варианты ответов
                    ans1.Content = "Ответ 1";
                    ans2.Content = "Ответ 2 (Правильный)";
                    ans3.Content = "Ответ 3";
                    ans4.Content = "Ответ 4";
                    
                    // присвоили метку правильного кнопке
                    ans2.Tag = "1";

                    // источником изображения выбрали нужный

                    qImage.Source = new BitmapImage(new Uri("pack://application:,,,/form1.png"));

                    // конец случая
                    break;

                    // ниже все по той же схеме
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

        // метод который показывет 
        // панель меню после теста

        private void ShowAfterTest()
        {
            TheoryMenu.Visibility = Visibility.Visible;
            PracticeMenu.Visibility = Visibility.Visible;
            ContentTheory1.Visibility = Visibility.Visible;
        }

        // метод который скрывает 
        // панель меню пока идет тест

        private void HideWhileTest()
        {
            TheoryMenu.Visibility = Visibility.Hidden;
            PracticeMenu.Visibility = Visibility.Hidden;
            ContentTheory1.Visibility = Visibility.Hidden;
        }

        // метод который вызывается нажатием кноки "начать тест"

        private void ButtonStartTestClick(object sender, RoutedEventArgs e)
        {
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
            
            scoreText.Content = "Текущий вопрос " + 1 + " из " + testquestionNumbers.Count;
            
            // следующий вопрос

            NextQuestion();
        }

        // метод который вызывается нажатием кнопки "закончить тест"

        private void ButtonEndTestClick(object sender, RoutedEventArgs e)
        {
            // очищаем весь интерфейс
            txtQuestion.Text = "";
            qImage.Visibility = Visibility.Hidden;
            scoreText.Content = "";
            
            // изменяем видимость элементов теста

            ChangeVisibilityTest();
            
            // показываем меню 
            ShowAfterTest();
            
            // скрываем кнопку "закончить тест"
            ButtonEndTest.Visibility = Visibility.Hidden;
        }
    }
}
