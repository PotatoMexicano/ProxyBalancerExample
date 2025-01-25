# Reverse Proxy com YARP

Este projeto é um **Reverse Proxy** simples, implementado com a biblioteca **YARP (Yet Another Reverse Proxy)** e configurado para fazer logs utilizando **Serilog**.

## 🎯 Objetivo
O objetivo deste projeto é fornecer um proxy reverso básico para rotear requisições HTTP, com suporte para logging estruturado para monitorar e depurar o tráfego.

## 🚀 Funcionalidades
- Roteamento de requisições HTTP para servidores backend configurados.
- Suporte a múltiplos destinos de backend.
- Configuração dinâmica via `appsettings.json`.
- Logging estruturado com **Serilog**.

## 🛠️ Tecnologias Utilizadas
- **Linguagem:** [C#](https://learn.microsoft.com/pt-br/dotnet/csharp/)
- **Framework:** ASP.NET Core
- **Bibliotecas:**
  - [YARP](https://microsoft.github.io/reverse-proxy/) (proxy reverso)
  - [Serilog](https://serilog.net/) (logging)

## 🏗️ Estrutura do Projeto
```plaintext
/
├── .dockerignore              # Configuração para Docker
├── .gitignore                 # Arquivos ignorados pelo Git
├── appsettings.json           # Configurações do proxy e logs
├── Dockerfile                 # Dockerfile para containerizar o projeto
├── Program.cs                 # Ponto de entrada da aplicação
├── ReverseProxyLoadBalance.sln # Solução do projeto
└── README.md                  # Documentação do projeto
