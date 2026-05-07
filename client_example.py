import requests
import hashlib
import platform
import subprocess

# Configurações do App (Pegue no Dashboard)
APP_ID = "SEU_APP_ID_AQUI"
APP_SECRET = "SEU_APP_SECRET_AQUI"
BASE_URL = "http://localhost:5000/api/auth"

def get_hwid():
    # Exemplo simples de HWID para Windows
    try:
        cmd = "wmic csproduct get uuid"
        uuid = subprocess.check_output(cmd, shell=True).decode().split('\n')[1].strip()
        return hashlib.sha256(uuid.encode()).hexdigest()
    except:
        return platform.node()

def register(username, password, license_key):
    payload = {
        "appId": APP_ID,
        "appSecret": APP_SECRET,
        "username": username,
        "password": password,
        "licenseKey": license_key,
        "hwid": get_hwid()
    }
    res = requests.post(f"{BASE_URL}/register", json=payload)
    print(res.json())

def login(username, password):
    payload = {
        "appId": APP_ID,
        "appSecret": APP_SECRET,
        "username": username,
        "password": password,
        "hwid": get_hwid()
    }
    res = requests.post(f"{BASE_URL}/login", json=payload)
    if res.status_code == 200:
        data = res.json()
        print(f"Login bem-sucedido! Expira em: {data['expiry']}")
    else:
        print(f"Erro: {res.json()}")

# Exemplo de uso:
# register("usuario1", "senha123", "CHAVE-DE-LICENCA")
# login("usuario1", "senha123")
