# UniTreeGroup Frontend-to-Backend Audit

## Scope and evidence

This is an audit only. No application source files were modified. The reviewed frontend is `app/page.tsx`; it contains a single client-rendered route with local state and hardcoded data. The reviewed backend evidence includes the available controllers, services, DTOs, enums, `Program.cs`, README, API testing document, and `FinancialFlowTests.cs` from the `Mapp17/UniTreeGroup` repository.

The wallet controller file could not be inspected successfully because the available file read returned no source content. Wallet-related behavior is therefore marked unverified where it depends on that controller.

## 1. Existing frontend features

- Dashboard with hardcoded wallet balance, user identity, contribution total, active group count, groups, and recent transactions.
- Transactions page with client-side search over the hardcoded transaction array.
- My Groups page with hardcoded group cards and a create-group button that only shows a dismissible informational banner.
- Profile page with hardcoded account details and a local sign-out state transition.
- Settings page with local checkbox state and a Save button with no handler or persistence.
- Help page with static informational cards.
- Login screen with prefilled credentials and a local `signedIn` boolean; no request is sent.
- Responsive sidebar/top navigation and local tab switching.
- Export report, filter, notifications, wallet details, and create-group controls are present visually but do not perform backend operations.

## 2. Overall conclusion

The frontend currently has **zero verified real backend integrations**. There is no API client, fetch call, route handler, server action, backend URL configuration, token storage, request mapping, response mapping, loading state, or API error state in the reviewed frontend. All financial and identity values shown to a user are hardcoded in `app/page.tsx`.

The UI is a visual prototype, not a connected client. Every backend-facing workflow remains missing or unverified.

## 3. Backend capabilities discovered

- Authentication: login endpoint returning a JWT.
- User management: registration and user listing.
- Wallet read: user wallet lookup is exposed by `UserController`; a separate wallet controller could not be inspected.
- Groups: create, list, detail, join, and members.
- Transactions: deposit, withdrawal, contribution, user history, and transaction/ledger lookup.
- Payouts: admin-only payout scheduling and admin-only payout lookup.
- Background payout processing is registered in `Program.cs` but the worker implementation was not among the inspected files.
- Double-entry ledger creation and financial guards are implemented in `TransactionsServices`.

## 4. Highest-priority findings

1. Add a real authentication flow: registration/login request handling, JWT lifecycle, protected route behavior, logout, and unauthorized-state handling.
2. Replace all hardcoded user, wallet, transaction, and group data with API reads.
3. Implement deposit, withdrawal, and contribution forms with the exact backend payloads and server error handling.
4. Implement group creation, group discovery/detail, join, and member views.
5. Implement transaction history and ledger detail views.
6. Implement payout views only for authorized administrators; do not expose admin actions to ordinary members.
7. Add loading, empty, validation, duplicate-reference, insufficient-funds, membership, conflict, and authorization states.
8. Verify API base URL, CORS, HTTPS certificate behavior, and JWT claims against a running backend.

## 5. Financial and security concerns to account for in the client

- A contribution requires `UniTreeGroupId`, membership in that group, and sufficient wallet funds.
- Withdrawals and contributions reject insufficient funds.
- Deposits, withdrawals, contributions, and payouts use a unique `Reference`; duplicate references produce a conflict.
- Transaction amounts must be greater than zero.
- Payouts require a group ID, sufficient pool funds, and a future schedule date when scheduled through the payout service.
- Group membership is limited to one active group and group capacity is enforced.
- JWT is required by the backend where `[Authorize]` is present. Payout endpoints require the `Admin` role, but the inspected `AuthService` token claims do not include a role claim; this requires live verification and may prevent admin authorization.
- The frontend must not treat local state changes as proof of a successful financial operation. It should update displayed data only after a successful API response.
- Money should remain decimal-safe and be displayed using the response currency; dates should preserve the backend timestamp and be formatted intentionally for the user's timezone.

## 6. Items requiring live backend testing

- Exact wallet-controller route and response shape.
- Whether authentication is actually globally required or only applied to controllers/actions carrying `[Authorize]`.
- Whether JWT role authorization works for payout endpoints, given the visible token construction.
- CORS and frontend origin configuration.
- Actual seeded user credentials and whether login accepts the seeded password format.
- Error response JSON shape from `ExceptionHandlingMiddleware`.
- Concurrency behavior under real PostgreSQL requests.
- Background payout scheduling/execution behavior.
- Swagger-generated parameter schemas and actual casing/serialization settings.

## 7. Recommended implementation order

### P0 — Identity and data foundation

Create one typed API client, configure the backend base URL, implement login/register, attach Bearer tokens to requests, and establish a current-user/session model. Replace hardcoded identity and wallet values with authenticated API data.

### P1 — Core member workflows

Implement wallet reads, transaction history, deposit, withdrawal, contribution, group list/detail, join, and create-group workflows. Add field-level validation and map backend errors to user-facing messages.

### P2 — Detail and operational views

Add group members, ledger/transaction detail, payout history where permitted, and responsive loading/empty/error states. Wire export only after a real transaction dataset exists.

### P3 — Admin and resilience

Add role-aware payout scheduling, verify admin claims, handle duplicate submissions and concurrency conflicts, and test every financial workflow against the running API.

## Status legend

- **COMPLETE**: confirmed functional frontend-to-backend integration; none found.
- **PARTIAL**: some UI exists, but the backend workflow is incomplete; most member-facing screens are in this category.
- **MISSING**: no frontend implementation of the backend operation.
- **INCORRECT**: an existing integration uses the wrong route, payload, response, or permission; no real integration was found to classify this way.
- **UNVERIFIED**: evidence is insufficient, primarily wallet-controller details, runtime configuration, and live behavior.
- **NOT DIRECTLY UI-FACING**: internal/background behavior such as ledger persistence and automated payout execution.

## Final assessment

The existing frontend should be treated as a presentation prototype. It does not yet cover the backend functionally, and no endpoint can be marked COMPLETE from source inspection. The API contract is sufficient to plan the integration, but live backend testing is required before claiming production readiness.

---

## Endpoint summary

See `docs/endpoint-coverage.md` for the endpoint-by-endpoint matrix and field mappings.

## Reviewed files

- Frontend: `app/page.tsx`.
- Backend: controllers for auth, groups, transactions, users, payouts; available wallet-controller read result; transaction, group, payout, and user DTOs; transaction, group, payout, auth, wallet, and user services; `Program.cs`; enums; README; API testing documentation; financial flow tests.

## Important limitation

The audit does not claim complete repository coverage because some backend files were unavailable or returned no inspectable source, including the wallet controller and the background payout worker implementation. Those gaps are explicitly marked rather than inferred.
  
