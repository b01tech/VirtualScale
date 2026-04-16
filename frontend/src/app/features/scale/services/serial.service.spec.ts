import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { SerialService } from './serial.service';
import { SerialStatusResponse } from '../models/serial-status';

describe('SerialService', () => {
  let service: SerialService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [SerialService],
    });
    service = TestBed.inject(SerialService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  describe('signals initialization', () => {
    it('should have initial ports signal empty', () => {
      expect(service.ports()).toEqual([]);
    });

    it('should have initial status signal with default values', () => {
      const status = service.status();
      
      expect(status.desiredConnected).toBe(false);
      expect(status.isConnected).toBe(false);
      expect(status.connectedPort).toBeNull();
      expect(status.lastError).toBeNull();
    });
  });

  describe('HTTP endpoint URLs', () => {
    it('should have correct API and hub URLs', () => {
      expect((service as any)._apiUrl).toContain('/api/serial');
      expect((service as any)._hubUrl).toContain('/hubs/serial');
    });
  });

  describe('status signal updates', () => {
    it('should be able to update status via signal', () => {
      const newStatus: SerialStatusResponse = {
        desiredConnected: true,
        desiredPort: 'COM3',
        isConnected: true,
        connectedPort: 'COM3',
        state: 1,
        lastError: null,
      };
      
      service.status.set(newStatus);
      
      expect(service.status().isConnected).toBe(true);
      expect(service.status().connectedPort).toBe('COM3');
    });
  });

  describe('ports signal updates', () => {
    it('should be able to update ports via signal', () => {
      const newPorts = ['COM1', 'COM2', 'COM3'];
      service.ports.set(newPorts);
      
      expect(service.ports()).toEqual(newPorts);
      expect(service.ports()).toHaveLength(3);
    });
  });
});
