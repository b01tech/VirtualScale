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
  protected readonly baseline = signal<Record<number, number>>({});
  protected readonly referenceDeltaScaled = signal<number | null>(null);
  protected readonly applied = signal<Record<number, number>>({});

  protected readonly serialStatus = this._serialService.status;
  protected readonly scale = this._scaleService.latest;
  protected readonly loadCells = this._scaleService.loadCells;

  protected readonly isConnected = computed(
    () => this.serialStatus().state === 2,
  );
  protected readonly isStable = computed(() => this.scale().isStable);
  protected readonly visibleCells = computed(() => {
    const max = Math.max(0, this.scale().numberOfCells ?? 0);
    return [...this.loadCells()].sort((a, b) => a.id - b.id).slice(0, max);
  });
  protected readonly hasCells = computed(() => this.visibleCells().length > 0);

  protected readonly orderedIds = computed(() =>
    this.visibleCells().map((c) => c.id),
  );

  protected readonly totalCells = computed(() => this.orderedIds().length);
  protected readonly totalSteps = computed(() => this.totalCells() + 1);

  protected readonly currentId = computed(() =>
    this.stepIndex() <= 0
      ? null
      : (this.orderedIds()[this.stepIndex() - 1] ?? null),
  );

  protected readonly currentCell = computed<LoadCellResponse | null>(() => {
    const id = this.currentId();
    if (id === null) {
      return null;
    }
    return this.visibleCells().find((c) => c.id === id) ?? null;
  });

  protected readonly isDone = computed(
    () => this.isOpen() && this.stepIndex() > this.totalCells(),
  );

  protected start() {
    this.error.set(null);
    this.isResetOpen.set(false);
    this.applied.set({});
    this.baseline.set({});
    this.referenceDeltaScaled.set(null);
    this.stepIndex.set(0);

    if (!this.isConnected()) {
      this.error.set("Serial desconectada");
      return;
    }

    if (this.totalCells() < 2) {
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

    if (!this.isStable()) {
      this.error.set("Aguardando estabilidade");
      return;
    }

    this.error.set(null);

    const currentStep = this.stepIndex();

    if (currentStep === 0) {
      const snapshot = this.visibleCells();
      const baseline: Record<number, number> = {};
      for (const cell of snapshot) {
        baseline[cell.id] = cell.rawValue;
      }
      this.baseline.set(baseline);
      this.stepIndex.set(1);
      return;
    }

    const cell = this.currentCell();
    if (!cell) {
      this.error.set("Célula não encontrada");
      return;
    }

    const baseline = this.baseline();
    const baselineRaw = baseline[cell.id];
    if (baselineRaw === undefined) {
      this.error.set("Baseline não definido");
      return;
    }

    const deltaRaw = cell.rawValue - baselineRaw;
    if (deltaRaw == 0) {
      this.error.set("Delta = 0, impossível calcular fator");
      return;
    }

    if (currentStep === 1) {
      this.referenceDeltaScaled.set(deltaRaw * cell.factor);
      this.stepIndex.set(2);
      return;
    }

    const referenceDeltaScaled = this.referenceDeltaScaled();
    if (referenceDeltaScaled === null) {
      this.error.set("Referência não definida");
      return;
    }

    const newFactor = referenceDeltaScaled / deltaRaw;

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
