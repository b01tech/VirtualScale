export interface ScaleResponse {
  bruteWeight: number;
  netWeight: number;
  tareWeight: number;
  isTared: boolean;
  isOnZero: boolean;
  isStable: boolean;
  filterLevel: number;
  numberOfCells: number;
  capMax: number;
  division: number;
  decimalPlaces: number;
  referenceWeight: number;
  resolution: number;
  unit: string;
  needsCalibrationAdjustment: boolean;
}
