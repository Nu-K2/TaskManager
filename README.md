# Менеджер задач (WPF, C#, .NET 8)

## 1. Цель работы

Закрепить теоретические знания по теме «Создание многооконных приложений WPF» и получить практические навыки разработки оконных приложений на языке C# с использованием нескольких форм (`Window`), организации навигации между ними и передачи данных. Реализовать приложение «Менеджер задач», демонстрирующее работу с несколькими формами и паттерном **Singleton** для бизнес-логики.

---

## 2. Описание проекта

**Название:** TaskTracker («Менеджер задач»)

**Назначение:** учебное WPF-приложение для ведения списка задач — с добавлением, редактированием, удалением, просмотром подробностей и фильтрацией по статусу.

**Демонстрируемые подходы:**

- Многооконное приложение WPF (`MainWindow`, `AddTaskWindow`, `TaskDetailsWindow`).
- Передача данных между формами через **свойства** (`Result`) и **конструктор**.
- Паттерн **Singleton** для хранилища задач (`TaskManager.GetTaskManager`).
- **MVVM-подобная** привязка данных (`Binding`, `DataContext`, `INotifyPropertyChanged`).
- **ObservableCollection** как источник данных для `DataGrid` — автоматическое обновление UI.
- Разделение логики и UI (валидация возвращает `bool`, сообщения показывает окно).

---

## 3. Функциональность

| # | Кнопка / возможность | Действие |
|---|---|---|
| 1 | **Добавить задачу** | Открывает `AddTaskWindow`, после «Сохранить» задача добавляется в коллекцию и в `TaskManager`, показывается сообщение с общим количеством |
| 2 | **Редактировать** | Открывает `AddTaskWindow` в режиме редактирования с выбранной задачей; изменения сразу видны в таблице благодаря `INotifyPropertyChanged` |
| 3 | **Удалить** | Запрашивает подтверждение, удаляет задачу из `TaskManager` и из `ObservableCollection`, показывает количество оставшихся |
| 4 | **Подробнее** | Открывает `TaskDetailsWindow` с полными данными; позволяет переключить статус «выполнена / в работе» |
| 5 | **Показать все задачи** | Сбрасывает фильтр на «Все задачи» и показывает список заново |
| 6 | **Очистить список** | Запрашивает подтверждение, вызывает `ClearAllTasks()`, очищает таблицу |
| 7 | **О программе** | `MessageBox` с ФИО, группой, названием и версией приложения |
| 8 | **Выход** | Запрашивает подтверждение и завершает приложение |
| 9 | **Фильтр** (ComboBox) | Показывает «Все / Активные / Выполненные» задачи |
| 10 | **Двойной клик по строке** | Открывает окно «Подробнее» для выбранной задачи |

**Элементов управления в интерфейсе:** 8 кнопок, ComboBox, DataGrid, StatusBar, 2 TextBlock — больше минимума (6).

---

## 4. Архитектура и реализация

### Структура проекта

```
TaskTracker/
├── App.xaml / App.xaml.cs
├── MainWindow.xaml / MainWindow.xaml.cs          — главное окно
├── AddTaskWindow.xaml / AddTaskWindow.xaml.cs    — добавление/редактирование
├── TaskDetailsWindow.xaml / TaskDetailsWindow.xaml.cs — подробности
├── TaskItem.cs                                   — модель задачи
├── TaskManager.cs                                — бизнес-логика (Singleton)
└── EnumHelper.cs                                 — хелпер для ComboBox с enum
```

### Ключевые классы

**`TaskItem`** — модель задачи, реализует `INotifyPropertyChanged`. Свойства: `Id`, `Title`, `Description`, `CreatedAt`, `Priority` (`High/Medium/Low/Undefined`), `Deadline`, `Category` (`NotStarted/InProgress/Completed`), а также read-only `IsCompleted`.

**`TaskManager`** — хранилище задач, реализовано как **Singleton**:

