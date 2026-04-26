"""Маршруты веб-интерфейса и API для работы с вариантами, заданиями и подгрузка чата."""

from flask import render_template, request, jsonify, Blueprint
from app.database import db
from app.models import Variant, Task
from sqlalchemy import func
import requests
import uuid
import urllib3
import os
from collections import defaultdict

urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

bp = Blueprint('main', __name__)

chat_sessions = defaultdict(list)

GIGACHAT_CLIENT_ID = "019d6d64-4888-7f09-82f5-7ade33b455da"
GIGACHAT_SECRET = os.getenv("GIGACHAT_SECRET", "")

@bp.route('/')
def index():
    return render_template('index.html')

@bp.route('/editor')
def editor_home():
    return render_template('editor_home.html')

@bp.route('/editor/variants')
def editor_variants_list():
    return render_template('variants_list.html')

@bp.route('/editor/variant/<int:variant_id>')
def editor_variant_edit(variant_id):
    return render_template('variant_edit.html', variant_id=variant_id)

@bp.route('/student')
def student_home():
    return render_template('student_home.html')

@bp.route('/student/variant/<int:variant_id>')
def student_variant(variant_id):
    return render_template('student_variant.html', variant_id=variant_id)

@bp.route('/api/variants', methods=['GET'])
def get_variants():
    completed = request.args.get('completed')
    query = Variant.query
    if completed is not None:
        completed = completed.lower() == 'true'
        query = query.filter_by(completed=completed)
    variants = query.all()
    return jsonify([{'id': v.id, 'name': v.name, 'completed': v.completed} for v in variants])

@bp.route('/api/variants', methods=['POST'])
def create_variant():
    data = request.json
    variant = Variant(name=data['name'])
    db.session.add(variant)
    db.session.commit()
    return jsonify({'id': variant.id, 'name': variant.name, 'completed': variant.completed})

@bp.route('/api/variants/<int:variant_id>', methods=['GET'])
def get_variant(variant_id):
    variant = Variant.query.get_or_404(variant_id)
    tasks = [{'id': t.id, 'order': t.order, 'text': t.text, 'image_data': t.image_data, 'key': t.key, 'student_answer': t.student_answer} for t in variant.tasks]
    return jsonify({'id': variant.id, 'name': variant.name, 'completed': variant.completed, 'tasks': tasks})

@bp.route('/api/variants/<int:variant_id>', methods=['PUT'])
def update_variant(variant_id):
    variant = Variant.query.get_or_404(variant_id)
    data = request.json
    variant.name = data.get('name', variant.name)
    variant.completed = data.get('completed', variant.completed)
    db.session.commit()
    return jsonify({'id': variant.id, 'name': variant.name, 'completed': variant.completed})

@bp.route('/api/variants/<int:variant_id>', methods=['DELETE'])
def delete_variant(variant_id):
    variant = Variant.query.get_or_404(variant_id)
    db.session.delete(variant)
    db.session.commit()
    return '', 204

@bp.route('/api/variants/<int:variant_id>/tasks', methods=['POST'])
def add_task(variant_id):
    variant = Variant.query.get_or_404(variant_id)
    data = request.json
    max_order = db.session.query(func.max(Task.order)).filter(Task.variant_id == variant_id).scalar() or -1
    task = Task(
        variant_id=variant_id, 
        order=max_order+1, 
        text=data.get('text', ''), 
        image_data=data.get('image_data', ''),
        key=data.get('key', '')
    )
    db.session.add(task)
    db.session.commit()
    return jsonify({'id': task.id, 'order': task.order, 'text': task.text, 'image_data': task.image_data, 'key': task.key})

@bp.route('/api/tasks/<int:task_id>', methods=['PUT'])
def update_task(task_id):
    task = Task.query.get_or_404(task_id)
    data = request.json
    
    if 'text' in data:
        task.text = data['text']
    if 'image_data' in data:
        task.image_data = data['image_data']
    if 'key' in data:
        task.key = data['key']
    
    db.session.commit()
    return jsonify({'id': task.id, 'order': task.order, 'text': task.text, 'image_data': task.image_data, 'key': task.key})

@bp.route('/api/tasks/<int:task_id>', methods=['DELETE'])
def delete_task(task_id):
    task = Task.query.get_or_404(task_id)
    db.session.delete(task)
    db.session.commit()
    return '', 204

@bp.route('/api/variants/<int:variant_id>/complete', methods=['POST'])
def complete_variant(variant_id):
    try:
        variant = Variant.query.get_or_404(variant_id)
        
        if variant.completed:
            return jsonify({'error': 'Variant already completed'}), 400
        
        data = request.json
        if not data:
            return jsonify({'error': 'No JSON data received'}), 400
            
        answers = data.get('answers', {})
        
        for task_id_str, answer in answers.items():
            try:
                task_id = int(task_id_str)
                task = Task.query.get(task_id)
                if task and task.variant_id == variant_id:
                    task.student_answer = answer.strip() if answer else ''
            except (ValueError, TypeError):
                continue
        
        variant.completed = True
        db.session.commit()
        
        results = []
        for task in variant.tasks:
            is_correct = False
            if task.key and task.student_answer:
                is_correct = task.student_answer.strip().lower() == task.key.strip().lower()
            results.append({
                'id': task.id,
                'text': task.text,
                'image_data': task.image_data,
                'key': task.key,
                'student_answer': task.student_answer,
                'is_correct': is_correct
            })
        
        return jsonify({
            'success': True,
            'results': results
        })
        
    except Exception as e:
        db.session.rollback()
        return jsonify({'error': str(e)}), 500

