import { Component, input } from "@angular/core";
import { DecimalPipe } from "@angular/common";
import { LoadCellResponse } from "../../models/loadcell-response";

@Component({
  selector: "app-loadcell-grid",
  imports: [DecimalPipe],
  templateUrl: "./loadcell-grid.html",
  styleUrl: "./loadcell-grid.scss",
})
export class LoadCellGrid {
  cells = input<LoadCellResponse[]>([]);
}
