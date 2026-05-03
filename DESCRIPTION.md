# BusinessTracker — Описание проекта

Персональный кабинет для финансового и логистического мониторинга: учёт продаж, выручки, рабочих смен и расходов на
питание сотрудников.

---

## Архитектура

Проект построен на **Layered Clean Architecture** с чётким разделением ответственностей:

```
BusinessTracker.Domain        ← Доменный слой (бизнес-логика, без зависимостей)
BusinessTracker.Common        ← Общие интерфейсы сервисов и репозиториев
BusinessTracker.Data          ← Слой данных (EF Core, PostgreSQL, репозитории)
BusinessTracker.Api           ← ASP.NET Core API (миграции БД, контроллеры)
BusinessTracker.Console       ← Консольный загрузчик данных из MSSQL
BusinessTracker.Tests         ← Тесты (NUnit 4)
```

### Слой Domain (`BusinessTracker.Domain`)

Не имеет внешних зависимостей. Содержит:

| Папка                | Содержимое                                                                                                                                                                           |
|----------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `Core/Abstractions/` | `ILoadingSettingsRepository`, `IJournalRowsRepository`, `IRevenueReportRepository`, `ISalesReportRepository`, `IWorkScheduleReportRepository`, `IModel`, `IId`, `IDto`, `IErrorText` |
| `Core/Enums/`        | `TransactionType`, `EmployeeRole`                                                                                                                                                    |
| `Core/Attributes/`   | `TemplateAttribute` (regex-валидация), `ColumnMappingAttribute` (маппинг из ADO.NET)                                                                                                 |
| `Models/`            | `DomainModel` (базовый класс с самовалидацией), `Organization`, `Employee`, `Category`, `Nomenclature`, `Transaction`, `LoadingSettings`                                             |
| `Models/Dto/`        | `JournalRowDto`, `RevenueReportRowDto`, `SalesReportRowDto`, `WorkScheduleReportRowDto`                                                                                              |
| `Logic/`             | `RevenueReportBuilder`, `SalesReportBuilder`, `WorkScheduleReportBuilder`, `DataMapper`, `ValidationHelper`                                                                          |

**Самовалидация** — `DomainModel.Validate()` рекурсивно проверяет:

1. Стандартные атрибуты (`[Required]`, `[Range]`, `[StringLength]`)
2. `[TemplateAttribute]` (регулярные выражения)
3. Вложенные `DomainModel`-объекты

**`JournalRowDto`** — плоская запись из журнала клиентской программы (legacy MSSQL). Поля:

| Поле               | Источник        | Описание                                     |
|--------------------|-----------------|----------------------------------------------|
| `Code`             | `journalid`     | Уникальный код транзакции                    |
| `TypeCode`         | `transtype`     | Код типа транзакции                          |
| `TransTypeName`    | `TransTypeName` | Наименование типа                            |
| `ReceiptNumber`    | `checknum`      | Номер чека                                   |
| `ProductCode`      | вычисляется     | Код продукта                                 |
| `CategoryCode`     | вычисляется     | Код категории                                |
| `EmployeeCode`     | вычисляется     | Код сотрудника                               |
| `Period`           | `dater`         | Дата и время транзакции                      |
| `Quantity`         | `quantity`      | Количество                                   |
| `Price`            | `price`         | Цена                                         |
| `Discount`         | `sumdiscount`   | Скидка                                       |
| `EmployeeName`     | —               | Имя сотрудника (расширенное поле)            |
| `CategoryName`     | —               | Наименование категории (расширенное поле)    |
| `NomenclatureName` | —               | Наименование номенклатуры (расширенное поле) |

**Построители отчётов** — статические классы. Принимают `IEnumerable<Transaction>` и возвращают Dto:

| Построитель                 | Логика                                                                                                                           |
|-----------------------------|----------------------------------------------------------------------------------------------------------------------------------|
| `RevenueReportBuilder`      | Группировка по дате; вся сумма попадает в `CashAmount` (разбивка по типу оплаты будет реализована после интеграции MSSQL-данных) |
| `SalesReportBuilder`        | Группировка по номенклатуре, суммирование `Quantity`/`Amount`/`Discount`                                                         |
| `WorkScheduleReportBuilder` | Сопоставление `StartShift` и `StopShift` по сотруднику, незакрытая смена — `ShiftEnd = null`                                     |

---

### Слой Common (`BusinessTracker.Common`)

Общие интерфейсы, разделяемые между Api и Console:

