export interface ProcessingHeaderProps {
  isProcessing: boolean;
  isCancelling: boolean;
}

/**
 * Top section that explains the page and shows current processing status.
 */
export function ProcessingHeader({ isProcessing, isCancelling }: ProcessingHeaderProps) {
  return (
    <div className="d-flex justify-content-between align-items-start flex-wrap gap-3">
      <div>
        <h1 className="h3 mb-2">OneInc Hard Processing Simulator</h1>
        <p className="text-secondary mb-0">
          Calls a backend job streamming one character at a time, super cool, nice animations! Subscribe for more!
        </p>
      </div>
      <span className={`status-pill ${isProcessing ? 'status-running' : 'status-idle'}`}>
        {isProcessing ? (isCancelling ? 'Cancelling...' : 'Running') : 'Idle'}
      </span>
    </div>
  );
}
