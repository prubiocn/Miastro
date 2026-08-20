# MIASTRO — Reglas y especificaciones consolidadas

**Fecha:** 20/08/2026  
**Estado:** definición funcional, doctrinal, de navegación e identidad visual consolidada.  
**Uso del documento:** referencia maestra para diseño, arquitectura técnica e implementación.

---

## 1. Visión

**Miastro** es una aplicación personal para explorar, comprender y consultar cartas astrológicas de forma clara, visual y profunda, mediante fichas personales e interpretaciones, como herramienta de apoyo para una consulta de astrología.

Está concebida para uso del astrólogo durante consultas profesionales, no como aplicación de uso directo por parte del cliente.

Debe permitir:

- gestionar fichas personales;
- calcular cartas astrológicas;
- visualizar cartas con claridad;
- interpretar cartas y ciclos;
- comparar cartas;
- generar informes;
- crear ruedas astrológicas con distintos estilos;
- exportar e imprimir;
- conservar historial de trabajos;
- resolver localidades, coordenadas y zonas horarias.

---

## 2. Principios rectores

### 2.1. Claridad antes que complejidad

Miastro debe evitar:

- interfaces sobrecargadas;
- exceso de datos secundarios;
- menús técnicos innecesarios;
- ventanas flotantes constantes;
- opciones que distraigan de la consulta.

La prioridad es consultar rápidamente:

- posiciones;
- casas;
- aspectos;
- ejes;
- relaciones entre cartas;
- síntesis interpretativas;
- evolución temporal.

### 2.2. La carta es la protagonista

> **La rueda debe ser el elemento visual principal.**

La interfaz acompaña a la carta y no compite con ella.

### 2.3. Una única pantalla central por persona

> **Miastro trabajará principalmente en una única pantalla central por persona, evitando ventanas flotantes y cambios innecesarios de contexto.**

Al cambiar entre Natal, Revolución Solar, Revolución Lunar, Tránsitos, Progresiones o Sinastría se mantendrá la estructura general y cambiará solo el contenido necesario.

### 2.4. La complejidad técnica queda en segundo plano

El usuario no debería tener que ocuparse, en uso normal, de:

- rutas internas;
- archivos técnicos;
- correcciones manuales de zona horaria;
- datos astronómicos auxiliares;
- configuraciones que no formen parte del trabajo astrológico directo.

---

## 3. Doctrina interpretativa general

La seña de identidad de Miastro será:

> **No interpretar piezas aisladas. Interpretar siempre sistemas de polaridades, ejes y relaciones.**

Toda interpretación debe buscar:

- tensión;
- complementariedad;
- equilibrio;
- integración;
- relación entre polos;
- contexto global.

### 3.1. Ejes zodiacales

- Aries ↔ Libra
- Tauro ↔ Escorpio
- Géminis ↔ Sagitario
- Cáncer ↔ Capricornio
- Leo ↔ Acuario
- Virgo ↔ Piscis

### 3.2. Ejes de casas

- Casa 1 ↔ Casa 7
- Casa 2 ↔ Casa 8
- Casa 3 ↔ Casa 9
- Casa 4 ↔ Casa 10
- Casa 5 ↔ Casa 11
- Casa 6 ↔ Casa 12

### 3.3. Regla doctrinal

No se interpretará un signo, casa, planeta o punto sin considerar:

- su eje;
- su polaridad;
- su casa;
- sus regencias;
- sus aspectos;
- su relación con el conjunto.

---

## 4. Áreas funcionales

1. Fichas personales.
2. Cálculo astrológico.
3. Visualización de cartas.
4. Interpretación.
5. Informes.
6. Ruedas artísticas.
7. Exportación e impresión.
8. Archivo de consultas y trabajos.
9. Catálogo geográfico y coordenadas.

---

## 5. Tipos de carta y técnicas

### 5.1. Incluidas en la planificación principal

- Carta natal.
- Tránsitos.
- Progresiones secundarias.
- Revolución solar.
- Revolución lunar.
- Sinastría.

### 5.2. Previstas para fases posteriores

