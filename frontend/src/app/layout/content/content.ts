import { Component } from "@angular/core";
import { RouterOutlet } from "@angular/router";

@Component({
  selector: "app-content",
  imports: [RouterOutlet],
  templateUrl: "./content.html",
  styles: `
    :host {
      width: 100%;
      flex: 1;
      display: flex;
      flex-direction: column;
      justify-content: center;
    }

    main {
      flex: 1;
      width: 100%;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: start;
    }
  `,
})
export class Content {}
