export interface ResultPanelProps {
  result: string;
}

/**
 * Read-only panel that shows streamed output in real time.
 */
export function ResultPanel({ result }: ResultPanelProps) {
  return (
    <div className="mt-4">
      <label htmlFor="resultText" className="form-label fw-semibold">
        Result (Live)
      </label>
      <textarea
        id="resultText"
        className="form-control font-monospace"
        rows={6}
        value={result}
        readOnly
      />
    </div>
  );
}
