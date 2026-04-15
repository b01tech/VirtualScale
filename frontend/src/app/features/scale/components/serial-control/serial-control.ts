import { Component, inject, OnDestroy, OnInit, signal } from "@angular/core";
import { firstValueFrom } from "rxjs";
import { SerialService } from "../../services/serial.service";
import { AppButton } from "../../../../shared/button/app-button";

@Component({
  selector: "app-serial-control",
  imports: [AppButton],
  templateUrl: "./serial-control.html",
  styleUrl: "./serial-control.scss",
})
export class SerialControl implements OnInit, OnDestroy {
  private readonly _serialService = inject(SerialService);

  protected readonly ports = this._serialService.ports;
  protected readonly status = this._serialService.status;
  protected readonly selectedPort = signal<string>("");
  protected readonly isBusy = signal(false);

  async ngOnInit() {
    await this._serialService.startLive();

    const [ports, status] = await Promise.all([
      firstValueFrom(this._serialService.loadPorts()),
      firstValueFrom(this._serialService.loadStatus()),
    ]);

    this._serialService.ports.set(ports);
    this._serialService.status.set(status);
    this.selectedPort.set(status.desiredPort ?? ports[0] ?? "");
  }

  async ngOnDestroy() {
    await this._serialService.stopLive();
  }

  protected async onRefreshPorts() {
    if (this.isBusy()) {
      return;
    }

    this.isBusy.set(true);
    try {
      const ports = await firstValueFrom(this._serialService.loadPorts());
      this._serialService.ports.set(ports);
      if (!this.selectedPort() && ports.length > 0) {
        this.selectedPort.set(ports[0]);
      }
    } finally {
      this.isBusy.set(false);
    }
  }

  protected async onConnect() {
    const port = this.selectedPort().trim();
    if (!port || this.isBusy()) {
      return;
    }

    this.isBusy.set(true);
    try {
      const status = await firstValueFrom(this._serialService.connect(port));
      this._serialService.status.set(status);
    } finally {
      this.isBusy.set(false);
    }
  }

  protected async onDisconnect() {
    if (this.isBusy()) {
      return;
    }

    this.isBusy.set(true);
    try {
      const status = await firstValueFrom(this._serialService.disconnect());
      this._serialService.status.set(status);
    } finally {
      this.isBusy.set(false);
    }
  }

  protected stateLabel(state: number): string {
    switch (state) {
      case 1:
        return "Conectando";
      case 2:
        return "Conectado";
      case 3:
        return "Erro";
      default:
        return "Desconectado";
    }
  }

  protected onPortChange(event: Event) {
    const target = event.target as HTMLSelectElement | null;
    this.selectedPort.set(target?.value ?? "");
  }
}
