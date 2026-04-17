import { CommonModule } from '@angular/common';
import { Component, inject, OnDestroy, OnInit, signal, WritableSignal } from '@angular/core';
import { ProductModel } from '../../models/product.model';
import { Observable, Subscription } from 'rxjs';
import HttpService from '../../services/http.service';
import { ApiResponse } from '../../models/api-response.model.';
import { FormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  standalone: true,
  templateUrl: './products.html',
  styleUrl: './products.css',
  imports: [CommonModule, FormsModule],
})
export default class HomeComponent implements OnInit, OnDestroy {
  private readonly httpService: HttpService = inject(HttpService);

  public products$: Observable<ApiResponse<ProductModel[]>> =
    this.httpService.get<ApiResponse<ProductModel[]>>('products');

  private postSubsription: Subscription = new Subscription();

  public isOpen = false;

  public productModel: ProductModel = {
    id: '',
    name: '',
    color: '',
    description: '',
    sku: '',
    price: 0,
  };

  public error: WritableSignal<string> = signal('');
  public searchInput: string = '';

  ngOnInit(): void {}

  public search(): void {
    const query = new Map<string, string>();
    query.set('color', this.searchInput);
    this.products$ = this.httpService.get<ApiResponse<ProductModel[]>>('products', {
      parameters: query,
    });
  }

  public openModal(): void {
    this.isOpen = true;
  }

  public closeModal(): void {
    this.isOpen = false;
  }

  public confirm(): void {
    this.postSubsription = this.httpService
      .post<ApiResponse<string>>('products', this.productModel)
      .subscribe({
        next: (response) => {
          this.products$ = this.httpService.get<ApiResponse<ProductModel[]>>('products');
          this.closeModal();
        },
        error: (err: HttpErrorResponse) => {
          const error = err.error as ApiResponse<string>;
          console.log(error);
          this.error.set(error.message);
        },
      });
  }

  ngOnDestroy(): void {
    this.postSubsription.unsubscribe();
  }
}
