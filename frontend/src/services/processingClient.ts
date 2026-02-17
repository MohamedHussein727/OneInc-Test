/**
 * Payload returned when backend accepts a new processing job.
 */
export interface CreateJobResult {
  jobId: string;
  totalCharacters: number;
}

const extractErrorMessage = async (response: Response): Promise<string> => {
  try {
    const json = (await response.json()) as { message?: string; title?: string; detail?: string };
    return json.message ?? json.title ?? json.detail ?? `Request failed with status ${response.status}`;
  } catch {
    const body = await response.text();
    return body || `Request failed with status ${response.status}`;
  }
};

/**
 * Creates a server-side processing job and returns metadata required by the client.
 */
export const createProcessingJob = async (input: string): Promise<CreateJobResult> => {
  const response = await fetch('/api/processing/jobs', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({ input })
  });

  if (!response.ok) {
    throw new Error(await extractErrorMessage(response));
  }

  return (await response.json()) as CreateJobResult;
};

/**
 * Streams output from a previously created server-side job.
 */
export const streamProcessingJob = async (
  jobId: string,
  onChunk: (chunk: string) => void,
  signal: AbortSignal
): Promise<void> => {
  const response = await fetch(`/api/processing/jobs/${jobId}/stream`, {
    method: 'GET',
    signal
  });

  if (!response.ok) {
    throw new Error(await extractErrorMessage(response));
  }

  if (!response.body) {
    throw new Error('The server response did not provide a stream body.');
  }

  const reader = response.body.getReader();
  const decoder = new TextDecoder();

  while (true) {
    const { done, value } = await reader.read();
    if (done) {
      break;
    }

    const chunk = decoder.decode(value, { stream: true });
    if (chunk.length > 0) {
      onChunk(chunk);
    }
  }

  const tail = decoder.decode();
  if (tail.length > 0) {
    onChunk(tail);
  }
};

/**
 * Sends a cancel command for an existing job.
 */
export const cancelProcessingJob = async (jobId: string): Promise<void> => {
  const response = await fetch(`/api/processing/jobs/${jobId}`, {
    method: 'DELETE',
    keepalive: true
  });

  if (response.status === 404) {
    return;
  }

  if (!response.ok) {
    throw new Error(await extractErrorMessage(response));
  }
};
