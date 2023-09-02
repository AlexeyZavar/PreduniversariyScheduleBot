import json

with open('./Schedule.json', 'r', encoding='utf-8') as f:
    data = json.load(f)

teachers = set()
for item in data:
    for day in item['data']:
        if day['teacher']:
            for teacher in day['teacher'].replace('/', ',').split(','):
                teachers.add(teacher.strip())

with open('./NameMappings.json', 'r', encoding='utf-8') as f:
    data2 = json.load(f)

res = {k: data2.get(k, '') for k in sorted(teachers)}
print(res)
