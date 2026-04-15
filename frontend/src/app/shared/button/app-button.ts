import { Component, input, output } from "@angular/core";

export type ButtonVariant = "primary" | "secondary";

@Component({
  selector: "app-button",
  templateUrl: "./app-button.html",
  styleUrl: "./app-button.scss",
})
export class AppButton {
  label = input.required<string>();
  variant = input<ButtonVariant>("primary");
  disabled = input(false);

  clicked = output<void>();
}
