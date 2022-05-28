using System;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Core;
using Windows.System;
using DodgeGame.Classes; /*Classes folder*/

namespace DodgeGame
{
    public sealed partial class MainPage : Page
    {
        /*Essentials*/
        private Rect window; /*Rec wich represent the current window*/
        private Board game;
        public Rectangle playerRect;
        private Rectangle[] enemisPieces;
        private DispatcherTimer timer;
        private const int TIMEINTERVAL = 10;

        /*UI additionals elements*/
        private Popup finish;
        private ProgressBar pb;
        private CommandBar cmdBar;
        private const double CMD_BAR_HEIGHT = 70;
        private AppBarButton Reset, Pause, Play, ChangeHero;
        private TextBlock EscapeMessege;

        /*User choosing hero managment */
        private int currentHeroIndex;
        private string HeroPicSrc, heroThemeSrc, clashSoundSrc;
        private MediaElement heroTheme, clashSound, congratsVideo;
        private const int HEROSETARGUMENTS = 3;
        private DispatcherTimer Videotimer;
        private const int VIDEODURATION = 67;

        /*src for images sound tracks and video*/
        /*BackGround photo*/
        private const string backGroundPhoto = "ms-appx:///imagesSrc/EightMile2.jpg";
        /*first hero src*/
        private const string Kunta = "ms-appx:///imagesSrc/Kendrick.png";
        private const string KuntaSound = "ms-appx:///AudioFiles/KendrickHumble8Bit.mp3";
        private const string KuntaClashSound = "ms-appx:///AudioFiles/BeHumbleKendrick.mp3";
        /*second hero src*/
        private const string Snoop = "ms-appx:///imagesSrc/Snoop.png";
        private const string SnoopSound = "ms-appx:///AudioFiles/StillDRE8Bit.mp3";
        private const string SnoopClashSound = "ms-appx:///AudioFiles/ProRapperSnoop.mp3";
        /*third hero src*/
        private const string Eminem = "ms-appx:///imagesSrc/Eminem.png";
        private const string EminemSound = "ms-appx:///AudioFiles/TheRealSlimShady8Bit.mp3";
        private const string EminemClashSound = "ms-appx:///AudioFiles/MotherEminem.mp3";
        private const string DaBaby = "ms-appx:///imagesSrc/The_Baby.png";
        private const string congrats = "ms-appx:///Video/KuntaCongrats.mp4";

        public MainPage()
        {
            this.InitializeComponent();
            currentHeroIndex = -1;
            startGame();
            createCmdBar();
            createLife();
            CreateFinishPopUp();
            createMediaElements();
            setHero(nextHero());
            createEscapeCongratsMessege();

            /*Start game only after all loaded*/
            createTimer(TIMEINTERVAL);

            /*event handlers*/
            timer.Tick += TmerTick;
            Window.Current.CoreWindow.KeyDown += KeyDownMethod; /*Enables key down method*/
            Pause.Click += Pause_Click;
            Reset.Click += ReSet_Click;
            Play.Click += Play_Click;
            ChangeHero.Click += ChangeHero_Click;
            finish.Closed += Finish_Closed;
            Videotimer.Tick += Videotimer_Tick;
        }

        /*Game start */
        public void startGame()
        {
            /* Gets window boundries ,sets board ,sets 1 player Rectangele ,set enemis Rectangele array and  Sets theme*/

            window = ApplicationView.GetForCurrentView().VisibleBounds;
            CanvasElm.Background = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(backGroundPhoto))
            };
            game = new Board(window.Width, window.Height);
            playerRect = createGamePiece(game.user);
            enemisPieces = new Rectangle[Board.ENEMYS_COUNT];

