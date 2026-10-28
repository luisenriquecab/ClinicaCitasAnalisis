# Guía y Guion Técnico para el Video de Defensa: Sistema de Control de Citas Clínicas
**Universidad Mariano Gálvez de Guatemala**  
**Facultad de Ingeniería en Sistemas de Información y Ciencias de la Computación**  
**Curso:** Análisis de Sistemas II (037) — Catedrático: Ing. José Eduardo Echeverria Ruíz  
**Estudiante:** Luis Enrique Cab Caal (Carné: `0902-23-11039`)  
**Ponderación del Criterio:** 5.0 Puntos Netos (33.33% de la nota final)

---

## 1. Estrategia y Enfoque del Video (Por qué CERO Código)

En **Análisis de Sistemas II**, el evaluador califica tu visión como **Ingeniero de Sistemas y Analista de Procesos**, no como un programador júnior que lee líneas de código.
Explicar código (`if/else`, sintaxis, métodos) aburre al evaluador y consume tiempo valioso. En cambio, demostrar **el problema de la clínica, los flujos de negocio, la experiencia de usuario y las garantías de calidad** demuestra un dominio profesional superior.

### Parámetros Técnicos Recomendados
* **Duración Óptima:** **8 a 10 minutos** (conciso, dinámico y al grano).
* **Herramientas de Grabación Sugeridas:** OBS Studio, Loom, Clipchamp (Windows) o PowerPoint con grabación de pantalla.
* **Resolución:** 1080p (Full HD), audio claro con micrófono cerca.
* **Modo de Presentación:** Navegador web en pantalla completa (`F11`) mostrando el archivo `docs/mockups/prototipo_interactivo_6_pantallas.html`.

---

## 2. Estructura Minuto a Minuto del Video

| Minuto | Bloque Temático | Qué se Muestra en Pantalla | Mensaje Clave |
| :---: | :--- | :--- | :--- |
| **0:00 - 1:30** | Introducción y Planteamiento del Problema | Diapositiva formal UMG y gráfico del problema clínico | Epidemia de inasistencias (*No-Shows*) y choques de horario multicanal. |
| **1:30 - 3:00** | Solución Arquitectónica y Visión de Procesos | Diagramas UML de Componentes y Despliegue Físico | Justificación de Clean Architecture, única fuente de verdad y MVVM Offline-First. |
| **3:00 - 7:00** | Demostración en Vivo del Sistema Clínico | Navegador en `prototipo_interactivo_6_pantallas.html` | Recorrido guiado de los 4 roles (Paciente, Médico, Recepcionista, Administración). |
| **7:00 - 8:30** | Automatizaciones, Calidad y Seguridad | Diapositiva con Matriz de Trazabilidad y QA | Motor de recordatorios a 24h/2h, pirámide de pruebas y seguridad OWASP (Semana 17). |
| **8:30 - 9:30** | Conclusiones y Cierre Profesional | Diapositiva de conclusiones y datos de contacto | Impacto en la clínica: reducción de ausentismo y optimización del tiempo médico. |

---

## 3. Guion Detallado: Qué Mostrar y Qué Decir

### Bloque 1: Introducción Institucional y El Problema Real (0:00 - 1:30)

* **En Pantalla:** Diapositiva formal con el logo de la UMG, título del proyecto, nombre del catedrático, tu nombre y carné. Luego cambia a una gráfica o esquema del problema de la clínica.
* **Qué Decir (Guion):**
  > *"Buen día, ingeniero José Eduardo Echeverria y compañeros. Mi nombre es Luis Enrique Cab Caal, estudiante del octavo ciclo de Ingeniería en Sistemas de la Universidad Mariano Gálvez de Guatemala. A continuación presento la defensa final del Sistema de Control de Citas Médicas para una clínica multidisciplinaria.*
  >
  > *Las clínicas de consulta externa enfrentan actualmente dos grandes problemas que generan pérdidas económicas y retrasos severos: el primero es la alta tasa de inasistencia o 'no-shows', donde hasta un 20% de los pacientes reservan un espacio y no se presentan, dejando ociosas las horas de médicos especialistas. El segundo problema son las colisiones de concurrencia: cuando un paciente en su teléfono y una recepcionista en ventanilla intentan apartar el mismo horario al mismo tiempo.*
  >
  > *Nuestra solución aborda de raíz ambos problemas mediante un modelo arquitectónico desacoplado, automatización de recordatorios en segundo plano y una experiencia de usuario accesible y libre de errores."*

