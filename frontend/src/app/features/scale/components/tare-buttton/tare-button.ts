import { Component, inject, signal } from "@angular/core";
import { firstValueFrom } from "rxjs";
import { ScaleService } from "../../services/scale.service";
import { AppButton } from "../../../../shared/button/app-button";
import { EqualizeLoadCells } from "../equalize-loadcells/equalize-loadcells";

@Component({
  selector: "app-tare-button",
  imports: [AppButton, EqualizeLoadCells],
  templateUrl: "./tare-button.html",
  styleUrl: "./tare-button.scss",
})
export class TareButton {
  private readonly _scaleService = inject(ScaleService);
  protected readonly isBusy = signal(false);

  async onTare() {
    if (this.isBusy()) {
      return;
    }
    this.isBusy.set(true);
    try {
      await firstValueFrom(this._scaleService.tareScale());
    } finally {
      this.isBusy.set(false);
    }
  }

  async onCalibrateZero() {
    if (this.isBusy()) {
      return;
    }
    this.isBusy.set(true);
    try {
      await firstValueFrom(this._scaleService.calibrateZero());
    } finally {
      this.isBusy.set(false);
    }
  }

  async onCalibrateSpan() {
    if (this.isBusy()) {
      return;
    }
    this.isBusy.set(true);
    try {
      await firstValueFrom(this._scaleService.calibrateSpan());
    } finally {
      this.isBusy.set(false);
    }
  }
}
