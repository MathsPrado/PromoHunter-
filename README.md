# 🎯 PromoHunter

Um sistema de monitoramento de preços assíncrono projetado em **.NET 9**, utilizando os melhores padrões de mercado focados em **Clean Architecture**, **Domain-Driven Design (DDD)** e **Mensageria com RabbitMQ**.

## 🚀 Arquitetura e Stack

A solução é estratificada em 6 projetos focados em uma severa separação de responsabilidades e na aplicação de Clean Code:

- **PromoHunter.Domain**: Núcleo da aplicação com Entidades puras e Regras de Negócio de notificação isoladas (Não possui referências a nenhum framework).
- **PromoHunter.Application**: Contratos, Serviços para Casos de Uso e Eventos distribuídos.
- **PromoHunter.Infrastructure**: Persistência via **Entity Framework Core (SQLite default, ajustável para SQL Server)**, provedores do **MassTransit (RabbitMQ)**, clients do **Telegram.Bot** e scrapers `HtmlAgilityPack`.
- **PromoHunter.Api**: Restful API Minimal e hospedeiro do `BackgroundService` que executa chamadas para Web Scrappers periodicamente (a cada 1 minuto). 
- **PromoHunter.Worker**: Microsserviço independente (RabbitMQ Consumer) que recebe cargas de notificação. Gerencia retentativas (`Polly`/Retry patterns) e limites de taxa de disparo para a API do Telegram (evitando bloqueios por spam).
- **PromoHunter.WebUI**: Front-End Premium construído de forma "Standalone" utilizando **Blazor WebAssembly** com `MudBlazor`. Ele provê uma interface completa, reativa e fluída embasada no estilo _Glassmorphism_.

## 📸 Painel FrontEnd (Blazor)

- ✅ **Dashboard** analítico com visão de monitoramentos vivos
- ✅ **Gestão Ativa** de Ligar/Pausar captadores em tempo real para um produto
- ✅ **Notificação Log Viewer** para avaliar os robôs do Telegram de modo descentralizado
- ✅ **MudChart** com uma visualização interativa mostrando a curva histórica das flutuações de preços nos provedores de e-commerce.

## 🔧 Como Executar (Local Development)

### 1. Iniciar o Hub de Mensageria (RabbitMQ)
Certifique-se de possuir o [Docker](https://www.docker.com/) rodando no seu ambiente e inicie o container:
```bash
docker-compose up -d
```

### 2. Configurar o Token (Worker)
Abra a pasta do projeto `PromoHunter.Worker` e gere (como secret) ou cole o seu `TelegramBotToken` obtido através do [@BotFather](https://telegram.me/BotFather). Sem ele rodando no Container ou nos settings da sua aplicação, o Telegram negará o acesso do Webhook.

### 3. Rodar a Stack em Paralelo
Abra três terminais apontando para a raiz e mande executar. Recomendável o boot do Banco (Mapeando o SQLite primeiramente):

**Terminal 1 (A API Central e Disparo dos Robôs Busca-Preço):**
```bash
cd PromoHunter.Api
dotnet ef database update
dotnet run
```

**Terminal 2 (Recebedor Paralelo das Mensagens p/ Telegram):**
```bash
cd PromoHunter.Worker
dotnet run
```

**Terminal 3 (Renderização do Admin Blazor):**
```bash
cd PromoHunter.WebUI
dotnet run
```

Por fim, abra o link disponibilizado no log (ex.: `localhost:5032`) no seu navegador e adicione a URL para acompanhamento!