| Интерфейс              | Описание                                          |
|------------------------|---------------------------------------------------|
| `ILoadingService`      | Push/PushAsync — приём и обработка транзакций     |
| `ISavingService`       | Save/SaveAsync — сохранение транзакций            |
| `IClientRepository<T>` | GetRows — чтение записей из клиентской БД (MSSQL) |
| `IHandler<T>`          | Маркерный интерфейс репозиториев                  |

---

### Слой Data (`BusinessTracker.Data`)

| Папка         | Содержимое                                            |
|---------------|-------------------------------------------------------|
| `Models/`     | EF-сущности (зеркало таблиц БД), включая `JournalRow` |
| `Logics/`     | `LoadingSettingsRepository`, `JournalRowsRepository`  |
| `Extensions/` | `RegistryExtension` — DI-регистрация зависимостей     |
| `Migrations/` | SQL-скрипты DbUp                                      |

**DI-регистрация** (`RegistryExtension`) поддерживает два варианта вызова:

```csharp
// С IConfiguration — строка подключения из ConnectionStrings:DefaultConnection
services.RegisterBusinessTrackerData(configuration);

// С явной строкой подключения (проброс из модели настроек)
services.RegisterBusinessTrackerData(connectionString);
```

Регистрируются как `Scoped`:

- `BusinessTrackerContext`
- `ILoadingSettingsRepository` → `LoadingSettingsRepository`
- `IJournalRowsRepository` → `JournalRowsRepository`

---

### Слой API (`BusinessTracker.Api`)

Точка входа: `Program.cs`

- Читает настройки из `ApiOptions` (через `IOptions<ApiOptions>`)
- Применяет DbUp-миграции при старте
- Регистрирует зависимости через `RegisterBusinessTrackerData`
- Слушает на `http://0.0.0.0:8000`

**Модель настроек** (`ApiOptions`):

```json
{
  "ApiOptions": {
    "PostgreConnectionString": "Host=localhost;Port=5433;...",
    "MsSqlConnectionString": ""
  }
}
```

**Документация API (Scalar):**

При запуске API доступны:

| URL                                     | Описание                              |
|-----------------------------------------|---------------------------------------|
| `http://localhost:8000/scalar/v1`       | Интерактивный UI для тестирования API |
| `http://localhost:8000/openapi/v1.json` | OpenAPI-спецификация (JSON)           |

Scalar подключён через `Microsoft.AspNetCore.OpenApi` + `Scalar.AspNetCore` (Swashbuckle несовместим с Microsoft.OpenApi
3.x).

**Контроллеры:**

| Контроллер          | Метод | Маршрут             | Описание                                          |
|---------------------|-------|---------------------|---------------------------------------------------|
| `JournalController` | POST  | `/api/journal/push` | Приём списка транзакций от клиентского приложения |

Тело запроса (`PushTransactionsRequest`):

```json
{
  "organizationId": "<uuid>",
  "transactions": [
    {
      ...
    }
  ]
}
```

Контроллер резолвит организацию из БД по `organizationId` и вызывает `ILoadingService.PushAsync`.

**Сервис `LoadingService`:**

1. Загружает `LoadingSettings` из репозитория (фильтр по `StartPosition`)
2. Отсекает уже обработанные транзакции (`Code < StartPosition`)
3. Сохраняет новые строки в `JournalRows` через `IJournalRowsRepository`
4. Обновляет `StartPosition` = max Code батча

---

### Слой Console (`BusinessTracker.Console`)

Консольное приложение с полноценным DI-контейнером.

**Модель настроек** (`ConsoleOptions`):

```json
{
  "ConsoleOptions": {
    "MsSqlConnectionString": "Server=...;Database=PersonalAccount;...",
    "PostgreConnectionString": "Host=localhost;Port=5433;...",
    "ApiBaseUrl": "http://localhost:8000"
  }
}
```

**DI-регистрация** (`Console/Extensions/RegistryExtension`):

- `ConsoleOptions` через `IOptions<ConsoleOptions>`
- `IClientRepository<JournalRowDto>` → `JournalRepository`
- `HttpClient` с именем `"api"`, BaseAddress = `ApiBaseUrl`

**Цикл работы** (`Program.cs`):

1. Открывает соединение с MSSQL через `ConsoleOptions.MsSqlConnectionString`
2. Загружает записи журнала через `IClientRepository<JournalRowDto>.GetRows()`
3. Отправляет POST `/api/journal/push` через `HttpClient`
4. Смещает `StartPosition` для следующего батча
5. Ожидает 1 час и повторяет

---

## Схема базы данных

