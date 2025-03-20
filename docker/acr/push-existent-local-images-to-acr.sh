#!/bin/bash
#cmd windows: bash push-existent-local-images-to-acr.sh
# Script para subir imagens local  existentes para o ACR

set -e  # Faz o script parar se qualquer comando falhar
ACR_NAME="casoftregistry"
ACR_LOGIN_SERVER="$ACR_NAME.azurecr.io"

# Lista das imagens que deseja enviar para o ACR
IMAGES=(
    "quay.io/prometheusmsteams/prometheus-msteams",
    "cesarags/casoft-store-web:v1"
    "cesarags/casoft-store-api-gateway:v1"
    "cesarags/casoft-store-basket:v1"
    "cesarags/casoft-store-catalog:v1"
    "cesarags/casoft-store-order:v1"
    "cesarags/casoft-store-product:v1"
    "cesarags/casoft-store-billing:v1"
    "cesarags/casoft-store-auth:v1"
    "cesarags/casoft-store-discount:v1"
)

# Login no Azure ACR
az acr login --name $ACR_NAME

# Loop para taggear e enviar as imagens
for IMAGE in "${IMAGES[@]}"; do
    IMAGE_NAME=$(echo $IMAGE | cut -d':' -f1)  # Extrai o nome da imagem sem a tag
    TAG=$(echo $IMAGE | cut -d':' -f2)         # cut -d':' -f2 captura tudo o que vem depois dos :

    echo "🔹 Criando tag para $IMAGE_NAME..."
    docker tag $IMAGE $ACR_LOGIN_SERVER/$IMAGE_NAME:$TAG

    echo "🔹 Enviando $IMAGE_NAME para o ACR..."
    docker push $ACR_LOGIN_SERVER/$IMAGE_NAME:$TAG

    echo "✅ Imagem $IMAGE_NAME:$TAG enviada com sucesso!"
done

echo "🚀 Todas as imagens foram enviadas para o ACR!"
