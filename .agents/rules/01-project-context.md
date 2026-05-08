# Projekt GradeHub Middleware

## Kontext
Wir entwickeln ein Universitäts-Projekt in C# .NET 8.
Ziel ist eine Middleware, die als Vermittler zwischen Moodle (REST/JSON Input) und dem Campus-System CIS (SOAP/XML Output) agiert. Gleichzeitig versenden wir E-Mails via SMTP.

## Architektur
- **Middleware:** ASP.NET Core Minimal API.
- **CIS Mock:** Ein separates WCF-Projekt, das Daten in `university_records.xml` speichert.
- **Mail:** Nutzung von `MailKit` für den SMTP-Versand.

## C# Code-Regeln
- Schreibe modernen, asynchronen Code (`async/await`).
- Nutze das `Microsoft.Extensions.Logging` Framework für jeden Schritt.
- Alle Datenmodelle (JSON und XML) sollen als saubere `record` Typen implementiert werden, nicht als reguläre Klassen.
