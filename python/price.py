import json

infos=[]
with open(r'D:\code\聯成_宏碁專案\Lamborghini\python\info_json\Aventador.json', 'r', encoding='utf-8') as f:
    data = json.load(f)

for d in data:
    infos.append({"title":d["title"],"price":None})
    



with open(r'D:\code\聯成_宏碁專案\Lamborghini\python\info_json\ConceptCars.json', 'r', encoding='utf-8') as f:
    data = json.load(f)

for d in data:
    infos.append({"title":d["title"],"price":None})
    


with open(r'D:\code\聯成_宏碁專案\Lamborghini\python\info_json\Huracán.json', 'r', encoding='utf-8') as f:
    data = json.load(f)

for d in data:
    infos.append({"title":d["title"],"price":None})


with open(r'D:\code\聯成_宏碁專案\Lamborghini\python\info_json\LimitedModel.json', 'r', encoding='utf-8') as f:
    data = json.load(f)

for d in data:
    infos.append({"title":d["title"],"price":None})


with open(r'D:\code\聯成_宏碁專案\Lamborghini\python\info_json\sportCar.json', 'r', encoding='utf-8') as f:
    data = json.load(f)

for d in data:
    infos.append({"title":d["title"],"price":None})



# with open(r'D:\code\聯成_宏碁專案\Lamborghini\python\Lambo_price.json', 'w', encoding='utf-8') as f:
#     json.dump(infos,f,indent=4,ensure_ascii=False)