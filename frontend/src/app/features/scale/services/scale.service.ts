import { inject, Injectable, signal } from "@angular/core";
import { ScaleResponse } from "../models/scale-response";
import { LoadCellResponse } from "../models/loadcell-response";
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
  private readonly _loadCellHubUrl = environment.apiBaseUrl + "/hubs/loadcells";
  private readonly _loadCellApiUrl = environment.apiBaseUrl + "/api/loadcells";
  private readonly _httpClient = inject(HttpClient);
  private _connection: HubConnection | null = null;
  private _loadCellConnection: HubConnection | null = null;

  readonly latest = signal<ScaleResponse>({
    bruteWeight: 0,
    netWeight: 0,
    tareWeight: 0,
    isTared: false,
    isStable: false,
    filterLevel: 0,
    numberOfCells: 1,
    capMax: 0,
    division: 1,
    decimalPlaces: 0,
    referenceWeight: 1,
    needsCalibrationAdjustment: false,
  });

  readonly loadCells = signal<LoadCellResponse[]>([]);

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

  async startLoadCellsLive(): Promise<void> {
    if (this._loadCellConnection?.state === HubConnectionState.Connected) {
      return;
    }

    const connection =
      this._loadCellConnection ??
      new HubConnectionBuilder()
        .withUrl(this._loadCellHubUrl, {
          transport: HttpTransportType.WebSockets,
          withCredentials: false,
        })
        .withAutomaticReconnect()
        .build();

    this._loadCellConnection = connection;
    await connection.start();

    const stream = connection.stream<LoadCellResponse>("Stream", null);
    stream.subscribe({
      next: (value) => this.upsertLoadCell(value),
      error: () => {},
      complete: () => {},
    });
  }

  async stopLoadCellsLive(): Promise<void> {
    if (!this._loadCellConnection) {
      return;
    }

    if (this._loadCellConnection.state !== HubConnectionState.Disconnected) {
      await this._loadCellConnection.stop();
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

  setFilterLevel(level: number) {
    return this._httpClient.post<void>(this._apiUrl + "/filter", { level });
  }

  setCalibrationSettings(payload: {
    numberOfCells: number;
    capMax: number;
    division: number;
    decimalPlaces: number;
    referenceWeight: number;
  }) {
    return this._httpClient.post<void>(this._apiUrl + "/settings", payload);
  }

  setLoadCellFactor(id: number, factor: number) {
    return this._httpClient.post<void>(this._loadCellApiUrl + "/factor", {
      id,
      factor,
    });
  }

  resetLoadCellFactors() {
    return this._httpClient.post<void>(
      this._loadCellApiUrl + "/factors/reset",
      {},
    );
  }

  private upsertLoadCell(value: LoadCellResponse) {
    this.loadCells.update((cells: LoadCellResponse[]) => {
      const next = [...cells];
      const index = next.findIndex((cell) => cell.id === value.id);
      if (index >= 0) {
        next[index] = value;
      } else {
        next.push(value);
      }
      next.sort((a, b) => a.id - b.id);
      return next;
    });
  }
}
