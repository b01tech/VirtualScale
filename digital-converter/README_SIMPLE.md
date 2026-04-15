# HX711 Digital Converter - Versão Simplificada

Sistema de leitura de 4 células de carga via módulos HX711 com transmissão serial de valores brutos.

## 📋 Requisitos

- Arduino Nano (ATmega328)
- 4x HX711 Modules
- 4x Load Cells
- PlatformIO ou Arduino IDE
- Dependency: `robtillaart/HX711@^0.6.3`

## 🔌 Pinagem - ✅ FUNCIONANDO

**Configuração com SCK compartilhado:**

```
Pino D6:  SCK (Clock) - COMPARTILHADO pelos 4 módulos
Pino D5:  DT1 (Data Cell 1)
Pino D4:  DT2 (Data Cell 2)
Pino D3:  DT3 (Data Cell 3)
Pino D2:  DT4 (Data Cell 4)
GND:      Comum (todos os GND)
VCC:      5V (todos os VCC)
```

**Total de pinos utilizados:** 5 (1 SCK + 4 DT)

### Diagrama de Conexão

```
Arduino Nano          HX711 Module 1      Arduino Nano          HX711 Module 2
├─ D6 (SCK) ──────────→ CLK               ├─ D6 (SCK) ──────────→ CLK (compartilhado)
└─ D5 (DT1) ──────────→ DOUT              └─ D4 (DT2) ──────────→ DOUT
                      GND → GND                                  GND → GND
                      VCC → 5V                                   VCC → 5V

Arduino Nano          HX711 Module 3      Arduino Nano          HX711 Module 4
├─ D6 (SCK) ──────────→ CLK               ├─ D6 (SCK) ──────────→ CLK (compartilhado)
└─ D3 (DT3) ──────────→ DOUT              └─ D2 (DT4) ──────────→ DOUT
                      GND → GND                                  GND → GND
                      VCC → 5V                                   VCC → 5V
```

## 📤 Formato de Saída Serial

```
1:126032,00\r\n
2:127686,00\r\n
3:126719,00\r\n
4:125811,00\r\n
1:126032,00\r\n
2:127686,00\r\n
3:126719,00\r\n
4:125811,00\r\n
...
```

**Formato:**

- `id` (1-4): Identificador do módulo HX711
- `value`: Valor bruto do ADC (número inteiro)
- `Formato decimal`: `,00` (sempre com 2 casas decimais)
- **Terminador:** `\r\n` (0x0D 0x0A - CR+LF)

**Intervalo entre leituras:** 200ms (configurável)

## ⚡ Compilar e Upload

**Com PlatformIO:**

```bash
pio run -t upload -e nanoatmega328new
```

**Com Arduino IDE:**

1. Abra `src/main.cpp`
2. Instale HX711: Sketch → Include Library → Manage Libraries
3. Selecione: Arduino Nano
4. Upload

## 🔍 Monitor Serial

```bash
pio device monitor -b 115200
```

Ou use Arduino IDE: Tools → Serial Monitor (115200 baud)

## ⚙️ Configuração

Editar em `include/GlobalConfiguration.h`:

```cpp
#define SERIAL_BAUD_RATE 115200      // Velocidade serial
#define DELAY_IN_MS 200               // Intervalo entre leituras (ms)
```

Editar em `include/Hardware.h` (se precisar alterar os pinos):

```cpp
#define LOADCELL_SCK 6                // Pino SCK (COMPARTILHADO por todos)
#define LOADCELL_DT1 5                // DT Cell 1
#define LOADCELL_DT2 4                // DT Cell 2
#define LOADCELL_DT3 3                // DT Cell 3
#define LOADCELL_DT4 2                // DT Cell 4
```

## 📊 Estrutura do Projeto

```
digital-converter/
├── platformio.ini
├── include/
│   ├── AdcHandler.h           // Gerencia HX711 modules
│   ├── AdcHandler.cpp
│   ├── GlobalConfiguration.h  // Configurações globais
│   ├── Hardware.h             // Definição de pinos
│   └── SerialHandler.h
├── src/
│   ├── main.cpp              // Loop principal
│   └── SerialHandler.cpp     // Transmissão serial
└── README_SIMPLE.md
```

## 🔧 API

### AdcHandler

```cpp
adcHandler.begin();                    // Inicializar módulos HX711
adcHandler.readAllValues(values);      // Ler os 4 sensores
adcHandler.readValue(cellIndex);       // Ler sensor individual (1-4)
adcHandler.isInitialized();            // Verificar inicialização
```

### SerialHandler

```cpp
serialHandler.begin();                         // Inicializar serial
serialHandler.sendLoadCellValue(id, value);   // Enviar id:value,00\r\n
```

## 📝 Example Usage

```cpp
#include "AdcHandler.h"
#include "SerialHandler.h"

AdcHandler adc;
SerialHandler serial;
long values[4];

void setup() {
    serial.begin();
    adc.begin();
}

void loop() {
    adc.readAllValues(values);

    for (int i = 0; i < 4; i++) {
        serial.sendLoadCellValue(i + 1, values[i]);
    }

    delay(200);
}
```

## 🆘 Troubleshooting

| Problema                   | Solução                                             |
| -------------------------- | --------------------------------------------------- |
| Sem saída serial           | Verificar conexão USB e baud rate (115200)          |
| Uma célula retorna -1      | Verificar conexão DT específica e cabo              |
| Valores muito altos/baixos | Verificar alimentação dos módulos HX711             |
| Leituras instáveis         | Reduzir ruído eletromagnético, usar cabos blindados |
| Erro de inicialização      | Verificar ligações GND compartilhado                |

## 📈 Performance

- **Taxa de Leitura:** 5 Hz (200ms/ciclo)
- **Latência:** ~50ms
- **Precisão:** Valor bruto do ADC (24-bit)
- **Resolução:** Dependente do load cell

## 🔄 Fluxo Principal

1. **Setup:** Inicializa serial e HX711 modules (com SCK compartilhado)
2. **Loop:**
   - Lê todos os 4 sensores (DT individuais com SCK compartilhado)
   - Envia cada valor individualmente via serial
   - Aguarda 200ms
   - Repete

## ✅ Status - FUNCIONANDO CORRETAMENTE

- **Versão:** 1.0
- **Configuração:** SCK compartilhado (D6) + 4 DT individuais
- **Compilação:** ✅ SUCCESS
- **Teste Funcional:** ✅ Todas as 4 células transmitindo dados
- **Células Lidas:** ✅ 1, 2, 3, 4 (100% funcionando)
- **Serial Output:** ✅ Formato id:value,00\r\n validado
- **Data de Atualização:** 2026-04-15

---

**Nota:** A calibração dos sensores é responsabilidade do sistema externo que recebe os dados via serial.
