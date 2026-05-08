# GradeHub Integration Middleware

This project implements an automated Student Grade Sync & Notification System.

## How to Manual Test (Happy Path)

To test the complete "Happy Path" (REST -> SOAP -> SMTP), follow these exact steps:

### 1. Prerequisites (SMTP Mock)
Because the middleware sends an email upon successful SOAP storage, you need a dummy SMTP server running locally on port **1025**.
- **Download [Papercut SMTP](https://github.com/ChangemakerStudios/Papercut-SMTP) (Windows)** or use Docker: `docker run -p 1080:1080 -p 1025:1025 maildev/maildev`
- Start it. It must listen on `localhost:1025`.

### 2. Start CIS Mock (Terminal 1)
Open a terminal in the root folder and run:
```powershell
cd src/GradeHub.CIS.Mock
dotnet run
```
*(Wait for "Now listening on: http://localhost:5066")*

### 3. Start Middleware (Terminal 2)
Open a second terminal in the root folder and run:
```powershell
cd src/GradeHub.Middleware
dotnet run
```
*(Wait for "Now listening on: http://localhost:5188")*

### 4. Send Test Request (REST)
Open Postman (or use PowerShell) and send a JSON payload to the Middleware:

- **Method:** `POST`
- **URL:** `http://localhost:5188/api/grades`
- **Headers:** `Content-Type: application/json`
- **Body (raw JSON):**
```json
{
  "studentEmail": "student@technikum-wien.at",
  "courseName": "Systems Integration",
  "gradeValue": "1",
  "professor": "Prof. Smith"
}
```

### 5. Verify the Happy Path
1. **REST Response:** Postman should return `200 OK` with `{"message": "Grade processed successfully."}`.
2. **SOAP Success:** Check `src/GradeHub.CIS.Mock/grades.csv`. A new line with your test data should appear.
3. **SMTP Success:** Check your local SMTP Mock (Papercut/Maildev). You should see a new email with the subject `"Grade recorded for Systems Integration"`.