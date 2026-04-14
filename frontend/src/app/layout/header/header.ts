import { Component } from "@angular/core";

@Component({
  selector: "app-header",
  templateUrl: "./header.html",
  styles: `
    :host {
      height: 100%;
      width: 100%;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      box-shadow: 0 0 10px rgba(0, 0, 0, 0.6);
    }

    h1 {
      font-family: var(--font-title);
      color: var(--text-primary);
    }
  `,
})
export class Header {
  readonly title = "VirtualScale";
}
