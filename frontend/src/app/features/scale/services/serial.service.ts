import { inject, Injectable, signal } from "@angular/core";
import { environment } from "../../../../environment/environment";
import { HttpClient } from "@angular/common/http";
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
  HttpTransportType,
} from "@microsoft/signalr";
import { SerialStatusResponse } from "../models/serial-status";

@Injectable({ providedIn: "root" })
export class SerialService {
  private readonly _apiUrl = environment.apiBaseUrl + "/api/serial";
  private readonly _hubUrl = environment.apiBaseUrl + "/hubs/serial";
  private readonly _httpClient = inject(HttpClient);
  private _connection: HubConnection | null = null;

  readonly ports = signal<string[]>([]);
  readonly status = signal<SerialStatusResponse>({
    desiredConnected: false,
    desiredPort: null,
    isConnected: false,
    connectedPort: null,
    state: 0,
    lastError: null,
  });

  loadPorts() {
    return this._httpClient.get<string[]>(this._apiUrl + "/ports");
  }

  loadStatus() {
    return this._httpClient.get<SerialStatusResponse>(this._apiUrl + "/status");
  }

  connect(port: string) {
    return this._httpClient.post<SerialStatusResponse>(
      this._apiUrl + "/connect",
      {
        port,
      },
    );
  }

  disconnect() {
    return this._httpClient.post<SerialStatusResponse>(
      this._apiUrl + "/disconnect",
      {},
    );
  }

  async startLive(): Promise<void> {
    if (this._connection?.state === HubConnectionState.Connected) {
      return;
    }

    const connection =
      this._connection ??
      new HubConnectionBuilder()
        .withUrl(this._hubUrl, {
          transport: HttpTransportType.WebSockets,
          withCredentials: false,
        })
        .withAutomaticReconnect()
        .build();

    this._connection = connection;
    await connection.start();

    const stream = connection.stream<SerialStatusResponse>("Stream");
    stream.subscribe({
      next: (value) => this.status.set(value),
      error: () => {},
      complete: () => {},
    });
  }

  async stopLive(): Promise<void> {
    if (!this._connection) {
      return;
    }
    if (this._connection.state !== HubConnectionState.Disconnected) {
      await this._connection.stop();
    }
  }
}