- Carta compuesta.
- Retornos planetarios.
- Direcciones.
- Arco solar.
- Carta dracónica.
- Armónicos.
- Otras técnicas que se decidan posteriormente.

### 5.3. Progresiones secundarias

> **1 día después del nacimiento = 1 año de vida.**

Se tratarán como técnica distinta del arco solar.

### 5.4. Arco solar

Se reserva para una fase posterior y deberá ser un módulo independiente.

---

## 6. Base de cálculo astrológico

Configuración inicial:

- zodiaco tropical;
- cálculo geocéntrico;
- longitud eclíptica;
- posiciones aparentes;
- velocidad incluida;
- sin topocentrismo por defecto;
- sin zodiaco sideral en la primera versión.

---

## 7. Sistemas de casas

- Placidus.
- Koch.

---

## 8. Planetas, cuerpos, puntos y ángulos

### Planetas principales

- Sol.
- Luna.
- Mercurio.
- Venus.
- Marte.
- Júpiter.
- Saturno.
- Urano.
- Neptuno.
- Plutón.

### Otros cuerpos y puntos

- Nodo Norte.
- Nodo Sur.
- Quirón.
- Ceres.
- Palas.
- Juno.
- Vesta.
- Lilith Media.
- Parte de la Fortuna.
- Ascendente.
- Medio Cielo.

### Vulcano

**Pendiente de definición.**

La asociación esotérica Tauro → Vulcano y Virgo → Ceres, si se incorpora, deberá vivir en una capa esotérica separada de las regencias base.

---

## 9. Lilith

En la primera versión se utilizará:

> **Lilith Media**

La Lilith Verdadera podrá ser configurable en una fase posterior.

---

## 10. Parte de la Fortuna

Se incluirá desde el inicio.

**Carta diurna:** Ascendente + Luna − Sol  
**Carta nocturna:** Ascendente + Sol − Luna

El resultado se normaliza a 0°–360°.

---

## 11. Regencias

- Aries → Marte
- Tauro → Venus
- Géminis → Mercurio
- Cáncer → Luna
- Leo → Sol
- Virgo → Mercurio
- Libra → Venus
- Escorpio → Marte / Plutón
- Sagitario → Júpiter
- Capricornio → Saturno
- Acuario → Saturno / Urano
- Piscis → Júpiter / Neptuno

Cuando exista doble regencia, el modelo debe conservar ambas.

En el futuro podrá elegirse visualmente:

- tradicional;
- moderna;
- ambas.

---

## 12. Aspectos

### 12.1. Aspectos principales

- Conjunción — 0°
- Semisextil — 30°
- Sextil — 60°
- Cuadratura — 90°
- Trígono — 120°
- Quincuncio — 150°
- Oposición — 180°

### 12.2. Aspectos espirituales

- Quintil — 72°
- Biquintil — 144°

### 12.3. Orbes

| Aspecto | Orbe base | Con Sol o Luna |
|---|---:|---:|
| Conjunción | 8° | 9° |
| Semisextil | 2° | 3° |
| Sextil | 4° | 5° |
| Cuadratura | 6° | 7° |
| Trígono | 6° | 7° |
| Quincuncio | 3° | 4° |
| Oposición | 8° | 9° |
| Quintil | 2° | 3° |
| Biquintil | 2° | 3° |

**Regla obligatoria:** si participa el Sol o la Luna, el orbe aumenta 1° en total. Si participan ambos, sigue siendo solo +1°.

### 12.4. Participantes en aspectos — V1

Sí participan:

- 10 planetas;
- Quirón;
- Ceres;
- Palas;
- Juno;
- Vesta;
- Ascendente;
- Medio Cielo.

No participan inicialmente:

- Nodo Norte;
- Nodo Sur;
- Lilith Media;
- Parte de la Fortuna.

---

## 13. Catálogo geográfico

Miastro debe disponer de un **catálogo geográfico integrado y utilizable sin conexión**.

Debe resolver:

- localidad;
- región;
- país;
- latitud;
- longitud;
- zona horaria.

