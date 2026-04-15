import { Component } from "@angular/core";
import { DisplayScale } from "../components/display-scale/display-scale";
import { ScaleActions } from "../components/scale-actions/scale-actions";

@Component({
  selector: "app-scale-page",
  imports: [DisplayScale, ScaleActions],
  templateUrl: "./scale-page.html",
  styleUrl: "./scale-page.scss",
})
export class ScalePage {}