```mermaid
erDiagram
    Organizations {
        UUID Id PK
        VARCHAR Name
        VARCHAR Inn UK
        TEXT Address
        JSONB LoadOptions
    }

    Users {
        UUID Id PK
        VARCHAR Name
        TEXT Password
    }

    LinksUserOrganizations {
        UUID Id PK
        UUID UserId FK
        UUID OrganizationId FK
    }

    Categories {
        UUID Id PK
        VARCHAR Name
        UUID OwnerId FK
    }

    Nomenclatures {
        UUID Id PK
        VARCHAR Name
        UUID CategoryId FK
    }

    Employees {
        UUID Id PK
        VARCHAR Name
        VARCHAR PhoneNumber
        UUID OwnerId FK
        INT Role
    }

    Transactions {
        UUID Id PK
        INT Type
        UUID OwnerId FK
        TIMESTAMPTZ TransactionDate
        UUID NomenclatureId FK
        UUID EmployeeId FK
        NUMERIC Amount
        NUMERIC Quantity
        NUMERIC Discount
    }

    JournalRows {
        UUID Id PK
        UUID OrganizationId FK
        BIGINT Code
        INT TypeCode
        TEXT TransTypeName
        INT ReceiptNumber
        BIGINT ProductCode
        BIGINT CategoryCode
        INT EmployeeCode
        TIMESTAMP Period
        FLOAT8 Quantity
        FLOAT8 Price
        FLOAT8 Discount
        INT RawId
        INT RawLoginId
        TEXT EmployeeName
        TEXT CategoryName
        TEXT NomenclatureName
    }

    Organizations ||--o{ Categories: "владеет"
    Organizations ||--o{ Employees: "владеет"
    Organizations ||--o{ Transactions: "владеет"
    Organizations ||--o{ LinksUserOrganizations: "привязана"
    Organizations ||--o{ JournalRows: "владеет"
    Users ||--o{ LinksUserOrganizations: "привязан"
    Categories ||--o{ Nomenclatures: "содержит"
    Nomenclatures ||--o{ Transactions: "участвует"
    Employees ||--o{ Transactions: "выполняет"
```

### Перечисления

**`TransactionType`**

| Значение   | Код | Описание        |
|------------|-----|-----------------|
| Sale       | 1   | Продажа         |
| Return     | 2   | Возврат         |
| Change     | 3   | Сдача           |
| StartShift | 4   | Начало смены    |
| StopShift  | 5   | Окончание смены |

**`EmployeeRole`**

| Значение      | Описание                 |
|---------------|--------------------------|
| Manager       | Менеджер (только чтение) |
| Administrator | Полный доступ            |

---

## Миграции базы данных

Используется **DbUp** — миграции применяются при старте `BusinessTracker.Api`.
Скрипты хранятся как Embedded Resources в `BusinessTracker.Data/Migrations/` и выполняются в алфавитном порядке:

| Скрипт                         | Описание                                                                   |
|--------------------------------|----------------------------------------------------------------------------|
| `init.sql`                     | Создание всех таблиц и индексов                                            |
| `seed_init.sql`                | Начальные данные (2 организации, 1 сотрудник, 1 категория, 1 номенклатура) |
| `upgrade_001_journal_rows.sql` | Таблица `JournalRows` для хранения плоских записей из MSSQL-журнала        |

### Шаги для первоначального развёртывания БД

```bash
# 1. Поднять PostgreSQL через Docker
cd _infra && docker-compose up -d

# 2. Запустить API — DbUp применит все миграции автоматически
dotnet run --project BusinessTracker.Api
```

### Добавление новой миграции

1. Создать файл `BusinessTracker.Data/Migrations/<name>.sql`
   Имя должно сортироваться **после** всех существующих скриптов (например, `upgrade_<описание>.sql`).

2. Убедиться, что файл попадает под `<EmbeddedResource Include="Migrations\*.*">` в `.csproj` (настроено глобально).

3. Запустить `dotnet run --project BusinessTracker.Api` — DbUp применит только новые скрипты.

### Сброс и пересоздание схемы

```bash
# Выполнить restore.sql через psql (удаляет и пересоздаёт БД)
psql -h localhost -p 5433 -U admin -d postgres -f _infra/restore.sql

# Затем запустить миграции заново
dotnet run --project BusinessTracker.Api
```

---

## Слой тестов (`BusinessTracker.Tests`)

| Файл                               | Тип            | Описание                                        |
|------------------------------------|----------------|-------------------------------------------------|
| `TestApplication.cs`               | Модульный      | Валидация доменных моделей                      |
| `TestCurrentApplication.cs`        | Модульный      | Версия приложения                               |
| `TestRevenueReportBuilder.cs`      | Модульный      | Построитель отчёта «Выручка»                    |
| `TestSalesReportBuilder.cs`        | Модульный      | Построитель отчёта «Продажи»                    |
| `TestWorkScheduleReportBuilder.cs` | Модульный      | Построитель отчёта «График работы»              |
| `TestLoadingSettings.cs`           | Интеграционный | Save/Load для `ILoadingSettingsRepository` (DI) |
| `TestLoadingService.cs`            | Интеграционный | Push через `IJournalRowsRepository` (DI)        |
| `TestDbContext.cs`                 | Интеграционный | Базовые запросы к БД                            |

