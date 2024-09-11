using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace FirstLab
{
    public class TaxPayer
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public decimal AnnualIncome { get; set; }
    }

    public partial class MainWindow : Window
    {
        public List<TaxPayer> currentTaxPayers = new List<TaxPayer>
        {
            new TaxPayer { LastName = "Иванов", FirstName = "Иван", AnnualIncome = 18000 },
            new TaxPayer { LastName = "Петров", FirstName = "Петр", AnnualIncome = 25000 },
            new TaxPayer { LastName = "Сидоров", FirstName = "Алексей", AnnualIncome = 45000 }
        };
        static readonly string currentDirectoryPath = Directory.GetCurrentDirectory();
        public string filePath = currentDirectoryPath + "/data.csv";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void FindXkButton_Click(object sender, RoutedEventArgs e)
        {
            double pointX1Value = Convert.ToDouble(pointX1.Text);
            double pointX2Value = Convert.ToDouble(pointX2.Text);
            double result = (pointX1Value + pointX2Value) / 2;
            MessageBox.Show($"x_k = {result}");
        }

        private void GenerateDataButton_Click(object sender, RoutedEventArgs e)
        {
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("LastName,FirstName,AnnualIncome"); // Заголовки колонок
                foreach (var taxpayer in currentTaxPayers)
                {
                    writer.WriteLine($"{taxpayer.LastName},{taxpayer.FirstName},{taxpayer.AnnualIncome}");
                }
            }
        }

        private void CalculateTaxButton_Click(object sender, RoutedEventArgs e) {
            string result = "";
            double tax = 0, annualIncome;
            var taxPayersFromCsv = new List<TaxPayer>();
            using (var reader = new StreamReader(filePath))
            {
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    annualIncome = Convert.ToDouble(values[2]);

                    if (annualIncome < 20000) {
                        tax = annualIncome * 0.12;
                    } else if (annualIncome > 40000) {
                        tax = annualIncome * 0.35;
                    } else {
                        tax = annualIncome * 0.2;
                    }

                    result += $"{values[0]} - {tax} руб.\n";
                }
            }
            MessageBox.Show(result);
        }
    }
}
