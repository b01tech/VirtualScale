import { inject, Injectable, signal } from "@angular/core";
import { ScaleResponse } from "../models/scale-response";
import { environment } from "../../../../environment/environment";
import { HttpClient } from "@angular/common/http";
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
  HttpTransportType,
} from "@microsoft/signalr";

@Injectable({ providedIn: "root" })
export class ScaleService {
  private readonly _apiUrl = environment.apiBaseUrl + "/api/scale";
  private readonly _hubUrl = environment.apiBaseUrl + "/hubs/scale";
  private readonly _httpClient = inject(HttpClient);
  private _connection: HubConnection | null = null;

  readonly latest = signal<ScaleResponse>({
    bruteWeight: 0,
    netWeight: 0,
    tareWeight: 0,
    isTared: false,
  });

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

    const stream = connection.stream<ScaleResponse>("Stream");
    stream.subscribe({
      next: (value) => this.latest.set(value),
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

  getSnapshot() {
    return this._httpClient.get<ScaleResponse>(this._apiUrl + "/");
  }

  tareScale() {
    return this._httpClient.post<void>(this._apiUrl + "/tare", {});
  }
  calibrateZero() {
    return this._httpClient.post<void>(this._apiUrl + "/calibrate/zero", {});
  }
  calibrateSpan() {
    return this._httpClient.post<void>(this._apiUrl + "/calibrate/span", {});
  }
}
