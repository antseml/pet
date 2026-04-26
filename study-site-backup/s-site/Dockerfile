# Сборка контейнера приложения Flask с зависимостями и сертификатом GigaChat.
FROM python:3.9-slim

WORKDIR /app

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY gigachat.crt /usr/local/share/ca-certificates/gigachat.crt
RUN update-ca-certificates

COPY . .

ENV FLASK_APP=run.py
EXPOSE 5000
CMD ["flask", "run", "--host=0.0.0.0"]

# Контейнер предназначен для запуска веб-сервиса на порту 5000.