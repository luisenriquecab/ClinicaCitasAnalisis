# ESTRUCTURA Y CONTENIDO DETALLADO DE DIAPOSITIVAS
## Sistema de Control de Citas Médicas Multidisciplinario
**Universidad Mariano Gálvez de Guatemala (UMG)**  
**Facultad de Ingeniería en Sistemas de Información y Ciencias de la Computación**  
**Curso:** Análisis de Sistemas II (Código 037)  
**Catedrático:** Ing. José Eduardo Echeverria Ruíz  
**Estudiante:** Luis Enrique Cab Caal (Carné: `0902-23-11039`)  
**Ponderación:** 5.0 Puntos Netos (Defensa Oral y Coherencia de Entrega Final)

---

## DIAPOSITIVA 1: Portada Institucional y Presentación del Proyecto
* **Tiempo Estimado:** 0:00 - 0:40 (40 segundos)
* **Objetivo:** Establecer formalidad académica, rigor institucional y presentar el proyecto con claridad.

### Contenido Visual de la Diapositiva:
* **Encabezado Institucional:**  
  UNIVERSIDAD MARIANO GÁLVEZ DE GUATEMALA  
  Facultad de Ingeniería en Sistemas de Información y Ciencias de la Computación  
  Curso: Análisis de Sistemas II (037) — Octavo Ciclo
* **Título Central:**  
  **SISTEMA DE CONTROL DE CITAS PARA UNA CLÍNICA MULTIDISCIPLINARIA**  
  *Diseño Arquitectónico, Experiencia de Usuario (UX/UI) y Aseguramiento de la Calidad (QA)*
* **Datos del Expositor:**  
  - **Estudiante:** Luis Enrique Cab Caal  
  - **Carné:** `0902-23-11039`  
  - **Catedrático:** Ing. José Eduardo Echeverria Ruíz  
  - **Fecha:** Ciclo Académico 2026 · Guatemala
* **Insignias Visuales:**  
  `Clean Architecture` | `Patrón MVVM Móvil` | `WCAG 2.1 AA` | `OWASP Top 10`

### Guion de Exposición (Notas del Orador):
> *"Muy buen día, ingeniero José Eduardo Echeverria y compañeros presentes. Mi nombre es Luis Enrique Cab Caal, estudiante del octavo ciclo de Ingeniería en Sistemas. A continuación tengo el honor de presentar la defensa final de nuestro proyecto: el Sistema de Control de Citas para una Clínica Multidisciplinaria. Este proyecto aborda desde el análisis del negocio de salud y el modelado arquitectónico, hasta el diseño de la experiencia de usuario y el aseguramiento riguroso de la calidad del software."*

---

## DIAPOSITIVA 2: El Problema Clínico: Ausentismo y Colisiones Concurrentes
* **Tiempo Estimado:** 0:40 - 1:30 (50 segundos)
* **Objetivo:** Demostrar que el sistema responde a una necesidad crítica de negocio y no es un simple ejercicio académico.

### Contenido Visual de la Diapositiva:
* **Título:** Problemática Actual en la Gestión de Citas Médicas
* **Subtítulo:** Impacto económico y operativo en clínicas de consulta externa
* **Columna 1: La Epidemia del "No-Show" (Inasistencia Inesperada)**
  - Hasta un **$15\% - 20\%$** de citas agendadas son abandonadas sin aviso previo.
  - **Horas médicas desperdiciadas:** El especialista permanece inactivo sin posibilidad de reasignar el turno.
  - **Sobrecosto financiero:** Pérdida directa de facturación y aumento artificial en las listas de espera.
* **Columna 2: Colisiones de Concurrencia Multicanal**
  - **Doble reserva simultánea:** Un paciente en la app móvil y una recepcionista en ventanilla apartando el mismo médico a las 09:30 AM en el mismo instante.
  - **Fricción en adultos mayores:** Plataformas complejas que exigen contraseñas enredadas y formularios saturados.
* **Métrica Destacada:** *Impacto estimado: 9 horas clínicas perdidas al mes por médico sin un sistema automatizado.*

