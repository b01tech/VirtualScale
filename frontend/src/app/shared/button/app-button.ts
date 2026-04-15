import { Component, input, output } from "@angular/core";
import { RouterLink } from "@angular/router";

export type ButtonVariant = "primary" | "secondary" | "warning" | "danger";

@Component({
  selector: "app-button",
  imports: [RouterLink],
  templateUrl: "./app-button.html",
  styleUrl: "./app-button.scss",
})
export class AppButton {
  label = input.required<string>();
  variant = input<ButtonVariant>("primary");
  disabled = input(false);
  route = input<string | any[] | null>(null);

  clicked = output<void>();
}
