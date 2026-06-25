# MisDatos .NET SDK

SDK en C# (.NET Standard 2.0) puro para consumir la API de MisDatos.

Este repositorio contiene la versión para el ecosistema .NET (compatible con Mono, .NET Framework, .NET Core y .NET 5+). Mantiene la misma estructura, firmas y comportamiento que nuestra librería original.

## Características
* **Sin dependencias pesadas:** Utiliza `HttpClient` nativo y `System.Text.Json`.
* **Compatibilidad COM:** Preparado con etiquetas `[ComVisible(true)]` para ser compilado y registrado como interfaz COM en sistemas legacy si es necesario.
* **Síncrono:** Mantiene la ejecución secuencial bloqueante original para máxima compatibilidad con código procedural y legacy.

🔗 **Versión original:** Si buscas la implementación en Python, visita nuestro [Repositorio de Python](https://github.com/SergioMisDatos/misdatos-python-sdk).