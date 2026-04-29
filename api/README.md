# UniTreeGroup: Digital Stokvel & Micro-Savings Engine

UniTreeGroup is a production-ready backend system built with **ASP.NET Core 8** designed to digitize traditional stokvel (group savings) systems. The engine prioritizes financial integrity through double-entry bookkeeping, ensures data consistency with explicit transactions, and handles high-concurrency financial operations.

## 🚀 Key Features

### 💰 Financial Core
*   **Double-Entry Bookkeeping:** Every transaction (Deposit, Contribution, Payout) is recorded with balanced Debit and Credit ledger entries.
*   **Atomic Transactions:** Uses `IDbContextTransaction` to ensure financial operations either complete fully or roll back entirely.
*   **Optimistic Concurrency:** Implements `RowVersion` (concurrency tokens) on Wallets and Groups to prevent race conditions and double-spending.
*   **Audit Trail:** Comprehensive ledger history for every financial event.

### 👥 Stokvel Management
*   **Group Dynamics:** Create and manage Stokvel groups with customizable contribution amounts and payout cycles.
*   **Membership Logic:** Secure join/leave logic with validation (e.g., users limited to one active group).
*   **Automated Payouts:** A background worker (`AutomatedPayoutBackgroundService`) automatically triggers payouts based on defined schedules.

### 🛡️ Security & Reliability
*   **JWT Authentication:** Secure endpoints with Bearer token authentication.
*   **Global Exception Handling:** Middleware-driven error handling for consistent API responses.
*   **Soft Deletes:** Global EF Core filters to preserve data history while maintaining a clean "active" state.

## 🛠️ Tech Stack
*   **Framework:** ASP.NET Core 8 (Web API)
*   **Database:** PostgreSQL
*   **ORM:** Entity Framework Core (Npgsql)
*   **Auth:** JWT Bearer
*   **Testing:** xUnit, Microsoft.Data.Sqlite (In-memory testing)
*   **Documentation:** Swagger / OpenAPI with Security Definitions

## 🏗️ Architecture
The project follows a clean, layered architecture:
*   **Domain Layer:** Core entities (`User`, `Wallet`, `UniTreeGroup`, `LedgerEntry`), Enums, and Base models.
*   **Service Layer:** Business logic, use cases, and transaction orchestration.
*   **Data Layer:** Repository pattern, Unit of Work, and EF Core DbContext.
*   **Presentation Layer:** RESTful Controllers and Data Transfer Objects (DTOs).

## 🚦 Getting Started

### Prerequisites
*   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
*   [PostgreSQL](https://www.postgresql.org/download/)

### Setup Instructions
1.  **Clone the repository:**
    ```bash
    git clone https://github.com/your-repo/unitree-group.git
    cd unitree-group/api
    ```

2.  **Configure the Database:**
    Update the connection string in `appsettings.json`:
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Host=localhost;Database=UniTreeDb;Username=postgres;Password=your_password"
    }
    ```

3.  **Run Migrations:**
    ```bash
    dotnet ef database update
    ```

4.  **Seed Data:**
    The application automatically seeds initial users, groups, and sequences on startup via `AppSeeder`.

5.  **Run the Application:**
    ```bash
    dotnet run
    ```
    Access the API via `https://localhost:7001` (or your configured port). Swagger will be available at `/swagger`.

## 🧪 Testing
The project includes integration tests that validate the entire financial flow, including double-entry integrity and concurrency protection.
```bash
dotnet test
```

## 📖 API Documentation
Once the app is running, visit `/swagger` to explore the interactive documentation.
*   **Auth:** Use the `/api/Auth/login` endpoint to get a token, then click "Authorize" in Swagger and enter `Bearer {your_token}`.
*   **Transactions:** Explore Deposit, Withdrawal, and Contribution endpoints.

## 📝 Business Rules
*   Users must have a Wallet to perform transactions.
*   A contribution requires a user to be a member of the target Stokvel group.
*   The system maintains a balanced Ledger: `Sum(Debit) == Sum(Credit)` for every Transaction Id.
*   PostgreSQL sequences are reset after seeding to ensure ID incrementation remains consistent.

---
*Developed as a high-integrity financial engineering project.*