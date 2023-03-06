using AutoTestBot.Models;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace AutoTestBot.Services
{
    class QuestionService
    {
        private List<QuestionModel> _questions;
        private readonly ITelegramBotClient _bot;
        public const int TicketQuestionsCount = 5;

        public int QuestionsCount
        {
            get
            {
                return _questions.Count;
            }
        }

        public int TicketsCount
        {
            get
            {
                return QuestionsCount / TicketQuestionsCount;
            }
        }

        public QuestionService(ITelegramBotClient bot)
        {
            var json = File.ReadAllText("uzlotin.json");
            _questions = JsonConvert.DeserializeObject<List<QuestionModel>>(json)!;
            _bot = bot;
        }

        public Ticket CreateTicket()
        {
            var random = new Random();
            var ticket = random.Next(0, TicketsCount);

            var startQuestionIndex = ticket * TicketQuestionsCount;

            return new Ticket()
            {
                Index = ticket,
                QuestionsCount = TicketQuestionsCount,
                StartIndex = startQuestionIndex,
                CurrentQuestionIndex = startQuestionIndex,
                CorrectCount = 0
            };
        }

        public void SendQuestionByIndex(long chatId, int index)
        {
            var question = _questions[index];
            var message = $"{question.Id}. {question.Question}";

            if (question.Media.Exist)
            {
                try
                {
                    var fileBytes = File.ReadAllBytes($"Autotest/{question.Media.Name}.png");
                    var ms = new MemoryStream(fileBytes);

                    _bot.SendPhotoAsync(
                        chatId: chatId,
                        photo: new InputOnlineFile(ms),
                        caption: message,
                        replyMarkup: CreateQuestionChoiceButtons(index));
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message);
                }
            }
            else
            {
                _bot.SendTextMessageAsync(chatId, message, replyMarkup: CreateQuestionChoiceButtons(index));
            }
        }

        public bool QuestionAnswer(int questionIndex, int choiceIndex)
        {
            return _questions[questionIndex].Choices[choiceIndex].Answer;
        }

        InlineKeyboardMarkup CreateQuestionChoiceButtons(int index, int? choiceIndex = null, bool? answer = null)
        {
            var choiceButtons = new List<List<InlineKeyboardButton>>();

            for (int i = 0; i < _questions[index].Choices.Count; i++)
            {
                var choiceText = answer == null ? _questions[index].Choices[i].Text : _questions[index].Choices[i].Text + answer;

                var choiceButton = new List<InlineKeyboardButton>()
                {

                    InlineKeyboardButton.WithCallbackData(choiceText, $"{index},{i}")
                };

                choiceButtons.Add(choiceButton);
            }

            return new InlineKeyboardMarkup(choiceButtons);
        }

    }
}