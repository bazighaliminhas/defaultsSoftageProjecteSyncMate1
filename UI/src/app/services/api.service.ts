import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { map } from 'rxjs/operators';
import { Order, User, UserType } from '../models/models';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  apiUrl = environment.apiUrl;
  constructor(private http: HttpClient, private jwt: JwtHelperService) { }

  createAccount(user: User) {
    return this.http.post(this.apiUrl + 'CreateAccount', user, {
      responseType: 'text',
    });
  }

  login(loginInfo: any) {
    let params = new HttpParams()
      .append('email', loginInfo.email)
      .append('password', loginInfo.password);
    return this.http.get(this.apiUrl + 'Login', {
      params: params,
    });
  }

  saveToken(token: string) {
    localStorage.setItem('access_token', token);
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('access_token');
  }

  deleteToken() {
    localStorage.removeItem('access_token');
    window.location.reload();
  }

  getTokenUserInfo(): User | null {
    if (!this.isLoggedIn()) return null;
    let token = this.jwt.decodeToken();
    let user: User = {
      id: token.id,
      firstName: token.firstName,
      lastName: token.lastName,
      email: token.email,
      mobile: token.mobile,
      password: '',
      status: token.status,
      createdDate: token.createdDate,
      userType: token.userType === 'USER' ? UserType.USER : UserType.ADMIN,
    };
    return user;
  }

  getAllUsers() {
    return this.http.get<User[]>(this.apiUrl + 'GetAllUsers').pipe(
      map((users) =>
        users.map((user) => {
          let temp: User = user;
          temp.userType = user.userType == 0 ? UserType.USER : UserType.ADMIN;
          return temp;
        })
      )
    );
  }

  blockUser(id: number) {
    return this.http.get(this.apiUrl + 'ChangeBlockStatus/1/' + id, {
      responseType: 'text',
    });
  }

  unblockUser(id: number) {
    return this.http.get(this.apiUrl + 'ChangeBlockStatus/0/' + id, {
      responseType: 'text',
    });
  }

  enableUser(id: number) {
    return this.http.get(this.apiUrl + 'ChangeEnableStatus/1/' + id, {
      responseType: 'text',
    });
  }

  disableUser(id: number) {
    return this.http.get(this.apiUrl + 'ChangeEnableStatus/0/' + id, {
      responseType: 'text',
    });
  }

  getOrders(orderId: number, fromDate: string, toDate: string, orderNumber: string, status: string) {
    return this.http.get<Order[]>(this.apiUrl + 'EDIProcessor/api/v1/orders/getOrders/' + orderId + '/' + fromDate + '/' + toDate + '/' + orderNumber + '/' + status);
  }

  uploadFile(file: File) {
    const formData = new FormData();
    formData.append('file', file, file.name);

    return this.http.post(this.apiUrl + 'EDIProcessor/api/v1/orders/process850', formData);
  }

  getOrderFiles(orderId: Number) {
    return this.http.get<Order[]>(this.apiUrl + 'EDIProcessor/api/v1/orders/getOrderFiles/' + orderId);
  }

  getStoresOrder(orderId: number) {
    return this.http.get<Order[]>(
      `${this.apiUrl}EDIProcessor/api/v1/orders/getOrderStores?orderId=${orderId}`,
      //`${this.apiurl}api/Users/getTopUsers?userNo=${userNo}`,
      {}
    );
  }

  async syncOrderStore(orderId: number) {
    return this.http.post(
      `${this.apiUrl}EDIProcessor/api/v1/orders/syncOrderStore?orderStoreId=${orderId}`,
      {}
    );
  }

  generateASN(orderId: number) {
    return this.http.post(
      `${this.apiUrl}EDIProcessor/api/v1/orders/process856?orderId=${orderId}`,
      {}
    );
  }

  markForASN(orderId: number) {
    return this.http.post(
      `${this.apiUrl}EDIProcessor/api/v1/orders/markFor856?orderId=${orderId}`,
      {}
    );
  }

  createInvoice(orderId: number) {
    return this.http.post(
      `${this.apiUrl}createInvoice?orderId=${orderId}`,
      {}
    );
  }

  process810(orderId: number) {
    return this.http.post(
      `${this.apiUrl}EDIProcessor/api/v1/orders/process810?orderId=${orderId}`,
      {}
    );
  }

  generate855EDI(orderId: number) {
    return this.http.post(
      `${this.apiUrl}EDIProcessor/api/v1/orders/process855?orderId=${orderId}`,
      {}
    );
  }

  syncOrder(orderId: number) {
    return this.http.post(
      `${this.apiUrl}EDIProcessor/api/v1/orders/syncOrder?orderId=${orderId}`,
      {}
    );
  }

  generateASNForStoreOrders(file: File, orderId: number): Observable<any> {
    const formData = new FormData();
    formData.append('file', file, file.name);

    const url = `${this.apiUrl}EDIProcessor/api/v1/orders/process856Store?orderId=${orderId}`;
    return this.http.post(url, formData);
  }

  generate810ForStoreOrders(file: File, orderId: number): Observable<any> {
    const formData = new FormData();
    formData.append('file', file, file.name);

    const url = `${this.apiUrl}EDIProcessor/api/v1/orders/process810Store?orderId=${orderId}`;
    return this.http.post(url, formData);
  }

  markForASNForStoreOrders(orderStoreId: number) {
    return this.http.post(
      `${this.apiUrl}EDIProcessor/api/v1/orders/markSendOrderStore?orderStoreId=${orderStoreId}`,
      {}
    );
  }

  createInvoiceForStoreOrders(orderStoreId: number) {
    return this.http.post(
      `${this.apiUrl}EDIProcessor/api/v1/orders/createInvoiceOrderStore?orderStoreId=${orderStoreId}`,
      {}
    );
  }

  getMaps(searchOption: string, searchValue: string): Observable<any> {
    const params = new HttpParams()
      .set('searchOption', searchOption)
      .set('searchValue', searchValue);

    return this.http.get(`${this.apiUrl}api/Maps/getMaps`, { params });
  }

  getRouteTypes(searchOption: string, searchValue: string): Observable<any> {
    const params = new HttpParams()
      .set('searchOption', searchOption)
      .set('searchValue', searchValue);

    return this.http.get(`${this.apiUrl}api/RouteTypes/getRouteTypes`, { params });
  }



  getPartnerGroups(searchOption: string, searchValue: string): Observable<any> {
    const params = new HttpParams()
      .set('searchOption', searchOption)
      .set('searchValue', searchValue);

    return this.http.get(`${this.apiUrl}api/PartnerGroups/getPartnerGroups`, { params });
  }

  
}
