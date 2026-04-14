import { Component } from "@angular/core";
import { RouterOutlet } from "@angular/router";

@Component({
  selector: "app-content",
  imports: [RouterOutlet],
  templateUrl: "./content.html",
  styles: `
    :host {
      display: flex;
      height: 100%;
      width: 100%;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      background-color: red;
    }
  `,
})
export class Content {}
