using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace Wpf.DataTemplates.WithoutDataTemplates
{

    class DataSource : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public static int TimeMod = 30;

        private readonly string blueColor = "#005073";
        public string BlueColor => blueColor;
        private readonly string goldColor = "#EFDC22";
        public string GoldColor => goldColor;

        private readonly string sliderThumb = "resources/images/rate1.png";
        public string SliderThumb => sliderThumb;
        private readonly string sliderThumbDisabled = "resources/images/rate.png";
        public string SliderThumbDisabled => sliderThumbDisabled;

        private readonly string settingsIcon = "resources/images/settings.png";
        public string SettingsIcon => settingsIcon;

        private readonly string playIcon = "resources/images/icon.ico";
        public string PlayIcon => playIcon;

        private readonly string infoIcon = "resources/images/about.png";
        public string InfoIcon => infoIcon;
        private readonly string infoIconS = "resources/images/about1.png";
        public string InfoIconS => infoIconS;

        private readonly string newGameIcon = "resources/images/play_btn.png";
        public string NewGameIcon => newGameIcon;
        private readonly string newGameIconS = "resources/images/play_btn1.png";
        public string NewGameIconS => newGameIconS;

        private readonly string tabBg = "resources/images/tab_bg.png";
        public string TabBg => tabBg;

        public static int difficulty = 1;
        public int Difficulty
        {
            get { return difficulty; }
            set
            {
                if (!difficulty.Equals(value))
                {
                    difficulty = value;
                    onPrCh(new PropertyChangedEventArgs(nameof(DifficultyText)));
                }
            }
        }
        private string difficultyText;
        public string DifficultyText
        {
            get
            {
                switch (difficulty)
                {
                    case 1:
                        difficultyText = "Beginner";
                        break;
                    case 2:
                        difficultyText = "Advanced";
                        break;
                    case 3:
                        difficultyText = "Master";
                        break;
                    case 4:
                        difficultyText = "Champion";
                        break;
                }
                return difficultyText;
            }
        }

        public static int sound = 0;
        public int Sound
        {
            get { return sound; }
            set
            {
                if (!sound.Equals(value))
                {
                    sound = value;
                    onPrCh(new PropertyChangedEventArgs(nameof(SoundText)));
                }
            }
        }
        private string soundText;
        public string SoundText
        {
            get
            {
                switch (sound)
                {
                    case 0:
                        soundText = "Off";
                        break;
                    case 1:
                        soundText = "On";
                        break;
                }
                return soundText;
            }
        }

        private readonly List<Card> cards;
        public List<Card> Cards => cards;

        List<string> loadedImages;

        public static DispatcherTimer gameTimer = new DispatcherTimer();

        public static int time = difficulty * TimeMod;
        public static int timeMax = difficulty * TimeMod;
        public int Time
        {
            get { return time; }
            set
            {
                if (!time.Equals(value))
                {
                    time = value;
                    onPrCh(new PropertyChangedEventArgs(nameof(Time)));
                }
            }
        }
        public int TimeMax
        {
            get { return timeMax; }
            set
            {
                if (!timeMax.Equals(value))
                {
                    timeMax = value;
                    onPrCh(new PropertyChangedEventArgs(nameof(TimeMax)));
                }
            }
        }

        public DataSource()
        {
            cards = new List<Card>();
            loadCards();
            gameTimer.Interval = new TimeSpan(0, 0, 1);
            gameTimer.Tick += TimerTick;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            time--;
            onPrCh(new PropertyChangedEventArgs(nameof(Time)));
            if (time == 0) gameTimer.Stop();
        }

        private void loadCards()
        {
            if (Directory.GetFiles(Card.path).Length != 0)
            {
                loadedImages = new List<string>();
                foreach (string f in Directory.GetFiles(Card.path))
                {
                    if (f.Split('.').Last() == "png") loadedImages.Add(f);
                }
                int d_tmp = (difficulty + 1) * (difficulty + 2) / 2;
                if (loadedImages.Count != 0 && loadedImages.Count >= d_tmp)
                {
                    cards.Clear();
                    List<int> nums = new List<int>();
                    List<int> numsR = new List<int>();
                    Random rand = new Random();
                    for (int i = 0; i < loadedImages.Count; i++) nums.Add(i);
                    for (int i = 0; i < d_tmp; i++)
                    {
                        int index = rand.Next(nums.Count);
                        numsR.Add(nums[index]); numsR.Add(nums[index]);
                        nums.RemoveAt(index);
                    }
                    for (int i = 0; i < d_tmp * 2; i++)
                    {
                        int index = rand.Next(numsR.Count);
                        cards.Add(new Card(loadedImages[numsR[index]], difficulty + 2));
                        numsR.RemoveAt(index);
                    }
                }
                else
                {
                    //Err();
                }
            }
            else
            {
                //Err();
            }
        }

        private ICommand newGCommand;
        public ICommand NewGCommand
        {
            get
            {
                if (newGCommand == null)
                {
                    newGCommand = new RelayCommand(
                        param => this.NewGame(),
                        param => this.CanEx()
                    );
                }
                return newGCommand;
            }
        }

        private bool CanEx()
        {
            return true;
        }

        private void NewGame()
        {
            gameTimer.Stop();
            Card.pair.Clear();
            Card.pairsCount = 0;
            loadCards();
            time = difficulty * TimeMod;
            onPrCh(new PropertyChangedEventArgs(nameof(Time)));
            timeMax = difficulty * TimeMod;
            onPrCh(new PropertyChangedEventArgs(nameof(TimeMax)));
            CollectionViewSource.GetDefaultView(Cards).Refresh();
        }

        private void onPrCh(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}