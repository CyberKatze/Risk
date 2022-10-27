using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Risklib;
using System.Windows.Threading;
using System.Media;
using System.Windows.Media.Effects;

namespace Risk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
      
        public  Game game;
        public UI ui;
       
      
        bool soundp = true;
        public MainWindow(Game g)
        {
            
            InitializeComponent();
          
            MainButton.IsEnabled = false;
            media.Source = new Uri(@"sounds/risk_overture.mp3",UriKind.Relative);
            media.Play();
           
            game = g;
            game.map = new Map();
            game.map.LoadByXml();
            game.StartGame();
            foreach(var item in game.Players)
            {
                PlayerList.Items.Add(item);
              
            }
            MainImage.Source = game.ui.GetMap();
            game.Showcard += Game_Showcard;
            game.win += Game_win1;
            game.defeated += Game_defeated1;
            ShowTrun();
            intitialUnitShow();
            status.Text = "شروع";
            UnitCount.Text = game.turn.player.unit.ToString();
            unitcountpanel.Visibility = Visibility.Visible;
        }

        private void Game_defeated1(Player p)
        {
            SetDefeated(p);
            defeatedpanel.Visibility = Visibility.Visible;
        }

        private void Game_win1(Player p)
        {
            SetVictory(p);
            victorypanel.Visibility = Visibility.Visible;
        }

        

        private void Game_Showcard()
        {
            SetCardsBox();
            status.Text = "قراردهی نیرو";
            UnitCount.Text = game.turn.player.unit.ToString();
            unitcountpanel.Visibility = Visibility.Visible;
            
        }

        // events in Mianwindow
        #region

        private void MainGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
           
            double R = 1000 / MainImage.ActualWidth;
            string countryname = game.ui.FindCountryByPoint((int)(e.GetPosition(MainImage).X * R), (int)(e.GetPosition(MainImage).Y * R));//give country name by the positon of mouse
            if (game.turn.Step == step.starting)
            {
                game.turn.player.Deploy(countryname);
                game.Starting();//handle change in turn of player
                UnitCount.Text = game.turn.player.unit.ToString();
            }
            else if (game.turn.Step==step.deploy) { 
            //when player click in country deploy army there if country match the player
            
            game.turn.player.Deploy(countryname);
                UnitCount.Text = game.turn.player.unit.ToString();
                if (game.turn.player.unit == 0)
                    MainButton.IsEnabled = true;
            }
            else if (game.turn.Step == step.attack)
            {
                if (game.attack.SelectedCountry != null)
                {
                    if (game.attack.SetTarget(countryname))//our country is selected check if this country has quality of being target
                    {
                        DiceInfo(game.attack);
                        DiceUiReset();
                        MainImage.Source = game.ui.GetMap();
                        attackpanel.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        game.attack.Attacker = game.turn.player;
                        game.attack.select(countryname);
                        MainImage.Source = game.ui.ShowSelectableCountry(game.attack.AttackableCountry()?.Select(x => x.Name).ToList()??null);
                    }
                }
                else
                {
                    game.attack.Attacker = game.turn.player;
                    game.attack.select(countryname);
                   MainImage.Source= game.ui.ShowSelectableCountry(game.attack.AttackableCountry()?.Select(x=>x.Name).ToList()??null);
                }
            }
            else if (game.turn.Step == step.transfer)
            {
                if (game.transfer.SelectedCountry != null)
                {
                    if (game.transfer.SetTatget(countryname))
                    {

                        SetTransferSlider(game.transfer.currentPlayer.Countries[game.transfer.SelectedCountry], game.transfer.currentPlayer.Countries[game.transfer.TargetCountry], game.transfer.SelectedCountry.Name, game.transfer.TargetCountry.Name);
                        transferpanel.Visibility = Visibility.Visible;
                        transfercancel.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        game.transfer.Select(countryname);
                        MainImage.Source = game.ui.ShowSelectableCountry(game.transfer.TransferableCountry()?.Select(x => x.Name).ToList() ?? null);
                    }
                }
                else
                {
                    game.transfer.currentPlayer = game.turn.player;
                    game.transfer.Select(countryname);
                    MainImage.Source = game.ui.ShowSelectableCountry(game.transfer.TransferableCountry()?.Select(x => x.Name).ToList() ?? null);
                }
            }
            ShowTrun();
            intitialUnitShow();
        }

        private void MainGrid_MouseMove(object sender, MouseEventArgs e)
        {
            double R = 1000/ MainImage.ActualWidth;
            List<string> countriesName=new List<string>();
            if (game.turn.Step == step.deploy && game.turn.player.unit == 0)
            {

            }
            else
            {
                countriesName = game.turn.player.Countries.Keys.Select(x => x.Name).ToList();
            }
            if (game.turn.Step==step.attack&&game.attack.SelectedCountry != null)
            {
                
                countriesName.AddRange(game.attack.AttackableCountry().Select(x => x.Name));
            }
            
            var temp = game.ui.MouseMoveCountry((int)(e.GetPosition(MainImage).X * R), (int)(e.GetPosition(MainImage).Y * R),countriesName);
            ImageOver.Source = temp;
        }

      
       
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            //Responsive Canvas for Labels
            #region
            double consw = MainImage.ActualWidth / MainCanvas.Width;
            double consh = MainImage.ActualHeight / MainCanvas.Height;
            MainCanvas.Width = MainImage.ActualWidth;
            MainCanvas.Height = MainImage.ActualHeight;
           foreach (var item in MainCanvas.Children)
            {
                if (item.GetType() == typeof(Label))


                    Canvas.SetLeft((Label)item, Canvas.GetLeft((Label)item) * consw);
                Canvas.SetTop((Label)item, Canvas.GetTop((Label)item) * consh);
                if (MainGrid.ActualWidth < 800)
                {
                    (item as Label).FontSize = 6;
                }
                if (MainGrid.ActualWidth > 800)
                {
                    (item as Label).FontSize = 12;
                }
            }
            #endregion
        }


  private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }



        private void Music_Click(object sender, RoutedEventArgs e)
        {
            if (soundp)
            {

                media.Pause();
                soundp = false;
                Music.Opacity = .5;
            }
            else
            {
                media.Play();
                soundp = true;
                Music.Opacity = 1;
            }
        
        }
        #endregion
        public void ChangeCountryUnit(string name, int unit)
        {
            var el = this.FindName(name);
            if (el != null)
                (el as Label).Content = unit;
        }

        public void DiceUi(Attack at)
        {

            //set dice in screen
            #region
            int i = 1;
            foreach (var x in game.attack.p1)
            {
                (this.FindName("p1dice" + i) as Image).Source = new BitmapImage(new Uri(@"Dice/w" + x + ".png", UriKind.Relative));
                i++;
            }
            for (; i <= 3; i++)
                (this.FindName("p1dice" + i) as Image).Source = null;
            
            i = 1;
            foreach (var x in game.attack.p2)
            {
                (this.FindName("p2dice" + i) as Image).Source = new BitmapImage(new Uri(@"Dice/b" + x + ".png", UriKind.Relative));
                i++;
            }
            for (; i <= 2; i++)
                (this.FindName("p2dice" + i) as Image).Source = null;
            #endregion

            //minus slodier count in attack
            p1dicecountrycountm.Text = ((at.Attacker.Countries[at.SelectedCountry] - Int32.Parse(p1dicecountrycount.Text))==0)?"":
                (at.Attacker.Countries[at.SelectedCountry] - Int32.Parse(p1dicecountrycount.Text)).ToString();
            if (at.Defender.Countries.Keys.Contains(game.attack.TargetCountry) )
                p2dicecountrycountm.Text = ((at.Defender.Countries[at.TargetCountry] - Int32.Parse(p2dicecountrycount.Text)) == 0) ? "" :
                (at.Defender.Countries[at.TargetCountry] - Int32.Parse(p2dicecountrycount.Text)).ToString();
            else
                p2dicecountrycountm.Text = ((at.Attacker.Countries[at.TargetCountry] - Int32.Parse(p2dicecountrycount.Text)) == 0) ? "" :
                (at.Attacker.Countries[at.TargetCountry] - Int32.Parse(p2dicecountrycount.Text)).ToString();

        }
        public void DiceUiReset()
        {
            int i = 1;
            for (; i <= 3; i++)
                (this.FindName("p1dice" + i) as Image).Source = null;
            i = 1;
            for (; i <= 2; i++)
                (this.FindName("p2dice" + i) as Image).Source = null;
            p1dicecountrycountm.Text = "";
            p2dicecountrycountm.Text = "";
        }

        private void DiceInfo(Attack at)
        {
            var converter = new System.Windows.Media.BrushConverter();
            p1diceimage.Background = new ImageBrush(at.Attacker.Avatar);
            var p1color = (Brush)converter.ConvertFromString(at.Attacker.Color);
            p1diceimage.BorderBrush = p1color;
            p1diceimage.Effect = DropShadow(at.Attacker.Color);
            p1dicename.Text = at.Attacker.Name;
            p1dicename.Foreground = p1color;
            p1dicecountry.Text = at.SelectedCountry.Name;
            p1dicecountrycount.Text = at.Attacker.Countries[at.SelectedCountry].ToString();
            p2diceimage.Background = new ImageBrush(at.Defender.Avatar);
            var p2color = (Brush)converter.ConvertFromString(at.Defender.Color);
            p2diceimage.BorderBrush = p2color;
            p2diceimage.Effect = DropShadow(at.Defender.Color);
            p2dicename.Text = at.Defender.Name;
            p2dicename.Foreground = p2color;
            p2dicecountry.Text = at.TargetCountry.Name;
            p2dicecountrycount.Text = at.Defender.Countries[at.TargetCountry].ToString();
        }
  
        private void intitialUnitShow()
        {
            foreach(Player p in game.Players)
            {
                foreach(Country c in p.Countries.Keys){
                    ChangeCountryUnit(c.Name, p.Countries[c]);
                }
            }
        }

        private void Share_Click(object sender, RoutedEventArgs e)
        {
            
        
        }

        private void continue_Click(object sender, RoutedEventArgs e)
        {
            if (game.attack.Attacker.Countries.Keys.Contains(game.attack.TargetCountry))//if the attacker have target country it means he win and tranfer panel should open
            {
                attackpanel.Visibility = Visibility.Hidden;
                SetTransferSlider(game.attack.Attacker.Countries[game.attack.SelectedCountry], game.attack.Attacker.Countries[game.attack.TargetCountry],game.attack.SelectedCountry.Name,game.attack.TargetCountry.Name);
                transferpanel.Visibility = Visibility.Visible;
                transfercancel.Visibility = Visibility.Hidden;
                game.transfer = new Transfer();
                game.transfer.currentPlayer = game.turn.player;
                game.transfer.Select(game.attack.SelectedCountry.Name);
                game.transfer.SetTatget(game.attack.TargetCountry.Name);
                game.attack.SelectedCountry = null;//make sure that the country is not selected anymore because it has not the condition for attack
            }
            else if (game.attack.Attacker.Countries[game.attack.SelectedCountry]<=1)
            {
                game.attack.SelectedCountry = null;//make sure that the country is not selected anymore because it has not the condition for attack
                attackpanel.Visibility = Visibility.Hidden;
            }
            else
            {
                DiceInfo(game.attack);
                game.attack.Dice();
                DiceUi(game.attack);//it make panel of dice ready 
                MainImage.Source = game.ui.GetMap();
                intitialUnitShow();//unit change in this event so unit in map should update
            }
        }

        private void MainButton_Click(object sender, RoutedEventArgs e)
        {
            if (game.turn.Step == step.deploy)
            {
                unitcountpanel.Visibility = Visibility.Hidden;
                status.Text = "حمله";
                game.turn.Step++;
                game.attack = new Attack(game);//restart last attack data
            }
            else if(game.turn.Step ==step.attack)
                    {
                status.Text = "انتقال";
                game.turn.Step++;
                game.transfer = new Transfer();//restart last transfer data
            }
            else if (game.turn.Step == step.transfer)
            {
                if (game.turn.player.CardDis)
                {
                    Card c = new Card();
                    c = (Card)game.CardDis();
                    SetCardShow(c);
                    cardshowpanel.Visibility = Visibility.Visible;
                }
                
                MainImage.Source = game.ui.GetMap();//restart map for make shor that imageover remove
                game.NextPlayer();
                UnitCount.Text = game.turn.player.unit.ToString();
                unitcountpanel.Visibility = Visibility.Visible;
                SetCardsBox();
                ShowTrun();
                MainButton.IsEnabled = false;
                status.Text = "قراردهی نیرو";
            }
        }

        private void reject_Click(object sender, RoutedEventArgs e)
        {
            attackpanel.Visibility = Visibility.Hidden;
            if (game.attack.Attacker.Countries.Keys.Contains(game.attack.TargetCountry))
            {
                SetTransferSlider(game.attack.Attacker.Countries[game.attack.SelectedCountry],game.attack.Attacker.Countries[game.attack.TargetCountry], game.attack.SelectedCountry.Name, game.attack.TargetCountry.Name);
                transferpanel.Visibility = Visibility.Visible;
                transfercancel.Visibility = Visibility.Hidden;
                game.transfer = new Transfer();
                game.transfer.currentPlayer = game.turn.player;
                game.transfer.Select(game.attack.SelectedCountry.Name);
                game.transfer.SetTatget(game.attack.TargetCountry.Name);
            }
          
                game.attack.SelectedCountry = null;//make sure that the country is not selected anymore because it has not the condition for attack
            
        }

        private void ShowTrun()
        {
            var x = game.Players.IndexOf(game.turn.player);
            var converter = new System.Windows.Media.BrushConverter();
            for(int i = 0; i < 6; i++)
            {
                var tran = (Brush)converter.ConvertFromString("#00000000");
                (this.FindName("p" + i) as Border).Background = tran;
            }
            
            var xc = (Brush)converter.ConvertFromString(game.turn.player.Color);
            (this.FindName("p" + x) as Border).Background =xc;
        }


        //transfer region include : event for sliderchange  ,event accepttransfer , cancel transfer ,  SetTransferSlider , TransferUnit(return),transfercountUI(show)
        #region
        private void transferslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            p1transfercount.Text = (transferslider.Maximum -transferslider.Value+1).ToString();
            TransferCountUI((int)(transferslider.Value));
           
        }


        private void SetTransferSlider(int source,int target,string s,string t)
        {
            transferslider.Maximum = source-1;
            transferslider.Minimum =  1;
            transferslider.Value = 1;

            p1transfername.Text = s;
            p2transfername.Text = t;
            p1transfercount.Text = (source-1).ToString();
            TransferCountUI(1);
            p2transfercount.Text = (target).ToString();

        }
        private int TransferUnit()
        {

            return Int32.Parse(transfercount.Text);
        }

        private void transferaccept_Click(object sender, RoutedEventArgs e)
        {
            game.transfer.Transfering(TransferUnit());
            transferpanel.Visibility = Visibility.Hidden;
            if (game.transfer.currentPlayer.Countries[game.transfer.SelectedCountry] <= 1)
            {
                game.transfer.SelectedCountry = null;//make sure that the country is not selected anymore because it has not the condition for attack
            }
            intitialUnitShow();
        }

        private void transfercancel_Click(object sender, RoutedEventArgs e)
        {
            transferpanel.Visibility = Visibility.Hidden;
        }

        private void TransferCountUI(int x)
        {
            if (x > 0)
                pretransfercount.Text = "+";
            else
                pretransfercount.Text = "";
                
            transfercount.Text = x.ToString();
        }
        #endregion

        //cards region ...
        #region
        private string translatecard(Card c)
        {
            switch (c)
            {
                case Card.Infantry:
                    return "Infantry";
                    
                case Card.Cavalry:
                    return "Cavalry";
                    
                case Card.Artillery:
                    return "Artillery";

                default:
                    return "";
                    
            }
        }

        private Card translatecardreverse(string c)
        {
            switch (c)
            {
                case "Infantry":
                    return Card.Infantry;

                case "Cavalry":
                    return Card.Cavalry;

                case "Artillery":
                    return Card.Artillery;

                default:
                    return Card.Infantry;

            }
        }
        private void SetCardShow(Card c)
        {
            cardshowimage.Source=new BitmapImage(new Uri(@"cards/" + translatecard(c) + ".png", UriKind.Relative));
        }

        private void cardaccept_Click(object sender, RoutedEventArgs e)
        {
            cardshowpanel.Visibility = Visibility.Hidden;
        }
        private void SetCardsBox()
        {
            int a=0, b=0, c=0;
            foreach (Card card in game.turn.player.Cards)
            {
                switch (card)
                {
                    case Card.Infantry:
                        a++;
                        break;
                    case Card.Cavalry:
                        b++;
                        break;
                    case Card.Artillery:
                        c++;
                        break;
                }
            }
                InfantryCount.Text = a.ToString();
            CavalryCount.Text = b.ToString();
            ArtilleryCount.Text = c.ToString();

            }
            private void Card_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Card clickedcard=translatecardreverse((sender as Image).Name);
            if (game.turn.Step == step.deploy)
            {
                switch (clickedcard)
                {
                    case Card.Infantry:
                        if(Int32.Parse( InfantryCount.Text.ToString()) > 0)
                        {
                            game.turn.player.UseCard(clickedcard);
                        }
                        break;
                    case Card.Cavalry:
                        if (Int32.Parse(CavalryCount.Text.ToString()) > 0)
                        {
                            game.turn.player.UseCard(clickedcard);
                        }
                        break;
                    case Card.Artillery:
                        if (Int32.Parse(ArtilleryCount.Text.ToString()) > 0)
                        {
                            game.turn.player.UseCard(clickedcard);
                        }
                        break;
                }
                UnitCount.Text = game.turn.player.unit.ToString();
                SetCardsBox();

            }
            #endregion



        }

        private void SetVictory(Player p)
        {
            var converter = new System.Windows.Media.BrushConverter();
            p1victoryimage.Background = new ImageBrush(p.Avatar);
            var pcolor = (Brush)converter.ConvertFromString(p.Color);
            p1victoryimage.BorderBrush = pcolor;
            p1victoryimage.Effect = DropShadow(p.Color);
            p1victoryname.Text = p.Name;
            p1victoryname.Foreground = pcolor;
            
        }
        private void SetDefeated(Player p)
        {
            var converter = new System.Windows.Media.BrushConverter();
            p1defeatedimage.Background = new ImageBrush(p.Avatar);
            var pcolor = (Brush)converter.ConvertFromString(p.Color);
            p1defeatedimage.BorderBrush = pcolor;
            p1defeatedimage.Effect = DropShadow(p.Color);
           
            p1defeatedname.Text = p.Name;
            p1defeatedname.Foreground = pcolor;
            PlayerList.Items.Remove(p);
        }

        private DropShadowEffect DropShadow(string color)
        {
            var converter = new System.Windows.Media.BrushConverter();
            var pcolor = (Brush)converter.ConvertFromString(color);
            DropShadowEffect DS = new DropShadowEffect();
            DS.ShadowDepth=3;
            DS.BlurRadius = 3;
            DS.Color = (pcolor as SolidColorBrush).Color;
            return DS;
        }
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).Visibility = Visibility.Hidden;
        }

        private void media_MediaEnded(object sender, RoutedEventArgs e)
        {
            if(soundp)
            {

            media.Position = TimeSpan.Zero;
            media.Play();
            }
        }
    }
}