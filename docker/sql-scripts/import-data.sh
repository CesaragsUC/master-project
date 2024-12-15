#!/bin/bash


sleep 5s
#rodar o comando para criar o banco
/opt/mssql-tools/bin/sqlcmd -S localhost,1433 -U SA -P "2Secure*Password2" -i quartznet.sql

echo "Script executado com sucesso!"