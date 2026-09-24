# pivrok – Web-Jobportal

> 🚧 **Status:** In aktiver Entwicklung (Work in Progress)

<img width="994" height="803" alt="image" src="https://github.com/user-attachments/assets/c8d5e88d-5c39-40ed-b53d-83e6ae989510" />


Eine moderne, modulare Web-Anwendung zur Verwaltung von Stellenangeboten und Bewerbungen mit getrennten, rollenbasierten Bereichen für Bewerber, Arbeitgeber und Administratoren.

---

## 🏗 Architektur & Designentscheidungen

Das Projekt setzt konsequent auf **Clean Architecture** und das **Repository Pattern**. Ziel ist eine vollständige Entkopplung der Geschäftslogik von der Benutzeroberfläche und der Datenbank.

### Schichtenarchitektur
* **Domain (Das Herz):** Enthält reine Fachobjekte (`User`, `JobPosting`,`CompanyProfile`, `Enums`,`CompanyRecruiter`) und Geschäftsregeln. Diese Schicht kennt weder die Datenbank noch das Web-UI – sie ist pure Logik.
* **Application (Das Gehirn):** Steuert die Use Cases der Anwendung und definiert Schnittstellen (z. B. `IUserRepository`), die festlegen, *was* getan werden muss.
* **Infrastructure (Die Muskeln):** Enthält die konkrete technische Umsetzung via Entity Framework Core 8, Microsoft SQL Server und ASP.NET Core Identity.

### ❓ Warum Clean Architecture?
* **Hohe Wiederverwendbarkeit:** Die Kern-Logik im "Herzen" ist völlig unabhängig von der Weboberfläche. Sie kann später ohne Änderungen für eine Smartphone-App weiterverwendet werden.
* **Einfache Wartung:** Ändert sich etwas an der Datenbank, muss nur die Infrastructure-Schicht angepasst werden. Der Rest des Codes bleibt unberührt.
* **Sauberer Code:** Das Projekt bleibt auch bei wachsendem Funktionsumfang übersichtlich, modular und leicht verständlich.

---

## 🔐 Authentifizierung: Eigenes System statt Microsoft-Scaffolding

Bei der Benutzeranmeldung wurde sich bewusst **gegen** das automatische Microsoft Identity Scaffolding (Razor Pages) und **für** ein eigenes MVC-basiertes Authentifizierungssystem (`AuthController` mit Identity-, Login- und Register-Views)entschieden.

### ❓ Warum diese Entscheidung?
1. **Kein Technologiemix:** Das automatische Gerüst von Microsoft generiert *Razor Pages*. Das hätte zu einer unsauberen Mischung aus Razor Pages und MVC im selben Projekt geführt.
2. **Flexible Schlüssel & Rollen:** Umstellung der Identity-Primärschlüssel auf **GUIDs** und Nutzung einer Rollen-Liste im Domain-User, um nahtlose Mehrfachrollen zu ermöglichen.
3. **Automatisches Rollen-Seeding:** Beim Anwendungsstart (`async Task Main in Program.cs`) werden Systemrollen automatisch und typsicher aus dem `UserRole`-Enum in der Datenbank angelegt.
4. **Zukunftssicher für Mobile Apps:** Für eine spätere Smartphone-App kann ein `AccountApiController` exakt dasselbe `IUserRepository` nutzen. Die komplette Login- und Sicherheitslogik muss nur ein einziges Mal geschrieben werden.

---

## ✨ Features & Rollenkonzept

### 👤 Bewerber (Jobseeker)
* Erstellung und Pflege des eigenen Bewerberprofils.
* Gezielte Suche und Filterung von Stellenanzeigen.
* Speichern von Favoriten und direktes Bewerben auf offene Stellen.

### 🏢 Arbeitgeber (Recruiter)
* Erstellung und Verwaltung von Stellenanzeigen mit automatischer Übernahme von Firmenstammdaten.
* **M:N Recruiter-Beziehung(`CompanyRecruiter`):** Ein Recruiter oder Headhunter kann mit einem einzigen Account Stellenanzeigen für verschiedene Unternehmen oder Tochtergesellschaften betreuen, sowie ein Unternehmen mehrere Recruiter(zb. durch verschiedene Fachabteilungen).
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
* **Domain- & Datenmodellierung:** Vollständige Modellierung von Stellenanzeigen (`JobPosting`), Unternehmensprofilen (`CompanyProfile`) und Benutzerdaten (`User`).
* **Authentifizierungs- & Auth-Flow:** Eigener `AuthController` mit vollfunktionsfähigen Views für Identity, Login und Register sowie GUID-basiertem Identity-Setup.
* **Async App-Bootstrapping:** Asynchrone `Main`-Methode in `Program.cs` mit automatischem Datenbank-Seeding für Identity-Rollen aus dem `UserRole`-Enum.
* **UI-Basis:** Stabile Flexbox-Struktur (`_Layout.cshtml`, `site.css`) für grüne Bewerber- und Allgemeinbereich ohne Overlay- oder Footer-Fehler.

### 🚀 Nächste Schritte (Roadmap)
* **Recruiter-Domain-Erweiterung:** Einbindung der `CompanyRecruiter`-Entität in die Domain und EF Core M:N-Mapping zur Verknüpfung von Recruitern und Firmen.
* Arbeitgeber-Portal (`RecruiterController`): Aufbau der blauen Recruiter-Landingpage (`Recruiter/Index`) und das Zuordnen von Firmenprofilen beim Inserieren.
* **Verifikations-Frontend:** Benutzeroberfläche für das Freischalten ausstehender Firmen im Admin-Dashboard.
* **Such- & Filter-Funktion:** Erweiterte Suche nach Jobtiteln, Kategorien und Standorten für Bewerber.
* **Bewerbungsprozess:** `Bewerberprofile` anlegen, um Formular und Dateiupload-Logik für Lebensläufe/Dokumente zu erstellen.

---

## 💻 Tech Stack & NuGet-Pakete

| Kategorie | Technologie / Tool |
| :--- | :--- |
| **Framework & Sprache** | C# / ASP.NET Core MVC (.NET 8) |
| **Datenbank & ORM** | Microsoft SQL Server, Entity Framework Core 8 (Code-First) |
| **Authentifizierung** | ASP.NET Core Identity (Custom ApplicationUser, Cookie-Auth via AccountController) |
| **Frontend** | Bootstrap 5, Bootstrap Icons, SweetAlert2, DataTables |
| **Entwicklungsumgebung** | Visual Studio 2022 |

### Installierte NuGet-Pakete (Infrastructure)
* **`Microsoft.EntityFrameworkCore.SqlServer`:** Der Datenbank-Treiber, damit C# zur Laufzeit mit dem SQL-Server kommunizieren kann.
* **`Microsoft.AspNetCore.Identity.EntityFrameworkCore`:** Der fertige Baustein für das Login- und Registrierungssystem (Identity-Tabellen).
* **`Microsoft.EntityFrameworkCore.Tools`:** Werkzeug-Set für Visual Studio, um Migrations-Befehle (`Add-Migration`, `Update-Database`) in der Konsole auszuführen.

---

## 🎓 Projektkontext

Dieses Projekt entsteht als benotetes Projekt im Rahmen der Umschulung zur **Fachinformatikerin für Anwendungsentwicklung (IHK)**.
