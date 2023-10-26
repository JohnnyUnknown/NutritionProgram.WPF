using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Console;

namespace NutritionProgram.WPF
{
    internal class ModelProgram
    {
        internal struct ProductStruct
        {
            public uint kcal;               // Количество ккал 
            public double protein;          // Грамм белка 
            public double fat;              // Грамм жиров 
            public double carbohydrates;    // Грамм углеводов 
        }

        internal struct NutrientsStruct
        {
            public string Meal { get; set; }             // Название приема пищи
            public uint kcal { get; set; }               // Количество ккал 
            public double protein { get; set; }          // Грамм белка 
            public double fat { get; set; }              // Грамм жиров 
            public double carbohydrates { get; set; }    // Грамм углеводов 

            public NutrientsStruct(string meal)
            {
                Meal = meal;
            }
        }

        internal class Product : INotifyPropertyChanged
        {
            internal string Name { get; set; }       // Название продукта
            internal ProductStruct prod;

            internal Product()
            {
                Name = "ПРОДУКТ";
            }

            internal Product(string name)
            {
                Name = name.ToUpper();
                Name = Name.Replace(' ', '_');
                SetNutrientsProd();
            }

            // Конструктор для формирования начальной таблицы продуктов
            internal Product(string name, double protein, double fat, double carbohydrates)
            {
                Name = name.ToUpper();
                Name = Name.Replace(' ', '_');
                prod.protein = protein;
                prod.fat = fat;
                prod.carbohydrates = carbohydrates;
                CountingCalories();
            }

            // Ввод данных о содержании нутриентов в продукте с проверками ввода
            private void SetNutrientsProd()
            {
                WriteLine($"Введите содержание нутриентов на 100 грамм {Name}");
                try
                {
                    Write("белки: ");
                    prod.protein = Convert.ToDouble(ReadLine());
                    Write("жиры: ");
                    prod.fat = Convert.ToDouble(ReadLine());
                    Write("углеводы: ");
                    prod.carbohydrates = Convert.ToDouble(ReadLine());
                    CountingCalories();     // Автоматический подсчет ккал 
                }
                catch (Exception e)
                {
                    // В случае некорректного ввода данных пользователем
                    Write($"\nОшибка ввода!!! {e.Message}\n\nВвести данные заново? 1 - да; 2 - нет: ");
                    try
                    {
                        int change = Convert.ToInt32(ReadLine());
                        if (change == 1) SetNutrientsProd();
                    }
                    catch
                    {
                        WriteLine($"\nЗначения нутриентов {Name} равны 0");
                        prod.kcal = 0;
                        prod.protein = 0;
                        prod.fat = 0;
                        prod.carbohydrates = 0;
                    }
                }
                WriteLine();
            }

            // Метод переименования продукта с проверкой ввода
            internal void RenameProd()
            {
                WriteLine($"Введите новое название продукта для {Name}");
                string? TempName = Name;
                TempName = ReadLine();
                if (TempName != null && TempName != "")
                {
                    TempName = TempName.ToUpper();
                    Name = TempName;
                }
                else
                {
                    WriteLine($"Ошибка ввода!!! Имя осталось прежним.\n");
                }
            }

            // Метод подсчета ккал
            internal void CountingCalories()
            {
                prod.kcal = (uint)(prod.protein * 4 + prod.fat * 9 + prod.carbohydrates * 4);
            }

            //Перегруженный метод вывода обЪекта класса
            public override string ToString()
            {
                return $"{Name}    ккал: {prod.kcal}  белки: {prod.protein} гр.  " +
                    $"жиры: {prod.fat} гр.  углеводы: {prod.carbohydrates} гр.\n";
            }

            public string ProductName
            {
                get
                {
                    return $"{Name}";
                }
            }

            public string ProductKcal
            {
                get
                {
                    return $"ккал: {prod.kcal}";
                }
            }

            public string ProductProtein
            {
                get
                {
                    return $"белки: {prod.protein} гр.";
                }
            }

            public string ProductFat
            {
                get
                {
                    return $"жиры: {prod.fat} гр.";
                }
            }

            public string ProductCarbo
            {
                get
                {
                    return $"углеводы: {prod.carbohydrates} гр.\n";
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged([CallerMemberName] string prop = "")
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(prop));
                }
            }
        }

        internal class ProductsTable : Product
        {
            internal SortedList<string, Product> products = new SortedList<string, Product>();
            static string filePath = "List.dat";
            protected internal static string filePath2 = "Nutrition.dat";

            protected internal ProductsTable()
            {
                FirstFilling();
            }

            // Первое создание списка продуктов
            private void FirstFilling()
            {
                // Считывание файла в строку и разбиение строки по словам
                string str = ReadFile();
                string[]? words = null;
                if (str != null && str != "")
                {
                    words = str.Split(new char[] { ' ', '~' }, StringSplitOptions.RemoveEmptyEntries);
                }
                // Проверка создан ли файл
                // Если строка == null или количество слов в строке не кратно 12 (неполная строка), 
                // то формируется список по умолчанию
                if (str == null || words == null || words.Length % 12 != 0)
                {
                    // Если файл не найден, то создается новый
                    // запись в файл
                    using (FileStream list = new FileStream(filePath,
                            FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        WriteLine("Составление списка продуктов по умолчанию...\n");
                        // Добавление базового списка продуктов
                        BaseProducts();

                        foreach (var k in products)   // KeyValuePair<string, Product> k
                        {
                            // преобразуем строку в байты
                            byte[] writeBytes = Encoding.Default.GetBytes('~' + k.Value.ToString());
                            // запись массива байтов в файл
                            list.Write(writeBytes, 0, writeBytes.Length);
                        }
                    }
                }
                // Если файл найден, то считываем информацию из него в SortedList<> products
                else
                {
                    ReadFileFillList();
                }
            }

            // Перезапись списка продуктов
            private void FillTable()
            {
                // запись в файл
                using (FileStream list = new FileStream(filePath,
                        FileMode.Truncate, FileAccess.Write))
                {
                    foreach (KeyValuePair<string, Product> k in products)
                    {
                        // преобразуем строку в байты
                        byte[] writeBytes = Encoding.Default.GetBytes('~' + k.Value.ToString());
                        // запись массива байтов в файл
                        list.Write(writeBytes, 0, writeBytes.Length);
                    }
                }
            }

            // Считывание списка продуктов из файла и вывод получившихся данных
            internal string ReadFile()
            {
                try
                {
                    // чтение из файла
                    using (FileStream list = File.OpenRead(filePath))
                    {
                        // выделяем массив для считывания данных из файла
                        byte[] readBytes = new byte[list.Length];
                        // считываем данные
                        list.Read(readBytes, 0, readBytes.Length);
                        // декодируем байты в строку и возвращаем
                        return Encoding.Default.GetString(readBytes);
                    }
                }
                // Если файла нет
                catch
                {
                    return null;
                }
            }

            // Считывание текста из файла и внесение данных в products для дальнейшей работы
            internal void ReadFileFillList()
            {
                string str;
                // чтение из файла
                using (FileStream list = File.OpenRead(filePath))
                {
                    // выделяем массив для считывания данных из файла
                    byte[] readBytes = new byte[list.Length];
                    // считываем данные
                    list.Read(readBytes, 0, readBytes.Length);
                    // декодируем байты в строку 
                    str = Encoding.Default.GetString(readBytes);
                }

                // Запись подстрок, разделенных разделителем '~' с удалением всех пустых строк
                string[] arrSubStr = str.Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries);

                // Объявление двумерного массива для хранения всех слов из списка продуктов
                int countI = 0, sizeWordsArr = 18;
                string[,] subArr = new string[arrSubStr.Length, sizeWordsArr];

                // Запись каждого слова каждой строки arrSubStr в массив строк
                foreach (string s in arrSubStr)
                {
                    // Сохранение слов строки во временный массив
                    string[] subArrTemp = s.Split(new char[] { ' ' });

                    // Запись слов из временного массива в соответствующую строку основного массива
                    for (int countJ = 0; countJ < subArrTemp.Length; countJ++)
                    {
                        if (subArrTemp.Length == sizeWordsArr)
                        {
                            subArr[countI, countJ] = subArrTemp[countJ];
                        }
                        else
                        {
                            break;
                        }
                    }
                    countI++;   // Плюс 1 к номеру строки основного массива
                }

                // Заполнение списка products из файла
                for (int i = 0; i < arrSubStr.Length; i++)
                {
                    for (int j = 0; j < sizeWordsArr; j++)
                    {
                        double x, y, z;
                        x = double.Parse(subArr[i, 8]);
                        y = double.Parse(subArr[i, 12]);
                        z = double.Parse(subArr[i, 16]);

                        // Проверка наличия в списке продукта по ключу
                        if (!products.ContainsKey(subArr[i, 0]))
                        {
                            // Если ключ не найден, то добавляем продукт
                            products.Add(subArr[i, 0], new Product(subArr[i, 0], x, y, z));
                        }
                    }
                }
            }

