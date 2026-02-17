/**
 * Central state used by the processing page.
 */
export interface ProcessingState {
  input: string;
  result: string;
  isProcessing: boolean;
  isCancelling: boolean;
  currentJobId: string | null;
  errorMessage: string | null;
  progressPercent: number;
  totalCharacters: number | null;
  receivedCharacters: number;
}

/**
 * Events emitted by the UI and network layer.
 */
export type ProcessingAction =
  | { type: 'input_changed'; payload: string }
  | { type: 'start' }
  | { type: 'job_created'; payload: { jobId: string; totalCharacters: number } }
  | { type: 'cancel_requested' }
  | { type: 'chunk_received'; payload: string }
  | { type: 'completed' }
  | { type: 'cancelled' }
  | { type: 'failed'; payload: string };

export const initialState: ProcessingState = {
  input: '',
  result: '',
  isProcessing: false,
  isCancelling: false,
  currentJobId: null,
  errorMessage: null,
  progressPercent: 0,
  totalCharacters: null,
  receivedCharacters: 0
};

/**
 * Calculates UI progress based on received and expected characters.
 */
export const calculateProgress = (received: number, total: number | null): number => {
  if (!total || total <= 0) {
    return 0;
  }

  return Math.min(100, Math.round((received / total) * 100));
};

/**
 * Pure reducer that keeps business state transitions deterministic and testable.
 */
export const processingReducer = (
  state: ProcessingState,
  action: ProcessingAction
): ProcessingState => {
  switch (action.type) {
    case 'input_changed':
      return {
        ...state,
        input: action.payload
      };

    case 'start':
      return {
        ...state,
        result: '',
        isProcessing: true,
        isCancelling: false,
        currentJobId: null,
        errorMessage: null,
        progressPercent: 0,
        totalCharacters: null,
        receivedCharacters: 0
      };

    case 'job_created':
      return {
        ...state,
        currentJobId: action.payload.jobId,
        totalCharacters: action.payload.totalCharacters
      };

    case 'cancel_requested':
      return {
        ...state,
        isCancelling: true,
        progressPercent: 0
      };

    case 'chunk_received': {
      if (state.isCancelling) {
        return state;
      }

      const chunkCharacters = Array.from(action.payload).length;
      const receivedCharacters = state.receivedCharacters + chunkCharacters;

      return {
        ...state,
        result: state.result + action.payload,
        receivedCharacters,
        progressPercent: calculateProgress(receivedCharacters, state.totalCharacters)
      };
    }

    case 'completed':
      return {
        ...state,
        isProcessing: false,
        isCancelling: false,
        currentJobId: null,
        progressPercent: 100
      };

    case 'cancelled':
      return {
        ...state,
        isProcessing: false,
        isCancelling: false,
        currentJobId: null
      };

    case 'failed':
      return {
        ...state,
        isProcessing: false,
        isCancelling: false,
        currentJobId: null,
        errorMessage: action.payload
      };

    default:
      return state;
  }
};
