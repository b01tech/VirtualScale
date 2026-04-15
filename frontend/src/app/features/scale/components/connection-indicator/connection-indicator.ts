import { Component, input } from "@angular/core";

@Component({
  selector: "app-connection-indicator",
  templateUrl: "./connection-indicator.html",
  styleUrl: "./connection-indicator.scss",
})
export class ConnectionIndicator {
  isConnected = input<boolean>(false);
}