            // Удаление продукта из списка по имени
            protected internal void RemoveProduct()
            {
                Write("Введите название продукта для удаления: ");
                string? name = null;
                name = ReadLine();
                WriteLine();
                if (name != null && name != "")
                {
                    name = name.ToUpper();
                    name = name.Replace(' ', '_');
                    // Проверка наличия в списке продукта по ключу
                    if (products.ContainsKey(name))
                    {
                        // Если ключ найден, то удаляем продукт из списка
                        products.Remove(name);
                        // Перезаписываем данные в файле
                        FillTable();
                    }
                    else
                    {
                        WriteLine($"Продукт с названием {name} не найден.\n");
                        ReadLine();
                    }
                }
                else
                {
                    WriteLine($"Ошибка ввода названия продукта!!!\n");
                    ReadLine();
                }
            }

            // Удаление всего списка продуктов в файле
            protected internal void RemoveAllProducts()
            {
                // Проверка наличия в списке продуктов
                using (FileStream list = new FileStream(filePath,
                        FileMode.Truncate, FileAccess.Write))
                {
                    // Очистка старого и добавление базового списка продуктов
                    products.Clear();
                    BaseProducts();

                    foreach (var k in products)
                    {
                        // преобразуем строку в байты
                        byte[] writeBytes = Encoding.Default.GetBytes('~' + k.Value.ToString());
                        // запись массива байтов в файл
                        list.Write(writeBytes, 0, writeBytes.Length);
                    }
                }
            }

            // Метод поиска продукта
            protected internal Product PrintProd()
            {
                string? key;
                ForegroundColor = ConsoleColor.Green;
                PrintTable();
                ForegroundColor = ConsoleColor.DarkYellow;
                Write("\nВведите название продукта: ");
                key = ReadLine();
                if (key != null && key != "")
                {
                    // Приводим введенное название к верхнему регистру и заменяем пробелы для правильного поиска
                    key = key.ToUpper();
                    key = key.Replace(" ", "_");
                    // Проверка есть ли в списке продуктов элемент с таким ключом
                    if (products.ContainsKey(key))
                    {
                        // Если ключ элемента списка совпадает с искомым названием продукта,
                        // то выводим значение элемента в консоль
                        foreach (var k in products)
                        {
                            if (key == k.Key) { return k.Value; }
                        }
                    }
                    else
                    {
                        WriteLine($"\nПродукт с названием {key} не найден.\n");
                        ReadLine();
                    }
                }
                else
                {
                    WriteLine("\nОшибка ввода названия продукта!!!\n");
                    ReadLine();
                }
                ResetColor();
                return new Product();
            }

            // Метод добавления продуктов в список
            protected internal void AddProduct()
            {
                Write("Введите название нового продукта: ");
                string? key = ReadLine();
                if (key == null || key == "") { key = "ПРОДУКТ"; }
                // Приведение названия к виду, используемому программой
                key = key.ToUpper();
                key = key.Replace(" ", "_");
                // Если продукт по имени не найден, то добавляем новый
                if (!products.ContainsKey(key))
                {
                    // Добавляем новый продукт в список
                    products.Add(key, new Product(key));
                    // Перезаписываем файл
                    FillTable();
                }
                // Если продукт с таким именем уже существует
                else
                {
                    WriteLine($"Продукт с названием {key} уже существует.\n");
                    ReadLine();
                }
            }

            // Метод, добавляющий базовый список продуктов
            private void BaseProducts()
            {
                products.Add("АРАХИС", new Product("Арахис", 26.3, 45.2, 9.9));
                products.Add("ОКОРОЧОК", new Product("Окорочок", 21.3, 11, 0.1));
                products.Add("ГРУДКА_КУР", new Product("ГРУДКА_КУР", 21.6, 1.9, 0.4));
                products.Add("СУДАК_ЖАР", new Product("СУДАК_ЖАР", 23.3, 2.6, 0));
                products.Add("ЯЙЦО", new Product("Яйцо", 12.7, 11.5, 0.7));
                products.Add("БАТОН_НАРЕЗНОЙ", new Product("Батон_нарезной", 7.5, 2.9, 51.4));
                products.Add("ХЛЕБ_БЕЛЫЙ", new Product("Хлеб белый", 8.9, 3.3, 46.7));
                products.Add("АПЕЛЬСИН", new Product("Апельсин", 0.9, 0.2, 8.1));
                products.Add("АРБУЗ", new Product("Арбуз", 0.7, 0.1, 5.8));
                products.Add("СУП_ФРИКАД", new Product("СУП_ФРИКАД", 6.7, 1.7, 3.8));
                products.Add("БОРЩ", new Product("БОРЩ", 4.9, 1.4, 4.9));
                products.Add("ОВСЯНОБЛИН_ГОТ", new Product("ОВСЯНОБЛИН_ГОТ", 16.6, 13.4, 46.2));
                products.Add("ОМЛЕТ_ГОТ", new Product("ОМЛЕТ_ГОТ", 21, 14, 6));
                products.Add("ПЛОВ", new Product("ПЛОВ", 6.6, 1.2, 14.5));
                products.Add("КАРТОФЕЛЬ_ПЕЧ", new Product("КАРТОФЕЛЬ_ПЕЧ", 3.4, 1.7, 14.5));

            }

            //Вывод списка продуктов из коллекции в консоль
            protected internal void PrintTable()
            {
                foreach (var key in products)
                {
                    //Write(key.Value.ToString());
                    key.Value.ToString();
                }
                //WriteLine("\n");
            }

            // Метод составления блюда используя продукты из списка и добавление полученного блюда в библиотеку продуктов

