import { Component, inject } from "@angular/core";
import { ScaleService } from "../../services/scale.service";

@Component({
  selector: "app-tare-button",
  templateUrl: "./tare-button.html",
  styleUrl: "./tare-button.scss",
})
export class TareButton {
  private readonly _scaleService = inject(ScaleService);
  onClick() {
    this._scaleService.tareScale().subscribe();
  }
}
