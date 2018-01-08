using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace edabot
{
    public partial class Form1 : Form
    {
        BackgroundWorker bw;

        public Form1()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();

            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //

            this.bw = new BackgroundWorker();
            this.bw.DoWork += bw_DoWork;
        }

        async void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            var key = e.Argument as String;
            try
            {
                var Bot = new Telegram.Bot.TelegramBotClient(key);
                await Bot.SetWebhookAsync("");

                Bot.OnInlineQuery += async (object si, Telegram.Bot.Args.InlineQueryEventArgs ei) =>
                {
                    var query = ei.InlineQuery.Query;

                    var msg = new Telegram.Bot.Types.InputMessageContents.InputTextMessageContent
                    {
                        ParseMode = Telegram.Bot.Types.Enums.ParseMode.Html,
                    };

                    Telegram.Bot.Types.InlineQueryResults.InlineQueryResult[] results = {
                        new Telegram.Bot.Types.InlineQueryResults.InlineQueryResultArticle
                        {
                        },
                        new Telegram.Bot.Types.InlineQueryResults.InlineQueryResultAudio
                        {
                        },
                        new Telegram.Bot.Types.InlineQueryResults.InlineQueryResultPhoto
                        {
                        },
                        new Telegram.Bot.Types.InlineQueryResults.InlineQueryResultVideo
                        {
                        }
                    };


                    await Bot.AnswerInlineQueryAsync(ei.InlineQuery.Id, results);
                };

                Bot.OnUpdate += async (object su, Telegram.Bot.Args.UpdateEventArgs evu) =>
                {

                    if (evu.Update.CallbackQuery != null || evu.Update.InlineQuery != null) return;
                    var update = evu.Update;
                    var message = update.Message;
                    //пусть будут чрезвычайно длинные строки, мне лень делать что-то умное, ключ апи раздают всем подряд, палить его не боюсь
                    var randomRestaurantUrl =
"https://search-maps.yandex.ru/v1/?apikey=0d7404f4-957c-4116-b9a3-efe9fc40fbc7&results=500&text=Куда сходить пообедать баумана казань&lang=ru_RU&type=biz";
                    var buhichUrl = "https://search-maps.yandex.ru/v1/?apikey=0d7404f4-957c-4116-b9a3-efe9fc40fbc7&results=500&text=Куда сходить выпить пива баумана казань&lang=ru_RU&type=biz";
                    if (message == null) return;
                    if (message.Type == Telegram.Bot.Types.Enums.MessageType.TextMessage)
                    {
                        if (message.Text == "/кудакушац")
                        {
                            string[] restaurants = { "Арома", "The Woods", "Brownbear", "ТЫ ПИДОР", "Big Butcher Grill", "OmNomNom", "Rock'n'Roll'ы", "Черный ящик", "Wok and Go", "Agafredo", "Твин Пикс", "KFC", "Тринити", "МАКДАК", "Бивень", "Соль", "The Тёлки", "Бинхартс"};
                            //хотел отправлять фотки, но потом стало лень их заливать
                            //string[] restaurantsPhotos = { "AgADAgAD5KkxG-nNgUsmW5IeJEGSBBIySw0ABA_maPNgEFdds1gRAAEC"};
                            //var randomPhoto = restaurantsPhotos[new Random().Next(0, restaurantsPhotos.Length)];
                            var randomCommand = restaurants[new Random().Next(0, restaurants.Length)];
                            //var photos = new Telegram.Bot.Types.FileToSend { FileId = randomPhoto };
                            string outputText = randomCommand + " - это место куда ты сегодня пойдешь обедать";
                            await Bot.SendTextMessageAsync(message.Chat.Id, outputText, replyToMessageId: message.MessageId);
                            //await Bot.SendPhotoAsync(message.Chat.Id, photos, replyToMessageId: message.MessageId);
                        }

                        if (message.Text == "/кудахавать")
                            await Bot.SendTextMessageAsync(message.Chat.Id, YandexRandomiser.GetRandomSearchResult(randomRestaurantUrl).Result, replyToMessageId: message.MessageId);

                        if (message.Text == "/кудабухать")
                            await Bot.SendTextMessageAsync(message.Chat.Id, YandexRandomiser.GetRandomSearchResult(buhichUrl).Result, replyToMessageId: message.MessageId);
                    }
                };

                // запускаем прием обновлений
                Bot.StartReceiving();
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException ex)
            {
                Console.WriteLine(ex.Message); // если ключ не подошел - пишем об этом в консоль отладки
            }

        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            var text = txtKey.Text; // получаем содержимое текстового поля txtKey в переменную text
            if (text != "" && this.bw.IsBusy != true)
            {
                this.bw.RunWorkerAsync(text); // передаем эту переменную в виде аргумента методу bw_DoWork
                BtnRun.Text = "Бот запущен...";
            }
        }
    }
}