> Интеграционные тесты требуют запущенной PostgreSQL (`docker-compose up`).

Все интеграционные тесты используют DI-контейнер (`ServiceProvider`) через `RegisterBusinessTrackerData(configuration)`.
`TestLoadingService` дополнительно гарантирует наличие таблицы `JournalRows` и сбрасывает `LoadOptions` организации
перед каждым тестом.

---

## Step 4: Фоновая обработка данных (нормализация)

### Задача

После приёма сырых строк журнала в `JournalRows` запускается фоновый пайплайн, который:

1. Читает необработанные записи на основе `LoadingSettings`
2. Определяет новые категории, номенклатуру и сотрудников (отсутствующих в БД)
3. Сохраняет найденные сущности в БД
4. Строит нормализованные `Transactions` со всеми связями

---

### Новые интерфейсы (`BusinessTracker.Domain/Core/Abstractions/`)

| Интерфейс                   | Группа                         | Описание                                                                             |
|-----------------------------|--------------------------------|--------------------------------------------------------------------------------------|
| `INewJournalRowsProvider`   | Получение данных по настройкам | Читает из `JournalRows` записи с `Code >= StartPosition`, не более `BatchSize`       |
| `INewCategoriesDetector`    | Определение новых сущностей    | Находит категории из `JournalRowDto.CategoryName/CategoryCode`, отсутствующие в БД   |
| `INewNomenclaturesDetector` | Определение новых сущностей    | Находит позиции из `JournalRowDto.NomenclatureName/ProductCode`, отсутствующие в БД  |
| `INewEmployeesDetector`     | Определение новых сущностей    | Находит сотрудников из `JournalRowDto.EmployeeName/EmployeeCode`, отсутствующих в БД |
| `ICategoryRepository`       | Запись данных                  | Сохранение и чтение категорий                                                        |
| `INomenclatureRepository`   | Запись данных                  | Сохранение и чтение номенклатуры                                                     |
| `IEmployeeRepository`       | Запись данных                  | Сохранение и чтение сотрудников                                                      |

---

### UML: Диаграмма интерфейсов

```mermaid
classDiagram
    direction TB

    class INewJournalRowsProvider {
        <<interface>>
        +GetUnprocessedAsync(LoadingSettings, CancellationToken) Task~IEnumerable~JournalRowDto~~
    }

    class INewCategoriesDetector {
        <<interface>>
        +DetectAsync(Organization, IEnumerable~JournalRowDto~, CancellationToken) Task~IEnumerable~Category~~
    }

    class INewNomenclaturesDetector {
        <<interface>>
        +DetectAsync(IEnumerable~Category~, IEnumerable~JournalRowDto~, CancellationToken) Task~IEnumerable~Nomenclature~~
    }

    class INewEmployeesDetector {
        <<interface>>
        +DetectAsync(Organization, IEnumerable~JournalRowDto~, CancellationToken) Task~IEnumerable~Employee~~
    }

    class ICategoryRepository {
        <<interface>>
        +SaveAsync(IEnumerable~Category~, CancellationToken) Task
        +GetByOwnerAsync(Guid, CancellationToken) Task~IEnumerable~Category~~
    }

    class INomenclatureRepository {
        <<interface>>
        +SaveAsync(IEnumerable~Nomenclature~, CancellationToken) Task
        +GetByCategoryAsync(Guid, CancellationToken) Task~IEnumerable~Nomenclature~~
    }

    class IEmployeeRepository {
        <<interface>>
        +SaveAsync(IEnumerable~Employee~, CancellationToken) Task
        +GetByOwnerAsync(Guid, CancellationToken) Task~IEnumerable~Employee~~
    }

    class ILoadingSettingsRepository {
        <<interface>>
        +Save(LoadingSettings, CancellationToken) Task
        +Load(Branch, CancellationToken) Task~LoadingSettings~
    }

    class IJournalRowsRepository {
        <<interface>>
        +SaveAsync(Guid, IEnumerable~JournalRowDto~, CancellationToken) Task
    }

    INewJournalRowsProvider ..> ILoadingSettingsRepository: зависит от
    INewCategoriesDetector ..> ICategoryRepository: использует для проверки
    INewNomenclaturesDetector ..> INomenclatureRepository: использует для проверки
    INewEmployeesDetector ..> IEmployeeRepository: использует для проверки
    IJournalRowsRepository <.. INewJournalRowsProvider: читает строки
```

