import { Component, signal } from "@angular/core";

@Component({
  selector: "app-display-scale",
  templateUrl: "./display-scale.html",
  styleUrl: "./display-scale.scss",
})
export class DisplayScale {
  protected netWeight = signal<number>(0);
}
