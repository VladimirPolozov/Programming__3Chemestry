using org.mariuszgromada.math.mxparser;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;



namespace FirstLab
{
    public class FunctionModel
    {
        //  Метод для поиска минимума методом дихотомии
        public static double FindPointOfIntersectionDihotomyMethod(string functionExpression, double parametrA, double parametrB, double epsilon)
        {
            Function expression = new Function("f(x) = " + functionExpression);
            double parametrAValue = SolveFunc(expression, parametrA.ToString());
            double parametrBValue = SolveFunc(expression, parametrB.ToString());

            while (parametrAValue - parametrBValue < epsilon) {
                double middleofSegment = (parametrA - parametrB) / 2;

                if (parametrAValue * parametrBValue < 0)
                {
                    parametrA = middleofSegment;
                } else {
                    parametrB = middleofSegment;
                }

                parametrAValue = SolveFunc(expression, parametrA.ToString());
                parametrBValue = SolveFunc(expression, parametrB.ToString());
            }

            return (parametrA + parametrB) / 2;
        }

        public static double SolveFunc(Function function, string x)
        {
            return new org.mariuszgromada.math.mxparser.Expression($"f({x})", function).calculate();
        }

        //  Метод для вычисления значения функции в точке x
        public static double EvaluateFunction(string functionExpression, double x)
        {
            var expression = new NCalc.Expression(functionExpression);
            expression.Parameters["x"] = x;
            return Convert.ToDouble(expression.Evaluate());
        }
    }

    public class FunctionViewModel : INotifyPropertyChanged
    {
        private string functionExpression;
        private double parametrA;
        private double parametrB;
        private double epsilon = 0.01; 
        private PlotModel plotModel;  // основной класс в библиотеке OxyPlot, используемый для создания графиков и диаграмм
        private string resultText;
        private int widthXAxis;
        private int widthYAxis;

        public string FunctionExpression
        {
            get => functionExpression;
            set
            {
                functionExpression = value;
                OnPropertyChanged(nameof(FunctionExpression));
            }
        }

        public double ParametrA
        {
            get => parametrA;
            set
            {
                parametrA = value;
                OnPropertyChanged(nameof(ParametrA));
            }
        }

        public double ParametrB
        {
            get => parametrB;
            set
            {
                parametrB = value;
                OnPropertyChanged(nameof(ParametrB));
            }
        }

        public double Epsilon
        {
            get => epsilon;
            set
            {
                epsilon = value;
                OnPropertyChanged(nameof(Epsilon));
            }
        }

        public PlotModel PlotModel
        {
            get => plotModel;
            private set
            {
                plotModel = value;
                OnPropertyChanged(nameof(PlotModel));
            }
        }

        public string ResultText
        {
            get => resultText;
            set
            {
                resultText = value;
                OnPropertyChanged(nameof(ResultText));
            }
        }

        public int WidthXAxis
        {
            get => widthXAxis;
            set
            {
                widthXAxis = value;
                OnPropertyChanged(nameof(WidthXAxis));
            }
        }

        public int WidthYAxis
        {
            get => widthYAxis;
            set
            {
                widthYAxis = value;
                OnPropertyChanged(nameof(WidthYAxis));
            }
        }

        // Команда для вызова метода
        public ICommand ConstructPlotCommand { get; }
        public ICommand FindPointOfIntersectionCommand { get; }

        public FunctionViewModel()
        {
            // Привязываем команду к методу
            ConstructPlotCommand = new RelayCommand(_ => ConstructPlot());
            FindPointOfIntersectionCommand = new RelayCommand(_ => FindPointOfIntersection());

            // Инициализируем пустой график
            PlotModel = new PlotModel { Title = "График функции" };
        }

        private void FindPointOfIntersection()
        {
            double result = FunctionModel.FindPointOfIntersectionDihotomyMethod(FunctionExpression, ParametrA, ParametrB, Epsilon);
            ResultText = $"Точка пересечения (x): {result}";
        }

        private void ConstructPlot()
        {
            // Обновляем график
            PlotModel = new PlotModel { Title = "График функции" };
            var series = new LineSeries { Title = "f(x)", StrokeThickness = 2 };

            // Настройка оси X
            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom, // Ось X снизу
                Minimum = WidthXAxis / -2,  // Минимум по X
                Maximum = WidthXAxis /  2,   // Максимум по X
                Title = "",  // Подпись оси
                //MajorGridlineStyle = LineStyle.Solid, // Основная сетка
                MinorGridlineStyle = LineStyle.Dot,   // Второстепенная сетка
                PositionAtZeroCrossing = true // Ось X пересекается с осью Y в 0
            };

            // Настройка оси Y
            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left, // Ось Y слева
                Minimum = WidthYAxis / -2,  // Минимум по Y
                Maximum = WidthYAxis / 2,   // Максимум по Y
                Title = "",  // Подпись оси
                //MajorGridlineStyle = LineStyle.Solid, // Основная сетка
                MinorGridlineStyle = LineStyle.Dot,   // Второстепенная сетка
                PositionAtZeroCrossing = true // Ось Y пересекается с осью X в 0
            };

            // Добавляем оси в модель
            PlotModel.Axes.Add(xAxis);
            PlotModel.Axes.Add(yAxis);

            // Рисуем график
            for (double x = xAxis.Minimum; x <= xAxis.Maximum; x += 1)
            {
                double y = FunctionModel.EvaluateFunction(FunctionExpression, x);
                series.Points.Add(new DataPoint(x, y));
            }

            PlotModel.Series.Clear();
            PlotModel.Series.Add(series);
            PlotModel.InvalidatePlot(true);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);
        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new FunctionViewModel();
        }
    }
}
