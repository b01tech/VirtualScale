import { Component, inject, input, OnDestroy, OnInit } from "@angular/core";

import { ScaleService } from "../../services/scale.service";
import { DecimalPipe } from "@angular/common";
import { TareIndicator } from "../tare-indicator/tare-indicator";
import { LoadCellGrid } from "../loadcell-grid/loadcell-grid";
import { SerialService } from "../../services/serial.service";
import { StableIndicator } from "../stable-indicator/stable-indicator";
import { ConnectionIndicator } from "../connection-indicator/connection-indicator";

@Component({
  selector: "app-display-scale",
  imports: [
    DecimalPipe,
    TareIndicator,
    LoadCellGrid,
    StableIndicator,
    ConnectionIndicator,
  ],
  templateUrl: "./display-scale.html",
  styleUrl: "./display-scale.scss",
})
export class DisplayScale implements OnInit, OnDestroy {
  private readonly _scaleService = inject(ScaleService);
  private readonly _serialService = inject(SerialService);

  showCells = input(true);

  protected readonly scale = this._scaleService.latest;
  protected readonly loadCells = this._scaleService.loadCells;
  protected readonly serialStatus = this._serialService.status;

  async ngOnInit() {
    await this._scaleService.startLive();
    await this._scaleService.startLoadCellsLive();
  }

  async ngOnDestroy() {
    await this._scaleService.stopLive();
    await this._scaleService.stopLoadCellsLive();
  }
}
