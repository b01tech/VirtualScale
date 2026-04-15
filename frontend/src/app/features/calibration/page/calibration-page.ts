import { Component } from "@angular/core";
import { DisplayScale } from "../../scale/components/display-scale/display-scale";
import { CalibrationActions } from "../components/calibration-actions/calibration-actions";
import { AppButton } from "../../../shared/button/app-button";

@Component({
  selector: "app-calibration-page",
  imports: [DisplayScale, CalibrationActions, AppButton],
  templateUrl: "./calibration-page.html",
  styleUrl: "./calibration-page.scss",
})
export class CalibrationPage {}