            protected internal void DishCompilation()
            {
                List<Product> product = new List<Product>();
                List<double> weightProd = new List<double>();
                double weightDish = 0, weight = 0;
                int count = -1;
                bool stop = false, metka = false;
                Clear();
                // Обновление списка продуктов из файла
                ReadFileFillList();
                do
                {
                    WriteLine("\t\t\t\tРежим составления блюда:\n\n\tВыберите из библиотеки продукты " +
                        "и добавьте в ваше блюдо. Введите вес каждого \n\t\t\tингредиента и вес получившегося блюда.\n");
                    // Вывод библиотеки продуктов
                    ForegroundColor = ConsoleColor.Green;
                    PrintTable();
                    ForegroundColor = ConsoleColor.DarkYellow;

                    WriteLine("\n\t\t\tСписок выбранных для блюда продуктов:\n");
                    for (int i = 0; i < product.Count; i++)
                    {
                        Write($"{weightProd[i]} гр.\t{product[i]}");
                    }
                    Write("\nВведите название продукта: ");
                    string? key = ReadLine();
                    if (key == null || key == "") { key = "ПРОДУКТ"; }
                    // Приведение названия к виду, используемому программой
                    key = key.ToUpper();
                    key = key.Replace(" ", "_");
                    // Если продукт по имени найден, то добавляем новый ингредиент
                    foreach (var p in products)
                    {
                        if (p.Key == key)
                        {
                            // Создаем временный объект и копируем значение выбранного продукта
                            Product temp = new Product();
                            temp.Name = p.Key;
                            temp.prod.protein = p.Value.prod.protein;
                            temp.prod.fat = p.Value.prod.fat;
                            temp.prod.carbohydrates = p.Value.prod.carbohydrates;
                            // Добавляем новый продукт в блюдо
                            product.Add(temp);
                            count++;                // Увеличиваем счетчик добавленных продуктов
                            metka = true;           // Метка добаления продукта за итерацию
                                                    // Ввод веса продукта пользователем
                            try
                            {
                                Write("Введите вес добавленного продукта: ");
                                weight = Convert.ToInt32(ReadLine());
                                if (weight < 0) { throw new Exception("Вес продукта не может быть отрицательным.\n"); }
                            }
                            catch
                            {
                                WriteLine("Некорректный ввод.\n");
                                weight = 0;
                            }
                            // Добавление значение веса выбранного продукта
                            weightProd.Add(weight);
                        }
                        weight = 0;
                    }

                    // Корректировка значений нутриентов продукта по весу, если
                    // было добавлен хоть один ингредиент
                    if (product.Count >= 0 && metka != false)
                    {
                        product[count].prod.protein *= (weightProd[count] / 100);
                        product[count].prod.fat *= (weightProd[count] / 100);
                        product[count].prod.carbohydrates *= (weightProd[count] / 100);

                        product[count].prod.protein = Math.Round(product[count].prod.protein, 1);
                        product[count].prod.fat = Math.Round(product[count].prod.fat, 1);
                        product[count].prod.carbohydrates = Math.Round(product[count].prod.carbohydrates, 1);
                        product[count].CountingCalories();
                    }
                    // Сброс метки добавления продукта. Если не будет введено нового продукта, то не произойдет
                    // корректировки значений нутриентов продукта по весу
                    metka = false;

                    // Запрос пользователя на выход из режима добавления продуктов
                    Write("\nДобавить еще продукт? 1 - Да; 2 - Нет.\n\n");
                    int change = 0;
                    do
                    {
                        try
                        {
                            change = Convert.ToInt32(ReadLine());
                        }
                        catch
                        {
                            WriteLine("Некорректный ввод.\n");
                            change = 0;
                        }
                    } while (change < 1 || change > 2);
                    stop = (change == 1) ? false : true;

                    Clear();
                } while (!stop);

                if (product.Count >= 0)
                {
                    // Ввод веса приготовленного блюда
                    do
                    {
                        try
                        {
                            Write("Введите вес приготовленного продукта: ");
                            weightDish = Convert.ToInt32(ReadLine());
                            if (weightDish < 0) { throw new Exception("Вес продукта не может быть отрицательным.\n"); }
                        }
                        catch
                        {
                            WriteLine("Некорректный ввод.\n");
                            weightDish = 0;
                        }
                    } while (weightDish == 0);

                    string name;
                    double sumProtein = 0;
                    double sumFat = 0;
                    double sumCarbohydrates = 0;
                    for (int i = 0; i < weightProd.Count; i++)
                    {
                        sumProtein += product[i].prod.protein;
                        sumFat += product[i].prod.fat;
                        sumCarbohydrates += product[i].prod.carbohydrates;
                    }
                    sumProtein /= (weightDish / 100);
                    sumFat /= (weightDish / 100);
                    sumCarbohydrates /= (weightDish / 100);

                    sumProtein = Math.Round(sumProtein, 1);
                    sumFat = Math.Round(sumFat, 1);
                    sumCarbohydrates = Math.Round(sumCarbohydrates, 1);

                    Write("Введите название нового блюда: ");
                    name = ReadLine();
                    products.Add(name, new Product(name, sumProtein, sumFat, sumCarbohydrates));
                    // Запись в файл
                    FillTable();
                }
            }
        }

        internal class DayNutrition : ProductsTable
        {
            internal NutrientsStruct[] NutrientsPerDay;  // Потребление КБЖУ за день
            internal Product[] Breakfast;          // Потребление КБЖУ за завтрак
            internal Product[] Brunch;             // Потребление КБЖУ за второй завтрак
            internal Product[] Lunch;              // Потребление КБЖУ за обед
            internal Product[] AfternoonTea;       // Потребление КБЖУ за полдник
            internal Product[] Dinner;             // Потребление КБЖУ за ужин
            internal Product[] Supper;             // Потребление КБЖУ за поздний ужин
            internal const int size = 20;            // Размер таблиц приемов пищи
            // Массивы со значениями весов продуктов для каждого приема пищи
            internal double[] weightProductsBreakfast, weightProductsBrunch, weightProductsLunch,
                weightProductsAfternoonTea, weightProductsDinner, weightProductsSupper;
            DateTime date;

            protected internal DayNutrition() : base()
            {
                NutrientsPerDay = new NutrientsStruct[7]
                    {
                new NutrientsStruct("Завтрак\t\t"), new NutrientsStruct("Второй завтрак\t"),
                new NutrientsStruct("Обед\t\t"), new NutrientsStruct("Полдник\t\t"), new NutrientsStruct("Ужин\t\t"),
                new NutrientsStruct("Поздний ужин\t"), new NutrientsStruct("Всего за день\t")
                    };
                // Выделение памяти под массивы продуктов
                Breakfast = new Product[size];
                Brunch = new Product[size];
                Lunch = new Product[size];
                AfternoonTea = new Product[size];
                Dinner = new Product[size];
                Supper = new Product[size];
                // Присвоение элементам массивов значений продуктов по умолчанию
                for (int i = 0; i < size; i++)
                {
                    Breakfast[i] = new Product();
                    Brunch[i] = new Product();
                    Lunch[i] = new Product();
                    AfternoonTea[i] = new Product();
                    Dinner[i] = new Product();
                    Supper[i] = new Product();
                }
                // Выделение памяти под массивы значений веса продуктов
                weightProductsBreakfast = new double[size];
                weightProductsBrunch = new double[size];
                weightProductsLunch = new double[size];
                weightProductsAfternoonTea = new double[size];
                weightProductsDinner = new double[size];
                weightProductsSupper = new double[size];

                // Формирование даты запуска программы
                date = new DateTime();
                date = DateTime.Today;

                FirstFilling();
            }

