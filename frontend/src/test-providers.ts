import { ApplicationConfig, Provider } from '@angular/core';
import { provideHttpClient } from '@angular/common/http';
import { HttpClient } from '@angular/common/http';

const mockHttpClient = {
  get: () => new Promise(() => {}),
  post: () => new Promise(() => {}),
};

export default [
  { provide: HttpClient, useValue: mockHttpClient },
] satisfies Provider[];
