import { Component, inject, signal } from "@angular/core";
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

  protected logout() {
    this._loginService.logout();
    this._router.navigate(["/login"]);
  }
}
