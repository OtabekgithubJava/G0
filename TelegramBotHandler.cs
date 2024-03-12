// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;
// using DTM;
// using Newtonsoft.Json;
// using Telegram.Bot;
// using Telegram.Bot.Exceptions;
// using Telegram.Bot.Polling;
// using Telegram.Bot.Types;
// using Telegram.Bot.Types.Enums;
// using Telegram.Bot.Types.ReplyMarkups;
// using File = System.IO.File;
//
// public class User
// {
//     public long UserId { get; set; }
//     public string? Username { get; set; }
//     public bool IsRegistered { get; set; }
//     public string? ContactNumber { get; set; }
//     
//     public bool IsInQuiz { get; set; }
//     public int Score { get; set; }
//
//     public User(long userId, string? username, bool isRegistered, string? contactNumber)
//     {
//         UserId = userId;
//         Username = username;
//         IsRegistered = isRegistered;
//         ContactNumber = contactNumber;
//     }
// }
//
//
// public class TelegramBotHandler
// {
//     private const string UsersFilePath = "users.txt"; 
//     private List<User> registeredUsers;
//     public static int TestNumber = 1;
//     public string Token { get; set; }
//     public object currentTime = DateTime.Now.ToString("HH:mm");
//
//     public TelegramBotHandler(string token)
//     {
//         this.Token = token;
//         LoadRegisteredUsers();
//     }
//
//     public async Task BotHandle()
//     {
//         var botClient = new TelegramBotClient(Token);
//
//         using CancellationTokenSource cts = new CancellationTokenSource();
//
//         ReceiverOptions receiverOptions = new ReceiverOptions
//         {
//             AllowedUpdates = Array.Empty<UpdateType>()
//         };
//
//         botClient.StartReceiving(
//             updateHandler: HandleUpdateAsync,
//             pollingErrorHandler: HandlePollingErrorAsync,
//             receiverOptions: receiverOptions,
//             cancellationToken: cts.Token
//             );
//
//         var me = await botClient.GetMeAsync();
//
//         Console.WriteLine($"Start listening for @{me.Username}");
//         Console.ReadLine();
//
//         cts.Cancel();
//         
//     }
//     
//     async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
//     {
//         
//         var handler = update.Type switch
//         {
//             UpdateType.Message => HandleMessageAsync(botClient, update, cancellationToken),
//             UpdateType.CallbackQuery => HandleCallBackQueryAsync(botClient, update, cancellationToken),
//             //Yana update larni davom ettirib tutishingiz mumkin
//         };
//
//         try
//         {
//             await handler;
//         }
//         catch (Exception ex)
//         {
//             await Console.Out.WriteLineAsync($"Xato:{ex.Message}");
//         }
//         var messageText = "";
//         if (update.Message?.Text is not null)
//             messageText = update.Message.Text;
//
//         if (update.Message != null)
//         {
//             var chatId = update.Message.Chat.Id;
//
//             var chatBio = update.Message.Chat?.Username;
//
//             string user_message = $"Received a '{messageText}' message in chat {chatId}.\t Bio => {chatBio}\t at {currentTime}\n";
//             string filepath = "/Users/otabek_coding/Desktop/Najot Ta'lim/DTM/Output.txt";
//             File.AppendAllText(filepath, user_message);
//
//             if (messageText == "/start")
//             {
//                 await SendStartMessageAsync(chatId, botClient, cancellationToken);
//             }
//             else if (messageText.ToLower().StartsWith("register") || messageText.ToLower().StartsWith("/register"))
//             {
//                 await RegisterUserAsync(update.Message.From, chatId, botClient, cancellationToken);
//             }
//             else if (messageText.ToLower().Equals("getme") || messageText.ToLower().Equals("/getme") || messageText.ToLower().Equals("get me"))
//             {
//                 await GetUserInfoAsync(update.Message.From, chatId, botClient, cancellationToken);
//             }
//             else if (messageText.ToLower().Equals("getall") || messageText.ToLower().Equals("/getall") || messageText.ToLower().Equals("get all"))
//             {
//                 await GetAllUsersInfoAsync(chatId, botClient, cancellationToken);
//             }
//             else if (update.Message.Type == MessageType.Contact)
//             {
//                 var user = registeredUsers.FirstOrDefault(u => u.UserId == update.Message.From.Id);
//
//                 if (user != null)
//                 {
//                     user.ContactNumber = update.Message.Contact.PhoneNumber;
//                     SaveRegisteredUsers();
//
//                     await botClient.SendTextMessageAsync(chatId, $"Contact number updated: {update.Message.Contact.PhoneNumber}", cancellationToken: cancellationToken);
//                 }
//                 else
//                 {
//                     await botClient.SendTextMessageAsync(chatId, "You are not registered. Please use /register to register.", cancellationToken: cancellationToken);
//                 }
//             }
//             else if (messageText.ToLower() == "quiz" || messageText.ToLower() == "/quiz")
//             {
//                 var user = registeredUsers.FirstOrDefault(u => u.UserId == update.Message.From.Id);
//
//                 if (user != null)
//                 {
//                     user.IsInQuiz = true;
//                     SaveRegisteredUsers();
//
//                     Console.WriteLine($"Quiz initiated. User ID: {user.UserId}");
//                     await HandleCallBackQueryAsync(botClient, update, cancellationToken);
//                 }
//             }
//             else if (update.CallbackQuery != null)
//             {
//                 // Handle callback queries
//                 await HandleCallBackQueryAsync(botClient, update, cancellationToken);
//             }
//             else
//             {
//                 await botClient.SendTextMessageAsync(
//                     chatId: chatId,
//                     text: "Welcome! Choose an option:",
//                     cancellationToken: cancellationToken);
//             }
//         }
//     }
//     
//     
//     private static async Task HandleCallBackQueryAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
//     {
//         var callBack = update.CallbackQuery;
//
//         // Create an instance of TestRepository
//         var testRepository = new TestRepository();
//     
//         // Call the GetTests method on the instance
//         var tests = testRepository.GetTests();
//         Console.WriteLine(TestNumber);
//
//         if (tests != null && tests.Count >= TestNumber)
//         {
//             var test = tests[TestNumber - 1];
//             var nextTest = tests[TestNumber];
//
//             await CheckAnswerAsync(test, botClient, callBack, cancellationToken);
//             TestNumber++;
//             
//             if (TestNumber <= tests.Count)
//             {
//                 await SendNextQuestion(nextTest, botClient, update, cancellationToken);
//             }
//             else
//             {
//                 await botClient.SendTextMessageAsync(
//                     chatId: callBack.From.Id,
//                     text: "Congratulations! You have completed all the tests.",
//                     cancellationToken: cancellationToken);
//             }
//         }
//         else
//         {
//             await botClient.SendTextMessageAsync(
//                 chatId: callBack.From.Id,
//                 text: "Error: Unable to retrieve tests or TestNumber is out of bounds.",
//                 cancellationToken: cancellationToken);
//         }
//     }
//
//     
//     //Keyingi testni botga tashlab beradi
//     private static async Task SendNextQuestion(Testcss nextTest, ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
//     {
//         // Construct the inline keyboard markup
//         InlineKeyboardMarkup inlineKeyboard = new(new[]
//         {
//             // row
//             new []
//             {
//                 InlineKeyboardButton.WithCallbackData(
//                     text: $"{nextTest.A}", 
//                     callbackData: "A"),
//             },
//             new []
//             {
//                 InlineKeyboardButton.WithCallbackData(
//                     text: $"{nextTest.B}", 
//                     callbackData: "B"),
//             },
//             new []
//             {
//                 InlineKeyboardButton.WithCallbackData(
//                     text: $"{nextTest.C}", 
//                     callbackData: "C"),
//             },
//             new []
//             {
//                 InlineKeyboardButton.WithCallbackData(
//                     text: $"{nextTest.D}", 
//                     callbackData: "D"),
//             }
//         });
//
//         // Debugging information
//         Console.WriteLine($"Sending question to chat ID: {update.Message.Chat.Id}");
//         Console.WriteLine($"Question: {nextTest.Question}");
//
//         // Send the message with the inline keyboard
//         if(update.Message is null)
//         {
//             await botClient.SendTextMessageAsync(
//                 chatId: update.CallbackQuery.From.Id,
//                 text: $"{nextTest.Question}",
//                 replyMarkup: inlineKeyboard,
//                 cancellationToken: cancellationToken
//             );
//         }
//         else
//         {
//             await botClient.SendTextMessageAsync(
//                 chatId: update.Message.Chat.Id,
//                 text: $"{nextTest.Question}",
//                 replyMarkup: inlineKeyboard,
//                 cancellationToken: cancellationToken
//             );
//         }
//     }
//
//     
//     //Callback queryda kelgan javobni to'g'ri yoki xatoligini tekshiradi
//     private static async Task CheckAnswerAsync(Testcss test, ITelegramBotClient botClient, CallbackQuery? callBack, CancellationToken cancellationToken)
//     {
//         if (callBack != null && callBack.Data == test.CorrectAnswer)
//         {
//             await botClient.SendTextMessageAsync(
//                 chatId: callBack.From.Id,
//                 text: $"To'g'ri malades",
//                 parseMode: ParseMode.Html,
//                 cancellationToken: cancellationToken);
//         }
//         else if (callBack != null)
//         {
//             await botClient.SendTextMessageAsync(
//                 chatId: callBack.From.Id,
//                 text: $"Xato javob",
//                 parseMode: ParseMode.Html,
//                 cancellationToken: cancellationToken);
//         }
//     }
//
//
//     private void LoadRegisteredUsers()
//     {
//         if (File.Exists(UsersFilePath))
//         {
//             try
//             {
//                 var lines = File.ReadAllLines(UsersFilePath);
//                 registeredUsers = lines.Select(line => JsonConvert.DeserializeObject<User>(line)).ToList();
//             }
//             catch (Exception)
//             {
//                 registeredUsers = new List<User>();
//             }
//         }
//         else
//         {
//             registeredUsers = new List<User>();
//         }
//     }
//
//     private void SaveRegisteredUsers()
//     {
//         var lines = registeredUsers.Select(user => JsonConvert.SerializeObject(user));
//         File.WriteAllLines(UsersFilePath, lines);
//     }
//     
//     private async Task SendStartMessageAsync(long chatId, ITelegramBotClient botClient, CancellationToken cancellationToken)
//     {
//         ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
//         {
//             new KeyboardButton[] { KeyboardButton.WithRequestContact("Share Contact"), new KeyboardButton("Register") },
//             new KeyboardButton[] { new KeyboardButton("Get Me"), new KeyboardButton("Get All") },
//         });
//
//
//         await botClient.SendTextMessageAsync(
//             chatId: chatId,
//             text: "Welcome! Choose an option:",
//             replyMarkup: replyKeyboardMarkup,
//             cancellationToken: cancellationToken);
//     }
//
//     private async Task RegisterUserAsync(Telegram.Bot.Types.User user, long chatId, ITelegramBotClient botClient, CancellationToken cancellationToken)
//     {
//         if (registeredUsers == null)
//         {
//             registeredUsers = new List<User>();
//         }
//
//         if (!IsUserRegistered(user.Id))
//         {
//             var contactNumberMessage = await WaitForUserResponseAsync(botClient, chatId, cancellationToken);
//
//             var newUser = new User(user.Id, user.Username ?? "Unknown", true, contactNumberMessage.Contact?.PhoneNumber);
//             registeredUsers.Add(newUser);
//             SaveRegisteredUsers();
//             
//             await botClient.SendTextMessageAsync(chatId, $"You have been successfully registered. Contact Number: {contactNumberMessage.Contact?.PhoneNumber}", cancellationToken: cancellationToken);
//         }
//         else
//         {
//             await botClient.SendTextMessageAsync(chatId, "You are already registered.", cancellationToken: cancellationToken);
//         }
//     }
//
//
//     private async Task<Message> WaitForUserResponseAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
//     {
//         Message message = null;
//
//         while (message == null || string.IsNullOrWhiteSpace(message.Text))
//         {
//             var updates = await botClient.GetUpdatesAsync(offset: 0, limit: 1, allowedUpdates: null, cancellationToken: cancellationToken);
//
//             if (updates.Length > 0 && updates[0].Message != null)
//             {
//                 message = updates[0].Message;
//             }
//             
//             await Task.Delay(500);
//         }
//
//         return message;
//     }
//
//     
//
//     private bool IsUserRegistered(long userId)
//     {
//         return registeredUsers?.Any(u => u.UserId == userId && u.IsRegistered) == true;
//     }
//
//     private async Task GetUserInfoAsync(Telegram.Bot.Types.User user, long chatId, ITelegramBotClient botClient, CancellationToken cancellationToken)
//     {
//         if (registeredUsers == null)
//         {
//             registeredUsers = new List<User>();
//         }
//
//         var userInfo = registeredUsers.FirstOrDefault(u => u.UserId == user.Id);
//
//         if (userInfo != null)
//         {
//             await botClient.SendTextMessageAsync(chatId, $"User Info:\nID: {userInfo.UserId}\nUsername: {userInfo.Username}\nRegistered: {userInfo.IsRegistered}\nContact Number: {userInfo.ContactNumber ?? "Not provided"}", cancellationToken: cancellationToken);
//         }
//         else
//         {
//             await botClient.SendTextMessageAsync(chatId, "You are not registered. Please use /register to register.", cancellationToken: cancellationToken);
//         }
//     }
//
//     private async Task GetAllUsersInfoAsync(long chatId, ITelegramBotClient botClient, CancellationToken cancellationToken)
//     {
//         var allUsersInfo = registeredUsers.Select(u => $"ID: {u.UserId}, Username: {u.Username}, Registered: {u.IsRegistered}, Contact Number: {u.ContactNumber ?? "Not provided"}");
//         var message = string.Join("\n", allUsersInfo);
//
//         await botClient.SendTextMessageAsync(chatId, $"All Users Info:\n{message}", cancellationToken: cancellationToken);
//     }
//     
//     public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
//     {
//         var errorMessage = exception switch
//         {
//             ApiRequestException apiRequestException
//                 => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
//             _ => exception.ToString()
//         };
//
//         Console.WriteLine(errorMessage);
//         return Task.CompletedTask;
//     }
// }