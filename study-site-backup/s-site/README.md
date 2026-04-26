# Study Site Backup

## Краткое описание
Небольшое Flask-приложение для работы с учебными вариантами: создание заданий, прохождение варианта учеником, проверка ответов и получение подсказок через GigaChat.

## Структура проекта
- `run.py` — точка входа приложения.
- `app/__init__.py` — инициализация Flask и базы данных.
- `app/routes.py` — веб-маршруты и API.
- `app/models.py` — модели `Variant` и `Task`.
- `app/database.py` — экземпляр SQLAlchemy.
- `app/templates/` — HTML-шаблоны интерфейса.
- `app/static/` — статические файлы (CSS).
- `requirements.txt` — Python-зависимости.
- `Dockerfile`, `docker-compose.yml` — запуск в контейнере.

## Как запустить
### Локально
1. Установить зависимости:
   `pip install -r requirements.txt`
2. Запустить приложение:
   `python run.py`
3. Открыть в браузере:
   `http://127.0.0.1:5000`

### Через Docker Compose
1. Выполнить:
   `docker-compose up --build`
2. Открыть:
   `http://127.0.0.1:5000`

## Основные зависимости
- Flask
- Flask-SQLAlchemy
- Werkzeug
- requests

## Дополнительные замечания
- База данных SQLite создается автоматически в `instance/variants.db`.
- История чата хранится в памяти процесса и очищается после перезапуска сервиса.
