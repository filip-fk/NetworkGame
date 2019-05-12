using QuizGame.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using QuizGame.Model;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Runtime.Serialization;
using Windows.UI;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Text;
using System.Threading.Tasks;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace QuizGame
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewQuestionsPage : Page
    {
        public GameViewModel ViewModel { get; set; }

        public NewQuestionsPage()
        {
            InitializeComponent();

            ApplicationViewTitleBar formattableTitleBar = ApplicationView.GetForCurrentView().TitleBar;
            formattableTitleBar.ButtonBackgroundColor = Colors.Transparent;
            formattableTitleBar.ButtonForegroundColor = Colors.Black;
            formattableTitleBar.BackgroundColor = Colors.Transparent;
            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
        }

        private async void textChangedEventHandler(object sender, TextChangedEventArgs e)
        {
            int j = Convert.ToInt32(time_k.Text);
            for (int i = 0; j > i; i++)
            {
                await Task.Delay(1000);

                j--;
                time_k.Text = j.ToString();

                if (j == 0)
                {
                    await ViewModel.NextQuestionCommand.Execute();
                    //time_k.Text = (ViewModel.CurrentQuestion.Time).ToString();
                }
            }
        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            await ViewModel.StopListeningAsync();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel = new GameViewModel();

            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame != null && rootFrame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }
        }

        private List<Question> Questions2 { get; set; } = new List<Question>()
            {
                new Question
                {
                    Text = "When should a teenager go to bed4?",
                    Options = new List<string> { "21:30", "22:00", "23:00", "Idk" },
                    CorrectAnswerIndex = 3,
                    Time = 20
                },
                new Question
                {
                    Text = "How many days in 2018",
                    Options = new List<string>
                    {
                        "365",
                        "364",
                        "356",
                        "360"
                    },
                    CorrectAnswerIndex = 1,
                    Time=20
                },
                new Question
                {
                    Text = "15+3x4=",
                    Options = new List<string> { "72", "27", "26", "36" },
                    CorrectAnswerIndex = 2,
                    Time=25
                },
                new Question
                {
                    Text = "What is the probability of rolling a 6 on a regular dice?",
                    Options = new List<string>
                    {
                        "20%",
                        "6%",
                        "7%",
                        "16.7%"
                    },
                    CorrectAnswerIndex = 4,
                    Time=20
                }
            };

        private async void SaveTuples(List<Question> Question_l, string name_100)
        {
            StorageFile userdetailsfile = await ApplicationData.Current.LocalFolder.CreateFileAsync
                (name_100, CreationCollisionOption.GenerateUniqueName);
            name_to_pass = userdetailsfile.Name;
            IRandomAccessStream raStream = await userdetailsfile.OpenAsync(FileAccessMode.ReadWrite);

            using (IOutputStream outStream = raStream.GetOutputStreamAt(0))
            {

                // Serialize the Session State. 

                DataContractSerializer serializer = new DataContractSerializer(typeof(List<Question>));

                serializer.WriteObject(outStream.AsStreamForWrite(), Question_l);

                await outStream.FlushAsync();
                outStream.Dispose();
                raStream.Dispose();
            }
        }

        private string name_to_pass { get; set; }

        private async void Create_Click(object sender, RoutedEventArgs e)
        {
            //string se = (sender as Button).Content as string;
            //if (se == "Play game")
            //{
                //sets new game
                if (name_b.Text != "")
                {
                    name_b.IsReadOnly = true;
                    SaveTuples(Questions3, name_b.Text);
                    Create.IsEnabled = false;
                    pbr_.Maximum = 1000;
                    for (int i = 0; i < pbr_.Maximum; i++)
                    {
                        await Task.Delay(1);
                        pbr_.Value++;
                    }
                    ViewModel.fileName = name_to_pass;
                    await ViewModel.CreateGameCommand.Execute();
                }
                else
                {
                    name_b.PlaceholderText = "No name given!";
                    name_b.PlaceholderForeground = new SolidColorBrush(Colors.Red);
                }
            //}
            //else
            //{
                //loads previous
            //    name_to_pass = name_b.Text;
            //    ViewModel.fileName = name_to_pass;
            //    await ViewModel.CreateGameCommand.Execute();
            //}
        }

        private async void StartGame_Click(object sender, RoutedEventArgs e)
        {
            pbr_.Value = 0;
            pbr_.Maximum = 1000;
            Start.IsEnabled = false;
            for (int i = 0; i < pbr_.Maximum; i++)
            {
                await Task.Delay(1);
                pbr_.Value++;
            }
            new_ques_.Visibility = Visibility.Collapsed;
            new_ques_f.Visibility = Visibility.Collapsed;
            await ViewModel.StartGameCommand.Execute();
            //string pgName = "newQuesPage";
            //this.Frame.Navigate(typeof(GamePage), pgName);
        }

        private void name_b_TextChanged(object sender, TextChangedEventArgs e)
        {
            name_b.PlaceholderForeground = new SolidColorBrush(Colors.LightGray);
            name_b.PlaceholderText = "Name";
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void add_q_Click(object sender, RoutedEventArgs e)
        {
            create_ques("", "", "", "", "", 20, 0, "");
        }

        private Boolean sel_item(int seq, int pset)
        {
            if (seq == pset)
                return true;
            else return false;
        }
        
        private void create_ques(string qm, string pm1, string pm2, string pm3, string pm4, int tmm, int sm, string gn)
        {
            int num_ = main10.Children.Count();
            if (num_ == 0) { num_ = 1; }
            /*
            else if (num_ == 1) { num_ = 1; }
            else if (num_ == 2) { num_ = 2; }
            else if (num_ == 3) { num_ = 3; }
            else if (num_ == 4) num_--;
            else num_--;*/
            var seti = new UISettings();
            var accent = seti.GetColorValue(UIColorType.Accent);
            var accentL3 = seti.GetColorValue(UIColorType.AccentLight3);

            Grid main0 = new Grid { Name = "ques10_", Margin = new Thickness(30, 30, 30, 0), BorderBrush = /*Application.Current.Resources["SystemAccentColor"] as SolidColorBrush*/ new SolidColorBrush(accent), BorderThickness = new Thickness(0, 0, 0, 2), Padding = new Thickness(0, 0, 0, 20) };

            Grid main1_ = new Grid { Name = "mainQ_", Margin = new Thickness(15, 15, 70, 15), VerticalAlignment = VerticalAlignment.Top };
            TextBlock q1_ = new TextBlock { FontSize = 20, Text = "Q. " + $"{num_}" + ":", FontWeight = FontWeights.ExtraBold, VerticalAlignment = VerticalAlignment.Center };
            TextBox q1_1 = new TextBox { Name = "ques_", FontSize = 23, Text = qm, PlaceholderText = "Question", Margin = new Thickness(70, 0, 0, 0), MinWidth = 200, BorderBrush = new SolidColorBrush(Colors.Transparent) };
            main1_.Children.Add(q1_); main1_.Children.Add(q1_1);

            Grid main2_ = new Grid { Name = "mainA_", Margin = new Thickness(40, 70, 70, 0), VerticalAlignment = VerticalAlignment.Top };
            TextBlock tb_ = new TextBlock { Text = "Possible answers:", FontWeight = FontWeights.Bold, FontSize = 20 };
            main2_.Children.Add(tb_);

            //add q1
            Grid sub1_ = new Grid { Name = "opt1_", Margin = new Thickness(0, 40, 0, 0), VerticalAlignment = VerticalAlignment.Top };
            RadioButton p1_ = new RadioButton { IsChecked = sel_item(1, sm), Name = "opt1_2", FontSize = 20, Content = "", VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Left, GroupName = q1_.Text };
            TextBox p1_1 = new TextBox { Text = pm1, Name = "opt1_1", Margin = new Thickness(30, 0, 0, 0), PlaceholderText = "Answer 1", FontSize = 23, BorderBrush = new SolidColorBrush(Colors.Transparent) };
            sub1_.Children.Add(p1_); sub1_.Children.Add(p1_1); main2_.Children.Add(sub1_);
            //add q2
            Grid sub2_ = new Grid { Name = "opt2_", Margin = new Thickness(0, 85, 0, 0), VerticalAlignment = VerticalAlignment.Top };
            RadioButton p2_ = new RadioButton { IsChecked = sel_item(2, sm), Name = "opt2_2", FontSize = 20, Content = "", VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Left, GroupName = q1_.Text };
            TextBox p2_1 = new TextBox { Text = pm2, Name = "opt2_1", Margin = new Thickness(30, 0, 0, 0), PlaceholderText = "Answer 2", FontSize = 23, BorderBrush = new SolidColorBrush(Colors.Transparent) };
            sub2_.Children.Add(p2_); sub2_.Children.Add(p2_1); main2_.Children.Add(sub2_);
            //add q3
            Grid sub3_ = new Grid { Name = "opt3_", Margin = new Thickness(0, 130, 0, 0), VerticalAlignment = VerticalAlignment.Top };
            RadioButton p3_ = new RadioButton { IsChecked = sel_item(3, sm), Name = "opt3_2", FontSize = 20, Content = "", VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Left, GroupName = q1_.Text };
            TextBox p3_1 = new TextBox { Text = pm3, Name = "opt3_1", Margin = new Thickness(30, 0, 0, 0), PlaceholderText = "Answer 3", FontSize = 23, BorderBrush = new SolidColorBrush(Colors.Transparent) };
            sub3_.Children.Add(p3_); sub3_.Children.Add(p3_1); main2_.Children.Add(sub3_);
            //add q4
            Grid sub4_ = new Grid { Name = "opt4_", Margin = new Thickness(0, 175, 0, 0), VerticalAlignment = VerticalAlignment.Top };
            RadioButton p4_ = new RadioButton { IsChecked = sel_item(4, sm), Name = "opt4_2", FontSize = 20, Content = "", VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Left, GroupName = q1_.Text };
            TextBox p4_1 = new TextBox { Text = pm4, Name = "opt4_1", Margin = new Thickness(30, 0, 0, 0), PlaceholderText = "Answer 4", FontSize = 23, BorderBrush = new SolidColorBrush(Colors.Transparent) };
            sub4_.Children.Add(p4_); sub4_.Children.Add(p4_1); main2_.Children.Add(sub4_);

            Grid main3_ = new Grid { Name = "mainT_", HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Stretch };
            RowDefinition rd1 = new RowDefinition { Height = new GridLength(1, GridUnitType.Star) };
            RowDefinition rd2 = new RowDefinition { Height = new GridLength(1, GridUnitType.Star) };
            RowDefinition rd3 = new RowDefinition { Height = new GridLength(1, GridUnitType.Star) };
            main3_.RowDefinitions.Add(rd1); main3_.RowDefinitions.Add(rd2); main3_.RowDefinitions.Add(rd3);

            Button b_done = new Button { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Content = "\xE73e", FontFamily = new FontFamily("segoe mdl2 assets"), Background = new SolidColorBrush(Colors.Transparent), FontSize = 25, Foreground = new SolidColorBrush(Colors.Green) };
            Button b_dele = new Button { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Content = "\xE74d", FontFamily = new FontFamily("segoe mdl2 assets"), Background = new SolidColorBrush(Colors.Transparent), FontSize = 25, Foreground = new SolidColorBrush(Colors.Red) };
            TextBox tbx_t = new TextBox { Text = tmm.ToString(), Name = "times_", HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Center, PlaceholderText = "Time", FontSize = 25, BorderBrush = new SolidColorBrush(Colors.Transparent), Background = new SolidColorBrush(Colors.Transparent) };
            b_dele.Click += B_dele_Click;
            b_done.Click += B_done_Click;
            main3_.Children.Add(b_done); main3_.Children.Add(b_dele); main3_.Children.Add(tbx_t); Grid.SetRow(b_done, 0); Grid.SetRow(b_dele, 1); Grid.SetRow(tbx_t, 2);

            main0.Children.Add(main1_); main0.Children.Add(main2_); main0.Children.Add(main3_);

            Grid add_q = new Grid { Margin = new Thickness(30, 30, 0, 0) };
            Button btn_a = new Button { Background = new SolidColorBrush(Color.FromArgb(9, 0, 0, 0)), HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Content = "+", FontSize = 50 };
            btn_a.Click += add_q_Click;
            add_q.Children.Add(btn_a);

            try
            {
                main10.Children.RemoveAt(main10.Children.Count() - 1);
            }
            catch { }

            main10.Children.Add(main0); main10.Children.Add(add_q);

            if (name_b.Text == "")
            {
                name_b.Text = gn;
            }
        }

        private List<Question> Questions3 { get; set; } = new List<Question>()
        {

        };

        private void B_done_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            btn.Content = "\xE72c";
            Grid main0 = (btn.Parent as Grid).Parent as Grid;
            main0.Background = new SolidColorBrush(Color.FromArgb(50, 91, 255, 124));
            main0.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 19, 196, 60));

            string txt = "No question assigned";
            string op1 = "Option 1 is empty";
            string op2 = "Option 2 is empty";
            string op3 = "Option 3 is empty";
            string op4 = "Option 4 is empty";
            int cAn = 1;
            int tm = 20;

            foreach (object g1 in main0.Children)
            {
                if (g1.GetType() == typeof(Grid))
                {
                    if (main0.Name == "ques10_")
                    {
                        if ((g1 as Grid).Name == "mainQ_")
                        {
                            Grid main1 = g1 as Grid;

                            foreach (object obj in main1.Children)
                            {
                                if (obj.GetType() == typeof(TextBox))
                                {
                                    if ((obj as TextBox).Name == "ques_")
                                    {
                                        txt = (obj as TextBox).Text;
                                    }
                                }
                            }
                        }
                        else if ((g1 as Grid).Name == "mainA_")
                        {
                            Grid main2 = g1 as Grid;

                            foreach (object g2 in main2.Children)
                            {
                                if (g2.GetType() == typeof(Grid))
                                {
                                    if ((g2 as Grid).Name == "opt1_")
                                    {
                                        Grid opg1 = g2 as Grid;

                                        foreach (object tbx in opg1.Children)
                                        {
                                            if (tbx.GetType() == typeof(TextBox))
                                            {
                                                if ((tbx as TextBox).Name == "opt1_1")
                                                {
                                                    op1 = (tbx as TextBox).Text;
                                                }
                                            }
                                            else if (tbx.GetType() == typeof(RadioButton))
                                            {
                                                if ((tbx as RadioButton).Name == "opt1_2")
                                                {
                                                    bool? b1 = (tbx as RadioButton).IsChecked;
                                                    if (b1.HasValue)
                                                    {
                                                        if ((bool)b1)
                                                        {
                                                            cAn = 1;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    else if ((g2 as Grid).Name == "opt2_")
                                    {
                                        Grid opg1 = g2 as Grid;

                                        foreach (object tbx in opg1.Children)
                                        {
                                            if (tbx.GetType() == typeof(TextBox))
                                            {
                                                if ((tbx as TextBox).Name == "opt2_1")
                                                {
                                                    op2 = (tbx as TextBox).Text;
                                                }
                                            }
                                            else if (tbx.GetType() == typeof(RadioButton))
                                            {

                                                if ((tbx as RadioButton).Name == "opt2_2")
                                                {
                                                    bool? b1 = (tbx as RadioButton).IsChecked;
                                                    if (b1.HasValue)
                                                    {
                                                        if ((bool)b1)
                                                        {
                                                            cAn = 2;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    else if ((g2 as Grid).Name == "opt3_")
                                    {
                                        Grid opg1 = g2 as Grid;

                                        foreach (object tbx in opg1.Children)
                                        {
                                            if (tbx.GetType() == typeof(TextBox))
                                            {
                                                if ((tbx as TextBox).Name == "opt3_1")
                                                {
                                                    op3 = (tbx as TextBox).Text;
                                                }
                                            }
                                            else if (tbx.GetType() == typeof(RadioButton))
                                            {

                                                if ((tbx as RadioButton).Name == "opt3_2")
                                                {
                                                    bool? b1 = (tbx as RadioButton).IsChecked;
                                                    if (b1.HasValue)
                                                    {
                                                        if ((bool)b1)
                                                        {
                                                            cAn = 3;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    else if ((g2 as Grid).Name == "opt4_")
                                    {
                                        Grid opg1 = g2 as Grid;

                                        foreach (object tbx in opg1.Children)
                                        {
                                            if (tbx.GetType() == typeof(TextBox))
                                            {
                                                if ((tbx as TextBox).Name == "opt4_1")
                                                {
                                                    op4 = (tbx as TextBox).Text;
                                                }
                                            }
                                            else if (tbx.GetType() == typeof(RadioButton))
                                            {
                                                if ((tbx as RadioButton).Name == "opt4_2")
                                                {
                                                    bool? b1 = (tbx as RadioButton).IsChecked;
                                                    if (b1.HasValue)
                                                    {
                                                        if ((bool)b1)
                                                        {
                                                            cAn = 4;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if ((g1 as Grid).Name == "mainT_")
                        {
                            Grid main3 = g1 as Grid;

                            foreach (object obj in main3.Children)
                            {
                                if (obj.GetType() == typeof(TextBox))
                                {
                                    if ((obj as TextBox).Name == "times_")
                                    {
                                        if ((obj as TextBox).Text != "")
                                        {
                                            tm = Convert.ToInt32((obj as TextBox).Text);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Questions3.Add(new Question { Text = txt,Name=name_b.Text, Options = new List<string> { op1, op2, op3, op4 }, CorrectAnswerIndex = cAn, Time = tm });
        }

        private void B_dele_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            main10.Children.Remove((btn.Parent as Grid).Parent as Grid);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private List<Question> Questions10
        {
            get;
            set;
        } = new List<Question>();

        private List<Question> Questions101
        {
            get;
            set;
        } = new List<Question>();

        private async void read_file()
        {
            var Serializer = new DataContractSerializer(typeof(List<Question>));/*
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(name_1000))
            {
                Questions10 = (List<Question>)Serializer.ReadObject(stream);
            }*/
               
            IReadOnlyList<StorageFile> fileList = await ApplicationData.Current.LocalFolder.GetFilesAsync();
            foreach (StorageFile file in fileList)
            {
                using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(file.Name))
                {
                    Questions10 = (List<Question>)Serializer.ReadObject(stream);

                    Question q = Questions10.First();
                    if (file.Name != "!45!random_check_NEW_no_double!912!")
                    {
                        names_a.Items.Add(q.Name);
                    }
                    else { names_a.Items.Add("Cteate new"); }
                }
            }
        }

        private async void save_empty(List<Question> Question_l, string name_100)
        {
            StorageFile userdetailsfile = await ApplicationData.Current.LocalFolder.CreateFileAsync
                (name_100, CreationCollisionOption.ReplaceExisting);
            name_to_pass = userdetailsfile.Name;
            IRandomAccessStream raStream = await userdetailsfile.OpenAsync(FileAccessMode.ReadWrite);

            using (IOutputStream outStream = raStream.GetOutputStreamAt(0))
            {

                // Serialize the Session State. 

                DataContractSerializer serializer = new DataContractSerializer(typeof(List<Question>));

                serializer.WriteObject(outStream.AsStreamForWrite(), Question_l);

                await outStream.FlushAsync();
                outStream.Dispose();
                raStream.Dispose();
            }
        }

        private void Load_games_Click(object sender, RoutedEventArgs e)
        {
            notify.Visibility = Visibility.Visible;
            Questions101.Add(new Question { Text = "", Time = 20, CorrectAnswerIndex = 1, Name = "", Options = new List<string> { "", "", "", "" } });
            save_empty(Questions101, "!45!random_check_NEW_no_double!912!");

            read_file();
        }

        private async void names_a_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            main10.Children.Clear();
            string text100 = (sender as ListView).SelectedItem as string;
            if(text100== "Cteate new")
            { text100 = "!45!random_check_NEW_no_double!912!"; }
            var Serializer = new DataContractSerializer(typeof(List<Question>));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(text100))
            {
                Questions10 = (List<Question>)Serializer.ReadObject(stream);
                read_q10();
            }
            notify.Visibility = Visibility.Collapsed;
            Start.Content = "Play game";
        }

        private void read_q10()
        {
            foreach(Question q in Questions10)
            {
                create_ques(q.Text, q.Options[0], q.Options[1], q.Options[2], q.Options[3], q.Time, q.CorrectAnswerIndex, q.Name);
            }
        }

        private async void next_ques_click(object sender, RoutedEventArgs e)
        {
            await ViewModel.NextQuestionCommand.Execute();
            time_k.Text = ViewModel.CurrentQuestion.Time.ToString();
        }
    }
}