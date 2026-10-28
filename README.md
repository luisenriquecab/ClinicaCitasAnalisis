# Sistema de Control de Citas Clínicas · UMG
**Universidad Mariano Gálvez de Guatemala**  
**Facultad de Ingeniería en Sistemas de Información y Ciencias de la Computación**  
**Curso:** Análisis de Sistemas II (037)  
**Catedrático:** Ing. José Eduardo Echeverria Ruíz  
**Estudiante:** Luis Enrique Cab Caal (Carné: `0902-23-11039`)  

---

## 📌 Descripción General del Proyecto
Proyecto final de diseño y aseguramiento de la calidad de software para un **Sistema de Control de Citas Médicas Multidisciplinario**. Resuelve la sobrecarga de agendamiento multicanal y ataca activamente la tasa de inasistencia (*No-Show*) mediante recordatorios automatizados y analítica clínica.

El proyecto está diseñado bajo **Clean Architecture** (Arquitectura Limpia) en el backend y el patrón **MVVM** para clientes móviles, cumpliendo estrictamente con la rúbrica oficial de **15.0 puntos** (Semanas 13 a 18).

---

## 📂 Estructura del Repositorio

```plaintext
ClinicaCitas/
├── docs/
│   ├── INFORME_FINAL_PROYECTO_SISTEMA_CITAS_UMG.md  # 📄 Documento Integrado Maestro (Entregables 1, 2, 3, 4 y Defensa)
│   └── mockups/
│       └── prototipo_interactivo_6_pantallas.html   # 🌐 Prototipo interactivo de las 6 pantallas clínicas (WCAG 2.1 AA)
├── src/
│   ├── Clinica.Domain/          # Núcleo de entidades (Cita, Medico, Paciente), enums e invariantes
│   ├── Clinica.Application/     # Casos de uso, DTOs, interfaces de servicio y motor de concurrencia
│   ├── Clinica.Infrastructure/  # Repositorios en memoria/EF Core, despachador de notificaciones y background worker
│   └── Clinica.API/             # Web API RESTful ASP.NET Core con CORS y Swagger
├── tests/
│   ├── Clinica.UnitTests/       # 12 Pruebas unitarias automatizadas (Invariantes, Concurrencia y Seguridad OWASP)
│   └── Clinica.IntegrationTests/# Pruebas de integración de endpoints REST
└── ClinicaCitas.slnx            # Archivo de solución .NET 10 / 9
```

---

## 🚀 Alineación con los 4 Entregables de la Rúbrica

| Entregable | Ponderación | Estado | Contenido Implementado |
| :--- | :---: | :---: | :--- |
| **1. Modelo Arquitectónico** | **2.5 pts** | ✅ Completo | Justificación Clean Architecture, justificación MVVM móvil con soporte Offline-First, Diagrama de Componentes y Diagrama de Despliegue con TLS 1.3. |
| **2. Diseño de Componentes** | **2.5 pts** | ✅ Completo | Interfaces formales (`ICitaManager`, `IAgendaMedicaService`, etc.), detalle de 2 componentes (Motor de Citas y Worker de Recordatorios), y refactorización de código Antes/Después con principios SOLID. |
| **3. Experiencia de Usuario** | **2.5 pts** | ✅ Completo | User Personas (Paciente adulto mayor, Médico, Recepción), 6 pantallas clínicas interactivas, 10 heurísticas de Nielsen y directrices de accesibilidad WCAG 2.1 AA (botones $\ge 48\text{ px}$). |
| **4. Plan de Calidad y Pruebas** | **2.5 pts** | ✅ Completo | 12 pruebas unitarias xUnit en verde (100% éxito), integración, criterios de aceptación Gherkin/BDD, pruebas de concurrencia y seguridad OWASP Top 10 (exigencia semana 17). |
| **Defensa y Coherencia** | **5.0 pts** | ✅ Completo | Matriz de trazabilidad integral de requisitos a pruebas, y banco de preguntas de examen con respuestas estratégicas para la defensa oral. |

---

## 🛠️ Cómo Ejecutar el Proyecto en Local

### Prerrequisitos
* [.NET SDK 9.0 o 10.0](https://dotnet.microsoft.com/)
* Navegador Web (Chrome, Edge o Firefox)

### 1. Ejecutar las Pruebas Unitarias y de Seguridad
```bash
dotnet test
```
*Todas las 12 pruebas deben reportarse en verde (Superadas).*

### 2. Iniciar el Servidor API REST
```bash
dotnet run --project src/Clinica.API --launch-profile http
```
*El servicio escuchará en `http://localhost:5140` y el demonio de recordatorios en segundo plano iniciará automáticamente.*

### 3. Abrir el Prototipo Interactivo (6 Pantallas)
Abre en tu navegador el archivo ubicado en:  
`docs/mockups/prototipo_interactivo_6_pantallas.html`

---

## 👥 Equipo de Trabajo
* **Luis Enrique Cab Caal** (Carné: `0902-23-11039`)
* Integrantes del equipo del curso Análisis de Sistemas II · UMG
