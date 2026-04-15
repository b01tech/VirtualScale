import { Component, inject, signal } from "@angular/core";
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
  protected readonly error = signal<string | null>(null);

  loginForm = this._fb.group({
    username: ["", Validators.required],
    password: ["", Validators.required],
  });

  onSubmit() {
    this.error.set(null);
    if (!this.loginForm.valid) {
      return;
    }

    const username = this.loginForm.value.username || "";
    const password = this.loginForm.value.password || "";

    const isLoggedIn = this._loginService.login(username, password);
    if (!isLoggedIn) {
      this.error.set("Usuário ou senha inválidos");
      return;
    }

    this._router.navigate(["/scale"]);
  }
}
