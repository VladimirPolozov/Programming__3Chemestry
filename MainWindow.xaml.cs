using org.mariuszgromada.math.mxparser;
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
        private static readonly double PHI = (1 + Math.Sqrt(5)) / 2;
        private const double DEFAULT_DELTA = 0.0000001F;

        //  Поиск точки пересечения графика функции с осью абсцисс методом дихотомии
        public static double FindPointOfIntersectionDihotomyMethod(string functionExpression, double parametrA, double parametrB, double epsilon)
        {
            Function expression = ConvertExpressionToFunctionFromString(functionExpression);
            double parametrAValue = SolveFunc(expression, parametrA);
            double parametrBValue = SolveFunc(expression, parametrB);
            double middleOfSegment = 0;
            double middleOfSegmentValue;
            double xMin = Math.Round(FindMinimumByGoldenSection(functionExpression, parametrA, parametrB, epsilon), 2, MidpointRounding.AwayFromZero);
            double xMinValue = SolveFunc(expression, xMin);

            if (parametrAValue * xMinValue > 0 && parametrBValue * xMinValue > 0)
            {
                throw new ArgumentException("Функция имеет более не имеет точек пересечения с осью абсцисс на заданном интервале, либо их количество чётно");
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

        //  Поиск точки минимума методом золотого сечения 
        public static double FindMinimumByGoldenSection(string functionExpression, double parametrA, double parametrB, double epsilon)
        {
            Function expression = ConvertExpressionToFunctionFromString(functionExpression);
            do
            {
                double firstDot = parametrB - (parametrB - parametrA) / PHI;
                double secondDot = parametrA + (parametrB - parametrA) / PHI;
                if (SolveFunc(expression, firstDot) >= SolveFunc(expression, secondDot))
                {
                    parametrA = firstDot;
                } else
                {
                    parametrB = secondDot;
                }
            } while (Math.Abs(parametrB - parametrA) > epsilon);
            return (parametrA + parametrB) / 2;
        }


        //  Поиск точки максимума методом золотого сечения 
        public static double FindMaximumByGoldenSection(string functionExpression, double parametrA, double parametrB, double epsilon)
        {
            Function expression = ConvertExpressionToFunctionFromString(functionExpression);
            do
            {
                double firstDot = parametrB - (parametrB - parametrA) / PHI;
                double secondDot = parametrA + (parametrB - parametrA) / PHI;
                if (SolveFunc(expression, firstDot) <= SolveFunc(expression, secondDot))
                {
                    parametrA = firstDot;
                } else
                {
                    parametrB = secondDot;
                }
            } while (Math.Abs(parametrB - parametrA) > epsilon);
            return (parametrA + parametrB) / 2;
        }

        // Поиск точки пересечения (нуль функции) методом Ньютона
        public static double FindPointOfIntersectionNewtonMethod(string functionExpression, double parametrB)
        {
            Function expression = ConvertExpressionToFunctionFromString(functionExpression);
            return parametrB - (SolveFunc(expression, parametrB) / GetDerivative(expression, parametrB));
        }

        // Поиск экстремума (точка минимума или максимума) методом Ньютона
        public static double FindExtremeNewtonMethod(string functionExpression, double parametrB)
        {
            Argument argument = new Argument($"x = {parametrB}");
            Expression firstDerivative = new Expression($"der(({functionExpression}), x)", argument);
            Expression secondDerivative = new Expression($"der(der({functionExpression}, x), x)", argument);
            double firstDerivativeValue = firstDerivative.calculate();
            double secondDerivativeValue = secondDerivative.calculate();
            return parametrB - (firstDerivativeValue / secondDerivativeValue);
        }

        public static double GetDerivative(Function function, double point)
        {
            return (SolveFunc(function, point + DEFAULT_DELTA) - SolveFunc(function, point - DEFAULT_DELTA)) / (2 * DEFAULT_DELTA);
        }

        //  Метод для вычисления значения функции в точке x
        public static double SolveFunc(Function function, double x)
        {
            return new Expression($"f({x.ToString().Replace(",", ".")})", function).calculate();
        }

        // конвертирует выражение из типа строка в тип Function
        public static Function ConvertExpressionToFunctionFromString(string functionExpression)
        {
            return new Function("f(x) = " + functionExpression);
        }
    }

    public class FunctionViewModel : INotifyPropertyChanged
    {
        private PlotModel plotModel;  // основной класс в библиотеке OxyPlot, используемый для создания графиков и диаграмм
        private string functionExpression;
        private double parametrA;
        private double parametrB;
        private double epsilon; 
        private string resultText;
        private int widthXAxis;
        private int widthYAxis;
        private int countOfSingsAfterComma;
        private int graphicThickness;
        private double graphicPointsDelta;

        public string FunctionExpression
        {
            get => functionExpression;
            set
            {
                functionExpression = value.ToLower();
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

        public int GraphicThickness
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
        public ICommand FindPointOfIntersectionDihotomyCommand { get; }
        public ICommand FindPointOfIntersectionNewtonCommand { get; }
        public ICommand SetDefaultDataCommand { get; }
        public ICommand FindMinimumByGoldenSectionCommand { get; }
        public ICommand FindMaximumByGoldenSectionCommand { get; }
        public ICommand FindExtremeNewtonCommand { get; }

        public FunctionViewModel()
        {
            // вставляем в форму данные по умолчанию
            SetDefaultData();

            // Привязываем команды к методу
            ConstructPlotCommand = new RelayCommand(_ => ConstructPlot());
            FindPointOfIntersectionDihotomyCommand = new RelayCommand(_ => FindPointOfIntersectionDihotomy());
            FindPointOfIntersectionNewtonCommand = new RelayCommand(_ => FindPointOfIntersectionNewton());
            SetDefaultDataCommand = new RelayCommand(_ => SetDefaultData());
            FindMinimumByGoldenSectionCommand = new RelayCommand(_ => FindMinimum());
            FindMaximumByGoldenSectionCommand = new RelayCommand(_ => FindMaximum());
            FindExtremeNewtonCommand = new RelayCommand(_ => FindExtremeNewton());

            // Инициализируем пустой график
            PlotModel = new PlotModel { Title = "График функции" };
        }

        public void SetDefaultData()
        {
            FunctionExpression = "x";
            ParametrA = -50;
            ParametrB = 50;
            Epsilon = 0.01;
            WidthXAxis = 50;
            WidthYAxis = 50;
            CountOfSingsAfterComma = 2;
            GraphicPointsDelta = 0.5;
            GraphicThickness = 2;
            resultText = "";
        }

        private void FindPointOfIntersectionDihotomy()
        {
            try
            {
                double result = FunctionModel.FindPointOfIntersectionDihotomyMethod(FunctionExpression, ParametrA, ParametrB, Epsilon);
                ResultText = $"Точка пересечения (x): {Math.Round(result, CountOfSingsAfterComma, MidpointRounding.AwayFromZero)}";
                ConstructPlot(result);
            } catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        public void FindPointOfIntersectionNewton()
        {
            try {
                double currentPoint;
                double nextPoint = parametrB;
                do
                {
                    currentPoint = nextPoint;
                    nextPoint = FunctionModel.FindPointOfIntersectionNewtonMethod(FunctionExpression, currentPoint);
                    ConstructPlot(currentPoint);
                    MessageBox.Show($"Промежуточный результат: x_i = {Math.Round(nextPoint, CountOfSingsAfterComma, MidpointRounding.AwayFromZero)}");
                } while (Math.Abs(nextPoint - currentPoint) > epsilon);
                ResultText = $"Точка пересечения (x): {Math.Round(nextPoint, CountOfSingsAfterComma, MidpointRounding.AwayFromZero)}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        public void FindExtremeNewton()
        {
            try
            {
                double currentPoint;
                double nextPoint = parametrB;
                do
                {
                    currentPoint = nextPoint;
                    nextPoint = FunctionModel.FindExtremeNewtonMethod(FunctionExpression, currentPoint);
                    ConstructPlot(nextPoint);
                    MessageBox.Show($"Промежуточный результат: x_i = {Math.Round(nextPoint, CountOfSingsAfterComma, MidpointRounding.AwayFromZero)}");
                } while (Math.Abs(nextPoint - currentPoint) > epsilon);
                ResultText = $"Точка пересечения (x): {Math.Round(nextPoint, CountOfSingsAfterComma, MidpointRounding.AwayFromZero)}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }

        }

        private void FindMinimum()
        {
            try
            {
                double result = FunctionModel.FindMinimumByGoldenSection(FunctionExpression, ParametrA, ParametrB, Epsilon);
                ResultText = $"Точка минимума (x): {Math.Round(result, CountOfSingsAfterComma, MidpointRounding.AwayFromZero)}";
                ConstructPlot(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void FindMaximum()
        {
            try
            {
                double result = FunctionModel.FindMaximumByGoldenSection(FunctionExpression, ParametrA, ParametrB, Epsilon);
                ResultText = $"Точка максимума (x): {Math.Round(result, CountOfSingsAfterComma, MidpointRounding.AwayFromZero)}";
                ConstructPlot(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void ConstructPlot(double markCoordX)
        {
            // Обновляем график
            PlotModel = new PlotModel { Title = "График функции" };
            var series = new LineSeries { Title = "f(x)", StrokeThickness = GraphicThickness };
            var mark = new LineSeries { Title = "f(x)", StrokeThickness = 1, Color = OxyColors.Blue };

            // Настройка оси X
            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom, // Ось X снизу
                Minimum = WidthXAxis / -2,  // Минимум по X
                Maximum = WidthXAxis / 2,   // Максимум по X
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

            // Рисуем вертикальную линию
            for (double y = yAxis.Minimum; y <= yAxis.Maximum; y += 1)
            {
                mark.Points.Add(new DataPoint(markCoordX, y));
            }

            PlotModel.Series.Add(series);
            PlotModel.Series.Add(mark);
            PlotModel.InvalidatePlot(true);
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
