"""Инициализация Flask-приложения, базы данных и регистрация маршрутов."""

from flask import Flask
from app.database import db

def create_app():
    app = Flask(__name__)
    app.config['SQLALCHEMY_DATABASE_URI'] = 'sqlite:///../instance/variants.db'
    app.config['SQLALCHEMY_TRACK_MODIFICATIONS'] = False
    
    db.init_app(app)
    
    # Модели импортируются внутри контекста, чтобы корректно создать таблицы при старте.
    with app.app_context():
        from app import models
        db.create_all()
    
    from app import routes
    app.register_blueprint(routes.bp)
    
    return app

# Особенность: таблицы создаются автоматически при запуске приложения.