#!/bin/bash

# Baixa as imagens do DockerHub para maquina local e envia para o ACR

set -e  # Faz o script parar se qualquer comando falhar
DOCKERHUB_ALIAS="casoftlabs"
ACR_NAME="casoftregistry"
ACR_LOGIN_SERVER="$ACR_NAME.azurecr.io"

# Lista das imagens que deseja buscar do Docker Hub e enviar para o ACR
IMAGES=(
    "${DOCKERHUB_ALIAS}/projeto-caotico:v3"
)

# Login no Azure ACR
docker login # ja tem que ter feito o login no docker hub via terminal

# Login no Azure ACR
az acr login --name $ACR_NAME

# Loop para buscar, taggear e enviar as imagens
for IMAGE in "${IMAGES[@]}"; do
    IMAGE_NAME=$(echo $IMAGE | cut -d':' -f1)  # Nome sem tag
    TAG=$(echo $IMAGE | cut -d':' -f2)         # Apenas a tag

    echo "🔹 Baixando a imagem do Docker Hub: $IMAGE..."
    docker pull $IMAGE

    echo "🔹 Criando tag para o ACR: $ACR_LOGIN_SERVER/$IMAGE_NAME:$TAG..."
    docker tag $IMAGE $ACR_LOGIN_SERVER/$IMAGE_NAME:$TAG

    echo "🔹 Enviando $IMAGE_NAME para o ACR..."
    docker push $ACR_LOGIN_SERVER/$IMAGE_NAME:$TAG

    echo "✅ Imagem $IMAGE_NAME:$TAG enviada com sucesso!"
done

echo "🚀 Todas as imagens foram buscadas do Docker Hub e enviadas para o ACR!"
