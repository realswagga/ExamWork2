using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using DataBaseLayer.Classes;
using DataBaseLayer.Models;
using DataBaseLayer.Services;

namespace Fragrant_World.Pages
{
    /// <summary>
    /// Interaction logic for StorePage.xaml
    /// </summary>
    public partial class StorePage : Page
    {
        private readonly ExamProductService _productService = new();

        internal List<ExamProduct> Products { get; set; }

        private double priceFrom;
        private double priceTo;

        //Переменные для плавной прокрутки
        private double targetVerticalOffset;
        private bool isUserScrolling = false;
        private const double smoothingFactor = 0.09;

        public StorePage()
        {
            InitializeComponent();
            UserNameLabel.Content = $"{UserDataBus.Surname} {UserDataBus.Name} {UserDataBus.Patronymic}";
            Style = (Style)FindResource(typeof(Page));

            PriceComboBox.SelectedIndex = 0;
            ManufacturerComboBox.SelectedIndex = 0;
            PriceRangeSliderFrom.Value = 100;
            PriceRangeSliderTo.Value = 15000;


            //Подписки для плавной прокрутки
            scrollViewer.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;
            scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            scrollViewer.PreviewMouseDown += ScrollViewer_PreviewMouseDown;
            scrollViewer.PreviewMouseUp += ScrollViewer_PreviewMouseUp;

            //Подписки обновления фильтров
            SearchTextBox.TextChanged += Filters_Changed;
            PriceComboBox.SelectionChanged += Filters_Changed;
            ManufacturerComboBox.SelectionChanged += Filters_Changed;
            PriceRangeSliderFrom.ValueChanged += Filters_Changed;
            PriceRangeSliderTo.ValueChanged += Filters_Changed;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await UpdateProductsAsync();
            await FillManufacturerComboBoxAsync();
        }
        private async void Filters_Changed(object sender, RoutedEventArgs e)
        {
            MainStackPanel.Children.Clear();
            await UpdateProductsAsync();
        }

