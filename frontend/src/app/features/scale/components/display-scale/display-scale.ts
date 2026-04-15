import { Component, inject, OnDestroy, OnInit } from "@angular/core";

import { ScaleService } from "../../services/scale.service";
import { DecimalPipe } from "@angular/common";
import { TareIndicator } from "../tare-indicator/tare-indicator";
import { LoadCellGrid } from "../loadcell-grid/loadcell-grid";

@Component({
  selector: "app-display-scale",
  imports: [DecimalPipe, TareIndicator, LoadCellGrid],
  templateUrl: "./display-scale.html",
  styleUrl: "./display-scale.scss",
})
export class DisplayScale implements OnInit, OnDestroy {
  private readonly _scaleService = inject(ScaleService);

  protected readonly scale = this._scaleService.latest;
  protected readonly loadCells = this._scaleService.loadCells;

  async ngOnInit() {
    await this._scaleService.startLive();
    await this._scaleService.startLoadCellsLive();
  }

  async ngOnDestroy() {
    await this._scaleService.stopLive();
    await this._scaleService.stopLoadCellsLive();
  }
}
