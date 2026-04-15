import { Injectable, signal } from "@angular/core";

@Injectable({
  providedIn: "root",
})
export class LoginService {
  private readonly _loggedIn = signal(false);

  constructor() {
    const persisted = sessionStorage.getItem("vs_logged_in");
    this._loggedIn.set(persisted === "true");
  }

  login(username: string, password: string): boolean {
    if (username !== "admin" || password !== "admin") {
      this._loggedIn.set(false);
      sessionStorage.removeItem("vs_logged_in");
      return false;
    }
    this._loggedIn.set(true);
    sessionStorage.setItem("vs_logged_in", "true");
    return true;
  }

  logout() {
    this._loggedIn.set(false);
    sessionStorage.removeItem("vs_logged_in");
  }

  isLoggedIn(): boolean {
    return this._loggedIn();
  }
}
