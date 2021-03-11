using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Helpers;

namespace Telegram.Bot.Examples.Echo
{
    public static class Program
    {
        private static TelegramBotClient Bot;
        private static FileStream fs1;

        public static async Task Main()
        {

            //fs1 = new FileStream("C:/SavedPictures/for_tg_bot/SavedFromTelegramPhoto.png", FileMode.Create);

            Bot = new TelegramBotClient(Configuration.BotToken);

            //FileStream fs1 = new FileStream("")


            var me = await Bot.GetMeAsync();
            //Console.Title = me.Username;

            

            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessageEdited += BotOnMessageReceived;
            Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            Bot.OnInlineQuery += BotOnInlineQueryReceived;
            Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
            Bot.OnReceiveError += BotOnReceiveError;

            //Bot.KickChatMemberAsync(-564573859, 1671384608);

            //Bot.UnbanChatMemberAsync(-564573859, 1671384608);

            //Bot.PromoteChatMemberAsync(-564573859, 1671384608);

            Bot.StartReceiving(Array.Empty<UpdateType>());
            Console.WriteLine($"Start listening for @{me.Username}");

            string consoleMessage;
            //Console.ReadLine();
            //Bot.StopReceiving();

            readmessagefromconsole:               //Send message to group
            Console.WriteLine("Input text: \n");
            consoleMessage = Console.ReadLine();

            if (consoleMessage == "/stopfs")
            {
                fs1.Close();
            }

            if (consoleMessage == "/stop")
            {
                Bot.StopReceiving();              //Stops bot and closes the program
                return;
            }
            else
            {
                SendConsoleMessage(consoleMessage);
                Console.WriteLine("---\nMessage was sended.");

                goto readmessagefromconsole;
            }
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            Console.WriteLine(message.Type);

            if (message.Type == MessageType.Photo)
            {

                fs1 = new FileStream("C:/SavedPictures/for_tg_bot/SavedFromTelegramPhoto.png", FileMode.Create);

                await Bot.SendTextMessageAsync(
                    message.Chat.Id,
                    "Reply id: " + message.Photo[0].FileId,
                    replyToMessageId: message.MessageId

                    );

                string FileId = message.Photo[0].FileId;

                int ch;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.telegram.org/bot" + Configuration.BotToken + "/getFile?file_id=" + FileId);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream responseStream = response.GetResponseStream();

                string responseString = "";

                for (int i = 0; i < 1000; i++)
                {


                    ch = responseStream.ReadByte();

                    if (ch == -1) break;

                    responseString += ((char)(ch));

                    //Console.Write((char)ch);
                }

                Console.WriteLine(responseString + "\n");

                string filePath = "";

                Console.WriteLine(responseString.IndexOf("file_path"));



                int startindex = responseString.IndexOf("file_path") + "file_path: \"".Length;

                filePath = responseString.Substring(startindex, responseString.Length - startindex - 3);



                Console.WriteLine("file_path:" + filePath);

                // ОБРАТИ ВНИМАНИЕ!!! ТУТ НЕБЕЗОПАСНОЕ ГОВНИЩЕ (НУ И СВЕРХУ ТОЖЕ ТАК ТО)
                await Bot.DownloadFileAsync(
                    filePath,
                    fs1
                    );

                fs1.Close();

                FileStream fs2 = new FileStream("C:/SavedPictures/for_tg_bot/SavedFromTelegramPhoto.png", FileMode.Open);

                Console.WriteLine("Пэнгешка скачалось");

                await Bot.SendDocumentAsync(
                    chatId: message.Chat.Id, 
                    document: new InputOnlineFile(fs2, fileName: "TestDoc.png"),
                    caption: "Подержи пережатое говно",
                    replyToMessageId: message.MessageId);

                Console.WriteLine("Пэнгешка отправилось");

                fs2.Close();
            }

            if (message.Type == MessageType.Document)
            {
                fs1 = new FileStream("C:/SavedPictures/for_tg_bot/SavedFromTelegramPhoto.png", FileMode.Create);
                await Bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: message.Document.FileId
                    );

                string FileId = message.Document.FileId;

                int ch;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.telegram.org/bot" + Configuration.BotToken + "/getFile?file_id=" + FileId);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream responseStream = response.GetResponseStream();

                string responseString = "";

                for (int i = 0; i < 1000; i++)
                {


                    ch = responseStream.ReadByte();

                    if (ch == -1) break;

                    responseString += ((char)(ch));

                    //Console.Write((char)ch);
                }

                Console.WriteLine(responseString + "\n");

                string filePath = "";

                Console.WriteLine(responseString.IndexOf("file_path"));



                int startindex = responseString.IndexOf("file_path") + "file_path: \"".Length;

                filePath = responseString.Substring(startindex, responseString.Length - startindex - 3);



                Console.WriteLine("file_path:" + filePath);

                // ОБРАТИ ВНИМАНИЕ!!! ТУТ НЕБЕЗОПАСНОЕ ГОВНИЩЕ (НУ И СВЕРХУ ТОЖЕ ТАК ТО)
                await Bot.DownloadFileAsync(
                    filePath,
                    fs1
                    );

                fs1.Close();

                FileStream fs2 = new FileStream("C:/SavedPictures/for_tg_bot/SavedFromTelegramPhoto.png", FileMode.Open);

                Console.WriteLine("Документ скачалось");

                await Bot.SendDocumentAsync(
                    chatId: message.Chat.Id,
                    document: new InputOnlineFile(fs2, fileName: "TestDoc.png"),
                    caption: "Подержи пережатое говно",
                    replyToMessageId: message.MessageId);

                Console.WriteLine("Документ отправилось");

                fs2.Close();

            }