Los resultados ambiguos deberán mostrarse de forma inequívoca, por ejemplo:

- Pamplona — Navarra — España.
- Pamplona — Norte de Santander — Colombia.

### 13.1. Zona horaria histórica

**Regla obligatoria:**

Miastro utilizará la zona horaria histórica correspondiente a la fecha concreta del nacimiento o cálculo, incluido el horario de verano cuando proceda.

El usuario no debe calcular ni introducir manualmente estas correcciones en condiciones normales.

---

## 14. Ficha personal

Cada persona tendrá:

- Nombre y apellidos.
- Fecha de nacimiento.
- Hora de nacimiento.
- Precisión de la hora.
- Localidad de nacimiento.
- País.
- Coordenadas.
- Zona horaria.
- Lugar actual de residencia.
- Teléfono.
- Correo electrónico.
- Nota privada general.

### 14.1. Precisión de la hora

- Exacta.
- Aproximada.
- Rango.
- Momento del día.
- Desconocida.

### 14.2. Historial asociado

- cartas;
- tránsitos;
- progresiones;
- revoluciones;
- sinastrías;
- informes;
- trabajos asociados.

### 14.3. Acciones

- editar;
- borrar datos cuando proceda;
- eliminar persona.

La eliminación completa exigirá confirmación explícita y afectará al historial asociado.

### 14.4. Primera acción

Si no existe carta natal:

> **Calcular carta natal**

---

## 15. Pantalla de alta y edición de persona

Debe parecer una **ficha de persona**, no una ventana técnica.

### Bloques

**Identidad**
- Nombre.
- Apellidos.

**Nacimiento**
- Fecha.
- Hora.
- Precisión de la hora.

**Lugar de nacimiento**
- Campo de localidad.
- Botón **Buscar localidad**.
- País.
- Región.
- Latitud.
- Longitud.
- Zona horaria histórica.

**Residencia actual**
- Localidad.
- País.
- Región.
- Coordenadas.
- Zona horaria.

**Contacto y notas**
- Teléfono.
- Correo electrónico.
- Nota privada.

### 15.1. Botón Buscar localidad

**Obligatorio.**

Flujo:

1. El usuario escribe una localidad.
2. Pulsa **Buscar localidad**.
3. Miastro muestra localidad, región y país.
4. El usuario selecciona el resultado correcto.
5. Miastro completa coordenadas y zona horaria histórica.

### 15.2. No mostrar en esta ficha

- carpetas internas;
- rutas del sistema;
- selección de archivos técnicos;
- configuraciones del motor astrológico.

---

## 16. Organización dentro de una persona

### Ver

- Natal.
- Revolución solar.
- Revolución lunar.
- Tránsitos.
- Progresiones.
- Sinastría.

### Informes

- Interpretación natal.
- Interpretación de revolución solar.
- Tránsitos diarios.
- Tránsitos semanales.
- Tránsitos mensuales.
- Otros periodos futuros.
- Interpretación kármica.
- Informe astromédico.
- Orientación profesional.
- Informe de pareja.

### Ruedas

- representaciones gráficas;
- estilos artísticos;
- exportación;
- impresión.

---

## 17. Arquitectura general de interfaz

La pantalla principal tendrá:

1. Cabecera superior general.
2. Panel izquierdo de personas.
3. Área central de trabajo.
4. Panel de análisis integrado en la vista de carta.

---

## 18. Cabecera superior general

### Izquierda

- Miastro.
- Mostrar/contraer panel de personas.

### Centro

- Buscador global de personas.

### Derecha

- **Nueva persona**.
- **Configuración**.
- **Ayuda**.
- Indicador de guardado cuando proceda.

### Reglas

- Siempre visible.
- Poco alta.
- Sin funciones astrológicas específicas.
- **Nueva persona** a un clic.
- Evitar menús clásicos tipo “Archivo · Editar · Ver · Herramientas” salvo necesidad real.

---

## 19. Panel izquierdo de personas

Debe incluir:

- buscador;
- **Nueva persona**;
- listado de personas;
- última consulta;
- favoritos;
- filtros:
  - Todas;
  - Recientes;
  - Favoritas.

