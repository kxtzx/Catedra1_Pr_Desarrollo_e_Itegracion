# Sistema de Gestión de Arriendos de Vehículos

## Estructura del Proyecto

Este proyecto está dividido en **3 sistemas separados**, cada uno ejecutándose de forma independiente en su propio puerto.

```
CatedraArriendos/
├── SistemaArriendos/          (Puerto 5036)
├── SistemaMantencion/         (Puerto 5101)
├── SistemaRRHH/               (Puerto 5203)
└── CatedraArriendos.sln       (Solución de VS)
```

---

## Cómo Ejecutar los Sistemas

Cada sistema se ejecuta **independientemente** en su propia terminal.

### **Sistema de Arriendos** (Puerto 5036)
```bash
cd SistemaArriendos
dotnet run
# Accede a: http://localhost:5036
```

### **Sistema de Mantenimiento** (Puerto 5101)
```bash
cd SistemaMantencion
dotnet run
# Accede a: http://localhost:5101
```

### 3️**Sistema de RRHH** (Puerto 5203)
```bash
cd SistemaRRHH
dotnet run
# Accede a: http://localhost:5203
```

---

## Base de Datos

- **Base Datos 1**: Arriendos + Mantenimiento (compartida)
- **Base Datos 2**: RRHH (separada)

Comunicación entre sistemas:
- Arriendos ↔ Mantenimiento: misma BD
- RRHH: se comunica solo leyendo archivo JSON de horas

---

##  Requisitos

- .NET 9.0
- Entity Framework Core
- SQL Server / MySQL

---


