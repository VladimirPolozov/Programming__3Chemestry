﻿using org.mariuszgromada.math.mxparser;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Expression = org.mariuszgromada.math.mxparser.Expression;


namespace FirstLab
{
    public class FunctionModel
    {
        //  Поиск точки пересечения графика функции с осью абсцисс методом дихотомии
        public static double FindPointOfIntersectionDihotomyMethod(string functionExpression, double parametrA, double parametrB, double epsilon)
        {
            Function expression = ConvertExpressionToFunctionFromString(functionExpression);
            double parametrAValue = SolveFunc(expression, parametrA);
            double parametrBValue = SolveFunc(expression, parametrB);
            double middleOfSegment = 0;
            double middleOfSegmentValue;

            if (parametrAValue * parametrBValue >= 0) {
                throw new ArgumentException("Функция не имеет точек пересечения с осью абсцисс на заданном интервале");
            }

            while (parametrB - parametrA > epsilon) {
                middleOfSegment = (parametrA + parametrB) / 2;
                middleOfSegmentValue = SolveFunc(expression, middleOfSegment);

                if (middleOfSegmentValue == 0) {
                    break;
                } else if (parametrAValue == 0) {
                    return parametrA;
                } else if (parametrBValue == 0)
                {
                    return parametrB;
                } else if (parametrAValue * middleOfSegmentValue < 0)
                {
                    parametrB = middleOfSegment;
                } else {
                    parametrA = middleOfSegment;
                    parametrAValue = middleOfSegmentValue;
                }
            }

            return middleOfSegment;
        }

        public static double SolveFunc(Function function, double x)
        {
            return new Expression($"f({x.ToString().Replace(",", ".")})", function).calculate();
        }

        //  Метод для вычисления значения функции в точке x
        public static Function ConvertExpressionToFunctionFromString(string functionExpression)
        {
            return new Function("f(x) = " + functionExpression);
        }
    }

    public class FunctionViewModel : INotifyPropertyChanged
    {
        private string functionExpression;
        private double parametrA = 50;
        private double parametrB = -50;
        private double epsilon = 0.01; 
        private PlotModel plotModel;  // основной класс в библиотеке OxyPlot, используемый для создания графиков и диаграмм
        private string resultText;
        private int widthXAxis = 50;
        private int widthYAxis = 50;
        private int countOfSingsAfterComma = 2;
        private double graphicPointsDelta = 0.5;
        private double graphicThickness = 2;

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

        public int CountOfSingsAfterComma
        {
            get => countOfSingsAfterComma;
            set
            {
                countOfSingsAfterComma = value;
                OnPropertyChanged(nameof(CountOfSingsAfterComma));
            }
        }

        public double GraphicPointsDelta
        {
            get => graphicPointsDelta;
            set
            {
                graphicPointsDelta = value;
                OnPropertyChanged(nameof(GraphicPointsDelta));
            }
        }

        public double GraphicThickness
        {
            get => graphicThickness;
            set
            {
                graphicThickness = value;
                OnPropertyChanged(nameof(GraphicThickness));
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
            ResultText = $"Точка пересечения (x): {Math.Round(result, CountOfSingsAfterComma, MidpointRounding.AwayFromZero)}";
        }

        private void ConstructPlot()
        {
            // Обновляем график
            PlotModel = new PlotModel { Title = "График функции" };
            var series = new LineSeries { Title = "f(x)", StrokeThickness = GraphicThickness };

            // Настройка оси X
            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom, // Ось X снизу
                Minimum = WidthXAxis / -2,  // Минимум по X
                Maximum = WidthXAxis /  2,   // Максимум по X
                Title = "",  // Подпись оси
                // MajorGridlineStyle = LineStyle.Solid, // Основная сетка
                // MinorGridlineStyle = LineStyle.Dot,   // Второстепенная сетка
                PositionAtZeroCrossing = true // Ось X пересекается с осью Y в 0
            };

            // Настройка оси Y
            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left, // Ось Y слева
                Minimum = WidthYAxis / -2,  // Минимум по Y
                Maximum = WidthYAxis / 2,   // Максимум по Y
                Title = "",  // Подпись оси
                // MajorGridlineStyle = LineStyle.Solid, // Основная сетка
                // MinorGridlineStyle = LineStyle.Dot,   // Второстепенная сетка
                PositionAtZeroCrossing = true // Ось Y пересекается с осью X в 0
            };

            // Добавляем оси в модель
            PlotModel.Axes.Add(xAxis);
            PlotModel.Axes.Add(yAxis);

            // Рисуем график
            for (double x = xAxis.Minimum; x <= xAxis.Maximum; x += graphicPointsDelta)
            {
                double y = FunctionModel.SolveFunc(FunctionModel.ConvertExpressionToFunctionFromString(FunctionExpression), x);
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
