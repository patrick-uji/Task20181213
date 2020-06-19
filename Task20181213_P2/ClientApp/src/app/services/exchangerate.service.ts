import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
const BASE_URL = "./api/ExchangeRates";
@Injectable()
export class ExchangeRateService {

  constructor(private http: HttpClient) {
  }

  public exchange(from: string, to: string, amount: number, date?: Date): Promise<number> {
    let url = `${BASE_URL}/exchange?from=${from}&to=${to}&amount=${amount}`;
    if (date) {
      url += `&date=${date}`;
    }
    return this.http.get<number>(url).toPromise();
  }

}