            // Создание файла со списками съеденных продуктов с разделением по приемам пищи
            private void FirstFilling()
            {
                // Проверка создан ли файл
                if (ReadFile2() == null)
                {
                    // Если файл не найден, то создается новый
                    using (FileStream list = new FileStream(filePath2,
                        FileMode.Create, FileAccess.Write))
                    {
                        // Объявление счетчика для перебора массива весов продуктов
                        int count = 0;
                        // Перебор всех элементов приема пищи
                        foreach (var k in Breakfast)
                        {
                            // преобразуем строку значений нутриентов и веса продукта в байты
                            byte[] writeBytes = Encoding.Default.GetBytes('~' + k.ToString() + " "
                                + weightProductsBreakfast[count]);
                            // Увеличиваем значение счетчика для массива весов продуктов
                            count++;
                            // запись массива байтов в файл
                            list.Write(writeBytes, 0, writeBytes.Length);
                        }

                        count = 0;
                        foreach (var k in Brunch)
                        {
                            byte[] writeBytes = Encoding.Default.GetBytes('~' + k.ToString() + " "
                                + weightProductsBrunch[count]);
                            count++;
                            list.Write(writeBytes, 0, writeBytes.Length);
                        }

                        count = 0;
                        foreach (var k in Lunch)
                        {
                            byte[] writeBytes = Encoding.Default.GetBytes('~' + k.ToString() + " "
                                + weightProductsLunch[count]);
                            count++;
                            list.Write(writeBytes, 0, writeBytes.Length);
                        }

                        count = 0;
                        foreach (var k in AfternoonTea)
                        {
                            byte[] writeBytes = Encoding.Default.GetBytes('~' + k.ToString() + " "
                                + weightProductsAfternoonTea[count]);
                            count++;
                            list.Write(writeBytes, 0, writeBytes.Length);
                        }

                        count = 0;
                        foreach (var k in Dinner)
                        {
                            byte[] writeBytes = Encoding.Default.GetBytes('~' + k.ToString() + " "
                                + weightProductsDinner[count]);
                            count++;
                            list.Write(writeBytes, 0, writeBytes.Length);
                        }

                        count = 0;
                        foreach (var k in Supper)
                        {
                            byte[] writeBytes = Encoding.Default.GetBytes('~' + k.ToString() + " "
                                + weightProductsSupper[count]);
                            count++;
                            list.Write(writeBytes, 0, writeBytes.Length);
                        }
                    }
                }
                // Если файл найден, то считываем информацию из него
                else
                {
                    ReadFile2FillArrays();
                }
            }

            // Запись рациона питания в файл
            private void FillTable()
            {
                // запись в файл
                using (FileStream list = new FileStream(filePath2,
                        FileMode.Truncate, FileAccess.Write))
                {
                    // Объявление счетчика для перебора массива весов продуктов
                    int count = 0;
                    // Перебор всех элементов приема пищи
                    foreach (var k in Breakfast)
                    {
                        // преобразуем строку значений нутриентов и веса продукта в байты
                        byte[] writeBytes = Encoding.Default.GetBytes('~' + k.ToString() + " "
                            + weightProductsBreakfast[count]);
                        // Увеличиваем значение счетчика для массива весов продуктов
                        count++;
                        // запись массива байтов в файл
                        list.Write(writeBytes, 0, writeBytes.Length);
                    }

                    count = 0;
                    foreach (var k in Brunch)
                    {
                        byte[] writeBytes = Encoding.Default.GetBytes('~' + k.ToString() + " "
                            + weightProductsBrunch[count]);
                        count++;
                        list.Write(writeBytes, 0, writeBytes.Length);
                    }

                    count = 0;
                    foreach (var k in Lunch)
                    {
                        byte[] writeBytes = Encoding.Default.GetBytes('~' + k.ToString() + " "
                            + weightProductsLunch[count]);
                        count++;
                        list.Write(writeBytes, 0, writeBytes.Length);
                    }

                    count = 0;
                    foreach (var k in AfternoonTea)
                    {
                        byte[] writeBytes = Encoding.Default.GetBytes('~' + k.ToString() + " "
                            + weightProductsAfternoonTea[count]);
                        count++;
                        list.Write(writeBytes, 0, writeBytes.Length);
                    }

                    count = 0;
                    foreach (var k in Dinner)
                    {
                        byte[] writeBytes = Encoding.Default.GetBytes('~' + k.ToString() + " "
                            + weightProductsDinner[count]);
                        count++;
                        list.Write(writeBytes, 0, writeBytes.Length);
                    }

                    count = 0;
                    foreach (var k in Supper)
                    {
                        byte[] writeBytes = Encoding.Default.GetBytes('~' + k.ToString() + " "
                            + weightProductsSupper[count]);
                        count++;
                        list.Write(writeBytes, 0, writeBytes.Length);
                    }
                }
            }

            // Считывание списка съеденных продуктов из файла и вывод получившихся данных
            private string ReadFile2()
            {
                try
                {
                    // чтение из файла
                    using (FileStream list = File.OpenRead(filePath2))
                    {
                        // выделяем массив для считывания данных из файла
                        byte[] readBytes = new byte[list.Length];
                        // считываем данные
                        list.Read(readBytes, 0, readBytes.Length);
                        // декодируем байты в строку и возвращаем
                        return Encoding.Default.GetString(readBytes);
                    }
                }
                // Если файла нет
                catch
                {
                    return null;
                }
            }

            // Считывание текста из файла и внесение данных в массивы приемов пищи и весов продуктов
            // для дальнейшей работы
            private void ReadFile2FillArrays()
            {
                // чтение и запись содержимого файла в строку
                string str = ReadFile2();

                // Запись подстрок, разделенных разделителем '~' с удалением всех пустых строк
                string[] arrSubStr = str.Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries);

                // Объявление двумерного массива для хранения всех слов из списка рациона
                int countI = 0, sizeWordsArr = 19;
                string[,] subArr = new string[arrSubStr.Length, sizeWordsArr];

                // Запись каждого слова каждой строки arrSubStr в массив строк
                foreach (string s in arrSubStr)
                {
                    // Сохранение слов строки во временный массив
                    string[] subArrTemp = s.Split(new char[] { ' ' });

                    // Запись слов из временного массива в соответствующую строку основного массива
                    for (int countJ = 0; countJ < subArrTemp.Length; countJ++)
                    {
                        if (subArrTemp.Length == sizeWordsArr)
                        {
                            subArr[countI, countJ] = subArrTemp[countJ];
                        }
                        else
                        {
                            break;
                        }
                    }
                    countI++;   // Плюс 1 к номеру строки основного массива
                }

                // Заполнение рациона из файла
                for (int i = 0; i < arrSubStr.Length; i++)
                {
                    double x = 0, y = 0, z = 0, k = 0;
                    for (int j = 0; j < sizeWordsArr; j++)
                    {
                        x = double.Parse(subArr[i, 8]);
                        y = double.Parse(subArr[i, 12]);
                        z = double.Parse(subArr[i, 16]);
                        k = double.Parse(subArr[i, 18]);

                    }

                    if (i < size)
                    {
                        Breakfast[i] = new Product(subArr[i, 0], x, y, z);
                        weightProductsBreakfast[i] = k;
                    }
                    else if (i < size * 2)
                    {
                        Brunch[i - size] = new Product(subArr[i, 0], x, y, z);
                        weightProductsBrunch[i - size] = k;
                    }
                    else if (i < size * 3)
                    {
                        Lunch[i - size * 2] = new Product(subArr[i, 0], x, y, z);
                        weightProductsLunch[i - size * 2] = k;
                    }
                    else if (i < size * 4)
                    {
                        AfternoonTea[i - size * 3] = new Product(subArr[i, 0], x, y, z);
                        weightProductsAfternoonTea[i - size * 3] = k;
                    }
                    else if (i < size * 5)
                    {
                        Dinner[i - size * 4] = new Product(subArr[i, 0], x, y, z);
                        weightProductsDinner[i - size * 4] = k;
                    }
                    else if (i < size * 6)
                    {
                        Supper[i - size * 5] = new Product(subArr[i, 0], x, y, z);
                        weightProductsSupper[i - size * 5] = k;
                    }
                }
                FormingMainTable();
            }

