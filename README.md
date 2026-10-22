# 🩸 LifeLine – Hyperlocal Blood Donor Network

A modern web application built with **ASP.NET Core 8 MVC**, **Entity Framework Core**, and **SQLite** that connects blood donors with patients in urgent need based on **blood group compatibility**, **hyperlocal proximity**, and **donor medical eligibility**.

---

## ✨ Features

- **User Authentication & Profiles:** Secure cookie-based authentication with salted password hashes (PBKDF2), role management, and donor preferences.
- **Blood Requests:** Post urgent requests specifying patient name, units needed, blood group, hospital location, and urgency levels (*Normal*, *Urgent*, *Critical*).
- **Biological Compatibility Engine:** Automatic medical matching (e.g., $O^-$ universal donor can give to all, $AB^+$ can receive from all).
- **90-Day Medical Cooldown:** Automatic enforcement of the standard 90-day cooldown interval to safeguard donor health.
- **Real-Time Donor Directory:** Search eligible donors by needed group and city/area.
- **Direct Donor Responses:** Donors can volunteer with a single click and send an arrival message.
- **Fulfillment & Contact Privacy:** Contact phone numbers are visible only to the requester and logged-in users to protect donor privacy. Marking a donor as *Donated* triggers cooldown and auto-fulfills the request when unit requirements are met.
- **Input Validation:** Strict client-side and server-side validation on mobile numbers (`^[6-9]\d{9}$`), names, passwords, and sanitization against injection.
- **Modern UI:** Built with Bootstrap 5.3, Bootstrap Icons, responsive layouts, active navigation, and auto-dismissing notifications.

---

## 🏗️ Tech Stack

- **Framework:** ASP.NET Core 8.0 MVC
- **Language:** C# 12 (.NET 8 SDK)
- **Data Access:** Entity Framework Core 8 (Code-First)
- **Database:** SQLite (`lifeline.db`)
- **Frontend:** Razor Views, Bootstrap 5.3, Bootstrap Icons, jQuery Validation

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Clone & Run
```bash
# Clone the repository
git clone https://github.com/RAJJBHALARA/LifeLine.git
cd LifeLine

# Run the project
dotnet run
```

Open your browser at `http://localhost:5000` (or `https://localhost:5001`). The SQLite database (`lifeline.db`) is automatically initialized and seeded on first run.

---

## 🔑 Demo Accounts

Use any of these pre-seeded demo users to test:
- **Email:** `aarav@demo.com`
- **Password:** `Password@123`

*(Other demo accounts: `priya@demo.com`, `rohan@demo.com`, `neha@demo.com` with the same password)*

---

## 📚 Project Documentation & Viva Preparation
For detailed architecture flowcharts, ER diagrams, business logic rules, and 30+ faculty viva questions with answers, see [VIVA_GUIDE.md](VIVA_GUIDE.md).

---

## 📄 License
This project is open-source and available under the [MIT License](LICENSE).