Cada fila mostrará:

- nombre;
- apellidos;
- última consulta.

Al pulsar se abre la ficha en el área central.

---

## 20. Ficha central de persona

### Cabecera fija

- Nombre y apellidos.
- Fecha de nacimiento.
- Hora.
- Precisión de la hora.
- Lugar de nacimiento.
- Residencia actual.
- **Editar**.
- **Eliminar**.
- **Historial**.

El historial se abrirá bajo demanda como panel lateral.

### Navegación principal

- **Ver**
- **Informes**
- **Ruedas**

### Subnavegación de Ver

- Natal.
- Revolución solar.
- Revolución lunar.
- Tránsitos.
- Progresiones.
- Sinastría.

### Apertura

- Con natal calculada → abrir Natal.
- Sin natal → mostrar Calcular carta natal.

---

## 21. Pantalla patrón de Carta Natal

Estructura:

- cabecera de persona;
- Ver / Informes / Ruedas;
- subnavegación;
- barra de controles;
- rueda grande a la izquierda;
- panel de análisis a la derecha;
- pie discreto.

### 21.1. Controles de la carta

- Planetas.
- Puntos.
- Aspectos.
- Cúspides.
- Etiquetas.
- **Modo Consulta**.
- **Modo Presentación**.

ASC y MC estarán siempre visibles.

Quirón estará visible por defecto.

### 21.2. Rueda

Aproximadamente dos tercios del ancho.

Debe incluir:

- signos;
- grados;
- casas;
- planetas;
- ASC;
- MC;
- Quirón;
- aspectos.

### 21.3. Panel derecho

Aproximadamente un tercio.

Pestañas:

- Datos.
- Posiciones.
- Aspectos.
- Distribución.
- Resumen.

Pestaña inicial:

> **Posiciones**

---

## 22. Pestaña Datos

Encabezado con:

- persona;
- fecha y hora;
- localidad;
- sistema de casas;
- zodiaco.

Tabla con:

- Glifo.
- Nombre.
- Grado.
- Signo.
- Regente.

No incluir inicialmente:

- dignidades;
- decanatos;
- términos;
- dispositor;
- datos técnicos secundarios.

---

## 23. Pestaña Posiciones

Lista desplegable con:

- Planetas.
- Nodos.
- Lilith Media.
- Parte de la Fortuna.
- Quirón.
- Ceres.
- Palas.
- Juno.
- Vesta.
- Ascendente.
- Medio Cielo.

Al desplegar:

- signo y posición exacta;
- casa;
- regente del signo;
- regente de la casa.

---

## 24. Pestaña Aspectos

Vista:

> **Matriz triangular de aspectos**

Debe:

- mostrar participantes en filas/columnas;
- mostrar símbolo si existe;
- dejar vacío si no existe;
- diferenciar tipos;
- hacer quintil y biquintil más discretos;
- mostrar cuerpos, aspecto y orbe al seleccionar o pasar sobre una celda.

---

## 25. Pestaña Distribución

No se usarán barras.

Bloques textuales:

- Cualidades: cardinal, fijo, mutable.
- Polaridad: masculina, femenina.
- Elementos: fuego, tierra, aire, agua.
- Cuadrantes: 1, 2, 3, 4.
- Hemisferios: superior/inferior e izquierdo/derecho.
- Equilibrios: dominante, equilibrado, débil.

Debe incluir una breve síntesis interpretativa.

---

## 26. Pestaña Resumen

Síntesis breve, no informe largo.

Debe incluir:

- rasgo dominante;
- Sol;
- Luna;
- Ascendente;
- elementos/modalidades destacados;
- planetas relevantes;
- patrones de aspectos;
- conclusión breve.

---

## 27. Rueda natal estándar

De fuera hacia dentro:

1. Anillo de signos.
2. Anillo de grados.
3. Anillo de casas.
4. Zona de planetas y puntos.
5. Centro de aspectos.

### Signos

- 12 signos;
- glifos visibles;
- divisiones de 30°;
- sin decoración excesiva.

