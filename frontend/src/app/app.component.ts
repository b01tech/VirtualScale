import { Component } from "@angular/core";
import { Header } from "./layout/header/header";
import { Content } from "./layout/content/content";

@Component({
  selector: "app-root",
  imports: [Header, Content],
  templateUrl: "./app.component.html",
  styleUrl: "./app.component.scss",
})
export class AppComponent {}
