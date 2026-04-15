import { Component, computed, effect, inject, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { firstValueFrom } from "rxjs";
import { AppButton } from "../../../../shared/button/app-button";
import { ScaleService } from "../../../scale/services/scale.service";

@Component({
  selector: "app-calibration-settings",
  imports: [ReactiveFormsModule, AppButton],
  templateUrl: "./calibration-settings.html",
  styleUrl: "./calibration-settings.scss",
})
export class CalibrationSettings {
  private readonly _scaleService = inject(ScaleService);
  private readonly _fb = inject(FormBuilder);

  protected readonly scale = this._scaleService.latest;
  protected readonly isOpen = signal(false);
  protected readonly isBusy = signal(false);

  protected readonly form = this._fb.group({
    numberOfCells: [
      2,
      [Validators.required, Validators.min(1), Validators.max(4)],
    ],
    unit: ["kg", [Validators.required]],
    capMax: [10, [Validators.required, Validators.min(0.000001)]],
    division: [2, [Validators.required]],
    decimalPlaces: [3, [Validators.required]],
    referenceWeight: [10, [Validators.required, Validators.min(0.000001)]],
  });

  protected readonly allowedCells = [1, 2, 3, 4] as const;
  protected readonly allowedDivisions = [1, 2, 5, 10, 20] as const;
  protected readonly allowedDecimalPlaces = [0, 1, 2, 3, 4] as const;

  protected readonly divisionValue = computed(() =>
    Number(this.form.controls.division.value ?? 1),
  );
  protected readonly decimalLocked = computed(
    () => this.divisionValue() === 10 || this.divisionValue() === 20,
  );
  protected readonly maxReferenceWeight = computed(() =>
    Number(this.form.controls.capMax.value ?? 0),
  );

  constructor() {
    effect(() => {
      if (this.decimalLocked()) {
        this.form.controls.decimalPlaces.setValue(0, { emitEvent: false });
      }
    });

    effect(() => {
      const capMax = this.maxReferenceWeight();
      const referenceControl = this.form.controls.referenceWeight;
      referenceControl.setValidators([
        Validators.required,
        Validators.min(0.000001),
        Validators.max(capMax),
      ]);
      referenceControl.updateValueAndValidity({ emitEvent: false });
    });
  }

  protected toggle() {
    if (this.isOpen()) {
      this.isOpen.set(false);
      return;
    }

    const current = this.scale();
    this.form.patchValue({
      numberOfCells: current.numberOfCells,
      unit: current.unit,
      capMax: current.capMax,
      division: current.division,
      decimalPlaces: current.decimalPlaces,
      referenceWeight: current.referenceWeight,
    });
    this.isOpen.set(true);
  }

  protected async save() {
    if (this.isBusy() || this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    this.isBusy.set(true);
    try {
      await firstValueFrom(
        this._scaleService.setCalibrationSettings({
          numberOfCells: Number(value.numberOfCells ?? 1),
          unit: String(value.unit ?? "kg"),
          capMax: Number(value.capMax ?? 0),
          division: Number(value.division ?? 1),
          decimalPlaces: Number(value.decimalPlaces ?? 0),
          referenceWeight: Number(value.referenceWeight ?? 1),
        }),
      );
      this.isOpen.set(false);
    } finally {
      this.isBusy.set(false);
    }
  }
}
