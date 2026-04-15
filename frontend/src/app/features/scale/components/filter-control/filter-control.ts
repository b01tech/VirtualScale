import { Component, computed, inject, signal } from "@angular/core";
import { firstValueFrom } from "rxjs";
import { AppButton } from "../../../../shared/button/app-button";
import { ScaleService } from "../../services/scale.service";
import { SerialService } from "../../services/serial.service";

@Component({
  selector: "app-filter-control",
  imports: [AppButton],
  templateUrl: "./filter-control.html",
  styleUrl: "./filter-control.scss",
})
export class FilterControl {
  private readonly _scaleService = inject(ScaleService);
  private readonly _serialService = inject(SerialService);

  protected readonly scale = this._scaleService.latest;
  protected readonly serialStatus = this._serialService.status;
  protected readonly isBusy = signal(false);

  protected readonly isConnected = computed(
    () => this.serialStatus().state === 2,
  );
  protected readonly selected = signal<number>(0);

  constructor() {
    this.selected.set(this.scale().filterLevel ?? 0);
  }

  protected onLevelChange(event: Event) {
    const target = event.target as HTMLSelectElement | null;
    const parsed = Number(target?.value ?? 0);
    this.selected.set(Number.isFinite(parsed) ? parsed : 0);
  }

  protected async apply() {
    if (!this.isConnected() || this.isBusy()) {
      return;
    }

    this.isBusy.set(true);
    try {
      await firstValueFrom(this._scaleService.setFilterLevel(this.selected()));
    } finally {
      this.isBusy.set(false);
    }
  }
}