### Grados

- marcas regulares;
- énfasis cada 5° y 10°;
- sin numeración excesiva.

### Casas

- 12 casas delimitadas;
- número visible;
- cúspides principales destacadas.

### Jerarquía visual

> **planetas → casas y ángulos → signos → aspectos secundarios**

---

## 28. Regla crítica de conjunciones y acumulaciones

**Obligatoria y crítica.**

La rueda debe separar:

- **posición astrológica real**;
- **posición gráfica del glifo**.

La posición real nunca cambia.

La posición gráfica puede desplazarse para garantizar legibilidad.

### Regla principal

> **Los glifos nunca podrán solaparse entre sí.**

### Estrategias permitidas

- separación angular;
- niveles radiales;
- desplazamiento lateral controlado;
- líneas o marcas hacia el grado real.

### Casos prioritarios

- conjunciones;
- stelliums;
- agrupaciones;
- proximidad a cúspides;
- proximidad a ASC;
- proximidad a MC;
- múltiples puntos opcionales.

### Comportamiento determinista

> **La misma carta y configuración debe dibujarse siempre igual.**

No se admitirán redistribuciones arbitrarias entre aperturas.

### Prioridad

La legibilidad prevalece sobre dibujar el glifo exactamente encima del grado, siempre que el grado real siga siendo trazable.

---

## 29. Visibilidad predeterminada

Visibles al abrir:

- Planetas.
- Signos.
- Casas.
- Aspectos.
- Quirón.
- ASC.
- MC.

Opcionales:

- Nodo Norte.
- Nodo Sur.
- Lilith Media.
- Parte de la Fortuna.
- Ceres.
- Palas.
- Juno.
- Vesta.

---

## 30. Interacción con la rueda

Al seleccionar un planeta o punto:

- se resalta;
- se resaltan sus aspectos;
- el panel derecho se sincroniza;
- Posiciones abre directamente ese elemento.

---

## 31. Pie de trabajo

Mostrar solo:

- sistema de casas;
- zodiaco;
- localidad utilizada;
- estado de guardado.

---

## 32. Revolución Solar

Carta anual comparada obligatoriamente con la natal.

### Barra específica

- año;
- localidad;
- cambiar localidad;
- año anterior;
- año siguiente.

### Vistas

- Revolución sola.
- Revolución + Natal.

La segunda será la recomendada.

### Pestañas

- Datos.
- Posiciones.
- Aspectos.
- Distribución.
- Comparativa natal.
- Resumen.

### Comparativa natal

Destacar:

- casas natales donde caen Sol, Luna, ASC y MC;
- activaciones de puntos natales;
- aspectos relevantes revolución–natal;
- áreas vitales enfatizadas;
- temas repetidos;
- tensiones;
- oportunidades;
- focos del año.

---

## 33. Revolución Lunar

Herramienta mensual práctica.

### Barra específica

- fecha;
- periodo de vigencia;
- localidad;
- lunación anterior;
- lunación siguiente.

### Vistas

- Revolución sola.
- Revolución + Natal.

La vista comparada será predeterminada.

### Pestañas

- Datos.
- Posiciones.
- Aspectos.
- Distribución.
- Comparativa natal.
- Resumen mensual.

### Foco interpretativo

- clima emocional;
- asuntos cotidianos;
- relaciones;
- hogar;
- necesidades personales;
- focos del mes.

---

## 34. Tránsitos

Pregunta guía:

> **¿Qué está activando ahora la carta natal de esta persona y durante cuánto tiempo?**

### Barra temporal

- fecha;
- hora;
- Hoy;
- día anterior;
- día siguiente;
- navegación por periodo.

### Vista

- Natal como base.
- Tránsitos superpuestos.

Controles:

- Natal.
- Tránsitos.
- Aspectos tránsito–natal.
- Aspectos internos de tránsito, opcionales.

Por defecto:

> **solo aspectos relevantes entre tránsito y natal.**

### Pestañas

- Datos.
- Posiciones.
- Aspectos.
- Tránsitos a natal.
- Periodo.
- Resumen.

### Periodos

