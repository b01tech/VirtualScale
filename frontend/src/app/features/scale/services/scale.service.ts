import { inject, Injectable } from "@angular/core";
import { ScaleResponse } from "../models/scale-response";
import { environment } from "../../../../environment/environment";
import { HttpClient } from "@angular/common/http";

@Injectable({ providedIn: "root" })
export class ScaleService {
  private readonly _apiUrl = environment.apiBaseUrl + "/api/scale";
  private readonly _httpClient = inject(HttpClient);

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
