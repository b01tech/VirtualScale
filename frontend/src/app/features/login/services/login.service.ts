import { Injectable } from "@angular/core";

@Injectable({
  providedIn: "root",
})
export class LoginService {
  private loggedIn: boolean = false;

  login(username: string, password: string): boolean {
    if (username !== "admin" || password !== "admin") {
      this.loggedIn = false;
      return false;
    }
    this.loggedIn = true;
    return true;
  }

  logout() {
    this.loggedIn = false;
  }

  isLoggedIn(): boolean {
    return this.loggedIn;
  }
}
