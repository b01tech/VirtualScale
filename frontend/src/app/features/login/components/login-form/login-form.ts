import { Component, inject } from "@angular/core";
import { LoginService } from "../../services/login.service";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { Router } from "@angular/router";

@Component({
  selector: "app-login-form",
  imports: [ReactiveFormsModule],
  templateUrl: "./login-form.html",
  styleUrls: ["./login-form.scss"],
})
export class LoginForm {
  private _loginService = inject(LoginService);
  private _fb = inject(FormBuilder);
  private _router = inject(Router);
  username: string = "";
  password: string = "";

  loginForm = this._fb.group({
    username: ["", Validators.required],
    password: ["", Validators.required],
  });

  onSubmit() {
    if (this.loginForm.valid) {
      this.username = this.loginForm.value.username || "";
      this.password = this.loginForm.value.password || "";
      const isLoggedIn = this._loginService.login(this.username, this.password);
      if (isLoggedIn) {
        console.log("login success");
        this._router.navigate(["/home"]);
      } else {
        console.log("login failed");
      }
    }
  }
}