---

### UML: Алгоритм фоновой обработки

```mermaid
sequenceDiagram
    participant BW as BackgroundWorker
    participant SR as ILoadingSettingsRepository
    participant JP as INewJournalRowsProvider
    participant CD as INewCategoriesDetector
    participant CR as ICategoryRepository
    participant ND as INewNomenclaturesDetector
    participant NR as INomenclatureRepository
    participant ED as INewEmployeesDetector
    participant ER as IEmployeeRepository
    BW ->> SR: Load(branch)
    SR -->> BW: LoadingSettings
    BW ->> JP: GetUnprocessedAsync(settings)
    JP -->> BW: IEnumerable~JournalRowDto~
    Note over BW: Шаг 1 — Категории
    BW ->> CD: DetectAsync(org, rows)
    CD ->> CR: GetByOwnerAsync(orgId)
    CR -->> CD: existing Category[]
    CD -->> BW: new Category[]
    BW ->> CR: SaveAsync(newCategories)
    Note over BW: Шаг 2 — Номенклатура
    BW ->> CR: GetByOwnerAsync(orgId)
    CR -->> BW: allCategories
    BW ->> ND: DetectAsync(allCategories, rows)
    ND ->> NR: GetByCategoryAsync(categoryId)
    NR -->> ND: existing Nomenclature[]
    ND -->> BW: new Nomenclature[]
    BW ->> NR: SaveAsync(newNomenclatures)
    Note over BW: Шаг 3 — Сотрудники
    BW ->> ED: DetectAsync(org, rows)
    ED ->> ER: GetByOwnerAsync(orgId)
    ER -->> ED: existing Employee[]
    ED -->> BW: new Employee[]
    BW ->> ER: SaveAsync(newEmployees)
    Note over BW: Шаг 4 — Финализация
    BW ->> SR: Save(settings с обновлённым StartPosition)
```

---

### Предполагаемый алгоритм реализации

Реализация `BackgroundWorker` выполняет следующие шаги:

1. **Загрузка настроек** — `ILoadingSettingsRepository.Load(branch)` возвращает `LoadingSettings` с текущим
   `StartPosition` и `BatchSize`.

2. **Чтение необработанных строк** — `INewJournalRowsProvider.GetUnprocessedAsync(settings)` делает выборку из таблицы
   `JournalRows` по условию `Code >= StartPosition LIMIT BatchSize`.

3. **Определение и сохранение новых категорий**
    - `INewCategoriesDetector.DetectAsync(org, rows)`: читает существующие категории через
      `ICategoryRepository.GetByOwnerAsync`, сопоставляет по `CategoryCode + CategoryName`, возвращает разницу.
    - `ICategoryRepository.SaveAsync(newCategories)`: сохраняет новые категории.

4. **Определение и сохранение новой номенклатуры**
    - После шага 3 перечитываются все категории (включая только что добавленные).
    - `INewNomenclaturesDetector.DetectAsync(allCategories, rows)`: сопоставляет по `ProductCode + NomenclatureName`,
      привязывает к категории.
    - `INomenclatureRepository.SaveAsync(newNomenclatures)`.

5. **Определение и сохранение новых сотрудников**
    - `INewEmployeesDetector.DetectAsync(org, rows)`: сопоставляет по `EmployeeCode + EmployeeName`.
    - `IEmployeeRepository.SaveAsync(newEmployees)`.

6. **Обновление позиции** — `StartPosition` устанавливается равным максимальному `Code` обработанного батча, настройки
   сохраняются через `ILoadingSettingsRepository.Save`.

> Два варианта запуска: **непосредственная обработка** (inline в `PushAsync`) и **отложенная** (фоновый сервис
`IHostedService`). Step 4 предполагает реализацию обоих вариантов с замером производительности через бенчмарки.

---

## Стек технологий

| Компонент              | Технология                            |
|------------------------|---------------------------------------|
| Язык                   | C# / .NET 10                          |
| ORM                    | Entity Framework Core 10              |
| База данных (основная) | PostgreSQL 16 (порт 5433)             |
| База данных (источник) | MSSQL (порт 1433, устаревший)         |
| Миграции               | DbUp                                  |
| Тестирование           | NUnit 4                               |
| HTTP-клиент            | System.Net.Http.HttpClient            |
| Контейнеризация        | Docker / docker-compose               |
| API-документация       | Microsoft.AspNetCore.OpenApi + Scalar |
