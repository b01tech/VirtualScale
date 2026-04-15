export interface SerialStatusResponse {
  desiredConnected: boolean;
  desiredPort: string | null;
  isConnected: boolean;
  connectedPort: string | null;
  state: number;
  lastError: string | null;
}
