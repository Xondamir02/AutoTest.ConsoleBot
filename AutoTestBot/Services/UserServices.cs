
using AutoTestBot.Models;
using AutoTestBot.Models.Users;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace AutoTestBot.Services
{
    class UserServices
    {
        private List<User> _users;
        private const string UserJsonFilePath = "users.json";
        private readonly QuestionService _questionService;

        public UserServices (QuestionService questionService)
        {
            _questionService = questionService;
            ReadUsersJson();
        }

        public User AddUser(long chatId, string name)
        {
            if (_users.Any(u => u.ChatId == chatId))
            {
                return _users.First(u => u.ChatId == chatId);
            }
            else
            {
                var user = new User
                {
                    ChatId = chatId,
                    Name = name,
                    Tickets = new List<Ticket>()
                };

          
                for (int i = 0; i < _questionService.TicketsCount; i++)
                {
                    user.Tickets.Add(new Ticket(i, QuestionService.TicketQuestionsCount));
                }

                _users.Add(user);

                SaveUsersJson();
                return user;
            }
        }

        public void UpdateUserStep(User user, ENextmessage step)
        {
            user.Step = step;
            SaveUsersJson();
        }

        private void ReadUsersJson()
        {
            if (File.Exists(UserJsonFilePath))
            {
                var json = File.ReadAllText(UserJsonFilePath);
                _users = JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
            }
            else
            {
                _users = new List<User>();
            }
        }

        public void SaveUsersJson()
        {
            var json = JsonConvert.SerializeObject(_users);
            File.WriteAllText(UserJsonFilePath, json);
        }
    }
}
