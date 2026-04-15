import { Component, computed, input } from "@angular/core";
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
  maxCells = input<number>(1);

  protected readonly visibleCells = computed(() => {
    const max = Math.max(0, this.maxCells());
    return [...this.cells()].sort((a, b) => a.id - b.id).slice(0, max);
  });
}