@bp.route('/api/chat', methods=['POST'])
def chat():
    data = request.json
    user_message = data.get('message', '')
    
    session_id = request.remote_addr
    
    if not user_message:
        return jsonify({'reply': 'Напишите вопрос.'})
    
    print(f"📝 Сессия {session_id}: {user_message}")
    
    history = chat_sessions[session_id]
    
    messages = [
        {"role": "system", "content": "Ты полезный помощник по учебе. Отвечай кратко, понятно, по-русски. Помни предыдущие вопросы и ответы в этом диалоге. Используй переносы строк для форматирования кода или списков."}
    ]
    
    for msg in history[-10:]:
        messages.append(msg)
    
    messages.append({"role": "user", "content": user_message})
    
    auth_key = GIGACHAT_SECRET
    if not auth_key:
        return jsonify({'reply': 'Сервис ИИ не настроен: добавьте переменную окружения GIGACHAT_SECRET.'})
    
    try:
        token_response = requests.post(
            "https://ngw.devices.sberbank.ru:9443/api/v2/oauth",
            headers={
                "Content-Type": "application/x-www-form-urlencoded",
                "Accept": "application/json",
                "RqUID": str(uuid.uuid4()),
                "Authorization": f"Basic {auth_key}"
            },
            data={"scope": "GIGACHAT_API_PERS"},
            verify=False,
            timeout=30
        )
        
        print(f"🔑 Токен статус: {token_response.status_code}")
        
        if token_response.status_code != 200:
            return jsonify({'reply': f'Ошибка авторизации: {token_response.status_code}'})
        
        token = token_response.json().get("access_token")
        
        if not token:
            return jsonify({'reply': 'Не удалось получить токен доступа'})
        
        chat_response = requests.post(
            "https://gigachat.devices.sberbank.ru/api/v1/chat/completions",
            headers={
                "Content-Type": "application/json",
                "Accept": "application/json",
                "Authorization": f"Bearer {token}"
            },
            json={
                "model": "GigaChat",
                "messages": messages,
                "temperature": 0.7,
                "max_tokens": 1000
            },
            verify=False,
            timeout=60
        )
        
        print(f"🤖 GigaChat статус: {chat_response.status_code}")
        
        if chat_response.status_code == 200:
            result = chat_response.json()
            reply = result.get("choices", [{}])[0].get("message", {}).get("content", "Не удалось получить ответ")
            
            chat_sessions[session_id].append({"role": "user", "content": user_message})
            chat_sessions[session_id].append({"role": "assistant", "content": reply.strip()})
            
            if len(chat_sessions[session_id]) > 20:
                chat_sessions[session_id] = chat_sessions[session_id][-20:]
            
            print(f"📤 Ответ: {reply[:100]}...")
            return jsonify({'reply': reply.strip()})
        else:
            error_msg = f"Ошибка GigaChat: {chat_response.status_code}"
            print(f"❌ {error_msg}")
            return jsonify({'reply': error_msg})
            
    except requests.exceptions.Timeout:
        return jsonify({'reply': '⏰ Сервер не отвечает. Попробуйте позже.'})
    except Exception as e:
        print(f"❌ Ошибка: {e}")
        return jsonify({'reply': f'Ошибка: {str(e)}'})

@bp.route('/api/explain', methods=['POST'])
def explain():
    data = request.json
    task_text = data.get('task_text', '')
    correct_answer = data.get('correct_answer', '')
    student_answer = data.get('student_answer', '')
    
    if not task_text or not correct_answer:
        return jsonify({'explanation': 'Недостаточно данных для объяснения.'})
    
    prompt = f"""Задание: {task_text}
Правильный ответ: {correct_answer}
Ответ ученика: {student_answer if student_answer else '(не дан)'}

Объясни ученику, почему правильный ответ именно такой. Дай краткое, понятное объяснение. Если ответ ученика был неверным, объясни его ошибку."""
    
    auth_key = GIGACHAT_SECRET
    if not auth_key:
        return jsonify({'explanation': 'Сервис ИИ не настроен: добавьте переменную окружения GIGACHAT_SECRET.'})
    
    try:
        token_response = requests.post(
            "https://ngw.devices.sberbank.ru:9443/api/v2/oauth",
            headers={
                "Content-Type": "application/x-www-form-urlencoded",
                "Accept": "application/json",
                "RqUID": str(uuid.uuid4()),
                "Authorization": f"Basic {auth_key}"
            },
            data={"scope": "GIGACHAT_API_PERS"},
            verify=False,
            timeout=30
        )
        
        if token_response.status_code != 200:
            return jsonify({'explanation': 'Ошибка авторизации'})
        
        token = token_response.json().get("access_token")
        
        chat_response = requests.post(
            "https://gigachat.devices.sberbank.ru/api/v1/chat/completions",
            headers={
                "Content-Type": "application/json",
                "Accept": "application/json",
                "Authorization": f"Bearer {token}"
            },
            json={
                "model": "GigaChat",
                "messages": [
                    {"role": "system", "content": "Ты добрый и понятный учитель. Объясняй кратко, по делу, на русском."},
                    {"role": "user", "content": prompt}
                ],
                "temperature": 0.7,
                "max_tokens": 500
            },
            verify=False,
            timeout=60
        )
        
        if chat_response.status_code == 200:
            result = chat_response.json()
            explanation = result.get("choices", [{}])[0].get("message", {}).get("content", "Не удалось получить объяснение")
            return jsonify({'explanation': explanation})
        else:
            return jsonify({'explanation': 'Не удалось получить объяснение от ИИ'})
            
    except Exception as e:
        return jsonify({'explanation': f'Ошибка: {str(e)}'})

# история чата хранится в памяти процесса и не сохраняется между перезапусками. Пофиксить.