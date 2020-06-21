import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Currency } from '../model/currency.model';
const BASE_URL = "./api/Currencies";
@Injectable()
export class CurrenciesService {

  constructor(private http: HttpClient) {
  }

  public getAll(): Promise<Currency[]> {
    return this.http.get<Currency[]>(`${BASE_URL}`).toPromise();
  }

}
