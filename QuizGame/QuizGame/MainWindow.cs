﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
        private SoundPlayer _falseSound = new SoundPlayer();
        private int _collectCount;
        private int _falseCount;
        private bool _isCurrentQuizAnswered;

        public MainWindow()
        {
            InitializeComponent();
            MakeQuizList();
            BindRadioButtons();
            ShowNextQuiz();

            //GetQuizFromFile();
            GetQuizStringFromGoogleSpreadSheetWithCSV();

            InitSounds();
            RefreshStatusText();
        }

        private void RefreshStatusText()
        {
            PlayStatus.Text = $"남음: {_quizList.Count - _collectCount - _falseCount}\r\n맞음: {_collectCount}\r\n틀림: {_falseCount}";
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
            //초기화
            _isCurrentQuizAnswered = false;

            foreach (var radioButton in radioButtons)
            {
                radioButton.IsChecked = false;
            }
            Description.Text = "";
            ResetAccentAnswer();
            HideLinkButton();

            //다풀었음
            if (_currentQuizIndex >= _quizList.Count)
            {
                _completeSound.Play();
                Description.Text = $"다 풀었습니다. 총 {_quizList.Count} 문제중 {_collectCount} 문제를 풀었습니다!!! 수고하셨어요!";
            }
            //다음문제
            else
            {
                _currentQuizIndex++;
                _currentQuiz = _quizList[_currentQuizIndex];
                RefreshAnswers(_currentQuiz);
            }

            RefreshStatusText();
        }

        private void RefreshAnswers(Quiz quiz)
        {
            QuizTitle.Text = quiz.Question;
            QuizNumber.Text = $"No.{_currentQuizIndex}";
            RadioAnswer1.Content = quiz.Answers[0].AnswerString;
            RadioAnswer2.Content = quiz.Answers[1].AnswerString;
            RadioAnswer3.Content = quiz.Answers[2].AnswerString;
            RadioAnswer4.Content = quiz.Answers[3].AnswerString;
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
                var link = line.Value["Link1"].ToString();
                var newQuiz = new Quiz(title, question, answerNumber, new List<string>() { answer1, answer2, answer3, answer4 }, link);
                quizs.Add(newQuiz);
            }
        }

        private void MakeQuizList()
        {
            _quizList = GetQuizFromCSV();
            _quizList.Shuffle();
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
                if (!_isCurrentQuizAnswered)
                {
                    _collectCount++;
                    _isCurrentQuizAnswered = true;
                }
                _correctSound.Play();
                ShowNextQuiz();
            }
            else
            {
                if (!_isCurrentQuizAnswered)
                {
                    _falseCount++;
                    _isCurrentQuizAnswered = true;
                }

                _falseSound.Play();
                Description.Text = $"틀렸습니다. ㅠㅠ ";
                AccentAnswer(currentQuiz.GetAnswerNumber());
                ShowLinkButton();
            }
        }

        private void ShowLinkButton()
        {
            if (string.IsNullOrEmpty(_currentQuiz.Link))
            {
                return;
            }

            HintButton.Visibility = Visibility.Visible;
            HintButton.Content = _currentQuiz.Link;
        }

        private void HideLinkButton()
        {
            HintButton.Visibility = Visibility.Hidden;
        }

        private void AccentAnswer(int getAnswerNumber)
        {
            switch (getAnswerNumber)
            {
                case 1:
                    RadioAnswer1.Foreground = Brushes.Red;
                    break;

                case 2:
                    RadioAnswer2.Foreground = Brushes.Red;
                    break;

                case 3:
                    RadioAnswer3.Foreground = Brushes.Red;
                    break;

                case 4:
                    RadioAnswer4.Foreground = Brushes.Red;
                    break;

                default:
                    throw new Exception();
            }
        }

        private void ResetAccentAnswer()
        {
            RadioAnswer1.Foreground = Brushes.Black;
            RadioAnswer2.Foreground = Brushes.Black;
            RadioAnswer3.Foreground = Brushes.Black;
            RadioAnswer4.Foreground = Brushes.Black;
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

            FileStream falseSoundStream = File.Open(@"false.wav", FileMode.Open);
            _falseSound = new SoundPlayer(falseSoundStream);
            _falseSound.Load();
        }

        private void HintButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.Assert(!string.IsNullOrEmpty(_currentQuiz.Link));
            System.Diagnostics.Process.Start($"{_currentQuiz.Link}");
        }
    }
}