- día;
- semana;
- mes;
- intervalo personalizado.

Mostrar:

> **entrada en orbe → exactitud → salida de orbe**

---

## 35. Progresiones secundarias

### Barra

- edad o fecha progresada;
- selector de fecha;
- periodo anterior;
- periodo siguiente.

### Vistas

- Natal.
- Progresada.
- Natal + Progresada.

La comparada será predeterminada.

### Pestañas

- Datos.
- Posiciones.
- Aspectos.
- Comparativa natal.
- Evolución.
- Resumen.

### Evolución

Destacar:

- Sol progresado;
- Luna progresada;
- ASC/MC progresados cuando el método definido lo permita;
- aspectos progresados;
- fechas de exactitud;
- duración del proceso.

Pregunta guía:

> **¿Qué proceso interno está madurando en esta etapa de la vida?**

---

## 36. Sinastría

### Cabecera

Mostrar las dos personas:

> **Persona 1 ↔ Persona 2**

Con:

- nombre;
- fecha;
- hora;
- lugar;
- precisión de la hora.

Debe existir **Cambiar persona**.

No usar “A” y “B” en la interfaz salvo internamente.

### Doble rueda

- persona abierta originalmente → anillo interior;
- segunda persona → anillo exterior.

Esto es solo visual.

### Diferenciación visual

No depender solo del color.

Usar también:

- posición;
- trazo;
- grosor;
- estilo;
- leyenda con nombres.

### Controles

- Persona 1.
- Persona 2.
- Aspectos cruzados.
- Aspectos internos.
- Casas.
- Puntos.
- Etiquetas.

Por defecto:

- ambas cartas visibles;
- aspectos cruzados visibles;
- aspectos internos ocultos.

### Panel derecho

- Datos.
- Posiciones.
- Aspectos cruzados.
- Casas activadas.
- Síntesis.

### Casas activadas

Dos bloques:

- Cómo Persona 1 activa a Persona 2.
- Cómo Persona 2 activa a Persona 1.

### Interacción

Al seleccionar un planeta:

- se resalta;
- se muestran sus aspectos con la otra carta;
- el panel muestra sus contactos;
- se indican las casas de la otra persona que activa.

---

## 37. Simetría obligatoria en sinastría e informe de pareja

> **El orden de carga de las personas no puede alterar la interpretación global.**

Miastro calculará internamente ambas direcciones:

- Persona 1 → Persona 2.
- Persona 2 → Persona 1.

Pero generará una interpretación integrada.

Cambiar el orden solo podrá modificar:

- disposición gráfica;
- orden de presentación.

No podrá modificar:

- conclusiones;
- valoración;
- síntesis global.

---

## 38. Interpretación natal

Debe basarse siempre en:

- ejes;
- polaridades;
- relación entre polos;
- integración.

Ejemplo conceptual: Marte en Aries se interpreta dentro del eje Aries–Libra, no de forma aislada.

---

## 39. Interpretación kármica

Atención obligatoria a:

- ejes de signos;
- ejes de casas;
- signos interceptados;
- Nodo Norte;
- Nodo Sur;
- Saturno;
- planetas retrógrados;
- Casa 12;
- Ascendente;
- regentes;
- aspectos relevantes.

### Prioridades

1. Casa 12.
2. Ascendente.
3. Nodo Sur ↔ Nodo Norte.
4. Saturno.
5. Retrógrados.
6. Signos interceptados.

El enfoque debe hablar de:

- procesos;
- patrones;
- interiorización;
- aprendizaje;
- integración;
- evolución.

No presentar como hechos demostrables afirmaciones sobre vidas pasadas.

---

## 40. Informe astromédico

Será:

> **simbólico, interpretativo y no diagnóstico.**

### Factores

- Casa 6 ↔ Casa 12;
- Casa 1 ↔ Casa 7;
- Ascendente y regente;
- Sol;
- Luna;
- Saturno;
- Marte;
- Júpiter;
- otros factores relevantes;
- elementos;
- cualidades;
- signos/casas cargados;
- aspectos tensionales y compensatorios.

