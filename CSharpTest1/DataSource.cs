using CSharpTest1;
using System.ComponentModel;
using System.Windows.Input;


namespace Wpf.DataTemplates.WithoutDataTemplates
{

    class DataSource : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        
        Random random = new Random();
        //int maxRandom = Int32.MaxValue;
        int maxRandom = 1000;
        private HedgehogCalculator? hedgehogCalculator;

        // Прив'язка зображень і динамічно змінюваного тексту
        public string Icon => "resources/images/icon.ico";
        public string ButtonPrev => "resources/images/arrow_prev.png";
        public string ButtonNext => "resources/images/arrow_next.png";

        private string resultText = string.Empty;
        public string ResultText => resultText;

        private static readonly string[] colorImages = [
            "resources/images/hedgehog_red.png", 
            "resources/images/hedgehog_green.png", 
            "resources/images/hedgehog_blue.png"
            ];

        private readonly string hedgehogRedImage = colorImages[0];

        private readonly string hedgehogGreenImage = colorImages[1];

        private readonly string hedgehogBlueImage = colorImages[2];

        private static int colorIndex = 0;
        private string colorChoise = colorImages[colorIndex];

        private string countRed = "1";
        private string countGreen = "1";
        private string countBlue = "1";
        
        public string HedgehogRedImage => hedgehogRedImage;
        public string HedgehogGreenImage => hedgehogGreenImage;
        public string HedgehogBlueImage => hedgehogBlueImage;
        public string ColorChoise => colorChoise;

        // Двостороння прив'язка текстових полів
        public string CountRed
        {
            get { return countRed; }
            set
            {
                if (!countRed.Equals(value))
                {
                    countRed = value;
                }
            }
        }
        public string CountGreen
        {
            get { return countGreen; }
            set
            {
                if (!countGreen.Equals(value))
                {
                    countGreen = value;
                }
            }
        }
        public string CountBlue
        {
            get { return countBlue; }
            set
            {
                if (!countBlue.Equals(value))
                {
                    countBlue = value;
                }
            }
        }

        
        // Прив'язка команд
        private ICommand? colorChangeCommand;
        public ICommand ColorChangeCommand
        {
            get
            {
                if (colorChangeCommand == null)
                {
                    colorChangeCommand = new RelayCommand(
                        param => this.colorChange((bool)param),
                        param => { return true; }
                    );
                }
                return colorChangeCommand;
            }
        }
        public void colorChange(bool isNext)
        {
            if (isNext)
            {
                if (colorIndex == colorImages.Length - 1) colorIndex = 0;
                else colorIndex++;
            }
            else
            {
                if (colorIndex == 0) colorIndex = colorImages.Length - 1;
                else colorIndex--;
            }
            
            colorChoise = colorImages[colorIndex];
            onPrCh(new PropertyChangedEventArgs(nameof(ColorChoise)));
        }


        private ICommand? randomizeCommand;
        public ICommand RandomizeCommand
        {
            get
            {
                if (randomizeCommand == null)
                {
                    randomizeCommand = new RelayCommand(
                        param => this.randomize(),
                        param => { return true; }
                    );
                }
                return randomizeCommand;
            }
        }
        public void randomize()
        {
            countRed = random.Next(1, maxRandom).ToString();
            countGreen = random.Next(1, maxRandom).ToString();
            countBlue = random.Next(1, maxRandom).ToString();
            colorIndex = random.Next(colorImages.Length);
            colorChoise = colorImages[colorIndex];
            onPrCh(new PropertyChangedEventArgs(nameof(ColorChoise)));
            onPrCh(new PropertyChangedEventArgs(nameof(CountRed)));
            onPrCh(new PropertyChangedEventArgs(nameof(CountGreen)));
            onPrCh(new PropertyChangedEventArgs(nameof(CountBlue)));
        }


        // Команда Calculate
        // Тут створюється новий об'єкт основного робочого классу HedgehogCalculator
        // і викликається його функція calculate(), яка містить основну логіку завдання
        // В залежності від того значення, що повернула функція calculate() виводитться текстовий результат
        private ICommand? calculateCommand;
        public ICommand CalculateCommand
        {
            get
            {
                if (calculateCommand == null)
                {
                    calculateCommand = new RelayCommand(
                        param => this.calculate(),
                        param => { return true; }
                    );
                }
                return calculateCommand;
            }
        }
        public void calculate()
        {
            // Про всяк випадок ще раз валідуємо дані, що прийшли з textbox
            try
            {
                int red = Int32.Parse(countRed);
                int green = Int32.Parse(countGreen);
                int blue = Int32.Parse(countBlue);
                if (red < 0 || green < 0 || blue < 0) throw new Exception();

                hedgehogCalculator = new HedgehogCalculator([red, green, blue], colorIndex);
                int result = hedgehogCalculator.Calculate();
                switch (result)
                {
                    case -1:
                        resultText = "Результат: всі їжачки не зможуть стати одного кольору.";
                        break;
                    case 0:
                        resultText = "Результат: всі їжачки відпочатку були заданого кольору кольору.";
                        break;
                    default:
                        resultText = resultText = "Результат: знадобиться " + result + " зустрічей, щоб всі їжачки стали обраного кольору.";
                        break;
                }
            }
            catch
            {
                resultText = "Невірний формат даних!";
            }           
            onPrCh(new PropertyChangedEventArgs(nameof(ResultText)));
        }

        private void onPrCh(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}