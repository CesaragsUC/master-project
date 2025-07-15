import pymongo
import random

client = pymongo.MongoClient(
    "mongodb://cesar:cesar@localhost:27017/?authSource=admin"
)
db = client["stressdb"]
collection = db["test"]

for i in range(10000):
    doc = {"value": i}
    collection.insert_one(doc)

# antes de tudo
# sudo apt-get update
# sudo apt-get install python-is-python3
# pip install pymongo
# python mongo_stress.py
# Esse script insere 10 mil documentos sequenciais, simulando carga de escrita.