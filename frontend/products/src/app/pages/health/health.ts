import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import HttpService from '../../services/http.service';
import { Observable } from 'rxjs';
import { HealthCheckModel } from '../../models/healthcheck.model';

@Component({
  standalone: true,
  templateUrl: './health.html',
  styleUrl: './health.css',
  imports: [CommonModule],
})
export default class HomeComponent implements OnInit {
  private readonly httpService: HttpService = inject(HttpService);

  public healthCheck$: Observable<HealthCheckModel> =
    this.httpService.get<HealthCheckModel>('products/healthcheck');

  ngOnInit(): void {}
}
