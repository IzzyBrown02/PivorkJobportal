# pivrok – Web-Jobportal

> 🚧 **Status:** In aktiver Entwicklung (Work in Progress)

Eine moderne, modulare Web-Anwendung zur Verwaltung von Stellenangeboten und Bewerbungen mit getrennten, rollenbasierten Bereichen für Bewerber, Arbeitgeber und Administratoren.

---

## 🏗 Architektur & Designentscheidungen

Das Projekt setzt konsequent auf **Clean Architecture** und das **Repository Pattern**. Ziel ist eine vollständige Entkopplung der Geschäftslogik von der Benutzeroberfläche und der Datenbank.

### Schichtenarchitektur
* **Domain (Das Herz):** Enthält reine Fachobjekte (`User`, `JobPosting`, `Enums`) und Geschäftsregeln. Diese Schicht kennt weder die Datenbank noch das Web-UI – sie ist pure Logik.
* **Application (Das Gehirn):** Steuert die Use Cases der Anwendung und definiert Schnittstellen (z. B. `IUserRepository`), die festlegen, *was* getan werden muss.
* **Infrastructure (Die Muskeln):** Enthält die konkrete technische Umsetzung via Entity Framework Core 8, Microsoft SQL Server und ASP.NET Core Identity.

### ❓ Warum Clean Architecture?
* **Hohe Wiederverwendbarkeit:** Die Kern-Logik im "Herzen" ist völlig unabhängig von der Weboberfläche. Sie kann später ohne Änderungen für eine Smartphone-App weiterverwendet werden.
* **Einfache Wartung:** Ändert sich etwas an der Datenbank, muss nur die Infrastructure-Schicht angepasst werden. Der Rest des Codes bleibt unberührt.
* **Sauberer Code:** Das Projekt bleibt auch bei wachsendem Funktionsumfang übersichtlich, modular und leicht verständlich.

---

## 🔐 Authentifizierung: Eigenes System statt Microsoft-Scaffolding

Bei der Benutzeranmeldung wurde sich bewusst **gegen** das automatische Microsoft Identity Scaffolding (Razor Pages) und **für** ein eigenes MVC-basiertes Authentifizierungssystem entschieden.

### ❓ Warum diese Entscheidung?
1. **Kein Technologiemix:** Das automatische Gerüst von Microsoft generiert *Razor Pages*. Das hätte zu einer unsauberen Mischung aus Razor Pages und MVC im selben Projekt geführt.
2. **Keine Architektur-Brüche:** Das Standard-Gerüst injiziert den `UserManager` direkt in die Benutzeroberfläche. In dieser Anwendung spricht der `AccountController` in der UI-Schicht ausschließlich mit der Schnittstelle `IUserRepository`. Die UI weiß nicht einmal, dass eine Datenbank existiert.
3. **Zukunftssicher für Mobile Apps:** Für eine spätere Smartphone-App kann ein `AccountApiController` exakt dasselbe `IUserRepository` nutzen. Die komplette Login- und Sicherheitslogik muss nur ein einziges Mal geschrieben werden.

---

## ✨ Features & Rollenkonzept

### 👤 Jobsuchender (Jobseeker)
* Erstellung und Pflege des eigenen Bewerberprofils.
* Gezielte Suche und Filterung von Stellenanzeigen.
* Speichern von Favoriten und direktes Bewerben auf offene Stellen.

### 🏢 Arbeitgeber & Recruiter (Employer)
* Erstellung und Verwaltung von Stellenanzeigen mit automatischer Übernahme von Firmenstammdaten.
* **N:M Recruiter-Beziehung:** Ein Recruiter oder Headhunter kann mit einem einzigen Account Stellenanzeigen für verschiedene Unternehmen oder Tochtergesellschaften betreuen.
* **Flexibler Rollenwechsel (Dual-Role):** Ein Nutzerkonto kann problemlos beide Rollen (Jobsuchender und Arbeitgeber) einnehmen und je nach Anmeldung den passenden Bereich nutzen.

