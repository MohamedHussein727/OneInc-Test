import { FormEvent } from 'react';

export interface ProcessingFormProps {
  input: string;
  isProcessing: boolean;
  isCancelling: boolean;
  onInputChange: (value: string) => void;
  onProcess: (event: FormEvent<HTMLFormElement>) => void;
  onCancel: () => void;
}

/**
 * Input form and action buttons for process/cancel commands.
 */
export function ProcessingForm({
  input,
  isProcessing,
  isCancelling,
  onInputChange,
  onProcess,
  onCancel
}: ProcessingFormProps) {
  return (
    <form onSubmit={onProcess} className="d-grid gap-3 mt-4">
      <div>
        <label htmlFor="inputText" className="form-label fw-semibold">
          Input Text
        </label>
        <textarea
          id="inputText"
          className="form-control"
          rows={5}
          value={input}
          onChange={(event) => onInputChange(event.target.value)}
          placeholder="Enter text to process..."
          disabled={isProcessing}
        />
      </div>

      <div className="button-row">
        <button
          type="submit"
          className="btn btn-primary btn-process"
          disabled={isProcessing || input.trim().length === 0}
        >
          Process
        </button>
        <button
          type="button"
          className="btn btn-outline-danger"
          disabled={!isProcessing || isCancelling}
          onClick={onCancel}
        >
          {isCancelling ? 'Cancelling...' : 'Cancel'}
        </button>
      </div>
    </form>
  );
}