            for (int i = 0; i < game.enemis.Length; i++)
            {
                enemisPieces[i] = createGamePiece(game.enemis[i]);
                enemisPieces[i].Name = $"Enem{game.enemis[i].SerialNum}";
                enemisPieces[i].Fill = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(DaBaby))
                };
            }
        }
        public Rectangle createGamePiece(GamePiece gp)
        {
            /*Responsible for creating UI object fo each gamePiece obj*/

            Rectangle currentShape = new Rectangle();
            if (gp is UserPiece)
            {
                currentShape.Fill = new SolidColorBrush(Colors.DarkSeaGreen);
            }
            else if (gp is EnemyPiece)
            {
                currentShape.Fill = new SolidColorBrush(Colors.IndianRed);
            }
            currentShape.Width = gp.RecWidth;
            currentShape.Height = gp.RecHeight;
            Canvas.SetLeft(currentShape, gp.X);
            Canvas.SetTop(currentShape, gp.Y);
            CanvasElm.Children.Add(currentShape);
            return currentShape;
        }
        private void createTimer(int TIMEINTERVAL)
        {
            /*game's main timer*/
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, TIMEINTERVAL);

            /*Timer for closing congrats video*/
            Videotimer = new DispatcherTimer();
            Videotimer.Interval = new TimeSpan(0, 0, VIDEODURATION);

            timer.Start();
        }

        /*Game managment*/
        private void TmerTick(object sender, object e)
        {
            /*moevs enemy ==> check for clash ==> decrease users life or remove enemis ==> check win check loose ==>
             * raise apropriate pop ==> raise level if win*/

            EnemisMove();
            game.userClashed();
            for (int i = 0; i < game.enemis.Length; i++)
            {
                if (game.enemisClash(game.enemis[i]))
                {
                    enemisPieces[i].Visibility = Visibility.Collapsed;
                    CanvasElm.Children.Remove(enemisPieces[i]);
                    clashSound.Play();
                }
            }
            pb.Value = game.user.HPPercent;
            if (game.user.Life <= 0)
            {
                lose();
            }
            else if (game.IsWinner())
            {
                win();
            }
        }
        private void KeyDownMethod(CoreWindow sender, KeyEventArgs args)
        {
            /*User movment and keyboard shortcuts*/

            /*Finished game kendrick video is on*/
            if (Videotimer.IsEnabled && (args.VirtualKey == VirtualKey.Enter || args.VirtualKey == VirtualKey.Space))
            {
                congratsVideo.Stop();
                Videotimer.Stop();
                congratsVideo.Visibility = Visibility.Collapsed;
                EscapeMessege.Visibility = Visibility.Collapsed;

                heroTheme.Play();
                RestartGame();
                timer.Start();
            }

            /*Level finished popup*/
            else if (finish.IsOpen && !Videotimer.IsEnabled)
            {
                finish.IsOpen = false;
            }

            /*If game paused p resume the game or restart case you user is no alive*/
            else if (!timer.IsEnabled && args.VirtualKey == VirtualKey.P && !Videotimer.IsEnabled)
            {
                if (!game.IsWinner() && game.user.Life > 0)
                {
                    timer.Start();
                }
                else
                {
                    RestartGame();
                }
            }
            /* Space change the hero theme*/
            else if (args.VirtualKey == VirtualKey.Space)
            {
                setHero(nextHero());
            }
            /*While game is running following the keybaord control the flow : UP, DOWN ,RIGHT, LEFT ==  W, S, D, A , R==> Restart P==> Pause*/

            else if (timer.IsEnabled == true && !Videotimer.IsEnabled)
            {
                switch (args.VirtualKey)
                {
                    case VirtualKey.Up:
                        {
                            game.userMove("up");
                            break;
                        }
                    case VirtualKey.W:
                        {
                            game.userMove("up");
                            break;
                        }
                    case VirtualKey.Down:
                        {
                            game.userMove("down");
                            break;
                        }
                    case VirtualKey.S:
                        {
                            game.userMove("down");
                            break;
                        }
                    case VirtualKey.Right:
                        {
                            game.userMove("right");
                            break;
                        }
                    case VirtualKey.D:
                        {
                            game.userMove("right");
                            break;
                        }
                    case VirtualKey.Left:
                        {
                            game.userMove("left");
                            break;
                        }
                    case VirtualKey.A:
                        {
                            game.userMove("left");
                            break;
                        }
                    case VirtualKey.R:
                        {
                            RestartGame();
                            break;
                        }
                    case VirtualKey.P:
                        {
                            timer.Stop();
                            break;
                        }
                }
                Canvas.SetTop(playerRect, game.user.Y);
                Canvas.SetLeft(playerRect, game.user.X);
            }
        }
        private void EnemisMove()
        {
            for (int i = 0; i < game.enemis.Length; i++)
            {
                if (game.enemis[i].Life > 0)
                {
                    game.enemyMove(game.enemis[i]);
                    Canvas.SetLeft(enemisPieces[i], game.enemis[i].GetCenterX());
                    Canvas.SetTop(enemisPieces[i], game.enemis[i].GetCenterY());
                }
            }
        }
        private void RestartGame()
        {
            double currentEnemySpeed = game.enemySpeed;
            game = new Board(window.Width, window.Height, Canvas.GetLeft(playerRect), Canvas.GetTop(playerRect), currentEnemySpeed);
            CanvasElm.Children.Clear();
            createLife();
            enemisPieces = new Rectangle[Board.ENEMYS_COUNT];
            playerRect = createGamePiece(game.user);
            playerRect.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(HeroPicSrc))
            };
            for (int i = 0; i < game.enemis.Length; i++)
            {
                enemisPieces[i] = createGamePiece(game.enemis[i]);
                enemisPieces[i].Name = $"Enem{game.enemis[i].SerialNum}";
                enemisPieces[i].Fill = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(DaBaby))
                };
                game.enemis[i].Life = 1;
            }
        }
        private void win()
        {
            game.enemySpeed += 0.5;
            timer.Stop();
            if (game.enemySpeed >= 9) /*Final speed*/
            {
                PlayCongrats();
                game.enemySpeed = 1;
            }
            else
            {
                openFinishPopup("Well done\nLets level up!\n" +
                    "Press any key");
            }

        }
        private void lose()
        {
            timer.Stop();
            openFinishPopup("Try again\n" +
                "Press any key");
        }

        /*Additionals UI*/
        private void CreateFinishPopUp()
        {
            finish = new Popup();
            finish.Width = 450;
            finish.Height = 125;
            FatherGrid.Children.Add(finish);
            finish.Margin = new Thickness(-1 * finish.Width / 2, -2 * finish.Height / 2, 0, 0);
            finish.IsOpen = false;
        }
        private void createMediaElements()
        {
            heroTheme = new MediaElement();
            heroTheme.Visibility = Visibility.Collapsed;
            heroTheme.Name = "SoundTheme";
            heroTheme.Margin = new Thickness(0);
            heroTheme.IsLooping = true;
            heroTheme.Volume = 0.4;
            FatherGrid.Children.Add(heroTheme);

            clashSound = new MediaElement();
            clashSound.Visibility = Visibility.Collapsed;
            clashSound.Name = "ClashTheme";
            clashSound.Volume = 0.6;
            clashSound.Margin = new Thickness(0);
            clashSound.AutoPlay = false;
            clashSound.IsLooping = false;
            FatherGrid.Children.Add(clashSound);

            congratsVideo = new MediaElement();
            congratsVideo.Source = new Uri(congrats);
            congratsVideo.Width = Width / 2;
            congratsVideo.Height = Height / 2;
            congratsVideo.IsLooping = false;
            congratsVideo.Volume = 1.0;
            Canvas.SetLeft(congratsVideo, 150);
            Canvas.SetTop(congratsVideo, 0);

        }
        private void createLife()
        {
            /*Create a wide life bar represent user's life*/

            pb = new ProgressBar();
            pb.Foreground = new SolidColorBrush(Color.FromArgb(175, 180, 0, 0));
            pb.Width = 500;
            pb.Height = 40;
            pb.Value = game.user.HPPercent;
            Canvas.SetLeft(pb, 0);
            Canvas.SetTop(pb, 0);
            CanvasElm.Children.Add(pb);
        }
        public void createEscapeCongratsMessege()
        {
            EscapeMessege = new TextBlock();
            EscapeMessege.Width = 350;
            EscapeMessege.Height = 90;
            EscapeMessege.FontSize = 40;
            EscapeMessege.Margin = new Thickness(0, 300, 0, 0);
            EscapeMessege.Text = "Press Space/Enter";
            EscapeMessege.Foreground = new SolidColorBrush(Color.FromArgb(175, 180, 180, 180));
            EscapeMessege.Visibility = Visibility.Collapsed;
            FatherGrid.Children.Add(EscapeMessege);
        }
        private void createCmdBar()
        {
            ToolTip toolTipPause = new ToolTip();
            ToolTip toolTipReplay = new ToolTip();
            ToolTip toolTipPlay = new ToolTip();
            ToolTip toolTipChangeHero = new ToolTip();

            BitmapIcon biPlay = new BitmapIcon();
            biPlay.UriSource = new Uri("ms-appx:///imagesSrc/PlayFav.ico");

            BitmapIcon biReplay = new BitmapIcon();
            biReplay.UriSource = new Uri("ms-appx:///imagesSrc/ReplayFav.ico");

            BitmapIcon biPause = new BitmapIcon();
            biPause.UriSource = new Uri("ms-appx:///imagesSrc/PauseFav.ico");

            BitmapIcon biChangeHero = new BitmapIcon();
            biChangeHero.UriSource = new Uri("ms-appx:///imagesSrc/MicFav.ico");

            cmdBar = new CommandBar();
            cmdBar.Background = new SolidColorBrush(Color.FromArgb(85, 255, 255, 255));

            Play = new AppBarButton();
            Play.Label = "Play";
            Play.Icon = biPlay;
            toolTipPlay.Content = "Press to play";
            ToolTipService.SetToolTip(Play, toolTipPlay);

            Reset = new AppBarButton();
            Reset.Label = "Replay";
            Reset.Icon = biReplay;
            toolTipReplay.Content = "Press to restart the game";
            ToolTipService.SetToolTip(Reset, toolTipReplay);

            Pause = new AppBarButton();
            Pause.Label = "Pause";
            Pause.Icon = biPause;
            toolTipPause.Content = "Press to pause the game";
            ToolTipService.SetToolTip(Pause, toolTipPause);

            ChangeHero = new AppBarButton();
            ChangeHero.Label = "Change hero";
            ChangeHero.Icon = biChangeHero;
            toolTipChangeHero.Content = "Press to change hero";
            ToolTipService.SetToolTip(ChangeHero, toolTipChangeHero);

            /*Cancels tab affect*/
            ChangeHero.IsTabStop = false;
            Pause.IsTabStop = false;
            Reset.IsTabStop = false;
            Play.IsTabStop = false;
            cmdBar.IsTabStop = false;

            /*Cancel right left*/
            ChangeHero.CanBeScrollAnchor = false;
            Pause.CanBeScrollAnchor = false;
            Reset.CanBeScrollAnchor = false;
            Play.CanBeScrollAnchor = false;
            cmdBar.CanBeScrollAnchor = false;

            cmdBar.PrimaryCommands.Add(Play);
            cmdBar.PrimaryCommands.Add(Pause);
            cmdBar.PrimaryCommands.Add(Reset);
            cmdBar.PrimaryCommands.Add(ChangeHero);
            cmdBar.Height = CMD_BAR_HEIGHT;
            FatherGrid.Children.Add(cmdBar);
        }
        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            if (!Videotimer.IsEnabled)
            {
                SimulatePCControl.PArrow();
            }
        }
        private void ReSet_Click(object sender, RoutedEventArgs e)
        {
            if (Videotimer.IsEnabled)
            {
                SimulatePCControl.SpaceArrow();
            }
            SimulatePCControl.RArrow();
        }
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (!Videotimer.IsEnabled)
            {
                if (!game.IsWinner() && game.user.Life > 0)
                {
                    timer.Start();
                }
                else if (!finish.IsOpen)
                {
                    RestartGame();
                    timer.Start();
                }
                else if (finish.IsOpen)
                {
                    finish.IsOpen = false;
                }

            }
        }

        /*UI manegment*/
        private void openFinishPopup(string st)
        {
            /*Inser apropriate messege in popup and open it*/

            TextBlock messege = new TextBlock();
            messege.Foreground = new SolidColorBrush(Color.FromArgb(220, 255, 255, 255));
            messege.Text = st;
            messege.TextAlignment = TextAlignment.Center;
            messege.FontSize = 55;
            finish.Child = messege;
            finish.IsOpen = true;
        }
        private void Finish_Closed(object sender, object e)
        {
            RestartGame();
            timer.Start();
        }
        private void setHero(string[] hereoSrcs)
        {
            /*Change for next hero's image and soundtrack */

            heroTheme.Stop();
            HeroPicSrc = hereoSrcs[0];

            heroThemeSrc = hereoSrcs[1];
            heroTheme.Source = new Uri(heroThemeSrc);

            clashSoundSrc = hereoSrcs[2];
            clashSound.Source = new Uri(clashSoundSrc);

            playerRect.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(HeroPicSrc))
            };
            heroTheme.Play();
        }
        private void PlayCongrats()
        {
            /*Plays a congrats video*/
            CanvasElm.Children.Add(congratsVideo);
            EscapeMessege.Visibility = Visibility.Visible;
            congratsVideo.Visibility = Visibility.Visible;
            congratsVideo.Play();
            heroTheme.Pause();
            clashSound.Pause();
            Videotimer.Start();
        }
        private void Videotimer_Tick(object sender, object e)
        {
            /* close the congrats video ==> restart the game*/

            congratsVideo.Stop();
            Videotimer.Stop();
            congratsVideo.Visibility = Visibility.Collapsed;
            EscapeMessege.Visibility = Visibility.Collapsed;

            heroTheme.Play();
            RestartGame();
            openFinishPopup("Well done\nRestart game\n" +
    "Press any key");
        }
        private void ChangeHero_Click(object sender, RoutedEventArgs e)
        {
            SimulatePCControl.SpaceArrow();
        }
        private string[] nextHero()
        {
            /*Returns next hero src in array*/

            string[] toReturn = new string[3];
            string[,] heroes = { { Kunta, KuntaSound, KuntaClashSound }, { Snoop, SnoopSound, SnoopClashSound }, { Eminem, EminemSound, EminemClashSound } };
            currentHeroIndex++;
            if (currentHeroIndex == 3)
            {
                currentHeroIndex = 0;
            }
            for (int i = 0; i < 3; i++)
            {
                toReturn[i] = heroes[currentHeroIndex, i];
            }
            return toReturn;
        }
    }
}