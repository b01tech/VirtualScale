import { Component, input } from "@angular/core";

@Component({
  selector: "app-tare-indicator",
  templateUrl: "./tare-indicator.html",
  styleUrl: "./tare-indicator.scss",
})
export class TareIndicator {
  isTared = input<boolean>(false);
}
