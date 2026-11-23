# Messenger Backend

Серверная часть мессенджера: микросервисы на .NET 10, PostgreSQL, RabbitMQ, SignalR.

Тема диплома: «Проектирование и разработка серверной части мессенджера на основе
микросервисной архитектуры с асинхронным обменом сообщениями с использованием RabbitMQ».

## Структура

```text
src/
├── Messenger.Gateway/     # YARP: единая точка входа, JWT, CORS, WebSocket proxy
├── Messenger.User/        # аккаунты, аутентификация, профиль, поиск
├── Messenger.Chat/        # чаты, участники, сообщения, история
├── Messenger.Realtime/    # SignalR, доставка
├── Messenger.Contracts/   # integration events, envelope
├── Messenger.Messaging/   # RabbitMQ, Outbox/Inbox (следующий этап)
└── Messenger.Shared/      # JWT, health, общие хелперы

deploy/                    # production compose, init-db, контракт для infra
```

Внутри каждого сервиса — `Api`, `Features`, `Domain`, `Infrastructure`.
Один проект на сервис, vertical slices.

## Локальный запуск

Нужны Docker Desktop и Docker Compose v2.

```bash
cp .env.example .env
docker compose up --build
```

| Что | URL |
|---|---|
| Gateway | `http://localhost:5000` |
| User Service | `http://localhost:5001` |
| Chat Service | `http://localhost:5002` |
| Realtime Service | `http://localhost:5003` |
| Health | `/health` у каждого сервиса |
| RabbitMQ Management | `http://localhost:15672` |
| PostgreSQL | `127.0.0.1:5432` |

Если `5432`, `5672` или `15672` уже заняты локальным Postgres/RabbitMQ,
поменяйте `POSTGRES_PORT`, `RABBITMQ_PORT`, `RABBITMQ_MANAGEMENT_PORT`
в `.env`.

Проверка:

```bash
curl http://localhost:5000/health
curl http://localhost:5001/health
curl http://localhost:5002/health
curl http://localhost:5003/health
```

Остановить:

```bash
docker compose down
```

Удалить данные (БД и RabbitMQ):

```bash
docker compose down -v
```

## Сервисы и порты

| Сервис | Внутренний порт | Локальный порт | Наружу в проде |
|---|---|---|---|
| gateway | 8080 | 5000 | через reverse proxy infra |
| user-service | 8080 | 5001 | нет |
| chat-service | 8080 | 5002 | нет |
| realtime-service | 8080 | 5003 | нет |
| postgres | 5432 | 127.0.0.1:5432 | нет |
| rabbitmq | 5672 / 15672 | 127.0.0.1 | нет |

## Конфигурация

Все секреты — только через env. `.env` в `.gitignore`; в репозитории лежит
только `.env.example`.

Основные переменные:

| Переменная | Назначение |
|---|---|
| `POSTGRES_PASSWORD` | пароль суперпользователя Postgres |
| `USER_DB_PASSWORD` | пароль роли `user_service` |
| `CHAT_DB_PASSWORD` | пароль роли `chat_service` |
| `RABBITMQ_DEFAULT_USER/PASS` | доступ к RabbitMQ |
| `JWT_SECRET` | HS256-секрет, одинаковый для всех сервисов |
| `JWT_ISSUER` / `JWT_AUDIENCE` | проверка JWT |

`deploy/init-db.sh` создаёт базы `messenger_users`, `messenger_chat`
и роли `user_service`, `chat_service`. Скрипт идемпотентен.

## Пользователи БД

Один экземпляр PostgreSQL, но у каждого сервиса — своя база и своя роль.
Сервис не может читать чужую базу, межсервисных `JOIN` и FK нет.

| Роль | База | Права | Пароль из env |
|---|---|---|---|
| `postgres` | `postgres` | суперпользователь: создаёт базы и роли | `POSTGRES_PASSWORD` |
| `user_service` | `messenger_users` | владелец своей базы, нет доступа к `messenger_chat` | `USER_DB_PASSWORD` |
| `chat_service` | `messenger_chat` | владелец своей базы, нет доступа к `messenger_users` | `CHAT_DB_PASSWORD` |

- `deploy/init-db.sh` создаёт роли и базы, если их нет, и обновляет пароли
  ролей из env. Повторный запуск безопасен.
- Connection strings передаются сервисам через env:
  - `ConnectionStrings__UsersDb` — только для User;
  - `ConnectionStrings__ChatDb` — только для Chat.
- Таблицы в этих базах появятся позже через EF Core миграции.
  Скрипт создаёт только базы и роли.
- Пароли не хранятся в репозитории; в `.env.example` только placeholder’ы.

## Docker-образы

Четыре multi-stage Dockerfile: `src/Messenger.*/Dockerfile`.

- build: `mcr.microsoft.com/dotnet/sdk:10.0`;
- runtime: `mcr.microsoft.com/dotnet/aspnet:10.0`;
- non-root `app`, порт `8080`, `curl` для healthcheck;
- платформа: `linux/amd64`.

## Контракты API

Swagger UI в dev:

- `http://localhost:5001/swagger` — User Service;
- `http://localhost:5002/swagger` — Chat Service, только для локальной отладки.

Сейчас контроллеры возвращают `501 Not Implemented`: это контракт для
параллельной работы с фронтом, бизнес-логика появится на следующих этапах.

## Релиз и деплой

Релиз создаётся по git-тегу:

```bash
git tag v0.1.0
git push origin v0.1.0
```

GitHub Actions:

1. собирает 4 образа и пушит в `ghcr.io/messagerteam/backend-*`;
2. собирает `backend-deploy-v0.1.0.zip`;
3. публикует zip в GitHub Release.

Артефакт содержит только deploy-файлы и ссылки на образы — без исходников
и без `build:`. В его `.env.example` уже подставлен `IMAGE_TAG` этого релиза,
поэтому infra не выбирает тег вручную. Контракт для infra-репозитория — в
[`deploy/README.md`](deploy/README.md).

Схема продакшена — same-origin:

```text
https://app.example.com/
├── /              → frontend dist
├── /api/v1/*      → gateway (127.0.0.1:8080)
└── /hubs/chat     → gateway (WebSocket)
```

TLS и reverse proxy — на infra. Postgres и RabbitMQ входят в deploy-набор,
наружу не публикуются. Мобильное приложение собирается через EAS и на сервер
не деплоится.
