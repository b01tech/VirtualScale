import { Routes } from "@angular/router";
import { ScalePage } from "./features/scale/pages/scale-page";
import { LoginPage } from "./features/login/page/login-page";

export const routes: Routes = [
  {
    path: "",
    redirectTo: "home",
    pathMatch: "full",
  },
  {
    path: "home",
    component: ScalePage,
  },
  {
    path: "login",
    component: LoginPage,
  },
];
