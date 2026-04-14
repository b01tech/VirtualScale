import { Component, inject, OnDestroy, OnInit } from "@angular/core";

import { ScaleService } from "../../services/scale.service";
import { DecimalPipe } from "@angular/common";
import { TareIndicator } from "../tare-indicator/tare-indicator";

@Component({
  selector: "app-display-scale",
  imports: [DecimalPipe, TareIndicator],
  templateUrl: "./display-scale.html",
  styleUrl: "./display-scale.scss",
})
export class DisplayScale implements OnInit, OnDestroy {
  private readonly _scaleService = inject(ScaleService);

  protected readonly scale = this._scaleService.latest;

  async ngOnInit() {
    await this._scaleService.startLive();
  }

  async ngOnDestroy() {
    await this._scaleService.stopLive();
  }
}
