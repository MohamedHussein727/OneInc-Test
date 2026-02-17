# OneInc Hard Processing Simulator

SPA solution that uses **.NET 10** as backend and **React** as frontend to simulate a heavy, long-running processing service.

## What it does

- Accepts user input text.
- Backend creates this output format:
  - Sorted unique characters with occurrence counts.
  - A slash `/`.
  - Base64 of the original input.
- Streams output back to the client one character at a time.
- Waits randomly **1-5 seconds** before each character.
- UI appends incoming characters in real time.
- Prevents starting a second process while one is active.
- Supports cancellation using a `Cancel` button.
- Displays a custom progress bar during processing.

Example for `Hello, World!`:

` 1!1,1H1W1d1e1l3o2r1/SGVsbG8sIFdvcmxkIQ==`

## Architecture

### Backend (`/backend/src/OneInc.Processing.Api`)

- `ICharacterAggregationService` + `CharacterAggregationService`
  - Builds sorted unique character statistics (Unicode-aware).
- `IProcessingOutputComposer` + `ProcessingOutputComposer`
  - Composes the final output payload.
- `ICharacterDelayStrategy` + `RandomCharacterDelayStrategy`
  - Supplies random delays (1-5s).
- `IProcessingStreamService` + `ProcessingStreamService`
  - Streams response one Unicode character at a time with per-char delay.
- `GlobalExceptionHandler`
  - Converts unhandled errors into safe API responses.
- `ProcessingOptions`
  - Protects service limits (default max input length: 2000).
- `ProcessingController`
  - Controller-based API entry point for future extensibility.
- `IProcessingJobCoordinator` + `ProcessingJobCoordinator`
  - Centralized job lifecycle management (`create`, `stream`, `cancel`).
- Rate limiting policy (`processing`)
  - Limits abusive request rates per client IP.

Endpoint:

- `POST /api/processing/jobs`
  - Request body: `{ "input": "..." }`
  - Response: `{ "jobId": "...", "totalCharacters": 123 }`
- `GET /api/processing/jobs/{jobId}/stream`
  - Response: `text/plain` streamed payload.
- `DELETE /api/processing/jobs/{jobId}`
  - Explicitly cancels server-side work for that job.

### Frontend (`/frontend`)

- React + TypeScript + Vite.
- `processingReducer` keeps business state deterministic/testable.
- `createProcessingJob` / `streamProcessingJob` / `cancelProcessingJob`
  - Models full backend job lifecycle.
- UI disables `Process` during active run and enables `Cancel`.
- Custom progress bar updated from streamed character count.
- Lightweight animations improve feedback without changing interaction complexity.

## Unit tests

### Backend tests (`/backend/tests/OneInc.Processing.Tests`)

- `CharacterAggregationServiceTests`
- `ProcessingOutputComposerTests`
- `ProcessingStreamServiceTests`
- `ProcessingJobCoordinatorTests`

### Frontend tests (`/frontend/src/tests`)

- `processingReducer.test.ts`
- `processingClient.test.ts`

## Run locally

### 1) Backend

```bash
cd /Users/mohamedmahmoud/Desktop/Work/openInc

dotnet run --project /Users/mohamedmahmoud/Desktop/Work/openInc/backend/src/OneInc.Processing.Api/OneInc.Processing.Api.csproj
```

Backend URL: `http://localhost:5107`

### 2) Frontend

```bash
cd /Users/mohamedmahmoud/Desktop/Work/openInc/frontend
npm install
npm run dev
```

Frontend URL: `http://localhost:5173`

Vite proxy forwards `/api` to `http://localhost:5107`.

## Notes on resiliency and scalability

- Cancel sends an explicit server command so the backend actually stops the active job.
- Streaming also links request-abort cancellation as a secondary safety net.
- Per-IP rate limiting protects the endpoint from misuse.
- Stateless service design supports horizontal scaling.
- Input constraints prevent oversized payload abuse.
- Structured logging and centralized exception handling simplify operations.