            // Метод вывода в консоль общей таблицы питания
            protected internal void PrintMainTable()
            {
                WriteLine("============================================================================================");
                WriteLine($"\t\t\t\tРацион за день. {date.ToLongDateString()}" +
                    "\n--------------------------------------------------------------------------------------------");
                for (int i = 0; i < NutrientsPerDay.Length; i++)
                {
                    if (i == NutrientsPerDay.Length - 1)
                    {
                        WriteLine("--------------------------------------------------------------------------------------------");
                    }
                    PrintMeal(NutrientsPerDay[i]);
                }
                WriteLine("============================================================================================");
                WriteLine("\t\t\t\t\tЗавтрак\n--------------------------------------------------------------------------------------------");
                PrintSubTable(Breakfast, weightProductsBreakfast);
                WriteLine("============================================================================================");
                WriteLine("\t\t\t\t\tВторой завтрак\n--------------------------------------------------------------------------------------------");
                PrintSubTable(Brunch, weightProductsBrunch);
                WriteLine("============================================================================================");
                WriteLine("\t\t\t\t\tОбед\n--------------------------------------------------------------------------------------------");
                PrintSubTable(Lunch, weightProductsLunch);
                WriteLine("============================================================================================");
                WriteLine("\t\t\t\t\tПолдник\n--------------------------------------------------------------------------------------------");
                PrintSubTable(AfternoonTea, weightProductsAfternoonTea);
                WriteLine("============================================================================================");
                WriteLine("\t\t\t\t\tУжин\n--------------------------------------------------------------------------------------------");
                PrintSubTable(Dinner, weightProductsDinner);
                WriteLine("============================================================================================");
                WriteLine("\t\t\t\t\tПоздний ужин\n--------------------------------------------------------------------------------------------");
                PrintSubTable(Supper, weightProductsSupper);
                WriteLine("============================================================================================\n");
            }

            // Метод вывода в консоль таблиц приемов пищи 
            private void PrintSubTable(Product[] ns, double[] arr)
            {
                int count = 0;
                for (int i = 0; i < ns.Length; i++)
                {
                    if (ns[i].Name != "ПРОДУКТ")
                    {
                        PrintMeal(ns[i], arr[i]);
                        count++;
                    }
                }
                if (count == 0) WriteLine("\t\t\t\tВы совсем ничего не съели.");
                WriteLine();
            }

            // Добавление продукта в прием пищи
            internal void AddProdToMeal(Product[] ns, double[] arr)
            {
                // Поиск свободного места по списку приемов пищи
                for (int i = 0; i < ns.Length; i++)
                {
                    // Свободное место найдено
                    if (ns[i].Name == "ПРОДУКТ")
                    {
                        // Поиск продукта в списке сохраненных продуктов
                        ns[i] = PrintProd();
                        // Если искомый продукт найден, то запрашиваем вес продукта для добавления
                        if (ns[i].Name != "ПРОДУКТ")
                        {
                            arr[i] = CountingNutrients(ns[i]);
                            {
                                // Если введено некорректное значение веса, то продукт удаляется из списка
                                if (arr[i] == 0) ns[i] = new Product();
                            }
                        }
                        break;
                    }
                }
                // Пересчет таблицы потребления за день
                //FormingMainTable();
                //FillTable();
            }

            // Метод подсчета ккал и содержание БЖУ в зависимости от веса
            private double CountingNutrients(Product ns)
            {
                Write("Введите вес продукта (в граммах): ");
                double weight;
                try
                {
                    weight = Convert.ToDouble(ReadLine());
                    if (weight < 0) throw new Exception("\nЗначение веса не может быть отрицательным!\n");
                }
                catch (Exception e)
                {
                    // Если введено некорректное значение, то вес продукта равен 0
                    WriteLine(e.Message);
                    weight = 0;
                }
                // Умножаем кол-во нутриентов на введенный вес
                ns.prod.protein *= (weight / 100);
                ns.prod.fat *= (weight / 100);
                ns.prod.carbohydrates *= (weight / 100);
                // Округление получившихся значений до двух цифр после запятой
                ns.prod.protein = Math.Round(ns.prod.protein, 1);
                ns.prod.fat = Math.Round(ns.prod.fat, 1);
                ns.prod.carbohydrates = Math.Round(ns.prod.carbohydrates, 1);
                // Вычисление ккал
                ns.prod.kcal = (uint)(ns.prod.protein * 4 + ns.prod.fat * 9 + ns.prod.carbohydrates * 4);
                return weight;
            }

            // Удаление продукта из списка приема пищи
            protected internal void DeleteProd(Product[] ns, double[] arr)
            {
                Write("\nВведите название продукта для удаления: ");
                string? name = null;
                bool metka = false;
                name = ReadLine();
                WriteLine();
                if (name != null && name != "")
                {
                    name = name.ToUpper();
                    name = name.Replace(' ', '_');
                    // Проверка наличия в списке продукта по ключу
                    for (int i = 0; i < ns.Length; i++)
                    {
                        if (ns[i].Name == name)
                        {
                            // Если ключ найден, то удаляем продукт из списка и обнуляем его вес
                            ns[i] = new Product();
                            arr[i] = 0;
                            metka = true;
                            // Перезаписываем данные в файле
                            FillTable();
                        }
                        else if (i == ns.Length - 1 && metka == false)
                        {
                            WriteLine($"Продукт с названием {name} не найден.\n");
                            ReadLine();
                        }
                    }
                }
                else
                {
                    WriteLine("Ошибка ввода названия продукта!!!\n");
                    ReadLine();
                }
                // Пересчет таблицы потребления за день
                FormingMainTable();
            }

            // Очистка приема пищи
            protected internal void ClearMeal(Product[] ns, double[] arr)
            {
                // Пролистываем весь массив продуктов
                for (int i = 0; i < ns.Length; i++)
                {
                    // Присваиваем каждому элементу значение по умолчанию (пустое)
                    ns[i] = new Product();
                    arr[i] = 0;
                }
                // Пересчет таблицы потребления за день
                FormingMainTable();
                FillTable();
            }

            // Очистка всех таблиц приемов пищи
            protected internal void ClearAllDay()
            {
                WriteLine("\n\t\t\tПроизведена полная очистка рациона.\n");
                ClearMeal(Breakfast, weightProductsBreakfast);
                ClearMeal(Brunch, weightProductsBrunch);
                ClearMeal(Lunch, weightProductsLunch);
                ClearMeal(AfternoonTea, weightProductsAfternoonTea);
                ClearMeal(Dinner, weightProductsDinner);
                ClearMeal(Supper, weightProductsSupper);
                ReadLine();
            }