        private void ExitImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService.Navigate(new AuthPage());
        }
        public async Task FillManufacturerComboBoxAsync()
        {
            try
            {
                var manufacturers = await _productService.GetManufacturersAsync();

                if (manufacturers != null)
                    foreach (var manufacturer in manufacturers)
                        ManufacturerComboBox.Items.Add(manufacturer);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex}");
            }
        }

        private void PriceRangeSlider_ValueChangedFrom(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue != e.OldValue)
            {
                priceFrom = e.NewValue;
                PriceFromTextBlock.Text = $"Цена от: {priceFrom:N0} руб";

                // Устанавливаем минимальное значение для второго слайдера
                PriceRangeSliderTo.Minimum = priceFrom;
            }
        }

        // Обработчик изменения значения второго слайдера (Цена до)
        private void PriceRangeSlider_ValueChangedTo(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue != e.OldValue)
            {
                priceTo = e.NewValue;
                PriceToTextBlock.Text = $"Цена до: {priceTo:N0} руб";

                // Устанавливаем максимальное значение для первого слайдера
                PriceRangeSliderFrom.Maximum = priceTo;
            }
        }

        //Обновляет товары
        private async Task UpdateProductsAsync()
        {
            Products = await _productService.GetProductsDataAsync(SearchTextBox.Text,
                PriceComboBox.SelectedIndex,
                Convert.ToDecimal(PriceRangeSliderFrom.Value),
                Convert.ToDecimal(PriceRangeSliderTo.Value),
                ManufacturerComboBox.SelectedValue.ToString());
            foreach (ExamProduct product in Products)
            {
                CreateProductsList(product);
            }
            ProductsQuantityLabel.Content = $"{Products.Count} из {_productService.GetProductsCount()}";
        }

        //Создание контекстного меню
        private static ContextMenu GenerateContextMenu(ExamProduct product)
        {
            MenuItem AddItemMenu = new MenuItem { Header = "Добавить в корзину", Tag = product };
            MenuItem EditItem = new MenuItem { Header = "Изменить товар", Tag = product };
            MenuItem DeleteItem = new MenuItem { Header = "Удалить товар", Tag = product };
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Items.Add(AddItemMenu);
            if (UserDataBus.RoleId == 1 || UserDataBus.RoleId == 3)
            {
                contextMenu.Items.Add(EditItem);
                contextMenu.Items.Add(DeleteItem);
            }
            AddItemMenu.Click += AddItemMenu_Click;
            return contextMenu;
        }

        //Метод для добавления товаров в корзину
        public static void AddItemMenu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.Tag is Product product)
            {
                /*Random random = new();
                Order order = new Order()
                {
                    OrderID = DataAccessLayer.GetLastOrderID(),
                    UserID = UserDataBus.UserId,
                    OrderStatus = "Новый",
                    OrderDate = DateTime.Now,
                    OrderDeliveryDate = DateTime.Now.AddDays(random.Next(2, 10)),
                    OrderPickupPoint = DataAccessLayer.GetPickupPoint(),
                    OrderPickupCode = DataAccessLayer.GetPickupCode(),
                };
                DataAccessLayer.UpdateExamOrder(order);
                DataAccessLayer.UpdateExamOrderProduct(DataAccessLayer.GetLastOrderID(), product.Article, 1);*/
            }
        }

        //Создание карточек товаров
        private void CreateProductsList(ExamProduct product)
        {
            Border productBorder = new()
            {
                Height = 130,
                Background = new SolidColorBrush(Color.FromRgb(245, 209, 174)),
                Margin = new Thickness(10),
            };

            productBorder.ContextMenu = GenerateContextMenu(product);

            //Подписка события для подсветки товара при наведении мышки
            productBorder.MouseEnter += ProductBorder_MouseEnter;
            productBorder.MouseLeave += ProductBorder_MouseLeave;

            //Создание главного StackPanel
            StackPanel externalStackPanel = new()
            {
                Margin = new Thickness(10),
            };

            Grid productGrid = new Grid();

            StackPanel innerStackPanel = new();

            //Создание метки для названия товара
            Label nameProductLabel = new()
            {
                Content = product.ProductName,
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 22,
                Effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 10,
                    ShadowDepth = 2,
                    Opacity = 0.1
                }
            };

            //Создание метки для производителя товара
            Label manufacturerProductLabel = new()
            {
                Content = product.ProductManufacturer,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, -7.5, 0, 0),
                FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromRgb(125, 125, 125))
            };

            //Создание метки для описания товара
            Label descriptionLabel = new()
            {
                Content = product.ProductDescription,
                HorizontalAlignment = HorizontalAlignment.Left,
                FontSize = 13,
            };

            //Создание контейнера для цены товара
            StackPanel priceStackPanel = new()
            {
                Orientation = Orientation.Horizontal,
            };

            //Создание метки для цены товара
            Label priceLabel = new()
            {
                Content = "Цена:",
                FontSize = 20
            };

            //Вычисление цены после скидки
            TextBlock discounCostTextBlock = new()
            {
                Text = product.ProductDiscountAmount.HasValue ?
                Math.Round(product.ProductCost - (product.ProductCost * product.ProductDiscountAmount.Value * 0.01m), 2)
                .ToString() : product.ProductCost.ToString(),
                Height = 20,
                FontSize = 18
            };

            //Создание места для картинки товара
            Border imageBorder = new()
            {
                Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)),
                Margin = new Thickness(0, 0, 10, 0),
                Height = 100,
                Width = 100,
                Effect = new DropShadowEffect
                {
                    Color = product.ProductDiscountAmount < 15 ? Color.FromRgb(245, 209, 174) : Color.FromRgb(127, 255, 0),
                    BlurRadius = product.ProductDiscountAmount < 15 ? 20 : 100,
                    ShadowDepth = 0,
                    Opacity = 0.7
                }
            };

            //Картинка товара
            Image image = new()
            {
                Source = new BitmapImage(new Uri("/Images/product.png", UriKind.Relative))
            };

            //Размещение всех элементов по контейнерам
            MainStackPanel.Children.Add(productBorder);
            productBorder.Child = externalStackPanel;

            externalStackPanel.Children.Add(productGrid);
            productGrid.ColumnDefinitions.Add(new ColumnDefinition());
            productGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            productGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            productGrid.Children.Add(innerStackPanel);
            innerStackPanel.Children.Add(nameProductLabel);
            innerStackPanel.Children.Add(manufacturerProductLabel);
            innerStackPanel.Children.Add(descriptionLabel);
            innerStackPanel.Children.Add(priceStackPanel);

            priceStackPanel.Children.Add(priceLabel);
            priceStackPanel.Children.Add(discounCostTextBlock);

            //Размещение скидки и старой цены
            if (product.ProductDiscountAmount > 0)
            {
                TextBlock priceTextBlock = new()
                {
                    Text = product.ProductCost.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    FontSize = 16,
                    Margin = new Thickness(3, 0, 0, 0),
                    TextDecorations = TextDecorations.Strikethrough,
                    Foreground = new SolidColorBrush(Color.FromRgb(115, 115, 115))
                };

                Label discountLabel = new()
                {
                    Content = $"-{product.ProductDiscountAmount}%",
                    FontSize = product.ProductDiscountAmount < 15 ? 30 : 36,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(0, 0, 10, 5),
                    Effect = new DropShadowEffect
                    {
                        Color = product.ProductDiscountAmount < 15 ? Color.FromRgb(245, 209, 174) : Color.FromRgb(127, 255, 0),
                        BlurRadius = product.ProductDiscountAmount < 15 ? 20 : 40,
                        ShadowDepth = 0,
                        Opacity = product.ProductDiscountAmount < 15 ? 0.6 : 1
                    }
                };

                priceStackPanel.Children.Add(priceTextBlock);
                productGrid.Children.Add(discountLabel);
                Grid.SetColumn(discountLabel, 1);
            }

            //Размещение плашки с изображением товара
            imageBorder.Child = image;
            productGrid.Children.Add(imageBorder);

            //Указание столбцов
            Grid.SetColumn(innerStackPanel, 0);
            Grid.SetColumn(imageBorder, 2);
        }

        private void ProductBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            if (border != null)
            {
                // Анимация изменения цвета фона
                ColorAnimation colorAnimation = new ColorAnimation
                {
                    To = Color.FromRgb(255, 204, 154),
                    Duration = new Duration(TimeSpan.FromSeconds(0.2))
                };
                border.Background.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);

                // Анимация добавления тени
                DropShadowEffect shadowEffect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 30,
                    ShadowDepth = 2,
                    Opacity = 0
                };
                border.Effect = shadowEffect;

                DoubleAnimation shadowAnimation = new DoubleAnimation
                {
                    To = 0.2,
                    Duration = new Duration(TimeSpan.FromSeconds(0.2))
                };
                shadowEffect.BeginAnimation(DropShadowEffect.OpacityProperty, shadowAnimation);
            }
        }

        private void ProductBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            if (border != null)
            {
                // Анимация изменения цвета фона
                ColorAnimation colorAnimation = new ColorAnimation
                {
                    To = Color.FromRgb(245, 209, 174),
                    Duration = new Duration(TimeSpan.FromSeconds(0.33))
                };
                border.Background.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);

                // Анимация удаления тени
                if (border.Effect is DropShadowEffect shadowEffect)
                {
                    DoubleAnimation shadowAnimation = new DoubleAnimation
                    {
                        To = 0,
                        Duration = new Duration(TimeSpan.FromSeconds(0.33))
                    };
                    shadowEffect.BeginAnimation(DropShadowEffect.OpacityProperty, shadowAnimation);
                }
            }
        }

        #region Плавная прокрутка
        //Множество событий для плавной прокрутки содержимого на экране
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            targetVerticalOffset = scrollViewer.VerticalOffset - e.Delta;
            if (targetVerticalOffset < 0)
            {
                targetVerticalOffset = 0;
            }
            else if (targetVerticalOffset > scrollViewer.ScrollableHeight)
            {
                targetVerticalOffset = scrollViewer.ScrollableHeight;
            }
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (isUserScrolling)
            {
                targetVerticalOffset = scrollViewer.VerticalOffset;
            }
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            double currentVerticalOffset = scrollViewer.VerticalOffset;
            double delta = targetVerticalOffset - currentVerticalOffset;
            if (Math.Abs(delta) > 0.1)
            {
                double newVerticalOffset = currentVerticalOffset + delta * smoothingFactor;
                scrollViewer.ScrollToVerticalOffset(newVerticalOffset);
            }
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            isUserScrolling = true;
        }

        private void ScrollViewer_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            isUserScrolling = false;
        }
        #endregion

        private void CartImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService.Navigate(new CheckoutPage());
        }
    }
}
