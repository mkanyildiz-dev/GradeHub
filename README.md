# GradeHub Integration Middleware

This project implements an automated Student Grade Sync & Notification System.

## How to Manual Test (Happy Path)

To test the complete "Happy Path" (REST -> SOAP -> SMTP), follow these exact steps:

### 1. Prerequisites (SendGrid Setup)
The middleware sends real email notifications using **SendGrid**. You must configure your API key securely before running:
1. Create a free SendGrid account and verify a **Single Sender** email address.
2. Generate a SendGrid API Key.
3. Open a terminal in `src/GradeHub.Middleware` and save the key to Windows User Secrets:
   ```powershell
   dotnet user-secrets init
   dotnet user-secrets set "SendGrid:ApiKey" "SG.YOUR_REAL_API_KEY_HERE"
   ```
4. Open `src/GradeHub.Middleware/appsettings.Development.json` and change `"SenderEmail"` to match the exact email address you verified in SendGrid.

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
  "studentEmail": "YOUR_PERSONAL_TEST_EMAIL@gmail.com",
  "courseName": "Systems Integration",
  "gradeValue": "1",
  "professor": "Prof. Smith"
}
```
*(Note: Use a real email address for `studentEmail` so you can verify receipt!)*

### 5. Verify the Happy Path
1. **REST Response:** Postman should return `200 OK` with `{"message": "Grade processed successfully."}`.
2. **SOAP Success:** Check `src/GradeHub.CIS.Mock/grades.csv`. A new line with your test data should appear.
3. **SMTP Success:** Check the inbox of the email address you put in `studentEmail`. You should receive a real email from your SendGrid verified sender!