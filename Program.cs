using Telegram.Bot;
using Telegram;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;
using System.IO;


namespace DTM
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string token = "6077885052:AAFJays-peGn1YKsD8W9vkORfLN569YsiO4"; // Gotta hide it
            
            // TelegramBotHandler.TestNumber = 1; 
            // TelegramBotHandler handler = new TelegramBotHandler(token);
            
            try
            { 
                // await handler.BotHandle();
            }
            catch (Exception ex)
            {
                throw new Exception("No error");
            }
        }
    }
}