### Guion de Exposición (Notas del Orador):
> *"Para entender la solución, primero debemos comprender la realidad de las clínicas ambulatorias. Existen dos cuellos de botella críticos: el primero es la alta tasa de inasistencias o 'No-Shows', donde hasta un 20% de pacientes faltan sin avisar, dejando tiempos muertos en las agendas de especialistas que otros pacientes urgentes necesitaban. El segundo problema son las colisiones de concurrencia: cuando se reciben solicitudes simultáneas por teléfono, recepción física y celulares. Nuestra meta fue erradicar ambos problemas a través de arquitectura y automatización."*

---

## DIAPOSITIVA 3: Modelo Arquitectónico: Clean Architecture (Arquitectura Limpia)
* **Tiempo Estimado:** 1:30 - 2:20 (50 segundos)
* **Objetivo:** Justificar teóricamente por qué se eligió Clean Architecture y cómo beneficia a la clínica.

### Contenido Visual de la Diapositiva:
* **Título:** Arquitectura del Sistema: Clean Architecture
* **Subtítulo:** Desacoplamiento total entre las reglas de salud y la tecnología
* **Diagrama de Capas Concéntricas:**
  1. **Dominio (Núcleo Puro):** Entidades clínicas (`Cita`, `Medico`, `Paciente`), reglas invariantes y penalizaciones de cancelación. Cero dependencias externas.
  2. **Aplicación (Casos de Uso):** Orquestación del agendamiento, motor de prevención de traslapes y cálculo de inasistencias.
  3. **Infraestructura:** Persistencia relacional, motor SQL Server y demonios de recordatorios.
  4. **Presentación (API REST):** Centralización de endpoints seguros consumidos equitativamente por Web, Móvil y Recepción.
* **3 Ventajas Clave para el Negocio:**
  - **Única Fuente de Verdad:** Todas las plataformas validan las mismas reglas.
  - **Independencia Tecnológica:** Se puede cambiar de base de datos o pasarela sin alterar las reglas de salud.
  - **Testabilidad al 100%:** Permite certificar el sistema antes de conectarlo a producción.

### Guion de Exposición (Notas del Orador):
> *"En el primer entregable evaluamos los estilos arquitectónicos y seleccionamos Clean Architecture fundamentada en una API REST centralizada. ¿Por qué? Porque en los sistemas hospitalarios, las reglas de salud jamás deben depender de la tecnología. Si mañana la clínica decide migrar de SQL Server a PostgreSQL o de ASP.NET a otro entorno, la lógica del agendamiento, los bloques de 30 minutos y las penalizaciones por cancelación tardía permanecen 100% intactas. Además, una sola API REST garantiza que la Web, el Móvil y la Recepción accedan a una única fuente de verdad sin discrepancias."*

---

## DIAPOSITIVA 4: Arquitectura Móvil (MVVM) y Topología de Despliegue Físico
* **Tiempo Estimado:** 2:20 - 3:10 (50 segundos)
* **Objetivo:** Explicar el soporte Offline-First en móviles y la seguridad perimetral de red.

### Contenido Visual de la Diapositiva:
* **Título:** Aplicación Móvil (MVVM) y Topología Física
* **Subtítulo:** Reactividad, estrategia Offline-First y protección perimetral
* **Bloque Izquierdo: Patrón MVVM en Dispositivos Móviles**
  - **View (Interfaz Reactiva):** Se actualiza automáticamente sin recargar la pantalla.
  - **ViewModel:** Gestiona el estado de las citas y comandos del usuario.
  - **Soporte Offline-First:** Almacenamiento local seguro (SQLite). El paciente puede revisar su número de consultorio y horario asignado aunque pierda señal celular dentro del hospital.
* **Bloque Derecho: Diagrama de Despliegue Físico**
  - **Perímetro DMZ / Proxy Inverso:** Cifrado forzado con **TLS 1.3** y filtro de firewall (WAF).
  - **Servidor de Aplicaciones:** Kestrel .NET ejecutando la API REST y el Demonio de Recordatorios.
  - **Servidor de Base de Datos:** SQL Server con cifrado en reposo (TDE) y bloqueos transaccionales por fila.