### 🛡️ Company-Admin & Verifikations-Workflow
Um Missbrauch durch gefälschte Firmenprofile zu verhindern, greift ein dreistufiger Sicherheits-Prozess:
1. **Ersteller-Prinzip:** Der erste Nutzer, der eine neue Firma mit geschäftlicher E-Mail-Adresse registriert, wird vom System automatisch als vorläufiger `CompanyAdmin` eingestuft.
2. **Sperre vor Verifizierung (Pending):** Das Firmenprofil und erstelle Stellenanzeigen erhalten den Status `Pending`. Sie sind im System eingerichtet, aber auf der Live-Seite für Bewerber noch nicht sichtbar.
3. **Manueller Support-Check:** Der Super-Admin prüft hochgeladene Nachweise und Unternehmensdaten im Backend. Erst nach manueller Freischaltung (`IsVerified = true`) gehen das Firmenprofil und alle zugehörigen Jobanzeigen live.

### ⚙️ Super-Admin (Portal-Betreiber)
* Vorkonfigurierter Admin-Account (Seeded Admin) beim Systemstart.
* Globale Benutzerverwaltung (Sperren, Entsperren und Vergabe von Rollen an bestehende Benutzer).
* Zentrale Übersicht aller im System existierenden Stellenanzeigen und ausstehenden Firmen-Freischaltungen.

---

## 📊 Entwicklungsstand & Roadmap

### ⏳ Umgesetzte Kernfunktionen (Status Quo)
* **Domain- & Datenmodellierung:** Vollständige Modellierung von Stellenanzeigen (`JobPosting`), Unternehmensprofilen (`CompanyProfile`) und Benutzerdaten.
* **Authentifizierungs- & Rollenlogik:** Eigenes Repository-System für flexible Zugriffsrechte und Rollenverteilung im Backend.
* **UI-Basis:** Erste Weboberflächen zur Erstellung und Übersicht von Stellenanzeigen im MVC-Muster.

### 🚀 Nächste Schritte (Roadmap)
* **Verifikations-Frontend:** Benutzeroberfläche für das Freischalten ausstehender Firmen im Admin-Dashboard.
* **Such- & Filter-Funktion:** Erweiterte Suche nach Jobtiteln, Kategorien und Standorten für Bewerber.
* **Bewerbungsprozess:** Formular und Dateiupload-Logik für Lebensläufe/Dokumente.
* **Frontend-Feinschliff:** Ausbau des Responsive Designs mittels Bootstrap 5, SweetAlert2 und DataTables.

---

## 💻 Tech Stack & NuGet-Pakete

| Kategorie | Technologie / Tool |
| :--- | :--- |
| **Framework & Sprache** | C# / ASP.NET Core MVC (.NET 8) |
| **Datenbank & ORM** | Microsoft SQL Server, Entity Framework Core 8 (Code-First) |
| **Authentifizierung** | ASP.NET Core Identity (Custom ApplicationUser, Cookie-Auth via AccountController & Repository) |
| **Frontend** | Bootstrap 5, Bootstrap Icons, SweetAlert2, DataTables |
| **Entwicklungsumgebung** | Visual Studio 2022 |

### Installierte NuGet-Pakete (Infrastructure)
* **`Microsoft.EntityFrameworkCore.SqlServer`:** Der Datenbank-Treiber, damit C# zur Laufzeit mit dem SQL-Server kommunizieren kann.
* **`Microsoft.AspNetCore.Identity.EntityFrameworkCore`:** Der fertige Baustein für das Login- und Registrierungssystem (Identity-Tabellen).
* **`Microsoft.EntityFrameworkCore.Tools`:** Werkzeug-Set für Visual Studio, um Migrations-Befehle (`Add-Migration`, `Update-Database`) in der Konsole auszuführen.

---

## 🎓 Projektkontext

Dieses Projekt entsteht als benotetes Projekt im Rahmen der Umschulung zur **Fachinformatikerin für Anwendungsentwicklung (IHK)**.
