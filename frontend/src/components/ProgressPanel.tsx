export interface ProgressPanelProps {
  progressPercent: number;
  isCancelling: boolean;
}

/**
 * Visual progress indicator for active processing/cancellation states.
 */
export function ProgressPanel({ progressPercent, isCancelling }: ProgressPanelProps) {
  const progressClassName = isCancelling
    ? 'custom-progress-fill is-cancelling'
    : 'custom-progress-fill is-running';

  return (
    <div className="mt-4">
      <div className="d-flex justify-content-between align-items-center mb-2">
        <span className="fw-semibold">Processing Progress</span>
        <span className="text-secondary">{progressPercent}%</span>
      </div>
      <div className="custom-progress-shell" aria-label="processing progress">
        <div className={progressClassName} style={{ width: `${progressPercent}%` }} />
      </div>
    </div>
  );
}
