import { DecimalPipe } from "@angular/common";
import { Component, computed, inject, signal } from "@angular/core";
import { firstValueFrom } from "rxjs";
import { AppButton } from "../../../../shared/button/app-button";
import { LoadCellResponse } from "../../models/loadcell-response";
import { ScaleService } from "../../services/scale.service";
import { SerialService } from "../../services/serial.service";

@Component({
  selector: "app-equalize-loadcells",
  imports: [AppButton, DecimalPipe],
  templateUrl: "./equalize-loadcells.html",
  styleUrl: "./equalize-loadcells.scss",
})
export class EqualizeLoadCells {
  private readonly _scaleService = inject(ScaleService);
  private readonly _serialService = inject(SerialService);

  protected readonly isOpen = signal(false);
  protected readonly isResetOpen = signal(false);
  protected readonly stepIndex = signal(0);
  protected readonly isBusy = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly referenceScaled = signal<number | null>(null);
  protected readonly applied = signal<Record<number, number>>({});

  protected readonly serialStatus = this._serialService.status;
  protected readonly loadCells = this._scaleService.loadCells;

  protected readonly isConnected = computed(
    () => this.serialStatus().state === 2,
  );
  protected readonly hasCells = computed(() => this.loadCells().length > 0);

  protected readonly orderedIds = computed(() =>
    [...this.loadCells()].map((c) => c.id).sort((a, b) => a - b),
  );

  protected readonly totalSteps = computed(() => this.orderedIds().length);

  protected readonly currentId = computed(
    () => this.orderedIds()[this.stepIndex()] ?? null,
  );

  protected readonly currentCell = computed<LoadCellResponse | null>(() => {
    const id = this.currentId();
    if (id === null) {
      return null;
    }
    return this.loadCells().find((c) => c.id === id) ?? null;
  });

  protected readonly isDone = computed(
    () => this.isOpen() && this.stepIndex() >= this.totalSteps(),
  );

  protected start() {
    this.error.set(null);
    this.isResetOpen.set(false);
    this.applied.set({});
    this.referenceScaled.set(null);
    this.stepIndex.set(0);

    if (!this.isConnected()) {
      this.error.set("Serial desconectada");
      return;
    }

    if (this.totalSteps() < 2) {
      this.error.set("Necessário pelo menos 2 células para equalizar");
      return;
    }

    this.isOpen.set(true);
  }

  protected close() {
    this.isOpen.set(false);
    this.isResetOpen.set(false);
    this.isBusy.set(false);
  }

  protected openReset() {
    this.error.set(null);
    if (!this.isConnected()) {
      this.error.set("Serial desconectada");
      return;
    }
    if (!this.hasCells()) {
      this.error.set("Sem células carregadas");
      return;
    }
    this.isOpen.set(false);
    this.isResetOpen.set(true);
  }

  protected async confirmReset() {
    if (this.isBusy()) {
      return;
    }

    if (!this.isConnected()) {
      this.error.set("Serial desconectada");
      return;
    }

    this.error.set(null);
    this.isBusy.set(true);
    try {
      await firstValueFrom(this._scaleService.resetLoadCellFactors());
      this.isResetOpen.set(false);
    } finally {
      this.isBusy.set(false);
    }
  }

  protected async confirm() {
    if (this.isBusy()) {
      return;
    }

    const cell = this.currentCell();
    if (!cell) {
      this.error.set("Célula não encontrada");
      return;
    }

    this.error.set(null);

    const currentStep = this.stepIndex();
    const raw = cell.rawValue;

    if (currentStep === 0) {
      this.referenceScaled.set(raw * cell.factor);
      this.stepIndex.set(currentStep + 1);
      return;
    }

    const referenceScaled = this.referenceScaled();
    if (referenceScaled === null) {
      this.error.set("Referência não definida");
      return;
    }

    if (raw === 0) {
      this.error.set("RawValue = 0, impossível calcular fator");
      return;
    }

    const newFactor = referenceScaled / raw;

    this.isBusy.set(true);
    try {
      await firstValueFrom(
        this._scaleService.setLoadCellFactor(cell.id, newFactor),
      );
      this.applied.update((prev) => ({ ...prev, [cell.id]: newFactor }));
      this.stepIndex.set(currentStep + 1);
    } finally {
      this.isBusy.set(false);
    }
  }
}
