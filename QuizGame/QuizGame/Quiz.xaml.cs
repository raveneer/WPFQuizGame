using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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
using Newtonsoft.Json;

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

        public MainWindow()
        {
            InitializeComponent();
            MakeQuizList();
            BindRadioButtons();
            ShowNextQuiz();
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
                MessageBox.Show("다 풀었습니다.");
            }
            else
            {
                _currentQuiz = _quizList[_currentQuizIndex];

                QuizTitle.Text = _currentQuiz.Question;
                RadioAnswer1.Content = _currentQuiz.Answers[0];
                RadioAnswer2.Content = _currentQuiz.Answers[1];
                RadioAnswer3.Content = _currentQuiz.Answers[2];
                RadioAnswer4.Content = _currentQuiz.Answers[3];

                _currentQuizIndex++;
            }
        }

        private void GetQuizFromGoogleSpreadSheet()
        {
            string url = "https://spreadsheets.google.com/feeds/list/1Ex3SLxf_wYMNum9hzJmV_XcfQP4U4t3DYYT8qhKl0Jk/od6/public/values?alt=json";
            WebClient webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            string json = webClient.DownloadString(url);
            Console.WriteLine(json);
        }

        private List<Quiz> GetQuizFromCSV()
        {
            var quizs = new List<Quiz>();
            var reader = new CSVReader();
            var dic = reader.ReadToDic("quiz");
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
            return quizs;
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
            CheckQuiz(_currentQuiz, 1);
        }

        private void RadioAnswer2_Checked(object sender, RoutedEventArgs e)
        {
            RadioAnswer2.IsChecked = true;
            CheckQuiz(_currentQuiz, 2);
        }

        private void RadioAnswer3_Checked(object sender, RoutedEventArgs e)
        {
            RadioAnswer3.IsChecked = true;
            CheckQuiz(_currentQuiz, 3);
        }

        private void RadioAnswer4_Checked(object sender, RoutedEventArgs e)
        {
            RadioAnswer4.IsChecked = true;
            CheckQuiz(_currentQuiz, 4);
        }

        private void CheckQuiz(Quiz currentQuiz, int i)
        {
            if (currentQuiz.AnswerNumber == i)
            {
                MessageBox.Show("맞았습니다! ^_^");
            }
            else
            {
                MessageBox.Show("틀렸습니다. ㅠㅠ");
            }
            ShowNextQuiz();
        }
    }

    public class Quiz
    {
        public string Title { get; }
        public string Question { get; }
        public int AnswerNumber { get; }
        public List<string> Answers = new List<string>();

        public Quiz(string title, string question, int answerNumber, List<string> answers)
        {
            Title = title;
            Question = question;
            AnswerNumber = answerNumber;
            Answers = answers;
        }
    }
}