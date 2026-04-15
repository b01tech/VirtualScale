# VirtualScale

Sistema de pesagem digital multi-célula com comunicação serial em tempo real.

## Arquitetura

```
┌─────────────────────────────────────────────────────────────────┐
│                     Desktop Application                          │
│  ┌─────────────────────┐         ┌─────────────────────────┐  │
│  │   Angular 21        │◄────────│     Tauri 2 (Rust)       │  │
│  │   SignalR Client    │         │     Desktop Window      │  │
│  └─────────┬───────────┘         └─────────────────────────┘  │
│            │                                                        │
└────────────┼────────────────────────────────────────────────────┘
             │ HTTP / SignalR
┌────────────▼────────────────────────────────────────────────────┐
│                      Backend (.NET 10)                            │
│  ┌─────────────────┐  ┌───────────────┐  ┌─────────────────┐   │
│  │  VirtualScale   │  │  SignalR Hub  │  │  Serial Handler │   │
│  │  .Api           │◄─┤  (WebSocket) │──┤  Worker         │   │
│  └─────────────────┘  └───────────────┘  └────────┬────────┘   │
│                                                    │            │
└────────────────────────────────────────────────────┼────────────┘
                                                     │ Serial
┌────────────────────────────────────────────────────▼────────────┐
│                   Microcontroller                                 │
│  ┌─────────────────┐  ┌─────────────┐                         │
│  │  Arduino Nano   │──┤  4x HX711   │──┤  4x Load Cells       │
│  │  (PlatformIO)   │  │  ADC Module │                         │
│  └─────────────────┘  └─────────────┘                         │
└─────────────────────────────────────────────────────────────────┘
```

## Stack Tecnológica

| Componente | Tecnologia | Versão |
|------------|------------|--------|
| Backend API | ASP.NET Core | 10.0 |
| Backend Worker | .NET Worker | 10.0 |
| Frontend Web | Angular | 21.2.8 |
| Desktop Runtime | Tauri | 2.x |
| Microcontroller | Arduino (PlatformIO) | - |
| Load Cell ADC | HX711 | - |

## Estrutura do Projeto

```
VirtualScale/
├── backend/                      # .NET Solution
│   ├── src/
│   │   ├── VirtualScale.Api/     # API + SignalR Hubs
│   │   ├── VirtualScale.Domain/  # Entidades e lógica de negócio
│   │   └── VirtualScale.Worker/  # Serial communication worker
│   └── test/
│       └── VirtualScale.Domain.Test/
├── frontend/                    # Angular + Tauri
│   ├── src/
│   │   └── app/
│   │       ├── features/         # Páginas e componentes
│   │       │   ├── scale/        # Página principal da balança
│   │       │   ├── calibration/  # Calibração
│   │       │   ├── settings/    # Configurações
│   │       │   └── login/       # Autenticação
│   │       └── shared/          # Componentes compartilhados
│   └── src-tauri/               # Tauri desktop wrapper
├── digital-converter/           # Firmware Arduino
│   ├── src/
│   │   └── main.cpp
│   └── include/
│       ├── AdcHandler.h         # Comunicação HX711
│       ├── SerialHandler.h      # Transmissão serial
│       └── Hardware.h           # Definição de pinos
└── docs/
    └── diagrams/
        └── flux.excalidraw
```

## Funcionalidades

### Backend
- API REST para configuração da balança
- SignalR Hubs para comunicação em tempo real
- Worker para comunicação serial com microcontrolador
- Processamento de dados de múltiplas células de carga
- Algoritmo de calibração (zero e span)
- Filtro digital configurável (nível 0-10)

### Frontend
- Interface Angular com SignalR para atualização em tempo real
- Display de peso bruto, líquido e Tara
- Indicadores de estabilidade e estado de conexão
- Controle de filtro digital
- Equalização de células de carga
- Configurações de calibração

### Microcontrolador
- Leitura simultânea de 4 células de carga
- ADC 24-bit via HX711
- Transmissão serial a 5Hz
- Pino SCK compartilhado (D6) + 4 DTs individuais (D2-D5)

## Começando

### Pré-requisitos

- .NET 10 SDK
- Node.js 20+
- Angular CLI
- Rust toolchain (para Tauri)
- PlatformIO
- Arduino Nano ou compatível

### Backend

```bash
cd backend/src/VirtualScale.Api
dotnet run
```

O backend estará disponível em `http://localhost:5000`.

### Frontend Web

```bash
cd frontend
npm install
npm run dev
```

### Desktop App (Tauri)

```bash
cd frontend
npm run tauri dev
```

### Firmware

```bash
cd digital-converter
pio run -t upload
pio device monitor -b 115200
```

## API Endpoints

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/scale/status` | Status atual da balança |
| GET | `/api/scale/loadcells` | Dados das células de carga |
| POST | `/api/scale/tare` | Aplicar Tara |
| POST | `/api/scale/zero` | Zerar balança |
| POST | `/api/scale/calibrate/zero` | Calibrar ponto zero |
| POST | `/api/scale/calibrate/span` | Calibrar span |
| GET | `/api/serial/ports` | Listar portas seriais disponíveis |
| POST | `/api/serial/connect` | Conectar a uma porta |
| POST | `/api/serial/disconnect` | Desconectar |

## SignalR Hubs

- `/hubs/scale` - Atualizações de peso em tempo real
- `/hubs/loadcells` - Dados individuais das células
- `/hubs/serial` - Status da conexão serial

## Configuração Serial

```json
// appsettings.json
{
  "Serial": {
    "Port": "COM3",
    "BaudRate": 115200,
    "Timeout": 1000
  }
}
```

## Calibração

1. Conectar todas as células de carga
2. Deixar balança em repouso
3. Calibrar Zero (sem peso na balança)
4. Colocar peso de referência conhecido
5. Calibrar Span (com peso de referência)

## Formato de Dados Serial

O microcontrolador envia no formato:
```
{id}:{valor_bruto},00\r\n
```

Exemplo:
```
1:126032,00\r\n
2:127686,00\r\n
3:126719,00\r\n
4:125811,00\r\n
```

## Roadmap

- [ ] Persistência de calibração
- [ ] Histórico de pesagens
- [ ] Relatórios e exportação
- [ ] Autenticação de usuários
- [ ] Suporte a mais tipos de células
- [ ] Testes E2E

## Licença

MIT
