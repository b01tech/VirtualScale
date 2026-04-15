import { Component, computed, inject, signal } from "@angular/core";
import { Router } from "@angular/router";
import { firstValueFrom } from "rxjs";
import { AppButton } from "../../../../shared/button/app-button";
import { LoginService } from "../../../login/services/login.service";
import { ScaleService } from "../../services/scale.service";

@Component({
  selector: "app-scale-actions",
  imports: [AppButton],
  templateUrl: "./scale-actions.html",
  styleUrl: "./scale-actions.scss",
})
export class ScaleActions {
  private readonly _loginService = inject(LoginService);
  private readonly _scaleService = inject(ScaleService);
  private readonly _router = inject(Router);
  protected readonly isBusy = signal(false);
  protected readonly scale = this._scaleService.latest;
  protected readonly canTare = computed(
    () => this.scale().isTared || this.scale().bruteWeight > 0,
  );
  protected readonly unitFactor = computed(() =>
    this.scale().unit === "g" ? 1000 : 1,
  );
  protected readonly canZero = computed(() => {
    const net = (this.scale().netWeight ?? 0) * this.unitFactor();
    const tolerance = 10 * (this.scale().resolution ?? 0);
    return Math.abs(net) <= tolerance;
  });

  protected async tare() {
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

  protected async zero() {
    if (this.isBusy()) {
      return;
    }
    this.isBusy.set(true);
    try {
      await firstValueFrom(this._scaleService.zeroScale());
    } finally {
      this.isBusy.set(false);
    }
  }

  protected logout() {
    this._loginService.logout();
    this._router.navigate(["/login"]);
  }
}
