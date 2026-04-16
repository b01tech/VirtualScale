import { TestBed } from "@angular/core/testing";
import { LoginService } from "./login.service";

describe("LoginService", () => {
  let service: LoginService;

  beforeEach(() => {
    sessionStorage.clear();
    TestBed.configureTestingModule({
      providers: [LoginService],
    });
    service = TestBed.inject(LoginService);
  });

  describe("initial state", () => {
    it("should have loggedIn signal as false initially", () => {
      expect(service.isLoggedIn()).toBe(false);
    });
  });

  describe("login", () => {
    it("should return true for correct credentials (admin/admin)", () => {
      const result = service.login("admin", "admin");
      expect(result).toBe(true);
    });

    it("should set loggedIn to true for correct credentials", () => {
      service.login("admin", "admin");
      expect(service.isLoggedIn()).toBe(true);
    });

    it("should persist login state to sessionStorage", () => {
      service.login("admin", "admin");
      expect(sessionStorage.getItem("vs_logged_in")).toBe("true");
    });

    it("should return false for incorrect username", () => {
      const result = service.login("user", "admin");
      expect(result).toBe(false);
    });

    it("should return false for incorrect password", () => {
      const result = service.login("admin", "wrong");
      expect(result).toBe(false);
    });

    it("should return false for empty credentials", () => {
      const result = service.login("", "");
      expect(result).toBe(false);
    });

    it("should not set loggedIn for incorrect credentials", () => {
      service.login("wrong", "wrong");
      expect(service.isLoggedIn()).toBe(false);
    });

    it("should remove vs_logged_in from sessionStorage on failed login", () => {
      sessionStorage.setItem("vs_logged_in", "true");
      service.login("wrong", "wrong");
      expect(sessionStorage.getItem("vs_logged_in")).toBeNull();
    });
  });

  describe("logout", () => {
    it("should set loggedIn to false", () => {
      service.login("admin", "admin");
      service.logout();
      expect(service.isLoggedIn()).toBe(false);
    });

    it("should remove vs_logged_in from sessionStorage", () => {
      service.login("admin", "admin");
      service.logout();
      expect(sessionStorage.getItem("vs_logged_in")).toBeNull();
    });
  });

  describe("session persistence", () => {
    it("should restore loggedIn state from sessionStorage", () => {
      sessionStorage.setItem("vs_logged_in", "true");
      const newService = new LoginService();
      expect(newService.isLoggedIn()).toBe(true);
    });

    it("should have loggedIn false when sessionStorage has no value", () => {
      sessionStorage.clear();
      const newService = new LoginService();
      expect(newService.isLoggedIn()).toBe(false);
    });
  });
});
