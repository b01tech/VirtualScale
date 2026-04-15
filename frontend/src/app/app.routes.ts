import { Routes } from "@angular/router";
import { ScalePage } from "./features/scale/pages/scale-page";
import { LoginPage } from "./features/login/page/login-page";
import { authGuard } from "./features/login/guards/auth.guard";
import { SettingsPage } from "./features/settings/page/settings-page";
import { CalibrationPage } from "./features/calibration/page/calibration-page";

export const routes: Routes = [
  {
    path: "",
    redirectTo: "login",
    pathMatch: "full",
  },
  {
    path: "scale",
    component: ScalePage,
    canActivate: [authGuard],
  },
  {
    path: "settings",
    component: SettingsPage,
    canActivate: [authGuard],
  },
  {
    path: "calibration",
    component: CalibrationPage,
    canActivate: [authGuard],
  },
  {
    path: "login",
    component: LoginPage,
  },
  {
    path: "**",
    redirectTo: "login",
  },
];
