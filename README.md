# Помощник кладовщика

## Стек технологий

- Backend: ASP.NET Core 3.1, Web API
- Frontend: Vue 3, Vite, Ant Design Vue
- БД: PostgreSQL
- ORM: Entity Framework Core 3.1
- Тесты: xUnit, Moq

## Установка

1. Конфигурируем `.env` в `assistant-storekeeper-backend` (копируем из `.env.example`):

   ```
   POSTGRES_HOST=
   POSTGRES_PORT=
   POSTGRES_DATABASE=
   POSTGRES_USERNAME=
   POSTGRES_PASSWORD=
   ```

2. Конфигурируем `.env` в `assistant-storekeeper-frontend` (копируем из `.env.example`):

   ```
   VITE_API_URL=
   ```

3. Создаём базу данных в PostgreSQL:

   ```bash
   createdb assistant_storekeeper
   ```

## Запуск

### Backend

1. `cd assistant-storekeeper-backend`
2. `dotnet ef database update`
3. `dotnet build`
4. `dotnet run`

При старте выполняются миграции и заполнение справочников (3 склада, 7 номенклатур).

### Frontend

1. `cd assistant-storekeeper-frontend`
2. `yarn install`
3. `yarn dev`

## URL

- Frontend: http://localhost:5173
- Backend API: http://localhost:5000


## Тесты

```bash
cd assistant-storekeeper-backend.Tests
dotnet test
```

## Что можно бы было добавить:

- Ролевую систему, кладовщик, менеджер и т.д и аутентификацию
- Экспорт перемещений в excel, word, pdf
- eslint для отслеживания правильности написания кода
