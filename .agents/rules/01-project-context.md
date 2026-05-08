# Projekt-Kontext: GradeHub Integration Middleware

## 1. Projekt-Übersicht
Wir entwickeln eine "Post Office" Middleware in C# .NET 8/9. Die Anwendung fungiert als intelligenter Router für Universitätsnoten. Das Ziel ist es, eingehende Notendaten von einem Quellsystem (Moodle) zu validieren, zu transformieren und gleichzeitig an zwei unterschiedliche Zielsysteme zu verteilen.

## 2. System-Architektur & Schnittstellen
Das gesamte Projekt wird als Monorepo geführt und besteht aus zwei Hauptkomponenten:

### A. GradeHub.Middleware (Hauptprojekt)
- **Eingang (Inbound):** REST-Schnittstelle (JSON). Erwartet Daten wie `StudentEmail`, `CourseName`, `GradeValue` und `Professor`.
- **Ausgang 1 (Integration):** SOAP/XML. Kommuniziert mit dem CIS Mock Service.
- **Ausgang 2 (Notification):** SMTP. Versendet eine Bestätigungs-E-Mail an den Studierenden (via MailKit).
- **Technologie:** ASP.NET Core Minimal API.

### B. GradeHub.CIS.Mock (Simulations-System)
- **Funktion:** Simuliert ein Legacy Campus-Informations-System (CIS).
- **Technologie:** ASP.NET Core 8/9 unter Verwendung von **SoapCore**. 
- **Verarbeitung:** Empfängt SOAP-Requests, extrahiert die Daten und speichert sie persistent in einer Datei namens `university_records.xml` im lokalen Verzeichnis.
- **Vorteil:** Ersetzt das alte .NET Framework WCF durch eine moderne, plattformunabhängige Lösung.



## 3. Technische Anforderungen & Standards
- **Sprache:** C# .NET 8 oder höher.
- **Programmierstil:** - Konsequente Nutzung von `async/await`.
    - Datenmodelle sollen als `record` Typen (Immutable) definiert werden.
    - Verwendung von Dependency Injection (DI) für den SOAP-Client und den E-Mail-Service.
- **Transformation:** Die Middleware muss die Logik enthalten, um flache JSON-Objekte in die für SOAP erforderliche XML-Struktur zu mappen.
- **Resilienz:** Implementierung von Basis-Fehlerbehandlung (Try-Catch), falls eines der Zielsysteme (SOAP oder SMTP) nicht erreichbar ist.
- **Logging:** Jeder Verarbeitungsschritt (Empfang, Transformation, Versand) muss über `ILogger` protokolliert werden.

## 4. Projekt-Struktur (Monorepo)
/GradeHub.sln
/src/GradeHub.Middleware (Modern REST API)
/src/GradeHub.CIS.Mock (Modern SOAP Service via SoapCore)
/docs/ (Pflichtenheft & Diagramme)