---

### Bloque 2: Solución Arquitectónica y Visión de Procesos (1:30 - 3:00)

* **En Pantalla:** Mostrar el **Diagrama de Componentes** y el **Diagrama de Despliegue Físico** del informe técnico.
* **Qué Decir (Guion):**
  > *"Desde la perspectiva del análisis arquitectónico, adoptamos el estándar de **Clean Architecture** (Arquitectura Limpia) con servicios REST centralizados y el patrón **MVVM** para clientes móviles.*
  >
  > *¿Por qué Clean Architecture? Porque separa estrictamente las reglas del negocio médico (como el cálculo de disponibilidad y las penalizaciones de cancelación) de los detalles tecnológicos como la base de datos o los dispositivos finales. Esto garantiza que el portal web de pacientes, la aplicación móvil y la ventanilla de recepción compartan una **única fuente de verdad** transaccional.*
  >
  > *Adicionalmente, la arquitectura móvil con soporte **Offline-First** permite que un paciente consulte sus citas programadas y número de consultorio incluso dentro del hospital donde la señal celular suele perderse."*

---

### Bloque 3: Demostración en Vivo del Sistema Clínico (3:00 - 7:00)
*(La parte central del video: interactúa con el prototipo visual navegando por las 6 pestañas)*

* **En Pantalla:** Abre en tu navegador el archivo `docs/mockups/prototipo_interactivo_6_pantallas.html` y presiona `F11` (Pantalla Completa).

#### Escenario 1: El Paciente (Doña Carmen, adulto mayor)
* **Acción en Pantalla:** Clic en pestaña **"1. Login & Onboarding"**.
* **Qué Decir:**
  > *"Iniciamos con el viaje del paciente. Aplicando las directrices de accesibilidad universal WCAG 2.1 y las heurísticas de Jakob Nielsen, eliminamos contraseñas complejas. Una paciente adulta mayor, como Doña Carmen, ingresa únicamente con su número de DPI y recibe un código OTP de verificación vía WhatsApp o SMS con un botón de alto contraste de más de 48 píxeles de área táctil."*

* **Acción en Pantalla:** Clic en pestaña **"2. Agendamiento Asistido"**.
* **Qué Decir:**
  > *"Una vez dentro, el sistema ofrece un asistente guiado de 4 pasos con visibilidad clara del progreso. Aquí vemos la tarjeta del Dr. Roberto Mendizábal, cardiólogo. El sistema calcula en tiempo real las franjas horarias de 30 minutos: las horas libres aparecen en **verde**, mientras que las horas ocupadas por otros pacientes quedan automáticamente bloqueadas y tachadas. Esto previene cualquier error humano de sobrecupo. Al seleccionar las 09:30 AM y confirmar, el sistema aparta atómicamente el horario."*

* **Acción en Pantalla:** Clic en pestaña **"3. Mis Citas (Paciente)"**.
* **Qué Decir:**
  > *"En la pantalla 'Mis Citas', el paciente ve el estado en tiempo real. Si surge un imprevisto, cuenta con opciones de 'Reprogramar' o 'Cancelar Cita'. Si cancela con más de 2 horas de anticipación, el sistema libera el cupo para otros pacientes sin penalización, notificando inmediatamente al médico."* (Haz clic en el botón Cancelar para mostrar el diálogo modal interactivo).

#### Escenario 2: El Médico Especialista (Dr. Mendizábal)
* **Acción en Pantalla:** Clic en pestaña **"4. Agenda del Médico"**.
* **Qué Decir:**
  > *"Pasamos al rol del médico. El doctor accede a su timeline cronológico del día de 08:00 a 16:00 hrs. Puede identificar qué paciente se encuentra en sala de espera, pasar una consulta a estado 'En Atención' y, si el paciente no llega, pulsar el botón de 'Marcar No-Show' en un solo clic, registrando el motivo en el expediente."*

