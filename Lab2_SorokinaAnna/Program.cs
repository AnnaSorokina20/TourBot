using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Completions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

class Program
{

    private static readonly HttpClient HttpClient = new HttpClient();

    private static readonly dynamic config = JsonConvert.DeserializeObject(System.IO.File.ReadAllText("D:\\ХАИ\\2 курс\\Семестр 2\\.Net\\Lab2\\Lab2_SorokinaAnna\\Lab2_SorokinaAnna\\config.json"));
    // Ініціалізація клієнта TelegramBot з API токеном
    private static readonly TelegramBotClient Bot = new TelegramBotClient((string)config.TelegramBotToken);
    // API ключі для OpenAI та OpenWeatherMap
    private static readonly string OpenAIApiKey = (string)config.OpenAIApiKey;
    private static readonly string WeatherApiKey = (string)config.WeatherApiKey;

    // Словник доступних турів з описами
    private static readonly Dictionary<string, string> Tours = new Dictionary<string, string>
    {
        { "Paris", "Visit the city of lights and explore its famous landmarks like the Eiffel Tower, Louvre Museum, and Notre-Dame Cathedral." },
        { "London", "Discover the rich history of London with tours to Buckingham Palace, Tower of London, and the British Museum." },
        { "New York", "Experience the vibrant life of New York City, including Times Square, Central Park, and the Statue of Liberty." }
    };

    static async Task Main(string[] args)
    {
        // Ініціалізація токена скасування та параметрів прийому
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { }// отримувати всі типи оновлень
        };

        // Запуск отримання повідомлень від користувачів
        Bot.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken);

        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        cts.Cancel();
    }

    // Обробка оновлень від користувачів
    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Перевірка, чи є оновленням повідомлення з текстом
        if (update.Type == UpdateType.Message && update.Message.Type == MessageType.Text)
        {
            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;

            // Обробка команди /start
            if (messageText == "/start")
            {
                var replyKeyboard = new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton[] { "City Information", "Tours" }
                })
                {
                    ResizeKeyboard = true
                };

                await botClient.SendTextMessageAsync(chatId, "Welcome to the Travel Agency Bot! Use the buttons below to navigate.", replyMarkup: replyKeyboard);
            }
            // Обробка команди /help
            else if (messageText == "/help")
            {
                await botClient.SendTextMessageAsync(chatId, "/start - Start the bot\n/help - Show this help message\nTours - Show available tours\nCity Information - Get information about a city");
            }
            // Обробка команди "Tours"
            else if (messageText == "Tours")
            {
                var replyKeyboard = new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton[] { "Paris", "London", "New York" },
                    new KeyboardButton[] { "/book Paris", "/book London", "/book New York" },
                    new KeyboardButton[] { "Back to Main Menu" }
                })
                {
                    ResizeKeyboard = true
                };

                await botClient.SendTextMessageAsync(chatId, "Here are some popular tours:", replyMarkup: replyKeyboard);
            }
            // Обробка команди /book
            else if (messageText.StartsWith("/book"))
            {
                var tourName = messageText.Length > 6 ? messageText.Substring(6).Trim() : string.Empty;
                if (string.IsNullOrEmpty(tourName) || !Tours.ContainsKey(tourName))
                {
                    await botClient.SendTextMessageAsync(chatId, "Please provide a valid tour name. Usage: /book [tour name]");
                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId, $"You have successfully booked the tour: {tourName}");
                }
            }
            // Обробка команди "City Information"
            else if (messageText == "City Information")
            {
                await botClient.SendTextMessageAsync(chatId, "Please enter the name of the city to get information about it.");
            }
            // Відображення інформації про обраний тур
            else if (Tours.ContainsKey(messageText))
            {
                await botClient.SendTextMessageAsync(chatId, Tours[messageText]);
            }
            // Повернення до головного меню
            else if (messageText == "Back to Main Menu")
            {
                var replyKeyboard = new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton[] { "City Information", "Tours" }
                })
                {
                    ResizeKeyboard = true
                };

                await botClient.SendTextMessageAsync(chatId, "Back to the main menu.", replyMarkup: replyKeyboard);
            }
            // Отримання інформації про місто та погоду
            else
            {
                var cityInfo = await GetCityInfoAsync(messageText);
                var weatherInfo = await GetWeatherInfoAsync(messageText);
                await botClient.SendTextMessageAsync(chatId, $"{cityInfo}\n\nWeather Info:\n{weatherInfo}");
            }
        }
    }

    // Отримання інформації про місто з OpenAI
    private static async Task<string> GetCityInfoAsync(string cityName)
    {
        var api = new OpenAIAPI(OpenAIApiKey);
        string result = "";
        string prompt = $"Tell me about the city {cityName}.";

        while (true)
        {
            var chatRequest = new ChatRequest
            {
                Model = "gpt-3.5-turbo",
                Messages = new[]
                {
                    new ChatMessage(ChatMessageRole.System, "You are a helpful assistant."),
                    new ChatMessage(ChatMessageRole.User, prompt)
                },
                MaxTokens = 200,
                Temperature = 0.7
            };

            try
            {
                var chatResponse = await api.Chat.CreateChatCompletionAsync(chatRequest);
                if (chatResponse.Choices == null || chatResponse.Choices.Count == 0)
                {
                    return "Sorry, I couldn't retrieve information about that city.";
                }

                var responseText = chatResponse.Choices[0].Message.Content.Trim();
                result += responseText;

                if (responseText.EndsWith('.') || responseText.EndsWith('!') || responseText.EndsWith('?'))
                {
                    break;
                }

                prompt = ""; // Продовження запиту, якщо відповідь неповна
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calling OpenAI API: {ex.Message}");
                return "Sorry, there was an error retrieving the information.";
            }
        }

        return result;
    }

    // Отримання інформації про погоду з OpenWeatherMap
    private static async Task<string> GetWeatherInfoAsync(string cityName)
    {
        try
        {
            var response = await HttpClient.GetStringAsync($"http://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={WeatherApiKey}&units=metric");
            var weatherData = JObject.Parse(response);
            var description = weatherData["weather"][0]["description"].ToString();
            var temperature = weatherData["main"]["temp"].ToString();

            return $"Current weather in {cityName}: {description}, {temperature}°C";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error calling Weather API: {ex.Message}");
            return "Sorry, there was an error retrieving the weather information.";
        }
    }

    // Обробка помилок бота
    private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine(exception.Message);
        return Task.CompletedTask;
    }
}