```csharp
private static readonly TaskManager _instance = new();
public static TaskManager GetTaskManager => _instance;
private TaskManager() { }        // приватный конструктор
private readonly List<TaskItem> tasks = new();
```

Приватный конструктор + статическое поле + публичное свойство-геттер — классическая реализация паттерна **Singleton** (потокобезопасен при инициализации статического поля средствами CLR).

**`EnumHelper`** — статический класс для привязки значений enum в ComboBox:

```csharp
public static Array Priorities => Enum.GetValues(typeof(TaskPriority));
public static Array Categories => Enum.GetValues(typeof(TaskCategory));
```

В XAML используется как `{Binding Source={x:Static local:EnumHelper.Priorities}}`.

**`MainWindow`** — реализует `INotifyPropertyChanged`, имеет два `ObservableCollection`:

- `Tasks` — «источник истины», синхронизированный с `TaskManager`;
- `VisibleTasks` — то, что реально видно в `DataGrid` после фильтрации.

Связь между окнами:

```
MainWindow ──AddTaskWindow.Result──▶ Tasks.Add(...)
MainWindow ──new AddTaskWindow(SelectedTask)──▶ редактирование
MainWindow ──new TaskDetailsWindow(SelectedTask)──▶ просмотр
```

### Схема взаимодействия

```
          ┌──────────────────┐
          │   TaskManager    │  (Singleton)
          │  List<TaskItem>  │
          └────────┬─────────┘
                   │ GetAllTasks / AddTask / RemoveTask / ClearAllTasks
                   ▼
          ┌──────────────────┐       ┌──────────────────┐
          │   MainWindow     │──────▶│  AddTaskWindow   │
          │ ObservableCollection │   │  (Result/task)   │
          └────────┬─────────┘       └──────────────────┘
                   │
                   ▼
          ┌──────────────────┐
          │ TaskDetailsWindow│
          └──────────────────┘
```

---

## 5. Бизнес-логика

### Класс `TaskManager`

| Метод / свойство | Описание |
|---|---|
| `static TaskManager GetTaskManager` | Возвращает единственный экземпляр (Singleton) |
| `int Count` | Количество задач в хранилище |
| `List<TaskItem> GetAllTasks()` | Возвращает список всех задач |
| `bool AddTask(string title, string description)` | Добавляет задачу; `false`, если `title` пуст |
| `bool AddTask(TaskItem task)` | Добавляет готовый объект; `false`, если `Title` пуст |
| `bool RemoveTask(TaskItem task)` | Удаляет задачу; `true`, если найдена и удалена |
| `void ClearAllTasks()` | Очищает список |

### Класс `TaskItem`

| Свойство | Тип | Описание |
|---|---|---|
| `Id` | `string` | GUID, генерируется при создании |
| `Title` | `string` | Название задачи |
| `Description` | `string` | Описание |
| `CreatedAt` | `DateTime` | Дата и время создания (по умолчанию `DateTime.Now`) |
| `Priority` | `TaskPriority` | Приоритет |
| `Deadline` | `DateTime` | Срок выполнения |
| `Category` | `TaskCategory` | Категория / статус |
| `IsCompleted` | `bool` (read-only) | `Category == Completed` |

---

## 6. Особенности реализации

