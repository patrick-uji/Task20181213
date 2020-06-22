import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { HttpClientModule } from '@angular/common/http';
import { BrowserModule } from '@angular/platform-browser';
import { MDBBootstrapModule } from 'angular-bootstrap-md';
import { HomeComponent } from './components/home/home.component';
import { CurrenciesService } from './services/currencies.service';
import { ToastComponent } from './components/toast/toast.component';
import { ExchangeRateService } from './services/exchangerate.service';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ExchangerComponent } from './components/exchanger/exchanger.component';
import { MarketHistoryComponent } from './components/market-history/market-history.component';
@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    ToastComponent,
    NavMenuComponent,
    ExchangerComponent,
    MarketHistoryComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'exchanger', component: ExchangerComponent },
      { path: 'market-history', component: MarketHistoryComponent },
    ]),
    MDBBootstrapModule.forRoot()
  ],
  providers: [
    ExchangeRateService,
    CurrenciesService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
