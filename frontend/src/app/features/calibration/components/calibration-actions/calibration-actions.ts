import { Component, inject, signal } from "@angular/core";
import { firstValueFrom } from "rxjs";
import { AppButton } from "../../../../shared/button/app-button";
import { ScaleService } from "../../../scale/services/scale.service";
import { EqualizeLoadCells } from "../../../scale/components/equalize-loadcells/equalize-loadcells";

@Component({
  selector: "app-calibration-actions",
  imports: [AppButton, EqualizeLoadCells],
  templateUrl: "./calibration-actions.html",
  styleUrl: "./calibration-actions.scss",
})
export class CalibrationActions {
  private readonly _scaleService = inject(ScaleService);
  protected readonly isBusy = signal(false);

  protected async calibrateZero() {
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

  protected async calibrateSpan() {
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
