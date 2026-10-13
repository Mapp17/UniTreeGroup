# UniTreeGroup Endpoint Coverage Matrix

## Scope

Frontend evidence is limited to `app/page.tsx`. It contains no API client or network requests. Therefore every endpoint below is either **MISSING** from the frontend, **UNVERIFIED** where the required backend source was unavailable, or **NOT DIRECTLY UI-FACING** for internal financial processing.

The inspected controllers expose **16 HTTP endpoints**: 1 auth, 5 group, 5 transaction, 3 user, and 2 payout endpoints. A separate wallet controller file was present but returned no inspectable source and is not included in the confirmed count.

## Matrix

| # | Method and route | Purpose | Auth / permission | Request | Response | Frontend coverage | Required correction |
|---:|---|---|---|---|---|---|---|
| 1 | `POST /api/Auth/login` | Authenticate a user and issue JWT | No `[Authorize]` shown on action; controller not marked authorized | `{ email, password }` via `LoginDto` | `{ token }` | **MISSING** — login form only toggles local state | Submit credentials, persist token securely for the session, handle 401, and load authenticated user data. |
| 2 | `POST /api/User/register` | Create a user and initialize wallet | No auth attribute shown | `{ fullName, email, phoneNumber }` via `CreateDto` | Created `User` entity | **MISSING** — no registration workflow | Add registration form; do not assume password support because `CreateDto` has no password field. Verify how a new user can subsequently authenticate. |
| 3 | `GET /api/User` | List users | No auth attribute shown | None | `UserDto[]`: full name, email, phone number | **MISSING** | Add only if a product workflow needs user discovery; protect or restrict it if backend policy changes. |
| 4 | `GET /api/User/{id}/wallet` | Read a user's wallet | No auth attribute shown | Route `id` | `UserWalletDto`: user ID, full names, balance, currency | **MISSING** | Use the authenticated user ID after login; display balance and currency from response, not hardcoded values. |
| 5 | `POST /api/unitree-groups` | Create a savings group | No auth attribute shown | `{ name, description, createdById, contributionAmount, payoutCycle, maxMembers }` | `GroupReadDto` | **PARTIAL** — create button opens an informational banner only | Add a validated form, use authenticated creator ID, map numeric `payoutCycle`, and refresh group data after success. |
| 6 | `GET /api/unitree-groups` | List all groups | No auth attribute shown | None | `GroupReadDto[]` | **MISSING** — groups are hardcoded | Fetch and map name, description, creator, contribution, cycle, capacity, count, status, and pool balance. |
| 7 | `GET /api/unitree-groups/{id}` | Read one group | No auth attribute shown | Route `id` | `GroupReadDto` | **MISSING** | Add group detail view using the returned ID and fields. |
| 8 | `POST /api/unitree-groups/{id}/join` | Join a group | No auth attribute shown | Body is a raw integer `userId` | `{ message }` | **MISSING** | Add join action and send the exact raw integer body; handle capacity and one-group-only conflict errors. |
| 9 | `GET /api/unitree-groups/{id}/members` | List group members | No auth attribute shown | Route `id` | `GroupMemberDto[]`: user ID, full name, email, joined at | **MISSING** | Add member list/detail view and format joined timestamps. |
| 10 | `POST /api/Transactions/deposit` | Deposit into wallet | No `[Authorize]` shown | `{ userId, uniTreeGroupId?, amount, reference, description? }` | `TransactionReadDto` | **MISSING** | Add deposit form; validate positive amount and unique reference; update wallet only after success. |
| 11 | `POST /api/Transactions/withdraw` | Withdraw from wallet | No `[Authorize]` shown | Same `TransactionRequestDto` | `TransactionReadDto` | **MISSING** | Add withdrawal form; handle insufficient funds and duplicate references. |
| 12 | `POST /api/Transactions/contribute` | Move wallet funds into group pool | No `[Authorize]` shown | Same DTO, with required `uniTreeGroupId` | `TransactionReadDto` | **MISSING** | Add group-aware contribution form; handle membership and balance guards. |
| 13 | `GET /api/Transactions/user/{id}` | Read user transaction history | No `[Authorize]` shown | Route `id` | `TransactionReadDto[]` | **PARTIAL** — table/search exists, but data is hardcoded | Replace local array with API response and map `transactionId`, `type`, `status`, `amount`, `currency`, `reference`, nullable description, and `createdAt`. |
| 14 | `GET /api/Transactions/{id}/ledger` | Read a transaction/ledger record | No `[Authorize]` shown | Route `id` | `TransactionReadDto` according to controller/service mapping | **MISSING** | Add transaction detail or statement view; verify whether this endpoint returns ledger lines or only the transaction DTO because current service mapping returns `TransactionReadDto`. |
| 15 | `POST /api/Payouts/schedule` | Schedule a payout | `[Authorize(Roles = "Admin")]` | `{ beneficiaryUserId, amount, scheduledDate }` | `PayoutReadDto` | **MISSING** | Add admin-only workflow after role claims are verified; validate future date and positive amount. |
| 16 | `GET /api/Payouts/group/{id}` | Read payouts for beneficiary/user ID | `[Authorize(Roles = "Admin")]` | Route `id` (service treats it as beneficiary ID) | `PayoutReadDto[]` | **MISSING** | Add admin-only payout history and clarify route naming/semantics with backend owners; it is called `group` but service queries beneficiary user ID. |