            if (message == null || message.Type != MessageType.Text)
                return;

            switch (message.Text.Split(' ').First())
            {
                // Send inline keyboard
                case "/inline":
                    await SendInlineKeyboard(message);
                    break;

                // send custom keyboard
                case "/keyboard":
                    await SendReplyKeyboard(message);
                    break;

                // send a photo
                case "/photo":
                    try
                    {
                        await SendDocument(message);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                        await Bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Error\n");
                    }
                    break;

                // request location or contact
                case "/request":
                    await RequestContactAndLocation(message);
                    break;

                case "/help":
                    await Usage(message);
                    break;

                case "/start":
                    await Bot.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Я могу то, то и то. Да и вообще я пиздатый"
                        );
                    break;

                case "/myinfo":
                    await SendInformationAboutUser(message);
                    break;

                case "/toconsole":
                    await Bot.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Сообщение для консоли..."
                        );
                    
                    await GetMessageForConsole(message);
                    await Bot.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "Отправлено!"
                        );
                    break;

                case "/test":
                    if (message.ReplyToMessage != null)
                    {
                        Message mes = message.ReplyToMessage;
                        await Bot.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: mes.Text
                        );
                    }
                    await Bot.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "aboba"
                        );
                    break;
                case "/testentity":
                   
                    
                    await Bot.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: "[чел ты гений](tg://user?id:1113634091)",
                        ParseMode.MarkdownV2
                        );
                    
                    break;

                default:
                    await Bot.SendTextMessageAsync(
                        message.Chat.Id, "ChatId: " + 
                        message.Chat.Id.ToString());
                    await Usage(message);
                    break;
            }
        //}

            // Send inline keyboard
            // You can process responses in BotOnCallbackQueryReceived handler
            static async Task SendInlineKeyboard(Message message)
            {
                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                // Simulate longer running task
                 //await Task.Delay(500);

                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("1.1", "11"),
                        InlineKeyboardButton.WithCallbackData("1.2", "12"),
                        InlineKeyboardButton.WithCallbackData("Test"),
                        InlineKeyboardButton.WithCallbackData("Test2"),
                        InlineKeyboardButton.WithCallbackData("Test3"),
                        InlineKeyboardButton.WithCallbackData("Test4"),
                        InlineKeyboardButton.WithCallbackData("Test8")
                    },
                    // second row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("2.1", "21"),
                        InlineKeyboardButton.WithCallbackData("2.2", "22"),
                    }
                }); 
                await Bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Choose",
                    replyMarkup: inlineKeyboard
                );
            }

            static async Task SendReplyKeyboard(Message message)
            {
                var replyKeyboardMarkup = new ReplyKeyboardMarkup(
                    new KeyboardButton[][]
                    {
                        new KeyboardButton[] { "1.1", "1.2", "1.3" },
                        new KeyboardButton[] { "2.1", "2.2" },
                        new KeyboardButton[] { "3.1", "3.2" },
                        new KeyboardButton[] { "4.1" }
                        //new KeyboardButton("ffjj")
                    });
                var replyKeyboardMarkup1 = new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton("Text1"),
                        new KeyboardButton("Text2"),
                        new KeyboardButton("Text2"),
                        new KeyboardButton("Text2"),
                        new KeyboardButton("Text2"),
                        new KeyboardButton("Text2"),
                    }
                ); 

                await Bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Choose",
                    replyMarkup: replyKeyboardMarkup

                );
            }

            static async Task SendDocument(Message message)
            {
                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                const string filePath = @"C:\Users\Daniil\source\repos\ConsoleApp6\ConsoleApp6\tux.png";
                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();
                await Bot.SendPhotoAsync(
                    chatId: message.Chat.Id,
                    photo: new InputOnlineFile(fileStream, fileName),
                    caption: "Nice Picture"
                );
            }

            static async Task RequestContactAndLocation(Message message)
            {
                var RequestReplyKeyboard = new ReplyKeyboardMarkup(new[]
                {
                    KeyboardButton.WithRequestLocation("Location"),
                    KeyboardButton.WithRequestContact("Contact"),
                    KeyboardButton.WithRequestPoll("Poll request")
                    
                });
                await Bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Who or Where are you?",
                    replyMarkup: RequestReplyKeyboard
                );
            }

            static async Task Usage(Message message)
            {
                const string usage = "Usage:\n" +
                                        "/inline   - send inline keyboard\n" +
                                        "/keyboard - send custom keyboard\n" +
                                        "/photo    - send a photo\n" +
                                        "/request  - request location or contact";
                await Bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: usage,
                    replyMarkup: new ReplyKeyboardRemove()
                );
            }
        }

        // Process Inline Keyboard callback data
        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;

            await Bot.AnswerCallbackQueryAsync(
                callbackQueryId: callbackQuery.Id,
                text: $"Received {callbackQuery.Data}"
            );

            await Bot.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: $"Received {callbackQuery.Data}"
            );
        }

        #region Inline Mode

        private static async void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs inlineQueryEventArgs)
        {
            Console.WriteLine($"Received inline query from: {inlineQueryEventArgs.InlineQuery.From.Id}");

            InlineQueryResultBase[] results = {
                // displayed result
                new InlineQueryResultArticle(
                    id: "3",
                    title: "TgBots",
                    inputMessageContent: new InputTextMessageContent(
                        "hello"
                    )
                )
            };
            await Bot.AnswerInlineQueryAsync(
                inlineQueryId: inlineQueryEventArgs.InlineQuery.Id,
                results: results,
                isPersonal: true,
                cacheTime: 0
            );
        }

        private static void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs chosenInlineResultEventArgs)
        {
            Console.WriteLine($"Received inline result: {chosenInlineResultEventArgs.ChosenInlineResult.ResultId}");
        }

        #endregion

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("Received error: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message
            );
        }

        private static async Task SendInformationAboutUser(Message usermessage)
        {
            var botUser = usermessage.From;

            Console.WriteLine(usermessage.Type);

            await Bot.SendTextMessageAsync(
                chatId: usermessage.Chat.Id,
                text: 
                
                "First Name: " + botUser.Username + "\n" +
                "Last Name: " + botUser.LastName + "\n" +
                "Id: " + botUser.Id + "\n" //+
                //"vCard Form: " + usermessage.Contact.Vcard.ToString()
            );
        }

        private static void SendConsoleMessage(string messageFromConsole)
        {
            Bot.SendTextMessageAsync(
                chatId: -564573859,
                text:
                messageFromConsole
                );
        }

        private static async Task GetMessageForConsole(Message messageForConsole)
        {   if (messageForConsole.Text.Substring(11) != null)
            {
                string mes = messageForConsole.Text.Substring(11);
                Console.WriteLine("Получено сообщение для консоли! Содержание: ");
                Console.WriteLine(mes);
            }
        }
    }
}