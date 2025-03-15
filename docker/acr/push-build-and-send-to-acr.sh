#!/bin/bash
# Script para subir imagens local pela primeira vez para o ACR

# Defina o nome do seu ACR

set -e  # Faz o script parar se qualquer comando falhar
ACR_NAME="casoftregistry"
ACR_LOGIN_SERVER="$ACR_NAME.azurecr.io"

# Lista das imagens que deseja enviar para o ACR
IMAGES=(
    "casoft-store-web:v1"
    "casoft-store-api-gateway:v1"
    "casoft-store-basket:v1"
    "casoft-store-catalog:v1"
    "casoft-store-order:v1"
    "casoft-store-product:v1"
    "casoft-store-billing:v1"
    "casoft-store-auth:v1"
    "casoft-store-discount:v1"
)

# Login no Azure ACR
az acr login --name $ACR_NAME

# Loop para construir, taggear e enviar as imagens
for IMAGE in "${IMAGES[@]}"; do
    echo "🔹 Construindo a imagem $IMAGE..."
    docker build -t $IMAGE .

    echo "🔹 Criando tag para $IMAGE..."
    docker tag $IMAGE $ACR_LOGIN_SERVER/$IMAGE:latest

    echo "🔹 Enviando $IMAGE para o ACR..."
    docker push $ACR_LOGIN_SERVER/$IMAGE:latest

    echo "✅ Imagem $IMAGE enviada com sucesso!"
done

echo "🚀 Todas as imagens foram enviadas para o ACR!"
