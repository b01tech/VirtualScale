import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ScaleService } from './scale.service';

describe('ScaleService', () => {
  let service: ScaleService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ScaleService],
    });
    service = TestBed.inject(ScaleService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  describe('signals initialization', () => {
    it('should have initial latest signal with default values', () => {
      const latest = service.latest();
      
      expect(latest.bruteWeight).toBe(0);
      expect(latest.netWeight).toBe(0);
      expect(latest.isTared).toBe(false);
      expect(latest.isOnZero).toBe(true);
      expect(latest.unit).toBe('kg');
    });

    it('should have empty loadCells signal initially', () => {
      expect(service.loadCells()).toEqual([]);
    });
  });

  describe('HTTP endpoint URLs', () => {
    it('should have correct API URLs', () => {
      expect((service as any)._apiUrl).toContain('/api/scale');
      expect((service as any)._hubUrl).toContain('/hubs/scale');
      expect((service as any)._loadCellHubUrl).toContain('/hubs/loadcells');
      expect((service as any)._loadCellApiUrl).toContain('/api/loadcells');
    });
  });

  describe('loadCells signal operations', () => {
    it('should update loadCells signal via upsertLoadCell', () => {
      (service as any).upsertLoadCell({ id: 1, rawValue: 100, factor: 1.0, status: 0 });
      
      expect(service.loadCells()).toHaveLength(1);
      expect(service.loadCells()[0].id).toBe(1);
      expect(service.loadCells()[0].rawValue).toBe(100);
    });

    it('should update existing loadCell in upsertLoadCell', () => {
      (service as any).upsertLoadCell({ id: 1, rawValue: 100, factor: 1.0, status: 0 });
      (service as any).upsertLoadCell({ id: 1, rawValue: 200, factor: 1.0, status: 0 });
      
      expect(service.loadCells()).toHaveLength(1);
      expect(service.loadCells()[0].rawValue).toBe(200);
    });

    it('should sort loadCells by id in upsertLoadCell', () => {
      (service as any).upsertLoadCell({ id: 2, rawValue: 200, factor: 1.0, status: 0 });
      (service as any).upsertLoadCell({ id: 1, rawValue: 100, factor: 1.0, status: 0 });
      (service as any).upsertLoadCell({ id: 3, rawValue: 300, factor: 1.0, status: 0 });
      
      const ids = service.loadCells().map((c) => c.id);
      expect(ids).toEqual([1, 2, 3]);
    });
  });

  describe('HTTP methods', () => {
    it('should call getSnapshot', () => {
      service.getSnapshot().subscribe();
      
      const req = httpMock.expectOne((r) => r.url.includes('/api/scale/'));
      expect(req.request.method).toBe('GET');
    });

    it('should call tareScale', () => {
      service.tareScale().subscribe();
      
      const req = httpMock.expectOne((r) => r.url.includes('/tare'));
      expect(req.request.method).toBe('POST');
    });

    it('should call saveCalibration', () => {
      service.saveCalibration().subscribe();
      
      const req = httpMock.expectOne((r) => r.url.includes('/calibration/save'));
      expect(req.request.method).toBe('POST');
    });
  });
});
