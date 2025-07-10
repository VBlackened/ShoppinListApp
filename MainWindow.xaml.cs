using Newtonsoft.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;

namespace Shoppy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadSavedTiles();
        }

        //Creating product tile
        public class ProductTile
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public required string Name { get; set; }
            public required string Symbol { get; set; }
        }

        private Button CreateTile(ProductTile product)
        {
            var button = new Button
            {
                Width = 70,
                Height = 70,
                Background = Brushes.White,
                BorderThickness = new Thickness(2),
                Padding = new Thickness(0),
                Tag = product,
                Cursor = Cursors.Hand,
                Content = new StackPanel
                {
                    Children =
                    {
                        new TextBlock
                        {
                            Text = product.Symbol,
                            FontSize = 24,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            TextAlignment = TextAlignment.Center,
                            Margin = new Thickness(0, 2, 0, 2)
                        },
                        new TextBlock
                        {
                            Text = product.Name,
                            FontSize = 12,
                            FontWeight = FontWeights.Bold,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            TextAlignment = TextAlignment.Center,
                            TextWrapping = TextWrapping.Wrap,
                            Margin = new Thickness(2, 0, 2, 2)
                        }
                    }
                }
            };

            button.Click += (s, e) =>
            {
                if (s is Button b)
                {
                    if (b.BorderBrush == Brushes.Goldenrod)
                    {
                        b.Background = Brushes.White;
                        b.BorderBrush = Brushes.Black;
                    }
                    else
                    {
                        b.Background = Brushes.LightGoldenrodYellow;
                        b.BorderBrush = Brushes.Goldenrod;
                    }
                }
            };

            return button;
        }

        //Add button
        private void btn_NewProduct_Click(object sender, RoutedEventArgs e)
        {
            string name = tbx_ProductName.Text;
            string symbol = cmb_ProductSymbol.Text;

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Enter product name", "No name",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                return;
            }

            var product = new ProductTile { Name = name, Symbol = symbol };
            savedTiles.Add(product);

            var tile = CreateTile(product);
            TilePanel.Children.Add(tile);


            SaveTiles();
        }

        //Save and load
        private List<ProductTile> savedTiles = new();
        private string tileDataPath = "products.json";

        private void SaveTiles()
        {
            string json = JsonConvert.SerializeObject(savedTiles, Formatting.Indented);
            File.WriteAllText(tileDataPath, json);
        }

        private void LoadSavedTiles()
        {
            if (File.Exists(tileDataPath))
            {
                string json = File.ReadAllText(tileDataPath);
                savedTiles = JsonConvert.DeserializeObject<List<ProductTile>>(json) ?? new List<ProductTile>();
                foreach (var product in savedTiles)
                {
                    var tile = CreateTile(product);
                    TilePanel.Children.Add(tile);
                }
            }
        }

        // Delete button
        private void btn_DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var toRemove = new List<UIElement>();



            foreach (Button tile in TilePanel.Children)
            {
                if (tile.Background == Brushes.LightGoldenrodYellow)
                {
                    var data = tile.Tag as ProductTile;
                    if (data != null)
                    {
                        savedTiles.RemoveAll(p => p.Id == data.Id);
                    }
                    toRemove.Add(tile);
                }
            }

            if (toRemove.Count == 0)
            {
                MessageBox.Show("No product selected for removal.", "No selection", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            foreach (var tile in toRemove)
                TilePanel.Children.Remove(tile);

            SaveTiles();
        }
    }
}