using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static NutritionProgram.WPF.ModelProgram;

namespace NutritionProgram.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Метки выбора приема пищи
        internal bool flagBreakfast = false, flagBrunch = false, flagLunch = false, flagAfternoonTea = false,
            flagDinner = false, flagSupper = false;

        internal DayNutrition dayNutrition;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModelProgram();
            dayNutrition = new DayNutrition();
        }



        // Изменение цвета фона при фокусе на названии приема пищи
        private void ChangeMealColor(TextBox textBox)
        {
            // Сброс всех меток выбора приема пищи
            flagBreakfast = false;
            flagBrunch = false;
            flagLunch = false;
            flagAfternoonTea = false;
            flagDinner = false;
            flagSupper = false;

            // Выбор нового приема пищи
            if (textBox.Name == "changeBreakfast")
                flagBreakfast = true;
            else if (textBox.Name == "changeBrunch")
                flagBrunch = true;
            else if (textBox.Name == "changeLunch")
                flagLunch = true;
            else if (textBox.Name == "changeAfternoonTea")
                flagAfternoonTea = true;
            else if (textBox.Name == "changeDinner")
                flagDinner = true;
            else if (textBox.Name == "changeSupper")
                flagSupper = true;

            // Установка цвета фона по умолчанию
            changeBreakfast.Background = new System.Windows.Media.SolidColorBrush(Color.FromArgb(a: 0xFF, r: 0x0b, g: 0x0b, b: 0x0b));
            changeBrunch.Background = new System.Windows.Media.SolidColorBrush(Color.FromArgb(a: 0xFF, r: 0x0b, g: 0x0b, b: 0x0b));
            changeLunch.Background = new System.Windows.Media.SolidColorBrush(Color.FromArgb(a: 0xFF, r: 0x0b, g: 0x0b, b: 0x0b));
            changeAfternoonTea.Background = new System.Windows.Media.SolidColorBrush(Color.FromArgb(a: 0xFF, r: 0x0b, g: 0x0b, b: 0x0b));
            changeDinner.Background = new System.Windows.Media.SolidColorBrush(Color.FromArgb(a: 0xFF, r: 0x0b, g: 0x0b, b: 0x0b));
            changeSupper.Background = new System.Windows.Media.SolidColorBrush(Color.FromArgb(a: 0xFF, r: 0x0b, g: 0x0b, b: 0x0b));
            // Установка цвета границ по умолчанию
            changeBreakfast.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
            changeBrunch.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
            changeLunch.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
            changeAfternoonTea.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
            changeDinner.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
            changeSupper.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);

            // Установка цвета фона и границ при выборе
            var backgroundBrush = new RadialGradientBrush
            {
                GradientOrigin = new Point(0.5, 0.5)
            };
            backgroundBrush.GradientStops.Add(new GradientStop(Colors.Orange, offset: 0.0));
            backgroundBrush.GradientStops.Add(new GradientStop(Color.FromArgb(a: 0xFF, r: 0x0b, g: 0x0b, b: 0x0b), offset: 0.8));

            textBox.Background = backgroundBrush;
            textBox.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Orange);
        }

        private void changeBreakfast_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangeMealColor(changeBreakfast);
        }

        private void changeBrunch_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangeMealColor(changeBrunch);
        }

        private void changeLunch_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangeMealColor(changeLunch);
        }

        private void changeAfternoonTea_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangeMealColor(changeAfternoonTea);
        }

        private void changeDinner_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangeMealColor(changeDinner);
        }

        private void changeSupper_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangeMealColor(changeSupper);
        }


        // Выбор продукта в библиотеке и добавление его в выбранный прием пищи
        private void TextBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AddProductToMeal();
        }

        // Добавление продукта в выбранный прием пищи
        internal void AddProductToMeal()
        {
            if (flagBreakfast == true) dayNutrition.AddProdToMeal(dayNutrition.Breakfast, dayNutrition.weightProductsBreakfast);
            else if (flagBrunch == true) dayNutrition.AddProdToMeal(dayNutrition.Brunch, dayNutrition.weightProductsBrunch);
            else if (flagLunch == true) dayNutrition.AddProdToMeal(dayNutrition.Lunch, dayNutrition.weightProductsLunch);
            else if (flagAfternoonTea == true) dayNutrition.AddProdToMeal(dayNutrition.AfternoonTea, dayNutrition.weightProductsAfternoonTea);
            else if (flagDinner == true) dayNutrition.AddProdToMeal(dayNutrition.Dinner, dayNutrition.weightProductsDinner);
            else if (flagSupper == true) dayNutrition.AddProdToMeal(dayNutrition.Supper, dayNutrition.weightProductsSupper);
            else MessageBox.Show("Не выбран прием пищи");
        }


        internal void DisplayAllMeal()
        {

        }
    }
}