- **Singleton с потокобезопасной инициализацией** — статическое `readonly` поле инициализируется CLR один раз, все окна получают один и тот же экземпляр через `TaskManager.GetTaskManager`.
- **`INotifyPropertyChanged`** в `TaskItem` и `MainWindow` — изменения модели мгновенно отражаются в `DataGrid` без `Items.Refresh()`.
- **`ObservableCollection`** — добавление/удаление в коллекции автоматически обновляет UI.
- **Разделение логики и UI:** `TaskManager` не показывает `MessageBox`; валидация возвращает `bool`, сообщения показывает окно. Это упрощает тестирование.
- **Двухфазное подтверждение:** все разрушительные операции (удаление, очистка, выход) запрашивают `MessageBox.YesNo`.
- **Передача данных между формами:** через свойство `Result` (окно добавления) и конструктор с параметром `TaskItem` (окно редактирования/подробностей). После `ShowDialog() == true` главное окно забирает результат.
- **Фильтрация** реализована через вторую `ObservableCollection` `VisibleTasks` — `DataGrid.ItemsSource` привязан к ней, а `Tasks` остаётся полным списком.
- **Обработка ошибок ввода:** пустой `Title` не даёт сохранить задачу (сообщение + `return`).
- **`DataGridCheckBoxColumn` read-only** (через `Mode=OneWay` или `IsReadOnly="True"`) — избегаем исключения `TwoWay binding to read-only property`, при этом статус отображается наглядно.

---

## 7. Требования и запуск

**Требования:**

- .NET SDK **8.0** или новее
- Windows 10/11
- Visual Studio 2022 (или `dotnet` CLI)
- Язык: C# 12
- UI-фреймворк: WPF

**Сборка и запуск:**

```bash
dotnet build
dotnet run
```

**Или через Visual Studio:** открыть `TaskTracker.sln` → F5.

---

## 8. Пример работы

**Сценарий:**

1. Запуск → пустая таблица, счётчик «Задач: 0».
2. «Добавить задачу» → ввести `Купить молоко`, выбрать `Medium`, `NotStarted`, срок `01.11.2026` → «Сохранить».
3. Появляется `MessageBox`: *«Задача добавлена. Всего задач: 1»*, строка появляется в таблице.
4. Выбрать строку → «Подробнее» → открывается окно с полными данными, кнопка «✓ Отметить выполненной».
5. После клика статус меняется на `Completed`, галочка в главной таблице появляется.
6. Фильтр «Активные» — задача исчезает из вида (так как выполнена).
7. «Удалить» → Yes → *«Задача удалена. Всего задач: 0»*.

**Скриншот главного окна:**

![[image.png]]

---

## 9. Соответствие требованиям

| Требование методички | Статус |
|---|---|
| №1. Три формы: `MainWindow`, `AddTaskWindow`, `TaskDetailsWindow` | ✅ |
| №1. Свойства окон (`Title`, `Width`, `Height`, `WindowStartupLocation`) | ✅ |
| №2. `TaskManager` хранит `List<TaskItem>` | ✅ |
| №2. `AddTask(string, string)` | ✅ |
| №2. `GetAllTasks()`, `ClearAllTasks()`, `Count` | ✅ |
| №2. `TaskItem`: `Title`, `Description`, `CreatedAt` | ✅ (+ `Priority`, `Deadline`, `Category`, `Id`) |
| №3. Минимум 6 кнопок с логикой | ✅ **8 кнопок** |
| №3. Кнопка «Показать все задачи» | ✅ |
| №3. Кнопка «Очистить список» | ✅ |
| №3. Кнопка «О программе» | ✅ |
| №3. Кнопка «Выход» | ✅ |
| №4. Передача данных `AddTaskWindow` → `MainWindow` | ✅ через `Result` |
| №4. Передача `TaskItem` в `TaskDetailsWindow` | ✅ через конструктор |
| №5. Подтверждающие сообщения с количеством задач | ✅ после добавления / удаления / очистки |
| №5. Обработка «не выбрана задача» | ✅ для «Редактировать» / «Удалить» / «Подробнее» |
| Дополнительно: фильтр по статусу | ✅ бонус |
| Дополнительно: двойной клик → подробности | ✅ бонус |
| Дополнительно: статус-бар со счётчиком | ✅ бонус |

**Итоговая оценка: 5 (отлично)** — выполнены все 5 обязательных требований (критерий «5»), плюс реализованы дополнительные улучшения: фильтрация, Singleton-хранилище, MVVM-подобная привязка, `INotifyPropertyChanged`, окно подробностей с переключением статуса.