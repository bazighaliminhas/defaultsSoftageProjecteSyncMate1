import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { CustomerProductCatalog } from '../models/models';

@Injectable({
  providedIn: 'root',
})
export class CustomerProductCatalogService {
  apiUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }

  updateCustomerProductCatalog(connectorModel: any): Observable<any> {
    return this.http.post<any>(this.apiUrl + 'api/CustomerProductCatalog/updateCustomerProductCatalog', connectorModel);
  }

  getCustomerProductCatalog(searchOption: string, searchValue: string): Observable<any> {
    const params = new HttpParams()
      .set('searchOption', searchOption)
      .set('searchValue', searchValue);

    return this.http.get(`${this.apiUrl}api/CustomerProductCatalog/getCustomerProductCatalog`, { params });
  }

  uploadCustomerProductCatalogFile(file: File, ERPCustomerID: string): Observable<any> {
    const formData = new FormData();
    formData.append('file', file, file.name);

    const url = `${this.apiUrl}api/CustomerProductCatalog/processCustomerProductCatalogFile?ERPCustomerID=${ERPCustomerID}`;
    return this.http.post(url, formData);
  }

  getHistoryCustomerProductCatalog(ERPCustomerID: string): Observable<any> {
    const url = `${this.apiUrl}api/CustomerProductCatalog/getHistoryCustomerProductCatalog?ERPCustomerID=${ERPCustomerID}`;
    return this.http.get(url);
  }
}
