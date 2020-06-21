import { Component, ViewChild } from '@angular/core';
import { Currency } from '../../model/currency.model';
import { ToastComponent } from '../toast/toast.component';
import { CurrenciesService } from '../../services/currencies.service';
import { ExchangeRateService } from '../../services/exchangerate.service';
@Component({
  selector: 'exchanger-component',
  templateUrl: './exchanger.component.html',
  styleUrls: ['./exchanger.component.css']
})
export class ExchangerComponent {

  public date: Date;
  public result: number;
  public amount: number;
  public useDate: boolean;
  public currencies: Currency[];
  public sourceCurrency: string = "EUR";
  public targetCurrency: string = "USD";

  @ViewChild(ToastComponent, {static: false})
  private toast: ToastComponent;

  constructor(private exchangeRateService: ExchangeRateService,
              currenciesService: CurrenciesService) {
    currenciesService.getAll().then(response => this.currencies = response);
  }

  public calculateExchange() {
    let promise = this.exchangeRateService.exchange(this.sourceCurrency, this.targetCurrency, this.amount, this.getSelectedDate());
    promise.then(response => this.result = response)
           .catch(httpResponse => this.toast.showHttpError(httpResponse));
  }

  private getSelectedDate(): Date {
    return this.useDate ? this.date : null;
  }

}
