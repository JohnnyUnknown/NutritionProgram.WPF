using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static NutritionProgram.WPF.ModelProgram;

namespace NutritionProgram.WPF
{
    class ViewModelProgram : INotifyPropertyChanged
    {
        // Метки выбора приема пищи
        //public bool flagBreakfast, flagBrunch, flagLunch, flagAfternoonTea, flagDinner, flagSupper;

        public ProductsTable productsTable;
        public Product selectedProduct;
        public DayNutrition dayNutrition;

        public ObservableCollection<Product> Products { get; set; }

        //private RelayCommand addCommand;
        //public RelayCommand AddCommand
        //{
        //    get
        //    {
        //        return addCommand ?? (addCommand = new RelayCommand(obj =>
        //        {
        //            if (flagBreakfast == true) dayNutrition.AddProdToMeal(dayNutrition.Breakfast, dayNutrition.weightProductsBreakfast);

        //        }));
        //    }
        //}


        public ViewModelProgram()
        {
            productsTable = new ProductsTable();
            TableProducts();
        }


        //Вывод списка продуктов из коллекции
        internal void TableProducts()
        {
            Products = new ObservableCollection<Product>();
            foreach (var key in productsTable.products)
            {
                Products.Add(key.Value);
            }
        }


        public Product SelectedProduct
        {
            get { return selectedProduct; }
            set
            {
                selectedProduct = value;
                OnPropertyChanged(SelectedProduct.ToString());
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

    }
}