            // Подсчет суммарного потребления нутриентов за день
            private void FormingMainTable()
            {
                // Обнуление сумм нутриентов за все приемы пищи за весь день
                for (int i = 0; i < NutrientsPerDay.Length; i++)
                {
                    NutrientsPerDay[i].kcal = 0;
                    NutrientsPerDay[i].fat = 0;
                    NutrientsPerDay[i].protein = 0;
                    NutrientsPerDay[i].carbohydrates = 0;
                }
                // Подсчет сумм нутриентов за каждый прием пищи
                for (int j = 0; j < size; j++)
                {
                    NutrientsPerDay[0].kcal += Breakfast[j].prod.kcal;
                    NutrientsPerDay[0].protein += Breakfast[j].prod.protein;
                    NutrientsPerDay[0].fat += Breakfast[j].prod.fat;
                    NutrientsPerDay[0].carbohydrates += Breakfast[j].prod.carbohydrates;

                    NutrientsPerDay[1].kcal += Brunch[j].prod.kcal;
                    NutrientsPerDay[1].protein += Brunch[j].prod.protein;
                    NutrientsPerDay[1].fat += Brunch[j].prod.fat;
                    NutrientsPerDay[1].carbohydrates += Brunch[j].prod.carbohydrates;

                    NutrientsPerDay[2].kcal += Lunch[j].prod.kcal;
                    NutrientsPerDay[2].protein += Lunch[j].prod.protein;
                    NutrientsPerDay[2].fat += Lunch[j].prod.fat;
                    NutrientsPerDay[2].carbohydrates += Lunch[j].prod.carbohydrates;

                    NutrientsPerDay[3].kcal += AfternoonTea[j].prod.kcal;
                    NutrientsPerDay[3].protein += AfternoonTea[j].prod.protein;
                    NutrientsPerDay[3].fat += AfternoonTea[j].prod.fat;
                    NutrientsPerDay[3].carbohydrates += AfternoonTea[j].prod.carbohydrates;

                    NutrientsPerDay[4].kcal += Dinner[j].prod.kcal;
                    NutrientsPerDay[4].protein += Dinner[j].prod.protein;
                    NutrientsPerDay[4].fat += Dinner[j].prod.fat;
                    NutrientsPerDay[4].carbohydrates += Dinner[j].prod.carbohydrates;

                    NutrientsPerDay[5].kcal += Supper[j].prod.kcal;
                    NutrientsPerDay[5].protein += Supper[j].prod.protein;
                    NutrientsPerDay[5].fat += Supper[j].prod.fat;
                    NutrientsPerDay[5].carbohydrates += Supper[j].prod.carbohydrates;
                }
                // Подсчет суммы нутриентов за все приемы пищи за весь день
                for (int i = 0; i < NutrientsPerDay.Length - 1; i++)
                {
                    NutrientsPerDay[6].kcal += NutrientsPerDay[i].kcal;
                    NutrientsPerDay[6].fat += NutrientsPerDay[i].fat;
                    NutrientsPerDay[6].protein += NutrientsPerDay[i].protein;
                    NutrientsPerDay[6].carbohydrates += NutrientsPerDay[i].carbohydrates;
                }

                // Округление значений всех сумм
                for (int i = 0; i < NutrientsPerDay.Length; i++)
                {
                    NutrientsPerDay[i].fat = Math.Round(NutrientsPerDay[i].fat, 1);
                    NutrientsPerDay[i].protein = Math.Round(NutrientsPerDay[i].protein, 1);
                    NutrientsPerDay[i].carbohydrates = Math.Round(NutrientsPerDay[i].carbohydrates, 1);
                }
            }

            // Распечатка продукта в таблице приема пищи
            private void PrintMeal(Product ns, double weight)
            {
                WriteLine($"{ns.Name}  вес: {weight} гр.  {ns.prod.kcal} ккал. белок: {ns.prod.protein} гр. " +
                    $"жиры: {ns.prod.fat} гр. углеводы: {ns.prod.carbohydrates} гр.");
            }

            // Распечатка нутриентов за весь прием пищи в главной таблице
            private void PrintMeal(NutrientsStruct ns)
            {
                WriteLine($"{ns.Meal}   {ns.kcal} ккал.  белок: {ns.protein} гр.  " +
                    $"жиры: {ns.fat} гр.  углеводы: {ns.carbohydrates} гр.");
            }

