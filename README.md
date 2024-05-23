# TourBot
Цей проект реалізує чат-бот для туристичного агентства у Telegram, використовуючи .NET, OpenAI API та OpenWeatherMap API. Бот надає інформацію про міста та поточні погодні умови, а також деталі про популярні тури.

### Особливості
  - Інформація про місто: надає детальну інформацію про місто за допомогою OpenAI.
  - Погодна інформація: отримує поточну погодну інформацію для міста за допомогою OpenWeatherMap API.
- Тури: відображає доступні тури та дозволяє бронювати їх.
- Інтерактивне меню: використовує кастомні клавіатури Telegram для легкого навігації.
### Вимоги
- .NET SDK
- API токен Telegram Bot
- OpenAI API ключ
- OpenWeatherMap API ключ
### Попередні умови
- .NET SDK: встановлення .NET SDK з офіційного сайту.
- API токен Telegram Bot: створення бота за допомогою BotFather та отримання API токен.
- OpenAI API ключ: реєстрація на OpenAI та отримання API ключ.
- OpenWeatherMap API ключ: реєстрація на OpenWeatherMap та отримання API ключ
### Використання
- /start: Запускає бота та показує головне меню.
- /help: Відображає інформацію про команди.
- City Information: Запитує у користувача назву міста для надання інформації про нього.
- Tours: Відображає доступні тури.
- /book [tour name]: Бронює вказаний тур.

### Структура коду
- Program.cs: Основний файл, що містить логіку бота.
- GetCityInfoAsync(string cityName): Отримує інформацію про місто за допомогою OpenAI API.
- GetWeatherInfoAsync(string cityName): Отримує поточну погодну інформацію за допомогою OpenWeatherMap API.
- HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken): Обробляє вхідні повідомлення та команди.
- HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken): Обробляє помилки під час роботи бота.

### Робота програми 
![image](https://github.com/AnnaSorokina20/TourBot_Lab2/assets/149331565/94c55ffb-d20b-407e-8429-261e7cdb2d2f)
![image](https://github.com/AnnaSorokina20/TourBot_Lab2/assets/149331565/3ee6205f-36fe-4dd5-9ed2-0b2e6fa5acab)
![image](https://github.com/AnnaSorokina20/TourBot_Lab2/assets/149331565/62763ee1-e9fd-4f04-9f84-e024eccd7e37)



