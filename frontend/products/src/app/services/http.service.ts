import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvService } from './env.service';

@Injectable({
  providedIn: 'root',
})
export default class HttpService {
  private readonly http = inject(HttpClient);
  private readonly envService = inject(EnvService);
  private readonly baseApiUrl: string = this.envService.baseUrl;

  public get<T>(
    url: string,
    options?: { parameters?: Map<string, string>; token?: string },
  ): Observable<T> {
    const uri =
      options == undefined || options.parameters == undefined
        ? this.completeUri(url)
        : this.completeUriWithQuerystring(url, options.parameters);

        console.log(this.http)
    return this.http.get<T>(uri);
  }

  public post<T>(url: string, body: any, options?: { token?: string }): Observable<T> {
    const uri = this.completeUri(url);
    const result = this.http.post<T>(uri, body, {
      headers: this.headers(options?.token),
    });

    return result;
  }

  private completeUri(url: string): string {
    let _url = `${this.baseApiUrl}${url}`;
    return _url;
  }

  private completeUriWithQuerystring(url: string, parameters: Map<string, string>): string {
    let param: string = '';
    let counter = 1;
    for (let [key, value] of parameters) {
      param += counter == parameters.size ? `${key}=${value}` : `${key}=${value}&`;
      counter++;
    }

    let _url = `${this.baseApiUrl}${url}/?${param}`;
    return _url;
  }

  private headers(token?: string): HttpHeaders {
    return token == undefined
      ? new HttpHeaders({ 'conent-type': 'application/json' })
      : new HttpHeaders({
          'conent-type': 'application/json',
          authorization: `Bearer ${token}`,
        });
  }
}
