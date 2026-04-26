"""Точка входа для запуска Flask-приложения в режиме разработки."""

from app import create_app

app = create_app()

if __name__ == '__main__':
    app.run(debug=True, host='0.0.0.0')

# Для production рекомендуется запуск через WSGI-сервер.