## Field mapping details

### `TransactionRequestDto`

- `userId`: integer, required by service lookup.
- `uniTreeGroupId`: nullable integer; required for contribution and payout processing, though payout is not exposed by `TransactionsController`.
- `amount`: decimal; must be greater than zero.
- `reference`: string; must be unique according to the service check.
- `description`: nullable string.

### `TransactionReadDto`

- `transactionId`: integer.
- `userId`: integer.
- `type`: string enum value: `Deposit`, `Withdrawal`, `Contribution`, `Payout`, or `Transfer`.
- `status`: string enum value: `Pending`, `Completed`, `Failed`, or `Reversed`.
- `amount`: decimal.
- `currency`: string, defaulting to `ZAR`.
- `reference`: string.
- `description`: nullable string.
- `createdAt`: timestamp.

The current frontend incorrectly substitutes `id` and `date` fields in its local type and does not represent `currency`, `transactionId`, nullable description, or the backend timestamp shape.

### `GroupReadDto`

- `id`, `name`, `description`, `createdByUserName`, `contributionAmount`, `payoutCycle`, `maxMembers`, `currentMemberCount`, `status`, and `poolBalance`.

The current frontend's local group type uses `contribution`, `members`, `pool`, and `progress`, none of which are backend response property names. `progress` is not returned by the backend and must not be treated as an API field without a defined calculation.

### `UserWalletDto`

- `userId`, `fullNames`, `balance`, and `currency`.

The frontend currently uses local `name`, `email`, and `balance`; email is not present in this wallet DTO and must come from a user/profile response instead.

### `PayoutReadDto`

- `id`, `beneficiaryUserId`, `beneficiaryName`, `amount`, `scheduledDate`, and `status`.

### `CreateGroupDto`

- `name`, `description`, `createdById`, `contributionAmount`, `payoutCycle`, and `maxMembers`.
- Service rules: positive contribution, at least 2 max members, valid creator, and unique name.

### `CreateDto`

- `fullName`, `email`, and `phoneNumber`.
- There is no password field in the inspected registration DTO, creating a gap between registration and the email/password login contract that needs backend clarification.

## Business-rule coverage

| Rule | Backend evidence | Frontend status |
|---|---|---|
| Positive transaction amount | `TransactionsServices` | Missing validation and submission |
| Unique transaction reference | `TransactionsServices` | Missing |
| Wallet must exist | Transaction service | Missing error state |
| Withdrawal balance guard | Transaction service | Missing |
| Contribution requires group ID | Transaction service | Missing |
| Contribution requires membership | Transaction service | Missing |
| Contribution requires sufficient funds | Transaction service | Missing |
| Group name uniqueness | Group service | Missing |
| Group contribution must be positive | Group service | Missing |
| Group capacity at least 2 | Group service | Missing |
| User may join only one group | Group service | Missing |
| Group capacity limit | Group service | Missing |
| Payout amount positive | Payout service | Missing |
| Payout date future | Payout service | Missing |
| Payout beneficiary exists | Payout service | Missing |
| Payout endpoints admin-only | Controller attributes | Missing role-aware UI and authorization handling |
| Double-entry ledger persistence | Transaction service/tests | Not directly UI-facing; transaction detail should expose a truthful result |
| Concurrency protection | README/tests claim it; exact entity configuration not reviewed here | Requires live backend testing |

## Classification notes

No endpoint is **COMPLETE**. The transactions table is the only area with a corresponding UI surface, but it is fed entirely by hardcoded data and therefore is **PARTIAL**, not complete. The create-group UI is also **PARTIAL** because it has no request. All other confirmed endpoints are **MISSING** from the frontend. Wallet-controller operations and background payout behavior are **UNVERIFIED** because their source was unavailable in the audit evidence.

## Live verification checklist

- Run Swagger/OpenAPI and compare generated schemas with DTO casing.
- Obtain a real JWT and verify login response and expiry.
- Verify whether the token contains an `Admin` role claim; the inspected `AuthService` adds name identifier, email, and name claims but no role claim.
- Confirm the registration-to-login password lifecycle.
- Test each financial endpoint with valid, invalid, duplicate-reference, insufficient-funds, non-member, and unauthorized inputs.
- Confirm that a contribution creates exactly two ledger entries and that repeated/concurrent submissions cannot double-spend.
- Confirm payout route semantics and automated background processing.
  