#### Escenario 3: Recepción y Ventanilla Rápida
* **Acción en Pantalla:** Clic en pestaña **"5. Tablero Recepción"**.
* **Qué Decir:**
  > *"Para la recepcionista en clínica presencial, la velocidad es crítica. Esta vista cuenta con un buscador universal instantáneo por DPI o teléfono, contadores del flujo del día en vivo y atajos de teclado como F2 para abrir una cita de emergencia en menos de 5 segundos."*

#### Escenario 4: Dirección Médica y Analítica
* **Acción en Pantalla:** Clic en pestaña **"6. Dashboard Inasistencias"**.
* **Qué Decir:**
  > *"Finalmente, el módulo administrativo ataca el problema del ausentismo. El tablero analiza los últimos 30 días: muestra que de 142 citas agendadas, hubo 18 inasistencias reales, traduciéndose en una tasa de No-Show del 12.68% y 9 horas médicas perdidas. Además, desglosa el porcentaje de ausencias por especialista y permite exportar auditorías a Excel y PDF."*

---

### Bloque 4: Automatizaciones, Calidad y Seguridad (7:00 - 8:30)

* **En Pantalla:** Regresa a diapositivas o muestra el documento final donde está la matriz de trazabilidad y la tabla de pruebas.
* **Qué Decir (Guion):**
  > *"Detrás de esta interfaz, el sistema cuenta con tres pilares de ingeniería fundamentales:*
  >
  > 1. * **Automatización en segundo plano:** Un demonio de software escanea continuamente las agendas para despachar recordatorios automáticos a las 24 horas y 2 horas previas a la cita vía WhatsApp y Push, reduciendo drásticamente las inasistencias olvidadas.
  > 2. * **Aseguramiento de la Calidad (QA):** Diseñamos un plan de pruebas integral estructurado en la pirámide de testing: pruebas unitarias sobre las reglas de negocio, validaciones BDD con sintaxis Gherkin y pruebas de estrés para simular 100 pacientes intentando reservar la misma cita en el mismo segundo, garantizando cero corrupción de datos.
  > 3. * **Seguridad y Privacidad (Semana 17):** Alineados con el estándar OWASP Top 10, protegemos los datos clínicos mediante control de acceso por roles (RBAC) para que un paciente jamás tenga visibilidad de expedientes ajenos, y neutralizamos cualquier vector de inyección SQL mediante repositorios fuertemente tipados."*

---

### Bloque 5: Conclusiones y Cierre Profesional (8:30 - 9:30)

* **En Pantalla:** Diapositiva de conclusiones con tus datos de contacto y carné.
* **Qué Decir (Guion):**
  > *"En conclusión, este proyecto de Análisis de Sistemas II no se limitó a un diseño superficial, sino que ofrece una solución de ingeniería robusta, trazable y coherente desde el modelo arquitectónico hasta la experiencia del usuario y la seguridad clínica.*
  >
  > *Logramos un sistema que optimiza el tiempo de los médicos especialistas, elimina las dobles reservas concurrentes y brinda un acceso digno y accesible para nuestros pacientes.*
  >
  > *Agradezco la atención del docente evaluador, Ing. José Eduardo Echeverria Ruíz, y quedo a disposición para cualquier retroalimentación. Muchas gracias."*

---

## 4. Consejos de Grabación para Obtener los 5 Puntos de Defensa

1. **Iluminación y Cámara:** Si puedes colocar tu cámara web en una esquina pequeña de la pantalla (*PiP - Picture in Picture*), suma muchos puntos de formalidad. Viste camisa formal o institucional.
2. **Fluidez sobre Perfección:** Si te equivocas en una palabra, haz una pausa de 2 segundos y repite la frase con calma. No es necesario reiniciar toda la grabación desde cero si tienes un editor simple para recortar.
3. **El Secreto del Éxito:** Habla con seguridad y entusiasmo. Muestra que dominas el flujo clínico: la clave de este proyecto es **cómo la tecnología resuelve la inasistencia de pacientes y los choques de horario**.
