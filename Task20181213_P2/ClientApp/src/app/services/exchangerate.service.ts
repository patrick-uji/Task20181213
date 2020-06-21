import { Utils } from '../utils';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ExchangeRateDateGroupDTO } from '../dto/exchangerate.dategroup.dto';
const BASE_URL = "./api/ExchangeRates";
@Injectable()
export class ExchangeRateService {

  constructor(private http: HttpClient) {
  }

  public exchange(from: string, to: string, amount: number, date?: Date): Promise<number> {
    let url = `${BASE_URL}/exchange?from=${from}&to=${to}&amount=${amount}`;
    if (date) {
      url += `&date=${Utils.getISODateOnly(date)}`;
    }
    return this.http.get<number>(url).toPromise();
  }

  public getAllExchangeRatesFor(code: string, from?: Date, to?: Date): Promise<ExchangeRateDateGroupDTO[]> {
    let url = `${BASE_URL}/${code}`;
    if (from) {
      url += `?from=${Utils.getISODateOnly(from)}`;
      if (to) {
        url += `&to=${Utils.getISODateOnly(to)}`;
      }
    }
    return this.http.get<ExchangeRateDateGroupDTO[]>(url).toPromise();
  }

}
