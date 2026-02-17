export interface ErrorAlertProps {
  message: string | null;
}

/**
 * Displays any request or stream error to the user.
 */
export function ErrorAlert({ message }: ErrorAlertProps) {
  if (!message) {
    return null;
  }

  return (
    <div className="alert alert-danger mt-4 mb-0" role="alert">
      {message}
    </div>
  );
}
