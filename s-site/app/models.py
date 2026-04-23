"""ORM-модели вариантов и заданий для хранения учебных данных."""

from app.database import db

class Variant(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    name = db.Column(db.String(200), nullable=False)
    completed = db.Column(db.Boolean, default=False)
    tasks = db.relationship('Task', backref='variant', cascade='all, delete-orphan', order_by='Task.order')

class Task(db.Model):
    id = db.Column(db.Integer, primary_key=True)
    variant_id = db.Column(db.Integer, db.ForeignKey('variant.id'), nullable=False)
    order = db.Column(db.Integer, default=0)
    text = db.Column(db.Text, default='')
    image_data = db.Column(db.Text)
    key = db.Column(db.Text, default='')
    student_answer = db.Column(db.Text, default='')

# Особенность: удаление варианта каскадно удаляет связанные задания.