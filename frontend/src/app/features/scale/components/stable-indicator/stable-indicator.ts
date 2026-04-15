import { Component, input } from "@angular/core";

@Component({
  selector: "app-stable-indicator",
  templateUrl: "./stable-indicator.html",
  styleUrl: "./stable-indicator.scss",
})
export class StableIndicator {
  isStable = input<boolean>(false);
}

