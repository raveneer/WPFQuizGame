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

            if (_currentQuizIndex == _quizList.Count - 1)
            {
                MessageBox.Show("다 풀었습니다.");
            }
            else
            {
                _currentQuiz = _quizList[_currentQuizIndex];

                QuizTitle.Text = _currentQuiz.Title;
                RadioAnswer1.Content = _currentQuiz.Answers[0];
                RadioAnswer2.Content = _currentQuiz.Answers[1];
                RadioAnswer3.Content = _currentQuiz.Answers[2];
                RadioAnswer4.Content = _currentQuiz.Answers[3];

                _currentQuizIndex++;
            }
        }

        private void MakeQuizList()
        {
            _quizList.Add(new Quiz("1+1 =? ", 1, new List<string>() { "2", "3", "4", "5" }));
            _quizList.Add(new Quiz("2+3 =? ", 2, new List<string>() { "4", "5", "6", "코와붕가!" }));
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
        }
    }

    public class Quiz
    {
        public string Title { get; }
        public int AnswerNumber { get; }
        public List<string> Answers = new List<string>();

        public Quiz(string title, int answerNumber, List<string> answers)
        {
            Title = title;
            AnswerNumber = answerNumber;
            Answers = answers;
        }
    }
}