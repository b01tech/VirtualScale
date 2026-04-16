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
  protected readonly isEqualizing = signal(false);
  protected readonly saveStatus = signal<{ success: boolean; message: string } | null>(null);

  protected async calibrateZero() {
    if (this.isBusy() || this.isEqualizing()) {
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
    if (this.isBusy() || this.isEqualizing()) {
      return;
    }
    this.isBusy.set(true);
    try {
      await firstValueFrom(this._scaleService.calibrateSpan());
    } finally {
      this.isBusy.set(false);
    }
  }

  protected async saveCalibration() {
    if (this.isBusy() || this.isEqualizing()) {
      return;
    }
    this.isBusy.set(true);
    this.saveStatus.set(null);
    try {
      await firstValueFrom(this._scaleService.saveCalibration());
      this.saveStatus.set({ success: true, message: "Calibração salva com sucesso!" });
    } catch {
      this.saveStatus.set({ success: false, message: "Falha ao salvar calibração" });
    } finally {
      this.isBusy.set(false);
      setTimeout(() => this.saveStatus.set(null), 3000);
    }
  }

  protected setEqualizing(active: boolean) {
    this.isEqualizing.set(active);
  }
}
