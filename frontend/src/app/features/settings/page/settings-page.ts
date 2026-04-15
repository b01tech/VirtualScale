import { Component } from "@angular/core";
import { SerialControl } from "../../scale/components/serial-control/serial-control";
import { FilterControl } from "../../scale/components/filter-control/filter-control";
import { AppButton } from "../../../shared/button/app-button";

@Component({
  selector: "app-settings-page",
  imports: [SerialControl, FilterControl, AppButton],
  templateUrl: "./settings-page.html",
  styleUrl: "./settings-page.scss",
})
export class SettingsPage {}
