import psycopg2
import threading
import random

def create_table():
    conn = psycopg2.connect(
        dbname='tabela_teste',
        user='admin',
        password='Teste@123',
        host='localhost',
        port=5432
    )
    cur = conn.cursor()
    cur.execute("""
        CREATE TABLE IF NOT EXISTS tabela_stress (
            id SERIAL PRIMARY KEY,
            valor INTEGER
        )
    """)
    conn.commit()
    cur.close()
    conn.close()

def worker():
    try:
        conn = psycopg2.connect(
            dbname='tabela_teste',
            user='admin',
            password='Teste@123',
            host='localhost',
            port=5432
        )
        cur = conn.cursor()
        for i in range(1000):
            cur.execute("INSERT INTO tabela_stress (valor) VALUES (%s)", (random.randint(1, 1000),))
            if (i + 1) % 100 == 0:
                conn.commit()
        conn.commit()
        cur.close()
        conn.close()
    except Exception as e:
        print(f"Erro na thread: {e}")

if __name__ == "__main__":
    create_table()
    threads = []
    num_threads = 20

    for _ in range(num_threads):
        t = threading.Thread(target=worker)
        t.start()
        threads.append(t)

    for t in threads:
        t.join()

    print("Stress test finalizado.")


#antes de rodar instalar:
#sudo apt-get install python3-psycopg2