### Júpiter

Especial atención como significador simbólico de la:

> **zona hepática**

Considerar:

- signo;
- casa;
- aspectos;
- estado global.

### Límites

No:

- diagnosticar;
- afirmar enfermedades;
- predecir patologías.

Sí hablar de:

- tendencias simbólicas;
- equilibrio;
- vitalidad;
- somatización simbólica;
- autocuidado.

---

## 41. Orientación profesional

### Ejes principales

- Casa 4 ↔ Casa 10.
- Casa 2 ↔ Casa 8.
- Casa 6 ↔ Casa 12.
- MC ↔ IC.

### Factores

- regente del MC;
- Sol;
- Saturno;
- Júpiter;
- Mercurio;
- Venus;
- Marte;
- elementos;
- modalidades;
- aspectos relevantes.

### Objetivo

No decir “tu profesión es X”.

Identificar:

- formas de trabajo;
- capacidades;
- funciones;
- entornos;
- estilos profesionales;
- potenciales coherentes.

---

## 42. Informe de pareja

No usar una puntuación simplista de compatibilidad.

Debe analizar:

- dinámica entre cartas;
- aspectos cruzados;
- casas activadas;
- ejes relacionales;
- Sol;
- Luna;
- Venus;
- Marte;
- Saturno;
- Nodos;
- ASC;
- DSC;
- MC;
- IC;
- patrones repetidos.

### Ejes prioritarios

- Casa 1 ↔ Casa 7.
- Casa 2 ↔ Casa 8.
- Casa 4 ↔ Casa 10.
- Casa 5 ↔ Casa 11.

### Preguntas de síntesis

- ¿Qué une a estas dos personas?
- ¿Dónde aparece la tensión?
- ¿Qué necesita aprender cada una?
- ¿Qué potencial tiene el vínculo si ambos polos se integran?

### Estructura

- Dinámica compartida.
- Cómo una persona influye en la otra.
- Influencia inversa.
- Reciprocidades.
- Desequilibrios.
- Síntesis.

Usar nombres reales, no A/B.

---

## 43. Ruedas artísticas

Miastro debe permitir diferentes representaciones para:

- consulta;
- impresión;
- exportación;
- presentación al consultante.

La rueda estándar será funcional y estable.

Los estilos artísticos serán una capa separada y no alterarán el cálculo.

---

## 44. Modos gráficos

### Modo Consulta

Prioriza:

- legibilidad;
- claridad;
- controles visibles;
- información compacta;
- sobriedad.

### Modo Presentación

Prioriza:

- acabado estético;
- más espacio;
- composición refinada;
- exportación;
- impresión;
- entrega al consultante;
- futuros temas gráficos.

---

## 45. Exportación e impresión

Miastro deberá permitir exportar:

- cartas;
- ruedas;
- informes.

También deberá permitir impresión.

Los formatos concretos se definirán en la arquitectura técnica.

---

## 46. Archivo de consultas y trabajos

Cada persona conservará historial de:

- cartas;
- revoluciones;
- tránsitos;
- progresiones;
- sinastrías;
- informes;
- trabajos realizados.

La estructura detallada de sesiones queda pendiente.

---

## 47. Identidad visual general

La identidad será:

> **Astrología contemporánea, clara y elegante, con fondo luminoso y la carta como protagonista.**

### Paleta base

- fondo general → blanco roto o marfil muy claro;
- paneles/tarjetas → blanco;
- líneas/divisores → gris cálido suave;
- color principal → azul grisáceo suave;
- secundario → dorado apagado o arena;
- texto principal → gris antracita;
- texto secundario → gris medio.

### Colores astrológicos

Con función, no como decoración:

- fuego → tonos cálidos;
- tierra → tonos terrosos;
- aire → tonos suaves y luminosos;
- agua → tonos azulados.

Los aspectos tendrán codificación propia.

Los tensos podrán tener más intensidad.

Quintil y biquintil serán más discretos.

### Tipografía

Una familia moderna y legible:

- Inter;
- Source Sans 3;
- o equivalente.

