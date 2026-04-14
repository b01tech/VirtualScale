import { Component } from "@angular/core";
import { LoginForm } from "../components/login-form/login-form";

@Component({
  selector: "app-login-page",
  templateUrl: "./login-page.html",
  styleUrls: ["./login-page.scss"],
  imports: [LoginForm],
})
export class LoginPage {}
