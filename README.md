# RegIN — Практическая работа №6
## Организация обработки исключений (WPF + SMTP + MySQL + Git)

**Учебное задание:** приобрести навыки работы с SMTP-протоколом, изучить конструкции обработки исключений, отработать командную работу через систему контроля версий.

**Команда:**
- **Beta-Naz** — инфраструктура (БД, почта, модели) + страницы Login и Recovery
- **EldarPukich** — UI-каркас (MainWindow, капча, Material Design) + страницы Regin и Confirmation

### Beta-Naz — `feature/beta-infra-login-recovery`

**Зона ответственности:** работа с базой данных, отправка почты, авторизация и восстановление пароля.

| Файлы | Шаги практики | Что делает |
|-------|---------------|------------|
| `Classes/WorkingDB.cs` | 10–12 | Класс для работы с MySQL (подключение, запросы, закрытие соединений) |
| `Classes/SendMail.cs` | 13–15 | Отправка писем через SMTP Яндекса (пароль приложения) |
| `Classes/User.cs` | 16–17 | Модель зарегистрированного пользователя |
| `Pages/Login.xaml` + `.xaml.cs` | 23–35 | Страница входа: авторизация, капча, блокировка на 3 минуты, ввод логина/пароля |
| `Pages/Recovery.xaml` + `.xaml.cs` | 36–47 | Восстановление пароля: проверка логина, капча, отправка нового пароля на почту |
| SQL-скрипт `db/regin.sql` | 73 | Создание БД `regin` и таблицы `users` |

### EldarPukich — `feature/eldar-ui-regin-confirmation`

**Зона ответственности:** каркас приложения, капча, регистрация и подтверждение по почте.

| Файлы | Шаги практики | Что делает |
|-------|---------------|------------|
| Структура папок + `Images/` | 5–7 | Создание `Classes`, `Elements`, `Images`, `Pages`, перенос картинок |
| `MainWindow.xaml` + `.xaml.cs` | 8–9, 20–21 | Окно с Frame, иконка, размеры; метод `OpenPage`, ссылка `mainWindow` |
| `App.xaml` | 22 | Подключение Material Design In XAML |
| `Elements/ElementCapture.xaml` + `.xaml.cs` | 18–19 | Пользовательский контрол «капча» |
| `Pages/Regin.xaml` + `.xaml.cs` | 58–72 | Регистрация: валидация логина/пароля, выбор и обрезка фото (Aspose.Imaging) |
| `Pages/Confirmation.xaml` + `.xaml.cs` | 48–57 | Подтверждение по коду из письма, таймер повторной отправки 60 сек |

### Внешние библиотеки

| Библиотека | Кто подключает | Шаг |
|-----------|----------------|-----|
| `MySQL.Data` (NuGet) | Beta-Naz | 12 |
| `MaterialDesignThemes` (NuGet) | EldarPukich | 22 |
| `Aspose.Imaging` (NuGet) | EldarPukich | 69 |

---

## Правила работы с Git

### Ветки

```
main
├── feature/beta-infra-login-recovery    ← Beta-Naz
└── feature/eldar-ui-regin-confirmation  ← EldarPukich
```

- `main` — защищённая ветка, только через Pull Request.
- Каждый работает **только в своей ветке** и **только в своих файлах**.

### Коммиты

Формат сообщения (как требует практика):

```
<Что сделано>. <Фамилия>.
```

Примеры:
- `Разработанная структура приложения. Фамилия.`
- `Разработанный класс WorkingDB. Фамилия.`
- `Разработанная страница Login. Фамилия.`

### Порядок работы

1. **Общий старт в `main`** (делается вместе, до расхождения по веткам):
   - создание проекта `RegIN_Фамилия` (шаги 1–4);
   - `.gitignore` (`bin/`, `obj/`, `.vs/`, `packages/`);
   - **заглушки контрактов** — `WorkingDB`, `SendMail`, `User`, `TypeConfirmation` (см. ниже);
   - пустые файлы страниц.
2. Каждый создаёт свою ветку и работает в ней.
3. Регулярно `git pull origin main` (или `git rebase main`), чтобы подтягивать чужие изменения.
4. По завершении этапа — **Pull Request** в `main`, второй участник делает review.
5. Конфликты возможны только в `MainWindow.xaml.cs` и `App.xaml` — их **трогает только EldarPukich**, поэтому конфликтов там быть не должно.

## Дополнительные задания (на «хорошо» / «отлично»)

| Задание | Кто выполняет | Где |
|---------|---------------|-----|
| Оптимизация кода, объединение повторяющихся методов | **Beta-Naz** | `Classes/Helpers.cs` (новый файл), вызовы — из страниц EldarPukich'a |
| Пинкод 4 знака после подтверждения почты | **Beta-Naz** (логика: БД + почта) + **EldarPukich** (UI-поле на `Login`) | `WorkingDB`, `SendMail`, `Login.xaml` |

> Метод `SetNotification` дублируется на `Login`, `Recovery`, `Regin` — при оптимизации выносится в `Classes/Helpers.cs` (создаёт Beta-Naz), страницы только вызывают.
---

## Структура репозитория

```
RegIN_Фамилия/
├── Classes/
│   ├── WorkingDB.cs        ← Beta-Naz
│   ├── SendMail.cs         ← Beta-Naz
│   ├── User.cs             ← общий (модель)
│   └── Helpers.cs          ← Beta-Naz (доп. задание)
├── Elements/
│   └── ElementCapture.xaml + .cs   ← EldarPukich
├── Images/                 ← EldarPukich
├── Pages/
│   ├── Login.xaml + .cs        ← Beta-Naz
│   ├── Recovery.xaml + .cs     ← Beta-Naz
│   ├── Regin.xaml + .cs        ← EldarPukich
│   └── Confirmation.xaml + .cs ← EldarPukich
├── MainWindow.xaml + .cs   ← EldarPukich
├── App.xaml                ← EldarPukich
├── db/
│   └── regin.sql           ← Beta-Naz
├── .gitignore
└── README.md
```