No usar tipografías “místicas” en la interfaz.

### Glifos

Deben ser:

- elegantes;
- clásicos;
- reconocibles;
- legibles en pantalla;
- válidos para impresión;
- homogéneos.

### Forma de interfaz

- bordes suaves;
- esquinas ligeramente redondeadas;
- espacios amplios;
- sombras mínimas;
- poca decoración;
- evitar exceso de cajas y botones.

Sensación buscada:

> **mesa de trabajo profesional**

### Jerarquía visual

> **Rueda → persona → información activa → controles**

### Modo oscuro

No se diseñará inicialmente.

Podrá estudiarse más adelante.

---

## 48. Reglas de coherencia entre módulos

Revolución Solar, Revolución Lunar, Tránsitos y Progresiones conservarán:

- rueda grande a la izquierda;
- panel derecho;
- controles equivalentes;
- interacción sincronizada;
- jerarquía visual;
- navegación común.

Solo cambiarán:

- cálculo;
- controles temporales;
- pestañas específicas;
- relación con la natal.

---

## 49. Reglas obligatorias de experiencia de uso

1. La rueda es protagonista.
2. Evitar ventanas flotantes.
3. Una pantalla central por persona.
4. Nueva persona a un clic.
5. Lugar de nacimiento con Buscar localidad.
6. Coordenadas y zona horaria histórica automáticas.
7. ASC y MC siempre visibles.
8. Quirón visible por defecto.
9. Aspectos visibles por defecto.
10. Puntos secundarios activables.
11. Glifos nunca solapados.
12. Disposición de glifos determinista.
13. Seleccionar un planeta sincroniza rueda y panel.
14. Sinastría independiente del orden de personas.
15. Los informes respetan ejes y polaridades.
16. La interfaz oculta complejidad técnica innecesaria.
17. Coherencia visual entre todas las técnicas.

---

## 50. Reglas obligatorias de interpretación

1. Interpretar ejes, no piezas aisladas.
2. Integrar polaridades.
3. Considerar contexto global.
4. En kármica: Casa 12, Ascendente, Nodos, Saturno, retrógrados y signos interceptados.
5. En astromédica: enfoque simbólico, no diagnóstico, con Júpiter como significador simbólico de la zona hepática.
6. En profesional: no dictar profesión única; identificar capacidades, entornos y funciones.
7. En pareja: análisis simétrico, sin puntuación simplista, integrando ambas direcciones.

---

## 51. Decisiones pendientes

- arquitectura técnica;
- lenguaje/framework de interfaz;
- motor astronómico concreto;
- base de datos;
- catálogo geográfico offline;
- formatos de exportación;
- estructura detallada de consultas;
- método exacto de progresión de ASC/MC;
- estilos artísticos concretos;
- posible incorporación de Vulcano;
- capa esotérica de regencias;
- Lilith Verdadera configurable;
- arco solar;
- direcciones;
- carta compuesta;
- retornos planetarios;
- dracónica;
- armónicos;
- sistema completo de generación de informes.

---

## 52. Estado del proyecto

Se considera suficientemente definida la:

- visión;
- funcionalidad principal;
- doctrina interpretativa;
- estructura de fichas;
- navegación;
- pantalla patrón;
- adaptación de técnicas;
- sinastría;
- reglas de conjunciones y solapamientos;
- identidad visual general.

---

## 53. Siguiente fase recomendada

> **Convertir este documento en una especificación técnica y de arquitectura antes de programar.**

Orden recomendado:

1. Arquitectura técnica.
2. Modelo de datos.
3. Motor de cálculo.
4. Catálogo geográfico offline.
5. Motor gráfico de la rueda.
6. Algoritmo de redistribución de glifos.
7. Sistema de informes.
8. Prototipo funcional mínimo.
9. Validación de cálculos y visualización.
10. Ampliación de módulos.

---

## 54. Regla final del proyecto

> **Miastro debe ser una herramienta profesional, clara, estable y coherente. La prioridad es que el cálculo sea fiable, la rueda sea legible y la interpretación tenga una identidad propia basada en ejes y polaridades.**
