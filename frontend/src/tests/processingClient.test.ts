import { beforeEach, describe, expect, it, vi } from 'vitest';
import {
  cancelProcessingJob,
  createProcessingJob,
  streamProcessingJob
} from '../services/processingClient';

describe('processingClient', () => {
  beforeEach(() => {
    vi.restoreAllMocks();
  });

  it('creates a job and streams chunks', async () => {
    const encoder = new TextEncoder();
    const body = new ReadableStream<Uint8Array>({
      start(controller) {
        controller.enqueue(encoder.encode('ab'));
        controller.enqueue(encoder.encode('🙂'));
        controller.close();
      }
    });

    const fetchMock = vi
      .fn()
      .mockResolvedValueOnce(
        new Response(JSON.stringify({ jobId: 'job-123', totalCharacters: 3 }), {
          status: 201,
          headers: { 'Content-Type': 'application/json' }
        })
      )
      .mockResolvedValueOnce(new Response(body, { status: 200 }));

    vi.stubGlobal('fetch', fetchMock);

    const created = await createProcessingJob('hello');
    const received: string[] = [];

    await streamProcessingJob(
      created.jobId,
      (chunk) => received.push(chunk),
      new AbortController().signal
    );

    expect(created.totalCharacters).toBe(3);
    expect(received.join('')).toBe('ab🙂');
  });

  it('sends cancel command', async () => {
    const fetchMock = vi.fn().mockResolvedValue(new Response(null, { status: 204 }));
    vi.stubGlobal('fetch', fetchMock);

    await cancelProcessingJob('job-123');

    expect(fetchMock).toHaveBeenCalledWith('/api/processing/jobs/job-123', {
      method: 'DELETE',
      keepalive: true
    });
  });
});
