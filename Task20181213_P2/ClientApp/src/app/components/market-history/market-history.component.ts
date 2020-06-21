import { Utils } from '../../utils';
import { Component, ViewChild } from '@angular/core';
import { Currency } from '../../model/currency.model';
import { ToastComponent } from '../toast/toast.component';
import { CurrenciesService } from '../../services/currencies.service';
import { ExchangeRateService } from '../../services/exchangerate.service';
import { ExchangeRateDateGroupDTO } from '../../dto/exchangerate.dategroup.dto';
interface ChartDataSet {
  data: number[],
  label: string
}
interface ChartColor {
  backgroundColor: string,
  borderColor: string,
  borderWidth: number
}

@Component({
  selector: 'market-history-component',
  templateUrl: './market-history.component.html',
  styleUrls: ['./market-history.component.css']
})
export class MarketHistoryComponent {

  public toDate: Date;
  public currencies: Currency[];
  public sourceCurrency: string = "EUR";
  public targetCurrency: string = "USD";
  public exchangeRates: ExchangeRateDateGroupDTO[];
  public fromDate: Date = Utils.addDaysToDate(new Date(), -10);

  public chartLabels: string[] = [];
  public chartDatasets: ChartDataSet[] = [{
    data: [],
    label: "Exchange rate"
  }];
  public chartColors: ChartColor[] = [{
    backgroundColor: "rgba(105, 0, 132, .2)",
    borderColor: "rgba(200, 99, 132, .7)",
    borderWidth: 2,
  }];

  @ViewChild(ToastComponent, { static: false })
  private toast: ToastComponent;

  constructor(private exchangeRatesService: ExchangeRateService,
              currenciesService: CurrenciesService) {
    currenciesService.getAll().then(response => this.currencies = response);
    this.requestExchangeRates();
  }

  public fromDateChanged(date: string) {
    this.fromDate = new Date(date);
    this.requestExchangeRates();
  }

  public toDateChanged(date: string) {
    this.toDate = new Date(date);
    this.requestExchangeRates();
  }

  public requestExchangeRates() {
    let promise = this.exchangeRatesService.getAllExchangeRatesFor(this.sourceCurrency, this.fromDate, this.toDate);
    promise.then(response => this.exchangeRatesReceived(response));
  }

  private exchangeRatesReceived(exchangeRates: ExchangeRateDateGroupDTO[]) {
    this.chartLabels = [];
    this.exchangeRates = exchangeRates;
    for (let currDateGroup of exchangeRates) {
      this.chartLabels.push(currDateGroup.date);
    }
    this.updateChart();
  }

  private updateChart() {
    let data = [];
    for (let currDateGroup of this.exchangeRates) {
      data.push(currDateGroup.rates[this.targetCurrency]);
    }
    this.chartDatasets[0].data = data;
  }

}
