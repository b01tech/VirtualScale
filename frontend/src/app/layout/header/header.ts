import { Component } from "@angular/core";

@Component({
  selector: "app-header",
  templateUrl: "./header.html",
  styles: `
    :host {
      width: 100%;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      box-shadow: 0 0 10px rgba(0, 0, 0, 0.6);
      padding: 16px;
    }

    h1 {
      font-family: var(--font-title);
      color: var(--text-primary);
      margin: 0;
    }
  `,
})
export class Header {
  readonly title = "VirtualScale";
}
