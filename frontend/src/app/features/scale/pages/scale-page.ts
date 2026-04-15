import { Component } from "@angular/core";
import { DisplayScale } from "../components/display-scale/display-scale";
import { TareButton } from "../components/tare-buttton/tare-button";
import { SerialControl } from "../components/serial-control/serial-control";

@Component({
  selector: "app-scale-page",
  imports: [DisplayScale, TareButton, SerialControl],
  templateUrl: "./scale-page.html",
  styleUrl: "./scale-page.scss",
})
export class ScalePage {}
