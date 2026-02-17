import { FormEvent, useReducer, useRef } from 'react';
import { ErrorAlert } from './components/ErrorAlert';
import { ProcessingForm } from './components/ProcessingForm';
import { ProcessingHeader } from './components/ProcessingHeader';
import { ProgressPanel } from './components/ProgressPanel';
import { ResultPanel } from './components/ResultPanel';
import {
  cancelProcessingJob,
  createProcessingJob,
  streamProcessingJob
} from './services/processingClient';
import {
  initialState,
  processingReducer
} from './state/processingReducer';

/**
 * Main SPA page that coordinates processing commands and real-time UI updates.
 */
export default function App() {
  const [state, dispatch] = useReducer(processingReducer, initialState);
  const controllerRef = useRef<AbortController | null>(null);
  const cancelRequestedRef = useRef(false);

  const handleProcess = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (state.isProcessing) {
      return;
    }

    cancelRequestedRef.current = false;
    dispatch({ type: 'start' });

    try {
      const job = await createProcessingJob(state.input);
      dispatch({
        type: 'job_created',
        payload: { jobId: job.jobId, totalCharacters: job.totalCharacters }
      });

      const controller = new AbortController();
      controllerRef.current = controller;

      await streamProcessingJob(
        job.jobId,
        (chunk) => dispatch({ type: 'chunk_received', payload: chunk }),
        controller.signal
      );

      if (controller.signal.aborted || cancelRequestedRef.current) {
        dispatch({ type: 'cancelled' });
      } else {
        dispatch({ type: 'completed' });
      }
    } catch (error) {
      if (controllerRef.current?.signal.aborted || cancelRequestedRef.current) {
        dispatch({ type: 'cancelled' });
      } else {
        const message =
          error instanceof Error
            ? error.message
            : 'An unknown error occurred while processing your request.';

        dispatch({ type: 'failed', payload: message });
      }
    } finally {
      controllerRef.current = null;
      cancelRequestedRef.current = false;
    }
  };

  const handleCancel = async () => {
    if (!state.currentJobId || !state.isProcessing) {
      return;
    }

    cancelRequestedRef.current = true;
    dispatch({ type: 'cancel_requested' });

    try {
      await cancelProcessingJob(state.currentJobId);
    } catch (error) {
      cancelRequestedRef.current = false;
      const message =
        error instanceof Error ? error.message : 'Cancel request failed unexpectedly.';
      dispatch({ type: 'failed', payload: message });
      return;
    }

    controllerRef.current?.abort();
  };

  return (
    <main className="container py-5 position-relative">
      <div className="ambient-blob ambient-blob-a" />
      <div className="ambient-blob ambient-blob-b" />

      <section className="card shadow-lg processing-card mx-auto app-entrance">
        <div className="card-body p-4 p-md-5">
          <ProcessingHeader
            isProcessing={state.isProcessing}
            isCancelling={state.isCancelling}
          />

          <ProcessingForm
            input={state.input}
            isProcessing={state.isProcessing}
            isCancelling={state.isCancelling}
            onInputChange={(value) => dispatch({ type: 'input_changed', payload: value })}
            onProcess={handleProcess}
            onCancel={() => {
              void handleCancel();
            }}
          />

          <ResultPanel result={state.result} />

          <ProgressPanel
            progressPercent={state.progressPercent}
            isCancelling={state.isCancelling}
          />

          <ErrorAlert message={state.errorMessage} />
        </div>
      </section>
    </main>
  );
}
