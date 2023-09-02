import json

with open('./Schedule.json', 'r', encoding='utf-8') as f:
    data = json.load(f)

for item in data:
    for lesson in item['data']:
        if not lesson['teacher'] and lesson['subject'] == 'Английский язык':
            lesson['teacher'] = 'REDACTED FOR OPEN SOURCE'

with open('./Schedule.json', 'w', encoding='utf-8') as f:
    json.dump(data, f, ensure_ascii=False, indent=4)