            // Выбор приема пищи
            protected internal Product[] BackMeal(out double[] arr)
            {
                int change;
                do
                {
                    try
                    {
                        WriteLine("\t\t\t\t1 - Завтрак;\n\t\t\t\t2 - Второй завтрак;\n\t\t\t\t3 - Обед;\n\t\t\t\t4 - Полдник;" +
                            "\n\t\t\t\t5 - Ужин;\n\t\t\t\t6 - Поздний ужин.\n");
                        change = Convert.ToInt32(ReadLine());
                        if (change < 1 || change > 6) { throw new Exception("Значение должно быть от 1 до 6\n"); }
                    }
                    catch (Exception ex)
                    {
                        WriteLine($"Error: {ex.Message}\n");
                        change = 0;
                    }
                } while (change == 0);

                if (change == 1)
                {
                    arr = weightProductsBreakfast;
                    return Breakfast;
                }
                else if (change == 2)
                {
                    arr = weightProductsBrunch;
                    return Brunch;
                }
                else if (change == 3)
                {
                    arr = weightProductsLunch;
                    return Lunch;
                }
                else if (change == 4)
                {
                    arr = weightProductsAfternoonTea;
                    return AfternoonTea;
                }
                else if (change == 5)
                {
                    arr = weightProductsDinner;
                    return Dinner;
                }
                else
                {
                    arr = weightProductsSupper;
                    return Supper;
                }
            }
        }

        internal class User
        {
            public string? Name { get; set; }
            public string? Gender { get; set; }
            public double Activity { get; set; }
            public int Weight { get; set; }
            public double Height { get; set; }
            public int Age { get; set; }
            public Target TargetOfDiet;
            public enum Target { Похудение = -1, Удержание_Веса = 0, Набор_Веса = 1 }
            public string? BodyMassIndex { get; set; }
            private static string filePath = "User.dat";

            public User()
            {
                FillUsersParametrs();
            }

            private void FillUsersParametrs()
            {
                byte[] writeBytes;
                if (ReadFile() == null)
                {
                    EnterName();
                    EnterGender();
                    EnterActivity();
                    EnterWeight();
                    EnterHeight();
                    EnterAge();
                    EnterTarget();
                    // Если файл не найден, то создается новый, запись в файл
                    using (FileStream list = new FileStream(filePath,
                            FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        writeBytes = Encoding.Default.GetBytes(this.ToString());
                        list.Write(writeBytes, 0, writeBytes.Length);
                    }
                }
                // Если файл найден, то считываем информацию из него
                else
                {
                    ReadFileFillList();
                }
            }

            // Перезапись данных пользователя
            public void FillUser()
            {
                // запись в файл
                using (FileStream list = new FileStream(filePath,
                        FileMode.Truncate, FileAccess.Write))
                {
                    // преобразуем строку в байты
                    byte[] writeBytes = Encoding.Default.GetBytes('~' + this.ToString());
                    // запись массива байтов в файл
                    list.Write(writeBytes, 0, writeBytes.Length);
                }
            }

            // Считывание списка продуктов из файла и вывод получившихся данных
            private string ReadFile()
            {
                try
                {
                    // чтение из файла
                    using (FileStream list = File.OpenRead(filePath))
                    {
                        // выделяем массив для считывания данных из файла
                        byte[] readBytes = new byte[list.Length];
                        // считываем данные
                        list.Read(readBytes, 0, readBytes.Length);
                        // декодируем байты в строку и возвращаем
                        return Encoding.Default.GetString(readBytes);
                    }
                }
                // Если файла нет
                catch
                {
                    return null;
                }
            }

            // Считывание текста из файла и внесение данных пользователя 
            private void ReadFileFillList()
            {
                string str;
                // чтение из файла
                using (FileStream list = File.OpenRead(filePath))
                {
                    // выделяем массив для считывания данных из файла
                    byte[] readBytes = new byte[list.Length];
                    // считываем данные
                    list.Read(readBytes, 0, readBytes.Length);
                    // декодируем байты в строку 
                    str = Encoding.Default.GetString(readBytes);
                }

                // Запись подстрок, разделенных разделителем '~' с удалением всех пустых строк
                string[] arrSubStr = str.Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries);

                // Заполнение списка products из файла
                for (int i = 0; i < arrSubStr.Length; i++)
                {
                    Name = arrSubStr[0];
                    Gender = arrSubStr[1];
                    Activity = double.Parse(arrSubStr[2]);
                    Weight = Int32.Parse(arrSubStr[3]);
                    Height = double.Parse(arrSubStr[4]);
                    Age = Int32.Parse(arrSubStr[5]);
                    string t = arrSubStr[6];
                    TargetOfDiet = (t == "Похудение") ? Target.Похудение :
                        (t == "Удержание_Веса") ? Target.Удержание_Веса : Target.Набор_Веса;
                    СalculationBMI();
                }
            }

            // Ввод имени пользователя
            public void EnterName()
            {
                do
                {
                    Write("Введите имя для вашего профиля: ");
                    Name = ReadLine();
                    Clear();
                    if (Name == null || Name.Length == 0) WriteLine("\nНедопустимое имя!!!\n");
                } while (Name == null || Name.Length == 0);
            }

            // Выбор пола пользователя
            private void EnterGender()
            {
                int g = 0;
                do
                {
                    try
                    {
                        WriteLine("Введите ваш пол: 1 - Муж; 2 - Жен.");
                        g = Convert.ToInt32(ReadLine());
                    }
                    catch
                    {
                        WriteLine("\nНекорректный ввод.\n");
                    }
                } while (g != 1 && g != 2);

                Clear();
                Gender = g == 1 ? "Муж" : "Жен";
            }

            // Выбор уровня активности пользователя
            public void EnterActivity()
            {
                int a = 0;
                do
                {
                    try
                    {
                        WriteLine("Выберите свой уровень активности в повседневной жизни.");
                        WriteLine("1 - Сидячий образ жизни;");
                        WriteLine("2 - Умеренная активность, легкие физические нагрузки, тренировки 1-3 раза/нед;");
                        WriteLine("3 - Средняя активность, занятия 3-5 раз/нед;");
                        WriteLine("4 - Высокая активность, интенсивные нагрузки, тренировки 6-7 раз/нед;");
                        WriteLine("5 - Спортсмены с ежедневными тренировками и люди, выполняющие \n" +
                            "\tтяжелую физическую работу 6-7 раз/нед;");
                        a = Convert.ToInt32(ReadLine());
                    }
                    catch
                    {
                        WriteLine("\nНекорректный ввод.\n");
                    }
                } while (a != 1 && a != 2 && a != 3 && a != 4 && a != 5);

                Clear();
                Activity = (a == 1) ? 1.2 :
                    (a == 2) ? 1.375 :
                    (a == 3) ? 1.55 :
                    (a == 4) ? 1.725 : 1.9;
            }

            // Ввод веса пользователя
            public void EnterWeight()
            {
                int w = 0;
                do
                {
                    try
                    {
                        Write("Введите ваш вес (кг): ");
                        w = Convert.ToInt32(ReadLine());
                    }
                    catch
                    {
                        WriteLine("\nНекорректный ввод.\n");
                    }
                } while (w < 30 || w > 300);

                Clear();
                Weight = w;
                СalculationBMI();       // Пересчет индекса массы тела
            }

            // Ввод роста пользователя
            public void EnterHeight()
            {
                int h = 0;
                do
                {
                    try
                    {
                        Write("Введите ваш рост (см): ");
                        h = Convert.ToInt32(ReadLine());
                    }
                    catch
                    {
                        WriteLine("\nНекорректный ввод.\n");
                    }
                } while (h < 100 || h > 250);

                Clear();
                Height = h;
            }

            // Ввод возраста пользователя
            public void EnterAge()
            {
                int a = 0;
                do
                {
                    try
                    {
                        Write("Введите ваш возраст: ");
                        a = Convert.ToInt32(ReadLine());
                    }
                    catch
                    {
                        WriteLine("\nНекорректный ввод.\n");
                    }
                    if (a < 14 && a >= 0) WriteLine("Диету в таком возрасте может назначить только врач.\n");
                } while (a < 14 || a > 100);

                Clear();
                Age = a;
            }

            //Выбор цели диеты
            public void EnterTarget()
            {
                int t = 0;
                do
                {
                    try
                    {
                        WriteLine("Выберите цель вашей диеты.");
                        WriteLine("1 - Снижение веса;");
                        WriteLine("2 - Удержание веса;");
                        WriteLine("3 - Набор веса;");
                        t = Convert.ToInt32(ReadLine());
                    }
                    catch
                    {
                        WriteLine("\nНекорректный ввод.\n");
                    }
                } while (t != 1 && t != 2 && t != 3);

                Clear();
                TargetOfDiet = (t == 1) ? Target.Похудение :
                    (t == 2) ? Target.Удержание_Веса : Target.Набор_Веса;
            }

            // Вычисление индекса массы тела
            private void СalculationBMI()
            {
                double BMI = Weight / Math.Pow(Height / 100, 2);
                BodyMassIndex = (BMI < 16) ? "Выраженный дефицит массы тела" :
                    (BMI <= 18.5) ? "Недостаточная масса тела" :
                    (BMI <= 25) ? "Нормальная масса тела" :
                    (BMI <= 30) ? "Избыточная масса тела (предожирение)" :
                    (BMI <= 35) ? "Ожирение I степени" :
                    (BMI <= 40) ? "Ожирение II степени" : "Ожирение III степени";
            }

            public void Print()
            {
                WriteLine($"Имя пользователя: {Name}; \nПол: {Gender}; Коэффициент активности {Activity}; " +
                    $"Вес {Weight}; Рост: {Height}; Возраст: {Age}; \nЦель диеты: {TargetOfDiet}; " +
                    $"Индекс массы тела: {BodyMassIndex}\n");
            }

            //Перегруженный метод вывода обЪекта класса
            public override string ToString()
            {
                return $"~{Name}~{Gender}~{Activity}~{Weight}~" +
                    $"{Height}~{Age}~{TargetOfDiet}~{BodyMassIndex}";
            }
        }

        internal class NutritionProgram : DayNutrition
        {
            User user;
            int baseKcal;
            double baseProtein;
            double baseFat;
            double baseCarbohydrates;

            public NutritionProgram() : base()
            {
                user = new User();
                BaseConsumption();
                DisplayProgram();
                MainMethod();
            }

            // Вычисление базового потребления исходя из показателей пользователя
            private void BaseConsumption()
            {
                // Коэффициент зависящий от пола пользователя
                int coefGender = (user.Gender == "Муж") ? 5 : -161;
                // Базовое количество ккал необходимое в сутки исходя из показателей пользователя
                baseKcal = (int)((10 * user.Weight + 6.25 * user.Height - 5 * user.Age + coefGender) * user.Activity);
                double coefTarget = 1;
                coefTarget = (user.TargetOfDiet == User.Target.Похудение) ? 0.8 :
                    (user.TargetOfDiet == User.Target.Удержание_Веса) ? 1 : 1.2;
                // Корректировка количества ккал исходя из целей диеты
                baseKcal = (int)(baseKcal * coefTarget);
                // Необходимое количество белка, жиров и углеводов
                baseProtein = baseKcal * 0.3 / 4;
                baseFat = baseKcal * 0.3 / 9;
                baseCarbohydrates = baseKcal * 0.4 / 4;

                baseProtein = Math.Round(baseProtein, 1);
                baseFat = Math.Round(baseFat, 1);
                baseCarbohydrates = Math.Round(baseCarbohydrates, 1);
            }

            // Главное меню программы
            private void MainMethod()
            {
                int change;
                ForegroundColor = ConsoleColor.DarkYellow;
                do
                {
                    try
                    {
                        WriteLine("\t\t\t\tВаши действия:\n\t\t\t1 - Изменить данные пользователя;\n\t\t\t2 - Внесение изменений " +
                            "в таблицу рациона;\n\t\t\t3 - Внести изменения в список продуктов;\n\t\t\t4 - Выход из программы.\n");
                        change = Convert.ToInt32(ReadLine());
                        if (change < 1 || change > 4) { throw new Exception("Значение должно быть от 1 до 4\n"); }
                    }
                    catch (Exception ex)
                    {
                        WriteLine($"Error: {ex.Message}\n");
                        change = 0;
                    }
                } while (change == 0);

                Clear();
                if (change == 1)
                {
                    SubMainUserData();
                }
                else if (change == 2)
                {
                    SubMainUserNutrition();
                }
                else if (change == 3)
                {
                    SubMainNutritionTable();
                }
                else
                {
                    Environment.Exit(0);
                }
                Clear();
                DisplayProgram();
                ForegroundColor = ConsoleColor.DarkGreen;
            }

            // Меню изменения данных пользователя
            private void SubMainUserData()
            {
                int change;
                // Очистка консоли и вывод данных пользователя
                Clear();
                ForegroundColor = ConsoleColor.DarkGreen;
                user.Print();
                ForegroundColor = ConsoleColor.DarkYellow;
                do
                {
                    try
                    {
                        WriteLine("\t\t\t\tИзменение данных пользователя:\n\t\t\t1 - Изменить имя;\n\t\t\t2 - Изменить вес" +
                            ";\n\t\t\t3 - Изменить режим активности;\n\t\t\t4 - Изменить цель диеты;\n\t\t\t5 - Назад.\n");
                        change = Convert.ToInt32(ReadLine());
                        if (change < 1 || change > 5) { throw new Exception("Значение должно быть от 1 до 5\n"); }
                    }
                    catch (Exception ex)
                    {
                        WriteLine($"Error: {ex.Message}\n");
                        change = 0;
                    }
                } while (change == 0);

                if (change == 1)
                {
                    ChangeName();       // Метод изменения имени пользователя
                }
                else if (change == 2)
                {
                    ChangeWeight();     // Метод изменения веса пользователя
                }
                else if (change == 3)
                {
                    ChangeActivity();   // Метод изменения уровня активности пользователя
                }
                else if (change == 4)
                {
                    ChangeTarget();     // Метод изменения цели диеты пользователя
                }
                else if (change == 5)
                {
                    Clear();                // Очистка консоли и возврат в главное меню
                    DisplayProgram();
                    MainMethod();
                }
                SubMainUserData();
            }

            // Меню изменения рациона пользователя
            private void SubMainUserNutrition()
            {
                int change;
                // Очистка консоли и вывод таблицы питания
                Clear();
                ForegroundColor = ConsoleColor.Blue;
                PrintMainTable();
                ForegroundColor = ConsoleColor.DarkYellow;
                do
                {
                    try
                    {
                        WriteLine("\t\t\t\tВнесение изменений в таблицу рациона:\n\t\t\t1 - Добавить продукт в прием пищи;\n\t\t\t" +
                            "2 - Удаление продукта из списка приема пищи;\n\t\t\t3 - Очистка приема пищи;\n\t\t\t" +
                            "4 - Очистка всего рациона;\n\t\t\t5 - Назад.\n");
                        change = Convert.ToInt32(ReadLine());
                        if (change < 1 || change > 5) { throw new Exception("Значение должно быть от 1 до 5\n"); }
                    }
                    catch (Exception ex)
                    {
                        WriteLine($"Error: {ex.Message}\n");
                        change = 0;
                    }
                } while (change == 0);

                if (change == 1)
                {
                    AddProdToMeal(BackMeal(out double[] arr), arr);       // Метод добавления продукта в рацион
                }
                else if (change == 2)
                {
                    ForegroundColor = ConsoleColor.Blue;
                    PrintMainTable();
                    WriteLine();
                    ForegroundColor = ConsoleColor.DarkYellow;
                    DeleteProd(BackMeal(out double[] arr), arr);          // Метод удаления продукта из рациона
                }
                else if (change == 3)
                {
                    ClearMeal(BackMeal(out double[] arr), arr);           // Метод очистки приема пищи
                }
                else if (change == 4)
                {
                    ClearAllDay();                                        // Метод очистки всего рациона
                }
                else if (change == 5)
                {
                    Clear();                                              // Очистка консоли и возврат в главное меню
                    DisplayProgram();
                    MainMethod();
                }
                SubMainUserNutrition();
            }

            // Меню изменения списка продуктов
            private void SubMainNutritionTable()
            {
                int change;
                // Очистка консоли и вывод списка сохраненных продуктов
                Clear();
                ForegroundColor = ConsoleColor.Green;
                WriteLine("\t\t\t\tСписок продуктов:\n");
                Write(ReadFile());
                ForegroundColor = ConsoleColor.DarkYellow;
                do
                {
                    try
                    {
                        WriteLine("\n\t\t\t\tВнесение изменений в список продуктов:\n\t\t\t1 - Добавить продукт в список;\n" +
                            "\t\t\t2 - Удалить продукт из списка;\n\t\t\t3 - Сброс библиотеки продуктов до первоначальной;" +
                            "\n\t\t\t4 - Режим составления блюда;\n\t\t\t5 - Назад.\n");
                        change = Convert.ToInt32(ReadLine());
                        if (change < 1 || change > 5) { throw new Exception("Значение должно быть от 1 до 5\n"); }
                    }
                    catch (Exception ex)
                    {
                        WriteLine($"Error: {ex.Message}\n");
                        change = 0;
                    }
                } while (change == 0);

                if (change == 1)
                {
                    AddProduct();               // Метод добавления продукта в список
                }
                else if (change == 2)
                {
                    RemoveProduct();            // Метод удаления продукта из списка
                }
                else if (change == 3)
                {
                    RemoveAllProducts();        // Сброс списка продуктов до первоначального
                }
                else if (change == 4)
                {
                    DishCompilation();          // Метод сборки блюда из продуктов и запись в библиотеку
                }
                else if (change == 5)
                {
                    Clear();
                    DisplayProgram();
                    MainMethod();                // Очистка консоли и возврат в главное меню
                }
                SubMainNutritionTable();
            }

            // Изменение имени пользователя
            private void ChangeName()
            {
                user.EnterName();
                user.FillUser();
            }

            // Изменение веса пользователя
            private void ChangeWeight()
            {
                user.EnterWeight();
                BaseConsumption();
                user.FillUser();
            }

            // Изменение режима активности пользователя
            private void ChangeActivity()
            {
                user.EnterActivity();
                BaseConsumption();
                user.FillUser();
            }

            // Изменение цели диеты
            private void ChangeTarget()
            {
                user.EnterTarget();
                BaseConsumption();
                user.FillUser();
            }

            // Вывод в консоль значений базового потребления в день
            private void DisplayBaseNutrition()
            {
                WriteLine($"\t\t\tНеобходимое кол-во элементов в день:\n\t\t{baseKcal} ккал. " +
                    $"Белок: {baseProtein} гр. Жиры: {baseFat} гр. Углеводы: {baseCarbohydrates} гр.\n");
            }

            // Общий вывод программы
            private void DisplayProgram()
            {
                ForegroundColor = ConsoleColor.DarkGreen;
                WriteLine("\t\t\t\tNutrition Program\n");
                user.Print();
                ForegroundColor = ConsoleColor.Red;
                DisplayBaseNutrition();
                ForegroundColor = ConsoleColor.Blue;
                PrintMainTable();
                WriteLine();
            }
        }

    }
}
