using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace QuizGame
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Quiz> _quizList = new List<Quiz>();
        private Quiz _currentQuiz;
        private int _currentQuizIndex;
        private List<RadioButton> radioButtons = new List<RadioButton>();
        private SoundPlayer _correctSound = new SoundPlayer();
        private SoundPlayer _completeSound = new SoundPlayer();
        private int _collectCount;

        public MainWindow()
        {
            InitializeComponent();
            MakeQuizList();
            BindRadioButtons();
            ShowNextQuiz();

            //GetQuizFromFile();
            GetQuizStringFromGoogleSpreadSheetWithCSV();

            InitSounds();
        }

        private void BindRadioButtons()
        {
            radioButtons.Add(RadioAnswer1);
            radioButtons.Add(RadioAnswer2);
            radioButtons.Add(RadioAnswer3);
            radioButtons.Add(RadioAnswer4);
        }

        private void ShowNextQuiz()
        {
            foreach (var radioButton in radioButtons)
            {
                radioButton.IsChecked = false;
            }

            if (_currentQuizIndex >= _quizList.Count)
            {
                _completeSound.Play();
                MessageBox.Show($"다 풀었습니다. 총 {_quizList.Count} 문제중 {_collectCount} 문제를 풀었습니다!!! 수고하셨어요!");
            }
            else
            {
                _currentQuiz = _quizList[_currentQuizIndex];

                QuizTitle.Text = _currentQuiz.Question;
                RadioAnswer1.Content = _currentQuiz.Answers[0].AnswerString;
                RadioAnswer2.Content = _currentQuiz.Answers[1].AnswerString;
                RadioAnswer3.Content = _currentQuiz.Answers[2].AnswerString;
                RadioAnswer4.Content = _currentQuiz.Answers[3].AnswerString;

                _currentQuizIndex++;
            }
        }

        private void MakeQuizDic(List<Quiz> quizs, Dictionary<string, Dictionary<string, object>> dic)
        {
            foreach (var line in dic)
            {
                var title = line.Key;
                var question = line.Value["Question"].ToString();
                var answerNumber = int.Parse(line.Value["CollectAnswer"].ToString());
                var answer1 = line.Value["Answer1"].ToString();
                var answer2 = line.Value["Answer2"].ToString();
                var answer3 = line.Value["Answer3"].ToString();
                var answer4 = line.Value["Answer4"].ToString();
                var newQuiz = new Quiz(title, question, answerNumber, new List<string>() { answer1, answer2, answer3, answer4 });
                quizs.Add(newQuiz);
            }
        }

        private void MakeQuizList()
        {
            _quizList = GetQuizFromCSV();
        }

        private void NextQuizButton_Click(object sender, RoutedEventArgs e)
        {
            ShowNextQuiz();
        }

        private void RadioAnswer1_Checked(object sender, RoutedEventArgs e)
        {
            RadioAnswer1.IsChecked = true;
            CheckQuiz(_currentQuiz, 0);
        }

        private void RadioAnswer2_Checked(object sender, RoutedEventArgs e)
        {
            RadioAnswer2.IsChecked = true;
            CheckQuiz(_currentQuiz, 1);
        }

        private void RadioAnswer3_Checked(object sender, RoutedEventArgs e)
        {
            RadioAnswer3.IsChecked = true;
            CheckQuiz(_currentQuiz, 2);
        }

        private void RadioAnswer4_Checked(object sender, RoutedEventArgs e)
        {
            RadioAnswer4.IsChecked = true;
            CheckQuiz(_currentQuiz, 3);
        }

        private void CheckQuiz(Quiz currentQuiz, int i)
        {
            if (currentQuiz.Check(i))
            {
                _correctSound.Play();
                _collectCount++;
            }
            else
            {
                MessageBox.Show($"틀렸습니다. ㅠㅠ \r\n 정답은 {currentQuiz.GetAnswerNumber()} 입니다");
            }
            ShowNextQuiz();
        }

        private string GetQuizStringFromGoogleSpreadSheetWithCSV()
        {
            string url = "https://docs.google.com/spreadsheets/d/e/2PACX-1vSd_w5ZOlUaNOWKVG7vNDGOrUMS5VdoUm_XMIh-cp0tFS4cSV-3UUl2NDB6pQh74V03rlTpMaLeOTpH/pub?output=csv";
            WebClient webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            string csv = webClient.DownloadString(url);
            Console.WriteLine(csv);
            return csv;
        }

        private List<Quiz> GetQuizFromCSV()
        {
            var quizs = new List<Quiz>();
            var reader = new CSVReader();
            var scvString = GetQuizStringFromGoogleSpreadSheetWithCSV();
            var dic = reader.ReadToDicFromString(scvString);
            MakeQuizDic(quizs, dic);
            return quizs;
        }

        private void InitSounds()
        {
            FileStream collectSoundStream = File.Open(@"collect.wav", FileMode.Open);
            _correctSound = new SoundPlayer(collectSoundStream);
            _correctSound.Load();

            FileStream completeSoundStream = File.Open(@"complete.wav", FileMode.Open);
            _completeSound = new SoundPlayer(completeSoundStream);
            _completeSound.Load();
        }
    }
}