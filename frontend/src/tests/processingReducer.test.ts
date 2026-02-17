import { describe, expect, it } from 'vitest';
import {
  calculateProgress,
  initialState,
  processingReducer
} from '../state/processingReducer';

describe('processingReducer', () => {
  it('calculates progress defensively', () => {
    expect(calculateProgress(0, null)).toBe(0);
    expect(calculateProgress(5, 10)).toBe(50);
    expect(calculateProgress(15, 10)).toBe(100);
  });

  it('stores job metadata and updates stream progress', () => {
    let state = processingReducer(initialState, { type: 'start' });
    state = processingReducer(state, {
      type: 'job_created',
      payload: { jobId: 'job-1', totalCharacters: 4 }
    });
    state = processingReducer(state, { type: 'chunk_received', payload: 'ab' });
    state = processingReducer(state, { type: 'chunk_received', payload: 'cd' });

    expect(state.currentJobId).toBe('job-1');
    expect(state.result).toBe('abcd');
    expect(state.receivedCharacters).toBe(4);
    expect(state.progressPercent).toBe(100);
  });

  it('sets cancellation state', () => {
    let state = processingReducer(initialState, { type: 'start' });
    state = processingReducer(state, {
      type: 'job_created',
      payload: { jobId: 'job-1', totalCharacters: 10 }
    });
    state = processingReducer(state, { type: 'chunk_received', payload: 'abc' });
    expect(state.progressPercent).toBe(30);

    state = processingReducer(state, { type: 'cancel_requested' });

    expect(state.isCancelling).toBe(true);
    expect(state.progressPercent).toBe(0);

    const unchanged = processingReducer(state, { type: 'chunk_received', payload: 'z' });
    expect(unchanged.result).toBe(state.result);
    expect(unchanged.receivedCharacters).toBe(state.receivedCharacters);

    state = processingReducer(state, { type: 'cancelled' });

    expect(state.isProcessing).toBe(false);
    expect(state.isCancelling).toBe(false);
    expect(state.currentJobId).toBeNull();
  });
});
