# Digital Load Cell Converter - 4x HX711 - ✅ FUNCIONANDO

Sistema simplificado para leitura de 4 células de carga via módulos HX711 com transmissão serial de valores brutos.

## 🎯 Características

✅ **4 Módulos HX711** - Leitura simultânea de 4 células de carga  
✅ **SCK Compartilhado** - Usa apenas 1 pino de clock (D6) para todos  
✅ **4 DT Individuais** - Cada célula com seu próprio DT (D5, D4, D3, D2)  
✅ **Transmissão Serial** - Formato simples: `id:value,00\r\n`  
✅ **Valores Brutos** - ADC 24-bit sem processamento  
✅ **Otimizado** - Usa apenas 18.4% da Flash e 16.4% da RAM

## 📋 Requisitos

- Arduino Nano (ATmega328)
- 4x HX711 Modules
- 4x Load Cells
- PlatformIO ou Arduino IDE
- Dependency: `robtillaart/HX711@^0.6.3`

## 🔌 Conexões - ✅ TESTADO

**Configuração com SCK compartilhado:**

```
Pino D6:  SCK (Clock) - COMPARTILHADO pelos 4 módulos
Pino D5:  DT1 (Data Cell 1)
Pino D4:  DT2 (Data Cell 2)
Pino D3:  DT3 (Data Cell 3)
Pino D2:  DT4 (Data Cell 4)
GND:      Comum (Todos os GND conectados)
VCC:      5V (Todos os VCC conectados)
```

**Total de pinos utilizados:** 5 (1 SCK compartilhado + 4 DT individuais)

### Diagrama Simplificado

```
Arduino Nano         HX711 Modules
├─ D6 (SCK) ──────→ (CLK1) ─X─ (CLK2) ─X─ (CLK3) ─X─ (CLK4)
├─ D5 (DT1) ──────→ (DOUT1)
├─ D4 (DT2) ──────→ (DOUT2)
├─ D3 (DT3) ──────→ (DOUT3)
├─ D2 (DT4) ──────→ (DOUT4)
└─ GND ────────────→ GND (todos)
  5V ──────────────→ VCC (todos)
```

## ⚡ Quick Start

### 1. Compilar e Upload

**Com PlatformIO:**

```bash
pio run -t upload -e nanoatmega328new
```

**Com Arduino IDE:**

1. Abra `src/main.cpp`
2. Instale a biblioteca HX711: Sketch → Include Library → Manage Libraries
3. Selecione placa: Arduino Nano
4. Upload

### 2. Monitor Serial

```bash
pio device monitor -b 115200
```

Ou use Arduino IDE: Tools → Serial Monitor (115200 baud)

### 3. Calibração

Ver [CALIBRATION_GUIDE.md](CALIBRATION_GUIDE.md) para procedimento detalhado

## 📤 Formato de Transmissão - ✅ VALIDADO

### Dados dos Sensores

```
1:126032,00\r\n
2:127686,00\r\n
3:126719,00\r\n
4:125811,00\r\n
1:126032,00\r\n
2:127686,00\r\n
3:126719,00\r\n
4:125811,00\r\n
```

**Formato por linha:**

- `id` (1-4): Identificador do módulo HX711
- `value`: Valor bruto do ADC (24-bit)
- `,00`: Sempre com 2 casas decimais
- **Terminador:** `\r\n` (0x0D 0x0A - CR+LF)

**Intervalo entre leituras:** 200ms (configurável)
**Taxa total:** 5 Hz (1 ciclo completo = ~200ms)

## ⚙️ Configuração

Editar em `include/GlobalConfiguration.h`:

```cpp
#define SERIAL_BAUD_RATE 115200      // Velocidade serial
#define DELAY_IN_MS 200               // Intervalo entre leituras (ms)
```

Editar em `include/Hardware.h`:

```cpp
#define LOADCELL_SCK 6                // Pino SCK (COMPARTILHADO)
#define LOADCELL_DT1 5                // DT Cell 1
#define LOADCELL_DT2 4                // DT Cell 2
#define LOADCELL_DT3 3                // DT Cell 3
#define LOADCELL_DT4 2                // DT Cell 4
```

## 🔧 API

### AdcHandler

```cpp
adcHandler.begin();                    // Inicializar módulos HX711
adcHandler.readAllValues(values);      // Ler os 4 sensores
adcHandler.readValue(cellIndex);       // Ler sensor individual (0-3)
adcHandler.isInitialized();            // Verificar inicialização
```

### SerialHandler

```cpp
serialHandler.begin();                         // Inicializar serial
serialHandler.sendLoadCellValue(id, value);   // Enviar id:value,00\r\n
```

## 📊 Exemplo de Uso

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
| Erro ao compilar           | Verificar incluir biblioteca HX711@^0.6.3           |

## 📈 Performance

- **Taxa de Leitura:** 5 Hz (200ms/ciclo)
- **Latência:** ~50ms
- **Precisão:** Valor bruto do ADC (24-bit)
- **Uso de RAM:** 16.4% (335 bytes)
- **Uso de Flash:** 18.4% (5656 bytes)

## 📝 Características da Implementação

✅ Leitura simplificada (valores brutos do ADC)  
✅ SCK compartilhado entre módulos  
✅ 4 DTs individuais para cada célula  
✅ Formato serial estruturado: `id:value,00\r\n`  
✅ Transmissão contínua a 5 Hz  
✅ Código otimizado (18.4% Flash, 16.4% RAM)  
✅ Classe AdcHandler encapsulada  
✅ Classe SerialHandler para comunicação  
✅ Sem calibração (responsabilidade do sistema externo)  
✅ Sem averaging (lê valor bruto diretamente)

## 📄 Estrutura do Projeto

```
digital-converter/
├── platformio.ini              (Configuração PlatformIO)
├── include/
│   ├── AdcHandler.h            (Header do gerenciador HX711)
│   ├── AdcHandler.cpp
│   ├── GlobalConfiguration.h   (Configurações globais)
│   ├── Hardware.h              (Definição de pinos)
│   └── SerialHandler.h         (Header comunicação serial)
├── src/
│   ├── main.cpp                (Loop principal)
│   └── SerialHandler.cpp       (Implementação serial)
├── test/                        (Testes unitários)
├── README.md                    (Este arquivo)
└── README_SIMPLE.md
```

---

**Status:** ✅ **PRONTO PARA PRODUÇÃO**  
**Versão:** 1.0 - Simplificada  
**Configuração:** SCK Compartilhado (D6) + 4 DT Individuais  
**Teste Funcional:** ✅ Todas as 4 células transmitindo corretamente  
**Compilação:** ✅ SUCCESS  
**Última atualização:** 2026-04-15

---

Para mais informações, veja [README_SIMPLE.md](README_SIMPLE.md)
