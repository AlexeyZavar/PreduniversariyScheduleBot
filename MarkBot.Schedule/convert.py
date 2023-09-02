import json

with open('Schedule.original.json', 'r', encoding='utf-8') as f:
    schedule_data = json.load(f)

with open('Schedule.groups.json', 'r', encoding='utf-8') as f:
    groups_data = json.load(f)

new_data = []
for key, val in schedule_data.items():
    groups = \
        next(filter(lambda x: x['first_name'] == key.split()[0] and x['last_name'] == key.split()[1], groups_data))[
            'groups']

    new_data.append({
        'name': key,
        'groups': groups,
        'data': val
    })

with open('Schedule.json', 'w', encoding='utf-8') as f:
    json.dump(new_data, f, ensure_ascii=False, indent=4)
