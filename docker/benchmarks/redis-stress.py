import redis
import time
import random

SERIE = 'metric:test'

r = redis.Redis(host='localhost', port=6379)

# (Re)cria a série temporal com DUPLICATE_POLICY LAST
try:
    r.delete(SERIE)  # Remove a série se já existir
    r.execute_command('TS.CREATE', SERIE, 'DUPLICATE_POLICY', 'LAST')
    print(f"Série temporal '{SERIE}' criada com DUPLICATE_POLICY LAST.")
except redis.exceptions.ResponseError as e:
    print(f"Erro ao criar a série: {e}")

# Stress test: insere 10.000 pontos rapidamente
for i in range(10000):
    value = random.uniform(0, 100)
    try:
        # Timestamp automático (*) para evitar duplicidade
        r.execute_command('TS.ADD', SERIE, '*', value)
    except redis.exceptions.ResponseError as e:
        print(f"Erro ao adicionar ponto: {e}")
    if i % 1000 == 0:
        print(f"{i} pontos inseridos...")

print("Stress test finalizado!")
