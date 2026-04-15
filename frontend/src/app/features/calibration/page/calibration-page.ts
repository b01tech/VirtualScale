import { Component } from "@angular/core";
import { DisplayScale } from "../../scale/components/display-scale/display-scale";
import { CalibrationActions } from "../components/calibration-actions/calibration-actions";
import { AppButton } from "../../../shared/button/app-button";
import { CalibrationSettings } from "../components/calibration-settings/calibration-settings";

@Component({
  selector: "app-calibration-page",
  imports: [DisplayScale, CalibrationActions, AppButton, CalibrationSettings],
  templateUrl: "./calibration-page.html",
  styleUrl: "./calibration-page.scss",
})
export class CalibrationPage {}