### Guion de Exposición (Notas del Orador):
> *"Para los clientes móviles de pacientes y médicos definimos el patrón MVVM. Esto nos otorga enlace de datos bidireccional reactivo: si recepción confirma o cancela un turno, la pantalla del paciente se actualiza al instante sin parpadear. Fundamentalmente, implementamos soporte Offline-First: si Doña Carmen entra al sótano de la clínica donde no hay señal telefónica, puede abrir su app y ver su número de consultorio y la fecha de su cita guardados en memoria local. A nivel de infraestructura física, el sistema opera bajo un perímetro cifrado con TLS 1.3 y base de datos con aislamiento transaccional estricto."*

---

## DIAPOSITIVA 5: Diseño UX/UI Centrado en el Usuario (WCAG 2.1 AA & Heurísticas)
* **Tiempo Estimado:** 3:10 - 4:00 (50 segundos)
* **Objetivo:** Presentar los estándares de usabilidad, inclusión para adultos mayores y catálogo de pantallas.

### Contenido Visual de la Diapositiva:
* **Título:** Experiencia del Usuario (UX/UI) e Inclusión Clínica
* **Subtítulo:** Diseñado bajo estándares WCAG 2.1 Nivel AA y las 10 Heurísticas de Jakob Nielsen
* **Pilares de Accesibilidad Implementados:**
  - **Áreas Táctiles Amplias (*Touch Targets*):** Botones con mínimo de **$48 \times 48\text{ px}$** para facilitar la pulsación en personas adultas mayores con temblor o artritis.
  - **Contraste Certificado:** Ratio superior a **$4.5:1$** en textos y **$7:1$** en botones de acción.
  - **Prevención de Errores (Nielsen #5):** Las horas ocupadas o días inhábiles aparecen deshabilitados en gris, haciendo matemáticamente imposible seleccionar un horario inválido.
* **El Catálogo de las 6 Pantallas Clínicas:**
  1. *Login Accesible (DPI + OTP WhatsApp)* | 2. *Asistente de Agendamiento en 4 Pasos*
  3. *Mis Citas y Reprogramación* | 4. *Agenda Timeline del Médico*
  5. *Tablero de Recepción Express (Atajos F2)* | 6. *Dashboard Analítico de Inasistencias*

### Guion de Exposición (Notas del Orador):
> *"En el Entregable 3 abordamos la Experiencia de Usuario. Un sistema médico debe ser inclusivo: Doña Carmen, de 63 años, no tiene por qué batallar con contraseñas complejas ni letras diminutas. Por ello, diseñamos bajo la norma internacional WCAG 2.1 AA con áreas de toque mayores a 48 píxeles y acceso simplificado por DPI con código OTP por WhatsApp. Además, aplicamos las 10 heurísticas de Jakob Nielsen, especialmente la prevención de errores: los turnos ocupados aparecen deshabilitados y tachados en gris, impidiendo que el usuario cometa equivocaciones."*

---

## DIAPOSITIVA 6: Demostración Funcional en Vivo: Los 4 Roles Clínicos
* **Tiempo Estimado:** 4:00 - 6:30 (2 minutos y 30 segundos)
* **Objetivo:** Demostrar el funcionamiento práctico del prototipo interactivo recorriendo las 6 pantallas.

### Contenido Visual de la Diapositiva:
*(Esta diapositiva sirve como puente visual para compartir pantalla con el navegador interactivo `prototipo_interactivo_6_pantallas.html`)*
* **Título:** Demostración en Vivo: Los 4 Actores del Ecosistema Clínico
* **Matriz de Recorrido de Pantallas:**
  - 👤 **Rol Paciente:** Onboarding por DPI $\rightarrow$ Asistente guiado de citas $\rightarrow$ Historial con opción de reprogramar o cancelar con aviso de 2 horas.
  - 🩺 **Rol Médico Especialista:** Vista diaria de 08:00 a 16:00 hrs $\rightarrow$ Iniciar atención $\rightarrow$ Marcar No-Show en 1 solo clic.
  - 🖥️ **Rol Recepcionista:** Búsqueda instantánea universal $\rightarrow$ Atajo `F2` para citas de emergencia $\rightarrow$ Monitor de sala de espera.
  - 📊 **Rol Dirección / Auditoría:** KPIs de inasistencias acumuladas $\rightarrow$ Horas perdidas $\rightarrow$ Exportación de reportes.

### Guion de Exposición (Notas del Orador):
> *(En este punto, pasas a compartir el archivo HTML del prototipo en pantalla completa pulsando F11)*
> *"Veamos el sistema en acción a través de sus cuatro roles principales:*
> *1. El Paciente: Doña Carmen ingresa su DPI y recibe su código de acceso. Al agendar con el Dr. Mendizábal, ve su horario en verde a las 09:30 AM y confirma en un clic. En 'Mis Citas' puede reprogramar o cancelar si surge una emergencia.*
> *2. El Médico: El Dr. Mendizábal abre su agenda matutina, ve al paciente en sala, inicia la atención y, si un paciente falta, pulsa 'Marcar No-Show' registrando la auditoría.*
> *3. Recepción: La secretaria utiliza el buscador rápido por DPI y asigna citas exprés con el atajo F2 sin demoras.*
> *4. Dirección Médica: El dashboard analiza los últimos 30 días, mostrando 18 inasistencias reales (12.68%) y 9 horas perdidas, permitiendo exportar el balance a Excel y PDF."*

---

## DIAPOSITIVA 7: Automatización en Segundo Plano: Motor de Recordatorios
* **Tiempo Estimado:** 6:30 - 7:20 (50 segundos)
* **Objetivo:** Explicar cómo el sistema previene activamente el ausentismo sin requerir intervención manual.

### Contenido Visual de la Diapositiva:
* **Título:** Automatización: Servicio en Segundo Plano (*Background Worker*)
* **Subtítulo:** Estrategia activa para reducir el ausentismo mediante recordatorios programados
* **Esquema del Flujo Automatizado:**
  ```plaintext
  [Base de Datos de Citas]
            │
            ▼ Escaneo periódico continuo cada 30 segundos
  ┌─────────────────────────────────────────────────────────────┐
  │         MOTOR DE NOTIFICACIONES EN SEGUNDO PLANO            │
  └──────────────┬──────────────────────────────┬───────────────┘
                 │                              │
         Ventana de 24 Horas             Ventana de 2 Horas
                 │                              │
                 ▼                              ▼
     Recordatorio Preventivo         Alerta de Salida Inmediata
    - Confirmación en 1 toque       - Número de Consultorio
    - Opción de liberar cupo        - Indicaciones de parqueo
                 │                              │
                 ▼                              ▼
      [WhatsApp / SMS / Push]        [Notificación Urgente]
  ```
* **Impacto Comprobado:** Reduce la tasa de *No-Show* hasta en un **$35\%$** al facilitar la liberación anticipada del espacio.

### Guion de Exposición (Notas del Orador):
> *"Una de las innovaciones más importantes del sistema es su motor de recordatorios en segundo plano. No depende de que una secretaria llame por teléfono uno por uno a los pacientes. Un servicio automatizado escanea continuamente las agendas: a las 24 horas previas envía un mensaje interactivo por WhatsApp permitiendo confirmar o liberar el cupo si no podrá asistir; y a las 2 horas previas despacha un recordatorio urgente con el número de consultorio. Esto ataca de raíz el olvido de citas médicas."*

---

## DIAPOSITIVA 8: Aseguramiento de la Calidad (QA) y Pruebas del Sistema
* **Tiempo Estimado:** 7:20 - 8:10 (50 segundos)
* **Objetivo:** Respaldar el Entregable 4 mostrando la pirámide de pruebas, BDD y prueba de concurrencia.

### Contenido Visual de la Diapositiva:
* **Título:** Plan de Calidad de Software y Aseguramiento (QA)
* **Subtítulo:** Pirámide integral de pruebas y verificación de invariantes
* **Estructura de la Pirámide de Calidad:**
  - **Pruebas Unitarias (12 de 12 Superadas - 100% en Verde):** Verificación de fechas pasadas, cancelaciones tardías, cambio de estados clínicos y cálculo de métricas de inasistencia.
  - **Pruebas de Validación / BDD (Gherkin):** Especificación en lenguaje natural orientado a escenarios de negocio (*Dado que... Cuando... Entonces...*).
  - **Pruebas de Concurrencia y Carga:** Simulación de 100 pacientes intentando reservar la misma cita en el segundo cero.
* **Resultado del Test de Concurrencia:**
  - Exactamente **1 solicitud exitosa** (`201 Created`).
  - **99 solicitudes rechazadas con elegancia** (`409 Conflict`) con mensaje asistido de horario ocupado.
  - **Cero duplicidad** y cero corrupción en la base de datos.

### Guion de Exposición (Notas del Orador):
> *"Para el Entregable 4 elaboramos un riguroso plan de calidad. Implementamos una batería de pruebas automatizadas con un 100% de éxito, validando que jamás se pueda agendar una cita en el pasado o calcular mal una penalización. Realizamos pruebas de validación con lenguaje Gherkin y sometimos el sistema a pruebas de concurrencia extrema: cuando 100 usuarios intentan apartar el mismo turno en el segundo cero, exactamente uno obtiene la cita y los otros 99 reciben un código 409 con una sugerencia de nuevo horario, garantizando cero sobrecupos."*

---

## DIAPOSITIVA 9: Seguridad de la Información Clínica (OWASP Top 10 - Semana 17)
* **Tiempo Estimado:** 8:10 - 8:50 (40 segundos)
* **Objetivo:** Cumplir explícitamente con el requisito de seguridad exigido en la semana 17 de la guía del curso.

### Contenido Visual de la Diapositiva:
* **Título:** Seguridad de la Información y Protección de Datos de Salud
* **Subtítulo:** Alineado con el estándar OWASP Top 10 y regulaciones de confidencialidad médica
* **Matriz de Seguridad:**
  | Riesgo OWASP | Vector de Ataque | Salvaguarda Implementada en el Sistema |
  | :--- | :--- | :--- |
  | **A01: Control de Acceso Roto** | Paciente intentando ver citas de otros usuarios | **Aislamiento Estricto por Claims (RBAC):** Cada paciente únicamente tiene visibilidad sobre sus propios registros médicos. |
  | **A03: Inyección SQL (SQLi)** | Cargas maliciosas en el motivo de consulta (`' DROP TABLE...`) | **Persistencia Fuertemente Tipada:** Se procesa como texto plano seguro sin interpretación dinámica de sentencias SQL. |
  | **A07: Fallas de Autenticación** | Suplantación de identidad o robo de sesión | **Tokens JWT Firmados:** Expiración corta de 15 minutos y doble factor OTP en ventanilla. |
  | **Privacidad de Salud** | Interceptación de datos clínicos en tránsito | **Cifrado Obligatorio TLS 1.3 (HTTPS):** Cabeceras HSTS activas en toda comunicación. |

### Guion de Exposición (Notas del Orador):
> *"En cumplimiento estricto con la Semana 17 del programa, incorporamos seguridad integral basada en el OWASP Top 10. La información médica es de máxima sensibilidad: protegemos la privacidad aislando los registros para que ningún paciente pueda consultar citas ajenas; neutralizamos la inyección SQL tratando las entradas con repositorios fuertemente tipados; y forzamos el cifrado en tránsito con TLS 1.3 y autenticación basada en roles. Esto garantiza un entorno seguro tanto para el hospital como para el paciente."*

---

## DIAPOSITIVA 10: Matriz de Trazabilidad Integral del Proyecto
* **Tiempo Estimado:** 8:50 - 9:25 (35 segundos)
* **Objetivo:** Demostrar coherencia absoluta entre los 4 entregables (Criterio de 5.0 puntos de defensa).

### Contenido Visual de la Diapositiva:
* **Título:** Matriz de Trazabilidad: Coherencia Extremo a Extremo
* **Subtítulo:** Alineación total entre Arquitectura, Componentes, Interfaces y Pruebas
* **Tabla de Trazabilidad:**
  | Requisito Funcional Mínimo | Modelo Arquitectónico (E1) | Componente del Sistema (E2) | Pantalla de Usuario (E3) | Plan de Pruebas y Seguridad (E4) |
  | :--- | :---: | :---: | :---: | :---: |
  | **Agendamiento Multicanal** | API REST Centralizada | `ICitaManager` / `CitaManager` | Pantalla 2 (Wizard de Reserva) | Test de Concurrencia (Conflicto 409) |
  | **Cancelación / Reprogramar** | Dominio y Reglas Invariantes | `Cita.Cancelar()` (Regla 2 hrs) | Pantalla 3 (Mis Citas - Acciones) | Test de Penalización Cancelación Tardía |
  | **Recordatorios Automáticos** | Demonio en Segundo Plano | `RecordatoriosBackgroundWorker` | Pantalla 1 (Aviso WhatsApp/Push) | Test de Ventanas 24h y 2h |
  | **Agenda Diaria del Médico** | Autenticación RBAC | `IAgendaMedicaService` | Pantalla 4 (Timeline Médico) | Test de Transición a 'En Atención' |
  | **Auditoría de Inasistencias** | Consultas Analíticas | `IInasistenciaTracker` | Pantalla 6 (Dashboard No-Shows) | Test de Cálculo de Tasa Porcentual |

### Guion de Exposición (Notas del Orador):
> *"Para garantizar la máxima coherencia del documento integrado, estructuramos una matriz de trazabilidad cruzada. Cada funcionalidad exigida por el catedrático —como el agendamiento, la cancelación, la agenda médica y el reporte de ausentismo— cuenta con su respaldo exacto en la arquitectura, su contrato en componentes, su pantalla mockup en el prototipo y su prueba de calidad y seguridad asociada. No hay ningún cabo suelto en la ingeniería del proyecto."*

---

## DIAPOSITIVA 11: Conclusiones Finales y Agradecimientos
* **Tiempo Estimado:** 9:25 - 10:00 (35 segundos)
* **Objetivo:** Conclusión de alto impacto, profesionalismo y despedida institucional.

### Contenido Visual de la Diapositiva:
* **Título:** Conclusiones y Logros de Ingeniería
* **Puntos de Conclusión:**
  1. **Solución Integral al Problema Real:** Se mitigó la inasistencia clínica y se blindó el sistema contra colisiones de reserva concurrentes.
  2. **Arquitectura Escalable y Mantenible:** Clean Architecture y MVVM aseguran un desacoplamiento que protege la inversión de software a largo plazo.
  3. **Aseguramiento de Calidad y Seguridad Certificada:** Suite de pruebas con 100% de aprobación y cumplimiento estricto del estándar OWASP Top 10.
* **Cierre Formal:**  
  UNIVERSIDAD MARIANO GÁLVEZ DE GUATEMALA  
  *"Conoceréis la verdad, y la verdad os hará libres"*  
  **Estudiante:** Luis Enrique Cab Caal — Carné: `0902-23-11039`  
  *¡Muchas gracias por su atención!*

### Guion de Exposición (Notas del Orador):
> *"En conclusión, este proyecto de Análisis de Sistemas II demuestra que una arquitectura sólida y un diseño centrado en el usuario pueden transformar la operación de un centro de salud, eliminando tiempos muertos y brindando un servicio médico digno, accesible y seguro.*
> 
> *Agradezco profundamente la orientación de nuestro catedrático, el Ing. José Eduardo Echeverria Ruíz, a lo largo de este ciclo, y quedo a su entera disposición para cualquier pregunta o retroalimentación. Muchas gracias."*
