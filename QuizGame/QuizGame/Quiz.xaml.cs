using System.Collections.Generic;
using System.Linq;

namespace QuizGame
{
    public class Quiz
    {
        public string Title { get; }
        public string Question { get; }
        public List<Answer> Answers = new List<Answer>();
        private int _collectCount;

        public Quiz(string title, string question, int answerNumber, List<string> answerString)
        {
            Title = title;
            Question = question;

            for (int i = 0; i < answerString.Count; i++)
            {
                var newAnswer = new Answer(i, answerString[i], i + 1 == answerNumber);
                Answers.Add(newAnswer);
            }
            Answers.Shuffle();
        }

        public int GetAnswerNumber()
        {
            return Answers.IndexOf(Answers.First(x => x.IsCorrect)) + 1;
        }

        public bool Check(int i)
        {
            return Answers[i].IsCorrect;
        }
    }

    public class Answer
    {
        public bool IsCorrect;
        public string AnswerString;
        public int AnswerNumber;

        public Answer(int answerNumber, string answerString, bool isCorrect)
        {
            AnswerNumber = answerNumber;
            AnswerString = answerString;
            IsCorrect = isCorrect;
        }
    }
}