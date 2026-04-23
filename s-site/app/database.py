"""Конфигурация и единая точка доступа к экземпляру SQLAlchemy."""

from flask_sqlalchemy import SQLAlchemy

db = SQLAlchemy()

# Экземпляр используется во всех модулях приложения через импорт `db`.
